' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: ProdData.vb
'
' Description: Collection for Devices moitored by the PLC, ProdDevice class, 
'              and ProductionLogger common code
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak (structure plagerized from Speedy)
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                                  Version
'    10/07/11   MSW     Prevent calling for a new download before the last one is done.
'                       Probably need to come up with a solution to support multiple downloads. 4.1.0.1
'    11/21/11   MSW     Tweak DMON logger attempting to keep it from rewriting an old file.     4.1.0.2
'                       The real fix ended up being in clsFSFTP, but I kept some of the changes here.
'    03/28/12   MSW     Move DB to XML                                                          4.1.3.0
'    09/30/13   MSW     PLC DLL                                                                 4.01.06.00
'    05/29/14   MSW     GetDiagData - Handle 0 length vin 
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Resources
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm

'********************************************************************************************
'Description: Production Zones Collection
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsProdZones
    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsProdZones"
    Private Const msTableName As String = "PLC Production Zones"  '1/21/09 gks - sqldb
    '***** End Module Constants *********************************************************************

    '***** Module Variables  ************************************************************************
    Friend WithEvents moPLC As New clsPLCComm
    '***** End Module Variables *********************************************************************

    '********** Event Variables *********************************************************************
    Friend Event ProdNotification(ByVal ZoneName As String, ByVal DeviceName As String, _
                                                                        ByVal ProdData() As String)
    Friend Event DiagNotification(ByVal ZoneName As String, ByVal DeviceName As String)
    Friend Event NewStatus(ByVal ZoneName As String, ByVal Status() As String)
    '********** End Event Variables *****************************************************************

#End Region

#Region " Properties "

#End Region

#Region " Routines "

    Friend Function Add(ByVal DeviceCollection As clsProdDevices) As Integer
        '********************************************************************************************
        'Description: Add a collection of Production Devices to the Zones collection
        '
        'Parameters: clsProdDevices
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Call subAddHandlers(DeviceCollection)
            Application.DoEvents()
            Return List.Add(DeviceCollection)

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: Add", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function

    Friend Sub MonitorStatus()
        '********************************************************************************************
        'Description: Start the production data ready monitor for all zones
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each o As clsProdDevices In List
            o.StartStatusMonitor()
        Next

    End Sub

    Private Sub subAddHandlers(ByVal DeviceCollection As clsProdDevices)
        '********************************************************************************************
        'Description: Add event handlers as Production Device collections are added to the Zone 
        '             collection.
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        AddHandler DeviceCollection.ZoneProdDataNotify, AddressOf subProdDataNotification
        AddHandler DeviceCollection.ZoneDiagNotification, AddressOf subDiagDataNotification

    End Sub

    Private Sub subInitCollection(ByRef ZoneCollection As clsZones, ByVal AllZones As Boolean)
        '*****************************************************************************************
        'Description: Create a clsPLCProdDevices class for each Zone we want to monitor.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01.21.09  gks     Change to SQL database
        ' 03/28/12  MSW     Move DB to XML                                                          4.1.3.0
        '*****************************************************************************************
        Const sXMLFILE As String = "ProdLinks"
        Const sXMLZONESTABLE As String = "ProdZones"
        Const sXMLZONENODE As String = "ProdZone"
        Const sXMLDEVICESTABLE As String = "ProdDevices"
        Const sXMLDEVICENODE As String = "ProdDevice"
        Try
            Dim sTopic As String = String.Empty
            Dim oZonesNode As XmlNode = Nothing
            Dim oZoneList As XmlNodeList = Nothing
            Dim oDevicesNode As XmlNode = Nothing
            Dim oDeviceList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            Dim bSuccess As Boolean = False
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oZonesNode = oXMLDoc.SelectSingleNode("//" & sXMLZONESTABLE)
                    oZoneList = oZonesNode.SelectNodes("//" & sXMLZONENODE)
                    oDevicesNode = oXMLDoc.SelectSingleNode("//" & sXMLDEVICESTABLE)
                    oDeviceList = oDevicesNode.SelectNodes("//" & sXMLDEVICENODE)

                    For Each oNode As XmlNode In oZoneList
                        If AllZones Or (oNode.Item("ZoneName").InnerXml = ZoneCollection.CurrentZone) Then
                            Dim o As New clsProdDevices(oNode, oDeviceList, ZoneCollection, Me)
                            Add(o)
                        End If
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUpdateMaintDate", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subInitCollection", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region " Events "

    Private Sub subProdDataNotification(ByVal ZoneName As String, ByVal DeviceName As String, _
                                                                        ByVal ProdData() As String)
        '********************************************************************************************
        'Description: New Production Data from a device. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent ProdNotification(ZoneName, DeviceName, ProdData)

    End Sub

    Private Sub subDiagDataNotification(ByVal ZoneName As String, ByVal DeviceName As String)
        '********************************************************************************************
        'Description:   Diag file Ready. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent DiagNotification(ZoneName, DeviceName)

    End Sub



    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                    ByVal sModule As String, ByVal AdditionalInfo As String) Handles moPLC.ModuleError
        '********************************************************************************************
        'Description: An error has occurred in the frmPLCComm that is used to monitor the "New 
        '             Production Data Present" status word in the PLC for this zone.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMsg As String = nErrNum.ToString & "-" & sErrDesc & " occurred in  Module " & sModule & "."

        If AdditionalInfo <> String.Empty Then
            sMsg = sMsg & " Additional Info: " & AdditionalInfo
        End If

        mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: moStatus_ModuleError", _
                               "Error: " & sMsg)

    End Sub

    Friend Sub New(ByRef ZoneCollection As clsZones, ByVal MonitorAllZones As Boolean)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        MyBase.New()
        subInitCollection(ZoneCollection, MonitorAllZones)

    End Sub

    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                    ByVal Data() As String) Handles moPLC.NewData
        '********************************************************************************************
        'Description: The frmPLCComm that monitors the "New Production Data Present" status word in 
        '             the PLC for this zone reported new data read.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try

            'Raise an event that each clsProdDevices object is listening for. The one who's ZoneName
            'matches will grab this Data.
            RaiseEvent NewStatus(ZoneName, Data)

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: moPLC_NewData", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

