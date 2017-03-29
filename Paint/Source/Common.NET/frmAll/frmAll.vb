' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmall
'
' Description: Display and compare data
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    11/05/09	  MSW	    CCType, opener properties
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    01/06/12   MSW     subShowNewPage - fix a spot where odg and dg1 were mixed up,  4.01.01.01
'												it threw errors if the added table was bigger than the first one
'    02/15/12   MSW     Print error and cancel handling                               4.01.01.02
'    04/11/12   MSW     Change byval to byref                                         4.01.03.00
'    04/16/13   MSW     Standalone ChangeLog form, Isolate DB,SQL dependencies        4.01.05.00
'                       Consolidate eParamType in mDeclares
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up 4.01.05.01
'    09/30/13   MSW     Use the control's min and max instead of literal constants    4.01.05.02
'    01/06/14   MSW     Disable the form controlbox                                   4.01.06.00
'*******************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports Response = System.Windows.Forms.DialogResult
Imports System.Configuration.ConfigurationSettings
'

Friend Class frmAll

#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const msSCREEN_NAME As String = "MultiView"   ' <-- For password area change log etc.
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMultiView"
    Private msALL As String = gcsRM.GetString("csALL")
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Private msOldRobot As String = String.Empty
    Private msOldParam As String = String.Empty
    Private msOldSubParam As String = String.Empty
    Private msOldZone As String = String.Empty
    Private colZones As clsZones = Nothing
    Private mDataSet As DataSet = New DataSet
    Private mParamsForLookup() As String
    Private mnDataSets As Integer = 0
    Private mnRowHeaderWidth As Integer = 0
    Private mnColWidth As Integer = 0
    Private mArm As clsArm = Nothing
    Private Const mnMAX_GRID As Integer = 8
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private msZone As String = msALL
    Private msScreenName As String
    Private mbShowDevCombo As Boolean = False
    Private mbShowParamCombo As Boolean = False
    Private mbShowSubParamCombo As Boolean = False
    Friend WithEvents imlToolBar As System.Windows.Forms.ImageList
    Private msDatabasepath As String = String.Empty
    Private mbByArm As Boolean = True
    Private mnParamType As eParamType = eParamType.Colors
    Private msParamName As String = String.Empty
    Private msSubParamName As String = String.Empty
    Private mbResetSubParamOnNewParam As Boolean = True
    Private mbUseOneGrid As Boolean = True
    Private mnColsPerDataSet() As Integer
    Private mCCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private mCCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private mbIncludeOpeners As Boolean = True
    Private mPrintHtml As clsPrintHtml
    '******** End Property Variable *************************************************************


    'Public Enum eParamType
    '    None
    '    Colors
    '    Valves
    'End Enum

