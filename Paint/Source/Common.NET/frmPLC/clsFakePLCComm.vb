' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2010
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsFakePLCComm
'
' Description: Class to use for PAINTworks SA (No PLC)
' 
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: HGB
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'********************************************************************************************
' 03/22/2010    HGB     Initial Code
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    07/09/13   MSW     Take clsZone out of plc class so it can get built to a dll    4.01.05.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Public Class clsPLCComm
    '***** TODO List ***************************************************************************
    '***** End TODO List ***********************************************************************

#Region "  Declares  "
    Implements IDisposable

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "FakePLCComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsFakePLCComm"
    Private Const msASSEMBLY As String = "PLCComm"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msASSEMBLY & ".CommonStrings"
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemotePath As String = String.Empty
    Private msReadData() As String
    Private mPLCType As ePLCType = ePLCType.None 'Default to Logix 5000
    '***** End Property Vars *******************************************************************

    '******* Events ****************************************************************************
    'when hotlink has new data 
    Public Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                         ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Public Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events ************************************************************************

    '***** Working Vars ************************************************************************
    Private mColLinks As New Collection(Of clsLink)
    Private moLink As clsLink = Nothing
    Private mWatcher As FileSystemWatcher               'Requires Dispose
    Private Const msXMLFILE As String = "FakePLCComm"
    Private Const msXMLNODE As String = "plctag"
    Private Delegate Sub FileChangedDelegate()
    Private mbUpdateData As Boolean = False
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************

    '***** End Enums ***************************************************************************

#End Region

#Region " Properties "

    Public ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return Me.Parent.Name
        End Get

    End Property

    Public Property PLCData() As String()
        '********************************************************************************************
        'Description:  Read or write PLC Data - All data in and out is string array, one word data
        '              is done as string(0).  Link is a hotlink if poll rate is > 0 if zero it is a
        '              one time read/write. Hotlink data returned via newdata event, but this must
        '              be called to start hotlink
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            If moLink IsNot Nothing Then
                moLink.bEnabled = moLink.bHotlink
                subReadConfigFile()
                If moLink.bXref Then
                    For Each oLink As clsLink In mColLinks
                        If (oLink.sTagName = moLink.sXref) AndAlso (moLink.nXref >= oLink.sData.GetLowerBound(0)) AndAlso ((moLink.nXref + moLink.sData.GetUpperBound(0)) <= oLink.sData.GetUpperBound(0)) Then
                            For nIdx As Integer = moLink.sData.GetLowerBound(0) To moLink.sData.GetUpperBound(0)
                                moLink.sData(nIdx) = oLink.sData(moLink.nXref + nIdx)
                            Next
                        End If
                    Next
                End If
                Return (moLink.sData)
            Else
                Return Nothing
            End If
        End Get

        Set(ByVal value As String())
            If moLink IsNot Nothing Then
                moLink.sData = value
                subWriteConfigFile()
            End If
        End Set

    End Property

    Public Property PLCType() As ePLCType
        '********************************************************************************************
        'Description:  Here for compatability only
        '
        'Parameters: none
        'Returns:    PLC Type
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mPLCType
        End Get

        Set(ByVal value As ePLCType)
            mPLCType = value
        End Set

    End Property

    ReadOnly Property MaxDataPerReadWrite() As Integer
        '********************************************************************************************
        'Description:  Included for compatability, not implemented
        '
        'Parameters: none
        'Returns:    integer
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return 255
        End Get

    End Property

    Public Property TagName() As String
        '********************************************************************************************
        'Description:  TagName - identifier into tag file
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msTagName
        End Get

        Set(ByVal value As String)
            msTagName = value
            Dim bFound As Boolean = False
            For Each oLink As clsLink In mColLinks
                If oLink.sTagName = msTagName Then
                    moLink = oLink
                    bFound = True
                End If
            Next
            If bFound = False Then
                'Not in thee config file, add it
                Dim oNewLink As New clsLink
                oNewLink.sTagName = msTagName
                Dim sTmp(0) As String
                sTmp(0) = "0"
                oNewLink.sData = sTmp
                oNewLink.sZoneame = msZoneName
                oNewLink.bHotlink = False
                oNewLink.bEnabled = False
                oNewLink.bChanged = False
                moLink = oNewLink
                mColLinks.Add(oNewLink)
                subWriteConfigFile()
            End If
        End Set

    End Property


    Public ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  return where we are looking for taginfo
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msXMLFilePath
        End Get

    End Property


    Public WriteOnly Property RemotePath() As String
        '********************************************************************************************
        'Description:  Handle zone stuff without zone class - isolating for dll.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/16/13  MSW     Take clsZone out of plc class so it can get built to a dll
        '********************************************************************************************
        Set(ByVal value As String)
            msRemotePath = value
        End Set
    End Property

    Public Property ZoneName() As String
    	'********************************************************************************************
        'Description:  This is to take the place of Zonename - set this with current zone this 
        '              carries dbpath etc with it. should already be checked for available etc.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/16/13  MSW     Take clsZone out of plc class so it can get built to a dll
        '********************************************************************************************
        Set(ByVal value As String)
            msZoneName = value
        End Set

        Get
            Return msZoneName
        End Get

    End Property
    Public WriteOnly Property Zone() As clsZone
        '********************************************************************************************
        'Description:  This is to take the place of Zonename - set this with current zone this 
        '              carries dbpath etc with it. should already be checked for available etc.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As clsZone)
            msZoneName = value.Name
        End Set

    End Property

