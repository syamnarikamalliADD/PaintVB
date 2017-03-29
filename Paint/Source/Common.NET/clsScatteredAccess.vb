' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsScatteredAccess (clsScatteredAccess.vb)
'
' Description:
' This class module manages scattered access.
'
' Dependencies:  
'
' Language: Microsoft Visual Basic 2008 .NET
'
' Author: Matt White
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'  By           Date                                                    Version
'  MSW          03/28/12  First draft                                   4.01.03.00
'  MSW          04/19/12  Add a bunch of error logging                  4.01.03.01
'  MSW          04/27/12  Check for uninit vars                         4.01.03.02
'  MSW          05/02/12  Worked comm failure management.               4.01.03.03
'  MSW          10/26/12  subParseName - Fix parsing so it doesn't mark 4.01.03.04
'                         the wrong variables as shared
'  MSW          10/29/12  Add Short and Byte types to subReadFromRobot  4.01.03.05
'  MSW          11/13/12  subReadFromRobot-Add more error handling      4.01.03.06
'  MSW          12/13/12  Add checks for object is nothing, offline,    4.01.03.07
'                         not assigned
'  MSW          04/16/13  subReadFromRobot - One more check for         4.01.05.00
'                         oVar isnot nothing
'  MSW          05/09/13  Robot Connection Status                       4.01.05.01
'                         Looks like it's better to check the events than the current status,
'                         look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
'  MSW          06/05/13  Fix scaling and offset for scattered access items 4.01.05.02
'  MSW          06/24/13  Support reading variable structures               4.01.05.03
'  MSW          07/09/13  Improve recovery after robot communication is lost 4.01.05.04
'                         Update and standardize logos, PLCComm.Dll
'  MSW          07/19/13  Improve some error handling and messaging          4.01.05.05
'  MSW          09/04/13  clsScatteredAccessItems.Refresh, Update -          4.01.05.06
'                            Fix conditions for using robot object
'  DE           09/20/13  Modified clsScatteredAccessItemConfig Sub New to   4.01.05.07
'                         handle Cultures that use "," in floating point  
'                         numbers rather than ".".
'  MSW          09/30/13  clsScatteredAccessItem - New - Error message update 4.01.06.00
'*************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports FRRobot
Imports FRRobotNeighborhood
Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
'*************************************************************************
'Some data types to define choices available in SA
Public Enum eSAReadOrDisplay
    eSABoth
    eSAReadOnly
    eSADisplayOnly
End Enum
Public Enum eSADataType
    eSAString
    eSAInteger
    eSABoolean
    eSAFloat
End Enum
Public Enum eSAObjectType
    eSAKarelVar
    eSASystemVar
    eSAIO
    eSARegNumeric
    eSARegString
    eSAStructure
End Enum

Module ScatteredAccessRoutines
    Private Const SAEXE As String = "ScatteredAccess"
    Private Const msMODULE As String = "ScatteredAccessRoutines"
    Public Sub subStartupSA()
        '********************************************************************************************
        'Description: Make sure scattered access is running
        '
        'Parameters: 
        ' 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oProcs() As Process = Process.GetProcessesByName(SAEXE)
            If oProcs.Length = 0 Then
                oProcs = Process.GetProcessesByName(SAEXE & ".vshost")
                If oProcs.Length = 0 Then
                    'Not running, launch scattered access
                    Dim sVBApps As String = String.Empty
                    mPWCommon.GetDefaultFilePath(sVBApps, eDir.VBApps, String.Empty, SAEXE & ".exe")
                    Dim nProcessId As Integer = Shell(sVBApps, AppWinStyle.NormalFocus, False)
                    Threading.Thread.Sleep(1000)
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSetControllerReadRequests", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
End Module
Friend Class clsScatteredAccessItem
    '*************************************************************************
    'clsScatteredAccessItem Class
    'This class will contain one item for accessing a robot value within the controller's scattered access class
    '*************************************************************************

#Region "Declares"

    Private Const msMODULE As String = "clsScatteredAccessItem"
    Private moFRItem As Object 'PCDK object
    Private msName As String   'Name - for convenience.  used for access to collection, manager screen
    Private mfScale As Single  'Scale for integer or float values.  PCDK value is multiplied by this value, then the offset is added
    Private mfOffset As Single 'Offset for integer or float values.  Added to scaled value
    Private mnIndex As Integer 'Index to rgisters or IO
    Private meDataType As eSADataType 'Type of data we're expecting
    Private meObjectType As eSAObjectType 'Type of PCDK object
    Private meIOType As FRRobot.FREIOTypeConstants
    Private moValue As Object  'Store several forms of the value
    Private mnVal As Integer
    Private mfVal As Single
    Private msVal As String
    Private mbVal As Boolean
    Private moTag As Object = Nothing
    Private eReadOrDisplay As eSAReadOrDisplay = eSAReadOrDisplay.eSABoth
    ' MSW         07/09/13      Improve recovery after robot communication is lost
    Private moRobot As FRRobot.FRCRobot
    Private msVarName As String = String.Empty
    Private msProgName As String = String.Empty
    Private moRC As clsController = Nothing
#End Region

#Region "Properties"
    '*************************************************************************
    'clsScatteredAccessItem Properties
    'Mostly simple read/rwrite access
    '*************************************************************************

    Friend Property Tag() As Object
        '********************************************************************************************
        'Description: Store an object to pass around to everybody
        '
        'Parameters: none
        'Returns:    tag
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moTag
        End Get
        Set(ByVal value As Object)
            moTag = value
        End Set
    End Property
    Friend Property FRObject() As Object
        '********************************************************************************************
        'The PCDK object used to access this value
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return moFRItem
        End Get
        Set(ByVal value As Object)
            moFRItem = value
        End Set
    End Property
    Friend Property Name() As String
        '********************************************************************************************
        'Name - Used to identify the item, name for manager display - Not the variable name
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return msName
        End Get
        Set(ByVal value As String)
            msName = value
        End Set
    End Property
    Friend Property Scale() As Single
        '********************************************************************************************
        'Feedback Scale
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return mfScale
        End Get
        Set(ByVal value As Single)
            mfScale = value
        End Set
    End Property
    Friend Property Offset() As Single
        '********************************************************************************************
        'Feedback Offset
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return mfOffset
        End Get
        Set(ByVal value As Single)
            mfOffset = value
        End Set
    End Property
    Friend Property Index() As Integer
        '********************************************************************************************
        'IO or Register Index
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal value As Integer)
            mnIndex = value
        End Set
    End Property
    Friend Property DataType() As eSADataType
        '********************************************************************************************
        'Data Type - available with conversion
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return meDataType
        End Get
        Set(ByVal value As eSADataType)
            meDataType = value
        End Set
    End Property
    Friend Property SAObjectType() As eSAObjectType
        '********************************************************************************************
        'PCDK Object Type (IO, Var, Sysvar, Reg)
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return meObjectType
        End Get
        Set(ByVal value As eSAObjectType)
            meObjectType = value
        End Set
    End Property
    Friend Property IOType() As FRRobot.FREIOTypeConstants
        '********************************************************************************************
        'IO type
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return meIOType
        End Get
        Set(ByVal value As FRRobot.FREIOTypeConstants)
            meIOType = value
        End Set
    End Property

    Public Property ReadOrDisplay() As eSAReadOrDisplay
        Get
            Return eReadOrDisplay
        End Get
        Set(ByVal value As eSAReadOrDisplay)
            eReadOrDisplay = value
        End Set
    End Property
