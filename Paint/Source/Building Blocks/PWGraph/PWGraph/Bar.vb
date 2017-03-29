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

Namespace Bar
    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
#Region " Class Bar Chart "
    Public Class BarChart
        Inherits Base.BaseGraph
        'This will hold the Bar pieces.
        Public BarSliceCollection As New Bar.BarPieceCollection()
        Private _Alignment As Base.eBarTypes = Base.eBarTypes.HorizontalLeft
        Public Property Alignment() As Base.eBarTypes
            Get
                Return _Alignment
            End Get
            Set(ByVal Value As Base.eBarTypes)
                _Alignment = Value
            End Set
        End Property
        'Private _ChartType As Base.eChartType = Base.eChartType.Bar

        Public Shadows ReadOnly Property ChartType() As Base.eChartType
            Get
                Return MyBase.ChartType
            End Get
        End Property

        Sub New()
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Bar
        End Sub
        Sub New(ByVal BarPieceCollection As BarPieceCollection)
            MyBase.new()
            MyBase.ChartType = Base.eChartType.Bar
            BarSliceCollection = BarPieceCollection
        End Sub
    End Class

#End Region
    'A BarSlice can be created alone or by just using the collection 
    'inside Chart
#Region " Class BarSlice "
    Public Class BarSlice
        Inherits Base.BaseChunk
        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()
            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub
        'Private _Rectangle As New System.Drawing.Rectangle(0, 0, 0, 0)
        'Friend Property [Width]() As Integer
        '    Get
        '        Return _Rectangle.Width
        '    End Get
        '    Set(ByVal Value As Integer)
        '        _Rectangle.Width = Value
        '    End Set
        'End Property
        'Friend Property [Height]() As Integer
        '    Get
        '        Return _Rectangle.Height
        '    End Get
        '    Set(ByVal Value As Integer)
        '        _Rectangle.Height = Value
        '    End Set
        'End Property
        'Friend ReadOnly Property [Left]() As Integer
        '    Get
        '        Return _Rectangle.Left
        '    End Get
        'End Property
        'Friend ReadOnly Property [Top]() As Integer
        '    Get
        '        Return _Rectangle.Top
        '    End Get
        'End Property
        'Friend Property [Size]() As System.Drawing.Size
        '    Get
        '        Return _Rectangle.Size
        '    End Get
        '    Set(ByVal Value As System.Drawing.Size)
        '        _Rectangle.Size = Value
        '    End Set
        'End Property
        'Friend Property [Rectangle]() As System.Drawing.Rectangle
        '    Get
        '        Return _Rectangle
        '    End Get
        '    Set(ByVal Value As System.Drawing.Rectangle)
        '        _Rectangle = Value
        '    End Set
        'End Property
    End Class

#End Region
    'A BarPieceCollection can be created all alone but it is a part of LineChart
#Region " Class BarPieceCollection "
    Public Class BarPieceCollection
        Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub
        Public Shadows Function Add(ByVal oBarPiece As BarSlice) As Integer
            Return MyBase.Add(oBarPiece)
        End Function

    End Class
#End Region

End Namespace
