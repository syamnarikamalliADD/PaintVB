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
' Description: Collection for styles and the class for a style
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
'    11/16/09   MSW     Add bEnabledOnly to optionally load only enabled items all load ...box routines
'    04/01/10   MSW     add support for mnPlantAsciiMaxLength
'    12/01/11   MSW     Mark a version                                                4.01.01.00
'    12/13/11   RJO     Added support for Sealer combined Style/Option                4.01.01.01
'    01/10/12   MSW     Ad some error handling to new items that aren't in the old tables 4.01.01.02
'    03/28/12   MSW     Move system style setup table to XML                          4.01.03.00
'    10/25/13   BTK     Tricoat By Style                                              4.01.03.01
'    01/06/14   MSW     Combined Style-Option support for sealer                      4.01.06.00
'    03/18/14   MSW     Make Tricoat deal with old DBs without throwing an exception  4.01.07.00
'    04/21/14   MSW     LoadStyleBoxFromCollection - combined style/option setup for sealer  4.01.07.01
'    05/16/14   BTK     Support degrade by style                                      4.01.07.02
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath


Friend Class clsSysStyles

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsStyles"
    '***** End Module Constants   **************************************************************

    '9.5.07 remove unnecessary initializations per fxCop
    Private moZone As clsZone
    Private mnMaxStyles As Integer ' = 0
    Private mbUseAscii As Boolean ' = False
    Private mnPlantStyleLowerLimit As Integer ' = 0
    Private mnPlantStyleUpperLimit As Integer = 99
    Private mnPlantOptionLowerLimit As Integer ' = 0 'RJO 12/13/11
    Private mnPlantOptionUpperLimit As Integer = 99 'RJO 12/13/11
    Private mnFanucStyleLowerLimit As Integer ' = 0
    Private mnFanucStyleUpperLimit As Integer = 99
    Private mnPlantAsciiMaxLength As Integer = 0
    Private mnDescriptionMaxLength As Integer = 30
    Private mbCombinedStyleOption As Boolean 'RJO 12/13/11
    Private mbStylesByZone As Boolean ' = False
    Private mbTwoCoat As Boolean = False
    Private mbStyleRegisters As Boolean = False
    Private mnRegisterLowerLimit As Integer = 19
    Private mnRegisterUpperLimit As Integer = 20

#End Region

#Region " Properties "


    Friend Property DescriptionMaxLength() As Integer
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
            Return mnDescriptionMaxLength
        End Get
        Set(ByVal value As Integer)
            mnDescriptionMaxLength = value
        End Set
    End Property
    Friend Property FanucStyleMaxValue() As Integer
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
            Return mnFanucStyleUpperLimit
        End Get
        Set(ByVal value As Integer)
            mnFanucStyleUpperLimit = value
        End Set
    End Property
    Friend Property FanucStyleMinValue() As Integer
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
            Return mnFanucStyleLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnFanucStyleLowerLimit = value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal Style As String, _
                                                ByVal StyleIsFanucStyle As Boolean) As clsSysStyle
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
            Dim o As clsSysStyle = Nothing
            For Each o1 As clsSysStyle In List
                If StyleIsFanucStyle Then
                    If o1.FanucNumber.Value.ToString(mLanguage.FixedCulture) = Style Then
                        o = o1
                    End If
                Else
                    If o1.PlantString = Style Then
                        o = o1
                    End If
                End If
            Next
            Return o
        End Get
        Set(ByVal Value As clsSysStyle)

            For Each o1 As clsSysStyle In List
                If StyleIsFanucStyle Then
                    If o1.FanucNumber.Value.ToString(mLanguage.FixedCulture) = Style Then
                        o1 = Value
                    End If
                Else
                    If o1.PlantString = Style Then
                        o1 = Value
                    End If
                End If
            Next

        End Set
    End Property
    Default Friend Overloads Property Item(ByVal index As Integer) As clsSysStyle
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
            Return CType(List(index), clsSysStyle)
        End Get
        Set(ByVal Value As clsSysStyle)
            List(index) = Value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal DisplayName As String) As clsSysStyle
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
            Dim o As clsSysStyle
            For Each o In List
                If o.DisplayName = DisplayName Then
                    Return o
                    Exit Property
                End If
            Next
            Return Nothing
        End Get
        Set(ByVal Value As clsSysStyle)
            Dim o As clsSysStyle
            For Each o In List
                If o.DisplayName = DisplayName Then
                    List(List.IndexOf(o)) = Value
                    Exit Property
                End If
            Next
        End Set
    End Property
    Friend ReadOnly Property MaxStyles() As Integer
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
            MaxStyles = mnMaxStyles
        End Get
    End Property
    Friend Property PlantStyleMaxValue() As Integer
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
            Return mnPlantStyleUpperLimit
        End Get
        Set(ByVal value As Integer)
            mnPlantStyleUpperLimit = value
        End Set
    End Property
    Friend Property PlantStyleMinValue() As Integer
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
            Return mnPlantStyleLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnPlantStyleLowerLimit = value
        End Set
    End Property
    Friend Property PlantOptionMaxValue() As Integer
        '********************************************************************************************
        'Description:
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************
        Get
            Return mnPlantOptionUpperLimit
        End Get
        Set(ByVal value As Integer)
            mnPlantOptionUpperLimit = value
        End Set
    End Property
    Friend Property PlantOptionMinValue() As Integer
        '********************************************************************************************
        'Description:
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************
        Get
            Return mnPlantOptionLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnPlantOptionLowerLimit = value
        End Set
    End Property
    Friend Property CombinedStyleOption() As Boolean
        '********************************************************************************************
        'Description: For Sealer Cells Style and Option can be combined 
        '             (Style * 100) + Option (SSOO)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added to support Sealer
        '********************************************************************************************
        Get
            Return mbCombinedStyleOption
        End Get
        Set(ByVal value As Boolean)
            mbCombinedStyleOption = value
        End Set
    End Property
    Friend Property TwoCoat() As Boolean
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
            TwoCoat = mbTwoCoat
        End Get
        Set(ByVal value As Boolean)
            mbTwoCoat = value
        End Set
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
    Friend Property RegisterMinValue() As Integer
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
            Return mnRegisterLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnRegisterLowerLimit = value
        End Set
    End Property
    Friend Property RegisterMaxValue() As Integer
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
            Return mnRegisterUpperLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnRegisterUpperLimit = value
        End Set
    End Property

    Friend Property StyleRegisters() As Boolean
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
            StyleRegisters = mbStyleRegisters
        End Get
        Set(ByVal value As Boolean)
            mbStyleRegisters = value
        End Set
    End Property
    Friend Property StylesByZone() As Boolean
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
            Return mbStylesByZone
        End Get
        Set(ByVal value As Boolean)
            mbStylesByZone = value
        End Set
    End Property
    Friend Property UseAscii() As Boolean
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
            Return mbUseAscii
        End Get
        Set(ByVal value As Boolean)
            mbUseAscii = value
        End Set
    End Property

