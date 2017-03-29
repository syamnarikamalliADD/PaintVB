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
' Form/Module: mVision  
'
' Description:  Vision report support routines.  
'                
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'******************************************************************************************************
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath

Module mVision

    Friend colVisionStatusItems As Collection(Of clsCboItem) = Nothing

    Private Const msMODULE As String = "mVision"
    Friend Const msVISIONTATUSLISTBOX As String = "lsbVisionStatus"
    Friend Function sGetVisionStat(ByVal sVisionStatus As String) As String
        For Each oItem As clsCboItem In colVisionStatusItems
            If oItem.DB = sVisionStatus.Trim Then
                Return oItem.Display
            End If
        Next
        Return String.Format(gpsRM.GetString("psVISION_STAT_UNKNOWN"), sVisionStatus)
    End Function

    Friend Function oGetVisionStatusBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Job Status listbox. this also copies the current language
        '               Strings for job status to the production log database
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String()

        Try
            bLoadVisionStatusItems()
            Dim o As CheckedListBox = frmCriteria.oGetCheckListBox(msVISIONTATUSLISTBOX)
            o.Size = New Size(170, frmCriteria.mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            ReDim sTag(0)
            sTag(0) = gcsRM.GetString("csALL")
            Dim nTagIndex As Integer = 0
            For Each oItem As clsCboItem In colVisionStatusItems
                nTagIndex = nTagIndex + 1
                ReDim Preserve sTag(nTagIndex)
                sTag(nTagIndex) = oItem.DB
                o.Items.Add(oItem.Display)
            Next

            o.Tag = sTag

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function

    Friend Function bLoadVisionStatusItems() As Boolean
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sXMLFILE As String = "VisionStatus"
        Dim sXMLNODE As String = "VisionStatusItem"
        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If colVisionStatusItems Is Nothing Then
            colVisionStatusItems = New Collection(Of clsCboItem)
        Else
            colVisionStatusItems.Clear()
        End If
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLFILE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: bLoadVisionStatusItems", _
                                               sXMLFilePath & " not found.")
                    Else
                        For Each oNode As XmlNode In oNodeList
                            Dim oItem As New clsCboItem
                            Try
                                oItem.DB = oNode.Item("Number").InnerXml
                                oItem.Display = gpsRM.GetString(oNode.Item("String").InnerXml)
                            Catch ex As Exception
                                mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: bLoadVisionStatusItems", _
                                                       "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                            End Try
                            colVisionStatusItems.Add(oItem)
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: bLoadVisionStatusItems", _
                                           "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: bLoadVisionStatusItems", _
                                   "Invalid Path syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try

    End Function

    Friend Function sGetVisionStatus(ByRef nStatus As Integer) As String
        '********************************************************************************************
        'Description: Get text for status integer from the robot
        '
        'Parameters: status number
        'Returns:    status text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim sStatus As String = gpsRM.GetString("psVISION_STAT_" & nStatus.ToString)
            If sStatus = String.Empty Then
                mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: sGetVisionStatus", _
                        "Unknown Vision Status: " & nStatus.ToString)

                sStatus = String.Format(gpsRM.GetString("psVISION_STAT_UNKNOWN"), nStatus.ToString)
            End If
            sStatus = nStatus.ToString & " - " & sStatus
            Return sStatus
        Catch ex As Exception
            mDebug.WriteEventToLog("Reports Module: " & msMODULE & " Routine: sGetVisionStatus" & _
                                    "Vision Status Error: " & nStatus.ToString & " - ", _
                                    ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        Return (nStatus.ToString)
    End Function

End Module
