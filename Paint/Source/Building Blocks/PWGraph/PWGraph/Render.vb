' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: Render
'
' Description: draw me
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Adapted from open source written by BRUCE SMITH
'
' Modification history:
'
'    Date       By      Reason                                                          Version
'    05/22/12   MSW     Prevent a height of 0 for really small bars                     4.1.4.0
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions    4.01.07.00
'******************************************************************************************************


Imports System.IO
Imports System.Threading
Imports System.drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports PWGraph.Pie
Imports PWGraph.Bar
Imports PWGraph.Line


Public Class Render

#Region " Declares "

    Public GraphPadding As New Base.Padding(4, 4, 10)
    Public goBitMap As Bitmap

#End Region
#Region " Routines "

    Private Function AutoSize(ByVal Graph As Base.BaseGraph, ByVal ChunkCollection As Base.BaseChunkCollection) As Size
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lheight, lwidth As Integer

        Select Case Graph.ChartType
            Case Base.eChartType.Pie
                Return New Size(300, 300)
            Case Base.eChartType.Bar, Base.eChartType.Line
                If Graph.GraphAlign = Base.eBarTypes.HorizontalLeft Or Graph.GraphAlign = Base.eBarTypes.HorizontalRight Then
                    lwidth = ChunkCollection.Count * 55
                    lheight = CInt((lwidth * 0.75))
                Else
                    lheight = ChunkCollection.Count * 55
                    lwidth = CInt((lheight * 0.75))
                End If
        End Select
    End Function
    Private Function CalcGraph(ByVal Graph As Base.BaseGraph) As Rectangle
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim lBorder As Integer = CInt(GraphPadding.Border * 2.8)
        Dim retRect As New Rectangle()
        retRect.X = lBorder
        retRect.Y = lBorder
        retRect.Width = Graph.ImageSize.Width - (lBorder * 2)
        retRect.Height = Graph.ImageSize.Height - (lBorder * 2)

        Return retRect

    End Function
    Private Function CalcGridRect(ByVal Graph As Base.BaseGraph) As Rectangle
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oRect As Rectangle = CalcGraph(Graph)
        Return New Rectangle(oRect.X, oRect.Y, oRect.Width + 3, oRect.Height)
    End Function
    Private Overloads Function DrawBarChart(ByVal BarChart As BarChart) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If BarChart.AutoSize Then
            BarChart.ImageSize = AutoSize(BarChart, BarChart.BarSliceCollection)
        End If

        goBitMap = GetBitmap(BarChart.ImageSize)

        Dim g As Graphics = Graphics.FromImage(goBitMap)
        g.SmoothingMode = SmoothingMode.HighSpeed ' antialias objects  
        g.Clear(BarChart.Color)  ' blank the image  

        BarChart.GraphRect = CalcGraph(BarChart)

        DrawGrid(BarChart, g)

        g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  

        DrawVerticalBar(BarChart, g)

        Select Case BarChart.Alignment
            Case Base.eBarTypes.HorizontalLeft
                goBitMap.RotateFlip(RotateFlipType.Rotate270FlipXY)
            Case Base.eBarTypes.HorizontalRight
                goBitMap.RotateFlip(RotateFlipType.Rotate90FlipX)
            Case Base.eBarTypes.VerticalBottom

            Case Base.eBarTypes.VerticalTop
                goBitMap.RotateFlip(RotateFlipType.Rotate180FlipX)
        End Select

        Return goBitMap

    End Function
    Private Overloads Function DrawBarChart(ByVal BarChart As BarChart, ByVal retStream As Stream) As Object
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim b As System.Drawing.Bitmap = CType(DrawChart(BarChart), Bitmap)
            b.Save(retStream, ImageFormat.Jpeg)
            b.Dispose()
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    Public Overloads Function DrawChart(ByVal Graph As Base.BaseGraph) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case Graph.ChartType
            Case Base.eChartType.Bar
                Return DrawBarChart(CType(Graph, BarChart))
            Case Base.eChartType.Line
                Return DrawLineChart(CType(Graph, LineChart))
            Case Base.eChartType.Pie
                Return DrawPieChart(CType(Graph, PieChart))

            Case Else
                Return Nothing
        End Select

    End Function
    Public Overloads Sub DrawChart(ByVal Graph As Base.BaseGraph, ByVal retStream As Stream)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case Graph.ChartType
            Case Base.eChartType.Bar
                DrawBarChart(CType(Graph, BarChart), retStream)
            Case Base.eChartType.Line
                DrawLineChart(CType(Graph, LineChart), retStream)
            Case Base.eChartType.Pie
                DrawPieChart(CType(Graph, PieChart), retStream)
        End Select
    End Sub
    Private Sub DrawGrid(ByVal Chart As Base.BaseGraph, ByRef g As Graphics)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBrush As New SolidBrush(Color.LightGray)
        Dim recRect As Rectangle = CalcGridRect(Chart)
        Dim oPen As New Pen(Color.LightGray)
        Dim tInfo As Base.MathInfo = Nothing
        Dim lTopValue As Long
        Dim fSteps As Single = 0.0
        Dim tStep As Integer = 15

        g.DrawRectangle(oPen, recRect)

        If Chart.ChartType = Base.eChartType.Bar Then
            lTopValue = CLng(CType(Chart, BarChart).BarSliceCollection.MaxValue)
            tInfo = getMathInfo(lTopValue)
        Else
            lTopValue = CLng(CType(Chart, LineChart).LinePlotCollection.MaxValue)
            tInfo = getMathInfo(lTopValue)
        End If


        fSteps = CSng((recRect.Height / tStep))

        Dim aX As Single = recRect.X 'Starting Vertical
        Dim aY As Single = recRect.Y 'Starting Horizontal
        Dim bX As Single = recRect.Right 'Ending Vertical
        Dim bY As Single = 0 'Ending Horizontal

        Dim Line_Value As Long = CLng((lTopValue / tStep))
        Dim lCount As Long

        For dChart As Double = aY To (recRect.Height + aY) Step fSteps

            aY = CSng(dChart)
            bY = CSng(dChart)

            If lCount Mod 2 = 0 Then
                oPen = New Pen(Color.LightGray, 1)
            Else
                oPen = New Pen(Color.LightGray, 1)
            End If

            g.DrawLine(oPen, aX, aY, bX, bY)
            lCount += 1
            lTopValue -= Line_Value
            If lCount > 5000 Then Exit For

        Next

    End Sub
    Public Overloads Function DrawKey(ByVal Graph As Base.BaseGraph) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim szSize As New System.Drawing.Size(0, 0)

        Return DrawKey(Graph, szSize)
    End Function
    Public Overloads Function DrawKey(ByVal Graph As Base.BaseGraph, ByVal szSize As System.Drawing.Size) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBMap As Bitmap
        Dim oColBaseChunk As Base.BaseChunkCollection = Nothing
        Dim oBrush As New SolidBrush(Graph.KeyTitleColor)
        Dim nMinWidth As Integer = 0

        Select Case Graph.ChartType
            Case Base.eChartType.Bar
                oColBaseChunk = CType(Graph, BarChart).BarSliceCollection
            Case Base.eChartType.Line
                oColBaseChunk = CType(Graph, LineChart).LinePlotCollection
            Case Base.eChartType.Pie
                oColBaseChunk = CType(Graph, PieChart).PieSliceCollection
        End Select

        If ((szSize.Width <= 0) Or (szSize.Height <= 0)) Then
            If Graph.KeyTitle.Length > oColBaseChunk.MaxKeyNameLength Then
                nMinWidth = Graph.KeyTitle.Length * Graph.KeyFontSize
            Else
                nMinWidth = oColBaseChunk.MaxKeyNameLength * Graph.KeyFontSize
            End If
            szSize.Width = (nMinWidth)
            szSize.Height = ((oColBaseChunk.Count * Graph.KeyFontSize * 2) + (2 * Graph.KeyTitleFontSize))
        Else
            Dim nHeight As Integer = szSize.Height
            Dim nFontSize As Integer = CInt(szSize.Height / (oColBaseChunk.Count * 2 + 4))

            If nFontSize < Graph.KeyFontSize Then
                Graph.KeyFontSize = nFontSize
            End If
            If Graph.KeyFontSize < 2 Then
                Graph.KeyFontSize = 2
            End If
            nFontSize = CInt(nFontSize * 1.3)
            If nFontSize < Graph.KeyTitleFontSize Then
                Graph.KeyTitleFontSize = nFontSize
            End If
        End If

        oBMap = GetBitmap(szSize)
        Dim g As Graphics = Graphics.FromImage(oBMap)
        g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  
        g.Clear(Graph.KeyBackColor)  ' blank the image  

        If Graph.KeyTitle <> String.Empty Then
            Dim titleRectF As New RectangleF(0, 0, szSize.Width, 25)
            Dim titleFormat As New StringFormat()
            Dim titleFont As New Font(Graph.KeyTitleFontName, Graph.KeyTitleFontSize, Graph.KeyTitleFontStyle)
            oBrush.Color = Graph.KeyTitleColor
            titleFormat.Alignment = StringAlignment.Near 'StringAlignment.Center
            titleFormat.LineAlignment = StringAlignment.Near 'StringAlignment.Center
            Dim szTmp As SizeF = g.MeasureString(Graph.KeyTitle, titleFont, titleRectF.Size, titleFormat)
            While (((szTmp.Width > titleRectF.Width) Or (szTmp.Height > titleRectF.Height)) And (Graph.KeyTitleFontSize > 5))
                Graph.KeyTitleFontSize = Graph.KeyTitleFontSize - 1
                titleFont = New Font(Graph.KeyTitleFontName, Graph.KeyTitleFontSize, Graph.KeyTitleFontStyle)
                szTmp = g.MeasureString(Graph.KeyTitle, titleFont, titleRectF.Size)
            End While
            g.DrawString(Graph.KeyTitle, titleFont, oBrush, titleRectF, titleFormat)
            titleRectF = Nothing
            titleFormat.Dispose()
            titleFont.Dispose()
        End If

        Dim oFont As Font
        Dim oApproxWidth As Integer = 0
        Dim oPen As New Pen(Color.Black, 2)
        Dim nKeyCount As Integer = oColBaseChunk.Count

        For Each oChunk As Base.BaseChunk In oColBaseChunk
            oFont = New Font(Graph.KeyFontName, Graph.KeyFontSize, Graph.KeyFontStyle)
            If Len(oChunk.KeyName) > oApproxWidth Then oApproxWidth = Len(oChunk.KeyName)

            oPen.Width = 1
            oBrush.Color = oChunk.Color
            g.FillRectangle(oBrush, 5, nKeyCount + 24, 10, 10)
            oPen.Color = Color.Black
            g.DrawRectangle(oPen, 5, nKeyCount + 24, 10, 10)
            oBrush.Color = Color.Black
            Dim nFontSize As Integer = Graph.KeyFontSize
            Dim szTmp As SizeF = g.MeasureString(Graph.KeyTitle, oFont, oBMap.Size)
            While ((szTmp.Width > oBMap.Width) And (nFontSize > 5))
                nFontSize = nFontSize - 1
                oFont = New Font(Graph.KeyFontName, nFontSize, Graph.KeyFontStyle)
                szTmp = g.MeasureString(Graph.KeyTitle, oFont, oBMap.Size)
            End While

            g.DrawString(oChunk.KeyName, oFont, oBrush, 17, nKeyCount + 21)
            nKeyCount += CInt(Graph.KeyFontSize * 1.7) '----This is the spacing between the Keys
        Next

        Return oBMap

    End Function
    Public Overloads Sub DrawKey(ByVal Graph As Base.BaseGraph, ByVal retStream As Stream)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim b As System.Drawing.Bitmap = CType(DrawChart(Graph), Bitmap)
            b.Save(retStream, ImageFormat.Jpeg)
            b.Dispose()
        Catch ex As Exception

        End Try

    End Sub
    Private Overloads Function DrawLineChart(ByVal LineChart As LineChart) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If LineChart.AutoSize Then
            LineChart.ImageSize = AutoSize(LineChart, LineChart.LinePlotCollection)
        End If

        goBitMap = GetBitmap(LineChart.ImageSize)

        Dim g As Graphics = Graphics.FromImage(goBitMap)
        g.SmoothingMode = SmoothingMode.HighQuality ' antialias objects  
        g.Clear(LineChart.Color)  ' blank the image  

        LineChart.GraphRect = CalcGraph(LineChart)

        DrawVerticalPlots(LineChart, g)

        If LineChart.Alignment = Base.eLineTypes.Horizontal Then
            goBitMap.RotateFlip(RotateFlipType.Rotate270FlipNone)
        End If


        Return goBitMap

    End Function
    Private Overloads Function DrawLineChart(ByVal LineChart As LineChart, ByVal retStream As Stream) As Object
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim b As System.Drawing.Bitmap = CType(DrawChart(LineChart), Bitmap)
            b.Save(retStream, ImageFormat.Jpeg)
            b.Dispose()

        Catch ex As Exception

        End Try
        Return Nothing

    End Function
    Private Overloads Function DrawPieChart(ByVal PieChart As PieChart) As System.Drawing.Image
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With PieChart
            If .AutoSize Then
                .ImageSize = AutoSize(PieChart, .PieSliceCollection)
            End If

            goBitMap = GetBitmap(.ImageSize)
            .GraphRect = CalcGraph(PieChart)

            'Change to percent if needed.
            If .ValueType = Base.eChartValueType.ValueTotal Then
                .PieSliceCollection.CalcPercent()
            Else
                .PieSliceCollection.CalcPercent(True)
            End If

            Dim nBoarder As Integer = GraphPadding.Border
            Dim nTotalBoarder As Integer = (nBoarder * 3)
            .GraphRect = New Rectangle(nBoarder, nBoarder, _
                                goBitMap.Width - nTotalBoarder, goBitMap.Height - nTotalBoarder)

            '   So I have a bitmap on which I can draw, and I have the pie percents now 
            'Its time to draw some stuff.

            Dim g As Graphics = Graphics.FromImage(goBitMap)
            Dim fStartAngle As Single = 0.0F
            Dim fSweepAngle As Single = 0.0F
            Dim oBrush As New SolidBrush(.Color)

            g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  
            g.Clear(.Color)  ' blank the image  

            Dim oPiePiece As PieSlice
            Dim gPen As New Pen(Color.Black, 1)
            Dim nDiameter As Integer = CInt(.Diameter)
            Dim nThickness As Integer = CInt(.Thickness)
            Dim pieRect As Rectangle

            For j As Integer = CInt(nDiameter * nThickness * 0.01F) To 0 Step -1
                For Each oPiePiece In PieChart.PieSliceCollection
                    'Flip through the pieces Build the pie
                    fSweepAngle = 360 * (oPiePiece.PiecePercent / 100)
                    pieRect = New Rectangle(PieChart.GraphRect.X, _
                            CInt(PieChart.GraphRect.Y + CSng(j)), nDiameter, nDiameter)

                    Dim hBrush As New HatchBrush(HatchStyle.Percent50, oPiePiece.Color)
                    g.FillPie(hBrush, pieRect, fStartAngle, fSweepAngle)
                    fStartAngle += fSweepAngle
                Next
            Next

            'Top Layer of Circle
            For j As Integer = CInt(nDiameter * 0.01F) To 0 Step -1
                For Each oPiePiece In PieChart.PieSliceCollection
                    'Flip through the pieces Build the pie
                    fSweepAngle = 360 * (oPiePiece.PiecePercent / 100)
                    pieRect = New Rectangle(PieChart.GraphRect.X, _
                            CInt(PieChart.GraphRect.Y + CSng(j)), nDiameter, nDiameter)

                    oBrush.Color = oPiePiece.Color

                    g.FillPie(oBrush, pieRect, fStartAngle, fSweepAngle)
                    fStartAngle += fSweepAngle
                Next
            Next

            g.Dispose()
            oBrush.Dispose()

        End With

        Return goBitMap

    End Function
    Private Overloads Sub DrawPieChart(ByVal PieChart As PieChart, ByVal retStream As Stream)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim b As System.Drawing.Bitmap = CType(DrawChart(PieChart), Bitmap)
            b.Save(retStream, ImageFormat.Jpeg)
            b.Dispose()

        Catch ex As Exception

        End Try
    End Sub
    Private Sub DrawValues(ByVal chart As Base.BaseGraph, ByRef g As Graphics)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case chart.GraphAlign
            Case Base.eBarTypes.HorizontalLeft

            Case Base.eBarTypes.HorizontalRight

            Case Base.eBarTypes.VerticalBottom

            Case Base.eBarTypes.VerticalTop

        End Select
    End Sub
    Private Sub DrawVerticalBar(ByVal BarChart As BarChart, ByRef g As Graphics)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      BY      Reason
        ' 05/22/12  MSW     prevent a height of 0 for really small bars
        '********************************************************************************************
        Dim oSlice As BarSlice
        Dim oBrush As New SolidBrush(BarChart.Color)
        Dim nTotalSpacing As Integer = BarChart.GraphRect.Width - (GraphPadding.CellSpacing * _
                                                            (BarChart.BarSliceCollection.Count - 1))
        nTotalSpacing -= (GraphPadding.Border * 2)

        Dim nWidth As Integer = CInt((nTotalSpacing / BarChart.BarSliceCollection.Count))
        Dim nHeight As Integer
        Dim nX As Integer = BarChart.GraphRect.Left + (GraphPadding.Border)
        Dim nY As Integer = BarChart.GraphRect.Bottom
        Dim recLBar As Rectangle
        Dim recRBar As Rectangle
        Dim recBar As Rectangle
        Dim recShadow As Rectangle

        For Each oSlice In BarChart.BarSliceCollection
            nHeight = CInt((BarChart.GraphRect.Height * _
                                        (oSlice.Value / BarChart.BarSliceCollection.MaxValue)))
            If nHeight <= 4 Then 'MSW 5/22/12
                nHeight = 5
            End If
            If nWidth <= 5 Then 'MSW 5/22/12
                nWidth = 6
            End If
            nY = ((BarChart.GraphRect.Bottom) - nHeight)

            recBar = New Rectangle(nX, nY, nWidth - 2, nHeight)
            recLBar = New Rectangle(nX, nY, CInt((nWidth / 2)), nHeight)
            recRBar = New Rectangle(CInt(nX + (nWidth / 2) - 2), nY, CInt(nWidth / 2), nHeight)

            Dim lgBrush As New LinearGradientBrush(recLBar, getLightColor(oSlice.Color, 80), _
                            getDarkColor(oSlice.Color, 55), LinearGradientMode.BackwardDiagonal)
            Dim rgBrush As New LinearGradientBrush(recRBar, getLightColor(oSlice.Color, 80), _
                            getDarkColor(oSlice.Color, 55), LinearGradientMode.ForwardDiagonal)
            'Shadow Rectangle , could be a switch
            recShadow = New Rectangle(nX + 2, nY - 3, nWidth, nHeight + 3)
            'Shadow color
            oBrush.Color = Color.LightGray
            'Draw Shadow
            g.FillRectangle(oBrush, recShadow)
            'Kill the rect
            recShadow = Nothing
            'Get the bar color
            'Draw the bar.
            g.FillRectangle(lgBrush, recLBar)
            g.FillRectangle(rgBrush, recRBar)
            'Outline the bar, could be a switch.
            Dim gPen As New Pen(getDarkColor(oSlice.Color, 1), 1)

            g.DrawRectangle(gPen, recBar)
            'Get the next x coord.
            nX += (nWidth + GraphPadding.CellPadding)
        Next

        oBrush.Dispose()

    End Sub
    Private Sub DrawVerticalPlots(ByVal LineChart As LineChart, ByRef g As Graphics)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      nY      Reason
        '********************************************************************************************
        Dim LinePlot As Line.LineSlice
        Dim LinePointCurr As Point = Nothing
        Dim LinePointLast As Point = Nothing
        Dim LastDotColor As Color

        Dim nWorkingAreaHeight As Integer
        Dim nWorkingAreaWidth As Integer
        Dim nX As Integer = 0
        Dim nY As Integer = GraphPadding.Border
        Dim oBrush As New SolidBrush(Color.Blue)
        Dim oPen As New Pen(LineChart.LineColor, 1)

        Dim PlotDotSize As Integer = 8

        nWorkingAreaHeight = (LineChart.GraphRect.Height - (GraphPadding.Border * 2)) - PlotDotSize
        nWorkingAreaWidth = (LineChart.GraphRect.Width - (GraphPadding.Border * 2)) - PlotDotSize

        Dim nVertSpacing As Integer = CInt(nWorkingAreaHeight / LineChart.LinePlotCollection.Count)
        Dim bFirstPass As Boolean = True
        nY = CInt((nVertSpacing / 2) + GraphPadding.Border)


        For Each LinePlot In LineChart.LinePlotCollection
            nX = CInt(nWorkingAreaWidth * Math.Round((LinePlot.Value / _
                                    LineChart.LinePlotCollection.MaxValue), 2))
            'Draw the colored dot...
            LinePointCurr = New Point(nX, nY)

            If bFirstPass Then
                bFirstPass = False
            Else
                'Draw a line from the last plot.
                oBrush.Color = LastDotColor
                g.DrawLine(oPen, LinePointAdd(LinePointLast, PlotDotSize), LinePointAdd(LinePointCurr, PlotDotSize))
                g.FillEllipse(oBrush, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            End If
            LinePointLast = LinePointCurr
            oBrush.Color = LinePlot.Color
            g.FillEllipse(oBrush, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            'Highlight the dot
            g.DrawEllipse(oPen, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            LastDotColor = LinePlot.Color

            nY += nVertSpacing

        Next


    End Sub
    Private Function GetBitmap(ByVal bmSize As Size) As System.Drawing.Bitmap
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return New System.Drawing.Bitmap(bmSize.Width, bmSize.Height, PixelFormat.Format24bppRgb)
    End Function
    Private Function getDarkColor(ByVal c As Color, ByVal d As Byte) As Color
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0

        If (c.R > d) Then r = (c.R - d)
        If (c.G > d) Then g = (c.G - d)
        If (c.B > d) Then b = (c.B - d)

        Dim c1 As Color = Color.FromArgb(r, g, b)
        Return c1

    End Function
    Private Function getLightColor(ByVal c As Color, ByVal d As Byte) As Color
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim r As Byte = 255
        Dim g As Byte = 255
        Dim b As Byte = 255

        If (CInt(c.R) + CInt(d) <= 255) Then r = (c.R + d)
        If (CInt(c.G) + CInt(d) <= 255) Then g = (c.G + d)
        If (CInt(c.B) + CInt(d) <= 255) Then b = (c.B + d)

        Dim c2 As Color = Color.FromArgb(r, g, b)
        Return c2

    End Function
    Protected Function getMathInfo(ByRef maxValue As Long) As Base.MathInfo
        Dim retInfo As New Base.MathInfo

        With retInfo
            Select Case maxValue
                Case 0 To 99
                    .Divisor = 1
                    .Multiplier = "1"
                    .Max += 5
                    .Max = CLng(Math.Round(.Max / 10) * 10)
                Case 100 To 999
                    .Divisor = 10
                    .Multiplier = "10"
                    .Max += 50
                    .Max = CLng(Math.Round(.Max / 100) * 100)
                Case 1000 To 9999
                    .Divisor = 100
                    .Multiplier = "100"
                    .Max += 500
                    .Max = CLng(Math.Round(.Max / 1000) * 1000)
                Case 10000 To 99999
                    .Divisor = 1000
                    .Multiplier = "1K"
                    .Max += 5000
                    .Max = CLng(Math.Round(.Max / 10000) * 10000)
                Case 100000 To 999999
                    .Divisor = 10000
                    .Multiplier = "10K"
                    .Max += 50000
                    .Max = CLng(Math.Round(.Max / 100000) * 100000)
                Case 1000000 To 9999999
                    .Divisor = 100000
                    .Multiplier = "100K"
                    .Max += 500000
                    .Max = CLng(Math.Round(.Max / 1000000) * 1000000)
                Case 10000000 To 99999999
                    .Divisor = 1000000
                    .Multiplier = "1M"
                    .Max += 5000000
                    .Max = CLng(Math.Round(.Max / 10000000) * 10000000)
                Case 100000000 To 999999999
                    .Divisor = 10000000
                    .Multiplier = "10M"
                    .Max += 50000000
                    .Max = CLng(Math.Round(.Max / 100000000) * 100000000)
                Case 1000000000 To 9999999999
                    .Divisor = 100000000
                    .Multiplier = "100M"
                    .Max += 500000000
                    .Max = CLng(Math.Round(.Max / 1000000000) * 1000000000)
                Case 10000000000 To 99999999999
                    .Divisor = 1000000000
                    .Multiplier = "1B"
                    .Max += 5000000000
                    .Max = CLng(Math.Round(.Max / 10000000000) * 10000000000)
                Case 100000000000 To 999999999999
                    .Divisor = 10000000000
                    .Multiplier = "10B"
                    .Max += 50000000000
                    .Max = CLng(Math.Round(.Max / 100000000000) * 100000000000)
                Case 1000000000000 To 9999999999999
                    .Divisor = 100000000000
                    .Multiplier = "100B"
                    .Max += 500000000000
                    .Max = CLng(Math.Round(.Max / 1000000000000) * 1000000000000)
            End Select
        End With

        Return retInfo

    End Function
    Private Function LinePointAdd(ByVal LinePoint As Point, ByVal PlotDotSize As Integer) As Point
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return New Point(CInt(LinePoint.X + (PlotDotSize / 2)), CInt(LinePoint.Y + (PlotDotSize / 2)))
    End Function

#End Region

End Class
