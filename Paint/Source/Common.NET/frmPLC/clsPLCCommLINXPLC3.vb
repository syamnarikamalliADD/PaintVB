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
' Form/Module: clsPLCCommLINXPLC3
'
' Description: Class to use rslinx and OPC to talk to a PLC3
' 
'
' Dependancies:  prescription drug
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports RsiOPCAuto
Imports System.Array

Friend Class clsPLCComm

#Region "  Declares  "

    '***** XML Setup****************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "RSLINXPLC3Comm.cfg" ' "RSLINXPLC3.xml"
    Private Const msXMLPREFIX As String = "pw4" '"fra"
    Private Const msXMLURN As String = "urn:AB_Plc3"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup***********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "frmRSLINXPLC3"
    Private Const msSERVERNAME As String = "RSLinx OPC Server"
    Private Const msREMSERVERNAME As String = "RSLinx Remote OPC Server"
    '***** End Module Constants   **************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private mbServerSet As Boolean = False
    Private msDefaultGroup As String = String.Empty
    Private msManAddress As String = String.Empty
    Private mnManLen As Integer = 0
    Private msManTopic As String = String.Empty
    Private mnManPoll As Integer = 0
    Private mbManTagActive As Boolean = False
    Private msRemoteComputer As String = String.Empty

    '***** End Property Vars *******************************************************************

    '******* Events *****************************************************************************
    'when hotlink has new data 
    Friend Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                                        ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events *************************************************************************

    '***** Working Vars ************************************************************************
    Private WithEvents moServer As OPCServer
    Private WithEvents moGroup As OPCGroup
    Private mcolLinks As clsLinks
    '***** End Working Vars *********************************************************************

    '***** Enums        ************************************************************************
    Private Enum eOPC_QUALITY
        BAD = &H0
        UNCERTAIN = &H40
        GOOD = &HC0
        CONFIG_ERROR = &H4
        NOT_CONNECTED = &H8
        DEVICE_FAILURE = &HC
        SENSOR_FAILURE = &H10
        LAST_KNOWN = &H14
        COMM_FAILURE = &H18
        OUT_OF_SERVICE = &H1C
        LAST_USABLE = &H44
        SENSOR_CAL = &H50
        EGU_EXCEEDED = &H54
        SUB_NORMAL = &H58
        LOCAL_OVERRIDE = &HD8
    End Enum
    Private Enum eOPC_STATUS
        RUNNING = &H1
        FAILED = &H2
        NOCONFIG = &H3
        SUSPENDED = &H4
        TEST = &H5
    End Enum
    '***** End Enums       *********************************************************************


    'to be removed when using xml
    Private Declare Function GetPrivateProfileString Lib "kernel32" _
                     Alias "GetPrivateProfileStringA" _
                     (ByVal lpSection As String, ByVal lpEntry$, _
                     ByVal lpDefault As String, _
                     ByVal lpReturnedString As String, _
                     ByVal nSize As Integer, _
                     ByVal lpFileName As String) As Integer



