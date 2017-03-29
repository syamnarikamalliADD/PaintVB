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
' Form/Module: frmPLCComm
'
' Description: Class for PLC access
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: 
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************

Public Class frmPLCComm
    Inherits System.Windows.Forms.Form

    '***** XML Setup****************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "GEFanucComm.cfg" ' "GEFPlcTags.xml"
    Private Const msXMLPREFIX As String = "pw4" '"fra"
    Private Const msXMLURN As String = "urn:Gef_Plc"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup***********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "frmGefPLC"
    '***** End Module Constants   **************************************************************

    '******* Enums  ****************************************************************************
    Private Enum eDT
        eBit
        eInteger
        eDWord
    End Enum
    '******* End Enums  ************************************************************************

    '***** Property Vars ***********************************************************************
    'info to read tags
    Private msTagName As String = String.Empty
    Private msZoneName As String = "Topics" 'Default to [Topics] Section of config file.
    'Private mnZoneNumber As Integer = 0
    ' well - how long is it??
    Private mnLinkLen As Short = 1
    Private mnUserLength As Short = 0
    ' well - how long has it been??
    Private mnPollRate As Short = 0
    '***** End Property Vars *******************************************************************


    '***** Working Vars ************************************************************************
    Private mnDataType As eDT
    ' so we dont check params every time on a hotlink
    Private mbParmsOK As Boolean = False
    ' stores the data read from the PLC
    Private msReadData(0) As String
    Friend WithEvents AxGECtl1 As AxGECTLLib.AxGECtl
    '***** End Working Vars *********************************************************************

    '******* Events *****************************************************************************
    'when hotlink has new data 
    Public Event NewData()

    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Public Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events *************************************************************************

    '******* Declares ***************************************************************************
    Private Declare Function GetPrivateProfileString Lib "kernel32" _
                         Alias "GetPrivateProfileStringA" _
                         (ByVal lpSection As String, ByVal lpEntry$, _
                         ByVal lpDefault As String, _
                         ByVal lpReturnedString As String, _
                         ByVal nSize As Integer, _
                         ByVal lpFileName As String) As Integer
    Private Structure TagParams
        Friend MemoryType As String
        Friend Address As Integer
        Friend LinkLen As Short
        Friend Topic As String
        Friend PollRate As Short
        Friend IPAddress As String
    End Structure
    Private moZone As clsZone
    '******* End Declares ***********************************************************************

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call

        'set config file path to default - can be modified later
        GetDefaultFilePath(XMLFilePath, mPWCommon.eDir.VBApps, "", msXMLFILENAME)

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPLCComm))
        Me.AxGECtl1 = New AxGECTLLib.AxGECtl
        CType(Me.AxGECtl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AxGECtl1
        '
        Me.AxGECtl1.Enabled = True
        Me.AxGECtl1.Location = New System.Drawing.Point(106, 9)
        Me.AxGECtl1.Name = "AxGECtl1"
        Me.AxGECtl1.OcxState = CType(resources.GetObject("AxGECtl1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxGECtl1.Size = New System.Drawing.Size(90, 24)
        Me.AxGECtl1.TabIndex = 1
        '
        'frmPLCComm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(208, 45)
        Me.Controls.Add(Me.AxGECtl1)
        Me.Name = "frmPLCComm"
        Me.Text = "PLC Communications"
        CType(Me.AxGECtl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Friend Property Zone() As clsZone
        Get
            Return moZone
        End Get
        Set(ByVal value As clsZone)
            moZone = value
        End Set
    End Property
    Public ReadOnly Property LinkLength() As Short
        '****************************************************************************************
        'Description: This Property is the number of elements in the link 
        '
        'Parameters: None
        'Returns:   length of link
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            LinkLength = mnLinkLen
        End Get
    End Property

    Public Property PlcData() As String()
        Get
            'do we need to set up?
            If mbParmsOK = False Then
                'check and setup tag info
                If Not bReadTagInfo(True) Then
                    mbParmsOK = False
                    Return msReadData
                End If
            End If

            'return the data
            Return msReadData

        End Get

        Set(ByVal Value() As String)

            'these should be one time writes, check params every time
            If Not bReadTagInfo(False) Then
                subErrorEvent(PLCCommErrors.WriteFailed)
                Exit Property
            End If

            'is it long enough?
            If (LinkLength - 1) <> UBound(Value) Then
                subErrorEvent(PLCCommErrors.WrongArraySize)
                Exit Property
            End If

            'write the data
            If bWriteDataToPLC(Value) = False Then
                subErrorEvent(PLCCommErrors.WriteFailed)
            End If

        End Set

    End Property

    Public ReadOnly Property PollRate() As Short
        '****************************************************************************************
        'Description: This Property is the Polling interval 
        '
        'Parameters: None
        'Returns:   Interval
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            PollRate = mnPollRate
        End Get
    End Property

    Public Property TagName() As String
        '****************************************************************************************
        'Description: This Property is the name of the tag we are interested in 
        '
        'Parameters: Tag Name
        'Returns:   Tag Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            TagName = msTagName
        End Get
        Set(ByVal sTagName As String)
            msTagName = sTagName
            mbParmsOK = False
        End Set

    End Property

    Public Property UserLength() As Short
        '****************************************************************************************
        'Description: This Property allows the data length specified in the Tag file to be
        '             overridden.
        '
        'Parameters: Tag Name
        'Returns:   Tag Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            UserLength = mnUserLength
        End Get
        Set(ByVal value As Short)
            mnUserLength = value
        End Set
    End Property

    Public ReadOnly Property XMLFileName() As String
        '****************************************************************************************
        'Description: This Property exposes the Tag File name to the application that is using 
        '             this form.
        '
        'Parameters: None
        'Returns:   TagFile Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            XMLFileName = msXMLFILENAME
        End Get
    End Property

    Public Property XMLFilePath() As String
        '****************************************************************************************
        'Description: This Property is the path and file Name of the XML File that stores
        '               the plc config tags
        '
        'Parameters: Path string
        'Returns:   Path string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            XMLFilePath = msXMLFilePath
        End Get
        Set(ByVal sPath As String)
            msXMLFilePath = sPath
        End Set

    End Property

    Public Property ZoneName() As String
        '****************************************************************************************
        'Description: This Property is the name of the Zone we are interested in  - typically
        '               from the cboZone dropbox
        '
        'Parameters: Zone Name
        'Returns:   Zone Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            ZoneName = msZoneName
        End Get
        Set(ByVal Value As String)
            msZoneName = Value
            mbParmsOK = False
        End Set

    End Property

    'Private Function bReadTagInfo(ByVal bReadData As Boolean) As Boolean
    '    '****************************************************************************************
    '    'Description: This function checks that all properties are set so we can proceed
    '    '               - Reminder tagnames are case sensitive
    '    'Parameters: None
    '    'Returns:   True if all OK
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '*****************************************************************************************

    '    Dim iRet As Integer
    '    Dim xNode As Xml.XmlNode
    '    Dim sxPath As String
    '    Dim nTmp As Integer
    '    Dim nTmpLen As Integer

    '    'do we have all the info?
    '    mbParmsOK = bRequiredPropertiesSet()
    '    ' bail if we are missing something
    '    If Not mbParmsOK Then Return False

    '    'first get info about the PLC
    '    sxPath = "PLC[ZoneNumber='" & ZoneNumber & "']"

    '    If GetXmlNode(XMLFilePath, sxPath, msXMLPREFIX, msXMLURN, xNode) = _
    '                                                    fRet.Success Then

    '        AxGECtl1.AutoPoll(0)
    '        AxGECtl1.Host = xNode.Item("IPAddress").InnerText
    '        If bReadData Then
    '            AxGECtl1.Function = 0   'read
    '        Else
    '            AxGECtl1.Function = 1   'write
    '        End If
    '    Else
    '        Return False
    '    End If

    '    ' now get the Tag info - Reminder tagnames are case sensitive
    '    sxPath = "Tags/Zone[ZoneNumber='" & ZoneNumber & "']/Tag[TagName='" & TagName & "']"

    '    If GetXmlNode(XMLFilePath, sxPath, msXMLPREFIX, msXMLURN, xNode) = _
    '                                                                    fRet.Success Then

    '        mnLinkLen = xNode.Item("LinkLen").InnerText
    '        'resize the return data array
    '        ReDim msReadData(mnLinkLen - 1)

    '        nTmp = nGetRegisterType(xNode.Item("MemoryType").InnerText)

    '        mnPollRate = xNode.Item("PollRate").InnerText

    '        With AxGECtl1
    '            .RegType = nTmp
    '            .Address = xNode.Item("Address").InnerText
    '            .AutoPoll(mnPollRate)
    '            'Manual read?
    '            If mnPollRate = 0 Then
    '                .Trigger()
    '                Application.DoEvents()
    '                ' this forces a redo in case the same address is
    '                ' manually read more than once - get fresh data
    '                mbParmsOK = False
    '            End If
    '        End With
    '    Else
    '        Return False
    '    End If

    '    'got everything
    '    Return True

    'End Function
    Private Function bReadTagInfo(ByVal bReadData As Boolean) As Boolean
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

        Dim oTagParams As TagParams
        Dim sProfString As String

        Const sPARAM_DELIM As String = ","

        'do we have all the info?
        mbParmsOK = bRequiredPropertiesSet()
        ' bail if we are missing something
        If Not mbParmsOK Then Return False

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
                    subErrorEvent(PLCCommErrors.ConfigItemNotDefined, _
                                  "Section: Tagnames, Entry: " & TagName & " not found.")
                    Return False
                Else
                    With oTagParams
                        .MemoryType = sParams(0)
                        .Address = CType(sParams(1), Integer)
                        .LinkLen = CType(sParams(2), Short)
                        .Topic = sParams(3)
                    End With
                End If
        End Select

        sProfString = sGetConfigString(ZoneName, oTagParams.Topic)
        Select Case sProfString
            Case "", "*Error*"
                Return False
            Case Else
                Dim sParams() As String

                sParams = Split(sProfString, sPARAM_DELIM)
                If UBound(sParams) <> 1 Then
                    'TODO - Localize Error Description
                    subErrorEvent(PLCCommErrors.ConfigItemNotDefined, _
                                  "Section: " & ZoneName & ", Entry: " & oTagParams.Topic & " not found.")
                    Return False
                Else
                    With oTagParams
                        .PollRate = CType(sParams(0), Short)
                        .IPAddress = sParams(1)
                    End With
                End If
        End Select

        'first get info about the PLC
        AxGECtl1.AutoPoll(0)
        AxGECtl1.Host = oTagParams.IPAddress
        If bReadData Then
            AxGECtl1.Function = 0   'read
        Else
            AxGECtl1.Function = 1   'write
        End If

        If UserLength > 0 Then
            mnLinkLen = UserLength
        Else
            mnLinkLen = oTagParams.LinkLen
        End If
        'UserLength is only valid for 1 use. Reset it here in case we forget.
        UserLength = 0

        'resize the return data array
        ReDim msReadData(LinkLength - 1)

        mnPollRate = oTagParams.PollRate

        With AxGECtl1
            .RegType = nGetRegisterType(oTagParams.MemoryType)
            .Address = oTagParams.Address
            .AutoPoll(PollRate)
            'Manual read?
            If PollRate = 0 Then
                .Trigger()
                Application.DoEvents()
                ' this forces a redo in case the same address is
                ' manually read more than once - get fresh data
                mbParmsOK = False
            End If
        End With

        'got everything
        Return True

    End Function

    Private Function bRequiredPropertiesSet() As Boolean
        '****************************************************************************************
        'Description: This function checks that all properties are set so we can proceed
        '
        'Parameters: None
        'Returns:   True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        If ZoneName = vbNullString Then
            subErrorEvent(PLCCommErrors.ZoneNumberNotSet)
            Return False
        End If

        If TagName = vbNullString Then
            subErrorEvent(PLCCommErrors.TagNameNotSet)
            Return False
        End If

        If XMLFilePath = vbNullString Then
            subErrorEvent(PLCCommErrors.ConfigFileNotFound)
            Return False
        End If


        Return True

    End Function

    Private Function bValidateData(ByVal sData As String()) As Boolean
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
        Dim nUBound As Integer = 0
        Dim bFloatFound As Boolean = False
        Dim nIndex As Integer = 0

        nUBound = UBound(sData)

        'check that its numeric - this should also catch null values
        For nIndex = 0 To nUBound
            If IsNumeric(sData(nIndex)) = False Then
                subErrorEvent(PLCCommErrors.InvalidData)
                Return False
            End If
        Next

        For nIndex = 0 To nUBound
            If InStr(sData(nIndex), ".") <> 0 Then
                bFloatFound = True
                Exit For
            End If
        Next

        Select Case mnDataType
            Case eDT.eBit
                If bFloatFound Then
                    subErrorEvent(PLCCommErrors.InvalidData)
                    Return False
                End If

                For nIndex = 0 To nUBound
                    If (sData(nIndex) <> "1") And (sData(nIndex) <> "0") Then
                        subErrorEvent(PLCCommErrors.InvalidData)
                        Return False
                    End If
                Next

            Case eDT.eDWord

            Case eDT.eInteger
                If bFloatFound Then
                    subErrorEvent(PLCCommErrors.InvalidData)
                    Return False
                End If

                Dim void As Short
                ' check if its an integer16
                Try
                    For nIndex = 0 To nUBound
                        void = CType(sData(nIndex), Short)
                    Next

                Catch ex As Exception
                    subErrorEvent(PLCCommErrors.InvalidData)
                    Return False
                End Try

        End Select  'mnDataType


        'data passed inspection
        Return True

    End Function

    Private Function bWriteDataToPLC(ByVal sData As String()) As Boolean
        '****************************************************************************************
        'Description: This function writes the data to the plc
        '
        'Parameters: array of string values
        'Returns:   True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************    
        Dim nIndex As Short

        If bValidateData(sData) Then

            With AxGECtl1
                For nIndex = 0 To CType(UBound(sData), Short)
                    Select Case mnDataType
                        Case eDT.eBit
                            If sData(nIndex) = "1" Then
                                .set_Bit(nIndex, True)
                            Else
                                .set_Bit(nIndex, False)
                            End If

                        Case eDT.eDWord
                            .set_FloatVal(nIndex, CType(sData(nIndex), Single))

                        Case eDT.eInteger
                            .set_WordVal(nIndex, CType(sData(nIndex), Short))

                    End Select
                Next

                'do the write
                .Trigger()

                Return True
            End With    'AxGECtl1

        Else
            Return False
        End If  ' bValidateData(sData)

    End Function

    Private Function nGetRegisterType(ByVal sType As String) As Short
        '****************************************************************************************
        'Description: This function gets an integer to tell the contol what kind of memory its
        '               Reading
        '
        'Parameters: String memory type
        'Returns:   Integer for control
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nRetVal As Short

        'default to 16 bit integer
        mnDataType = eDT.eInteger

        Select Case sType
            Case "I" '0 - %I  Discrete Inputs
                mnDataType = eDT.eBit
                nRetVal = 0
            Case "Q" '1 - %Q  Discrete Outputs
                mnDataType = eDT.eBit
                nRetVal = 1
            Case "T" '2 - %T  Discrete Temporaries
                mnDataType = eDT.eBit
                nRetVal = 2
            Case "M" '3 - %M  Discrete Internals
                mnDataType = eDT.eBit
                nRetVal = 3
            Case "SA" '4 - %SA Discrete Status A-Table
                nRetVal = 4
            Case "SB" '5 - %SB Discrete Status B-Table
                nRetVal = 5
            Case "SC" '6 - %SC Discrete Status C-Table
                nRetVal = 6
            Case "S" '7 - %S  Discrete Status (READ ONLY)
                nRetVal = 7
            Case "G" '8 - %G  Genius Global Data
                nRetVal = 8
            Case "AI" '9 - %AI Analog Inputs
                nRetVal = 9
            Case "AQ" '10 - %AQ    Analog Outputs
                nRetVal = 10
            Case "R" '11 - %R Registers
                nRetVal = 11
            Case "DWord" '11 - %R Registers 2 at a time
                mnDataType = eDT.eDWord
                nRetVal = 11
        End Select

        ' set up the size of the link here
        If mnDataType = eDT.eDWord Then
            AxGECtl1.CtlSize = CType(LinkLength * 2, Short)
        Else
            AxGECtl1.CtlSize = LinkLength
        End If

        Return nRetVal

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

        If Dir(XMLFilePath) <> "" Then
            lStringLength = GetPrivateProfileString(sSection, sItem, "*Error*", sConfigData, _
                                                    nMAX_STRING_LEN, XMLFilePath)
            nCommentLoc = InStr(sConfigData, sCOMMENT_DELIM)

            If nCommentLoc > 0 Then
                ' Remove comments
                sGetConfigString = Trim(Microsoft.VisualBasic.Left(sConfigData, nCommentLoc - 1))
            Else
                sGetConfigString = Trim(sConfigData)
            End If 'nCommentLoc > 0

        Else
            'TODO - Localize Error Description
            subErrorEvent(PLCCommErrors.ConfigFileNotFound, _
                          "File " & XMLFilePath & " not found.")
        End If ' Dir(XMLFilePath) <> ""

        If sGetConfigString = "*Error*" Then
            'TODO - Localize Error Description
            subErrorEvent(PLCCommErrors.ConfigItemNotDefined, _
                          "Section:" & sSection & ", Entry:" & sItem & " not found.")
        End If

    End Function
    Private Sub subErrorEvent(ByVal nErrorNumber As Integer, _
                        Optional ByVal sErrDesc As String = vbNullString)
        '****************************************************************************************
        'Description: Raise error to the calling program
        '
        'Parameters: Error number and description
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim sDesc As String

        sDesc = sErrDesc

        RaiseEvent ModuleError(nErrorNumber, sDesc, Me.Name, _
                                            ZoneName & " " & TagName)

    End Sub

    Private Sub AxGECtl1_OnErrorEvent(ByVal sender As Object, _
            ByVal e As AxGECTLLib._DGECtlEvents_OnErrorEventEvent) Handles AxGECtl1.OnErrorEvent

        Dim i%

        'If mnPollRate > 0 Then
        '    'if we disconnect up poll to 10 seconds
        '    AxGECtl1.AutoPoll(10000)
        'End If
        'AxGECtl1.AutoPoll(0)
        For i% = 0 To UBound(msReadData)
            msReadData(i%) = String.Empty
        Next

        'subErrorEvent(AxGECtl1.ErrorCode, AxGECtl1.ErrorString)
        'ship empty array
        RaiseEvent NewData()

    End Sub

    Private Sub AxGECtl1_OnReadDone(ByVal sender As Object, ByVal e As System.EventArgs) Handles AxGECtl1.OnReadDone


        '****************************************************************************************
        'Description: This is called by the control when it is done reading the PLC
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim nIndex As Short

        For nIndex = 0 To CType(mnLinkLen - 1, Short)
            Select Case mnDataType
                Case eDT.eBit
                    If AxGECtl1.get_Bit(nIndex) Then
                        msReadData(nIndex) = "1"
                    Else
                        msReadData(nIndex) = "0"
                    End If
                Case eDT.eInteger
                    msReadData(nIndex) = AxGECtl1.get_WordVal(nIndex).ToString
                Case eDT.eDWord
                    msReadData(nIndex) = AxGECtl1.get_FloatVal(nIndex).ToString
            End Select
        Next

        RaiseEvent NewData()

    End Sub



    Protected Overrides Sub Finalize()
        'AxGECtl1.AutoPoll(0)

        MyBase.Finalize()
    End Sub

    Private Sub AxGECtl1_OnReadDone1(ByVal sender As Object, ByVal e As System.EventArgs) Handles AxGECtl1.OnReadDone

    End Sub
End Class
