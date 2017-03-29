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
' Form/Module: Presets.vb
'
' Description: Placeholder version
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
'    07/10/08   gks     Change Friend to Shared per fxCop
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

#Region " Namespace "
Friend Enum ePresetParam
    Fluid = 0
    Atom_Speed = 1
    Fan_Shap = 2
    Estat = 3
    LastParamNum = 3
    OverRideEnable = 13
    Description = 7
    NumberOfEstatSteps = 7
End Enum 'collection of preset classes

#End Region
'********************************************************************************************
'Description: Preset Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsPresets

    Inherits CollectionBase

#Region " Declares "


#End Region
#Region " Properties "

    Shared ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does Old = New?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return False

        End Get
    End Property
    Property ColorChangePresets() As Boolean
        '********************************************************************************************
        'Description: Are these colorchange presets?
        '
        'Parameters:  none
        'Returns:   true if colorchange, false if fluid - you gotta set
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property
    Property DisplayName() As String
        '********************************************************************************************
        'Description: Display name for color parameter e.g. "17 - PaintShop Purple"
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return "Placeholder Class"
        End Get
        Set(ByVal value As String)
        End Set
    End Property

    Shared ReadOnly Property EstatStepValue(ByVal StepNumber As Integer) As clsIntValue
        '********************************************************************************************
        'Description: The KV value for an estat step
        '
        'Parameters:  step number
        'Returns:    integer
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim fred As New clsIntValue
            Return fred
        End Get
    End Property
    Shared ReadOnly Property EstatStepChanged() As Boolean
        '********************************************************************************************
        'Description: The KV value for an estat step
        '
        'Parameters:  
        'Returns:    true if changes
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return False
        End Get
    End Property
    Shared Property EstatStepValuesLoaded() As Boolean
        '********************************************************************************************
        'Description: did we loadem?
        '
        'Parameters:  
        'Returns:    true if loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Boolean)

        End Set
        Get
            Return False
        End Get
    End Property
    Shared Property EstatType() As eEstatType
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
            Return eEstatType.None
        End Get
        Set(ByVal value As eEstatType)
        End Set
    End Property
    Property FanucColor() As Integer
        '********************************************************************************************
        'Description: Pointer to fanuc color table
        '
        'Parameters:  none
        'Returns:    integer
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return 0
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property
    Overloads Shared ReadOnly Property Item(ByVal PresetNumber As Integer) As clsPreset
        '********************************************************************************************
        'Description: Get you presets here!
        '
        'Parameters:  by preset number 1 - n
        'Returns:    a preset
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Nothing
        End Get
    End Property
    Overloads Shared ReadOnly Property Item(ByVal DisplayName As String) As clsPreset
        '********************************************************************************************
        'Description: Get you presets here!
        '
        'Parameters:  by preset description e.g. "3-Door Hinge"
        'Returns:    a preset
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Nothing
        End Get
    End Property
    Overloads Shared ReadOnly Property OverRideChanged() As Boolean
        '********************************************************************************************
        'Description: Sombody changed an override value
        '
        'Parameters:  none
        'Returns:   True if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return False

        End Get
    End Property
    Overloads Shared ReadOnly Property OverRideChanged(ByVal ParamNum As ePresetParam) As Boolean
        '********************************************************************************************
        'Description: Sombody changed a particular override value
        '
        'Parameters:  none
        'Returns:   True if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return False
        End Get
    End Property
    Shared ReadOnly Property OverRideEnabled() As clsBoolValue
        '********************************************************************************************
        'Description: Are Overrides enabled?
        '
        'Parameters:  none
        'Returns:   value from first preset (should all be the same)
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get


            Return Nothing

        End Get
    End Property
    Shared ReadOnly Property OverRidePercent(ByVal ParamNum As ePresetParam) As clsIntValue
        '********************************************************************************************
        'Description: Values of the overrides
        '
        'Parameters:  whaich param
        'Returns:   value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Nothing
        End Get
    End Property
    Shared Property ParameterMaxValue(ByVal ParamNum As ePresetParam) As Integer
        '********************************************************************************************
        'Description: Max value allowed by robot
        '
        'Parameters:  whaich param
        'Returns:   value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return 0
        End Get
        Set(ByVal value As Integer)
        End Set

    End Property
    Shared Property ParameterMinValue(ByVal ParamNum As ePresetParam) As Integer
        '********************************************************************************************
        'Description: Min value allowed by robot
        '
        'Parameters:  whaich param
        'Returns:   value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return 0
        End Get
        Set(ByVal value As Integer)
        End Set

    End Property
    Shared Property ParameterName(ByVal Param As ePresetParam) As String
        '********************************************************************************************
        'Description: Display name for parameter e.g "Fluid Flow"
        '
        'Parameters:  Which parameter
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Nothing
        End Get
        Set(ByVal value As String)

        End Set
    End Property

