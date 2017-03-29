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
Imports System.Drawing
Imports System.Drawing.Imaging

Namespace Base

#Region " Structures "

    Public Structure Padding

        Private mnCellPadding As Integer
        Private mnCellSpacing As Integer
        Private mnBorder As Integer

        Sub New(ByVal Padding As Integer, ByVal Spacing As Integer, ByVal CellBorder As Integer)
            mnCellPadding = Padding
            mnCellSpacing = Spacing
            mnBorder = CellBorder
        End Sub
        Public Property CellPadding() As Integer
            Get
                Return mnCellPadding
            End Get
            Set(ByVal Value As Integer)
                mnCellPadding = Value
            End Set
        End Property
        Public Property CellSpacing() As Integer
            Get
                Return mnCellSpacing
            End Get
            Set(ByVal Value As Integer)
                mnCellSpacing = Value
            End Set
        End Property
        Public Property Border() As Integer
            Get
                Return mnBorder
            End Get
            Set(ByVal Value As Integer)
                mnBorder = Value
            End Set
        End Property
    End Structure

    Public Structure MathInfo

        Private mlDivisor As Long
        Private msMultiplier As String
        Private mlMax As Long

        Public Property Divisor() As Long
            Get
                Return mlDivisor
            End Get
            Set(ByVal value As Long)
                mlDivisor = value
            End Set
        End Property

        Public Property Multiplier() As String
            Get
                Return msMultiplier
            End Get
            Set(ByVal value As String)
                msMultiplier = value
            End Set
        End Property

        Public Property Max() As Long
            Get
                Return mlMax
            End Get
            Set(ByVal value As Long)
                mlMax = value
            End Set
        End Property
    End Structure

#End Region
#Region " Enums"
    Public Enum eBarTypes
        HorizontalLeft = 0
        HorizontalRight = 1
        VerticalTop = 2
        VerticalBottom = 3
    End Enum
    Public Enum eLineTypes
        Horizontal = 1
        Vertical = 2
    End Enum
    Public Enum eChartValueType
        ValuePercent = 1
        ValueTotal = 2
    End Enum
    Public Enum eChartType
        Pie = 0
        Bar = 1
        Line = 2
    End Enum
#End Region

#Region " Class BaseChunk "
    Public MustInherit Class BaseChunk
        Inherits TextStruct

        Private dValue As Decimal
        Private sKeyName As String

        Public Property KeyName() As String
            Get
                Return sKeyName
            End Get
            Set(ByVal value As String)
                sKeyName = value
            End Set
        End Property
        Public Property [Value]() As Decimal
            Get
                Return dValue
            End Get
            Set(ByVal Value1 As Decimal)
                dValue = Value1
            End Set
        End Property
    End Class

