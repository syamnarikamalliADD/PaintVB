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
' Form/Module: SysColors.vb
'
' Description: Collection of System Colors, system colors classes
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
'    07/26/07   gks     Changed clsvalve.number to clsIntValue
'    11/16/09   MSW     Add bEnabledOnly to optionally load only enabled items all load ...box routines
'    04/01/10   MSW     add support for mnPlantAsciiMaxLength
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    01/06/12   MSW     Add IgnorePresets() property to speed up screens that don't need preset data 4.01.01.01
'    03/28/12   MSW     Move system color setup table to XML                    4.01.03.00
'    04/27/12   MSW     fix some indexing, Some XML reads needed .InnerText instead of .ToString 4.01.03.01
'    08/30/12   RJO     Bug fix to mSysColorCommon Function bSaveColorsToXML. The 
'                       RobotsRequired value was being saved to the ARGB field.       4.01.03.02
'    11/06/12   RJO     Removed "NumberOfColorsTheirUsing" hack from mSysColorCommon. 4.01.03.03
'    12/13/12   MSW     clsSysColors.Load check list length against zone setting      4.01.03.04
'    08/21/13   AGP     GetColorTableDataset - Change from ADD                        4.01.05.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath


'********************************************************************************************
'Description: System Color Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsSysColors

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsSysColors"
    'Friend Const gsSYS_COL_XMLFILE As String = "System Colors.xml"
    'Friend Const gsSYS_COL_XMLSCHEMA As String = "System Colors.xsd"
    '***** End Module Constants   **************************************************************
    '9.5.07 remove unnecessary initializations per fxCop
    Private msLastColSelected As String = String.Empty
    Private mbColorsByStyle As Boolean ' = False
    Private mbColorIsAscii As Boolean ' = False
    Private mnPlantAsciiMaxLength As Integer = 0
    Private mbUse2K As Boolean

    'BTK 02/23/10 Added Tricoat Tab
    Private mbUseTricoat As Boolean
    Private mbTwoCoats As Boolean
    Protected Friend msColorValveName() As String
    Private mRobot As clsArm ' = Nothing
    Private mnEffectiveColors As Integer ' = 0
    Private mnMinColNumber As Integer = 1
    Private mnMaxColNumber As Integer = 999
    Private mnMinValveNumber As Integer = 1
    Private mnMaxValveNumber As Integer = 31
    Private mbIgnorePresets As Boolean = False
#End Region
#Region " Properties "
    Friend Property IgnorePresets() As Boolean
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
            Return mbIgnorePresets
        End Get
        Set(ByVal value As Boolean)
            mbIgnorePresets = value
        End Set
    End Property
    Friend ReadOnly Property IsAscii() As Boolean
        Get
            '********************************************************************************************
            'Description: Is the Plant Number ASCII?
            '
            'Parameters: none
            'Returns:    t or f
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Return mbColorIsAscii
        End Get
    End Property
    Friend Property PlantAsciiMaxLength() As Integer
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
            Return mnPlantAsciiMaxLength
        End Get
        Set(ByVal value As Integer)
            mnPlantAsciiMaxLength = value
        End Set
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal index As Integer) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a system color by its index
        '
        'Parameters: index
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As clsSysColor = DirectCast(List(index), clsSysColor)
            ' if we are switching colors clear out the data
            If o.DisplayName <> msLastColSelected Then
                If mbIgnorePresets = False Then
                    o.Presets.Clear()
                End If
                msLastColSelected = o.DisplayName
            End If
            Return o
        End Get
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal FanucNumber As Integer, _
                                                            ByVal void As Boolean) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a system color by its index
        '
        'Parameters: Void is just here so we can overload
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As clsSysColor

            For Each o In List
                If FanucNumber = o.FanucNumber Then
                    ' if we are switching colors clear out the data
                    If o.DisplayName <> msLastColSelected Then
                        If mbIgnorePresets = False Then
                            o.Presets.Clear()
                        End If
                        msLastColSelected = o.DisplayName
                    End If

                    Return o

                End If
            Next

            Return Nothing

        End Get
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal DisplayName As String) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a System Color by its displayname
        '
        'Parameters: index
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '6/11/09    MSW     support index by valve description, too.
        ' 10/07/10  MSW     Undo the last one
        '********************************************************************************************

        Get
            Dim sTestName As String = String.Empty
            'Dim sValveName As String = String.Empty
            Dim o As clsSysColor

            For Each o In List

                sTestName = o.DisplayName
                'sValveName = o.Valve.DisplayName
                If sTestName = DisplayName Then 'Or sValveName = DisplayName Then
                    ' if we are switching colors clear out the data
                    If sTestName <> msLastColSelected Then
                        If mbIgnorePresets = False Then
                            o.Presets.Clear()
                        End If
                        msLastColSelected = sTestName
                    End If
                    Return o
                    Exit Property
                End If
            Next
            Return Nothing
        End Get
    End Property
    Friend ReadOnly Property ByValve(ByVal DisplayName As String) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a System Color by its displayname
        '
        'Parameters: index
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/12/10  MSW     Support index by valve description, but do it the right way this time.
        '********************************************************************************************

        Get
            Dim sTestName As String = String.Empty
            Dim sValveName As String = String.Empty
            Dim o As clsSysColor

            For Each o In List

                sTestName = o.DisplayName
                sValveName = o.Valve.DisplayName
                If sValveName = DisplayName Then 'sTestName = DisplayName Then
                    ' if we are switching colors clear out the data
                    If sTestName <> msLastColSelected Then
                        If mbIgnorePresets = False Then
                            o.Presets.Clear()
                        End If
                        msLastColSelected = sTestName
                    End If
                    Return o
                    Exit Property
                End If
            Next
            Return Nothing
        End Get
    End Property
    Friend ReadOnly Property ByValve(ByVal ValveNumber As Integer) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a System Color by its displayname
        '
        'Parameters: index
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/12/10  MSW     Support index by valve number, but do it the right way this time.
        '********************************************************************************************
        Get
            Dim o As clsSysColor

            For Each o In List
                If ValveNumber = o.Valve.Number.Value Then
                    ' if we are switching colors clear out the data
                    If o.DisplayName <> msLastColSelected Then
                        If mbIgnorePresets = False Then
                            o.Presets.Clear()
                        End If
                        msLastColSelected = o.DisplayName
                    End If

                    Return o

                End If
            Next

            Return Nothing

        End Get
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal DisplayName As String, _
                                                ByVal DisplayStyle As Boolean) As clsSysColor
        '********************************************************************************************
        'Description: Get or set a System Color by its displayname
        '               yet more style color hacking
        'Parameters: index
        'Returns:    clsSysColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Dim sTestName As String = String.Empty
            Dim o As clsSysColor

            For Each o In List

                sTestName = o.DisplayName(DisplayStyle)

                If sTestName = DisplayName Then
                    ' if we are switching colors clear out the data
                    If sTestName <> msLastColSelected Then
                        If mbIgnorePresets = False Then
                            o.Presets.Clear()
                        End If
                        msLastColSelected = sTestName

                    End If
                    Return o
                    Exit Property
                End If
            Next
            Return Nothing
        End Get
    End Property
    Friend ReadOnly Property ColorsByStyle() As Boolean
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
            Return mbColorsByStyle
        End Get
    End Property
    Friend ReadOnly Property Robot() As clsArm
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
            Return mRobot
        End Get
    End Property
    Friend ReadOnly Property EffectiveColors() As Integer
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
            Return mnEffectiveColors
        End Get
    End Property
    Friend ReadOnly Property MaximumPlantColorNumber() As Integer
        Get
            Return mnMaxColNumber
        End Get
    End Property
    Friend ReadOnly Property MinimumPlantColorNumber() As Integer
        Get
            Return mnMinColNumber
        End Get
    End Property
    Friend ReadOnly Property MaximumValveNumber() As Integer
        Get
            Return msColorValveName.GetUpperBound(0)
        End Get
    End Property
    Friend ReadOnly Property MinimumValveNumber() As Integer
        'MSW 9/01/09 - change to use value from data instead of constant
        Get
            Return mnMinValveNumber
        End Get
    End Property
    'Friend ReadOnly Property NumberOfColorsTheirUsing() As Integer
    '    '********************************************************************************************
    '    'Description: needs to go away
    '    '
    '    'Parameters: 
    '    'Returns:    
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************

    '    Get
    '        'this is more fallout from wilmington colors per style thing and a quick hack
    '        ' this is when maxcolors <> effectivecolors and colorstheirusing < effectivecolors
    '        Return 18
    '    End Get
    'End Property
    Friend ReadOnly Property Use2K() As Boolean
        '********************************************************************************************
        'Description: Using 2K?
        '
        'Parameters: none
        'Returns:    t or f
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUse2K
        End Get
    End Property

    Friend ReadOnly Property UseTricoat() As Boolean
        '********************************************************************************************
        'Description: Using 2K?
        '
        'Parameters: none
        'Returns:    t or f
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseTricoat
        End Get
    End Property
    Friend ReadOnly Property UseTwoCoat() As Boolean
        '********************************************************************************************
        'Description: Using 2K?
        '
        'Parameters: none
        'Returns:    t or f
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbTwoCoats
        End Get
    End Property

