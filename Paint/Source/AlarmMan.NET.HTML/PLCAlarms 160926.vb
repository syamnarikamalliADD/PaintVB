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
' Form/Module: PLCAlarms.vb
'
' Description: Collection for Devices moitored by the PLC, AlarmDevice class, 
'              and PLC Alarm common code
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
'    Date       By      Reason                                                        Version
'    01/16/08   BTK     In clsPLCAlarmDevice_BuildAlarm added code so we log the zone 
'                       number for plc generated zone alarms.
'    10/08/08   gks     Add 32 bit
'    04/16/10   RJO     Changes to clsPLCAlarmDevice AssocData, subGetAlarms and 
'                       moDevicePLCComm_ModuleError to attempt to recover when 
'                       moDevicePLCComm goes out to lunch.
'    11/18/10   AM      Commented out Culture to be force to Default culture and should use Display Culture
'    04/05/12   MSW     Move DB from SQL to XML
'    07/27/12   JBW     Bug fix to clsPLCAlarmDevice moDevicePLCComm_ModuleError exception recovery code.
'    11/02/12   MSW     moDevicePLCComm_ModuleError - Move redim outside of for loop
'    09/30/13   MSW     PLC DLL
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

Public Enum ePLCAssocData
    ProdId = 0
    JobID = 1
    Style = 2
    Color = 3
End Enum

'********************************************************************************************
'Description: PLC Alarm Zones Collection
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsPLCAlarmZones
    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsPLCAlarmZones"
    Private Const msTableName As String = "PLC ALarm Zones"
    '***** End Module Constants *********************************************************************

    '***** Module Variables  ************************************************************************
    'Friend WithEvents moPLC As New clsPLCComm
    Friend WithEvents goPLC As New clsPLCComm
    '***** End Module Variables *********************************************************************

    '********** Event Variables *********************************************************************
    Friend Event AlarmNotification(ByVal Alarm As clsPLCAlarm)
    Friend Event BumpProgress()
    Friend Event NewStatus(ByVal ZoneName As String, ByVal Status() As String)
    '********** End Event Variables *****************************************************************

#End Region

#Region " Properties "

#End Region

#Region " Routines "

    Friend Function Add(ByVal DeviceCollection As clsPLCAlarmDevices) As Integer
        '********************************************************************************************
        'Description: Add a collection of PLC Alarm Devices to the Zones collection
        '
        'Parameters: clsPLCAlarmDevices
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
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: Add", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Friend Function GetAssocData(ByVal ZoneName As String, ByVal DeviceName As String, _
                                 ByVal Equipment As Integer) As String()
        '********************************************************************************************
        'Description:  Return an up-to-date array of associated process data (ProdID, JobId, Style, 
        '              Color) for the device (if any) that matches the arguments. Process data items 
        '              that are not enabled will have a have a value of "" (String.Empty).
        '
        'Parameters: ZoneName, DeviceName
        'Returns:    Associated process data array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAssocData(3) As String
        Dim bFound As Boolean = False

        'Inititalize array
        For nIndex As Integer = 0 To 3
            sAssocData(nIndex) = String.Empty
        Next

        Try
            For Each oZone As clsPLCAlarmDevices In List
                If oZone.ZoneName = ZoneName Then
                    bFound = True
                    sAssocData = oZone.GetAssocData(DeviceName, Equipment)
                End If
            Next

            If Not bFound Then
                mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: GetAssocData", _
                                       "Error: Could not find a zone with matching name. [" & ZoneName & "]")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: GetAssocData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            GetAssocData = sAssocData
        End Try

    End Function

    Friend Sub MonitorAlarms()
        '********************************************************************************************
        'Description: Start the alarm monitor for all zones
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            For Each o As clsPLCAlarmDevices In List
                o.StartStatusMonitor()
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: MonitorAlarms", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subAddHandlers(ByVal DeviceCollection As clsPLCAlarmDevices)
        '********************************************************************************************
        'Description: add event handlers as PLC Alarm Device collectionss are added to the Zone 
        '             collection.
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        AddHandler DeviceCollection.BumpProgress, AddressOf subBumpProgress
        AddHandler DeviceCollection.ZoneAlarmNotify, AddressOf subAlarmNotification

    End Sub

    Private Sub subInitCollection(ByRef ZoneCollection As clsZones, ByVal AllZones As Boolean)
        '*****************************************************************************************
        'Description: Create a clsPLCAlarmDevices class for each Zone we want to monitor.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 5/9/08    gks     Switch to sql database
        ' 04/05/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Const sXMLFILE As String = "PLC Alarm Zones"
        Const sXMLTABLE As String = "PLCAlarmZones"
        Const sXMLNODE As String = "PLCAlarmZone"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                    If oNodeList.Count > 0 Then
                        For Each oNode As XmlNode In oNodeList

                            If AllZones Or (oNode.Item("ZoneName").InnerXml = ZoneCollection.CurrentZone) Then
                                Dim o As New clsPLCAlarmDevices(oNode, ZoneCollection, Me)
                                Add(o)
                            End If
                        Next
                    Else

                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitCollection", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subInitCollection", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub subAlarmNotification(ByVal Alarm As clsPLCAlarm)
        '********************************************************************************************
        'Description: New alarm received from a device. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent AlarmNotification(Alarm)

    End Sub

    Private Sub subBumpProgress()
        '********************************************************************************************
        'Description: Pass on the Progress
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent BumpProgress()

    End Sub

    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, ByVal sModule As String, ByVal AdditionalInfo As String) Handles goPLC.ModuleError
        '********************************************************************************************
        'Description: An error has occurred in the frmPLCComm that is used to monitor the "Alarms
        '             Present" status word in the PLC for this zone.
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

        mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: moPLC_ModuleError", _
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

        Try
            subInitCollection(ZoneCollection, MonitorAllZones)
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, ByVal Data() As String) Handles goPLC.NewData
        '********************************************************************************************
        'Description: The frmPLCComm that monitors the "Alarms Present" status word in the PLC for 
        '             this zone reported new data read.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try

            'Raise an event that each clsPLCAlarmDevices object is listening for. The one who's ZoneName
            'matches will grab this Data.
            RaiseEvent NewStatus(ZoneName, Data)

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: moPLC_NewData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