#End Region

#Region " Routines"

    Friend Function Add(ByVal Value As clsSysStyle) As Integer
        '********************************************************************************************
        'Description: Add a system style to collection
        '
        'Parameters: clsSysStyle
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With Value
            .UseAscii = UseAscii
            .FanucNumber.MaxValue = FanucStyleMaxValue
            .FanucNumber.MinValue = FanucStyleMinValue
            .Description.MaxLength = DescriptionMaxLength
        End With

        Return List.Add(Value)

    End Function 'Add
    Private Function bLoadCollection(ByRef oZone As clsZone) As Boolean
        '*****************************************************************************************
        'Description: load the styles collection
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '4/7/08     gks     change to sql database
        '12/13/11   RJO     Added support for Sealer combined Style/Option
        '*****************************************************************************************
        Const sXMLFILE As String = "SystemStyles"
        Const sXMLTABLE As String = "SystemStyles"
        Const sXMLNODE As String = "Style"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & oZone.Name & "//" & sXMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", sXMLFilePath & " not found.")
                    Else
                        Dim nIndex As Integer = 0
                        For Each oNode As XmlNode In oNodeList
                            Dim o As New clsSysStyle(nIndex, oNode, moZone, CombinedStyleOption) 'RJO 12/13/11
                            Add(o)
                            nIndex += 1
                            If nIndex = MaxStyles Then Exit For
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                End Try
                While MyBase.Count < MaxStyles
                    Dim o As New clsSysStyle(MyBase.Count + 1, moZone, CombinedStyleOption)
                    Add(o)
                End While
            Else

                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            Return False
        End Try

        Return True
    End Function
    Friend Shadows Function Count() As Integer
        '********************************************************************************************
        'Description: How Many
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return List.Count
    End Function
    Friend Function IndexOf(ByVal value As clsSysStyle) As Integer
        '********************************************************************************************
        'Description: Get Index of system style in collection
        '
        'Parameters: clsSysStyle
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysStyle)
        '********************************************************************************************
        'Description: Add a system style at specific location
        '
        'Parameters: position,clsSysStyle
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysStyle)
        '********************************************************************************************
        'Description: Remove a system style 
        '
        'Parameters: clsSysStyle
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Function Contains(ByVal value As clsSysStyle) As Boolean
        '********************************************************************************************
        'Description: If value is not of type system style, this will return false.
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
    Private Function bGetConfigData(ByRef oZone As clsZone) As Boolean
        '********************************************************************************************
        'Description:   When form is opened, loads the upper and lower limits from the database.
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    12/13/11   RJO     Added support for Sealer combined Style/Option
        '    01/10/12   MSW     Ad some error handling to new items that aren't in the old tables
        '    03/28/12   MSW     Move system style setup table to XML
        '********************************************************************************************
        Const sXMLFILE As String = "SystemStyleSetup"
        Const sXMLTABLE As String = "SystemStyleSetup"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If oMainNode IsNot Nothing Then
                    'load ScreenData structure
                    PlantStyleMinValue = CType(oMainNode.Item("StyleLowerLimit").InnerXml, Integer)
                    PlantStyleMaxValue = CType(oMainNode.Item("StyleUpperLimit").InnerXml, Integer)
                    Try
                        PlantOptionMinValue = CType(oMainNode.Item("OptionLowerLimit").InnerXml, Integer) 'RJO 12/13/11
                        PlantOptionMaxValue = CType(oMainNode.Item("OptionUpperLimit").InnerXml, Integer) 'RJO 12/13/11
                        mbCombinedStyleOption = CType(oMainNode.Item("UseCombinedStyleOption").InnerXml, Boolean)
                    Catch ex As Exception
                        PlantOptionMinValue = 0
                        PlantOptionMaxValue = 0
                        mbCombinedStyleOption = False
                    End Try
                    FanucStyleMinValue = CType(oMainNode.Item("FanucStyleLowerLimit").InnerXml, Integer)
                    FanucStyleMaxValue = CType(oMainNode.Item("FanucStyleUpperLimit").InnerXml, Integer)
                    PlantAsciiMaxLength = 2 'TODO - Hard Coded for now
                    DescriptionMaxLength = gsMAX_DESC_LEN
                    StylesByZone = CType(oMainNode.Item("StylesbyZone").InnerXml, Boolean)
                    UseAscii = CType(oMainNode.Item("UseAscii").InnerXml, Boolean)
                    Try
                        mnPlantAsciiMaxLength = CType(oMainNode.Item("AsciiCharacters").InnerXml, Integer)
                    Catch ex As Exception
                        mnPlantAsciiMaxLength = 0
                    End Try
                    Try
                        mbTwoCoat = CType(oMainNode.Item("UseTwoCoat").InnerXml, Boolean)
                    Catch ex As Exception
                        mbTwoCoat = False
                    End Try
                    Try
                        mbStyleRegisters = CType(oMainNode.Item("UseStyleRegisters").InnerXml, Boolean)
                    Catch ex As Exception
                        mbStyleRegisters = False
                    End Try
                    Try
                        mnRegisterLowerLimit = CType(oMainNode.Item("RegisterMin").InnerXml, Integer)
                    Catch ex As Exception
                        mnRegisterLowerLimit = 19
                    End Try
                    Try
                        mnRegisterUpperLimit = CType(oMainNode.Item("RegisterMax").InnerXml, Integer)
                    Catch ex As Exception
                        mnRegisterUpperLimit = 20
                    End Try

                End If
                mnMaxStyles = oZone.MaxStyles
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetConfigData", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
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
        Try
            Dim sStyleInfo(List.Count - 1) As String

            For i As Integer = 0 To List.Count - 1
                Dim o As clsSysStyle = DirectCast(List.Item(i), clsSysStyle)
                sStyleInfo(i) = "[" & o.PlantString & " - " & _
                                    o.Description.Text & "]"
            Next

            Return sStyleInfo

        Catch ex As Exception
            Dim s(0) As String
            s(0) = String.Empty
            Return s
        End Try

    End Function
    Friend Sub LoadStyleBoxFromCollection(ByRef rCombo As ComboBox, _
                                          Optional ByVal bEnabledOnly As Boolean = False, _
                                          Optional ByVal bFanucStyleTag As Boolean = False, _
                                          Optional ByVal DegradeStyleRbtsReq As Boolean = False, _
                                          Optional ByVal NumberOfDegrades As Integer = 0)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 11/16/09      MSW     Add bEnabledOnly to optionally load only enabled items
        ' 12/17/09      MSW     add bFanucStyleTag to rerturn with fanuc colors for the tag
        '********************************************************************************************
        rCombo.Items.Clear()
        Dim sStyle As String()
        ReDim sStyle(0)
        Dim nPad As Integer = 4
        If mbUseAscii Then
            nPad = mnPlantAsciiMaxLength
        Else
            If mnPlantStyleUpperLimit > 999 Then
                nPad = 4
            ElseIf mnPlantStyleUpperLimit > 99 Then
                nPad = 3
            ElseIf mnPlantStyleUpperLimit > 9 Then
                nPad = 2
            Else
                nPad = 1
            End If
        End If
        For Each o As clsSysStyle In List
            'attach so we can use instead of listindex
            If DegradeStyleRbtsReq Then
                Dim nDegrade As Integer
                Dim bAddStyle As Boolean = False
                For nDegrade = 0 To NumberOfDegrades - 1
                    If (o.DegradeRobotsRequired(nDegrade).Value > 0) Or (bEnabledOnly = False) Then
                        bAddStyle = True
                    End If
                Next
                If bAddStyle Then
                    'attach so we can use instead of listindex
                    ReDim Preserve sStyle(rCombo.Items.Count)
                    If bFanucStyleTag Then
                        sStyle(rCombo.Items.Count) = o.FanucNumber.Value.ToString
                    Else
                        sStyle(rCombo.Items.Count) = o.PlantString
                    End If
                    rCombo.Items.Add(o.DisplayName(bFanucStyleTag, nPad))
                End If
            Else
            If (o.RobotsRequired.Value > 0) Or (bEnabledOnly = False) Then
                ReDim Preserve sStyle(rCombo.Items.Count)
                If bFanucStyleTag Then
                    sStyle(rCombo.Items.Count) = o.FanucNumber.Value.ToString
                Else
                    sStyle(rCombo.Items.Count) = o.PlantString
                End If
                    rCombo.Items.Add(o.DisplayName(bFanucStyleTag, nPad))
                End If
            End If
        Next

        rCombo.Tag = sStyle

    End Sub

    Friend Sub LoadStyleBoxFromCollection(ByRef rCcb As CheckedListBox, ByVal AddAll As Boolean, _
                                          Optional ByVal bEnabledOnly As Boolean = False, _
                                          Optional ByVal bFanucStyleTag As Boolean = False, _
                                          Optional ByVal DegradeStyleRbtsReq As Boolean = False, _
                                          Optional ByVal NumberOfDegrades As Integer = 0)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 11/16/09      MSW     Add bEnabledOnly to optionally load only enabled items
        ' 12/17/09      MSW     add bFanucStyleTag to rerturn with fanuc colors for the tag
        ' 04/21/14      MSW     LoadStyleBoxFromCollection - combined style/option setup for sealer
        '********************************************************************************************
        rCcb.Items.Clear()
        Dim sStyle As String()
        ReDim sStyle(0)

        If AddAll Then
            rCcb.Items.Add(gcsRM.GetString("csALL"))
            sStyle(0) = gcsRM.GetString("csALL")
        End If
        Dim nPad As Integer = 4
        If mbUseAscii Then
            nPad = mnPlantAsciiMaxLength
        Else
            If mnPlantStyleUpperLimit > 999 Then
                nPad = 4
            ElseIf mnPlantStyleUpperLimit > 99 Then
                nPad = 3
            ElseIf mnPlantStyleUpperLimit > 9 Then
                nPad = 2
            Else
                nPad = 1
            End If
        End If
        For Each o As clsSysStyle In List
            'attach so we can use instead of listindex
            If DegradeStyleRbtsReq Then
                Dim nDegrade As Integer
                Dim bAddStyle As Boolean = False
                For nDegrade = 0 To NumberOfDegrades - 1
                    If (o.DegradeRobotsRequired(nDegrade).Value > 0) Or (bEnabledOnly = False) Then
                        bAddStyle = True
                    End If
                Next
                If bAddStyle Then
                    'attach so we can use instead of listindex
                    ReDim Preserve sStyle(rCcb.Items.Count)
                    If bFanucStyleTag Then
                        sStyle(rCcb.Items.Count) = o.FanucNumber.Value.ToString
                    Else
                        sStyle(rCcb.Items.Count) = o.PlantString
                    End If
                    rCcb.Items.Add(o.DisplayName(bFanucStyleTag, nPad))
                End If
            Else
            If (o.RobotsRequired.Value > 0) Or (bEnabledOnly = False) Then
                ReDim Preserve sStyle(rCcb.Items.Count)
                If bFanucStyleTag Then
                    sStyle(rCcb.Items.Count) = o.FanucNumber.Value.ToString
                Else
                    sStyle(rCcb.Items.Count) = o.PlantString
                End If
                    rCcb.Items.Add(o.DisplayName(bFanucStyleTag, nPad))
                End If
            End If
        Next

        rCcb.Tag = sStyle

    End Sub
    Friend Sub Update()
        '********************************************************************************************
        'Description: get the red out
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each o As clsSysStyle In List
            o.Update()
        Next
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByRef oZone As clsZone, Optional ByVal bLoadRepairDescriptions As Boolean = False)
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

        'Initialize
        MyBase.new()
        List.Clear()

        moZone = oZone

        If bGetConfigData(oZone) Then
            'Call CheckDb()  ---- move to styles screen
            'Load the styles collection
            Call bLoadCollection(oZone)
        End If

        If oZone.DegradeStyleRbtsReq Then
            LoadDegradeRobotReq(4)
        End If

        If oZone.RepairPanels > 0 And bLoadRepairDescriptions Then
            ' this loads panel names from database
            LoadRepairPanelDescriptions()
        End If

    End Sub
    Friend Sub LoadRepairPanelDescriptions()
        '********************************************************************************************
        'Description: Load repair panel descriptions for all styles  
        '              
        '
        'Parameters:
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Const sXMLFILE As String = "SystemRepairPanels"
        Const sXMLTABLE As String = "SystemRepairPanels"
        Const sXMLNODE As String = "Style"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oStyleNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                For Each oStyle As clsSysStyle In List
                    oStyleNode = oMainNode.SelectSingleNode("//" & moZone.Name & "//" & sXMLNODE & oStyle.PlantNumber.Text)
                    Try
                        If oStyleNode Is Nothing Then
                            oStyle.InitRepairPanelDescriptions(moZone)
                        Else
                            oStyle.LoadRepairPanelDescriptions(oStyleNode, moZone)
                        End If
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":LoadRepairPanelDescriptions", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                        ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                    End Try
                Next
            Else

                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
        End Try
    End Sub
    Friend Sub LoadDegradeRobotReq(ByVal NumOfDegrade As Integer)
        '********************************************************************************************
        'Description: Load Degrade Robot Required for each degrade option. 
        '              
        '
        'Parameters:
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Const sXMLFILE As String = "SystemDegrade"
        Const sXMLTABLE As String = "SystemDegrade"
        Const sXMLNODE As String = "Style"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oStyleNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                For Each oStyle As clsSysStyle In List
                    oStyleNode = oMainNode.SelectSingleNode("//" & moZone.Name & "//" & sXMLNODE & oStyle.PlantNumber.Text)
                    Try
                        If oStyleNode Is Nothing Then
                            oStyle.InitDegradeRbtReq(moZone)
                        Else
                            oStyle.LoadDegradeRbtReq(oStyleNode, moZone)
                        End If
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":LoadDegradeRobotReq", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                        ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                    End Try
                Next
            Else

                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
        End Try
    End Sub

