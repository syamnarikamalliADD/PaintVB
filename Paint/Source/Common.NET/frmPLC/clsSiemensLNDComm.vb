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
' Form/Module: clsSiemensLNDComm
'
' Description: Class to use Libnodave DLL to communicate a Siemens S7 PLC
' 
' Dependencies: Windows\System32\libnodave.dll 
'
' Language: Microsoft Visual Basic .Net 2008
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
' 02/18/2013    RJO     Initial Code                                               4.00.00.00
' 08/27/2013    BTK     Various changes for reading different data types.          4.00.00.01
' 10/16/2013    BTK     Made changes to update last operation time when doing      4.00.00.02
'                       a manual read.  If we don't do this then we assume the 
'                       connection is bad and try to restart it.  If the
'                       connection isn't bad and we close and open connections
'                       we use up the available connections on the PLC.
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.Xml

Friend Class clsPLCComm

#Region "  Declares  "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsSiemensLNDComm"
    Private Const mnMANUAL_POLL_RATE As Integer = 0 'A poll rate of this value specifies a One-Time-Read vs. a HotLink
    Private Const mnMAX_RETRY_COUNT As Integer = 3 'Maximum Onetime read retries
    Private Const mnHOTLINK_POLL_RATE As Integer = 100 'Frequncy (ms) to check if hotlinks need to be updated
    Private Const mnREFRESH_COUNT As Integer = 10 'after this many same data polled reads, send data as new data anyway
    Private Const msDELIMITER As String = ","
    '***** End Module Constants  ***************************************************************

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "SiemensLNDComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Property Vars ***********************************************************************
    Private mbRead As Boolean
    Private mnTimeout As Integer
    Private mPLCType As ePLCType = ePLCType.S7_400
    Private msRemoteComputer As String = String.Empty
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
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

    '******* Functions *************************************************************************
    Private Declare Function GetTickCount Lib "kernel32" () As Integer
    '******* End Functions *********************************************************************

    '***** Working Vars ************************************************************************
    Private mbBusy As Boolean 'A read or write operation is in progress
    Private WithEvents mConnection As clsConnection = Nothing
    Private mConnections As New clsConnections
    Private mLinks As New clsLinks
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************
    Friend Enum ePLCType 'This might be useful in the future, but is not used currently
        S7_200 = 1
        S7_300 = 2
        S7_400 = 3
    End Enum
    '***** End Enums ***************************************************************************

#End Region

