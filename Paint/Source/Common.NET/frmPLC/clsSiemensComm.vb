' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2011
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsSiemensComm
'
' Description: Class to talk to a Siemens PLC
' 
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
' PLC Object: Simatic Net V8.0 - Add project COM reference "Siemens OPC DAAutomation 2.0"
'
' Author: Dereck Wonnacott
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                         Version                                                    
'********************************************************************************************
' 07/6/201      DAW     Initial Code
' 06/12/12      MSW     Mark  a version                                      4.01.01.00
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict Off ' Required for late binding..... I think? (DAW)

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Imports OPCSiemensDAAutomation

Friend Class clsPLCComm
#Region " Declares  "

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "SiemensComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsSiemensComm"
    Private Const mnTIMEOUT As Integer = 1001 'Hotlink Timeout period in milliseconds
    Private Const mnMANUAL_POLL_RATE As Integer = 5000 'A poll rate of this value specifies a One-Time-Read vs. a HotLink

    Private Const msOPC_SERVER_NAME As String = "OPC.SimaticNET"
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemoteComputer As String = String.Empty
    Private msReadData() As String
    Private mbRead As Boolean
    Private mPLCType As ePLCType = ePLCType.Siemens
    '***** End Property Vars *******************************************************************

    '******* Events ****************************************************************************
    'when hotlink has new data 
    Friend Event NewData(ByVal ZoneName As String, ByVal TagName As String, ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events ************************************************************************

    '***** Working Vars ************************************************************************
    Private WithEvents mOPCServer As OPCServer = New OPCServer
    Private WithEvents mOPCGroups As OPCGroups = mOPCServer.OPCGroups

    Private mLinks As New clsLinks
    Private mnHotLinks As Integer 'How many HotLinks have been created.
    Private mbOffline As Boolean = False ' debug
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************
    Friend Enum ePLCType
        PLC5 = 1
        SLC500 = 2
        Logix5000 = 3
        Siemens = 4
    End Enum
    '***** End Enums ***************************************************************************

#End Region
#Region " Properties "
    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for ASABTCP. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return Nothing
        End Get

    End Property
    Friend Property PLCData() As String()
        '********************************************************************************************
        'Description:  Read or write PLC Data - All data in and out is string array, one word data
        '              is done as string(0).  Link is a hotlink if poll rate is > 0 if zero it is a
        '              one time read/write. Hotlink data returned via newdata event, but this must
        '              be called to start hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Dim sData() As String = Nothing
            mbRead = True
            If ReadPLCData(sData, PLCTagInfo(TagName)) Then
                mbRead = False
                Return sData
            Else
                Dim sMsg1 As String = gcsRM.GetString("csCOULD_NOT_READ_PLC")
                Dim sMsg2 As String = gcsRM.GetString("csZONE_NAME")

                sMsg2 = sMsg2 & " = [" & ZoneName & "], " & _
                                 gcsRM.GetString("csTAG_NAME") & "= [" & TagName & "]"
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCData", _
                                       "Could not read PLC Data. ZoneName = [" & ZoneName & "], Tagname = [" & TagName & "]")
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ReadFailed, sMsg1, msMODULE & ":clsPLCComm:PLCData", sMsg2)
                mbRead = False
                Return Nothing
            End If
        End Get

        Set(ByVal value As String())
            mbRead = False
            If WritePLCData(value, PLCTagInfo(TagName)) = False Then
                Dim sMsg1 As String = gcsRM.GetString("csCOULD_NOT_WRITE_PLC")
                Dim sMsg2 As String = gcsRM.GetString("csZONE_NAME")

                sMsg2 = sMsg2 & " = [" & ZoneName & "], " & _
                                 gcsRM.GetString("csTAG_NAME") & " = [" & TagName & "]"
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCData", _
                                      "Could not write PLC Data. ZoneName = [" & ZoneName & "], Tagname = [" & TagName & "]")
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.WriteFailed, sMsg1, msMODULE & ":clsPLCComm:PLCData", sMsg2)
            End If
        End Set
    End Property
    Friend Property PLCType() As ePLCType
        '********************************************************************************************
        'Description:  ASABTCP.ocx is capable of reading/writing data to 3 PLC types (PLC-5, SLC-500, 
        '              and Logix 5000). This property is set by default to Logix 5000 as the other 
        '              types are the exception.
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
    Friend Property TagName() As String
        '********************************************************************************************
        'Description:  TagName - identifier into tag file
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
        End Set

    End Property
    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  return where we are looking for taginfo
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
    Friend WriteOnly Property Zone() As clsZone
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
            Dim sRemPath As String = String.Empty

            If value.IsRemoteZone Then
                msRemoteComputer = "\\" & value.ServerName
                sRemPath = msRemoteComputer & "\" & value.ShareName
            Else
                msRemoteComputer = String.Empty
            End If

            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, sRemPath, msXMLFILENAME) Then
                msZoneName = value.Name
            Else
                msZoneName = String.Empty
                Dim sTmp As String = gcsRM.GetString("csCANT_FIND_CONFIG_FILE") & _
                                " " & gcsRM.GetString("csZONE_NAME") & ":=" & ZoneName
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:Zone", "Could not find configuration file: " & _
                                       msXMLFILENAME & ". ZoneName = " & value.Name)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigFileNotFound, sTmp, _
                                       msMODULE & ":clsPLCComm:Zone", String.Empty)
            End If
            'should add ping here??
        End Set

    End Property
    Private ReadOnly Property ZoneName() As String
        '********************************************************************************************
        'Description:  Zone name 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msZoneName
        End Get

    End Property
