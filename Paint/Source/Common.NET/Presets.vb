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
' Description: collection to hold presets, preset class
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
'    11/10/09   MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'                       parm name on the robot is used for resource lookup:
'                       for APP_CON_NAM= "FF", rsFF = "Paint", rsFF_CAP = "Paint cc/min", rsFF_UNITS ="
'                       supports 2nd shaping air, volume on CC screen
'    11/16/09   MSW     support flexible screen config - parm order changes for estat steps
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
    LastParamNum = 4
    OverRideEnable = 13
    Description = 7
    EstatDescription = 8  '6/20/08 - 8 is guess
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
' 12/17/09  MSW     Add style number property
'******************************************************************************************** 
Friend Class clsPresets

    Inherits CollectionBase

#Region " Declares "
    '9.5.07 remove initializations per fxCop

    Private msColorName As String = String.Empty
    Private mnFanucColor As Integer ' = 0
    Private msParamNames(ePresetParam.EstatDescription) As String
    Private mbColorChange As Boolean ' = False
    Private mnMaxVal(ePresetParam.LastParamNum) As Integer
    Private mnMinVal(ePresetParam.LastParamNum) As Integer
    Private mnEstatStepVal(ePresetParam.NumberOfEstatSteps) As clsIntValue
    Private mbEstatStepLoaded As Boolean ' = False
    Private mEstatType As eEstatType = eEstatType.None
    Private mnNumParms As Integer 'Number of parameters
    Private mnParmToScrn() As Integer = Nothing
    Private mnStyle As Integer = 1
