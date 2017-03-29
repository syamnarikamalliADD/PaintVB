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
' Form/Module: clsPrintHtml
'
' Description: Print form - uses the browser object to manage print functions.
'                           currently only for a RichTextBox or textbox control
'               Use:
'                   first call CreateSimpleDoc(...) - It builds an html document
'                   Once a document is built, all these are available:
'                   PrintDoc (bNow) - true = print immediately, false brings browser up print dialog
'                   showPrintPreview() - print preview window.
'                   showPageSetup() - page setup window for header,footer,margin.
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 09/25/2009    MSW     first draft
' 11/22/2010    MSW     Use print routines to support export to *.csv
' 09/14/11      MSW     Assemble a standard version of everything                       4.1.0.0
' 10/31/11      MSW     remove the size and visible settings.  It seems to work 
'                       without them and they were causing the main form to resize.     4.1.0.2
' 11/07/11      MSW     change default file name to use the title if available 
'                       for pdf print default names                                     4.1.0.2
' 11/23/11      RJO     In fwStartDoc, make sure WebBrowser mWebPrint is not visible.   4.1.0.1
' 11/07/11      MSW     support import from csv                                         4.01.01.00
' 12/14/11      MSW     For CSV output, replace "," with ";"                            4.01.01.01
' 01/03/12      MSW     Import/Export updates                                           4.01.01.01
' 01/13/12      MSW     Add subCreateFromHtml to support direct HTML input              4.01.01.02
' 02/15/12      MSW     Expand import and export file types, handle multiple sheets     4.01.01.03
' 04/11/12      MSW     Change some function calls to pass by reference                 4.01.03.00
' 04/27/12      MSW     Pull back some of the extra features.  Default to only offering 
'                       CSV export, screens that use extra features can ask for it      4.01.03.01
' 05/08/12      MSW     Work on ODS output to get it to work with Excel 2010            4.01.03.02
'                       Work on XLSX output  - excel export w/o linking to excel
' 06/07/12      MSW     More work on the export and graphs for reports and charts       4.01.04.00
'                       GetDTFromCSV - Add bNotEmpty to detect empty lines with the commas filled in.
' 02/22/13      MSW     Deal with a formatting error in import from XLSX                4.01.04.01
' 02/27/13      MSW     GetDTFromCSV - XLSX import for DMON-Fix shared string handling  4.01.04.02
'                       subExportCell - Get rid of an extra > in the ODS output
'                       GetDTFromCSV - add some delays before Directory.Delete, ignore errors
' 04/30/13      MSW     GetDTFromCSV - add some more error messaging                    4.01.05.00
' 05/16/14      MSW     Avoid foreign numbers                                           4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath


