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
' Form/Module: SysStyleID.vb
'
' Description: Collection for style ID and the class for a style
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
'    03/30/12   MSW     Move from SQL to XML                                          4.01.03.00
'    07/08/13   MSW     Move Style ID to XML                                          4.01.05.00
'    01/06/14   MSW     Support sealer style ID                                       4.01.06.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath



Friend Class clsSysStylesID

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsSysStylesID"
    '***** End Module Constants   **************************************************************

    '9.5.07 remove unnecessary initializations per fxCop
    Private moZone As clsZone
    Private mnMaxStyles As Integer ' = 0
    Private mbUseAscii As Boolean ' = False
    Private mnNumberOfPhotoeyes As Integer ' = 0
    Private mnNumberOfConveyorSnapshots As Integer
    Private mnIDPositionLowerLimit As Integer
    Private mnIDPositionUpperLimit As Integer


#End Region

#Region " Properties "


    Friend Property NumberOfConveyorSnapshots() As Integer
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
            Return mnNumberOfConveyorSnapshots
        End Get
        Set(ByVal value As Integer)
            mnNumberOfConveyorSnapshots = value
        End Set
    End Property

    Friend Property IDPositionLowerLimit() As Integer
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
            Return mnIDPositionLowerLimit
        End Get
        Set(ByVal value As Integer)
            mnIDPositionLowerLimit = value
        End Set
    End Property
    Friend Property IDPositionUpperLimit() As Integer
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
            Return mnIDPositionUpperLimit
        End Get
        Set(ByVal value As Integer)
            mnIDPositionUpperLimit = value
        End Set
    End Property

    Default Friend Overloads Property Item(ByVal index As Integer) As clsSysStyleID
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
            Return CType(List(index), clsSysStyleID)
        End Get
        Set(ByVal Value As clsSysStyleID)
            List(index) = Value
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
    Friend Property NumberOfPhotoeyes() As Integer
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
            Return mnNumberOfPhotoeyes
        End Get
        Set(ByVal value As Integer)
            mnNumberOfPhotoeyes = value
        End Set
    End Property
#End Region