#Region " LibNoDave DLL Declares "

    '***** LibNoDave Constants *****************************************************************
    '
    '    Protocol types to be used with newInterface:
    '
    Private Const daveProtoMPI As Integer = 0      '  MPI for S7 300/400
    Private Const daveProtoMPI2 As Integer = 1    '  MPI for S7 300/400, "Andrew's version"
    Private Const daveProtoMPI3 As Integer = 2    '  MPI for S7 300/400, Step 7 Version, not yet implemented
    Private Const daveProtoPPI As Integer = 10    '  PPI for S7 200
    Private Const daveProtoAS511 As Integer = 20    '  S5 via programming interface
    Private Const daveProtoS7online As Integer = 50    '  S7 using Siemens libraries & drivers for transport
    Private Const daveProtoISOTCP As Integer = 122 '  ISO over TCP
    Private Const daveProtoISOTCP243 As Integer = 123 '  ISO o?ver TCP with CP243
    Private Const daveProtoMPI_IBH As Integer = 223   '  MPI with IBH NetLink MPI to ethernet gateway */
    Private Const daveProtoPPI_IBH As Integer = 224   '  PPI with IBH NetLink PPI to ethernet gateway */
    Private Const daveProtoUserTransport As Integer = 255 '  Libnodave will pass the PDUs of S7 Communication to user defined call back functions.
    '
    '    ProfiBus speed constants:
    '
    Private Const daveSpeed9k As Integer = 0
    Private Const daveSpeed19k As Integer = 1
    Private Const daveSpeed187k As Integer = 2
    Private Const daveSpeed500k As Integer = 3
    Private Const daveSpeed1500k As Integer = 4
    Private Const daveSpeed45k As Integer = 5
    Private Const daveSpeed93k As Integer = 6
    '
    '    S7 specific constants:
    '
    Private Const daveBlockType_OB As String = "8"
    Private Const daveBlockType_DB As String = "A"
    Private Const daveBlockType_SDB As String = "B"
    Private Const daveBlockType_FC As String = "C"
    Private Const daveBlockType_SFC As String = "D"
    Private Const daveBlockType_FB As String = "E"
    Private Const daveBlockType_SFB As String = "F"
    '
    ' Use these constants for parameter "area" in daveReadBytes and daveWriteBytes
    '
    Private Const daveSysInfo As Integer = &H3      '  System info of 200 family
    Private Const daveSysFlags As Integer = &H5   '  System flags of 200 family
    Private Const daveAnaIn As Integer = &H6      '  analog inputs of 200 family
    Private Const daveAnaOut As Integer = &H7     '  analog outputs of 200 family
    Private Const daveP As Integer = &H80          ' direct access to peripheral adresses
    Private Const daveInputs As Integer = &H81
    Private Const daveOutputs As Integer = &H82
    Private Const daveFlags As Integer = &H83 'Marker data (Mbits M0 - MXXXX
    Private Const daveDB As Integer = &H84 '  data blocks
    Private Const daveDI As Integer = &H85  '  instance data blocks
    Private Const daveV As Integer = &H87      ' don't know what it is
    Private Const daveCounter As Integer = 28  ' S7 counters
    Private Const daveTimer As Integer = 29    ' S7 timers
    Private Const daveCounter200 As Integer = 30       ' IEC counters (200 family)
    Private Const daveTimer200 As Integer = 31         ' IEC timers (200 family)
    '
    Private Const daveOrderCodeSize As Integer = 21    ' Length of order code (MLFB number)
    '
    '    Library specific:
    '
    '
    '    Result codes. Genarally, 0 means ok,
    '    >0 are results (also errors) reported by the PLC
    '    <0 means error reported by library code.
    '
    Private Const daveResOK As Integer = 0                        ' means all ok
    Private Const daveResNoPeripheralAtAddress As Integer = 1     ' CPU tells there is no peripheral at address
    Private Const daveResMultipleBitsNotSupported As Integer = 6  ' CPU tells it does not support to read a bit block with a
    ' length other than 1 bit.
    Private Const daveResItemNotAvailable200 As Integer = 3       ' means a a piece of data is not available in the CPU, e.g.
    ' when trying to read a non existing DB or bit bloc of length<>1
    ' This code seems to be specific to 200 family.
    Private Const daveResItemNotAvailable As Integer = 10         ' means a a piece of data is not available in the CPU, e.g.
    ' when trying to read a non existing DB
    Private Const daveAddressOutOfRange As Integer = 5            ' means the data address is beyond the CPUs address range
    Private Const daveWriteDataSizeMismatch As Integer = 7        ' means the write data size doesn't fit item size
    Private Const daveResCannotEvaluatePDU As Integer = -123
    Private Const daveResCPUNoData As Integer = -124
    Private Const daveUnknownError As Integer = -125
    Private Const daveEmptyResultError As Integer = -126
    Private Const daveEmptyResultSetError As Integer = -127
    Private Const daveResUnexpectedFunc As Integer = -128
    Private Const daveResUnknownDataUnitSize As Integer = -129
    Private Const daveResShortPacket As Integer = -1024
    Private Const daveResTimeout As Integer = -1025
    '
    '    Max number of bytes in a single message.
    '
    Private Const daveMaxRawLen As Integer = 2048
    '
    '    Some definitions for debugging:
    '
    Private Const daveDebugRawRead As Integer = &H1            ' Show the single bytes received
    Private Const daveDebugSpecialChars As Integer = &H2       ' Show when special chars are read
    Private Const daveDebugRawWrite As Integer = &H4           ' Show the single bytes written
    Private Const daveDebugListReachables As Integer = &H8     ' Show the steps when determine devices in MPI net
    Private Const daveDebugInitAdapter As Integer = &H10       ' Show the steps when Initilizing the MPI adapter
    Private Const daveDebugConnect As Integer = &H20           ' Show the steps when connecting a PLC
    Private Const daveDebugPacket As Integer = &H40
    Private Const daveDebugByte As Integer = &H80
    Private Const daveDebugCompare As Integer = &H100
    Private Const daveDebugExchange As Integer = &H200
    Private Const daveDebugPDU As Integer = &H400      ' debug PDU handling
    Private Const daveDebugUpload As Integer = &H800   ' debug PDU loading program blocks from PLC
    Private Const daveDebugMPI As Integer = &H1000
    Private Const daveDebugPrintErrors As Integer = &H2000     ' Print error messages
    Private Const daveDebugPassive As Integer = &H4000
    Private Const daveDebugErrorReporting As Integer = &H8000
    Private Const daveDebugOpen As Integer = &H8000
    Private Const daveDebugAll As Integer = &H1FFFF
    '***** End LibNoDave Constants *************************************************************

    '***** LibNoDave DLL Sub/Function Declares *************************************************
    '
    '    Set and read debug level:
    '
    Private Declare Sub daveSetDebug Lib "libnodave.dll" (ByVal level As Integer)
    Private Declare Function daveGetDebug Lib "libnodave.dll" () As Integer
    '
    ' You may wonder what sense it might make to set debug level, as you cannot see
    ' messages when you opened excel or some VB application from Windows GUI.
    ' You can invoke Excel from the console or from a batch file with:
    ' <myPathToExcel>\Excel.Exe <MyPathToXLS-File>VBATest.XLS >ExcelOut
    ' This will start Excel with VBATest.XLS and all debug messages (and a few from Excel itself)
    ' go into the file ExcelOut.
    '
    '    Error code to message string conversion:
    '    Call this function to get an explanation for error codes returned by other functions.
    '
    '
    ' The folowing doesn't work properly. A VB string is something different from a pointer to char:
    '
    ' Private Declare Function daveStrerror Lib "libnodave.dll" Alias "daveStrerror" (ByVal en As integer) As String
    '
    Private Declare Function daveInternalStrerror Lib "libnodave.dll" Alias "daveStrerror" (ByVal en As Integer) As Integer
    ' So, I added another function to libnodave wich copies the text into a VB String.
    ' This function is still not useful without some code araound it, so I call it "internal"
    Private Declare Sub daveStringCopy Lib "libnodave.dll" (ByVal internalPointer As Integer, ByVal s As String)
    '
    ' Setup a new interface structure using a handle to an open port or socket:
    '
    Private Declare Function daveNewInterface Lib "libnodave.dll" (ByVal fd1 As Integer, ByVal fd2 As Integer, ByVal name As String, ByVal localMPI As Integer, ByVal protocol As Integer, ByVal speed As Integer) As Integer
    '
    ' Setup a new connection structure using an initialized daveInterface and PLC's MPI address.
    ' Note: The parameter di must have been obtained from daveNewinterface.
    '
    Private Declare Function daveNewConnection Lib "libnodave.dll" (ByVal di As Integer, ByVal mpi As Integer, ByVal Rack As Integer, ByVal Slot As Integer) As Integer
    '
    '    PDU handling:
    '    PDU is the central structure present in S7 communication.
    '    It is composed of a 10 or 12 byte header,a parameter block and a data block.
    '    When reading or writing values, the data field is itself composed of a data
    '    header followed by payload data
    '
    '    retrieve the answer:
    '    Note: The parameter dc must have been obtained from daveNewConnection.
    '
    Private Declare Function daveGetResponse Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '    send PDU to PLC
    '    Note: The parameter dc must have been obtained from daveNewConnection,
    '          The parameter pdu must have been obtained from daveNewPDU.
    '
    Private Declare Function daveSendMessage Lib "libnodave.dll" (ByVal dc As Integer, ByVal pdu As Integer) As Integer
    '******
    '
    'Utilities:
    '
    '****
    '*
    '    Hex dump PDU:
    '
    Private Declare Sub daveDumpPDU Lib "libnodave.dll" (ByVal pdu As Integer)
    '
    '    Hex dump. Write the name followed by len bytes written in hex and a newline:
    '
    Private Declare Sub daveDump Lib "libnodave.dll" (ByVal name As String, ByVal pdu As Integer, ByVal length As Integer)
    '
    '    names for PLC objects. This is again the internal function. Use the wrapper code below.
    '
    Private Declare Function daveInternalAreaName Lib "libnodave.dll" Alias "daveAreaName" (ByVal en As Integer) As Integer
    Private Declare Function daveInternalBlockName Lib "libnodave.dll" Alias "daveBlockName" (ByVal en As Integer) As Integer
    '
    '   swap functions. They change the byte order, if byte order on the computer differs from
    '   PLC byte order:
    '
    Private Declare Function daveSwapIed_16 Lib "libnodave.dll" (ByVal x As Integer) As Integer
    Private Declare Function daveSwapIed_32 Lib "libnodave.dll" (ByVal x As Integer) As Integer
    '
    '    Data conversion convenience functions. The older set has been removed.
    '    Newer conversion routines. As the terms WORD, INT, INTEGER etc have different meanings
    '    for users of different programming languages and compilers, I choose to provide a new
    '    set of conversion routines named according to the bit length of the value used. The 'U'
    '    or 'S' stands for unsigned or signed.
    '
    '
    '    Get a value from the position b points to. B is typically a pointer to a buffer that has
    '    been filled with daveReadBytes:
    '
    Private Declare Function toPLCfloat Lib "libnodave.dll" (ByVal f As Single) As Single
    Private Declare Function daveToPLCfloat Lib "libnodave.dll" (ByVal f As Single) As Integer
    '
    ' Copy and convert value of 8,16,or 32 bit, signed or unsigned at position pos
    ' from internal buffer:
    '
    Private Declare Function daveGetS8from Lib "libnodave.dll" (ByRef buffer As Byte) As Integer
    Private Declare Function daveGetU8from Lib "libnodave.dll" (ByRef buffer As Byte) As Integer
    Private Declare Function daveGetS16from Lib "libnodave.dll" (ByRef buffer As Byte) As Integer
    Private Declare Function daveGetU16from Lib "libnodave.dll" (ByRef buffer As Byte) As Integer
    Private Declare Function daveGetS32from Lib "libnodave.dll" (ByRef buffer As Byte) As Integer
    '
    ' Is there an unsigned integer? Or a longer integer than integer? This doesn't work.
    ' Private Declare Function daveGetU32from Lib "libnodave.dll" (ByRef buffer As Byte) As integer
    '
    Private Declare Function daveGetFloatfrom Lib "libnodave.dll" (ByRef buffer As Byte) As Single
    '
    ' Copy and convert a value of 8,16,or 32 bit, signed or unsigned from internal buffer. These
    ' functions increment an internal buffer position. This buffer position is set to zero by
    ' daveReadBytes, daveReadBits, daveReadSZL.
    '
    Private Declare Function daveGetS8 Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveGetU8 Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveGetS16 Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveGetU16 Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveGetS32 Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' Is there an unsigned integer? Or a longer integer than integer? This doesn't work.
    'Private Declare Function daveGetU32 Lib "libnodave.dll" (ByVal dc As integer) As integer
    Private Declare Function daveGetFloat Lib "libnodave.dll" (ByVal dc As Integer) As Single
    '
    ' Read a value of 8,16,or 32 bit, signed or unsigned at position pos from internal buffer:
    '
    Private Declare Function daveGetS8At Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    Private Declare Function daveGetU8At Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    Private Declare Function daveGetS16At Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    Private Declare Function daveGetU16At Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    Private Declare Function daveGetS32At Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    '
    ' Is there an unsigned integer? Or a longer integer than integer? This doesn't work.
    'Private Declare Function daveGetU32At Lib "libnodave.dll" (ByVal dc As integer, ByVal pos As integer) As integer
    Private Declare Function daveGetFloatAt Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Single
    '
    ' Copy and convert a value of 8,16,or 32 bit, signed or unsigned into a buffer. The buffer
    ' is usually used by daveWriteBytes, daveWriteBits later.
    '
    Private Declare Function davePut8 Lib "libnodave.dll" (ByRef buffer As Byte, ByVal value As Integer) As Integer
    Private Declare Function davePut16 Lib "libnodave.dll" (ByRef buffer As Byte, ByVal value As Integer) As Integer
    Private Declare Function davePut32 Lib "libnodave.dll" (ByRef buffer As Byte, ByVal value As Integer) As Integer
    Private Declare Function davePutFloat Lib "libnodave.dll" (ByRef buffer As Byte, ByVal value As Single) As Integer
    '
    ' Copy and convert a value of 8,16,or 32 bit, signed or unsigned to position pos of a buffer.
    ' The buffer is usually used by daveWriteBytes, daveWriteBits later.
    '
    Private Declare Function davePut8At Lib "libnodave.dll" (ByRef buffer As Byte, ByVal pos As Integer, ByVal value As Integer) As Integer
    Private Declare Function davePut16At Lib "libnodave.dll" (ByRef buffer As Byte, ByVal pos As Integer, ByVal value As Integer) As Integer
    Private Declare Function davePut32At Lib "libnodave.dll" (ByRef buffer As Byte, ByVal pos As Integer, ByVal value As Integer) As Integer
    Private Declare Function davePutFloatAt Lib "libnodave.dll" (ByRef buffer As Byte, ByVal pos As Integer, ByVal value As Single) As Integer
    '
    ' Takes a timer value and converts it into seconds:
    '
    Private Declare Function daveGetSeconds Lib "libnodave.dll" (ByVal dc As Integer) As Single
    Private Declare Function daveGetSecondsAt Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Single
    '
    ' Takes a counter value and converts it to integer:
    '
    Private Declare Function daveGetCounterValue Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveGetCounterValueAt Lib "libnodave.dll" (ByVal dc As Integer, ByVal pos As Integer) As Integer
    '
    ' Get the order code (MLFB number) from a PLC. Does NOT work with 200 family.
    '
    Private Declare Function daveGetOrderCode Lib "libnodave.dll" (ByVal en As Integer, ByRef buffer As Byte) As Integer
    '
    ' Connect to a PLC.
    '
    Private Declare Function daveConnectPLC Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '
    ' Read a value or a block of values from PLC.
    '
    Private Declare Function daveReadBytes Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Read a long block of values from PLC. Long means too long to transport in a single PDU.
    '
    Private Declare Function daveReadManyBytes Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Write a value or a block of values to PLC.
    '
    Private Declare Function daveWriteBytes Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Write a long block of values to PLC. Long means too long to transport in a single PDU.
    '
    Private Declare Function daveWriteManyBytes Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Read a bit from PLC. numBytes must be exactly one with all PLCs tested.
    ' Start is calculated as 8*byte number+bit number.
    '
    Private Declare Function daveReadBits Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Write a bit to PLC. numBytes must be exactly one with all PLCs tested.
    '
    Private Declare Function daveWriteBits Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte) As Integer
    '
    ' Set a bit in PLC to 1.
    '
    Private Declare Function daveSetBit Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal byteAddress As Integer, ByVal bitAddress As Integer) As Integer
    '
    ' Set a bit in PLC to 0.
    '
    Private Declare Function daveClrBit Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal byteAddress As Integer, ByVal bitAddress As Integer) As Integer
    '
    ' Read a diagnostic list (SZL) from PLC. Does NOT work with 200 family.
    '
    Private Declare Function daveReadSZL Lib "libnodave.dll" (ByVal dc As Integer, ByVal ID As Integer, ByVal index As Integer, ByRef buffer As Byte, ByVal buflen As Integer) As Integer
    '
    Private Declare Function daveListBlocksOfType Lib "libnodave.dll" (ByVal dc As Integer, ByVal typ As Integer, ByRef buffer As Byte) As Integer
    Private Declare Function daveListBlocks Lib "libnodave.dll" (ByVal dc As Integer, ByRef buffer As Byte) As Integer
    Private Declare Function internalDaveGetBlockInfo Lib "libnodave.dll" Alias "daveGetBlockInfo" (ByVal dc As Integer, ByRef buffer As Byte, ByVal btype As Integer, ByVal number As Integer) As Integer
    '
    Private Declare Function daveGetProgramBlock Lib "libnodave.dll" (ByVal dc As Integer, ByVal blockType As Integer, ByVal number As Integer, ByRef buffer As Byte, ByRef length As Integer) As Integer
    '
    ' Start or Stop a PLC:
    '
    Private Declare Function daveStart Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveStop Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' Set outputs (digital or analog ones) of an S7-200 that is in stop mode:
    '
    Private Declare Function daveForce200 Lib "libnodave.dll" (ByVal dc As Integer, ByVal area As Integer, ByVal start As Integer, ByVal value As Integer) As Integer
    '
    ' Initialize a multivariable read request.
    ' The parameter PDU must have been obtained from daveNew PDU:
    '
    Private Declare Sub davePrepareReadRequest Lib "libnodave.dll" (ByVal dc As Integer, ByVal pdu As Integer)
    '
    ' Add a new variable to a prepared request:
    '
    Private Declare Sub daveAddVarToReadRequest Lib "libnodave.dll" (ByVal pdu As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer)
    '
    ' Executes the entire request:
    '
    Private Declare Function daveExecReadRequest Lib "libnodave.dll" (ByVal dc As Integer, ByVal pdu As Integer, ByVal rs As Integer) As Integer
    '
    ' Use the n-th result. This lets the functions daveGet<data type> work on that part of the
    ' internal buffer that contains the n-th result:
    '
    Private Declare Function daveUseResult Lib "libnodave.dll" (ByVal dc As Integer, ByVal rs As Integer, ByVal resultNumber As Integer) As Integer
    '
    ' Frees the memory occupied by single results in the result structure. After that, you can reuse
    ' the resultSet in another call to daveExecReadRequest.
    '
    Private Declare Sub daveFreeResults Lib "libnodave.dll" (ByVal rs As Integer)
    '
    ' Adds a new bit variable to a prepared request. As with daveReadBits, numBytes must be one for
    ' all tested PLCs.
    '
    Private Declare Sub daveAddBitVarToReadRequest Lib "libnodave.dll" (ByVal pdu As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer)
    '
    ' Initialize a multivariable write request.
    ' The parameter PDU must have been obtained from daveNew PDU:
    '
    Private Declare Sub davePrepareWriteRequest Lib "libnodave.dll" (ByVal dc As Integer, ByVal pdu As Integer)
    '
    ' Add a new variable to a prepared write request:
    '
    Private Declare Sub daveAddVarToWriteRequest Lib "libnodave.dll" (ByVal pdu As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte)
    '
    ' Add a new bit variable to a prepared write request:
    '
    Private Declare Sub daveAddBitVarToWriteRequest Lib "libnodave.dll" (ByVal pdu As Integer, ByVal area As Integer, ByVal areaNumber As Integer, ByVal start As Integer, ByVal numBytes As Integer, ByRef buffer As Byte)
    '
    ' Execute the entire write request:
    '
    Private Declare Function daveExecWriteRequest Lib "libnodave.dll" (ByVal dc As Integer, ByVal pdu As Integer, ByVal rs As Integer) As Integer
    '
    ' Initialize an MPI Adapter or NetLink Ethernet MPI gateway.
    ' While some protocols do not need this, I recommend to allways use it. It will do nothing if
    ' the protocol doesn't need it. But you can change protocols without changing your program code.
    '
    Private Declare Function daveInitAdapter Lib "libnodave.dll" (ByVal di As Integer) As Integer
    '
    ' Disconnect from a PLC. While some protocols do not need this, I recommend to allways use it.
    ' It will do nothing if the protocol doesn't need it. But you can change protocols without
    ' changing your program code.
    '
    Private Declare Function daveDisconnectPLC Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '
    ' Disconnect from an MPI Adapter or NetLink Ethernet MPI gateway.
    ' While some protocols do not need this, I recommend to allways use it.
    ' It will do nothing if the protocol doesn't need it. But you can change protocols without
    ' changing your program code.
    '
    Private Declare Function daveDisconnectAdapter Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '
    ' List nodes on an MPI or Profibus Network:
    '
    Private Declare Function daveListReachablePartners Lib "libnodave.dll" (ByVal dc As Integer, ByRef buffer As Byte) As Integer
    '
    '
    ' Set/change the timeout for an interface:
    '
    Private Declare Sub daveSetTimeout Lib "libnodave.dll" (ByVal di As Integer, ByVal maxTime As Integer)
    '
    ' Read the timeout setting for an interface:
    '
    Private Declare Function daveGetTimeout Lib "libnodave.dll" (ByVal di As Integer) As Integer
    '
    ' Get the name of an interface. Do NOT use this, but the wrapper function defined below!
    '
    Private Declare Function daveInternalGetName Lib "libnodave.dll" Alias "daveGetName" (ByVal en As Integer) As Integer
    '
    ' Get the MPI address of a connection.
    '
    Private Declare Function daveGetMPIAdr Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' Get the length (in bytes) of the last data received on a connection.
    '
    Private Declare Function daveGetAnswLen Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' Get the maximum length of a communication packet (PDU).
    ' This value depends on your CPU and connection type. It is negociated in daveConnectPLC.
    ' A simple read can read MaxPDULen-18 bytes.
    '
    Private Declare Function daveGetMaxPDULen Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' Reserve memory for a resultSet and get a handle to it:
    '
    Private Declare Function daveNewResultSet Lib "libnodave.dll" () As Integer
    '
    ' Destroy handles to daveInterface, daveConnections, PDUs and resultSets
    ' Free the memory reserved for them.
    '
    Private Declare Sub daveFree Lib "libnodave.dll" (ByVal item As Integer)
    '
    ' Reserve memory for a PDU and get a handle to it:
    '
    Private Declare Function daveNewPDU Lib "libnodave.dll" () As Integer
    '
    ' Get the error code of the n-th single result in a result set:
    '
    Private Declare Function daveGetErrorOfResult Lib "libnodave.dll" (ByVal resultSet As Integer, ByVal resultNumber As Integer) As Integer
    '
    Private Declare Function daveForceDisconnectIBH Lib "libnodave.dll" (ByVal di As Integer, ByVal src As Integer, ByVal dest As Integer, ByVal mpi As Integer) As Integer
    '
    ' Helper functions to open serial ports and IP connections. You can use others if you want and
    ' pass their results to daveNewInterface.
    '
    ' Open a serial port using name, baud rate and parity. Everything else is set automatically:
    '
    Private Declare Function setPort Lib "libnodave.dll" (ByVal portName As String, ByVal baudrate As String, ByVal parity As Byte) As Integer
    '
    ' Open a TCP/IP connection using port number (1099 for NetLink, 102 for ISO over TCP) and
    ' IP address. You must use an IP address, NOT a hostname!
    '
    Private Declare Function openSocket Lib "libnodave.dll" (ByVal port As Integer, ByVal peer As String) As Integer
    '
    ' Open an access oint. This is a name in you can add in the "set Programmer/PLC interface" dialog.
    ' To the access point, you can assign an interface like MPI adapter, CP511 etc.
    '
    Private Declare Function openS7online Lib "libnodave.dll" (ByVal peer As String, ByVal handle As Integer) As Integer
    '
    ' Close connections and serial ports opened with above functions:
    '
    Private Declare Function closePort Lib "libnodave.dll" (ByVal fh As Integer) As Integer
    '
    ' Close sockets opened with above functions:
    '
    Private Declare Function closeSocket Lib "libnodave.dll" (ByVal fh As Integer) As Integer
    '
    ' Close handle opened by opens7online:
    '
    Private Declare Function closeS7online Lib "libnodave.dll" (ByVal fh As Integer) As Integer
    '
    ' Read Clock time from PLC:
    '
    Private Declare Function daveReadPLCTime Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    ' set clock to a value given by user
    '
    Private Declare Function daveSetPLCTime Lib "libnodave.dll" (ByVal dc As Integer, ByRef timestamp As Byte) As Integer
    '
    ' set clock to PC system clock:
    '
    Private Declare Function daveSetPLCTimeToSystime Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '       BCD conversions:
    '
    Private Declare Function daveToBCD Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    Private Declare Function daveFromBCD Lib "libnodave.dll" (ByVal dc As Integer) As Integer
    '
    '***** End LibNoDave Sub/Function Declares *************************************************