End Class

'********************************************************************************************
'Description: PLC Alarm Devices Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsPLCAlarmDevices
    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsPLCAlarmDevices"
    Private Const msTableName As String = "PLC ALarm Devices"
    '***** End Module Constants *********************************************************************

    '***** Module Variables *************************************************************************
    Private mnAck As Integer = 0
    'Private moPLC As clsPLCComm = Nothing
    Friend goPLC As clsPLCComm = Nothing
    Friend goZone As clsZone = Nothing '>>>New
    Private msLastStatus As String = "0"
    Private msStatus() As String
    '***** End Module Variables *********************************************************************

    '********** Event Variables *********************************************************************
    Friend Event ZoneAlarmNotify(ByVal Alarm As clsPLCAlarm)
    Friend Event BumpProgress()
    Friend Event GetAlarms(ByVal BitIndex As Integer)
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
        'Description: Get the PLC Alarms Processed acknowledge tag name 
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
        'Description: Get the PLC Active Alarms status tag name 
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

    Friend Function Add(ByVal Device As clsPLCAlarmDevice) As Integer
        '********************************************************************************************
        'Description: Add a PLC Alarm Device to collection
        '
        'Parameters: clsPLCAlarmDevice
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Call subAddHandlers(Device)
            Application.DoEvents()
            RaiseEvent BumpProgress()
            Return List.Add(Device)

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: Add", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Private Function DeviceExists(ByVal BitIndex As Integer) As Boolean
        '********************************************************************************************
        'Description: Return True if a clsPLCAlarmDevice with this Bitindex exitis
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

            For Each o As clsPLCAlarmDevice In List
                If o.BitIndex = BitIndex Then
                    bExists = True
                    Exit For
                End If
            Next

            Return bExists

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: DeviceExists", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function

    Friend Function GetAssocData(ByVal DeviceName As String, ByVal Equipment As Integer) As String()
        '********************************************************************************************
        'Description:  Return an up-to-date array of associated process data (ProdID, JobId, Style, 
        '              Color) for the device/equipment (if any) that matches the arguments. Process  
        '              data items that are not enabled will have a have a value of "" (String.Empty).
        '
        'Parameters: DeviceName, Equipment (arm) number
        'Returns:    Associated process data array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAssocData(3) As String
        Dim bFound As Boolean = False

        'Inititalize array
        For nIndex As Integer = 0 To 3
            sAssocData(nIndex) = String.Empty
        Next

        Try
            For Each oDevice As clsPLCAlarmDevice In List
                If oDevice.DeviceName = DeviceName Then
                    bFound = True
                    oDevice.Equipment = Equipment
                    sAssocData = oDevice.AssocData
                End If
            Next

            If Not bFound Then
                mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: GetAssocData", _
                                       "Error: Could not find a device with matching name. [" & DeviceName & "]")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: GetAssocData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            GetAssocData = sAssocData
        End Try

    End Function

    Friend Sub StartStatusMonitor()
        '********************************************************************************************
        'Description: Set up a hot link to monitor the status register for this zone. This is done
        '             after this instance of clsPLCAlarmDevices is created so we won't miss any 
        '             alarms.
        '             Note: The Tag poll rate is what tells clsPLCComm toset this up as a hotlink. 
        '                   The moPLC NewData event will fire when this data changes.
        '
        'Parameters: ZoneName, Alarm Data Changed Status Data
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nRefreshAll As Integer = 0

            'First, we'll jump start this thing to make sure we get all of the current plc alarms.
            'Otherwise, nothing will show up in the active alarms table until we receive a change
            'event from the hotlink below '05/21/07 RJO
            For nBit As Integer = 0 To 15
                If DeviceExists(nBit) Then nRefreshAll += CType(2 ^ nBit, Integer)
            Next 'nBit

            msLastStatus = nRefreshAll.ToString

            For nbit As Integer = 0 To 15
                If DeviceExists(nbit) Then RaiseEvent GetAlarms(nbit)
            Next 'nBit

            With goPLC
                .ZoneName = goZone.Name
                .TagName = StatusTag
                'Need to read once to start hotlink
                Dim void As String() = .PLCData
            End With 'goPLC

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: StartStatusMonitor", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subCheckStatus(ByVal Zone As String, ByVal Status() As String)
        '********************************************************************************************
        'Description: Check if Status belongs to this zone. If so, act on it.
        '
        'Parameters: ZoneName, Alarm Data Changed Status Data
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            If Zone = ZoneName Then

                'Convert 16 bit Signed to Unsigned if necessary
                If Strings.Left(Status(0), 1) = "-" Then
                    Dim nStatus As Integer = CType(Status(0), Integer)
                    nStatus += 65536
                    Status(0) = nStatus.ToString
                End If

                If Status(0) = msLastStatus Then
                    'Nothing's new
                    Exit Sub
                Else

                    If Status(0) = "0" Then
                        'Set the Ack to "0" so the PLC knows it's OK to send 
                        'a new batch of alarms
                        msLastStatus = "0"

                        With goPLC
                            .ZoneName = goZone.Name
                            .TagName = AckTag
                            .PLCData = Status
                        End With

                        Exit Sub
                    Else
                        Dim nStatus As Integer = CType(Status(0), Integer)

                        mnAck = 0
                        msLastStatus = Status(0)
                        'Tell the Device classes with new alarms to go get them.
                        For nBit As Integer = 0 To 15
                            If (nStatus And CType(2 ^ nBit, Integer)) > 0 Then
                                If DeviceExists(nBit) Then
                                    RaiseEvent GetAlarms(nBit)
                                    My.Application.DoEvents()
                                Else
                                    mnAck += CType(2 ^ nBit, Integer)
                                    mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subCheckStatus", _
                                                           "Error: An Alarm Table Change Preset status flag was received for an " & _
                                                           "unknown device. Bit = [" & nBit.ToString & "]")
                                End If
                            End If
                        Next 'nBit
                    End If 'Status(0) = "0"

                End If 'Status(0) = msLastStatus

            End If 'Zone = ZoneName

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subCheckStatus", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subAddHandlers(ByVal Device As clsPLCAlarmDevice)
        '********************************************************************************************
        'Description: Add event handlers as PLC Alarm Devices are added to collection
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        AddHandler Device.DeviceAlarmNotify, AddressOf subAlarmNotification
        AddHandler Device.BumpProgress, AddressOf subBumpProgress
        AddHandler Device.GetAlarmsCompleted, AddressOf subUpdateAck

    End Sub

    Private Sub subInitCollection(ByVal ZoneData As XmlNode, ByRef Zones As clsZones)
        '*****************************************************************************************
        'Description: Load the PLC Alarm Device collection
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 5/9/08    gks     Add DB
        ' 04/05/12  MSW     Move SQL to XML
        '*****************************************************************************************



        'Get Property info from the datarow that was passed in when this collection was instantiated.
        With ZoneData
            msZoneName = .Item("ZoneName").InnerText
            msStatusTag = .Item("StatusTag").InnerText
            msAckTag = .Item("AckTag").InnerText
        End With 'ZoneData

        goZone = Zones.Item(msZoneName)
        Const sXMLFILE As String = "PLC Alarm Devices"
        Const sXMLTABLE As String = "PLCAlarmDevices"
        Const sXMLNODE As String = "PLCAlarmDevice"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    For Each z As clsZone In Zones 'NRU 160906 Added for multizone
                        oNodeList = oMainNode.SelectNodes("//" & z.Name & "//" & sXMLNODE) 'NRU 160906 Changed for multizone was: Zones.CurrentZone
                        If oNodeList.Count > 0 Then
                            For Each oNode As XmlNode In oNodeList
                                Dim o As New clsPLCAlarmDevice(oNode, Me)
                                Add(o)
                            Next
                        Else

                        End If
                    Next 'NRU 160906 Added for multizone
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitCollection", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subInitCollection", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Sub

    Private Sub subUpdateAck(ByVal BitIndex As Integer)
        '********************************************************************************************
        'Description: An event has been received from an AlarmDevice class, indicating it is done 
        '             processing changes in the Alarm table. Update the Ack varible by setting the
        '             bit corresponding to BitIndex. If all devices in this zone are done, send the
        '             Ack register to the PLC.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            'set bit [BitIndex] in mnAck
            mnAck += CType(2 ^ BitIndex, Integer)

            If mnAck = CType(msLastStatus, Integer) Then
                Dim sData(0) As String

                'Were done getting all the alarm table changes for devices in this zone. Tell the PLC.
                sData(0) = mnAck.ToString
                'Application.DoEvents()

                With goPLC
                    .ZoneName = goZone.Name
                    .TagName = AckTag
                    .PLCData = sData
                End With

            End If 'mnAck = CType(msLastStatus, Integer)

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subUpdateAck", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub subAlarmNotification(ByVal Alarm As clsPLCAlarm)
        '********************************************************************************************
        'Description: New alarm received from a device. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent ZoneAlarmNotify(Alarm)

    End Sub

    Private Sub subBumpProgress()
        '********************************************************************************************
        'Description: Pass on the Progress
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent BumpProgress()

    End Sub

    Friend Sub New(ByVal ZoneData As XmlNode, ByRef Zones As clsZones, _
                ByRef Parent As clsPLCAlarmZones)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters:  Zone Datarow, Database Path, Reference to Parent class
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '5/9/08     gks     Add DB
        '*****************************************************************************************

        MyBase.New()
        goPLC = Parent.goPLC
        AddHandler Parent.NewStatus, AddressOf subCheckStatus
        Call subInitCollection(ZoneData, Zones)

    End Sub

