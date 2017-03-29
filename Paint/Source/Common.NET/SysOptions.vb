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
' Form/Module: SysOptions.vb
'
' Description: Collection for Options and the class for a Option
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: George S.
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
'    11/21/11   MSW     add option register init                                        
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    03/28/12   MSW     Move system option setup table to XML                         4.01.03.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath


Friend Class clsSysOptions

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsOptions"
    '***** End Module Constants   **************************************************************

    Private moZone As clsZone
    Private mnMaxOptions As Integer = 0
    Private mnPlantOptionLowerLimit As Integer = 0
    Private mnPlantOptionUpperLimit As Integer = 99
    Private mnFanucOptionLowerLimit As Integer = 0
    Private mnFanucOptionUpperLimit As Integer = 99
    Private mnDescriptionMaxLength As Integer = 30
    Private mbOptionsByZone As Boolean = False
    Private mbUseAscii As Boolean ' = False
    Private mnPlantAsciiMaxLength As Integer = 0
    Private mbOptionRegisters As Boolean = False
    Private mnRegisterLowerLimit As Integer = 17
    Private mnRegisterUpperLimit As Integer = 18

#End Region
#Region " Properties "
    Friend Property OptionRegisters() As Boolean
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
            OptionRegisters = mbOptionRegisters
        End Get
        Set(ByVal value As Boolean)
            mbOptionRegisters = value
        End Set
    End Property

    Friend ReadOnly Property DatabasePath() As String
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
            Return moZone.DatabasePath
        End Get
    End Property
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
    Friend Property FanucOptionMaxValue() As Integer
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
            Return mnFanucOptionUpperLimit
        End Get
        Set(ByVal value As Integer)
            mnFanucOptionUpperLimit = value
        End Set
    End Property
    Friend Property FanucOptionMinValue() As Integer
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
            Return mnFanucOptionLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnFanucOptionLowerLimit = value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal index As Integer) As clsSysOption
        '********************************************************************************************
        'Description: Get or set a system Option by its index
        '
        'Parameters: index
        'Returns:    clsSysOption
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsSysOption)
        End Get
        Set(ByVal Value As clsSysOption)
            List(index) = Value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal DisplayName As String) As clsSysOption
        '********************************************************************************************
        'Description: Get or set a System Option by its displayname
        '
        'Parameters: index
        'Returns:    clsSysOption
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As clsSysOption
            For Each o In List
                If o.DisplayName = DisplayName Then
                    Return o
                    Exit Property
                End If
            Next
            Return Nothing
        End Get
        Set(ByVal Value As clsSysOption)
            Dim o As clsSysOption
            For Each o In List
                If o.DisplayName = DisplayName Then
                    List(List.IndexOf(o)) = Value
                    Exit Property
                End If
            Next
        End Set
    End Property
    Friend ReadOnly Property MaxOptions() As Integer
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
            MaxOptions = mnMaxOptions
        End Get
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
        '********************************************************************************************
        Get
            Return mnPlantOptionLowerLimit
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then Exit Property
            mnPlantOptionLowerLimit = value
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
    Friend Property OptionsByZone() As Boolean
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
            Return mbOptionsByZone
        End Get
        Set(ByVal value As Boolean)
            mbOptionsByZone = value
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


    Private ReadOnly Property Zone() As clsZone
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
            Zone = moZone
        End Get
    End Property

