' 03/05/14  MSW     ZDT Changes
'Copy minigrid from reports, customize a bit for ZDT

Friend Class frmMiniGrid

    Private Const msSCREEN_DUMP_NAME As String = "BSD_ZDT_MiniGrid"
    Private mSaveSize1 As Size = New Size(700, 500)
    Private mSaveSize2 As Size = New Size(200, 200)
    Friend mnColumn As Integer = -1
    Friend mnRow As Integer = -1
    Private mnUpdateLogStep As Integer = 0
    Private mbSaveAfterUpdate As Boolean = False
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
        Try

            Dim sz As Size = Me.Size
            If sz.Width > 100 Then
                sz.Width = sz.Width - 44
            End If
            If sz.Height > 100 Then
                sz.Height = sz.Height - 100
            End If
            dgvMini.Size = sz
            lblUpdateMessage.Width = sz.Width - 12
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: frmMiniGrid_Layout", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub subSaveData()
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
        Dim sFileName As String = String.Empty
        Dim oSFD As New SaveFileDialog
        Try
            With oSFD
                Dim sPathTmp As String = String.Empty
                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                    oSFD.InitialDirectory = sPathTmp
                End If
                oSFD.CheckPathExists = True
                oSFD.AddExtension = True
                oSFD.OverwritePrompt = True
                oSFD.FileName = colZones.CurrentZone & "_" & "ZDT.ZIP"
                oSFD.DefaultExt = gpsRM.GetString("psZIP_EXT")
                oSFD.Filter = gpsRM.GetString("psZIPMASK")
                oSFD.FilterIndex = 1
                Dim oVal As DialogResult = oSFD.ShowDialog
                If (oVal = System.Windows.Forms.DialogResult.OK) Then
                    sFileName = oSFD.FileName
                Else
                    sFileName = String.Empty
                End If

            End With
            If sFileName <> String.Empty Then
                If IO.File.Exists(sFileName) Then
                    IO.File.Delete(sFileName)
                End If
                mZDT.subZipResults(sFileName)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: subSaveData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, frmMain.msSCREEN_NAME, _
                    frmMain.Status, MessageBoxButtons.OK)
        End Try
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
        Try
            Select Case e.ClickedItem.Name ' case sensitive
                Case "btnClose"
                    Me.Close()
                Case "btnSave"
                    'Save away the ZDT Data
                    Select Case MessageBox.Show(gpsRM.GetString("psUPDATE_ZDT_DATA_TEXT"), gpsRM.GetString("psUPDATE_ZDT_DATA_CAP"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                        Case Windows.Forms.DialogResult.Yes
                            mbSaveAfterUpdate = True
                            subUpdateLogs()
                        Case Windows.Forms.DialogResult.No
                            subSaveData()
                        Case Windows.Forms.DialogResult.Cancel
                    End Select
                Case "btnUpdateLog"
                    mbSaveAfterUpdate = False
                    subUpdateLogs()
                Case "btnHelp"
                    subLaunchHelp(gs_HELP_ZDT, frmMain.oIPC)
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: tlsMain_ItemClicked", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub


    Private Sub frmMiniGrid_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: load menu items
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            btnClose.Text = gcsRM.GetString("csCLOSE")
            btnClose.Enabled = True
            btnClose.Image = DirectCast(gcsRM.GetObject("imgClose"), Image)

            btnSave.Text = gcsRM.GetString("csSAVE")
            btnSave.Enabled = True
            btnSave.Image = DirectCast(gcsRM.GetObject("imgSave"), Image)
            btnSave.ToolTipText = gpsRM.GetString("psSAVE_ZDT_TT")

            btnUpdateLog.Text = gpsRM.GetString("psUPDATE_LOG")
            btnUpdateLog.Enabled = (frmMain.oZDT.moZDTConfig.colFileLog IsNot Nothing)
            btnUpdateLog.Image = DirectCast(gcsRM.GetObject("imgRestore"), Image)
            btnUpdateLog.ToolTipText = gpsRM.GetString("psUPDATE_LOG_TT")
            mnuResetItemStatus.Text = gpsRM.GetString("psRESET_STATUS")
            mnuResetData.Text = gpsRM.GetString("psRESET_DATA")
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: frmMiniGrid_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub mnuResetItemStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuResetItemStatus.Click
        '********************************************************************************************
        'Description: reset menu clicked
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            frmMain.oZDT.subResetStatus(dgvMini, mnuResetStatus, mnColumn, mnRow)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: mnuResetItemStatus_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub mnuResetData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuResetData.Click
        '********************************************************************************************
        'Description: reset menu clicked
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            frmMain.oZDT.subArchiveTestData(dgvMini, mnColumn, mnRow)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: mnuResetData_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub dgvMini_CellMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgvMini.CellMouseDown
        '********************************************************************************************
        'Description: mouse down on a cell - set up menu enables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If e.ColumnIndex > -1 And e.RowIndex > -1 Then
                mnColumn = e.ColumnIndex
                mnRow = e.RowIndex
                If dgvMini.Rows(mnRow).Cells(mnColumn).ContextMenuStrip IsNot Nothing Then
                    If dgvMini.Rows(mnRow).Cells(mnColumn).Tag IsNot Nothing Then
                        Dim bTag() As Boolean = DirectCast(dgvMini.Rows(mnRow).Cells(mnColumn).Tag, Boolean())
                        If bTag IsNot Nothing Then
                            mnuResetItemStatus.Enabled = bTag(0)
                            mnuResetData.Enabled = bTag(1)
                            mnuResetStatus.Enabled = bTag(0) Or bTag(1)
                        Else
                            mnuResetItemStatus.Enabled = False
                            mnuResetData.Enabled = False
                        End If
                    Else
                        mnuResetItemStatus.Enabled = False
                        mnuResetData.Enabled = False
                    End If

                    If e.Button = Windows.Forms.MouseButtons.Left Then
                        'Manually launch the menu
                        Dim oPoint As Point = New Point(e.X, e.Y)
                        oPoint = dgvMini.PointToScreen(oPoint)
                        Dim nXOffset As Integer = dgvMini.RowHeadersWidth
                        Dim nYOffset As Integer = dgvMini.ColumnHeadersHeight
                        If mnColumn > 0 Then
                            For nColumn As Integer = 0 To mnColumn - 1
                                nXOffset = nXOffset + dgvMini.Columns(nColumn).Width
                            Next
                        End If
                        If mnRow > 0 Then
                            For nRow As Integer = 0 To mnRow - 1
                                nYOffset = nYOffset + dgvMini.Rows(nRow).Height
                            Next
                        End If
                        oPoint.X = oPoint.X + nXOffset
                        oPoint.Y = oPoint.Y + nYOffset
                        dgvMini.Rows(mnRow).Cells(mnColumn).ContextMenuStrip.Show(oPoint)
                    End If

                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: dgvMini_CellMouseDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub dgvMini_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMini.CellMouseEnter
        '********************************************************************************************
        'Description: mouse moved into a cell
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnColumn = e.ColumnIndex
        mnRow = e.RowIndex
    End Sub
    Private Sub subUpdateLogs()
        '********************************************************************************************
        'Description: update the log files from the robot
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            lblUpdateMessage.Text = gpsRM.GetString("psUPDATING_LOG")
            lblUpdateMessage.Visible = True
            tlsMain.Enabled = False
            If frmMain.oZDT.bSendFileReqToPLC Then
                mnUpdateLogStep = 1
                tmrUpdateLog.Interval = 500
                tmrUpdateLog.Enabled = True
            Else
                'Skip the send to PLC.
                mnUpdateLogStep = 10
                tmrUpdateLog.Interval = 100
                tmrUpdateLog.Enabled = True
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: subUpdateLogs", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub tmrUpdateLog_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdateLog.Tick
        '********************************************************************************************
        'Description: update the log files from the robot
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrUpdateLog.Enabled = False
            If mnUpdateLogStep < 10 Then
                lblUpdateMessage.Text = lblUpdateMessage.Text & "."
                mnUpdateLogStep = mnUpdateLogStep + 1
                tmrUpdateLog.Enabled = True
            Else
                If mnUpdateLogStep = 10 Then
                    frmMain.oZDT.moZDTConfig.subGetLogFiles(frmMain.oZDT.moZDTConfig.msLogFilePath)
                    lblUpdateMessage.Text = gpsRM.GetString("psUPDATE_COMPLETE")
                    mnUpdateLogStep = mnUpdateLogStep + 1
                    tmrUpdateLog.Interval = 750
                    tmrUpdateLog.Enabled = True
                    If mbSaveAfterUpdate Then
                        subSaveData()
                    End If
                Else
                    lblUpdateMessage.Visible = False
                End If
                tlsMain.Enabled = True
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: frmMiniGrid Routine: tmrUpdateLog_Tick", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub



End Class