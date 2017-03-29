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
' Form/Module: mDebug
'
' Description: Debugging / logging items
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    10/31/12   MSW     Copy OutputDebugString declaration from Honda source          4.01.01.01
'    01/06/14   MSW     Make sure it saves to the correct log                         4.01.06.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Diagnostics
Imports System.Threading

Friend Module mDebug

#Region " Declares "

    '********************************************************************************************
    'Description:  Windows API to write debug messages that can be captured by SysInternals 
    '              DebugView.exe
    'Parameters: Debug string to write
    'Returns:    None 
    '********************************************************************************************
    Public Declare Sub OutputDebugString Lib "kernel32" Alias "OutputDebugStringA" (ByVal lpOutputString As String)
 
#End Region
#Region " Routines "

    Friend Function ShowErrorMessagebox(ByVal sText As String, ByVal ex As Exception, _
                                        ByVal sScreen_Name As String, ByRef sStatus As String, _
                                        Optional ByVal Buttons As MessageBoxButtons = _
                                        MessageBoxButtons.OK, Optional ByVal Icons As MessageBoxIcon _
                                        = MessageBoxIcon.Warning) As DialogResult
        '********************************************************************************************
        'Description:  Common routine to show error messagebox
        '
        'Parameters: Text not in exception
        'Returns:    which button pressed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        WriteEventToLog(sScreen_Name, ex.StackTrace & vbCrLf & ex.Message)

        sStatus = gcsRM.GetString("csERROR")

        Dim sTmp As String = sText & vbCrLf & ex.Message

        Return MessageBox.Show(sTmp, sScreen_Name, Buttons, _
                        Icons, MessageBoxDefaultButton.Button1, _
                        MessageBoxOptions.DefaultDesktopOnly)


    End Function
    Friend Sub WriteEventToLog(ByVal sSource As String, ByVal StringtoWrite As String)
        '********************************************************************************************
        'Description:  Common routine to write to event log
        '
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/06/14  MSW     Make sure it saves to the correct log                                    
        '********************************************************************************************
        Dim myLog As New EventLog()
        Dim EventPermission As New EventLogPermission(Security.Permissions.PermissionState.Unrestricted)
        'EventPermission.ad()
        Dim bNew As Boolean = False
        Dim PW4 As String = gcsRM.GetString("csPAINTWORKS")
        Try
            If EventLog.SourceExists(sSource) Then
                If EventLog.LogNameFromSourceName(sSource, ".") <> PW4 Then
                    EventLog.DeleteEventSource(sSource)
                    EventLog.CreateEventSource(sSource, PW4)
                    bNew = True
                End If
            Else
                EventLog.CreateEventSource(sSource, PW4)
                bNew = True
            End If
        Catch ex As Exception
            Exit Sub
        End Try

        ' Create an EventLog instance and assign its source.
        'Dim myLog As New EventLog()
        myLog.Source = sSource
        If bNew Then
            myLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 1000)
        End If
        ' Write an informational entry to the event log.    
        myLog.WriteEntry(StringtoWrite)
        Debug.Print("")
        Debug.Print(sSource & " - " & StringtoWrite)
        Debug.Print("")
    End Sub
#End Region


End Module