#End Region

#Region " Properties "

    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for Libnodave. Maintained for compatibility
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
            Dim sData As String = String.Empty

            mbRead = True

            If ReadPLCData(sData, PLCTagInfo(TagName)) Then
                mbRead = False
                Return Strings.Split(sData, msDELIMITER)
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
        'Description:  Libnodave is capable of reading/writing data to 3 PLC types (S7 200, S7 300, 
        '              and S7 400). This property is set by default to S7 400 but can be set to one
        '              of the other PLC types here.
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

    Private ReadOnly Property Timeout() As Integer
        '********************************************************************************************
        'Description:  Connection timeout value in ms.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnTimeout
        End Get

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

#Region " Functions "

    Shared Function AreaNum(ByVal Area As String) As Integer
        '********************************************************************************************
        'Description:  Return the Area Number constant for the supplied Area Name.
        '
        'Parameters: Area
        'Returns:    Area Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case Area.ToUpper
            Case "SYSINFO"  '200 Family
                AreaNum = daveSysInfo
            Case "SYSFLAGS" '200 Family
                AreaNum = daveSysFlags
            Case "ANAIN" '200 Family
                AreaNum = daveAnaIn
            Case "ANAOUT" '200 Family
                AreaNum = daveAnaOut
            Case "P" 'Peripheral Addresses
                AreaNum = daveP
            Case "INPUTS"
                AreaNum = daveInputs
            Case "OUTPUTS"
                AreaNum = daveOutputs
            Case "FLAGS", "FLAGSBIT"
                AreaNum = daveFlags
            Case "DB", "DBBIT", "DBD", "DBREAL"
                AreaNum = daveDB
            Case "DI"
                AreaNum = daveDI
            Case "COUNTER"
                AreaNum = daveCounter
            Case "TIMER"
                AreaNum = daveTimer
            Case "COUNTER200"
                AreaNum = daveCounter200 'IEC counters (200 family)
            Case "TIMER200"
                AreaNum = daveTimer200 'IEC timers (200 family)
            Case Else
                AreaNum = daveV 'don't know what it is
        End Select

    End Function

    Friend Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String
        '****************************************************************************************
        'Description: This function takes a integer from a hotlink etc. and formats it for the
        '             time base that this particular PLC uses.

        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim fTmp As Single = CSng(ValueIn / 1000) 'assuming all 0.001 timebase

        Return Format(fTmp, sFormat)

    End Function

    Friend Function FormatToTimerValue(ByVal ValueIn As Integer) As String
        '****************************************************************************************
        'Description: This function takes a integer from a textbox etc and formats it for the 
        '             value we need to send to plc
        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nTmp As Integer = ValueIn * 1000 'assuming all 0.001 timebase

        Return nTmp.ToString

    End Function

    Private Function OneTimeRead(ByRef Data As String, ByVal Link As clsLink) As Boolean
        '****************************************************************************************
        'Description: This function reads data One Time from the plc
        '
        'Parameters: Data Array to be returned, Link
        'Returns:    True if all OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            Dim nResult As Integer
            Dim lTicksNow As Long = GetTickCount

            Select Case Link.Area
                Case "DB"
                    Dim nBuffer(Link.Length) As Integer
                    Dim bytBuffer(Link.Length * 2) As Byte

                    Do While mbBusy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    Call subCheckConnection(mConnection)

                    mbBusy = True
                    nResult = daveReadManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, Link.Start, _
                                                Link.Length * 2, bytBuffer(0))


                    If nResult <> daveResOK Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", "PLC data read failure, " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:OneTimeRead", _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")

                        mbBusy = False
                        Return False
                    Else
                        Dim nIndex As Integer

                        Data = String.Empty
                        For nPointer As Integer = 0 To (Link.Length - 1) * 2 Step 2
                            nBuffer(nPointer \ 2) = daveGetS16from(bytBuffer(nPointer)) 'Signed
                            Data += nBuffer(nPointer \ 2).ToString
                            If nIndex < (Link.Length - 1) Then Data += msDELIMITER
                            nIndex += 1
                        Next
                    End If
                    mbBusy = False
                Case "DBD"
                    Dim nBuffer(Link.Length) As Integer
                    Dim bytBuffer(Link.Length * 4) As Byte

                    Do While mbBusy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    Call subCheckConnection(mConnection)

                    mbBusy = True
                    nResult = daveReadManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, Link.Start, _
                                                Link.Length * 4, bytBuffer(0))

                    If nResult <> daveResOK Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", "PLC data read failure, " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:OneTimeRead", _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        mbBusy = False
                        Return False
                    Else
                        Dim nIndex As Integer

                        Data = String.Empty
                        For nPointer As Integer = 0 To (Link.Length - 1) * 4 Step 4
                            nBuffer(nPointer \ 4) = daveGetS32from(bytBuffer(nPointer)) 'Signed
                            Data += nBuffer(nPointer \ 4).ToString
                            If nIndex < (Link.Length - 1) Then Data += msDELIMITER
                            nIndex += 1
                        Next
                    End If
                    mbBusy = False
                Case "DBREAL"
                    Dim nBuffer(Link.Length) As Single
                    Dim bytBuffer(Link.Length * 4) As Byte

                    Do While mbBusy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    Call subCheckConnection(mConnection)

                    mbBusy = True
                    nResult = daveReadManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, Link.Start, _
                                                Link.Length * 4, bytBuffer(0))

                    If nResult <> daveResOK Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", "PLC data read failure, " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:OneTimeRead", _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        mbBusy = False
                        Return False
                    Else
                        Dim nIndex As Integer

                        Data = String.Empty
                        For nPointer As Integer = 0 To (Link.Length - 1) * 4 Step 4
                            nBuffer(nPointer \ 5) = daveGetFloatfrom(bytBuffer(nPointer)) 'Signed
                            Data += nBuffer(nPointer \ 4).ToString
                            If nIndex < (Link.Length - 1) Then Data += msDELIMITER
                            nIndex += 1
                        Next
                    End If
                    mbBusy = False
                Case "DBBIT"
                    Dim nBuffer(0) As Integer
                    Dim bytBuffer(0) As Byte

                    Do While mbBusy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    Call subCheckConnection(mConnection)

                    mbBusy = True
                    nResult = daveReadBits(mConnection.Instance, AreaNum(Link.Area), Link.Address, _
                                                 (Link.Start * 8) + Link.Bit, 1, bytBuffer(0))


                    If nResult <> daveResOK Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        mbBusy = False
                        Return False
                    Else
                        Data = String.Empty
                        Data = bytBuffer(0).ToString
                    End If
                    mbBusy = False
                Case "FLAGSBIT"
                    Dim nBuffer(0) As Integer
                    Dim bytBuffer(0) As Byte

                    Do While mbBusy
                        Application.DoEvents()
                        Threading.Thread.Sleep(10)
                    Loop

                    Call subCheckConnection(mConnection)

                    mbBusy = True
                    nResult = daveReadBits(mConnection.Instance, AreaNum(Link.Area), 0, _
                                                 Link.Start, 1, bytBuffer(0))
                    If nResult <> daveResOK Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data read failure, " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                               ", Result code = [" & nResult.ToString & "]")
                        mbBusy = False
                        Return False
                    Else
                        Data = String.Empty
                        Data = bytBuffer(0).ToString
                    End If
                    mbBusy = False
                Case Else
                    'Unsupported data area
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", "Data type " & Link.Area & _
                                           " not supported. Tag = " & Link.TagName)
                    RaiseEvent ModuleError(vbObjectError + 1013, gcsRM.GetString("csUNSUP_DATA_TYPE"), _
                                           msMODULE & ":clsPLCComm:OneTimeRead", _
                                           gcsRM.GetString("csPLC_DATA_TYPE") & " = " & Link.Area & ", " & _
                                           gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName)
                    mbBusy = False
                    Return False
            End Select 'Link.Area

            Threading.Thread.Sleep(100)

            Return (nResult = daveResOK)

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:OneTimeRead", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:OneTimeRead", gcsRM.GetString("csTAG_NAME") & " = " & _
                                   Link.TagName & vbCrLf & ex.Message)
            mbBusy = False
            Return False
        End Try

    End Function

    Private Function PLCTagInfo(ByVal TagName As String) As clsLink
        '********************************************************************************************
        'Description:  Return a clsLink that contains data related to the PLC Tag
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
            oNode = oNodeList(0)

            With oLink
                Dim sBit As String = TryCast(oNode.Item("bit").InnerXml, String)

                .TagName = TagName
                .Area = oNode.Item("area").InnerXml
                .Address = CType(oNode.Item("addr").InnerXml, Integer)
                .Start = CType(oNode.Item("start").InnerXml, Integer)
                .Length = CType(oNode.Item("length").InnerXml, Integer)

                If IsNothing(sBit) = False Then
                    If sBit <> String.Empty Then
                        .Bit = CType(oNode.Item("bit").InnerXml, Integer)
                    End If
                End If

                sTopic = oNode.Item("topic").InnerXml

                .ElapsedTime = 0
                .SameDataCount = 0
                .ZoneName = ZoneName

                Dim sRef As String = String.Empty
                For nIndex As Integer = 0 To .Length - 1
                    sRef += "0"
                    If nIndex < (.Length - 1) Then sRef += msDELIMITER
                Next
                .RefData = sRef
            End With
        End If

        If oNodeList.Count > 1 Then
            'Should only be one match!!!
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                         "(" & oNodeList.Count.ToString(CultureInfo.InvariantCulture) & _
                         ") instances found of Tag [" & TagName & "].")
            RaiseEvent ModuleError(vbObjectError + 1012, gcsRM.GetString("csPLC_TAG_MULT_INST") & " [" & TagName & "]", _
                                   msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
        End If

        Try
            sPath = "//" & Strings.Replace(ZoneName, " ", "_") & "[id='" & sTopic & "']"
            oNodeList = oXMLDoc.SelectNodes(sPath)
        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csINVALID_XPATH_SYNTAX") & _
                                   " [" & sPath & "]", msMODULE & ":clsPLCComm:PLCTagInfo", ex.Message)
            Return Nothing
        End Try

        If oNodeList.Count = 0 Then
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", "Topic [" & sTopic & "] not found.")
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csPLC_TOPIC_NOT_DEFINED") & _
                                   " [" & sTopic & "]", msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
            Return Nothing
        Else
            oNode = oNodeList(0)

            With oLink
                .PollRate = CType(oNode.Item("pollrate").InnerXml, Integer)
                .ConnectionName = oNode.Item("connection").InnerXml
            End With
        End If

        If oNodeList.Count > 1 Then
            'Should only be one match!!!
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:PLCTagInfo", _
                         "(" & oNodeList.Count.ToString(CultureInfo.InvariantCulture) & _
                         ") instances found of Topic [" & sTopic & "].")
            RaiseEvent ModuleError(vbObjectError + 1012, gcsRM.GetString("csPLC_TOPIC_MULT_INST") & " [" & sTopic & "]", _
                                   msMODULE & ":clsPLCComm:PLCTagInfo", String.Empty)
        End If

        Return oLink

    End Function

    Shared Function ProtcolType(ByVal Protocol As String) As Integer
        '********************************************************************************************
        'Description:  Return the protocol type number assigned to the protocol string.
        '
        'Parameters: protocol string
        'Returns:    protocol type number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case Protocol
            Case "MPI"
                ProtcolType = daveProtoMPI
            Case "MPI2"
                ProtcolType = daveProtoMPI2
            Case "MPI3"
                ProtcolType = daveProtoMPI3
            Case "PPI"
                ProtcolType = daveProtoPPI
            Case "AS511"
                ProtcolType = daveProtoAS511
            Case "S7ONLINE"
                ProtcolType = daveProtoS7online
            Case "ISOTCP"
                ProtcolType = daveProtoISOTCP
            Case "ISOTCP243"
                ProtcolType = daveProtoISOTCP243
            Case "MPI_IBH"
                ProtcolType = daveProtoMPI_IBH
            Case "PPI_IBH"
                ProtcolType = daveProtoPPI_IBH
            Case "USER"
                ProtcolType = daveProtoUserTransport
            Case Else
                ProtcolType = daveProtoUserTransport
        End Select

    End Function

    Private Function ReadPLCData(ByRef Data As String, ByVal Link As clsLink) As Boolean
        '********************************************************************************************
        'Description:  Read data from the PLC corresponding to the configuration in Link and create a
        '              hotlinkif required.
        '
        'Parameters: Data array for return data, Link containing configuration
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lTicksNow As Long = GetTickCount

        Try

            If Not IsNothing(Link) Then
                'make sure we're connected to the PLC associated with this link
                Call subGetConnection(Link.ConnectionName)
                'determine if this is a One-Timer or a HotLink
                If Link.PollRate = mnMANUAL_POLL_RATE Then
                    Dim bSuccess As Boolean
                    Dim nRetry As Integer
                    'Just in case it's been awhile.  If too long it will cause the code to disconnect and reconnect to the PLC.
                    mConnection.LastOpTime = lTicksNow
                    'OneTimeRead
                    For nRetry = 0 To mnMAX_RETRY_COUNT
                        If OneTimeRead(Data, Link) Then
                            mConnection.LastOpTime = lTicksNow
                            bSuccess = True
                            Exit For
                        End If 'OneTimeRead()
                    Next 'nRetry

                    If nRetry > 0 Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", "Retry count = " & nRetry.ToString)
                    End If

                    Return bSuccess
                Else
                    'HotLink
                    Call subCreateHotLink(Link)
                    Data = Link.RefData
                    Return True
                End If

            Else
                Return False
            End If 'Not IsNothing(Link)

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(PLCCommErrors.ReadFailed, gcsRM.GetString("csCOULD_NOT_READ_PLC"), _
                                   msMODULE & ":clsPLCComm:ReadPLCData", gcsRM.GetString("csTAG_NAME") & _
                                   " = " & Link.TagName)
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
                                       "], type = " & Link.Area)
                RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                        gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                       msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                Return False
            Else
                bFloat = Strings.InStr(sElement, ".") > 0
            End If

            'check that it's the correct type for the data Type 
            Select Case Link.Area.ToUpper
                Case "V"
                    'This area ("V") doesn't exist but was left here as a model of how to handle 
                    'verification of special case data types. Otherwise, they are assumed to be
                    'Integer.
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Area)
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
                                                   "], type = " & Link.Area & _
                                                   vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                    gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                   msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                            Return False
                        End Try

                    End If 'bFloat

                Case "DBBIT", "FLAGSBIT"
                    'This area ("V") doesn't exist but was left here as a model of how to handle 
                    'verification of special case data types. Otherwise, they are assumed to be
                    'Integer.
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Area)
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
                                                   "], type = " & Link.Area & _
                                                   vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                    gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                                   msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                            Return False
                        End Try

                    End If 'bFloat

                Case "DBREAL"
                    Dim sngData As Single ' = 0

                    Try
                        sngData = CType(sElement, Single)
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Area & _
                                               vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End Try

                Case "DBD"
                    Dim lData As Long ' = 0

                    Try
                        lData = CType(sElement, Long)
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Area & _
                                               vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                        RaiseEvent ModuleError(mPWCommon.PLCCommErrors.InvalidData, _
                                                gcsRM.GetString("csBAD_DATA_PLC_WRITE"), _
                                               msMODULE & ":clsPLCComm:ValidateData", String.Empty)
                        Return False
                    End Try

                Case Else
                    If bFloat Then
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:ReadPLCData", _
                                               "Invalid Data, value = [" & sElement & _
                                               "], type = " & Link.Area)
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
        Dim lTicksNow As Long = GetTickCount

        'make sure we're connected to the PLC associated with this link
        Call subGetConnection(Link.ConnectionName)


        If ValidateData(Data, Link) Then

            Try
                Dim nResult As Integer

                Select Case Link.Area.ToUpper
                    Case "DB"
                        Dim bytBuffer(Link.Length * 2) As Byte

                        For nIndex As Integer = 0 To (Link.Length - 1)
                            Call davePut16At(bytBuffer(0), nIndex * 2, CType(Data(nIndex), Integer))
                        Next

                        Do While mbBusy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        Call subCheckConnection(mConnection)

                        mbBusy = True
                        nResult = daveWriteManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, _
                                                     Link.Start, Link.Length * 2, bytBuffer(0))
                        If nResult <> daveResOK Then
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                                   msMODULE & ":clsPLCComm:WritePLCData", _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            mbBusy = False
                            Return False
                        End If
                        mbBusy = False
                    Case "DBD"
                        Dim bytBuffer(Link.Length * 4) As Byte

                        For nIndex As Integer = 0 To (Link.Length - 1)
                            Call davePut32At(bytBuffer(0), nIndex * 4, CType(Data(nIndex), Integer))
                        Next

                        Do While mbBusy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        Call subCheckConnection(mConnection)

                        mbBusy = True
                        nResult = daveWriteManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, _
                                                     Link.Start, Link.Length * 4, bytBuffer(0))
                        If nResult <> daveResOK Then
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                                   msMODULE & ":clsPLCComm:WritePLCData", _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            mbBusy = False
                            Return False
                        End If
                        mbBusy = False
                    Case "DBREAL"
                        Dim bytBuffer(Link.Length * 4) As Byte

                        For nIndex As Integer = 0 To (Link.Length - 1)
                            Call davePutFloatAt(bytBuffer(0), nIndex * 4, CType(Data(nIndex), Single))
                        Next

                        Do While mbBusy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        Call subCheckConnection(mConnection)

                        mbBusy = True
                        nResult = daveWriteManyBytes(mConnection.Instance, AreaNum(Link.Area), Link.Address, _
                                                     Link.Start, Link.Length * 4, bytBuffer(0))
                        If nResult <> daveResOK Then
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                                   msMODULE & ":clsPLCComm:WritePLCData", _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            mbBusy = False
                            Return False
                        End If
                        mbBusy = False
                    Case "DBBIT"

                        Dim bytBuffer(0) As Byte

                        If CType(Data(0), Byte) = 1 Then
                            bytBuffer(0) = 1
                        Else
                            bytBuffer(0) = 0
                        End If

                        Do While mbBusy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        Call subCheckConnection(mConnection)

                        mbBusy = True
                        nResult = daveWriteBits(mConnection.Instance, AreaNum(Link.Area), Link.Address, _
                                                     (Link.Start * 8) + Link.Bit, 1, bytBuffer(0))


                        If nResult <> daveResOK Then
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                                   msMODULE & ":clsPLCComm:WritePLCData", _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            mbBusy = False
                            Return False
                        End If
                        mbBusy = False
                    Case "FLAGSBIT"
                        Dim bytBuffer(0) As Byte

                        If CType(Data(0), Byte) = 1 Then
                            bytBuffer(0) = 1
                        Else
                            bytBuffer(0) = 0
                        End If

                        Do While mbBusy
                            Application.DoEvents()
                            Threading.Thread.Sleep(10)
                        Loop

                        Call subCheckConnection(mConnection)

                        mbBusy = True
                        nResult = daveWriteBits(mConnection.Instance, AreaNum(Link.Area), 0, _
                                                     Link.Start, 1, bytBuffer(0))
                        If nResult <> daveResOK Then
                            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "PLC data write failure, " & _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), _
                                                   msMODULE & ":clsPLCComm:WritePLCData", _
                                                   gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & _
                                                   ", Result code = [" & nResult.ToString & "]")
                            mbBusy = False
                            Return False
                        End If
                        mbBusy = False
                    Case Else
                        'Unsupported data area
                        mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", "Data type " & Link.Area & _
                                               " not supported. Tag = " & Link.TagName)
                        RaiseEvent ModuleError(vbObjectError + 1013, gcsRM.GetString("csUNSUP_DATA_TYPE"), _
                                               msMODULE & ":clsPLCComm:WritePLCData", _
                                               gcsRM.GetString("csPLC_DATA_TYPE") & " = " & Link.Area & ", " & _
                                               gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName)

                        Return False
                End Select 'Link.Area
                Return (nResult = daveResOK)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:WritePLCData", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ModuleError(PLCCommErrors.WriteFailed, gcsRM.GetString("csCOULD_NOT_WRITE_PLC"), msMODULE & _
                                       ":clsPLCComm:WritePLCData", _
                                       gcsRM.GetString("csTAG_NAME") & " = " & Link.TagName & vbCrLf & ex.Message)
                mbBusy = False
                Return False
            End Try

        Else
            Return False
        End If  'ValidateData(Data, oLink)

    End Function

