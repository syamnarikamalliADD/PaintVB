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
' Form/Module: AlarmAssocData.vb
'
' Description: Gets associated data for robot alarms
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports FRRobot
Imports System.Xml
Imports System.Xml.XPath

Friend Class clsAlarmAssocData

#Region " Declares "
    '******** Robot Scattered Access Variables    ***************************************************
    Private mobjRobot As FRCRobot = Nothing
    Private mobjSA As FRCScatteredAccess = Nothing
    Private mobjStyle As FRCVar = Nothing
    Private mobjColor As FRCVar = Nothing
    Private mobjValve As FRCVar = Nothing
    Private mobjJobName As FRCVar = Nothing
    Private mobjProcess As FRCVar = Nothing
    Private mobjNode As FRCVar = Nothing
    '******** End Robot Scattered Access Variables    ***********************************************

    '******** Module Variables    *******************************************************************
    Private mbStyleEnabled As Boolean
    Private mbColorEnabled As Boolean
    Private mbValveEnabled As Boolean
    Private mbJobEnabled As Boolean
    Private mbProcessEnabled As Boolean
    Private mbNodeEnabled As Boolean
    Private msStyleProgName As String
    Private msColorProgName As String
    Private msValveProgName As String
    Private msJobProgName As String
    Private msProcessProgName As String
    Private msNodeProgName As String
    Private msStyleVarName As String
    Private msColorVarName As String
    Private msValveVarName As String
    Private msJobVarName As String
    Private msProcessVarName As String
    Private msNodeVarName As String
    '******** End Module Variables    *************************************************************

    '******** Property Variables    *****************************************************************
    Private mbInitSuccess As Boolean
    Private mvarStyle As String = String.Empty
    Private mvarColor As String = String.Empty
    Private mvarValve As String = String.Empty
    Private mvarJobName As String = String.Empty
    Private mvarProcess As String = String.Empty
    Private mvarNode As Integer
    '******** End Property Variables    *************************************************************

#End Region

#Region " Properties "

    Friend Property InitSuccess() As Boolean
        '*****************************************************************************************
        'Description: Feedback to the parent
        '
        'Parameters: 
        'Returns:       True if we hooked up successfully, False if not
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return mbInitSuccess
        End Get
        Set(ByVal value As Boolean)
            mbInitSuccess = value
        End Set

    End Property

    Friend Property Style() As String
        '*****************************************************************************************
        'Description: Current Style from Robot Controller
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarStyle)
        End Get
        Set(ByVal value As String)
            If IsNothing(value) Then
                mvarStyle = String.Empty
            Else
                mvarStyle = value
            End If
        End Set

    End Property

    Friend Property Color() As String
        '*****************************************************************************************
        'Description: Current Color from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarColor)
        End Get
        Set(ByVal value As String)

            Try
                If IsNothing(value) Then
                    value = String.Empty
                Else
                    Dim nPos As Integer = InStr(value, "-")

                    If nPos > 0 Then
                        mvarColor = Strings.Trim(Strings.Left(value, nPos - 1))
                    Else
                        mvarColor = String.Empty
                    End If
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: Color [Set]", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End Set

    End Property

    Friend Property Valve() As String
        '*****************************************************************************************
        'Description: Current Color Valve from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarValve)
        End Get
        Set(ByVal value As String)
            If IsNothing(value) Then
                value = String.Empty
            Else
                mvarValve = value
            End If
        End Set

    End Property

    Friend Property JobName() As String
        '*****************************************************************************************
        'Description: Current Job Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarJobName)
        End Get
        Set(ByVal value As String)
            If IsNothing(value) Then
                value = String.Empty
            Else
                mvarJobName = value
            End If
        End Set

    End Property

    Friend Property Process() As String
        '*****************************************************************************************
        'Description: Current Process Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarProcess)
        End Get
        Set(ByVal value As String)
            If IsNothing(value) Then
                value = String.Empty
            Else
                mvarProcess = value
            End If
        End Set

    End Property

    Friend Property Node() As Integer
        '*****************************************************************************************
        'Description: Current Process Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return (mvarNode)
        End Get
        Set(ByVal value As Integer)
            If IsNothing(value) Then
                value = 0
            Else
                mvarNode = value
            End If
        End Set

    End Property

#End Region

