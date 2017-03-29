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
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions    4.01.07.00
'******************************************************************************************************

Namespace Pie
    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
#Region " Class PieChart"

    Public Class PieChart
        Inherits Base.BaseGraph
        'This will hold the pie pieces.
        Public PieSliceCollection As New Pie.PiePieceCollection()
        Public Diameter As Single
        Public Thickness As Single
        Sub New()
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Pie
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size)
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Pie
            ImageSize = ImgSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean)
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Pie
            AutoSize = ImgAutoSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean, ByVal PiePieceCollection As PiePieceCollection)
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Pie
            AutoSize = ImgAutoSize
            PieSliceCollection = PiePieceCollection
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size, ByVal PiePieceCollection As PiePieceCollection)
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Pie
            PieSliceCollection = PiePieceCollection
        End Sub
    End Class

#End Region
    'A pie slice can be created alone or by just using the collection 
    'inside PieChart
#Region " Class PieSlice "

    Public Class PieSlice
        Inherits Base.BaseChunk
        Private mdSweepAngle As Decimal = 0D
        Private mdPiecePercent As Decimal = 0D

        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()

            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub

        Friend Property PiecePercent() As Decimal
            Get
                Return mdPiecePercent
            End Get
            Set(ByVal Value As Decimal)
                mdPiecePercent = Value
            End Set
        End Property
        Friend Property SweepAngle() As Decimal
            Get
                Return (360 * (mdPiecePercent / 100))
            End Get
            Set(ByVal Value As Decimal)
                mdSweepAngle = Value
            End Set
        End Property
    End Class

#End Region
    'A pie collection can be created all alone but it is a part of PieChart
#Region " Class PiePieceCollection "

    Public Class PiePieceCollection
        Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub

        Public Shadows Function Add(ByVal oPiePiece As PieSlice) As Integer
            Return MyBase.Add(oPiePiece)
        End Function
        Friend Sub CalcPercent()
            Dim oBaseChunk As PieSlice
            Dim lTvalue As Decimal = TotalValue
            For Each oBaseChunk In List
                Try
                    oBaseChunk.PiecePercent = Decimal.Round(((oBaseChunk.Value / lTvalue) * 100), 2)
                Catch

                End Try
            Next
        End Sub
        Friend Sub CalcPercent(ByVal CalcIsPercent As Boolean)
            Dim oBaseChunk As PieSlice
            Dim lTvalue As Decimal = TotalValue
            For Each oBaseChunk In List
                Try
                    oBaseChunk.PiecePercent = oBaseChunk.Value
                Catch

                End Try
            Next
        End Sub
    End Class
#End Region
 
End Namespace
