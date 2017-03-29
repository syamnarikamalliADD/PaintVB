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
' Form/Module: clsASMBTCPComm
'
' Description: Class to use ASMBTCP ActiveX to talk to a Modicon PLC
' 
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick O.
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'********************************************************************************************
'    07/27/2007 RJO     Initial Code for ASABTCP ActiveX (Allen Bradley)
'    10/08/2013 DE      Adapted to ASMPTCP ActiveX (Modicon/Modbus)                4.01.01.00
'    10/09/2012 RJO     Cleanup                                                    4.01.01.01
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Friend Class clsPLCComm

#Region "  Declares  "

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "ASMBTCPComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsASMBTCPComm"
    Private Const mnTIMEOUT As Integer = 1001 'Hotlink Timeout period in milliseconds
    Private Const mnMANUAL_POLL_RATE As Integer = 5000 'A poll rate of this value specifies a One-Time-Read vs. a HotLink
    Private Const mnMAX_RETRY_COUNT As Integer = 3 'Maximum Onetime read retries
    Private Const mnREFRESH_COUNT As Integer = 10 'after this many same data polled reads, send data as new data anyway
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemoteComputer As String = String.Empty
    Private msReadData() As String
    Private mbRead As Boolean '= False
    Private mPLCType As ePLCType = ePLCType.Modicon 'Default to Modicon
    '***** End Property Vars *******************************************************************

    '******* Events ****************************************************************************
    'when hotlink has new data 
    Friend Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                         ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events ************************************************************************

    '***** Working Vars ************************************************************************
    Private mbOTRWInProg As Boolean ' = False
    Private mLinks As New clsLinks
    Private mnHotLinks As Integer ' = 0 'How many HotLinks have been created.
    Private mnASMBTCPReturnCode As Integer ' = 0
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************
    Friend Enum ePLCType
        Modicon = 10
    End Enum
    '***** End Enums ***************************************************************************

#End Region

