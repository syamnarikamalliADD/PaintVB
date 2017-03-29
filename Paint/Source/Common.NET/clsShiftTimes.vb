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
' Form/Module: clsShiftTimes.vb
'
' Description: Collection for Shift schedule times
' 
'
' Dependancies:  
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
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    03/28/12   MSW     Move DB to XML                                                4.01.03.00
'    10/24/14   MSW     Read Days from XML - for semi auto reports support            4.01.07.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath

Friend Class clsShifts
    Inherits CollectionBase

#Region " Declares"

    Private mnYear As Integer = DatePart(DateInterval.Year, Now)
    Private mnMonth As Integer = DatePart(DateInterval.Month, Now)
    Private mnDay As Integer = DatePart(DateInterval.Day, Now)

#End Region

#Region " Properties"

    Default Friend ReadOnly Property Item(ByVal ShiftNum As Integer) As clsShift
        '********************************************************************************************
        'Description: 
        '
        'Parameters: clszone
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If ShiftNum < 1 Then Return Nothing
            If ShiftNum > 3 Then Return Nothing
            Return CType(List.Item(ShiftNum - 1), clsShift)
        End Get
    End Property
#End Region

#Region " Routines"

    Private Sub subLoadCollection(ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: clszone
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move DB to XML
        '********************************************************************************************
        Const sXMLTABLE As String = "Schedules"
        Const sXMLNODE As String = "Schedule"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim nNode As Integer = 0
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

                'should be 3 shifts in order from the database
                Dim oShifts(2) As clsShift
                oShifts(0) = New clsShift
                oShifts(1) = New clsShift
                oShifts(2) = New clsShift
                oShifts(0).Number = 1
                oShifts(1).Number = 2
                oShifts(2).Number = 3

                For Each oNode As XmlNode In oNodeList

                    Dim nId As Integer = CType(oNode.Item("ID").InnerXml, Integer)
                    Dim nShift As Integer = 0
                    If nId > 12 Then
                        nShift = 2
                        nId = nId - 12
                    ElseIf nId > 6 Then
                        nShift = 1
                        nId = nId - 6
                    End If
                    Select Case nId
                        Case 1
                            oShifts(nShift).StartTime = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).EndTime = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)
                            If oNode.InnerXml.Contains("Days") Then
                                oShifts(nShift).Days = CType(oNode.Item("Days").InnerXml, Integer)
                            Else
                                oShifts(nShift).Days = 127
                            End If
                        Case 2
                            oShifts(nShift).Break1Start = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).Break1End = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Break1Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)
                        Case 3
                            oShifts(nShift).Break2Start = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).Break2End = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Break2Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)
                        Case 4
                            oShifts(nShift).Break3Start = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).Break3End = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Break3Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)
                        Case 5
                            oShifts(nShift).Break4Start = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).Break4End = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Break4Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)
                        Case 6
                            oShifts(nShift).Break5Start = GetEventDate(CType(oNode.Item("StartHour").InnerXml, Integer), _
                               CType(oNode.Item("StartMinute").InnerXml, Integer), _
                               CType(oNode.Item("StartSecond").InnerXml, Integer))
                            oShifts(nShift).Break5End = GetEventDate(CType(oNode.Item("EndHour").InnerXml, Integer), _
                                                     CType(oNode.Item("EndMinute").InnerXml, Integer), _
                                                     CType(oNode.Item("EndSecond").InnerXml, Integer))
                            oShifts(nShift).Break5Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)

                    End Select
                Next
                List.Add(oShifts(0))
                List.Add(oShifts(1))
                List.Add(oShifts(2))

            Catch ex As Exception

                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "clsShiftTimes" & " Routine: subLoadCollection", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

            End Try
        End If

    End Sub
    Private Function GetEventDate(ByVal Hour As Integer, ByVal Minute As Integer, ByVal Second As Integer) As Date
        '********************************************************************************************
        'Description: Combine event time with Year, Month and Day from Now to return a Date type.
        '
        '
        'Parameters: Hour, Minute, and Second
        'Returns: Date    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dtEvent As New DateTime(mnYear, mnMonth, mnDay, Hour, Minute, Second)

        Return dtEvent

    End Function
    Friend Shared Function GetTimePart(ByVal vDate As Date) As Date
        '********************************************************************************************
        'Description: Date always has date part with it. take time part of incoming and use it with
        '               today  for time comparisons
        '
        '
        'Parameters: clszone
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim d As Date = CDate("0:00:00")
        d = d.AddHours(vDate.Hour)
        d = d.AddMinutes(vDate.Minute)
        d = d.AddSeconds(vDate.Second)
        d = d.AddMilliseconds(vDate.Millisecond)
        Return d
    End Function
#End Region

#Region " Events"

    Friend Sub New(ByRef oZone As clsZone)
        '********************************************************************************************
        'Description: Make a new shift time collection
        '
        'Parameters: clszone
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        subLoadCollection(oZone)
    End Sub

#End Region

End Class

Friend Class clsShift

