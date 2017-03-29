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
' Form/Module: clsASABTCPComm
'
' Description: Class to use ASACTCP ActiveX to talk to a Logix PLC
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
'    05/26/2009 MSW     WritePLCData - BOOL debug - force a read first.
'    11/19/2010 MSW     Add SINT data type
'    03/17/2011 RJO     Many fixes throughout to correct one-time data read errors.
'    03/31/2011 RJO/JBW Added code to AxAsabtcp_Complete event to raise the NewData event if
'                       the polled data has been the same for the last mnREFRESH_COUNT reads.
'    12/1/11    MSW     Mark a version                                             4.01.01.00
'    01/31/2013 ADD/RJO Added 10ms Sleep to all tight loops with DoEvents to allow 4.01.01.01
'                       other threads to be serviced and speed up operation.
'    09/30/13   MSW     Move PLC comm to a DLL                                     4.01.06.00
'    05/20/14   MSW     Add to error logging                                       4.01.07.00
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO


Public Class clsPLCComm
    Implements IPLCComm
    '***** TODO List ***************************************************************************
    'TODO - Figure out how to do BOOL and BOOL16 in AxAsabtcp_UnsolicitedMessage
    'TODO - Test operation with a PLC-5
    'TODO - Test operation with a SLC
    '***** End TODO List ***********************************************************************

#Region "  Declares  "

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "ASABTCPComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsASABTCPComm"
    Private Const mnTIMEOUT As Integer = 1001 'Hotlink Timeout period in milliseconds
    Private Const mnMANUAL_POLL_RATE As Integer = 5000 'A poll rate of this value specifies a One-Time-Read vs. a HotLink
    Private Const mnMAX_RETRY_COUNT As Integer = 3 'Maximum Onetime read retries 'RJO 03/17/11
    Private Const mnREFRESH_COUNT As Integer = 10 'after this many same data polled reads, send data as new data anyway 'JBW 03/31/11
    Private Const msASSEMBLY As String = "PLCComm"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msASSEMBLY & ".CommonStrings"
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemotePath As String = String.Empty
    Private msReadData() As String
    Private mbRead As Boolean '= False
    Private mPLCType As ePLCType = ePLCType.Logix5000 'Default to Logix 5000
    '***** End Property Vars *******************************************************************

    '******* Events ****************************************************************************
    'when hotlink has new data 
    Public Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                         ByVal Data() As String) Implements IPLCComm.NewData
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Public Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String) Implements IPLCComm.ModuleError
    '******* End Events ************************************************************************

    '***** Working Vars ************************************************************************
    Private mbOTRWInProg As Boolean ' = False
    Private mLinks As New clsLinks
    Private mnHotLinks As Integer ' = 0 'How many HotLinks have been created.
    Private mnASABTCPReturnCode As Integer ' = 0
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************
    'Public Enum ePLCType
    '    PLC5 = 1
    '    SLC500 = 2
    '    Logix5000 = 3
    'End Enum
    '***** End Enums ***************************************************************************

#End Region

#Region " Properties "

    Public ReadOnly Property DefaultGroupName() As String Implements IPLCComm.DefaultGroupName
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
            Return Me.Parent.Name
        End Get

    End Property

    Public Property PLCData() As String() Implements IPLCComm.PLCData
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
            Dim oLink As clsLink = PLCTagInfo(TagName)
            If oLink Is Nothing OrElse (WritePLCData(value, oLink) = False) Then
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

    Public Property PLCType() As ePLCType Implements IPLCComm.PLCType
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

    Public ReadOnly Property MaxDataPerReadWrite() As Integer Implements IPLCComm.MaxDataPerReadWrite
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

    Public Property TagName() As String Implements IPLCComm.TagName
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


    Public ReadOnly Property XMLPath() As String Implements IPLCComm.XMLPath
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
	
    Public Property RemotePath() As String Implements IPLCComm.RemotePath
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
        Get
            Return msRemotePath
        End Get
    End Property

    Public Property ZoneName() As String Implements IPLCComm.ZoneName
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
        Set(ByVal value As String)

            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, msRemotePath, msXMLFILENAME) Then
                msZoneName = value
            Else
                msZoneName = String.Empty
                Dim sTmp As String = gcsRM.GetString("csCANT_FIND_CONFIG_FILE") & _
                                " " & gcsRM.GetString("csZONE_NAME") & ":=" & ZoneName
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:Zone", "Could not find configuration file: " & _
                                       msXMLFILENAME & ". ZoneName = " & value)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigFileNotFound, sTmp, _
                                       msMODULE & ":clsPLCComm:Zone", String.Empty)
            End If
            'should add ping here??
        End Set
        Get
            Return msZoneName
        End Get

    End Property

