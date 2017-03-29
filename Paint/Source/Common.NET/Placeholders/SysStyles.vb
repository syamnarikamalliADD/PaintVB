' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: SysStyles.vb
'
' Description: Placeholder version
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: RickO
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


Imports System
Imports System.Collections


Friend Class clsSysStyles

    Inherits CollectionBase

#Region " Declares "

#End Region

#Region " Properties "


    Overloads Shared Property Item(ByVal index As Integer) As clsSysStyle
        '********************************************************************************************
        'Description: Get or set a system style by its index
        '
        'Parameters: index
        'Returns:    clsSysStyle
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As New clsSysStyle
            Return o
        End Get
        Set(ByVal Value As clsSysStyle)

        End Set
    End Property
    Overloads Shared Property Item(ByVal DisplayName As String) As clsSysStyle
        '********************************************************************************************
        'Description: Get or set a System Style by its displayname
        '
        'Parameters: index
        'Returns:    clsSysStyle
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As New clsSysStyle
            Return o
        End Get
        Set(ByVal Value As clsSysStyle)
        End Set
    End Property
 #End Region

#Region " Routines"

 
    Friend Function GetStyleInfoArray() As String()
        '********************************************************************************************
        'Description: return an array of style name information for the colors by style hack
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim s(0) As String
        s(0) = String.Empty
        Return s

    End Function
    'Friend Sub LoadStyleBoxFromCollection(ByRef rCombo As ComboBox)
    '    '********************************************************************************************
    '    'Description:  
    '    '
    '    'Parameters:    None
    '    'Returns:       None
    '    '
    '    'Modification history:
    '    '
    '    '    Date       By      Reason
    '    '********************************************************************************************
    'End Sub

    Friend Sub LoadStyleBoxFromCollection(ByRef rCombo As ComboBox, _
                                          Optional ByVal bEnabledOnly As Boolean = False, _
                                          Optional ByVal bFanucColorTag As Boolean = False)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
    End Sub

    Friend Sub LoadStyleBoxFromCollection(ByRef rCcb As CheckedListBox, ByVal AddAll As Boolean, _
                                          Optional ByVal bEnabledOnly As Boolean = False, _
                                          Optional ByVal bFanucColorTag As Boolean = False)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByVal oZone As clsZone)
        '********************************************************************************************
        'Description: Create a collection of styles based on the 'Styles' table  
        '              
        '
        'Parameters:
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

#End Region

End Class 'clsSysStyles

Friend Class clsSysStyle

#Region " Declarations "

#End Region

#Region " Properties "


    Shared ReadOnly Property Description() As clsTextValue
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
        Get
            Dim o As New clsTextValue
            Return o
        End Get
    End Property
    Shared ReadOnly Property DisplayName() As String
        '********************************************************************************************
        'Description:  This property is to generate a name to display in comboboxes etc
        '               and so we can search on what's in combo and not its listindex
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return String.Empty
        End Get
    End Property
    Shared ReadOnly Property PlantString() As String
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
        Get
            Return String.Empty
        End Get
    End Property
#End Region

#Region " Routines "

#End Region

#Region " Events "

#End Region

End Class 'clsSysStyle

Friend Class clsPlantSpecific

#Region " Declarations "

#End Region

#Region " Properties "

#End Region

#Region " Routines "

#End Region

#Region " Events "
#End Region

End Class 'clsPlantSpecific