#End Region
#Region " Routines "

    Shared Sub Add(ByVal Preset As clsPreset)
        '********************************************************************************************
        'Description: Add to collection
        '
        'Parameters:  a preset
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


    End Sub
    Shared Function SetOverRidePercent(ByVal ParamNum As ePresetParam, ByVal Value As Integer) _
                                                                                        As Boolean
        '********************************************************************************************
        'Description: set the value
        '
        'Parameters:   which param , value to set
        'Returns:    true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Return True

    End Function
    Shared Sub SetEnableOverRide(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Turn on / off the override enable for presets in collection
        '
        'Parameters:   
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


    End Sub
    Shared Sub Update()
        '********************************************************************************************
        'Description: make the old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
    End Sub
    Shared Sub UpdateOverride()
        '********************************************************************************************
        'Description: make the old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
    End Sub

#End Region
#Region " Events "

    Friend Sub New()
        '********************************************************************************************
        'Description: Class constructor
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

End Class 'clsPresets
'********************************************************************************************
'Description: A single preset class
'
'
'Modification history:
'
' Date      By      Reason
'7/10/08    gks     Changed Friend to Shared per fxCop
'******************************************************************************************** 
Friend Class clsPreset

#Region " Declares "


#End Region
#Region " Properties "

    Shared ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Does New match old
        '
        'Parameters:  none
        'Returns:    True if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return False

        End Get
    End Property
    Shared ReadOnly Property Description() As clsTextValue
        '********************************************************************************************
        'Description: What you call it (to its face)
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim fred As New clsTextValue
            Return fred
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
            Return "Placeholder"
        End Get
    End Property
    Shared ReadOnly Property EffectiveValue(ByVal ParamNum As ePresetParam) As Single
        '********************************************************************************************
        'Description: what it is after applying override
        '
        'Parameters:  which one
        'Returns:    modified value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return 0

        End Get
    End Property
    Shared Property Index() As Integer
        '********************************************************************************************
        'Description: Preset Number
        '
        'Parameters:  none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return 0
        End Get
        Set(ByVal Value As Integer)
        End Set
    End Property
    Shared ReadOnly Property OverRidePercent(ByVal ParamNum As ePresetParam) As clsIntValue
        '********************************************************************************************
        'Description: Override value
        '
        'Parameters:  which parameter
        'Returns:    value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Dim fred As New clsIntValue
            Return fred

        End Get
    End Property
    Shared ReadOnly Property OverRideEnabled() As clsBoolValue
        '********************************************************************************************
        'Description: Using Override value?
        '
        'Parameters:  None
        'Returns:    true if enabled
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim fred As New clsBoolValue
            Return fred
        End Get
    End Property
    Shared ReadOnly Property OverrideChanged() As Boolean
        '********************************************************************************************
        'Description: Does New match old
        '
        'Parameters:  none
        'Returns:    True if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return False

        End Get
    End Property
    Shared ReadOnly Property Param(ByVal ParamNum As ePresetParam) As clsSngValue
        '********************************************************************************************
        'Description: this is the value of the preset
        '
        'Parameters:  which parameter
        'Returns:    value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Dim fred As New clsSngValue
            Return fred
        End Get
    End Property
    Shared WriteOnly Property Parent() As clsPresets
        '********************************************************************************************
        'Description: reference back to collection
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As clsPresets)
        End Set
    End Property

#End Region
#Region " Routines "

 
    Shared Sub Update()
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

    End Sub
    Shared Sub UpdateOverRide()
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


    End Sub

#End Region
#Region " Events "

    Friend Sub New()
        '********************************************************************************************
        'Description: constructor
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

    End Sub

#End Region

End Class 'clsPreset