#End Region
#Region " Routines "
    Friend Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String
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
    Friend Function FormatToTimerValue(ByVal ValueIn As Integer) As String
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
    Friend Sub RemoveAllHotLinks()
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
        Try

            mOPCGroups.RemoveAll()
            mLinks.Clear()

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Friend Sub RemoveHotLink(ByVal Tag As String, ByVal Zone As clsZone)
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
        Dim oLink As clsLink = Nothing
        Try
            For Each oLink In mLinks
                If (oLink.TagName = Tag) And (oLink.ZoneName = Zone.Name) Then

                    '
                    ' Remove form OPC server 
                    '
                    mLinks.Remove(oLink)
                    Exit For
                End If
            Next 'oLink

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Private Sub ReadPLCdata(ByRef oData As Object, ByRef link As clsLink)

        If IsArray(oData) Then
            Dim sRef() As String
            ReDim sRef(link.Length - 1)
            For i As Integer = 0 To UBound(sRef)
                sRef(i) = CStr(oData(i))
            Next
            link.RefData = sRef
        Else
            link.RefData(0) = CStr(oData)
        End If

    End Sub
#End Region
#Region " Functions "
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
        '7/10/08    gks     Change from private to shared per fxcop
        '********************************************************************************************

        Select Case Type
            Case "INT"
                MemType = "I" 'Integer
            Case "DINT"
                MemType = "D" 'Double
            Case "REAL"
                MemType = "R" 'Real / Floating Point
            Case "BIT16"
                MemType = "I" 'Bit oprerations on 16 bit word(s)
            Case "BIT32"
                MemType = "D" 'Bit oprerations on 32 bit word(s)
            Case "BOOL"
                MemType = "D" 'Logix 5000 Boolean data type
            Case Else
                MemType = String.Empty
        End Select

    End Function
    Private Function OneTimeRead(ByRef link As clsLink) As Boolean
        '****************************************************************************************
        'Description: This function reads data One Time from the plc
        '
        'Parameters: None
        'Returns:    True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim oItem As OPCItem = Nothing
        Dim oData As Object = 0

        Try
            '
            ' One time Read code here
            '
            oItem = GetOPCItem(link)
            If Not mbOffline Then
                oItem.Read(CShort(OPCDataSource.OPCDevice), oData)
            End If
            ReadPLCdata(oData, link)


        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:OneTimeRead", ex.Message)
            Return False
        End Try

        Return True

    End Function
    Private Function PLCTagInfo(ByVal TagName As String) As clsLink
        '********************************************************************************************
        'Description:  Return a structure that contains data related to the PLC Tag
        '
        'Parameters: TagName - Tag to look up
        'Returns:    Tag related info
        '
        'Modification history:
        '
        ' Date      By      Reason
        '2/3/06     Geo     Original
        '********************************************************************************************
        Dim sError As String

        Dim sPath As String = "//plctag[id='" & TagName & "']"
        Dim sTopic As String = String.Empty
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList
        Dim oXMLDoc As New XmlDocument
        Dim oLink As New clsLink

        Try
            oXMLDoc.Load(XMLPath)
            oNodeList = oXMLDoc.SelectNodes(sPath)
        Catch ex As Exception
            sError = gcsRM.GetString("csINVALID_XPATH_SYNTAX") & "[" & sPath & "]"
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", sError & " - " & ex.Message)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, sError, msMODULE & ":clsPLCComm:PLCTagInfo", ex.Message)
            Return Nothing
        End Try

        If oNodeList.Count = 0 Then
            sError = gcsRM.GetString("csPLC_TAG_NOT_DEFINED") & " [" & TagName & "] "
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", sError)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, sError, msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
            Return Nothing
        Else
            oNode = oNodeList(0) 'Should only be one match!!!
            sTopic = oNode.Item("topic").InnerXml

            With oLink
                .TagName = TagName
                .ZoneName = ZoneName
                .Address = oNode.Item("addr").InnerXml
                .Length = CType(oNode.Item("length").InnerXml, Short)
                .Topic = sTopic.ToUpper
                If Not oNode.Item("bit") Is Nothing Then
                    Dim sBit As String = TryCast(oNode.Item("bit").InnerXml, String)
                    If IsNothing(sBit) = False Then
                        If sBit <> String.Empty Then
                            .Bit = CType(oNode.Item("bit").InnerXml, Short)
                        End If
                    End If
                End If
                If Not oNode.Item("type") Is Nothing Then
                    .Type = oNode.Item("type").InnerXml
                    .MemType = MemType(.Type)
                End If
                Dim sRef(.Length - 1) As String
                For nIndex As Integer = 0 To .Length - 1
                    sRef(nIndex) = "0"
                Next
                .RefData = sRef
            End With

            'Make sure Zonename doesn't include any spaces. If it does, replace them with "_" (underbars)
            sPath = "//" & Strings.Replace(ZoneName, " ", "_") & "[id='" & sTopic & "']"
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                         "(" & oNodeList.Count.ToString & _
                         ") instances found of Tag [" & TagName & "].")
            RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_TAG_MULT_INST") & " [" & TagName & "]", _
                                   msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
        End If

        Try
            oNodeList = oXMLDoc.SelectNodes(sPath)
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csINVALID_XPATH_SYNTAX") & _
                                   " [" & sPath & "]", msMODULE & ":clsPLCComm:PLCTagInfo", ex.Message)
            Return Nothing
        End Try

        If oNodeList.Count = 0 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Topic [" & sTopic & "] for Zone [" & ZoneName & "] not found.")
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, _
                                   gcsRM.GetString("csPLC_TOPIC_NOT_DEFINED") & " [" & sTopic & "] " & _
                                   gcsRM.GetString("csZONE_NAME") & " [" & ZoneName & "]", _
                                   msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
            Return Nothing
        Else
            oNode = oNodeList(0) 'Should only be one match!!!
            With oLink
                .IP_Address = oNode.Item("ip").InnerXml
                .PollRate = CType(oNode.Item("rate").InnerXml, Integer)
                .Slot = oNode.Item("slot").InnerXml
                .ControlName = String.Empty
                .Busy = False
                .ElapsedTime = 0
                .ReturnCode = 0
            End With
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                           "(" & oNodeList.Count.ToString & _
                           ") instances found of Topic [" & sTopic & "] for Zone [" & ZoneName & "].")
            RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_TOPIC_MULT_INST") & " [" & sTopic & "]", _
                                   msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
        End If

        Return oLink

    End Function
    Private Function ReadPLCData(ByRef Data() As String, ByVal Link As clsLink) As Boolean
        '********************************************************************************************
        'Description:  This is for read of the data - also called on hotlink
        '
        'Parameters: none
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If Not IsNothing(Link) Then



                ' Poke the PLC for data
                If OneTimeRead(Link) Then
                    Data = Link.RefData
                    Return True
                End If
            End If

            Return False

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csCOULD_NOT_READ_PLC"), msMODULE & ":clsPLCComm:ReadPLCData", String.Empty)
        End Try

    End Function
    Private Function WritePLCData(ByVal Data() As String, ByVal Link As clsLink) As Boolean
        '********************************************************************************************
        'Description:  Write data to the PLC
        '
        'Parameters: array of string values
        'Returns:   True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/26/09  MSW     BOOL debug - force a read first.
        '********************************************************************************************
        Dim oItem As OPCItem = Nothing
        Dim oData As Object = 0
        Try
            For i As Integer = 0 To (UBound(Data) - 1)
                oData(i) = CInt(Data(i))
            Next
            For i As Integer = UBound(Data) To (Link.Length - 1)
                oData = CInt(Data(i))
            Next

            oItem = GetOPCItem(Link)
            If Not mbOffline Then
                oItem.Write(oData)
            End If

            Return True

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), msMODULE & ":clsPLCComm:WritePLCData", String.Empty)
            Return False
        End Try
    End Function
    Private Function GetOPCItem(ByRef link As clsLink) As OPCItem
        Dim sAddress As String = link.IP_Address & link.Address & "," & link.Length
        Dim oGroup As OPCGroup = Nothing

        'Connect to server
        If mOPCServer.ServerState <> 1 Then
            mOPCServer.Connect(msOPC_SERVER_NAME)
        End If

        ' Create OPC group if it doesnt exist
        For Each grp As OPCGroup In mOPCGroups
            If grp.Name = link.Topic Then
                oGroup = grp
                Exit For
            End If
        Next
        If oGroup Is Nothing Then
            oGroup = mOPCGroups.Add(link.Topic)
            If link.Topic = "MANUAL" Or link.PollRate = mnMANUAL_POLL_RATE Then
                oGroup.IsActive = False
                oGroup.IsSubscribed = False
            Else
                oGroup.IsActive = True
                oGroup.IsSubscribed = True
                mnHotLinks += 1
                link.Clienthandle = 1000 + mnHotLinks
                mLinks.Add(link)
            End If
        End If

        ' Create OPC Item if it doesn't exist
        For Each item As OPCItem In oGroup.OPCItems
            If item.ItemID = sAddress Then
                Return item
            End If
        Next
        Return oGroup.OPCItems.AddItem(sAddress, link.Clienthandle)

    End Function