#End Region

#Region " Public Methods "
    Public Function AreaNum(ByVal Area As String) As Integer Implements IPLCComm.AreaNum
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
    Public Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String Implements IPLCComm.FormatFromTimerValue
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
    Public Function FormatToTimerValue(ByVal ValueIn As Integer) As String Implements IPLCComm.FormatToTimerValue
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

    Public Sub RemoveAllHotLinks() Implements IPLCComm.RemoveAllHotLinks
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
                            Dim oAsabtcp As AxASABTCPLib.AxAsabtcp = DirectCast(oControl, AxASABTCPLib.AxAsabtcp)

                            oAsabtcp.Disconnect()
                            RemoveHandler oAsabtcp.Complete, AddressOf AxAsabtcp_Complete
                            RemoveHandler oAsabtcp.UnsolicitedMessage, AddressOf AxAsabtcp_UnsolicitedMessage
                            oAsabtcp.Dispose()
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

    Public Sub RemoveHotLink(ByVal Tag As String, ByVal Zone As String) Implements IPLCComm.RemoveHotLink
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
                If (oLink.TagName = Tag) And (oLink.ZoneName = Zone) Then
                    sControlName = oLink.ControlName
                    Exit For
                End If
            Next 'oLink

            If sControlName <> String.Empty Then
                For Each oControl As Control In Me.Controls
                    If oControl.Name = sControlName Then
                        Dim oAsabtcp As AxASABTCPLib.AxAsabtcp = DirectCast(oControl, AxASABTCPLib.AxAsabtcp)
                        Debug.Print("Link Removed==" & oControl.Name)
                        oAsabtcp.Disconnect()
                        RemoveHandler oAsabtcp.Complete, AddressOf AxAsabtcp_Complete
                        RemoveHandler oAsabtcp.UnsolicitedMessage, AddressOf AxAsabtcp_UnsolicitedMessage
                        oAsabtcp.Dispose()
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

    Public Function MemType(ByVal Type As String) As String Implements IPLCComm.MemType
        '********************************************************************************************
        'Description:  Return the MemType for the supplied Data Type.
        '
        'Parameters: Type - Data Type
        'Returns:    MemType
        '
        'Modification history:
        '
        ' Date      By      Reason
        '7/10/08    gks     Change from private to shared per fxcop\
        ' 11/19/10  MSW     Add SINT data type
        '********************************************************************************************

        Select Case Type
            Case "SINT"
                MemType = "S" 'Integer
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

    Public Function TagParams(ByVal TagName As String) As String Implements IPLCComm.TagParams
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
        Return String.Empty

    End Function


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
        tmrHotLink.Interval = 1000 'ms
        tmrManRW.Interval = 1000 'ms RJO 03/17/11
        AxAsabtcp0.TimeoutTrans = 500 'ms RJO 03/17/11

    End Sub


#End Region

