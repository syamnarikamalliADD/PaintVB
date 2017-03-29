' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2008
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsActEasyIFComm
'
' Description: Class to use ActEasyIF ActiveX to talk to a Mitsubishi PLC
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
' 08/15/2008    RJO     Initial Code
' 06/22/2009	MSW	    Update zone properties to match current AB PLC form
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/20/2010 RJO     Added .Close calls in clcPLCComm ManualRead and WritePLCData functions
'                       in the cases where the manual read or write fails to prevent F0000003 
'                       errors.                                                       4.01.01.01
' 09/13/2012    AM      Added to support splitting read or write bigger than 200 words 4.01.04.00
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
    Private Const msXMLFILENAME As String = "ActEasyIFComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsActEasyIFComm"
    Private Const mnMANUAL_POLL_RATE As Integer = 5000 'A poll rate of this value specifies a One-Time-Read vs. a HotLink
    Private Const mnREFRESH_INTERVAL As Integer = 5 'Refresh hotlink data if data has remained the same for this many polled reads
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemoteComputer As String = String.Empty
    Private msReadData() As String
    Private mPLCType As ePLCType = ePLCType.QSeries 'Default to Q Series
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
    Private mnHotLinks As Integer = 0 'How many HotLinks are active.
    Private mnCommIndex As Integer = 0 'Unique index number for newly created hotlink controls
    Private mcolPollQueue As New Collection 'Queue of HotLink Data Read Requests
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************
    Friend Enum ePLCType
        QSeries = 1
    End Enum
    '***** End Enums ***************************************************************************

    '2012/09/13 AM Hard coded MaxDataRead/Write per transaction
    Private mnMaxDataPerReadWrite As Integer = 200

#End Region

