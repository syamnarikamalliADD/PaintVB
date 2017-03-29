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
' Form/Module: frmMain
'
' Description:
' 
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
'    08/25/09   MSW     subWriteRecordToSQLDB - add /10 to cycle and cc time
'    04/21/10   MSW     support some ASCII Data
'    04/14/11   MSW     Add ghost column
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    10/07/11   MSW     Change all the shorts to integers.  The only thing they can 
'                       accomplish is causing an overflow error.                        4.1.0.1
'    12/02/11   MSW     Changes to prevent duplicate DMON files                         4.1.1.0
'    01/17/12   MSW     Disable task bar so it doesn't get shut down accidentally       4.1.1.1
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm 4.01.01.02
'    02/15/12   MSW     Force 32 bit build for compatability with PCDK                  4.01.01.03
'    03/28/12   MSW     Move DB to XML                                                  4.01.03.00
'    06/12/12   MSW     Change so screens write change log items to XML and ProdLogger  4.01.04.00
'                       moves to XML to remove 5 second delay for first SQL access on most screens
'    10/10/12   JBW     Ghost fix - it's an integer not a boolean                       4.01.04.01
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances    4.01.04.02
'    04/16/13   MSW     Add Canadian language files                                     4.01.05.00
'                       Change to generic setup                                     
'                       subInitializeForm - support some additional labels for
'                       production report items when indexing for dmon logger
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.01
'    09/30/13   MSW     PLC DLL                                                         4.01.06.00
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    05/26/14   MSW     Add ZDT Monitor                                                 4.01.07.01
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Configuration.ConfigurationSettings
Imports System.Data.OleDb
Imports Connstat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath

Friend Class frmMain

#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "Prodlogger"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Private mcolZones As clsZones = Nothing
    Private WithEvents mcolProdZones As clsProdZones = Nothing
    'Private WithEvents colControllers As clsControllers = Nothing
    'Private WithEvents colArms As clsArms = Nothing
    'Private mRobot As clsArm = Nothing


    'ZDT Monitor
    'Dim colControllers As clsControllers = New clsControllers(mcolZones, False)
    'Dim colArms As clsArms = New clsArms()
    'colArms = LoadArmCollection(colControllers)
    Private moZDTLogger As clsZDTLogger

    Private mcolProdData As New Collection
    Friend mcolProdElements As New Collection

    'Windows Messaging variables
    Private msWinMsg As String = String.Empty
    Private mnMsgCount As Integer = 0
    Private mnMsgLen As Integer = 0
    '******** End Form Variables    *****************************************************************

    ''******** Form Structures   *********************************************************************
    Private Structure udsProdRecord 'Based on PW3 Prodlog Table
        Public sParamData As String
        Public sColumns As String
    End Structure


    Private Structure udsPLCDataElement
        Public FieldName As String      'udsProdRecord Element Name
        Public Offset As Integer        'start word location in PLC Data Table
        Public Length As Integer        'data length (PLC Data Table words)
        Public Type As String           'Production report Data Type {Ascii, BinaryString16, BinaryString32,
        'Boolean, Integer16, Integer32, TextString}
        Public Enable As Boolean        'True = use this data, False = use default data
        Public Scale As Integer         'Scale value for decimals
    End Structure
    '******** End Form Structures   *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** SQL DATABASE Variables     *****************************************************************
    Private mSQLDB As clsSQLAccess
    Private mSQLCmd As SqlClient.SqlCommand
    '******** End SQL DATABASE  Variables    *************************************************************

    Private mbDone As Boolean
    '    04/21/10   MSW     support some ASCII Data
    Friend gbAsciiColor As Boolean = False
    Friend gnAsciiColorNumChar As Integer = 0
    Friend gbAsciiStyle As Boolean = False
    Friend gnAsciiStyleNumChar As Integer = 0

    '********New program-to-program communication object******************************************
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '********************************************************************************************
    Private WithEvents oChangeLogger As clsChangeLogger
#End Region