#End Region

#Region " Routines "

    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description:  Cleanup.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

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
        tmrHotLink.Interval = mnHOTLINK_POLL_RATE
        mnTimeout = 30000 '(ms)

    End Sub

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

        If mLinks.Count > 0 Then

            Try
                Dim nLinkUBound As Integer = mLinks.Count - 1
                Dim nIndex As Integer
                Dim sLinkTags(nLinkUBound) As String
                Dim sLinkZones(nLinkUBound) As String

                'make sure we're not polling
                Do While tmrHotLink.Enabled = False
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

                tmrHotLink.Enabled = False

                For Each Link As clsLink In mLinks
                    sLinkTags(nIndex) = Link.TagName
                    sLinkZones(nIndex) = Link.ZoneName
                    nIndex += 1
                Next 'Link

                nIndex = 0
                Do While mLinks.Count > 0
                    For Each Link As clsLink In mLinks
                        If (Link.TagName = sLinkTags(nIndex)) And (Link.ZoneName = sLinkZones(nIndex)) Then
                            mLinks.Remove(Link)
                            Exit For
                        End If
                    Next 'oLink
                    nIndex += 1
                Loop


            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveAllHotLinks", ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End If

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

        If mLinks.Count > 0 Then

            Try
                Dim bFound As Boolean

                'make sure we're not polling
                Do While tmrHotLink.Enabled = False
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

                tmrHotLink.Enabled = False

                For Each Link As clsLink In mLinks
                    If (Link.TagName = Tag) And (Link.ZoneName = Zone.Name) Then
                        mLinks.Remove(Link)
                        bFound = True
                        Exit For
                    End If
                Next 'oLink

                tmrHotLink.Enabled = (mLinks.Count > 0)

                If Not bFound Then
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", "Hotlink to tag [" & _
                                           Tag & "] not found in collection.")
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:RemoveHotLink", ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End If 'mLinks.Count > 0

    End Sub

    Private Sub subCheckConnection(ByRef Connection As clsConnection)
        '********************************************************************************************
        'Description:  The connection times out if no read or write operations have ocurred within
        '              the timeout period. If the connection is close to or past its expiration time,
        '              re-establish the connection.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lTicksNow As Long = GetTickCount
        Dim lElapsed As Long



        With Connection
            If lTicksNow < .LastOpTime Then
                'rollover - this happens once every 49.7 days so just assume timeout
                lElapsed = Timeout
            Else
                lElapsed = lTicksNow - .LastOpTime
            End If
            'When checking elapsed time, make sure connection won't time out before start of next operation
            If (lElapsed + 100) > Timeout Then
                .CloseConnection()
                Threading.Thread.Sleep(100)
                'this automatically updates LastOpTime
                .OpenConnection()
            Else
                '08/13/13 BTK Moved to subUpdateHotlinks.  We only want to update the LastOpTime when we have successfully read data from the PLC.
                '.LastOpTime = lTicksNow
            End If
        End With

    End Sub

    Private Sub subCreateHotLink(ByRef Link As clsLink)
        '********************************************************************************************
        'Description:  Add the Link argument to the mLinks collection of polled addresses. Read the
        '              address one time to update Link reference data.
        '
        'Parameters: Link 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim bAddLink As Boolean = True
            Dim sData As String = String.Empty

            If mLinks.Count > 0 Then
                bAddLink = mLinks.Item(Link.TagName) Is Nothing
            End If

            If bAddLink Then
                If OneTimeRead(sData, Link) Then

                    For nIndex As Integer = 0 To Link.Length - 1
                        Link.RefData = sData
                    Next

                    mLinks.Add(Link)

                    If mLinks.Count = 1 Then
                        'This is the first one so start the update timer
                        tmrHotLink.Enabled = True
                    End If

                    RaiseEvent NewData(ZoneName, Link.TagName, Strings.Split(sData, msDELIMITER))
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                           "Hotlink not created due to data read failure. Tag = " & Link.TagName)
                End If
            End If 'bAddLink

        Catch ex As Exception
            mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subCreateHotLink", _
                                   ex.Message & vbCrLf & ex.StackTrace)
            RaiseEvent ModuleError(mPWCommon.PLCCommErrors.CommLinkError, ex.Message, _
                                   msMODULE & ":clsPLCComm:subCreateHotLink", String.Empty)
        End Try

    End Sub

    Private Sub subGetConnection(ByVal Name As String)
        '********************************************************************************************
        'Description:  Set mConnection equal to the connection associated with the current TagName's
        '              Topic. Add connection to mConnetions if it doesn't exist
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bAddConnection As Boolean = (mConnections.Count = 0)

        If Not bAddConnection Then
            mConnection = mConnections.Item(Name)
            If mConnection Is Nothing Then
                bAddConnection = True
            End If
        End If

        If bAddConnection Then
            mConnection = New clsConnection(XMLPath, Name)
            mConnections.Add(mConnection)
        End If

    End Sub

    Private Sub subUpdateHotLinks()
        '********************************************************************************************
        'Description:  Update hotlinks that are due to be serviced.
        '
        'Parameters: TimerState
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lTicksNow As Long = GetTickCount
        'Loop though the links to see which ones need to be refreshed
        For Each Link As clsLink In mLinks
            Link.ElapsedTime += mnHOTLINK_POLL_RATE

            If Link.ElapsedTime >= Link.PollRate Then
                'Read Link address
                Try
                    Dim sData As String = String.Empty

                    Link.ElapsedTime = 0 'reset

                    If OneTimeRead(sData, Link) Then
                        'Check for changes
                        If sData = Link.RefData Then
                            Link.SameDataCount += 1
                            If Link.SameDataCount >= mnREFRESH_COUNT Then
                                Link.SameDataCount = 0
                            End If
                        Else
                            Link.SameDataCount = 0
                        End If

                        'mConnection.LastOpTime = lTicksNow

                        If Link.SameDataCount = 0 Then
                            RaiseEvent NewData(ZoneName, Link.TagName, Strings.Split(sData, msDELIMITER))
                            '08/13/13 BTK Moved from subCheckConnection.  We want to update the LastOpTime when we have successfully read data from the PLC.
                            mConnection.LastOpTime = lTicksNow
                            Link.RefData = sData
                        End If
                    End If 'OneTimeRead(sData, Link)

                Catch ex As Exception
                    mDebug.WriteEventToLog(msMODULE & ":clsPLCComm:subUpdateHotLinks", _
                                           "Failed to read tag [" & "" & "]" & _
                                           vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                End Try

            End If 'Link.ElapsedTime >= Link.PollRate

        Next 'nIndex
    End Sub

#End Region

#Region " Events "

    Private Sub mConnection_ConnectionError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                            ByVal sModule As String, ByVal AdditionalInfo As String) Handles mConnection.ConnectionError
        '********************************************************************************************
        ' Description:  An error has been raised by the Connection class. Forward it to the parent
        '               application.
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent ModuleError(nErrNum, sErrDesc, sModule, AdditionalInfo)

    End Sub

    Private Sub tmrHotLink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHotLink.Tick
        '********************************************************************************************
        ' Description:  Call subUpdateHotLinks on a defined interval
        '
        ' Parameters: none
        ' Returns: none
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrHotLink.Enabled = False

        If mLinks.Count > 0 Then
            Call subUpdateHotLinks()
            tmrHotLink.Enabled = True
        End If

    End Sub