#Region " Routines"

    Friend Function Add(ByVal Value As clsSysStyleID) As Integer
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

        ' With Value
        '.ConveyorCount.Value = NumberOfConveyorSnapshots
        '.PhotoeyePattern.Value = NumberOfPhotoeyes
        'End With

        Return List.Add(Value)

    End Function 'Add
    Private Function bLoadCollection(ByRef oZone As clsZone, Optional ByVal nStyle As Integer = -1) As Boolean
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
        '07/08/13   MSW     move Style ID to XML
        '*****************************************************************************************
        'Dim DB As clsSQLAccess = New clsSQLAccess
        'Dim sTableName As String = gsSYSID_DS_TABLENAME
        'Dim DS As DataSet = New DataSet
        'Dim DT As DataTable = New DataTable
        'Dim DR As DataRow = Nothing

        'Try
        '    DS.Locale = mLanguage.FixedCulture
        '    DT.Locale = mLanguage.FixedCulture

        '    With DB

        '        .DBFileName = gsPROCESS_DBNAME
        '        .Zone = oZone
        '        DS = mSysStyleID.GetStyleIDDataset(DB, moZone.Name)

        '        If (DS Is Nothing) = False Then
        '            If DS.Tables.Contains("[" & sTableName & "]") Then
        '                Dim nIndex As Integer = 0

        '                DT = DS.Tables("[" & sTableName & "]")
        '                For Each DR In DT.Rows
        '                    Dim o As New clsSysStyleID(nIndex, DR, moZone)
        '                    Add(o)
        '                    nIndex += 1
        '                    If nIndex = (MaxStyles + 1) * mnNumberOfConveyorSnapshots Then Exit For
        '                Next
        '            Else
        '                .Close()
        '                Return False
        '            End If ' DS.Tables.Contains("[" & sTableName & "]")

        '            .Close()
        '        Else
        '            Return False
        '        End If
        '    End With

        '    Return True
        'Catch ex As Exception
        '    Return False
        'End Try
        'Friend Const gsSYSID_DS_TABLENAME As String = "StyleID"
        'Friend Const gsSYSID_DS_ITEMNAME As String = "Item"
        'Friend Const gsSYSID_COL_STYLE As String = "Style"
        'Friend Const gsSYSID_COL_CONVEYOR_COUNT As String = "ConveyorCount"
        'Friend Const gsSYSID_COL_PHOTOEYE_PATTERN As String = "PhotoeyePattern"
        'Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN As String = "SunroofPhotoeyePattern"
        'Friend Const gsSYSID_COL_PHOTOEYE_IGNORE As String = "PhotoeyeIgnore"
        'Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE As String = "SunroofPhotoeyeIgnore"

        Const sXMLFILE As String = gsSYSID_DS_TABLENAME
        Const sXMLTABLE As String = gsSYSID_DS_TABLENAME
        Const sXMLNODE As String = gsSYSID_DS_ITEMNAME

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
                            If nStyle >= -1 Then
                                If oNode.Item(gsSYSID_COL_STYLEINDEX).InnerText = nStyle.ToString Then
                                    Dim o As New clsSysStyleID(nIndex, oNode, moZone)
                                    Add(o)
                                    nIndex += 1
                                    If nIndex > (mnNumberOfConveyorSnapshots) Then Exit For
                                End If
                            Else
                                Dim o As New clsSysStyleID(nIndex, oNode, moZone) 'RJO 12/13/11
                                Add(o)
                                nIndex += 1
                                If nIndex = (MaxStyles * mnNumberOfConveyorSnapshots) Then Exit For
                            End If
                            Debug.Print(oNode.InnerXml)
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                End Try
                While MyBase.Count <= (MaxStyles * mnNumberOfConveyorSnapshots)
                    Dim o As New clsSysStyleID(MyBase.Count, moZone)
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
    Friend Function IndexOf(ByVal value As clsSysStyleID) As Integer
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
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsSysStyleID)
        '********************************************************************************************
        'Description: Add a system style at specific location
        '
        'Parameters: position,clsSysStyleID
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsSysStyleID)
        '********************************************************************************************
        'Description: Remove a system style 
        '
        'Parameters: clsSysStyleID
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Friend Function Contains(ByVal value As clsSysStyleID) As Boolean
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
        '********************************************************************************************
        Const sXMLFILE As String = "StyleIDSetup"
        Const sXMLTABLE As String = "StyleIDSetup"

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
                    Try
                        NumberOfConveyorSnapshots = CType(oMainNode.Item("NumberStyleIDConveyorSnapshots").InnerXml, Integer)
                        NumberOfPhotoeyes = CType(oMainNode.Item("NumberStyleIDPhotoeyes").InnerXml, Integer)
                        IDPositionLowerLimit = CType(oMainNode.Item("IDPositionLowerLimit").InnerXml, Integer)
                        IDPositionUpperLimit = CType(oMainNode.Item("IDPositionUpperLimit").InnerXml, Integer)
                    Catch ex As Exception
                        NumberOfConveyorSnapshots = 1
                        NumberOfPhotoeyes = 1
                        IDPositionLowerLimit = 0
                        IDPositionUpperLimit = 1000
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

    Friend Sub LoadStyleBoxFromCollection(ByRef rCombo As ComboBox, _
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
        Dim sStyle As String()
        ReDim sStyle(0)

        For Each o As clsSysStyle In List
            'attach so we can use instead of listindex
            ReDim Preserve sStyle(rCombo.Items.Count)
            sStyle(rCombo.Items.Count) = o.PlantString

            rCombo.Items.Add(o.DisplayName)
        Next

        rCombo.Tag = sStyle

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
        For Each o As clsSysStyleID In List
            o.Update()
        Next
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByRef oZone As clsZone, Optional ByVal nStyle As Integer = -1)
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
            Call bLoadCollection(oZone, nStyle)
        End If

    End Sub

#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class 'clsSysStylesID

Friend Class clsSysStyleID

#Region " Declarations "

    '9.5.07 remove initialization per fxCop
    Private mnItemNo As Integer ' = 0
    Private moConveyorCount As clsIntValue = Nothing
    Private moPhotoeyePattern As clsIntValue = Nothing
    Private moSunroofPhotoeyePattern As clsIntValue = Nothing
    Private moPhotoeyeIgnore As clsIntValue = Nothing
    Private moSunroofPhotoeyeIgnore As clsIntValue = Nothing
    Private moZone As clsZone = Nothing

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
            If moConveyorCount.Changed Then Return True
            If moPhotoeyePattern.Changed Then Return True
            If moSunroofPhotoeyePattern.Changed Then Return True
            If moPhotoeyeIgnore.Changed Then Return True
            If moSunroofPhotoeyeIgnore.Changed Then Return True
            Return False
        End Get
    End Property


    Friend ReadOnly Property ConveyorCount() As clsIntValue
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
            Return moConveyorCount
        End Get
    End Property
    Friend ReadOnly Property PhotoeyePattern() As clsIntValue
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
            Return moPhotoeyePattern
        End Get
    End Property
    Friend ReadOnly Property SunroofPhotoeyePattern() As clsIntValue
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
            Return moSunroofPhotoeyePattern
        End Get
    End Property
    Friend ReadOnly Property PhotoeyeIgnore() As clsIntValue
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
            Return moPhotoeyeIgnore
        End Get
    End Property
    Friend ReadOnly Property SunroofPhotoeyeIgnore() As clsIntValue
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
            Return moSunroofPhotoeyeIgnore
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