#Region " Properties "

    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for ASMBTCP. Maintained for compatibility
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
        'Description:  ASMBTCP.ocx is capable of reading/writing data to Modicon PLCs only.
        '              Note: Not used for ASMBTCP. Maintained for compatibility.
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
        Dim sHotLinks(0) As String
        Dim nIndex As Integer = -1

        Try

            For Each oLink As clsLink In mLinks
                If (oLink.PollRate <> mnMANUAL_POLL_RATE) Then
                    nIndex += 1
                    ReDim Preserve sHotLinks(nIndex)
                    sHotLinks(nIndex) = oLink.ControlName

                    For Each oControl As Control In Me.Controls
                        If oControl.Name = sHotLinks(nIndex) Then
                            Dim oAsmbtcp As AxASMBTCPLib.AxASMBTCP = DirectCast(oControl, AxASMBTCPLib.AxASMBTCP)

                            oAsmbtcp.Disconnect()
                            RemoveHandler oAsmbtcp.Complete, AddressOf AxAsmbtcp_Complete
                            RemoveHandler oAsmbtcp.UnsolicitedMessage, AddressOf AxAsmbtcp_UnsolicitedMessage
                            oAsmbtcp.Dispose()
                        End If
                    Next 'oControl

                End If '(oLink.PollRate <> mnMANUAL_POLL_RATE)
            Next 'oLink

            If nIndex >= 0 Then
                For Each sControlName As String In sHotLinks
                    Dim oLink As clsLink = mLinks.Item(sControlName)

                    mLinks.Remove(oLink)
                Next 'sControlName
            End If 'nIndex

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
        Dim sControlName As String = String.Empty
        Dim oLink As clsLink = Nothing

        Try

            For Each oLink In mLinks
                If (oLink.TagName = Tag) And (oLink.ZoneName = Zone.Name) Then
                    sControlName = oLink.ControlName
                    Exit For
                End If
            Next 'oLink

            If sControlName <> String.Empty Then
                For Each oControl As Control In Me.Controls
                    If oControl.Name = sControlName Then
                        Dim oAsmbtcp As AxASMBTCPLib.AxASMBTCP = DirectCast(oControl, AxASMBTCPLib.AxASMBTCP)
                        Debug.Print("Link Removed==" & oControl.Name)
                        oAsmbtcp.Disconnect()
                        RemoveHandler oAsmbtcp.Complete, AddressOf AxAsmbtcp_Complete
                        RemoveHandler oAsmbtcp.UnsolicitedMessage, AddressOf AxAsmbtcp_UnsolicitedMessage
                        oAsmbtcp.Dispose()
                        Exit For
                    End If
                Next 'oControl

                mLinks.Remove(oLink)
                Application.DoEvents()
            End If 'sControlName <> String.Empty

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subConfigControl(ByRef Control As AxASMBTCPLib.AxASMBTCP, ByVal Link As clsLink)
        '********************************************************************************************
        'Description:  Set the Asmbtcp control properties required for all read/write operations.
        '
        'Parameters: Asmbtcp Control, Link Info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            With Control
                .AutoPollEnabled = False
                .NodeAddress = Link.IP_Address & ",," & Link.Slot
                .MemQty = Link.Length
                .MemStart = Link.Address
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subConfigControl", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subConfigControl", String.Empty)
        End Try

    End Sub

    Private Sub subCreateHotLink(ByRef Link As clsLink)
        '********************************************************************************************
        'Description:  Create a new Asmbtcp control to HotLink to the new address.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim oControl As New AxASMBTCPLib.AxASMBTCP

            mnHotLinks += 1
            If mnHotLinks = 1 Then tmrHotLink.Enabled = True

            oControl.Name = "AxAsmbtcp" & mnHotLinks.ToString(CultureInfo.InvariantCulture)
            oControl.BeginInit()
            Me.Controls.Add(oControl)
            oControl.EndInit()

            Link.ControlName = oControl.Name
            mLinks.Add(Link)

            AddHandler oControl.Complete, AddressOf AxAsmbtcp_Complete
            AddHandler oControl.UnsolicitedMessage, AddressOf AxAsmbtcp_UnsolicitedMessage
            Call subConfigControl(oControl, Link)

            With oControl
                .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_READ_HOLDING_REGISTERS
                .AutoPollInterval = Link.PollRate
                .TimeoutTrans = 250

                If Link.PollRate > 0 Then
                    .AutoPollEnabled = True
                Else

                    .AutoPollEnabled = False
                    .SlaveMemStart = Link.Slot
                    .SlaveEnabled = True
                End If

                .AsyncRefresh()

                Do While .Busy
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

                mLinks.Item(oControl.Name).Busy = True

            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subCreateHotLink", String.Empty)
        End Try

    End Sub

    Private Sub subUpdateManualLink(ByVal Link As clsLink)
        '********************************************************************************************
        'Description:  Make sure the AxAsmbtcp0 link is in the collection. Update it with new tag
        '              data if it is, otherwise add it to the collection.
        '
        'Parameters: Link - Link info to update the collection
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Link.ControlName = "AxAsmbtcp0"

            If IsNothing(mLinks.Item("AxAsmbtcp0")) Then
                Call mLinks.Add(Link)
            Else
                With mLinks.Item(Link.ControlName)
                    .Address = Link.Address
                    .Bit = Link.Bit
                    .IP_Address = Link.IP_Address
                    .Length = Link.Length
                    .PollRate = Link.PollRate
                    .Slot = Link.Slot
                    .Type = Link.Type
                    .ZoneName = Link.ZoneName
                End With
            End If 'IsNothing(mLinks.Item(Link.ControlName))

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subUpdateManualLink", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, ex.Message, msMODULE & ":clsPLCComm:subUpdateManualLink", String.Empty)
        End Try

    End Sub

#End Region