#End Region

#Region " Routines "
    Shared Function AreaNum(ByVal Area As String) As Integer
        '********************************************************************************************
        'Description:  Not used in this one
        '
        'Parameters: Area
        'Returns:    Area Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        AreaNum = 0
    End Function
    Public Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String
        '****************************************************************************************
        'Description: This function takes a integer from a hotlink etc. and formats it 
        '               for the time base that this particular PLC uses
        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'assuming all 0.001 timebase
        Dim fTmp As Single = CSng(ValueIn / 1000)
        Return Format(fTmp, sFormat)

    End Function
    Public Function FormatToTimerValue(ByVal ValueIn As Integer) As String
        '****************************************************************************************
        'Description: This function takes a integer from a textbox etc and formats it 
        '               for the value we need to send to plc
        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'assuming all 0.001 timebase
        Dim nTmp As Integer = ValueIn * 1000
        Return nTmp.ToString

    End Function

    Public Sub RemoveAllHotLinks()
        '********************************************************************************************
        'Description:  Dispose of all hotlinks.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each oLink As clsLink In mColLinks
        Next

    End Sub

    Public Sub RemoveHotLink(ByVal Tag As String, ByVal Zone As String)
        '********************************************************************************************
        'Description:  Dispose of a hotlink we're no longer using.
        '
        'Parameters: Tag name, Zone name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each oLink As clsLink In mColLinks
            If oLink.sTagName = Tag Then
                oLink.bEnabled = False
            End If
        Next
    End Sub

    Shared Function MemType(ByVal Type As String) As String
        '********************************************************************************************
        'Description:  Return the MemType for the supplied Data Type.
        '
        'Parameters: Type - Data Type
        'Returns:    MemType
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        MemType = String.Empty

    End Function


#End Region

    Public Function TagParams(ByVal TagName As String) As String
        '********************************************************************************************
        'Description:  for compatability, not implemented
        '
        'Parameters: none
        'Returns:    Returns "Address,Length,Station,PollRate" string for current tag
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return String.empty

    End Function

#Region " Functions "


#End Region