#End Region
#Region " Routines"

    Friend Function Add(ByVal Value As clsSysOption) As Integer
        '********************************************************************************************
        'Description: Add a system Option to collection
        '
        'Parameters: clsSysOption
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With Value
            .FanucNumber.MaxValue = FanucOptionMaxValue
            .FanucNumber.MinValue = FanucOptionMinValue
            .Description.MaxLength = DescriptionMaxLength
        End With

        Return List.Add(Value)

    End Function 'Add
    Private Function bLoadCollection(ByRef oZone As clsZone) As Boolean
        '*****************************************************************************************
        'Description: load the Options collection
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Const sXMLFILE As String = "SystemOptions"
        Const sXMLTABLE As String = "SystemOptions"
        Const sXMLNODE As String = "Option"

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
                            Dim o As New clsSysOption(nIndex, oNode)
                            Add(o)
                            nIndex += 1
                            If nIndex = MaxOptions Then Exit For
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                End Try
                While MyBase.Count < MaxOptions
                    Dim o As New clsSysOption(MyBase.Count + 1)
                    Add(o)
                End While
            Else

                MessageBox.Show("Cannot Find " & sXMLFILE & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)

        End Try

        Return True

    End Function
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
        '    03/28/12   MSW     Move system option setup table to XML
        '********************************************************************************************
        Const sXMLFILE As String = "SystemOptionSetup"
        Const sXMLTABLE As String = "SystemOptionSetup"

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
                    PlantOptionMinValue = CType(oMainNode.Item("OptionLowerLimit").InnerXml, Integer)
                    PlantOptionMaxValue = CType(oMainNode.Item("OptionUpperLimit").InnerXml, Integer)
                    FanucOptionMinValue = CType(oMainNode.Item("FanucOptionLowerLimit").InnerXml, Integer)
                    FanucOptionMaxValue = CType(oMainNode.Item("FanucOptionUpperLimit").InnerXml, Integer)
                    PlantAsciiMaxLength = 2 'TODO - Hard Coded for now
                    DescriptionMaxLength = gsMAX_DESC_LEN
                    OptionsByZone = CType(oMainNode.Item("OptionByZone").InnerXml, Boolean)
                    UseAscii = CType(oMainNode.Item("UseAscii").InnerXml, Boolean)
                    Try
                        mnPlantAsciiMaxLength = CType(oMainNode.Item("AsciiCharacters").InnerXml, Integer)
                    Catch ex As Exception
                        mnPlantAsciiMaxLength = 0
                    End Try
                    Try
                        mbOptionRegisters = CType(oMainNode.Item("UseOptionRegisters").InnerXml, Boolean)
                    Catch ex As Exception
                        mbOptionRegisters = False
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
                    mnMaxOptions = oZone.MaxOptions

                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetConfigData", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
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
    Friend Function IndexOf(ByVal value As clsSysOption) As Integer
        '********************************************************************************************
        'Description: Get Index of system Option in collection
        '
        'Parameters: clsSysOption
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysOption)
        '********************************************************************************************
        'Description: Add a system Option at specific location
        '
        'Parameters: position,clsSysOption
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysOption)
        '********************************************************************************************
        'Description: Remove a system Option 
        '
        'Parameters: clsSysOption
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Function Contains(ByVal value As clsSysOption) As Boolean
        '********************************************************************************************
        'Description: If value is not of type system Option, this will return false.
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
    Friend Sub LoadOptionBoxFromCollection(ByRef rCombo As ComboBox, _
                        Optional ByVal bEnabledOnly As Boolean = False)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 11/16/09  MSW     Add bEnabledOnly to optionally load only enabled items 
        '********************************************************************************************
        rCombo.Items.Clear()
        Dim sOption As String()
        ReDim sOption(0)

        For Each o As clsSysOption In List
            If (o.RobotsRequired.Value > 0) Or (bEnabledOnly = False) Then
                'attach so we can use instead of listindex
                ReDim Preserve sOption(rCombo.Items.Count)
                sOption(rCombo.Items.Count) = o.PlantString

                rCombo.Items.Add(o.DisplayName)

            End If
        Next

        rCombo.Tag = sOption

    End Sub
    Friend Sub LoadOptionBoxFromCollection(ByRef rCcb As CheckedListBox, ByVal AddAll As Boolean, _
                        Optional ByVal bEnabledOnly As Boolean = False)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 11/16/09  MSW     Add bEnabledOnly to optionally load only enabled items 
        '********************************************************************************************
        rCcb.Items.Clear()
        Dim sOption As String()
        ReDim sOption(0)

        If AddAll Then
            rCcb.Items.Add(gcsRM.GetString("csALL"))
            sOption(0) = gcsRM.GetString("csALL")
        End If

        For Each o As clsSysOption In List
            If (o.RobotsRequired.Value > 0) Or (bEnabledOnly = False) Then
                'attach so we can use instead of listindex
                ReDim Preserve sOption(rCcb.Items.Count)
                sOption(rCcb.Items.Count) = o.PlantString

                rCcb.Items.Add(o.DisplayName)

            End If            
        Next

        rCcb.Tag = sOption

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
        For Each o As clsSysOption In List
            o.Update()
        Next
    End Sub