#End Region
#Region " Properties "
    Friend Property StyleNumber() As Integer
        '********************************************************************************************
        'Description: Style number for presets by style.
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/17/09  MSW     Add style number property
        '********************************************************************************************
        Get
            Return mnStyle
        End Get
        Set(ByVal value As Integer)
            mnStyle = value
        End Set
    End Property
    Friend Property NumParms() As Integer
        '********************************************************************************************
        'Description: Number of parameters
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumParms
        End Get
        Set(ByVal value As Integer)
            ReDim Preserve mnParmToScrn(value)
            If value > mnNumParms Then
                For nItem As Integer = mnNumParms + 1 To value
                    mnParmToScrn(nItem) = 0
                Next
            End If
            mnNumParms = value
        End Set
    End Property
    Friend Property ScreenParm(ByVal nIndex As Integer) As Integer
        '********************************************************************************************
        'Description: Parameter number for a column on the screen
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mnParmToScrn IsNot Nothing Then
                If (nIndex <= mnParmToScrn.GetUpperBound(0)) And (nIndex >= 0) Then
                    Return mnParmToScrn(nIndex)
                Else
                    Return -1
                End If
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Integer)
            If nIndex > mnNumParms Then
                ReDim Preserve mnParmToScrn(nIndex)
                For nItem As Integer = mnNumParms + 1 To nIndex
                    mnParmToScrn(nItem) = 0
                Next
            End If
            mnParmToScrn(nIndex) = value
        End Set
    End Property
    Friend ReadOnly Property Changed() As Boolean
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

            For Each o As clsPreset In List
                If o.Changed Then
                    Return True
                End If
            Next

            Return False

        End Get
    End Property
    Friend Property ColorChangePresets() As Boolean
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
            Return mbColorChange
        End Get
        Set(ByVal value As Boolean)
            mbColorChange = value
        End Set
    End Property
    Friend Property DisplayName() As String
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
            Return msColorName
        End Get
        Set(ByVal value As String)
            msColorName = value
        End Set
    End Property
    Friend ReadOnly Property EstatStepValue(ByVal StepNumber As Integer) As clsIntValue
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
            If StepNumber < 0 Then Return Nothing
            If StepNumber > ePresetParam.NumberOfEstatSteps Then Return Nothing
            Return mnEstatStepVal(StepNumber)
        End Get
    End Property
    Friend ReadOnly Property EstatStepChanged() As Boolean
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
            Dim i%
            For i% = 0 To ePresetParam.NumberOfEstatSteps
                If mnEstatStepVal(i%).Changed Then Return True
            Next
            Return False
        End Get
    End Property
    Friend Property EstatStepValuesLoaded() As Boolean
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
            mbEstatStepLoaded = value
        End Set
        Get
            Return mbEstatStepLoaded
        End Get
    End Property
    Friend Property EstatType() As eEstatType
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
            Return mEstatType
        End Get
        Set(ByVal value As eEstatType)
            mEstatType = value
        End Set
    End Property
    Friend Property FanucColor() As Integer
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
            Return mnFanucColor
        End Get
        Set(ByVal value As Integer)
            mnFanucColor = value
        End Set
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal PresetNumber As Integer) As clsPreset
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

            Try
                'list is zero based presets start at 1
                Return DirectCast(List(PresetNumber - 1), clsPreset)

            Catch ex As System.ArgumentException
                ' the asked for key is not found
                Trace.WriteLine("Module: Presets, Routine: Item(Integer), Error: " & ex.Message)
                Trace.WriteLine("Module: Presets, Routine: Item(Integer), StackTrace: " & ex.StackTrace)
                Return Nothing
            Catch ex As System.NullReferenceException
                'this is for a null pointer
                Trace.WriteLine("Module: Presets, Routine: Item(Integer), Error: " & ex.Message)
                Trace.WriteLine("Module: Presets, Routine: Item(Integer), StackTrace: " & ex.StackTrace)
                Return Nothing
            End Try
        End Get
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal DisplayName As String) As clsPreset
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
            Dim o As clsPreset
            For Each o In List
                If o.DisplayName = DisplayName Then
                    Return o
                End If
            Next
            ' failed
            'sometimes during copies and such, the display name may be from another robot
            ' and not be in the collection - so we will try to infer the preset number
            'from the string passed in
            Dim nPos As Integer = Strings.InStr(DisplayName, "-")
            If nPos > 0 Then
                Dim sTmp As String = Strings.Left(DisplayName, (nPos - 1))
                Dim nTmp As Integer = CType(sTmp, Integer)
                If nTmp > 0 And nTmp <= List.Count Then
                    Return DirectCast(List(nTmp - 1), clsPreset)
                End If

            End If
            ' really failed
            Return Nothing
        End Get
    End Property
    Friend Overloads ReadOnly Property OverRideChanged() As Boolean
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
            Dim i%

            For i% = ePresetParam.Fluid To ePresetParam.LastParamNum
                If OverRideChanged(CType(i%, ePresetParam)) Then Return True
            Next

            If OverRideEnabled.Changed Then Return True

            Return False

        End Get
    End Property
    Friend Overloads ReadOnly Property OverRideChanged(ByVal ParamNum As Integer) As Boolean
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
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return False

            ' should all be the same
            For Each o As clsPreset In List
                Return o.OverRidePercent(ParamNum).Changed
            Next

            'opps
            Return False
        End Get
    End Property
    Friend ReadOnly Property OverRideEnabled() As clsBoolValue
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

            For Each o As clsPreset In List
                Return o.OverRideEnabled
            Next

            Return Nothing

        End Get
    End Property
    Friend ReadOnly Property OverRidePercent(ByVal ParamNum As Integer) As clsIntValue
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
            'all should be the same return 1st one
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return Nothing
            For Each o As clsPreset In List
                Return o.OverRidePercent(ParamNum)
            Next
            'opps
            Return Nothing
        End Get
    End Property
    Friend Property ParameterMaxValue(ByVal ParamNum As ePresetParam) As Integer
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
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return Nothing

            Return mnMaxVal(ParamNum)
        End Get
        Set(ByVal value As Integer)
            mnMaxVal(ParamNum) = value
        End Set

    End Property
    Friend Property ParameterMaxValue(ByVal ParamNum As Integer) As Integer
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
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return Nothing

            Return mnMaxVal(ParamNum)
        End Get
        Set(ByVal value As Integer)
            mnMaxVal(ParamNum) = value
        End Set

    End Property
    Friend Property ParameterMinValue(ByVal ParamNum As ePresetParam) As Integer
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
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return Nothing
            Return mnMinVal(ParamNum)
        End Get
        Set(ByVal value As Integer)
            mnMinVal(ParamNum) = value
        End Set

    End Property
    Friend Property ParameterMinValue(ByVal ParamNum As Integer) As Integer
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
            If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return Nothing
            Return mnMinVal(ParamNum)
        End Get
        Set(ByVal value As Integer)
            mnMinVal(ParamNum) = value
        End Set

    End Property
    Friend Property ParameterName(ByVal Param As Integer) As String
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
            Return msParamNames(Param)
        End Get
        Set(ByVal value As String)
            msParamNames(Param) = value
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Add(ByVal Preset As clsPreset)
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

        Try
            If Preset.Index <= 0 Then
                Preset.Index = (List.Count + 1)
            End If

            Preset.Parent = Me

            List.Add(Preset)

        Catch ex As System.ArgumentException
            'this is thrown for duplicate key
            Trace.WriteLine("Module: clsPresets, Routine: Add, Error: " & ex.Message)
            Trace.WriteLine("Module: clsPresets, Routine: Add, StackTrace: " & ex.StackTrace)
        Catch ex As System.NullReferenceException
            'this is for a null pointer
            Trace.WriteLine("Module: clsPresets, Routine: Add, Error: " & ex.Message)
            Trace.WriteLine("Module: clsPresets, Routine: Add, StackTrace: " & ex.StackTrace)
        End Try


    End Sub
    Friend Function SetOverRidePercent(ByVal ParamNum As Integer, ByVal Value As Integer) _
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

        If ParamNum < 0 Or ParamNum > ePresetParam.LastParamNum Then Return False
        If (Value < 50) Or (Value > 150) Then Return False

        For Each o As clsPreset In List
            o.OverRidePercent(ParamNum).Value = Value
        Next

        Return True

    End Function
    Friend Sub SetEnableOverRide(ByVal bEnable As Boolean)
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

        For Each o As clsPreset In List
            o.OverRideEnabled.Value = bEnable
        Next

    End Sub
    Friend Sub Update()
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
        For Each o As clsPreset In List
            o.Update()
        Next
    End Sub
    Friend Sub UpdateOverride()
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
        For Each o As clsPreset In List
            o.UpdateOverRide()
        Next
        Me.OverRideEnabled.Update()
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
        MyBase.New()
        Dim i%
        For i% = 0 To mnEstatStepVal.GetUpperBound(0)
            mnEstatStepVal(i%) = New clsIntValue
            mnEstatStepVal(i%).Value = 0
            mnEstatStepVal(i%).MinValue = 0
            mnEstatStepVal(i%).MaxValue = ePresetParam.NumberOfEstatSteps
        Next
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
' 06/20/08  gks     Add estat desc
'******************************************************************************************** 
Friend Class clsPreset