#End Region
#Region " Class BaseGraph "

    Public MustInherit Class BaseGraph
        Inherits TextStruct
        Public ReadOnly Property Version() As String
            Get
                Return "3.0"
            End Get
        End Property
        Private msKeyFontName As String = "Ariel"
        Private mnKeyFontSize As Integer = 10
        Private mKeyFontStyle As System.Drawing.FontStyle = FontStyle.Regular

        Private mKeyBackColor As Color = Drawing.Color.White

        Private msKeyTitle As String = ""
        Private mKeyTitleColor As Color = Color.Black
        Private msKeyTitleFontName As String = "Ariel"
        Private mnKeyTitleFontSize As Integer = 10
        Private mKeyTitleFontStyle As System.Drawing.FontStyle = FontStyle.Regular

        Private mImageSize As Size = New Size(100, 100)
        Private mValueType As eChartValueType = eChartValueType.ValueTotal
        Private mChartType As eChartType = eChartType.Pie
        Private mbGraphBorder As Boolean = True
        Private mGraphRect As Rectangle = New Rectangle(0, 0, 0, 0)
        Private mbAutoSize As Boolean = False
        Private mGraphAlign As eBarTypes = eBarTypes.HorizontalLeft

        Public Property KeyFontName() As String
            Get
                Return msKeyFontName
            End Get
            Set(ByVal value As String)
                msKeyFontName = value
            End Set
        End Property
        Public Property KeyFontSize() As Integer
            Get
                Return mnKeyFontSize
            End Get
            Set(ByVal value As Integer)
                mnKeyFontSize = value
            End Set
        End Property
        Public Property KeyFontStyle() As System.Drawing.FontStyle
            Get
                Return mKeyFontStyle
            End Get
            Set(ByVal value As System.Drawing.FontStyle)
                mKeyFontStyle = value
            End Set
        End Property

        Public Property KeyBackColor() As Color
            Get
                Return mKeyBackColor
            End Get
            Set(ByVal value As Color)
                mKeyBackColor = value
            End Set
        End Property

        Public Property KeyTitle() As String
            Get
                Return msKeyTitle
            End Get
            Set(ByVal value As String)
                msKeyTitle = value
            End Set
        End Property
        Public Property KeyTitleColor() As Color
            Get
                Return mKeyTitleColor
            End Get
            Set(ByVal value As Color)
                mKeyTitleColor = value
            End Set
        End Property
        Public Property KeyTitleFontName() As String
            Get
                Return msKeyTitleFontName
            End Get
            Set(ByVal value As String)
                msKeyTitleFontName = value
            End Set
        End Property
        Public Property KeyTitleFontSize() As Integer
            Get
                Return mnKeyTitleFontSize
            End Get
            Set(ByVal value As Integer)
                mnKeyTitleFontSize = value
            End Set
        End Property
        Public Property KeyTitleFontStyle() As System.Drawing.FontStyle
            Get
                Return mKeyTitleFontStyle
            End Get
            Set(ByVal value As System.Drawing.FontStyle)
                mKeyTitleFontStyle = value
            End Set
        End Property

        Public Property AutoSize() As Boolean
            Get
                Return mbAutoSize
            End Get
            Set(ByVal Value As Boolean)
                mbAutoSize = Value
            End Set
        End Property
        Public Property ImageSize() As Size
            Get
                Return mImageSize
            End Get
            Set(ByVal Value As Size)
                mImageSize = Value
            End Set
        End Property
        Public Property ValueType() As eChartValueType
            Get
                Return mValueType
            End Get
            Set(ByVal Value As eChartValueType)
                mValueType = Value
            End Set
        End Property
        Public Property ChartType() As eChartType
            Get
                Return mChartType
            End Get
            Set(ByVal Value As eChartType)
                mChartType = Value
            End Set
        End Property
        Public Property GraphBorder() As Boolean
            Get
                Return mbGraphBorder
            End Get
            Set(ByVal Value As Boolean)
                mbGraphBorder = Value
            End Set
        End Property
        Protected Friend Property GraphRect() As Rectangle
            Get
                Return mGraphRect
            End Get
            Set(ByVal Value As Rectangle)
                mGraphRect = Value
            End Set
        End Property
        Public Property GraphAlign() As eBarTypes
            Get
                Return mGraphAlign
            End Get
            Set(ByVal Value As eBarTypes)
                mGraphAlign = Value
            End Set
        End Property
    End Class

#End Region
#Region " Class TextStruct "

    Public MustInherit Class TextStruct
        Private mColor As Color = Color.Black
        Public Property [Color]() As Color
            Get
                Return mColor
            End Get
            Set(ByVal Value As Color)
                mColor = Value
            End Set
        End Property
    End Class

#End Region
#Region " Class BaseChunkCollection"

    Public Class BaseChunkCollection
        Inherits CollectionBase
        Private mdtotalValue As Decimal = 0D

        Public Shadows Function Add(ByVal Chunk As BaseChunk) As Integer
            mdtotalValue += Chunk.Value
            Return List.Add(Chunk)
        End Function
        Public Shadows Function Remove(ByVal Index As Integer) As Boolean
            Try
                mdtotalValue -= CType(List.Item(Index), BaseChunk).Value
                List.RemoveAt(Index)
            Catch ex As Exception
                'Log Error
            End Try
            Return True
        End Function
        Public ReadOnly Property MaxKeyNameLength() As Integer
            Get
                Dim nMaxValue As Integer

                For Each oBaseChunk As Base.BaseChunk In List

                    If nMaxValue < oBaseChunk.KeyName.Length Then
                        nMaxValue = oBaseChunk.KeyName.Length
                    End If
                Next

                Return nMaxValue

            End Get
        End Property
        Public ReadOnly Property MaxValue() As Decimal
            Get
                Dim oBaseChunk As BaseChunk
                Dim nMaxValue As Decimal = Decimal.MinValue
                For Each oBaseChunk In List
                    If nMaxValue < oBaseChunk.Value Then
                        nMaxValue = oBaseChunk.Value
                    End If
                Next
                Return nMaxValue
            End Get
        End Property
        Public ReadOnly Property MinValue() As Decimal
            Get
                Dim oBaseChunk As BaseChunk
                Dim nMinValue As Decimal = Decimal.MaxValue
                For Each oBaseChunk In List
                    If nMinValue > oBaseChunk.Value Then
                        nMinValue = oBaseChunk.Value
                    End If
                Next
                Return nMinValue
            End Get
        End Property
        Public ReadOnly Property TotalValue() As Decimal
            Get
                Return mdtotalValue
            End Get
        End Property

    End Class

#End Region

End Namespace