#End Region
#Region "  Properties"

    Friend WriteOnly Property Zone() As clsZone
        '********************************************************************************************
        'Description:  This is to take the place of Zonename - set this with current zone this 
        '               carries dbpath etc with it. should already be checked for available etc.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As clsZone)


            If value.IsRemoteZone Then
                msRemoteComputer = value.ServerName
            Else
                msRemoteComputer = String.Empty
            End If

            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, msRemoteComputer, msXMLFILENAME) Then
                msZoneName = value.Name
            Else
                msZoneName = String.Empty
                Dim sTmp As String = gcsRM.GetString("csCANT_FIND_CONFIG_FILE") & _
                                " " & gcsRM.GetString("csZONE_NAME") & ":=" & Zonename
                Trace.WriteLine(sTmp)
                RaiseEvent ModuleError(0, sTmp & value.Name, _
                                                            msMODULE, String.Empty)
            End If

            'should add ping here??

        End Set
    End Property
    Friend Property Zonename() As String
        '********************************************************************************************
        'Description:  Zonename required to be set first
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
        Set(ByVal value As String)
            Call subSetZoneName(value)
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
            Call subSetTagName(value)
        End Set
    End Property
    Friend Property PLCData() As String()
        '********************************************************************************************
        'Description:  Read or write PLC Data - All data in and out is string array, one word data
        '               is done as string(0).  Link is a hotlink if poll rate is > 0 if zero it is a
        '               one time read/write. Hotlink data returned via newdata event, but this must
        '               be called to start hotlink
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
            If bReadPLCData(sData) Then
                mbManTagActive = False
                Return sData
            Else

                Dim sTmp As String = gcsRM.GetString("csCOULD_NOT_READ_PLC")
                Dim sTmp1 As String = gcsRM.GetString("csZONE_NAME")
                sTmp1 = sTmp1 & ":=" & Zonename & ", " & _
                                 gcsRM.GetString("csTAG_NAME") & ":=" & TagName
                Trace.WriteLine(sTmp & " " & sTmp1)
                RaiseEvent ModuleError(0, sTmp, msMODULE, sTmp1)
                mbManTagActive = False
                Return Nothing
            End If
        End Get
        Set(ByVal value As String())
            If bWritePLCData(value) = False Then

                Dim sTmp As String = gcsRM.GetString("csCOULD_NOT_WRITE_PLC")
                Dim sTmp1 As String = gcsRM.GetString("csZONE_NAME")
                sTmp1 = sTmp1 & ":=" & Zonename & ", " & _
                                 gcsRM.GetString("csTAG_NAME") & ":=" & TagName
                Trace.WriteLine(sTmp & " " & sTmp1)
                RaiseEvent ModuleError(0, sTmp, msMODULE, sTmp1)

                ''''''''''''''temp
                '''''''''''''MessageBox.Show(sTmp)

            End If
            mbManTagActive = False
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
    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msDefaultGroup
        End Get
    End Property

