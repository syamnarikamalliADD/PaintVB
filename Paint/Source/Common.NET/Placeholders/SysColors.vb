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
' Form/Module: clsSysColors - Placeholder
'
' Description: Use this if you dont need colors but use the clsrobot
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
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On



Friend Class clsSysColors
    Inherits Collections.CollectionBase

    Friend Sub Load(ByVal void As clsArm)

    End Sub
    Shared ReadOnly Property EffectiveColors() As Integer
        Get
            Return 0
        End Get
    End Property
    Shared ReadOnly Property ColorsByStyle() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Sub New(ByVal void As clsArm)

    End Sub
    Shared ReadOnly Property NumberOfColorsTheirUsing() As Integer
        Get
            Return 0
        End Get
    End Property
End Class

Friend Class clsSysColor
    Shared ReadOnly Property Valve() As Object 'clsValve
        '********************************************************************************************
        'Description: Where does this color come from
        '
        'Parameters:  none
        'Returns:    a valve
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Nothing
        End Get
    End Property
    Shared ReadOnly Property FanucNumber() As Integer
        Get
            Return Nothing
        End Get
    End Property
    Shared Property DisplayName() As String
        Get
            Return "void"
        End Get
        Set(ByVal value As String)

        End Set
    End Property
    Shared ReadOnly Property DisplayName(ByVal bIncludeStyleDesc As Boolean) As String
        Get
            Return "void"
        End Get
    End Property
    Public Sub New()

    End Sub
    Shared Property StyleColorDesc() As String
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)

        End Set
    End Property

End Class
'Friend Class clsValve
'    Friend Property DisplayName() As String
'        Get
'            Return Nothing
'        End Get
'        Set(ByVal value As String)

'        End Set
'    End Property
'End Class
Friend Module mSysColorCommon
    '********************************************************************************************
    'Description: system color module - moved routines from mpwcommon
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '******************************************************************************************** 

    Friend Function LoadColorBoxFromDB(ByRef cboParam As ComboBox, ByRef colZones As clsZones, _
                        ByRef sLookupParams() As String, ByVal bAddAll As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Load color combo with color names
        '             
        '
        'Parameters: sLookupParams is fanuc number
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Return True
    End Function
    Friend Function LoadValveBoxFromDB(ByRef cboParam As ComboBox, ByRef colZones As clsZones, _
                    ByRef sLookupParams() As String, ByVal bAddAll As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Load color combo with color names
        '             
        '
        'Parameters: sLookupParams is fanuc number
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Return True
    End Function
    Friend Function ColorStringForDisplay(ByVal PlantNumber As String, _
                                                            ByVal Description As String) As String
        '********************************************************************************************
        'Description: This Routine builds the string to put in the combobox
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        
        Return String.Empty

    End Function
    Friend Sub LoadColorBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                                ByRef rCbo As ComboBox)

    End Sub
    Friend Sub LoadValveBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                                ByRef rCbo As ComboBox)

    End Sub
    Friend Sub LoadColorBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                            ByRef rClb As CheckedListBox)

    End Sub
    Friend Sub LoadValveBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                                ByRef rClb As CheckedListBox)

    End Sub
End Module 'mSysColorCommon