#End Region

End Class 'clsSysStyles

Friend Class clsSysStyle

#Region " Declarations "

    '9.5.07 remove initialization per fxCop
    Private mnItemNo As Integer ' = 0
    Private mbUseAscii As Boolean = False
    Private moOptionNo As clsTextValue = Nothing
    Private moPlantNo As clsTextValue = Nothing
    Private moFanucNo As clsIntValue = Nothing
    Private moDesc As clsTextValue = Nothing
    Private moRobReq As clsIntValue = Nothing
    Private moPlantSpec As clsPlantSpecific = Nothing
    Private msImageKey As String = "default"
    Private moEntStart As clsIntValue = Nothing
    Private moExitStart As clsIntValue = Nothing
    Private moMuteLen As clsIntValue = Nothing
    Private moZone As clsZone = Nothing
    Private mbTwoCoatStyle As clsBoolValue
    Private moRegisterNo As clsIntValue = Nothing
    Private moRepairPanelDesc() As clsTextValue = Nothing
    Private moDegradeRobReq() As clsIntValue = Nothing
    '10/25/13 BTK Tricoat By Style
    Private moTricoatRobEnb As clsIntValue = Nothing
#End Region

#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description:
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Added two coats by style changed
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        Get
            If moPlantNo.Changed Then Return True
            If moFanucNo.Changed Then Return True
            If moDesc.Changed Then Return True
            If moRobReq.Changed Then Return True
            If moEntStart.Changed Then Return True
            If moExitStart.Changed Then Return True
            If moMuteLen.Changed Then Return True
            If mbTwoCoatStyle.Changed Then Return True 'jbw added
            If moRegisterNo.Changed Then Return True
            If moRepairPanelDesc IsNot Nothing Then
                For Each oDesc As clsTextValue In moRepairPanelDesc
                    If oDesc.Changed Then Return True
                Next
            End If
            '10/25/13 BTK Tricoat By Style
            If moTricoatRobEnb.Changed Then Return True
            If moDegradeRobReq IsNot Nothing Then
                For Each oDegrade As clsIntValue In moDegradeRobReq
                    If oDegrade.Changed Then Return True
                Next
            End If
            Return False
        End Get
    End Property
    Friend ReadOnly Property Description() As clsTextValue
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
            Return moDesc
        End Get
    End Property
    Friend ReadOnly Property RepairPanelDescriptions() As clsTextValue()
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
            Return moRepairPanelDesc
        End Get
    End Property
    Friend ReadOnly Property RepairPanelDescription(ByVal nPanel As Integer) As clsTextValue
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
            If moRepairPanelDesc IsNot Nothing Then
                Dim nIdx As Integer = nPanel - 1
                If nIdx <= moRepairPanelDesc.GetUpperBound(0) Then
                    Return moRepairPanelDesc(nIdx)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property
    Friend ReadOnly Property TwoCoatStyle() As clsBoolValue
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
            Return mbTwoCoatStyle
        End Get
    End Property
    Friend ReadOnly Property DisplayName(Optional ByVal bFanucStyleToo As Boolean = False, Optional ByVal nPad As Integer = 4, Optional ByVal bCombinedStyleOption As Boolean = False) As String
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
            Dim sTmp As String

            sTmp = moPlantNo.Text

            If bCombinedStyleOption Then
                Dim sTmpOpt As String = moOptionNo.Text
                If sTmpOpt.Length = 1 Then
                    sTmpOpt = "0" & sTmpOpt
                End If
                sTmp = sTmp & sTmpOpt
            Else


            End If
            'pad with spaces - try to align - doesnt work great
            Select Case (Len(sTmp) + 4 - nPad)
                Case 1
                    sTmp = sTmp & "       - "
                Case 2
                    sTmp = sTmp & "     - "
                Case 3
                    sTmp = sTmp & "   - "
                Case Else
                    sTmp = sTmp & " - "
            End Select
            If bFanucStyleToo Then
                sTmp = sTmp & "(" & moFanucNo.Value.ToString & ") "
            End If
            sTmp = sTmp & moDesc.Text

            Return sTmp
        End Get
    End Property

    Friend ReadOnly Property EntranceMuteStartCount() As clsIntValue
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
            Return moEntStart
        End Get
    End Property
    Friend ReadOnly Property ExitMuteStartCount() As clsIntValue
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
            Return moExitStart
        End Get
    End Property
    Friend ReadOnly Property FanucNumber() As clsIntValue
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
            Return moFanucNo
        End Get
    End Property
    Friend Property ItemNumber() As Integer
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
            Return mnItemNo
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Exit Property
            mnItemNo = Value
        End Set
    End Property
    Friend Property ImageKey() As String
        Get
            Return msImageKey
        End Get
        Set(ByVal value As String)
            msImageKey = value
        End Set
    End Property
    Friend ReadOnly Property MuteLength() As clsIntValue
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
            Return moMuteLen
        End Get
    End Property
    Friend ReadOnly Property PlantNumber() As clsTextValue
        '********************************************************************************************
        'Description:
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '5/22/08    gks     Change to string - eliminate plant ascii property
        '********************************************************************************************
        Get
            Return moPlantNo
        End Get
    End Property
    Friend ReadOnly Property RobotsRequired() As clsIntValue
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
            Return moRobReq
        End Get
    End Property
    Friend ReadOnly Property DegradeRobotsRequired(ByVal Degrade As Integer) As clsIntValue
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
            Return moDegradeRobReq(Degrade)
        End Get
    End Property
    Friend ReadOnly Property StyleRegister() As clsIntValue
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
            Return moRegisterNo
        End Get
    End Property
    Friend ReadOnly Property TricoatEnable() As clsIntValue
        '********************************************************************************************
        'Description:
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  BTK     Tricoat By Style
        '********************************************************************************************
        Get
            Return moTricoatRobEnb
        End Get
    End Property
    Friend Property UseAscii() As Boolean
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
            UseAscii = mbUseAscii
        End Get
        Set(ByVal value As Boolean)
            mbUseAscii = value
        End Set
    End Property


    Friend ReadOnly Property PlantString() As String
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
            Return moPlantNo.Text
        End Get
    End Property
    Friend ReadOnly Property PlantSpecific() As clsPlantSpecific
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
            Return moPlantSpec
        End Get
    End Property
    Friend ReadOnly Property OptionNumber() As clsTextValue
        '********************************************************************************************
        'Description: Option number for Sealer combined Style/Option
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/13/11   RJO     Added to support Sealer
        '********************************************************************************************
        Get
            Return moOptionNo
        End Get
    End Property
    Friend ReadOnly Property OptionString() As String
        '********************************************************************************************
        'Description: Option string for Sealer combined Style/Option
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/13/11   RJO     Added to support Sealer
        '********************************************************************************************
        Get
            Return moOptionNo.Text
        End Get

    End Property