#End Region

End Class

'********************************************************************************************
'Description: PLC Alarm Device object
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************

Friend Class clsPLCAlarmDevice

#Region " Declares "

    '***** Module Constants *************************************************************************
    Private Const msMODULE As String = "clsPLCAlarmDevice"
    Private Const mnASSOC_DATA_READ_INTERVAL As Long = 10000000 'Ticks (10K ticks per ms)
    '***** End Module Constants *********************************************************************

    '***** Module Structures ************************************************************************
    Private Structure udsAssocData
        Friend Enabled As Boolean
        Friend Ascii As Boolean
        Friend Length As Integer
        Friend Eq1Offset As Integer
        Friend Eq2Offset As Integer
        Friend Eq3Offset As Integer
        Friend Eq4Offset As Integer
    End Structure

    Private Enum eAlarmText
        Description = 0
        Severity = 1
        CauseMnemonic = 2
    End Enum
    '***** End Module Structures ********************************************************************

    '************ Property Variables ****************************************************************
    Private mnBitIndex As Integer = 0
    Private mnEquipment As Integer = 0
    Private mnLength As Integer = 0
    Private msDeviceName As String = String.Empty
    Private msFacilityName As String = String.Empty
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    '************ End Property Variables ************************************************************

    '******** Event Variables ***********************************************************************
    Friend Event DeviceAlarmNotify(ByVal Alarm As clsPLCAlarm)
    Friend Event BumpProgress()
    Friend Event GetAlarmsCompleted(ByVal BitIndex As Integer)
    '******** End Event Variables *******************************************************************

    '******** Module Variables **********************************************************************
    Private mProdID As udsAssocData
    Private mJobID As udsAssocData
    Private mStyle As udsAssocData
    Private mColor As udsAssocData
    Private mdtLastRead As DateTime = DateTime.Now
    Private WithEvents moDevicePLCComm As New clsPLCComm
    Private moZone As clsZone = Nothing
    Private msAlarmData() As String = Nothing
    Private msAlarmRef() As String = Nothing
    '******** End Module Variables ******************************************************************

