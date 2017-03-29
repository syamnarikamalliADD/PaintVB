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
'    10/29/09   MSW     get server name on the run.  Keeping default if we can't find it
'    11/19/09   MSW     hold onto the managed computer object, made it a shared module level object.
'                       This will probably need work for remote zones.
'    02/18/10   MSW     CompactDatabase - allow more time
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'                       Use HGB's compiler directives for now to get a mostly common source.  Try setting
'				        "PAINTworksSA = True"  out on a new project sometime.  It looks like a better way to do it.
'    12/02/11   MSW     Change to use PAINTworks SQL instance by default                   4.01.01.00
'    12/06/11   MSW     Use "PAINTWORKS" server if available, if not, try anything that's there 4.01.01.01
'    04/11/12   RJO     Revised encrypted builder.UserID and Password in 
'                       sBuildConnectionString for Crypto.dll v4.1.2.0                  4.01.02.00
'    02/04/13  ADD/RJO  Add 10ms Sleep time to bOpenConnection tight loop with DoEvents 4.01.02.01
'    04/30/13   MSW     Standalone changelog changes                                    4.01.05.00
'                       Implement Dispose like Microsoft suggests for Visual Studio 2012
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

'HGB used to set the SQLServer name for SA
#Const PAINTworksSA = True

Imports System.Data.Sql
Imports System.Data.SqlClient
Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Smo.Wmi
Imports Microsoft.SqlServer.Management.Common