#End Region
#Region " Events "
    Sub GlobalDataChange(ByVal TransactionID As Integer, _
                         ByVal GroupHandle As Integer, _
                         ByVal NumItems As Integer, _
                         ByRef ClientHandles As System.Array, _
                         ByRef ItemValues As System.Array, _
                         ByRef Qualities As System.Array, _
                         ByRef TimeStamps As System.Array) _
                         Handles mOPCGroups.GlobalDataChange
        '
        ' You know... do that thing with that widget and make sure not to forget to shizzle
        '
        For item As Integer = 1 To NumItems
            For Each link As clsLink In mLinks
                If link.Clienthandle = ClientHandles(item) Then
                    ReadPLCdata(ItemValues(item), link)
                    RaiseEvent NewData(link.ZoneName, link.TagName, link.RefData)
                End If
            Next
        Next

    End Sub
#End Region
#Region " clsLink "
    Private Class clsLink
#Region " Declares "
        Private msAddress As String = String.Empty
        Private msControlName As String = String.Empty
        Private mnBit As Short ' = 0
        Private mbBusy As Boolean ' = False
        Private mnElapsedTime As Integer ' = 0
        Private msIP_Address As String = String.Empty
        Private mnLength As Short ' = 0
        Private msMemType As String = String.Empty
        Private mnPollRate As Integer ' = 0
        Private msRefData() As String
        Private mnReturnCode As Integer ' = 0
        Private msSlot As String = String.Empty
        Private msTagName As String = String.Empty
        Private msType As String = String.Empty
        Private msZoneName As String = String.Empty
        Private msTopic As String = String.Empty
        Private mnClientHandle As Integer