#Region " Routines "

    Private Sub subInitialize(ByRef oRobot As FRCRobot)
        '*****************************************************************************************
        'Description: Build scattered access to alarm associated data
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            Dim oList() As Object = Nothing
            Dim nItems As Integer = 0

            mobjRobot = oRobot

            If mbStyleEnabled Then
                mobjStyle = TryCast(mobjRobot.Programs(msStyleProgName).Variables(msStyleVarName), FRCVar)
                mobjStyle.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjStyle
                nItems += 1
            End If

            If mbColorEnabled Then
                mobjColor = TryCast(mobjRobot.Programs(msColorProgName).Variables(msColorVarName), FRCVar)
                mobjColor.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjColor
                nItems += 1
            End If

            If mbValveEnabled Then
                mobjValve = TryCast(mobjRobot.Programs(msValveProgName).Variables(msValveVarName), FRCVar)
                mobjValve.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjValve
                nItems += 1
            End If

            If mbJobEnabled Then
                mobjJobName = TryCast(mobjRobot.Programs(msJobProgName).Variables(msJobVarName), FRCVar)
                mobjJobName.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjJobName
                nItems += 1
            End If

            If mbProcessEnabled Then
                mobjProcess = TryCast(mobjRobot.Programs(msProcessProgName).Variables(msProcessVarName), FRCVar)
                mobjProcess.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjProcess
                nItems += 1
            End If

            If mbNodeEnabled Then
                mobjNode = TryCast(mobjRobot.Programs(msNodeProgName).Variables(msNodeVarName), FRCVar)
                mobjNode.NoUpdate = True
                ReDim Preserve oList(nItems)
                oList(nItems) = mobjNode
                nItems += 1
            End If

            If nItems > 0 Then
                mobjSA = mobjRobot.CreateScatteredAccess(oList)
            Else
                mbInitSuccess = False
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: subInitialize", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            mbInitSuccess = False
        End Try

    End Sub

    Private Function ReadVarNames(ByVal XMLPath As String, ByVal ArmNumber As Integer, ByVal IsOpener As Boolean) As Boolean
        '*****************************************************************************************
        'Description: Read Assoc Data Program and Variable names from the xml config file.
        '
        'Parameters: XMLPath
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim bSuccess As Boolean
        Dim sPath As String = "//AssocData[Equipment='" & ArmNumber.ToString & "']"
        Dim oNodeList As XmlNodeList
        Dim oXMLDoc As New XmlDocument

        Try
            oXMLDoc.Load(XMLPath)
            oNodeList = oXMLDoc.SelectNodes(sPath)

        Catch ex As Exception
            mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: ReadVarNames", _
                                   "Error: Invalid XPath syntax [" & sPath & "] - " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try

        Try
            If oNodeList.Count = 0 Then
                mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: ReadVarNames", _
                                       "Error: Equipment [" & ArmNumber.ToString & "] not found.")
            Else
                Dim oNode As XmlNode = oNodeList(0)

                With oNode

                    If Not IsOpener Then
                        msStyleProgName = .Item("StyleProg").InnerXml
                        msStyleVarName = .Item("StyleVar").InnerXml
                        If (msStyleProgName <> String.Empty) And (msStyleVarName <> String.Empty) Then
                            mbStyleEnabled = True
                        End If

                        msColorProgName = .Item("ColorProg").InnerXml
                        msColorVarName = .Item("ColorVar").InnerXml
                        If (msColorProgName <> String.Empty) And (msColorVarName <> String.Empty) Then
                            mbColorEnabled = True
                        End If

                        msValveProgName = .Item("ValveProg").InnerXml
                        msValveVarName = .Item("ValveVar").InnerXml
                        If (msValveProgName <> String.Empty) And (msValveVarName <> String.Empty) Then
                            mbValveEnabled = True
                        End If
                    End If

                    msJobProgName = .Item("JobProg").InnerXml
                    msJobVarName = .Item("JobVar").InnerXml
                    If (msJobProgName <> String.Empty) And (msJobVarName <> String.Empty) Then
                        mbJobEnabled = True
                    End If

                    msProcessProgName = .Item("ProcessProg").InnerXml
                    msProcessVarName = .Item("ProcessVar").InnerXml
                    If (msProcessProgName <> String.Empty) And (msProcessVarName <> String.Empty) Then
                        mbProcessEnabled = True
                    End If

                    msNodeProgName = .Item("NodeProg").InnerXml
                    msNodeVarName = .Item("NodeVar").InnerXml
                    If (msNodeProgName <> String.Empty) And (msNodeVarName <> String.Empty) Then
                        mbNodeEnabled = True
                    End If

                End With 'oNode
                bSuccess = True
            End If 'oNodeList.Count = 0

        Catch ex As Exception
            mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: ReadVarNames", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return bSuccess

    End Function

    Friend Sub Refresh()
        '*****************************************************************************************
        'Description: Refresh property values with new data from the robot controller.
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            If mobjRobot.IsConnected Then
                mobjSA.Refresh()

                If mbStyleEnabled Then Style = TryCast(mobjStyle.Value, String)
                If mbColorEnabled Then Color = TryCast(mobjColor.Value, String) 'String64
                If mbValveEnabled Then Valve = TryCast(mobjValve.Value.ToString, String) 'Integer32
                If mbJobEnabled Then JobName = TryCast(mobjJobName.Value, String) 'String64
                If mbProcessEnabled Then Process = TryCast(mobjProcess.Value, String) 'String64
                If mbNodeEnabled Then Node = CType(mobjNode.Value, Integer)
            Else
                If mbStyleEnabled Then Style = "err"
                If mbStyleEnabled Then Color = "err"
                If mbStyleEnabled Then Valve = "err"
                If mbStyleEnabled Then JobName = "err"
                If mbProcessEnabled Then Process = "err"
                If mbNodeEnabled Then Node = 0
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("AlarmAssocData.vb Module: clsAlarmAssocData Routine: Refresh", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Friend Sub New(ByRef oRobot As FRCRobot, ByVal XMLPath As String, ByVal ArmNumber As Integer, _
                   ByVal IsOpener As Boolean)
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
        mbInitSuccess = ReadVarNames(XMLPath, ArmNumber, IsOpener)
        Call subInitialize(oRobot)

    End Sub


#End Region

End Class