#End Region
#Region " Properties "
    Friend Property IncludeOpeners() As Boolean
        '********************************************************************************************
        'Description:  Include openers in robot list
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbIncludeOpeners
        End Get
        Set(ByVal Value As Boolean)
            mbIncludeOpeners = Value
        End Set

    End Property

    Friend Property CCType1() As eColorChangeType
        '********************************************************************************************
        'Description:  provide the ability to filter robots by CC type
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCCType1
        End Get
        Set(ByVal value As eColorChangeType)
            mCCType1 = value
        End Set
    End Property
    Friend Property CCType2() As eColorChangeType
        '********************************************************************************************
        'Description:  provide the ability to filter robots by CC type
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCCType2
        End Get
        Set(ByVal value As eColorChangeType)
            mCCType2 = value
        End Set
    End Property
    Friend Property UseOneGrid() As Boolean
        '********************************************************************************************
        'Description:  Reset subparam (last cbo) when param (2nd to last) changes
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseOneGrid
        End Get
        Set(ByVal Value As Boolean)
            mbUseOneGrid = Value
        End Set

    End Property
    Friend Property ByArm() As Boolean
        '********************************************************************************************
        'Description:  Select arm names instead of controllers
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbByArm
        End Get
        Set(ByVal value As Boolean)
            mbByArm = value
        End Set
    End Property
    Friend Property DatabasePath() As String
        '********************************************************************************************
        'Description: Path to database
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            If msDatabasepath = String.Empty Then
                GetDefaultFilePath(msDatabasepath, eDir.Database, String.Empty, String.Empty)
            End If
            Return msDatabasepath

        End Get
        Set(ByVal value As String)

            If Strings.Right(value, 1) <> "\" Then
                value = value & "\"
            End If
            msDatabasepath = value

        End Set
    End Property
    Friend Property SubParamName() As String
        '********************************************************************************************
        'Description:  Use the first param (usually color)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msSubParamName
        End Get
        Set(ByVal Value As String)
            msSubParamName = Value
            lblSubParam.Text = msSubParamName
        End Set
    End Property
    Friend Property ParamName() As String
        '********************************************************************************************
        'Description:  Use the first param (usually color)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msParamName
        End Get
        Set(ByVal Value As String)
            msParamName = Value
            lblParam.Text = msParamName
        End Set
    End Property
    Friend Property DataLoaded() As Boolean
        '********************************************************************************************
        'Description:  Data loaded flag for form
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            'if not loaded disable all controls
            subEnableControls(Value)
        End Set

    End Property
    Friend Property ResetSubParamOnNewParam() As Boolean
        '********************************************************************************************
        'Description:  Reset subparam (last cbo) when param (2nd to last) changes
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbResetSubParamOnNewParam
        End Get
        Set(ByVal Value As Boolean)
            mbResetSubParamOnNewParam = Value
        End Set

    End Property

    Friend WriteOnly Property InitialZone() As String
        '********************************************************************************************
        'Description:  What zone
        '
        'Parameters: zonename
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As String)
            msZone = Value
        End Set
    End Property
    Friend Property Progress() As Integer
        '********************************************************************************************
        'Description:  run the progress bar
        '
        'Parameters: 1 to 100 percent
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        ' 09/30/13  MSW     Use the control's min and max instead of literal constants
        '********************************************************************************************
        Set(ByVal Value As Integer)
            Try
                If Value < tspProgress.Minimum Then Value = tspProgress.Minimum
                If Value > tspProgress.Maximum Then Value = tspProgress.Minimum
                mnProgress = Value
                tspProgress.Value = mnProgress
                If mnProgress > 0 And mnProgress < 100 Then
                    lblSpacer.Width = gtSSSize.SpaceLabelInvisSize
                    tspProgress.Visible = True
                Else
                    lblSpacer.Width = gtSSSize.SpaceLabelVisbleSize
                    tspProgress.Visible = False
                End If
                stsStatus.Invalidate()
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Progress", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
        Get
            Return mnProgress
        End Get
    End Property
    Friend Property ScreenName() As String
        '********************************************************************************************
        'Description:  allow frmMain to pass in it's screen name (msSCREEN_NAME)
        '
        'Parameters: none
        'Returns:    The screen name associated with frmMain
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msScreenName
        End Get
        Set(ByVal value As String)
            msScreenName = value
            subFormatScreenLayout()
        End Set
    End Property
    Friend Property ShowDeviceCombo() As Boolean
        '********************************************************************************************
        'Description:  allow frmMain to pass in whether a Robot Select box should be shown (mbShowDevCombo)
        '
        'Parameters: none
        'Returns:    True if Robot Select box should be shown
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbShowDevCombo
        End Get
        Set(ByVal value As Boolean)
            mbShowDevCombo = value
            lblRobot.Visible = value
            cboRobot.Visible = value
        End Set
    End Property
    Friend Property ShowParamCombo() As Boolean
        '********************************************************************************************
        'Description:  allow frmMain to pass in whether a parameter Select box should be shown 
        '
        'Parameters: none
        'Returns:    True if parameter Select box should be shown
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbShowParamCombo
        End Get
        Set(ByVal value As Boolean)
            mbShowParamCombo = value
            lblParam.Visible = value
            cboParam.Visible = value
        End Set
    End Property
    Friend WriteOnly Property ParameterType() As eParamType
        '********************************************************************************************
        'Description:  sys color or valve
        '
        'Parameters: Parametername
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As eParamType)
            mnParamType = value
            Select Case value
                Case eParamType.None
                    mbShowParamCombo = False
                Case eParamType.Colors
                    mbShowParamCombo = True
                Case eParamType.Valves
                    mbShowParamCombo = True
            End Select
        End Set
    End Property

    Friend Property ShowSubParamCombo() As Boolean
        '********************************************************************************************
        'Description:  allow frmMain to pass in whether a subparameter Select box should be shown 
        '
        'Parameters: none
        'Returns:    True if subparameter Select box should be shown
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbShowSubParamCombo
        End Get
        Set(ByVal value As Boolean)
            mbShowSubParamCombo = value
            lblSubParam.Visible = value
            cboSubParam.Visible = value
        End Set
    End Property
    Friend Property Status(Optional ByVal StatusStripOnly As Boolean = False) As String
        '********************************************************************************************
        'Description:  write status messages to listbox and statusbar
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
		' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        '********************************************************************************************
        Get
            Return stsStatus.Text
        End Get
        Set(ByVal Value As String)
            Try
                If StatusStripOnly = False Then
                    mPWCommon.AddToStatusBox(lstStatus, Value)
                End If
                stsStatus.Items("lblStatus").Text = Strings.Replace(Value, vbTab, "  ")
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
#End Region
#Region " Routines "

    Private Sub subChangeZone()
        '********************************************************************************************
        'Description: New Zone Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Progress = 10

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Items.Count <> 0 Then
                If cboZone.Text = colZones.CurrentZone Then Exit Sub
            End If
        End If

        If colZones.SetCurrentZone(cboZone.Text) = False Then
            If msOldZone <> String.Empty Then
                cboZone.Text = msOldZone
            Else
                cboZone.SelectedIndex = -1
            End If

            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

            Exit Sub
        End If


        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            subFormatScreenLayout()


            If mbShowDevCombo Then
                LoadRobotBoxFromDB(cboRobot, colZones.ActiveZone, Not (ByArm), False, mCCType1, mCCType2, mbIncludeOpeners)
                cboParam.Items.Clear()
                msOldParam = String.Empty
                msOldRobot = String.Empty
                msOldSubParam = String.Empty
                'statusbar text
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            Else
                btnAdd.Enabled = (mnDataSets < mnMAX_GRID) Or mbUseOneGrid
            End If  'mbShowDevCombo

            msOldZone = cboZone.Text

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  Clear out textboxes reset colors etc. here
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subEnableControls(False)
        DataLoaded = False

    End Sub
    Private Sub subClearGrid(ByRef odg As DataGridView)
        '********************************************************************************************
        'Description:  Clear out Grid
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With odg
            .ColumnCount = 0
            .RowCount = 0
        End With
        Dim olbl As Label
        For nCol As Integer = 0 To mnMAX_GRID - 1
            If (nCol > mnDataSets) Or ((nCol > 0) And (DataLoaded = False)) Then
                tlpMain.ColumnStyles(nCol).Width = 0
            End If
            If (nCol > mnDataSets) Then
                olbl = DirectCast(tlpMain.Controls("lblTable" & (nCol + 1).ToString), Label)
                olbl.Text = String.Empty
            End If
        Next
        If UseOneGrid Then
            tlpMain.RowStyles(0).Height = 0
        Else
            tlpMain.RowStyles(0).Height = 20
        End If
    End Sub
    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        btnClose.Enabled = True

        If bEnable = False Then
            btnPrint.Enabled = False
            btnStatus.Enabled = True
            pnlMain.Enabled = False
            btnAdd.Enabled = False
            btnClear.Enabled = False
        Else
            btnPrint.Enabled = (True And (mnDataSets > 0))
            btnStatus.Enabled = True
            pnlMain.Enabled = True
            If (mnDataSets < mnMAX_GRID) Or mbUseOneGrid Then
                If cboZone.Text <> String.Empty Then
                    If mbShowDevCombo Then
                        If cboRobot.Text <> String.Empty Then
                            If mbShowParamCombo Then
                                If cboParam.Text <> String.Empty Or (cboParam.Items.Count = 0) Then
                                    If mbShowSubParamCombo Then
                                        If cboSubParam.Text <> String.Empty Then
                                            btnAdd.Enabled = True
                                        End If
                                    Else
                                        btnAdd.Enabled = True
                                    End If ' mbShowSubParamCombo
                                End If ' cboParam.Text <> String.Empty 
                            Else
                                btnAdd.Enabled = True
                            End If ' mbShowParamCombo
                        End If ' cboRobot.Text <> String.Empty
                    Else
                        btnAdd.Enabled = True
                    End If ' mbShowDevCombo
                End If
            Else
                btnAdd.Enabled = False
            End If
            Select Case frmMain.Privilege
                Case ePrivilege.None
                    mnuPrintFile.Enabled = False
                Case Else
                    mnuPrintFile.Enabled = (True And (mnDataSets > 0))
            End Select
            btnClear.Enabled = (True And (mnDataSets > 0))
        End If

    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description:  set up the screen layout When adding a new screen manual setup goes here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        cboParam.Visible = mbShowParamCombo
        Dim olbl As Label
        For nCol As Integer = 0 To mnMAX_GRID - 1
            If nCol > 0 Then
                tlpMain.ColumnStyles(nCol).Width = 0
            End If
            olbl = DirectCast(tlpMain.Controls("lblTable" & (nCol + 1).ToString), Label)
            olbl.Text = String.Empty
        Next
        If UseOneGrid Then
            tlpMain.RowStyles(0).Height = 0
        Else
            tlpMain.RowStyles(0).Height = 20
        End If
        Select Case ScreenName
            Case "Presets"
                lblRobot.Visible = True
                cboRobot.Visible = True
                lblParam.Visible = mbShowParamCombo
                mnRowHeaderWidth = 60
                mnColWidth = 50
            Case "ColorChange"
                lblRobot.Visible = True
                cboRobot.Visible = True
                lblParam.Visible = mbShowParamCombo
                cboParam.Visible = mbShowParamCombo

                mnRowHeaderWidth = 100
                mnColWidth = 40
            Case "Calibration"
                lblRobot.Visible = True
                cboRobot.Visible = True
                lblParam.Visible = mbShowParamCombo
                cboParam.Visible = mbShowParamCombo
                mnRowHeaderWidth = 100
                mnColWidth = 100
            Case Else
                lblRobot.Visible = True
                cboRobot.Visible = True
                lblParam.Visible = mbShowParamCombo
                cboParam.Visible = mbShowParamCombo

                mnRowHeaderWidth = 100
                mnColWidth = 40
        End Select

    End Sub
    Private Sub subFormatGrid(ByRef rTable As DataTable, ByRef odg As DataGridView)
        '********************************************************************************************
        'Description:  set up the Grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subClearGrid(odg)

        With odg
            .SuspendLayout()

            Dim nOffset As Integer
            If mbUseOneGrid Then
                nOffset = 1
            Else
                nOffset = 0
            End If

            .ColumnHeadersVisible = True
            .RowHeadersVisible = True
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            .RowCount = rTable.Rows.Count + nOffset

            Dim nRowInTable As Integer
            ' load row header text
            For nRowInTable = 0 To rTable.Rows.Count - 1
                .Rows(nRowInTable + nOffset).HeaderCell.Value = rTable.Rows(nRowInTable).Item(gsALL_INDEX)
            Next
            .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
            .RowHeadersWidth = mnRowHeaderWidth

            .Rows(0).Cells(0).Style.WrapMode = DataGridViewTriState.True

            .ResumeLayout()
        End With

    End Sub
    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False

            Progress = 5

            colZones = New clsZones(DatabasePath)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)

            ' this is after initializeform to overwrite version number
            Me.Text = gcsRM.GetString("csVIEWALL_FORM")
            'other screen specific
            If msParamName <> String.Empty Then
                lblParam.Text = msParamName
            Else
                lblParam.Text = gcsRM.GetString("csCOLOR_CAP")
            End If


            If msSubParamName <> String.Empty Then
                lblSubParam.Text = msSubParamName
            Else
                lblSubParam.Text = gcsRM.GetString("csPARAMETER_CAP")
            End If
            Progress = 98


            mPrintHtml = New clsPrintHtml(frmMain.msSCREEN_NAME & "_" & msSCREEN_NAME, True, 35)
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound
            'for statusbox
            Status = gcsRM.GetString("csDONE")

            'statusbar text
            If msZone <> String.Empty Then
                cboZone.Text = msZone
            End If
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            Else
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            End If

            lblWidth.Text = gcsRM.GetString("csWIDTH")
            chkSyncScroll.Text = gcsRM.GetString("csSYNCSCROLL")

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            lReply = ShowErrorMessagebox(gcsRM.GetString("ERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.AbortRetryIgnore)

            Select Case lReply
                Case Response.Abort
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    End
                Case Response.Ignore
                Case Response.Retry
                    subInitializeForm()
            End Select
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subLoadData()
        '********************************************************************************************
        'Description:  Data Load Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        DataLoaded = False

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA") & vbTab & _
                                                cboRobot.Text & ";" & vbTab & _
                                                cboParam.Text & ";"
            Status = sTmp
            Progress = 20

            '>>>>>> This is a generic call - routine needs to be provided in screen specific module
            Dim DS As DataSet = GetViewAllData(cboZone.Text, cboRobot.Text, cboParam, _
                                                cboSubParam, colZones.DatabasePath)

            Progress = 80
            If DS Is Nothing Then
                Status = gcsRM.GetString("csERROR")
                Exit Try
            End If



            'get table - should be only one returned
            Dim dtTmp As New DataTable
            dtTmp = DS.Tables(0)

            If dtTmp Is Nothing Then
                Status = gcsRM.GetString("csERROR")
                Exit Try
            End If

            'Track columns per data set for printing
            ReDim Preserve mnColsPerDataSet(mnDataSets + 1)

            subShowNewPage(dtTmp)

            ' increment counter
            mnDataSets += 1

            DataLoaded = True
            Status = gcsRM.GetString("csLOADDONE")


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            subEnableControls(False)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try
    End Sub
    Private Sub subChangeParameter()
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        'just in case
        If cboParam.Text = String.Empty Then
            Exit Sub
        Else
            If cboParam.Text = msOldParam Then Exit Sub
        End If
        msOldParam = cboParam.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            If mbShowSubParamCombo = False Then
                'enable add button
                btnAdd.Enabled = (mnDataSets < mnMAX_GRID) Or mbUseOneGrid
            Else
                If mbResetSubParamOnNewParam Then
                    msOldSubParam = String.Empty
                    btnAdd.Enabled = False
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    LoadMultiScreenSubParameterBox(cboSubParam, colZones, cboParam.Text, False)
                    If cboSubParam.Items.Count = 1 Then
                        cboSubParam.SelectedIndex = 0
                    End If
                End If
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subChangeSubParameter()
        '********************************************************************************************
        'Description:  New subParameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        'just in case
        If cboSubParam.Text = String.Empty Then
            Exit Sub
        End If
        msOldSubParam = cboSubParam.Text
        'enable add button
        btnAdd.Enabled = (mnDataSets < mnMAX_GRID) Or mbUseOneGrid

    End Sub
    'Private Sub subBuildOneTable(ByVal nStartGrid As Integer, ByVal nEndGrid As Integer, _
    '                             ByRef rPrint As clsPrint, ByRef sGroupHeaderTextString As String)
    '    '********************************************************************************************
    '    'Description:  Data Print Routine - print one table
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    Try
    '        Dim nGrid As Integer
    '        Dim nCol As Integer
    '        Dim odg As DataGridView
    '        Dim olbl As Label
    '        Dim sHeader As String = String.Empty
    '        sHeader = " " & vbTab 'Add index column

    '        sGroupHeaderTextString = " " & vbTab 'Add index column
    '        For nGrid = nStartGrid To nEndGrid
    '            odg = DirectCast(tlpMain.Controls("dg" & (nGrid).ToString), DataGridView)
    '            For nCol = 0 To odg.Columns.Count - 1
    '                If odg.Columns(nCol).HeaderText = String.Empty Then
    '                    sHeader = sHeader & " "
    '                Else
    '                    sHeader = sHeader & odg.Columns(nCol).HeaderText
    '                End If
    '                If nCol < (odg.Columns.Count - 1) Or (nGrid < nEndGrid) Then
    '                    sHeader = sHeader & vbTab
    '                End If
    '            Next
    '            If Not (mbUseOneGrid) Then
    '                ' Multiple grids, add headings from labels
    '                olbl = DirectCast(tlpMain.Controls("lblTable" & (nGrid).ToString), Label)
    '                If olbl.Text = String.Empty Then
    '                    sGroupHeaderTextString = sGroupHeaderTextString & " "
    '                Else
    '                    sGroupHeaderTextString = sGroupHeaderTextString & olbl.Text
    '                End If
    '                If nEndGrid > nGrid Then
    '                    sGroupHeaderTextString = sGroupHeaderTextString & vbTab
    '                End If
    '            Else 'labels in the grid, do it the hard way
    '                'If olbl.Text = String.Empty Then
    '                '    sGroupHeaderTextString = sGroupHeaderTextString & " "
    '                'Else
    '                '    sGroupHeaderTextString = sGroupHeaderTextString & olbl.Text
    '                'End If
    '            End If
    '        Next

    '        rPrint.ColumnHeadersString = sHeader

    '        Dim nRow As Integer

    '        For nRow = 0 To dg1.Rows.Count - 1

    '            Dim sText As String = String.Empty
    '            If nRow > 0 Then
    '                sText = dg1.Rows(nRow).HeaderCell.Value.ToString & vbTab ' nRow.ToString & vbTab 'Add index column
    '            Else
    '                sText = " " & vbTab 'Add index column
    '            End If
    '            For nGrid = nStartGrid To nEndGrid
    '                odg = DirectCast(tlpMain.Controls("dg" & (nGrid).ToString), DataGridView)

    '                For nCol = 0 To odg.Columns.Count - 1
    '                    If odg.Rows(nRow).Cells(nCol).Value Is Nothing Then
    '                        sText = sText & " "
    '                    Else
    '                        sText = sText & odg.Rows(nRow).Cells(nCol).Value.ToString
    '                    End If
    '                    If nCol < (odg.Columns.Count - 1) Or (nGrid < nEndGrid) Then
    '                        sText = sText & vbTab
    '                    End If
    '                Next
    '            Next 'nRow
    '            rPrint.BodyTextStrings = sText
    '        Next 'nGrid
    '    Catch ex As Exception
    '        Trace.WriteLine(ex.Message)
    '        Trace.WriteLine(ex.StackTrace)

    '        ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
    '                            Status, MessageBoxButtons.OK)

    '        Status = gcsRM.GetString("csPRINTFAILED")

    '    Finally
    '        Progress = 100
    '        Me.Cursor = System.Windows.Forms.Cursors.Default
    '        btnPrint.Enabled = True
    '        btnClose.Enabled = True

    '    End Try
    'End Sub
    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description:  show page setup dialog 
        '
        'Parameters: none
        'Returns:    Print settings to use in printing
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dg1 IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPageSetup()
            End If
        End If
    End Sub

    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuPrint.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dg1 IsNot Nothing) AndAlso DataLoaded Then
            bPrintdoc(True)
        End If
    End Sub
    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dg1 IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subSaveAs()
            End If
        End If

    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dg1 IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If
    End Sub
    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description:  offer options for printout table setup.
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPrintHtml.subShowOptions()
    End Sub
    Private Function bPrintdoc(ByVal bPrint As Boolean) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
       ' 02/15/12  MSW     Print error and cancel handling'
        '********************************************************************************************
        Try
            Dim bCancel as boolean = false
            Dim sRedFormat(1) As String
            sRedFormat(0) = "<b><i>"
            sRedFormat(1) = "</i></b>"
            If UseOneGrid Then

                mPrintHtml.subSetPageFormat()
                mPrintHtml.subClearTableFormat()
                mPrintHtml.ReformatRed = sRedFormat
                mPrintHtml.subSetColumnCfg("align=right", 0)
                mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
                mPrintHtml.HeaderRowsPerTable = 2
                Dim sTitle As String = colZones.SiteName & "  " & colZones.CurrentZone
                mPrintHtml.subCreateSimpleDoc(dg1, Status, gpsRM.GetString("psSCREEN_NAME") & " - " & msSCREEN_NAME)
            Else
                'More than one grid on the screen.
                mPrintHtml.subStartDoc(Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msSCREEN_NAME, false, bCancel)
                if Not(bCancel) then
                  For nGrid As Integer = 1 To mnDataSets
                      Dim olbl As Label = DirectCast(tlpMain.Controls("lblTable" & (nGrid).ToString), Label)
                      Dim odg As DataGridView = DirectCast(tlpMain.Controls("dg" & (nGrid).ToString), DataGridView)
                      If (odg IsNot Nothing) Then
                          mPrintHtml.subSetPageFormat()
                          mPrintHtml.subClearTableFormat()
                          mPrintHtml.ReformatRed = sRedFormat
                          mPrintHtml.subSetColumnCfg("align=right", 0)
                          mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
                          mPrintHtml.HeaderRowsPerTable = 1
                          Dim sTitle(0) As String
                          sTitle(0) = olbl.Text
                          mPrintHtml.subAddObject(odg, Status, sTitle)
                      End If
                  Next
                  mPrintHtml.subCloseFile(Status)
                end if
            End If
            If bPrint Then
                mPrintHtml.subPrintDoc()
            End If
            Return (Not(bCancel))
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Status = gcsRM.GetString("csPRINTFAILED")
            Return (False)

        End Try

    End Function
    Private Sub subChangeRobot()
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/17/10  MSW     Enable btnAdd here for cc presets (by controller only)
        '********************************************************************************************

        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5


            ' if there is no parameter combo, load data here
            GetViewAllRobot(mArm, cboZone.Text, cboRobot.Text, DatabasePath)
            If mbShowParamCombo = False Then
                If mbShowSubParamCombo Then
                    If (cboSubParam.Items.Count = 0) Or mbResetSubParamOnNewParam Then 'RJO 01/08/10 If Not (mbResetSubParamOnNewParam) Then
                        msOldSubParam = String.Empty
                        btnAdd.Enabled = False
                        '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                        LoadMultiScreenSubParameterBox(cboSubParam, colZones, cboRobot.Text, False)
                        If cboSubParam.Items.Count = 1 Then
                            cboSubParam.SelectedIndex = 0
                        End If
                    Else
                        btnAdd.Enabled = (mnDataSets < mnMAX_GRID) Or mbUseOneGrid
                    End If
                Else
                    subEnableControls(True) 'Enable btnAdd here for cc presets (by controller only)
                End If
            Else 'load parameter box
                If (cboParam.Items.Count = 0) Or mbResetSubParamOnNewParam Then 'RJO 01/08/10
                    btnAdd.Enabled = False
                    msOldParam = String.Empty
                    'msOldSubParam = String.Empty 'RJO 01/08/10
                    Select Case mnParamType
                        Case eParamType.Colors
                            mSysColorCommon.LoadColorBoxFromCollection(mArm.SystemColors, cboParam)
                        Case eParamType.Valves
                            mSysColorCommon.LoadValveBoxFromCollection(mArm.SystemColors, cboParam)
                    End Select
                End If

                If (cboSubParam.Items.Count = 0) Or mbResetSubParamOnNewParam Then 'RJO 01/08/10 If Not (mbResetSubParamOnNewParam) Then
                    msOldSubParam = String.Empty
                    btnAdd.Enabled = False
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    LoadMultiScreenSubParameterBox(cboSubParam, colZones, cboRobot.Text, False)
                    If cboSubParam.Items.Count = 1 Then
                        cboSubParam.SelectedIndex = 0
                    End If
                End If
            End If

                Status(True) = gcsRM.GetString("csSELECT_COLOR")

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
        End Try


    End Sub
    Private Sub subShowNewPage(ByRef rDataTable As DataTable)
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: nozne
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/12  MSW     fix a spot where odg and dg1 were mixed up, it threw errors
        '                   if the added table was bigger than the first one
        '********************************************************************************************

        Dim sName As String = rDataTable.TableName
        Dim odg As DataGridView
        If UseOneGrid Then
            odg = DirectCast(tlpMain.Controls("dg1"), DataGridView)
        Else
            odg = DirectCast(tlpMain.Controls("dg" & (mnDataSets + 1).ToString), DataGridView)
            AddHandler odg.Scroll, AddressOf dg_Scroll
            If mnDataSets > 0 Then
                'Enable a wider view if needed
                trkWidth.Visible = True
                lblWidth.Visible = True
                chkSyncScroll.Visible = True
                trkWidth.Maximum = (mnDataSets + 1) * trkWidth.Minimum
                trkWidth.Value = trkWidth.Minimum
                tlpMain.Width = (pnlMain.Width - 5)
            Else
                chkSyncScroll.Enabled = True
            End If
        End If

        Dim nOffset As Integer
        'If multiple grids, we're using labels instead of title row
        If mbUseOneGrid Then
            nOffset = 1
        Else
            nOffset = 0
        End If
        If mnDataSets = 0 Or UseOneGrid = False Then
            ' set up grid
            subFormatGrid(rDataTable, odg)
            For nCol As Integer = 0 To mnMAX_GRID - 1
                If nCol <= mnDataSets Then
                    tlpMain.ColumnStyles(nCol).Width = 25
                Else
                    tlpMain.ColumnStyles(nCol).Width = 0
                End If
            Next
        Else
            odg.Columns.Add(String.Empty, String.Empty)
            odg.Columns(odg.Columns.Count - 1).Width = 5
            odg.Columns.Add(rDataTable.Columns(1).Caption, rDataTable.Columns(1).Caption)
        End If

        Dim nRowInTable As Integer = 0
        Dim nColInTable As Integer = 0
        Dim nRowInGrid As Integer = 0
        Dim nColInGrid As Integer = 0

        If Not (UseOneGrid) Then
            'Seperate grids, use labels
            Dim olbl As Label
            olbl = DirectCast(tlpMain.Controls("lblTable" & (mnDataSets + 1).ToString), Label)
            olbl.Text = sName
        End If



        With odg
            .SuspendLayout()
            Dim sNames() As String = Split(sName, ";")
            ' the index field should always be item 0 (by design) ignore this when adding columns
            For nColInTable = 1 To rDataTable.Columns.Count - 1

                nColInGrid = .Columns.Count - 1

                .Columns(nColInGrid).Width = mnColWidth
                .Columns(nColInGrid).DefaultCellStyle.Alignment = _
                                    DataGridViewContentAlignment.MiddleCenter


                If mbUseOneGrid Then
                    If sNames.GetUpperBound(0) >= (rDataTable.Columns.Count - 2) Then
                        If nColInTable = 1 Then
                            .Columns(nColInGrid).HeaderText = sName
                        Else
                            .Columns(nColInGrid).HeaderText = String.Empty
                        End If
                    Else
                        If sNames.GetUpperBound(0) >= (nColInTable - 1) Then
                            If nColInTable = 1 Then
                                .Columns(nColInGrid).HeaderCell.Style.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold)
                            End If
                            .Columns(nColInGrid).HeaderText = sNames(nColInTable - 1)
                        Else
                            .Columns(nColInGrid).HeaderText = String.Empty
                        End If
                    End If
                    .Rows(0).Cells(nColInGrid).Style.WrapMode = DataGridViewTriState.True
                    .Rows(0).Cells(nColInGrid).Value = rDataTable.Columns(nColInTable).Caption
                Else
                    .Columns(nColInGrid).HeaderText = rDataTable.Columns(nColInTable).Caption
                End If

                If (.Rows.Count - nOffset) < rDataTable.Rows.Count Then
                    Dim i%
                    For i% = (.Rows.Count + nOffset - 1) To rDataTable.Rows.Count
                        .Rows.Add()
                        .Rows(i%).HeaderCell.Value = rDataTable.Rows(i% - 1).Item(gsALL_INDEX)
                    Next
                End If

                'cells are filled here **************************
                For nRowInTable = 0 To rDataTable.Rows.Count - 1

                    nRowInGrid = nRowInTable + nOffset

                    .Rows(nRowInGrid).Cells(nColInGrid).Value = _
                                    rDataTable.Rows(nRowInTable).Item(nColInTable).ToString

                    If (mnDataSets > 0) Then
                        Dim nRefCol As Integer = (nColInTable - 1) ' -1 to ignore the index col
                        If nRefCol < dg1.Columns.Count Then ' odg.Columns.Count
                            If (dg1.Rows(nRowInGrid).Cells(nRefCol).Value Is Nothing) = False Then
                                If dg1.Rows(nRowInGrid).Cells(nRefCol).Value.ToString = _
                                                    .Rows(nRowInGrid).Cells(nColInGrid).Value.ToString Then
                                    .Rows(nRowInGrid).Cells(nColInGrid).Style.ForeColor = Color.Black
                                Else
                                    .Rows(nRowInGrid).Cells(nColInGrid).Style.ForeColor = Color.Red
                                End If
                            Else
                                .Rows(nRowInGrid).Cells(nColInGrid).Style.ForeColor = Color.Red

                            End If
                        Else
                            .Rows(nRowInGrid).Cells(nColInGrid).Style.ForeColor = Color.Red
                        End If
                    End If ' (mnDataSets > 0)
                Next ' nRowInTable

                '*************************************************

                ' add next column unless its the last
                If (nColInTable <> (rDataTable.Columns.Count - 1)) Then
                    .Columns.Add(rDataTable.Columns(nColInTable + 1).Caption, _
                                                rDataTable.Columns(nColInTable + 1).Caption)

                End If

            Next ' nColInTable
            .AutoResizeRow(0)

            For Each oCol As DataGridViewColumn In odg.Columns
                oCol.SortMode = DataGridViewColumnSortMode.NotSortable
            Next
            If chkSyncScroll.Enabled Then
                If odg.Columns.Count <> dg1.Columns.Count Then
                    chkSyncScroll.Checked = False
                    chkSyncScroll.Enabled = False
                End If
            End If
            .ResumeLayout()
            If mnDataSets > 0 And (mbUseOneGrid = True) Then
                mnColsPerDataSet(mnDataSets + 1) = odg.ColumnCount - mnColsPerDataSet(mnDataSets)
            Else
                mnColsPerDataSet(mnDataSets + 1) = odg.ColumnCount
            End If
        End With
        frmAll_Layout()

    End Sub
