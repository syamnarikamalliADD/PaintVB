Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private mbNetworkAvailable As Boolean = False
        Private mbLostConnection As Boolean = False

        Private Sub MyApplication_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.Devices.NetworkAvailableEventArgs) Handles Me.NetworkAvailabilityChanged
            '********************************************************************************************
            'Description:  Post an alarm to the active alarms grid if the GUI loses it's network connection.
            '
            'Parameters: None
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            If e.IsNetworkAvailable Then
                If mbLostConnection Then
                    'Reset the alarm if we posted one
                    'NRU 161006 Changed hardcoded zone 1's to mnMinimumZoneNumber
                    Call frmMain.subPostInternalAlarm("006", "GUI", "WARN", mnMinimumZoneNumber, "Reset") 'Network Connection Lost
                    mbLostConnection = False
                    mbNetworkAvailable = True
                End If
            Else
                If mbNetworkAvailable Then
                    'Post alarm
                    'NRU 161006 Changed hardcoded zone 1's to mnMinimumZoneNumber
                    Call frmMain.subPostInternalAlarm("006", "GUI", "WARN", mnMinimumZoneNumber, "Active") 'Network Connection Lost
                    mbLostConnection = True
                    mbNetworkAvailable = False
                End If
            End If

        End Sub

        Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
            '********************************************************************************************
            'Description:  Put an entry in the Application event log if we try to start a second instance
            '              of Alarm Manager.
            '
            'Parameters: None
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            mDebug.WriteEventToLog("Module: AlarmMan - ApplicationEvents.vb, Routine: MyApplication_StartupNextInstance", _
                                   "Error: Attempt to start a second instance of Alarm Manager")

        End Sub

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            '********************************************************************************************
            'Description:  Display and log unhandled exceptions.
            '
            'Parameters: None
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            MessageBox.Show(e.Exception.Message & " - " & e.Exception.StackTrace, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, 0)
            mDebug.WriteEventToLog("Module: AlarmMan - ApplicationEvents.vb, Routine: MyApplication_UnhanldedException", _
                                   "Error: " & e.Exception.Message & vbCrLf & "StackTrace: " & e.Exception.StackTrace)

        End Sub

    End Class

End Namespace