#End Region
#Region " Events "
    Friend Sub New(ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Create a collection of Options based on the 'Options' table located at
        '             vDatabasePath that consists of Options from vZone and has vMaxOptions items.
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


    End Sub

#End Region

End Class 'clsSysOptions

Friend Class clsSysOption

#Region " Declarations "

    Private mnItemNo As Integer = 0
    Private moPlantNo As clsTextValue = Nothing
    Private moFanucNo As clsIntValue = Nothing
    Private moDesc As clsTextValue = Nothing
    Private moRobReq As clsIntValue = Nothing
    Private moRegisterNo As clsIntValue = Nothing

    '    Private moPlantSpec As clsPlantSpecific = Nothing
    ' Private msImageKey As String = "default"

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
        '********************************************************************************************
        Get
            If moPlantNo.Changed Then Return True
            If moFanucNo.Changed Then Return True
            If moDesc.Changed Then Return True
            If moRobReq.Changed Then Return True
            If moRegisterNo.Changed Then Return True
            Return False
        End Get
    End Property
    Friend ReadOnly Property OptionRegister() As clsIntValue
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
            Dim sTmp As String

            sTmp = moPlantNo.Text
            'pad with spaces - try to align - doesnt work great
            Select Case Len(sTmp)
                Case 1
                    sTmp = sTmp & "       -  "
                Case 2
                    sTmp = sTmp & "     -  "
                Case 3
                    sTmp = sTmp & "   -  "
                Case 4
                    sTmp = sTmp & "  -  "
            End Select

            sTmp = sTmp & moDesc.Text

            Return sTmp
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
    '    Friend ReadOnly Property PlantSpecific() As clsPlantSpecific
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
    '        Get
    '            Return moPlantSpec
    '        End Get
    '    End Property

#End Region
#Region " Routines "
    Friend Sub New(ByVal vIndex As Integer, ByRef oNode As XmlNode)
        ItemNumber = vIndex
        moPlantNo = New clsTextValue
        moFanucNo = New clsIntValue
        moDesc = New clsTextValue
        moRobReq = New clsIntValue
        moRegisterNo = New clsIntValue
        '        moPlantSpec = New clsPlantSpecific
        With oNode
            moPlantNo.Text = .Item(gsSYSO_COL_PLANTNUM).InnerXml
            moFanucNo.Value = CType(.Item(gsSYSO_COL_FANUCNUM).InnerXml, Integer)
            moDesc.Text = .Item(gsSYSO_COL_DESC).InnerXml
            moRobReq.Value = CType(.Item(gsSYSO_COL_ENABLE).InnerXml, Integer)
            Try
                moRegisterNo.Value = CType(.Item(gsSYSO_REGISTER).InnerXml, Integer)
            Catch ex As Exception
                moRegisterNo.Value = 0
            End Try

        End With
        Update()
    End Sub
    Friend Sub New(ByVal vIndex As Integer)
        ItemNumber = vIndex
        moPlantNo = New clsTextValue
        moFanucNo = New clsIntValue
        moDesc = New clsTextValue
        moRobReq = New clsIntValue
        moRegisterNo = New clsIntValue

        moPlantNo.Text = vIndex.ToString
        moFanucNo.Value = 0
        moDesc.Text = "Option " & vIndex.ToString
        moRobReq.Value = 0
        moRegisterNo.Value = 0
        Update()
    End Sub
    Friend Sub Update()
        moPlantNo.Update()
        moFanucNo.Update()
        moDesc.Update()
        moRobReq.Update()
        moRegisterNo.Update()
    End Sub

#End Region

End Class 'clsSysOption

'Friend Class clsPlantSpecific
'#Region " Declarations "
'Private mnJobCodes(9) As clsIntValue

'Private Structure PathNumbers'
'Dim NumberByJobcode() As clsIntValue
'End Structure
'Private mPaths(9) As PathNumbers
'Private mnRepairPaths(3) As PathNumbers
'Private Const mnVALIDREPAIRCMBWORDS As Integer = 50

'Private Structure RepairSetup'
'Dim LightMask As Integer
'Dim ValidBits() As Integer
'End Structure
'Private mRepairSetup(3) As RepairSetup

'#End Region
'#Region " Properties "
'Friend Property RepairSetupMask(ByVal RobotIndex As Integer) As Integer
'    Get
'        Return mRepairSetup(RobotIndex).LightMask
'    End Get
'    Set(ByVal value As Integer)
'        mRepairSetup(RobotIndex).LightMask = value
'    End Set
'End Property
'Friend ReadOnly Property RepairValidPanelWordCount(ByVal RobotIndex As Integer) As Integer
'    Get
'        Return mnVALIDREPAIRCMBWORDS
'    End Get
'End Property
'Friend Property RepairValidPanelWords(ByVal Robotindex As Integer, ByVal WordIndex As Integer) As Integer
'    Get
'        Return mRepairSetup(Robotindex).ValidBits(WordIndex)
'    End Get
'    Set(ByVal value As Integer)
'        mRepairSetup(Robotindex).ValidBits(WordIndex) = value
'    End Set
'End Property
'Friend ReadOnly Property JobCode(ByVal Index As Integer) As clsIntValue
'N18:0 based on Option index (N17:0)
'    Get
'        If Index > UBound(mnJobCodes) Then Return Nothing
'        If Index < 0 Then Return Nothing
'        Return mnJobCodes(Index)
'   End Get
'
'End Property

'Friend ReadOnly Property Paths(ByVal JCIndex As Integer, ByVal PathArrayInx As Integer) As clsIntValue
'    Get
'       If JCIndex < 0 Then Return Nothing
'       If JCIndex > 9 Then Return Nothing
'       Return mPaths(JCIndex).NumberByJobcode(PathArrayInx)
'   End Get

'End Property
'Friend ReadOnly Property RepairPaths(ByVal RIndex As Integer, ByVal PathArrayInx As Integer) As clsIntValue
'    Get
'        If RIndex < 0 Then Return Nothing
'        If RIndex > 3 Then Return Nothing
'        Return mnRepairPaths(RIndex).NumberByJobcode(PathArrayInx)
'    End Get

'End Property

'#End Region
'#Region " Routines "

'#End Region
'#Region " Events "

'Friend Sub New()
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
'Dim i%, j%
'    For i% = 0 To 9
'        mnJobCodes(i%) = New clsIntValue
'    Next

' For i% = 0 To 9
' ReDim mPaths(i%).NumberByJobcode(30)
' For j% = 0 To 30
' mPaths(i%).NumberByJobcode(j%) = New clsIntValue
' Next
' Next


' For i% = 0 To 3
' ReDim mnRepairPaths(i%).NumberByJobcode(115)
' For j% = 0 To 115
' mnRepairPaths(i%).NumberByJobcode(j%) = New clsIntValue
' Next
' Next

'    For i% = 0 To 3
' ReDim mRepairSetup(i%).ValidBits(mnVALIDREPAIRCMBWORDS)
' mRepairSetup(i%).LightMask = 0
' For j% = 0 To 50
' mRepairSetup(i%).ValidBits(j%) = 0
' Next
' Next

'End Sub

'#End Region

'End Class 'clsPlantSpecific