#Region " Properties "

    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                msROBOT_ASSEMBLY_LOCAL)
            
        End Set

    End Property

    Friend ReadOnly Property DisplayCulture() As Globalization.CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return New Globalization.CultureInfo(msCulture)
        End Get

    End Property

    Friend Property Status(Optional ByVal void As Boolean = False) As String
        '********************************************************************************************
        'Description: just here for compatability
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)

        End Set
    End Property

#End Region

#Region " Routines "

    Private Function BuildAsciiString(ByVal Data() As String) As String
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
            Dim nData(Data.GetUpperBound(0)) As Integer

            For nIndex As Integer = 0 To Data.GetUpperBound(0)
                nData(nIndex) = CType(Data(nIndex), Integer)
                'Get the left character
                nElement = nData(nIndex) And &HFF00
                nElement = nElement \ 256
                If CheckChar(nElement) Then
                    sTemp = sTemp & Chr(nElement)
                End If
                'Get the right character
                nElement = nData(nIndex) And &HFF
                If CheckChar(nElement) Then
                    sTemp = sTemp & Chr(nElement)
                End If
            Next

            Return Strings.Trim(sTemp)

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
            Return String.Empty
        End Try

    End Function

    Private Function BuildBinaryString(ByVal Data() As String, ByVal Length As Integer) As String
        '********************************************************************************************
        'Description: Converts an array of integers to a binary string of the specified length. 
        '
        'Parameters: Data - Array of ascii data expressed as integers
        'Returns:    String 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sReturn As String = String.Empty
        Dim nLength As Integer = Data.GetUpperBound(0)

        Try
            For nDataWord As Integer = Data.GetUpperBound(0) To 0 Step -1
                Dim nData As Integer = CType(Data(nDataWord), Integer)

                For nBit As Integer = 15 To 0 Step -1
                    If (nData And CType(2 ^ nBit, Integer)) > 0 Then
                        sReturn = sReturn & "1"
                    Else
                        sReturn = sReturn & "0"
                    End If
                Next nBit
            Next 'nDataWord

            sReturn = Strings.Right(sReturn, Length)
            '"0" pad the string to get it to the correct length (if necessary)
            If Strings.Len(sReturn) < Length Then
                For nPad As Integer = 1 To (Length - Strings.Len(sReturn))
                    sReturn = "0" & sReturn
                Next 'nPad
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sReturn

    End Function

    Private Function BuildProdRecord(ByVal ProdData() As String) As udsProdRecord
        '********************************************************************************************
        'Description: Construct a Production Data structure using data from the PLC based on the
        '             configuration data from ProdData.XML.
        '
        'Parameters: ProdData - Array of production data from PLC
        'Returns:    udsProdRecord 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/10  MSW     support some ASCII Data
        ' 04/14/11  MSW     Add ghost column
        ' 10/07/11  MSW     Change all the shorts to integers.  The only thing they can accomplish is causing an overflow error.
        ' 10/10/12  JBW     Ghost fix - it's an integer not a boolean
        ' 04/16/13  MSW     Change to generic setup
        '********************************************************************************************
        Dim oProdRec As New udsProdRecord
        Dim nUB As Integer = ProdData.GetUpperBound(0)

        Try
            'Call subInitProdRecord(oProdRec)
            With oProdRec
                .sColumns = "[Zone], [Device]"
                .sParamData = "'" & ProdData(nUB - 1) & "', '" & _
                                ProdData(nUB) & "'"
                For Each oElement As udsPLCDataElement In mcolProdElements
                    Dim sPLCData(oElement.Length - 1) As String
                    Dim sData As String = String.Empty
                    .sColumns = .sColumns & ", [" & oElement.FieldName & "]"
                    .sParamData = .sParamData & ", "
                    For nIndex As Integer = 0 To oElement.Length - 1
                        sPLCData(nIndex) = ProdData(oElement.Offset + nIndex)
                    Next 'nIndex
                    sData = FormatProdData(sPLCData, oElement.Type)
                    Select Case oElement.Type.ToLower
                        Case "int", "integer", "integer16", "integer32"
                            .sParamData = .sParamData & sPLCData(0)
                        Case "text", "textstring"
                            .sParamData = .sParamData & sData
                        Case "asciistyle"
                            .sParamData = .sParamData & mMathFunctions.CvIntegerToASCII(CType(sData, Integer), gnAsciiStyleNumChar)
                        Case "asciicolor"
                            .sParamData = .sParamData & mMathFunctions.CvIntegerToASCII(CType(sData, Integer), gnAsciiColorNumChar)
                        Case "float", "decimal", "numeric", "real"
                            Dim nInt As Integer = CType(sPLCData(0), Integer)
                            Dim nDbl As Double = CType(nInt, Double) / oElement.Scale
                            .sParamData = .sParamData & nDbl.ToString
                        Case "bool", "boolean", "bit"
                            Dim bData As Boolean = CType(sData, Boolean)

                            If bData Then
                                .sParamData = .sParamData & "1"
                            Else
                                .sParamData = .sParamData & "0"
                            End If
                        Case Else
                            .sParamData = .sParamData & sData
                    End Select
                  
                Next 'oElement

            End With 'oProdRec

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return oProdRec

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
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
            Return False
        End Try

    End Function

    Friend Function FormatProdData(ByVal Data() As String, ByVal Type As String) As String
        '********************************************************************************************
        'Description: Format Data() (from PLC) as specified by Type and return a string.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '1/21/09    gks     change to friend to call from proddevice - this will need a decipher 
        '                   vin number routine one day
        '********************************************************************************************
        Dim sReturn As String = String.Empty

        Try

            Select Case Type
                Case "Ascii"
                    sReturn = "'" & BuildAsciiString(Data) & "'"
                Case "BinaryString16"
                    sReturn = "'" & BuildBinaryString(Data, 16) & "'"
                Case "BinaryString32"
                    sReturn = "'" & BuildBinaryString(Data, 32) & "'"
                Case "Boolean"
                    If Data(0) = "0" Then
                        sReturn = "False"
                    Else
                        sReturn = "True"
                    End If
                Case "Integer16", "TextString", "Float"
                    sReturn = Data(0)
                Case "Integer32"
                    Dim nUB As Integer = Data.GetUpperBound(0)

                    For nIndex As Integer = 0 To nUB
                        If (nIndex = 0) And (nUB > 0) Then
                            If Data(nIndex) <> "0" Then sReturn = Data(nIndex)
                        Else
                            sReturn = sReturn & Data(nIndex)
                        End If
                    Next
                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME, "FormatProdData: Unrecognized format specifier [" & Type & "].")
                    sReturn = "0"
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sReturn

    End Function

    Private Function nBitStrToInt(ByVal DataString As String) As Integer
        '********************************************************************************************
        'Description: Converts binary string to an integer. Example: input string "1010101" returns
        '             85. Stolen straight from vb6 - had to get rid of loopie!
        '
        'Parameters: DataString - string of 1's and 0's
        'Returns:   Long Integer value
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim nNewVal As Integer
        Dim nStrLen As Integer
        Dim nIndex As Integer
        Try

            nNewVal = 0
            nStrLen = Strings.Len(DataString)

            For nIndex = 0 To (nStrLen - 1)

                If Strings.Mid(DataString, nStrLen - nIndex, 1) = "1" Then

                    If nIndex < 31 Then
                        nNewVal = CType(nNewVal + 2 ^ nIndex, Integer)
                    Else
                        nNewVal = CType(nNewVal + ((2 ^ nIndex) - 4294967296), Integer)
                    End If

                End If 'Strings.Mid(DataString, StrLen - nIndex, 1) = "1"

            Next 'nIndex

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: nBitStrToInt", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return nNewVal

    End Function

    Private Sub subCheckForPreviousInstance()
        '********************************************************************************************
        'Description:  This sub is the VB.NET equivalent of App.PrevInstance in VB 6
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If Process.GetProcessesByName _
              (Process.GetCurrentProcess.ProcessName).Length > 1 Then
                Dim sMsg As String = "Another instance of Prodlogger.exe is currently running! Exiting application."

                mDebug.WriteEventToLog(msSCREEN_NAME, sMsg)
                Application.Exit()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: subCheckForPreviousInstance", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subDoScreenAction(ByRef DR As DataRow)
        '********************************************************************************************
        'Description:  Prodlogger has received a command from another Paintworks application.
        '
        'Parameters: Command to execute
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            Select Case DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString.ToLower

                Case "setproperties"
                    'Future - for whatever commands we may need to pass to this app.

                Case "close"
                    'Get outta Dodge
                    Application.Exit()
                    Me.Close()

            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: subDoScreenAction", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub



    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/10  MSW     support some ASCII Data
        ' 04/13/13  MSW     subInitializeForm - support some additional labels for
        '                   production report items when indexing for dmon logger
        '********************************************************************************************
        Dim bSuccess As Boolean = True

        Try
            Dim sMsg As String = String.Empty

            'Create the Zones Colection
            mcolZones = New clsZones(String.Empty)

            'Load the Production Data Element collection

            '********New program-to-program communication object******************************************
            oIPC = New Paintworks_IPC.clsInterProcessComm() '(gs_COM_ID_PROD_LOG, , , True) 'RJO 03/09/12
            '******************************************************************************************** 
            sMsg = gpsRM.GetString("psCONFIG_PROD_DEV_MSG", DisplayCulture)
            'Call mWorksComm.SendFRWMMessage("poststatusmsg," & sMsg & ",0,0,0,0", "PW4_Main")
            Dim sMessage(1) As String
            sMessage(0) = "poststatusmsg"
            sMessage(1) = sMsg
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
            Call subLoadData(mcolZones.DatabasePath)

            mSQLDB = New clsSQLAccess
            mSQLDB.DBFileName = gsPRODLOG_DBNAME
            mSQLDB.Zone = mcolZones.ActiveZone
            mSQLDB.DBTableName = gsPROD_DS_TABLENAME

            'Create connections to Production Data
            mcolProdZones = New clsProdZones(mcolZones, True)

            'Get ascii data setup
            Dim oStyles As New clsSysStyles(mcolZones.ActiveZone)
            gbAsciiStyle = oStyles.UseAscii
            gnAsciiStyleNumChar = oStyles.PlantAsciiMaxLength

            oChangeLogger = New clsChangeLogger(mcolZones.ActiveZone)
            Dim sValves() As String = Nothing
            Dim oColors As XMLNodeList = Nothing
            Dim bColorsByStyle As Boolean
            Dim bUse2K As Boolean
            Dim bUseTricoat As Boolean
            Dim bTwoCoats As Boolean
            GetSystemColorInfoFromDB( mcolZones.ActiveZone, oColors, _
                            sValves, gbAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, gnAsciiColorNumChar, bTwoCoats)

            '1/21/09 set pointers for diaglogger
            'mWorksComm sometimes mixes these messages up if more than one app is sending to Main.
            'Disable this message until we have a remoting solution.
            sMsg = gpsRM.GetString("psCONFIG_DIAG_LOGR_MSG", DisplayCulture)
            'Call mWorksComm.SendFRWMMessage("poststatusmsg," & sMsg & ",0,0,0,0", "PW4_Main")
            sMessage(0) = "poststatusmsg"
            sMessage(1) = sMsg
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

            Dim nStyle As Integer
            Dim nColor As Integer
            Dim nOption As Integer
            Dim nVin As Integer
            Dim nVinLen As Integer
            Dim sVinEle As String = String.Empty
            
            For Each oElement As udsPLCDataElement In mcolProdElements
                Select Case oElement.FieldName
                    Case "PlantStyle", "Plant Style"
                        nStyle = oElement.Offset
                        If gbAsciiStyle Then
                            oElement.Type = "text"
                        End If
                    Case "PlantColor", "Plant Color"
                        nColor = oElement.Offset
                        If gbAsciiColor Then
                            oElement.Type = "text"
                        End If
                    Case "PlantOption", "Plant Option"
                        nOption = oElement.Offset
                    Case "ProdID", "VIN Number"
                        nVin = oElement.Offset
                        nVinLen = oElement.Length
                        sVinEle = oElement.Type
                End Select
                If oElement.FieldName.ToLower.Contains("vin") Or oElement.FieldName.ToLower.Contains("bssn") Then
                    nVin = oElement.Offset
                    nVinLen = oElement.Length
                    sVinEle = oElement.Type
                End If
            Next

            For Each oDevices As clsProdDevices In mcolProdZones
                For Each ProdDev As clsProdDevice In oDevices
                    With ProdDev
                        .ColorPointer = nColor
                        .OptionPointer = nOption
                        .StylePointer = nStyle
                        .VINLength = nVinLen
                        .VINPointer = nVin
                        .VINElementType = sVinEle
                    End With
                Next 'ProdDev
            Next 'oDevices 

            mcolProdZones.MonitorStatus()
            'TODO - Need to verify that this works
            'tmrCheckTime.Enabled = True



            'ZDT Monitor
            Dim colControllers As clsControllers = New clsControllers(mcolZones, False)
            Dim colArms As clsArms = New clsArms()
            colArms = LoadArmCollection(colControllers)
            moZDTLogger = New clsZDTLogger(mcolZones, colArms)

        Catch ex As Exception
            Dim sErrMsg As String = gpsRM.GetString("psLOAD_ERROR", DisplayCulture)

            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
            bSuccess = False
            'Call mWorksComm.SendFRWMMessage("poststatusmsg," & sErrMsg & ",0,0,0,0", "PW4_Main")
            Dim sMessage(1) As String
            sMessage(0) = "poststatusmsg"
            sMessage(1) = sErrMsg
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

        Finally
            'Call mWorksComm.SendFRWMMessage("prodlogrload," & bSuccess.ToString & ",0,0,0,0", "PW4_Main")
            Dim sMessage(1) As String
            sMessage(0) = "prodlogrload"
            sMessage(1) = bSuccess.ToString
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
        End Try

    End Sub

    Private Sub subLoadData(ByVal DataFilePath As String)
        '********************************************************************************************
        'Description:  Load Production Data configuration info into mcolProdElements
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DS As New DataSet
        Dim DR As DataRow = Nothing

        Try
            DS.ReadXmlSchema(DataFilePath & "XML\ProdData.xsd")
            DS.ReadXml(DataFilePath & "XML\ProdData.xml")

            For Each DR In DS.Tables("PLCDataElement").Rows
                'Check the enable to see if we're even looking at this one
                If CType(DR.Item("Enable"), Boolean) = True Then
                    Dim oDataElement As New udsPLCDataElement

                    With oDataElement
                        .FieldName = DR.Item("FieldName").ToString
                        .Offset = CType(DR.Item("Offset"), Integer)
                        .Length = CType(DR.Item("Length"), Integer)
                        If .Length = 0 Then
                            .Length = 1
                        End If
                        .Type = DR.Item("Type").ToString
                        Try
                            .Scale = CType(DR.Item("Scale"), Integer)
                            If .Scale = 0 Then
                                .Scale = 1
                            End If
                        Catch ex As Exception
                            .Scale = 1
                        End Try
                    End With ' oDataElement

                    mcolProdElements.Add(oDataElement)

                End If 'CType(DR.Item("Enable")...
            Next 'DR

            DS.Dispose()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub subProcessCommandLine()
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSeparators As String = " "
        Dim sCommands As String = Microsoft.VisualBasic.Command()
        Dim sArgs() As String = sCommands.Split(sSeparators.ToCharArray)

    End Sub

    Private Sub subWriteRecordToSQLDB(ByVal ProdRecord As udsProdRecord)
        '********************************************************************************************
        'Description:  Write the Production Record Data to the Production Log Database
        '
        'Parameters: ProdRecord
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/09  MSW     add /10 to cycle and cc time
        ' 04/16/10  RJO     Added code to check (and restore if necessary) SQL Server connection.
        ' 04/14/11  MSW     Add ghost column
        ' 04/16/13  MSW     Change to generic setup
        '******************************************************************************************** 

        Try
            Dim cmd As SqlClient.SqlCommand = mSQLDB.GetStoredProcedureCommand("AddProductionRecordFlex")


            'add parameters to command object
            With cmd.Parameters
                .Add(New SqlClient.SqlParameter("@Columns", SqlDbType.NVarChar))
                .Add(New SqlClient.SqlParameter("@Data", SqlDbType.NVarChar))

                .Item("@Columns").Value = ProdRecord.sColumns
                .Item("@Data").Value = ProdRecord.sParamData

                'ship it
                'make sure we have a connection to SQL Server first 'RJO 04/16/10
                If mSQLDB.ConnectionOpen Then
                    Debug.Print(ProdRecord.sColumns)
                    Debug.Print(ProdRecord.sParamData)
                    cmd.ExecuteNonQuery()
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: subWriteRecordToSQLDB", _
                                    "Error: The SQL DB connection could not be opened.")
                End If

            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: subWriteRecordToSQLDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try

    End Sub


