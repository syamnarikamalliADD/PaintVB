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
' Form/Module: ValueClasses.vb
'
' Description: Classes to deal with Strings, integers, singles, and booleans
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


'********************************************************************************************
'Description: Boolean Value Class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsBoolValue

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop
    Private mbValue As Boolean ' = False
    Private mbOldValue As Boolean ' = False
    Private mbNew As Boolean ' = False
    Private mnIndex As Integer ' = 0
    Private msValueName As String = "Value"
    Private msFormatStr As String = String.Empty

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does new = old?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mbValue <> mbOldValue Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Friend ReadOnly Property ForeColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: color for font
        '
        'Parameters:  none
        'Returns:    color
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mbValue = mbOldValue Then
                Return System.Drawing.Color.Black
            Else
                Return System.Drawing.Color.Red
            End If
        End Get
    End Property
    Friend Property FormatString() As String
        '********************************************************************************************
        'Description: doesnt do much for a boolean
        '
        'Parameters:  none
        'Returns:    formatstring
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFormatStr
        End Get
        Set(ByVal Value As String)
            msFormatStr = Value
        End Set
    End Property
    Friend Property Index() As Integer
        '********************************************************************************************
        'Description: to assign to a position in collection etc
        '
        'Parameters:  none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            mnIndex = Value
        End Set
    End Property
    Friend Property Name() As String
        '********************************************************************************************
        'Description: if you want to name it
        '
        'Parameters:  none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msValueName
        End Get
        Set(ByVal Value As String)
            msValueName = Value
        End Set
    End Property
    Friend ReadOnly Property OldValue() As Boolean
        '********************************************************************************************
        'Description: What it used to be when loaded or updated
        '
        'Parameters:  none
        'Returns:    old Value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbOldValue
        End Get
    End Property
    Friend Property Value() As Boolean
        '********************************************************************************************
        'Description: What's it worth to you?
        '
        'Parameters:  none
        'Returns:    value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbValue
        End Get
        Set(ByVal BooleanValue As Boolean)
            If mbNew Then
                mbValue = BooleanValue
                mbOldValue = mbValue
                mbNew = False
                Exit Property
            End If
            mbValue = BooleanValue
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: make old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbOldValue = mbValue
    End Sub

#End Region
#Region " Events"

    Friend Sub New()
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        mbNew = True
    End Sub

#End Region

End Class 'clsBoolValue
'********************************************************************************************
'Description: Integer Value Class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsIntValue

#Region " Declares "

    '9.5.07 remove initializations per fxCop
    Private mnValue As Integer ' = 0
    Private mnOldValue As Integer ' = 0
    Private mbNew As Boolean = False
    Private mnIndex As Integer ' = 0
    Private mnMaxValue As Integer = Integer.MaxValue
    Private mnMinValue As Integer = Integer.MinValue
    Private msValueName As String = "Value"
    Private msFormatStr As String = String.Empty
    Private mbValid As Boolean ' = False

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does new = old?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mnValue <> mnOldValue Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Friend ReadOnly Property ForeColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: color for font
        '
        'Parameters:  none
        'Returns:    color
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mnValue = mnOldValue Then
                Return System.Drawing.Color.Black
            Else
                Return System.Drawing.Color.Red
            End If
        End Get
    End Property
    Friend Property FormatString() As String
        '********************************************************************************************
        'Description: place to hold format string
        '
        'Parameters:  none
        'Returns:    formatstring
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFormatStr
        End Get
        Set(ByVal Value As String)
            msFormatStr = Value
        End Set
    End Property
    Friend Property Index() As Integer
        '********************************************************************************************
        'Description: to assign to a position in collection etc
        '
        'Parameters:  none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            mnIndex = Value
        End Set
    End Property
    Friend Property MaxValue() As Integer
        '********************************************************************************************
        'Description: How big is it?
        '
        'Parameters:  none
        'Returns:    Big Johnson
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxValue
        End Get
        Set(ByVal Value As Integer)
            If Value < mnMinValue Then Exit Property
            mnMaxValue = Value
        End Set
    End Property
    Friend Property MinValue() As Integer
        '********************************************************************************************
        'Description: How small is it?
        '
        'Parameters:  none
        'Returns:    min value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMinValue
        End Get
        Set(ByVal Value As Integer)
            If Value > mnMaxValue Then Exit Property
            mnMinValue = Value
        End Set
    End Property
    Friend Property Name() As String
        '********************************************************************************************
        'Description: if you want to name it
        '
        'Parameters:  none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msValueName
        End Get
        Set(ByVal Value As String)
            msValueName = Value
        End Set
    End Property
    Friend ReadOnly Property OldValue() As Integer
        '********************************************************************************************
        'Description: What it used to be when loaded or updated
        '
        'Parameters:  none
        'Returns:    old Value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnOldValue
        End Get
    End Property
    Friend ReadOnly Property Valid() As Boolean
        '********************************************************************************************
        'Description: is the value OK?
        '
        'Parameters:  none
        'Returns:    true if ok
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbValid
        End Get
    End Property
    Friend Property Value() As Integer
        '********************************************************************************************
        'Description: What's it worth to you?
        '
        'Parameters:  none
        'Returns:    value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnValue
        End Get
        Set(ByVal IntegerValue As Integer)
            mnValue = IntegerValue
            mbValid = True
            If (IntegerValue < mnMinValue) Or (IntegerValue > mnMaxValue) Then
                mbValid = False
            End If
            If mbNew Then
                mnOldValue = mnValue
                mbNew = False
            End If
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: make old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnOldValue = mnValue
    End Sub

#End Region
#Region " Events"

    Friend Sub New()
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
        MyBase.New()
        mbNew = True
        msFormatStr = "#"
    End Sub

#End Region

End Class 'clsIntValue
'********************************************************************************************
'Description: Single Value Class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsSngValue

#Region " Declares "

    '9.5.07 remove initializations per fxCop
    Private mfValue As Single ' = 0
    Private mfOldValue As Single ' = 0
    Private mbNew As Boolean ' = False
    Private mnIndex As Integer ' = 0
    Private mfMaxValue As Single = Single.MaxValue
    Private mfMinValue As Single = Single.MinValue
    Private msValueName As String = "Value"
    Private msFormatStr As String = String.Empty
    Private mbValid As Boolean ' = False

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does new = old?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mfValue <> mfOldValue Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Friend ReadOnly Property ForeColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: color for font
        '
        'Parameters:  none
        'Returns:    color
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mfValue = mfOldValue Then
                Return System.Drawing.Color.Black
            Else
                Return System.Drawing.Color.Red
            End If
        End Get
    End Property
    Friend Property FormatString() As String
        '********************************************************************************************
        'Description: place to hold format string
        '
        'Parameters:  none
        'Returns:    formatstring
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFormatStr
        End Get
        Set(ByVal Value As String)
            msFormatStr = Value
        End Set
    End Property
    Friend Property Index() As Integer
        '********************************************************************************************
        'Description: to assign to a position in collection etc
        '
        'Parameters:  none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            mnIndex = Value
        End Set
    End Property
    Friend Property MaxValue() As Single
        '********************************************************************************************
        'Description: How big is it?
        '
        'Parameters:  none
        'Returns:    max value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfMaxValue
        End Get
        Set(ByVal Value As Single)
            If Value < mfMinValue Then Exit Property
            mfMaxValue = Value
        End Set
    End Property
    Friend Property MinValue() As Single
        '********************************************************************************************
        'Description: How small is it?
        '
        'Parameters:  none
        'Returns:    min value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfMinValue
        End Get
        Set(ByVal Value As Single)
            If Value > mfMaxValue Then Exit Property
            mfMinValue = Value
        End Set
    End Property
    Friend Property Name() As String
        '********************************************************************************************
        'Description: if you want to name it
        '
        'Parameters:  none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msValueName
        End Get
        Set(ByVal Value As String)
            msValueName = Value
        End Set
    End Property
    Friend ReadOnly Property OldValue() As Single
        '********************************************************************************************
        'Description: What it used to be when loaded or updated
        '
        'Parameters:  none
        'Returns:    old Value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfOldValue
        End Get
    End Property
    Friend ReadOnly Property Valid() As Boolean
        '********************************************************************************************
        'Description: is the value OK?
        '
        'Parameters:  none
        'Returns:    true if ok
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbValid
        End Get
    End Property
    Friend Property Value() As Single
        '********************************************************************************************
        'Description: What's it worth to you? 
        '
        'Parameters:  none
        'Returns:    value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfValue
        End Get
        Set(ByVal SingleValue As Single)
            mfValue = SingleValue
            mbValid = True
            If msFormatStr <> String.Empty Then
                Try
                    Dim sTmp As String = Format(mfValue, msFormatStr)
                    mfValue = CSng(sTmp)
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                End Try
            End If
            If (SingleValue < mfMinValue) Or (SingleValue > mfMaxValue) Then
                mbValid = False
            End If
            If mbNew Then
                mfOldValue = mfValue
                mbNew = False
            End If
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: Make old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mfOldValue = mfValue
    End Sub

