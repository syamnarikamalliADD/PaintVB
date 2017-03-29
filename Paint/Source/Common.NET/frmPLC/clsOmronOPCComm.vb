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
' Form/Module: clsOmronOPCComm
'
' Description: Class to use OPCDatx ActiveX to talk to an OPC Server i.e. Omron
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
'    07/27/2007 RJO     Initial Code
'    01/15/2008 GEO     Modified for OPC control (OMRON PLC)
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Friend Class clsPLCComm
    '***** TODO List ***************************************************************************
    'TODO - Figure out how to do BOOL and BOOL16 in AxAsabtcp_UnsolicitedMessage
    'TODO - Test operation with a PLC-5
    'TODO - Test operation with a SLC
    'TODO - OPC Server only INT and BOOL are addressed 
    'TOD  - Error handling for comm faults and OPC server errors
    '***** End TODO List ***********************************************************************

#Region "  Declares  "

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "OmronOPCComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsOmronOPCComm"
    Private Const msOPCServerName As String = "opctestlab.opcanalyzer.1" '"omron.neopc" '"Opctestlab.opcanalyzer.1"
    Private Const mnTIMEOUT As Integer = 1001 'Hotlink Timeout period in milliseconds
    Friend Const mnMANUAL_POLL_RATE As Integer = 5000 'A poll rate of this value specifies a One-Time-Read vs. a HotLink
    ' added for OPC server Geo 1/15/08
    Private Const mnOPC_RW_TIMEOUT As Integer = 3000 'OPC control Read variable time out
    Private Const msHOTLINKTAG As String = "HOTLINK" ' USE Controls tag to flag link type
    Private Const msMANUALLINKTAG As String = "MANUAL"
    Private Const mnDEFAULTUPDATERATE As Integer = 50
    Private Const mnOPC_COMM_QUALITY_OK As Integer = 192 'OPC Constant
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemoteComputer As String = String.Empty
    Private msReadData() As String
    Private mbRead As Boolean = False
    Private mPLCType As ePLCType = ePLCType.Logix5000 'Default to Logix 5000
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
    Private mbOTRWInProg As Boolean = False
    Private mLinks As New clsLinks
    Private mnHotLinks As Integer = 0 'How many HotLinks have been created.
    Private mnOPCdatxReturnCode As Integer = 0
    '***** End Working Vars ********************************************************************


#End Region

