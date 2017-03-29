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
' Form/Module: frmChart
'
' Description: DmonViewer
' 
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
'    01/13/2012 MSW     first draft                                                   
'    05/08/12   MSW     Work on XLSX output  - excel export w/o linkning to excel     
'    10/05/12   MSW     Fix up colors in item config                                  
'    02/27/13   MSW     pnlChart_Paint - Check length of mnYScaleToUse before access  
'    09/30/13   MSW     Save screenshots as jpegs                                     
'********************************************************************************************
Public Class frmChart
    Private mOdtItemData As DataTable
    Private mOdtScheduleData As DataTable
    Private mOdtTable As DataTable
    Private moFileInfo As frmMain.tFileInfo
    Private moItemInfo() As frmMain.tItemInfo = Nothing
    Private mnTAB_CHART As Integer = 0
    Private mnTAB_CONFIG As Integer = 1
    Private mnTAB_TABLE As Integer = 2
    Private mnTAB_SCHEDULE As Integer = 3
    Private mnTAB_ITEMS As Integer = 4
    Private mnCFG_COL_ITEM As Integer = 0
    Private mnCFG_COL_NAME As Integer = 1
    Private mnCFG_COL_UNITS As Integer = 2
    Private mnCFG_COL_TYPE As Integer = 3
    Private mnCFG_COL_SCALE As Integer = 4
    Private mnCFG_COL_OFFSET As Integer = 5
    Private mnCFG_COL_MIN_DATA As Integer = 6
    Private mnCFG_COL_MAX_DATA As Integer = 7
    Private mnCFG_COL_MIN_DRAW As Integer = 8
    Private mnCFG_COL_MAX_DRAW As Integer = 9
    Private mnCFG_COL_MULTIPLEX As Integer = 10
    Private mnCFG_COL_COLOR As Integer = 11
    Private msCFG_COL_ITEM As String = "Item"
    Private msCFG_COL_NAME As String = "Name"
    Private msCFG_COL_UNITS As String = "Units"
    Private msCFG_COL_TYPE As String = "Type"
    Private msCFG_COL_SCALE As String = "Scale"
    Private msCFG_COL_OFFSET As String = "Offset"
    Private msCFG_COL_MIN_DATA As String = "MinData"
    Private msCFG_COL_MAX_DATA As String = "MaxData"
    Private msCFG_COL_MIN_DRAW As String = "MinDraw"
    Private msCFG_COL_MAX_DRAW As String = "MaxDraw"
    Private msCFG_COL_MULTIPLEX As String = "MultiPlex"
    Private msCFG_COL_COLOR As String = "Color"
    Private Const msSCREEN_DUMP_NAME As String = "View_DmonViewer_Chart"
    Private mnCheckBoxMouseDown As Integer = -1
    Private mbEditsMade As Boolean = False
    Private mnCurrentTabIndex As Integer = 0
    Private mnDataGridSelCol As Integer = -1
    Private mnDataGridSelRow As Integer = -1
    Private mnMouseDownX As Integer = 0
    Private mnMouseDownY As Integer = 0
    Private mbEventBlocker As Boolean = False
    Private msOldEditVal As String = String.Empty
    Private mnScale(0) As Integer
    Private mbRefreshConfig As Boolean = False
    Private mnLineIndex() As Integer = Nothing
    Private msLineCursorValue() As String = Nothing
    Private mbShowValues As Boolean = False
    Private mbMouseDown As Boolean = False
    Private mnMousePos As Integer = 0
    Private mnMouseStartPos As Integer = 0
    Private Const mnMouseMinDiff As Integer = 5
    Private mnMouseDownDR As Integer = 0
    Private mnBitScale As Integer = -1
    Private moLineColor() As System.Drawing.Color = {Color.Black, Color.Black, Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Yellow, Color.Brown, _
        Color.Brown, Color.DarkViolet, Color.Magenta, Color.LightBlue, Color.LimeGreen, Color.AntiqueWhite, Color.Chocolate, _
        Color.DeepSkyBlue, Color.Turquoise, Color.SteelBlue, Color.Aquamarine, Color.Chocolate, Color.Firebrick, Color.ForestGreen}
    Private mnLineScaleIndex() As Integer
    Private mfTableData(,) As Single
    Private mnTableRowMax As Integer = 0
    Private mnTableColMax As Integer = 0
    Private Structure tChartStats
        Dim nTimeCol As Integer
        Dim nTickCol As Integer
        Dim nNumberCol As Integer
        Dim nXaxis As Integer
        Dim fXScaleMin As Single
        Dim fXScaleMax As Single
        Dim fXScaleRange As Single
        Dim nXScrollMax As Integer
        Dim sXunits As String
    End Structure
    Private moChartStats As tChartStats


    'Chart drawing variables
    Private mnPnlHeight As Integer = 0
    Private mnPnlWidth As Integer = 0
    'Grid dimensions
    Private mnGrphHeight As Integer = 0
    Private mnGrphWidth As Integer = 0
    Private mptOrigin As Point = Nothing
    Private mnRight As Integer = 0
    Private mnTop As Integer = 0
    Private mnRight2 As Integer = 0
    Private mnLeft2 As Integer = 0
    'Scale units to pixels or whatever unit we're drawing in
    'The values aren't important here, just make sure they don't cause a divide by 0 when properties are set during startup
    Private mfScrollWidth As Single = 100
    Private mfScrolloffset As Single = 0
    Private mfXmin As Single = 0
    Private mfXmax As Single = 100
    Private mfXScale As Single = 1
    Private mnYScaleToUse() As Integer
    Private mnYScales As Integer = 0
    Private mbActiveScales() As Boolean
    Private mfYmax() As Single
    Private mfYmin() As Single
    Private mfYRange() As Single
    Private mfYScale() As Single
    Private Const mnNUM_VLINES As Integer = 10
    Private Const mnNUM_HLINES As Integer = 10
    'Grid lines
    Private mfVertGridLineSpacing As Single = 20
    Private mfHorzGridLineSpacing As Single = 20
    'Digital lines
    Private mnBitSpacing As Integer = 20
    Private mnBit0Value As Integer = 2
    Private mnBit1Value As Integer = 16

    'Grid pens
    Private moPenBorder As Pen = New Pen(Drawing.Color.Black, 3)
    Private moPengrid As Pen = New Pen(Drawing.Color.Black, 1)


    Private Structure tMultiPlexCfg
        Dim sName As String
        Dim nItem As Integer
        Dim nBit As Integer
    End Structure
    Private moMultiPlexCfg() As tMultiPlexCfg

    Private Structure tScaleMerge
        Dim sUnits As String
        Dim fMin As Single
        Dim fMax As Single
    End Structure
    Private moScaleMerge() As tScaleMerge = Nothing
    Friend Property EditsMade() As Boolean
        Get
            Return mbEditsMade
        End Get
        Set(ByVal value As Boolean)
            mbEditsMade = value
            If value Then
                mbRefreshConfig = True 'only cleared by calling applyConfig
            End If
        End Set
    End Property
    Friend Property ItemDT() As DataTable
        Get
            Return mOdtItemData
        End Get
        Set(ByVal value As DataTable)
            mOdtItemData = value
        End Set
    End Property
    Friend Property ScheduleDT() As DataTable
        Get
            Return mOdtScheduleData
        End Get
        Set(ByVal value As DataTable)
            mOdtScheduleData = value
        End Set
    End Property
    Friend Property TableDT() As DataTable
        Get
            Return mOdtTable
        End Get
        Set(ByVal value As DataTable)
            mOdtTable = value
            mnTableRowMax = mOdtTable.Rows.Count - 1
            mnTableColMax = mOdtTable.Columns.Count - 1
            ReDim mfTableData(mnTableColMax, mnTableRowMax)
            For nRow As Integer = 0 To mnTableRowMax
                For nCol As Integer = 0 To mnTableColMax
                    Try
                        mfTableData(nCol, nRow) = CType(mOdtTable.Rows.Item(nRow).Item(nCol), Single)
                    Catch ex As Exception
                        mfTableData(nCol, nRow) = 0
                    End Try
                Next
            Next
        End Set
    End Property
    Friend Property FileInfo() As frmMain.tFileInfo
        Get
            Return moFileInfo
        End Get
        Set(ByVal value As frmMain.tFileInfo)
            moFileInfo = value
        End Set
    End Property
    Friend Property ItemInfo() As frmMain.tItemInfo()
        Get
            Return moItemInfo
        End Get
        Set(ByVal value As frmMain.tItemInfo())
            moItemInfo = value
            If moLineColor Is Nothing OrElse value.GetUpperBound(0) > moLineColor.GetUpperBound(0) Then
                ReDim Preserve moLineColor(value.GetUpperBound(0))
            End If
            For nItem As Integer = 0 To value.GetUpperBound(0)
                If value(nItem).oColor.R <> 0 Or value(nItem).oColor.G <> 0 Or value(nItem).oColor.B <> 0 Then
                    moLineColor(nItem) = value(nItem).oColor
                End If
            Next
        End Set
    End Property
    Public Function ColorToString(ByRef oColor As Color) As String
        If oColor.IsNamedColor Then
            Return oColor.Name
        Else
            Return String.Format("A={0};R={1};G={2};B={3}", oColor.A, oColor.R, oColor.G, oColor.B)
        End If
    End Function
    Public Function StringToColor(ByRef sColor As String) As Color
        Try
            Dim oColor As Color
            Dim sParts() As String = Split(sColor.Replace(",", ";"), ";")
            If sParts.GetUpperBound(0) >= 2 Then
                Dim nTmp(sParts.GetUpperBound(0)) As Integer
                For nComp As Integer = 0 To sParts.GetUpperBound(0)
                    Dim nIdx As Integer = InStr(sParts(nComp), "=")
                    If nIdx > 0 Then
                        nTmp(nComp) = CType(sParts(nComp).Substring(2), Integer)
                    Else

                        nTmp(nComp) = CType(sParts(nComp), Integer)
                    End If
                Next
                If sParts.GetUpperBound(0) = 2 Then
                    oColor = Color.FromArgb(nTmp(0), nTmp(1), nTmp(2))
                Else
                    oColor = Color.FromArgb(nTmp(0), nTmp(1), nTmp(2), nTmp(3))
                End If
            ElseIf sParts.GetUpperBound(0) = 0 Then
                'Try named color
                oColor = Color.FromName(sColor)
            End If
            Return oColor
        Catch ex As Exception
            Return Color.FromArgb(0, 0, 0, 0)
        End Try
    End Function
    Private Sub frmChart_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Load the form, init some labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tabChart.Text = gpsRM.GetString("psCHART")
        tabConfig.Text = gpsRM.GetString("psCONFIG")
        tabSchedule.Text = gpsRM.GetString("psSCHEDULE")
        tabItems.Text = gpsRM.GetString("psITEMS")
        tabTable.Text = gpsRM.GetString("psTABLE")
        Me.Text = gpsRM.GetString("psCHART")
        btnExport.Text = gpsRM.GetString("psEXPORT")
        btnExport.Image = DirectCast(gcsRM.GetObject("imgSave"), Image)
        mScreenSetup.InitializeForm(Me)
        lblZoneCap.Text = gpsRM.GetString("psCOL_ZONE")
        lblDeviceCap.Text = gpsRM.GetString("psCOL_DEVICE")
        lblStyleCap.Text = gpsRM.GetString("psCOL_STYLE")
        lblOptionCap.Text = gpsRM.GetString("psCOL_OPTION")
        lblColorCap.Text = gpsRM.GetString("psCOL_COLOR")
        lblVinCap.Text = gpsRM.GetString("psCOL_VIN")
        lblDateCap.Text = gpsRM.GetString("psCOL_DATE")
        lblFileCap.Text = gpsRM.GetString("psCOL_FILENAME")

        lblZone.Text = String.Empty
        lblDevice.Text = String.Empty
        lblStyle.Text = String.Empty
        lblOption.Text = String.Empty
        lblColor.Text = String.Empty
        lblVin.Text = String.Empty
        lblDate.Text = String.Empty
        lblFile.Text = String.Empty
        mnuBit.Text = gpsRM.GetString("psBIT")
        mnuInteger.Text = gpsRM.GetString("psINTEGER")
        mnuFloat.Text = gpsRM.GetString("psFLOAT")
        mnuMultiPlex.Text = gpsRM.GetString("psMULTIPLEX")
        mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
        mnuUnSelectAll.Text = gcsRM.GetString("csSELECT_NONE")
        mnuSelectColor.Text = gpsRM.GetString("psSELECT_COLOR")
    End Sub
    Private Sub subDoScaleMerge()
        '********************************************************************************************
        'Description: Find the ranges for units
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If moItemInfo Is Nothing Then
            Exit Sub
        End If
        Dim nNumItems As Integer = moItemInfo.GetUpperBound(0)
        ReDim moScaleMerge(0)
        moScaleMerge(0).sUnits = String.Empty
        moScaleMerge(0).fMin = 0
        moScaleMerge(0).fMax = 0
        For nItem As Integer = 0 To nNumItems
            Select Case moItemInfo(nItem).sType.ToLower
                Case gpsRM.GetString("psBIT") ', gpsRM.GetString("psMULTIPLEX")
                Case Else
                    Dim nCount As Integer = moScaleMerge.GetUpperBound(0)
                    Dim bFound As Boolean = False
                    For nMergeIdx As Integer = 0 To nCount
                        If moScaleMerge(nMergeIdx).sUnits.Trim.ToLower = moItemInfo(nItem).sUnits.Trim.ToLower Then
                            bFound = True
                            If moItemInfo(nItem).fPlotMin < moScaleMerge(nMergeIdx).fMin Then
                                moScaleMerge(nMergeIdx).fMin = moItemInfo(nItem).fPlotMin
                            End If
                            If moItemInfo(nItem).fPlotMax > moScaleMerge(nCount).fMax Then
                                moScaleMerge(nMergeIdx).fMax = moItemInfo(nItem).fPlotMax
                            End If
                        End If
                    Next
                    If bFound = False Then
                        nCount = nCount + 1
                        ReDim Preserve moScaleMerge(nCount)
                        moScaleMerge(nCount).sUnits = moItemInfo(nItem).sUnits
                        moScaleMerge(nCount).fMin = moItemInfo(nItem).fPlotMin
                        moScaleMerge(nCount).fMax = moItemInfo(nItem).fPlotMax
                    End If
            End Select
        Next
        '2nd pass, copy the min and max back from the merge into each item
        If moMultiPlexCfg IsNot Nothing Then
            nNumItems = nNumItems + moMultiPlexCfg.GetUpperBound(0) + 1
        End If
        ReDim Preserve mnLineScaleIndex(nNumItems)
        mnBitScale = -1
        For nItem As Integer = 0 To moItemInfo.GetUpperBound(0)
            Select Case moItemInfo(nItem).sType
                'Case gpsRM.GetString("psMULTIPLEX")
                '    mnLineScaleIndex(nItem) = -1
                Case gpsRM.GetString("psBIT")
                    mnLineScaleIndex(nItem) = -1
                    mnBitScale = mnBitScale - 1
                Case Else
                    If moItemInfo(nItem).sUnits.Trim <> String.Empty Then
                        For nScaleItem As Integer = 0 To moScaleMerge.GetUpperBound(0)
                            If moItemInfo(nItem).sUnits <> String.Empty AndAlso moScaleMerge(nScaleItem).sUnits.Trim.ToLower = moItemInfo(nItem).sUnits.Trim.ToLower Then
                                moItemInfo(nItem).fPlotMin = moScaleMerge(nScaleItem).fMin
                                moItemInfo(nItem).fPlotMax = moScaleMerge(nScaleItem).fMax
                                mnLineScaleIndex(nItem) = nScaleItem
                            End If
                        Next
                    End If
            End Select
        Next
        mnBitScale = mnBitScale + 1
        subLineSelectionChanged()
    End Sub
    Private Sub ApplyConfig(ByVal bFirst As Boolean)
        '********************************************************************************************
        'Description: Show the form
        '
        'Parameters: bFirst = first call from show event
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subDoScaleMerge()
        Dim nNumItems As Integer = moItemInfo.GetUpperBound(0)
        If bFirst Then
            dgConfig.RowCount = 1
            dgConfig.ColumnCount = 0
            dgConfig.Columns.Add(msCFG_COL_ITEM, gpsRM.GetString("psITEM"))
            dgConfig.Columns.Add(msCFG_COL_NAME, gpsRM.GetString("psNAME"))
            dgConfig.Columns.Add(msCFG_COL_UNITS, gpsRM.GetString("psUNITS"))
            dgConfig.Columns.Add(msCFG_COL_TYPE, gpsRM.GetString("psTYPE"))
            dgConfig.Columns.Add(msCFG_COL_SCALE, gpsRM.GetString("psSCALE"))
            dgConfig.Columns.Add(msCFG_COL_OFFSET, gpsRM.GetString("psOFFSET"))
            dgConfig.Columns.Add(msCFG_COL_MIN_DATA, gpsRM.GetString("psMINDATA"))
            dgConfig.Columns.Add(msCFG_COL_MAX_DATA, gpsRM.GetString("psMAXDATA"))
            dgConfig.Columns.Add(msCFG_COL_MIN_DRAW, gpsRM.GetString("psMINDRAW"))
            dgConfig.Columns.Add(msCFG_COL_MAX_DRAW, gpsRM.GetString("psMAXDRAW"))
            dgConfig.Columns.Add(msCFG_COL_MULTIPLEX, gpsRM.GetString("psMULTIPLEX"))
            dgConfig.Columns.Add(msCFG_COL_COLOR, gpsRM.GetString("psCOLOR"))
            dgConfig.Rows.Add(nNumItems + 1)
        End If

        moMultiPlexCfg = Nothing
        'Private mnLineIndex() As Integer = Nothing
        'Private moLineColor() As System.Drawing.Color
        'Private mnLineScaleIndex() As Integer
        ReDim mnLineIndex(nNumItems)
        ReDim msLineCursorValue(nNumItems)
        Dim nCount As Integer = moLineColor.GetUpperBound(0)
        If nCount < nNumItems Then
            ReDim Preserve moLineColor(nNumItems)
            For nIdx As Integer = nCount + 1 To moLineColor.GetUpperBound(0)
                moLineColor(nIdx) = Drawing.Color.Black
            Next
        End If
        For nItem As Integer = 0 To nNumItems
            With dgConfig.Rows.Item(nItem)
                If bFirst Then
                    'Fill the config table
                    .Cells(msCFG_COL_ITEM).ValueType = GetType(Integer)
                    .Cells(msCFG_COL_ITEM).Value = nItem + 1
                    .Cells(msCFG_COL_NAME).ValueType = GetType(String)
                    .Cells(msCFG_COL_NAME).Value = moItemInfo(nItem).sName
                    .Cells(msCFG_COL_UNITS).ValueType = GetType(String)
                    .Cells(msCFG_COL_UNITS).Value = moItemInfo(nItem).sUnits
                    .Cells(msCFG_COL_TYPE).ValueType = GetType(String)
                    .Cells(msCFG_COL_TYPE).Value = moItemInfo(nItem).sType
                    .Cells(msCFG_COL_TYPE).ContextMenuStrip = mnuDataType
                    .Cells(msCFG_COL_SCALE).ValueType = GetType(Double)
                    .Cells(msCFG_COL_SCALE).Value = moItemInfo(nItem).fScale
                    .Cells(msCFG_COL_OFFSET).ValueType = GetType(Double)
                    .Cells(msCFG_COL_OFFSET).Value = moItemInfo(nItem).fOffset
                    .Cells(msCFG_COL_MIN_DATA).ValueType = GetType(Double)
                    .Cells(msCFG_COL_MIN_DATA).Value = moItemInfo(nItem).fMin
                    .Cells(msCFG_COL_MAX_DATA).ValueType = GetType(Double)
                    .Cells(msCFG_COL_MAX_DATA).Value = moItemInfo(nItem).fMax
                    .Cells(msCFG_COL_MIN_DRAW).ValueType = GetType(Double)
                    .Cells(msCFG_COL_MIN_DRAW).Value = moItemInfo(nItem).fPlotMin
                    .Cells(msCFG_COL_MAX_DRAW).ValueType = GetType(Double)
                    .Cells(msCFG_COL_MAX_DRAW).Value = moItemInfo(nItem).fPlotMax
                    .Cells(msCFG_COL_MULTIPLEX).ValueType = GetType(String)
                    .Cells(msCFG_COL_MULTIPLEX).Value = moItemInfo(nItem).sMultiPlex
                    .Cells(msCFG_COL_COLOR).ValueType = GetType(String)
                    If (moItemInfo(nItem).oColor.A > 0) And (moItemInfo(nItem).oColor.A > 0 Or moItemInfo(nItem).oColor.A > 0 Or moItemInfo(nItem).oColor.A > 0) Then
                        moLineColor(nItem) = moItemInfo(nItem).oColor
                    End If
                    .Cells(msCFG_COL_COLOR).Style.ForeColor = moLineColor(nItem)
                    .Cells(msCFG_COL_COLOR).Value = ColorToString(moLineColor(nItem))
                End If
                'Add item to checklist
                Dim sLine As String = moItemInfo(nItem).sName
                If moItemInfo(nItem).sUnits <> String.Empty Then
                    sLine = sLine & " (" & moItemInfo(nItem).sUnits & ")"
                End If
                Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
                If oChk Is Nothing Then
                    oChk = New CheckBox
                    oChk.AutoSize = True
                    oChk.Name = "chkLine" & nItem.ToString
                    oChk.Size = New System.Drawing.Size(70, 17)
                    oChk.TabIndex = 0
                    oChk.UseVisualStyleBackColor = True
                    oChk.ContextMenuStrip = Me.mnuCheckBoxes
                    spltChart.Panel1.Controls.Add(oChk)
                    AddHandler oChk.CheckedChanged, AddressOf chkLine_CheckedChanged
                    AddHandler oChk.MouseDown, AddressOf chkLine_MouseDown
                End If
                Dim nTop As Integer = chkLine0.Top + nItem * (chkLine1.Top - chkLine0.Top)
                oChk.Location = New System.Drawing.Point(chkLine0.Left, nTop)
                oChk.ForeColor = moLineColor(nItem)
                oChk.Text = sLine
                oChk.Visible = True

                mnLineIndex(nItem) = nItem
                mnLineScaleIndex(nItem) = -1

                Dim bMulti As Boolean = False
                If moItemInfo(nItem).sType.ToLower = gpsRM.GetString("psMULTIPLEX").ToLower Then
                    bMulti = True
                    'Check multiplex names:
                    moItemInfo(nItem).sMultiPlex = moItemInfo(nItem).sMultiPlex.Replace(",", ";")
                    moItemInfo(nItem).sMultiPlex = moItemInfo(nItem).sMultiPlex.Replace(vbTab, ";")
                    Dim sMultiNames() As String = Split(moItemInfo(nItem).sMultiPlex, ";")
                    Dim nMaxBit As Integer = 1
                    Do While ((2 ^ nMaxBit) < moItemInfo(nItem).fMax)
                        nMaxBit += 1
                    Loop
                    nMaxBit = nMaxBit - 1
                    If sMultiNames.GetUpperBound(0) > nMaxBit Then
                        nMaxBit = sMultiNames.GetUpperBound(0)
                    ElseIf sMultiNames.GetUpperBound(0) < nMaxBit Then
                        ReDim Preserve sMultiNames(nMaxBit)
                    End If
                    Dim nStartBit As Integer = 0
                    If moMultiPlexCfg Is Nothing Then
                        ReDim moMultiPlexCfg(nMaxBit)
                        moItemInfo(nItem).nMultiPlexStart = 0
                    Else
                        nStartBit = moMultiPlexCfg.GetUpperBound(0) + 1
                        ReDim Preserve moMultiPlexCfg(nStartBit + nMaxBit)
                        moItemInfo(nItem).nMultiPlexStart = nStartBit
                    End If
                    moItemInfo(nItem).nMultiPlexCount = 0
                    For nBit As Integer = 0 To nMaxBit
                        If sMultiNames(nBit) IsNot Nothing AndAlso sMultiNames(nBit) <> String.Empty Then
                            moItemInfo(nItem).nMultiPlexCount = moItemInfo(nItem).nMultiPlexCount + 1
                            With moMultiPlexCfg(nBit + nStartBit)
                                .sName = sMultiNames(nBit)
                                .nItem = nItem
                                .nBit = nBit
                            End With
                        End If
                    Next
                    ReDim Preserve moMultiPlexCfg(moItemInfo(nItem).nMultiPlexCount + nStartBit - 1)
                Else
                    moItemInfo(nItem).nMultiPlexStart = -1
                    moItemInfo(nItem).nMultiPlexCount = 0
                End If

                'Identify the time if available. If not, pick an x axis
                'Number	Tick	Time
                'constants and an option for a local language lookup.
                'Also check the boxes for any columns not listed here
                Select Case moItemInfo(nItem).sName.ToLower
                    Case "time", gpsRM.GetString("psTIME_COL")
                        moChartStats.nTimeCol = nItem
                    Case "tick", gpsRM.GetString("psTICK_COL")
                        moChartStats.nTickCol = nItem
                    Case "number", gpsRM.GetString("psNUMBER_COL")
                        moChartStats.nNumberCol = nItem
                    Case "sample trigger", "process #", "line number", "reserved"
                    Case Else
                        mbEventBlocker = True
                        oChk.Checked = Not (bMulti)
                        mbEventBlocker = False
                End Select
            End With
        Next
        If moMultiPlexCfg IsNot Nothing Then

            Dim nOffset As Integer = moItemInfo.GetUpperBound(0) + 1
            ReDim Preserve mnLineIndex(moMultiPlexCfg.GetUpperBound(0) + nOffset)
            ReDim Preserve mnLineScaleIndex(moMultiPlexCfg.GetUpperBound(0) + nOffset)
            ReDim Preserve msLineCursorValue(moMultiPlexCfg.GetUpperBound(0) + nOffset)
            nCount = moLineColor.GetUpperBound(0)
            If nCount < moMultiPlexCfg.GetUpperBound(0) + nOffset Then
                ReDim Preserve moLineColor(moMultiPlexCfg.GetUpperBound(0) + nOffset)
                For nIdx As Integer = nCount + 1 To moLineColor.GetUpperBound(0)
                    moLineColor(nIdx) = Drawing.Color.Black
                Next
            End If
            For nItem As Integer = 0 To moMultiPlexCfg.GetUpperBound(0)
                'Add item to checklist
                Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & (nItem + nOffset).ToString), CheckBox)
                If oChk Is Nothing Then
                    oChk = New CheckBox
                    oChk.AutoSize = True
                    oChk.Name = "chkLine" & (nItem + nOffset).ToString
                    oChk.Size = New System.Drawing.Size(70, 17)
                    oChk.TabIndex = 0
                    oChk.UseVisualStyleBackColor = True
                    oChk.ContextMenuStrip = Me.mnuCheckBoxes
                    spltChart.Panel1.Controls.Add(oChk)
                    AddHandler oChk.CheckedChanged, AddressOf chkLine_CheckedChanged
                    AddHandler oChk.MouseDown, AddressOf chkLine_MouseDown
                End If
                Dim nTop As Integer = chkLine0.Top + (nItem + nOffset) * (chkLine1.Top - chkLine0.Top)
                oChk.Location = New System.Drawing.Point(chkLine0.Left, nTop)
                oChk.ForeColor = moLineColor(nItem + nOffset)
                oChk.Text = moMultiPlexCfg(nItem).sName
                oChk.Visible = True

                'Go to negative tag numbers for multiplex bits
                'Offset by 1 so we don't get zero going both ways
                mnLineIndex(nItem + nOffset) = -1 * (nItem + 1)
                mnLineScaleIndex(nItem + nOffset) = -1
                mbEventBlocker = True
                If moMultiPlexCfg(nItem).sName.Trim.ToLower = "reserved" Then
                    oChk.Checked = False
                Else
                    oChk.Checked = True
                End If
                mbEventBlocker = False
            Next
        End If

        With moChartStats
            If .nTimeCol > -1 Then
                .nXaxis = .nTimeCol
            ElseIf .nTickCol > -1 Then
                .nXaxis = .nTickCol
            ElseIf .nNumberCol > -1 Then
                .nXaxis = .nNumberCol
            End If
            If .nXaxis > -1 Then
                .fXScaleMin = moItemInfo(.nXaxis).fMin
                .fXScaleMax = moItemInfo(.nXaxis).fMax
            Else
                .fXScaleMin = 0
                .fXScaleMax = mnTableRowMax - 1
            End If
            .fXScaleRange = .fXScaleMax - .fXScaleMin

            If .nXaxis >= 0 Then
                .sXunits = moItemInfo(.nXaxis).sUnits
            Else
                .sXunits = gpsRM.GetString("psPOINTS")
            End If
            If .fXScaleRange <= 1 Then
                'Hardcode scale, disable trackbar
                trkScale.Enabled = False
                .nXScrollMax = 1
                ReDim mnScale(0)
                mnScale(0) = 1
                trkScale.Minimum = 0
                trkScale.Maximum = 0
                trkScale.TickFrequency = 1
                trkScale.SmallChange = 1
                trkScale.LargeChange = 1
                trkScale.Value = 0
                trkScale_Scroll(trkScale)
            Else
                Dim nMax As Integer = CType(.fXScaleRange, Integer)
                If nMax < .fXScaleRange Then 'Force it to round up
                    nMax = nMax + 1
                End If
                Dim sTrkMax As String = nMax.ToString
                Dim nMax2 As Integer = CType(10 ^ sTrkMax.Length, Integer)
                .nXScrollMax = nMax
                Dim nMin As Integer = CType(0.1 * .fXScaleMax / mnTableRowMax, Integer)
                Dim sTrkMin As String = nMin.ToString
                Dim nMin2 As Integer = CType(10 ^ sTrkMin.Length, Integer)

                Dim nIndex As Integer = 0
                Dim nScale As Integer = 1
                ReDim mnScale(3 * (1 + sTrkMax.Length - sTrkMin.Length) + 1)
                Do While nIndex = 0
                    If nScale >= nMin Then
                        mnScale(nIndex) = nScale
                        mnScale(nIndex + 1) = nScale * 2
                        mnScale(nIndex + 2) = nScale * 5
                        nIndex = nIndex + 3
                    ElseIf (nScale * 2) >= nMin Then
                        mnScale(nIndex) = nScale * 2
                        mnScale(nIndex + 1) = nScale * 5
                        nIndex = nIndex + 2
                    ElseIf (nScale * 5) >= nMin Then
                        mnScale(nIndex) = nScale * 5
                        nIndex = nIndex + 1
                    End If
                    nScale = nScale * 10
                Loop
                Dim bDone As Boolean = False
                Do While bDone = False
                    mnScale(nIndex) = nScale
                    If mnScale(nIndex) >= nMax Then
                        bDone = True
                    Else
                        nIndex = nIndex + 1
                        mnScale(nIndex) = nScale * 2
                        If mnScale(nIndex) >= nMax Then
                            bDone = True
                        Else
                            nIndex = nIndex + 1
                            mnScale(nIndex) = nScale * 5
                            If mnScale(nIndex) >= nMax Then
                                bDone = True
                            Else
                                nIndex = nIndex + 1
                                nScale = nScale * 10
                            End If
                        End If
                    End If
                Loop
                ReDim Preserve mnScale(nIndex)
                trkScale.Minimum = 0
                trkScale.Maximum = nIndex
                trkScale.TickFrequency = 1
                trkScale.SmallChange = 1
                trkScale.LargeChange = 3
                trkScale.Value = CType((trkScale.Maximum - trkScale.Minimum) / 2, Integer)

                trkScale.Enabled = True
                trkScale_Scroll(trkScale)
            End If
        End With
        subDoScaleMerge()
        mbRefreshConfig = False
    End Sub
    Private Sub frmChart_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: Show the form
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moChartStats.nTimeCol = -1
        moChartStats.nTickCol = -1
        moChartStats.nNumberCol = -1
        moChartStats.nXaxis = -1

        dgtable.DataSource = mOdtTable
        dgSchedule.DataSource = mOdtScheduleData
        dgItems.DataSource = mOdtItemData

        tlsMain.Enabled = True
        btnSave.Enabled = True
        btnPrint.Enabled = True
        mnuPrint.Enabled = True
        mnuPageSetup.Enabled = True
        mnuPrintFile.Enabled = True
        mnuPrintOptions.Enabled = True
        mnuPrintPreview.Enabled = True
        btnExport.Visible = True
        btnSave.Visible = False

        lblZone.Text = moFileInfo.sZone
        lblDevice.Text = moFileInfo.sDevice
        lblStyle.Text = moFileInfo.sOption
        lblOption.Text = moFileInfo.sOption
        lblColor.Text = moFileInfo.sColor
        lblVin.Text = moFileInfo.sVIN
        lblDate.Text = moFileInfo.dtDate.ToString
        lblFile.Text = moFileInfo.sLocation & moFileInfo.sFileName
        ApplyConfig(True)
    End Sub

    Private Sub subPrintCommon(ByVal bExport As Boolean)
        '********************************************************************************************
        'Description: Common print routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFileNameSplit() As String = Split(moFileInfo.sFileName, ".")
        Dim sFileName As String = String.Empty
        For nTmp As Integer = 0 To sFileNameSplit.GetUpperBound(0) - 1
            sFileName = sFileName & sFileNameSplit(nTmp)
        Next
        Dim bCancel As Boolean = False

        If bExport Then
            Dim sTitle(4) As String
            sTitle(1) = ""
            sTitle(2) = gpsRM.GetString("psCOL_ZONE") & ": " & moFileInfo.sZone
            sTitle(3) = ""
            sTitle(4) = gpsRM.GetString("psCOL_DEVICE") & ": " & moFileInfo.sDevice
            Dim sSubTitle(8) As String
            sSubTitle(0) = gpsRM.GetString("psCOL_STYLE") & ": " & moFileInfo.sStyle
            sSubTitle(1) = ""
            sSubTitle(2) = gpsRM.GetString("psCOL_COLOR") & ": " & moFileInfo.sColor
            sSubTitle(3) = ""
            sSubTitle(4) = gpsRM.GetString("psCOL_OPTION") & ": " & moFileInfo.sOption
            sSubTitle(5) = ""
            sSubTitle(6) = gpsRM.GetString("psCOL_VIN") & ": " & moFileInfo.sVIN
            sSubTitle(7) = ""
            sSubTitle(8) = moFileInfo.dtDate.ToString
            frmMain.gPrintHtml.SubTitleCols = 6
            Dim oExportFormat As clsPrintHtml.eExportFormat
            Dim sFileOut As String = String.Empty
            Dim nColWidths(3) As Single
            nColWidths(0) = 1
            nColWidths(1) = 1.2
            nColWidths(2) = 2
            nColWidths(3) = 2.5
            frmMain.gPrintHtml.ColumnWidthsList = nColWidths
            frmMain.gPrintHtml.subStartDoc("", sFileName, bExport, bCancel, sFileOut, oExportFormat)
            If bCancel Then
                Exit Sub
            End If
            Select Case oExportFormat
                Case clsPrintHtml.eExportFormat.nXLS
                    'Supports pictures
                    tabMain.SelectedTab = tabChart
                    Application.DoEvents()
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                    Dim oSC As New ScreenShot.ScreenCapture
                    oSC.CaptureWindowToFile(tabChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    sTitle(0) = tabChart.Text
                    frmMain.gPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, tabChart.Text)
                Case clsPrintHtml.eExportFormat.nODS
                    'Supports pictures
                    tabMain.SelectedTab = tabChart
                    Application.DoEvents()
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                    Dim oSC As New ScreenShot.ScreenCapture
                    oSC.CaptureWindowToFile(tabChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    sTitle(0) = tabChart.Text
                    frmMain.gPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, tabChart.Text, "A", "5", "O", "39")
                Case clsPrintHtml.eExportFormat.nXLSX
                    'Supports pictures
                    tabMain.SelectedTab = tabChart
                    Application.DoEvents()
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "png")
                    Dim oSC As New ScreenShot.ScreenCapture
                    oSC.CaptureWindowToFile(tabChart.Handle, sTmpFile, Imaging.ImageFormat.Png)
                    sTitle(0) = tabChart.Text
                    frmMain.gPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, tabChart.Text, "A", "5", "R", "39")
                Case Else
                    'no pictures
            End Select
            Dim nColWidthIndex(11) As Integer
            nColWidthIndex(0) = 0
            nColWidthIndex(1) = 2
            nColWidthIndex(2) = 2
            nColWidthIndex(3) = 1
            nColWidthIndex(4) = 0
            nColWidthIndex(5) = 0
            nColWidthIndex(6) = 0
            nColWidthIndex(7) = 0
            nColWidthIndex(8) = 0
            nColWidthIndex(9) = 0
            nColWidthIndex(10) = 1
            nColWidthIndex(11) = 0
            frmMain.gPrintHtml.ColumnWidthIndex = nColWidthIndex

            sTitle(0) = tabConfig.Text
            frmMain.gPrintHtml.subAddObject(dgConfig, "", sTitle, sSubTitle, tabConfig.Text)


            frmMain.gPrintHtml.ColumnWidthIndex = Nothing
            sTitle(0) = tabTable.Text
            frmMain.gPrintHtml.subAddObject(dgtable, "", sTitle, sSubTitle, tabTable.Text)

            ReDim nColWidthIndex(10)
            nColWidthIndex(0) = 0
            nColWidthIndex(1) = 0
            nColWidthIndex(2) = 3
            nColWidthIndex(3) = 2
            nColWidthIndex(4) = 1
            nColWidthIndex(5) = 0
            nColWidthIndex(6) = 0
            nColWidthIndex(7) = 0
            nColWidthIndex(8) = 0
            nColWidthIndex(9) = 0
            nColWidthIndex(10) = 0
            frmMain.gPrintHtml.ColumnWidthIndex = nColWidthIndex
            sTitle(0) = tabSchedule.Text
            frmMain.gPrintHtml.subAddObject(dgSchedule, "", sTitle, sSubTitle, tabSchedule.Text)

            ReDim nColWidthIndex(13)
            nColWidthIndex(0) = 0
            nColWidthIndex(1) = 3
            nColWidthIndex(2) = 2
            nColWidthIndex(3) = 1
            nColWidthIndex(4) = 0
            nColWidthIndex(5) = 0
            nColWidthIndex(6) = 0
            nColWidthIndex(7) = 0
            nColWidthIndex(8) = 0
            nColWidthIndex(9) = 0
            nColWidthIndex(10) = 0
            nColWidthIndex(11) = 0
            nColWidthIndex(12) = 0
            nColWidthIndex(13) = 0
            frmMain.gPrintHtml.ColumnWidthIndex = nColWidthIndex
            sTitle(0) = tabItems.Text
            frmMain.gPrintHtml.subAddObject(dgItems, "", sTitle, sSubTitle, tabItems.Text)
            frmMain.gPrintHtml.subCloseFile("")
        Else
            Dim sTitle(2) As String
            sTitle(0) = gpsRM.GetString("psCOL_ZONE") & ": " & moFileInfo.sZone
            sTitle(1) = gpsRM.GetString("psCOL_DEVICE") & ": " & moFileInfo.sDevice
            Dim sSubTitle(4) As String
            sSubTitle(0) = gpsRM.GetString("psCOL_STYLE") & ": " & moFileInfo.sStyle
            sSubTitle(1) = gpsRM.GetString("psCOL_COLOR") & ": " & moFileInfo.sColor
            sSubTitle(2) = gpsRM.GetString("psCOL_OPTION") & ": " & moFileInfo.sOption
            sSubTitle(3) = gpsRM.GetString("psCOL_VIN") & ": " & moFileInfo.sVIN
            sSubTitle(4) = moFileInfo.dtDate.ToString
            Dim oDG As DataGridView = Nothing
            sTitle(2) = tabMain.SelectedTab.Text
            sFileName = sFileName & " - " & tabMain.SelectedTab.Text
            Select Case tabMain.SelectedIndex
                Case mnTAB_CHART
                    oDG = Nothing
                    frmMain.gPrintHtml.subStartDoc("", sFileName, False, bCancel)
                    If bCancel Then
                        Exit Sub
                    End If
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                    Dim oSC As New ScreenShot.ScreenCapture
                    oSC.CaptureWindowToFile(tabChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    frmMain.gPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, tabChart.Text)
                    frmMain.gPrintHtml.subCloseFile("")
                Case mnTAB_CONFIG
                    oDG = dgConfig
                Case mnTAB_TABLE
                    oDG = dgtable
                Case mnTAB_SCHEDULE
                    oDG = dgSchedule
                Case mnTAB_ITEMS
                    oDG = dgItems
            End Select
            If oDG IsNot Nothing Then
                frmMain.gPrintHtml.subCreateSimpleDoc(oDG, frmMain.Status, sFileName, sTitle, sSubTitle, bExport)
            End If
        End If

    End Sub
    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
           ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
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

        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                Exit Sub
            Case "btnSave"
                subSaveConfig()
            Case "btnExport"
                subPrintCommon(True)
                frmMain.gPrintHtml.subPrintDoc(True)
            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subPrintCommon(False)
                frmMain.gPrintHtml.subPrintDoc(True)
        End Select
        tlsMain.Refresh()
    End Sub

    Private Sub subSaveConfig()
        '********************************************************************************************
        'Description: Save Config data
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        dgConfig.EndEdit()
        Application.DoEvents()
        mbEditsMade = False
        frmMain.subSaveDmonItemsToXML(moItemInfo)
        For nItem As Integer = 0 To moItemInfo.GetUpperBound(0)
            With dgConfig.Rows.Item(nItem)
                .Cells(msCFG_COL_ITEM).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_ITEM).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_NAME).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_NAME).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_UNITS).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_UNITS).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_TYPE).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_TYPE).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_SCALE).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_SCALE).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_OFFSET).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_OFFSET).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_MIN_DATA).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_MIN_DATA).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_MAX_DATA).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_MAX_DATA).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_MIN_DRAW).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_MIN_DRAW).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_MAX_DRAW).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_MAX_DRAW).Style.ForeColor = Color.Black
                .Cells(msCFG_COL_MULTIPLEX).Style.SelectionForeColor = Color.Black
                .Cells(msCFG_COL_MULTIPLEX).Style.ForeColor = Color.Black
            End With
        Next
    End Sub

    Private Sub mnuPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrint.Click
        '********************************************************************************************
        'Description: Print functions
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subPrintCommon(False)
        frmMain.gPrintHtml.subPrintDoc(True)
    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description: Print functions
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subPrintCommon(False)
        frmMain.gPrintHtml.subShowPrintPreview()
    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description: Print functions
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subPrintCommon(False)
        frmMain.gPrintHtml.subShowPageSetup()
    End Sub

    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description: Print functions
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subPrintCommon(False)
        frmMain.gPrintHtml.subSaveAs()
    End Sub

    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description: Print functions
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subPrintCommon(False)
        frmMain.gPrintHtml.subShowOptions()
    End Sub


    Private Sub tabMain_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  New tab selected. check for save here while we can still cancel the selection
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (mnCurrentTabIndex = mnTAB_CONFIG) Then
            If EditsMade AndAlso bAskForSave() = False Then
                e.Cancel = True
            End If
        End If
    End Sub
    Private Sub tabMain_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
        '********************************************************************************************
        'Description: Tab Change
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oDG As DataGridView = Nothing
        Dim sFileSuffix As String = String.Empty
        If (mnCurrentTabIndex = mnTAB_CONFIG) And (mnTAB_CONFIG <> tabMain.SelectedIndex) And _
            (mbEditsMade Or mbRefreshConfig) Then
            'Leaving config panel, redraw
            Call ApplyConfig(False)
        End If
        mnCurrentTabIndex = tabMain.SelectedIndex
        Select Case tabMain.SelectedIndex
            Case mnTAB_CONFIG
                btnSave.Enabled = True
                btnSave.Visible = True
                btnExport.Enabled = True
                btnExport.Visible = True
            Case Else
                btnSave.Enabled = False
                btnSave.Visible = False
                btnExport.Enabled = True
                btnExport.Visible = True
        End Select
    End Sub
    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    true to continue
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
            Case DialogResult.Yes 'Response.Yes
                subSaveConfig()
                Return True
            Case DialogResult.Cancel
                Return False
            Case Else
                Return True
        End Select

    End Function

    Private Sub dgConfig_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgConfig.CellClick
        If mbEventBlocker Then Exit Sub
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex
        Dim oCell As DataGridViewCell = DirectCast(dgConfig.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
        Select Case e.ColumnIndex
            Case mnCFG_COL_UNITS, mnCFG_COL_SCALE, mnCFG_COL_OFFSET, mnCFG_COL_MIN_DRAW, mnCFG_COL_MAX_DRAW, mnCFG_COL_MULTIPLEX
                'Text edit
                If frmMain.Privilege > ePrivilege.None Then
                    If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                        dgConfig.BeginEdit(True)
                    End If
                End If
            Case mnCFG_COL_COLOR
                'Text edit
                If frmMain.Privilege > ePrivilege.None Then
                    If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                        dgConfig.BeginEdit(True)
                    End If
                End If
            Case mnCFG_COL_TYPE
                'CBO edit
                Dim oPoint As Point = New Point((mnMouseDownX + dgConfig.RowHeadersWidth), _
                                        (mnMouseDownY))
                Dim nIdx As Integer = 0
                Do While nIdx < mnDataGridSelCol
                    oPoint.X += dgConfig.Columns(nIdx).Width
                    nIdx += 1
                Loop
                nIdx = 0
                Do While nIdx < mnDataGridSelRow
                    If dgConfig.Rows(nIdx).Visible Then
                        oPoint.Y += dgConfig.Rows(nIdx).Height
                    End If
                    nIdx += 1
                Loop
                oCell.ContextMenuStrip.Show(dgConfig.PointToScreen(oPoint))
            Case Else
                Debug.Print(e.ColumnIndex.ToString)
        End Select
    End Sub
    Private Sub dgConfig_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgConfig.CellEnter
        '********************************************************************************************
        'Description: mouse-down - record the column and index for the pop-up menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        If mbEventBlocker Then Exit Sub
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex
        Select Case e.ColumnIndex
            Case mnCFG_COL_UNITS, mnCFG_COL_SCALE, mnCFG_COL_OFFSET, mnCFG_COL_MIN_DRAW, mnCFG_COL_MAX_DRAW, mnCFG_COL_MULTIPLEX
                'Text edit
                If frmMain.Privilege > ePrivilege.None Then
                    Dim oCell As DataGridViewCell = DirectCast(dgConfig.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
                    If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                        dgConfig.BeginEdit(True)
                    End If
                End If
            Case mnCFG_COL_COLOR
                'Text edit
                If frmMain.Privilege > ePrivilege.None Then
                    Dim oCell As DataGridViewCell = DirectCast(dgConfig.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
                    If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                        dgConfig.BeginEdit(True)
                    End If
                End If
            Case mnCFG_COL_TYPE
                'CBO edit
            Case Else
                Debug.Print(e.ColumnIndex.ToString)
        End Select
    End Sub

    Private Sub dgConfig_CellMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgConfig.CellMouseDown
        '********************************************************************************************
        'Description: mouse-down - record the column and index for the pop-up menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex
        mnMouseDownX = e.X
        mnMouseDownY = e.Y
    End Sub
    Private Sub dgConfig_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles dgConfig.CellBeginEdit
        '********************************************************************************************
        'Description:  Start edit - save the old value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        If dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
            msOldEditVal = String.Empty
        Else
            msOldEditVal = dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
        End If
    End Sub

    Private Sub dgConfig_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgConfig.CellEndEdit
        '********************************************************************************************
        'Description: end edit
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        'Check for invalid data
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        Try
            Dim oCell As DataGridViewCell = DirectCast(dgConfig.Rows(e.RowIndex).Cells.Item(e.ColumnIndex), DataGridViewCell)
            If oCell.Value Is Nothing Then
                dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                        MessageBoxButtons.OK, _
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Exit Sub
            End If
            Dim sVal As String = oCell.Value.ToString
            Dim bRestore As Boolean = (frmMain.Privilege = ePrivilege.None)
            If bRestore Then
                dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                Exit Sub
            End If
            Debug.Print(moItemInfo(e.RowIndex).sName)
            Dim sText As String = dgConfig.Rows(e.RowIndex).Cells.Item(e.ColumnIndex).Value.ToString
            Select Case e.ColumnIndex
                Case mnCFG_COL_ITEM, mnCFG_COL_MAX_DATA, mnCFG_COL_MIN_DATA, mnCFG_COL_NAME
                    'Shouldn't be able to edit these
                    dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                    Exit Sub
                Case mnCFG_COL_UNITS
                    moItemInfo(e.RowIndex).sUnits = dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
                Case mnCFG_COL_COLOR
                    Dim sColor As String = sVal
                    Dim oColor As Color = StringToColor(sColor)
                    If oColor.A <> 0 Or oColor.R <> 0 Or oColor.G <> 0 Or oColor.B <> 0 Then
                        moLineColor(e.RowIndex) = oColor
                        moItemInfo(e.RowIndex).oColor = oColor
                        dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = ColorToString(oColor)
                        dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.ForeColor = oColor
                        EditsMade = True
                        Exit Sub 'Skip the marking red for edit
                    Else
                        dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                        Exit Sub
                    End If
                Case mnCFG_COL_MULTIPLEX
                    moItemInfo(e.RowIndex).sMultiPlex = dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
                Case mnCFG_COL_SCALE, mnCFG_COL_OFFSET, mnCFG_COL_MAX_DRAW, mnCFG_COL_MIN_DRAW
                    'Numbers
                    If dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString = msOldEditVal Then
                        Exit Sub
                    End If
                    If IsNumeric(sText) Then
                        Dim sngTmp As Single = CType(sText, Single)
                        Select Case e.ColumnIndex
                            Case mnCFG_COL_SCALE
                                moItemInfo(e.RowIndex).fScale = sngTmp
                            Case mnCFG_COL_OFFSET
                                moItemInfo(e.RowIndex).fOffset = sngTmp
                            Case mnCFG_COL_MAX_DRAW
                                If sngTmp < moItemInfo(e.RowIndex).fMax Then
                                    dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                                    MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    Exit Sub
                                Else
                                    moItemInfo(e.RowIndex).fPlotMax = sngTmp
                                End If
                            Case mnCFG_COL_MIN_DRAW
                                If sngTmp > moItemInfo(e.RowIndex).fMin Then
                                    dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                                    MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    Exit Sub
                                Else
                                    moItemInfo(e.RowIndex).fPlotMin = sngTmp
                                End If
                        End Select
                    Else
                        dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                        MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        Exit Sub
                    End If
                Case Else
            End Select
            'Mark changes in red
            dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.ForeColor = Color.Red
            dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.SelectionForeColor = Color.Red
            EditsMade = True
        Catch ex As Exception
            dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
        End Try
    End Sub
    Private Sub mdgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgConfig.DataError
        '********************************************************************************************
        'Description: edit error 
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sVal As String = dgConfig.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        dgConfig.CancelEdit()
    End Sub
    Private Sub dgConfig_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles dgConfig.KeyPress
        '********************************************************************************************
        'Description:  offer login
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Not (dgConfig.IsCurrentCellInEditMode) Then
            If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) Then
                If frmMain.Privilege = ePrivilege.None Then
                    frmMain.Privilege = ePrivilege.Edit
                End If
            End If
        End If
    End Sub

    Private Sub mnu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBit.Click, mnuFloat.Click, mnuInteger.Click, mnuMultiPlex.Click
        '********************************************************************************************
        'Description:  config page pop-up menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oCell As DataGridViewCell = DirectCast(dgConfig.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        If oCell.Value.ToString <> oMnu.Text Then
            oCell.Value = oMnu.Text
            moItemInfo(mnDataGridSelRow).sType = oMnu.Text
            oCell.Style.ForeColor = Color.Red
            oCell.Style.SelectionForeColor = Color.Red
            EditsMade = True
        End If
    End Sub

    Private Sub chkLine_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles chkLine0.MouseDown, chkLine1.MouseDown
        '********************************************************************************************
        'Description:  Record the checkbox with the mousedown in case it's need for context menu handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oChk As CheckBox = DirectCast(sender, CheckBox)
        'chkLine#
        mnCheckBoxMouseDown = CType(oChk.Name.Substring(7), Integer)
    End Sub
    Private Sub chkLine_CheckedChanged(Optional ByVal sender As System.Object = Nothing, Optional ByVal e As System.EventArgs = Nothing) Handles chkLine0.CheckedChanged, chkLine1.CheckedChanged
        '********************************************************************************************
        'Description:  checkbox selection changed, redraw
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker = False Then
            subDoScaleMerge()
        End If
    End Sub

    Private Sub mnuSelectColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectColor.Click
        '********************************************************************************************
        'Description:  Line color selection
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnCheckBoxMouseDown >= 0 Then
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & mnCheckBoxMouseDown.ToString), CheckBox)

            Dim cd As New ColorDialog
            Try
                cd.AllowFullOpen = True
                cd.AnyColor = True
                cd.FullOpen = True
                cd.Color = oChk.ForeColor
                If cd.ShowDialog() = DialogResult.OK Then
                    moLineColor(mnCheckBoxMouseDown) = cd.Color
                    oChk.ForeColor = cd.Color
                    dgConfig.Rows(mnCheckBoxMouseDown).Cells(msCFG_COL_COLOR).Style.ForeColor = cd.Color
                    dgConfig.Rows(mnCheckBoxMouseDown).Cells(msCFG_COL_COLOR).Value = ColorToString(cd.Color)
                End If
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
            End Try

            pnlChart.Invalidate()
        End If
    End Sub

    Private Sub mnuUnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnSelectAll.Click
        '********************************************************************************************
        'Description:  Select All
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        For nItem As Integer = 0 To mnLineIndex.GetUpperBound(0)
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
            oChk.Checked = False
        Next
        mbEventBlocker = False
        subDoScaleMerge()
    End Sub

    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  Unselect All
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        For nItem As Integer = 0 To mnLineIndex.GetUpperBound(0)
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
            oChk.Checked = True
        Next
        mbEventBlocker = False
        subDoScaleMerge()
    End Sub
    Private Sub frmChart_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
                    subLaunchHelp(gs_HELP_VIEW_DMON_CHART, frmMain.oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case tabMain.SelectedIndex
                        Case mnTAB_CHART
                            sName = sName & ".jpg"
                        Case mnTAB_CONFIG
                            sName = sName & "_Config.jpg"
                        Case mnTAB_TABLE
                            sName = sName & "_Table.jpg"
                        Case mnTAB_SCHEDULE
                            sName = sName & "_Schedule.jpg"
                        Case mnTAB_ITEMS
                            sName = sName & "_Items.jpg"
                    End Select

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                Case Keys.Escape
                    Me.Close()
                Case Else

            End Select
        End If
    End Sub


    Private Sub trkScale_Scroll(ByVal sender As System.Object, Optional ByVal e As System.EventArgs = Nothing) Handles trkScale.Scroll
        '********************************************************************************************
        'Description:  adjust chart scale
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        '  X-Axis x units/division, y units/page
        '  trkScale.Value
        Dim sTmp(3) As Object
        sTmp(0) = (mnScale(trkScale.Value) / 5).ToString
        sTmp(1) = moChartStats.sXunits
        sTmp(2) = mnScale(trkScale.Value).ToString
        sTmp(3) = moChartStats.sXunits
        lblScale.Text = String.Format(gpsRM.GetString("psLBL_SCALE"), sTmp)
        'Setup Scrollbar
        hscrChart.Minimum = 0
        Dim fTmp As Double = (hscrChart.Value + (hscrChart.LargeChange / 2)) / hscrChart.Maximum
        hscrChart.Maximum = moChartStats.nXScrollMax
        If moChartStats.nXScrollMax > mnScale(trkScale.Value) Then
            hscrChart.LargeChange = mnScale(trkScale.Value)
            hscrChart.Enabled = True
        Else
            hscrChart.LargeChange = moChartStats.nXScrollMax
            hscrChart.Value = 0
            hscrChart.Enabled = False
        End If

        subScalingChanged()
    End Sub


    Private Sub hscrChart_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles hscrChart.ValueChanged
        '********************************************************************************************
        'Description:  scroll bar moved
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subScalingChanged()
    End Sub



    Private Sub pnlChart_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlChart.Resize
        '********************************************************************************************
        'Description:  Chart area resize
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim rPanel As Panel = DirectCast(sender, Panel)
        'Available panel space
        mnPnlHeight = rPanel.Height
        mnPnlWidth = rPanel.Width

        'Grid dimensions
        mnGrphHeight = CInt(mnPnlHeight * 0.8)
        mnGrphWidth = CInt(mnPnlWidth * 0.8)
        mptOrigin = New Point(CInt(mnPnlWidth * 0.1), CInt(mnPnlHeight * 0.9))
        mnRight = mptOrigin.X + mnGrphWidth
        mnRight2 = CInt(mnPnlWidth * 0.95)
        mnLeft2 = CInt(mnPnlWidth * 0.05)
        mnTop = mptOrigin.Y - mnGrphHeight
        subScalingChanged()
    End Sub

    Private Sub subScalingChanged()
        '********************************************************************************************
        'Description:  procecss when chart scaling changes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Scale units to pixels or whatever unit we're drawing in
        mfScrollWidth = mnScale(trkScale.Value)
        mfScrolloffset = CType((hscrChart.Value * mnScale(trkScale.Value) / hscrChart.LargeChange), Single)
        mfXmin = moChartStats.fXScaleMin + mfScrolloffset
        mfXmax = mfXmin + mfScrollWidth
        mfXScale = mnGrphWidth / mfScrollWidth
        'Grid lines
        mfHorzGridLineSpacing = (mfScrollWidth / mnNUM_VLINES)
        pnlChart.Invalidate()
    End Sub
    Private Sub subLineSelectionChanged()
        '********************************************************************************************
        'Description:  procecss when line selection checkboxes change
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If moScaleMerge Is Nothing OrElse mnLineIndex Is Nothing Then
            Exit Sub
        End If

        ReDim mnYScaleToUse(moScaleMerge.GetUpperBound(0))
        For nCnt As Integer = 0 To moScaleMerge.GetUpperBound(0)
            mnYScaleToUse(nCnt) = -1
        Next

        Dim mnYScales As Integer = moScaleMerge.GetUpperBound(0)

        Dim nDigitalScale As Integer = 10
        Dim nDigitalCount As Integer = 0
        ReDim mbActiveScales(mnYScales)
        ReDim mfYmax(mnYScales)
        ReDim mfYmin(mnYScales)
        ReDim mfYRange(mnYScales)
        ReDim mfYScale(mnYScales)
        For nYscale As Integer = 0 To mnYScales
            mfYmax(nYscale) = moScaleMerge(nYscale).fMax
            mfYmin(nYscale) = moScaleMerge(nYscale).fMin
            mfYRange(nYscale) = mfYmax(nYscale) - mfYmin(nYscale)
            If mfYRange(nYscale) < 1 Then
                mfYRange(nYscale) = 1
            End If
            mfYScale(nYscale) = mnGrphHeight / mfYRange(nYscale)
            If (moScaleMerge(nYscale).sUnits.ToLower <> "seconds") AndAlso (moScaleMerge(nYscale).sUnits <> String.Empty) Then
                mnYScaleToUse(0) = nYscale
            End If
            mbActiveScales(nYscale) = False
            Debug.Print(nYscale.ToString & ": " & mfYScale(nYscale).ToString)
        Next
        Dim nScalesActiveCount As Integer = 0
        Dim nIndex As Integer = 0
        Dim bDigitalUsed(0) As Boolean
        For nItem As Integer = 0 To mnLineScaleIndex.GetUpperBound(0)
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
            If oChk IsNot Nothing AndAlso oChk.Checked Then
                If mnLineScaleIndex(nItem) >= 0 Then
                    If mbActiveScales(mnLineScaleIndex(nItem)) = False Then
                        mbActiveScales(mnLineScaleIndex(nItem)) = True
                        nScalesActiveCount = nScalesActiveCount + 1
                        mnYScaleToUse(nIndex) = mnLineScaleIndex(nItem)
                        nIndex = nIndex + 1
                    End If
                Else
                    'Digital
                    Dim nScale As Integer = 1
                    Dim nItemTmp As Integer = nItem
                    If nItem > moItemInfo.GetUpperBound(0) Then 'Multiplex
                        nItemTmp = moMultiPlexCfg((nItem - moItemInfo.GetUpperBound(0)) - 1).nItem
                    Else
                        nScale = CType(moItemInfo(nItem).fScale, Integer)
                    End If
                    If nDigitalScale < nScale Then
                        nDigitalScale = nScale
                    End If
                    nDigitalCount = nDigitalCount + 1
                    If nDigitalScale < nDigitalCount Then
                        nDigitalScale = nDigitalCount
                    End If
                    Dim nOffset As Integer = CInt(moItemInfo(nItemTmp).fOffset)
                    If nOffset >= nDigitalScale Then
                        nDigitalScale = nOffset + 1
                    End If
                    If nDigitalScale > (bDigitalUsed.GetUpperBound(0) + 1) Then
                        ReDim Preserve bDigitalUsed(nDigitalScale)
                    End If
                    If bDigitalUsed(nOffset) And nItem <= moItemInfo.GetUpperBound(0) + 1 Then
                        For nIdx As Integer = 0 To bDigitalUsed.GetUpperBound(0)
                            If bDigitalUsed(nIdx) = False Then
                                bDigitalUsed(nIdx) = True
                                moItemInfo(nItemTmp).fOffset = nIdx
                                dgConfig.Rows.Item(nItemTmp).Cells(msCFG_COL_OFFSET).Value = moItemInfo(nItemTmp).fOffset
                                Exit For
                            End If
                        Next
                    Else
                        bDigitalUsed(nOffset) = True
                    End If
                End If
            End If
        Next

        'Grid lines
        mfVertGridLineSpacing = (mfYRange(mnYScaleToUse(0)) / mnNUM_HLINES)

        mnBitSpacing = CType((mnGrphHeight / nDigitalScale), Integer)
        mnBit1Value = CType((mnBitSpacing * 0.8), Integer)
        mnBit0Value = CType((mnBitSpacing * 0.1), Integer)
        'Draw
        pnlChart.Invalidate()

    End Sub
    Private Sub subLabelValues()
        '********************************************************************************************
        'Description:  Update linelabels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For nItem As Integer = 0 To mnLineIndex.GetUpperBound(0)
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
            If oChk IsNot Nothing Then
                Dim sLine As String = String.Empty
                Dim sUnits As String = String.Empty
                If nItem > moItemInfo.GetUpperBound(0) Then
                    sLine = moMultiPlexCfg((nItem - moItemInfo.GetUpperBound(0)) - 1).sName
                Else
                    sLine = moItemInfo(nItem).sName
                    sUnits = moItemInfo(nItem).sUnits
                End If
                If mbShowValues Then
                    If sUnits <> String.Empty Then
                        sLine = sLine & " (" & msLineCursorValue(nItem) & " " & moItemInfo(nItem).sUnits & ")"
                    Else
                        sLine = sLine & " " & msLineCursorValue(nItem)
                    End If
                ElseIf sUnits <> String.Empty Then
                    sLine = sLine & " (" & sUnits & ")"
                End If
                oChk.ForeColor = moLineColor(nItem)
                oChk.Text = sLine
            End If
        Next
    End Sub
    Private Sub pnlChart_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlChart.Paint
        '********************************************************************************************
        'Description:  draw the chart
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/27/13  MSW     pnlChart_Paint - Check length of mnYScaleToUse before access
        '********************************************************************************************
        'Find where the last grid line is, adjust the grid size to match
        Dim x As Single
        Dim y As Single
        ' Create a font object for the scales.
        Dim oFonta As New System.Drawing.Font("Arial", 8)
        Dim oFontb As New System.Drawing.Font("Arial", 10)
        Dim oFontc As New System.Drawing.Font("Arial", 6)
        Dim sText As String = String.Empty
        Dim txtsize As SizeF
        Dim lblLeft As Integer = mptOrigin.X
        For nLine As Integer = 0 To mnNUM_VLINES
            x = mptOrigin.X + (mfHorzGridLineSpacing * nLine * mfXScale)
            If x > mnRight Then
                mnRight = CInt(x)
            End If
            'vertical gridline
            If (nLine = 0) Or (nLine = mnNUM_VLINES And mnYScaleToUse(1) >= 0) Then
                e.Graphics.DrawLine(moPenBorder, x, mptOrigin.Y, x, mnTop)
            Else
                e.Graphics.DrawLine(moPengrid, x, mptOrigin.Y, x, mnTop)
            End If
            'x-axis labels
            sText = ((mfHorzGridLineSpacing * nLine) + mfScrolloffset).ToString
            txtsize = e.Graphics.MeasureString(sText, oFonta)
            e.Graphics.DrawString(sText, oFonta, Brushes.Black, (x - txtsize.Width / 2), (mptOrigin.Y + 2))
        Next
        sText = moChartStats.sXunits
        txtsize = e.Graphics.MeasureString(sText, oFontb)
        Dim fY As Single = mptOrigin.Y + txtsize.Height + 3
        Dim fX As Single = mptOrigin.X + ((mnGrphWidth - txtsize.Width) / 2)
        e.Graphics.DrawString(sText, oFontb, Brushes.Black, fX, fY)

        For nLine As Integer = 0 To mnNUM_HLINES
            y = mptOrigin.Y - (mfVertGridLineSpacing * nLine * mfYScale(mnYScaleToUse(0)))
            If y < mnTop Then
                mnTop = CInt(y)
            End If
            'horizontal gridline
            If nLine = 0 Then
                e.Graphics.DrawLine(moPenBorder, mptOrigin.X, y, mnRight, y)
            Else
                e.Graphics.DrawLine(moPengrid, mptOrigin.X, y, mnRight, y)
            End If
            'Y-axis labels
            sText = (Math.Round(mfVertGridLineSpacing * nLine) + mfYmin(mnYScaleToUse(0))).ToString
            txtsize = e.Graphics.MeasureString(sText, oFonta)
            e.Graphics.DrawString(sText, oFonta, Brushes.Black, mptOrigin.X - txtsize.Width - 2, (y - txtsize.Height / 2))
            If (lblLeft > (mptOrigin.X - txtsize.Width - 2)) Then
                lblLeft = CInt(mptOrigin.X - txtsize.Width - 2)
            End If
            If ((mnYScaleToUse.GetUpperBound(0) >= 1) AndAlso (mnYScaleToUse(1) >= 0)) Then
                sText = (Math.Round((mfYRange(mnYScaleToUse(1)) / mnNUM_HLINES) * nLine) + mfYmin(mnYScaleToUse(1))).ToString
                txtsize = e.Graphics.MeasureString(sText, oFonta)
                e.Graphics.DrawString(sText, oFonta, Brushes.Black, mnRight + 2, (y - txtsize.Height / 2))
            End If
            If ((mnYScaleToUse.GetUpperBound(0) >= 2) AndAlso (mnYScaleToUse(2) >= 0)) Then
                sText = (Math.Round((mfYRange(mnYScaleToUse(2)) / mnNUM_HLINES) * nLine) + mfYmin(mnYScaleToUse(2))).ToString
                txtsize = e.Graphics.MeasureString(sText, oFonta)
                e.Graphics.DrawString(sText, oFonta, Brushes.Black, mnLeft2 - txtsize.Width - 2, (y - txtsize.Height / 2))
            End If
            If ((mnYScaleToUse.GetUpperBound(0) >= 3) AndAlso (mnYScaleToUse(3) >= 0)) Then
                sText = (Math.Round((mfYRange(mnYScaleToUse(3)) / mnNUM_HLINES) * nLine) + mfYmin(mnYScaleToUse(3))).ToString
                txtsize = e.Graphics.MeasureString(sText, oFonta)
                e.Graphics.DrawString(sText, oFonta, Brushes.Black, mnRight2 + 2, (y - txtsize.Height / 2))
            End If
        Next
        sText = moScaleMerge(mnYScaleToUse(0)).sUnits
        txtsize = e.Graphics.MeasureString(sText, oFontb)
        fY = mnTop - CType((txtsize.Height * 1.5), Single)
        fX = mptOrigin.X - txtsize.Width - 2
        e.Graphics.DrawString(sText, oFontb, Brushes.Black, fX, fY)
        If ((mnYScaleToUse.GetUpperBound(0) >= 1) AndAlso (mnYScaleToUse(1) >= 0)) Then
            sText = moScaleMerge(mnYScaleToUse(1)).sUnits
            txtsize = e.Graphics.MeasureString(sText, oFontb)
            fY = mnTop - CType((txtsize.Height * 1.5), Single)
            fX = mnRight + 2
            e.Graphics.DrawString(sText, oFontb, Brushes.Black, fX, fY)
        End If
        If ((mnYScaleToUse.GetUpperBound(0) >= 2) AndAlso (mnYScaleToUse(2) >= 0)) Then
            sText = moScaleMerge(mnYScaleToUse(2)).sUnits
            txtsize = e.Graphics.MeasureString(sText, oFontb)
            fY = mnTop - CType((txtsize.Height * 1.5), Single)
            fX = mnLeft2 - txtsize.Width - 2
            e.Graphics.DrawString(sText, oFontb, Brushes.Black, fX, fY)
        End If
        If ((mnYScaleToUse.GetUpperBound(0) >= 3) AndAlso (mnYScaleToUse(3) >= 0)) Then
            sText = moScaleMerge(mnYScaleToUse(3)).sUnits
            txtsize = e.Graphics.MeasureString(sText, oFontb)
            fY = mnTop - CType((txtsize.Height * 1.5), Single)
            fX = mnRight2 + 2
            e.Graphics.DrawString(sText, oFontb, Brushes.Black, fX, fY)
        End If

        If mnTableRowMax > 0 Then
            'Draw Lines
            Dim fPrevY(mnLineIndex.GetUpperBound(0)) As Single
            Dim fNextY(mnLineIndex.GetUpperBound(0)) As Single
            Dim fPrevX As Single = -1
            Dim fNextX As Single = -1

            Dim nDR As Integer = 0
            Dim bFirst As Boolean = True
            'Speed up by skipping some points when they zoom out
            Dim noffset As Integer = CInt(Math.Round(100 / mfXScale))
            If noffset < 1 Then
                noffset = 1
            End If
            'Find first row in range
            While (nDR < mnTableRowMax) AndAlso (mfTableData(moChartStats.nXaxis, nDR) < mfXmin)
                nDR = nDR + 1
            End While
            fPrevX = mptOrigin.X + ((mfTableData(moChartStats.nXaxis, nDR) - mfXmin) * mfXScale)

            Dim fTmp As Single
            Dim bLineCheck(mnLineIndex.GetUpperBound(0)) As Boolean
            For nItem As Integer = 0 To mnLineIndex.GetUpperBound(0)
                Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & nItem.ToString), CheckBox)
                bLineCheck(nItem) = oChk.Checked
                If oChk.Checked Then

                    If nItem > moItemInfo.GetUpperBound(0) Then
                        'Multiplex
                        With moMultiPlexCfg((nItem - moItemInfo.GetUpperBound(0)) - 1)
                            Dim nMultiVal As Integer = CType(mfTableData(.nItem, nDR), Integer)
                            Dim nVal As Integer = 0
                            'Dim nBit As Integer = CType(mOdtTable.Rows.Item(nDR).Item(mnLineIndex(nItem)), Integer)
                            If (nMultiVal And CType((2 ^ .nBit), Integer)) <> 0 Then
                                nVal = mnBit1Value
                            Else
                                nVal = mnBit0Value
                            End If
                            y = mptOrigin.Y - (moItemInfo(.nItem).fOffset + .nBit) * mnBitSpacing
                            fPrevY(nItem) = y - (nVal)
                            y = y - CType((mnBit0Value + mnBit1Value) / 2, Single)
                            sText = moMultiPlexCfg((nItem - moItemInfo.GetUpperBound(0)) - 1).sName
                            txtsize = e.Graphics.MeasureString(sText, oFonta)
                            e.Graphics.DrawString(sText, oFontc, Brushes.Black, mptOrigin.X - 10 - txtsize.Width, (y - txtsize.Height / 2))
                        End With

                    Else
                        If mnLineScaleIndex(nItem) >= 0 Then
                            fTmp = (mfTableData(mnLineIndex(nItem), nDR) - moItemInfo(nItem).fOffset) * moItemInfo(nItem).fScale
                            fPrevY(nItem) = mptOrigin.Y - ((fTmp - mfYmin(mnLineScaleIndex(nItem))) * mfYScale(mnLineScaleIndex(nItem)))
                        Else
                            'Bit
                            Dim nVal As Integer = 0
                            If mfTableData(mnLineIndex(nItem), nDR) > 0.5 Then
                                nVal = mnBit1Value
                            Else
                                nVal = mnBit0Value
                            End If
                            y = mptOrigin.Y - (moItemInfo(nItem).fOffset * mnBitSpacing)
                            fPrevY(nItem) = y - (nVal)
                            y = y - CType((mnBit0Value + mnBit1Value) / 2, Single)
                            sText = moItemInfo(nItem).sName
                            txtsize = e.Graphics.MeasureString(sText, oFonta)
                            e.Graphics.DrawString(sText, oFontc, Brushes.Black, mptOrigin.X - 10 - txtsize.Width, (y - txtsize.Height / 2))
                        End If
                    End If
                End If
            Next

            'Go through rows to draw
            Dim bSetLabels As Boolean = False
            Dim bMouseCursorFound As Boolean = False
            Dim oPenLine As Pen = New Pen(Color.LimeGreen, 3)
            While (nDR < mnTableRowMax) AndAlso (mfTableData(moChartStats.nXaxis, nDR) < mfXmax)
                fNextX = mptOrigin.X + ((mfTableData(moChartStats.nXaxis, nDR) - mfXmin) * mfXScale)
                If mbShowValues AndAlso (bMouseCursorFound = False) AndAlso ((fNextX > mnMousePos) OrElse (nDR > (mnTableRowMax - noffset))) Then
                    bMouseCursorFound = True
                    'Found the cursor position
                    moPengrid.DashStyle = Drawing2D.DashStyle.DashDotDot
                    e.Graphics.DrawLine(moPengrid, fNextX, mptOrigin.Y, fNextX, mnTop)
                    moPengrid.DashStyle = Drawing2D.DashStyle.Solid
                    If mbMouseDown Then
                        'update the labels
                        mnMouseDownDR = nDR
                        bSetLabels = True
                    End If
                End If
                For nItem As Integer = 0 To mnLineIndex.GetUpperBound(0)

                    If bLineCheck(nItem) Or bSetLabels Then
                        oPenLine.Color = moLineColor(nItem)
                        If nItem > moItemInfo.GetUpperBound(0) Then
                            'Multiplex
                            With moMultiPlexCfg((nItem - moItemInfo.GetUpperBound(0)) - 1)
                                Dim nMultiVal As Integer = CType(mfTableData(.nItem, nDR), Integer)
                                Dim nVal As Integer = 0
                                'Dim nBit As Integer = CType(mOdtTable.Rows.Item(nDR).Item(mnLineIndex(nItem)), Integer)
                                If (nMultiVal And CType((2 ^ .nBit), Integer)) <> 0 Then
                                    nVal = mnBit1Value
                                    If bSetLabels Then
                                        msLineCursorValue(nItem) = 1.ToString
                                    End If
                                Else
                                    nVal = mnBit0Value
                                    If bSetLabels Then
                                        msLineCursorValue(nItem) = 0.ToString
                                    End If
                                End If
                                If bLineCheck(nItem) Then
                                    fNextY(nItem) = mptOrigin.Y - ((moItemInfo(.nItem).fOffset + .nBit) * mnBitSpacing) - (nVal)
                                    e.Graphics.DrawLine(oPenLine, fPrevX, fPrevY(nItem), fNextX, fNextY(nItem))
                                    fPrevY(nItem) = fNextY(nItem)
                                End If
                            End With
                        Else
                            If mnLineScaleIndex(nItem) >= 0 Then
                                fTmp = (mfTableData(mnLineIndex(nItem), nDR) - moItemInfo(nItem).fOffset) * moItemInfo(nItem).fScale
                                If bSetLabels Then
                                    msLineCursorValue(nItem) = fTmp.ToString
                                End If
                                If bLineCheck(nItem) Then
                                    fNextY(nItem) = mptOrigin.Y - ((fTmp - mfYmin(mnLineScaleIndex(nItem))) * mfYScale(mnLineScaleIndex(nItem)))
                                    e.Graphics.DrawLine(oPenLine, fPrevX, fPrevY(nItem), fNextX, fNextY(nItem))
                                    fPrevY(nItem) = fNextY(nItem)
                                End If
                            Else
                                'Bit
                                Dim nBit As Integer = CType(mfTableData(mnLineIndex(nItem), nDR), Integer)
                                If bSetLabels Then
                                    msLineCursorValue(nItem) = nBit.ToString
                                End If
                                Dim nVal As Integer = 0
                                If nBit = 1 Then
                                    nVal = mnBit1Value
                                Else
                                    nVal = mnBit0Value
                                End If
                                If bLineCheck(nItem) Then
                                    fNextY(nItem) = mptOrigin.Y - (moItemInfo(nItem).fOffset * mnBitSpacing) - (nVal)
                                    e.Graphics.DrawLine(oPenLine, fPrevX, fPrevY(nItem), fNextX, fNextY(nItem))
                                    fPrevY(nItem) = fNextY(nItem)
                                End If
                            End If
                        End If
                    End If
                Next

                If bSetLabels Then
                    subLabelValues()
                    bSetLabels = False
                End If
                fPrevX = fNextX
                nDR = nDR + noffset
            End While

        End If
    End Sub

    Private Sub pnlChart_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnlChart.MouseDown
        '********************************************************************************************
        'Description:  Mark a vertical line and display values in the labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbShowValues = True
        mbMouseDown = True
        mnMousePos = e.Location.X
        mnMouseStartPos = mnMousePos
        pnlChart.Invalidate()
    End Sub
    Private Sub pnlChart_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnlChart.MouseUp
        '********************************************************************************************
        'Description:  Mark a vertical line and display values in the labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbMouseDown Then
            If Math.Abs(mnMouseStartPos - e.Location.X) > mnMouseMinDiff Then
                mnMousePos = e.Location.X
                mnMouseStartPos = mnMousePos
                pnlChart.Invalidate()
            End If
            mbMouseDown = False
        End If
    End Sub
    Private Sub pnlChart_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnlChart.MouseMove
        '********************************************************************************************
        'Description:  Mark a vertical line and display values in the labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbMouseDown Then
            If Math.Abs(mnMouseStartPos - e.Location.X) > mnMouseMinDiff Then
                mnMousePos = e.Location.X
                mnMouseStartPos = mnMousePos
                pnlChart.Invalidate()
            End If
        End If
    End Sub

    Private Sub dgConfig_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgConfig.CellDoubleClick
        '********************************************************************************************
        'Description:  Line color selection
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If e.ColumnIndex = mnCFG_COL_COLOR Then
            Dim oChk As CheckBox = DirectCast(spltChart.Panel1.Controls("chkLine" & e.RowIndex.ToString), CheckBox)
            Dim cd As New ColorDialog
            Try
                cd.AllowFullOpen = True
                cd.AnyColor = True
                cd.FullOpen = True
                cd.Color = oChk.ForeColor
                If cd.ShowDialog() = DialogResult.OK Then
                    moLineColor(mnCheckBoxMouseDown) = cd.Color
                    dgConfig.Rows(e.RowIndex).Cells(msCFG_COL_COLOR).Style.ForeColor = cd.Color
                    dgConfig.Rows(e.RowIndex).Cells(msCFG_COL_COLOR).Value = ColorToString(cd.Color)
                End If
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
            End Try


        End If
    End Sub


End Class