#End Region

#Region " Properties "

    Friend ReadOnly Property AssocData() As String()
        '********************************************************************************************
        'Description:  Return an up-to-date array of this device's associated process data (ProdID, 
        '              JobId, Style, Color). Process data items that are not enabled will have a 
        '              have a value of "" (String.Empty).
        '
        'Parameters: none
        'Returns:    Associated process data array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/16/10   RJO     If for some reason the alarm table can't be read, exit stage left
        '********************************************************************************************
        Get
            Dim sAssocData(3) As String
            Dim lElapsedTime As Long = DateTime.Now.ToBinary - mdtLastRead.ToBinary
            Dim bReadEnable As Boolean = mProdID.Enabled Or mJobID.Enabled Or mStyle.Enabled Or mColor.Enabled

            'Initialize Array
            For nIndex As Integer = 0 To 3
                sAssocData(nIndex) = String.Empty
            Next

            Try
                If bReadEnable And (IsNothing(msAlarmData) Or (lElapsedTime > mnASSOC_DATA_READ_INTERVAL)) Then

                    'Read alarm Table from PLC
                    With moDevicePLCComm
                        .ZoneName = moZone.Name
                        .TagName = TagName
                        msAlarmData = .PLCData
                    End With
                    mdtLastRead = DateTime.Now
                    '>>> Debug
                    'Trace.WriteLine("Module: " & msMODULE & " - Read Zone: " & ZoneName & ", Device: " & DeviceName & ", Equip: " & Equipment.ToString & " Assoc Data from PLC.")
                    '<<< Debug
                    If msAlarmData Is Nothing Then 'RJO 04/16/10
                        mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: AssocData", _
                                               "Error: Failed to read alarm data table from PLC.")
                        Exit Property
                    End If
                End If

                'Extract the Associated Process Data
                If mProdID.Enabled Then
                    Select Case Equipment
                        Case 1
                            sAssocData(ePLCAssocData.ProdId) = GetAssocData(mProdID.Eq1Offset, mProdID.Length, mProdID.Ascii)
                        Case 2
                            sAssocData(ePLCAssocData.ProdId) = GetAssocData(mProdID.Eq2Offset, mProdID.Length, mProdID.Ascii)
                        Case 3
                            sAssocData(ePLCAssocData.ProdId) = GetAssocData(mProdID.Eq3Offset, mProdID.Length, mProdID.Ascii)
                        Case 4
                            sAssocData(ePLCAssocData.ProdId) = GetAssocData(mProdID.Eq4Offset, mProdID.Length, mProdID.Ascii)
                    End Select
                End If
                If mJobID.Enabled Then
                    Select Case Equipment
                        Case 1
                            sAssocData(ePLCAssocData.JobID) = GetAssocData(mJobID.Eq1Offset, mJobID.Length, mJobID.Ascii)
                        Case 2
                            sAssocData(ePLCAssocData.JobID) = GetAssocData(mJobID.Eq2Offset, mJobID.Length, mJobID.Ascii)
                        Case 3
                            sAssocData(ePLCAssocData.JobID) = GetAssocData(mJobID.Eq3Offset, mJobID.Length, mJobID.Ascii)
                        Case 4
                            sAssocData(ePLCAssocData.JobID) = GetAssocData(mJobID.Eq4Offset, mJobID.Length, mJobID.Ascii)
                    End Select
                End If
                If mStyle.Enabled Then
                    Select Case Equipment
                        Case 1
                            sAssocData(ePLCAssocData.Style) = GetAssocData(mStyle.Eq1Offset, mStyle.Length, mStyle.Ascii)
                        Case 2
                            sAssocData(ePLCAssocData.Style) = GetAssocData(mStyle.Eq2Offset, mStyle.Length, mStyle.Ascii)
                        Case 3
                            sAssocData(ePLCAssocData.Style) = GetAssocData(mStyle.Eq3Offset, mStyle.Length, mStyle.Ascii)
                        Case 4
                            sAssocData(ePLCAssocData.Style) = GetAssocData(mStyle.Eq4Offset, mStyle.Length, mStyle.Ascii)
                    End Select
                End If
                If mColor.Enabled Then
                    Select Case Equipment
                        Case 1
                            sAssocData(ePLCAssocData.Color) = GetAssocData(mColor.Eq1Offset, mColor.Length, mColor.Ascii)
                        Case 2
                            sAssocData(ePLCAssocData.Color) = GetAssocData(mColor.Eq2Offset, mColor.Length, mColor.Ascii)
                        Case 3
                            sAssocData(ePLCAssocData.Color) = GetAssocData(mColor.Eq3Offset, mColor.Length, mColor.Ascii)
                        Case 4
                            sAssocData(ePLCAssocData.Color) = GetAssocData(mColor.Eq4Offset, mColor.Length, mColor.Ascii)
                    End Select
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: AssocData", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Finally
                AssocData = sAssocData
            End Try

        End Get

    End Property

    Friend ReadOnly Property BitIndex() As Integer
        '********************************************************************************************
        'Description:  Get the Bit Number in the Alarm Status word that corresponds to this device.
        '
        'Parameters: none
        'Returns:    BitIndex
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnBitIndex
        End Get

    End Property

    Friend Property Equipment() As Integer
        '********************************************************************************************
        'Description: The Equipment (Arm) number we'll be reading associated data for  
        '
        'Parameters: none
        'Returns:    Equipment (Arm) number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnEquipment
        End Get
        Set(ByVal value As Integer)
            mnEquipment = value
        End Set

    End Property

    Friend ReadOnly Property FacilityName() As String
        '********************************************************************************************
        'Description:  Get the Facility Code that corresponds to this device. (ex. "PLCZ", "PLCR")
        '
        'Parameters: none
        'Returns:    Facility Code
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msFacilityName
        End Get

    End Property

    Private ReadOnly Property Length() As Integer
        '********************************************************************************************
        'Description:  Get the length of the Alarm Bit Table in datatable words
        '
        'Parameters: none
        'Returns:    Length
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnLength
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
        'Description:  Get the Tag name assigned to the alarm data table address for this device.
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

    Private ReadOnly Property ZoneName() As String
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