#End Region

    Private Sub clsProdZones_ProdNotification(ByVal ZoneName As String, ByVal DeviceName As String, ByVal ProdData() As String) Handles Me.ProdNotification

    End Sub
End Class

'********************************************************************************************
'Description: Production Devices Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsProdDevices

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsProdDevices"
    Private Const msTableName As String = "PLC Production Devices"  '1/21/09 gks sqldb
    '***** End Module Constants *********************************************************************

    '***** Module Variables *************************************************************************
    Private mnAck As Integer = 0
    Friend goPLC As clsPLCComm = Nothing
    Friend goZone As clsZone = Nothing '>>>New
    Private msLastStatus(2) As String ' = "0"  '1/21/09 gks change to array
    Private msProdData() As String
    Private msStatus() As String
    '***** End Module Variables *********************************************************************

    '********** Event Variables *********************************************************************
    Friend Event ZoneProdDataNotify(ByVal Zone As String, ByVal Device As String, _
                                                                    ByVal ProdData() As String)
    Friend Event ZoneDiagNotification(ByVal ZoneName As String, ByVal DeviceName As String)
    '********** End Event Variables *****************************************************************

    '********** Property Variables ******************************************************************
    Private msAckTag As String = String.Empty
    Private msStatusTag As String = String.Empty
    Private msZoneName As String = String.Empty
    '********** End Property Variables **************************************************************

#End Region