#End Region
#Region " Routines"

    Friend Function Add(ByVal Value As clsSysColor) As Integer
        '********************************************************************************************
        'Description: Add a system color to collection
        '
        'Parameters: clsSysColor
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return List.Add(Value)

    End Function 'Add        
    Friend Function Contains(ByVal value As clsSysColor) As Boolean
        '********************************************************************************************
        'Description: If value is not of type system color, this will return false.
        '
        'Parameters: clsSysColor
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Contains(value)
    End Function 'Contains
    Friend Function IndexOf(ByVal value As clsSysColor) As Integer
        '********************************************************************************************
        'Description: Get Index of system color in collection
        '
        'Parameters: clsSysColor
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysColor)
        '********************************************************************************************
        'Description: Add a system color at specific location
        '
        'Parameters: position,clsSysColor
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysColor)
        '********************************************************************************************
        'Description: Remove a system color 
        '
        'Parameters: clsSysColor
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Sub Load(ByRef rRobot As clsArm)
        '********************************************************************************************
        'Description: Load the collection of colors from the xml file
        '               NOTE: Changes to this routine may also be needed to be made to 
        '               mPWcommon.loadcolorboxfromdb
        '
        'Parameters: owning Robot
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/24/08  gks     Change to use SQL Database
        ' 02/23/10  BTK     Added Tricoat Tab
        ' 12/13/12  MSW     clsSysColors.Load check list length against zone setting
        '********************************************************************************************
        Dim sColorValveNames() As String = Nothing
        Dim oColors As XmlNodeList = Nothing
        Dim lTmp As Integer = 0
        Dim nUB As Integer
        Dim oZone As clsZone = rRobot.Controller.Zone

        'get rid of old data if any
        If List.Count > 0 Then List.Clear()

        mRobot = rRobot

        Try
            If GetSystemColorInfoFromDB(oZone, oColors, _
                                          sColorValveNames, mbColorIsAscii, mbColorsByStyle, mbUse2K, mbUseTricoat, mnPlantAsciiMaxLength, mbTwoCoats) Then

                'Valves
                If (sColorValveNames Is Nothing) = False Then
                    nUB = sColorValveNames.GetUpperBound(0) + 1
                    ReDim msColorValveName(nUB)
                    For nValve As Integer = 1 To nUB
                        'these should be sorted already but...
                        msColorValveName(nValve) = sColorValveNames(nValve - 1)
                    Next

                End If  '(dsValves Is Nothing) = False

                'Colors
                If (oColors Is Nothing) = False Then

                    Dim nColorIndex As Integer = 0
                    For Each oNode As XmlNode In oColors
                        Dim o As clsSysColor = New clsSysColor(rRobot)
                        With o
                            .PlantNumber.Text = oNode.Item(gsSYSC_COL_PLANTNUM).InnerText
                            .FanucNumber = CType(oNode.Item(gsSYSC_COL_FANUCNUM).InnerText, Integer)
                            .Description.Text = oNode.Item(gsSYSC_COL_DESC).InnerText
                            .Valve.Number.Value = CType(oNode.Item(gsSYSC_COL_VALVENUM).InnerText, Integer)
                            lTmp = CType(oNode.Item(gsSYSC_COL_ARGB).InnerText, Integer)
                            If lTmp = 0 Then
                                .DisplayColor = System.Drawing.SystemColors.Control
                            Else
                                .DisplayColor = Color.FromArgb(lTmp)
                            End If
                            If .Valve.Number.Value <= UBound(msColorValveName) Then
                                .Valve.Description.Text = msColorValveName(.Valve.Number.Value)
                            End If
                            .DisplayName = .PlantNumber.Text & " - " & .Description.Text
                            .RobotsRequired.Value = CType(oNode.Item(gsSYSC_COL_ENABLE).InnerText, Integer)

                            ' 02/23/10  BTK     Added Tricoat Tab
                            If mbUseTricoat Then
                                .Tricoat.Text = oNode.Item(gsSYSC_COL_TRICOATNUM).InnerText
                            End If
                            If mbTwoCoats Then
 
                                Try
                                    .TwoCoats.Value = CType(oNode.Item(gsSYSC_COL_TWOCOATS).InnerText, Boolean)
                                Catch ex As Exception
                                    .TwoCoats.Value = False
                                End Try
                            End If

                            If mbUse2K Then
                                .Use2K = True
                                Try
                                    .HardenerValve.Value = CType(oNode.Item(gsSYSC_COL_H_VALVE).InnerText, Integer)
                                Catch ex As Exception
                                    .HardenerValve.Value = 1
                                End Try
                                Try
                                    .ResinRatio.Value = CType(oNode.Item(gsSYSC_COL_R_RATIO).InnerText, Integer)
                                Catch ex As Exception
                                    .ResinRatio.Value = 1
                                End Try
                                Try
                                    .HardenerRatio.Value = CType(oNode.Item(gsSYSC_COL_H_RATIO).InnerText, Integer)
                                Catch ex As Exception
                                    .HardenerRatio.Value = 1
                                End Try
                                Try
                                    .ResinSolventValve.Value = CType(oNode.Item(gsSYSC_COL_R_SOLV).InnerText, Integer)
                                Catch ex As Exception
                                    .ResinSolventValve.Value = 1
                                End Try
                                Try
                                    .HardenerSolventValve.Value = CType(oNode.Item(gsSYSC_COL_H_SOLV).InnerText, Integer)
                                Catch ex As Exception
                                    .HardenerSolventValve.Value = 1
                                End Try
                                'not used as yet
                                .ResinValve.Value = 0

                            End If 'mbUse2K

                        End With
                        Add(o)
                        ' 12/13/12  MSW     clsSysColors.Load check list length against zone setting
                        If List.Count = oZone.MaxColors Then
                            Exit For
                        End If
                    Next

                End If ' (dsColors Is Nothing) = False

            End If  'GetSystemColorInfoFromDB
 
            mnEffectiveColors = List.Count

            'loaded the colors - now check for the styles by color hack
            If mbColorsByStyle Then
                Dim o As New clsSysStyles(oZone)

                Dim sStyleInfo() As String
                sStyleInfo = o.GetStyleInfoArray

                'this is going to hurt if the database is set up wrong
                Dim nStyles As Integer = o.Count
                Dim nColors As Integer = CInt(List.Count / nStyles)
                Call UpdateForPresetsByStyle(sStyleInfo, nStyles, nColors)
                mnEffectiveColors = nColors
            End If

        Catch ex As Exception
            Debug.WriteLine("Module: clsSysColors, Routine: Load, Error: " & ex.Message)
            Debug.WriteLine("Module: clsSysColors, Routine: Load, StackTrace: " & ex.StackTrace)
        End Try


    End Sub
    Friend Function UpdateForPresetsByStyle(ByVal sStyleDesc() As String, ByVal nStyles As Integer, _
                                            ByVal nEffectiveColors As Integer) As Boolean
        '********************************************************************************************
        'Description: This is for the presets by style hack
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

        Try
            'first some sanity checks
            If nStyles - 1 <> UBound(sStyleDesc) Then Return False
            If nStyles < 1 Then Return False
            If List.Count / nStyles <> nEffectiveColors Then Return False

            Dim nStyle As Integer
            Dim nColor As Integer
            Dim sPlantCols(nEffectiveColors - 1) As String

            'plant colors only valid for 1st style - this is the "colors" we display
            For nColor = 0 To nEffectiveColors - 1
                Dim o As clsSysColor = DirectCast(List(nColor), clsSysColor)
                o.StyleDesc = String.Empty
                'this is just color number and desc here
                sPlantCols(nColor) = o.DisplayName
            Next

            'give them bogus names
            For nStyle = 0 To nStyles - 1
                For nColor = 0 To nEffectiveColors - 1
                    Dim nPtr As Integer = (nStyle * nEffectiveColors) + nColor
                    Dim o As clsSysColor = DirectCast(List(nPtr), clsSysColor)
                    o.StyleDesc = sStyleDesc(nStyle)
                    ''this was adding desc twice  5/22/07
                    'o.PlantNumber.Text = sPlantCols(nColor)
                    o.DisplayName = o.StyleDesc & " " & sPlantCols(nColor)
                    o.ColorsByStyle = True
                    o.Update()
                Next
            Next

            mbColorsByStyle = True
            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function ValveNumberFromValveName(ByVal sValveDisplayName As String) As Integer
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
        Try
            For Each o As clsSysColor In List
                If o.Valve.DisplayName = sValveDisplayName Then
                    Return o.Valve.Number.Value
                End If
            Next
            Return 0
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return 0
        End Try
    End Function
    Friend Function FanucNumberFromValveName(ByVal sValveDisplayName As String) As Integer
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
        Try
            For Each o As clsSysColor In List
                If o.Valve.DisplayName = sValveDisplayName Then
                    Return o.FanucNumber
                End If
            Next
            Return 0
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return 0
        End Try
    End Function
    Friend Function FanucNumberFromValveNumber(ByVal nValve As Integer) As Integer
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
        Try
            For Each o As clsSysColor In List
                If o.Valve.Number.Value = nValve Then
                    Return o.FanucNumber
                End If
            Next
            Return 0
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return 0
        End Try
    End Function
    Friend Shadows Sub Update()
        '********************************************************************************************
        'Description:  make old = new
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each o As clsSysColor In List
            o.Update()
        Next
    End Sub
    Protected Friend Sub RefreshValveDescriptions()
        '********************************************************************************************
        'Description:  make sure desc match for valve number - when valve # is changed etc.
        '
        'Parameters:  none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each o As clsSysColor In List
            Try
                o.Valve.Description.Text = msColorValveName(o.Valve.Number.Value)
            Catch ex As Exception
                o.Valve.Description.Text = gcsRM.GetString("csNO_DESCRIPTION")
            End Try
        Next
    End Sub