#Region " Private Routines "
    Private Sub subConfigControl(ByRef Control As AxASABTCPLib.AxAsabtcp, ByVal Link As clsLink)
        '********************************************************************************************
        'Description:  Set the Asabtcp control properties required for all read/write operations.
        '
        'Parameters: Asabtcp Control, Link Info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            With Control
                .AutoPollEnabled = False
                '.Disconnect()
                .NodeAddress = Link.IP_Address & ",," & Link.Slot
                .MemQty = Link.Length
                .MemType = Link.MemType
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
        'Description:  Create a new Asabtcp control to HotLink to the new address.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim oControl As New AxASABTCPLib.AxAsabtcp

            mnHotLinks += 1
            If mnHotLinks = 1 Then tmrHotLink.Enabled = True

            oControl.Name = "AxAbtcp" & mnHotLinks.ToString(System.Globalization.CultureInfo.InvariantCulture)
            oControl.BeginInit()
            Me.Controls.Add(oControl)
            oControl.EndInit()

            Link.ControlName = oControl.Name
            mLinks.Add(Link)

            AddHandler oControl.Complete, AddressOf AxAsabtcp_Complete
            AddHandler oControl.UnsolicitedMessage, AddressOf AxAsabtcp_UnsolicitedMessage
            Call subConfigControl(oControl, Link)

            With oControl
                Select Case mPLCType
                    Case ePLCType.PLC5
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_PLC5_READ_WORD
                    Case ePLCType.SLC500
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_SLC500_READ_WORD
                    Case ePLCType.Logix5000
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_LOGIX_READ_ELEMENT
                End Select

                .AutoPollInterval = Link.PollRate
                .TimeoutTrans = 250 'RJO 03/17/11

                If Link.PollRate > 0 Then
                    .AutoPollEnabled = True
                Else
                    'for unsolicited msgs from SLC
                    .AutoPollEnabled = False
                    'LogixSlot Parameter contains Data Table Offset for a SLC
                    .SlaveMemStart = Link.Slot
                    .SlaveEnabled = True
                End If

                .AsyncRefresh()

                Do While .Busy
                    Application.DoEvents()
                    Threading.Thread.Sleep(10) 'ADD/RJO 01/31/13
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
        'Description:  Make sure the AxAsabtcp0 link is in the collection. Update it with new tag
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
            Link.ControlName = "AxAsabtcp0"

            If IsNothing(mLinks.Item("AxAsabtcp0")) Then
                Call mLinks.Add(Link)
            Else
                With mLinks.Item(Link.ControlName)
                    .Address = Link.Address
                    .Bit = Link.Bit
                    .IP_Address = Link.IP_Address
                    .Length = Link.Length
                    .MemType = Link.MemType
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

    Private Function ControlFromName(ByVal Name As String) As AxASABTCPLib.AxAsabtcp
        '********************************************************************************************
        'Description:  Return a reference to the named AxAsabtcp control.
        '
        'Parameters: Name - AxAsabtcp Control Name
        'Returns:    The control
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oAxAsabtcp As AxASABTCPLib.AxAsabtcp = Nothing

        Try

            For Each oControl As Control In Me.Controls
                If oControl.Name = Name Then
                    oAxAsabtcp = DirectCast(oControl, AxASABTCPLib.AxAsabtcp)
                    Exit For
                End If
            Next 'oControl

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ControlFromName", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, ex.Message, msMODULE & ":clsPLCComm:ControlFromName", String.Empty)
        End Try

        Return oAxAsabtcp

    End Function

    Private Function OneTimeRead() As Boolean
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

            With AxAsabtcp0
                Select Case mPLCType
                    Case ePLCType.PLC5
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_PLC5_READ_WORD
                    Case ePLCType.SLC500
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_SLC500_READ_WORD
                    Case ePLCType.Logix5000
                        .Function = ASABTCPLib.enumFunction.FUNC_AB_LOGIX_READ_ELEMENT

                End Select

                mnASABTCPReturnCode = 0 'RJO 03/17/11
                .AsyncRefresh()

                Do While .Busy
                    Application.DoEvents()
                    Threading.Thread.Sleep(10) 'ADD/RJO 01/31/13
                Loop

                tmrManRW.Enabled = True
                Do Until mbOTRWInProg = False
                    Application.DoEvents()
                    Threading.Thread.Sleep(10) 'ADD/RJO 01/31/13
                Loop
                tmrManRW.Enabled = False 'RJO 03/17/11

                If mnASABTCPReturnCode = 0 Then 'RJO 03/17/11
                    bSuccess = True
                End If
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:OneTimeRead", ex.Message)
            Return False
        End Try

        Return bSuccess 'True 'RJO 03/17/11

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
        '2/3/06    Geo    Original

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
                .MemType = MemType(.Type)
                'ReDim .RefData(.Length - 1)
                Dim sRef(.Length - 1) As String
                For nIndex As Integer = 0 To .Length - 1
                    ' Geo filling this array with "-" is causing an exception error
                    ' when converting to a number consider modification Geo. 3/19/09
                    ' MSW 6/24/10 - default to 0 so it doesn't crash
                    sRef(nIndex) = "0"
                Next
                .RefData = sRef
            End With

            'Make sure Zonename doesn't include any spaces. If it does, replace them with "_" (underbars)
            sPath = "//" & Strings.Replace(ZoneName, " ", "_") & "[id='" & sTopic & "']"
            ' 
            ' Not sure what this is for? Geo. 3/18/09
            'sPath = "//" & Strings.Replace(ZoneName, " ", "_x0020_") & "[id='" & sTopic & "']"
        End If

        If oNodeList.Count > 1 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                         "(" & oNodeList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) & _
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
                           "(" & oNodeList.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) & _
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
                'determine if this is a One-Timer or a HotLink
                If Link.PollRate = mnMANUAL_POLL_RATE Then
                    Dim bSuccess As Boolean = False
                    Dim nRetryCount As Integer = 0

                    'One-Time Read
                    Call subUpdateManualLink(Link)
                    Call subConfigControl(AxAsabtcp0, Link)

                    For nRetryCount = 0 To mnMAX_RETRY_COUNT  'RJO 03/17/11
                        'Make sure the control is not busy
                        Do While AxAsabtcp0.Busy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10) 'ADD/RJO 01/31/13
                        Loop

                        If OneTimeRead() Then
                            Data = msReadData
                            bSuccess = True
                            Exit For
                        End If 'OneTimeRead()
                        AxAsabtcp0.Disconnect()
                    Next
                    If nRetryCount > 0 Then mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", "Retry count = " & nRetryCount.ToString)
                    Return bSuccess
                Else
                    'HotLink
                    Call subCreateHotLink(Link)
                    '2/13/09 gks added following to return data instead of nothing
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
        ' 11/19/10  MSW     Add SINT data type
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
                            sData.GetLength(0).ToString(System.Globalization.CultureInfo.InvariantCulture) & _
                            ", Link.Length = " & _
                            Link.Length.ToString(System.Globalization.CultureInfo.InvariantCulture))
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.WrongArraySize, _
                            gcsRM.GetString("csPLC_WRITE_SIZE_ERROR"), _
                           msMODULE & ":clsPLCComm:ValidateData", "Data length = " & _
                           sData.GetLength(0).ToString(System.Globalization.CultureInfo.InvariantCulture) & _
                           ", Link.Length = " & _
                           Link.Length.ToString(System.Globalization.CultureInfo.InvariantCulture))
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
        ' 05/26/09  MSW     BOOL debug - force a read first.
        ' 11/19/10  MSW     Add SINT data type
        '********************************************************************************************
        If link Is Nothing Then
            Return False
        End If
        If ValidateData(Data, Link) Then

            Try
                Call subConfigControl(AxAsabtcp0, Link)
                Call subUpdateManualLink(Link)
                'This prevents using leftovers from the last read.
                'Booleans need to read the whole word, then write it back.
                'Only tested with "BOOL", but I'm guessing the others need it, too.
                'If (Link.Type = "BOOL") Or (Link.Type = "BIT16") Or (Link.Type = "BIT32") Then
                OneTimeRead()
                'End If
                With AxAsabtcp0
                    For nIndex As Short = 0 To CType(Data.GetUpperBound(0), Short)

                        Select Case mPLCType
                            Case ePLCType.PLC5
                                .Function = ASABTCPLib.enumFunction.FUNC_AB_PLC5_WRITE_WORD
                            Case ePLCType.SLC500
                                .Function = ASABTCPLib.enumFunction.FUNC_AB_SLC500_WRITE_WORD
                            Case ePLCType.Logix5000
                                .Function = ASABTCPLib.enumFunction.FUNC_AB_LOGIX_WRITE_ELEMENT
                        End Select 'PLCType

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
                            Case "BOOL" 'RSLogix 5000 Boolean Type
                                .SetDataBitM(Link.Bit, CType(Data(0), Integer) > 0)
                        End Select 'Link.Type

                    Next 'nIndex

                    .AsyncRefresh()

                    Do While .Busy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10) 'ADD/RJO 01/31/13
                    Loop

                    tmrManRW.Enabled = True

                End With 'AxAsabtcp0

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

    Private Sub AxAsabtcp_Complete(ByVal sender As Object, _
                                   ByVal e As AxASABTCPLib._DAsabtcpEvents_CompleteEvent) _
                                   Handles AxAsabtcp0.Complete
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
        ' 05/20/14  MSW     Add error logging
        '*****************************************************************************************
        '
        Try
            Dim oAsabtcp As AxASABTCPLib.AxAsabtcp = DirectCast(sender, AxASABTCPLib.AxAsabtcp)
            Try

            Dim sReadData() As String = Nothing
            Dim nUBound As Short = 0


            If oAsabtcp.Result = 0 Then
                With mLinks.Item(oAsabtcp.Name)

                    .Busy = False
                    .ElapsedTime = 0
                    nUBound = CType(.Length - 1, Short)
                    'If we're reading a BIT or BOOL type, the read length is always 1 regrdless of data length (.Length)
                    If (.Type = "BIT16") Or (.Type = "BIT32") Or (.Type = "BOOL") Then nUBound = 0

                    If (oAsabtcp.Name <> "AxAsabtcp0") Or mbRead Then
                        ReDim sReadData(nUBound) '(.Length - 1)

                        For nIndex As Short = 0 To nUBound 'CType(.Length - 1, Short)
                            Select Case .Type
                                Case "SINT"
                                    sReadData(nIndex) = _
                                        oAsabtcp.GetDataByteM(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                Case "INT"
                                    sReadData(nIndex) = _
                                        oAsabtcp.GetDataWordM(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                Case "DINT"
                                    sReadData(nIndex) = _
                                        oAsabtcp.GetDataLongM(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                Case "REAL"
                                    sReadData(nIndex) = _
                                        oAsabtcp.GetDataFloatM(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                                Case "BIT16", "BIT32", "BOOL"
                                    Dim nTmp As Integer = -CType(oAsabtcp.GetDataBitM(.Bit), Integer)
                                    sReadData(nIndex) = nTmp.ToString(System.Globalization.CultureInfo.InvariantCulture)
                            End Select
                        Next 'nIndex
                    End If 'mbRead

                    If oAsabtcp.AutoPollEnabled Then 'This is a hot-link
                        Dim bNewData As Boolean = False
                        Dim sRefData() As String = .RefData

                        For nIndex As Short = 0 To nUBound '.Length - 1
                            If sReadData(nIndex) <> sRefData(nIndex) Then
                                bNewData = True
                                Exit For
                            End If
                        Next 'nIndex
                        If bNewData = False Then 'RJO 03/31/11
                            .SameDataCount += 1
                        End If
                        If .SameDataCount >= mnREFRESH_COUNT Then 'RJO 03/31/11
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
                        tmrManRW.Enabled = False 'RJO 03/17/11
                    End If 'oAsabtcp.AutoPollEnabled

                End With 'mLinks.Item(oAsabtcp.Name)
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsabtcp_Complete Result Code: " & oAsabtcp.Result.ToString & " : " & oAsabtcp.ResultString, oAsabtcp.MemStart)
                    RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsPLCComm:AxAsabtcp_Complete - " & oAsabtcp.MemStart, "Result Code: " & oAsabtcp.Result.ToString & " : " & oAsabtcp.ResultString)
            End If 'oAsabtcp.Result = 0

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsabtcp_Complete", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                       msMODULE & ":clsPLCComm:AxAsabtcp_Complete - " & oAsabtcp.MemStart, ex.Message)

            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsabtcp_Complete", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:AxAsabtcp_Complete", ex.Message)
        End Try

    End Sub

    Private Sub AxAsabtcp_UnsolicitedMessage(ByVal sender As Object, _
                                             ByVal e As AxASABTCPLib._DAsabtcpEvents_UnsolicitedMessageEvent) _
                                             Handles AxAsabtcp0.UnsolicitedMessage
        '********************************************************************************************
        ' Description: This routine responds to an unsolicited message from the PLC
        '
        ' Parameters: Index
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        ' 11/19/10  MSW     Add SINT data type - the byte version of get_SlaveDataWord doesn't exit, but
        '                   I don't think it'll get used.
        '********************************************************************************************

        Try
            Dim oAsabtcp As AxASABTCPLib.AxAsabtcp = DirectCast(sender, AxASABTCPLib.AxAsabtcp)
            Dim oLink As clsLink = mLinks.Item(oAsabtcp.Name)
            Dim sReadData() As String = Nothing

            With oAsabtcp
                ReDim sReadData(.SlaveMemQty)

                For nIndex As Short = 0 To CType(.SlaveMemQty - 1, Short)
                    Select Case oLink.Type
                        Case "SINT"
                            sReadData(nIndex) = _
                                .get_SlaveDataWord(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                            Debug.Assert(False) 'Can't do this
                        Case "INT"
                            sReadData(nIndex) = _
                                .get_SlaveDataWord(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                        Case "DINT"
                            sReadData(nIndex) = _
                                .get_SlaveDataLong(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                        Case "REAL"
                            sReadData(nIndex) = _
                                .get_SlaveDataFloat(nIndex).ToString(System.Globalization.CultureInfo.InvariantCulture)
                            'TODO - Work out the details for BIT16, BIT32, and BOOL Unsolicited Messages
                            'Case "BIT16"
                            'Case "BIT32"
                            'Case "BOOL"
                    End Select
                Next ' nIndex

                RaiseEvent NewData(oLink.ZoneName, oLink.TagName, sReadData)
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:AxAsabtcp_UnsolicitedMessage", _
                            ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                       msMODULE & ":clsPLCComm:AxAsabtcp_UnsolicitedMessage", ex.Message)
        End Try

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

                If oLink.ControlName <> "AxAsabtcp0" Then
                    If oLink.Busy Then
                        Dim oAsabtcp As AxASABTCPLib.AxAsabtcp = ControlFromName(oLink.ControlName)

                        If oAsabtcp.Busy Then
                            oLink.ElapsedTime += tmrHotLink.Interval
                            If oLink.ElapsedTime > mnTIMEOUT Then
                                'communication breakdown (it drives me insane) - fire the error event once
                                'If (oAsabtcp.Result <> 0) And (oAsabtcp.Result <> mnASABTCPReturnCode) Then 'RJO 03/17/11
                                If (oAsabtcp.Result <> 0) And (oAsabtcp.Result <> oLink.ReturnCode) Then
                                    'oLink.ReturnCode = oAsabtcp.Result 'RJO 03/17/11
                                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotlink_Tick", oLink.ControlName & _
                                                           " hotlink comm timeout. Tag = " & oLink.TagName & _
                                                           ", Address = " & oLink.Address & _
                                                           ", IP Address = " & oLink.IP_Address & "." & vbCrLf & _
                                                           "Error Code: " & AxAsabtcp0.Result.ToString & " - " & oAsabtcp.ResultString)
                                    RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                                           msMODULE & ":clsPLCComm:tmrHotlink_Tick", oAsabtcp.ResultString)
                                End If
                                oLink.ReturnCode = oAsabtcp.Result 'RJO 03/17/11
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
                If (AxAsabtcp0.Result <> 0) And (AxAsabtcp0.Result <> mnASABTCPReturnCode) Then
                    Dim sAddress As String = "Address: " & AxAsabtcp0.MemStart & ", Length: " & AxAsabtcp0.MemQty.ToString & ". "
                    mnASABTCPReturnCode = AxAsabtcp0.Result
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", _
                       "AxAsabtcp0 comm timeout. " & sAddress & "Error Code: " & AxAsabtcp0.Result.ToString & " - " & AxAsabtcp0.ResultString)
                    RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsPLCComm:tmrManRW_Tick", AxAsabtcp0.ResultString)
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
        Private msMemType As String = String.Empty
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

        Public Property Address() As String
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

        Public Property Bit() As Short
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

        Public Property Busy() As Boolean
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

        Public Property ControlName() As String
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

        Public Property ElapsedTime() As Integer
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

        Public Property IP_Address() As String
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

        Public Property Length() As Short
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

        Public Property MemType() As String
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

        Public Property PollRate() As Integer
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

        Public Property RefData() As String()
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

        Public Property ReturnCode() As Integer
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
        Public Property SameDataCount() As Integer
            '********************************************************************************************
            'Description: The number of times that a hotlink has come back with the same data
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '03/31/11   JBW     Created
            '********************************************************************************************

            Get
                Return mnSameDataCount
            End Get

            Set(ByVal value As Integer)
                mnSameDataCount = value
            End Set

        End Property
        Public Property Slot() As String
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

        Public Property TagName() As String
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

        Public Property Type() As String
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

        Public Property ZoneName() As String
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


        Public Sub New()
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

        Public Overloads ReadOnly Property Count() As Integer
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

        Public Function Add(ByRef oLink As clsLink) As Integer
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

        Public Function Item(ByVal ControlName As String) As clsLink
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

        Public Sub Remove(ByVal Link As clsLink)
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

        Public Sub New()
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