#Region " Properties "

    Private ReadOnly Property AckTag() As String
        '********************************************************************************************
        'Description: Get the Production Data Processed acknowledge tag name 
        '
        'Parameters: none
        'Returns:    Tag Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msAckTag
        End Get

    End Property

    Private ReadOnly Property StatusTag() As String
        '********************************************************************************************
        'Description: Get the Production Data status tag name 
        '
        'Parameters: none
        'Returns:    Tag Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msStatusTag
        End Get

    End Property

    Friend ReadOnly Property ZoneName() As String
        '********************************************************************************************
        'Description: Get the Zone Name
        '
        'Parameters: none
        'Returns:    Zone Name
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

    Friend Function Add(ByVal Device As clsProdDevice) As Integer
        '********************************************************************************************
        'Description: Add a Production Device to collection
        '
        'Parameters: clsPLCProdDevice
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Return List.Add(Device)

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: Add", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function

    Private Function DeviceExists(ByVal BitIndex As Integer) As Boolean
        '********************************************************************************************
        'Description: Return True if a clsProdDevice with this Bitindex exitis
        '
        'Parameters: BitIndex
        'Returns:    True = Device exists
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim bExists As Boolean = False

            For Each o As clsProdDevice In List
                If o.BitIndex = BitIndex Then
                    bExists = True
                    Exit For
                End If
            Next

            Return bExists

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: DeviceExists", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            Return False
        End Try

    End Function

    Friend Sub StartStatusMonitor()
        '********************************************************************************************
        'Description: Set up a hot link to monitor the status register for this zone. This is done
        '             after this instance of clsProdDevices is created so we won't miss any 
        '             production data.
        '             Note: The Tag poll rate is what tells clsPLCComm to set this up as a hotlink. 
        '                   The moPLC NewData event will fire when this data changes.
        '
        'Parameters: ZoneName, Production Data Changed Status Data
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nRefreshAll As Integer = 0

            With goPLC
                .ZoneName = goZone.Name
                .TagName = StatusTag
                'Need to read once to start hotlink
                Dim void As String() = .PLCData
            End With 'goPLC

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: StartStatusMonitor", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subCheckStatus(ByVal Zone As String, ByVal Status() As String)
        '********************************************************************************************
        'Description: Check if Status belongs to this zone. If so, act on it.
        '
        'Parameters: ZoneName, Production Data Available Status Data
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 1/21/09   gks     change to monitor 3 words - word 0 is production data (existing) 
        '                   word 1 for diag data - word 2 for ezdebug data
        '********************************************************************************************

        Try

            If Zone = ZoneName Then
                Dim bNew As Boolean = False

                'Convert 16 bit Signed to Unsigned if necessary
                For nIndex As Integer = 0 To Status.GetUpperBound(0)
                    If Strings.Left(Status(nIndex), 1) = "-" Then
                        Dim nStatus As Integer = CType(Status(nIndex), Integer)
                        nStatus += 65536
                        Status(nIndex) = nStatus.ToString
                    End If
                    bNew = bNew Or (Status(nIndex) <> msLastStatus(nIndex))
                Next


                'If (Status(0) = msLastStatus(0)) And (Status(1) = msLastStatus(1)) And _
                '                                        (Status(2) = msLastStatus(2)) Then

                If bNew = False Then
                    'Nothing's new
                    Exit Sub
                Else
                    'Ack to PLC immediately
                    With goPLC
                        .ZoneName = goZone.Name
                        .TagName = AckTag
                        .PLCData = Status
                    End With

                    ' at least 1 word changed
                    For nIndex As Integer = 0 To Status.GetUpperBound(0)
                        ' update reference in case we err out on the way
                        msLastStatus(nIndex) = Status(nIndex) '"0"
                    Next
                    ' the production data is last, this routine sets writes the ack words 
                    ' after each device is read. the above data gets all data at once and
                    ' should be done in the background
                    If (Status(0) <> "0") Then
                        Call subDoProduction(CType(Status(0), Integer))
                    End If
                    If (Status.GetUpperBound(0) >= 1) Then
                        If (Status(1) <> "0") And (Status(1) <> String.Empty) Then
                            subDoDMON(CType(Status(1), Integer))
                        End If
                    End If

                End If 'Status(0) = msLastStatus

            End If 'Zone = ZoneName

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subCheckStatus", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subDoProduction(ByVal StatusWord As Integer)
        '********************************************************************************************
        'Description: Do the production log part - broke out from subcheckstatus
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mnAck = 0
        'Tell the Device classes with new production Data to go get it.
        For nBit As Integer = 0 To 15
            If ((StatusWord) And CType(2 ^ nBit, Integer)) > 0 Then
                If DeviceExists(nBit) Then
                    For Each oDevice As clsProdDevice In List

                        If oDevice.BitIndex = nBit And oDevice.ZoneName = ZoneName Then 'NRU 160930 Added zone requirement

                            Dim sProdData() As String = Nothing

                            If oDevice.GetProdData(sProdData) Then
                                RaiseEvent ZoneProdDataNotify(ZoneName, oDevice.DeviceName, sProdData)
                            End If

                        End If 'oDevice.BitIndex = nBit
                    Next 'oDevice
                Else
                    mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subDoProduction", _
                                           "Error: A Production Data Present status flag was received for an " & _
                                           "unknown device. Bit = [" & nBit.ToString & "]")
                End If 'DeviceExists(nBit)
                'Call subUpdateAck(nBit)
            End If '(nStatus And CType...
        Next 'nBit

    End Sub
    Private Sub subDoDMON(ByVal DiagWord As Integer)
        '********************************************************************************************
        'Description: Do the production log part - broke out from subcheckstatus
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mnAck = 0
        'Tell the Device classes with new production Data to go get it.
        For nBit As Integer = 0 To 15
            If ((DiagWord) And CType(2 ^ nBit, Integer)) > 0 Then
                If DeviceExists(nBit) Then
                    For Each oDevice As clsProdDevice In List

                        If oDevice.BitIndex = (nBit) And oDevice.ZoneName = ZoneName Then 'NRU 160930 Added zone requirement

                            Dim sProdData() As String = Nothing

                            oDevice.GetProdData(sProdData)


                            If (DiagWord And CType(2 ^ nBit, Integer)) > 0 Then
                                oDevice.GetDiagData(sProdData)
                            End If

                        End If 'oDevice.BitIndex = nBit
                    Next 'oDevice
                Else
                    mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subDoProduction", _
                                           "Error: A Production Data Present status flag was received for an " & _
                                           "unknown device. Bit = [" & nBit.ToString & "]")
                End If 'DeviceExists(nBit)
                'Call subUpdateAck(nBit)
            End If '(nStatus And CType...
        Next 'nBit

    End Sub

    Private Sub subInitCollection(ByRef ZoneData As XmlNode, ByRef DeviceData As XmlNodeList, ByRef ZoneCollection As clsZones)
        '*****************************************************************************************
        'Description: Load the Production Device collection
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '01.21.09   gks     Change to SQLdb
        ' 03/28/12  MSW     Move DB to XML
        '*****************************************************************************************
        Try
            Try
                msZoneName = ZoneData.Item("ZoneName").InnerXml
                msStatusTag = ZoneData.Item("StatusTag").InnerXml
                msAckTag = ZoneData.Item("AckTag").InnerXml

                goZone = ZoneCollection.Item(msZoneName) '>>>New
                ZoneCollection.CurrentZone = msZoneName
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUpdateMaintDate", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try


            Dim tArmArray() As tArm = GetArmArray(ZoneCollection.ActiveZone)

            For Each oNode As XmlNode In DeviceData
                Dim o As New clsProdDevice(oNode.ChildNodes, Me)
                For Each Arm As tArm In tArmArray
                    If Arm.ArmDisplayName = o.DeviceName Then
                        o.HostName = Arm.Controller.HostName
                        Exit For
                    End If
                Next
                Add(o)
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subInitCollection", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateAck(ByVal BitIndex As Integer)
        '********************************************************************************************
        'Description: A ProdDevice class has read the production data table from the PLC. Update the 
        '             Ack varible by setting the bit corresponding to BitIndex. If all devices in 
        '             this zone are done, send the Ack register to the PLC.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '1/21/09    gks     change to array
        '********************************************************************************************
        Try

            'set bit [BitIndex] in mnAck
            mnAck += CType(2 ^ BitIndex, Integer)

            If mnAck = CType(msLastStatus(0), Integer) Then
                'Dim sData(1) As String

                'For nIndex As Integer = 0 To 1
                '    sData(nIndex) = msLastStatus(nIndex)
                'Next
                ''Were done getting all the production data table changes for devices in this zone. Tell the PLC.
                With goPLC
                    .ZoneName = goZone.Name
                    .TagName = AckTag
                    .PLCData = msLastStatus
                End With

            End If 'mnAck = CType(msLastStatus, Integer)

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subUpdateAck", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByVal ZoneData As XmlNode, ByRef Devices As XmlNodeList, ByVal ZoneCollection As clsZones, _
                                                                    ByRef Parent As clsProdZones)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters:  Zone Datarow, Database Path, Reference to Parent class
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move DB to XML
        '*****************************************************************************************

        MyBase.New()
        goPLC = Parent.moPLC
        AddHandler Parent.NewStatus, AddressOf subCheckStatus

        Call subInitCollection(ZoneData, Devices, ZoneCollection)

    End Sub