#End Region
#Region " Events "

    Friend Sub New(ByRef rRobot As clsArm)
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
        Call Load(rRobot)
    End Sub

#End Region

End Class 'clsSysColors
'********************************************************************************************
'Description: System Color class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsSysColor

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop
    Private moDesc As clsTextValue ' = Nothing
    Private mnFanucNo As Integer ' = 0
    Private moPlantNo As clsTextValue ' = Nothing
    Private moPresets As clsPresets ' = Nothing
    Private moRobReq As clsIntValue ' = Nothing
    Private moValve As clsValve ' = Nothing
    Private msStyleDesc As String = String.Empty
    Private mbColorsByStyle As Boolean ' = False
    Private mRobot As clsArm ' = Nothing
    Private msDisplayName As String
    Private moColor As System.Drawing.Color = System.Drawing.SystemColors.Control
    'BTK 02/23/10 Added Tricoat Tab
    Private moTricoat As clsTextValue ' = Nothing
    Private mbUseTricoat As Boolean
    'jbw 02/11/11 Added two coats box
    Private mbTwoCoats As clsBoolValue
    '2k
    Private moHardRatio As clsSngValue '= Nothing
    Private moHardValve As clsIntValue '= Nothing
    Private moHardSolValve As clsIntValue '= Nothing
    Private moResinRatio As clsSngValue '= Nothing
    Private moResinSolValve As clsIntValue '= Nothing
    Private moResinValve As clsIntValue '= Nothing
    Private mbUse2K As Boolean