#Region " Properties "

    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for OPCdatx. Maintained for compatibility
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
            Dim l As clsLink = mLinks.Item(TagName, False)

            If l Is Nothing Then
                l = PLCTagInfo(TagName)
                mLinks.Add(l)
                Call subConfigControl(l)
            End If

            If ReadPLCData(sData, l) Then
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
            Dim l As clsLink = mLinks.Item(TagName, False)

            If l Is Nothing Then
                l = PLCTagInfo(TagName)
                Call subConfigControl(l)
                mLinks.Add(l)
            End If

            If WritePLCData(value, l) = False Then
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
        'Description:  OPCdatx.ocx is capable of reading/writing data to 3 PLC types (PLC-5, SLC-500, 
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
                'If (oLink.PollRate <> mnMANUAL_POLL_RATE) Then
                '    nIndex += 1
                '    ReDim Preserve sHotLinks(nIndex)
                '    sHotLinks(nIndex) = oLink.ControlName

                '    For Each oControl As Control In Me.Controls
                '        If oControl.Name = sHotLinks(nIndex) Then
                '            Dim oOPCdatx As AxOPCDATALib.AxOPCData = DirectCast(oControl, AxOPCDATALib.AxOPCData)
                '            oOPCdatx.Disconnect()
                '            RemoveHandler oOPCdatx.ValueChanged, AddressOf AxOPCData_ValueChanged
                '            oOPCdatx.Dispose()
                '        End If
                '    Next 'oControl

                'End If '(oLink.PollRate <> mnMANUAL_POLL_RATE)
                If oLink.IsHotLink Then
                    oLink.PLCControl.Disconnect()
                    RemoveHandler oLink.PLCControl.ValueChanged, AddressOf AxOPCData_ValueChanged
                End If
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
                'For Each oControl As Control In Me.Controls
                '    If oControl.Name = sControlName Then
                '        Dim oOPCdatx As AxOPCDATALib.AxOPCData = DirectCast(oControl, AxOPCDATALib.AxOPCData)
                '        oOPCdatx.Disconnect()
                '        RemoveHandler oOPCdatx.ValueChanged, AddressOf AxOPCData_ValueChanged
                '        oOPCdatx.Dispose()
                '        Exit For
                '    End If
                'Next 'oControl
                oLink.PLCControl.Disconnect()
                RemoveHandler oLink.PLCControl.ValueChanged, AddressOf AxOPCData_ValueChanged
                mLinks.Remove(oLink)
            End If 'sControlName <> String.Empty

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subConfigControl(ByRef Link As clsLink)

        '********************************************************************************************
        'Description:  Set the OPCdatx control properties required for all read/write operations.
        '
        'Parameters: OPCdatx Control, Link Info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim iResult As Integer
        Try
            With Link.PLCControl  'Control
                .AutoConnect = False
                .OPCServer = msOPCServerName
                .OPCServerTimeout = mnTIMEOUT
                .AutoConnectTimeout = mnOPC_RW_TIMEOUT
                .DefaultUpdateRate = mnDEFAULTUPDATERATE
                .Activated = True
                If Link.PollRate = mnMANUAL_POLL_RATE Then
                    .Tag = msMANUALLINKTAG
                Else
                    .Tag = msHOTLINKTAG
                    Link.IsHotLink = True
                End If
                'Link.ControlName = .Name
                iResult = .Connect()

                If iResult <> 0 Then
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subConfigControl", CStr(iResult))
                    RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, CStr(iResult), _
                                           msMODULE & ":clsPLCComm:subConfigControl", String.Empty)
                End If
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
        'Description:  Create a new OPCdatx control to HotLink to the new address.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        '1/15/08    geo   Modified for OPC control (OMRON)
        Try

            ''''Dim oControl As New AxOPCDATALib.AxOPCData

            mnHotLinks += 1
            '''''GEO REMOVED FOR OPC 1/15/08  If mnHotLinks = 1 Then tmrHotLink.Enabled = True
            ''''Link.ControlName = "AxOPCData" & mnHotLinks.ToString(Globalization.CultureInfo.InvariantCulture)
            '''''oControl.Name = "AxOPCData" & mnHotLinks.ToString(Globalization.CultureInfo.InvariantCulture)
            ''''Dim textbox1 As New TextBox

            '''''oControl.BeginInit()
            '''''Me.Controls.Add(oControl)
            '''''oControl.EndInit()
            ''''Link.ControlName = oControl.Name
            '''''mLinks.Add(Link)
            Call subConfigControl(Link)

            AddHandler Link.PLCControl.ValueChanged, AddressOf AxOPCData_ValueChanged

            With Link.PLCControl
                Dim lngResult As Long
                Dim connectionTable(3) As String

                connectionTable(0) = "Text" 'property Not used for Hot Link Needs something though
                connectionTable(1) = Link.Address 'Tag in OPC server to subscribe to 
                connectionTable(2) = Link.PollRate.ToString 'subscription rate, in ms. 
                connectionTable(3) = "0.0" 'deadband 
                .Tag = msHOTLINKTAG 'flag for hot link 
                Link.IsHotLink = True
                lngResult = .ConnectObject(Nothing, connectionTable) 'using nothing will enable change event

                If lngResult = 0 Then
                    ' MsgBox("Successfully connected")
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                   "Failed to connect, error # " & lngResult.ToString)
                    'MsgBox("Failed to connect, error # " & lngResult)
                End If
                '''Application.DoEvents()
                ''''mLinks.Item(oControl.Name).Busy = True
                '''Link.Busy = True
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subCreateHotLink", String.Empty)
        End Try

    End Sub
#End Region

#Region " Functions "
    'Private Function ControlFromName(ByVal Name As String) As AxOPCDATALib.AxOPCData
    '    '********************************************************************************************
    '    'Description:  Return a reference to the named AxOPCdatx control.
    '    '
    '    'Parameters: Name - AxOPCdatx Control Name
    '    'Returns:    The control
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    '''1/15/08   geo      Modified to work with OPC control (Omron)
    '    ''Dim oAxOPCdatx As AxOPCDATALib.AxOPCData = Nothing
    '    ''Try

    '    ''    ''    For Each oControl As Control In Me.Controls
    '    ''    ''        If oControl.Name = Name Then
    '    ''    ''            oAxOPCdatx = DirectCast(oControl, AxOPCDATALib.AxOPCData)
    '    ''    ''            Exit For
    '    ''    ''        End If
    '    ''    ''    Next 'oControl


    '    ''Catch ex As Exception
    '    ''    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ControlFromName", ex.Message & vbCrLf & ex.StackTrace)
    '    ''    RaiseEvent ModuleError(0, ex.Message, msMODULE & ":clsPLCComm:ControlFromName", String.Empty)
    '    ''End Try

    '    ''Return oAxOPCdatx
    '    Return TryCast(Me.Controls(Name), AxOPCDATALib.AxOPCData)

    'End Function

    Private Function OneTimeRead(ByVal Link As clsLink, ByRef Data() As String) As Boolean
        '****************************************************************************************
        'Description: This function reads data One Time from the plc
        '
        'Parameters: None
        'Returns:    True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '8/4/04     gks     read twice
        '*****************************************************************************************
        Dim result As Integer
        Dim State As Integer
        Dim objValue As Object = Nothing
        Dim sAddrLink As String = Link.Address
        Try

            With Link.PLCControl 'frmPLCControls.Fred

                result = .ReadVariable(sAddrLink, objValue, State, mnOPC_RW_TIMEOUT)

                If result = 0 Then ' data returned ok
                    If State = mnOPC_COMM_QUALITY_OK Then  ' Communication quality
                        'worked as expected
                        'msReadData = CvShortToString(objValue)
                        Data = CvShortToString(objValue)
                        '.Disconnect()
                        Return True
                    End If
                End If

                'lets try a 2 time read
                Application.DoEvents()
                objValue = Nothing
                result = .ReadVariable(sAddrLink, objValue, State, mnOPC_RW_TIMEOUT)

                If result = 0 Then
                    If State = mnOPC_COMM_QUALITY_OK Then  ' Communication quality
                        'worked as expected
                        'msReadData = CvShortToString(objValue)
                        Data = CvShortToString(objValue)
                        '.Disconnect()
                        Return True
                    Else
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", _
                                               "PLC Data Quality is Not Good Verify Communication with PLC and OPC Server:" & State.ToString)

                    End If
                Else


                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", _
                                           "Error connecting name to object: " & sAddrLink & _
                                           " " & result.ToString)
                End If

                'failed twice
                '.Disconnect()
                Return False
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:OneTimeRead", ex.Message)
            Return False
        End Try


    End Function

    Private Function CvShortToString(ByVal oObj As Object) As String()
        '****************************************************************************************
        'Description: Converts a object array of type short to String Array
        '
        'Parameters: None
        'Returns:    True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim bIsArray As Boolean = IsArray(oObj) ' is the object an array
        If bIsArray = True Then
            Dim ArrTemp As Array = DirectCast(oObj, System.Array) 'convert to system.array
            Dim iArrLenght As Integer = UBound(ArrTemp) ' get lenght of array
            Dim iLoop As Integer = 0
            Dim iArr(iArrLenght) As Integer  ' create a temp int array
            Dim sArr(iArrLenght) As String  ' create a temp string array
            Array.Copy(ArrTemp, iArr, iArrLenght)
            For iLoop = 0 To iArrLenght
                sArr(iLoop) = iArr(iLoop).ToString
            Next
            CvShortToString = sArr
        Else ' single word read
            Dim sArr(0) As String
            If Not IsNothing(oObj) Then
                sArr(0) = oObj.ToString
                CvShortToString = sArr
            Else
                sArr(0) = "err" ' should generate an error message from the sub which called conversion
                CvShortToString = sArr
            End If
        End If
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
                    sRef(nIndex) = "-"
                Next
                .RefData = sRef
            End With

            'Make sure Zonename doesn't include any spaces. If it does, replace them with "_" (underbars)
            sPath = "//" & Strings.Replace(ZoneName, " ", "_") & "[id='" & sTopic & "']"
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                                   "(" & oNodeList.Count.ToString & ") instances found of Tag [" & TagName & "].")
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
                .ControlName = .TagName 'String.Empty
                .Busy = False
                .ElapsedTime = 0
                .ReturnCode = 0
            End With
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                                   "(" & oNodeList.Count.ToString & ") instances found of Topic [" & sTopic & "] for Zone [" & ZoneName & "].")
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
                '''determine if this is a One-Timer or a HotLink
                ''If Link.PollRate = mnMANUAL_POLL_RATE Then
                ''    Call subUpdateManualLink(Link)
                ''    'Debug.Print(Link.Address)
                ''    If AxOPCData0.Activated = False Then
                ''        Call subConfigControl(AxOPCData0, Link)
                ''    End If
                ''    AxOPCData0.Update()

                ''    If OneTimeRead() Then 'One time read will update msReadData Array
                ''        Data = msReadData

                ''        Return True
                ''    Else
                ''        Return False
                ''    End If 'OneTimeRead()
                ''Else
                ''    'HotLink
                ''    Call subCreateHotLink(Link)
                ''    Return True
                ''End If
                If Link.IsHotLink Then
                    Call subCreateHotLink(Link)
                Else
                    Link.PLCControl.Update()
                    If OneTimeRead(Link, Data) Then
                        'Data = msReadData
                        Return True
                    Else
                        Return False
                    End If
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
            Dim bFloat As Boolean = False

            'check that its numeric - this should also catch null values
            If IsNumeric(sElement) = False Then
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                       "Invalid Data, value = [" & sElement & "], type = " & Link.Type)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                       msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                Return False
            Else
                bFloat = Strings.InStr(sElement, ".") > 0
            End If
            'check that it's the correct type for the data Type 
            Select Case Link.Type
                Case "INT"
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    Else
                        Dim nData As Short = 0

                        Try
                            nData = CType(sElement, Short)
                        Catch ex As Exception
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                                   "Invalid Data, value = [" & sElement & "], type = " & Link.Type & _
                                                   vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                   msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                            Return False
                        End Try
                    End If 'bFloat
                Case "DINT"
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End If
                Case "REAL"
                    'We already know it's numeric so pretty much anything goes...
                Case "BIT16", "BIT32", "BOOL"
                    If (sElement <> "0") And (sElement <> "1") Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & "], type = " & Link.Type)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
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
                                       "Invalid data length = " & sData.GetLength(0).ToString & _
                                       ", Link.Length = " & Link.Length.ToString)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.WrongArraySize, gcsRM.GetString("csPLC_WRITE_SIZE_ERROR"), _
                                       msMODULE & ":clsPLCComm:ValidateData", "Data length = " & sData.GetLength(0).ToString & _
                                       ", Link.Length = " & Link.Length.ToString)
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
        

        Dim iResult As Integer

        If ValidateData(Data, Link) Then
            Try
                With Link.PLCControl ' frmPLCControls.AxOPCData0
                    For nIndex As Short = 0 To CType(Data.GetUpperBound(0), Short)

                        Select Case Link.Type
                            Case "INT"
                                iResult = .WriteVariable(Link.Address, _
                                            DirectCast(Data(nIndex), Object), mnOPC_RW_TIMEOUT)
                            Case "DINT"
                                iResult = .WriteVariable(Link.Address, _
                                            DirectCast(Data(nIndex), Object), mnOPC_RW_TIMEOUT)
                            Case "BOOL"
                                If Data(nIndex) = "0" Then
                                    Dim objTempWR As Object = False
                                    iResult = .WriteVariable(Link.Address, objTempWR, mnOPC_RW_TIMEOUT)
                                Else
                                    Dim objTempWR As Object = True
                                    iResult = .WriteVariable(Link.Address, objTempWR, mnOPC_RW_TIMEOUT)
                                End If
                        End Select 'Link.Type
                    Next 'nIndex

                    'Application.DoEvents()
                    ' tmrManRW.Enabled = True

                    '.Disconnect()

                End With 'AxOPCdatx0

                If iResult = 0 Then
                    Return True
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", _
                                           "PLC Write Error Result code: " & iResult.ToString & _
                                           " PLC tag: " & Link.Address)
                    'MsgBox("PLC Write Error Result code: " & iResult & " PLC tag: " & _
                    'Link.Address & vbCrLf & ":clsPLCComm:WritePLCData", MsgBoxStyle.Critical)
                    ' mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", ex.Message & vbCrLf & ex.StackTrace)
                    RaiseEvent ModuleError(0, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), msMODULE & ":clsPLCComm:WritePLCData", String.Empty)
                    Return False
                End If

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

 

    Private Sub AxOPCData_ValueChanged(ByVal sender As System.Object, _
                                        ByVal e As AxOPCDATALib._DS7DataEvents_ValueChangedEvent)

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
        ' MsgBox(CStr(e.property) & "|" & e.quality & "|..." & e.value.ToString & "|" & e.varName) ' Display each argument

        Try 'change event
            Dim oOPCdatx As AxOPCDATALib.AxOPCData = DirectCast(sender, AxOPCDATALib.AxOPCData)
            Dim sReadData() As String = Nothing
            Dim nUBound As Short = 0
            If e.quality = mnOPC_COMM_QUALITY_OK Then 'DATA RECIVED OK FROM OPC CONTROL
                'Debug.Print("Change Event sec.=" & (CStr(Date.Now.Second)) & "." & (CStr(Date.Now.Millisecond)))
                With mLinks.Item(oOPCdatx.Name)
                    '.Busy = False
                    .ElapsedTime = 0
                    nUBound = CType(.Length - 1, Short)
                    ReDim sReadData(nUBound) '(.Length - 1)
                    'If (oOPCdatx.Name <> "AxOPCdatx0") Or mbRead Then
                    If mLinks.Item(oOPCdatx.Name).IsHotLink Or mbRead Then
                        For nIndex As Short = 0 To nUBound 'CType(.Length - 1, Short)
                            Select Case .Type
                                Case "INT"
                                    sReadData = CvShortToString(e.value)
                                Case "DINT"
                                    sReadData = CvShortToString(e.value)
                                Case "BOOL"
                                    'Opc Control is returning an Object with boolean
                                    If CType(e.value.ToString, Boolean) = False Then
                                        sReadData(nIndex) = "0"
                                    End If
                                    If CType(e.value.ToString, Boolean) = True Then
                                        sReadData(nIndex) = "1"
                                    End If
                            End Select
                        Next 'nIndex
                        sReadData = CvShortToString(e.value)
                    End If 'mbRead

                    If oOPCdatx.Tag.ToString = msHOTLINKTAG Then 'This is a hot-link
                        Dim bNewData As Boolean = False
                        Dim sRefData() As String = .RefData

                        For nIndex As Short = 0 To nUBound '.Length - 1
                            If sReadData(nIndex) <> sRefData(nIndex) Then
                                bNewData = True
                                Exit For
                            End If
                        Next 'nIndex

                        If bNewData Then
                            .RefData = sReadData
                            RaiseEvent NewData(.ZoneName, .TagName, .RefData)
                        End If
                        '.Busy = True
                        '.ElapsedTime = 0
                        'Application.DoEvents()

                    Else
                        'One time read, update msReadData
                        'Application.DoEvents()
                        'mbOTRWInProg = False
                        'msReadData = sReadData
                    End If 'oAsabtcp.AutoPollEnabled
                End With 'mLinks.Item(oAsabtcp.Name)
            End If 'oAsabtcp.Result = 0

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxOPCdatx_Complete", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:AxOPCdatx_Complete", ex.Message)
        End Try

    End Sub


    Protected Overrides Sub Finalize()
        '****************************************************************************************
        'Description: ShutDown
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        MyBase.Finalize()

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

    End Sub

    'Private Sub tmrHotlink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHotLink.Tick
    '    '********************************************************************************************
    '    ' Description:  This routine allows the program to error out when a HotLink read operation
    '    '               takes too long.
    '    '
    '    ' Parameters: none
    '    ' Returns: none
    '    '
    '    ' Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    ' 1/15/08  GEO     REMOVED FOR OPC BECAUSE OPC HAS BUILT IN TIMEOUT FUNCTION
    '    tmrHotLink.Enabled = False

    '    Try
    '        For Each oLink As clsLink In mLinks

    '            If oLink.ControlName <> "AxOPCData0" Then
    '                If oLink.Busy Then
    '                    Dim oOPCdatx As AxOPCDATALib.AxOPCData = ControlFromName(oLink.ControlName)
    '                    'If oOPCdatx.Busy Then
    '                    oLink.ElapsedTime += tmrHotLink.Interval
    '                    If oLink.ElapsedTime > mnTIMEOUT Then
    '                        'communication breakdown (it drives me insane) - fire the error event once
    '                        '     If (oAsabtcp.Result <> 0) And (oAsabtcp.Result <> mnASABTCPReturnCode) Then
    '                        ' oLink.ReturnCode = oAsabtcp.Result
    '                        ' mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotlink_Tick", oLink.ControlName & _
    '                        '                       " hotlink comm timeout. Tag = " & oLink.TagName & _
    '                        '                       ", Address = " & oLink.Address & _
    '                        '                       ", IP Address = " & oLink.IP_Address & "." & vbCrLf & _
    '                        '                       oAsabtcp.ResultString)
    '                        'RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
    '                        '                      msMODULE & ":clsPLCComm:tmrHotlink_Tick", oAsabtcp.ResultString)
    '                        ' End If
    '                    End If
    '                Else
    '                    oLink.Busy = False
    '                    oLink.ElapsedTime = 0
    '                End If

    '                'End If 'oLink.Busy
    '            End If 'oLink.ControlName...

    '        Next 'oLink

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotlink_Tick", ex.Message & vbCrLf & ex.StackTrace)
    '        RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_COMM_ERROR"), _
    '                               msMODULE & ":clsPLCComm:tmrHotlink_Tick", ex.Message)
    '    End Try

    '    tmrHotLink.Enabled = True

    'End Sub

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
                'geo2               If (AxAsabtcp0.Result <> 0) And (AxAsabtcp0.Result <> mnASABTCPReturnCode) Then
                'geo2 mnASABTCPReturnCode = AxAsabtcp0.Result
                'geo2 mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", _
                'geo2  "AxAsabtcp0 comm timeout. " & AxAsabtcp0.ResultString)
                'geo2 RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                'geo2                       msMODULE & ":clsPLCComm:tmrManRW_Tick", AxAsabtcp0.ResultString)
                'geo2 End If
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
        Private mnBit As Short = 0
        Private mbBusy As Boolean = False
        Private mnElapsedTime As Integer = 0
        Private msIP_Address As String = String.Empty
        Private mnLength As Short = 0
        Private mnPollRate As Integer = 0
        Private msRefData() As String
        Private mnReturnCode As Integer = 0
        Private msSlot As String = String.Empty
        Private msTagName As String = String.Empty
        Private msType As String = String.Empty
        Private msZoneName As String = String.Empty
        Dim PLCForm As New frmPLCControls
        Private WithEvents oOMPlc As AxOPCDATALib.AxOPCData
        Private mbIsHot As Boolean
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
            'Description: Name of the OPCdatx Control associated with this link
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
                oOMPlc.Name = value
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
                If value = mnMANUAL_POLL_RATE Then
                    IsHotLink = False
                Else
                    IsHotLink = True
                End If
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
            'Description: The value of the OPCdatx control "Result" from the last PLC Data Table read/
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
        Friend Property IsHotLink() As Boolean
            Get
                Return mbIsHot
            End Get
            Set(ByVal value As Boolean)
                mbIsHot = value
            End Set
        End Property
        Friend ReadOnly Property PLCControl() As AxOPCDATALib.AxOPCData
            Get
                Return oOMPlc
            End Get
        End Property

#End Region

#Region " Events "

        Protected Overrides Sub Finalize()
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

            MyBase.Finalize()
            Try
                oOMPlc.Disconnect()
            Catch ex As Exception

            End Try

        End Sub

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
            oOMPlc = DirectCast(PLCForm.Controls("Fred"), AxOPCDATALib.AxOPCData)

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
                'If oLink.PLCControl.Name = "Fred" Then
                '    oLink.PLCControl.Name = "Fred" & List.Count.ToString
                '    oLink.ControlName = oLink.PLCControl.Name
                'End If
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
        Friend Function Item(ByVal TagName As String, ByVal Void As Boolean) As clsLink

            Dim o As clsLink = Nothing

            For Each o In List
                If o.TagName = TagName Then
                    Return o
                End If
            Next

            Return Nothing

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

        Protected Overrides Sub Finalize()
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

            MyBase.Finalize()

        End Sub

#End Region

    End Class  'clsLinks


  

End Class 'clsPLCComm