#End Region

 
    Private Sub clsProdDevices_ZoneProdDataNotify(ByVal Zone As String, ByVal Device As String, ByVal ProdData() As String) Handles Me.ZoneProdDataNotify

    End Sub
End Class

'********************************************************************************************
'Description: Production Device object
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************

Friend Class clsProdDevice

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsProdDevice"
    '***** End Module Constants *********************************************************************

    '************ Property Variables ****************************************************************
    Private mnBitIndex As Integer = 0
    Private msDeviceName As String = String.Empty
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msDmonPath As String = String.Empty
    Private mnStylePtr As Integer
    Private mnColorPtr As Integer
    Private mnOptionPtr As Integer
    Private mnVinPtr As Integer
    Private mnVinLen As Integer
    Private msHostName As String
    Private msElement As String
    '************ End Property Variables ************************************************************

    '******** Module Variables **********************************************************************
    Private moPLC As clsPLCComm = Nothing
    Private moZone As clsZone = Nothing
    Private msProdData() As String = Nothing
    Private msProdRef() As String = Nothing
    '******** End Module Variables ******************************************************************

    '***** Background worker variables    ************************************************************
    Private WithEvents mbgDiagWorker As System.ComponentModel.BackgroundWorker
    'Private WithEvents mbgEZWorker As New System.ComponentModel.BackgroundWorker
    '***** End Background worker variables    ********************************************************