#End Region
#Region " Properties"

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
        ' 02/23/10  BTK     Added Tricoat Tab
        '********************************************************************************************
        Get
            If moPlantNo.Changed Then Return True
            If moDesc.Changed Then Return True
            If (moValve Is Nothing) = False Then
                If moValve.Changed Then Return True
            End If
            If (moRobReq Is Nothing) = False Then
                If moRobReq.Changed Then Return True
            End If

            ' 02/23/10  BTK     Added Tricoat Tab
            If mbUseTricoat And (moTricoat Is Nothing) = False Then
                If moTricoat.Changed Then Return True
            End If
            If mbTwoCoats.changed Then Return True

            '2K
            If Not mbUse2K Then Return False

            If moResinRatio.Changed Then Return True
            If moHardRatio.Changed Then Return True
            If moResinValve.Changed Then Return True
            If moHardValve.Changed Then Return True
            If moResinSolValve.Changed Then Return True
            If moHardSolValve.Changed Then Return True
            If moResinSolValve.Changed Then Return True

            Return False

        End Get
    End Property
    Friend ReadOnly Property Description() As clsTextValue
        '********************************************************************************************
        'Description: Whats it called
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moDesc
        End Get
    End Property
    Friend ReadOnly Property Tricoat() As clsTextValue
        '********************************************************************************************
        'Description: Whats it called
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moTricoat
        End Get
    End Property
    Friend ReadOnly Property TwoCoats() As clsBoolValue
        '********************************************************************************************
        'Description: Whats it called
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/18/11  JBW     created
        '********************************************************************************************
        Get
            Return mbTwoCoats
        End Get
    End Property
    Friend Property StyleDesc() As String
        '********************************************************************************************
        'Description: Whats it called for the presets by style hack
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msStyleDesc
        End Get
        Set(ByVal Value As String)
            msStyleDesc = Value
        End Set
    End Property
    Friend Property DisplayColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: A color that can be used for cartoons etc - picked from a dialog
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            moColor = value
        End Set
    End Property
    Friend Property DisplayName() As String
        '********************************************************************************************
        'Description:  This property is to generate a name to display in comboboxes etc
        '               and so we can search on what's in combo and not its listindex
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '5/22/07    gks     changed from read only - still a TODO
        '********************************************************************************************
        Get
            Dim sTmp As String = String.Empty
            If msDisplayName Is Nothing Then
                sTmp = moPlantNo.Text & " - " & moDesc.Text
            Else
                sTmp = msDisplayName
            End If
            Return sTmp

        End Get
        Set(ByVal value As String)
            msDisplayName = value
        End Set
    End Property
    Friend ReadOnly Property DisplayName(ByVal bIncludeStyleDesc As Boolean) As String
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
            Dim sTmp As String = String.Empty
            If (mbColorsByStyle And bIncludeStyleDesc) Then
                sTmp = StyleDesc & " " & moPlantNo.Text & " - " & moDesc.Text
            Else
                sTmp = moPlantNo.Text & " - " & moDesc.Text
            End If
            Return sTmp
        End Get
    End Property
    Friend Property FanucNumber() As Integer
        '********************************************************************************************
        'Description: pointer into fanuc table
        '
        'Parameters:  none
        'Returns:    position
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnFanucNo
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Exit Property
            mnFanucNo = Value
        End Set
    End Property
    Friend ReadOnly Property PlantNumber() As clsTextValue
        '********************************************************************************************
        'Description: How does the plant call it
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moPlantNo
        End Get
    End Property
    Friend ReadOnly Property Presets() As clsPresets
        '********************************************************************************************
        'Description: Collection of fluid presets goes here
        '
        'Parameters:  none
        'Returns:    Presets
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/21/07   gks     add displayname and fanuc color
        '********************************************************************************************
        Get
            If moPresets Is Nothing Then
                moPresets = New clsPresets
                moPresets.DisplayName = Me.DisplayName
                moPresets.FanucColor = Me.FanucNumber
                moPresets.ColorChangePresets = False
            End If

            Return moPresets

        End Get
    End Property
    Friend ReadOnly Property RobotsRequired() As clsIntValue
        '********************************************************************************************
        'Description: Does the owning robot do this color?
        '
        'Parameters:  none
        'Returns:    true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moRobReq
        End Get
    End Property
    Friend Property ColorsByStyle() As Boolean
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
            Return mbColorsByStyle
        End Get
        Set(ByVal value As Boolean)
            mbColorsByStyle = value
        End Set
    End Property
    Friend ReadOnly Property Valve() As clsValve
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
            If moValve Is Nothing Then
                moValve = New clsValve(mRobot)
            End If
            Return moValve
        End Get
    End Property

    '2k
    Friend ReadOnly Property HardenerRatio() As clsSngValue
        '********************************************************************************************
        'Description: HardenerRatio
        '
        'Parameters:  none
        'Returns:    ratio
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moHardRatio Is Nothing Then
                moHardRatio = New clsSngValue
            End If
            Return moHardRatio
        End Get
    End Property
    Friend ReadOnly Property HardenerValve() As clsIntValue
        '********************************************************************************************
        'Description: HardenerValve
        '
        'Parameters:  none
        'Returns:    valve number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moHardValve Is Nothing Then
                moHardValve = New clsIntValue
            End If
            Return moHardValve
        End Get
    End Property
    Friend ReadOnly Property HardenerSolventValve() As clsIntValue
        '********************************************************************************************
        'Description: HardenerSolventValve
        '
        'Parameters:  none
        'Returns:    valve number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moHardSolValve Is Nothing Then
                moHardSolValve = New clsIntValue
            End If
            Return moHardSolValve
        End Get
    End Property
    Friend ReadOnly Property ResinRatio() As clsSngValue
        '********************************************************************************************
        'Description: ResinRatio
        '
        'Parameters:  none
        'Returns:    ratio
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moResinRatio Is Nothing Then
                moResinRatio = New clsSngValue
            End If
            Return moResinRatio
        End Get
    End Property
    Friend ReadOnly Property ResinValve() As clsIntValue
        '********************************************************************************************
        'Description: ResinValve
        '
        'Parameters:  none
        'Returns:    valve number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moResinValve Is Nothing Then
                moResinValve = New clsIntValue
            End If
            Return moResinValve
        End Get
    End Property
    Friend ReadOnly Property ResinSolventValve() As clsIntValue
        '********************************************************************************************
        'Description: ResinSolventValve
        '
        'Parameters:  none
        'Returns:    valve number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moResinSolValve Is Nothing Then
                moResinSolValve = New clsIntValue
            End If
            Return moResinSolValve
        End Get
    End Property
    Protected Friend Property Use2K() As Boolean
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
            Return mbUse2K
        End Get
        Set(ByVal value As Boolean)
            mbUse2K = value
        End Set
    End Property
    Protected Friend Property UseTricoat() As Boolean
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/23/10  BTK     Added Tricoat Tab
        '********************************************************************************************
        Get
            Return mbUseTricoat
        End Get
        Set(ByVal value As Boolean)
            mbUseTricoat = value
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: make 0ld = new
        '
        'Parameters:  none
        'Returns:   none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moPlantNo.Update()
        moDesc.Update()
        If (moValve Is Nothing) = False Then
            moValve.Update()
        End If
        If (moRobReq Is Nothing) = False Then
            moRobReq.Update()
        End If

        If mbUseTricoat And (moTricoat Is Nothing) Then
            moTricoat.Update()
        End If
        'jbw added twocoat stuff
        mbTwoCoats.Update()
        '2K
        If Not mbUse2K Then Exit Sub

        moResinRatio.Update()
        moHardRatio.Update()
        moResinValve.Update()
        moHardValve.Update()
        moResinSolValve.Update()
        moHardSolValve.Update()

    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByVal rRobot As clsArm)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/23/10  BTK     Added Tricoat Tab
        '********************************************************************************************
        mRobot = rRobot
        moDesc = New clsTextValue
        moPlantNo = New clsTextValue
        moRobReq = New clsIntValue
        moValve = New clsValve(mRobot)

        moTricoat = New clsTextValue
        'jbw added
        mbTwoCoats = New clsBoolValue
        '2K
        moHardRatio = New clsSngValue
        moHardValve = New clsIntValue
        moHardSolValve = New clsIntValue
        moResinRatio = New clsSngValue
        moResinSolValve = New clsIntValue
        moResinValve = New clsIntValue

    End Sub

#End Region

End Class 'clsSysColor
'********************************************************************************************
'Description: System Color 2K class
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 

'********************************************************************************************
'Description: Valve Class
'
'
'Modification history:
'
' Date      By      Reason
' 07/26/07  gks     made number a clsIntValue
'******************************************************************************************** 
Friend Class clsValve

#Region " Declares "

    '9.5.07 remove initialization per fxCop
    Private moNumber As clsIntValue ' = Nothing
    Private moDesc As clsTextValue ' = Nothing
    Private moOpenLoop As clsBoolValue ' = Nothing
    Private moColorChange As clsColorChange ' = Nothing
    Private mRobot As clsArm ' = Nothing

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
            If moDesc.Changed Then Return True

            If (moOpenLoop Is Nothing) = False Then
                If moOpenLoop.Changed Then Return True
            End If

            If (moColorChange Is Nothing) = False Then
                If moColorChange.Changed Then Return True
            End If

            If moNumber.Changed Then Return True

            Return False

        End Get
    End Property
    Friend ReadOnly Property ColorChange() As clsColorChange
        '********************************************************************************************
        'Description: color change cycle stuff
        '
        'Parameters:  none
        'Returns:   color change cycles
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moColorChange Is Nothing Then
                moColorChange = New clsColorChange(mRobot)
            End If
            Return moColorChange
        End Get
    End Property
    Friend ReadOnly Property Description() As clsTextValue
        '********************************************************************************************
        'Description: Give it a name
        '
        'Parameters:  none
        'Returns:    text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moDesc
        End Get
    End Property
    Friend ReadOnly Property Number() As clsIntValue
        '********************************************************************************************
        'Description: Give it a number
        '
        'Parameters:  none
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moNumber
        End Get

    End Property
    Friend ReadOnly Property OpenLoop() As clsBoolValue
        '********************************************************************************************
        'Description: for accuflow 
        '
        'Parameters:  none
        'Returns:    closed loop status
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moOpenLoop Is Nothing Then
                moOpenLoop = New clsBoolValue
            End If
            Return moOpenLoop
        End Get
    End Property
    Friend ReadOnly Property DisplayName() As String
        '********************************************************************************************
        'Description: for combo boxes etc
        '
        'Parameters:  none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moNumber.Value.ToString(mLanguage.CurrentCulture) & " - " & moDesc.Text
        End Get
    End Property


#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: make old = new
        '
        'Parameters:  none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moDesc.Update()
        moNumber.Update()
        If (moColorChange Is Nothing) = False Then
            moColorChange.Update()
        End If
        If (moOpenLoop Is Nothing) = False Then
            moOpenLoop.Update()
        End If
    End Sub

#End Region
#Region " Events "

    Friend Sub New(ByVal rRobot As clsArm)
        '********************************************************************************************
        'Description: valve number set to 0 in the new of clsintvalue
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        mRobot = rRobot
        moDesc = New clsTextValue
        moNumber = New clsIntValue

    End Sub
    

#End Region