#End Region
#Region "  Routines "

    Friend Sub BuildATag(ByVal Address As String, ByVal Length As Integer, _
                            ByVal Topicname As String, ByVal PollRate As Integer)
        '********************************************************************************************
        'Description:  This is a wilmington Specific routine so the tag file doesnt get out of hand
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msManAddress = Address
        mnManLen = Length
        msManTopic = Topicname
        mnManPoll = PollRate
        mbManTagActive = True

    End Sub
    Private Sub subSetZoneName(ByVal sZonename As String)
        '********************************************************************************************
        'Description:  this determines where we look for tag info
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If sZonename = String.Empty Then
            msZoneName = sZonename
            Dim sTmp As String = gcsRM.GetString("csMISSING_ZONE_NAME")
            Trace.WriteLine(sTmp)
            RaiseEvent ModuleError(0, sTmp, msMODULE, String.Empty)
            Exit Sub
        End If

        Try
            If sZonename <> String.Empty Then
                ' no change
                If sZonename = msZoneName Then Exit Sub
            End If

            If Strings.Left(sZonename, 2) = "\\" Then
                'remote machine
                'TODO
            Else
                'local machine
                If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, msRemoteComputer, msXMLFILENAME) Then
                    msZoneName = sZonename
                Else
                    msZoneName = String.Empty
                    Dim sTmp As String = gcsRM.GetString("csCANT_FIND_CONFIG_FILE") & _
                                    " " & gcsRM.GetString("csZONE_NAME") & ":=" & Zonename
                    Trace.WriteLine(sTmp)
                    RaiseEvent ModuleError(0, sTmp & sZonename, _
                                                                msMODULE, String.Empty)
                End If
            End If


        Catch ex As Exception
            msZoneName = String.Empty
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, String.Empty)
        End Try

    End Sub
    Private Overloads Sub subRemoveLinkItem(ByRef olink As clsLink, ByVal RemoveHotlink As Boolean)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (olink.PollRate > 0) And RemoveHotlink Then
            Call subRemoveLink(olink)
        End If
    End Sub
    Private Overloads Sub subRemoveLinkItem(ByRef oLink As clsLink)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Call subRemoveLink(oLink)
    End Sub
    Private Sub subRemoveLink(ByRef olink As clsLink)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim h(1) As Integer
        Dim e(1) As Integer
        h(1) = olink.ServerHandle
        Dim ah As Array = CType(h, Array)
        Dim err As Array = CType(e, Array)

        'don't remove hotlink
        If olink.PollRate > 0 Then Exit Sub

        Try
            moGroup.OPCItems.Remove(1, ah, err)
        Catch ex As Exception

        End Try

    End Sub
    Private Sub subSetTagName(ByVal sTagname As String)
        '********************************************************************************************
        'Description:  use tagname to get required parameters
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            'check if zone is set
            If msZoneName = String.Empty Then
                Dim sTmp As String = gcsRM.GetString("csMISSING_ZONE_NAME")
                Trace.WriteLine(sTmp)
                RaiseEvent ModuleError(0, sTmp, msMODULE, String.Empty)
                Exit Sub
            End If

            msTagName = sTagname

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, String.Empty)
        End Try

    End Sub
    Private Function bGetLinkInfo(ByRef oLink As clsLink) As Boolean
        '********************************************************************************************
        'Description:  create a clsLink - makes sure server and group exist
        '
        'Parameters: clsLink variable
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If msZoneName = String.Empty Then Return False
        If msTagName = String.Empty Then Return False



        Try

            If bReadTagInfo(oLink) Then
                'got what we need to read- now read it
                If bConnectServer(msRemoteComputer) Then

                Else
                    Return False
                End If  'bConnectServer
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function bReadPLCData(ByRef sData() As String) As Boolean
        '********************************************************************************************
        'Description:  This is for  read of the data - also called on hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLink As clsLink = Nothing
        'make sure we are reading the device not memory
        Dim Source As Short = CType(OPCDataSource.OPCDevice, Short)

        Try
            ' this also starts server and adds group if necessary
            If bGetLinkInfo(oLink) Then
                'create the linkitem
                If bAddLinkItem(oLink) Then
                    ' this is syncread
                    oLink.Item.Read(Source)

                    If oLink.Item.Quality = eOPC_QUALITY.GOOD Then
                        If oLink.LinkLength = 1 Then
                            '1 word read
                            Dim sVal As Short = CType(oLink.Item.Value, Short)
                            ReDim sData(0)
                            sData(0) = CStr(sVal)
                        Else
                            Dim sVal As Short()
                            sVal = CType(CType(oLink.Item.Value, Array), Short())
                            Dim ub% = UBound(sVal)
                            ReDim sData(ub%)
                            Dim i%
                            For i% = 0 To ub%
                                sData(i%) = CStr(sVal(i%))
                            Next
                        End If

                        subRemoveLinkItem(oLink)

                        Return True

                    Else
                        'no good data, now what?
                        Trace.WriteLine("Not good read quality. quality =" & oLink.Item.Quality)
                        'Property raises error
                        subRemoveLinkItem(oLink)
                        Return False
                    End If  'oLink.Item.Quality = mnGOODQUALITY
                Else
                    Trace.WriteLine("bAddLinkItem(oLink) returned false for read")
                    Return False
                End If  'bAddLinkItem(oLink)
            Else
                Trace.WriteLine("bGetLinkInfo(oLink) returned false for read")
                Return False
            End If  'bGetLinkInfo(oLink)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, "bReadPLCData")
            Return False
        End Try

    End Function
    Private Function bAddLinkItem(ByRef oLink As clsLink) As Boolean
        '********************************************************************************************
        'Description:  this sets up a new link item and adds to collection of links if a hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            If moGroup.IsActive Then moGroup.IsActive = False

            'need to do something about this
            moGroup.OPCItems.DefaultAccessPath = oLink.Topic

            oLink.Item = moGroup.OPCItems.AddItem(oLink.Address, moGroup.OPCItems.Count + 1)
            oLink.GroupName = moGroup.Name
            'this is the link identifier to grab out of collection
            oLink.Handle = oLink.Item.ClientHandle
            oLink.ServerHandle = oLink.Item.ServerHandle
            oLink.ZoneName = Zonename

            If oLink.PollRate <> 0 Then
                'Hotlink
                moGroup.UpdateRate = oLink.PollRate
                moGroup.IsSubscribed = True

                If mcolLinks Is Nothing Then
                    mcolLinks = New clsLinks
                End If

                mcolLinks.Add(oLink)

            End If

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, "bAddLinkItem")
            Return False
        Finally
            moGroup.IsActive = True
        End Try


    End Function
    Private Function bWritePLCData(ByVal sData() As String) As Boolean
        '********************************************************************************************
        'Description:  Write data to the PLC
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLink As clsLink = Nothing
        Dim shVal As Short()

        'is data OK?
        Try
            Dim i%
            Dim ub% = UBound(sData)
            ReDim shVal(ub%)
            For i% = 0 To ub%
                shVal(i%) = CType(sData(i%), Short)
            Next
        Catch ex As Exception
            'this should catch the bum data if sent
            Dim sTmp As String = gcsRM.GetString("csBAD_DATA_PLC_WRITE")
            Trace.WriteLine(sTmp)
            RaiseEvent ModuleError(0, sTmp, msMODULE, String.Empty)
            Return False
        End Try

        Try
            ' this also starts server and adds group if necessary
            If bGetLinkInfo(oLink) Then
                'create the linkitem
                If bAddLinkItem(oLink) Then
                    'I object!
                    oLink.Item.Write(CType(sData, Object))
                    'it appears that you cant trust the quality feedback after a write
                    ' I don't know how to tell if it failed...
                    subRemoveLinkItem(oLink)
                    Return True
                    ' ''If oLink.Item.Quality = eOPC_QUALITY.GOOD Then
                    ' ''    subRemoveLinkItem(oLink)
                    ' ''    Return True
                    ' ''Else
                    ' ''    'no good data, now what?
                    ' ''    Trace.WriteLine("Not good write quality. quality =" & oLink.Item.Quality)
                    ' ''    'Property raises error
                    ' ''    subRemoveLinkItem(oLink)
                    ' ''    Return False
                    ' ''End If  'oLink.Item.Quality = mnGOODQUALITY
                Else
                    Trace.WriteLine("bAddLinkItem(oLink) returned false for write")
                    Return False
                End If  'bAddLinkItem(oLink)
            Else
                Trace.WriteLine("bGetLinkInfo(oLink) returned false for write")
                Return False
            End If  'bGetLinkInfo(oLink)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, String.Empty)
            Return False
        End Try

    End Function
    Private Function bAddGroup(ByVal sGroupName As String) As Boolean
        '********************************************************************************************
        'Description: check to see if group exists, if not add it 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bFound As Boolean = False

        If sGroupName = String.Empty Then
            sGroupName = "WhatsHisName"
        End If

        Try

            If moServer.OPCGroups.Count = 0 Then
                moGroup = moServer.OPCGroups.Add(sGroupName)
                bFound = True
            Else
                Dim o As OPCGroup
                For Each o In moServer.OPCGroups
                    If o.Name = sGroupName Then
                        moGroup = moServer.OPCGroups.GetOPCGroup(sGroupName)
                        bFound = True
                    End If
                Next
            End If

            If Not bFound Then
                'has groups but not right one
                moGroup = moServer.OPCGroups.Add(sGroupName)
            End If

            With moGroup
                'todo - find out what this stuff does
                .IsActive = False
                .IsSubscribed = False
                .UpdateRate = 1000
                .DeadBand = 0
                .TimeBias = 0
            End With

            Return True


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, "bAddGroup")
            Return False
        End Try

    End Function
    Private Function bConnectServer(ByVal sRemoteName As String) As Boolean
        '********************************************************************************************
        'Description: Get a hold of the server
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Select Case moServer.ServerState
                Case OPCServerState.OPCRunning
                    'are we on right machine?
                    If moServer.ServerNode <> sRemoteName Then
                        'add code to switch servers here
                        moServer.Disconnect()
                        If sRemoteName = String.Empty Then
                            'local
                            moServer.Connect(msSERVERNAME)
                        Else
                            'remote
                            If Strings.Right(sRemoteName, 1) = "\" Then
                                sRemoteName = Strings.Left(sRemoteName, Strings.Len(sRemoteName) - 1)
                            End If
                            moServer.Connect(msREMSERVERNAME, sRemoteName)
                        End If

                    End If
                Case OPCServerState.OPCDisconnected
                    'connect me!
                    If sRemoteName = String.Empty Then
                        'local
                        moServer.Connect(msSERVERNAME)
                    Else
                        'remote
                        If Strings.Right(sRemoteName, 1) = "\" Then
                            sRemoteName = Strings.Left(sRemoteName, Strings.Len(sRemoteName) - 1)
                        End If
                        moServer.Connect(msREMSERVERNAME, sRemoteName)
                    End If
                Case Else
                    'need to do something here too
                    Dim sTmp As String = gcsRM.GetString("csOPCSERVER_STATE_ERR") & _
                                                    moServer.ServerState
                    Trace.WriteLine(sTmp)
                    RaiseEvent ModuleError(0, sTmp, msMODULE, String.Empty)
                    Return False
            End Select

            'add default group
            If bAddGroup(DefaultGroupName) Then Return True

            'oh crap
            Return False

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(0, ex.Message, msMODULE, String.Empty)
            Return False
        End Try

    End Function
    Friend Function FormatFromTimerValue(ByVal ValueIn As Integer) As String
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
        Return Format(fTmp, "0.0")

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
    Friend Function RemoveHotLinks() As Boolean
        '********************************************************************************************
        'Description:  get rid of hotlinks
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For i As Integer = mcolLinks.Count To 1 Step -1
                mcolLinks.Remove(mcolLinks.Item(i))
            Next

            Application.DoEvents()

            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Return False

        End Try
    End Function