#End Region

#Region " Properties "

    Friend ReadOnly Property BitIndex() As Integer
        '********************************************************************************************
        'Description:  Get the Bit Number in the Production Status word that corresponds to this device.
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnBitIndex
        End Get

    End Property

    Friend ReadOnly Property DeviceName() As String
        '********************************************************************************************
        'Description:  Get the Device Name
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msDeviceName
        End Get

    End Property

    Friend ReadOnly Property TagName() As String
        '********************************************************************************************
        'Description:  Get the Tag name assigned to the production data table address for this device.
        '
        'Parameters: none
        'Returns:    TagName
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msTagName
        End Get

    End Property

    Friend ReadOnly Property ZoneName() As String 'NRU 160930 Private to Friend
        '********************************************************************************************
        'Description:  Get the Zone Name to tell the PLC Comm class which zone this device belongs to.
        '
        'Parameters: none
        'Returns:    ZoneName
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msZoneName
        End Get

    End Property

    Friend Property ColorPointer() As Integer
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnColorPtr
        End Get

        Set(ByVal value As Integer)
            mnColorPtr = value
        End Set

    End Property

    Friend Property HostName() As String
        '********************************************************************************************
        'Description:  For ???
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msHostName
        End Get

        Set(ByVal value As String)
            msHostName = value
        End Set

    End Property

    Friend Property OptionPointer() As Integer
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnOptionPtr
        End Get

        Set(ByVal value As Integer)
            mnOptionPtr = value
        End Set

    End Property

    Friend Property StylePointer() As Integer
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnStylePtr
        End Get

        Set(ByVal value As Integer)
            mnStylePtr = value
        End Set

    End Property

    Friend Property VINElementType() As String
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msElement
        End Get

        Set(ByVal value As String)
            msElement = value
        End Set

    End Property

    Friend Property VINPointer() As Integer
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnVinPtr
        End Get

        Set(ByVal value As Integer)
            mnVinPtr = value
        End Set

    End Property

    Friend Property VINLength() As Integer
        '********************************************************************************************
        'Description:  For Diaglogger
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnVinLen
        End Get

        Set(ByVal value As Integer)
            mnVinLen = value
        End Set

    End Property