#Region " Declares"

    Private mStart As Date
    Private mEnd As Date
    Private mB1Start As Date
    Private mB2Start As Date
    Private mB3Start As Date
    Private mB4Start As Date
    Private mB5Start As Date
    Private mB1End As Date
    Private mB2End As Date
    Private mB3End As Date
    Private mB4End As Date
    Private mB5End As Date
    Private mbB1Enb As Boolean
    Private mbB2Enb As Boolean
    Private mbB3Enb As Boolean
    Private mbB4Enb As Boolean
    Private mbB5Enb As Boolean
    Private mbEnb As Boolean
    Private mnNum As Integer
    Private mbDays(6) As Boolean

#End Region

#Region " Properties"

    Friend Property StartTime() As Date
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
            Return mStart
        End Get
        Set(ByVal value As Date)
            mStart = value
        End Set
    End Property
    Friend Property EndTime() As Date
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
            Return mEnd
        End Get
        Set(ByVal value As Date)
            mEnd = value
        End Set
    End Property
    Friend Property Break1Start() As Date
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
            Return mB1Start
        End Get
        Set(ByVal value As Date)
            mB1Start = value
        End Set
    End Property
    Friend Property Break2Start() As Date
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
            Return mB2Start
        End Get
        Set(ByVal value As Date)
            mB2Start = value
        End Set
    End Property
    Friend Property Break3Start() As Date
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
            Return mB3Start
        End Get
        Set(ByVal value As Date)
            mB3Start = value
        End Set
    End Property
    Friend Property Break4Start() As Date
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
            Return mB4Start
        End Get
        Set(ByVal value As Date)
            mB4Start = value
        End Set
    End Property
    Friend Property Break5Start() As Date
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
            Return mB5Start
        End Get
        Set(ByVal value As Date)
            mB5Start = value
        End Set
    End Property
    Friend Property Break1End() As Date
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
            Return mB1End
        End Get
        Set(ByVal value As Date)
            mB1End = value
        End Set
    End Property
    Friend Property Break2End() As Date
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
            Return mB2End
        End Get
        Set(ByVal value As Date)
            mB2End = value
        End Set
    End Property
    Friend Property Break3End() As Date
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
            Return mB3End
        End Get
        Set(ByVal value As Date)
            mB3End = value
        End Set
    End Property
    Friend Property Break4End() As Date
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
            Return mB4End
        End Get
        Set(ByVal value As Date)
            mB4End = value
        End Set
    End Property
    Friend Property Break5End() As Date
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
            Return mB5End
        End Get
        Set(ByVal value As Date)
            mB5End = value
        End Set
    End Property
    Friend Property Enabled() As Boolean
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
            Return mbEnb
        End Get
        Set(ByVal value As Boolean)
            mbEnb = value
        End Set
    End Property
    Friend Property Break1Enabled() As Boolean
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
            Return mbB1Enb
        End Get
        Set(ByVal value As Boolean)
            mbB1Enb = value
        End Set
    End Property
    Friend Property Break2Enabled() As Boolean
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
            Return mbB2Enb
        End Get
        Set(ByVal value As Boolean)
            mbB2Enb = value
        End Set
    End Property
    Friend Property Break3Enabled() As Boolean
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
            Return mbB3Enb
        End Get
        Set(ByVal value As Boolean)
            mbB3Enb = value
        End Set
    End Property
    Friend Property Break4Enabled() As Boolean
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
            Return mbB4Enb
        End Get
        Set(ByVal value As Boolean)
            mbB4Enb = value
        End Set
    End Property
    Friend Property Break5Enabled() As Boolean
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
            Return mbB5Enb
        End Get
        Set(ByVal value As Boolean)
            mbB5Enb = value
        End Set
    End Property
    Friend ReadOnly Property IsActive() As Boolean
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
            'dont know what date may be in database
            Dim dStart As Date = clsShifts.GetTimePart(mStart)
            Dim dEnd As Date = clsShifts.GetTimePart(mEnd)
            Dim dNow As Date = clsShifts.GetTimePart(Now)

            If dEnd < dStart Then
                'midnite wrap
                If (dNow > dStart) Then
                    Return True
                End If
                If (dNow < dEnd) Then
                    Return True
                End If
                Return False
            Else
                If dNow < dStart Then Return False
                If dNow > dEnd Then Return False
                Return True
            End If
        End Get
    End Property
    Protected Friend Property Number() As Integer
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
            Return mnNum
        End Get
        Set(ByVal value As Integer)
            mnNum = value
        End Set
    End Property
    Protected Friend Property Days(ByVal nDay As Integer) As Boolean
        Get
            Return mbDays(nDay)
        End Get
        Set(ByVal value As Boolean)
            mbDays(nDay) = value
        End Set
    End Property
    Protected Friend Property Days() As Integer
        Get
            Dim nDays As Integer = 0
            For nDay As Integer = DayOfWeek.Sunday To DayOfWeek.Saturday
                If mbDays(nDay) Then
                    nDays = nDays Or nBitMask(nDay)
                End If
            Next
            Return nDays
        End Get
        Set(ByVal value As Integer)
            For nDay As Integer = DayOfWeek.Sunday To DayOfWeek.Saturday

                mbDays(nDay) = ((value And nBitMask(nDay)) <> 0)

            Next
        End Set
    End Property
#End Region

#Region " Routines"

#End Region

#Region " Events"

    Friend Sub New()
        '********************************************************************************************
        'Description: Make a new shift 
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

End Class