Friend Class clsPrintHtml
    Private msTempFile As String = "PWPrint"
    Private Const msPageBreakHTML As String = "<br clear=""all"" style=""page-break-before:always"">"
    Private msDeleteFile As String = String.Empty
    Private WithEvents mWebPrint As WebBrowser = Nothing
    Private mbDECODE_RTF As Boolean = True
    Private mbPageReady As Boolean = False
    Private mbPrintBusy As Boolean = False
    Private msWriteFile As String
    'Extra file variables for ODS output
    Private msOutputFile As String
    Private msTmpFolder As String
    Private msPictureOutput As String
    Private msPictureTag As String
    Public Const msCONTENT_FILE As String = "content.xml"
    Public Const msSTYLES_FILE As String = "styles.xml"
    Public Const msMETA_FILE As String = "meta.xml"
    Public Const msMIME_TYPE_FILE As String = "mimetype"
    Public Const msSETTINGS_FILE As String = "settings.xml"
    Public Const msMETA_INF_FOLDER As String = "META-INF\"
    Public Const msMANIFEST_FILE As String = "manifest.xml"
    Private msManifestAdd As String = String.Empty
    Private mFileWriter As System.IO.StreamWriter
    Private msReformatRed() As String = Nothing
    Private mnTableSplitMaxRows As Integer = 0
    Private mnTableSplitRepeatRows As Integer = 0
    Private mbPrintColumnNames As Boolean = True
    Private mbTablePageBreaks As Boolean = False
    Private msScreenName As String = String.Empty
    Private mbExportFile As Boolean = False
    Private msRowText As String = String.Empty
    '***** XML Setup ***************************************************************************
    'Name of the config file
    Private Const msXMLFILENAME As String = "PrintOptions.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************
    Private mnColCount As Integer = 0
    Private mnSheet As Integer = 0
    Private msDelim As String = ","
    Private msPageTitle As String = String.Empty
    Private mnSubTitleCols As Integer = 3
    Private msZipUtil As String = String.Empty
    Private msExcelUtil As String = String.Empty
    Private msFilter As String = String.Empty
    Private colTabs As Collection
    Private colSharedStrings As Collection
    Private msColumn As String = String.Empty
    Private mnRow As Integer = 0
    Public Enum eExportFormat As Integer
        nNONE = -1
        nCSV = 0
        nTXT = 1
        nXML = 2
        nODS = 3
        nXLS = 4
        nXLSX = 5
    End Enum
    Private meExportFormat As eExportFormat = eExportFormat.nNONE
    Private Enum eBlockCfgval As Integer
        nOFF = 0
        nON = 1
        nNOCHANGE = -1
        nLEFT = 2
        nCENTER = 3
        nRIGHT = 4
        nTOP = 2
        nMIDDLE = 3
        nBOTTOM = 4
        nH1 = 1
        nH2 = 2
        nH3 = 3
        nH4 = 4
        nH5 = 5
        nH6 = 6
    End Enum
    Private Structure tBlockFormat
        Dim Bold As eBlockCfgval
        Dim Italic As eBlockCfgval
        Dim Heading As eBlockCfgval
        Dim HAlign As eBlockCfgval
        Dim VAlign As eBlockCfgval
        'That's it for now, maybe we'll get into fonts later
    End Structure
    Private mColCfg() As tBlockFormat = Nothing
    Private mRowCfg() As tBlockFormat = Nothing
    Private mBlockCfg As tBlockFormat = Nothing
    Private mnColWidths() As Single = Nothing
    Private mnColWidthIndex() As Integer = Nothing
    Private Enum ePrintTask
        Preview
        PageSetup
        Print
        PrintNow
        SaveAs
        None
    End Enum
    Private mPrintTask As ePrintTask = ePrintTask.Print
    Friend Property ColumnWidthsList() As Single()
        'List of column widths in the export file
        'Set before start doc call.
        Get
            Return mnColWidths
        End Get
        Set(ByVal value As Single())
            mnColWidths = value
        End Set
    End Property
    Friend Property ColumnWidthIndex() As Integer()
        'Index into columnWidthsList for the current table
        Get
            Return mnColWidthIndex
        End Get
        Set(ByVal value As Integer())
            mnColWidthIndex = value
        End Set
    End Property
    Friend Property SubTitleCols() As Integer
        '********************************************************************************************
        'Description:  Number of columns in subtitle table
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnSubTitleCols
        End Get
        Set(ByVal value As Integer)
            mnSubTitleCols = value
        End Set
    End Property

    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  return where we are looking for print options
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msXMLFilePath
        End Get

    End Property
    Friend ReadOnly Property Busy() As Boolean
        Get
            Try
                If mWebPrint Is Nothing Then
                    Return False
                Else
                    Return mWebPrint.IsBusy Or mbPrintBusy
                End If
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property
    Friend Property ReformatRed() As String()
        Get
            Return msReformatRed
        End Get
        Set(ByVal value As String())
            If value.GetUpperBound(0) > 0 Then
                msReformatRed = value
            Else
                msReformatRed = Nothing
            End If

        End Set
    End Property
    Friend Property MaxRowsPerTable() As Integer
        '********************************************************************************************
        'Description:  Maximum rows before splitting up auto-generated tables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTableSplitMaxRows
        End Get
        Set(ByVal value As Integer)
            mnTableSplitMaxRows = value
        End Set
    End Property
    Friend Property HeaderRowsPerTable() As Integer
        '********************************************************************************************
        'Description:  Number of header rows to repeat at the top when splitting up auto-generated tables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTableSplitRepeatRows
        End Get
        Set(ByVal value As Integer)
            mnTableSplitRepeatRows = value
        End Set
    End Property
    Friend Property PrintColumnNames() As Boolean
        '********************************************************************************************
        'Description:  Determines if column names are printed for data tables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbPrintColumnNames
        End Get
        Set(ByVal value As Boolean)
            mbPrintColumnNames = value
        End Set
    End Property
    Friend Sub subClearTableFormat()
        '********************************************************************************************
        'Description:  clear table and row formatting
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mColCfg = Nothing
        mRowCfg = Nothing
        msReformatRed = Nothing
    End Sub
    Friend Sub subClearBlockFormat()
        '********************************************************************************************
        'Description:  clear formatting for text blocks
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mBlockCfg = Nothing

    End Sub
    Private Function sGetFormatString(ByRef BlockCfg() As tBlockFormat, ByVal nIndex As Integer) As String()
        '********************************************************************************************
        'Description:  make a format string from the blockcfg - with array bounds check
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (BlockCfg Is Nothing) OrElse (nIndex > BlockCfg.GetUpperBound(0)) OrElse (nIndex < BlockCfg.GetLowerBound(0)) Then
            Dim sTmp(2) As String
            sTmp(0) = String.Empty  'Inside the block tag
            sTmp(1) = String.Empty  'starting tags (<b>...)
            sTmp(2) = String.Empty  'closing tags (</b>...)
            Return (sTmp)
        Else
            Return sGetFormatString(BlockCfg(nIndex))
        End If
    End Function
    Private Function sGetFormatString(ByRef BlockCfg As tBlockFormat) As String()
        '********************************************************************************************
        'Description:  make a format string from the blockcfg
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp(2) As String
        sTmp(0) = String.Empty  'Inside the block tag
        sTmp(1) = String.Empty  'starting tags (<b>...)
        sTmp(2) = String.Empty  'closing tags (</b>...)
        If BlockCfg.Bold = eBlockCfgval.nON Then
            sTmp(1) = sTmp(1) & "<b>"
            sTmp(2) = "</b>" & sTmp(2)
        End If
        If BlockCfg.Italic = eBlockCfgval.nON Then
            sTmp(1) = sTmp(1) & "<i>"
            sTmp(2) = "</i>" & sTmp(2)
        End If
        If CInt(BlockCfg.Heading) > 0 Then
            sTmp(1) = sTmp(1) & "<h" & CInt(BlockCfg.Heading).ToString & ">"
            sTmp(2) = "</h" & CInt(BlockCfg.Heading).ToString & ">" & sTmp(2)
        End If
        Select Case BlockCfg.HAlign
            Case eBlockCfgval.nLEFT
                sTmp(0) = sTmp(0) & " align=""left"""
            Case eBlockCfgval.nCENTER
                sTmp(0) = sTmp(0) & " align=""center"""
            Case eBlockCfgval.nRIGHT
                sTmp(0) = sTmp(0) & " align=""right"""
        End Select
        Select Case BlockCfg.VAlign
            Case eBlockCfgval.nTOP
                sTmp(0) = sTmp(0) & " align=""top"""
            Case eBlockCfgval.nMIDDLE
                sTmp(0) = sTmp(0) & " align=""middle"""
            Case eBlockCfgval.nBOTTOM
                sTmp(0) = sTmp(0) & " align=""bottom"""
        End Select
        Return sTmp
    End Function
    Private Sub subInitCfgStruct(ByRef BlockCfg As tBlockFormat)
        '********************************************************************************************
        'Description:  initialize the cell config
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        BlockCfg.Bold = eBlockCfgval.nNOCHANGE
        BlockCfg.Italic = eBlockCfgval.nNOCHANGE
        BlockCfg.Heading = eBlockCfgval.nNOCHANGE
        BlockCfg.HAlign = eBlockCfgval.nNOCHANGE
        BlockCfg.VAlign = eBlockCfgval.nNOCHANGE
    End Sub
    Private Sub subSet1BlockCfg(ByRef BlockCfg As tBlockFormat, ByRef Changes As tBlockFormat)
        '********************************************************************************************
        'Description:  update the cell config
        '
        'Parameters: BlockCfg - cell that gets changed, Changes
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Changes.Bold <> eBlockCfgval.nNOCHANGE Then
            BlockCfg.Bold = Changes.Bold
        End If
        If Changes.Italic <> eBlockCfgval.nNOCHANGE Then
            BlockCfg.Italic = Changes.Italic
        End If
        If Changes.Heading <> eBlockCfgval.nNOCHANGE Then
            BlockCfg.Heading = Changes.Heading
        End If
        If Changes.HAlign <> eBlockCfgval.nNOCHANGE Then
            BlockCfg.HAlign = Changes.HAlign
        End If
        If Changes.VAlign <> eBlockCfgval.nNOCHANGE Then
            BlockCfg.VAlign = Changes.VAlign
        End If
    End Sub

    Private Function eGetTagCode(ByVal sTag As String) As eBlockCfgval
        '********************************************************************************************
        'Description:  get on or off value from string
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If InStr(sTag, "off") > 0 Then
                Return eBlockCfgval.nOFF
            ElseIf InStr(sTag, "0") > 0 Then
                Return eBlockCfgval.nOFF
            ElseIf InStr(sTag, "on") > 0 Then
                Return eBlockCfgval.nON
            ElseIf InStr(sTag, "1") > 0 Then
                Return eBlockCfgval.nON
            ElseIf InStr(sTag, "left") > 0 Then
                Return eBlockCfgval.nLEFT
            ElseIf InStr(sTag, "middle") > 0 Then
                Return eBlockCfgval.nMIDDLE
            ElseIf InStr(sTag, "right") > 0 Then
                Return eBlockCfgval.nRIGHT
            ElseIf InStr(sTag, "top") > 0 Then
                Return eBlockCfgval.nTOP
            ElseIf InStr(sTag, "center") > 0 Then
                Return eBlockCfgval.nCENTER
            ElseIf InStr(sTag, "bottom") > 0 Then
                Return eBlockCfgval.nBOTTOM
            Else
                Return eBlockCfgval.nON 'default to on if it's not specific
            End If
        Catch ex As Exception
            Return eBlockCfgval.nNOCHANGE
        End Try
    End Function
    Private Function eGetNumericTag(ByVal sTag As String) As eBlockCfgval
        '********************************************************************************************
        'Description:  get numeric value from string
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nEq As Integer = InStr(sTag, "=")
            If nEq > 0 Then
                'heading=1, h=1
                Dim sVal As String = sTag.Substring(nEq).Trim
                If IsNumeric(sTag) Then
                    Return CType(CInt(sTag), eBlockCfgval)
                ElseIf InStr(sTag, "off") > 0 Then
                    Return eBlockCfgval.nOFF
                ElseIf InStr(sTag, "on") > 0 Then
                    Return eBlockCfgval.nON
                Else
                    Return eBlockCfgval.nNOCHANGE
                End If
            Else
                'h1,heading1
                Dim sVal As String = sTag.Trim.Substring(sTag.Length - 1)
                If IsNumeric(sTag) Then
                    Return CType(CInt(sTag), eBlockCfgval)
                Else
                    Return eBlockCfgval.nNOCHANGE
                End If
            End If

        Catch ex As Exception
            Return eBlockCfgval.nNOCHANGE
        End Try
    End Function
    Private Sub subSetBlockCfg(ByRef BlockCfg() As tBlockFormat, ByVal sSetting As String, ByVal nFirstItem As Integer, Optional ByVal nLastItem As Integer = -1)
        '********************************************************************************************
        'Description:  set config for a range of rows or columns
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim CellChange As tBlockFormat
        subInitCfgStruct(CellChange)
        If nLastItem = -1 Then
            nLastItem = nFirstItem
        End If
        Dim sSettings() As String = Split(sSetting, ",")
        For Each sTmp As String In sSettings
            Dim sTmp2 As String = sTmp.ToLower.Trim
            If sTmp2.StartsWith("bold") Then
                CellChange.Bold = eGetTagCode(sTmp2)
            ElseIf sTmp2.StartsWith("italic") Then
                CellChange.Italic = eGetTagCode(sTmp2)
            ElseIf sTmp2.StartsWith("align") OrElse sTmp2.StartsWith("halign") Then
                CellChange.HAlign = eGetTagCode(sTmp2)
            ElseIf sTmp2.StartsWith("valign") Then
                CellChange.VAlign = eGetTagCode(sTmp2)
            ElseIf sTmp2.StartsWith("heading") Then
                CellChange.Heading = eGetNumericTag(sTmp2)
            ElseIf sTmp2.StartsWith("h") Then
                CellChange.Heading = eGetNumericTag(sTmp2)
            End If
        Next
        If BlockCfg Is Nothing Then
            'Nothing set, start with a redim
            ReDim BlockCfg(nLastItem)
            For nItem As Integer = 0 To nLastItem
                subInitCfgStruct(BlockCfg(nItem))
                If nItem >= nFirstItem Then
                    subSet1BlockCfg(BlockCfg(nItem), CellChange)
                End If
            Next
        Else
            ' Not empty
            Dim nOldBound As Integer = mRowCfg.GetUpperBound(0)
            If nOldBound < nLastItem Then
                'Add new rows
                ReDim Preserve BlockCfg(nLastItem)
                For nItem As Integer = nOldBound + 1 To nLastItem
                    subInitCfgStruct(BlockCfg(nItem))
                    If nItem >= nFirstItem Then
                        subSet1BlockCfg(BlockCfg(nItem), CellChange)
                    End If
                Next
            End If
            If nOldBound >= nFirstItem Then
                'settings in rows that are already there
                If nLastItem > nOldBound Then
                    nLastItem = nOldBound
                End If
                For nItem As Integer = nFirstItem To nLastItem
                    subSet1BlockCfg(BlockCfg(nItem), CellChange)
                Next
            End If
        End If
    End Sub
    Friend Sub subSetRowcfg(ByVal sSetting As String, ByVal nFirstItem As Integer, Optional ByVal nLastItem As Integer = -1)
        '********************************************************************************************
        'Description:  set format for a range of rows
        '
        'Parameters: sSetting = parameters to set in string form, first and last rows
        '           sSetting ex. =  "bold=on,h1,valign=middle"
        '   Supported tags: valign={top|middle|bottom}, halign={left|center|right}
        '                   bold[={on|off|0|1}, italic[={on|off|0|1}
        '                   h[eading][=]{1...6}        
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subSetBlockCfg(mRowCfg, sSetting, nFirstItem, nLastItem)
    End Sub
    Friend Sub subSetColumnCfg(ByVal sSetting As String, ByVal nFirstItem As Integer, Optional ByVal nLastItem As Integer = -1)
        '********************************************************************************************
        'Description:  set format for a range of columns
        '
        'Parameters: sSetting = parameters to set in string form, first and last columns
        '           sSetting ex. =  "bold=on,h1,valign=middle"
        '   Supported tags: valign={top|middle|bottom}, halign={left|center|right}
        '                   bold[={on|off|0|1}, italic[={on|off|0|1}
        '                   h[eading][=]{1...6}
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subSetBlockCfg(mColCfg, sSetting, nFirstItem, nLastItem)
    End Sub
    Friend Sub subDeleteTempFile()
        '********************************************************************************************
        'Description:  Deletes the last output file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Delete the old output file
        If msDeleteFile <> String.Empty Then
            Try
                My.Computer.FileSystem.DeleteFile(msDeleteFile)
                msDeleteFile = String.Empty
            Catch ex As Exception
                'just let it continue
            End Try
        End If
    End Sub
    Private Sub subPrintRtfToHtml(ByVal rtbPage As RichTextBox, ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  convert simple RTF to HTML
        '               If this gets really ugly and doesn't work, go up to the declares and set 
        '               mbDECODE_RTF = False to print plain text
        'Parameters: RichTextBox, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim nIndex As Integer = 0
        Dim nLength As Integer = rtbPage.Rtf.Length
        Dim sChar As String
        Dim nBracketCount As Integer = 0
        Dim sLineOut As String = String.Empty
        Const nMAX_TAG_LEN As Integer = 10
        Dim bInParagraph As Boolean = False
        While nIndex < nLength
            'The bracket count just ignores a bunch of header font and color info.
            sChar = rtbPage.Rtf.Substring(nIndex, 1)
            If nBracketCount <= 1 Then
                Select Case sChar
                    Case "{"
                        nBracketCount += 1
                        nIndex += 1
                    Case "}"
                        nBracketCount -= 1
                        nIndex += 1
                    Case "\"
                        'formatting tags
                        If ((nIndex + nMAX_TAG_LEN) >= nLength) Then
                            sChar = rtbPage.Rtf.Substring(nIndex)
                        Else
                            sChar = rtbPage.Rtf.Substring(nIndex, nMAX_TAG_LEN)
                        End If
                        'Paragraph alignment
                        If sChar.StartsWith("\pard") Then
                            If bInParagraph Then
                                sLineOut = sLineOut & "</p>"
                                fileWriter.WriteLine(sLineOut)
                                sLineOut = String.Empty
                            End If
                            If sChar.StartsWith("\pard\qc") Then
                                sLineOut = sLineOut & "<p align=center>"
                                bInParagraph = True
                                nIndex += 8
                            ElseIf sChar.StartsWith("\pard\qr") Then
                                sLineOut = sLineOut & "<p align=right>"
                                bInParagraph = True
                                nIndex += 8
                            Else
                                sLineOut = sLineOut & "<p align=left>"
                                bInParagraph = True
                                nIndex += 5
                            End If
                            sChar = rtbPage.Rtf.Substring(nIndex, 1)
                            'Check the next character
                            If ((sChar = " ") Or (sChar = vbCr) Or (sChar = vbLf)) Then
                                'whitespace after the tag means the next character gets printed
                                nIndex += 1
                                'If it's "\" then it'll be antoher tag
                            End If
                        Else
                            ' Simple formatting 
                            If sChar.StartsWith("\par") Then 'Newline
                                sLineOut = sLineOut & "<br>"
                                fileWriter.WriteLine(sLineOut)
                                sLineOut = String.Empty
                                nIndex += 4
                            ElseIf sChar.StartsWith("\tab") Then 'tab
                                sLineOut = sLineOut & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                nIndex += 4
                            ElseIf sChar.StartsWith("\b0") Then 'Bold
                                sLineOut = sLineOut & "</b>"
                                nIndex += 3
                            ElseIf sChar.StartsWith("\b") Then
                                sLineOut = sLineOut & "<b>"
                                nIndex += 2
                            ElseIf sChar.StartsWith("\i0") Then 'Italic
                                sLineOut = sLineOut & "</i>"
                                nIndex += 3
                            ElseIf sChar.StartsWith("\i") Then
                                sLineOut = sLineOut & "<i>"
                                nIndex += 2
                            ElseIf sChar.StartsWith("\ulnone") Then 'Underline
                                sLineOut = sLineOut & "</u>"
                                nIndex += 7
                            ElseIf sChar.StartsWith("\ul") Then
                                sLineOut = sLineOut & "<u>"
                                nIndex += 3
                            Else
                                'Unknown tag - skip to the next tag or whitespace
                                Do
                                    nIndex += 1
                                    sChar = rtbPage.Rtf.Substring(nIndex, 1)
                                Loop Until ((sChar = " ") Or (sChar = vbCr) Or (sChar = vbLf) Or (sChar = "\") _
                                    Or (sChar = "{") Or (sChar = "}"))
                            End If
                            sChar = rtbPage.Rtf.Substring(nIndex, 1)
                            'Check the next character
                            If ((sChar = " ") Or (sChar = vbCr) Or (sChar = vbLf)) Then
                                'whitespace after the tag means the next character gets printed
                                nIndex += 1
                                'If it's "\" then it'll be another tag
                            End If
                        End If
                    Case Else
                        sLineOut = sLineOut & sChar
                        nIndex += 1
                End Select
            Else
                Select Case sChar
                    Case "{"
                        nBracketCount += 1
                    Case "}"
                        nBracketCount -= 1
                End Select
                nIndex += 1
            End If
        End While
        If sLineOut.Length > 0 Then
            fileWriter.WriteLine(sLineOut)
            sLineOut = String.Empty
        End If

    End Sub
    Private Sub subPrintStringArrayToHtml(ByVal sText As String(), ByRef fileWriter As System.IO.StreamWriter, _
                                          Optional ByVal sHeader As String = Nothing)
        '********************************************************************************************
        'Description:  convert string array to HTML table
        ' 
        '
        'Parameters: string array, filewriter object, uses module table config settings
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************

        Dim nRowCfgOffset As Integer = 0
        Dim nBaseRowCfgOffset As Integer = 0
        Dim nColCfgOffset As Integer = 0
        Dim sFormatRow(2) As String
        Dim sFormatCol(2) As String
        Dim sItem As String = String.Empty
        If mbExportFile Then
            For nRow As Integer = 0 To sText.GetUpperBound(0)
                subExportRowStart(fileWriter)
                Dim sTxtSplit() As String = Split(sText(nRow), vbTab)
                For nCell As Integer = 0 To sTxtSplit.GetUpperBound(0)
                    subExportCell(sTxtSplit(nCell), fileWriter)
                Next
                subExportRowEnd(fileWriter)
            Next 'row
        Else
            'Just check the first line for tabs.
            'If there are tabs, build a table, otherwise treat it like plain text.
            If InStr(sText(0), vbTab) > 0 Then
                Dim bDone As Boolean = False
                Dim nHeaderRows As Integer
                Dim nMaxRows As Integer
                If mbTablePageBreaks Then
                    nHeaderRows = mnTableSplitRepeatRows
                    nMaxRows = mnTableSplitMaxRows + mnTableSplitRepeatRows
                Else
                    nHeaderRows = mnTableSplitRepeatRows
                    nMaxRows = Integer.MaxValue
                End If
                'Check for allowable settings
                If nHeaderRows > sText.GetUpperBound(0) Then
                    nHeaderRows = sText.GetUpperBound(0) + 1
                End If
                If nMaxRows > sText.GetUpperBound(0) Then
                    nMaxRows = sText.GetUpperBound(0) + 1
                End If
                Dim nDataRow As Integer = nHeaderRows
                Do While (bDone = False)
                    'Make a table
                    fileWriter.WriteLine("<table border=""1"" cellspacing=""0"">")
                    If nHeaderRows > 0 Then
                        fileWriter.WriteLine("<thead>")
                        For nRow As Integer = 0 To nHeaderRows - 1
                            Dim sRow As String() = Split(sText(nRow), vbTab)
                            sFormatRow = sGetFormatString(mRowCfg, (nRow + nRowCfgOffset + nBaseRowCfgOffset))
                            fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                            For nCol As Integer = 0 To sRow.GetUpperBound(0)
                                sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                                sItem = sRow(nCol)
                                If sItem = String.Empty Then
                                    sItem = "&nbsp;"
                                End If
                                fileWriter.WriteLine("<td" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                        sItem & sFormatCol(2) & sFormatRow(2) & "</td>")
                            Next
                            fileWriter.WriteLine("</tr>")
                        Next 'row
                        fileWriter.WriteLine("</thead>")
                    End If
                    fileWriter.WriteLine("<tbody>")
                    For nRow As Integer = nHeaderRows To nMaxRows - 1
                        Dim sRow As String() = Split(sText(nDataRow), vbTab)
                        nDataRow += 1
                        sFormatRow = sGetFormatString(mRowCfg, (nRow + nRowCfgOffset + nBaseRowCfgOffset))
                        fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                        For nCol As Integer = 0 To sRow.GetUpperBound(0)
                            sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                            sItem = sRow(nCol)
                            If sItem = String.Empty Then
                                sItem = "&nbsp;"
                            End If
                            fileWriter.WriteLine("<td" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                    sItem & sFormatCol(2) & sFormatRow(2) & "</td>")
                        Next
                        fileWriter.WriteLine("</tr>")
                        If nDataRow > sText.GetUpperBound(0) Then
                            bDone = True
                            Exit For
                        End If
                    Next 'row
                    'CBZ 7/27/11 end loop if only 1 row
                    If nMaxRows = 1 Then
                        bDone = True
                    End If
                    fileWriter.WriteLine("</tbody></table>")
                    If bDone = False Then
                        If nHeaderRows >= nMaxRows Then
                            bDone = True
                        Else
                            fileWriter.WriteLine(msPageBreakHTML)
                            If sHeader IsNot Nothing Then
                                fileWriter.WriteLine(sHeader)
                            End If
                        End If
                    End If
                Loop
            Else
                'Just print it out
                fileWriter.WriteLine("<PRE>")
                For nRow As Integer = 0 To sText.GetUpperBound(0)
                    fileWriter.WriteLine(sText(nRow))
                Next 'row
                fileWriter.WriteLine("</PRE>")
            End If

        End If

    End Sub
    Private Sub subPrintDgToHtml(ByRef oDG As DataGridView, ByRef fileWriter As System.IO.StreamWriter, _
                                          Optional ByVal sHeader As String = Nothing)
        '********************************************************************************************
        'Description:  convert datagridview to HTML
        ' 
        '
        'Parameters: datagridview, filewriter object, uses module table config settings
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        ' 05/06/14  MSW     Avoid foreign numbers
        '********************************************************************************************
        Dim nRowCfgOffset As Integer = 0
        Dim nBaseRowCfgOffset As Integer = 0
        Dim nColCfgOffset As Integer = 0
        Dim sFormatRow(2) As String
        Dim sFormatCol(2) As String
        Dim sItem As String = String.Empty
        Dim bDone As Boolean = False
        Dim nHeaderRows As Integer
        Dim nMaxRows As Integer
        If oDG.Rows.Count = 0 Then
            Exit Sub
        End If
        'MSW 5/6/14 avoid foreign numbers
        Dim sCurrentCulture As String = My.Application.Culture.Name
        My.Application.ChangeCulture("en-US")
        'Check for blank row headers
        Dim bRowHeaders As Boolean = oDG.Rows(0).HeaderCell.Visible
        If bRowHeaders Then
            bRowHeaders = False
            For Each oRow As DataGridViewRow In oDG.Rows
                If (oRow.HeaderCell.Value IsNot Nothing) AndAlso (oRow.HeaderCell.Value.ToString <> String.Empty) Then
                    bRowHeaders = True
                    Exit For
                End If
            Next
        End If
        If mbExportFile Then
            If oDG.ColumnHeadersVisible Then
                subExportRowStart(mFileWriter)
                If bRowHeaders Then
                    subExportCell(" ", mFileWriter)
                End If
                For nCol As Integer = 0 To oDG.ColumnCount - 1
                    If oDG.Columns(nCol).Visible Then
                        sItem = oDG.Columns(nCol).HeaderText.ToString
                        If sItem = String.Empty Then
                            sItem = " "
                        End If
                        subExportCell(sItem, mFileWriter)
                    End If
                Next
                subExportRowEnd(mFileWriter)
            End If
            For nRow As Integer = 0 To oDG.RowCount - 1
                If oDG.Rows(nRow).Visible() Then
                    subExportRowStart(mFileWriter)
                    If bRowHeaders Then
                        Dim sText As String = String.Empty
                        If oDG.Rows(nRow).HeaderCell.Value IsNot Nothing Then
                            sText = oDG.Rows(nRow).HeaderCell.Value.ToString
                        End If
                        subExportCell(sText, mFileWriter)
                    End If
                    For nCol As Integer = 0 To oDG.ColumnCount - 1
                        'Swap out commas from inside cells before putting in csv format
                        Dim sText As String = String.Empty
                        If oDG.Rows(nRow).Cells(nCol).Value IsNot Nothing Then
                            sText = oDG.Rows(nRow).Cells(nCol).Value.ToString()
                        End If
                        subExportCell(sText, mFileWriter)
                    Next
                    subExportRowEnd(mFileWriter)
                End If
            Next
        Else
            If mbTablePageBreaks Then
                nHeaderRows = mnTableSplitRepeatRows
                nMaxRows = mnTableSplitMaxRows + mnTableSplitRepeatRows
            Else
                nHeaderRows = 0
                nMaxRows = Integer.MaxValue
            End If
            'Check for allowable settings
            Dim nColHeaderCount As Integer = 0
            If oDG.ColumnHeadersVisible Then
                nColHeaderCount = 1
            End If
            Dim nRowHeaderCount As Integer = 0
            If oDG.RowHeadersVisible Then
                nRowHeaderCount = 1
            End If
            If nHeaderRows >= (oDG.RowCount + nRowHeaderCount - 1) Then
                nHeaderRows = oDG.RowCount + nRowHeaderCount
            End If
            If nMaxRows > (oDG.RowCount + nRowHeaderCount - 1) Then
                nMaxRows = oDG.RowCount + nRowHeaderCount
            End If
            Dim nNextRow As Integer = nHeaderRows - nRowHeaderCount
            If nNextRow < 0 Then
                nNextRow = 0
            End If
            Dim nDataRow As Integer = 0
            Dim bInHeader As Boolean = False
            Do While (bDone = False)
                fileWriter.WriteLine("<table border=""1"" cellspacing=""0"">")
                If nHeaderRows > 0 Then
                    fileWriter.WriteLine("<thead>")
                    bInHeader = True
                Else
                    fileWriter.WriteLine("<tbody>")
                End If
                If oDG.ColumnHeadersVisible And ((nHeaderRows > 0) Or (nDataRow = 0)) Then 'Headers vis. And ((repeat headers) or (1st pass))
                    nBaseRowCfgOffset = 1
                    sFormatRow = sGetFormatString(mRowCfg, 0)
                    fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                    If bRowHeaders Then
                        nColCfgOffset = 1
                        sFormatCol = sGetFormatString(mColCfg(0))
                        fileWriter.WriteLine("<th" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                     "&nbsp;" & sFormatCol(2) & sFormatRow(2) & "</th>")
                    Else
                        nColCfgOffset = 0
                    End If
                    For nCol As Integer = 0 To oDG.ColumnCount - 1
                        If oDG.Columns(nCol).Visible Then
                            sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                            sItem = oDG.Columns(nCol).HeaderText.ToString
                            If sItem = String.Empty Then
                                sItem = "&nbsp;"
                            End If
                            fileWriter.WriteLine("<th" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                              sItem & sFormatCol(2) & sFormatRow(2) & "</th>")
                        Else
                            nColCfgOffset = nColCfgOffset - 1
                        End If
                    Next
                    fileWriter.WriteLine("</tr>")
                Else
                    nRowCfgOffset = 0
                    nBaseRowCfgOffset = 0
                End If
                For nRow As Integer = 0 To (nMaxRows - nRowHeaderCount) - 1
                    If (nHeaderRows - nRowHeaderCount) > nRow Then
                        nDataRow = nRow
                        If ((nMaxRows - nRowHeaderCount) - 1) >= (oDG.RowCount - 1) Then
                            bDone = True
                        End If
                        If bInHeader Then
                            bInHeader = False
                            fileWriter.WriteLine("</thead><tbody>")
                        End If
                    Else
                        nDataRow = nNextRow
                        nNextRow += 1
                        If nDataRow >= oDG.RowCount Then
                            'This row is too far, get out now
                            bDone = True
                            Exit For
                        End If
                        If nNextRow >= oDG.RowCount Then
                            'finish this row, then get out
                            bDone = True
                        End If
                    End If
                    If oDG.Rows(nDataRow).Visible Then
                        sFormatRow = sGetFormatString(mRowCfg, (nRow + nRowCfgOffset + nBaseRowCfgOffset))
                        fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                        If bRowHeaders Then
                            nColCfgOffset = 1
                            sFormatCol = sGetFormatString(mColCfg(0))
                            If (oDG.Rows(nDataRow).HeaderCell.Value IsNot Nothing) Then
                                sItem = oDG.Rows(nDataRow).HeaderCell.Value.ToString
                                If sItem = String.Empty Then
                                    sItem = "&nbsp;"
                                End If
                            Else
                                sItem = "&nbsp;"
                            End If
                            fileWriter.WriteLine("<td" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                                            sItem & sFormatCol(2) & sFormatRow(2) & "</td>")
                        Else
                            nColCfgOffset = 0
                        End If
                        For nCol As Integer = 0 To oDG.ColumnCount - 1
                            If oDG.Columns(nCol).Visible Then
                                If (oDG.Rows(nDataRow).Cells(nCol).Value IsNot Nothing) Then
                                    sItem = oDG.Rows(nDataRow).Cells(nCol).Value.ToString
                                    If sItem = String.Empty Then
                                        sItem = "&nbsp;"
                                    End If
                                Else
                                    sItem = "&nbsp;"
                                End If

                                'Catch the differences in compares
                                If (msReformatRed IsNot Nothing) AndAlso (oDG.Rows(nDataRow).Cells(nCol).Style.ForeColor = Color.Red) Then
                                    sItem = msReformatRed(0) & sItem & msReformatRed(1)
                                End If
                                'transfer formating that can be properly represented in print
                                If (oDG.Rows(nDataRow).Cells(nCol).Style.Font IsNot Nothing) Then
                                    If (oDG.Rows(nDataRow).Cells(nCol).Style.Font.Bold) Then
                                        sItem = "<b>" & sItem & "</b>"
                                    End If
                                    If (oDG.Rows(nDataRow).Cells(nCol).Style.Font.Italic) Then
                                        sItem = "<i>" & sItem & "</i>"
                                    End If
                                End If
                                sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                                fileWriter.WriteLine("<td" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                        sItem & sFormatCol(2) & sFormatRow(2) & "</td>")
                            Else
                                nColCfgOffset = nColCfgOffset - 1
                            End If
                        Next
                        fileWriter.WriteLine("</tr>")
                    Else
                        nRowCfgOffset = nRowCfgOffset - 1
                    End If
                Next 'row
                fileWriter.WriteLine("</tbody></table>")
                If bDone = False Then
                    fileWriter.WriteLine(msPageBreakHTML)
                    If sHeader IsNot Nothing Then
                        fileWriter.WriteLine(sHeader)
                    End If

                End If
            Loop
        End If
        'MSW 5/6/14 avoid foreign numbers
        My.Application.ChangeCulture(sCurrentCulture)
    End Sub
    Private Sub subPrintDataTableToHtml(ByRef oTable As DataTable, ByRef fileWriter As System.IO.StreamWriter, _
                                          Optional ByVal sHeader As String = Nothing)
        '********************************************************************************************
        'Description:  convert datagridview to HTML
        ' 
        '
        'Parameters: datagridview, filewriter object, uses module table config settings
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        ' 05/06/14  MSW     Avoid foreign numbers
        '********************************************************************************************
        Dim nRowCfgOffset As Integer = 0
        Dim nBaseRowCfgOffset As Integer = 0
        Dim nColCfgOffset As Integer = 0
        Dim sFormatRow(2) As String
        Dim sFormatCol(2) As String
        Dim sItem As String = String.Empty
        Dim bDone As Boolean = False
        Dim nHeaderRows As Integer
        Dim nMaxRows As Integer
        If oTable.Rows.Count = 0 Then
            Exit Sub
        End If
        'MSW 5/6/14 avoid foreign numbers
        Dim sCurrentCulture As String = My.Application.Culture.Name
        My.Application.ChangeCulture("en-US")

        'Check for blank row headers
        If mbExportFile Then
            If mbPrintColumnNames Then
                subExportRowStart(mFileWriter)
                For nCol As Integer = 0 To oTable.Columns.Count - 1
                    sItem = oTable.Columns(nCol).ColumnName
                    If sItem = String.Empty Then
                        sItem = " "
                    End If
                    subExportCell(sItem, mFileWriter)
                Next
                subExportRowEnd(mFileWriter)
            End If
            For nRow As Integer = 0 To oTable.Rows.Count - 1
                subExportRowStart(mFileWriter)
                For nCol As Integer = 0 To oTable.Columns.Count - 1
                    subExportCell(oTable.Rows(nRow).Item(nCol).ToString, mFileWriter)
                Next
                subExportRowEnd(mFileWriter)
            Next
        Else
            If mbTablePageBreaks Then
                nHeaderRows = mnTableSplitRepeatRows
                nMaxRows = mnTableSplitMaxRows + mnTableSplitRepeatRows
            Else
                nHeaderRows = 0
                nMaxRows = Integer.MaxValue
            End If
            'Check for allowable settings
            Dim nColHeaderCount As Integer = 1
            Dim nRowHeaderCount As Integer = 0
            If nHeaderRows >= (oTable.Rows.Count + nRowHeaderCount - 1) Then
                nHeaderRows = oTable.Rows.Count + nRowHeaderCount
            End If
            If nMaxRows > (oTable.Rows.Count + nRowHeaderCount - 1) Then
                nMaxRows = oTable.Rows.Count + nRowHeaderCount
            End If
            Dim nNextRow As Integer = nHeaderRows - nRowHeaderCount
            If nNextRow < 0 Then
                nNextRow = 0
            End If
            Dim nDataRow As Integer = 0
            Dim bInHeader As Boolean = False
            Do While (bDone = False)
                fileWriter.WriteLine("<table border=""1"" cellspacing=""0"">")
                If nHeaderRows > 0 Then
                    fileWriter.WriteLine("<thead>")
                    bInHeader = True
                Else
                    fileWriter.WriteLine("<tbody>")
                End If
                nBaseRowCfgOffset = 1
                If mbPrintColumnNames Then
                    sFormatRow = sGetFormatString(mRowCfg, 0)
                    fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                    nColCfgOffset = 0

                    For nCol As Integer = 0 To oTable.Columns.Count - 1
                        sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                        sItem = oTable.Columns(nCol).ColumnName
                        If sItem = String.Empty Then
                            sItem = "&nbsp;"
                        End If
                        fileWriter.WriteLine("<th" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                          sItem & sFormatCol(2) & sFormatRow(2) & "</th>")

                    Next
                    fileWriter.WriteLine("</tr>")
                End If
                For nRow As Integer = 0 To (nMaxRows - nRowHeaderCount) - 1
                    If (nHeaderRows - nRowHeaderCount) > nRow Then
                        nDataRow = nRow
                        If ((nMaxRows - nRowHeaderCount) - 1) >= (oTable.Rows.Count - 1) Then
                            bDone = True
                        End If
                        If bInHeader Then
                            bInHeader = False
                            fileWriter.WriteLine("</thead><tbody>")
                        End If
                    Else
                        nDataRow = nNextRow
                        nNextRow += 1
                        If nDataRow >= oTable.Rows.Count Then
                            'This row is too far, get out now
                            bDone = True
                            Exit For
                        End If
                        If nNextRow >= oTable.Rows.Count Then
                            'finish this row, then get out
                            bDone = True
                        End If
                    End If

                    sFormatRow = sGetFormatString(mRowCfg, (nRow + nRowCfgOffset + nBaseRowCfgOffset))
                    fileWriter.WriteLine("<tr" & sFormatRow(0) & ">")
                    nColCfgOffset = 0
                    For nCol As Integer = 0 To oTable.Columns.Count - 1
                        If (oTable.Rows(nDataRow).Item(nCol) IsNot Nothing) Then
                            sItem = oTable.Rows(nDataRow).Item(nCol).ToString
                            If sItem = String.Empty Then
                                sItem = "&nbsp;"
                            End If
                        Else
                            sItem = "&nbsp;"
                        End If

                        sFormatCol = sGetFormatString(mColCfg, (nCol + nColCfgOffset))
                        fileWriter.WriteLine("<td" & sFormatCol(0) & ">" & sFormatRow(1) & sFormatCol(1) & _
                                sItem & sFormatCol(2) & sFormatRow(2) & "</td>")
                    Next
                    fileWriter.WriteLine("</tr>")
                Next 'row
                fileWriter.WriteLine("</tbody></table>")
                If bDone = False Then
                    fileWriter.WriteLine(msPageBreakHTML)
                    If sHeader IsNot Nothing Then
                        fileWriter.WriteLine(sHeader)
                    End If

                End If
            Loop
        End If
        'MSW 5/6/14 avoid foreign numbers
        My.Application.ChangeCulture(sCurrentCulture)
    End Sub
    Private Function fwStartDoc(ByRef sStatus As String, Optional ByRef sTitle As String = "", Optional ByRef sFileName As String = "") As System.IO.StreamWriter
        '********************************************************************************************
        'Description:  open html file for write, print the header
        '
        'Parameters: status property, title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        ' 11/07/11  MSW     change default file name to use the title if available for pdf print default names
        ' 11/23/11  RJO     Make sure WebBrowser mWebPrint is not visible.
        '********************************************************************************************

        Dim fileWriter As System.IO.StreamWriter = Nothing
        'Dim fileStream As FileStream = Nothing
        mbPageReady = False
        'Delete the old output file
        subDeleteTempFile()

        msPictureOutput = String.Empty
        msPictureTag = String.Empty

        Try
            If mbExportFile Then
                If sFileName = "" Then
                    Const sCSV_EXT As String = "csv"
                    Dim o As New SaveFileDialog
                    With o
                        .FileName = sMakeValidFileName(sTitle)
                        .Filter = msFilter
                        .Title = gcsRM.GetString("csSAVE_FILE_DLG_CAP")
                        .AddExtension = True
                        .CheckPathExists = True
                        .DefaultExt = "." & sCSV_EXT
                        Select Case .ShowDialog()
                            Case DialogResult.OK
                                msWriteFile = .FileName
                            Case Else
                                msWriteFile = String.Empty
                        End Select
                    End With
                    sFileName = msWriteFile
                Else
                    msWriteFile = sFileName
                End If

                Dim sSplit() As String = Split(msWriteFile, ".")

                'user pressed cancel
                If msWriteFile = String.Empty Then
                    Return Nothing
                End If

                If sTitle = String.Empty Then
                    msPageTitle = gpsRM.GetString("psSCREENCAPTION")
                Else
                    msPageTitle = sTitle
                End If
                colTabs = New Collection
                colSharedStrings = New Collection
                Select Case sSplit(sSplit.GetUpperBound(0)).ToLower
                    Case "csv"
                        meExportFormat = eExportFormat.nCSV
                        msDelim = ","
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                    Case "txt"
                        meExportFormat = eExportFormat.nTXT
                        msDelim = vbTab
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                    Case "xml"
                        meExportFormat = eExportFormat.nXML
                        msDelim = vbTab
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                        fileWriter.WriteLine(gcsRM.GetString("csXML_PRINT_HEADER"))
                    Case "xls"
                        meExportFormat = eExportFormat.nXLS
                        msDelim = vbTab
                        msOutputFile = msWriteFile 'Final output file
                        msWriteFile = sGetTmpFileName("XLS_TMP.XML") 'temp file to build spreadsheet
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                        fileWriter.WriteLine(gcsRM.GetString("csXML_PRINT_HEADER"))
                    Case "ods"
                        meExportFormat = eExportFormat.nODS
                        msDelim = vbTab
                        msOutputFile = msWriteFile 'Final output file
                        msTmpFolder = sGetTmpFileName("ODS_TMP") 'temp folder to build the zip structure
                        msTmpFolder = msTmpFolder & "\"
                        IO.Directory.CreateDirectory(msTmpFolder)
                        IO.Directory.CreateDirectory(msTmpFolder & msMETA_INF_FOLDER)
                        msWriteFile = msTmpFolder & msCONTENT_FILE
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                        fileWriter.Write(gcsRM.GetString("csODS_PRINT_HEADER1"))
                        '<style:style style:name="co1" style:family="table-column">
                        '	<style:table-column-properties fo:break-before="auto" style:column-width="0.8925in"/>
                        '</style:style>
                        fileWriter.WriteLine("<style:style style:name=""co1"" style:family=""table-column"">")
                        fileWriter.WriteLine("<style:table-column-properties fo:break-before=""auto"" style:column-width=""1.0000in""/>")
                        fileWriter.WriteLine("</style:style>")
                        If mnColWidths IsNot Nothing Then
                            For nCol As Integer = 0 To mnColWidths.GetUpperBound(0)
                                fileWriter.WriteLine("<style:style style:name=""co" & (nCol + 2).ToString & """ style:family=""table-column"">")
                                fileWriter.WriteLine("<style:table-column-properties fo:break-before=""auto"" style:column-width=""" & mnColWidths(nCol) & "in""/>")
                                fileWriter.WriteLine("</style:style>")
                            Next
                        End If

                        fileWriter.Write(gcsRM.GetString("csODS_PRINT_HEADER2"))
                        msManifestAdd = String.Empty
                    Case "xlsx"
                        meExportFormat = eExportFormat.nXLSX
                        msDelim = vbTab
                        msOutputFile = msWriteFile 'Final output file
                        msTmpFolder = sGetTmpFileName("XLSX_TMP") 'temp folder to build the zip structure
                        msTmpFolder = msTmpFolder & "\"
                        IO.Directory.CreateDirectory(msTmpFolder)
                        IO.Directory.CreateDirectory(msTmpFolder & "docProps")
                        IO.Directory.CreateDirectory(msTmpFolder & "_rels")
                        IO.Directory.CreateDirectory(msTmpFolder & "xl")
                        IO.Directory.CreateDirectory(msTmpFolder & "xl\_rels")
                        IO.Directory.CreateDirectory(msTmpFolder & "xl\theme")
                        IO.Directory.CreateDirectory(msTmpFolder & "xl\worksheets")
                        msManifestAdd = String.Empty
                    Case Else
                        meExportFormat = eExportFormat.nCSV
                        msDelim = ","
                End Select
                mnSheet = 1
            Else
                Const sEXT As String = ".html"
                'Stop anything else going on
                If mWebPrint Is Nothing Then
                    mWebPrint = New WebBrowser
                    frmMain.Controls.Add(mWebPrint)
                    'Make sure we can't see this on the screen 'RJO 11/23/11
                    mWebPrint.Top = mWebPrint.Height * -1
                    mWebPrint.Width = mWebPrint.Left * -1
                    mWebPrint.ScriptErrorsSuppressed = True
                End If

                'Get an available file name
                If sTitle <> String.Empty Then
                    'Use title for the file name to give PDF print drivers a default name.
                    Dim sTmp As String = String.Empty
                    For nChar As Integer = 0 To sTitle.Length - 1
                        Dim chTmp As Char = sTitle.Chars(nChar)
                        If Char.IsLetterOrDigit(chTmp) OrElse (chTmp = "-") Or (chTmp = "_") Then
                            sTmp = sTmp & chTmp
                        End If
                    Next

                    msWriteFile = sGetTmpFileName(sTmp, sEXT)
                Else
                    msWriteFile = sGetTmpFileName(msTempFile, sEXT)
                End If
                msDeleteFile = msWriteFile

                'Start writing to an html file
                fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                'Header
                fileWriter.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")
                fileWriter.WriteLine("<html><head><title>")
                If sTitle = String.Empty Then
                    fileWriter.WriteLine(gpsRM.GetString("psSCREENCAPTION"))
                Else
                    fileWriter.WriteLine(sTitle)
                End If
                fileWriter.WriteLine("</title></head><BODY>")
            End If
            Return fileWriter
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            If mbExportFile Then
                ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)

            Else
                ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)
            End If
            fileWriter.Close()
            Return Nothing
        End Try
    End Function
    Private Function sGetNextLetter(ByRef sLetter As String) As Boolean
        '********************************************************************************************
        'Description:  Increment a letter count
        ' 
        'Parameters: letter string
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If sLetter = "Z" Then
            sLetter = "A"
            Return True
        Else
            sLetter = Chr(Asc(sLetter) + 1)
            Return False
        End If
    End Function
    Private Function bCheckForDateTime(ByRef sText As String, ByRef dtDate As DateTime, _
                                       ByRef bElapsed As Boolean, ByRef bDateOnly As Boolean, _
                                       Optional ByRef nTotalHours As Integer = 0) As Boolean
        Try
            Dim sTmp() As String = Split(sText, ":")
            bElapsed = False
            bDateOnly = False
            If sTmp.GetUpperBound(0) = 2 Then
                'May be time, try a couple options
                If IsNumeric(sTmp(0)) AndAlso IsNumeric(sTmp(1)) AndAlso IsNumeric(sTmp(2)) Then
                    'treat as elapsed time if it's reasonable
                    If (CSng(sTmp(1)) < 60) AndAlso (CSng(sTmp(2)) < 60) Then
                        '<Cell ss:StyleID="s25"><Data ss:Type="DateTime">1899-12-31T00:07:29.000</Data></Cell>
                        nTotalHours = CInt(sTmp(0))
                        Dim nHours As Integer = (nTotalHours Mod 24)
                        Dim nDays As Integer = CInt((nTotalHours - nHours) / 24)
                        dtDate = New DateTime(1899, 12, 31, nHours, CInt(sTmp(1)), CInt(sTmp(2)))
                        '(1899, 12, 31, nHours, CInt(sTmp(1)), CInt(sTmp(2)))
                        dtDate = dtDate.AddDays(nDays)
                        bElapsed = True
                        Return (True)
                    Else
                        'Check for a full date
                        'assuming mm/dd/{yy}yy hh:mm:ss
                        If IsNumeric(sTmp(1)) Then
                            Dim nMin As Integer = CInt(sTmp(1))
                            Dim nSec As Integer = 0
                            Dim nHour As Integer = 0
                            If IsNumeric(sTmp(2)) Then
                                nSec = CInt(sTmp(2))
                            Else
                                If InStr(sTmp(2), "PM", CompareMethod.Text) > 0 Then
                                    nSec = CInt(sTmp(2).Substring(0, 2).Trim)
                                    nHour = 12
                                ElseIf InStr(sTmp(2), "AM", CompareMethod.Text) > 0 Then
                                    nSec = CInt(sTmp(2).Substring(0, 2).Trim)
                                Else
                                    'Don't know what's going on, give up
                                    Return (False)
                                End If
                            End If
                            Dim sTmp2 As String() = Split(sTmp(1), " ")
                            If sTmp2.GetUpperBound(0) = 1 AndAlso IsNumeric(sTmp2(1)) Then
                                nHour = nHour + CInt(sTmp2(1))
                                Dim sTmp3 As String() = Split(sTmp2(0), "/")
                                If sTmp3.GetUpperBound(0) = 2 AndAlso IsNumeric(sTmp3(0)) AndAlso IsNumeric(sTmp3(1)) AndAlso IsNumeric(sTmp3(2)) Then
                                    Dim nMonth As Integer = CInt(sTmp3(0))
                                    Dim nDay As Integer = CInt(sTmp3(1))
                                    Dim nYear As Integer = CInt(sTmp3(2))
                                    If nYear < 50 Then
                                        nYear = nYear + 2000
                                    ElseIf nYear < 100 Then
                                        nYear = nYear + 1900
                                    End If
                                    nTotalHours = nHour
                                    dtDate = New DateTime(nYear, nMonth, nDay, nHour, nMin, nSec)
                                    Return (True)
                                Else
                                    Return (False)
                                End If
                            Else
                                Return (False)
                            End If
                        Else
                            Return (False)
                        End If
                    End If
                Else
                    Return (False)
                End If
            Else
                'Check for Date
                Dim sTmp3 As String() = Split(sText, "/")
                If sTmp3.GetUpperBound(0) = 2 AndAlso IsNumeric(sTmp3(0)) AndAlso IsNumeric(sTmp3(1)) AndAlso IsNumeric(sTmp3(2)) Then
                    Dim nMonth As Integer = CInt(sTmp3(0))
                    Dim nDay As Integer = CInt(sTmp3(1))
                    Dim nYear As Integer = CInt(sTmp3(2))
                    If nYear < 50 Then
                        nYear = nYear + 2000
                    ElseIf nYear < 100 Then
                        nYear = nYear + 1900
                    End If
                    nTotalHours = 0
                    dtDate = New DateTime(nYear, nMonth, nDay, 0, 0, 0)
                    bDateOnly = True
                    Return (True)
                Else
                    Return (False)
                End If
            End If
        Catch ex As Exception
            Return (False)
        End Try

    End Function
    Private Sub subExportCell(ByVal sText As String, ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  Export a cell
        ' 
        'Parameters: string array, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/26/12  MSW     Make subroutines to support export to various file types
        ' 02/27/13  MSW     subExportCell - Get rid of an extra > in the ODS output
        '********************************************************************************************
        mnColCount = mnColCount + 1
        Dim dtDate As DateTime = Nothing
        Dim bElapsed As Boolean = False
        Dim bDateOnly As Boolean = False
        Select Case meExportFormat
            Case eExportFormat.nCSV, eExportFormat.nTXT
                'Swap out commas from inside cells before putting in csv format
                sText = sText.Replace(msDelim, ";")
                If msRowText <> String.Empty Then
                    msRowText = msRowText & msDelim
                End If
                msRowText = msRowText & sText
            Case eExportFormat.nXML, eExportFormat.nXLS
                sText = sText.Replace("<", "&lt;")
                sText = sText.Replace(">", "&gt;")
                sText = sText.Replace("&", "&amp;")
                sText = sText.Replace("""", "&quot;")
                If sText = String.Empty Then
                    sText = " "
                End If
                If IsNumeric(sText) Then
                    'Set numeric data in the spreadsheet
                    '<Cell ss:StyleID="s23"><Data ss:Type="Number">204</Data></Cell>
                    fileWriter.WriteLine("    <Cell ss:StyleID=""s23""><Data ss:Type=""Number"">" & sText.Trim & "</Data></Cell>")
                ElseIf sText <> String.Empty AndAlso sText.Substring(sText.Length - 1) = "%" AndAlso IsNumeric(sText.Substring(0, sText.Length - 1)) Then
                    'Percentage - store as a formatted number
                    '<Cell ss:StyleID="s24"><Data ss:Type="Number">2.0000000000000001E-4</Data></Cell>
                    Dim nTmp As Single = CType(sText.Substring(0, sText.Length - 1), Single) / 100
                    fileWriter.WriteLine("    <Cell ss:StyleID=""s24""><Data ss:Type=""Number"">" & nTmp.ToString & "</Data></Cell>")
                Else
                    If bCheckForDateTime(sText, dtDate, bElapsed, bDateOnly) Then
                        If bElapsed Then
                            fileWriter.WriteLine("    <Cell ss:StyleID=""s25""><Data ss:Type=""DateTime"">" & _
                                                 dtDate.Year.ToString("0000") & "-" & dtDate.Month.ToString("00") & "-" & dtDate.Day.ToString("00") & _
                                                 "T" & dtDate.Hour.ToString("00") & ":" & dtDate.Minute.ToString("00") & ":" & dtDate.Second.ToString("00") & _
                                                 ".000</Data></Cell>")
                        ElseIf bDateOnly Then
                            fileWriter.WriteLine("    <Cell ss:StyleID=""s26""><Data ss:Type=""DateTime"">" & _
                                                 dtDate.Year.ToString("0000") & "-" & dtDate.Month.ToString("00") & "-" & dtDate.Day.ToString("00") & _
                                                 "T" & dtDate.Hour.ToString("00") & ":" & dtDate.Minute.ToString("00") & ":" & dtDate.Second.ToString("00") & _
                                                 ".000</Data></Cell>")
                        Else
                            fileWriter.WriteLine("    <Cell ss:StyleID=""s27""><Data ss:Type=""DateTime"">" & _
                                                 dtDate.Year.ToString("0000") & "-" & dtDate.Month.ToString("00") & "-" & dtDate.Day.ToString("00") & _
                                                 "T" & dtDate.Hour.ToString("00") & ":" & dtDate.Minute.ToString("00") & ":" & dtDate.Second.ToString("00") & _
                                                 ".000</Data></Cell>")
                        End If
                    Else
                        'String output
                        fileWriter.WriteLine("    <Cell><Data ss:Type=""String"">" & sText & "</Data></Cell>")
                    End If
                End If
            Case eExportFormat.nODS
                sText = sText.Replace("<", "&lt;")
                sText = sText.Replace(">", "&gt;")
                sText = sText.Replace("&", "&amp;")
                sText = sText.Replace("""", "&quot;")
                If sText = String.Empty Then
                    sText = " "
                End If
                If IsNumeric(sText) Then
                    fileWriter.Write("    <table:table-cell office:value-type=""float"" office:value=""" & sText.Trim & """><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                ElseIf sText <> String.Empty AndAlso sText.Substring(sText.Length - 1) = "%" AndAlso IsNumeric(sText.Substring(0, sText.Length - 1)) Then
                    'Percentage - store as a formatted number
                    '<table:table-cell table:style-name="ce1" office:value-type="percentage" office:value="0.693"><text:p>69.3%</text:p></table:table-cell>
                    Dim nTmp As Single = CType(sText.Substring(0, sText.Length - 1), Single) / 100
                    fileWriter.Write("    <table:table-cell table:style-name=""ce1"" office:value-type=""percentage"" office:value=""" & nTmp & """><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                Else
                    Dim nTotalHours As Integer = 0
                    If bCheckForDateTime(sText, dtDate, bElapsed, bDateOnly, nTotalHours) Then
                        Dim fTmp As Double = dtDate.ToOADate
                        If bElapsed Then
                            '<table:table-cell table:style-name="ce2" office:value-type="time" office:time-value="PT00H24M25S">
                            '<text:p>00:24:25</text:p>
                            '</table:table-cell>
                            fileWriter.Write("    <table:table-cell table:style-name=""ce2"" office:value-type=""time"" office:time-value=""" & _
                                             "PT" & nTotalHours.ToString & "H" & dtDate.Minute.ToString & "M" & dtDate.Second.ToString & "S" & _
                                             """><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                        ElseIf bDateOnly Then
                            '<table:table-cell table:style-name="ce3" office:value-type="date" office:date-value="2012-04-24">
                            '<text:p>04/24/2012</text:p>
                            '</table:table-cell>
                            fileWriter.Write("    <table:table-cell table:style-name=""ce3"" office:value-type=""date"" office:date-value=""" & _
                                             dtDate.Year.ToString & "-" & dtDate.Month.ToString & "-" & dtDate.Day.ToString & """><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                        Else
                            '<table:table-cell table:style-name="ce4" office:value-type="date" office:date-value="2012-04-30T23:59:59">
                            '	<text:p>04/30/12 11:59:59 PM</text:p>
                            '</table:table-cell>
                            fileWriter.Write("    <table:table-cell table:style-name=""ce4"" office:value-type=""date"" office:date-value=""" & _
                                             dtDate.Year.ToString & "-" & dtDate.Month.ToString & "-" & dtDate.Day.ToString & "T" & _
                                             dtDate.Hour.ToString & ":" & dtDate.Minute.ToString & ":" & dtDate.Second.ToString & _
                                                """><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                        End If
                    Else
                        fileWriter.Write("    <table:table-cell office:value-type=""string""><text:p>" & sText & "</text:p></table:table-cell>" & vbLf)
                    End If
                End If
            Case eExportFormat.nXLSX
                sText = sText.Replace("<", "&lt;")
                sText = sText.Replace(">", "&gt;")
                sText = sText.Replace("&", "&amp;")
                sText = sText.Replace("""", "&quot;")
                If sText = String.Empty Then
                    sText = " "
                End If
                If IsNumeric(sText) Then
                    fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""1""><v>" & sText.Trim & "</v></c>")
                ElseIf sText <> String.Empty AndAlso sText.Substring(sText.Length - 1) = "%" AndAlso IsNumeric(sText.Substring(0, sText.Length - 1)) Then
                    'Percentage - store as a formatted number
                    '<Cell ss:StyleID="s24"><Data ss:Type="Number">2.0000000000000001E-4</Data></Cell>
                    Dim nTmp As Single = CType(sText.Substring(0, sText.Length - 1), Single) / 100
                    fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""7""><v>" & nTmp.ToString & "</v></c>")
                Else
                    If bCheckForDateTime(sText, dtDate, bElapsed, bDateOnly) Then
                        If bElapsed Then
                            dtDate = dtDate.AddDays(-1)
                            Dim fTmp As Double = dtDate.ToOADate
                            fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""4""><v>" & fTmp.ToString & "</v></c>")
                        ElseIf bDateOnly Then
                            Dim fTmp As Double = dtDate.ToOADate
                            fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""6""><v>" & fTmp.ToString & "</v></c>")
                        Else
                            Dim fTmp As Double = dtDate.ToOADate
                            fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""5""><v>" & fTmp.ToString & "</v></c>")
                        End If
                    Else
                        fileWriter.WriteLine("<c r=""" & msColumn & mnRow.ToString & """ s=""1"" t=""s""><v>" & colSharedStrings.Count.ToString & "</v></c>")
                        colSharedStrings.Add(sText)
                    End If
                End If
                If msColumn.Length = 1 Then
                    If sGetNextLetter(msColumn) Then
                        msColumn = "A" & msColumn
                    End If
                Else
                    Dim sLetter As String = msColumn.Substring(1)
                    msColumn = msColumn.Substring(0, 1)
                    If sGetNextLetter(sLetter) Then
                        sGetNextLetter(msColumn)
                    End If
                    msColumn = msColumn & sLetter
                End If
            Case Else
        End Select
    End Sub
    Private Sub subExportRowEnd(ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  Export row end
        ' 
        'Parameters: string array, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/26/12  MSW     Make subroutines to support eport to various file types
        '********************************************************************************************
        Select Case meExportFormat
            Case eExportFormat.nCSV, eExportFormat.nTXT
                'Swap out commas from inside cells before putting in csv format
                fileWriter.WriteLine(msRowText)
            Case eExportFormat.nXML, eExportFormat.nXLS
                fileWriter.WriteLine("  <Cell ss:Index=" & """" & (mnColCount + 1).ToString & """" & "/></Row>")
            Case eExportFormat.nODS
                fileWriter.Write("  </table:table-row>" & vbLf)
            Case eExportFormat.nXLSX
                fileWriter.WriteLine("</row>")
                mnRow = mnRow + 1
            Case Else
        End Select
    End Sub
    Private Sub subExportRowStart(ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  Export row start
        ' 
        'Parameters: string array, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/26/12  MSW     Make subroutines to support eport to various file types
        '********************************************************************************************
        mnColCount = 0
        Select Case meExportFormat
            Case eExportFormat.nCSV, eExportFormat.nTXT
                'Swap out commas from inside cells before putting in csv format
                msRowText = String.Empty
            Case eExportFormat.nXML, eExportFormat.nXLS
                fileWriter.WriteLine("  <Row>")
            Case eExportFormat.nODS
                fileWriter.Write("  <table:table-row table:style-name=""ro1"">" & vbLf)
            Case eExportFormat.nXLSX
                fileWriter.WriteLine("<row r=""" & mnRow.ToString & """ spans=""1:1"">")
                msColumn = "A"
            Case Else
        End Select
    End Sub

    Private Sub subExportTableEnd(ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  Export row end
        ' 
        'Parameters: string array, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/26/12  MSW     Make subroutines to support eport to various file types
        '********************************************************************************************
        Select Case meExportFormat
            Case eExportFormat.nCSV, eExportFormat.nTXT
                'Swap out commas from inside cells before putting in csv format
                fileWriter.WriteLine("")
            Case eExportFormat.nXML, eExportFormat.nXLS
                fileWriter.WriteLine("</Table><x:WorksheetOptions/></ss:Worksheet>")
            Case eExportFormat.nODS
                fileWriter.Write("</table:table>" & vbLf)
            Case eExportFormat.nXLSX
                fileWriter.WriteLine(gcsRM.GetString("csXLSX_SHEET_END"))
                fileWriter.Close()
            Case Else
        End Select
    End Sub
    Private Sub subExportTableStart(ByRef sTableName As String, ByRef fileWriter As System.IO.StreamWriter)
        '********************************************************************************************
        'Description:  Export row start
        ' 
        'Parameters: string array, filewriter object
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/26/12  MSW     Make subroutines to support eport to various file types
        '********************************************************************************************
        'mnColCount = nColCount
        Select Case meExportFormat
            Case eExportFormat.nCSV, eExportFormat.nTXT
                'Don't care
            Case eExportFormat.nXML, eExportFormat.nXLS
                fileWriter.WriteLine("<ss:Worksheet ss:Name=""" & sTableName & """><Table ss:StyleID=""ta1"">")
                If mnColWidths IsNot Nothing AndAlso mnColWidthIndex IsNot Nothing Then
                    For nCol As Integer = 0 To mnColWidthIndex.GetUpperBound(0)
                        If (mnColWidthIndex(nCol) >= 0) AndAlso (mnColWidthIndex(nCol) <= mnColWidths.GetUpperBound(0)) AndAlso _
                                (mnColWidths(mnColWidthIndex(nCol)) > 0) Then
                            If nCol = 0 Then
                                fileWriter.WriteLine("<Column ss:Width=""" & (mnColWidths(mnColWidthIndex(nCol)) * 50).ToString & """/>")
                            Else
                                fileWriter.WriteLine("<Column ss:Index=""" & (nCol + 1).ToString & """ ss:Width=""" & _
                                                     (mnColWidths(mnColWidthIndex(nCol)) * 70).ToString & """/>")
                            End If
                        End If
                    Next
                End If
            Case eExportFormat.nODS
                fileWriter.Write("<table:table table:name=""" & sTableName & """ table:style-name=""ta1"">" & vbLf)
                '<table:table-column table:style-name="co1" table:default-cell-style-name="Default"/>
                '<table:table-column table:style-name="co2" table:default-cell-style-name="Default"/>
                '<table:table-column table:style-name="co2" table:default-cell-style-name="Default"/>
                '<table:table-column table:style-name="co3" table:default-cell-style-name="Default"/>
                If mnColWidths IsNot Nothing AndAlso mnColWidthIndex IsNot Nothing Then
                    For nCol As Integer = 0 To mnColWidthIndex.GetUpperBound(0)
                        If (mnColWidthIndex(nCol) >= 0) AndAlso (mnColWidthIndex(nCol) <= mnColWidths.GetUpperBound(0))  Then
                            fileWriter.WriteLine("<table:table-column table:style-name=""co" & (mnColWidthIndex(nCol) + 2).ToString & """ table:default-cell-style-name=""Default""/>")
                        Else
                            fileWriter.WriteLine("<table:table-column table:style-name=""co1"" table:default-cell-style-name=""Default""/>")
                        End If
                    Next
                End If
            Case eExportFormat.nXLSX
                msColumn = "A"
                mnRow = 1
                msWriteFile = msTmpFolder & "xl\worksheets\sheet" & (colTabs.Count + 1).ToString & ".xml"
                fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msWriteFile, False)
                fileWriter.WriteLine(gcsRM.GetString("csXLSX_SHEET_START"))
                Dim bCols As Boolean = False
                If mnColWidths IsNot Nothing AndAlso mnColWidthIndex IsNot Nothing Then
                    For nCol As Integer = 0 To mnColWidthIndex.GetUpperBound(0)
                        If (mnColWidthIndex(nCol) >= 0) AndAlso (mnColWidthIndex(nCol) <= mnColWidths.GetUpperBound(0)) AndAlso _
                                (mnColWidths(mnColWidthIndex(nCol)) > 0) Then
                            If bCols = False Then
                                fileWriter.WriteLine("<cols>")
                                bCols = True
                            End If
                            fileWriter.WriteLine("<col min=""" & (nCol + 1).ToString & """ max=""" & (nCol + 1).ToString & """ width=""" & _
                                 (mnColWidths(mnColWidthIndex(nCol)) * 13).ToString & """ bestFit=""1"" customWidth=""1""/>")
                        End If
                    Next
                    If bCols Then
                        fileWriter.WriteLine("</cols>")
                    End If
                End If
                fileWriter.WriteLine("<sheetData>")

            Case Else

        End Select
        colTabs.Add(sTableName)
    End Sub
    Public Sub subAddPicture(ByRef sFileName As String, ByRef sStatus As String, _
                              Optional ByRef sTitle() As String = Nothing, _
                              Optional ByRef sSubTitle() As String = Nothing, _
                              Optional ByVal sSheetName As String = "", _
                              Optional ByVal sLeft As String = "", _
                              Optional ByVal s_Top As String = "", _
                              Optional ByVal sWidth As String = "", _
                              Optional ByVal sHeight As String = "")
        '********************************************************************************************
        'Description:  Write the title table/header for standard printouts
        '
        'Parameters: {Rich}TextBox, status property, page title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/01/12  MSW     Add print features for DMON Viewer
        '********************************************************************************************
        Dim sHeader As String = String.Empty
        If sLeft = String.Empty Then
            sLeft = "A"
        End If
        If s_Top = String.Empty Then
            s_Top = "5"
        End If
        If sWidth = String.Empty Then
            sWidth = "M"
        End If
        If sHeight = String.Empty Then
            sHeight = "28"
        End If
        If mbExportFile Then
            Select Case meExportFormat
                Case eExportFormat.nODS
                    Dim sSplitFilename() As String = Split(sFileName, "\")
                    Dim sFile As String = sSplitFilename(sSplitFilename.GetUpperBound(0))
                    IO.Directory.CreateDirectory(msTmpFolder & "Pictures")
                    IO.File.Copy(sFileName, msTmpFolder & "Pictures\" & sFile)
                    subWriteTitles(sHeader, sStatus, sTitle, sSubTitle, sSheetName)
                    subExportRowStart(mFileWriter)
                    '<table:table-row table:style-name="ro1">
                    mFileWriter.Write("    <table:table-cell>" & vbLf)
                    '	<table:table-cell>

                    mFileWriter.Write("      <draw:frame table:end-cell-address=""Chart." & sWidth & sHeight & """ table:end-x=""0.4626in"" table:end-y=""0.072in"" draw:z-index=""0"" draw:name=""Graphics 1"" draw:style-name=""gr1"" draw:text-style-name=""P1"" svg:width=""10.000in"" svg:height=""3.6807in"" svg:x=""0in"" svg:y=""0in"">" & vbLf)
                    '<draw:frame table:end-cell-address="Chart.Q31" table:end-x="0.4626in" table:end-y="0.072in" draw:z-index="0" draw:name="Graphics 1" draw:style-name="gr1" svg:width="14.687in" svg:height="4.6949in" svg:x="0in" svg:y="0in">
                    '		<draw:frame table:end-cell-address="Chart.Q31" table:end-x="0.4626in" table:end-y="0.072in" draw:z-index="0" draw:name="Graphics 1" draw:style-name="gr1" draw:text-style-name="P1" svg:width="14.687in" svg:height="5.4059in" svg:x="0in" svg:y="0in">
                    mFileWriter.Write("        <draw:image xlink:href=""Pictures/" & sFile & """ xlink:type=""simple"" xlink:show=""embed"" xlink:actuate=""onLoad"">" & vbLf)
                    '			<draw:image xlink:href="Pictures/100000000000058200000207F5E6E7C4.jpg" xlink:type="simple" xlink:show="embed" xlink:actuate="onLoad">
                    mFileWriter.Write("          <text:p/>" & vbLf)
                    '				<text:p/>
                    mFileWriter.Write("        </draw:image>" & vbLf)
                    '			</draw:image>
                    mFileWriter.Write("      </draw:frame>" & vbLf)
                    '		</draw:frame>
                    mFileWriter.Write("    </table:table-cell>" & vbLf)
                    '	</table:table-cell>
                    subExportRowEnd(mFileWriter)
                    '</table:table-row>
                    subExportTableEnd(mFileWriter)
                    '</table:table>
                    msManifestAdd = msManifestAdd & "<manifest:file-entry manifest:media-type=""image/jpeg"" manifest:full-path=""Pictures/" & sFile & """/>" & vbLf
                    '<manifest:file-entry manifest:media-type="image/jpeg" manifest:full-path="Pictures/ChartPic3.jpg"/>
                    msManifestAdd = msManifestAdd & "<manifest:file-entry manifest:media-type="""" manifest:full-path=""Pictures/""/>" & vbLf
                    '<manifest:file-entry manifest:media-type="" manifest:full-path="Pictures/"/>
                Case eExportFormat.nXLS
                    subWriteTitles(sHeader, sStatus, sTitle, sSubTitle, sSheetName)
                    subExportTableEnd(mFileWriter)
                    msPictureOutput = sFileName
                    msPictureTag = sLeft & s_Top '"A" & (CType((sTitle.GetUpperBound(0) + sSubTitle.GetUpperBound(0) / mnSubTitleCols), Integer) + 1).ToString
                Case eExportFormat.nXLSX
                    subWriteTitles(sHeader, sStatus, sTitle, sSubTitle, sSheetName)
                    'subExportTableEnd(mFileWriter)
                    mFileWriter.WriteLine(gcsRM.GetString("csXLSX_SHEET_DRAWING_END"))
                    mFileWriter.Close()

                    Dim nLeft As Integer = 0
                    Dim nTop As Integer = 5
                    Dim nWidth As Integer = 6
                    Dim nHeight As Integer = 32
                    Try
                        If IsNumeric(s_Top) Then
                            nTop = CInt(s_Top) - 1
                        End If
                    Catch ex As Exception
                    End Try
                    Try
                        If IsNumeric(sHeight) Then
                            nHeight = CInt(sHeight) - 1
                        End If
                    Catch ex As Exception
                    End Try
                    Try
                        nLeft = Asc(sLeft) - Asc("A")
                    Catch ex As Exception
                    End Try
                    Try
                        nWidth = Asc(sWidth) - Asc("A")
                    Catch ex As Exception
                    End Try
                    msPictureTag = String.Format(gcsRM.GetString("csXLSX_DRAWINGS_DRAWINGS"), nLeft.ToString, nTop.ToString, nWidth.ToString, nHeight.ToString)

                    Dim sSplitFilename() As String = Split(sFileName, "\")
                    msPictureOutput = sSplitFilename(sSplitFilename.GetUpperBound(0))
                    IO.Directory.CreateDirectory(msTmpFolder & "xl\media")
                    IO.File.Copy(sFileName, msTmpFolder & "xl\media\" & msPictureOutput)
                Case Else
                    Exit Sub
            End Select
        Else
            subWriteTitles(sHeader, sStatus, sTitle, sSubTitle, sSheetName)
            Dim sTmp As String = mPWCommon.sMakeValidFileName(sFileName)
            Dim sSplit() As String = Split(sTmp, ".")
            Dim sExt As String = sSplit(sSplit.GetUpperBound(0))
            Dim sCopy As String = sGetTmpFileName(sTmp, sExt)
            IO.File.Copy(sFileName, sCopy)
            sSplit = Split(sCopy, "\")
            mFileWriter.WriteLine("<IMG SRC=" & sSplit(sSplit.GetUpperBound(0)) & ">")
            '<IMG SRC="T10_html_ma19183c.jpg">
        End If
    End Sub
    Public Sub subWriteTitles(ByRef sHeader As String, ByRef sStatus As String, _
                                      Optional ByRef sTitle() As String = Nothing, _
                                      Optional ByRef sSubTitle() As String = Nothing, _
                                      Optional ByVal sSheetName As String = "")
        '********************************************************************************************
        'Description:  Write the title table/header for standard printouts
        '
        'Parameters: {Rich}TextBox, status property, page title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/01/12  MSW     Add print features for DMON Viewer
        '********************************************************************************************

        If mbExportFile Then
            If sSheetName = "" Then
                sSheetName = "Sheet" & mnSheet
            End If
            subExportTableStart(sSheetName, mFileWriter)

            subExportRowStart(mFileWriter)
            subExportCell(msPageTitle, mFileWriter)
            subExportRowEnd(mFileWriter)

            mnSheet = mnSheet + 1
            If sTitle IsNot Nothing Then
                subExportRowStart(mFileWriter)
                For Each sTmp As String In sTitle
                    'Swap out commas from inside cells before putting in csv format
                    subExportCell(sTmp.Replace(msDelim, ";"), mFileWriter)
                Next
                subExportRowEnd(mFileWriter)
            End If
            If sSubTitle IsNot Nothing Then
                Dim sLine As String = String.Empty
                Dim nCol As Integer = 0
                For Each sTmp As String In sSubTitle
                    If sTmp Is Nothing Then
                        sTmp = String.Empty
                    End If
                    If nCol = 0 Then
                        subExportRowStart(mFileWriter)
                    End If
                    subExportCell(sTmp.Replace(msDelim, ";"), mFileWriter)
                    nCol += 1
                    If nCol = mnSubTitleCols Then
                        subExportRowEnd(mFileWriter)
                        nCol = 0
                    End If
                Next
                If nCol > 0 Then
                    subExportRowEnd(mFileWriter)
                End If
            End If
        Else
            'sHeader - save the header lines for multipage tables
            If sTitle IsNot Nothing Then
                For Each sTmp As String In sTitle
                    sHeader = sHeader & "<H2>" & sTmp & "</H2>"
                Next
            End If
            If sSubTitle IsNot Nothing Then
                If sSubTitle.Length > 3 Then 'Clean up crowded subtitles
                    Dim nCol As Integer = 0
                    sHeader = sHeader & "<table border=""0"" cellspacing=""0""><tbody><tr>"
                    For Each sTmp As String In sSubTitle
                        If nCol = 3 Then
                            nCol = 0
                            sHeader = sHeader & "</TR><TR>"
                        End If
                        sHeader = sHeader & "<TH>" & sTmp & "&nbsp;&nbsp;</TH>"
                        nCol += 1
                    Next
                    sHeader = sHeader & "</TR></tbody></table>"
                Else
                    For Each sTmp As String In sSubTitle
                        sHeader = sHeader & "<H3>" & sTmp & "</H3>"
                    Next
                End If
            End If
        End If
        If sHeader <> String.Empty Then
            mFileWriter.Write(sHeader) ' Write once here for any kind of page object
        End If
    End Sub

    Public Sub subAddObject(ByVal oObject As Object, ByRef sStatus As String, _
                                  Optional ByRef sTitle() As String = Nothing, _
                                  Optional ByRef sSubTitle() As String = Nothing, _
                                  Optional ByVal sSheetName As String = "")
        '********************************************************************************************
        'Description:  build a simple html document from a vb object
        '           Currently supported objects: textbox, richtextbox, string, string array, datagridview, datatable
        '
        'Parameters: {Rich}TextBox, status property, page title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        Dim sHeader As String = String.Empty
        Try
            subWriteTitles(sHeader, sStatus, sTitle, sSubTitle, sSheetName)
            'Print object if it's supported
            If (TypeOf oObject Is RichTextBox) Then
                If mbDECODE_RTF And (mbExportFile = False) Then
                    'call a sub to translate some basic rtf tags to html
                    subPrintRtfToHtml(DirectCast(oObject, RichTextBox), mFileWriter)
                Else
                    Dim oRTF As RichTextBox = DirectCast(oObject, RichTextBox)
                    If mbExportFile Then
                        'Not really a csv this way, but it'll save something
                        'Dump the textbox text into the file
                        For nLine As Integer = 0 To oRTF.Lines.GetUpperBound(0)
                            subExportRowStart(mFileWriter)
                            subExportCell(oRTF.Lines(nLine), mFileWriter)
                            subExportRowEnd(mFileWriter)
                        Next
                    Else
                        '<PRE> tells it to display the following text like a text editor
                        mFileWriter.WriteLine("<PRE>")
                        'Dump the textbox text into the file
                        For nLine As Integer = 0 To oRTF.Lines.GetUpperBound(0)
                            mFileWriter.WriteLine(oRTF.Lines(nLine))
                        Next
                        mFileWriter.WriteLine("</PRE>")
                    End If
                End If
            ElseIf (TypeOf oObject Is TextBox) Then
                Dim oTB As TextBox = DirectCast(oObject, TextBox)
                ''''''''''Print plain text
                If mbExportFile Then
                    'Not really a csv this way, but it'll save something
                    'Dump the textbox text into the file
                    For nLine As Integer = 0 To oTB.Lines.GetUpperBound(0)
                        subExportRowStart(mFileWriter)
                        subExportCell(oTB.Lines(nLine), mFileWriter)
                        subExportRowEnd(mFileWriter)
                    Next
                Else
                    '<PRE> tells it to display the following text like a text editor
                    mFileWriter.WriteLine("<PRE>")
                    'Dump the textbox text into the file
                    For nLine As Integer = 0 To oTB.Lines.GetUpperBound(0)
                        mFileWriter.WriteLine(oTB.Lines(nLine))
                    Next
                    mFileWriter.WriteLine("</PRE>")
                End If
            ElseIf (TypeOf oObject Is DataGridView) Then
                'call a sub to build a table
                subPrintDgToHtml(DirectCast(oObject, DataGridView), mFileWriter, sHeader)
            ElseIf (TypeOf oObject Is DataTable) Then
                'call a sub to build a table
                subPrintDataTableToHtml(DirectCast(oObject, DataTable), mFileWriter, sHeader)
            ElseIf (TypeOf oObject Is String) Then
                If mbExportFile Then
                    'Not really a csv this way, but it'll save something
                    subExportRowStart(mFileWriter)
                    subExportCell(oObject.ToString, mFileWriter)
                    subExportRowEnd(mFileWriter)
                Else
                    '<PRE> tells it to display the following text like a text editor
                    mFileWriter.WriteLine("<PRE>")
                    mFileWriter.Write(oObject.ToString)
                    mFileWriter.WriteLine("</PRE>")
                End If
            ElseIf (TypeOf oObject Is String()) Then
                'call a sub to build a table or write plain text, depending on the content.
                subPrintStringArrayToHtml(DirectCast(oObject, String()), mFileWriter, sHeader)
            End If
            If mbExportFile Then
                subExportTableEnd(mFileWriter)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            If mbExportFile Then
                ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)

            Else
                ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)
            End If
        End Try
    End Sub
    Public Sub subAddParagraph(ByRef sStatus As String, _
                               Optional ByVal sText As String = "&nbsp;", _
                               Optional ByVal sFormatString As String() = Nothing)
        '********************************************************************************************
        'Description:  Add a paragraph to the output file
        '
        'Parameters: text
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        If mbExportFile Then
            mFileWriter.WriteLine("")
        Else
            mFileWriter.WriteLine("<p>")
            If sFormatString IsNot Nothing AndAlso sFormatString.GetUpperBound(0) > 0 Then
                mFileWriter.WriteLine("<" & sFormatString(0) & ">")
                mFileWriter.Write(sText)
                mFileWriter.WriteLine("<" & sFormatString(1) & ">")
            Else
                mFileWriter.Write(sText)
            End If
            mFileWriter.WriteLine("</p>")
        End If
    End Sub
    Public Sub subAddHeading(ByRef sStatus As String, _
                             Optional ByVal nHeading As Integer = 1, _
                               Optional ByVal sText As String = "&nbsp;", _
                               Optional ByVal sFormatString As String() = Nothing)
        '********************************************************************************************
        'Description:  Add a paragraph to the output file
        '
        'Parameters: text
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        If mbExportFile Then
            mFileWriter.WriteLine(sText)
        Else
            mFileWriter.WriteLine("<H" & nHeading.ToString & ">")
            If sFormatString IsNot Nothing AndAlso sFormatString.GetUpperBound(0) > 0 Then
                mFileWriter.WriteLine("<" & sFormatString(0) & ">")
                mFileWriter.Write(sText)
                mFileWriter.WriteLine("<" & sFormatString(1) & ">")
            Else
                mFileWriter.Write(sText)
            End If
            mFileWriter.WriteLine("</H" & nHeading.ToString & ">")
        End If
    End Sub
    Public Sub subAddPageBreak(ByRef sStatus As String)
        '********************************************************************************************
        'Description:  Add a paragraph to the output file
        '
        'Parameters: text
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        If mbExportFile Then
            Select Case meExportFormat
                Case eExportFormat.nCSV, eExportFormat.nTXT
                    mFileWriter.WriteLine(String.Empty)
                    mFileWriter.WriteLine(String.Empty)
                    mFileWriter.WriteLine(String.Empty)
                Case Else
            End Select
        Else
            mFileWriter.WriteLine(msPageBreakHTML)
        End If
    End Sub
    Public Sub subCloseFile(ByRef sStatus As String, Optional ByRef FileWriter As System.IO.StreamWriter = Nothing)
        '********************************************************************************************
        'Description:  open html file for write, print the header
        '
        'Parameters: status property, file
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************

        Try
            If FileWriter Is Nothing Then
                FileWriter = mFileWriter
            End If
            If mbExportFile = False Then
                FileWriter.WriteLine("</BODY></HTML>")
                FileWriter.Close()
            Else
                Select Case meExportFormat
                    Case eExportFormat.nCSV, eExportFormat.nTXT
                        FileWriter.Close()
                    Case eExportFormat.nXML
                        FileWriter.WriteLine("</Workbook>")
                        FileWriter.Close()
                    Case eExportFormat.nXLS
                        FileWriter.WriteLine("</Workbook>")
                        FileWriter.Close()
                        Dim sCmd As String = String.Empty
                        If msPictureOutput = String.Empty Or msPictureTag = String.Empty Then
                            sCmd = msExcelUtil & " " & gs_XML_TO_XLS & " """ & msWriteFile & """ """ & msOutputFile & """"
                        Else
                            sCmd = msExcelUtil & " " & gs_XML_TO_XLS_PIC & " """ & msWriteFile & """ """ & msOutputFile & """ """ & msPictureTag & """ """ & msPictureOutput & """"
                        End If
                        'Shell out to excel converter
                        Shell(sCmd, AppWinStyle.Hide, True, 30000)
                        File.Delete(msWriteFile)
                        If Not (IO.File.Exists(msOutputFile)) Then
                            Dim sTmp As String = gcsRM.GetString("csFAILED_XML_XLS")
                            Dim sPath As String = String.Empty
                            If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_EXCEL_UTIL_ERRLOG)) Then
                                If IO.File.Exists(sPath) Then
                                    sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                                End If
                            End If
                            MessageBox.Show(sTmp, gcsRM.GetString("csEXPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If
                    Case eExportFormat.nODS
                        FileWriter.WriteLine("</office:spreadsheet>")
                        FileWriter.WriteLine("</office:body>")
                        FileWriter.WriteLine("</office:document-content>")
                        FileWriter.Close()
                        'Add support files.
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & msSETTINGS_FILE, False, Encoding.UTF8)
                        FileWriter.WriteLine(gcsRM.GetString("csODS_SETTINGS_XML"))
                        FileWriter.Close()
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & msMIME_TYPE_FILE, False, Encoding.UTF7)
                        FileWriter.Write("application/vnd.oasis.opendocument.spreadsheet")
                        FileWriter.Close()
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & msMETA_FILE, False, Encoding.UTF8)
                        FileWriter.WriteLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
                        FileWriter.WriteLine("<office:document-meta xmlns:office=""urn:oasis:names:tc:opendocument:xmlns:office:1.0"" xmlns:xlink=""http://www.w3.org/1999/xlink"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:meta=""urn:oasis:names:tc:opendocument:xmlns:meta:1.0"" xmlns:ooo=""http://openoffice.org/2004/office"" xmlns:grddl=""http://www.w3.org/2003/g/data-view#"" office:version=""1.2"" grddl:transformation=""http://docs.oasis-open.org/office/1.2/xslt/odf2rdf.xsl"">")
                        FileWriter.WriteLine("	<office:meta>")
                        FileWriter.WriteLine("	</office:meta>")
                        FileWriter.WriteLine("</office:document-meta>")
                        FileWriter.Close()
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & msSTYLES_FILE, False, Encoding.UTF8)
                        FileWriter.WriteLine(gcsRM.GetString("csODS_STYLES_XML"))
                        FileWriter.Close()
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & msMETA_INF_FOLDER & msMANIFEST_FILE, False)
                        FileWriter.WriteLine(gcsRM.GetString("csODS_META_INF_MANIFEST_XML1"))
                        FileWriter.WriteLine(msManifestAdd)
                        FileWriter.WriteLine(gcsRM.GetString("csODS_META_INF_MANIFEST_XML2"))
                        FileWriter.Close()

                        Try
                            If File.Exists(msOutputFile) Then
                                My.Computer.FileSystem.DeleteFile(msOutputFile)
                            End If
                            Dim sCmd As String = msZipUtil & " " & gs_ZIP_ALL & " " & """" & msTmpFolder.Substring(0, msTmpFolder.Length - 1) & """" & "  " & """" & msOutputFile & """"
                            'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                            Shell(sCmd, AppWinStyle.Hide, True, 30000)
                            My.Computer.FileSystem.DeleteDirectory(msTmpFolder, FileIO.DeleteDirectoryOption.DeleteAllContents)
                            If Not (IO.File.Exists(msOutputFile)) Then
                                Dim sTmp As String = gcsRM.GetString("csFAILED_SAVE_ODS")
                                Dim sPath As String = String.Empty
                                If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_ERRLOG)) Then
                                    If IO.File.Exists(sPath) Then
                                        sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                                    End If
                                End If
                                MessageBox.Show(sTmp, gcsRM.GetString("csEXPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        Catch ex As Exception
                            Trace.WriteLine(ex.Message)
                            Trace.WriteLine(ex.StackTrace)
                            ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)

                            FileWriter.Close()
                        End Try
                    Case eExportFormat.nXLSX

                        'Support Files
                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "docProps\core.xml", False)
                        Dim sT1 As String = Now.ToUniversalTime.Year.ToString("00") & "-" & _
                                            Now.ToUniversalTime.Month.ToString("00") & "-" & _
                                            Now.ToUniversalTime.Day.ToString("00") & "T" & _
                                            Now.ToUniversalTime.Hour.ToString("00") & ":" & _
                                            Now.ToUniversalTime.Minute.ToString("00") & ":" & _
                                            Now.ToUniversalTime.Second.ToString("00") & "Z"
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_CORE"), sT1, sT1))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "docProps\app.xml", False)
                        Dim sTabs As String = String.Empty
                        For Each sTmp As String In colTabs
                            sTabs = sTabs & String.Format(gcsRM.GetString("csXLSX_APP_1"), sTmp)
                        Next
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_APP"), colTabs.Count.ToString, colTabs.Count.ToString, sTabs))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "_rels\.rels", False)
                        FileWriter.Write(gcsRM.GetString("csXLSX_RELS1"))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\theme\theme1.xml", False)
                        FileWriter.Write(gcsRM.GetString("csXLSX_THEME"))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\_rels\workbook.xml.rels", False)
                        FileWriter.Write(gcsRM.GetString("csXLSX_WB_XML_RELS_1"))
                        Dim nItem As Integer = 1
                        For Each sTab As String In colTabs
                            FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB_XML_RELS_SHEET"), nItem.ToString, nItem.ToString))
                            nItem = nItem + 1
                        Next
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB_XML_RELS_THEME"), nItem.ToString))
                        nItem = nItem + 1
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB_XML_RELS_STYLES"), nItem.ToString))
                        nItem = nItem + 1
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB_XML_RELS_SHARED"), nItem.ToString))
                        FileWriter.Write(gcsRM.GetString("csXLSX_WB_XML_RELS_2"))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\workbook.xml", False)
                        sTabs = String.Empty
                        nItem = 1
                        For Each sTab As String In colTabs
                            sTabs = sTabs & String.Format(gcsRM.GetString("csXLSX_WB_SHEET"), sTab, nItem.ToString, nItem.ToString)
                            nItem = nItem + 1
                        Next
                        If colTabs.Count = 1 Then
                            FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB1TAB"), sTabs))

                        Else
                            FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_WB"), sTabs))
                        End If
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\styles.xml", False)
                        FileWriter.Write(gcsRM.GetString("csXLSX_STYLES"))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\sharedStrings.xml", False)
                        sTabs = String.Empty
                        FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_SHAREDSTRING_1"), colSharedStrings.Count.ToString, colSharedStrings.Count.ToString))
                        For Each sString As String In colSharedStrings
                            If sString.Trim = String.Empty Then
                                FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_SHAREDSTRING_ITEM_WS"), sString))
                            Else
                                FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_SHAREDSTRING_ITEM"), sString))
                            End If
                        Next
                        FileWriter.Write(gcsRM.GetString("csXLSX_SHAREDSTRING_2"))
                        FileWriter.Close()

                        FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "[Content_Types].xml", False)
                        sTabs = String.Empty
                        FileWriter.Write(gcsRM.GetString("csXLSX_CONTENT_1"))
                        FileWriter.Write(gcsRM.GetString("csXLSX_CONTENT_PNG"))
                        For nTab As Integer = 1 To colTabs.Count
                            FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_CONTENT_ITEMS"), nTab.ToString))
                        Next
                        If msPictureOutput <> String.Empty And msPictureTag <> String.Empty Then
                            FileWriter.Write(gcsRM.GetString("csXLSX_CONTENT_DRAWING"))
                        End If
                        FileWriter.Write(gcsRM.GetString("csXLSX_CONTENT_2"))
                        FileWriter.Close()

                        If msPictureOutput <> String.Empty And msPictureTag <> String.Empty Then

                            IO.Directory.CreateDirectory(msTmpFolder & "xl\worksheets\_rels")
                            IO.Directory.CreateDirectory(msTmpFolder & "xl\drawings")
                            IO.Directory.CreateDirectory(msTmpFolder & "xl\drawings\_rels")

                            FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\worksheets\_rels\sheet1.xml.rels", False)
                            FileWriter.Write(gcsRM.GetString("csXLSX_SHEETS_RELS"))
                            FileWriter.Close()

                            FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\drawings\_rels\drawing1.xml.rels", False)
                            FileWriter.Write(String.Format(gcsRM.GetString("csXLSX_DRAWINGS_RELS"), "../media/" & msPictureOutput))
                            FileWriter.Close()

                            FileWriter = My.Computer.FileSystem.OpenTextFileWriter(msTmpFolder & "xl\drawings\drawing1.xml", False)
                            FileWriter.Write(msPictureTag)
                            FileWriter.Close()

                        End If


                        Try
                            If File.Exists(msOutputFile) Then
                                My.Computer.FileSystem.DeleteFile(msOutputFile)
                            End If
                            Dim sCmd As String = msZipUtil & " " & gs_ZIP_ALL & " " & """" & msTmpFolder.Substring(0, msTmpFolder.Length - 1) & """" & "  " & """" & msOutputFile & """"
                            'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                            Shell(sCmd, AppWinStyle.Hide, True, 30000)
                            My.Computer.FileSystem.DeleteDirectory(msTmpFolder, FileIO.DeleteDirectoryOption.DeleteAllContents)
                            If Not (IO.File.Exists(msOutputFile)) Then
                                Dim sTmp As String = gcsRM.GetString("csFAILED_SAVE_XLSX")
                                Dim sPath As String = String.Empty
                                If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_ERRLOG)) Then
                                    If IO.File.Exists(sPath) Then
                                        sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                                    End If
                                End If
                                MessageBox.Show(sTmp, gcsRM.GetString("csEXPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        Catch ex As Exception
                            Trace.WriteLine(ex.Message)
                            Trace.WriteLine(ex.StackTrace)
                            ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)

                            FileWriter.Close()
                        End Try

                End Select
            End If
            Application.DoEvents()
            If mbExportFile = False Then
                mWebPrint.Url = New Uri(msWriteFile)
                'Wait for the browser to load the file.
                'The navigate event calls the browser's print, print dialog, or print preview routine
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            If mbExportFile Then
                ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)

            Else
                ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)
            End If
            FileWriter.Close()
        End Try
    End Sub

    Friend Sub subStartDoc(ByRef sStatus As String, Optional ByRef sPageTitle As String = "", _
                                Optional ByVal bExportFile As Boolean = False, _
                                Optional ByRef bCancelled As Boolean = False, _
                                Optional ByRef sFileName As String = "", _
                                Optional ByRef oFileType As eExportFormat = eExportFormat.nNONE)
        '********************************************************************************************
        'Description:  Start a document
        '
        'Parameters: page title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        mbPrintBusy = True
        mbPageReady = False
        mPrintTask = ePrintTask.None
        mbExportFile = bExportFile
        Try
            mFileWriter = fwStartDoc(sStatus, sPageTitle, sFileName)
            oFileType = meExportFormat
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            If mbExportFile Then
                ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)

            Else
                ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)
            End If
            If mFileWriter IsNot Nothing Then
                mFileWriter.Close()
            End If

        End Try
        If mFileWriter Is Nothing And oFileType <> eExportFormat.nXLSX Then
            bCancelled = True
        End If
        mbPrintBusy = False
    End Sub

    Friend Sub subCreateSimpleDoc(ByVal oObject As Object, ByRef sStatus As String, _
                                  Optional ByRef sPageTitle As String = "", _
                                  Optional ByRef sTitle() As String = Nothing, _
                                  Optional ByRef sSubTitle() As String = Nothing, _
                                  Optional ByVal bExportFile As Boolean = False, _
                                  Optional ByRef bCancelled As Boolean = False, _
                                  Optional ByRef sFileName As String = "")
        '********************************************************************************************
        'Description:  build a simple html document from a vb object
        '           Currently supported objects: textbox, richtextbox, string, string array, datagridview, datatable

        '
        'Parameters: {Rich}TextBox, status property, page title
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/10  MSW     Use print routines to support export to *.csv - mbExportFile
        '********************************************************************************************
        mbPrintBusy = True
        mbPageReady = False
        mPrintTask = ePrintTask.None
        subStartDoc(sStatus, sPageTitle, bExportFile, bCancelled, sFileName)
        If bCancelled Then
            Exit Sub
        End If
        Try
            If mFileWriter IsNot Nothing Or ((msTmpFolder <> "") And (meExportFormat = eExportFormat.nXLSX)) Then
                subAddObject(oObject, sStatus, sTitle, sSubTitle, msPageTitle)
                subCloseFile(sStatus, mFileWriter)
            Else
                bCancelled = True
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            If mbExportFile Then
                ShowErrorMessagebox(gcsRM.GetString("csEXPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)

            Else
                ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    sStatus, MessageBoxButtons.OK)
            End If
            mFileWriter.Close()
        End Try
        mbPrintBusy = False
    End Sub
    Private Sub subRunPrintTask()
        '********************************************************************************************
        'Description:  Web page is loaded, select which print task to run
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/31/11  MSW     remove the size and visible settings.  It seems to work without them and it
        '                   they were causing the main form to resize.
        '********************************************************************************************
        If mWebPrint Is Nothing Then
            Exit Sub
        End If
        mbPrintBusy = True
        'mWebPrint.Size = New Size(800, 600)
        'mWebPrint.Visible = True
        Select Case mPrintTask
            Case ePrintTask.Preview
                mPrintTask = ePrintTask.None
                mWebPrint.ShowPrintPreviewDialog()
            Case ePrintTask.PageSetup
                mPrintTask = ePrintTask.None
                mWebPrint.ShowPageSetupDialog()
            Case ePrintTask.PrintNow
                mPrintTask = ePrintTask.None
                mWebPrint.Print()
            Case ePrintTask.Print
                mPrintTask = ePrintTask.None
                mWebPrint.ShowPrintDialog()
            Case ePrintTask.SaveAs
                mPrintTask = ePrintTask.None
                mWebPrint.ShowSaveAsDialog()
            Case Else
                'Do nothing for now
        End Select
        mbPrintBusy = False
    End Sub
    Friend Sub subSetPageFormat()
        '********************************************************************************************
        'Description:  page format
        '
        'Parameters: None
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oSettings As New Printing.PageSettings
        'mWebPrint.
    End Sub
    Friend Sub subPrintDoc(Optional ByVal bNow As Boolean = False)
        '********************************************************************************************
        'Description:  print the host file
        '
        'Parameters: bnow = skip the dialog box
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If bNow Then
            mPrintTask = ePrintTask.PrintNow
        Else
            mPrintTask = ePrintTask.Print
        End If

        If mbPageReady Then
            subRunPrintTask()
        End If
    End Sub
    Friend Sub subShowPrintPreview()
        '********************************************************************************************
        'Description:  run print preview for output window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintTask = ePrintTask.Preview
        If mbPageReady Then
            subRunPrintTask()
        End If
    End Sub
    Friend Sub subSaveAs()
        '********************************************************************************************
        'Description:  Save the output window to a file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintTask = ePrintTask.SaveAs
        If mbPageReady Then
            subRunPrintTask()
        End If
    End Sub
    Friend Sub subShowPageSetup()
        '********************************************************************************************
        'Description:  run page setup for output window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintTask = ePrintTask.PageSetup
        If mbPageReady Then
            subRunPrintTask()
        End If
    End Sub

    Private Sub webPrint_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles mWebPrint.DocumentCompleted
        '********************************************************************************************
        'Description:  Web page is loaded, select which print task to run
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/31/11  MSW     remove the size and visible settings.  It seems to work without them and it
        '                   they were causing the main form to resize.
        '********************************************************************************************
        mbPrintBusy = True
        Dim webPrint As WebBrowser = DirectCast(sender, WebBrowser)
        If webPrint Is Nothing Then
            Exit Sub
        End If
        'webPrint.Size = New Size(800, 600)
        'webPrint.Visible = True

        Select Case mPrintTask
            Case ePrintTask.Preview
                mPrintTask = ePrintTask.None
                webPrint.ShowPrintPreviewDialog()
            Case ePrintTask.PageSetup
                mPrintTask = ePrintTask.None
                webPrint.ShowPageSetupDialog()
            Case ePrintTask.PrintNow
                mPrintTask = ePrintTask.None
                webPrint.Print()
            Case ePrintTask.Print
                mPrintTask = ePrintTask.None
                webPrint.ShowPrintDialog()
            Case ePrintTask.SaveAs
                mPrintTask = ePrintTask.None
                mWebPrint.ShowSaveAsDialog()
            Case Else
                'Do nothing for now
        End Select
        mbPageReady = True
        subDeleteTempFile()
        mbPrintBusy = False
    End Sub
    Protected Overrides Sub Finalize()
        subDeleteTempFile()
        MyBase.Finalize()
    End Sub
    Private Sub subReadXMLSetup()
        '********************************************************************************************
        'Description:  Read PrintOptions from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = "//Screen[id='" & msScreenName & "']"
        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            oXMLDoc.Load(XMLPath)

            oMainNode = oXMLDoc.SelectSingleNode("//ScreenOptions")
            oNodeList = oMainNode.SelectNodes(sPath)
            Try
                If oNodeList.Count = 0 Then
                    mDebug.WriteEventToLog(msScreenName & ":clsPrintHtml:subReadXMLSetup", "ScreenOptions[" & msScreenName & "] not found.")
                    subSaveXMLSetup()
                Else
                    oNode = oNodeList(0) 'Should only be one match!!!
                    Dim sTmp As String = oNode.Item("SplitTables").InnerXml
                    If (InStr(sTmp.ToLower, "true") > 0) Then
                        mbTablePageBreaks = True
                    Else
                        mbTablePageBreaks = False
                    End If
                    mnTableSplitMaxRows = CInt(oNode.Item("MaxRows").InnerXml)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msScreenName & ":clsPrintHtml:subReadXMLSetup", "Invalid XML Data: [" & sPath & "] - " & ex.Message)
            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(msScreenName & ":clsPrintHtml:subReadXMLSetup", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
        End Try
    End Sub
    Public Sub subSaveXMLSetup()
        '********************************************************************************************
        'Description:  Save PrintOptions to XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = "//Screen[id='" & msScreenName & "']"
        Dim sTopic As String = String.Empty
        Dim oNode As XmlNode = Nothing
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            If (IO.File.Exists(msXMLFilePath) = False) Then
                IO.File.Create(msXMLFilePath)
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement("ScreenOptions")
                oMainNode = oXMLDoc.SelectSingleNode("//ScreenOptions")
            Else
                Try
                    oXMLDoc.Load(msXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//ScreenOptions")
                Catch ex As Exception
                    oXMLDoc = New XmlDocument
                    oMainNode = oXMLDoc.CreateElement("ScreenOptions")
                    oXMLDoc.AppendChild(oMainNode)
                    oMainNode = oXMLDoc.SelectSingleNode("//ScreenOptions")
                End Try
            End If

            oNodeList = oMainNode.SelectNodes(sPath)
            If oNodeList.Count = 0 Then
                oNode = oXMLDoc.CreateElement("Screen")
                Dim oNameNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "id", Nothing)
                Dim oSplitTablesNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "SplitTables", Nothing)
                Dim oMaxRowsNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "MaxRows", Nothing)
                oSplitTablesNode.InnerXml = mbTablePageBreaks.ToString
                oMaxRowsNode.InnerXml = mnTableSplitMaxRows.ToString
                oNameNode.InnerXml = msScreenName
                oNode.AppendChild(oNameNode)
                oNode.AppendChild(oSplitTablesNode)
                oNode.AppendChild(oMaxRowsNode)
                oMainNode.AppendChild(oNode)
            Else
                oNode = oNodeList(0) 'Should only be one match!!!
                oNode.Item("SplitTables").InnerXml = mbTablePageBreaks.ToString
                oNode.Item("MaxRows").InnerXml = mnTableSplitMaxRows.ToString
            End If
            Dim oIOStream As System.IO.StreamWriter = New System.IO.StreamWriter(msXMLFilePath)
            Dim oWriter As XmlTextWriter = New XmlTextWriter(oIOStream)
            oWriter.Formatting = Formatting.Indented
            oXMLDoc.WriteTo(oWriter)
            oWriter.Close()
            oIOStream.Close()
        Catch ex As Exception
            mDebug.WriteEventToLog(msScreenName & ":clsPrintHtml:subSaveXMLSetup", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)

        End Try
    End Sub
    Public Sub subShowOptions()
        '********************************************************************************************
        'Description:  show print options dialog
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        frmPrintOptions.SplitTableCheckBoxState = mbTablePageBreaks
        frmPrintOptions.MaxRowsValue = mnTableSplitMaxRows
        If frmPrintOptions.ShowDialog = DialogResult.OK Then
            mbTablePageBreaks = frmPrintOptions.SplitTableCheckBoxState
            mnTableSplitMaxRows = frmPrintOptions.MaxRowsValue
            subSaveXMLSetup()
        End If
    End Sub
    Public Sub New(Optional ByRef sScreenName As String = "", Optional ByRef bDefPageBreaks As Boolean = True, _
                   Optional ByRef nDefMaxRows As Integer = 30, _
                      Optional ByVal bExportCSVEnabled As Boolean = True, Optional ByVal bExportXMLEnabled As Boolean = False, _
                      Optional ByVal bExportTXTEnabled As Boolean = False, Optional ByVal bExportXLSEnabled As Boolean = False, _
                      Optional ByVal bExportODSEnabled As Boolean = False, Optional ByVal bExportXLSXEnabled As Boolean = False)
        '********************************************************************************************
        'Description:  Get the screen name to load options
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If sScreenName = "" Then
            msScreenName = String.Empty
            mbTablePageBreaks = False
            mnTableSplitMaxRows = 30
            mnTableSplitRepeatRows = 1
        Else
            'Don't care about remote path for this, jsut take the local computer settings
            msScreenName = sScreenName
            'Set defaults before trying to read the XML file
            mnTableSplitMaxRows = nDefMaxRows
            mnTableSplitRepeatRows = 1
            mbTablePageBreaks = bDefPageBreaks
            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) Then
                subReadXMLSetup()
            Else
                mDebug.WriteEventToLog(sScreenName & ":clsPrintHtml", "Could not find configuration file: " & _
                                       msXMLFILENAME)
            End If
        End If

        msFilter = sGetImportExportFilter(msExcelUtil, msZipUtil, _
                       bExportCSVEnabled, bExportXMLEnabled, _
                       bExportTXTEnabled, bExportXLSEnabled, _
                       bExportODSEnabled, bExportXLSXEnabled)

    End Sub
    Public Sub subCreateFromHtml(ByRef sSource As String)
        '********************************************************************************************
        'Description:  Build a document directly from HTML source
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Stop anything else going on
        If mWebPrint Is Nothing Then
            mWebPrint = New WebBrowser
            frmMain.Controls.Add(mWebPrint)
            'Make sure we can't see this on the screen 'RJO 11/23/11
            mWebPrint.Top = mWebPrint.Height * -1
            mWebPrint.Width = mWebPrint.Left * -1
            mWebPrint.ScriptErrorsSuppressed = True
        End If
        mbPageReady = False
        mWebPrint.DocumentText = sSource
    End Sub
End Class
Module ImportExport
    Private Const mbExportAllEnabled As Boolean = False

    Public Sub GetDTFromCSV(ByRef sTitleReq As String, ByRef sTableStart() As String, _
                            ByRef sHeader As String, ByRef oDT As DataTable, _
                            Optional ByRef sFile As String = "")
        '********************************************************************************************
        'Description:  Import settings from a csv file
        '
        'Parameters:    sTitleReq - a string that must be found in the header lines before it looks for the table 
        '               sTableStart - identify the first column name in the table to start reading the table
        'Returns:       sHeader - gather all the header lines into an array
        '               oDT - data table 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/07/11  MSW     support import from csv
        '********************************************************************************************
        Dim oTmpDT(0) As DataTable
        Dim sTmpHdr(0) As String
        GetDTFromCSV(sTitleReq, sTableStart, sTmpHdr, oTmpDT, sFile)
        sHeader = sTmpHdr(0)
        oDT = oTmpDT(0)
    End Sub
    Public Sub subPrintXMLNodeDetails(ByRef oNode As XmlNode)

        Debug.Print("oNode : " & oNode.Name & " - " & oNode.ChildNodes.Count)
        Debug.Print("  First : " & oNode.FirstChild.Name)
        Debug.Print("  Last : " & oNode.LastChild.Name)
        Debug.Print("  InnerXML : " & oNode.InnerXml)
        Debug.Print("  InnerText : " & oNode.InnerText)
        'Debug.Print("  BaseURI : " & oNode.BaseURI)
        'Debug.Print("  NamespaceURI : " & oNode.NamespaceURI)
        Debug.Print("  LocalName : " & oNode.LocalName)
        Debug.Print("  OuterXml : " & oNode.OuterXml)
        Debug.Print("  Prefix : " & oNode.Prefix)
        Debug.Print("  Value : " & oNode.Value)
        Dim oAttributes As XmlAttributeCollection = oNode.Attributes
        For Each oAttrib As XmlAttribute In oAttributes
            Debug.Print(oAttrib.Name & ": " & oAttrib.Value)
        Next
    End Sub
    Public Function sGetImportExportFilter(Optional ByRef sExcelUtil As String = "", Optional ByRef sZipUtil As String = "", _
                      Optional ByVal bExportCSVEnabled As Boolean = True, Optional ByVal bExportXMLEnabled As Boolean = False, _
                      Optional ByVal bExportTXTEnabled As Boolean = False, Optional ByVal bExportXLSEnabled As Boolean = False, _
                      Optional ByVal bExportODSEnabled As Boolean = False, Optional ByVal bExportXLSXEnabled As Boolean = False) As String
        Dim sFilter As String = String.Empty
        If (bExportCSVEnabled Or mbExportAllEnabled) Then
            If sFilter <> String.Empty Then
                sFilter = sFilter & "|"
            End If
            sFilter = sFilter & gcsRM.GetString("csSAVE_CSV_FILTER")
        End If
        If (bExportTXTEnabled Or mbExportAllEnabled) Then
            If sFilter <> String.Empty Then
                sFilter = sFilter & "|"
            End If
            sFilter = sFilter & gcsRM.GetString("csSAVE_TXT_FILTER")
        End If
        If (bExportXMLEnabled Or mbExportAllEnabled) Then
            If sFilter <> String.Empty Then
                sFilter = sFilter & "|"
            End If
            sFilter = sFilter & gcsRM.GetString("csSAVE_XML_FILTER")
        End If

        If (bExportXLSEnabled Or mbExportAllEnabled) And (GetDefaultFilePath(sExcelUtil, mPWCommon.eDir.VBApps, String.Empty, gs_EXCEL_UTIL_FILENAME)) Then
            sFilter = sFilter & "|" & gcsRM.GetString("csSAVE_XLS_FILTER")
        Else
            sExcelUtil = String.Empty
        End If
        If Not (GetDefaultFilePath(sZipUtil, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_FILENAME)) Then
            sZipUtil = String.Empty
        End If

        If (bExportODSEnabled Or mbExportAllEnabled) And (sZipUtil <> String.Empty) Then
            sFilter = sFilter & "|" & gcsRM.GetString("csSAVE_ODS_FILTER")

        End If
        If (bExportXLSXEnabled Or mbExportAllEnabled) And (sZipUtil <> String.Empty) Then
            sFilter = sFilter & "|" & gcsRM.GetString("csSAVE_XLSX_FILTER")
        Else
            sZipUtil = String.Empty
        End If

        Return sFilter
    End Function
    Public Sub GetDTFromCSV(ByRef sTitleReq As String, ByRef sTableStart() As String, _
                            ByRef sHeader() As String, ByRef oDT() As DataTable, _
                            Optional ByRef sFile As String = "", _
                      Optional ByVal bExportCSVEnabled As Boolean = True, Optional ByVal bExportXMLEnabled As Boolean = False, _
                      Optional ByVal bExportTXTEnabled As Boolean = False, Optional ByVal bExportXLSEnabled As Boolean = False, _
                      Optional ByVal bExportODSEnabled As Boolean = False, Optional ByVal bExportXLSXEnabled As Boolean = False)
        '********************************************************************************************
        'Description:  Import settings from a csv file
        '
        'Parameters:    sTitleReq - a string that must be found in the header lines before it looks for the table 
        '               sTableStart - identify the first column name in the table to start reading the table
        'Returns:       sHeader - gather all the header lines into an array
        '               oDT - data table 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/07/11  MSW     support import from csv
        ' 06/07/12  MSW     GetDTFromCSV - Add bNotEmpty to detect empty lines with the commas filled in.
        ' 02/22/13  MSW     Deal with a formatting error in import from XLSX
        ' 02/27/13  MSW     GetDTFromCSV - XLSX import for DMON-Fix shared string handling
        ' 02/27/13  MSW     GetDTFromCSV - add some delays before Directory.Delete, ignore errors
        '********************************************************************************************

        Dim sZipUtil As String = String.Empty
        Dim sExcelUtil As String = String.Empty
        Try
            Dim sFilter As String = sGetImportExportFilter(sExcelUtil, sZipUtil, _
                       bExportCSVEnabled, bExportXMLEnabled, _
                       bExportTXTEnabled, bExportXLSEnabled, _
                       bExportODSEnabled, bExportXLSXEnabled)
            If sFile = "" Then
                Const sCSV_EXT As String = "csv"
                Dim o As New OpenFileDialog
                With o
                    .Filter = sFilter
                    .Title = gcsRM.GetString("csOPEN_FILE_DLG_CAP")
                    .AddExtension = True
                    .CheckPathExists = True
                    .DefaultExt = "." & sCSV_EXT
                    If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                        sFile = .FileName
                    End If
                End With
            End If
        Catch ex As Exception

        End Try
        If sFile = String.Empty Then
            Exit Sub
        End If

        Dim sSplit() As String = Split(sFile, ".")
        Dim sDelim As String = ","
        Dim eImportFormat As clsPrintHtml.eExportFormat
        Select Case sSplit(sSplit.GetUpperBound(0)).ToLower
            Case "csv"
                eImportFormat = clsPrintHtml.eExportFormat.nCSV
                sDelim = ","
            Case "txt"
                eImportFormat = clsPrintHtml.eExportFormat.nTXT
                sDelim = vbTab
            Case "xml"
                eImportFormat = clsPrintHtml.eExportFormat.nXML
                sDelim = vbTab
            Case "xls"
                eImportFormat = clsPrintHtml.eExportFormat.nXLS
                sDelim = vbTab
            Case "ods"
                eImportFormat = clsPrintHtml.eExportFormat.nODS
                sDelim = vbTab
            Case "xlsx"
                eImportFormat = clsPrintHtml.eExportFormat.nXLSX
                sDelim = vbTab
        End Select
        Dim nTableNumber As Integer = 0
        Dim nTableWidth As Integer = 0
        ' Open the file to read from.
        Select Case eImportFormat
            Case clsPrintHtml.eExportFormat.nCSV, clsPrintHtml.eExportFormat.nTXT
                'open as text
                Dim sr As System.IO.StreamReader = Nothing
                Try
                    'process the file
                    'Open the stream and read it back.
                    Try
                        sr = System.IO.File.OpenText(sFile)

                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                        Trace.WriteLine(ex.StackTrace)
                        ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                             frmMain.Status, MessageBoxButtons.OK)
                        Exit Sub
                    End Try
                    Dim nLine As Integer = 0
                    Dim nStep As Integer = 0
                    ReDim sHeader(0)
                    sHeader(0) = String.Empty
                    If sTitleReq Is Nothing OrElse sTitleReq = String.Empty Then
                        ReDim oDT(0)
                        nStep = 1
                    End If
                    sHeader(nTableNumber) = String.Empty
                    Dim sColNames() As String = Nothing
                    Do While sr.Peek() >= 0
                        Dim sLine As String = sr.ReadLine()
                        'Strip out qoutes that some programs (open office) may save to text cells
                        sLine = sLine.Replace("""", "")
                        sLine = sLine.Replace("'", "")
                        Debug.Print(sLine)
                        Dim sText() As String = Split(sLine, sDelim)
                        Select Case nStep
                            Case 0
                                For Each sTmp As String In sText
                                    If sHeader(nTableNumber) = String.Empty Then
                                        sHeader(nTableNumber) = sTmp
                                    Else
                                        sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                    End If
                                Next
                                If InStr(sHeader(nTableNumber).ToLower, sTitleReq.ToLower) > 0 Then
                                    nStep = 1
                                End If
                            Case 1
                                For Each sTmp As String In sTableStart
                                    If (sText(0).Trim.ToLower = sTmp.Trim.ToLower) Then
                                        ReDim Preserve oDT(nTableNumber)
                                        oDT(nTableNumber) = New DataTable
                                        Dim nLastCol As Integer = sText.Length - 1
                                        Do While sText(nLastCol).Trim = String.Empty And nLastCol > 0
                                            nLastCol = nLastCol - 1
                                        Loop
                                        ReDim sColNames(nLastCol)
                                        For nCol As Integer = 0 To nLastCol
                                            Dim sCol As String = sText(nCol)
                                            If sCol = String.Empty OrElse oDT(nTableNumber).Columns.Contains(sCol) Then
                                                sCol = "Column" & nCol.ToString
                                            End If
                                            oDT(nTableNumber).Columns.Add(sCol)
                                            sColNames(nCol) = sCol
                                        Next
                                        nTableWidth = nLastCol + 1
                                        nStep = 2
                                        Exit For
                                    End If
                                Next
                                If nStep = 1 Then
                                    For Each sTmp As String In sText
                                        sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                    Next
                                End If
                            Case 2
                                If sText.Length >= nTableWidth Then
                                    oDT(nTableNumber).Rows.Add()
                                    Dim bNotEmpty As Boolean = False
                                    For nCol As Integer = 0 To sText.Length - 1
                                        If nCol >= sColNames.Length Then
                                            ReDim Preserve sColNames(nCol)
                                            sColNames(nCol) = "Col" & nCol.ToString
                                            oDT(nTableNumber).Columns.Add(sColNames(nCol))
                                        End If
                                        nTableWidth = sText.Length
                                        oDT(nTableNumber).Rows(oDT(nTableNumber).Rows.Count - 1).Item(nCol) = sText(nCol)
                                        If sText(nCol).Trim <> String.Empty Then
                                            bNotEmpty = True
                                        End If
                                    Next
                                    If bNotEmpty = False Then
                                        nStep = 0
                                        nTableNumber = nTableNumber + 1
                                        ReDim Preserve sHeader(nTableNumber)
                                        nTableWidth = 0
                                    End If
                                Else
                                    nStep = 0
                                    nTableNumber = nTableNumber + 1
                                    ReDim Preserve sHeader(nTableNumber)
                                    nTableWidth = 0
                                End If
                        End Select
                    Loop
                    If oDT Is Nothing Then
                        MessageBox.Show(gcsRM.GetString("csINVALID_DATA"), gcsRM.GetString("csIMPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                    sr.Close()


                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                         frmMain.Status, MessageBoxButtons.OK)
                Finally
                    If (sr IsNot Nothing) Then
                        sr.Close()
                    End If
                End Try
            Case clsPrintHtml.eExportFormat.nXLSX
                'open as XML
                Dim oXMLDoc As New XmlDocument
                Dim sColSharedString As New Collection
                Dim sSheetNames() As String
                Dim nSheets As Integer = 0
                Try
                    Dim bFailed As Boolean = False
                    Dim oMainNode As XmlNode = Nothing
                    Dim oNodeList As XmlNodeList = Nothing
                    Dim sTmpFolder As String = String.Empty
                    Dim nsmgr As XmlNamespaceManager = Nothing
                    Dim oTable As XmlNode = Nothing
                    Dim oRowList As XmlNodeList = Nothing
                    Dim oCellList As XmlNodeList = Nothing
                    sTmpFolder = sGetTmpFileName("XLSX_TMP") 'temp folder for unzip
                    Dim sCmd As String = sZipUtil & " " & gs_UNZIP_ALL & " " & """" & sFile & """" & "  " & """" & sTmpFolder & """"
                    sTmpFolder = sTmpFolder & "\"
                    'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                    Shell(sCmd, AppWinStyle.Hide, True, 30000)

                    If File.Exists(sTmpFolder & "xl\sharedStrings.xml") Then
                        Try
                            oXMLDoc.Load(sTmpFolder & "xl\sharedStrings.xml")
                        Catch ex As Exception
                            Trace.WriteLine(ex.Message)
                            Trace.WriteLine(ex.StackTrace)
                            ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                                 frmMain.Status, MessageBoxButtons.OK)
                            Exit Sub
                        End Try
                        For Each oNode As XmlNode In oXMLDoc.ChildNodes
                            If oNode.Name.ToLower = "sst" Then
                                For Each oNodeb As XmlNode In oNode.ChildNodes
                                    sColSharedString.Add(oNodeb.InnerText)
                                Next
                            End If
                        Next
                    End If

                    'Sheet list
                    If File.Exists(sTmpFolder & "xl\workbook.xml") Then
                        oXMLDoc.Load(sTmpFolder & "xl\workbook.xml")
                        For Each oNode As XmlNode In oXMLDoc.ChildNodes
                            If oNode.Name.ToLower = "workbook" Then
                                For Each oNodeb As XmlNode In oNode.ChildNodes
                                    If oNodeb.Name = "sheets" Then
                                        nSheets = oNodeb.ChildNodes.Count
                                        ReDim sSheetNames(nSheets - 1)
                                        For Each oNodec As XmlNode In oNodeb.ChildNodes
                                            Dim nSheet As Integer = CType(oNodec.Attributes("sheetId").Value, Integer)
                                            sSheetNames(nSheet - 1) = oNodec.Attributes("name").Value
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    Else
                        Dim sTmp As String = gcsRM.GetString("csFAILED_OPEN_XLSX")
                        Dim sPath As String = String.Empty
                        If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_ERRLOG)) Then
                            If IO.File.Exists(sPath) Then
                                sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                            End If
                        End If
                        MessageBox.Show(sTmp, gcsRM.GetString("csIMPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                        bFailed = True
                    End If


                    ReDim sHeader(0)
                    sHeader(0) = String.Empty
                    Dim nLine As Integer = 0
                    If sTitleReq Is Nothing OrElse sTitleReq = String.Empty Then
                        ReDim oDT(0)
                    End If
                    nTableNumber = -1
                    Dim sColNames() As String = Nothing

                    For nSheet As Integer = 0 To nSheets - 1
                        Dim sSheet As String = (nSheet + 1).ToString
                        If File.Exists(sTmpFolder & "xl\worksheets\sheet" & sSheet & ".xml") Then
                            Try
                                oXMLDoc.Load(sTmpFolder & "xl\worksheets\sheet" & sSheet & ".xml")
                            Catch ex As Exception
                                Trace.WriteLine(ex.Message)
                                Trace.WriteLine(ex.StackTrace)
                                ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                                     frmMain.Status, MessageBoxButtons.OK)
                                Exit Sub
                            End Try

                            nTableNumber = nTableNumber + 1
                            ReDim Preserve sHeader(nTableNumber)
                            Dim nStep As Integer = 0
                            If sTitleReq Is Nothing OrElse sTitleReq = String.Empty Then
                                nStep = 1
                            End If
                            nTableWidth = 0

                            For Each oNode As XmlNode In oXMLDoc.ChildNodes
                                If oNode.Name.ToLower = "worksheet" Then
                                    For Each oNodeSheet As XmlNode In oNode.ChildNodes
                                        If oNodeSheet.Name.ToLower = "sheetData" Then
                                            For Each oNodeRow As XmlNode In oNodeSheet.ChildNodes
                                                If oNodeRow.Name.ToLower = "row" Then
                                                    Dim sRow As String = String.Empty
                                                    For Each oNodeCol As XmlNode In oNodeRow.ChildNodes
                                                        Dim bSharedString As Boolean = False
                                                        For Each oAttr As XmlAttribute In oNodeCol.Attributes
                                                            If oAttr.Name.ToLower = "t" AndAlso oAttr.InnerXml.ToLower = "s" Then
                                                                bSharedString = True
                                                                Exit For
                                                            End If
                                                        Next
                                                        Dim sTextTmp As String = oNodeCol.FirstChild.InnerXml
                                                        If bSharedString Then
                                                            Try
                                                                Dim nIndex As Integer = CType(sTextTmp, Integer) + 1
                                                                ' 02/26/13  MSW     GetDTFromCSV - XLSX import for DMON - Fix shared string handling.
                                                                If nIndex <= sColSharedString.Count Then
                                                                    sRow = sRow & sColSharedString(nIndex).ToString & vbTab
                                                                Else
                                                                    sRow = sRow & sTextTmp & vbTab
                                                                End If
                                                            Catch ex As Exception
                                                                sRow = sRow & sTextTmp & vbTab
                                                            End Try
                                                        Else
                                                            sRow = sRow & sTextTmp & vbTab
                                                        End If
                                                    Next

                                                    Dim sText() As String = Split(sRow, sDelim)
                                                    Select Case nStep
                                                        Case 0
                                                            For Each sTmp As String In sText
                                                                If sHeader(nTableNumber) = String.Empty Then
                                                                    sHeader(nTableNumber) = sTmp
                                                                Else
                                                                    sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                                                End If
                                                            Next
                                                            If InStr(sHeader(nTableNumber).ToLower, sTitleReq.ToLower) > 0 Then
                                                                nStep = 1
                                                            End If
                                                        Case 1
                                                            For Each sTmp As String In sTableStart
                                                                If (sText(0).Trim.ToLower = sTmp.Trim.ToLower) And _
                                                                ((sTmp <> String.Empty) Or (sText.Length > 1)) Then
                                                                    ReDim Preserve oDT(nTableNumber)
                                                                    oDT(nTableNumber) = New DataTable
                                                                    Dim nLastCol As Integer = sText.Length - 1
                                                                    Do While sText(nLastCol).Trim = String.Empty And nLastCol > 0
                                                                        nLastCol = nLastCol - 1
                                                                    Loop
                                                                    ReDim sColNames(nLastCol)
                                                                    For nCol As Integer = 0 To nLastCol
                                                                        oDT(nTableNumber).Columns.Add(sText(nCol))
                                                                        sColNames(nCol) = sText(nCol)
                                                                    Next
                                                                    nTableWidth = nLastCol + 1
                                                                    nStep = 2
                                                                    Exit For
                                                                End If
                                                            Next
                                                            If nStep = 1 Then
                                                                For Each sTmp As String In sText
                                                                    sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                                                Next
                                                            End If
                                                        Case 2
                                                            oDT(nTableNumber).Rows.Add()
                                                            For nCol As Integer = 0 To sText.Length - 1
                                                                If nCol >= sColNames.Length Then
                                                                    ReDim Preserve sColNames(nCol)
                                                                    sColNames(nCol) = "Col" & nCol.ToString
                                                                    oDT(nTableNumber).Columns.Add(sColNames(nCol))
                                                                End If
                                                                oDT(nTableNumber).Rows(oDT(nTableNumber).Rows.Count - 1).Item(nCol) = sText(nCol)
                                                            Next
                                                    End Select
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                    Try
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                        Directory.Delete(sTmpFolder, True)
                    Catch ex As Exception
                        'Just let this go,
                    End Try

                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                         frmMain.Status, MessageBoxButtons.OK)
                End Try
            Case clsPrintHtml.eExportFormat.nODS, clsPrintHtml.eExportFormat.nXML, clsPrintHtml.eExportFormat.nXLS, clsPrintHtml.eExportFormat.nXLSX
                'open as XML
                Dim oXMLDoc As New XmlDocument
                Try
                    Dim bODS As Boolean = False 'simplify format choice later
                    Dim oMainNode As XmlNode = Nothing
                    Dim oNodeList As XmlNodeList = Nothing
                    Dim sTmpFolder As String = String.Empty
                    Dim nsmgr As XmlNamespaceManager = Nothing
                    Dim oTable As XmlNode = Nothing
                    Dim oRowList As XmlNodeList = Nothing
                    Dim oCellList As XmlNodeList = Nothing
                    Select Case eImportFormat
                        Case clsPrintHtml.eExportFormat.nODS
                            bODS = True
                            sTmpFolder = sGetTmpFileName("ODS_TMP") 'temp folder for unzip
                            Dim sCmd As String = sZipUtil & " " & gs_UNZIP_ALL & " " & """" & sFile & """" & "  " & """" & sTmpFolder & """"
                            sTmpFolder = sTmpFolder & "\"
                            'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                            Shell(sCmd, AppWinStyle.Hide, True, 30000)
                            If File.Exists(sTmpFolder & clsPrintHtml.msCONTENT_FILE) Then
                                oXMLDoc.Load(sTmpFolder & clsPrintHtml.msCONTENT_FILE)
                                nsmgr = New XmlNamespaceManager(oXMLDoc.NameTable)
                                nsmgr.AddNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0")
                                nsmgr.AddNamespace("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0")
                                nsmgr.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0")
                                nsmgr.AddNamespace("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0")
                                nsmgr.AddNamespace("draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0")
                                oMainNode = oXMLDoc.SelectSingleNode("descendant::office:spreadsheet", nsmgr)
                                oNodeList = oXMLDoc.SelectNodes("descendant::table:table", nsmgr)
                            Else
                                Dim sTmp As String = gcsRM.GetString("csFAILED_OPEN_ODS")
                                Dim sPath As String = String.Empty
                                If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_ERRLOG)) Then
                                    If IO.File.Exists(sPath) Then
                                        sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                                    End If
                                End If
                                MessageBox.Show(sTmp, gcsRM.GetString("csIMPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                            Try
                                Application.DoEvents()
                                Threading.Thread.Sleep(10)
                                Directory.Delete(sTmpFolder, True)
                            Catch ex As Exception
                                'Just let this go,
                            End Try
                        Case clsPrintHtml.eExportFormat.nXML
                            bODS = False
                            oXMLDoc.Load(sFile)
                            nsmgr = New XmlNamespaceManager(oXMLDoc.NameTable)
                            nsmgr.AddNamespace("c", "urn:schemas-microsoft-com:office:component:spreadsheet")
                            nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office")
                            nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet")
                            nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel")
                            oNodeList = oXMLDoc.SelectNodes("descendant::ss:Worksheet", nsmgr)
                        Case clsPrintHtml.eExportFormat.nXLS
                            Dim sTmpFile As String = sGetTmpFileName("XLS_TMP.XML")
                            Dim sCmd As String = sExcelUtil & " " & gs_XLS_TO_XML & " " & """" & sFile & """" & "  " & """" & sTmpFile & """"
                            'Shell out to excel converter
                            Debug.Print(sCmd)
                            Shell(sCmd, AppWinStyle.Hide, True, 30000)
                            If Not (IO.File.Exists(sTmpFile)) Then
                                Dim sTmp As String = gcsRM.GetString("csFAILED_XLS_XML")
                                Dim sPath As String = String.Empty
                                If (GetDefaultFilePath(sPath, mPWCommon.eDir.VBApps, String.Empty, gs_EXCEL_UTIL_ERRLOG)) Then
                                    If IO.File.Exists(sPath) Then
                                        sTmp = sTmp & My.Computer.FileSystem.ReadAllText(sPath)
                                    End If
                                End If
                                MessageBox.Show(sTmp, gcsRM.GetString("csIMPORTFAILED"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit Sub
                            End If
                            oXMLDoc.Load(sTmpFile)
                            nsmgr = New XmlNamespaceManager(oXMLDoc.NameTable)
                            nsmgr.AddNamespace("c", "urn:schemas-microsoft-com:office:component:spreadsheet")
                            nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office")
                            nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet")
                            nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel")
                            oNodeList = oXMLDoc.SelectNodes("descendant::ss:Worksheet", nsmgr)
                            File.Delete(sTmpFile)
                    End Select

                    If oNodeList IsNot Nothing Then
                        ReDim sHeader(0)
                        sHeader(0) = String.Empty
                        Dim nLine As Integer = 0
                        Dim nStep As Integer = 0
                        If sTitleReq Is Nothing OrElse sTitleReq = String.Empty Then
                            ReDim oDT(0)
                            nStep = 1
                        End If
                        nTableNumber = -1
                        Dim sColNames() As String = Nothing

                        For Each oSheet As XmlNode In oNodeList
                            'Scroll through each sheet
                            'subPrintXMLNodeDetails(oSheet)
                            nStep = 1
                            nTableNumber = nTableNumber + 1
                            ReDim Preserve sHeader(nTableNumber)
                            nTableWidth = 0

                            Dim sOuterXml As String = oSheet.OuterXml
                            Dim sSheetName As String = String.Empty
                            If bODS Then
                                sSheetName = oSheet.Attributes("table:name").Value
                                oRowList = oSheet.SelectNodes("table:table-row", nsmgr)
                            Else
                                sSheetName = oSheet.Attributes("ss:Name").Value
                                For Each oNode As XmlNode In oSheet.ChildNodes
                                    If oNode.Name = "Table" Then
                                        oTable = oNode
                                        oRowList = oNode.ChildNodes
                                    End If
                                Next
                            End If
                            Dim nRow As Integer = 0
                            For Each oRow As XmlNode In oRowList
                                'Debug.Print(oRow.Name)
                                nRow = nRow + 1
                                If bODS Then
                                    oCellList = oRow.SelectNodes("table:table-cell", nsmgr)
                                Else
                                    oCellList = oRow.ChildNodes
                                End If
                                Dim sLine As String = String.Empty
                                'Build each row into a tabbed list for easier processing
                                Dim nCellCount As Integer = 0
                                For Each oCell As XmlNode In oCellList
                                    nCellCount = nCellCount + 1
                                    Dim nRepeat As Integer = -1
                                    Dim nIndex As Integer = -1
                                    For Each oAttrib As XmlAttribute In oCell.Attributes
                                        If oAttrib.Name = "table:number-columns-repeated" Then
                                            nRepeat = CType(oAttrib.Value, Integer)
                                            Exit For
                                        End If
                                        If oAttrib.Name = "ss:index" Then
                                            nIndex = CType(oAttrib.Value, Integer)
                                            Exit For
                                        End If
                                    Next
                                    Try
                                        If nIndex > 0 Then
                                            Do While nCellCount < nIndex
                                                nCellCount = nCellCount + 1
                                                sLine = sLine & " " & vbTab
                                            Loop
                                        End If
                                        If nRepeat > 1 Then
                                            For nTmp As Integer = 2 To nRepeat
                                                nCellCount = nCellCount + 1
                                                sLine = sLine & oCell.InnerText & vbTab
                                            Next
                                        End If
                                        sLine = sLine & oCell.InnerText & vbTab
                                    Catch ex As Exception

                                    End Try
                                Next

                                Dim sText() As String = Split(sLine, sDelim)
                                Select Case nStep
                                    Case 0
                                        For Each sTmp As String In sText
                                            If sHeader(nTableNumber) = String.Empty Then
                                                sHeader(nTableNumber) = sTmp
                                            Else
                                                sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                            End If
                                        Next
                                        If InStr(sHeader(nTableNumber).ToLower, sTitleReq.ToLower) > 0 Then
                                            nStep = 1
                                        End If
                                    Case 1
                                        For Each sTmp As String In sTableStart
                                            If (sText(0).Trim.ToLower = sTmp.Trim.ToLower) And _
                                            ((sTmp <> String.Empty) Or (sText.Length > 1)) Then
                                                ReDim Preserve oDT(nTableNumber)
                                                oDT(nTableNumber) = New DataTable
                                                Dim nLastCol As Integer = sText.Length - 1
                                                Do While sText(nLastCol).Trim = String.Empty And nLastCol > 0
                                                    nLastCol = nLastCol - 1
                                                Loop
                                                ReDim sColNames(nLastCol)
                                                For nCol As Integer = 0 To nLastCol
                                                    oDT(nTableNumber).Columns.Add(sText(nCol))
                                                    sColNames(nCol) = sText(nCol)
                                                Next
                                                nTableWidth = nLastCol + 1
                                                nStep = 2
                                                Exit For
                                            End If
                                        Next
                                        If nStep = 1 Then
                                            For Each sTmp As String In sText
                                                sHeader(nTableNumber) = sHeader(nTableNumber) & vbTab & sTmp
                                            Next
                                        End If
                                    Case 2
                                        oDT(nTableNumber).Rows.Add()
                                        For nCol As Integer = 0 To sText.Length - 1
                                            If nCol >= sColNames.Length Then
                                                ReDim Preserve sColNames(nCol)
                                                sColNames(nCol) = "Col" & nCol.ToString
                                                oDT(nTableNumber).Columns.Add(sColNames(nCol))
                                            End If
                                            oDT(nTableNumber).Rows(oDT(nTableNumber).Rows.Count - 1).Item(nCol) = sText(nCol)
                                        Next
                                End Select
                            Next
                        Next
                    End If

                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, frmMain.msSCREEN_NAME, _
                                         frmMain.Status, MessageBoxButtons.OK)
                End Try
        End Select

    End Sub

End Module