#End Region

#Region " Routines "

    Friend Function GetProdData(ByRef ProdData() As String) As Boolean
        '********************************************************************************************
        'Description: When a Device sets it's status flag to indicate New Production Data Available, 
        '             the collection class calls this sub to read the data from the PLC. When this 
        '             routine completes, it returns the Production Data and the BitIndex of the device.
        '
        'Parameters: Index
        'Returns:    Index, Production Data array 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bNew As Boolean = False

        Try
            'Read Production Data Table from PLC
            With moPLC
                .ZoneName = moZone.Name
                .TagName = TagName
                msProdData = .PLCData
            End With

            '12/10/08
            If msProdData Is Nothing Then
                'no plc read for some reason
                Dim sMsg As String = "Failed PLC Read Routine: GetProdData, Tag =  " & TagName
                Throw New Exception(sMsg)
            End If

            If msProdRef Is Nothing Then
                'This is the initial read. Size this array to match msProdData and fill it with "0"s.
                ReDim msProdRef(msProdData.GetUpperBound(0))
                For Each sItem As String In msProdRef
                    sItem = "0"
                Next
            End If

            'Determine if this is new data
            For nIndex As Integer = 0 To msProdData.GetUpperBound(0)
                If msProdData(nIndex) <> msProdRef(nIndex) Then
                    bNew = True
                    Exit For
                End If
            Next

            'Update the last state array
            If bNew Then
                msProdRef = msProdData
            End If
            ProdData = msProdData

        Catch ex As Exception
            Dim sMsg As String = "Zone: " & ZoneName & ", Device: " & DeviceName & ", Routine: GetProdData, "

            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE, sMsg & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            GetProdData = bNew
        End Try

    End Function

    Friend Function GetDiagData(ByVal ProdData As String()) As Boolean
        '********************************************************************************************
        'Description: After getting production data, the device checks the diag status bit (which is
        '               Really an enable bit) if set - gets diag data
        '
        'Parameters: Index
        'Returns:    Index, Production Data array 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/11  MSW     Prevent calling for a new download before the last one is done.
        '                   Probably need to come up with a solution to support multiple downloads.
        ' 05/29/14  MSW     GetDiagData - Handle 0 length vin
        '********************************************************************************************
        Dim sColor As String = String.Empty
        If frmMain.gbAsciiColor Then
            sColor = mMathFunctions.CvIntegerToASCII(CType(ProdData(mnColorPtr), Integer), frmMain.gnAsciiColorNumChar)
        Else
            sColor = ProdData(mnColorPtr)
        End If

        Dim sStyle As String = String.Empty
        If frmMain.gbAsciiStyle Then
            sStyle = mMathFunctions.CvIntegerToASCII(CType(ProdData(mnStylePtr), Integer), frmMain.gnAsciiStyleNumChar)
        Else
            sStyle = ProdData(mnStylePtr)
        End If
        Dim sDate As String = Format(Now, "yyyyMMdd HHmmss")
        'dig out the file name from production data
        Dim sFileName As String = msDeviceName & gsDMON_DELIMITER & _
                                sDate & gsDMON_DELIMITER & _
                                sStyle & gsDMON_DELIMITER & _
                                sColor & gsDMON_DELIMITER & _
                                ProdData(mnOptionPtr) & gsDMON_DELIMITER
        Dim sVinStr As String = String.Empty
        If mnVinLen > 0 Then
            ' figure out the VIN string
            Dim sVin(mnVinLen - 1) As String

            For nIndex As Integer = mnVinPtr To mnVinPtr + mnVinLen - 1
                sVin(nIndex - mnVinPtr) = ProdData(nIndex)
            Next

            sVinStr = frmMain.FormatProdData(sVin, msElement)

        End If

        'finish file name
        sFileName = sFileName & sVinStr & gsDMON_DELIMITER & moZone.ZoneNumber & ".dt"

        'get file
        Try
            If mbgDiagWorker Is Nothing Then
                mbgDiagWorker = New System.ComponentModel.BackgroundWorker
                mbgDiagWorker.RunWorkerAsync(sFileName)
            Else
                If mbgDiagWorker.IsBusy Then
                    mDebug.WriteEventToLog("Prodlogger Module:ProdData.vb , Routine: GetDiagData", _
                                           "mbgDiagWorker.IsBusy = True")
                Else
                    mbgDiagWorker.RunWorkerAsync(sFileName)
                End If

            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module:ProdData.vb , Routine: GetDiagData", _
                               "mbgDiagWorker.IsBusy failed")
        End Try


    End Function
    Private Sub subInitialize(ByVal nlDevice As XmlNodeList)
        '********************************************************************************************
        'Description: When a new Production Device is created, its info is passed in in the form of a 
        '             datarow from the Prod Devices table in the configuration database.
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move DB to XML
        '********************************************************************************************

        Try
            'Get the data that defines the location of the production data table for this device.
            For Each oNode As XmlNode In nlDevice
                Select Case oNode.Name
                    Case "DeviceName"
                        msDeviceName = oNode.InnerText
                    Case "ZoneName"
                        msZoneName = oNode.InnerText
                    Case "BitIndex"
                        mnBitIndex = CType(oNode.InnerText, Integer)
                    Case "Tag"
                        msTagName = oNode.InnerText
                End Select
            Next
            If GetDefaultFilePath(msDmonPath, eDir.DmonData, String.Empty, String.Empty) Then
            Else
                msDmonPath = String.Empty
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Prodlogger Module: " & msMODULE & ", Routine: subInitialize", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
#End Region

