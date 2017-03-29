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
' Form/Module: frmMain
'
' Description: PWInstall  - Paintworks installation program.
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    11/20/2012 MSW     first draft                                                   4.01.05.00
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    01/06/14   MSW     fairfax updates                                               4.01.06.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports VB = Microsoft.VisualBasic
Imports System.Drawing.Printing
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip
Friend Class frmMain
    Inherits System.Windows.Forms.Form

    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "PWInstall"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    'Private Const mnROWSPACE As Integer = 40 ' interval for rows of textboxes etc

    Private msCulture As String = "en-US" 'Default to English

    'Installation sequence counter
    Private mnBigStep As Integer = 0
    Private mnLittleStep As Integer = 0
    Private mbAbort As Boolean = False
    Private msPaintFolderName As String = "Paint"
    Private msRootFolder As String = String.Empty
    Private msPaintFolderSource As String = String.Empty
    Private msPaintFolderDest As String = String.Empty
    Private msZone As String = String.Empty
    Private msRobots() As String = Nothing
    Private msMakeFolders() As String = Nothing
    Private msAutoBackupFolder As String = String.Empty
    Private msDuplicatePaintFolder As String = String.Empty
    Private msRobotBackupFolders() As String = Nothing
    Private Const mnCOMPLETE As Integer = 99
    Friend Class tZoneSelect
        Public ZoneName As String
        Public RobotNames() As String
    End Class
    Private colZoneSelect As New Collection
    Friend Class InstallAction
        Public action As String
        Public Parameter() As String
        Public EnableAction As Boolean
        Public ZoneSelect() As String
    End Class
    Private colActions As New Collection
    Friend Sub SetStatus(ByVal sMessage As String)
        '********************************************************************************************
        'Description:  Update the status box.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lstStatus.Items.Add(sMessage)
        lstStatus.SelectedIndex = lstStatus.Items.Count - 1
        Application.DoEvents()

    End Sub

    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(String.Empty, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                String.Empty)


        End Set

    End Property
    Private Sub subCopyAll(ByVal sFrom As String, ByVal sTo As String)
        '********************************************************************************************
        'Description:  Copy folders
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If sFrom.Substring(sFrom.Length - 1, 1) <> "\" Then
                sFrom = sFrom & "\"
            End If
            If sTo.Substring(sTo.Length - 1, 1) <> "\" Then
                sTo = sTo & "\"
            End If
            If Not (IO.Directory.Exists(sTo)) Then
                IO.Directory.CreateDirectory(sTo)
            End If
            Dim sFiles() As String = IO.Directory.GetFiles(sFrom)
            For Each sFile As String In sFiles
                If (sFile.ToLower.EndsWith(".zip") <> True) Then
                    Dim sTmp() As String = Split(sFile, "\")
                    Dim sFileTo As String = sTo & sTmp(sTmp.GetUpperBound(0))
                    Try
                        If mbAbort Then
                            Exit Sub
                        End If
                        SetStatus(String.Format(gpsRM.GetString("psCOPYING01"), sFile, sFileTo))
                        IO.File.Copy(sFile, sFileTo, True)
                    Catch ex As Exception
                        SetStatus("IO.File.Copy(" & sFile & ", " & sFileTo & ", True)")
                        SetStatus(ex.Message)
                    End Try
                End If
            Next
            Dim sFolders() As String = IO.Directory.GetDirectories(sFrom)
            For Each sFolder As String In sFolders
                Dim sTmp() As String = Split(sFolder, "\")
                Dim sFolderTo As String = sTo & sTmp(sTmp.GetUpperBound(0))
                Try
                    If mbAbort Then
                        Exit Sub
                    End If
                    If colZoneSelect.Count > 0 AndAlso sFrom.ToLower.EndsWith("database\") Then
                        If cboZone.Text = sTmp(sTmp.GetUpperBound(0)) Then
                            SetStatus(String.Format(gpsRM.GetString("psCOPYING01"), sFolder, sFolderTo))
                            subCopyAll(sFolder, sFolderTo)
                        End If
                    Else
                        SetStatus(String.Format(gpsRM.GetString("psCOPYING01"), sFolder, sFolderTo))
                        subCopyAll(sFolder, sFolderTo)
                    End If
                Catch ex As Exception
                    SetStatus("subCopyAll(" & sFolder & ", " & sFolderTo & ", True)")
                    SetStatus(ex.Message)
                End Try
            Next
        Catch ex As Exception
            SetStatus(ex.Message)
        End Try
    End Sub
    Private Sub subMakePaintFolder(ByRef sPaint As String)
        '********************************************************************************************
        'Description:  Make Paint folder structure
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If Not (IO.Directory.Exists(sPaint)) Then
                IO.Directory.CreateDirectory(sPaint)
            End If
            'Main Paint folder
            Dim sSplit() As String = Split(sPaint, "\")
            Dim sTmpString As String = String.Empty
            For Each sTmp As String In sSplit
                If sTmp <> String.Empty Then
                    sTmpString = sTmpString & sTmp & "\"
                    If Not (IO.Directory.Exists(sPaint)) Then
                        IO.Directory.CreateDirectory(sTmpString)
                    End If
                End If
            Next
            If mbAbort Then
                Exit Sub
            End If
            'Robot backup folders
            For Each sTmp As String In msRobotBackupFolders
                If sTmp <> String.Empty Then
                    Dim sTmp2 As String = sPaint & sTmp & "\"
                    If Not (IO.Directory.Exists(sTmp2)) Then
                        IO.Directory.CreateDirectory(sTmp2)
                    End If
                    For Each sTmp3 As String In msRobots
                        If sTmp3 <> String.Empty Then
                            If Not (IO.Directory.Exists(sTmp2 & sTmp3)) Then
                                IO.Directory.CreateDirectory(sTmp2 & sTmp3)
                            End If
                        End If
                    Next
                End If
            Next
            If mbAbort Then
                Exit Sub
            End If
            'other folders
            For Each sTmp As String In msMakeFolders
                If sTmp <> String.Empty Then
                    Dim sTmp2 As String = sPaint & sTmp & "\"
                    If Not (IO.Directory.Exists(sTmp2)) Then
                        IO.Directory.CreateDirectory(sTmp2)
                    End If
                End If
            Next
            'Autobackup path
            Dim sTmpPath As String = String.Empty
            If msAutoBackupFolder.StartsWith("\") Then
                'relative path
                sTmpPath = sPaint & msAutoBackupFolder
            Else
                'absolute path
                sTmpPath = msAutoBackupFolder
            End If
            If mbAbort Then
                Exit Sub
            End If
            If sTmpPath <> String.Empty Then
                Dim sSplit2() As String = Split(sTmpPath, "\")
                Dim sTmpString2 As String = String.Empty
                For Each sTmp As String In sSplit2
                    If sTmp <> String.Empty Then
                        sTmpString2 = sTmpString2 & sTmp & "\"
                        If Not (IO.Directory.Exists(sTmpString2)) Then
                            IO.Directory.CreateDirectory(sTmpString2)
                        End If
                    End If
                Next
            End If

            SetStatus(String.Format(gpsRM.GetString("psCOPYING012"), _
                                    msPaintFolderName, msPaintFolderSource, msPaintFolderDest))
            subCopyAll(msPaintFolderSource, sPaint)

        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbAbort = True
        End Try

    End Sub
    Private Function bCheckFolderChoice(ByRef sPath As String) As Boolean
        '********************************************************************************************
        'Description: make sure the drive exists so we can make the path,
        '             convert relative paths to absolute
        '             
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If sPath.StartsWith(".\") Then
            sPath = sPath.Replace(".\", msRootFolder)
        End If
        If sPath.Length > 1 Then
            Dim sDrive As String = sPath.Substring(0, 2).ToUpper
            Return IO.Directory.Exists(sDrive)
        Else
            Return False
        End If
    End Function
    Private Sub btnSkip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSkip.Click
        '********************************************************************************************
        'Description:  Skip a step
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case mnBigStep
            Case 0
                'Skip Paint Folder Configuration
                mnLittleStep = 0
                mnBigStep = 1
                lblLabel.Text = (gpsRM.GetString("psLBL_INST_PROG"))
                btnNext.Text = gpsRM.GetString("psNEXT")
                btnSkip.Text = gpsRM.GetString("psSELECT")
                btnNext.Enabled = True
                btnSkip.Enabled = True
                btnNext.Visible = True
                btnSkip.Visible = True
                cboZone.Visible = False
            Case 1
                Select Case mnLittleStep
                    Case 0
                        'Select individual programs
                        For nItem As Integer = 1 To colActions.Count
                            Try
                                Dim oAction As InstallAction = DirectCast(colActions(nItem), InstallAction)
                                With oAction
                                    Dim sText As String = .Parameter(0)
                                    Select Case .action
                                        Case "run"
                                            If .Parameter.GetUpperBound(0) > 0 Then
                                                sText = (.Parameter(1))
                                            End If
                                        Case "subst"
                                            If .Parameter.GetUpperBound(0) > 1 Then
                                                sText = (.Parameter(2))
                                            End If
                                        Case "vbs", "msi"
                                            If .Parameter.GetUpperBound(0) > 0 Then
                                                sText = (.Parameter(1))
                                            End If
                                        Case "extract"
                                            Try
                                                If IO.File.Exists(.Parameter(0)) Then
                                                    Dim sOutputPath As String = msPaintFolderDest
                                                    If .Parameter.GetUpperBound(0) > 0 Then
                                                        sOutputPath = .Parameter(1)
                                                    End If
                                                    sText = (String.Format(gpsRM.GetString("psEXTRACT"), .Parameter(0), sOutputPath))
                                                End If
                                            Catch ex As Exception
                                                sText = (ex.Message)
                                            End Try

                                        Case "copyfolder"
                                            sText = (String.Format(gpsRM.GetString("psCOPY01"), .Parameter(0), .Parameter(1)))
                                        Case "copyfile"
                                            sText = (String.Format(gpsRM.GetString("psCOPY01"), .Parameter(0), .Parameter(1)))
                                        Case "regimport"
                                            sText = (String.Format(gpsRM.GetString("psREGIMPORT"), .Parameter(0)))
                                    End Select
                                    clbActions.Items.Add(sText)
                                    clbActions.SetItemChecked(nItem - 1, .EnableAction)
                                End With
                            Catch ex As Exception
                                SetStatus(ex.Message)
                            End Try
                        Next
                        btnSkip.Text = gpsRM.GetString("psDONE")
                        mnLittleStep = 1
                        btnNext.Enabled = False
                        btnSkip.Enabled = True
                        btnNext.Visible = True
                        btnSkip.Visible = True
                        clbActions.Visible = True
                        cboZone.Visible = False
                    Case 1
                        For nItem As Integer = 1 To colActions.Count
                            Try
                                Dim oAction As InstallAction = DirectCast(colActions(nItem), InstallAction)
                                oAction.EnableAction = clbActions.GetItemChecked(nItem - 1)
                            Catch ex As Exception
                                SetStatus(ex.Message)
                            End Try
                        Next
                        btnSkip.Text = gpsRM.GetString("psSELECT")
                        mnLittleStep = 0
                        btnNext.Enabled = True
                        btnSkip.Enabled = True
                        btnNext.Visible = True
                        btnSkip.Visible = True
                        clbActions.Visible = False
                End Select
            Case 2
                'Skip Duplicate Paint Folder Configuration
                mnBigStep = mnCOMPLETE
                lblLabel.Text = gpsRM.GetString("psLBL_COMPLETE")
                btnCancel.Visible = False
                btnNext.Text = gpsRM.GetString("psFINISH")
        End Select
    End Sub
    Private Sub btnNext_Click(Optional ByVal eventSender As System.Object = Nothing, _
                              Optional ByVal eventArgs As System.EventArgs = Nothing) Handles btnNext.Click
        '********************************************************************************************
        'Description:  Next button - run the installation
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Me.Cursor = Cursors.WaitCursor
            btnNext.Enabled = False
            btnSkip.Enabled = False
            cboZone.Enabled = False
            Select Case mnBigStep
                Case 0
                    'Make folders, copy the bulk of it.
                    'Make Paint Folder
                    SetStatus(String.Format(gpsRM.GetString("psCREATINGFOLDERS"), msPaintFolderName, msPaintFolderDest))
                    subMakePaintFolder(msPaintFolderDest)
                    mnLittleStep = 0
                    mnBigStep = 1
                    lblLabel.Text = (gpsRM.GetString("psLBL_INST_PROG"))
                    btnNext.Text = gpsRM.GetString("psNEXT")
                    btnSkip.Text = gpsRM.GetString("psSELECT")
                    btnNext.Enabled = True
                    btnSkip.Enabled = True
                    btnNext.Visible = True
                    btnSkip.Visible = True
                    cboZone.Visible = False
                Case 1
                    cboZone.Visible = False
                    'Run through action list.
                    For Each oTmp As Object In colActions
                        Try
                            Dim oAction As InstallAction = DirectCast(oTmp, InstallAction)
                            With oAction
                                Dim bEnable As Boolean = True
                                If (colZoneSelect IsNot Nothing) AndAlso (colZoneSelect.Count > 0) AndAlso _
                                   (.ZoneSelect IsNot Nothing) Then
                                    bEnable = False
                                    For Each oZone As String In .ZoneSelect
                                        If oZone = cboZone.Text Then
                                            bEnable = True
                                            Exit For
                                        End If
                                    Next
                                End If
                                If .EnableAction And bEnable Then
                                    Select Case .action
                                        Case "run"
                                            Try

                                                If .Parameter.GetUpperBound(0) > 0 Then
                                                    SetStatus(.Parameter(1))
                                                Else
                                                    SetStatus(String.Format(gpsRM.GetString("psINSTALLING"), .Parameter(0)))
                                                End If
                                                Shell(.Parameter(0), AppWinStyle.NormalFocus, True)
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try

                                        Case "subst"
                                            Try

                                                If .Parameter.GetUpperBound(0) > 1 Then
                                                    SetStatus(.Parameter(2))
                                                Else
                                                    SetStatus(String.Format(gpsRM.GetString("psINSTALLING"), .Parameter(0)))
                                                End If
                                                Dim allDrives() As IO.DriveInfo = IO.DriveInfo.GetDrives()
                                                Dim oDrive As IO.DriveInfo = allDrives(allDrives.GetUpperBound(0))
                                                Dim sTmpDrive As String = "D" 'Chr(Asc(oDrive.Name.Substring(0, 1)) + 1)
                                                Dim bOK As Boolean = False
                                                Do While bOK = False
                                                    bOK = True
                                                    For Each oDrive In allDrives
                                                        If sTmpDrive = oDrive.Name.Substring(0, 1) Then
                                                            bOK = False
                                                            sTmpDrive = Chr(Asc(sTmpDrive) + 1)
                                                            Exit For
                                                        End If
                                                    Next
                                                Loop
                                                Dim sCmd As String = "subst " & sTmpDrive & ": " & """" & .Parameter(0) & """"
                                                Debug.Print(sCmd)
                                                Shell(sCmd, AppWinStyle.MinimizedFocus, True)
                                                Shell(sTmpDrive & ":\" & .Parameter(1), AppWinStyle.NormalFocus, True)
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try

                                        Case "vbs", "msi"
                                            Try
                                                If .Parameter.GetUpperBound(0) > 0 Then
                                                    SetStatus(.Parameter(1))
                                                Else
                                                    SetStatus(String.Format(gpsRM.GetString("psINSTALLING"), .Parameter(0)))
                                                End If
                                                Shell("CMD /c""" & .Parameter(0) & """", AppWinStyle.NormalFocus, True)
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try

                                        Case "extract"
                                            Dim oZip As ZipFile = Nothing
                                            Try
                                                If IO.File.Exists(.Parameter(0)) Then
                                                    oZip = ZipFile.Read(.Parameter(0))
                                                    Dim sOutputPath As String = msPaintFolderDest
                                                    If .Parameter.GetUpperBound(0) > 0 Then
                                                        sOutputPath = .Parameter(1)
                                                    End If
                                                    If Not (IO.Directory.Exists(sOutputPath)) Then
                                                        IO.Directory.CreateDirectory(sOutputPath)
                                                    End If
                                                    SetStatus(String.Format(gpsRM.GetString("psEXTRACTING"), .Parameter(0), sOutputPath))
                                                    oZip.ExtractAll(sOutputPath, ExtractExistingFileAction.OverwriteSilently)
                                                End If
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try

                                        Case "copyfolder"
                                            Try
                                                SetStatus(String.Format(gpsRM.GetString("psCOPYING01"), .Parameter(0), .Parameter(1)))
                                                subCopyAll(.Parameter(0), .Parameter(1))
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try
                                        Case "copyfile"
                                            Try
                                                SetStatus(String.Format(gpsRM.GetString("psCOPYING01"), .Parameter(0), .Parameter(1)))
                                                My.Computer.FileSystem.CopyFile(.Parameter(0), .Parameter(1), True)
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try
                                        Case "regimport"
                                            Try
                                                SetStatus(String.Format(gpsRM.GetString("psREGIMPORTING"), .Parameter(0)))
                                                Shell("reg import """ & .Parameter(0) & """", AppWinStyle.NormalFocus, True)
                                            Catch ex As Exception
                                                SetStatus(ex.Message)
                                            End Try
                                    End Select
                                End If
                            End With
                        Catch ex As Exception
                            SetStatus(ex.Message)
                        End Try
                    Next
                    If bCheckFolderChoice(msDuplicatePaintFolder) Then
                        'Make a duplicate paint folder if it's required
                        'Create backup %1 folder at %2?
                        lblLabel.Text = String.Format(gpsRM.GetString("psLBL_DUPE"), _
                            msPaintFolderName, msDuplicatePaintFolder)
                        mnBigStep = 2
                        btnNext.Text = gpsRM.GetString("psSTART")
                        btnSkip.Text = gpsRM.GetString("psSKIP")
                        btnNext.Enabled = True
                        btnSkip.Enabled = True
                        btnNext.Visible = True
                        btnSkip.Visible = True
                    Else
                        mnBigStep = mnCOMPLETE
                        lblLabel.Text = gpsRM.GetString("psLBL_COMPLETE")
                        btnCancel.Visible = False
                        btnSkip.Visible = False
                        btnNext.Text = gpsRM.GetString("psFINISH")
                    End If
                Case 2
                    'Make duplicate paint folder
                    SetStatus(String.Format(gpsRM.GetString("psCREATINGDUPE"), msPaintFolderName, msDuplicatePaintFolder))
                    subCopyAll(msPaintFolderDest, msDuplicatePaintFolder)
                    'Go to the installation actions
                    mnBigStep = mnCOMPLETE
                    lblLabel.Text = gpsRM.GetString("psLBL_COMPLETE")
                    btnCancel.Visible = False
                    btnSkip.Visible = False
                    btnNext.Text = gpsRM.GetString("psFINISH")
                Case mnCOMPLETE
                    mbAbort = True

                    Dim fileWriter As System.IO.StreamWriter = Nothing
                    Try
                        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(msPaintFolderDest & gpsRM.GetString("psINSTALLLOG"), False)
                        For nItem As Integer = 1 To lstStatus.Items.Count - 1
                            Debug.Print(lstStatus.Items(nItem).ToString)
                            fileWriter.WriteLine(lstStatus.Items(nItem).ToString)
                        Next
                        fileWriter.Close()
                    Catch ex As Exception

                    End Try

                    Me.Close()
            End Select
        Catch ex As Exception
            SetStatus(ex.Message)
        End Try
        btnNext.Enabled = True
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description:  Trap Function Key presses 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.N, Keys.Space, Keys.Enter
                    If btnNext.Enabled Then
                        btnNext_Click()
                    End If
                Case Keys.Cancel, Keys.C, Keys.Escape
                    Me.Close()
                Case Else
            End Select
        End If
    End Sub

    Private Sub btnCancel_Click(Optional ByVal sender As System.Object = Nothing, Optional ByVal e As System.EventArgs = Nothing) Handles btnCancel.Click
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Close()
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (mnBigStep < mnCOMPLETE) And (mbAbort = False) Then
            mbAbort = bAbortInstall()
            e.Cancel = (mbAbort = False)
        End If
    End Sub
    Private Function bAbortInstall() As Boolean
        '********************************************************************************************
        'Description:  confirm before aborting the installation 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lRet As DialogResult

        lRet = MessageBox.Show(gpsRM.GetString("psABORTINSTALL_MSQ"), _
                            gpsRM.GetString("psABORTINSTALL_CAP"), _
                            MessageBoxButtons.YesNo, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

        Select Case lRet
            Case DialogResult.Yes
                Return True
            Case DialogResult.No
                Return False
            Case Else
                Return False
        End Select

    End Function

    Private Sub frmMain_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        '********************************************************************************************
        'Description:  Startup installation 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            mLanguage.GetResourceManagers(String.Empty, msBASE_ASSEMBLY_LOCAL, _
                                          String.Empty)
            btnNext.Text = gpsRM.GetString("psSTART")
            btnSkip.Text = gpsRM.GetString("psSKIP")
            btnCancel.Text = gpsRM.GetString("psCANCEL")
            Me.Text = gpsRM.GetString("psFORM")
            lblLabel.Text = String.Empty
            lstStatus.Text = String.Empty
            lstStatus.Items.Insert(0, gpsRM.GetString("psREADING_CONFIG"))
            mnuSelectAll.Text = gpsRM.GetString("psMNUSELECTALL")
            mnuUnSelectAll.Text = gpsRM.GetString("psMNUUNSELECTALL")
            'Title and robot pic.  This works in development and with a 4:3 screen.  It may
            'Need adjustement for widescreen
            'picFANUC.Width = CInt(Me.Width / 2.75)
            'picFANUC.Height = CInt(picFANUC.Width * 0.16)
            'picFANUC.Left = (Me.Width - picFANUC.Width) \ 2
            'picFANUC.Top = CInt(Me.Height * 0.025)

            'lblPAINTworks.AutoSize = True
            'lblPAINTworks.Font = New System.Drawing.Font("Tahoma", CSng(Me.Height * 0.068), System.Drawing.FontStyle.Bold)
            lblPAINTworks.Text = gpsRM.GetString("psPAINTWORKS_CAP")

            'lblPAINTworks.Top = picFANUC.Height + picFANUC.Top + 10
            'lblPAINTworks.Left = (Me.Width - lblPAINTworks.Width) \ 2

        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbAbort = True
        End Try
        Application.DoEvents()
        If mbAbort Then
            Exit Sub
        End If
        Dim sPath As String = String.Empty
        Try
            msRootFolder = Application.StartupPath & "\"
            Dim sSplit() As String = Split(msRootFolder, ":")
            If sSplit.GetUpperBound(0) = 1 Then
                msRootFolder = sSplit(0).ToUpper & ":" & sSplit(1)
            End If
            sPath = Application.StartupPath & "\" & "PWInstall.config"
            Dim sArg As String
            For Each s As String In My.Application.CommandLineArgs
                'If a culture string has been passed in, set the current culture (display language)
                sArg = "/culture="
                If s.ToLower.StartsWith(sArg) Then
                    Culture = s.Remove(0, sArg.Length)
                End If
                sArg = "/path="
                If s.ToLower.StartsWith(sArg) Then
                    Dim sTmp As String = s.Remove(0, sArg.Length)
                    If IO.Directory.Exists(sTmp) Then
                        If sTmp.Substring(sTmp.Length - 1, 1) <> "\" Then
                            sTmp = sTmp & "\"
                        End If
                        sTmp = sTmp & "PWInstall.config"
                    End If
                    If IO.File.Exists(sTmp) Then
                        sPath = sTmp
                        msRootFolder = sPath.Substring(0, sPath.Length - 16)
                        Dim sSplitRoot() As String = Split(msRootFolder, ":")
                        If sSplitRoot.GetUpperBound(0) = 1 Then
                            msRootFolder = sSplitRoot(0).ToUpper & ":" & sSplitRoot(1)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbAbort = True
        End Try
        Application.DoEvents()
        If mbAbort Then
            Exit Sub
        End If
        Dim sr As System.IO.StreamReader = Nothing
        Dim sZoneSelect() As String
        Try
            sr = System.IO.File.OpenText(sPath)

            Do While sr.Peek() >= 0
                Try
                    Dim sLine As String = sr.ReadLine()
                    If sLine.Substring(0, 1) <> "!" Then
                        If sLine.Substring(0, 1) = "#" Then
                            Dim sZoneSplit() As String = Split(sLine, "#")
                            sZoneSelect = Split(sZoneSplit(1), ",")
                            sLine = sZoneSplit(sZoneSplit.GetUpperBound(0))
                        Else
                            sZoneSelect = Nothing
                        End If
                        Dim sLineSplit() As String = Split(sLine, ",")
                        Select Case sLineSplit(0).ToLower
                            Case "zones"

                                For nItem As Integer = 1 To sLineSplit.GetUpperBound(0)
                                    cboZone.Items.Add(sLineSplit(nItem))
                                    Dim oZoneSelect As New tZoneSelect
                                    oZoneSelect.ZoneName = sLineSplit(nItem)
                                    colZoneSelect.Add(oZoneSelect)
                                Next
                                cboZone.Visible = True
                                cboZone.Enabled = True
                            Case "paintfoldersource"
                                msPaintFolderSource = sLineSplit(1)
                            Case "paintfolderdest"
                                msPaintFolderDest = sLineSplit(1)
                            Case "robots"
                                Dim nCount As Integer = sLineSplit.GetUpperBound(0) - 1
                                ReDim msRobots(nCount)
                                For nItem As Integer = 0 To nCount
                                    msRobots(nItem) = sLineSplit(nItem + 1)
                                Next
                                If sZoneSelect IsNot Nothing AndAlso (colZoneSelect.Count > 0) Then
                                    For Each oZone As tZoneSelect In colZoneSelect
                                        For Each sZoneName As String In sZoneSelect
                                            If sZoneName = oZone.ZoneName Then
                                                ReDim oZone.RobotNames(msRobots.GetUpperBound(0))
                                                For nItem As Integer = 0 To msRobots.GetUpperBound(0)
                                                    oZone.RobotNames(nItem) = msRobots(nItem)
                                                Next
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            Case "makefolders"
                                Dim nCount As Integer = sLineSplit.GetUpperBound(0) - 1
                                ReDim msMakeFolders(nCount)
                                For nItem As Integer = 0 To nCount
                                    msMakeFolders(nItem) = sLineSplit(nItem + 1)
                                Next
                            Case "autobackupfolder"
                                msAutoBackupFolder = sLineSplit(1)
                            Case "DuplicatePaintFolder"
                                msDuplicatePaintFolder = sLineSplit(1)
                            Case "robotbackupfolders"
                                Dim nCount As Integer = sLineSplit.GetUpperBound(0) - 1
                                ReDim msRobotBackupFolders(nCount)
                                For nItem As Integer = 0 To nCount
                                    msRobotBackupFolders(nItem) = sLineSplit(nItem + 1)
                                Next
                            Case "run", "vbs", "msi", "extract", "copyfolder", "copyfile", "regimport", "subst"
                                Dim oAction As New InstallAction
                                oAction.action = sLineSplit(0).ToLower
                                oAction.EnableAction = True
                                Dim nCount As Integer = sLineSplit.GetUpperBound(0) - 1
                                ReDim oAction.Parameter(nCount)
                                For nItem As Integer = 0 To nCount
                                    oAction.Parameter(nItem) = Environment.ExpandEnvironmentVariables(sLineSplit(nItem + 1))
                                    If oAction.Parameter(nItem).StartsWith(".\") Then
                                        oAction.Parameter(nItem) = oAction.Parameter(nItem).Replace(".\", msRootFolder)
                                    End If
                                Next
                                If sZoneSelect Is Nothing Then
                                    oAction.ZoneSelect = Nothing
                                Else
                                    nCount = sZoneSelect.GetUpperBound(0)
                                    ReDim oAction.ZoneSelect(nCount)
                                    For nItem As Integer = 0 To nCount
                                        oAction.ZoneSelect(nItem) = sZoneSelect(nItem)
                                    Next
                                End If
                                colActions.Add(oAction)
                            Case Else
                                '?
                        End Select
                    End If
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    mbAbort = True
                End Try
            Loop
            sr.Close()
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbAbort = True
        End Try
        Application.DoEvents()
        If mbAbort Then
            Exit Sub
        End If
        Try
            SetStatus(gpsRM.GetString("psCHECKFOLDERS"))
            'First step, confirm folder info is OK
            'Convert any ".\..." 
            If bCheckFolderChoice(msPaintFolderSource) Then
                If InStr(msPaintFolderSource.ToLower, "sealer") > 0 Then
                    msPaintFolderName = "Sealer"
                End If
                If bCheckFolderChoice(msPaintFolderDest) Then
                    'The %1 folder will be copied from %2 to %3.
                    lblLabel.Text = String.Format(gpsRM.GetString("psLBL_PNTFOLDER"), _
                        msPaintFolderName, msPaintFolderSource, msPaintFolderDest)
                    mnLittleStep = 1
                Else
                    MessageBox.Show(gpsRM.GetString("psERROR"), _
                                String.Format(gpsRM.GetString("psINVALID_DEST"), msPaintFolderDest), _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                    mbAbort = True
                End If
            Else
                MessageBox.Show(gpsRM.GetString("psERROR"), _
                                String.Format(gpsRM.GetString("psINVALID_SOURCE"), msPaintFolderSource), _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                mbAbort = True
            End If
            btnNext.Text = gpsRM.GetString("psSTART")
            btnSkip.Text = gpsRM.GetString("psSKIP")
            If cboZone.Visible Then
                btnNext.Enabled = False
                btnSkip.Enabled = False
            Else
                btnNext.Enabled = True
                btnSkip.Enabled = True
            End If
            btnNext.Visible = True
            btnSkip.Visible = True
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psERROR"), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbAbort = True
        End Try
        mnBigStep = 0
        mnLittleStep = 0
        If mbAbort Then
            mnBigStep = mnCOMPLETE
        End If
    End Sub


    Private Sub mnuUnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnSelectAll.Click
        For nItem As Integer = 0 To clbActions.Items.Count - 1
            clbActions.SetItemChecked(nItem, False)
        Next
    End Sub

    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        For nItem As Integer = 0 To clbActions.Items.Count - 1
            clbActions.SetItemChecked(nItem, True)
        Next
    End Sub

    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboZone.SelectedIndexChanged
        If cboZone.Text <> String.Empty Then
            btnNext.Enabled = True
            btnSkip.Enabled = True
            msZone = cboZone.Text
            For Each oZone As tZoneSelect In colZoneSelect
                If cboZone.Text = oZone.ZoneName Then
                    ReDim msRobots(oZone.RobotNames.GetUpperBound(0))
                    For nItem As Integer = 0 To oZone.RobotNames.GetUpperBound(0)
                        msRobots(nItem) = oZone.RobotNames(nItem)
                    Next
                    Exit For
                End If
            Next
            Debug.Print(msZone)
        End If
    End Sub

End Class