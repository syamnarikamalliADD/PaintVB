Friend Class frmMiniGrid

    Private Const msSCREEN_DUMP_NAME As String = "Reports_MiniGrid"
    Private mSaveSize1 As Size = New Size(700, 500)
    Private mSaveSize2 As Size = New Size(200, 200)
    Friend WriteOnly Property DataTableIn() As DataTable
        Set(ByVal value As DataTable)
            mScreenSetup.subSetUpToolStrip(tlsMain)
            dgvMini.ColumnHeadersVisible = True
            dgvMini.RowHeadersVisible = True
            dgvMini.DataSource = value
            Me.Size = mSaveSize1
            Me.ShowDialog()
            mSaveSize1 = Me.Size
        End Set
    End Property

    Friend Sub DisplayCycleList(ByRef sNames() As String, ByVal nBitMask As Integer)
        '********************************************************************************************
        'Description:  use the minigrid to display cycle names
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mScreenSetup.subSetUpToolStrip(tlsMain)
        dgvMini.DataSource = Nothing
        dgvMini.RowCount = 0
        dgvMini.ColumnHeadersVisible = False
        dgvMini.RowHeadersVisible = False
        dgvMini.ColumnCount = 1
        For nIdx As Integer = 0 To sNames.GetUpperBound(0)
            If (CType((2 ^ nIdx), Long) And nBitMask) > 0 Then
                dgvMini.RowCount = dgvMini.RowCount + 1
                dgvMini.Rows.Item(dgvMini.RowCount - 1).Cells(0).ValueType = GetType(String)
                dgvMini.Rows.Item(dgvMini.RowCount - 1).Cells(0).Value = sNames(nIdx)
            End If
        Next
        dgvMini.Rows.Add()

        Me.Size = mSaveSize2
        Me.ShowDialog()
        mSaveSize2 = Me.Size
        Return
    End Sub

    Private Sub frmMiniGrid_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    Select Case frmCriteria.ReportType
                        Case frmMain.eReportType.Change
                            subLaunchHelp(gs_HELP_REPORTS_CHANGE, frmMain.oIPC)
                        Case frmMain.eReportType.Alarm
                            subLaunchHelp(gs_HELP_REPORTS_ALARM, frmMain.oIPC)
                        Case frmMain.eReportType.Conveyor
                            subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, frmMain.oIPC)
                        Case frmMain.eReportType.Production
                            subLaunchHelp(gs_HELP_REPORTS_PRODUCTION, frmMain.oIPC)
                        Case frmMain.eReportType.Downtime
                            subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, frmMain.oIPC)
                        Case frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto
                            subLaunchHelp(gs_HELP_REPORTS_RMCHARTS, frmMain.oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case frmCriteria.ReportType
                        Case frmMain.eReportType.Change
                            sName = sName & "_ChangeLog"
                        Case frmMain.eReportType.Alarm
                            sName = sName & "_Alarms"
                        Case frmMain.eReportType.Conveyor
                            sName = sName & "_Conveyor"
                        Case frmMain.eReportType.Production
                            sName = sName & "_Production"
                        Case frmMain.eReportType.Downtime
                            sName = sName & "_Downtime"
                        Case frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto
                            sName = sName & "_Charts"
                    End Select
                    sName = sName & ".jpg"
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                Case Keys.Escape
                    Me.Close()
                Case Else
            End Select
        End If
    End Sub

    Private Sub frmMiniGrid_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles Me.Layout
        '********************************************************************************************
        'Description:  adjust the grid size to match the form
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/01/09  MSW     allow resize on pop-up window
        '********************************************************************************************
        'starting size
        'dgvMini.size = 617, 444
        'frmMiniGrid.size = 661, 510
        Dim sz As Size = Me.Size

        If sz.Width > 100 Then
            sz.Width = sz.Width - 44
        End If
        If sz.Height > 100 Then
            sz.Height = sz.Height - 100
        End If
        dgvMini.Size = sz

    End Sub

    Private Sub tlsMain_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/26/14  CBZ     tlsMain_ItemClicked - Add close menu
        '********************************************************************************************
        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
        End Select

    End Sub

End Class