#End Region
#Region "  Events  "
    Friend Sub New()
        '****************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        moServer = New OPCServer
        msDefaultGroup = My.Application.Info.AssemblyName

    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Private Sub moGroup_DataChange(ByVal TransactionID As Integer, ByVal NumItems As Integer, _
                    ByRef ClientHandles As System.Array, ByRef ItemValues As System.Array, _
                    ByRef Qualities As System.Array, ByRef TimeStamps As System.Array) _
                    Handles moGroup.DataChange
        '****************************************************************************************
        'Description: this is where the data from the hotlink comes in
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim i%, j%
        If TransactionID <> 0 Then
            ' this is an asyncrefresh callback
        Else
            ' this is a hotlink
        End If

        Try
            For i% = 1 To NumItems
                Select Case CType(Qualities.GetValue(i%), Integer)
                    Case eOPC_QUALITY.GOOD
                        'get the linkobject for this data
                        Dim o As clsLink = mcolLinks.Item(CType(ClientHandles.GetValue(i%), Integer))

                        'todo - find out why?
                        If o Is Nothing Then Exit Sub

                        If o.LinkLength = 1 Then
                            Dim v As Short = CType(ItemValues.GetValue(i%), Short)
                            Dim s(0) As String
                            s(0) = CType(v, String)
                            RaiseEvent NewData(o.ZoneName, o.Tagname, s)
                        Else
                            Dim v As Short() = CType(ItemValues.GetValue(i%), Short())
                            Dim s(UBound(v)) As String
                            For j% = 0 To UBound(v)
                                s(j%) = CType(v(j%), String)
                            Next
                            RaiseEvent NewData(o.ZoneName, o.Tagname, s)
                        End If
                    Case Else
                        'just pretend it never happened
                        'Trace.WriteLine("bad quality on datachange event - quality =" _
                        '        & CType(Qualities.GetValue(i%), Integer).ToString & Now)
                End Select
            Next

        Catch ex As Exception
            Trace.WriteLine(ex.Message)

        End Try

    End Sub
    Private Sub moServer_ServerShutDown(ByVal Reason As String) Handles _
                                                            moServer.ServerShutDown

    End Sub