#End Region

#Region " Routines "
    Friend Sub LoadRepairPanelDescriptions(ByRef oStyleNode As XmlNode, ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Load repair descriptions from XML nodes
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim moRepairPanelDesc(oZone.RepairPanels - 1)
        Dim nIndex As Integer = -1
        For Each oNode As XmlNode In oStyleNode.ChildNodes
            Try
                nIndex = CType(oNode.Item("PanelNumber").InnerText, Integer) - 1
                moRepairPanelDesc(nIndex) = New clsTextValue
                moRepairPanelDesc(nIndex).Text = oNode.Item("Description").InnerText
            Catch ex As Exception
            End Try
        Next
        For nIndex = 0 To oZone.RepairPanels - 1
            If moRepairPanelDesc(nIndex) Is Nothing Then
                moRepairPanelDesc(nIndex) = New clsTextValue
                moRepairPanelDesc(nIndex).Text = "Panel " & (nIndex + 1).ToString
            End If
        Next
    End Sub

    Friend Sub LoadDegradeRbtReq(ByRef oStyleNode As XmlNode, ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Load degrade robot required from XML nodes
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nDegradeIndex As Integer = -1
        ReDim moDegradeRobReq(oZone.NumberOfDegrades - 1)
        For Each oNode As XmlNode In oStyleNode.ChildNodes
            Try
                nDegradeIndex = CType(oNode.Item("DegradeNumber").InnerText, Integer)
                moDegradeRobReq(nDegradeIndex) = New clsIntValue
                moDegradeRobReq(nDegradeIndex).Value = CType(oNode.Item("Enable").InnerText, Integer)
            Catch ex As Exception
            End Try
        Next
        'For nIndex = 0 To oZone.RepairPanels - 1
        '    If moRepairPanelDesc(nIndex) Is Nothing Then
        '        moRepairPanelDesc(nIndex) = New clsTextValue
        '        moRepairPanelDesc(nIndex).Text = "Panel " & (nIndex + 1).ToString
        '    End If
        'Next
    End Sub

    Friend Sub InitRepairPanelDescriptions(ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Load repair descriptions from XML nodes
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim moRepairPanelDesc(oZone.RepairPanels - 1)
        For nIndex As Integer = 0 To oZone.RepairPanels - 1
            If moRepairPanelDesc(nIndex) Is Nothing Then
                moRepairPanelDesc(nIndex) = New clsTextValue
                moRepairPanelDesc(nIndex).Text = "Panel " & (nIndex + 1).ToString
            End If
        Next
    End Sub

    Friend Sub InitDegradeRbtReq(ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Load repair descriptions from XML nodes
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim moDegradeRobReq(oZone.NumberOfDegrades - 1)
        For nIndex As Integer = 0 To oZone.NumberOfDegrades - 1
            If moDegradeRobReq(nIndex) Is Nothing Then
                moDegradeRobReq(nIndex) = New clsIntValue
                moDegradeRobReq(nIndex).Value = 0
            End If
        Next
    End Sub

    Friend Sub Update()
        '********************************************************************************************
        'Description: get the red out
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Added two coat by style configuration for DHAM
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        moPlantNo.Update()
        moOptionNo.Update() 'RJO 13/13/11
        moFanucNo.Update()
        moDesc.Update()
        moRobReq.Update()
        moPlantSpec.Update()
        moEntStart.Update()
        moExitStart.Update()
        moMuteLen.Update()
        'jbw added two coat style
        mbTwoCoatStyle.Update()
        moRegisterNo.Update()
        If moRepairPanelDesc IsNot Nothing Then
            For Each oDesc As clsTextValue In moRepairPanelDesc
                oDesc.Update()
            Next
        End If
        '10/25/13 BTK Tricoat By Style
        moTricoatRobEnb.Update()
        If moDegradeRobReq IsNot Nothing Then
            For Each oDegrade As clsIntValue In moDegradeRobReq
                oDegrade.Update()
            Next
        End If
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByVal vIndex As Integer, ByRef oNode As XmlNode, ByRef oZone As clsZone, _
                   ByVal UseCombinedStyleOption As Boolean)
        '********************************************************************************************
        'Description: Hello world
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Added two coat by style capability for DHAM
        ' 12/13/11  RJO     Added Option to support Sealer combined Style/Option
        ' 10/25/13  BTK     Tricoat By Style
        ' 03/18/14  MSW     Make Tricoat deal with old DBs without throwing an exception
        '********************************************************************************************
        ItemNumber = vIndex
        moZone = oZone
        moPlantNo = New clsTextValue
        moOptionNo = New clsTextValue
        moFanucNo = New clsIntValue
        moDesc = New clsTextValue
        moRobReq = New clsIntValue
        moPlantSpec = New clsPlantSpecific(oZone)
        moEntStart = New clsIntValue
        moExitStart = New clsIntValue
        moMuteLen = New clsIntValue
        mbTwoCoatStyle = New clsBoolValue
        moRegisterNo = New clsIntValue
        '10/25/13 BTK Tricoat By Style
        moTricoatRobEnb = New clsIntValue

        With oNode
            If UseCombinedStyleOption Then 'RJO 12/13/11
                Dim nStyle As Integer = CType(.Item(gsSYSS_COL_PLANTNUM).InnerXml, Integer) \ 100
                Dim nOption As Integer = CType(.Item(gsSYSS_COL_PLANTNUM).InnerXml, Integer) Mod 100

                moPlantNo.Text = nStyle.ToString
                moOptionNo.Text = nOption.ToString
            Else
                moPlantNo.Text = .Item(gsSYSS_COL_PLANTNUM).InnerXml
                moOptionNo.Text = "0"
            End If
            moFanucNo.Value = CType(.Item(gsSYSS_COL_FANUCNUM).InnerXml, Integer)
            moDesc.Text = .Item(gsSYSS_COL_DESC).InnerXml
            moRobReq.Value = CType(.Item(gsSYSS_COL_ENABLE).InnerXml, Integer)
            msImageKey = .Item(gsSYSS_COL_IMG_KEY).InnerXml
            Try
                moEntStart.Value = CType(.Item(gsSYSS_COL_ENT_MUTE).InnerXml, Integer) 'RJO 04/08/10
                moExitStart.Value = CType(.Item(gsSYSS_COL_EXIT_MUTE).InnerXml, Integer) 'RJO 04/08/10
                moMuteLen.Value = CType(.Item(gsSYSS_COL_MUTE_LEN).InnerXml, Integer) 'RJO 04/08/10
            Catch ex As Exception
                moEntStart.Value = 0
                moExitStart.Value = 0
                moMuteLen.Value = 0
            End Try
            Try
                mbTwoCoatStyle.Value = CType(.Item(gsSYSC_COL_TWOCOATS).InnerXml, Boolean)
            Catch ex As Exception
                mbTwoCoatStyle.Value = False
            End Try
            Try
                moRegisterNo.Value = CType(.Item(gsSYSS_REGISTER).InnerXml, Integer)
            Catch ex As Exception
                moRegisterNo.Value = 0
            End Try

            '07/08/13 BTK Tricoat By Style
            '03/18/14  MSW make Tricoat deal with old DBs without throwing an exception
            Try
                If oNode.InnerXml.Contains(gsSYSS_COL_TRICOATENB) Then
                    moTricoatRobEnb.Value = CType(.Item(gsSYSS_COL_TRICOATENB).InnerXml, Integer)
                Else
                    moTricoatRobEnb.Value = 0
                End If
            Catch ex As Exception
                moTricoatRobEnb.Value = 0
            End Try

        End With
        Update()
    End Sub
    Friend Sub New(ByVal vIndex As Integer, ByRef oZone As clsZone, _
                   ByVal UseCombinedStyleOption As Boolean)
        '********************************************************************************************
        'Description: Hello world
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        ItemNumber = vIndex
        moZone = oZone
        moPlantNo = New clsTextValue
        moOptionNo = New clsTextValue
        moFanucNo = New clsIntValue
        moDesc = New clsTextValue
        moRobReq = New clsIntValue
        moPlantSpec = New clsPlantSpecific(oZone)
        moEntStart = New clsIntValue
        moExitStart = New clsIntValue
        moMuteLen = New clsIntValue
        mbTwoCoatStyle = New clsBoolValue
        moRegisterNo = New clsIntValue
        '10/25/13 BTK Tricoat By Style
        moTricoatRobEnb = New clsIntValue



        moPlantNo.Text = vIndex.ToString
        moOptionNo.Text = "0"
        moFanucNo.Value = vIndex
        moDesc.Text = "Style " & vIndex.ToString
        moRobReq.Value = 0
        msImageKey = String.Empty
        moEntStart.Value = 0
        moExitStart.Value = 0
        moMuteLen.Value = 0
        mbTwoCoatStyle.Value = False
        moRegisterNo.Value = 0
        '10/25/13 BTK Tricoat By Style
        moTricoatRobEnb.value = 0

        Update()
    End Sub

#End Region

End Class 'clsSysStyle

Friend Module mSysStyles

#Region " Declarations"

#End Region

#Region " Routines "

#End Region

End Module
Friend Class clsPlantSpecific

#Region " Declarations "

    Private Const mnCONT As Integer = 5
    Private Const mnUBND As Integer = 10
    Private mnDegradeNum(mnCONT, mnUBND) As clsIntValue
    Private mDegradeRepairPanels As clsDegradeRepairPanels
#End Region

#Region " Properties "

    Friend Property DegradeNumber(ByVal ControllerNumber As Integer, _
                                    ByVal RobotNumberDown As Integer) As clsIntValue
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
            Return mnDegradeNum(ControllerNumber, RobotNumberDown)
        End Get
        Set(ByVal value As clsIntValue)
            mnDegradeNum(ControllerNumber, RobotNumberDown) = value
        End Set
    End Property
    Friend ReadOnly Property MaxDegrades() As Integer
        '********************************************************************************************
        'Description: how many degrades per style (number of plc slots per style)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnUBND
        End Get
    End Property
    Friend Property DegradeRepairPanels() As clsDegradeRepairPanels
        Get
            Return mDegradeRepairPanels
        End Get
        Set(ByVal value As clsDegradeRepairPanels)
            mDegradeRepairPanels = value
        End Set
    End Property
#End Region

#Region " Routines "

    Friend Sub Update()
        For nCont As Integer = 1 To mnCONT
            For i As Integer = 1 To mnUBND
                mnDegradeNum(nCont, i).Update()
            Next
            If mDegradeRepairPanels Is Nothing = False Then
                mDegradeRepairPanels.Update()
            End If
        Next
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByRef oZone As clsZone)
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
        For nCont As Integer = 0 To mnCONT
            For i As Integer = 1 To mnUBND
                mnDegradeNum(nCont, i) = New clsIntValue
                mnDegradeNum(nCont, i).Value = i
            Next
        Next
        If oZone.RepairPanels > 0 Then
            mDegradeRepairPanels = New clsDegradeRepairPanels(oZone)

        End If
    End Sub

#End Region

End Class 'clsPlantSpecific