#End Region

#Region " Events "


    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description:  Bye Bye, Farewell, Auf Wiedersehen, Good Night.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If mSQLDB IsNot Nothing Then
                mSQLDB.Close()
                mSQLDB.Dispose()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: Finalize", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        MyBase.Finalize()
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, _
                        ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

        Catch ex As Exception

        End Try

    End Sub

    Private Sub frmMain_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        '********************************************************************************************
        'Description: I'm shy
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Hide()

    End Sub

    Private Sub frmMain_Disposed(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Disposed
        Try
            mSQLDB.Dispose()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: frmMain_Disposed", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, _
                        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: dont close from the x
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.CloseReason
            Case CloseReason.UserClosing
                e.Cancel = True
        End Select

    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Runs after class constructor (new)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subCheckForPreviousInstance()

        Try
            Dim sCultureArg As String = "/culture="

            'If a culture string has been passed in, set the current culture (display language)
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    Culture = s.Remove(0, sCultureArg.Length)
                    Exit For
                End If
            Next

            Call subInitializeForm()

        Catch ex As Exception

            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                        " Routine: frmMain_Load", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        Finally
            Me.Show()
            Me.Left = 111000 ' this shows up on oven plc programming terminal
        End Try

    End Sub

    Private Sub mcolProdZones_ProdNotification(ByVal ZoneName As String, ByVal DeviceName As String, _
                                ByVal ProdData() As String) Handles mcolProdZones.ProdNotification
        '********************************************************************************************
        'Description:  New Production data has just been received from DeviceName. Put it in a 
        '              collection.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nUB As Integer = ProdData.GetUpperBound(0)
            ReDim Preserve ProdData(nUB + 2)

            ProdData(nUB + 1) = ZoneName
            ProdData(nUB + 2) = DeviceName

            mcolProdData.Add(ProdData)
            tmrNewData.Enabled = True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & _
                                    " Routine: mcolProdZones_ProdNotification", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description:  frmMain has just been instantiated. Do some initialization.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub tmrNewData_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrNewData.Tick
        '********************************************************************************************
        'Description:  Retrieve Production from the collection and log it in the Production Log.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrNewData.Enabled = False

        Try

            If mcolProdData.Count > 0 Then
                Dim ProdData() As String = DirectCast(mcolProdData.Item(1), String())
                Dim ProdRec As udsProdRecord = BuildProdRecord(ProdData)

                'MS Acess database
                'Call subWriteRecordToDB(ProdRec)
                'SQL database
                Call subWriteRecordToSQLDB(ProdRec)

                mcolProdData.Remove(1)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME, ex.Message & vbCrLf & ex.StackTrace)
            'if you have an error writing to database, get rid of the thing anyway
            Try
                mcolProdData.Remove(1)
            Catch ex1 As Exception
                'really in trouble now
                mcolProdData.Clear()
            End Try
        Finally
            If mcolProdData.Count > 0 Then tmrNewData.Enabled = True
        End Try

    End Sub


    '********New program-to-program communication object******************************************
    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As DataSet) Handles oIPC.NewMessage
        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Dim DR As DataRow = Nothing

            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    DR = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)
                    Call subDoScreenAction(DR)
                Case Else
            End Select
        End If
    End Sub
    '********************************************************************************************

#End Region

End Class