#End Region
#Region " Properties "
        Friend Property Address() As String
            '********************************************************************************************
            'Description: The PLC Data Table address string
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msAddress
            End Get

            Set(ByVal value As String)
                msAddress = value
            End Set

        End Property
        Friend Property Topic() As String
            Get
                Return msTopic
            End Get
            Set(ByVal value As String)
                msTopic = value
            End Set
        End Property
        Friend Property Clienthandle() As Integer
            Get
                Return mnClientHandle
            End Get
            Set(ByVal value As Integer)
                mnClientHandle = value
            End Set
        End Property
        Friend Property Bit() As Short
            '********************************************************************************************
            'Description: The bit number within the PLC Dat word for Boolean Data Type.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnBit
            End Get

            Set(ByVal value As Short)
                mnBit = value
            End Set

        End Property
        Friend Property Busy() As Boolean
            '********************************************************************************************
            'Description: True if a read or write operation is in progress.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mbBusy
            End Get

            Set(ByVal value As Boolean)
                mbBusy = value
            End Set

        End Property
        Friend Property ControlName() As String
            '********************************************************************************************
            'Description: Name of the Asabtcp Control associated with this link
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msControlName
            End Get

            Set(ByVal value As String)
                msControlName = value
            End Set

        End Property
        Friend Property ElapsedTime() As Integer
            '********************************************************************************************
            'Description: The elapsed time (in milliseconds) a read or write operation has been in 
            '             progress.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Get
                Return mnElapsedTime
            End Get

            Set(ByVal value As Integer)
                mnElapsedTime = value
            End Set

        End Property
        Friend Property IP_Address() As String
            '********************************************************************************************
            'Description: The IP Address of the PLC that data is being read from/written to.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msIP_Address
            End Get

            Set(ByVal value As String)
                msIP_Address = value
            End Set

        End Property
        Friend Property Length() As Short
            '********************************************************************************************
            'Description: The number of PLC Data Table words being read/written
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnLength
            End Get

            Set(ByVal value As Short)
                If value < 1 Then Exit Property
                mnLength = value
            End Set

        End Property
        Friend Property MemType() As String
            '********************************************************************************************
            'Description: Internal data type specifier
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msMemType
            End Get

            Set(ByVal value As String)
                msMemType = value
            End Set

        End Property
        Friend Property PollRate() As Integer
            '********************************************************************************************
            'Description: In the case of a HotLink, the data read Poll Rate (in milliseconds)
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnPollRate
            End Get

            Set(ByVal value As Integer)
                If value < 0 Then Exit Property
                mnPollRate = value
            End Set

        End Property
        Friend Property RefData() As String()
            '********************************************************************************************
            'Description: In the case of a HotLink, this is the data that was returned on the last
            '             "Complete" event. It is used to determine if a data change has occurred.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msRefData
            End Get

            Set(ByVal value As String())
                msRefData = value
            End Set

        End Property
        Friend Property ReturnCode() As Integer
            '********************************************************************************************
            'Description: The value of the Asabtcp control "Result" from the last PLC Data Table read/
            '             write operation.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnReturnCode
            End Get

            Set(ByVal value As Integer)
                mnReturnCode = value
            End Set

        End Property
        Friend Property Slot() As String
            '********************************************************************************************
            'Description: In the case of a Logix PLC, this is the slot (number) where the target CPU
            '             resides. In the case of a SLC500 CPU, this is the Data Table Offset for
            '             unsolicited messages. This property is not used for a PLC-5.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msSlot
            End Get

            Set(ByVal value As String)
                msSlot = value
            End Set

        End Property
        Friend Property TagName() As String
            '********************************************************************************************
            'Description: The TagName.
            '
            'Parameters:
            'Returns:    
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
            End Set

        End Property
        Friend Property Type() As String
            '********************************************************************************************
            'Description: The data type specified in the Tag file.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msType
            End Get

            Set(ByVal value As String)
                msType = value
            End Set

        End Property
        Friend Property ZoneName() As String
            '********************************************************************************************
            'Description: The name of the precess zone that this PLC data is associated with.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msZoneName
            End Get

            Set(ByVal value As String)
                msZoneName = value
            End Set

        End Property