#End Region
#Region " Events "

    Friend Sub New()
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
        MyBase.New()
        mbNew = True
        msFormatStr = "0.0"
    End Sub
    

#End Region

End Class 'clsSngValue
'********************************************************************************************
'Description: String Value Class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsTextValue

#Region " Declares "

    Private msText As String = String.Empty
    Private msOldText As String = String.Empty
    '9.5.07 remove initialization per fxCop
    Private mnIndex As Integer ' = 0
    Private mnMaxLength As Integer = 50
    Private mbNew As Boolean = False
    Private msDefaultText As String = "-"

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does new = old?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If msText <> msOldText Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Friend Property DefaultText() As String
        '********************************************************************************************
        'Description: give it a default value
        '
        'Parameters:  none
        'Returns:   default value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msDefaultText
        End Get
        Set(ByVal Value As String)
            msDefaultText = Value
        End Set
    End Property
    Friend ReadOnly Property ForeColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: color for font
        '
        'Parameters:  none
        'Returns:    color
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If msText = msOldText Then
                Return System.Drawing.Color.Black
            Else
                Return System.Drawing.Color.Red
            End If
        End Get
    End Property
    Friend Property Index() As Integer
        '********************************************************************************************
        'Description: to assign to a position in collection etc
        '
        'Parameters:  none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            mnIndex = Value
        End Set
    End Property
    Friend Property MaxLength() As Integer
        '********************************************************************************************
        'Description: How long is it?
        '
        'Parameters:  none
        'Returns:    Dont lie.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxLength
        End Get
        Set(ByVal Value As Integer)
            If Value < 1 Then Value = 1
            If Value < 255 Then Value = 255
            mnMaxLength = Value
        End Set
    End Property
    Friend ReadOnly Property OldText() As String
        '********************************************************************************************
        'Description: What it was before you changed it
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msOldText
        End Get
    End Property
    Friend Property Text() As String
        '********************************************************************************************
        'Description: Wacha callit
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msText
        End Get
        Set(ByVal TextString As String)
            If TextString.Length > mnMaxLength Then
                TextString = TextString.Substring(0, (mnMaxLength - 1))
            End If
            msText = TextString
            If mbNew Then
                msOldText = msText
                mbNew = False
            End If
        End Set
    End Property
    Friend Shared ReadOnly Property WhatDoesThisDo() As String
        '********************************************************************************************
        'Description: you asked...
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return "Nothing"
        End Get
    End Property

#End Region
#Region " Routines "
    Friend Sub Update()
        '********************************************************************************************
        'Description: make old = new
        '
        'Parameters:  none
        'Returns:   none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msOldText = msText
    End Sub
#End Region
#Region " Events "

    Friend Sub New()
        MyBase.New()
        mbNew = True
    End Sub

#End Region

End Class 'clsTextValue
