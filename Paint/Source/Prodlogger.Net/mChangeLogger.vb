
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
' Form/Module: mChangeLogger
'
' Description: Manage the change log
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
'   06/08/12    MSW     1st version                                                   4.01.04.00
'   02/27/13    MSW     Support standalone programs without SQL                       4.01.04.04
'                       by moving SaveToChangeLogSqlOld from mPWCommon
'                       Requires ...\Source\Common.NET\mPWCommon.vb v4.01.04.04 
'   04/16/13    MSW     Implement Dispose like Microsoft suggests                     4.01.04.05
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Diagnostics
Imports System.Xml
Imports System.Xml.XPath

Class clsChangeLogger

    Implements IDisposable

    Private msPath As String = String.Empty
    Private mWatcher As FileSystemWatcher               'Requires Dispose
    Private moZone As clsZone
    Property Path() As String
        Get
            Return msPath
        End Get
        Set(ByVal value As String)
            msPath = value
        End Set
    End Property
    Public Sub New(ByRef oZone As clsZone)
        mPWCommon.GetDefaultFilePath(msPath, eDir.ChangeLogger, String.Empty, String.Empty)
        moZone = oZone

        mWatcher = New FileSystemWatcher

        With mWatcher
            .Path = msPath
            'Watch for changes in LastWrite times 
            .NotifyFilter = NotifyFilters.LastWrite
            .Filter = "*.xml"

            'Add event handler
            AddHandler .Changed, AddressOf OnChanged

            ' Begin watching.
            .EnableRaisingEvents = True
        End With
        Dim sFiles As String() = Directory.GetFiles(msPath)
        For Each sFile As String In sFiles
            If My.Computer.FileSystem.FileExists(sFile) Then
                AddToChangelog(sFile)
                My.Computer.FileSystem.DeleteFile(sFile)
            End If
        Next
        SaveToChangeLogSqlOld(moZone)
    End Sub
    Private Sub AddToChangelog(ByRef sFile As String)
        '********************************************************************************************
        'Description: Read from text file and add to the change log
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sXMLTABLE As String = "ChangeLog"
        Const sXMLNODE As String = "ChangeItem"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Try
            oXMLDoc.Load(sFile)
            oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
            oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

            For Each oNode As XmlNode In oNodeList
                Dim sArea As String = oNode.Item(gsCHANGE_AREA).InnerText()
                Dim sDevice As String = oNode.Item(gsCHANGE_DEVICE).InnerText()
                Dim sParm1 As String = oNode.Item(gsCHANGE_PARAM).InnerText()
                Dim sSpecific As String = oNode.Item(gsCHANGE_DESC).InnerText()
                Dim sTime As String = oNode.Item(gsCHANGE_TIME).InnerText()
                Dim sUser As String = oNode.Item(gsCHANGE_PWUSER).InnerText()
                Dim sZoneName As String = oNode.Item(gsCHANGE_ZONE).InnerText()
                AddChangeRecordToCollection(sArea, sUser, 1, sDevice, sParm1, sSpecific, sZoneName, sTime, False)
            Next

        Catch ex As Exception
            Dim sTmp As String = "Module: mChangeLogger, Routine: AddToChangelog(" & sFile & "), Error: "
            Trace.WriteLine(sTmp & ex.Message)
            Trace.WriteLine("Module: mChangeLogger, Routine: AddToChangelog, StackTrace: " & ex.StackTrace)
            mDebug.ShowErrorMessagebox(sTmp, ex, System.Windows.Forms.Application.ProductName, _
                                        frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try


    End Sub

    Private Sub OnChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        '********************************************************************************************
        'Description: A text file of interest was written to. Process, then delete the file.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ChangeType
            Case WatcherChangeTypes.Changed
                If My.Computer.FileSystem.FileExists(e.FullPath) Then
                    AddToChangelog(e.FullPath)
                    SaveToChangeLogSqlOld(moZone)
                    My.Computer.FileSystem.DeleteFile(e.FullPath)
                End If
        End Select

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
                If Not (mWatcher Is Nothing) Then
                    mWatcher.Dispose()
                    mWatcher = Nothing
                End If
            End If

            ' free native resources 
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsChangeLogger" & _
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
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: clsChangeLogger" & _
                        " Routine: Dispose", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub 'Dispose
End Class
Module mChangeLogger
    Friend Function SaveToChangeLogSqlOld(ByRef oZone As clsZone) As Boolean
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
                    For Each tC As mPWCommon.tChange In mPWCommon.colChangesSQL
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