#End Region

#Region " Routines "

    Private Function BuildAlarm(ByVal AlarmNumber As Integer, ByVal Active As Boolean) As clsPLCAlarm
        '********************************************************************************************
        'Description: Populate and return an clsPLCAlarm object. 
        '
        'Parameters: AlarmNumber - PLC Alarm Number, Active (True=Active, False=Reset)
        'Returns:    None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/16/08  BTK     Added code to log the zone number for plc generated zone alarms.
        ' 11/18/10  AM      Commented out Culture to be force to Default culture and should use Display Culture
        '********************************************************************************************
        Try
            Dim oAlarm As New clsPLCAlarm
            'Dim Culture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(frmMain.gsDefaultCulture)
            Dim Culture As System.Globalization.CultureInfo = frmMain.DisplayCulture
            Dim sDelim As String = frmMain.gAlarmsRM.GetString("Delimiter", Culture)
            Dim sEquip As String = frmMain.gAlarmsRM.GetString("Equipment", Culture).ToUpper
            Dim nEquip As Integer = 0
            Dim sAlarmText() As String = Split(frmMain.gAlarmsRM.GetString(FacilityName & "_" & Strings.Format(AlarmNumber, "000"), Culture), sDelim)
            Dim nElements As Integer = sAlarmText.GetUpperBound(0)

            With oAlarm
                .AlarmNumber = FacilityName & "-" & Strings.Format(AlarmNumber, "000")
                .Device = DeviceName
                If (nElements = 0) And (sAlarmText(0).Length = 0) Then
                    .Description = gpsRM.GetString("psRESOURCE_ERR", Culture) & " [" & FacilityName & "_" & Strings.Format(AlarmNumber, "000") & "]"
                    .Severity = "WARNING"
                Else
                    .Description = sAlarmText(eAlarmText.Description)
                    If nElements >= eAlarmText.Severity Then .Severity = sAlarmText(eAlarmText.Severity)
                    If nElements >= eAlarmText.CauseMnemonic Then .CauseMnemonic = sAlarmText(eAlarmText.CauseMnemonic)
                End If
                .StartSerial = DateTime.Now
                .EndSerial = DateTime.Now

                'Determine the equipment number (if any)
                Dim sTemp As String = Strings.Left(.Description, Strings.Len(sEquip)).ToUpper


                If sTemp = sEquip Then
                    sTemp = Strings.Mid(.Description, Strings.Len(sEquip) + 1, 1)
                    If IsNumeric(sTemp) Then nEquip = CType(sTemp, Integer)
                End If
                Equipment = nEquip


                If mProdID.Enabled Then
                    Select Case Equipment
                        Case 1
                            .ProdID = GetAssocData(mProdID.Eq1Offset, mProdID.Length, mProdID.Ascii)
                        Case 2
                            .ProdID = GetAssocData(mProdID.Eq2Offset, mProdID.Length, mProdID.Ascii)
                        Case 3
                            .ProdID = GetAssocData(mProdID.Eq3Offset, mProdID.Length, mProdID.Ascii)
                        Case 4
                            .ProdID = GetAssocData(mProdID.Eq4Offset, mProdID.Length, mProdID.Ascii)
                    End Select
                End If
                If mJobID.Enabled Then
                    Select Case Equipment
                        Case 1
                            .JobID = GetAssocData(mJobID.Eq1Offset, mJobID.Length, mJobID.Ascii)
                        Case 2
                            .JobID = GetAssocData(mJobID.Eq2Offset, mJobID.Length, mJobID.Ascii)
                        Case 3
                            .JobID = GetAssocData(mJobID.Eq3Offset, mJobID.Length, mJobID.Ascii)
                        Case 4
                            .JobID = GetAssocData(mJobID.Eq4Offset, mJobID.Length, mJobID.Ascii)
                    End Select
                End If
                If mStyle.Enabled Then
                    Select Case Equipment
                        Case 1
                            .StyleNumber = GetAssocData(mStyle.Eq1Offset, mStyle.Length, mStyle.Ascii)
                        Case 2
                            .StyleNumber = GetAssocData(mStyle.Eq2Offset, mStyle.Length, mStyle.Ascii)
                        Case 3
                            .StyleNumber = GetAssocData(mStyle.Eq3Offset, mStyle.Length, mStyle.Ascii)
                        Case 4
                            .StyleNumber = GetAssocData(mStyle.Eq4Offset, mStyle.Length, mStyle.Ascii)
                    End Select
                End If
                If mColor.Enabled Then
                    Select Case Equipment
                        Case 1
                            .ColorNumber = GetAssocData(mColor.Eq1Offset, mColor.Length, mColor.Ascii)
                        Case 2
                            .ColorNumber = GetAssocData(mColor.Eq2Offset, mColor.Length, mColor.Ascii)
                        Case 3
                            .ColorNumber = GetAssocData(mColor.Eq3Offset, mColor.Length, mColor.Ascii)
                        Case 4
                            .ColorNumber = GetAssocData(mColor.Eq4Offset, mColor.Length, mColor.Ascii)
                    End Select
                End If
                .Category = FacilityName
                If Active Then
                    .Status = "Active"
                Else
                    .Status = "Reset"
                End If
                'BTK 01/16/08 Added code to log the zone number for plc generated zone alarms.
                .Zone = moZone.ZoneNumber
            End With 'oAlarm

            Return oAlarm

        Catch ex As Exception
            mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: BuildAlarm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return Nothing
        End Try

    End Function

    Private Function BuildAsciiString(ByVal Data() As Integer) As String
        '********************************************************************************************
        'Description: Converts an array of integers containing 2 characters each (bits 8-15 = left
        '             character, Bits 0-7 = right character) to a string. 
        '
        'Parameters: Data - Array of ascii data expressed as integers
        'Returns:    String 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nElement As Integer = 0
            Dim sTemp As String = String.Empty

            For nIndex As Integer = 0 To Data.GetUpperBound(0)
                'Get the left character
                nElement = Data(nIndex) And &HFF0000
                nElement = nElement \ 65536
                If CheckChar(nElement) Then
                    sTemp = sTemp & Chr(nElement)
                End If

                nElement = Data(nIndex) And &HFF00
                nElement = nElement \ 256
                If CheckChar(nElement) Then
                    sTemp = sTemp & Chr(nElement)
                End If
                'Get the right character
                nElement = Data(nIndex) And &HFF
                If CheckChar(nElement) Then
                    sTemp = sTemp & Chr(nElement)
                End If
            Next

            Return Strings.Trim(sTemp)

        Catch ex As Exception
            mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: BuildAsciiString", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return String.Empty
        End Try

    End Function

    Private Function CheckChar(ByVal CharCode As Integer) As Boolean
        '********************************************************************************************
        'Description: Returns true if the supplied char code is a Space, Number, UC alpha, LC Alpha,
        '             or special character.
        '
        'Parameters: Data - Array of ascii data expressed as integers
        'Returns:    String 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            CheckChar = (CharCode > 31) And (CharCode < 127)
        Catch ex As Exception
            mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: CheckChar", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function

    Private Sub subGetAlarms(ByVal Index As Integer)
        '********************************************************************************************
        'Description: When a Device sets it's status flag to indicate the Alarm Bit Table has changed, 
        '             the collection class calls this sub to determine what changed and generate
        '             new alarms. When this routine completes, it returns the BitIndex of the device.
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/16/10   RJO     If for some reason the alarm table can't be read, exit stage left
        '********************************************************************************************
        If Index = BitIndex Then

            Try
                'Read alarm Table from PLC
                With moDevicePLCComm
                    .ZoneName = moZone.Name
                    .TagName = TagName
                    msAlarmData = .PLCData
                End With

                If msAlarmData Is Nothing Then 'RJO 04/16/10
                    mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: AssocData", _
                                           "Error: Failed to read alarm data table from PLC.")
                    Exit Sub
                End If

                If msAlarmRef Is Nothing Then
                    'This is the initial read. Size this array to match msAlarmData and fill it with "0"s.
                    ReDim msAlarmRef(msAlarmData.GetUpperBound(0))
                    For Each sItem As String In msAlarmRef
                        sItem = "0"
                    Next
                End If

                'Find words that changed in the Alarm data table
                For nIndex As Integer = 0 To Length - 1
                    If msAlarmData(nIndex) <> msAlarmRef(nIndex) Then
                        'For each bit that changed in the alarm data word
                        '   Populate an ActiveAlarm object
                        '   Raise an AlarmNotification event
                        Call subSendAlarms(nIndex)
                    End If
                Next

                'Update the last state array
                msAlarmRef = msAlarmData

            Catch ex As Exception
                mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: DoAlarms", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Finally
                'Let the parent collection know that all new alarms from this device have been sent.
                RaiseEvent GetAlarmsCompleted(Index)
            End Try

        End If 'Index = BitIndex

    End Sub


    Private Function GetAssocData(ByVal Offset As Integer, ByVal Length As Integer, ByVal Ascii As Boolean) As String
        '********************************************************************************************
        'Description: Return the Associated Data at the specified location in the Alarm Data Table
        '             in the specified format. 
        '
        'Parameters: Offset - Start Word in Data Table
        '            Length - Number of words in data table
        '            Ascii - (True=Ascii format, False=Number)
        'Returns:    Associated Data 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            If Length = 0 Then
                mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: GetAssocData", _
                                       "Error: Data Length = 0.")
                Return String.Empty
            End If

            If msAlarmData Is Nothing Then Return String.Empty '7/11/08 gks

            If msAlarmData.GetUpperBound(0) >= Offset + Length - 1 Then
                Dim sAssocData As String = String.Empty

                If Ascii Then
                    Dim nData(Length - 1) As Integer
                    For nIndex As Integer = 0 To Length - 1
                        nData(nIndex) = CType(msAlarmData(nIndex + Offset), Integer)
                    Next
                    If nData(0) > 0 Then
                        Debug.Print(nData(0).ToString)
                    End If
                    sAssocData = BuildAsciiString(nData)
                Else
                    'This is a number so if there's more than one data word, we'll concatinate them
                    For nIndex As Integer = Offset To Offset + Length - 1
                        sAssocData = sAssocData & msAlarmData(nIndex)
                    Next
                End If 'Ascii

                Return sAssocData
            Else
                mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: GetAssocData", _
                                       "Error: The specified data location is outside of the boundaries of the Alarm Data Table.")
                Return String.Empty
            End If 'msAlarmData.GetUpperBound(0) >= Offset + Length

        Catch ex As Exception
            mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: GetAssocData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return String.Empty
        End Try

    End Function

    Private Sub subInitialize(ByRef oDevice As XmlNode)
        '********************************************************************************************
        'Description: When a new PLC Alarm Device is created, its info is passed in in the form of a 
        '             datarow from the PLC Alarm Devices table in the configuration database.
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/05/12  MSW     Move SQL to XML
        '********************************************************************************************

        Try
            'Get the data that defines the dimensions and location of the alarm data table for this device.
            With oDevice
                msDeviceName = .Item("DeviceName").InnerText
                msZoneName = .Item("ZoneName").InnerText
                msFacilityName = .Item("FacilityName").InnerText
                mnBitIndex = CType(.Item("BitIndex").InnerText, Integer)
                msTagName = .Item("Tag").InnerText
                mnLength = CType(.Item("Length").InnerText, Integer)
                mProdID.Enabled = CType(.Item("UseProdID").InnerText, Boolean)
                mProdID.Ascii = CType(.Item("AsciiProdID").InnerText, Boolean)
                mProdID.Length = CType(.Item("ProdIDLength").InnerText, Integer)
                mProdID.Eq1Offset = CType(.Item("Eq1ProdIDOffset").InnerText, Integer)
                mProdID.Eq2Offset = CType(.Item("Eq2ProdIDOffset").InnerText, Integer)
                mProdID.Eq3Offset = CType(.Item("Eq3ProdIDOffset").InnerText, Integer)
                mProdID.Eq4Offset = CType(.Item("Eq4ProdIDOffset").InnerText, Integer)
                mJobID.Enabled = CType(.Item("UseJobID").InnerText, Boolean)
                mJobID.Ascii = CType(.Item("AsciiJobID").InnerText, Boolean)
                mJobID.Length = CType(.Item("JobIDLength").InnerText, Integer)
                mJobID.Eq1Offset = CType(.Item("Eq1JobIDOffset").InnerText, Integer)
                mJobID.Eq2Offset = CType(.Item("Eq2JobIDOffset").InnerText, Integer)
                mJobID.Eq3Offset = CType(.Item("Eq3JobIDOffset").InnerText, Integer)
                mJobID.Eq4Offset = CType(.Item("Eq4JobIDOffset").InnerText, Integer)
                mStyle.Enabled = CType(.Item("UseStyle").InnerText, Boolean)
                mStyle.Ascii = CType(.Item("AsciiStyle").InnerText, Boolean)
                mStyle.Length = CType(.Item("StyleLength").InnerText, Integer)
                mStyle.Eq1Offset = CType(.Item("Eq1StyleOffset").InnerText, Integer)
                mStyle.Eq2Offset = CType(.Item("Eq2StyleOffset").InnerText, Integer)
                mStyle.Eq3Offset = CType(.Item("Eq3StyleOffset").InnerText, Integer)
                mStyle.Eq4Offset = CType(.Item("Eq4StyleOffset").InnerText, Integer)
                mColor.Enabled = CType(.Item("UseColor").InnerText, Boolean)
                mColor.Ascii = CType(.Item("AsciiColor").InnerText, Boolean)
                mColor.Length = CType(.Item("ColorLength").InnerText, Integer)
                mColor.Eq1Offset = CType(.Item("Eq1ColorOffset").InnerText, Integer)
                mColor.Eq2Offset = CType(.Item("Eq2ColorOffset").InnerText, Integer)
                mColor.Eq3Offset = CType(.Item("Eq3ColorOffset").InnerText, Integer)
                mColor.Eq4Offset = CType(.Item("Eq4ColorOffset").InnerText, Integer)
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: subInitialize", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subSendAlarms(ByVal Index As Integer)
        '********************************************************************************************
        'Description: Compare the AlarmData word to the AlarmRef word. Generate a new alarm for each
        '             bit that differs between the two.
        '
        'Parameters: Index - Word number in data table
        'Returns:    None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '10/08/08   gks     Add 32 bit
        '********************************************************************************************
        Try
            Dim nActive As Long = CType(msAlarmData(Index), Long)
            Dim nRef As Long = CType(msAlarmRef(Index), Long)
            Dim nDif As Long = nActive Xor nRef
            Dim nOffset As Integer = Index * frmMain.gnWordHiBit '16

            'Convert 16 bit Signed to Unsigned  if necessary
            If nActive < 0 Then
                nActive += frmMain.gnWordHiBitVal '65536
                msAlarmData(Index) = nActive.ToString
            End If

            For nBit As Integer = 0 To frmMain.gnWordHiBit - 1 '15
                Dim nMask As Long = CType(2 ^ nBit, Long)

                If (nDif And nMask) > 0 Then
                    'We found one
                    Dim bActive As Boolean = (nActive And nMask) > 0
                    Dim oAlarm As clsPLCAlarm = BuildAlarm(nBit + nOffset, bActive)

                    If Not (oAlarm Is Nothing) Then RaiseEvent DeviceAlarmNotify(oAlarm)
                    'My.Application.DoEvents()
                End If
            Next 'nBit

        Catch ex As Exception
            mDebug.WriteEventToLog("Zone: " & ZoneName & ", Device: " & DeviceName & "Module: " & msMODULE & ", Routine: subSendAlarms", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Sub


#End Region

#Region " Events "

    Friend Sub New(ByVal Device As XmlNode, ByVal Parent As clsPLCAlarmDevices)

        '********************************************************************************************
        'Description: When a new controller is requested, its info is passed in in the form of a 
        '             datarow from the database.
        '
        'Parameters: datarow, Reference to PLC Comm class, Reference to the Parent collection
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/05/12  MSW     Move SQL to XML
        '********************************************************************************************

        MyBase.New()
        'moDevicePLCComm = Parent.goPLC
        moZone = Parent.goZone
        AddHandler Parent.GetAlarms, AddressOf subGetAlarms
        subInitialize(Device)

    End Sub


    Private Sub moDevicePLCComm_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, ByVal sModule As String, ByVal AdditionalInfo As String) Handles moDevicePLCComm.ModuleError
        '********************************************************************************************
        'Description: An error has occurred in the frmPLCComm that is used to read alarm and 
        ' associared data from the PLC for this device.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/10  RJO     When moDevicePLCComm goes out into la la land, it never recovers.
        '                   Added code to attempt to recover.
        ' 07/27/12  JBW     Bug Fix to exception recovery.
        ' 11/02/12  MSW     moDevicePLCComm_ModuleError - Move redim outside of for loop
        '********************************************************************************************
        Dim sMsg As String = nErrNum.ToString & "-" & sErrDesc & " occurred in  Module " & sModule & "."

        'moDevicePLCComm = Nothing 'RJO 04/16/10

        If AdditionalInfo <> String.Empty Then
            sMsg = sMsg & " Additional Info: " & AdditionalInfo
        End If

        mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: moDevicePLCComm_ModuleError", _
                               "Error: " & sMsg)

        'Try to recover from exception gracefully 'RJO 04/16/10
        If (msAlarmData Is Nothing) And Not (msAlarmRef Is Nothing) Then
            'JBW if we do this without redimensioning first it will crash AlarmMan.
            ReDim msAlarmData(msAlarmRef.GetUpperBound(0))
            For nIndex As Integer = 0 To msAlarmRef.GetUpperBound(0)
                msAlarmData(nIndex) = msAlarmRef(nIndex)
            Next
        End If

        'moDevicePLCComm = New clsPLCComm

    End Sub