#Region " Properties "

    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for ActEasyIF. Maintained for compatibility.
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

            If ReadPLCData(sData, PLCTagInfo(TagName)) Then
                Return sData
            Else
                Dim sMsg1 As String = gcsRM.GetString("csCOULD_NOT_READ_PLC")
                Dim sMsg2 As String = gcsRM.GetString("csZONE_NAME")

                sMsg2 = sMsg2 & " = [" & ZoneName & "], " & _
                                 gcsRM.GetString("csTAG_NAME") & "= [" & TagName & "]"
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCData", _
                                       "Could not read PLC Data. ZoneName = [" & ZoneName & "], Tagname = [" & TagName & "]")
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ReadFailed, sMsg1, msMODULE & ":clsPLCComm:PLCData", sMsg2)
                Return Nothing
            End If
        End Get

        Set(ByVal value As String())

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
        'Description:  Used when the ocx is capable of reading/writing data to multiple PLC types. 
        '              This property is set by default to Q Series (the only defined type).
        '              Note: Not used for ActEasyIF. Maintained for compatibility.
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
        'Description:  Used to set the number of Read or Writes when sending more than 200 words
        '              to the PLC.
        '
        'Parameters: none
        'Returns:    PLC Type
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/12  AM      Added to support splitting read or write bigger than 200 words
        '********************************************************************************************

        Get
            Return mnMaxDataPerReadWrite
        End Get

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
        'Description:  Return where we are looking for taginfo.
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
        '              carries dbpath etc with it. Should already be checked for available etc.
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
        End Set

    End Property

    Private ReadOnly Property ZoneName() As String
        '********************************************************************************************
        'Description:  Zone Name. 
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
        'Nope
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
        'assuming all 0.001 timebase
        Dim nTmp As Integer = ValueIn * 10
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

            tmrPoll.Enabled = False
            tmrHotLink.Enabled = False
            mnHotLinks = 0
            mnCommIndex = 0

            Application.DoEvents()

            'Empty out the collection of queued poll requests
            For nItem As Integer = 1 To mcolPollQueue.Count
                mcolPollQueue.Remove(1)
            Next 'nItem

            For Each oLink As clsLink In mLinks
                If Strings.Right(oLink.ControlName, 1) <> "0" Then
                    'This is a HotLink control. Close the connection, then deep 6 the control. 
                    For Each oControl As Control In Me.Controls
                        If oControl.Name = oLink.ControlName Then
                            Dim oAxActEasyIF As AxACTMULTILib.AxActEasyIF = DirectCast(oControl, AxACTMULTILib.AxActEasyIF)

                            oAxActEasyIF.Close()
                            Me.Controls.Remove(oControl)
                        End If
                    Next 'oControl
                End If 'Strings.Right(oLink.ControlName, 1) <> "0"
            Next 'oLink

            'Now toast the link collection
            mLinks = New clsLinks

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveAllHotLinks", ex.Message & vbCrLf & ex.StackTrace)
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
                        Dim oAxActEasyIF As AxACTMULTILib.AxActEasyIF = DirectCast(oControl, AxACTMULTILib.AxActEasyIF)
                        Call oAxActEasyIF.Close()
                        Me.Controls.Remove(oControl)
                        Exit For
                    End If
                Next 'oControl

                mLinks.Remove(oLink)
            End If 'sControlName <> String.Empty

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subConfigControl(ByRef Control As AxACTMULTILib.AxActEasyIF, ByVal Link As clsLink)
        '********************************************************************************************
        'Description:  Set the ActEasyIF control properties required for all read/write operations.
        '
        'Parameters: ActEasyIF Control, Link Info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            With Control
                'Not a whole lot of configuration to do for ActEasyIF
                .ActLogicalStationNumber = Link.Station
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subConfigControl", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subConfigControl", String.Empty)
        End Try

    End Sub

    Private Function CreateHotLink(ByRef Link As clsLink) As Boolean
        '********************************************************************************************
        'Description:  Create a new ActEasyIF control to HotLink to the new address.
        '
        'Parameters: Link - Hotlink info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean

        Try

            Dim oControl As New AxACTMULTILib.AxActEasyIF
            Dim nReturnCode As Integer

            mnHotLinks += 1
            mnCommIndex += 1

            oControl.Name = "AxActEasyIF" & mnCommIndex.ToString(Globalization.CultureInfo.InvariantCulture)
            oControl.BeginInit()
            Me.Controls.Add(oControl)
            oControl.EndInit()

            Call subConfigControl(oControl, Link)

            nReturnCode = oControl.Open
            If nReturnCode = 0 Then
                'Open success
                Link.ControlName = oControl.Name
                mLinks.Add(Link)
                'mLinks.Item(oControl.Name).Busy = True
                'mcolPollQueue.Add(Link.ControlName)
                bSuccess = True
            Else
                'Open fail
                mnHotLinks -= 1
                Me.Controls.Remove(oControl)

                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                       "Unable to open PLC communication - Logical Station Number: " & _
                                       Link.Station.ToString & ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csMPLC_CREATE_HL_FAIL"), _
                                       msMODULE & ":clsPLCComm:subCreateHotLink", _
                                       gcsRM.GetString("csMPLC_COMM_OPEN_FAIL") & Link.Station.ToString)
            End If

            If mnHotLinks = 1 Then
                tmrHotLink.Enabled = True
                tmrPoll.Enabled = True
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subCreateHotLink", String.Empty)
        End Try

        Return bSuccess

    End Function

    Private Sub subPolledRead(ByRef Link As clsLink)
        '********************************************************************************************
        'Description:  Read the hotlinked PLC data and raise the NewData event if the data
        '              changed since we last looked at it.
        '
        'Parameters: Link - HotLink info
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bFound As Boolean

        Try
            For Each oControl As Control In Me.Controls
                If oControl.Name = Link.ControlName Then
                    Dim oAxActEasyIF As AxACTMULTILib.AxActEasyIF = DirectCast(oControl, AxACTMULTILib.AxActEasyIF)
                    Dim nPlcData(Link.Length - 1) As Integer
                    Dim nReturnCode As Integer

                    Select Case Link.MemType
                        Case "M", "SM" 'Bit type
                            nReturnCode = oAxActEasyIF.ReadDeviceRandom(Link.Address, Link.Length, nPlcData(0))
                        Case Else 'Word/Block Type
                            nReturnCode = oAxActEasyIF.ReadDeviceBlock(Link.Address, Link.Length, nPlcData(0))
                    End Select

                    If nReturnCode = 0 Then
                        'Read Success
                        Dim bNewData As Boolean

                        For nItem As Integer = 0 To nPlcData.GetUpperBound(0)
                            If Not bNewData Then bNewData = (nPlcData(nItem).ToString <> Link.RefData(nItem))
                            Link.RefData(nItem) = nPlcData(nItem).ToString
                        Next 'nItem

                        'Determine if the hotlink should be refreshed
                        If Not bNewData Then
                            Link.SameValueReadCount += 1
                            If Link.SameValueReadCount >= mnREFRESH_INTERVAL Then
                                Link.SameValueReadCount = 0
                                bNewData = True
                            End If
                        End If

                        If bNewData Then RaiseEvent NewData(Link.ZoneName, Link.TagName, Link.RefData)
                        '>>>DEBUG RaiseEvent NewData(Link.ZoneName, Link.TagName, Link.RefData)
                    Else
                        'Read Fail
                        Dim sMsg As String = gcsRM.GetString("csMPLC_STATION") & Link.Station.ToString & ", " & _
                                             gcsRM.GetString("csMPLC_ADDR") & Link.Address
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subPolledRead", "Could not read PLC data." & _
                                               " TagName: " & Link.TagName & " Station: " & Link.Station.ToString & _
                                               ", Address: " & Link.Address & ", Length: " & Link.Length.ToString & _
                                               ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                        RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:subPolledRead", sMsg)
                    End If 'nReturnCode = 0

                    bFound = True
                    Exit For

                End If 'oControl.Name = Link.ControlName
            Next 'oControl

            If Not bFound Then
                'AxActEasyIF control not found in collection
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subPolledRead", Link.ControlName & " control not found in collection.")
            End If

        Catch ex As Exception
            Dim sMsg As String = gcsRM.GetString("csMPLC_STATION") & Link.Station.ToString & ", " & _
                                                 gcsRM.GetString("csMPLC_ADDR") & Link.Address

            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subPolledRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, ex.Message, msMODULE & ":clsPLCComm:subPolledRead", sMsg)
        Finally
            Link.ElapsedTime = 0
            Link.Busy = False
        End Try

    End Sub

    Private Sub subUpdateManualLink(ByVal Link As clsLink)
        '********************************************************************************************
        'Description:  Make sure the AxActEasyIF0 link is in the collection. Update it with new tag
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
            Link.ControlName = "AxActEasyIF0"

            If IsNothing(mLinks.Item("AxActEasyIF0")) Then
                Call mLinks.Add(Link)
            Else
                With mLinks.Item(Link.ControlName)
                    .Address = Link.Address
                    .Station = Link.Station
                    .Length = Link.Length
                    .MemType = Link.MemType
                    .PollRate = Link.PollRate
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

    Private Function LinkPoke(ByRef Link As clsLink) As String()
        '****************************************************************************************
        'Description: This function does the initial data read when a HotLink is created
        '
        'Parameters: Link Info
        'Returns:    PLC Data
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sData(Link.Length - 1) As String
        Dim nPLCData(Link.Length - 1) As Integer

        Try
            For Each oControl As Control In Me.Controls
                If oControl.Name = Link.ControlName Then
                    Dim oAxActEasyIF As AxACTMULTILib.AxActEasyIF = DirectCast(oControl, AxACTMULTILib.AxActEasyIF)
                    Dim nReturnCode As Integer

                    Select Case Link.MemType
                        Case "M", "SM" 'Bit type
                            nReturnCode = oAxActEasyIF.ReadDeviceRandom(Link.Address, Link.Length, nPLCData(0))
                        Case Else 'Word/Block Type
                            nReturnCode = oAxActEasyIF.ReadDeviceBlock(Link.Address, Link.Length, nPLCData(0))
                    End Select

                    If nReturnCode = 0 Then
                        'Read Success
                        For nItem As Integer = 0 To nPLCData.GetUpperBound(0)
                            Link.RefData(nItem) = nPLCData(nItem).ToString
                            sData(nItem) = Link.RefData(nItem)
                        Next 'nItem
                    Else
                        'Read Fail
                        Dim sMsg As String = gcsRM.GetString("csMPLC_STATION") & Link.Station.ToString & ", " & _
                                             gcsRM.GetString("csMPLC_ADDR") & Link.Address
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:LinkPoke", "Could not read PLC data." & _
                                               " TagName: " & Link.TagName & " Station: " & Link.Station.ToString & _
                                               ", Address: " & Link.Address & ", Length: " & Link.Length.ToString & _
                                               ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                        RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:LinkPoke", sMsg)
                    End If 'nReturnCode = 0

                    Exit For

                End If 'oControl.Name = Link.ControlName
            Next 'oControl

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:LinkPoke", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:LinkPoke", ex.Message)
        End Try

        Return sData

    End Function

    Private Function ManualRead(ByVal Link As clsLink) As Boolean
        '****************************************************************************************
        'Description: This function reads data One Time from the plc
        '
        'Parameters: Link Info
        'Returns:    True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nPlcData(Link.Length - 1) As Integer
        Dim nReturnCode As Integer
        Dim bSuccess As Boolean
        ReDim msReadData(Link.Length - 1)

        Try
            mbOTRWInProg = True
            tmrManRW.Enabled = True

            With AxActEasyIF0

                nReturnCode = .Open

                If nReturnCode = 0 Then
                    'Open Success
                    Select Case Link.MemType
                        Case "M", "SM" 'Bit Type
                            nReturnCode = .ReadDeviceRandom(Link.Address, Link.Length, nPlcData(0))
                        Case Else 'All other cases should be data word/block type
                            nReturnCode = .ReadDeviceBlock(Link.Address, Link.Length, nPlcData(0))
                    End Select
                Else
                    'Open fail
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ManualRead", _
                                          "Unable to open PLC communication - Logical Station Number: " & _
                                          Link.Station.ToString & ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                    RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                           msMODULE & ":clsPLCComm:ManualRead", _
                                           gcsRM.GetString("csMPLC_COMM_OPEN_FAIL") & Link.Station.ToString)
                    bSuccess = False
                End If

                If nReturnCode = 0 Then
                    'Read Success
                    nReturnCode = .Close
                Else
                    'Read Fail
                    Dim sMsg As String = gcsRM.GetString("csMPLC_STATION") & Link.Station.ToString & ", " & _
                                         gcsRM.GetString("csMPLC_ADDR") & Link.Address
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ManualRead", "Could not read PLC data." & _
                                           " TagName: " & Link.TagName & " Station: " & Link.Station.ToString & _
                                           ", Address: " & Link.Address & ", Length: " & Link.Length.ToString & _
                                           ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                    RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                           msMODULE & ":clsPLCComm:ManualRead", sMsg)
                    bSuccess = False

                    nReturnCode = .Close 'RJO 04/20/10
                End If

                If nReturnCode = 0 Then
                    'Close Success
                    bSuccess = True
                    For nIndex As Integer = 0 To nPlcData.GetUpperBound(0)
                        msReadData(nIndex) = nPlcData(nIndex).ToString
                    Next
                Else
                    'Close fail
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ManualRead", _
                                          "Unable to close PLC communication - Logical Station Number: " & _
                                          Link.Station.ToString & ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                    RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                           msMODULE & ":clsPLCComm:ManualRead", _
                                           gcsRM.GetString("csMPLC_COMM_CLOSE_FAIL") & Link.Station.ToString)
                    bSuccess = False
                End If

                mbOTRWInProg = False

            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ManualRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:ManualRead", ex.Message)
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
                .Length = CType(oNode.Item("length").InnerXml, Short)
                .MemType = Strings.Left(.Address, 1).ToUpper
                If Strings.Left(.Address, 2).ToUpper = "SM" Then
                    .MemType = "SM"
                End If

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
                .Station = CType(oNode.Item("station").InnerXml, Integer)
                .PollRate = CType(oNode.Item("rate").InnerXml, Integer)
                .ControlName = String.Empty
                .Busy = False
                .ElapsedTime = 0
                .SameValueReadCount = 0
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
                'determine if this is a One-Timer or a HotLink
                If Link.PollRate = mnMANUAL_POLL_RATE Then
                    'One-Time Read
                    Call subUpdateManualLink(Link)
                    Call subConfigControl(AxActEasyIF0, Link)

                    If ManualRead(Link) Then
                        Data = msReadData
                        Return True
                    Else
                        Return False
                    End If 'ManualRead(Link)
                Else
                    'HotLink
                    If CreateHotLink(Link) Then
                        Data = LinkPoke(Link)
                        Return True
                    Else
                        Return False
                    End If 'CreateHotLink(Link)
                End If 'Link.PollRate = mnMANUAL_POLL_RATE

            Else
                Return False
            End If 'Not IsNothing(Link)

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), msMODULE & ":clsPLCComm:ReadPLCData", String.Empty)
        End Try

    End Function

    Friend Function TagParams(ByVal TagName As String) As String
        '********************************************************************************************
        'Description:  TagParams - Debug tool 
        '
        'Parameters: none
        'Returns:    Returns "Address,Length,Station,PollRate" string for current tag
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLink As clsLink = PLCTagInfo(TagName)
        Dim sTagParams As String = "<err>,<err>,<err>,<err>"

        If Not IsNothing(oLink) Then
            sTagParams = oLink.Address & "," & oLink.Length.ToString & "," & oLink.Station.ToString & "," & oLink.PollRate.ToString
        End If

        Return sTagParams

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
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ValidateData", _
                                       "Invalid Data, value = [" & sElement & "], type = " & Link.MemType)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                       msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                Return False
            Else
                bFloat = Strings.InStr(sElement, ".") > 0
            End If

            If bFloat Then
                'No Floating Point allowed for ActEasyIF
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ValidateData", _
                                       "Invalid Data, value = [" & sElement & "], type = " & Link.MemType)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                       msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                Return False
            End If

            'check that it's the correct type for the data Type 
            Select Case Link.MemType

                Case "M", "SM" 'Bit Type
                    If (sElement <> "0") And (sElement <> "1") Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ValidateData", _
                                               "Invalid Data, value = [" & sElement & "], type = " & Link.MemType)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End If

                Case Else 'Word/Block Type
                    Dim nData As Integer

                    Try
                        nData = CType(sElement, Integer)
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ValidateData", _
                                               "Invalid Data, value = [" & sElement & "], type = " & Link.MemType & _
                                               vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End Try

            End Select

        Next 'sElement

        If Link.MemType = "M" Or Link.MemType = "SM" Then
            'Bit Type, length must be 1
            If sData.GetLength(0) = 1 Then Return True
        End If

        If sData.GetLength(0) = Link.Length Then
            'data passed inspection
            Return True
        Else

            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ValidateData", _
                                   "Invalid data length = " & sData.GetLength(0).ToString & _
                                   ", Link.Length = " & Link.Length.ToString)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.WrongArraySize, gcsRM.GetString("csPLC_WRITE_SIZE_ERROR"), _
                                   msMODULE & ":clsPLCComm:ValidateData", "Data length = " & sData.GetLength(0).ToString & _
                                   ", Link.Length = " & Link.Length.ToString)
            Return False
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
                Dim nPlcData(Link.Length - 1) As Integer
                Dim nReturnCode As Integer
                Dim bSuccess As Boolean

                Call subConfigControl(AxActEasyIF0, Link)
                Call subUpdateManualLink(Link)

                For nIndex As Integer = 0 To Data.GetUpperBound(0)
                    nPlcData(nIndex) = CType(Data(nIndex), Integer)
                Next

                mbOTRWInProg = True
                tmrManRW.Enabled = True

                With AxActEasyIF0

                    nReturnCode = .Open

                    If nReturnCode = 0 Then
                        'Open Success
                        Select Case Link.MemType
                            Case "M", "SM" 'Bit Type
                                nReturnCode = .WriteDeviceRandom(Link.Address, Link.Length, nPlcData(0))
                            Case Else 'All other cases should be data word/block type
                                nReturnCode = .WriteDeviceBlock(Link.Address, Link.Length, nPlcData(0))
                        End Select
                    Else
                        'Open fail
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", _
                                               "Unable to open PLC communication - Logical Station Number: " & _
                                               Link.Station.ToString & ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", _
                                               gcsRM.GetString("csMPLC_COMM_OPEN_FAIL") & Link.Station)
                        bSuccess = False
                    End If

                    If nReturnCode = 0 Then
                        'Write Success
                        nReturnCode = .Close
                    Else
                        'Write Fail
                        Dim sMsg As String = gcsRM.GetString("csMPLC_STATION") & Link.Station.ToString & ", " & _
                                             gcsRM.GetString("csMPLC_ADDR") & Link.Address

                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "Could not write PLC data." & _
                                               " TagName: " & Link.TagName & " Station: " & Link.Station.ToString & _
                                               ", Address: " & Link.Address & ", Length: " & Link.Length.ToString & _
                                               ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", sMsg)
                        bSuccess = False
                        nReturnCode = .Close 'RJO 04/20/10
                    End If

                    If nReturnCode = 0 Then
                        'Close Success
                        bSuccess = True
                    Else
                        'Close fail
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", _
                                               "Unable to close PLC communication - Logical Station Number: " & _
                                                Link.Station.ToString & ", Code = " & Hex(nReturnCode).ToString & " (Hex)")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", _
                                               gcsRM.GetString("csMPLC_COMM_CLOSE_FAIL") & Link.Station)
                        bSuccess = False
                    End If

                    mbOTRWInProg = False

                End With

                Return True

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), msMODULE & ":clsPLCComm:WritePLCData", String.Empty)
                Return False
            End Try

        Else
            Return False
        End If  'ValidateData(Data, oLink)

    End Function

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

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tmrHotLink.Interval = 50 'ms
        tmrManRW.Interval = 1000 'ms
        tmrPoll.Interval = 50 'ms

    End Sub

    Private Sub tmrHotLink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHotLink.Tick
        '********************************************************************************************
        ' Description:  This routine queues up hotlink read requests.
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            tmrHotLink.Enabled = False

            For Each oLink As clsLink In mLinks
                If oLink.ControlName <> "AxActEasyIF0" Then
                    oLink.ElapsedTime += tmrHotLink.Interval
                    'TODO - Might want to add some error checking or debug code here to see just how long
                    'these reads are taking.
                    If Not oLink.Busy Then
                        If oLink.ElapsedTime >= oLink.PollRate Then
                            oLink.Busy = True
                            mcolPollQueue.Add(oLink.ControlName)
                        End If
                    End If 'Not oLink.Busy
                End If 'oLink.ControlName <> "AxActEasyIF0"
            Next 'oLink

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrHotLink_Tick", ex.Message & vbCrLf & ex.StackTrace)
        Finally
            tmrHotLink.Enabled = True
        End Try

    End Sub

    Private Sub tmrManRW_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrManRW.Tick
        '********************************************************************************************
        ' Description:  This routine allows the program to error out when a manual (one-time) read 
        '               or write operation takes too long. Needed????
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
                'communication breakdown (it's always the same)
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", _
                                       "AxActEasyIF0 comm timeout.")
                RaiseEvent ModuleError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                       msMODULE & ":clsPLCComm:tmrManRW_Tick", gcsRM.GetString("csMPLC_COMM_TIMEOUT"))
            End If 'mbOTRWInProg

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrManRW_Tick", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(0, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                   msMODULE & ":clsPLCComm:tmrManRW_Tick", ex.Message)
        End Try

        mbOTRWInProg = False

    End Sub

    Private Sub tmrPoll_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPoll.Tick
        '********************************************************************************************
        ' Description:  tmrPoll is a free running timer that calls for the queued hotlink read requests. 
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            tmrPoll.Enabled = False

            'Read the next hotlink in the queue
            If mcolPollQueue.Count > 0 Then
                Dim sItem As String = mcolPollQueue.Item(1).ToString

                If IsNothing(mLinks.Item(sItem)) Then
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrPoll_Tick", _
                                           "Link [" & sItem & "] does not exist.")
                Else
                    Dim oLink As clsLink = mLinks.Item(sItem)

                    Call subPolledRead(oLink)
                End If
                mcolPollQueue.Remove(1)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:tmrPoll_Tick", ex.Message & vbCrLf & ex.StackTrace)
        Finally
            tmrPoll.Enabled = True
        End Try

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
        Private mbBusy As Boolean = False
        Private mnElapsedTime As Integer = 0
        Private mnStation As Integer = 0
        Private mnLength As Short = 0
        Private msMemType As String = String.Empty
        Private mnPollRate As Integer = 0
        Private msRefData() As String
        Private mnSameValueReadCount As Integer = 0
        Private msTagName As String = String.Empty
        Private msZoneName As String = String.Empty
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
            'Description: Name of the ActEasyIF Control associated with this link
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

        Friend Property Station() As Integer
            '********************************************************************************************
            'Description: The Logical Station Number of the PLC that data is being read from/written to.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnStation
            End Get

            Set(ByVal value As Integer)
                mnStation = value
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

        Friend Property SameValueReadCount() As Integer
            '********************************************************************************************
            'Description: The number of times that the value(s) that were read from the PLC during a 
            '             polled read operation matched the reference values. Used to periodically 
            '             refresh hotlink data.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnSameValueReadCount
            End Get

            Set(ByVal value As Integer)
                mnSameValueReadCount = value
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

End Class