#Region " Functions "

    Private Function ControlFromName(ByVal Name As String) As AxASMBTCPLib.AxASMBTCP
        '********************************************************************************************
        'Description:  Return a reference to the named AxAsmbtcp control.
        '
        'Parameters: Name - AxAsmbtcp Control Name
        'Returns:    The control
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oAxAsmbtcp As AxASMBTCPLib.AxASMBTCP = Nothing

        Try

            For Each oControl As Control In Me.Controls
                If oControl.Name = Name Then
                    oAxAsmbtcp = DirectCast(oControl, AxASMBTCPLib.AxASMBTCP)
                    Exit For
                End If
            Next 'oControl

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ControlFromName", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, ex.Message, msMODULE & ":clsPLCComm:ControlFromName", String.Empty)
        End Try

        Return oAxAsmbtcp

    End Function

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
        'assuming all 0.1 timebase
        Dim fTmp As Single = CSng(ValueIn / 10)
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
        'assuming all 0.1 timebase
        Dim nTmp As Integer = ValueIn * 10
        Return nTmp.ToString

    End Function

    Private Function OneTimeRead(Optional ByVal bIsBool As Boolean = False) As Boolean
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
        Dim bSuccess As Boolean = False

        Try
            mbOTRWInProg = True

            With AxASMBTCP0

                If bIsBool = False Then
                    .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_READ_HOLDING_REGISTERS
                Else
                    .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_READ_COIL_STATUS
                End If

                mnASMBTCPReturnCode = 0
                .AsyncRefresh()

                Do While .Busy
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

                tmrManRW.Enabled = True
                Do Until mbOTRWInProg = False
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop
                tmrManRW.Enabled = False

                If mnASMBTCPReturnCode = 0 Then
                    bSuccess = True
                End If
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:OneTimeRead", ex.Message)
            Return False
        End Try

        Return bSuccess

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
        '********************************************************************************************
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
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csINVALID_XPATH_SYNTAX") & _
                                   " [" & sPath & "]", msMODULE & ":clsPLCComm:PLCTagInfo", ex.Message)
            Return Nothing
        End Try

        If oNodeList.Count = 0 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Tag [" & TagName & "] not found.")
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csPLC_TAG_NOT_DEFINED") & _
                                   " [" & TagName & "]", msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
            Return Nothing
        Else
            oNode = oNodeList(0) 'Should only be one match!!!
            sTopic = oNode.Item("topic").InnerXml

            With oLink
                .TagName = TagName
                .ZoneName = ZoneName
                .Address = oNode.Item("addr").InnerXml
                Dim sBit As String = TryCast(oNode.Item("bit").InnerXml, String)
                If IsNothing(sBit) = False Then
                    If sBit <> String.Empty Then
                        .Bit = CType(oNode.Item("bit").InnerXml, Short)
                    End If
                End If
                .Length = CType(oNode.Item("length").InnerXml, Short)
                .Type = oNode.Item("type").InnerXml

                Dim sRef(.Length - 1) As String
                For nIndex As Integer = 0 To .Length - 1
                    ' default to 0 so it doesn't crash
                    sRef(nIndex) = "0"
                Next
                .RefData = sRef
            End With

            'Make sure Zonename doesn't include any spaces. If it does, replace them with "_" (underbars)
            sPath = "//" & Strings.Replace(ZoneName, " ", "_") & "[id='" & sTopic & "']"
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                         "(" & oNodeList.Count.ToString(CultureInfo.InvariantCulture) & _
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
                           "(" & oNodeList.Count.ToString(CultureInfo.InvariantCulture) & _
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
        Dim bIsBool As Boolean = False

        Try

            If Not IsNothing(Link) Then
                'determine if this is a One-Timer or a HotLink
                If Link.PollRate = mnMANUAL_POLL_RATE Then
                    Dim bSuccess As Boolean = False
                    Dim nRetryCount As Integer = 0

                    'One-Time Read
                    Call subUpdateManualLink(Link)
                    Call subConfigControl(AxAsmbtcp0, Link)

                    For nRetryCount = 0 To mnMAX_RETRY_COUNT
                        'Make sure the control is not busy
                        Do While AxAsmbtcp0.Busy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        If Link.Type = "Bool" Then
                            bIsBool = True
                        Else
                            bIsBool = False
                        End If
                        If OneTimeRead(bIsBool) Then
                            Data = msReadData
                            bSuccess = True
                            Exit For
                        End If 'OneTimeRead()
                        AxASMBTCP0.Disconnect()
                    Next
                    If nRetryCount > 0 Then mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", "Retry count = " & nRetryCount.ToString)
                    Return bSuccess
                Else
                    'HotLink
                    Call subCreateHotLink(Link)
                    'Return data instead of nothing
                    Application.DoEvents()
                    Application.DoEvents()
                    Data = Link.RefData
                    Return True
                End If

            Else
                Return False
            End If 'Not IsNothing(Link)

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csCOULD_NOT_READ_PLC"), msMODULE & ":clsPLCComm:ReadPLCData", String.Empty)
        End Try

    End Function

    Private Function ValidateData(ByVal sData As String(), ByVal Link As clsLink) As Boolean
        '****************************************************************************************
        'Description: This function checks for nulls and proper data type
        '
        'Parameters: array of string values
        'Returns:   True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************    
        Dim sElement As String = String.Empty

        If IsNothing(Link) Then Return False

        For Each sElement In sData
            Dim bFloat As Boolean ' = False

            'check that its numeric - this should also catch null values
            If IsNumeric(sElement) = False Then
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                       "Invalid Data, value = [" & sElement & _
                                       "], type = " & Link.Type)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                        gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                       msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                Return False
            Else
                bFloat = Strings.InStr(sElement, ".") > 0
            End If

            'check that it's the correct type for the data Type 
            Select Case Link.Type

                Case "SINT"
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False

                    Else
                        Dim nData As Short ' = 0

                        Try
                            nData = CType(sElement, Byte)
                        Catch ex As Exception
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                                   "Invalid Data, value = [" & sElement & _
                                                   "], type = " & Link.Type & _
                                                   vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                    gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                   msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                            Return False
                        End Try

                    End If 'bFloat

                Case "INT"
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False

                    Else
                        Dim nData As Short ' = 0

                        Try
                            nData = CType(sElement, Short)
                        Catch ex As Exception
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                                   "Invalid Data, value = [" & sElement & _
                                                   "], type = " & Link.Type & _
                                                   vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                    gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                   msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                            Return False
                        End Try

                    End If 'bFloat

                Case "DINT"
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End If

                Case "REAL"
                    'We already know it's numeric so pretty much anything goes...

                Case "BIT16", "BIT32", "BOOL"
                    If (sElement <> "0") And (sElement <> "1") Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End If

            End Select
        Next 'sElement

        If sData.GetLength(0) = Link.Length Then
            'data passed inspection
            Return True
        Else
            If (Link.Type = "BIT16") Or (Link.Type = "BIT32") Or (Link.Type = "BOOL") Then
                If sData.GetLength(0) = 1 Then Return True
            Else
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                            "Invalid data length = " & _
                            sData.GetLength(0).ToString(CultureInfo.InvariantCulture) & _
                            ", Link.Length = " & _
                            Link.Length.ToString(CultureInfo.InvariantCulture))
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.WrongArraySize, _
                            gcsRM.GetString("csPLC_WRITE_SIZE_ERROR"), _
                           msMODULE & ":clsPLCComm:ValidateData", "Data length = " & _
                           sData.GetLength(0).ToString(CultureInfo.InvariantCulture) & _
                           ", Link.Length = " & _
                           Link.Length.ToString(CultureInfo.InvariantCulture))
                Return False
            End If
        End If

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
        '********************************************************************************************

        If ValidateData(Data, Link) Then

            Try
                Call subConfigControl(AxAsmbtcp0, Link)
                Call subUpdateManualLink(Link)
                'This prevents using leftovers from the last read.
                'Booleans need to read the whole word, then write it back.
                'Only tested with "BOOL", but I'm guessing the others need it, too.
                OneTimeRead()

                With AxAsmbtcp0
                    If Link.Type <> "BOOL" Then
                        If Data.GetUpperBound(0) > 0 Then
                            .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_PRESET_MULTIPLE_REGISTERS
                        Else
                            .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_PRESET_SINGLE_REGISTER
                        End If
                    Else
                        .Function = ASMBTCPLib.enumAsmbtcpFunction.FUNC_MB_FORCE_SINGLE_COIL
                    End If

                    For nIndex As Short = 0 To CType(Data.GetUpperBound(0), Short)

                        Select Case Link.Type
                            Case "SINT"
                                .SetDataByteM(nIndex, CType(Data(nIndex), Short))
                            Case "INT"
                                .SetDataWordM(nIndex, CType(Data(nIndex), Short))
                            Case "DINT"
                                .SetDataLongM(nIndex, CType(Data(nIndex), Integer))
                            Case "REAL"
                                .SetDataFloatM(nIndex, CType(Data(nIndex), Single))
                            Case "BIT16", "BIT32"
                                .SetDataBitM(Link.Bit, CType(Data(0), Integer) > 0)
                            Case "BOOL" '
                                .set_DataBit(Link.Bit, CType(Data(0), Integer) > 0)
                        End Select 'Link.Type

                    Next 'nIndex

                    .AsyncRefresh()

                    Do While .Busy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    tmrManRW.Enabled = True

                End With 'AxAsmbtcp0

                Return True

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ModuleError(0, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), msMODULE & ":clsPLCComm:WritePLCData", String.Empty)
                Return False
            End Try

        Else
            Return False
        End If  'ValidateData(Data, oLink)

    End Function

