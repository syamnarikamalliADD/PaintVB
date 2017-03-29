' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsXMLPlcTag
'
' Description: Class file for reading PLC Tag Info from XML file
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: George Sinnott
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    5/30/07     GEO     original code .net 
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************

Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Public Class clsXMLPlcTag
#Region " Declares "

    Private msXmlFilePath As String = String.Empty
    'Private Const msXmlFileName As String = "ASABTCPComm.xml"  '"GEFanucComm.cfg" ' "GEFPlcTags.xml"
    Private msXmlFileName As String = String.Empty
    Private xmldoc As New XmlDocument() ' xml file to load into memory.
    ' PLC TAG INFO
    Private msAddr As String = String.Empty
    ' Private msWord As String = String.Empty
    Private msBit As String = String.Empty
    Private msLength As String = String.Empty
    Private msType As String = String.Empty
    Private msTopic As String = String.Empty
    Private msIPAddr As String = String.Empty
    Private msPollRate As String = String.Empty
    Private msSlot As String = String.Empty

    Public gsTagToFind As String = String.Empty

#End Region
#Region " Properties "

    Friend Property XMLFilePath() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            XMLFilePath = msXmlFilePath
        End Get
        Set(ByVal FullPath As String)
            msXmlFilePath = FullPath
        End Set
    End Property
    Friend Property XMLFileName() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            XMLFileName = msXmlFileName
        End Get
        Set(ByVal FileName As String)
            msXmlFileName = FileName
        End Set
    End Property


    Public Property xAddr() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        'which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion

        Get
            xAddr = msAddr
        End Get
        Set(ByVal lsTagName As String)
            msAddr = lsTagName
        End Set

        'geo3
    End Property


    'Friend Property xWord() As String
    '****************************************************************************************
    'Description: This Property in with plccomm to read tags from XML file.  The property's
    '  which begin with 'x' ie xLength is refering to an xml file 
    'Parameters: Name
    'Returns:   Name
    '
    'Modification history:
    '
    ' Date      By      Reason
    '*****************************************************************************************
    '5/30/07    Geo      .net xml conversion

    '    Get
    '        xWord = msWord
    '    End Get
    '    Set(ByVal lsWord As String)
    '        msWord = lsWord
    '    End Set


    'geo3
    'End Property

    Friend Property xBit() As String
        '****************************************************************************************        'Description: This Property in with plccomm to read tags from XML file.  The property's
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion

        Get
            xBit = msBit
        End Get
        Set(ByVal lsBit As String)
            msBit = lsBit
        End Set
    End Property

    Friend Property xlength() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xlength = msLength
        End Get
        Set(ByVal lsLength As String)
            msLength = lsLength
        End Set

    End Property
    Friend Property xType() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xType = msType
        End Get
        Set(ByVal lsType As String)
            msType = lsType
        End Set
    End Property

    Friend Property xTopic() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xTopic = msTopic
        End Get
        Set(ByVal lsTopic As String)
            msTopic = lsTopic
        End Set

    End Property
    Friend Property xIPAddr() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xIPAddr = msIPAddr
        End Get
        Set(ByVal lsIPAddr As String)
            msIPAddr = lsIPAddr
        End Set

    End Property
    Friend Property xPollRate() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xPollRate = msPollRate
        End Get
        Set(ByVal lsPollRate As String)
            msPollRate = lsPollRate
        End Set

    End Property
    Friend Property xSlot() As String
        '****************************************************************************************
        'Description: This Property in with plccomm to read tags from XML file.  The property's
        '  which begin with 'x' ie xLength is refering to an xml file 
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '5/30/07    Geo      .net xml conversion
        Get
            xSlot = msSlot
        End Get
        Set(ByVal lsSlot As String)
            msSlot = lsSlot
        End Set

    End Property

#End Region

#Region " Routines "
    Friend Sub ReadPlcTagInfo(ByVal sTagName As String)
        Dim xpath As String = "//plctag[id='" & sTagName & "']"
        Dim xnl As XmlNodeList
        Try
            xnl = xmldoc.SelectNodes(xpath)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Invalid XPath syntax", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
        Dim xn As XmlNode
        Dim msg As String = ""
        For Each xn In xnl
            msg &= xn.Name
            If Not (xn.Value Is Nothing) Then msg &= "(" & xn.Value.ToString & ")"
            msg &= ControlChars.CrLf
            xAddr = xn.Item("addr").InnerXml              '<tag>d_ValidPlantStyleSetup</tag> 
            'xWord = xn.Item("word").InnerXml              '<word>0</word>
            xBit = xn.Item("bit").InnerXml                '<bit>0</bit> 
            xlength = xn.Item("length").InnerXml          '<length>100</length> 
            xType = xn.Item("type").InnerXml              '<type>DINT</type> 
            xTopic = xn.Item("topic").InnerXml            '<topic>PLC1Manual</topic> 
        Next

        xpath = "//topic[id='" & msTopic & "']"
        xnl = Nothing
        Try
            xnl = xmldoc.SelectNodes(xpath)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Invalid XPath syntax", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
        For Each xn In xnl
            xIPAddr = xn.Item("ip").InnerXml        '<ip>172.22.194.90</ip>
            xPollRate = xn.Item("rate").InnerXml    '<rate>300</rate>
            xSlot = xn.Item("slot").InnerXml        '<slot>0</slot>
        Next
    End Sub

#End Region
#Region " Events "
    Public Sub New()
        MyBase.New()
        'set default Path
        XMLFileName = "ASABTCPComm.xml"
        mPWCommon.GetDefaultFilePath(msXmlFilePath, _
                                    mPWCommon.eDir.XML, String.Empty, String.Empty)
        LoadXMLDoc()
    End Sub
    Friend Sub New(ByVal XMLPath As String)
        '****************************************************************************************
        'Description: If we have a path on initialization
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        msXmlFilePath = XMLPath
    End Sub

    Friend Function LoadXMLDoc() As XmlDocument

        xmldoc.Load(msXmlFilePath & msXmlFileName)
        Return xmldoc

    End Function

    Protected Overrides Sub Finalize()
        '****************************************************************************************
        'Description: Make sure we clean up after ourselves 
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'Me.Close() '<<<gks lots of exceptions, dunno if this will help
        MyBase.Finalize()
    End Sub
#End Region
End Class