#End Region
#Region " Temp for config file "
    'to be removed when using xml
    Private Function bReadTagInfo(ByRef o As clsLink) As Boolean
        '****************************************************************************************
        'Description: This function checks that all properties are set so we can proceed
        '               - Reminder tagnames are case sensitive
        'Parameters: None
        'Returns:   True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim sProfString As String = "*Error*"

        Const sPARAM_DELIM As String = ","

        If mbManTagActive = False Then
            '' this is the normal route

            'Read the Tag Parameters into a structure
            sProfString = sGetConfigString("Tagnames", TagName)
            Select Case sProfString
                Case "", "*Error*"
                    Return False
                Case Else
                    Dim sParams() As String

                    sParams = Split(sProfString, sPARAM_DELIM)
                    If UBound(sParams) <> 3 Then
                        'TODO - Localize Error Description
                        ''subErrorEvent(PLCCommErrors.ConfigItemNotDefined, _
                        ''              "Section: Tagnames, Entry: " & TagName & " not found.")
                        Return False
                    Else
                        'expect a tag that looks like "N300:0,L1,Manual,0"
                        o = New clsLink
                        With o
                            .Tagname = TagName
                            .LinkLength = CType(sParams(1), Integer)
                            .PollRate = CType(sParams(3), Integer)
                            .Topic = sParams(2)  'This topic must be already set up in rslinx
                            .Address = sParams(0)
                        End With
                    End If
            End Select

        Else
            'manual one time read/write
            o = New clsLink
            With o
                .Tagname = "Custom"
                .LinkLength = mnManLen
                .PollRate = mnManPoll
                .Topic = msManTopic  'This topic must be already set up in rslinx
                .Address = msManAddress
            End With

        End If

        'got everything
        Return True

    End Function
    Private Function sGetConfigString(ByVal sSection As String, ByVal sItem As String) As String
        '********************************************************************************************
        ' This Function returns a msDELIM separated parameter string from the PLC Tag configuration
        ' file.
        '
        ' Parameters: sSection as String ; Data section in config file
        '             sItem as String ; Data Item in config file
        ' Returns: Comma separated config string
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nCommentLoc As Integer = 0
        Dim lStringLength As Long = 0
        Dim sConfigData As String = String.Empty

        Const nMAX_STRING_LEN As Integer = 100
        Const sCOMMENT_DELIM As String = ";"

        sGetConfigString = String.Empty
        sConfigData = Space(nMAX_STRING_LEN)

        If Dir(msXMLFilePath) <> "" Then
            lStringLength = GetPrivateProfileString(sSection, sItem, "*Error*", sConfigData, _
                                                    nMAX_STRING_LEN, msXMLFilePath)
            nCommentLoc = InStr(sConfigData, sCOMMENT_DELIM)

            If nCommentLoc > 0 Then
                ' Remove comments
                sGetConfigString = Trim(Strings.Left(sConfigData, nCommentLoc - 1))
            Else
                sGetConfigString = Trim(sConfigData)
            End If 'nCommentLoc > 0

        Else
            ''TODO - Localize Error Description
            'subErrorEvent(PLCCommErrors.ConfigFileNotFound, _
            '              "File " & XMLFilePath & " not found.")
        End If ' Dir(XMLFilePath) <> ""

        If sGetConfigString = "*Error*" Then
            ''TODO - Localize Error Description
            'subErrorEvent(PLCCommErrors.ConfigItemNotDefined, _
            '              "Section:" & sSection & ", Entry:" & sItem & " not found.")
        End If

    End Function
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
        Private mnHandle As Integer = 0
        Private msTagName As String = String.Empty
        Private mnLinkLen As Integer = 0
        Private mnPollRate As Integer = 0
        Private msTopic As String = String.Empty
        Private msAddress As String = String.Empty
        Private msGroupName As String = String.Empty
        Private msZoneName As String = String.Empty
        Private moItem As OPCItem = Nothing
        Private mnServerHandle As Integer = 0
