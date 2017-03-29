'This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2012
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: StartupModule (StartupModule.vb)
'
' Description:
' Add this module to projects that (mysteriously) do not have the
' Window application framework properties groupbox present on the
' Application tab of the project properties page to prevent more
' than a single instance of the application from running. Then, 
' change the "Startup object:" setting in the dropbox to "StartupModule".
'
' Dependencies:  
'
' Language: Microsoft Visual Basic 2008 .NET
'
' Author: Matt White/ Rick Olejniczak
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'  By           Date                                                                Version
'  MSW/RJO      10/19/11  First draft                                               4.01.01.00
'  MSW          04/18/2013 Try harder to give focus to the already running program  4.01.05.00
Module StartupModule



    Public Sub Main()
        Try
            Dim oProc As Process = Process.GetCurrentProcess
            Dim sName As String = oProc.ProcessName
            Dim nDbg As Integer = InStr(sName, ".vshost", CompareMethod.Text)
            Dim nCnt As Integer = 1
            If nDbg > 0 Then
                sName = sName.Substring(0, nDbg - 1)
                nCnt = 0
            End If
            Dim oProcs() As Process = Process.GetProcessesByName(sName)

            If oProcs.Length <= nCnt Then
                Application.Run(frmMain)
            Else
                Dim nProc As Integer = 0
                Dim nThisProc As System.IntPtr = oProc.Handle
                For nIndex As Integer = oProcs.GetLowerBound(0) To oProcs.GetUpperBound(0)
                    If nThisProc <> oProcs(nIndex).Handle Then
                        nProc = nIndex
                    End If
                Next
                'This is probably overkill, just hitting it with everything we can to get the desired program in front
                'ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'Restore
                '... SW_SHOW = 5, SW_MINIMIZE = 6, SW_SHOWMINNOACTIVE = 7, SW_SHOWNA = 8, SW_RESTORE = 9, SW_SHOWDEFAULT = 10, 
                ' SW_FORCEMINIMIZE = 11, SW_MAX = 11 } [DllImport("shell32.dll")] static extern IntPtr 
                'ShellExecute( IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirecto...
                'ShowWindowAsync(hWnd, SW_SHOW);
                'This is probably overkill, just hitting it with everything we can to get the desired program in front
                ShowWindowAsync(oProcs(nProc).MainWindowHandle, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                'ShowWindowAsync(hWnd, SW_RESTORE);
                ShowWindowAsync(oProcs(nProc).MainWindowHandle, 9) 'SW_RESTORE
                Threading.Thread.Sleep(10)
                SetForegroundWindow(oProcs(nProc).MainWindowHandle)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:StartupModule" & _
                        " Routine: Main", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

End Module