#End Region

#Region " Events "

    Private Sub AxAsmbtcp_Complete(ByVal sender As Object, _
                                   ByVal e As AxASMBTCPLib._DAsmbtcpEvents_CompleteEvent) _
                                   Handles AxAsmbtcp0.Complete
        '****************************************************************************************
        'Description: The Complete event fires when the current asynchronous transaction has 
        '             completed. A Complete event will fire for each call to AsyncRefresh,  
        '             regardless of success or failure.         '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        '
        Try
            Dim oAsmbtcp As AxASMBTCPLib.AxASMBTCP = DirectCast(sender, AxASMBTCPLib.AxASMBTCP)
            Dim sReadData() As String = Nothing
            Dim nUBound As Short = 0


            If oAsmbtcp.Result = 0 Then
                With mLinks.Item(oAsmbtcp.Name)

                    .Busy = False
                    .ElapsedTime = 0
                    nUBound = CType(.Length - 1, Short)
                    'If we're reading a BIT or BOOL type, the read length is always 1 regrdless of data length (.Length)
                    If (.Type = "BIT16") Or (.Type = "BIT32") Or (.Type = "BOOL") Then nUBound = 0

                    If (oAsmbtcp.Name <> "AxAsmbtcp0") Or mbRead Then
                        ReDim sReadData(nUBound) '(.Length - 1)

                        For nIndex As Short = 0 To nUBound 'CType(.Length - 1, Short)
                            Select Case .Type
                                Case "SINT"
                                    sReadData(nIndex) = _
                                        oAsmbtcp.GetDataByteM(nIndex).ToString(CultureInfo.InvariantCulture)
                                Case "INT"
                                    sReadData(nIndex) = _
                                        oAsmbtcp.GetDataWordM(nIndex).ToString(CultureInfo.InvariantCulture)
                                Case "DINT"
                                    sReadData(nIndex) = _
                                        oAsmbtcp.GetDataLongM(nIndex).ToString(CultureInfo.InvariantCulture)
                                Case "REAL"
                                    sReadData(nIndex) = _
                                        oAsmbtcp.GetDataFloatM(nIndex).ToString(CultureInfo.InvariantCulture)
                                Case "BIT16", "BIT32", "BOOL"
                                    Dim nTmp As Integer = -CType(oAsmbtcp.GetDataBitM(.Bit), Integer)
                                    sReadData(nIndex) = nTmp.ToString(CultureInfo.InvariantCulture)
                            End Select
                        Next 'nIndex
                    End If 'mbRead

                    If oAsmbtcp.AutoPollEnabled Then 'This is a hot-link
                        Dim bNewData As Boolean = False
                        Dim sRefData() As String = .RefData

                        For nIndex As Short = 0 To nUBound '.Length - 1
                            If sReadData(nIndex) <> sRefData(nIndex) Then
                                bNewData = True
                                Exit For
                            End If
                        Next 'nIndex
                        If bNewData = False Then
                            .SameDataCount += 1
                        End If
                        If .SameDataCount >= mnREFRESH_COUNT Then
                            bNewData = True
                            .SameDataCount = 0
                        End If
                        If bNewData Then
                            .RefData = sReadData
                            RaiseEvent NewData(.ZoneName, .TagName, .RefData)
                        End If
                        .Busy = True
                        .ElapsedTime = 0
                    Else
                        'One time read, update msReadData
                        mbOTRWInProg = False
                        msReadData = sReadData
                        tmrManRW.Enabled = False
                    End If 'oAsmbtcp.AutoPollEnabled

                End With 'mLinks.Item(oAsmbtcp.Name)
            End If 'oAsmbtcp.Result = 0

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsmbtcp_Complete", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:AxAsmbtcp_Complete", ex.Message)
        End Try

    End Sub

    Private Sub AxAsmbtcp_UnsolicitedMessage(ByVal sender As Object, _
                                             ByVal e As AxASMBTCPLib._DAsmbtcpEvents_UnsolicitedMessageEvent) _
                                             Handles AxAsmbtcp0.UnsolicitedMessage
        '********************************************************************************************
        ' Description: This routine responds to an unsolicited message from the PLC
        '
        ' Parameters: Index
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim oAsmbtcp As AxASMBTCPLib.AxASMBTCP = DirectCast(sender, AxASMBTCPLib.AxASMBTCP)
            Dim oLink As clsLink = mLinks.Item(oAsmbtcp.Name)
            Dim sReadData() As String = Nothing

            With oAsmbtcp
                ReDim sReadData(.SlaveMemQty)

                For nIndex As Short = 0 To CType(.SlaveMemQty - 1, Short)
                    Select Case oLink.Type
                        Case "SINT"
                            sReadData(nIndex) = _
                                .get_DataWord(nIndex).ToString(CultureInfo.InvariantCulture)
                            Debug.Assert(False) 'Can't do this
                        Case "INT"
                            sReadData(nIndex) = _
                                .get_DataWord(nIndex).ToString(CultureInfo.InvariantCulture)
                        Case "DINT"
                            sReadData(nIndex) = _
                                .get_DataLong(nIndex).ToString(CultureInfo.InvariantCulture)
                        Case "REAL"
                            sReadData(nIndex) = _
                                .get_DataFloat(nIndex).ToString(CultureInfo.InvariantCulture)
                            'TODO - Work out the details for BIT16, BIT32, and BOOL Unsolicited Messages
                            'Case "BIT16"
                            'Case "BIT32"
                            'Case "BOOL"
                    End Select
                Next ' nIndex

                RaiseEvent NewData(oLink.ZoneName, oLink.TagName, sReadData)
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsmbtcp_UnsolicitedMessage", _
                            ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                       msMODULE & ":clsPLCComm:AxAsmbtcp_UnsolicitedMessage", ex.Message)
        End Try

    End Sub

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

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tmrHotLink.Interval = 1000 'ms
        tmrManRW.Interval = 1000 'ms 
        AxASMBTCP0.TimeoutTrans = 500 'ms 
        AxASMBTCP0.TimeoutConnect = 500 'ms

    End Sub

    Private Sub tmrHotlink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHotLink.Tick
        '********************************************************************************************
        ' Description:  This routine allows the program to error out when a HotLink read operation
        '               takes too long.
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrHotLink.Enabled = False

        Try

            For Each oLink As clsLink In mLinks

                If oLink.ControlName <> "AxAsmbtcp0" Then
                    If oLink.Busy Then
                        Dim oAsmbtcp As AxASMBTCPLib.AxASMBTCP = ControlFromName(oLink.ControlName)

                        If oAsmbtcp.Busy Then
                            oLink.ElapsedTime += tmrHotLink.Interval
                            If oLink.ElapsedTime > mnTIMEOUT Then
                                'communication breakdown (it drives me insane) - fire the error event once
                                If (oAsmbtcp.Result <> 0) And (oAsmbtcp.Result <> oLink.ReturnCode) Then
                                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotlink_Tick", oLink.ControlName & _
                                                           " hotlink comm timeout. Tag = " & oLink.TagName & _
                                                           ", Address = " & oLink.Address & _
                                                           ", IP Address = " & oLink.IP_Address & "." & vbCrLf & _
                                                           "Error Code: " & AxAsmbtcp0.Result.ToString & " - " & oAsmbtcp.ResultString)
                                    RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                                           msMODULE & ":clsPLCComm:tmrHotlink_Tick", oAsmbtcp.ResultString)
                                End If
                                oLink.ReturnCode = oAsmbtcp.Result
                            End If
                        Else
                            oLink.Busy = False
                            oLink.ElapsedTime = 0
                        End If

                    End If 'oLink.Busy
                End If 'oLink.ControlName...

            Next 'oLink

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotlink_Tick", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:tmrHotlink_Tick", ex.Message)
        End Try

        tmrHotLink.Enabled = True

    End Sub

    Private Sub tmrManRW_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrManRW.Tick
        '********************************************************************************************
        ' Description:  This routine allows the program to error out when a manual (one-time)read 
        '               or write operation takes too long.
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrManRW.Enabled = False

        Try

            If mbOTRWInProg Then
                'communication breakdown (it drives me insane) - fire the error event once
                If (AxAsmbtcp0.Result <> 0) And (AxAsmbtcp0.Result <> mnAsmbtcpReturnCode) Then
                    Dim sAddress As String = "Address: " & AxAsmbtcp0.MemStart & ", Length: " & AxAsmbtcp0.MemQty.ToString & ". "
                    mnAsmbtcpReturnCode = AxAsmbtcp0.Result
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", _
                       "AxAsmbtcp0 comm timeout. " & sAddress & "Error Code: " & AxAsmbtcp0.Result.ToString & " - " & AxAsmbtcp0.ResultString)
                    RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsPLCComm:tmrManRW_Tick", AxAsmbtcp0.ResultString)
                End If
            End If 'mbOTRWInProg

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:tmrManRW_Tick", ex.Message)
        End Try

        mbOTRWInProg = False

    End Sub