#End Region
#Region " Properties "

        Friend Property ServerHandle() As Integer
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
                Return mnServerHandle
            End Get
            Set(ByVal value As Integer)
                mnServerHandle = value
            End Set
        End Property
        Friend Property Handle() As Integer
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
                Return mnHandle
            End Get
            Set(ByVal value As Integer)
                mnHandle = value
            End Set
        End Property
        Friend Property Tagname() As String
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
                Return msTagName
            End Get
            Set(ByVal value As String)
                msTagName = value
            End Set
        End Property
        Friend Property LinkLength() As Integer
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
                Return mnLinkLen
            End Get
            Set(ByVal value As Integer)
                If value < 1 Then Exit Property
                mnLinkLen = value
            End Set
        End Property
        Friend Property PollRate() As Integer
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
                Return mnPollRate
            End Get
            Set(ByVal value As Integer)
                If value < 0 Then Exit Property
                mnPollRate = value
            End Set
        End Property
        Friend Property Topic() As String
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
                Return msTopic
            End Get
            Set(ByVal value As String)
                msTopic = value
            End Set
        End Property
        Friend Property Address() As String
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
                Return msAddress & ",L" & mnLinkLen
            End Get
            Set(ByVal value As String)
                msAddress = value
            End Set
        End Property
        Friend Property GroupName() As String
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
                Return msGroupName
            End Get
            Set(ByVal value As String)
                msGroupName = value
            End Set
        End Property
        Friend Property ZoneName() As String
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
                Return msZoneName
            End Get
            Set(ByVal value As String)
                msZoneName = value
            End Set
        End Property
        Friend Property Item() As OPCItem
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
                Item = moItem
            End Get
            Set(ByVal value As OPCItem)
                moItem = value
            End Set
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
            moItem = Nothing
            MyBase.Finalize()
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

        End Sub