#End Region

    Private Class clsConnection
        '********************************************************************************************
        'Description:  This class contains the properties and methods associated with a connection
        '              to a single PLC.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

#Region " Declares "

        '***** Property Vars ***********************************************************************
        Private mnInstance As Integer
        Private mnInterface As Integer
        Private msIPAddress As String
        Private mnLastOpTime As Long
        Private mnMPIAddressLocal As Integer
        Private mnMPIAddressPlc As Integer
        Private msName As String
        Private mnPort As Integer
        Private msProtocol As String
        Private mnRack As Integer
        Private mnSlot As Integer
        '***** End Property Vars *******************************************************************

        '******* Events ****************************************************************************
        Friend Event ConnectionError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                     ByVal sModule As String, ByVal AdditionalInfo As String)
        '******* End Events ************************************************************************

        '******* Functions *************************************************************************
        Private Declare Function GetTickCount Lib "kernel32" () As Integer
        '******* End Functions *********************************************************************

#End Region

#Region " Properties "

        Friend Property Instance() As Integer
            '********************************************************************************************
            'Description:  Connection instance used for Open/Close Connection and data Read/Write
            '
            'Parameters: Connection instance
            'Returns:    Connection instance
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnInstance
            End Get

            Set(ByVal value As Integer)
                mnInstance = value
            End Set

        End Property

        Private ReadOnly Property IPAddress() As String
            '********************************************************************************************
            'Description:  IP address of the PLC/CP as a string, e.g. 192.168.0.3 
            '
            'Parameters: none
            'Returns:    IP address
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msIPAddress
            End Get

        End Property

        Friend Property LastOpTime() As Long
            '********************************************************************************************
            'Description:  The system tick in ms when the last read or write operation was performed. 
            '              This is used to determine if the connection is active or timed out.
            '
            'Parameters: none
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnLastOpTime
            End Get

            Set(ByVal value As Long)
                mnLastOpTime = value
            End Set

        End Property

        Private ReadOnly Property MPIAddressLocal() As Integer
            '********************************************************************************************
            'Description:  Only meaningful for MPI or PPI but required parameter for daveNewInterface.
            '
            'Parameters: none
            'Returns:    MPIAddressLocal
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnMPIAddressLocal
            End Get

        End Property

        Private ReadOnly Property MPIAddressPlc() As Integer
            '********************************************************************************************
            'Description:  Only meaningful for MPI or PPI but required parameter for daveNewConnection.
            '
            'Parameters: none
            'Returns:    MPIAddressPLC
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnMPIAddressPlc
            End Get

        End Property

        Friend ReadOnly Property Name() As String
            '********************************************************************************************
            'Description:  The name associated with this connection.
            'Parameters: none
            'Returns:    Connection name
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msName
            End Get

        End Property

        Private Property PLCInterface() As Integer
            '********************************************************************************************
            'Description:  Interface instance used for Open and Close Connection
            '
            'Parameters: Interface instance
            'Returns:    Interface instance
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnInterface
            End Get

            Set(ByVal value As Integer)
                mnInterface = value
            End Set

        End Property

        Private ReadOnly Property Port() As Integer
            '********************************************************************************************
            'Description:  Port is the port number for the protocol, usually 102 for ISO over TCP or 1099 
            '              for the IBH/MHJ NetLink protocol.
            '
            'Parameters: none
            'Returns:    port number
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnPort
            End Get

        End Property

        Private ReadOnly Property Protocol() As String
            '********************************************************************************************
            'Description:  The name of the protocol type to be used for the interfcce.
            '              (MPI, MPI2, MPI3, PPI, AS511, S7ONLONE, ISOTCP, ISOTCP243, MPI_IBH, PPI_IBH, 
            '               or USER)
            'Parameters: none
            'Returns:    protocol name
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msProtocol
            End Get

        End Property

        Private ReadOnly Property Rack() As Integer
            '********************************************************************************************
            'Description:  Rack Number the CPU is mounted in (normally 0). Only meaningful for ISO over 
            '              TCP.
            '
            'Parameters: none
            'Returns:    Rack Number
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnRack
            End Get

        End Property

        Private ReadOnly Property Slot() As Integer
            '********************************************************************************************
            'Description:  Slot Number the CPU is mounted in (normally 2). Only meaningful for ISO over 
            '              TCP.
            '
            'Parameters: none
            'Returns:    Slot Number
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnSlot
            End Get

        End Property