End Class 'clsValve
Friend Module mSysColorCommon
    Const msMODULE As String = "mSysColorCommon"
    '********************************************************************************************
    'Description: system color module - moved routines from mpwcommon
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '******************************************************************************************** 
    Friend Function bSaveValvesToXML(ByRef oZone As clsZone, ByRef sValveNames() As String) As Boolean
        '********************************************************************************************
        'Description: Save color valve names to XML
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Save valves to XML
        Const sXMLFILE As String = "ColorValves"
        Const sXMLTABLE As String = "ColorValves"
        Const sXMLNODE As String = "ColorValve"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oZoneNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        'NVSP MixRoom Computer Changes 12/13/2016
        Dim bFilePath As Boolean = False
        Dim RemotePWServer As String = "\\" & oZone.ServerName & oZone.ShareName

        Try
            'NVSP MixRoom Computer Changes 12/13/2016
            If Not IsNothing(oZone) And oZone.IsRemoteZone Then
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, RemotePWServer, sXMLFILE & ".XML")
            Else
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            End If

            'NVSP MixRoom Computer Changes 12/13/2016
            If bFilePath Then
                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                If oZoneNode Is Nothing Then
                    oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                    oMainNode.AppendChild(oZoneNode)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                End If
            Else
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement(sXMLTABLE)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                oMainNode.AppendChild(oZoneNode)
                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bSaveValvesToXML", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            Return False
        End Try
        oZoneNode.RemoveAll()

        Try

            For nValve As Integer = 0 To sValveNames.GetUpperBound(0)

                'Build XML node
                Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                Dim oNumNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ValveNumber", Nothing)
                oNumNode.InnerXml = (nValve + 1).ToString
                oNode.AppendChild(oNumNode)
                Dim oNameNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_DESC, Nothing)
                oNameNode.InnerXml = sValveNames(nValve)
                oNode.AppendChild(oNameNode)
                oZoneNode.AppendChild(oNode)
            Next

            oXMLDoc.Save(sXMLFilePath)

        Catch ex As Exception

            mDebug.WriteEventToLog(msMODULE & ":bSaveValvesToXML", ex.Message)
            Return False
        End Try

        Return True
    End Function
    Friend Function bSaveColorsToXML(ByRef oZone As clsZone, ByRef oColors As clsSysColors) As Boolean
        '********************************************************************************************
        'Description: Save color table to XML
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/30/12  RJO     Bug fix. The RobotsRequired value was being saved to the ARGB field.
        '********************************************************************************************
        'Save valves to XML
        Const sXMLFILE As String = "SystemColors"
        Const sXMLTABLE As String = "SystemColors"
        Const sXMLNODE As String = "SystemColor"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oZoneNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        'NVSP MixRoom Computer Changes 12/13/2016
        Dim bFilePath As Boolean = False
        Dim RemotePWServer As String = "\\" & oZone.ServerName & oZone.ShareName

        Try
            'NVSP MixRoom Computer Changes 12/13/2016
            If Not IsNothing(oZone) And oZone.IsRemoteZone Then
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, RemotePWServer, sXMLFILE & ".XML")
            Else
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            End If
            'NVSP MixRoom Computer Changes 12/13/2016
            If bFilePath Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                If oZoneNode Is Nothing Then
                    oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                    oMainNode.AppendChild(oZoneNode)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                End If
            Else
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement(sXMLTABLE)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                oMainNode.AppendChild(oZoneNode)
                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bSaveColorsToXML", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            Return False
        End Try
        oZoneNode.RemoveAll()

        Try

            For Each oColor As clsSysColor In oColors

                'Build XML node
                Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "PlantNumber", Nothing)
                oPNNode.InnerXml = oColor.PlantNumber.Text
                oNode.AppendChild(oPNNode)
                Dim oFNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "FanucNumber", Nothing)
                oFNNode.InnerXml = oColor.FanucNumber.ToString
                oNode.AppendChild(oFNNode)
                Dim oVNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ValveNumber", Nothing)
                oVNNode.InnerXml = oColor.Valve.Number.Value.ToString
                oNode.AppendChild(oVNNode)
                Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_DESC, Nothing)
                oDescNode.InnerXml = oColor.Description.Text
                oNode.AppendChild(oDescNode)
                Dim oRRNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_ENABLE, Nothing)
                oRRNode.InnerXml = oColor.RobotsRequired.Value.ToString
                oNode.AppendChild(oRRNode)
                Dim oRGBNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_ARGB, Nothing)
                'oRGBNode.InnerXml = oColor.RobotsRequired.Value.ToString
                oRGBNode.InnerXml = oColor.DisplayColor.ToArgb.ToString 'RJO 08/30/12
                oNode.AppendChild(oRGBNode)
                Dim oTCNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "TricoatNumber", Nothing)
                oTCNNode.InnerXml = oColor.Tricoat.Text
                oNode.AppendChild(oTCNNode)
                Dim o2CNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_TWOCOATS, Nothing)
                o2CNode.InnerXml = oColor.TwoCoats.Value.ToString
                oNode.AppendChild(o2CNode)

                Dim oHdrNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerValve", Nothing)
                oHdrNode.InnerXml = oColor.HardenerValve.Value.ToString
                oNode.AppendChild(oHdrNode)
                Dim oResRatNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ResinRatio", Nothing)
                oResRatNode.InnerXml = oColor.ResinRatio.Value.ToString
                oNode.AppendChild(oResRatNode)
                Dim oHdrRatNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerRatio", Nothing)
                oHdrRatNode.InnerXml = oColor.HardenerRatio.Value.ToString
                oNode.AppendChild(oHdrRatNode)
                Dim oResSlvNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ResinSolvent", Nothing)
                oResSlvNode.InnerXml = oColor.ResinSolventValve.Value.ToString
                oNode.AppendChild(oResSlvNode)
                Dim oHdrSlvNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerSolvent", Nothing)
                oHdrSlvNode.InnerXml = oColor.HardenerSolventValve.Value.ToString
                oNode.AppendChild(oHdrSlvNode)

                oZoneNode.AppendChild(oNode)
            Next

            oXMLDoc.Save(sXMLFilePath)

        Catch ex As Exception

            mDebug.WriteEventToLog(msMODULE & ":bSaveColorsToXML", ex.Message)
            Return False
        End Try

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

        Dim sTmp As String = PlantNumber & " - "
        sTmp = sTmp & Description

        Return sTmp

    End Function
    Friend Function GetValveTableDataset(ByRef oZone As clsZone) As String()
        '********************************************************************************************
        'Description: This Routine gets an XMLNodeList with the color valve table
        '
        'Parameters: name of zone of interest 
        'Returns:    XMLNodeList or nothing if problem
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sXMLFILE As String = "ColorValves"
        Const sXMLTABLE As String = "ColorValves"
        Const sXMLNODE As String = "ColorValve"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Dim sColorValveNames() As String = Nothing
        Dim bRepair As Boolean = False
        'NVSP MixRoom Computer Changes 12/13/2016
        Dim bFilePath As Boolean = False
        Dim RemotePWServer As String = "\\" & oZone.ServerName & oZone.ShareName

        Try
            'NVSP MixRoom Computer Changes 12/13/2016
            If Not IsNothing(oZone) And oZone.IsRemoteZone Then
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, RemotePWServer, sXMLFILE & ".XML")
            Else
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            End If

            'NVSP MixRoom Computer Changes 12/13/2016
            If bFilePath Then
                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & oZone.Name & "//" & sXMLNODE)
                Try
                    ReDim sColorValveNames(oZone.MaxValves - 1)
                    For nValve As Integer = 1 To oZone.MaxValves
                        sColorValveNames(nValve - 1) = "Color " & nValve.ToString
                    Next
                    For Each oNode As XmlNode In oNodeList
                        Try
                            Dim nValve As Integer = CInt(oNode.Item("ValveNumber").InnerText)
                            If nValve <= oZone.MaxValves Then
                                sColorValveNames(nValve - 1) = oNode.Item(gsSYSC_COL_DESC).InnerText()
                            End If
                        Catch ex As Exception
                            bRepair = True
                        End Try
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":GetValveTableDataset", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    bRepair = True
                End Try
            Else
                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            bRepair = True
        End Try
        'Repair
        If bRepair Then
            bSaveValvesToXML(oZone, sColorValveNames)
        End If

        Return sColorValveNames
    End Function
    Friend Function GetColorTableDataset(ByRef oZone As clsZone) As XmlNodeList
        '********************************************************************************************
        'Description: This Routine gets a dataset with the System Colors table
        '
        'Parameters: name of zone of interest 
        'Returns:    Dataset or nothing if problem
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/21/13  AGP     GetColorTableDataset - Change from ADD
        '********************************************************************************************
        Const sXMLFILE As String = "SystemColors"
        Const sXMLTABLE As String = "SystemColors"
        Const sXMLNODE As String = "SystemColor"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oZoneNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Dim sColorValveNames() As String = Nothing
        Dim bRepair As Boolean = False
        'NVSP MixRoom Computer Changes 12/13/2016
        Dim bFilePath As Boolean = False
        Dim RemotePWServer As String = "\\" & oZone.ServerName & oZone.ShareName

        Try
            'NVSP MixRoom Computer Changes 12/13/2016
            If Not IsNothing(oZone) And oZone.IsRemoteZone Then
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, RemotePWServer, sXMLFILE & ".XML")
            Else
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            End If

            'NVSP MixRoom Computer Changes 12/13/2016
            If bFilePath Then
                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                If oZoneNode Is Nothing Then
                    oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                    oMainNode.AppendChild(oZoneNode)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                End If
                Try
                    If oZoneNode IsNot Nothing Then
                        While oZoneNode.ChildNodes.Count < oZone.MaxColors
                            'Dim sCol As String = (oNodeList.Count + 1).ToString
                            ' 08/21/13  AGP     fix 
                            Dim sCol As String = (oZoneNode.ChildNodes.Count + 1).ToString
                            Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                            Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "PlantNumber", Nothing)
                            oPNNode.InnerXml = sCol
                            oNode.AppendChild(oPNNode)
                            Dim oFNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "FanucNumber", Nothing)
                            oFNNode.InnerXml = sCol
                            oNode.AppendChild(oFNNode)
                            Dim oVNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ValveNumber", Nothing)
                            oVNNode.InnerXml = "1"
                            oNode.AppendChild(oVNNode)
                            Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_DESC, Nothing)
                            oDescNode.InnerXml = "Color " & sCol
                            oNode.AppendChild(oDescNode)
                            Dim oRRNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_ENABLE, Nothing)
                            oRRNode.InnerXml = "0"
                            oNode.AppendChild(oRRNode)
                            Dim oRGBNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_ARGB, Nothing)
                            oRGBNode.InnerXml = "0"
                            oNode.AppendChild(oRGBNode)
                            Dim oTCNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "TricoatNumber", Nothing)
                            oTCNNode.InnerXml = "0"
                            oNode.AppendChild(oTCNNode)
                            Dim o2CNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSC_COL_TWOCOATS, Nothing)
                            o2CNode.InnerXml = "False"
                            oNode.AppendChild(o2CNode)

                            Dim oHdrNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerValve", Nothing)
                            oHdrNode.InnerXml = "1"
                            oNode.AppendChild(oHdrNode)
                            Dim oResRatNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ResinRatio", Nothing)
                            oResRatNode.InnerXml = "1"
                            oNode.AppendChild(oResRatNode)
                            Dim oHdrRatNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerRatio", Nothing)
                            oHdrRatNode.InnerXml = "1"
                            oNode.AppendChild(oHdrRatNode)
                            Dim oResSlvNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ResinSolvent", Nothing)
                            oResSlvNode.InnerXml = "1"
                            oNode.AppendChild(oResSlvNode)
                            Dim oHdrSlvNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "HardenerSolvent", Nothing)
                            oHdrSlvNode.InnerXml = "1"
                            oNode.AppendChild(oHdrSlvNode)
                            oZoneNode.AppendChild(oNode)

                            bRepair = True
                        End While
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":GetColorTableDataset", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            Else
                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":GetColorTableDataset", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            bRepair = True
        End Try
        'Repair
        If bRepair Then
            oXMLDoc.Save(sXMLFilePath)
        End If

        oNodeList = oMainNode.SelectNodes("//" & oZone.Name & "//" & sXMLNODE)
        Return oNodeList


    End Function
    Friend Function GetSystemColorInfoFromDB(ByVal Zone As clsZone, _
                                    ByRef oColors As XmlNodeList, ByRef sColorValveNames() As String, _
                                    ByRef UseAscii As Boolean, ByRef ColorsByStyle As Boolean, _
                                    ByRef Use2K As Boolean, ByRef UseTricoat As Boolean, _
                                    ByRef PlantAsciiMaxLength As Integer, ByRef TwoCoats As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Load color information from SQL database
        '
        'Parameters: 
        'Returns:    true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/18/11  JBW     Added two coats stuff
        ' 03/28/12  MSW     Move system color setup table to XML
        '********************************************************************************************
        Const sXMLFILE As String = "SystemColorSetup"
        Const sXMLTABLE As String = "SystemColorSetup"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        'NVSP MixRoom Computer Changes 12/13/2016
        Dim bFilePath As Boolean = False
        Dim RemotePWServer As String = "\\" & Zone.ServerName & Zone.ShareName

        Try
            'NVSP MixRoom Computer Changes 12/13/2016
            If Not IsNothing(Zone) And Zone.IsRemoteZone Then
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, RemotePWServer, sXMLFILE & ".XML")
            Else
                bFilePath = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            End If

            'NVSP MixRoom Computer Changes 12/13/2016
            If bFilePath Then
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If oMainNode IsNot Nothing Then
                    'load ScreenData structure
                    ColorsByStyle = CType(oMainNode.Item("PresetsByStyle").InnerXml, Boolean)
                    UseAscii = CType(oMainNode.Item("UseAscii").InnerXml, Boolean)
                    Try
                        PlantAsciiMaxLength = CType(oMainNode.Item("AsciiCharacters").InnerXml, Integer)
                    Catch ex As Exception
                        PlantAsciiMaxLength = 0
                    End Try
                    Use2K = CType(oMainNode.Item("Use2K").InnerXml, Boolean)
                    UseTricoat = CType(oMainNode.Item("UseTricoat").InnerXml, Boolean)
                Else
                    Throw New MissingFieldException("Unable to access " & sXMLTABLE)
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "mSysColorCommon" & " Routine: GetSystemColorInfoFromDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        '********************************************************************************************    
        'colorvalve names 
        sColorValveNames = GetValveTableDataset(Zone)

        If (sColorValveNames Is Nothing) Then
            Throw New MissingFieldException("Unable to access Valve Table")
        End If

        '********************************************************************************************    
        'System Colors

        oColors = GetColorTableDataset(Zone)

        If (oColors Is Nothing) Then

            'this aint good
            Throw New MissingFieldException("Unable to access Color Table")
        End If ' DS.Tables.Contains("[" & gsSYSC_DS_TABLENAME & "]") zone

        Return True
    End Function
    Friend Function LoadColorBoxFromDB(ByRef cboParam As ComboBox, ByRef colZones As clsZones, _
                     ByVal bAddAll As Boolean, Optional ByVal bEnabledOnly As Boolean = False, _
                     Optional ByRef bAsciiColor As Boolean = False, _
                     Optional ByVal nPlantAsciiMaxLength As Integer = 0) As Boolean
        '********************************************************************************************
        'Description:  Load color combo with color names
        '               NOTE: Changes to this routine may also be needed to be made to 
        '               syscolors.clssyscolors.load
        '
        'Parameters: zone data, cbo
        'Returns:    cbo.tag holds the plant color number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Add bEnabledOnly to optionally load only enabled items
        ' 02/23/10  BTK     Added Tricoat Tab
        ' 02/18/11  JBW     Added two coats stuff
        '********************************************************************************************    
        Dim sColorValveNames() As String = Nothing
        Dim bColorsByStyle As Boolean
        Dim bUse2K As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean
        Dim sTmp As String = String.Empty
        Dim sTag As String()
        If cboParam Is Nothing Then Return False
        Dim nTagIndex As Integer = 0
        Dim oColors As XmlNodeList = Nothing

        cboParam.Items.Clear()


        If bAddAll Then
            cboParam.Items.Add(gcsRM.GetString("csALL"))
            nTagIndex += 1
        End If

        Try
            'Load color box from databse

            If GetSystemColorInfoFromDB(colZones.ActiveZone, oColors, _
                                        sColorValveNames, bAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, nPlantAsciiMaxLength, bTwoCoats) Then

                Dim nStyles As Integer = 1
                If bColorsByStyle Then
                    'if colors by style, we need to know how many styles as there are a set of colors
                    'for each style
                    Dim o As New clsSysStyles(colZones.ActiveZone)
                    If o.Count > 0 Then nStyles = o.Count
                End If

                If oColors IsNot Nothing Then

                    Dim nUB As Integer = oColors.Count ' + 1

                    ' nstyles is 1 if not colors by style
                    nUB = nUB \ nStyles

                    ReDim sTag(oColors.Count)
                    sTag(0) = "*" ' all

                    Dim bAdd As Boolean = False

                    For i As Integer = 0 To nUB - 1
                        Debug.Print(oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText)
                        If bEnabledOnly Then
                            bAdd = CInt(oColors(i).Item(gsSYSC_COL_ENABLE).InnerText) > 0
                        Else
                            bAdd = True
                        End If
                        If bAdd Then
                            'fanuc number
                            sTag(nTagIndex) = oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText

                            sTmp = ColorStringForDisplay(oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText, _
                                        oColors(i).Item(gsSYSC_COL_DESC).InnerText)
                            cboParam.Items.Add(sTmp)
                            nTagIndex += 1
                        End If
                    Next 'i
                Else
                    'this aint good
                    Throw New MissingFieldException(gsSYSC_DS_TABLENAME & " is missing " & _
                                                                    gsSYSC_DS_TABLENAME & " table")
                End If ' DS.Tables.Contains("[" & gsSYSC_DS_TABLENAME & "]")


            Else
                Return False

            End If



            cboParam.Tag = sTag
            Return True

        Catch ex As Exception

            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, StackTrace: " & ex.StackTrace)

            Return False
        End Try

    End Function

    Friend Function LoadColorBoxFromDB(ByRef rClb As CheckedListBox, ByRef oZone As clsZone, _
                                        ByRef bAddAll As Boolean, Optional ByRef bEnabledOnly As Boolean = False, _
                                        Optional ByRef bAsciiColor As Boolean = False, _
                                        Optional ByRef nPlantAsciiMaxLength As Integer = 0) As Boolean
        '********************************************************************************************
        'Description:  Load Listbox for Reports
        '               NOTE: Changes to this routine may also be needed to be made to 
        '               syscolors.clssyscolors.load
        '
        'Parameters: sLookupParams is Plant  number  Here
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Add bEnabledOnly to optionally load only enabled items
        ' 02/23/10  BTK     Added Tricoat Tab
        ' 02/18/11  JBW     Added two coats stuff
        '********************************************************************************************    
        Dim oColors As XmlNodeList = Nothing
        Dim sColorValveNames() As String = Nothing
        Dim bColorsByStyle As Boolean
        Dim bUse2K As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean
        Dim sTmp As String = String.Empty
        Dim sLookupParams As String()
        Dim nTagIndex As Integer = 0

        If rClb Is Nothing Then Return False

        rClb.Items.Clear()
        ReDim sLookupParams(0)

        If bAddAll Then
            rClb.Items.Add(gcsRM.GetString("csALL"))
            sLookupParams(0) = gcsRM.GetString("csALL")
            nTagIndex += 1
        End If

        Try
            'Load color box from databse

            If GetSystemColorInfoFromDB(oZone, oColors, _
                                        sColorValveNames, bAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, nPlantAsciiMaxLength, bTwoCoats) Then

                Dim nStyles As Integer = 1
                If bColorsByStyle Then
                    'if colors by style, we need to know how many styles as there are a set of colors
                    'for each style
                    Dim o As New clsSysStyles(oZone)
                    If o.Count > 0 Then nStyles = o.Count
                End If

                If oColors IsNot Nothing Then

                    Dim nUB As Integer = oColors.Count ' + 1

                    ' nstyles is 1 if not colors by style
                    nUB = nUB \ nStyles

                    ReDim Preserve sLookupParams(oColors.Count)
                    sLookupParams(0) = "*"
                    Dim bAdd As Boolean = False

                    For i As Integer = 0 To nUB - 1
                        If bEnabledOnly Then
                            bAdd = CInt(oColors(i).Item(gsSYSC_COL_ENABLE).InnerText) > 0
                        Else
                            bAdd = True
                        End If
                        If bAdd Then
                            'Plant number
                            sLookupParams(nTagIndex) = oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText

                            sTmp = ColorStringForDisplay(oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText, _
                                        oColors(i).Item(gsSYSC_COL_DESC).InnerText)

                            rClb.Items.Add(sTmp)
                            nTagIndex += 1

                        End If
                    Next 'i

                Else
                    'this aint good

                    Throw New MissingFieldException(gsSYSC_DS_TABLENAME & " is missing " & _
                                                                    gsSYSC_DS_TABLENAME & " table")
                End If ' DS.Tables.Contains("[" & gsSYSC_DS_TABLENAME & "]")


            Else

                Return False

            End If

            rClb.Tag = sLookupParams

            Return True

        Catch ex As Exception

            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, StackTrace: " & ex.StackTrace)

            Return False
        End Try

    End Function
    Friend Function LoadColorBoxFromDB(ByRef cboParam As ComboBox, ByRef colZones As clsZones, _
                        ByRef sLookupParams() As String, ByVal bAddAll As Boolean, _
                        Optional ByVal bEnabledOnly As Boolean = False, _
                        Optional ByRef bAsciiColor As Boolean = False, _
                        Optional ByVal nPlantAsciiMaxLength As Integer = 0) As Boolean
        '********************************************************************************************
        'Description:  Load color combo with color names
        '               NOTE: Changes to this routine may also be needed to be made to 
        '               syscolors.clssyscolors.load
        '
        'Parameters: sLookupParams is fanuc number
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Add bEnabledOnly to optionally load only enabled items
        ' 02/23/10  BTK     Added Tricoat Tab
        ' 02/18/11  JBW     Added two coats stuff
        '********************************************************************************************    
        Dim sColorValveNames() As String = Nothing
        Dim oColors As XmlNodeList = Nothing
        Dim bColorsByStyle As Boolean
        Dim bUse2K As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean
        Dim sTmp As String = String.Empty
        Dim nTagIndex As Integer = 0

        If cboParam Is Nothing Then Return False

        cboParam.Items.Clear()


        If bAddAll Then
            cboParam.Items.Add(gcsRM.GetString("csALL"))
            nTagIndex += 1
        End If

        Try
            'Load color box from databse

            If GetSystemColorInfoFromDB(colZones.ActiveZone, oColors, _
                                        sColorValveNames, bAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, nPlantAsciiMaxLength, bTwoCoats) Then

                Dim nStyles As Integer = 1
                If bColorsByStyle Then
                    'if colors by style, we need to know how many styles as there are a set of colors
                    'for each style
                    Dim o As New clsSysStyles(colZones.ActiveZone)
                    If o.Count > 0 Then nStyles = o.Count
                End If

                If oColors IsNot Nothing Then

                    Dim nUB As Integer = oColors.Count ' + 1

                    ' nstyles is 1 if not colors by style
                    nUB = nUB \ nStyles

                    ReDim sLookupParams(oColors.Count)
                    sLookupParams(0) = "*" ' all

                    Dim bAdd As Boolean = False

                    For i As Integer = 0 To nUB - 1
                        If bEnabledOnly Then
                            bAdd = CInt(oColors(i).Item(gsSYSC_COL_ENABLE).InnerText) > 0
                        Else
                            bAdd = True
                        End If
                        If bAdd Then
                            'fanuc number
                            sLookupParams(nTagIndex) = oColors(i).Item(gsSYSC_COL_FANUCNUM).InnerText

                            sTmp = ColorStringForDisplay(oColors(i).Item(gsSYSC_COL_PLANTNUM).InnerText, _
                                        oColors(i).Item(gsSYSC_COL_DESC).InnerText)
                            cboParam.Items.Add(sTmp)

                            nTagIndex += 1
                        End If
                    Next 'i

                Else
                    'this aint good

                    Throw New MissingFieldException(gsSYSC_DS_TABLENAME & " is missing " & _
                                                                    gsSYSC_DS_TABLENAME & " table")
                End If ' DS.Tables.Contains("[" & gsSYSC_DS_TABLENAME & "]")


            Else
                Return False

            End If
            cboParam.Tag = sLookupParams
            Return True

        Catch ex As Exception

            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, StackTrace: " & ex.StackTrace)

            Return False
        End Try

    End Function
    Friend Sub LoadColorBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                    ByRef rCbo As ComboBox, ByVal JustEffectiveColors As Boolean, _
                                    Optional ByVal bPlantColorTag As Boolean = False)
        '********************************************************************************************
        'Description:  More fallout from the colors by style hack
        '
        'Parameters: color collection , combo to load
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/01/10  MSW     Support tag with either plant or fanuc number
        ' 11/06/12  RJO     Removed "NumberOfColorsTheirUsing" hack.
        '********************************************************************************************    
        If JustEffectiveColors = False Then
            LoadColorBoxFromCollection(ColorCollection, rCbo)
            Exit Sub
        End If


        Dim sTag As String()

        Dim o As clsSysColor
        ReDim sTag(0)

        rCbo.Items.Clear()
        '??assume load in order
        For Each o In ColorCollection
            'attach so we can use instead of listindex
            ReDim Preserve sTag(rCbo.Items.Count)
            If bPlantColorTag Then
                sTag(rCbo.Items.Count) = o.PlantNumber.Text
            Else
                sTag(rCbo.Items.Count) = o.FanucNumber.ToString(mLanguage.FixedCulture)
            End If
            'more colors by style hacking
            rCbo.Items.Add(o.DisplayName(False))

            If rCbo.Items.Count = ColorCollection.EffectiveColors Then Exit For
            'this is a case where the colors per style was 20 but theyre only using 18
            'If rCbo.Items.Count = ColorCollection.NumberOfColorsTheirUsing Then Exit For 'RJO 11/06/12
        Next

        rCbo.Tag = sTag

        If rCbo.Items.Count = 1 Then
            'this selects the color in cases of just one color. 
            rCbo.Text = rCbo.Items(0).ToString
        End If

    End Sub
    Friend Sub LoadColorBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                                    ByRef rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load color combo with robot names
        '
        'Parameters: color collection , combo to load
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim o As clsSysColor
        Dim sFanuc As String()

        rCbo.Items.Clear()
        ReDim sFanuc(0)

        For Each o In ColorCollection
            'attach so we can use instead of listindex
            ReDim Preserve sFanuc(rCbo.Items.Count)
            sFanuc(rCbo.Items.Count) = o.FanucNumber.ToString(mLanguage.FixedCulture)

            rCbo.Items.Add(o.DisplayName)
        Next

        If rCbo.Items.Count = 1 Then

            'this selects the color in cases of just one color. 
            rCbo.Text = rCbo.Items(0).ToString
        End If

        rCbo.Tag = sFanuc

    End Sub
    Friend Sub LoadColorBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                                    ByRef rLst As CheckedListBox)
        '********************************************************************************************
        'Description:  Load color combo with robot names
        '
        'Parameters: color collection , combo to load
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/21/07   gks     add tag
        '********************************************************************************************    
        Dim o As clsSysColor
        Dim sFanuc As String()

        rLst.Items.Clear()
        rLst.BeginUpdate()
        ReDim sFanuc(0)

        For Each o In ColorCollection
            'attach so we can use instead of listindex
            ReDim Preserve sFanuc(rLst.Items.Count)
            sFanuc(rLst.Items.Count) = o.FanucNumber.ToString(mLanguage.FixedCulture)

            rLst.Items.Add(o.DisplayName)
        Next

        If rLst.Items.Count = 1 Then
            'this selects the color in cases of just one color. 
            rLst.SelectedIndex = 0
        End If

        rLst.EndUpdate()

        rLst.Tag = sFanuc

    End Sub
    Friend Sub LoadValveBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                            ByRef rCbo As ComboBox, _
                                                            Optional ByVal bIndexByValve As Boolean = False)
        '********************************************************************************************
        'Description:  Load control with valve names
        '
        'Parameters: color collection , cobntrol to load
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '07/10/08   gks     moved here from mrobotcommon
        '********************************************************************************************    
        Dim o As clsSysColor
        Dim sValves() As String
        Dim sFanucNum() As String
        Dim i%
        Dim bFound As Boolean = False

        rCbo.Items.Clear()
        ReDim sValves(0)
        ReDim sFanucNum(0)

        For Each o In ColorCollection
            Dim sTmp As String = o.Valve.DisplayName
            bFound = False
            Dim ub As Integer = UBound(sValves)
            For i% = 0 To ub
                If sTmp = sValves(i%) Then
                    bFound = True
                    Exit For
                End If
            Next

            If Not bFound Then
                ReDim Preserve sValves(ub + 1)
                ReDim Preserve sFanucNum(ub)
                sValves(ub + 1) = sTmp
                If bIndexByValve Then
                    'use the fanuc number to init this valve in case valves are out of order
                    sFanucNum(ub) = o.Valve.Number.Value.ToString(mLanguage.FixedCulture)
                Else
                    'use the fanuc number to init this valve in case valves are out of order
                    sFanucNum(ub) = o.FanucNumber.ToString(mLanguage.FixedCulture)
                End If
            End If

        Next

        For i% = 1 To UBound(sValves)
            rCbo.Items.Add(sValves(i%))
        Next

        rCbo.Tag = sFanucNum

        If rCbo.Items.Count = 1 Then
            'this selects the color in cases of just one color. 
            rCbo.Text = rCbo.Items(0).ToString
        End If

    End Sub
    Friend Sub LoadValveBoxFromCollection(ByRef ColorCollection As clsSysColors, _
                                                            ByRef rClb As CheckedListBox, _
                                                            Optional ByVal bIndexByValve As Boolean = False)
        '********************************************************************************************
        'Description:  Load control with valve names
        '
        'Parameters: color collection , cobntrol to load
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '07/10/08   gks     moved here from mrobotcommon
        '********************************************************************************************    
        Dim o As clsSysColor
        Dim sValves() As String
        Dim sFanucNum() As String
        Dim i%
        Dim bFound As Boolean = False

        rClb.Items.Clear()
        ReDim sValves(0)
        ReDim sFanucNum(0)

        For Each o In ColorCollection
            Dim sTmp As String = o.Valve.DisplayName
            bFound = False
            Dim ub As Integer = UBound(sValves)
            For i% = 0 To ub
                If sTmp = sValves(i%) Then
                    bFound = True
                    Exit For
                End If
            Next

            If Not bFound Then
                ReDim Preserve sValves(ub + 1)
                ReDim Preserve sFanucNum(ub)
                sValves(ub + 1) = sTmp
                If bIndexByValve Then
                    'use the fanuc number to init this valve in case valves are out of order
                    sFanucNum(ub) = o.Valve.Number.Value.ToString(mLanguage.FixedCulture)
                Else
                    'use the fanuc number to init this valve in case valves are out of order
                    sFanucNum(ub) = o.FanucNumber.ToString(mLanguage.FixedCulture)
                End If
            End If

        Next

        For i% = 1 To UBound(sValves)
            rClb.Items.Add(sValves(i%))
        Next

        rClb.Tag = sFanucNum

    End Sub
    Friend Function LoadValveBoxFromDB(ByRef cboParam As ComboBox, ByRef colZones As clsZones, _
                 ByRef sLookupParams() As String, ByVal bAddAll As Boolean, _
                        Optional ByRef bAsciiColor As Boolean = False, _
                        Optional ByVal nPlantAsciiMaxLength As Integer = 0, Optional ByVal bUse2K As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  Load color combo with color names
        '               NOTE: Changes to this routine may also be needed to be made to 
        '               syscolors.clssyscolors.load
        '
        'Parameters: sLookupParams is fanuc number
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/23/10  BTK     Added Tricoat Tab
        ' 02/18/11  JBW     Added two coats stuff
        '********************************************************************************************    
        Dim sColorValveNames() As String = Nothing
        Dim oColors As XmlNodeList = Nothing
        Dim sTmp As String = String.Empty
        If cboParam Is Nothing Then Return False
        Dim bColorsByStyle As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean

        cboParam.Items.Clear()


        If bAddAll Then cboParam.Items.Add(gcsRM.GetString("csALL"))

        Try
            'Load color box from databse

            If GetSystemColorInfoFromDB(colZones.ActiveZone, oColors, _
                                        sColorValveNames, bAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, nPlantAsciiMaxLength, bTwoCoats) Then



                'Valves
                If (sColorValveNames Is Nothing) = False Then

                    For nValve As Integer = 1 To sColorValveNames.GetUpperBound(0) + 1
                        cboParam.Items.Add(nValve.ToString & " - " & sColorValveNames(nValve - 1))
                    Next
                End If  '(oValves Is Nothing) = False

            Else
                Return False

            End If


            Return True

        Catch ex As Exception

            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: LoadColorBoxFromDB, StackTrace: " & ex.StackTrace)

            Return False
        End Try

    End Function
End Module 'mSysColorCommon