#End Region
    End Class  'clsLink

    Private Class clsLinks
        '********************************************************************************************
        'Description:  This class is to hold the clsLinks
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Inherits CollectionBase

#Region " Declares"
        Dim mCol As Collection
#End Region
#Region " Properties "
        Friend Overloads ReadOnly Property Count() As Integer
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
                Return mCol.Count
            End Get
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
            mCol = New Collection
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
            mCol = Nothing
            MyBase.Finalize()
        End Sub

#End Region
#Region " Routines "

        Friend Sub Add(ByRef oLink As clsLink)
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
            mCol.Add(oLink)
        End Sub
        Friend Sub Remove(ByRef oLink As clsLink)
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
            Try
                Dim oServer As OPCServer = New OPCServer
                Dim oGroups As OPCGroups = oServer.OPCGroups

                If oGroups.Count = 0 Then Exit Sub
                For Each o As OPCGroup In oGroups
                    If o.Name = oLink.GroupName Then
                        Dim h(1) As Integer
                        Dim e(1) As Integer
                        h(1) = oLink.ServerHandle
                        Dim ah As Array = CType(h, Array)
                        Dim err As Array = CType(e, Array)
                        o.OPCItems.Remove(1, ah, err)

                    End If

                Next

            Catch ex As Exception

            End Try

        End Sub
        Friend Function Item(ByVal ItemHandle As Integer) As clsLink
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
            Try
                Dim o As clsLink
                For Each o In mCol
                    If o.Handle = ItemHandle Then
                        Return o
                    End If
                Next
                'not found
                Return Nothing

            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Return Nothing
            End Try
        End Function

#End Region

    End Class  'clsLinks

End Class ' clsPLCComm -clsPLCCommLINXPLC3
