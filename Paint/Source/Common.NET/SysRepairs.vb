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
'    06 30 08   gks     Redo
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/11/12   MSW     Move system repair setup table to XML                         4.01.03.00
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

Friend Class clsSysRepairPanels

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsSysRepairPanels"
    '***** End Module Constants   **************************************************************

    Private moZone As clsZone
    Private mnPLCStartAddress As Integer
    Private msDegradeOption As String = String.Empty
#End Region

#Region " Properties "

    Friend Property DegradeOption() As String
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
            Return msDegradeOption
        End Get
        Set(ByVal value As String)
            msDegradeOption = value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal index As Integer) As clsSysRepairPanel
        '********************************************************************************************
        'Description: Get or set a system style by its index
        '
        'Parameters: index
        'Returns:    clsSysRepairPanel
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsSysRepairPanel)
        End Get
        Set(ByVal Value As clsSysRepairPanel)
            List(index) = Value
        End Set
    End Property
    Friend ReadOnly Property Zone() As clsZone
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

    Friend Function Add(ByVal Value As clsSysRepairPanel) As Integer
        '********************************************************************************************
        'Description: Add a system style to collection
        '
        'Parameters: clsSysRepairPanel
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return List.Add(Value)

    End Function 'Add
    Private Function bLoadCollection(ByRef oZone As clsZone) As Boolean
        '*****************************************************************************************
        'Description: load the Repairs collection
        '
        'Parameters: pass in the style index and load up the collection
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try

            For nPanel As Integer = 1 To oZone.RepairPanels
                Dim o As New clsSysRepairPanel(nPanel)
                o.BitOffset = oZone.RobotsRequiredStartingBit
                Add(o)
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Friend Function Clone() As clsSysRepairPanels
        '********************************************************************************************
        'Description: this routine is to make a new collection without going thru the database
        '               load for each style it is attached to. the description is copied, the 
        '               robots required is set when data is actually loaded
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim void As clsZone = Nothing
        Dim oClone As New clsSysRepairPanels(void)

        Dim dr As DataRow = Nothing

        For Each o As clsSysRepairPanel In List
            Dim oNew As New clsSysRepairPanel(o.PanelNumber)
            With oNew
                .PanelNumber = o.PanelNumber
                ' not copying .RobotRequired is set on load of data from plc
            End With
            oClone.Add(oNew)
        Next

        Return oClone

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
    Friend Function IndexOf(ByVal value As clsSysRepairPanel) As Integer
        '********************************************************************************************
        'Description: Get Index of system style in collection
        '
        'Parameters: clsSysRepairPanel
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysRepairPanel)
        '********************************************************************************************
        'Description: Add a system style at specific location
        '
        'Parameters: position,clsSysRepairPanel
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysRepairPanel)
        '********************************************************************************************
        'Description: Remove a system style 
        '
        'Parameters: clsSysRepairPanel
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Function Contains(ByVal value As clsSysRepairPanel) As Boolean
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
    Friend Sub Update()
        For Each r As clsSysRepairPanel In List
            r.Update()
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

        'Initialize
        MyBase.new()
        List.Clear()

        If oZone Is Nothing = False Then
            Call bLoadCollection(oZone)
        End If

    End Sub
    Friend Sub New(ByRef oZone As clsZone, ByRef sDegradeOption As String)
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

        'Initialize
        MyBase.new()
        List.Clear()

        If oZone Is Nothing = False Then
            Call bLoadCollection(oZone)
        End If
        DegradeOption = sDegradeOption
    End Sub

#End Region

End Class 'clsSysRepairs

Friend Class clsSysRepairPanel

#Region " Declarations "

    Private mnPanelNumber As Integer
    Private Const nMAX_ROB As Integer = 15  ' should be less than 16 robots in zone
    Private moRobReq(nMAX_ROB) As clsBoolValue
    Dim nBitOffset As Integer = 1
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

            
            For i As Integer = 0 To nMAX_ROB

                If moRobReq(i).Changed Then Return True

            Next

            Return False
        End Get
    End Property
    Protected Friend Property BitOffset() As Integer
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
            Return nBitOffset
        End Get
        Set(ByVal value As Integer)
            nBitOffset = value
        End Set
    End Property
    Protected Friend Property PanelNumber() As Integer
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
            Return mnPanelNumber
        End Get
        Set(ByVal value As Integer)
            mnPanelNumber = value
        End Set
    End Property
    Friend ReadOnly Property RobotRequired(ByVal RobotIndex As Integer) As clsBoolValue
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
            Return moRobReq(RobotIndex)
        End Get
    End Property

    Friend Property RobotsRequired() As Integer
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

            Dim nTmp As Integer = 0
            For nBit As Integer = 0 To nMAX_ROB
                If moRobReq(nBit).Value Then
                    nTmp = nTmp + CInt(2 ^ (nBit + nBitOffset))
                End If
            Next
            Return nTmp
        End Get
        Set(ByVal value As Integer)
            For nBit As Integer = 0 To nMAX_ROB
                Dim nTmp As Integer = CInt(2 ^ (nBit + nBitOffset))
                If (nTmp And value) <> 0 Then
                    moRobReq(nBit).Value = True
                Else
                    moRobReq(nBit).Value = False
                End If
            Next
        End Set
    End Property
