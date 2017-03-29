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

Namespace Line

    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
#Region " Class LineChart "

    Public Class LineChart
        Inherits Base.BaseGraph
        Private _Alignment As Base.eLineTypes = Base.eLineTypes.Horizontal
        Private _LineColor As System.Drawing.Color = System.Drawing.Color.Black

        Public LinePlotCollection As New Line.LinePlotCollection()
        Public Property Alignment() As Base.eLineTypes
            Get
                Return _Alignment
            End Get
            Set(ByVal Value As Base.eLineTypes)
                _Alignment = Value
            End Set
        End Property
        Public Property LineColor() As System.Drawing.Color
            Get
                Return _LineColor
            End Get
            Set(ByVal Value As System.Drawing.Color)
                _LineColor = Value
            End Set
        End Property
        Sub New()
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Line
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size)
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Line
            ImageSize = ImgSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean)
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Line
            AutoSize = ImgAutoSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean, ByVal LinePieceCollection As LinePlotCollection)
            MyBase.New()
            MyBase.ChartType = Base.eChartType.Line
            AutoSize = ImgAutoSize
            LinePieceCollection = LinePieceCollection
        End Sub
        Sub New(ByVal LinePlotCollection As LinePlotCollection)
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Line
            LinePlotCollection = LinePlotCollection
        End Sub
    End Class

#End Region

    'A LineSlice can be created alone or by just using the collection 
    'inside Chart
#Region " Class LineSlice "

    Public Class LineSlice
        Inherits Base.BaseChunk
        Friend Point As System.Drawing.Point = New System.Drawing.Point(0, 0)

        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()
            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub
    End Class

#End Region

    'A LinePlotCollection can be created all alone but it is a part of LineChart
#Region " Class LinePlotCollection "
    Public Class LinePlotCollection
        Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub
        Public Shadows Function Add(ByVal oPiePiece As LineSlice) As Integer
            Return MyBase.Add(oPiePiece)
        End Function
    End Class
#End Region

End Namespace
