Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
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