#End Region

#Region " Routines "

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
        moConveyorCount.Update()
        moPhotoeyePattern.Update()
        moSunroofPhotoeyePattern.Update()
        moPhotoeyeIgnore.Update()
        moSunroofPhotoeyeIgnore.Update()
    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByRef vIndex As Integer, ByRef vDataRow As DataRow, ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Hello world
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ItemNumber = vIndex
        moZone = oZone
        moConveyorCount = New clsIntValue
        moPhotoeyePattern = New clsIntValue
        moSunroofPhotoeyePattern = New clsIntValue
        moPhotoeyeIgnore = New clsIntValue
        moSunroofPhotoeyeIgnore = New clsIntValue

        With vDataRow
            moConveyorCount.Value = CType(.Item(gsSYSID_COL_CONVEYOR_COUNT), Integer)
            moPhotoeyePattern.Value = CType(.Item(gsSYSID_COL_PHOTOEYE_PATTERN), Integer)
            moSunroofPhotoeyePattern.Value = CType(.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN), Integer)
            moPhotoeyeIgnore.Value = CType(.Item(gsSYSID_COL_PHOTOEYE_IGNORE), Integer)
            moSunroofPhotoeyeIgnore.Value = CType(.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE), Integer)
        End With
        Update()
    End Sub

    Friend Sub New(ByRef vIndex As Integer, ByRef oNode As XmlNode, ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Hello world
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  MSW     move Style ID to XML
        '********************************************************************************************
        ItemNumber = vIndex
        moZone = oZone
        moConveyorCount = New clsIntValue
        moPhotoeyePattern = New clsIntValue
        moSunroofPhotoeyePattern = New clsIntValue
        moPhotoeyeIgnore = New clsIntValue
        moSunroofPhotoeyeIgnore = New clsIntValue

        With oNode
            moConveyorCount.Value = CType(.Item(gsSYSID_COL_CONVEYOR_COUNT).InnerXml, Integer)
            moPhotoeyePattern.Value = CType(.Item(gsSYSID_COL_PHOTOEYE_PATTERN).InnerXml, Integer)
            moSunroofPhotoeyePattern.Value = CType(.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN).InnerXml, Integer)
            moPhotoeyeIgnore.Value = CType(.Item(gsSYSID_COL_PHOTOEYE_IGNORE).InnerXml, Integer)
            moSunroofPhotoeyeIgnore.Value = CType(.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE).InnerXml, Integer)
        End With
        Update()
    End Sub
    Friend Sub New(ByRef vIndex As Integer, ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Hello world
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ItemNumber = vIndex
        moZone = oZone
        moConveyorCount = New clsIntValue
        moPhotoeyePattern = New clsIntValue
        moSunroofPhotoeyePattern = New clsIntValue
        moPhotoeyeIgnore = New clsIntValue
        moSunroofPhotoeyeIgnore = New clsIntValue
        moConveyorCount.Value = 0
        moPhotoeyePattern.Value = 0
        moSunroofPhotoeyePattern.Value = 0
        moPhotoeyeIgnore.Value = 0
        moSunroofPhotoeyeIgnore.Value = 0

        Update()
    End Sub
#End Region

End Class 'clsSysStyleID

Friend Module mSysStyleID

#Region " Declarations"

#End Region

#Region " Routines "

    'Friend Function GetStyleIDDataset(ByRef oDB As clsSQLAccess, ByVal sZonename As String) As DataSet
    '    '********************************************************************************************
    '    'Description: This Routine gets a dataset with the color valve table
    '    '
    '    'Parameters: a clsSQLAccess pointing to the proper place, name of zone of interest 
    '    'Returns:    Dataset or nothing if problem
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    Dim sTable As String = gsSYSID_DS_TABLENAME

    '    Try
    '        With oDB
    '            .DBTableName = sTable
    '            .SQLString = "SELECT * FROM [" & sTable & "] AS s" & _
    '                        " WHERE [s].[" & gsCOL_ZONENAME & "] ='" & _
    '                        sZonename & "'" & _
    '                        "ORDER BY [s].[" & gsCOL_SORTORDER & "] ASC"

    '            Dim DS As New DataSet
    '            DS.Locale = mLanguage.FixedCulture

    '            DS = .GetDataSet(False)
    '            If DS.Tables.Contains("[" & sTable & "]") Then
    '                Return DS
    '            Else
    '                Return Nothing
    '            End If

    '        End With


    '    Catch ex As Exception
    '        Return Nothing
    '    End Try

    'End Function

#End Region

End Module
Friend Class clsPlantSpecificStyleID

#Region " Declarations "



#End Region

#Region " Properties "




#End Region

#Region " Routines "

 

#End Region

#Region " Events "



#End Region

    Public Sub New()

    End Sub
End Class 'clsPlantSpecificStyleID