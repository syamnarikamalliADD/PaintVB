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
' Form/Module: clsDBAccess
'
' Description: Class for database access
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick O.
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    03/25/09   rjo     Added Friend TestDB for PW4_Main MS Access database check.
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/16/13   MSW     Standalone ChangeLog form, Isolate DB,SQL dependencies        4.01.05.00
'                       Move DB dependent routines to mDBCommon
'                       Implement Dispose like Microsoft suggests for Visual Studio 2012
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Data.OleDb

Friend Module mDBCommon
    Friend Function SaveToChangeLog(ByVal DatabasePath As String) As Boolean
        '*****************************************************************************************
        'Description: Save collection to database
        '               NOTE - This saves to access database for the old style reports to read
        '               it also calls the routine to save to SQL database - This part will 
        '               eventually go away -
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim DB As clsDBAccess = New clsDBAccess
        Dim DS As DataSet = New DataSet
        Dim sTmpsql As String = String.Empty
        Dim sTable As String = gsCHANGE_DS_TABLENAME
        Dim DT As DataTable = Nothing
        Dim tC As tChange = Nothing

        If colChanges Is Nothing Then Return True
        If colChanges.Count = 0 Then Return True

        DS.Locale = mLanguage.CurrentCulture

        Try

            'Select_:
            sTmpsql = "SELECT TOP 10 " & sTable & ".* FROM " & sTable

            With DB
                .DBFileName = gsCHANGE_DBNAME
                .DBFilePath = DatabasePath
                .DBTableName = gsCHANGE_DS_TABLENAME
                .SQLString = sTmpsql
                DS = .GetDataSet(False)

                If DS.Tables.Contains(gsCHANGE_DS_TABLENAME) Then
                    DT = DS.Tables(gsCHANGE_DS_TABLENAME)
                    For Each tC In colChanges
                        Dim dr As DataRow = DT.NewRow
                        DT.Rows.Add(dr)

                        dr.Item(gsCHANGE_AREA) = tC.Area
                        If tC.Device <> String.Empty Then
                            dr.Item(gsCHANGE_DEVICE) = tC.Device
                        End If
                        dr.Item(gsCHANGE_PARM1) = tC.Parm1
                        dr.Item(gsCHANGE_SPECIFIC) = tC.Specific
                        dr.Item(gsCHANGE_STARTSER) = tC.StartSerial
                        dr.Item(gsCHANGE_USER) = tC.User
                        dr.Item(gsCHANGE_ZONE) = tC.Zone

                    Next
                    .UpdateDataSet(DS)
                    .Close()
                    Return True

                Else
                    .Close()
                    Return False
                End If
            End With

        Catch ex As Exception
            Dim sTmp As String = "Module: mPWCommon, Routine: SaveToChangeLog, Error: "
            Trace.WriteLine(sTmp & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: SaveToChangeLog, StackTrace: " & ex.StackTrace)
            mDebug.ShowErrorMessagebox(sTmp, ex, System.Windows.Forms.Application.ProductName, _
                                        frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False

        Finally
            colChanges = Nothing
        End Try

    End Function

End Module
Friend Class clsDBAccess
    Implements IDisposable
#Region " Declares "
    '***** Module Constants  *******************************************************************
    ' for MS Access Databases
    Private Const msPASSWORD As String = "Persist Security Info=False;Jet OLEDB:Database Password="
    Private Const msPROVIDER As String = "Provider=Microsoft.Jet.OleDb.4.0"
    Private Const msSOURCE As String = "Data Source="

    Private Const msMODULE As String = "clsDBAccess"
    '***** End Module Constants   **************************************************************

    '***** Property Vars ***********************************************************************
    Private msDBFilePath As String = String.Empty
    Private msDBFileName As String = String.Empty
    Private msDBPassword As String = String.Empty
    Private msSQLStr As String = String.Empty
    Private msTableName As String = String.Empty
    Private mbUseDBPassword As Boolean ' = False
    '***** End Property Vars *******************************************************************

    '***** Working Vars ************************************************************************
    ' the following are module scoped so we can clean up afterwords
    Private mcnDB As OleDbConnection ' = Nothing
    Private mdrDB As OleDbDataReader ' = Nothing
    Private mcmDB As OleDbCommand ' = Nothing
    Private mdaAdapter As OleDbDataAdapter ' = Nothing
    Private mcbCmd As OleDbCommandBuilder ' = Nothing
    '***** End Working Vars *********************************************************************

    '******* Events *****************************************************************************
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events *************************************************************************
#End Region
#Region " Properties "

    Friend Property DBFilePath() As String
        '****************************************************************************************
        'Description: This Property is the path to the desired database 
        '
        'Parameters: path
        'Returns:   path
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            DBFilePath = msDBFilePath
        End Get
        Set(ByVal FullPath As String)
            msDBFilePath = FullPath
        End Set
    End Property
    Friend Property DBFileName() As String
        '****************************************************************************************
        'Description: This Property is the Name and Extention of the desired database 
        '
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            DBFileName = msDBFileName
        End Get
        Set(ByVal FileName As String)
            msDBFileName = FileName
        End Set
    End Property
    Friend WriteOnly Property DBPassword() As String
        '****************************************************************************************
        'Description: This Property is the Password (if required) to open the database 
        '
        'Parameters: Password
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Set(ByVal value As String)
            msDBPassword = value
        End Set
    End Property
    Friend Property DBTableName() As String
        '****************************************************************************************
        'Description: This Property is the Name  of the desired database Table
        '               Note: the command builder does not like spaces and such so
        '               Table names are forced to be in brackets. This may cause confusion
        '               when using the table name outside this class - therefore it can be read
        '               from here too.
        '
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            DBTableName = msTableName
        End Get
        Set(ByVal Value As String)
            msTableName = Value
            If Left(msTableName, 1) <> "[" Then
                msTableName = "[" & msTableName
            End If
            If Right(msTableName, 1) <> "]" Then
                msTableName = msTableName & "]"
            End If
        End Set
    End Property
    Friend Property SQLString() As String
        '****************************************************************************************
        'Description: This Property is the Query string - 
        '                       at a minimum, "Select * from [tablename]" 
        '
        'Parameters: Query string
        'Returns:   Query string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Get
            SQLString = msSQLStr
        End Get
        Set(ByVal Value As String)
            msSQLStr = Value
        End Set

    End Property

#End Region
#Region " Routines "

    Friend Function DeleteOldRecords(ByVal CutoffDate As Date) As Integer
        '****************************************************************************************
        'Description: This Function deletes records from log database tables that are older than 
        '             the cutoff date.
        '
        'Parameters: cutoff date - records older than this date will be deleted
        'Returns:    number of records affected
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nRecords As Integer

        Try
            SQLString = "DELETE FROM " & DBTableName & " WHERE ((" & DBTableName & _
                        ".[Start Serial] < #" & CutoffDate & "#));"

            If bGetDBConnection() Then
                mcmDB = New OleDbCommand(SQLString, mcnDB)
                nRecords = mcmDB.ExecuteNonQuery()
            End If

        Catch ex As Exception
            Dim sText As String = "Module: clsDBAccess, Routine: DeleteOldRecords, "

            Trace.WriteLine(sText & "Error: " & ex.Message)
            Trace.WriteLine(sText & "StackTrace: " & ex.StackTrace)
        End Try

        Return nRecords

    End Function

    Friend Function GetDataReader() As OleDbDataReader
        '****************************************************************************************
        'Description: This Function returns a datareader object in response to a query
        '               on failure post system error to avoid language thing.
        '
        'Parameters: None
        'Returns:   Datareader object ( = nothing on failure)
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        If bGetDBConnection() Then
            Try

                mcmDB = New OleDbCommand(SQLString, mcnDB)
                mdrDB = mcmDB.ExecuteReader()

            Catch ex As Exception
                Dim sText As String = "Module: clsDBAccess, Routine: GetDataReader, Error: " _
                                                                                & ex.Message
                Trace.WriteLine(sText)
                Trace.WriteLine("Module: clsDBAccess, Routine: GetDataReader, StackTrace: " _
                                                                            & ex.StackTrace)

                mDebug.ShowErrorMessagebox(sText, ex, _
                                System.Windows.Forms.Application.ProductName, _
                                frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Stop)


                RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)

            End Try
        End If  'bGetDBConnection()

        Return mdrDB

    End Function
    Friend Function GetDataSet(Optional ByVal UsePassword As Boolean = False) As DataSet
        '****************************************************************************************
        'Description: This Function gets a dataset object that can be used to modify the database
        '               table. Call updatedataset when your done monkeying with it
        '
        'Parameters: None
        'Returns:   Dataset
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim dsDataSet As New DataSet

        mbUseDBPassword = UsePassword
        dsDataSet.Locale = mLanguage.FixedCulture


        Try
            If bGetDBConnection() Then
                mdaAdapter = New OleDbDataAdapter
                mdaAdapter.SelectCommand = New OleDbCommand(SQLString, mcnDB)
                ' this command builder figures out how to save the data when we are done
                mcbCmd = New OleDbCommandBuilder(mdaAdapter)
                If msTableName = String.Empty Then
                    mdaAdapter.Fill(dsDataSet)
                Else
                    mcbCmd.QuotePrefix = "["
                    mcbCmd.QuoteSuffix = "]"
                    mdaAdapter.Fill(dsDataSet, msTableName)
                End If
            End If

        Catch ex As Exception
            Dim sText As String = "Module: clsDBAccess, Routine: GetDataSet, Error: " _
                                                                            & ex.Message
            Trace.WriteLine(sText)
            Trace.WriteLine("Module: clsDBAccess, Routine: GetDataSet, StackTrace: " _
                                                                        & ex.StackTrace)

            mDebug.ShowErrorMessagebox(sText, ex, _
                            System.Windows.Forms.Application.ProductName, _
                            frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Stop)


            RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)

        End Try

        Return dsDataSet

    End Function
    Friend Function UpdateDataSet(ByRef rDataset As DataSet) As Boolean
        '****************************************************************************************
        'Description: This saves the data to the database
        '
        ' TODO - This works great if the dataset is built from a single table that has a Primary
        '        key field. Otherwise, it throws an exception. To fix this for datasets that don't
        '        meet this criteria, we need to build the following manually:
        '
        '        mdaAdapter.DeleteCommand - if the new dataset has fewer rows than the original
        '        mdaAdapter.InsertCommand - if the new dataset has more rows than the original
        '        mdaAdapter.UpdateCommand - if the new dataset has changed values
        '
        '       See (painful) sample code below
        '
        'Parameters: Data Set
        'Returns:   True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            mdaAdapter.Update(rDataset, msTableName)
            Return True

        Catch ex As Exception
            Dim sText As String = "Module: clsDBAccess, Routine: UpdateDataSet, Error: " _
                                                                            & ex.Message
            Trace.WriteLine(sText)
            Trace.WriteLine("Module: clsDBAccess, Routine: UpdateDataSet, StackTrace: " _
                                                                        & ex.StackTrace)

            mDebug.ShowErrorMessagebox(sText, ex, _
                            System.Windows.Forms.Application.ProductName, _
                            frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Stop)


            RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)

            Return False
        End Try

        '<Sample Code> ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        'Private Function CreateCustomerAdapter(ByVal connection As OleDbConnection) As OleDbDataAdapter

        '    Dim dataAdapter As OleDbDataAdapter = New OleDbDataAdapter()
        '    Dim command As OleDbCommand
        '    Dim parameter As OleDbParameter

        '    ' Create the SelectCommand.
        '    command = New OleDbCommand("SELECT * FROM dbo.Customers " & _
        '        "WHERE Country = ? AND City = ?", connection)

        '    command.Parameters.Add("Country", OleDbType.VarChar, 15)
        '    command.Parameters.Add("City", OleDbType.VarChar, 15)

        '    dataAdapter.SelectCommand = command

        '' Create the DeleteCommand.
        'Command = New OleDbCommand( _
        '    "DELETE * FROM Customers WHERE CustomerID = ?", _
        '    connection)

        'parameter = Command.Parameters.Add( _
        '    "CustomerID", OleDbType.Char, 5, "CustomerID")
        'parameter.SourceVersion = DataRowVersion.Original

        'dataAdapter.DeleteCommand = Command()

        '' Create the InsertCommand.
        'Command = New OleDbCommand( _
        '    "INSERT INTO Customers (CustomerID, CompanyName) " & _
        '    "VALUES (?, ?)", connection)

        'Command.Parameters.Add( _
        '    "CustomerID", OleDbType.Char, 5, "CustomerID")
        'Command.Parameters.Add( _
        '    "CompanyName", OleDbType.VarChar, 40, "CompanyName")

        'adapter.InsertCommand = Command()

        '    ' Create the UpdateCommand.
        '    command = New OleDbCommand("UPDATE dbo.Customers " & _
        '        "SET CustomerID = ?, CompanyName = ? " & _
        '        "WHERE CustomerID = ?", connection)

        '    command.Parameters.Add( _
        '        "CustomerID", OleDbType.Char, 5, "CustomerID")
        '    command.Parameters.Add( _
        '        "CompanyName", OleDbType.VarChar, 40, "CompanyName")

        '    parameter = command.Parameters.Add( _
        '        "oldCustomerID", OleDbType.Char, 5, "CustomerID")
        '    parameter.SourceVersion = DataRowVersion.Original

        '    dataAdapter.UpdateCommand = command

        '    Return dataAdapter

        'End Function
        '<\Sample Code> ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    End Function
    Friend Function RepairDB() As Boolean
        '****************************************************************************************
        'Description: This Function attempts to Repair (Compact) an Access Database according to 
        '             properties entered. On failure returns False.
        '             ref. http://msdn.microsoft.com/en-us/library/bb237197.aspx
        '
        'Parameters: None
        'Returns:    True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            Dim jro As JRO.JetEngine = New JRO.JetEngine()
            Dim sSourceDB As String = sBuildConnString()
            Dim sDBName As String = DBFileName
            Dim sDestDB As String = Strings.Left(sDBName, Strings.Len(sDBName) - 4) & "REP.mdb"

            DBFileName = sDestDB
            sDestDB = sBuildConnString()

            jro.CompactDatabase(sSourceDB, sDestDB)
            Return True

        Catch ex As Exception

            mDebug.WriteEventToLog(ex.Source & " Module: clsDBAccess Routine: RepairDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False

        End Try

    End Function


    Friend Function TestDB(ByVal UsePassword As Boolean) As Integer
        '****************************************************************************************
        'Description: This Function Opens a Database according to properties entered.
        '             On failure returns the error number.
        '
        'Parameters: None
        'Returns:    0 if success, Err.Number if fail
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            mbUseDBPassword = UsePassword
            ' build the connection string
            Dim sConStr As String = sBuildConnString()

            mbUseDBPassword = False
            If Not (mcnDB Is Nothing) Then mcnDB.Close()
            mcnDB = New OleDbConnection(sConStr)
            mcnDB.Open()

            Return 0 'Success

        Catch ex As Exception

            mDebug.WriteEventToLog(ex.Source & " Module: clsDBAccess Routine: TestDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return Err.Number

        End Try

    End Function
    Private Function bGetDBConnection() As Boolean
        '****************************************************************************************
        'Description: This Function Opens a Database according to properties entered
        '               on failure post system error to avoid language thing.
        '
        'Parameters: UsePassword =
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sConStr As String = String.Empty

        Try
            ' build the connection string
            sConStr = sBuildConnString()
            If Not (mcnDB Is Nothing) Then mcnDB.Close()
            mcnDB = New OleDbConnection(sConStr)
            mcnDB.Open()

            Return True

        Catch ex As Exception
            Dim sText As String = "Module: clsDBAccess, Routine: bGetDBConnection, Error: " _
                                                                            & ex.Message
            Trace.WriteLine(sText)
            Trace.WriteLine("Module: clsDBAccess, Routine: bGetDBConnection, StackTrace: " _
                                                                        & ex.StackTrace)

            mDebug.ShowErrorMessagebox(sText, ex, _
                            System.Windows.Forms.Application.ProductName, _
                            frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Stop)


            RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)


            Return False

        End Try

    End Function
    Private Function sBuildConnString() As String
        '****************************************************************************************
        'Description: This Function builds the connection string
        '
        'Parameters: None
        'Returns:   connection string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sTmp As String

        sTmp = msPROVIDER & ";" & msSOURCE & DBFilePath & DBFileName

        If mbUseDBPassword Then
            sTmp = sTmp & ";" & msPASSWORD & msDBPassword
        End If

        Return sTmp

    End Function
    Friend Sub Close()
        '****************************************************************************************
        'Description: Clean up after ourselves
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            If Not (mdrDB Is Nothing) Then
                mdrDB.Close()
                mdrDB = Nothing
            End If
            If Not (mcmDB Is Nothing) Then
                mcmDB.Dispose()
                mcmDB = Nothing
            End If
            If Not (mdaAdapter Is Nothing) Then
                mdaAdapter.Dispose()
                mdaAdapter = Nothing
            End If
            If Not (mcnDB Is Nothing) Then
                mcnDB.Close()
                mcnDB.Dispose()
                mcnDB = Nothing
            End If
        Catch ex As Exception
            Trace.WriteLine("Module: clsDBAccess, Routine: Close, Error: " & ex.Message)
            Trace.WriteLine("Module: clsDBAccess, Routine: Close, StackTrace: " & ex.StackTrace)
            ' Error? What Error? I didn't see no error
        End Try

    End Sub

#End Region
#Region " Events "

    Friend Sub New()
        '****************************************************************************************
        'Description: Make sure we have a path on initialization
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        'set default Path
        mPWCommon.GetDefaultFilePath(msDBFilePath, _
                                    mPWCommon.eDir.Database, String.Empty, String.Empty)
    End Sub
    Friend Sub New(ByVal DatabasePath As String)
        '****************************************************************************************
        'Description: If we have a path on initialization
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        msDBFilePath = DatabasePath
    End Sub

    Protected Overridable Overloads Sub Dispose(disposing As Boolean)
        '****************************************************************************************
        'Description: Implement Dispose like Microsoft suggests
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Implement Dispose like Microsoft suggests
        '*****************************************************************************************
        Try
            If disposing Then
                ' dispose managed resources
                If Not (mdaAdapter Is Nothing) Then
                    mdaAdapter.Dispose()
                    mdaAdapter = Nothing
                End If
                If Not (mcnDB Is Nothing) Then
                    mcnDB.Close()
                    mcnDB.Dispose()
                    mcnDB = Nothing
                End If
                If Not (mcmDB Is Nothing) Then
                    mcmDB.Dispose()
                    mcmDB = Nothing
                End If
                If Not (mcbCmd Is Nothing) Then
                    mcbCmd.Dispose()
                    mcbCmd = Nothing
                End If
            End If

            ' free native resources 
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsDBAccess" & _
                        " Routine: Dispose(disposing As Boolean)", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub 'Dispose


    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        '****************************************************************************************
        'Description: Implement Dispose like Microsoft suggests
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Implement Dispose like Microsoft suggests
        '*****************************************************************************************

        Try
            Dispose(True)
            GC.SuppressFinalize(Me)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsDBAccess" & _
                        " Routine: Dispose", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub 'Dispose


#End Region
End Class