#End Region

#Region " Methods "

        Friend Function CloseConnection() As Integer
            '********************************************************************************************
            'Description:  Disconnect from PLC. 
            '
            'Parameters: none
            'Returns:    Result of operation
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim nResult As Integer

            Try
                nResult = daveDisconnectPLC(Instance)
                If nResult = daveResOK Then
                    Call daveFree(Instance)
                    Instance = 0
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:CloseConnection", _
                                           "Close Connection [" & Name & "] failed. Error Code = [" & nResult.ToString & "]")
                    RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                               msMODULE & ":clsConnection:CloseConnection", _
                                               gcsRM.GetString("csERROR") & ": [" & nResult.ToString & "]")
                    Return nResult
                End If

                nResult = daveDisconnectAdapter(PLCInterface)
                If nResult = daveResOK Then
                    Call daveFree(PLCInterface)
                    PLCInterface = 0
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:CloseConnection", _
                                           "Connection [" & Name & "] Disconnect Adapter failed. Error Code = [" & _
                                           nResult.ToString & "]")
                    RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                               msMODULE & ":clsConnection:CloseConnection", _
                                               gcsRM.GetString("csERROR") & ": [" & nResult.ToString & "]")
                    Return nResult
                End If

                If Port > 100 Then
                    nResult = closePort(Port)
                Else
                    nResult = closeS7online(Port)
                End If

                If (nResult <> daveResOK) And (nResult <> daveResNoPeripheralAtAddress) Then
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:CloseConnection", _
                                           "Connection [" & Name & "] Close Port failed. Error Code = [" & _
                                           nResult.ToString & "]")
                    RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                               msMODULE & ":clsConnection:CloseConnection", _
                                               gcsRM.GetString("csERROR") & ": [" & nResult.ToString & "]")
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnection:CloseConnection", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsConnection:CloseConnection", ex.Message)
            End Try

            Return nResult

        End Function

        Friend Function OpenConnection() As Integer
            '********************************************************************************************
            'Description:  Connect to the PLC. 
            '
            'Parameters: none
            'Returns:    Result of operation
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim nResult As Integer

            Try

                Dim nSocket As Integer = openSocket(Port, IPAddress)
                PLCInterface = daveNewInterface(nSocket, nSocket, Name, MPIAddressLocal, _
                                                             ProtcolType(Protocol), daveSpeed187k)

                nResult = daveInitAdapter(PLCInterface)

                If nResult = daveResOK Then
                    Instance = daveNewConnection(PLCInterface, MPIAddressPlc, Rack, Slot)
                    nResult = daveConnectPLC(Instance)
                Else
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:OpenConnection", _
                                           "Connection [" & Name & "] Init Adapter failed. Error Code = [" & nResult.ToString & "]")
                    RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                               msMODULE & ":clsConnection:OpenConnection", _
                                               gcsRM.GetString("csERROR") & ": [" & nResult.ToString & "]")
                    Return nResult
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnection:OpenConnection", ex.Message & vbCrLf & ex.StackTrace)
                RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsConnection:OpenConnection", ex.Message)
            End Try

            If nResult = daveResOK Then
                LastOpTime = GetTickCount
            Else
                mDebug.WriteEventToLog(msMODULE & ":clsConnection:OpenConnection", _
                                       "Connection [" & Name & "] Connect to PLC failed. Error Code = [" & _
                                       nResult.ToString & "]")
                RaiseEvent ConnectionError(PLCCommErrors.CommLinkError, gcsRM.GetString("csPLC_COMM_ERROR"), _
                                           msMODULE & ":clsConnection:OpenConnection", _
                                           gcsRM.GetString("csERROR") & ": [" & nResult.ToString & "]")
            End If

            Return nResult

        End Function

        Private Sub subGetConfig(ByVal XMLPath As String, ByVal Name As String)
            '********************************************************************************************
            'Description:  Read connection configuration data from XML file and set properties..
            '
            'Parameters: none
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim sPath As String = "//connection[name='" & Strings.Replace(Name, " ", "_") & "']"
            Dim oNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList
            Dim oXMLDoc As New XmlDocument

            Try
                oXMLDoc.Load(XMLPath)
                oNodeList = oXMLDoc.SelectNodes(sPath)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnection:subGetConfig", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
                RaiseEvent ConnectionError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csINVALID_XPATH_SYNTAX") & _
                                           " [" & sPath & "]", msMODULE & ":clsConnection:subGetConfig", ex.Message)
                Exit Sub
            End Try

            Select Case oNodeList.Count

                Case 0
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:subGetConfig", _
                                           "Tag [" & Strings.Replace(Name, " ", "_") & "] not found.")
                    RaiseEvent ConnectionError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csPLC_TAG_NOT_DEFINED") & _
                                               " [" & Strings.Replace(Name, " ", "_") & "]", _
                                               msMODULE & ":clsConnection:subGetConfig", String.Empty)
                    Exit Sub

                Case 1
                    Dim sItem As String = "port"

                    msName = Name

                    Try
                        oNode = oNodeList.Item(0)
                        'Set configuration properties
                        mnPort = CType(oNode.Item(sItem).InnerXml, Integer)
                        sItem = "rack"
                        mnRack = CType(oNode.Item(sItem).InnerXml, Integer)
                        sItem = "slot"
                        mnSlot = CType(oNode.Item(sItem).InnerXml, Integer)
                        sItem = "protocol"
                        msProtocol = oNode.Item(sItem).InnerXml
                        sItem = "ip"
                        msIPAddress = oNode.Item(sItem).InnerXml
                        sItem = "mpilocal"
                        mnMPIAddressLocal = CType(oNode.Item(sItem).InnerXml, Integer)
                        sItem = "mpiplc"
                        mnMPIAddressPlc = CType(oNode.Item(sItem).InnerXml, Integer)

                    Catch ex As Exception
                        mDebug.WriteEventToLog(msMODULE & ":clsConnection:subGetConfig", _
                                               "Configuration Item [" & sItem & "] not found.")
                        RaiseEvent ConnectionError(mPWCommon.PLCCommErrors.ConfigItemNotDefined, gcsRM.GetString("csPLC_TAG_NOT_DEFINED") & _
                                                   " [" & sItem & "]", msMODULE & ":clsConnection:subGetConfig", String.Empty)
                        Exit Sub

                    End Try

                Case Else
                    mDebug.WriteEventToLog(msMODULE & ":clsConnection:subGetConfig", _
                                           "(" & oNodeList.Count.ToString(CultureInfo.InvariantCulture) & _
                                           ") instances found of Tag [" & Strings.Replace(Name, " ", "_") & "].")
                    RaiseEvent ConnectionError(vbObjectError + 1010, gcsRM.GetString("csPLC_TAG_MULT_INST") & _
                                               " [" & Strings.Replace(Strings.Replace(Name, " ", "_"), " ", "_") & "]", _
                                               msMODULE & ":clsConnection:subGetConfig", String.Empty)
                    Exit Sub

            End Select

        End Sub