#End Region

#Region " Routines "

    Friend Sub Update()
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
        
        For i As Integer = 0 To nMAX_ROB
            moRobReq(i).Update()
        Next

    End Sub

#End Region

#Region " Events "

   
    Friend Sub New(ByVal nPanel As Integer)
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


   
        For i As Integer = 0 To nMAX_ROB
            moRobReq(i) = New clsBoolValue
        Next

        mnPanelNumber = nPanel
        
        Update()

    End Sub
#End Region

End Class 'clsSysRepairPanel

Friend Class clsDegradeRepairPanels

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsDegradeRepairPanels"
    '***** End Module Constants   **************************************************************

    Private moZone As clsZone
    Private mnPLCStartAddress As Integer

#End Region

#Region " Properties "

    Default Friend Overloads Property Item(ByVal index As Integer) As clsSysRepairPanels
        '********************************************************************************************
        'Description: Get or set a system style by its index
        '
        'Parameters: index
        'Returns:    clsSysRepairPanel
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsSysRepairPanels)
        End Get
        Set(ByVal Value As clsSysRepairPanels)
            List(index) = Value
        End Set
    End Property
    Friend ReadOnly Property Zone() As clsZone
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

    Friend Function Add(ByVal Value As clsSysRepairPanels) As Integer
        '********************************************************************************************
        'Description: Add a system style to collection
        '
        'Parameters: clsSysRepairPanel
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return List.Add(Value)

    End Function 'Add
    Private Function bLoadCollection(ByRef oZone As clsZone) As Boolean
        '*****************************************************************************************
        'Description: load the Repairs collection
        '
        'Parameters: pass in the style index and load up the collection
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try

            Dim o As New clsSysRepairPanels(oZone, gcsRM.GetString("csNONE"))
            Add(o)

            For nPanel As Integer = 0 To oZone.RepairPanels - 1
                Dim o1 As New clsSysRepairPanels(oZone)
                Add(o1)
            Next


            Return True
        Catch ex As Exception
            Return False
        End Try

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
    Friend Function IndexOf(ByVal value As clsSysRepairPanels) As Integer
        '********************************************************************************************
        'Description: Get Index of system style in collection
        '
        'Parameters: clsSysRepairPanel
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysRepairPanels)
        '********************************************************************************************
        'Description: Add a system style at specific location
        '
        'Parameters: position,clsSysRepairPanel
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysRepairPanels)
        '********************************************************************************************
        'Description: Remove a system style 
        '
        'Parameters: clsSysRepairPanel
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Function Contains(ByVal value As clsSysRepairPanels) As Boolean
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
    Friend Sub Update()
        For Each r As clsSysRepairPanels In List
            r.Update()
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

        'Initialize
        MyBase.new()
        List.Clear()

        If oZone Is Nothing = False Then
            Call bLoadCollection(oZone)
        End If

    End Sub

#End Region

End Class 'clsDegradeRepairPanels


Friend Module mSysRepair

#Region " Routines "

    Friend Function GetRepairDataset(ByRef sZonename As String) As XmlNode
        '********************************************************************************************
        'Description: This Routine gets a dataset with the Repair table
        '
        'Parameters:  name of zone of interest 
        'Returns:    Dataset or nothing if problem
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/05/12  MSW     Move from SQL to XML
        '********************************************************************************************

        Const sXMLFILE As String = "SystemRepairPanels"
        Const sXMLTABLE As String = "SystemRepairPanels"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oZoneNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oZoneNode = oMainNode.SelectSingleNode("//" & sZonename)
            Else
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement(sXMLTABLE)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oXMLDoc.CreateElement(sZonename)
                oZoneNode = oXMLDoc.SelectSingleNode("//" & sZonename)
            End If
            Return oZoneNode

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

#End Region

End Module 'msysrepair