#Region " Events "


    Public Sub New()
        '****************************************************************************************
        'Description: Initialize
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, String.Empty, _
                                      String.Empty)


        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Load the config
        subReadConfigFile()

        'Set up a watch
        mWatcher = New FileSystemWatcher
        Dim sXMLFilePath As String = String.Empty

        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, String.Empty) Then
            With mWatcher
                .Path = sXMLFilePath
                'Watch for changes in LastWrite times 
                .NotifyFilter = NotifyFilters.LastWrite
                .Filter = msXMLFILE & ".XML"

                'Add event handler
                AddHandler .Changed, AddressOf OnChanged

                ' Begin watching.
                .EnableRaisingEvents = True
            End With
        End If
        tmrHotLink.Interval = 250
        tmrHotLink.Enabled = True
    End Sub
    Private Sub OnChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        '********************************************************************************************
        'Description: A text file of interest was written to. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ChangeType
            Case WatcherChangeTypes.Changed
                If My.Computer.FileSystem.FileExists(e.FullPath) Then

                    subFileChanged()
                End If
        End Select

    End Sub
    Private Sub subFileChanged()
        '********************************************************************************************
        'Description: A text file of interest was written to. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbUpdateData = True

    End Sub
    Private Sub subWriteConfigFile()
        '********************************************************************************************
        'Description:  Write settings to XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************



        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty

        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, msXMLFILE & ".XML") Then

                Dim bOK As Boolean = False
                For nAttempt As Integer = 0 To 10
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        bOK = True
                        Exit For
                    Catch ex As Exception
                        Threading.Thread.Sleep(50) 'slow down the read so whoever's writing the file has a chance to let go
                    End Try
                Next

                oMainNode = oXMLDoc.SelectSingleNode("//" & msXMLFILE)
                oNodeList = oMainNode.SelectNodes("//" & msXMLNODE)

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("clsFakePLCComm:subWriteConfigFile", sXMLFilePath & " not found.")
                    Else
                        'For Each oLink As clsLink In mColLinks
                        Dim bAdd As Boolean = True
                        For Each oNode As XmlNode In oNodeList
                            If oNode.Item("id").InnerXml = moLink.sTagName Then
                                Dim sData As String = String.Empty
                                For Each oItem As String In moLink.sData
                                    sData = sData & oItem & " "
                                Next
                                oNode.Item("value").InnerXml = sData.Trim
                                bAdd = False
                            End If
                            If moLink.bXref AndAlso oNode.Item("id").InnerXml = moLink.sXref Then
                                Dim sData As String = oNode.Item("value").InnerXml
                                Dim sTmpSplit As String() = Split(sData, " ")
                                If moLink.nXref >= sTmpSplit.GetLowerBound(0) AndAlso (moLink.nXref + moLink.sData.GetUpperBound(0)) <= sTmpSplit.GetUpperBound(0) Then
                                    For nIdx As Integer = moLink.sData.GetLowerBound(0) To moLink.sData.GetUpperBound(0)
                                        sTmpSplit(moLink.nXref + nIdx) = moLink.sData(nIdx)
                                    Next

                                    sData = String.Empty

                                    For Each oItem As String In sTmpSplit
                                        sData = sData & oItem & " "
                                    Next
                                    oNode.Item("value").InnerXml = sData.Trim
                                End If
                                bAdd = False
                            End If
                        Next
                        'Next
                        If bAdd Then
                            Dim oNode As XmlNode = oXMLDoc.CreateElement(msXMLNODE)
                            Dim oIDNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "id", Nothing)
                            oIDNode.InnerXml = moLink.sTagName
                            oNode.AppendChild(oIDNode)
                            Dim oCmtNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "comment", Nothing)
                            oCmtNode.InnerXml = moLink.sComment
                            oNode.AppendChild(oCmtNode)
                            Dim oValNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "value", Nothing)
                            oValNode.InnerXml = "0"
                            oNode.AppendChild(oValNode)
                            Dim oHLNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "hotlink", Nothing)
                            oHLNode.InnerXml = False.ToString
                            oNode.AppendChild(oHLNode)

                            Dim oXrTNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "xreftag", Nothing)
                            Dim oXrINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "xrefindex", Nothing)
                            If moLink.bXref Then
                                oXrTNode.InnerXml = String.Empty
                                oXrINode.InnerXml = "0"
                            Else
                                oXrTNode.InnerXml = moLink.sXref
                                oXrINode.InnerXml = moLink.nXref.ToString
                            End If
                            oNode.AppendChild(oXrTNode)
                            oNode.AppendChild(oXrINode)

                            oMainNode.AppendChild(oNode)
                        End If
                    End If
                    oXMLDoc.Save(sXMLFilePath)
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsFakePLCComm:subWriteConfigFile", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("clsFakePLCComm:subWriteConfigFile", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try
    End Sub
    Private Sub subReadConfigFile()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty

        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, msXMLFILE & ".XML") Then
                Dim bOK As Boolean = False
                For nAttempt As Integer = 0 To 10
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        bOK = True
                        Exit For
                    Catch ex As Exception
                        Threading.Thread.Sleep(50) 'slow down the read so whoever's writing the file has a chance to let go
                    End Try
                Next

                oMainNode = oXMLDoc.SelectSingleNode("//" & msXMLFILE)
                oNodeList = oMainNode.SelectNodes("//" & msXMLNODE)

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("clsFakePLCComm:subReadConfigFile", sXMLFilePath & " not found.")
                    Else
                        For Each oNode As XmlNode In oNodeList
                            Try
                                Dim sID As String = oNode.Item("id").InnerXml
                                Dim bAdd As Boolean = True
                                For Each oLink As clsLink In mColLinks
                                    If oLink.sTagName = sID Then
                                        bAdd = False
                                        Dim sTmp() As String = Split(oNode.Item("value").InnerXml)
                                        If oLink.bHotlink And oLink.bEnabled Then
                                            Dim bUpdate As Boolean = False
                                            If sTmp.GetUpperBound(0) <> oLink.sData.GetUpperBound(0) Then
                                                bUpdate = True
                                            Else
                                                For nItem As Integer = 0 To sTmp.GetUpperBound(0)
                                                    If sTmp(nItem) <> oLink.sData(nItem) Then
                                                        bUpdate = True
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                            If bUpdate Then
                                                oLink.sData = sTmp
                                                oLink.bChanged = True
                                                'RaiseEvent NewData(oLink.sZoneame, oLink.sTagName, oLink.sData)
                                            End If
                                        Else
                                            oLink.sData = sTmp
                                        End If
                                        oLink.sData = Split(oNode.Item("value").InnerXml)
                                        oLink.bHotlink = CType(oNode.Item("hotlink").InnerXml, Boolean)
                                        If oNode.InnerXml.Contains("<comment>") Then
                                            Try
                                                oLink.sComment = oNode.Item("comment").InnerXml
                                            Catch ex As Exception
                                                'don't complain, just deal with it
                                                oLink.sComment = ""
                                            End Try
                                        Else
                                            oLink.sComment = ""
                                        End If
                                        If oNode.InnerXml.Contains("<xreftag>") AndAlso oNode.InnerXml.Contains("<xrefindex>") Then
                                            Try
                                                oLink.sXref = oNode.Item("xreftag").InnerXml
                                                oLink.nXref = CType(oNode.Item("xrefindex").InnerXml, Integer)
                                                oLink.bXref = (oLink.nXref >= 0) AndAlso (oLink.sXref <> String.Empty)

                                            Catch ex As Exception
                                                'don't complain, just deal with it
                                                oLink.bXref = False
                                            End Try
                                        Else
                                            oLink.bXref = False
                                        End If

                                    End If
                                Next
                                If bAdd Then
                                    Dim oNewLink As New clsLink
                                    oNewLink.sZoneame = msZoneName
                                    oNewLink.sTagName = sID
                                    oNewLink.sData = Split(oNode.Item("value").InnerXml)
                                    oNewLink.bHotlink = CType(oNode.Item("hotlink").InnerXml, Boolean)
                                    oNewLink.bEnabled = False
                                    oNewLink.bChanged = False
                                    mColLinks.Add(oNewLink)
                                End If
                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "clsFakePLCComm:subReadConfigFile", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsFakePLCComm:subReadConfigFile", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("clsFakePLCComm:subReadConfigFile", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try
    End Sub


    Private Sub tmrHotLink_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrHotLink.Tick
        '********************************************************************************************
        'Description:  send update events - from this thread
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrHotLink.Enabled = False
            If mbUpdateData Then
                subReadConfigFile()
                For Each oLink As clsLink In mColLinks
                    If oLink.bChanged Then
                        oLink.bChanged = False
                        RaiseEvent NewData(oLink.sZoneame, oLink.sTagName, oLink.sData)
                    End If
                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("clsFakePLCComm:tmrHotLink_Tick", ex.Message)
        End Try
        tmrHotLink.Enabled = True
    End Sub
#End Region

    Private Class clsLink

        Public sData() As String
        Public sTagName As String = String.Empty
        Public sZoneame As String = String.Empty
        Public bHotlink As Boolean = False
        Public bEnabled As Boolean = False
        Public bChanged As Boolean = False
        Public sComment As String = String.Empty
        Public bXref As Boolean = False
        Public sXref As String = String.Empty
        Public nXref As Integer = 0
    End Class  'clsLink

End Class 'clsPLCComm