#End Region

    Private Class clsLink
        '********************************************************************************************
        'Description:  This class is to keep track of associated link info for hotlinks
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

#Region "  Declares "
        Private msAddress As String = String.Empty
        Private msControlName As String = String.Empty
        Private mnBit As Short ' = 0
        Private mbBusy As Boolean ' = False
        Private mnElapsedTime As Integer ' = 0
        Private msIP_Address As String = String.Empty
        Private mnLength As Short ' = 0
        Private mnPollRate As Integer ' = 0
        Private msRefData() As String
        Private mnReturnCode As Integer ' = 0
        Private msSlot As String = String.Empty
        Private msTagName As String = String.Empty
        Private msType As String = String.Empty
        Private msZoneName As String = String.Empty
        Private mnSameDataCount As Integer '= 0
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
            'Description: Name of the Asmbtcp Control associated with this link
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
            'Description: The value of the Asmbtcp control "Result" from the last PLC Data Table read/
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

        Friend Property SameDataCount() As Integer
            '********************************************************************************************
            'Description: The number of times that a hotlink has come back with the same data
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnSameDataCount
            End Get

            Set(ByVal value As Integer)
                mnSameDataCount = value
            End Set

        End Property

        Friend Property Slot() As String
            '********************************************************************************************
            'Description: The slot (number) where the target CPU resides. 
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

#Region " Events "

        Friend Sub New()
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

        End Sub

#End Region

    End Class  'clsLink

    Private Class clsLinks
        '********************************************************************************************
        'Description:  This class is to hold the collection of Links
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Inherits CollectionBase

#Region " Declares"

#End Region

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

#Region " Events "

        Friend Sub New()
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


        End Sub

#End Region

    End Class  'clsLinks

End Class 'clsPLCComm