#End Region

#Region " Events "

        Protected Overrides Sub Finalize()
            '****************************************************************************************
            'Description: Cleanup
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

        Public Sub New(ByVal XMLPath As String, ByVal Name As String)
            '****************************************************************************************
            'Description: Initialize and Open the connection to the PLC
            '
            'Parameters: XMLPath - Configuration file location, Name - Connection Name
            'Returns:   
            '
            'Modification history:
            '
            ' Date      By      Reason
            '*****************************************************************************************

            Call subGetConfig(XMLPath, Name)
            OpenConnection()

        End Sub

#End Region

    End Class

    Private Class clsConnections
        '********************************************************************************************
        'Description:  This class holds the collection of Connections
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

        Friend Function Add(ByRef Connection As clsConnection) As Integer
            '********************************************************************************************
            'Description: Add a new Connection to the collection
            '
            'Parameters:
            'Returns:    The index in the collection of the added Connection   
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim nIndex As Integer = 0

            Try
                nIndex = List.Add(Connection)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnections:Add", ex.Message & vbCrLf & ex.StackTrace)
            End Try

            Return nIndex

        End Function

        Friend Function Item(ByVal Name As String) As clsConnection
            '********************************************************************************************
            'Description: Fetch the Connection item that matches the Name argument.
            '
            'Parameters: Name - Connection identifier
            'Returns:    clsConnection associated with the Name argument
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                Dim Connection As clsConnection

                For Each Connection In List
                    If Connection.Name = Name Then
                        Return Connection
                    End If
                Next
                'not found
                Return Nothing

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnections:Item", ex.Message & vbCrLf & ex.StackTrace)
                Return Nothing
            End Try

        End Function

        Friend Sub Remove(ByVal Connection As clsConnection)
            '********************************************************************************************
            'Description: Remove the Connection from the list
            '
            'Parameters: Connection - Connection to be removed
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                MyBase.List.Remove(Connection)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsConnections:Remove", ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub

#End Region

#Region " Events "

#End Region

        Public Sub New()

        End Sub
    End Class

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
        Private msArea As String = String.Empty
        Private mnAddress As Integer
        Private mnBit As Integer
        Private mnByte As Integer
        Private msConnectionName As String = String.Empty
        Private mnElapsedTime As Integer
        Private msIP_Address As String = String.Empty
        Private mnLength As Integer
        Private mnPollRate As Integer
        Private msRefData As String
        Private mnStart As Integer
        Private msTagName As String = String.Empty
        Private msType As String = String.Empty
        Private msZoneName As String = String.Empty
        Private mnSameDataCount As Integer
#End Region

#Region " Properties "

        Friend Property Address() As Integer
            '********************************************************************************************
            'Description: The PLC Data Table address number
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnAddress
            End Get

            Set(ByVal value As Integer)
                mnAddress = value
            End Set

        End Property

        Friend Property Area() As String
            '********************************************************************************************
            'Description: Data table area name.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msArea
            End Get

            Set(ByVal value As String)
                msArea = value
            End Set

        End Property

        Friend Property Bit() As Integer
            '********************************************************************************************
            'Description: The bit number for daveSetBit and daveClrBit operations.
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

            Set(ByVal value As Integer)
                mnBit = value
            End Set

        End Property

        Friend Property ConnectionName() As String
            '********************************************************************************************
            'Description: The name of the PLC Connection associated with this Link.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msConnectionName
            End Get

            Set(ByVal value As String)
                msConnectionName = value
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

        Friend Property Length() As Integer
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

            Set(ByVal value As Integer)
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

        Friend Property RefData() As String
            '********************************************************************************************
            'Description: In the case of a HotLink, this is the data that was returned from the last
            '             read operation. It is used to determine if a data change has occurred.
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

            Set(ByVal value As String)
                msRefData = value
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

        Friend Property Start() As Integer
            '********************************************************************************************
            'Description: The start word number in the PLC Data Table words being read/written
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnStart
            End Get

            Set(ByVal value As Integer)
                mnStart = value
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

        Friend Function Item(ByVal TagName As String) As clsLink
            '********************************************************************************************
            'Description: Fetch the Link item that matches the TagName argument.
            '
            'Parameters: TagName - Link identifier
            'Returns:    clsLink associated with the named Tag
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                Dim oLink As clsLink

                For Each oLink In List
                    If oLink.TagName = TagName Then
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

#End Region

    End Class  'clsLinks

End Class