Friend Module mSQLCommon
    Friend Function SaveToChangeLogSql(ByRef oZone As clsZone) As Boolean
        '*****************************************************************************************
        'Description: Save collection to SQL Server database
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/07/12  MSW     SaveToChangeLog - Change cursor and call doevents to deal with slow SQL access
        '*****************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim DS As DataSet = New DataSet
        Dim sTmpsql As String = "SELECT TOP 10 * FROM " & gsCHANGE_DS_TABLENAME
        Dim DT As DataTable = Nothing


        If mPWCommon.colChangesSQL Is Nothing Then Return True
        If mPWCommon.colChangesSQL.Count = 0 Then Return True

        'For many screeens this is the first SQL access, so it's real slow.
        Try
            frmMain.Cursor = Cursors.WaitCursor
            Application.DoEvents()
        Catch

        End Try


        DS.Locale = mLanguage.CurrentCulture

        Try

            With DB
                .DBFileName = gsCHANGE_DATABASENAME
                .DBTableName = gsCHANGE_DS_TABLENAME
                .Zone = oZone
                .SQLString = sTmpsql
                DS = .GetDataSet(False)

                If DS.Tables.Contains(gsCHANGE_DS_TABLENAME) Then
                    DT = DS.Tables(gsCHANGE_DS_TABLENAME)
                    For Each tC As tChange In mPWCommon.colChangesSQL
                        Dim dr As DataRow = DT.NewRow

                        dr.BeginEdit()

                        dr.Item(gsCHANGE_AREA) = tC.Area
                        If tC.Device <> String.Empty Then
                            dr.Item(gsCHANGE_DEVICE) = tC.Device
                        End If
                        dr.Item(gsCHANGE_PARAM) = tC.Parm1
                        dr.Item(gsCHANGE_DESC) = tC.Specific
                        dr.Item(gsCHANGE_TIME) = tC.StartSerial
                        dr.Item(gsCHANGE_PWUSER) = tC.User
                        dr.Item(gsCHANGE_ZONE) = tC.ZoneName

                        dr.EndEdit()

                        DT.Rows.Add(dr)

                    Next
                    .UpdateDataSet(DS, gsCHANGE_DS_TABLENAME)
                    .Close()
                    'For many screeens this is the first SQL access, so it's real slow.
                    Try
                        frmMain.Cursor = Cursors.Default
                        Application.DoEvents()
                    Catch

                    End Try

                    Return True

                Else
                    .Close()
                    Try
                        frmMain.Cursor = Cursors.Default
                    Catch

                    End Try
                    Return False
                End If
            End With

        Catch ex As Exception
            Dim sTmp As String = "Module: mPWCommon, Routine: SaveToChangeLog(oZone), Error: "
            Trace.WriteLine(sTmp & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: SaveToChangeLog, StackTrace: " & ex.StackTrace)
            mDebug.ShowErrorMessagebox(sTmp, ex, System.Windows.Forms.Application.ProductName, _
                                        frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Try
                frmMain.Cursor = Cursors.Default
            Catch

            End Try
            Return False

        Finally
            mPWCommon.colChangesSQL = Nothing

        End Try
        Try
            frmMain.Cursor = Cursors.Default
        Catch

        End Try
    End Function

End Module
Friend Class clsSQLAccess

    Implements IDisposable
#Region " Declares "
    '***** Module Constants  *******************************************************************
    ' Who knows what they will call it next year?
    'Changed to variable, try to figure it out on the run
    'HGB for PAINTworks SA use a named instance. This avoids conflicts when using the
    '    PAINTworks SA Installer since we do not know if a copy of SQL Server already
    '    exists on the destination machine.
    '#If PAINTworksSA Then
    Private msSQLSERVERNAME As String = "PAINTWORKS"
    Private mbServerFound As Boolean = False
    '#Else
    '    Private msSQLSERVERNAME As String = "SQLEXPRESS"
    '#End If
    'Preinstalled by dell it came up with this
    'Private Const msSQLSERVERNAME As String = "MSSMLBIZ"
    Private Const msMODULE As String = "clsSQLAccess"
    Private Const msFILEEXT As String = ".mdf"
    '***** End Module Constants   **************************************************************

    '***** Property Vars ***********************************************************************
    Private msDBFileName As String = String.Empty
    Private msDBPassword As String = String.Empty
    Private msSQLStr As String = String.Empty
    Private msTableName As String = String.Empty
    Private mbUseDBPassword As Boolean
    Private mZone As clsZone
    '***** End Property Vars *******************************************************************

    '***** Working Vars ************************************************************************
    ' the following are module scoped so we can clean up afterwords
    Private mcnSQL As SqlConnection
    Private mdaAdapter As SqlDataAdapter
    Private mcbCmd As SqlCommandBuilder
    Private mStopWatch As Stopwatch
    Shared oMC As ManagedComputer = Nothing
    '***** End Working Vars *********************************************************************

    '******* Events *****************************************************************************
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                                ByVal sModule As String, ByVal AdditionalInfo As String)
    Friend Event CompactDone(ByVal Success As Boolean)
    '******* End Events *************************************************************************
#End Region
#Region " Properties "

    Friend ReadOnly Property ConnectionOpen() As Boolean
        '****************************************************************************************
        'Description: This Property returns the state of the connection to SQL server 
        '
        'Parameters: Name
        'Returns:   Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return bOpenDatabase()
        End Get
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
            If msTableName <> String.Empty Then
                If Left(msTableName, 1) <> "[" Then
                    msTableName = "[" & msTableName
                End If
                If Right(msTableName, 1) <> "]" Then
                    msTableName = msTableName & "]"
                End If
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
    Friend Property Zone() As clsZone
        '****************************************************************************************
        'Description: Clszone so we can get zone info
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return mZone
        End Get
        Set(ByVal value As clsZone)
            mZone = value
        End Set
    End Property

#End Region
#Region " Routines "
    Friend Function CompactDatabase(ByVal CutOffDate As Date) As Boolean
        '****************************************************************************************
        'Description: Run the procedure DeleteOldRecords on database
        'Parameters: Date to pass to procedure
        'Returns:   true when done
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 2/18/10   MSW     CompactDatabase - allow more time
        '*****************************************************************************************
        Try
            If bOpenDatabase() Then
                Dim s As SqlClient.SqlCommand
                s = GetStoredProcedureCommand("DeleteOldRecords")
                s.CommandTimeout = 30 'MSW 2/18/10 allow more time
                s.Parameters.Add(New SqlClient.SqlParameter("@DeleteFromDate", _
                        SqlDbType.DateTime)).Value = CutOffDate
                s.ExecuteNonQuery()
                RaiseEvent CompactDone(True)
                Return True
            Else
                Return False
                RaiseEvent CompactDone(False)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            RaiseEvent CompactDone(False)
            Return False
        End Try
    End Function
    Friend Function GetStoredProcedureCommand(ByVal ProcedureName As String) As SqlCommand
        '****************************************************************************************
        'Description: This Function gets a sqlcommand object so calling app can set parameters
        'Parameters: name of procedure
        'Returns:   Dataset
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            If mcnSQL Is Nothing Then
                'need to establish a connection
                If bOpenDatabase() = False Then Return Nothing
            End If

            Dim s As New SqlCommand(ProcedureName, mcnSQL)
            s.CommandType = CommandType.StoredProcedure
            Return s

        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Friend Function GetDataSet(Optional ByVal UsePassword As Boolean = False, Optional ByVal bHideError As Boolean = False, Optional ByVal bHideMessageThrowError As Boolean = False) As DataSet
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
        'Trace.WriteLine("Getdataset called : +" & mStopWatch.ElapsedMilliseconds & " Milliseconds")

        Dim dsDataSet As New DataSet
        dsDataSet.Locale = mLanguage.FixedCulture

        Try

            If bOpenDatabase() = False Then Return Nothing

            'Trace.WriteLine("Connection open : +" & mStopWatch.ElapsedMilliseconds & " Milliseconds")

            mdaAdapter = New SqlDataAdapter
            mdaAdapter.SelectCommand = New SqlCommand(SQLString, mcnSQL)
            mdaAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            mcbCmd = New SqlCommandBuilder(mdaAdapter)

            If msTableName = String.Empty Then
                mdaAdapter.Fill(dsDataSet)
            Else
                mcbCmd.QuotePrefix = "["
                mcbCmd.QuoteSuffix = "]"
                mdaAdapter.Fill(dsDataSet, msTableName)
            End If

            'Trace.WriteLine("Data Returned : +" & mStopWatch.ElapsedMilliseconds & " Milliseconds")
            'mStopWatch.Stop()
            Return dsDataSet


        Catch ex As Exception
            If Not (bHideError Or bHideMessageThrowError) Then
                Dim sText As String = "Module: clsSQLAccess, Routine: GetDataSet, Error: " _
                                                                                & ex.Message
                Trace.WriteLine(sText)
                Trace.WriteLine("Module: clsSQLAccess, Routine: GetDataSet, StackTrace: " _
                                                                            & ex.StackTrace)

                mDebug.ShowErrorMessagebox(sText, ex, _
                                System.Windows.Forms.Application.ProductName, _
                                frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If

            If Not (bHideError) Then
                '4/22/08    routine needs to handle
                'RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)
                Throw 'ex 'per fxCop to preserve detail
            End If
            
            
        End Try

        Return dsDataSet

    End Function
    Friend Function GetDataSet(ByRef rSQLCommand As SqlClient.SqlCommand) As DataSet
        '****************************************************************************************
        'Description: This Function gets a dataset object  - only good for single table now
        '
        'Parameters: a sqlcommand object
        'Returns:   Dataset
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim dsDataSet As New DataSet
        dsDataSet.Locale = mLanguage.FixedCulture

        Try

            mdaAdapter = New SqlDataAdapter(rSQLCommand)
            mdaAdapter.Fill(dsDataSet, DBTableName)
            Return dsDataSet

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        Return dsDataSet

    End Function
    Friend Function UpdateDataSet(ByRef rDataset As DataSet, ByVal TableName As String) As Boolean
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
        '
        'Parameters: Data Set
        'Returns:   True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try

            If Strings.Left(TableName, 1) <> "[" Then TableName = "[" & TableName
            If Strings.Right(TableName, 1) <> "]" Then TableName = TableName & "]"

            mdaAdapter.Update(rDataset, TableName)

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

    End Function
    Private Function bOpenDatabase() As Boolean
        '****************************************************************************************
        'Description: Open up a database
        '
        'Parameters: 
        'Returns:   connection string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            If (mcnSQL Is Nothing) = False Then
                'see if we are still open
                If mcnSQL.Database = DBFileName Then
                    'so far so good
                    If mcnSQL.State = ConnectionState.Open Then
                        Return True
                    End If
                End If
            End If

            'try the windows login first
            Dim sConn As String = sBuildConnectionString(True)
            mcnSQL = New SqlClient.SqlConnection(sConn)

            Return bOpenConnection(mcnSQL)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Private Function sBuildConnectionString(ByVal UseWindowsAuthentication As Boolean) As String
        '****************************************************************************************
        'Description: This function builds a connection string.
        '
        'Parameters: 
        'Returns:   connection string
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/29/09  MSW     get server name on the run.  Keeping default if we can't find it
        ' 11/19/09  MSW     hold onto managed computer object, made it a shared 
        ' 09/06/11  HGB     added remote SQL server access
        ' 04/11/12  RJO     revised encrypted builder.UserID and Password for Crypto.dll v4.1.2.0
        '*****************************************************************************************

        Try

            Dim builder As New SqlConnectionStringBuilder

            'For remote zone access
            If Zone.IsRemoteZone Then
                builder.IntegratedSecurity = False
            Else
                builder.IntegratedSecurity = UseWindowsAuthentication
            End If
            If Zone.IsRemoteZone Then ' Not UseWindowsAuthentication Then
                If Zone.IsRemoteZone Then ' Not UseWindowsAuthentication Then
                    'NVSP Remote Login 12/13/2016
                    'builder.UserID = Crypto.Crypto.Decrypt _
                    '   ("253 18 97 78 225 7 12 107 143 62 18 78 17 36 8 34")
                    builder.UserID = "RemoteLogin"
                    builder.Password = Crypto.Crypto.Decrypt _
                        ("234 166 229 232 79 125 89 94 245 227 43 74 217 54 13 222")
                    builder.NetworkLibrary = "dbmssocn" 'use TCP/IP
                End If
                builder.Password = Crypto.Crypto.Decrypt _
                    ("234 166 229 232 79 125 89 94 245 227 43 74 217 54 13 222")
                builder.NetworkLibrary = "dbmssocn" 'use TCP/IP
            End If
            builder.InitialCatalog = DBFileName
            builder.ContextConnection = False 'default
            builder.UserInstance = False
            builder.MultipleActiveResultSets = True ' not default
            builder.LoadBalanceTimeout = 30 ' not default
            builder.AsynchronousProcessing = True ' not default

            If Zone Is Nothing Then

                Throw New DataException("No Zone Object for Data Access")

            Else

                builder.AttachDBFilename = Zone.DatabasePath & DBFileName & msFILEEXT

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Declare and create an instance of the ManagedComputer object that 
                'represents the WMI Provider services.
                If oMC Is Nothing Then
                    'Dim mc As ManagedComputer
                    If Zone.IsRemoteZone Then
                        oMC = New ManagedComputer(Zone.ServerName)
                    Else
                        oMC = New ManagedComputer()
                    End If
                End If
                'Reference the Microsoft SQL Server express 2005 service.
                'Somebody forgot to use the constant
                'Deal with GM's setup.  Force the DB name
                Dim sTmpString As String = String.Empty
                For Each tmpsvc As Service In oMC.Services
                    'MSW 12/06/11 - Use prefered server if available, if not, try anything that's there
                    If tmpsvc.Name.StartsWith("MSSQL$" & msSQLSERVERNAME) Then
                        sTmpString = msSQLSERVERNAME
                        Exit For
                    End If
                    If tmpsvc.Name.StartsWith("MSSQL$") Then
                        sTmpString = tmpsvc.Name.Substring(6)
                    End If
                Next
                If sTmpString <> String.Empty Then
                    msSQLSERVERNAME = sTmpString
                    mbServerFound = True
                End If
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If Zone.IsRemoteZone Then
                    builder.DataSource = Zone.ServerName & "\" & msSQLSERVERNAME
                    builder.AttachDBFilename = ""
                Else
                    builder.DataSource = "(local)\" & msSQLSERVERNAME
                End If
            End If

            Return builder.ConnectionString

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            RaiseEvent ModuleError(Err.Number, ex.Message, msMODULE, String.Empty)

            Return String.Empty

        End Try

    End Function
    Friend Function bCreateNewDatatable(ByRef DT As DataTable) As Boolean
        '****************************************************************************************
        'Description: Create a new table in the DB using the passed in DT
        '
        'Parameters: data table 
        'Returns:   true if successful
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        If bOpenDatabase() = False Then Return False

        Dim oServer As New Server
        oServer.ConnectionContext.ConnectionString = sBuildConnectionString(True)
        Dim oDB As Database = oServer.Databases(DBFileName)
        Dim t As New Table(oDB, DBTableName)
        Dim oDatatype As DataType
        For nCol As Integer = 0 To DT.Columns.Count - 1
            If (DT.Columns.Item(nCol).DataType Is GetType(Integer)) Or _
                (DT.Columns.Item(nCol).DataType Is GetType(Long)) Or _
                (DT.Columns.Item(nCol).DataType Is GetType(Short)) Then
                odatatype = DataType.Int
            ElseIf DT.Columns.Item(nCol).DataType Is GetType(Date) Then
                odatatype = DataType.DateTime
            ElseIf (DT.Columns.Item(nCol).DataType Is GetType(Single)) Or _
                    (DT.Columns.Item(nCol).DataType Is GetType(Double)) Or _
                    (DT.Columns.Item(nCol).DataType Is GetType(Decimal)) Then
                oDatatype = DataType.Float
            Else 'If DT.Columns.Item(nCol).DataType Is GetType(String) Then
                'default to string
                oDatatype = DataType.NVarCharMax
            End If
            Dim c As Column
            c = New Column(t, DT.Columns.Item(nCol).Caption, odatatype)
            t.Columns.Add(c)
        Next
        t.Create()
        For nRow As Integer = 0 To DT.Rows.Count - 1

            For nCol As Integer = 0 To DT.Columns.Count - 1
            Next
        Next
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
            If Not (mdaAdapter Is Nothing) Then
                mdaAdapter.Dispose()
                mdaAdapter = Nothing
            End If
            If Not (mcnSQL Is Nothing) Then
                mcnSQL.Close()
                mcnSQL.Dispose()
                mcnSQL = Nothing
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: clsSQLAccess, Routine: Close, Error: " & ex.Message)
            Trace.WriteLine("Module: clsSQLAccess, Routine: Close, StackTrace: " & ex.StackTrace)
            ' Error? What Error? I didn't see no error
        End Try

    End Sub
    Public Sub subBackupDB(ByVal DBName As String, _
                           ByVal sDBPath As String, _
                           ByVal sBackupPath As String, _
                           Optional ByRef sStatus As String = Nothing)
        '********************************************************************************************
        'Description: Creates a backup of the (DBName) SQL server database.
        '
        'Parameters: DBName - The database file name (ex. "Alarm Log.mdf")
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         11/12/09      Copy servername code from SQLClass - VB had an attitude about using the same variable
        ' HGB         05/18/11      Changed to use the server name string - was hardcoded.
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmMain.DisplayCulture

        Try
            Dim dtExpire As Date
            Dim oBackup As New Backup
            Dim oBdi As BackupDeviceItem
            Dim oBuilder As New SqlClient.SqlConnectionStringBuilder("Network Address=(local)")

            Dim oServer As New Server
            Dim sBackupFileName As String = "dbo." & Strings.Replace(DBName.ToLower, "mdf", "bak")
            Dim sDBName As String = Strings.Left(DBName, Strings.Len(DBName) - 4)

            If oMC Is Nothing Then
                'Dim mc As ManagedComputer
                oMC = New ManagedComputer()
            End If

            Dim sTmpString As String = String.Empty
            For Each tmpsvc As Service In oMC.Services
                'MSW 12/06/11 - Use prefered server if available, if not, try anything that's there
                If tmpsvc.Name.StartsWith("MSSQL$" & msSQLSERVERNAME) Then
                    sTmpString = msSQLSERVERNAME
                    Exit For
                End If
                If tmpsvc.Name.StartsWith("MSSQL$") Then
                    sTmpString = tmpsvc.Name.Substring(6)
                End If
            Next
            If sTmpString <> String.Empty Then
                msSQLSERVERNAME = sTmpString
                mbServerFound = True
            End If


            'Backup DBName to backup folder
            'Connect to the local, default instance of SQL Server.
            With oBuilder
                .IntegratedSecurity = True
                .InitialCatalog = sDBName
                .ContextConnection = False 'default
                .UserInstance = False
                .MultipleActiveResultSets = True ' not default
                .LoadBalanceTimeout = 30 ' not default
                .AsynchronousProcessing = True ' not default
                .AttachDBFilename = sDBPath & DBName
                .DataSource = "(local)\" & msSQLSERVERNAME ' HGB Was hardcoded to "(local)\SQLEXPRESS"
                oServer.ConnectionContext.ConnectionString = .ConnectionString

            End With

            'Define a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
            oBdi = New BackupDeviceItem(sBackupPath & "\" & sBackupFileName, DeviceType.File)

            'Define the backup expiration date.
            dtExpire = DateAdd(DateInterval.Year, 1, Now)

            'Define the Backup object variable. 
            With oBackup
                'Specify the type of backup, the description, the name, and the database to be backed up.
                .Action = BackupActionType.Database
                .BackupSetDescription = "Full backup of " & sDBName
                .BackupSetName = sDBName & " Backup"
                .Database = sDBName
                'Add the backup device to the Backup object.
                .Devices.Add(oBdi)
                'Set the Incremental property to False to specify that this is a full database backup.
                .Incremental = False
                'Set the backup expiration date
                .ExpirationDate = dtExpire
                'Specify that the log must be truncated after the backup is complete.
                .LogTruncation = BackupTruncateLogType.Truncate
                'Run SqlBackup to perform the full database backup on the instance of SQL Server.
                .SqlBackup(oServer)
                'Remove the backup device from the Backup object.
                .Devices.Remove(oBdi)
            End With

            oServer = Nothing

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & "- Module: clsSQLAccess- Routine: subBackupDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, frmMain.msSCREEN_NAME, _
                    sStatus, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Function bOpenConnection(ByRef conn As SqlConnection) As Boolean
        '****************************************************************************************
        'Description: checking is expensive, only call if needed
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/05/09  MSW     replace SQLEXPRESS" with the constant 
        ' 10/29/09  MSW     get server name on the run. 
        ' 11/19/09  MSW     hold onto managed computer object, made it a shared 
        ' 02/04/13  ADD/RJO Add 10ms Sleep time to tight loop with DoEvents
        '*****************************************************************************************

        Try
            conn.Open()
            Return True

        Catch sex As SqlException

            Select Case sex.Number
                Case -1
                    'This is the error code generated when the service was stopped -     
                    'sex.Message: "An error has occurred while establishing a connection to the server.  
                    'When connecting to SQL Server 2005, this failure may be caused by the fact 
                    'that under the default settings SQL Server does not allow remote connections. 
                    '(provider: SQL Network Interfaces, 
                    'error: 26 - Error Locating Server/Instance Specified)"

                    'assume not running and try to start
                    Try

                        'Declare and create an instance of the ManagedComputer object that 
                        'represents the WMI Provider services.
                        If oMC Is Nothing Then
                            'Dim mc As ManagedComputer
                            If Zone.IsRemoteZone Then
                                oMC = New ManagedComputer(Zone.ServerName)
                            Else
                                oMC = New ManagedComputer()
                            End If
                        End If

                        'Reference the Microsoft SQL Server express 2005 service.
                        'Somebody forgot to use the constant
                        Dim svc As Service = Nothing
                        Dim sTmpString As String = String.Empty
                        For Each tmpsvc As Service In oMC.Services
                            'MSW 12/06/11 - Use prefered server if available, if not, try anything that's there
                            If tmpsvc.Name.StartsWith("MSSQL$" & msSQLSERVERNAME) Then
                                svc = tmpsvc
                                sTmpString = msSQLSERVERNAME
                                Exit For
                            End If
                            If tmpsvc.Name.StartsWith("MSSQL$") Then
                                svc = tmpsvc
                                sTmpString = tmpsvc.Name.Substring(6)
                            End If
                        Next
                        If sTmpString <> String.Empty Then
                            msSQLSERVERNAME = sTmpString
                            mbServerFound = True
                        End If

                        If svc IsNot Nothing Then
                            Select Case svc.ServiceState
                                Case ServiceState.Running
                                    'happy happy joy joy
                                    'this case should not happen
                                    conn.Open()
                                    Return True
                                Case ServiceState.Stopped
                                    svc.Start()
                                    Do Until svc.ServiceState = ServiceState.Running
                                        Application.DoEvents()
                                        Threading.Thread.Sleep(10) 'ADD/RJO 02/04/13
                                        svc.Refresh()
                                    Loop
                                    conn.Open()
                                    Return True
                                Case Else
                                    Return False
                            End Select
                        Else
                            Return False
                        End If
                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                        Return False
                    End Try

                    'fail login
                Case 18456
                    'try the SQL login
                    Dim c As String = sBuildConnectionString(False)
                    Try
                        conn.ConnectionString = c
                        conn.Open()
                        Return True

                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                        Return False
                    End Try
            End Select
        End Try

    End Function

#End Region
#Region " Events "

    Friend Sub New()
        '****************************************************************************************
        'Description: 
        '
        'Parameters: None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        'mStopWatch = New Stopwatch
        'mStopWatch.Start()
        'Trace.WriteLine("New Class SQL access")
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
                If Not (mcnSQL Is Nothing) Then
                    mcnSQL.Close()
                    mcnSQL.Dispose()
                    mcnSQL = Nothing
                End If
                If Not (mcbCmd Is Nothing) Then
                    mcbCmd.Dispose()
                    mcbCmd = Nothing
                End If
            End If

            ' free native resources 
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsSQLAccess" & _
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
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsSQLAccess" & _
                        " Routine: Dispose", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Sub 'Dispose

#End Region

End Class