#Region " Events "

    Friend Sub New(ByVal Device As XmlNodeList, ByVal Parent As clsProdDevices)
        '********************************************************************************************
        'Description: When a new production device is added, its info is passed in in the form of a 
        '             datarow from the database.
        '
        'Parameters: datarow, Reference to PLC Comm class, Reference to the Parent collection
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move DB to XML
        '********************************************************************************************

        MyBase.New()
        moPLC = Parent.goPLC
        moZone = Parent.goZone
        subInitialize(Device)

    End Sub

    Private Sub mbgDiagWorker_DoWork(ByVal sender As Object, _
                    ByVal e As System.ComponentModel.DoWorkEventArgs) Handles mbgDiagWorker.DoWork
        '********************************************************************************************
        'Description: This is the background task of getting the file
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFTP As New clsFSFtp(msHostName)

        Try
            With oFTP
                If .Connected() Then
                    Try
                        .WorkingDir = "RD:"
                        .DestDir = msDmonPath
                        '.Directory("*.DT")
                        .EnableOverWrite = True
                        .GetFile("Diag.DT", e.Argument.ToString)
                        '.Delete("Diag.DT")
                    Catch ex As Exception
                    End Try
                End If
                .Close()
            End With
        Catch ex As Exception
        End Try
        oFTP = Nothing
        mbgDiagWorker = Nothing
    End Sub



#End Region


End Class