#End Region
#Region "ReadWriteProperties"
    Friend Property Value() As Object
        '********************************************************************************************
        'Access value as generic object
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Get
            Try
                subReadFromRobot()
                Return moValue
            Catch ex As Exception
                'Reconnect and retry
                Renew()
            End Try
            subReadFromRobot()
            Return moValue
        End Get
        Set(ByVal value As Object)
            Try
                moValue = value
                If TypeOf (value) Is Integer Then
                    mnVal = DirectCast(moValue, Integer)
                    mfVal = mnVal
                    mbVal = (mnVal > 0)
                    msVal = mnVal.ToString
                ElseIf TypeOf (value) Is Single Then
                    mfVal = DirectCast(moValue, Single)
                    mnVal = CInt(mfVal)
                    mbVal = (mfVal > 0)
                    msVal = mfVal.ToString
                ElseIf TypeOf (value) Is Boolean Then
                    mbVal = DirectCast(moValue, Boolean)
                    If mbVal Then
                        mnVal = 1
                        mfVal = 1
                    Else
                        mnVal = 0
                        mfVal = 0
                    End If
                    'For easier transition, old SA returned 1 and 0 for boolean
                    'msVal = mbVal.ToString
                    msVal = mnVal.ToString
                ElseIf TypeOf (value) Is String Then
                    msVal = DirectCast(moValue, String)
                    If IsNumeric(msVal) Then
                        mfVal = CType(msVal, Single)
                        mnVal = CInt(mfVal)
                        mbVal = (mfVal > 0)
                    Else
                        mnVal = 0
                        mfVal = 0.0
                        Try
                            mbVal = CType(msVal, Boolean)
                        Catch ex As Exception
                            mbVal = False
                        End Try
                    End If
                ElseIf TypeOf (value) Is Double Then
                    Dim dTmp As Double = DirectCast(moValue, Double)
                    mfVal = CType(dTmp, Single)
                    mnVal = CInt(mfVal)
                    mbVal = (mfVal > 0)
                    msVal = mfVal.ToString
                ElseIf TypeOf (value) Is Decimal Then
                    Dim decTmp As Decimal = DirectCast(moValue, Decimal)
                    mfVal = CType(decTmp, Single)
                    mnVal = CInt(mfVal)
                    mbVal = (mfVal > 0)
                    msVal = mfVal.ToString
                ElseIf TypeOf (value) Is Long Then
                    Dim lTmp As Long = DirectCast(moValue, Long)
                    mnVal = CType(lTmp, Integer)
                    mfVal = mnVal
                    mbVal = (mnVal > 0)
                    msVal = mnVal.ToString
                End If

                subWriteToRobot()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: Value", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property nValue() As Integer
        '********************************************************************************************
        'Access value as Integer
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Get
            Try
                subReadFromRobot()
                Return mnVal
            Catch ex As Exception
                'Reconnect and retry
                Renew()
            End Try
            subReadFromRobot()
            Return mnVal
        End Get
        Set(ByVal value As Integer)
            Try
                moValue = value
                mnVal = value
                mfVal = value
                msVal = value.ToString
                mbVal = (value > 0)
                subWriteToRobot()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: nValue", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property fValue() As Single
        '********************************************************************************************
        'Access value as floating point
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Get
            Try
                subReadFromRobot()
                Return mfVal
            Catch ex As Exception
                'Reconnect and retry
                Renew()
            End Try
            subReadFromRobot()
            Return mfVal
        End Get
        Set(ByVal value As Single)
            Try
                moValue = value
                mnVal = CInt(value)
                mfVal = value
                msVal = value.ToString
                mbVal = (value > 0)
                subWriteToRobot()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: fValue", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property sValue() As String
        '********************************************************************************************
        'Access value as String
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Get
            Try
                subReadFromRobot()
                Return msVal
            Catch ex As Exception
                'Reconnect and retry
                Renew()
            End Try
            subReadFromRobot()
            Return msVal
        End Get
        Set(ByVal value As String)
            Try
                moValue = value
                msVal = value
                Try
                    mnVal = CType(value, Integer)
                Catch ex As Exception
                    mnVal = 0
                End Try
                Try
                    mfVal = CType(value, Single)
                Catch ex As Exception
                    mfVal = 0.0
                End Try
                Try
                    mbVal = CType(value, Boolean)
                Catch ex As Exception
                    mbVal = (mnVal > 0)
                End Try
                subWriteToRobot()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: sValue", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property bValue() As Boolean
        '********************************************************************************************
        'Access value as boolean
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Get
            Try
                subReadFromRobot()
                Return mbVal
            Catch ex As Exception
                'Reconnect and retry
                Renew()
            End Try
            subReadFromRobot()
            Return mbVal
        End Get
        Set(ByVal value As Boolean)
            Try
                moValue = value
                msVal = value.ToString
                Try
                    mbVal = CType(value, Boolean)
                Catch ex As Exception
                    mbVal = False
                End Try
                Try
                    mnVal = CInt(value)
                Catch ex As Exception
                    mnVal = 0
                End Try
                Try
                    mfVal = CType(value, Single)
                Catch ex As Exception
                    mfVal = 0.0
                End Try
                subWriteToRobot()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bValue", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property NoRefresh() As Boolean
        '********************************************************************************************
        'Access to object's NoRefresh property
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/13/12      Add checks for object is nothing, offline, not assigned
        '********************************************************************************************
        Get
            Try
                Select Case meObjectType
                    Case eSAObjectType.eSAIO
                        Dim oPt As FRCIOSignal = DirectCast(moFRItem, FRRobot.FRCIOSignal)

                        If oPt IsNot Nothing AndAlso oPt.IsAssigned AndAlso (oPt.IsOffline = False) Then
                            Return oPt.NoRefresh
                        Else
                            Return False
                        End If

                    Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar, eSAObjectType.eSARegString, eSAObjectType.eSARegString
                        Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                        Return oVar.NoRefresh
                End Select
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: NoRefresh", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                If eReadOrDisplay <> eSAReadOrDisplay.eSADisplayOnly Then
                    Select Case meObjectType
                        Case eSAObjectType.eSAIO
                            Dim oPt As FRCIOSignal = DirectCast(moFRItem, FRRobot.FRCIOSignal)
                            If oPt IsNot Nothing AndAlso oPt.IsAssigned AndAlso (oPt.IsOffline = False) Then
                                oPt.NoRefresh = value
                            End If
                        Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar, eSAObjectType.eSARegString, eSAObjectType.eSARegString
                            Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                            oVar.NoRefresh = value
                        Case eSAObjectType.eSAStructure
                            Dim oVars As FRRobot.FRCVars = DirectCast(moFRItem, FRRobot.FRCVars)
                            oVars.NoRefresh = value
                    End Select
                End If
            Catch ex As Exception
                Dim sName As String = moRC.Name & " " & msName
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: NoRefresh", _
                       sName & " - Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property NoUpdate() As Boolean
        '********************************************************************************************
        'Access to object's NoUpdate property
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/13/12      Add checks for object is nothing, offline, not assigned
        '********************************************************************************************
        Get
            Try
                Select Case meObjectType
                    Case eSAObjectType.eSAIO
                        Dim oPt As FRCIOSignal = DirectCast(moFRItem, FRRobot.FRCIOSignal)
                        If oPt IsNot Nothing AndAlso oPt.IsAssigned AndAlso (oPt.IsOffline = False) Then
                            Return oPt.NoUpdate
                        Else
                            Return False
                        End If
                    Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar, eSAObjectType.eSARegString, eSAObjectType.eSARegString
                        Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                        Return oVar.NoUpdate
                End Select
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: NoUpdate", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                Select Case meObjectType
                    Case eSAObjectType.eSAIO
                        Dim oPt As FRCIOSignal = DirectCast(moFRItem, FRRobot.FRCIOSignal)
                        If oPt IsNot Nothing AndAlso oPt.IsAssigned AndAlso (oPt.IsOffline = False) Then
                            oPt.NoUpdate = value
                        End If
                    Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar, eSAObjectType.eSARegString, eSAObjectType.eSARegString
                        Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                        oVar.NoUpdate = value
                End Select
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: NoUpdate", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
#End Region
#Region "ReadWriteRoutines"

    Private Sub subReadFromRobot()
        '********************************************************************************************
        'Read data from robot
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         10/29/12      Add short and byte types
        ' MSW         12/13/12      Add checks for object is nothing, offline, not assigned
        ' MSW         04/16/13      subReadFromRobot - One more check for oVar isnot nothing
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        'Try/Catch blocks were removed so it can do error recovery in the calling routines - leave them out
        'Try
            Select Case meObjectType
                Case eSAObjectType.eSAIO
                'Try
                        Select Case meIOType
                            Case FREIOTypeConstants.frAInType, FREIOTypeConstants.frAOutType
                                Dim oPt As FRRobot.FRCAnalogIOSignal = DirectCast(moFRItem, FRRobot.FRCAnalogIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = 0
                                    mnVal = 0
                                Else
                                    moValue = oPt.Value
                                    mnVal = oPt.Value
                                End If
                                mfVal = mnVal
                                mbVal = (mnVal > 0)
                            Case FREIOTypeConstants.frDInType, FREIOTypeConstants.frDOutType
                                Dim oPt As FRRobot.FRCDigitalIOSignal = DirectCast(moFRItem, FRRobot.FRCDigitalIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frFlagType
                                Dim oPt As FRRobot.FRCFlagSignal = DirectCast(moFRItem, FRRobot.FRCFlagSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frGPInType, FREIOTypeConstants.frGPOutType
                                Dim oPt As FRRobot.FRCGroupIOSignal = DirectCast(moFRItem, FRRobot.FRCGroupIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = 0
                                    mnVal = 0
                                Else
                                    moValue = oPt.Value
                                    mnVal = oPt.Value
                                End If
                                mfVal = mnVal
                                mbVal = (mnVal > 0)
                            Case FREIOTypeConstants.frMarkerType
                                Dim oPt As FRRobot.FRCMarkerSignal = DirectCast(moFRItem, FRRobot.FRCMarkerSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frPLCInType, FREIOTypeConstants.frPLCOutType
                                Dim oPt As FRRobot.FRCPLCIOSignal = DirectCast(moFRItem, FRRobot.FRCPLCIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                moValue = oPt.Value
                                mbVal = oPt.Value
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frRDInType, FREIOTypeConstants.frRDOutType
                                Dim oPt As FRRobot.FRCRobotIOSignal = DirectCast(moFRItem, FRRobot.FRCRobotIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frSOPInType, FREIOTypeConstants.frSOPOutType
                                Dim oPt As FRRobot.FRCSOPIOSignal = DirectCast(moFRItem, FRRobot.FRCSOPIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frTPInType, FREIOTypeConstants.frTPOutType
                                Dim oPt As FRRobot.FRCTPIOSignal = DirectCast(moFRItem, FRRobot.FRCTPIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frUOPInType, FREIOTypeConstants.frUOPOutType
                                Dim oPt As FRRobot.FRCUOPIOSignal = DirectCast(moFRItem, FRRobot.FRCUOPIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                            Case FREIOTypeConstants.frWDInType, FREIOTypeConstants.frWDOutType
                                Dim oPt As FRRobot.FRCWeldDigitalIOSignal = DirectCast(moFRItem, FRRobot.FRCWeldDigitalIOSignal)
                                If oPt Is Nothing OrElse oPt.IsOffline OrElse (oPt.IsAssigned = False) Then
                                    moValue = False
                                    mbVal = False
                                Else
                                    moValue = oPt.Value
                                    mbVal = oPt.Value
                                End If
                                If mbVal Then
                                    mnVal = 1
                                    mfVal = 1
                                Else
                                    mnVal = 0
                                    mfVal = 0
                                End If
                        End Select
                        'For compatability with the old object, return boolean as integer
                        msVal = mnVal.ToString

                'Catch ex As Exception
                '    moValue = 0
                '    mbVal = False
                '    mnVal = 0
                '    mfVal = 0
                '    msVal = mnVal.ToString
                '    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadFromRobot", _
                '           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                'End Try
                Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar
                'Try
                        Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                        If oVar IsNot Nothing AndAlso oVar.IsInitialized Then 'msw 4/8/13
                            If oVar Is Nothing Then
                                moValue = String.Empty
                                mnVal = 0
                                mfVal = 0
                                mbVal = False
                                msVal = String.Empty
                            Else
                                moValue = oVar.Value
                                If TypeOf (oVar.Value) Is Short Then
                                    mnVal = DirectCast(moValue, Short)
                                    mfVal = mnVal
                                    mbVal = (mnVal > 0)
                                    msVal = mnVal.ToString
                                ElseIf TypeOf (oVar.Value) Is Byte Then
                                    mnVal = DirectCast(moValue, Byte)
                                    mfVal = mnVal
                                    mbVal = (mnVal > 0)
                                    msVal = mnVal.ToString
                                ElseIf TypeOf (oVar.Value) Is Integer Then
                                    mnVal = DirectCast(moValue, Integer)
                                    mfVal = mnVal
                                    mbVal = (mnVal > 0)
                                    msVal = mnVal.ToString
                                ElseIf TypeOf (oVar.Value) Is Single Then
                                    mfVal = DirectCast(moValue, Single)
                                    mnVal = CInt(mfVal)
                                    mbVal = (mfVal > 0)
                                    msVal = mfVal.ToString
                                ElseIf TypeOf (oVar.Value) Is Boolean Then
                                    mbVal = DirectCast(moValue, Boolean)
                                    If mbVal Then
                                        mnVal = 1
                                        mfVal = 1
                                    Else
                                        mnVal = 0
                                        mfVal = 0
                                    End If
                                    'For compatability with the old object, return boolean as integer
                                    'msVal = mbVal.ToString
                                    msVal = mnVal.ToString
                                ElseIf TypeOf (oVar.Value) Is String Then
                                    msVal = DirectCast(moValue, String)
                                    If IsNumeric(msVal) Then
                                        mfVal = CType(msVal, Single)
                                        mnVal = CInt(mfVal)
                                        mbVal = (mfVal > 0)
                                    Else
                                        mnVal = 0
                                        mfVal = 0.0
                                        'Try
                                        '    mbVal = CType(msVal, Boolean)
                                        'Catch ex As Exception
                                        '    mbVal = False
                                        'End Try
                                    End If
                                End If

                            End If
                        Else
                            moValue = String.Empty
                            mnVal = 0
                            mfVal = 0
                            mbVal = False
                            msVal = String.Empty
                        End If
                'Catch ex As Exception
                '    moValue = String.Empty
                '    mnVal = 0
                '    mfVal = 0
                '    mbVal = False
                '    msVal = String.Empty
                '    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadFromRobot", _
                '           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                'End Try
                Case eSAObjectType.eSARegNumeric
                'Try
                        'Dim oReg As FRRobot.FRCRegNumeric = DirectCast(moFRItem, FRRobot.FRCRegNumeric)
                        Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                        Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oVar.Value, FRRobot.FRCRegNumeric)
                        If oReg Is Nothing Then
                            moValue = 0
                            mnVal = 0
                            mfVal = 0
                            mbVal = False
                            msVal = "0"
                        Else
                            Select Case oReg.Type
                                Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, FRETypeCodeConstants.frShortType
                                    moValue = oReg.RegLong
                                    mnVal = oReg.RegLong
                                    mfVal = mnVal
                                    mbVal = (mnVal > 0)
                                    msVal = mnVal.ToString
                                Case FRETypeCodeConstants.frRealType
                                    moValue = oReg.RegFloat
                                    mfVal = oReg.RegFloat
                                    mnVal = CInt(mfVal)
                                    mbVal = (mfVal > 0)
                                    msVal = mfVal.ToString
                            End Select
                        End If

                'Catch ex As Exception
                '    moValue = 0
                '    mnVal = 0
                '    mfVal = 0
                '    mbVal = False
                '    msVal = "0"
                '    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadFromRobot", _
                '           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                'End Try
                Case eSAObjectType.eSARegString
                'Try
                        Dim oRegStr As FRRobot.FRCRegString = DirectCast(moFRItem, FRRobot.FRCRegString)
                        If oRegStr Is Nothing Then
                            moValue = String.Empty
                            mnVal = 0
                            mfVal = 0
                            mbVal = False
                            msVal = String.Empty
                        Else
                            moValue = oRegStr.Value
                            msVal = oRegStr.Value
                            If IsNumeric(msVal) Then
                                mfVal = CType(msVal, Single)
                                mnVal = CInt(mfVal)
                                mbVal = (mfVal > 0)
                            Else
                                mnVal = 0
                                mfVal = 0.0
                                Try
                                    mbVal = CType(msVal, Boolean)
                                Catch ex As Exception
                                    mbVal = False
                                End Try
                            End If
                        End If
                'Catch ex As Exception
                '    moValue = String.Empty
                '    mnVal = 0
                '    mfVal = 0
                '    mbVal = False
                '    msVal = String.Empty
                '    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadFromRobot", _
                '           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                'End Try
            End Select
            If mfScale <> 0 Then
                mfVal = (mfVal * mfScale) + mfOffset
                mnVal = CInt(mfVal)
                msVal = mfVal.ToString
                moValue = mfVal
            End If
        'Catch ex As Exception
        '    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadFromRobot", _
        '   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        'End Try
    End Sub
    Private Sub subWriteToRobot()
        '********************************************************************************************
        'write data to robot
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try
            Dim fTmp As Single = mfVal
            Dim nTmp As Integer = mnVal
            If mfScale <> 0 And ((meDataType = eSADataType.eSAFloat) Or (meDataType = eSADataType.eSAInteger)) Then
                fTmp = (mfVal - mfOffset) / mfScale
                nTmp = CInt(fTmp)
                msVal = mfVal.ToString
            End If
            Select Case meObjectType
                Case eSAObjectType.eSAIO
                    Select Case meIOType
                        Case FREIOTypeConstants.frAInType, FREIOTypeConstants.frAOutType
                            Dim oPt As FRRobot.FRCAnalogIOSignal = DirectCast(moFRItem, FRRobot.FRCAnalogIOSignal)
                            oPt.Value = nTmp
                        Case FREIOTypeConstants.frDInType, FREIOTypeConstants.frDOutType
                            Dim oPt As FRRobot.FRCDigitalIOSignal = DirectCast(moFRItem, FRRobot.FRCDigitalIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frFlagType
                            Dim oPt As FRRobot.FRCFlagSignal = DirectCast(moFRItem, FRRobot.FRCFlagSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frGPInType, FREIOTypeConstants.frGPOutType
                            Dim oPt As FRRobot.FRCGroupIOSignal = DirectCast(moFRItem, FRRobot.FRCGroupIOSignal)
                            oPt.Value = nTmp
                        Case FREIOTypeConstants.frMarkerType
                            Dim oPt As FRRobot.FRCMarkerSignal = DirectCast(moFRItem, FRRobot.FRCMarkerSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frPLCInType, FREIOTypeConstants.frPLCOutType
                            Dim oPt As FRRobot.FRCPLCIOSignal = DirectCast(moFRItem, FRRobot.FRCPLCIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frRDInType, FREIOTypeConstants.frRDOutType
                            Dim oPt As FRRobot.FRCRobotIOSignal = DirectCast(moFRItem, FRRobot.FRCRobotIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frSOPInType, FREIOTypeConstants.frSOPOutType
                            Dim oPt As FRRobot.FRCSOPIOSignal = DirectCast(moFRItem, FRRobot.FRCSOPIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frTPInType, FREIOTypeConstants.frTPOutType
                            Dim oPt As FRRobot.FRCTPIOSignal = DirectCast(moFRItem, FRRobot.FRCTPIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frUOPInType, FREIOTypeConstants.frUOPOutType
                            Dim oPt As FRRobot.FRCUOPIOSignal = DirectCast(moFRItem, FRRobot.FRCUOPIOSignal)
                            oPt.Value = mbVal
                        Case FREIOTypeConstants.frWDInType, FREIOTypeConstants.frWDOutType
                            Dim oPt As FRRobot.FRCWeldDigitalIOSignal = DirectCast(moFRItem, FRRobot.FRCWeldDigitalIOSignal)
                            oPt.Value = mbVal
                    End Select
                Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar
                    Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                    moValue = oVar.Value
                    If TypeOf (oVar.Value) Is Integer Then
                        oVar.Value = nTmp
                    ElseIf TypeOf (oVar.Value) Is Single Then
                        oVar.Value = fTmp
                    ElseIf TypeOf (oVar.Value) Is Boolean Then
                        oVar.Value = mbVal
                    ElseIf TypeOf (oVar.Value) Is String Then
                        oVar.Value = msVal
                    End If
                Case eSAObjectType.eSARegString
                    Dim oReg As FRRobot.FRCRegNumeric = DirectCast(moFRItem, FRRobot.FRCRegNumeric)
                    Select Case oReg.Type
                        Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, FRETypeCodeConstants.frShortType
                            oReg.RegLong = nTmp
                        Case FRETypeCodeConstants.frRealType
                            oReg.RegFloat = fTmp
                    End Select
                Case eSAObjectType.eSARegString
                    Dim oRegStr As FRRobot.FRCRegString = DirectCast(moFRItem, FRRobot.FRCRegString)
                    oRegStr.Value = msVal
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subWriteToRobot", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
#End Region
    Private Function subParseIndex(ByRef sStringData As String, ByVal nEq As Integer, ByRef bShared As Boolean) As Integer
        '********************************************************************************************
        'Description: configure the IO index for equipment number
        '
        'Parameters: incoming string, equipment #, shared status 
        ' 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim sTmp As String = subParseName(sStringData, nEq, bShared)
            Return (CType(sTmp, Integer))
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subParseIndex(" & _
                                   sStringData & "," & nEq.ToString & ")", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return 0
        End Try
    End Function
    Private Function subParseName(ByRef sStringData As String, ByVal nEq As Integer, ByRef bShared As Boolean) As String
        '********************************************************************************************
        'Description: configure the variable name or IO index for equipment number
        '
        'Parameters: incoming string, equipment #, shared status 
        ' 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/26/12  MSW     subParseName - Fix parsing so it doesn't mark the wrong variables as shared
        '********************************************************************************************
        Dim sName As String = sStringData
        Try
            bShared = True
            If InStr(sStringData, ";") > 0 Then
                Try
                    Dim sTmpArray() As String = Split(sStringData, ";")
                    For nItem As Integer = 0 To sTmpArray.GetUpperBound(0) - 1 Step 2
                        If sTmpArray(nItem) = ("#EQ" & nEq.ToString & "#") Then
                            sName = sTmpArray(nItem + 1)
                            bShared = False
                            Exit For
                        End If
                        If sTmpArray(nItem) = nEq.ToString Then
                            sName = sTmpArray(nItem + 1)
                            bShared = False
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subParseName", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            Else
                If InStr(sName, "#EQ#", CompareMethod.Text) > 0 Then
                    bShared = False
                    sName = sStringData.Replace("#EQ#", nEq.ToString)
                    sName = sName.Replace("#Eq#", nEq.ToString)
                End If
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subParseName", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        Return (sName)
    End Function
    Public Sub Renew()
        '********************************************************************************************
        'Description: get the object from PCDK after it went bad
        '
        'Parameters: Eq# range, all for a controller, or the specific arm number, 
        '   item config
        '   Robot object
        ' 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' MSW         07/09/13      Improve recovery after robot communication is lost
        '********************************************************************************************
        Try
            'Private moFRItem As Object 'PCDK object
            'Private meDataType As eSADataType 'Type of data we're expecting
            'Private meObjectType As eSAObjectType 'Type of PCDK object
            'Private meIOType As FRRobot.FREIOTypeConstants
            Select Case meObjectType
                Case eSAObjectType.eSAIO
                    Dim oIOSignals As FRRobot.FRCIOSignals
                    Select Case meIOType
                        Case FRRobot.FREIOTypeConstants.frAInType, FRRobot.FREIOTypeConstants.frAOutType
                            Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCAnalogIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSAInteger
                        Case FRRobot.FREIOTypeConstants.frDInType, FRRobot.FREIOTypeConstants.frDOutType
                            Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCDigitalIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frFlagType
                            Dim oIOType As FRRobot.FRCFlagType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCFlagType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frGPInType, FRRobot.FREIOTypeConstants.frGPOutType
                            Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCGroupIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSAInteger
                        Case FRRobot.FREIOTypeConstants.frMarkerType
                            Dim oIOType As FRRobot.FRCMarkerType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCMarkerType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frPLCInType, FRRobot.FREIOTypeConstants.frPLCOutType
                            Dim oIOType As FRRobot.FRCPLCIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCPLCIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frRDInType, FRRobot.FREIOTypeConstants.frRDOutType
                            Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCRobotIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frSOPInType, FRRobot.FREIOTypeConstants.frSOPOutType
                            Dim oIOType As FRRobot.FRCSOPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCSOPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frTPInType, FRRobot.FREIOTypeConstants.frTPOutType
                            Dim oIOType As FRRobot.FRCTPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCTPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frUOPInType, FRRobot.FREIOTypeConstants.frUOPOutType
                            Dim oIOType As FRRobot.FRCUOPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCUOPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frWDInType, FRRobot.FREIOTypeConstants.frWDOutType
                            Dim oIOType As FRRobot.FRCWeldDigitalIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCWeldDigitalIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case Else
                    End Select
                Case eSAObjectType.eSAStructure
                    If msProgName <> String.Empty Then
                        moFRItem = moRobot.Programs(msProgName).Variables(msVarName)
                    Else
                        moFRItem = moRobot.SysVariables(msVarName)
                    End If
                    Dim oVars As FRRobot.FRCVars = DirectCast(moFRItem, FRRobot.FRCVars)
                Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar
                    If meObjectType = eSAObjectType.eSAKarelVar Then
                        moFRItem = moRobot.Programs(msProgName).Variables(msVarName)
                    Else
                        moFRItem = moRobot.SysVariables(msVarName)
                    End If
                    Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                    If oVar.IsInitialized Then
                        If TypeOf (oVar.Value) Is Integer Then
                            meDataType = eSADataType.eSAInteger
                        ElseIf TypeOf (oVar.Value) Is Single Then
                            meDataType = eSADataType.eSAFloat
                        ElseIf TypeOf (oVar.Value) Is Boolean Then
                            meDataType = eSADataType.eSABoolean
                        ElseIf TypeOf (oVar.Value) Is String Then
                            meDataType = eSADataType.eSAString
                        End If
                    Else
                        Select Case oVar.TypeCode
                            Case FRETypeCodeConstants.frBooleanType
                                meDataType = eSADataType.eSABoolean
                            Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, _
                                FRETypeCodeConstants.frShortType
                                meDataType = eSADataType.eSAInteger
                            Case FRETypeCodeConstants.frRealType
                                meDataType = eSADataType.eSAFloat
                            Case FRETypeCodeConstants.frStringType
                                meDataType = eSADataType.eSAString
                        End Select
                        meDataType = eSADataType.eSAInteger
                    End If
                Case eSAObjectType.eSARegNumeric
                    moFRItem = moRobot.RegNumerics(mnIndex.ToString)
                    Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                    Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oVar.Value, FRRobot.FRCRegNumeric)
                    Select Case oReg.Type
                        Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, FRETypeCodeConstants.frShortType
                            meDataType = eSADataType.eSAInteger
                        Case FRETypeCodeConstants.frRealType
                            meDataType = eSADataType.eSAFloat
                    End Select
                Case eSAObjectType.eSARegString
                    moFRItem = moRobot.RegStrings(mnIndex.ToString)
            End Select
            If moFRItem Is Nothing Then
                Debug.Assert(False)
            Else
                subReadFromRobot()
            End If
        Catch ex As Exception
            Dim sName As String = moRC.Name & " " & msName
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: new", _
           sName & " - Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Public Sub New(ByRef nEq As Integer, ByRef oItemConfig As clsScatteredAccessItemConfig, _
                   ByRef oRC As clsController, ByRef bShared As Boolean, _
                   ByRef bByEgName As Boolean)
        '********************************************************************************************
        'Description: Add Items from an item config
        '
        'Parameters: Eq# range, all for a controller, or the specific arm number, 
        '   item config
        '   Robot object
        ' 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/09/13  MSW     Improve recovery after robot communication is lost
        ' 09/30/13  MSW     clsScatteredAccessItem - New - Error message update
        '********************************************************************************************
        bShared = False
        Try
            'Private moFRItem As Object 'PCDK object
            'Private meDataType As eSADataType 'Type of data we're expecting
            'Private meObjectType As eSAObjectType 'Type of PCDK object
            'Private meIOType As FRRobot.FREIOTypeConstants
            If bByEgName Then
                msName = "Eq" & nEq.ToString & ": " & oItemConfig.Name
            Else
                msName = oItemConfig.Name
            End If
            mfScale = oItemConfig.Scale
            mfOffset = oItemConfig.Offset
            meObjectType = oItemConfig.ObjectType
            eReadOrDisplay = oItemConfig.ReadOrDisplay
            moRobot = oRC.Robot
            moRC = oRC
            Select Case meObjectType
                Case eSAObjectType.eSAIO
                    meIOType = oItemConfig.IOType
                    mnIndex = subParseIndex(oItemConfig.Index, nEq, bShared)
                    Dim oIOSignals As FRRobot.FRCIOSignals
                    Select Case meIOType
                        Case FRRobot.FREIOTypeConstants.frAInType, FRRobot.FREIOTypeConstants.frAOutType
                            Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCAnalogIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSAInteger
                        Case FRRobot.FREIOTypeConstants.frDInType, FRRobot.FREIOTypeConstants.frDOutType
                            Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCDigitalIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frFlagType
                            Dim oIOType As FRRobot.FRCFlagType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCFlagType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frGPInType, FRRobot.FREIOTypeConstants.frGPOutType
                            Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCGroupIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSAInteger
                        Case FRRobot.FREIOTypeConstants.frMarkerType
                            Dim oIOType As FRRobot.FRCMarkerType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCMarkerType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frPLCInType, FRRobot.FREIOTypeConstants.frPLCOutType
                            Dim oIOType As FRRobot.FRCPLCIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCPLCIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frRDInType, FRRobot.FREIOTypeConstants.frRDOutType
                            Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCRobotIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frSOPInType, FRRobot.FREIOTypeConstants.frSOPOutType
                            Dim oIOType As FRRobot.FRCSOPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCSOPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frTPInType, FRRobot.FREIOTypeConstants.frTPOutType
                            Dim oIOType As FRRobot.FRCTPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCTPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frUOPInType, FRRobot.FREIOTypeConstants.frUOPOutType
                            Dim oIOType As FRRobot.FRCUOPIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCUOPIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case FRRobot.FREIOTypeConstants.frWDInType, FRRobot.FREIOTypeConstants.frWDOutType
                            Dim oIOType As FRRobot.FRCWeldDigitalIOType = DirectCast(moRobot.IOTypes(meIOType), FRRobot.FRCWeldDigitalIOType)
                            oIOSignals = oIOType.Signals
                            moFRItem = oIOSignals(mnIndex)
                            meDataType = eSADataType.eSABoolean
                        Case Else
                    End Select
                Case eSAObjectType.eSAStructure
                    msProgName = subParseName(oItemConfig.Program, nEq, bShared)
                    msVarName = subParseName(oItemConfig.Variable, nEq, bShared)
                    mnIndex = 0
                    If msProgName <> String.Empty Then
                        moFRItem = moRobot.Programs(msProgName).Variables(msVarName)
                    Else
                        moFRItem = moRobot.SysVariables(msVarName)
                    End If
                    Dim oVars As FRRobot.FRCVars = DirectCast(moFRItem, FRRobot.FRCVars)
                Case eSAObjectType.eSAKarelVar, eSAObjectType.eSASystemVar
                    msProgName = subParseName(oItemConfig.Program, nEq, bShared)
                    msVarName = subParseName(oItemConfig.Variable, nEq, bShared)
                    mnIndex = 0
                    If meObjectType = eSAObjectType.eSAKarelVar Then
                        moFRItem = moRobot.Programs(msProgName).Variables(msVarName)
                    Else
                        moFRItem = moRobot.SysVariables(msVarName)
                    End If
                    Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                    If oVar.IsInitialized Then
                        If TypeOf (oVar.Value) Is Integer Then
                            meDataType = eSADataType.eSAInteger
                        ElseIf TypeOf (oVar.Value) Is Single Then
                            meDataType = eSADataType.eSAFloat
                        ElseIf TypeOf (oVar.Value) Is Boolean Then
                            meDataType = eSADataType.eSABoolean
                        ElseIf TypeOf (oVar.Value) Is String Then
                            meDataType = eSADataType.eSAString
                        End If
                    Else
                        Select Case oVar.TypeCode
                            Case FRETypeCodeConstants.frBooleanType
                                meDataType = eSADataType.eSABoolean
                            Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, _
                                FRETypeCodeConstants.frShortType
                                meDataType = eSADataType.eSAInteger
                            Case FRETypeCodeConstants.frRealType
                                meDataType = eSADataType.eSAFloat
                            Case FRETypeCodeConstants.frStringType
                                meDataType = eSADataType.eSAString
                        End Select
                        meDataType = eSADataType.eSAInteger
                    End If
                Case eSAObjectType.eSARegNumeric
                    mnIndex = subParseIndex(oItemConfig.Index, nEq, bShared)
                    moFRItem = moRobot.RegNumerics(mnIndex.ToString)
                    Dim oVar As FRRobot.FRCVar = DirectCast(moFRItem, FRRobot.FRCVar)
                    Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oVar.Value, FRRobot.FRCRegNumeric)
                    Select Case oReg.Type
                        Case FRETypeCodeConstants.frByteType, FRETypeCodeConstants.frIntegerType, FRETypeCodeConstants.frShortType
                            meDataType = eSADataType.eSAInteger
                        Case FRETypeCodeConstants.frRealType
                            meDataType = eSADataType.eSAFloat
                    End Select
                Case eSAObjectType.eSARegString
                    mnIndex = subParseIndex(oItemConfig.Index, nEq, bShared)
                    meDataType = eSADataType.eSAString
                    moFRItem = moRobot.RegStrings(mnIndex.ToString)
            End Select
            If moFRItem Is Nothing Then
                Debug.Assert(False)
            Else
                subReadFromRobot()
            End If
        Catch ex As Exception
            Dim sName As String = moRC.Name & " " & msName
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: new", _
                                   sName & " - Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
End Class

Friend Class clsScatteredAccessItems

    '*************************************************************************
    'clsScatteredAccessCntr Class
    'This class holds all the scattered access items for a controller
    'It provides access for creating and maintaining a scattered access object, 
    'or for accessing a scattered access object maintained by another program
    '*************************************************************************

    Private Const msMODULE As String = "clsScatteredAccessItems"
    Private sName As String
    Private nNumber As Integer
    Private moSAObject As FRRobot.FRCScatteredAccess
    Private moController As clsController = Nothing
    Private moRobot As FRRobot.FRCRobot = Nothing
    Private moItems As Collection = Nothing
    Private mbSetupOK As Boolean = True
    Private mbObjectOK As Boolean = False
    Public ReadOnly Property Data() As String()
        '********************************************************************************************
        'Description: return a string array compatable with howthe old object was used
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                Dim sData(moItems.Count * 2 - 1) As String
                Dim nIndex As Integer = 0
                For Each oSAItem As clsScatteredAccessItem In moItems
                    sData(nIndex) = oSAItem.Name
                    sData(nIndex + 1) = oSAItem.sValue
                    nIndex = nIndex + 2

                Next
                Return sData
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: Data", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                Return Nothing
            End Try
        End Get
    End Property
    Public ReadOnly Property SetupOK() As Boolean
        '********************************************************************************************
        'Description: True if the items were configured OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return  mbSetupOK
        End Get
    End Property
    Public ReadOnly Property ObjectOK() As Boolean
        '********************************************************************************************
        'Description: True if the scattered access object was created OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return (moSAObject IsNot Nothing) 'mbObjectOK
        End Get
    End Property
    Public Property Name() As String
        '********************************************************************************************
        'Description: Store the controller name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property
    Public Property Number() As Integer
        '********************************************************************************************
        'Description: Store the controller number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return nNumber
        End Get
        Set(ByVal value As Integer)
            nNumber = value
        End Set
    End Property
    Public ReadOnly Property Items() As Collection
        '********************************************************************************************
        'Description: Access to individual items
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moItems
        End Get
    End Property
    Public ReadOnly Property Item(ByVal sName As String) As clsScatteredAccessItem
        '********************************************************************************************
        'Description: Access to individual items by name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                For Each oItem As clsScatteredAccessItem In moItems
                    If sName = oItem.Name Then
                        Return oItem
                    End If
                Next
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: Item", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            Return Nothing
        End Get
    End Property
    Public Sub DisconnectSA()
        '********************************************************************************************
        'Description: Clear out the scattered access item - for comm disconnects
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moSAObject = Nothing
        mbObjectOK = False
    End Sub
    Public Sub SetupSA(ByRef oRobot As FRRobot.FRCRobot)
        '********************************************************************************************
        'Description: Build a scattered access object and connect it
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '********************************************************************************************
        Try
            Dim oSAArray(moItems.Count - 1) As Object
            SetNoRefresh()
            SetNoUpdate()
            Dim nSAItem As Integer = 0
            For nItem As Integer = 1 To moItems.Count
                Dim oItem As clsScatteredAccessItem = DirectCast(moItems(nItem), clsScatteredAccessItem)
                If oItem.ReadOrDisplay <> eSAReadOrDisplay.eSADisplayOnly Then
                    oSAArray(nSAItem) = oItem.FRObject
                    nSAItem = nSAItem + 1
                End If
            Next
            ReDim Preserve oSAArray(nSAItem - 1)
            moRobot = oRobot
            moSAObject = oRobot.CreateScatteredAccess(oSAArray)
            mbObjectOK = True
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: SetupSA", _
           sName & " - Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            mbObjectOK = False
        End Try
    End Sub

    Public Sub SetupSA(ByRef oCntr As clsController)
        '********************************************************************************************
        'Description: Build a scattered access object and connect it
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '********************************************************************************************
        Try
            Dim oSAArray(moItems.Count - 1) As Object
            SetNoRefresh()
            SetNoUpdate()
            For nItem As Integer = 1 To moItems.Count
                Dim oItem As clsScatteredAccessItem = DirectCast(moItems(nItem), clsScatteredAccessItem)
                oSAArray(nItem - 1) = oItem.FRObject
            Next
            moRobot = oCntr.Robot
            moController = oCntr
            moSAObject = oCntr.Robot.CreateScatteredAccess(oSAArray)
            mbObjectOK = True
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: SetupSA", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            mbObjectOK = False
        End Try
    End Sub
    Public Sub SetNoRefresh()
        '********************************************************************************************
        'Description: Tell all the individual items not to read normally
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each oItem As clsScatteredAccessItem In moItems
            Try
                oItem.NoRefresh = True
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: SetNoRefresh", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        Next
    End Sub
    Public Sub SetNoUpdate()
        '********************************************************************************************
        'Description: Tell all the individual items not to update normally
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each oItem As clsScatteredAccessItem In moItems
            Try
                oItem.NoUpdate = True
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: SetNoUpdate", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        Next
    End Sub
    Public Sub Update()
        '********************************************************************************************
        'Description: Do a write with the scattered access object
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        ' 09/04/13  MSW     Refresh, Update - Fix conditions for using robot object
        '********************************************************************************************

        Try
            'If (moController Is Nothing OrElse moController.RCMConnectStatus = ConnStat.frRNConnected) Then
			If (moController IsNot Nothing AndAlso moController.RCMConnectStatus = ConnStat.frRNConnected) Then
                If moRobot.IsConnected Then
                    moSAObject.Update()
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: SUpdate", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Public Sub Refresh()
        '********************************************************************************************
        'Description: Do a read with the scattered access object
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        ' 09/04/13  MSW     Refresh, Update - Fix conditions for using robot object
        '********************************************************************************************
        Try
            'If (moController Is Nothing OrElse moController.RCMConnectStatus = ConnStat.frRNConnected) Then
			If (moController IsNot Nothing AndAlso moController.RCMConnectStatus = ConnStat.frRNConnected) Then
                If moRobot.IsConnected Then
                    moSAObject.Refresh()
                End If
            End If
        Catch ex As Exception
            mbObjectOK = False
        End Try
    End Sub
    Private Sub subAddItems(ByRef nEq As Integer, ByRef oItemConfigs As Collection, ByRef oRC As clsController, _
                            ByVal fVersion As Single, ByVal bAddShared As Boolean, ByVal bByEqName As Boolean, _
                            Optional ByVal oReadOrDisplay As eSAReadOrDisplay = eSAReadOrDisplay.eSABoth)
        '********************************************************************************************
        'Description: Add items from a list based on version
        '
        'Parameters: Eq# range, all for a controller, or the specific arm number, 
        '   item config collection
        '   Robot object
        '   Robot Version
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For Each oCfgItem As clsScatteredAccessItemConfig In oItemConfigs
                Try
                    Dim nCheckVersion As Single = oCfgItem.VersionMin
                    Dim bDispReadOK As Boolean = True
                    Select Case oReadOrDisplay
                        Case eSAReadOrDisplay.eSABoth
                            bDispReadOK = True
                        Case eSAReadOrDisplay.eSADisplayOnly
                            bDispReadOK = ((oCfgItem.ReadOrDisplay = eSAReadOrDisplay.eSADisplayOnly) Or (oCfgItem.ReadOrDisplay = eSAReadOrDisplay.eSABoth))
                        Case eSAReadOrDisplay.eSAReadOnly
                            bDispReadOK = ((oCfgItem.ReadOrDisplay = eSAReadOrDisplay.eSAReadOnly) Or (oCfgItem.ReadOrDisplay = eSAReadOrDisplay.eSABoth))
                    End Select
                    If bDispReadOK And (fVersion > oCfgItem.VersionMin) And _
                       ((fVersion < oCfgItem.VersionMax) Or (oCfgItem.VersionMax < 0.1)) Then
                        Dim bSharedItem As Boolean = False
                        Dim oSAItem As New clsScatteredAccessItem(nEq, oCfgItem, oRC, bSharedItem, bByEqName)
                        If (oSAItem.FRObject IsNot Nothing) AndAlso (bAddShared = True) Or (bSharedItem = False) Then
                            For nItem As Integer = 1 To moItems.Count
                                Dim oItem As clsScatteredAccessItem = DirectCast(moItems(nItem), clsScatteredAccessItem)
                                If oItem.Name = oSAItem.Name Then
                                    moItems.Remove(nItem)
                                    Exit For
                                End If
                            Next
                            moItems.Add(oSAItem)
                        End If
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsScatteredAccessItems:subAddItems - " & oRC.Name & ", Eq" & nEq.ToString & oCfgItem.Name, ex.Message)
                    mbSetupOK = False
                End Try
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subAddItems", _
             "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            mbSetupOK = False
        End Try
    End Sub
    Public Sub New(ByRef oSAConfig As clsScatteredAccessGlobal, ByRef oRC As clsController, _
                   Optional ByVal oReadOrDisplay As eSAReadOrDisplay = eSAReadOrDisplay.eSADisplayOnly)
        '********************************************************************************************
        'Description: Build Scattered access items for a Controller
        '
        'Parameters: oSAConfig - SA global config, oRC - Controller object
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '********************************************************************************************

        Try
            nNumber = oRC.ControllerNumber
            sName = oRC.Name
            moItems = New Collection
            moController = oRC
            Dim fVersion As Single = oRC.Version
            Dim bShared As Boolean = True
            For Each oArm As clsArm In oRC.Arms
                Dim sSAGroups() As String = oArm.ScatteredAccessGroups
                Try
                    For Each oGroup As clsScatteredAccessGrpCfg In oSAConfig.Groups
                        For Each sGroup As String In sSAGroups
                            If sGroup = oGroup.Name Then
                                subAddItems(oArm.ArmNumber, oGroup.Items, oRC, fVersion, bShared, True, oReadOrDisplay)
                            End If
                        Next
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsScatteredAccessItems:New(" & oArm.Name & ")", ex.Message)
                    mbSetupOK = False
                End Try
                bShared = False
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog("clsScatteredAccessItems:New(" & oRC.Name & ")", ex.Message)
            mbSetupOK = False
        End Try
    End Sub

    Public Sub New(ByRef oSAConfig As clsScatteredAccessGlobal, ByRef oArm As clsArm, _
            Optional ByVal oReadOrDisplay As eSAReadOrDisplay = eSAReadOrDisplay.eSADisplayOnly)
        '********************************************************************************************
        'Description: Build Scattered access items for an Arm
        '
        'Parameters: oSAConfig - SA global config, oArm - Arm object
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/09/13  MSW     Robot Connection Status
        '                   Looks like it's better to check the events than the current status,
        '                   look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '********************************************************************************************
        nNumber = oArm.RobotNumber
        sName = oArm.Name
        moItems = New Collection
        moController = oArm.Controller
        Dim nArm As Integer = oArm.ArmNumber
        Dim fVersion As Single = oArm.Controller.Version
        Dim sSAGroups() As String = oArm.ScatteredAccessGroups
        Try
            For Each oGroup As clsScatteredAccessGrpCfg In oSAConfig.Groups
                For Each sGroup As String In sSAGroups
                    If sGroup = oGroup.Name Then
                        subAddItems(nArm, oGroup.Items, oArm.Controller, fVersion, True, False, oReadOrDisplay)
                    End If
                Next
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog("clsScatteredAccessItems:New(" & oArm.Name & ")", ex.Message)
            mbSetupOK = False
        End Try
    End Sub
End Class



'*************************************************************************
'Classes for loading SA config from XML files
'*************************************************************************
Friend Class clsScatteredAccessItemConfig
    '*************************************************************************
    'clsSAItemConfig Class
    'Holds one item config from XML file
    '*************************************************************************

    Private Const msMODULE As String = "clsScatteredAccessItemConfig"
    Private fVersionMin As Single
    Private fVersionMax As Single
    Private sName As String
    Private sType As String
    Private eDataType As eSADataType
    Private eObjectType As eSAObjectType
    Private eIOType As FRRobot.FREIOTypeConstants
    Private sProgram As String
    Private sVariable As String
    Private sIndex As String
    Private fOffset As Single
    Private sReadOrDisplay As String = "Both"
    Private eReadOrDisplay As eSAReadOrDisplay = eSAReadOrDisplay.eSABoth

    Private fScale As Single
    Public Property ReadOrDisplay() As eSAReadOrDisplay
        Get
            Return eReadOrDisplay
        End Get
        Set(ByVal value As eSAReadOrDisplay)
            eReadOrDisplay = value
        End Set
    End Property
    Public Property VersionMin() As Single
        Get
            Return fVersionMin
        End Get
        Set(ByVal value As Single)
            fVersionMin = value
        End Set
    End Property
    Public Property VersionMax() As Single
        Get
            Return fVersionMax
        End Get
        Set(ByVal value As Single)
            fVersionMax = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property
    Public Property IOType() As FRRobot.FREIOTypeConstants
        Get
            Return eIOType
        End Get
        Set(ByVal value As FRRobot.FREIOTypeConstants)
            Try
                eIOType = value
                eObjectType = eSAObjectType.eSAIO
                Select Case eIOType
                    Case FREIOTypeConstants.frAInType
                        sType = "AIN"
                    Case FREIOTypeConstants.frAOutType
                        sType = "AOUT"
                    Case FREIOTypeConstants.frDInType
                        sType = "DIN"
                    Case FREIOTypeConstants.frDOutType
                        sType = "DOUT"
                    Case FREIOTypeConstants.frFlagType
                        sType = "FLAG"
                    Case FREIOTypeConstants.frGPInType
                        sType = "GIN"
                    Case FREIOTypeConstants.frGPOutType
                        sType = "GOUT"
                    Case FREIOTypeConstants.frMarkerType
                        sType = "MARKER"
                    Case FREIOTypeConstants.frPLCInType
                        sType = "PLCIN"
                    Case FREIOTypeConstants.frPLCOutType
                        sType = "PLCOUT"
                    Case FREIOTypeConstants.frRDInType
                        sType = "RIN"
                    Case FREIOTypeConstants.frRDOutType
                        sType = "ROUT"
                    Case FREIOTypeConstants.frSOPInType
                        sType = "SOPIN"
                    Case FREIOTypeConstants.frSOPOutType
                        sType = "SOPOUT"
                    Case FREIOTypeConstants.frTPInType
                        sType = "TPIN"
                    Case FREIOTypeConstants.frTPOutType
                        sType = "TPOUT"
                    Case FREIOTypeConstants.frUOPInType
                        sType = "UOPIN"
                    Case FREIOTypeConstants.frUOPOutType
                        sType = "UOPOUT"
                    Case FREIOTypeConstants.frWDInType
                        sType = "WIN"
                    Case FREIOTypeConstants.frWDOutType
                        sType = "WOUT"
                End Select

            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: IOType", _
                 "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Public Property ObjectType() As eSAObjectType
        Get
            Return eObjectType
        End Get
        Set(ByVal value As eSAObjectType)
            Try
                eObjectType = value
                Select Case eObjectType
                    Case eSAObjectType.eSAIO
                        ' sType set with io type
                    Case eSAObjectType.eSAKarelVar
                        sType = "KAREL"
                    Case eSAObjectType.eSARegNumeric
                        sType = "NUMREG"
                    Case eSAObjectType.eSARegString
                        sType = "STRREG"
                    Case eSAObjectType.eSASystemVar
                        sType = "SYSTEM"
                End Select
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: ObjectType", _
                 "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Public Property Type() As String
        Get
            Return sType
        End Get
        Set(ByVal value As String)
            Try
                sType = value
                Dim sTmp As String = sType.Trim.ToLower
                Select Case sTmp
                    Case "block", "structure"
                        eObjectType = eSAObjectType.eSAStructure
                    Case "variable", "karel", "karelvar", "var", "karelvariable"
                        eObjectType = eSAObjectType.eSAKarelVar
                    Case "system", "sysvar", "systemvariable", "systemvar"
                        eObjectType = eSAObjectType.eSASystemVar
                    Case "reg", "regnum", "regnumeric", "numreg"
                        eObjectType = eSAObjectType.eSARegNumeric
                    Case "regstring", "stringreg", "strreg"
                        eObjectType = eSAObjectType.eSARegString
                        eDataType = eSADataType.eSAString
                    Case "ai", "ain"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frAInType
                        eDataType = eSADataType.eSAInteger
                    Case "ao", "aout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frAOutType
                        eDataType = eSADataType.eSAInteger
                    Case "di", "din"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frDInType
                        eDataType = eSADataType.eSABoolean
                    Case "do", "dout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frDOutType
                        eDataType = eSADataType.eSABoolean
                    Case "flag", "f"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frFlagType
                        eDataType = eSADataType.eSABoolean
                    Case "gi", "gin", "gpi", "gpin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frGPInType
                        eDataType = eSADataType.eSAInteger
                    Case "go", "gout", "gpo", "gpout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frGPOutType
                        eDataType = eSADataType.eSAInteger
                    Case "m", "marker"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frMarkerType
                        eDataType = eSADataType.eSABoolean
                    Case "pi", "pin", "plci", "plcin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frPLCInType
                        eDataType = eSADataType.eSABoolean
                    Case "po", "pout", "plco", "plcout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frPLCOutType
                        eDataType = eSADataType.eSABoolean
                    Case "ri", "rin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frRDInType
                        eDataType = eSADataType.eSABoolean
                    Case "ro", "rout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frRDOutType
                        eDataType = eSADataType.eSABoolean
                    Case "si", "sin", "sopi", "sopin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frSOPInType
                        eDataType = eSADataType.eSABoolean
                    Case "so", "sout", "sopo", "sopout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frSOPOutType
                        eDataType = eSADataType.eSABoolean
                    Case "ti", "tin", "tpi", "tpin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frTPInType
                        eDataType = eSADataType.eSABoolean
                    Case "to", "tout", "tpo", "tpout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frTPOutType
                        eDataType = eSADataType.eSABoolean
                    Case "ui", "uin", "uopi", "uopin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frUOPInType
                        eDataType = eSADataType.eSABoolean
                    Case "uo", "uout", "uopo", "uopout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frUOPOutType
                        eDataType = eSADataType.eSABoolean
                    Case "wi", "win", "wdi", "wdin"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frWDInType
                        eDataType = eSADataType.eSABoolean
                    Case "wo", "wout", "wdo", "wdout"
                        eObjectType = eSAObjectType.eSAIO
                        eIOType = FREIOTypeConstants.frWDOutType
                        eDataType = eSADataType.eSABoolean
                End Select
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: Type", _
                 "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Public Property Program() As String
        Get
            Return sProgram
        End Get
        Set(ByVal value As String)
            sProgram = value
        End Set
    End Property
    Public Property Variable() As String
        Get
            Return sVariable
        End Get
        Set(ByVal value As String)
            sVariable = value
        End Set
    End Property
    Public Property Index() As String
        Get
            Return sIndex
        End Get
        Set(ByVal value As String)
            sIndex = value
        End Set
    End Property
    Public Property Offset() As Single
        Get
            Return fOffset
        End Get
        Set(ByVal value As Single)
            fOffset = value
        End Set
    End Property
    Public Property Scale() As Single
        Get
            Return fScale
        End Get
        Set(ByVal value As Single)
            fScale = value
        End Set
    End Property

    Public Sub New(ByRef oNode As XmlNode)
        '********************************************************************************************
        'Description: Load a scattered access item config from an XML node
        '
        'Parameters: oNode - XML node containing a scattered access item config
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/20/13  DE      Modified to handle Cultures that use "," in floating point numbers rather 
        '                   than ".".
        '********************************************************************************************
        Try
            Try
                fVersionMin = Single.Parse(oNode.Item("VersionMin").InnerXml, mLanguage.FixedCulture) 'CType(oNode.Item("VersionMin").InnerXml, Single) 'DE 09/20/13
            Catch ex As Exception
                fVersionMin = 0
            End Try
            Try
                fVersionMax = Single.Parse(oNode.Item("VersionMax").InnerXml, mLanguage.FixedCulture) 'CType(oNode.Item("VersionMax").InnerXml, Single) 'DE 09/20/13
            Catch ex As Exception
                fVersionMax = 0
            End Try
            sName = oNode.Item("Name").InnerXml
            Type = oNode.Item("Type").InnerXml
            sProgram = oNode.Item("Program").InnerXml
            sVariable = oNode.Item("Variable").InnerXml
            sIndex = oNode.Item("Index").InnerXml
            Try
                fScale = Single.Parse(oNode.Item("Scale").InnerXml, mLanguage.FixedCulture) 'CType(oNode.Item("Scale").InnerXml, Single) 'DE 09/20/13
            Catch ex As Exception
                fScale = 0
            End Try
            Try
                fOffset = Single.Parse(oNode.Item("Offset").InnerXml, mLanguage.FixedCulture) 'CType(oNode.Item("Offset").InnerXml, Single) 'DE 09/20/13
            Catch ex As Exception
                fOffset = 0
            End Try
            Try
                sReadOrDisplay = oNode.Item("ReadOrDisplay").InnerXml
                Select Case sReadOrDisplay.ToLower
                    Case "both"
                        eReadOrDisplay = eSAReadOrDisplay.eSABoth
                    Case "display", "displayonly"
                        eReadOrDisplay = eSAReadOrDisplay.eSADisplayOnly
                    Case "read", "readonly"
                        eReadOrDisplay = eSAReadOrDisplay.eSAReadOnly
                    Case Else
                        eReadOrDisplay = eSAReadOrDisplay.eSABoth
                        sReadOrDisplay = "Both"
                End Select
            Catch ex As Exception
                eReadOrDisplay = eSAReadOrDisplay.eSABoth
                sReadOrDisplay = "Both"
            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog("clsSAItemConfig:New", "Invalid XML Data: " & ex.Message)
        End Try
    End Sub
End Class
Friend Class clsScatteredAccessGrpCfg
    '*************************************************************************
    'clsScatteredAccessGrpCfg Class
    'Holds one group of item configs from XML file
    '*************************************************************************

    Private Const msMODULE As String = "clsScatteredAccessGrpCfg"
    Private sName As String
    Private moItems As Collection
    Public ReadOnly Property Items() As Collection
        Get
            Return moItems
        End Get
    End Property
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property
    Public Sub New(ByRef oNode As XmlNode)
        '********************************************************************************************
        'Description: Load a scattered access item group from an XML node
        '
        'Parameters: oNode - XML node containing a scattered access item group
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oNodeList As XmlNodeList = Nothing
        Try
            sName = oNode.Name
            moItems = New Collection
            oNodeList = oNode.ChildNodes
            If oNodeList.Count = 0 Then
                mDebug.WriteEventToLog("clsScatteredAccessGrpCfg:New", sName & ": No child nodes found.")
            Else
                Dim nMax As Integer = oNodeList.Count - 1
                For nNode As Integer = 0 To nMax
                    Dim oItem As New clsScatteredAccessItemConfig(oNodeList(nNode))
                    moItems.Add(oItem)
                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("clsScatteredAccessGrpCfg:New", "Invalid XML Data: " & ex.Message)
        End Try

    End Sub
End Class
Friend Class clsScatteredAccessGlobal
    '*************************************************************************
    'clsScatteredAccessGlobal Class
    'Loads up the wohle XML config
    '*************************************************************************
    Private Const msMODULE As String = "clsScatteredAccessGlobal"
    Private moGroups As Collection
    Private mnUpdateRatems As Integer
    Public ReadOnly Property UpdateRatems() As Integer
        Get
            Return mnUpdateRatems
        End Get
    End Property
    Public ReadOnly Property Groups() As Collection
        Get
            Return moGroups
        End Get
    End Property
    Private Sub subLoadFromXML()
        '********************************************************************************************
        'Description: Load scattered access config from XML
        '
        'Parameters: Zone - zone for database selection
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sSA_XMLTABLE As String = "ScatteredAccess"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        mnUpdateRatems = 250
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sSA_XMLTABLE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sSA_XMLTABLE)
                oNodeList = oMainNode.ChildNodes

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("ScatteredAccessGlobal:subLoadFromXML", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        For nNode As Integer = 0 To nMax
                            If oNodeList(nNode).Name = "UpdateRateMS" Then
                                Try
                                    mnUpdateRatems = CType(oNodeList(nNode).InnerText, Integer)
                                Catch ex As Exception
                                End Try
                            Else
                                Dim oGroup As New clsScatteredAccessGrpCfg(oNodeList(nNode))
                                moGroups.Add(oGroup)
                            End If
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("ScatteredAccessGlobal:subLoadFromXML", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("ScatteredAccessGlobal:subLoadFromXML", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try

    End Sub

    Friend Sub New()
        '********************************************************************************************
        'Description: Load scattered access config from XML
        '
        'Parameters: Zone - zone for database selection
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moGroups = New Collection
        subLoadFromXML()
    End Sub
End Class