#End Region

End Class

'********************************************************************************************
'Description: Active Alarm Data Class
'
'
'Modification history:
'
' Date      By      Reason
'*******************************************************************************************

Friend Class clsPLCAlarm
    Friend AlarmNumber As String = String.Empty     'Alarm# field <FacilityName>-<AlarmNumber>
    Friend Device As String = String.Empty          'Device field
    Friend Description As String = String.Empty     'Description field
    Friend Severity As String = String.Empty        'Severity field
    Friend CauseMnemonic As String = String.Empty   'Some robot alarms Have an associated Cause Alarm, otherwise ""
    Friend StartSerial As DateTime = DateTime.Now   'Start Serial field
    Friend EndSerial As DateTime = DateTime.Now     'End Serial field
    Friend Zone As Integer = 0                      'Zone field
    Friend ProdID As String = String.Empty          'Prod ID field (VIN Number  or Carrier Number)
    Friend JobID As String = String.Empty           'Job ID field (Sequence Number)
    Friend StyleNumber As String = String.Empty     'Style Number field <Plant Style Code>-<Description>
    Friend ColorNumber As String = String.Empty     'Color Number field <Plant Color Code>-<Description>
    Friend ValveNumber As String = String.Empty     'Valve Number field <Color Valve Number>-<Description>
    Friend Category As String = String.Empty        'Category for R&M = Facility Name (ex. SRVO, MOTN, etc.)
    Friend DowntimeFlag As Boolean = False          'True = This alarm causes downtime
    Friend ErrorFacility As Integer = -1            'Facility Code - Used to identify Robot RESET and PLC alarms
    Friend Status As String = String.Empty          '"Active" or "Reset" - Used to determine when the alarm should be written to the log
End Class