#Region " Declares "

    Private Const mnLastParam As Integer = ePresetParam.LastParamNum

    '9.5.07 remove initializations per fxCop
    Private mnIndex As Integer ' = 0
    Private moDescription As clsTextValue ' = Nothing
    Private moESDescription As clsTextValue ' = Nothing
    Private moOverRide As clsBoolValue ' = Nothing
    Private moOver() As clsIntValue ' = Nothing
    Private moParam() As clsSngValue ' = Nothing
    Private mParent As clsPresets ' = Nothing

#End Region
#Region " Properties "
    Friend ReadOnly Property Changed() As Boolean
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
            Dim i%

            For i% = 0 To mnLastParam
                If moParam(i%).Changed Then Return True
            Next

            If moDescription.Changed Then Return True

            ' no changes
            Return False

        End Get
    End Property
    Friend ReadOnly Property Description() As clsTextValue
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
            Return moDescription
        End Get
    End Property
    Friend ReadOnly Property EstatDescription() As clsTextValue
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
            Return moESDescription
        End Get
    End Property

    Friend ReadOnly Property DisplayName() As String
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
            Return mnIndex.ToString(mLanguage.CurrentCulture) & " - " & moDescription.Text
        End Get
    End Property
    Friend ReadOnly Property EstatDisplayName() As String
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
            Return mnIndex.ToString(mLanguage.CurrentCulture) & " - " & moESDescription.Text
        End Get
    End Property
    Friend ReadOnly Property EffectiveValue(ByVal ParamNum As Integer) As Single
        '********************************************************************************************
        'Description: what it is after applying override
        '
        'Parameters:  which one
        'Returns:    modified value
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Support flexible screen config - parm order changes for estat steps
        '********************************************************************************************
        Get
            Dim fMul As Single = 1
            Dim bStep As Boolean = False
            If (ParamNum < 0) Or (ParamNum > mnLastParam) Then Return Nothing
            Dim nParm As Integer = mParent.ScreenParm(ParamNum)
            If (nParm < 0) Or (nParm > nParm) Then Return Nothing

            Select Case (nParm - 1)
                Case ePresetParam.Estat
                    If mParent.EstatType = eEstatType.FB200 Then
                        bStep = True
                    End If
                Case Else

            End Select

            If bStep Then
                If mParent.EstatStepValuesLoaded Then
                    If ((Param(ParamNum).Value) >= 0) And _
                                ((Param(ParamNum).Value) <= ePresetParam.NumberOfEstatSteps) Then

                        Return CSng(mParent.EstatStepValue(CInt(Param(ParamNum).Value)).Value)
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If ' mParent.EstatStepValuesLoaded

            Else

                If (moOverRide Is Nothing) = False Then
                    If moOverRide.Value = True Then
                        fMul = CType(moOver(ParamNum).Value, Single) / 100
                    End If
                End If

                Dim GottaUseADouble As Double = CDbl(moParam(ParamNum).Value * fMul)
                Return CSng(Math.Round(GottaUseADouble))

            End If


        End Get
    End Property
    Friend Property Index() As Integer
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
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value > 0 Then mnIndex = Value
        End Set
    End Property
    Friend ReadOnly Property OverRidePercent(ByVal ParamNum As Integer) As clsIntValue
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

            If (ParamNum < 0) Or (ParamNum > mnLastParam) Then Return Nothing
            If moOver Is Nothing Then
                subNewOverRide()
            End If
            Return moOver(ParamNum)

        End Get
    End Property
    Friend ReadOnly Property OverRideEnabled() As clsBoolValue
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
            If moOverRide Is Nothing Then
                subNewOverRide()
            End If
            Return moOverRide
        End Get
    End Property
    Friend ReadOnly Property OverrideChanged() As Boolean
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
            Dim i%

            If moOver Is Nothing Then
                subNewOverRide()
                Return False
            End If

            For i% = 0 To mnLastParam
                If moOver(i%).Changed Then Return True
            Next
            ' no changes
            Return False

        End Get
    End Property
    Default Friend ReadOnly Property Param(ByVal ParamNum As ePresetParam) As clsSngValue
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

            If (ParamNum < 0) Or (ParamNum > mnLastParam) Then Return Nothing
            Return moParam(ParamNum)

        End Get
    End Property
    Default Friend ReadOnly Property Param(ByVal ParamNum As Integer) As clsSngValue
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

            If (ParamNum < 0) Or (ParamNum > mnLastParam) Then Return Nothing
            Return moParam(ParamNum)

        End Get
    End Property
    Protected Friend WriteOnly Property Parent() As clsPresets
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
            mParent = value
        End Set
    End Property

#End Region
#Region " Routines "

    Private Sub subNewOverRide()
        '********************************************************************************************
        'Description: set up the override structure
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Dim i%

        ReDim moOver(mnLastParam)

        For i% = 0 To mnLastParam
            moOver(i%) = New clsIntValue
            moOver(i%).MaxValue = 150
            moOver(i%).MinValue = 50
            moOver(i%).Value = 100
        Next
        moOverRide = New clsBoolValue
    End Sub
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
        Dim i%

        For i% = 0 To mnLastParam
            moParam(i%).Update()
        Next
        moDescription.Update()

    End Sub
    Friend Sub UpdateOverRide()
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
        Dim i%

        If moOver Is Nothing Then
            subNewOverRide()
        End If
        For i% = 0 To mnLastParam
            moOver(i%).Update()
        Next
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
        MyBase.New()

        Dim i%
        'overrides set up on first use
        ReDim moParam(mnLastParam)

        'assume default is 4 params - flow,atom,fan,estat
        For i% = 0 To mnLastParam
            moParam(i%) = New clsSngValue
        Next

        moDescription = New clsTextValue
        moESDescription = New clsTextValue '6/20/08

    End Sub

#End Region

End Class 'clsPreset