#End Region
#Region " Events "

    Private Sub frmAll_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                        Handles MyBase.Load
        '********************************************************************************************
        'Description: Runs after class constructor (new)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subInitializeForm()

    End Sub
    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
     ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ClickedItem.Name
            Case "btnClose"
                Me.Close()

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible


            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True)

            Case "btnAdd"
                subLoadData()

            Case "btnClear"
                trkWidth.Visible = False
                lblWidth.Visible = False
                chkSyncScroll.Visible = False
                chkSyncScroll.Enabled = True
                tlpMain.Width = (pnlMain.Width - 5)
                DataLoaded = False
                subClearGrid(dg1)
                btnClear.Enabled = False
                mnDataSets = 0
                subEnableControls(True)
        End Select
    End Sub
    Private Sub frmAll_Closing(ByVal sender As Object, _
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


    End Sub
    
    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles cboZone.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboZone.Text <> String.Empty Then
            subChangeZone()
        End If

    End Sub
    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles cboRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Robot Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboRobot.Text <> msOldRobot Then
            subChangeRobot()
        End If

    End Sub
    Private Sub cboParam_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles cboParam.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboParam.Text <> msOldParam Then
            subChangeParameter()
        End If

    End Sub
    Private Sub frmAll_Layout(Optional ByVal sender As Object = Nothing, _
                            Optional ByVal e As System.Windows.Forms.LayoutEventArgs = Nothing) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nHeight As Integer = tscMain.ContentPanel.Height - pnlMain.Top
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100
        pnlMain.Height = nHeight
        pnlMain.Width = nWidth
        If trkWidth.Visible Then
            tlpMain.Width = (pnlMain.Width - 5) * trkWidth.Value \ trkWidth.Minimum
        Else
            tlpMain.Width = pnlMain.Width - 5
        End If
        Const nMINSTATUS_LEFT As Integer = 600
        Const nMAXSTATUS_LEFT As Integer = 1000
        Dim nStatusLeft As Integer = nWidth - lstStatus.Width
        If nStatusLeft < nMINSTATUS_LEFT Then
            nStatusLeft = nMINSTATUS_LEFT
        End If
        If nStatusLeft > nMAXSTATUS_LEFT Then
            nStatusLeft = nMAXSTATUS_LEFT
        End If
        lstStatus.Left = nStatusLeft
    End Sub


#End Region

    Private Sub cboSubParam_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSubParam.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboSubParam.Text <> msOldSubParam Then
            subChangeSubParameter()
        End If
    End Sub

    Private Sub dg_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs)
        '********************************************************************************************
        'Description:  sync scrollbars
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If DataLoaded = False Then Exit Sub
        If chkSyncScroll.Visible = False Then Exit Sub
        If chkSyncScroll.Checked = False Then Exit Sub
        Dim odgScroller As DataGridView = DirectCast(sender, DataGridView)
        Dim odg As DataGridView
        For nGrid As Integer = 1 To mnDataSets
            odg = DirectCast(tlpMain.Controls("dg" & (nGrid).ToString), DataGridView)
            If odg.Name <> odgScroller.Name Then
                odg.FirstDisplayedScrollingColumnIndex = odgScroller.FirstDisplayedScrollingColumnIndex
                odg.FirstDisplayedScrollingRowIndex = odgScroller.FirstDisplayedScrollingRowIndex
            End If
        Next
    End Sub

    Private Sub trkWidth_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles trkWidth.Scroll
        '********************************************************************************************
        'Description:  trackbar changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If DataLoaded = False Then Exit Sub
        tlpMain.Width = (pnlMain.Width - 5) * trkWidth.Value \ trkWidth.Minimum
    End Sub

End Class