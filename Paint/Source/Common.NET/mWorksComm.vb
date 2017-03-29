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
' Form/Module: mWorksComm
'
' Description: Routines for communication between Paintworks applications using Windows
'              Messages
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Sir Not Appearing In This Film
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    11/19/09   MSW     Try to get in touch with a program running in visual studio
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices

Friend Module mWorksComm

#Region " Declares "

    '******** Module Constants   *********************************************************************
    Friend Const WM_NULL As Int32 = &H0
    Friend Const WM_GETTEXT As Int32 = &HD
    '******** End Module Constants   *****************************************************************

    '******** Module Structures   ********************************************************************
    Friend Enum ScreenMessage
        'ProcID = 0
        Action = 0 '1
        Params = 1 '2
        X = 2 '3
        Y = 3 '4
        Width = 4 '5
        Height = 5 '6
    End Enum

    Friend Structure udsFRWM
        Friend ScreenAction As String
        Friend ScreenParams As String
        Friend ScreenX As String
        Friend ScreenY As String
        Friend ScreenWidth As String
        Friend ScreenHeight As String
    End Structure
    '******** End Module Structures   ****************************************************************

    '******** Windows API Functions   ****************************************************************
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Function SendMessage( _
         ByVal hWnd As Int32, _
         ByVal Msg As Int32, _
         ByVal wParam As Int32, _
         ByVal lParam As Int32) As Int32
    End Function


#End Region

#Region " Routines "
    Friend Sub SendFRWMMessage(ByVal Message As String, ByVal AppName As String)
        '********************************************************************************************
        'Description:  Send a message to the named Application
        '
        'Parameters: Message, AppName
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim procWorksApp As Process = GetProcess(AppName)

            If Not IsNothing(procWorksApp) Then

                'Convert message string to byte array
                Dim bytMsg() As Byte = System.Text.Encoding.Default.GetBytes(Message)

                'This works for .NET apps but not for VB6 apps
                Dim hWnd As Int32 = procWorksApp.MainWindowHandle.ToInt32

                'First, send the header message. LParam tells the target process how many 
                'characters to expect.
                SendMessage(hWnd, WM_NULL, 0, Message.Length)

                'Now, send the message body. WParam contains 1 character so we'll send as many
                'messages as it takes to deliver the whole string to the target process.
                For nItem As Integer = 0 To (Message.Length - 1)
                    SendMessage(hWnd, WM_NULL, bytMsg(nItem), 0)
                Next 'nItem
            End If 'Not IsNothing(procWorksApp)

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: mWorksComm, Routine: SendFRWMMessage", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Throw ex
        End Try

    End Sub

    Friend Sub SendFRWMMessage(ByVal Message As String, ByVal ProcID As Integer, Optional ByVal sName As String = "")
        '********************************************************************************************
        'Description:  Send a message to the named Application
        '
        'Parameters: Message, ProcessID
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim procWorksApp As Process = Process.GetProcessById(ProcID)

            If Not IsNothing(procWorksApp) Then
                'Convert message string to byte array
                Dim bytMsg() As Byte = System.Text.Encoding.Default.GetBytes(Message)

                'This works for .NET apps but not for VB6 apps
                Dim hWnd As Int32 = procWorksApp.MainWindowHandle.ToInt32

                'First, send the header message. LParam tells the target process how many 
                'characters to expect.
                SendMessage(hWnd, WM_NULL, 0, Message.Length)

                'Now, send the message body. WParam contains 1 character so we'll send as many
                'messages as it takes to deliver the whole string to the target process.
                For nItem As Integer = 0 To (Message.Length - 1)
                    SendMessage(hWnd, WM_NULL, bytMsg(nItem), 0)
                Next 'nItem

            End If 'Not IsNothing(procWorksApp)

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: mWorksComm, Routine: SendFRWMMessage", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Throw ex
        End Try

    End Sub

#End Region

#Region " Functions"

    Public Function GetProcess(ByVal AppName As String) As Process
        '********************************************************************************************
        'Description:  Return the Process associated with the supplied Application Name
        '
        'Parameters: AppName
        'Returns:    AppName's Process
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/19/09  MSW     Try to get in touch with a program running in visual studio
        '********************************************************************************************
        Try
            Dim procByName As Process() = Process.GetProcessesByName(AppName)

            If procByName.Length > 0 Then
                'Running
                Return procByName(0)
            Else
                'Try to get in touch with a program running in visual studio
                procByName = Process.GetProcessesByName(AppName & ".vshost")
                If procByName.Length > 0 Then
                    'Running
                    Return procByName(0)
                Else
                    Return Nothing
                End If
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: mWorksComm, Routine: GetProcess", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Throw ex
            Return Nothing
        End Try

    End Function

    Friend Function GetScreenCommand(ByVal Message As String) As udsFRWM
        '********************************************************************************************
        'Description:  Return a Screen Command structure for the supplied FRWM Message
        '
        'Parameters: Message
        'Returns:    Screen Command
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Trace.WriteLine(Threading.Thread.CurrentThread.Name & " GetScreenCommand : " & Message)

        Try
            Dim sFields() As String = Split(Message, ",")
            Dim ScreenCommand As udsFRWM = Nothing

            For nField As Integer = 0 To sFields.GetUpperBound(0)
                Select Case nField
                    Case ScreenMessage.Action
                        ScreenCommand.ScreenAction = sFields(nField)
                    Case ScreenMessage.Params
                        ScreenCommand.ScreenParams = sFields(nField)
                    Case ScreenMessage.X
                        ScreenCommand.ScreenX = sFields(nField)
                    Case ScreenMessage.Y
                        ScreenCommand.ScreenY = sFields(nField)
                    Case ScreenMessage.Width
                        ScreenCommand.ScreenWidth = sFields(nField)
                    Case ScreenMessage.Height
                        ScreenCommand.ScreenHeight = sFields(nField)
                    Case Else
                        'There's some extra stuff tacked on to this message.
                        'Who knows what it is.
                End Select
            Next 'nField

            'Trace.WriteLine(" GetScreenCommand : " & ScreenCommand.ToString)
            Return ScreenCommand

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: mWorksComm, Routine: GetScreenCommand", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Throw ex
            Return Nothing
        End Try

    End Function

#End Region

End Module