#End Region
    End Class  'clsLink
    Private Class clsLinks
        '********************************************************************************************
        'Description:  This class is to hold a collection of Links
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Inherits CollectionBase
#Region " Properties "
        Friend Overloads ReadOnly Property Count() As Integer
            '********************************************************************************************
            'Description: How many links are in the collection
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return List.Count
            End Get

        End Property
#End Region
#Region " Routines "
        Friend Function Add(ByRef oLink As clsLink) As Integer
            '********************************************************************************************
            'Description: Add a new link to the collection
            '
            'Parameters:
            'Returns:    The index in the collection of the added link   
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim index As Integer = 0

            Try
                index = List.Add(oLink)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Add", ex.Message & vbCrLf & ex.StackTrace)
            End Try

            Return index

        End Function
        Friend Function Item(ByVal ControlName As String) As clsLink
            '********************************************************************************************
            'Description: Fetch the Link item that belongs to the named control
            '
            'Parameters: ControlName - Link identifier
            'Returns:    clsLink associated with the named control
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                Dim oLink As clsLink

                For Each oLink In List
                    If oLink.ControlName = ControlName Then
                        Return oLink
                    End If
                Next
                'not found
                Return Nothing

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Item", ex.Message & vbCrLf & ex.StackTrace)
                Return Nothing
            End Try

        End Function
        Friend Sub Remove(ByVal Link As clsLink)
            '********************************************************************************************
            'Description: Remove the Link from the list
            '
            'Parameters: Link - Link to be removed
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                MyBase.List.Remove(Link)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Remove", ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub
#End Region
    End Class  'clsLinks
#End Region
End Class 'clsPLCComm

