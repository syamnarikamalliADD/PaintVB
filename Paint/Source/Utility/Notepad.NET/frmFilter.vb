Imports System.Windows.Forms

Public Class frmFilter
    Property EnableWildCards() As Boolean
        '********************************************************************************************
        'Description:  enable wildcards
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return chkWildcards.Checked
        End Get
        Set(ByVal value As Boolean)
            chkWildcards.Checked = value
        End Set
    End Property
    Property CreatedStartEnable() As Boolean
        '********************************************************************************************
        'Description:  Creation time start checkbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return dtpStartDateCre.Checked
        End Get
        Set(ByVal value As Boolean)
            dtpStartDateCre.Checked = value
        End Set
    End Property
    Property CreatedEndEnable() As Boolean
        '********************************************************************************************
        'Description:  Creation time end checkbox
        '
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return dtpEndDateCre.Checked
        End Get
        Set(ByVal value As Boolean)
            dtpEndDateCre.Checked = value
        End Set
    End Property
    Property ModifiedStartEnable() As Boolean
        '********************************************************************************************
        'Description:  Modified time start checkbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return dtpStartDateMod.Checked
        End Get
        Set(ByVal value As Boolean)
            dtpStartDateMod.Checked = value
        End Set
    End Property
    Property ModifiedEndEnable() As Boolean
        '********************************************************************************************
        'Description:  Modified time end checkbox
        '
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return dtpEndDateMod.Checked
        End Get
        Set(ByVal value As Boolean)
            dtpEndDateMod.Checked = value
        End Set
    End Property
    Property CreatedStart() As DateTime
        '********************************************************************************************
        'Description:  Creation start time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get

            Return New DateTime(dtpStartDateCre.Value.Year, dtpStartDateCre.Value.Month, dtpStartDateCre.Value.Day, _
                                dtpStartTimeCre.Value.Hour, dtpStartTimeCre.Value.Minute, dtpStartTimeCre.Value.Second)
        End Get
        Set(ByVal value As DateTime)
            dtpStartDateCre.Value = value.Date
            dtpStartTimeCre.Value = value
        End Set
    End Property
    Property CreatedEnd() As DateTime
        '********************************************************************************************
        'Description:  Creation end time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get

            Return New DateTime(dtpEndDateCre.Value.Year, dtpEndDateCre.Value.Month, dtpEndDateCre.Value.Day, _
                                dtpEndTimeCre.Value.Hour, dtpEndTimeCre.Value.Minute, dtpEndTimeCre.Value.Second)
        End Get
        Set(ByVal value As DateTime)
            dtpEndDateCre.Value = value.Date
            dtpEndTimeCre.Value = value
        End Set
    End Property
    Property ModifiedStart() As DateTime
        '********************************************************************************************
        'Description:  Modified start time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get

            Return New DateTime(dtpStartDateMod.Value.Year, dtpStartDateMod.Value.Month, dtpStartDateMod.Value.Day, _
                                dtpStartTimeMod.Value.Hour, dtpStartTimeMod.Value.Minute, dtpStartTimeMod.Value.Second)
        End Get
        Set(ByVal value As DateTime)
            dtpStartDateMod.Value = value.Date
            dtpStartTimeMod.Value = value
        End Set
    End Property
    Property ModifiedEnd() As DateTime
        '********************************************************************************************
        'Description:  Modified end time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get

            Return New DateTime(dtpEndDateMod.Value.Year, dtpEndDateMod.Value.Month, dtpEndDateMod.Value.Day, _
                                dtpEndTimeMod.Value.Hour, dtpEndTimeMod.Value.Minute, dtpEndTimeMod.Value.Second)
        End Get
        Set(ByVal value As DateTime)
            dtpEndDateMod.Value = value.Date
            dtpEndTimeMod.Value = value
        End Set
    End Property
    Property Topic() As String
        '********************************************************************************************
        'Description:  topic text box
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return txtTopic.Text
        End Get
        Set(ByVal value As String)
            txtTopic.Text = value
        End Set
    End Property
    Property Author() As String
        '********************************************************************************************
        'Description:  author cbo box
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return cboAuthor.Text
        End Get
        Set(ByVal value As String)
            cboAuthor.Text = value
        End Set
    End Property
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description:  OK button.  Set the status  and return
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description:  cancel button.  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description:  Previous button.  Set the backwards status  and return
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.Text = gpsRM.GetString("psFILTER_FRM_CAP")
        lblTopic.Text = gpsRM.GetString("psTOPIC_CAP")
        lblAuthor.Text = gpsRM.GetString("psAUTHOR_CAP")
        lblCreatedDate.Text = gpsRM.GetString("psCREATION_DATE")
        lblModifiedDate.Text = gpsRM.GetString("psMODIFIED_DATE")
        lblStartDateCre.Text = gpsRM.GetString("psSTART")
        lblStartDateMod.Text = gpsRM.GetString("psSTART")
        lblEndDateCre.Text = gpsRM.GetString("psEND")
        lblEndDateMod.Text = gpsRM.GetString("psEND")
        chkWildcards.Text = gpsRM.GetString("psENABLE_WILDCARDS")
        btnCancel.Enabled = True
        btnOK.Text = gcsRM.GetString("csOK")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        'Fill up the author cbo
        If cboAuthor.Items.Count = 0 Then
            cboAuthor.Items.Add(gcsRM.GetString("csALL"))
        End If
        For nRow As Integer = 0 To frmMain.dgvList.Rows.Count - 1
            Dim sAuthor As String = frmMain.dgvList.Rows.Item(nRow).Cells.Item(gpsRM.GetString("psAUTHOR_CAP")).Value.ToString
            Dim nIndex As Integer = 1
            Dim bFound As Boolean = False
            While (nIndex < cboAuthor.Items.Count) And (bFound = False)
                bFound = (sAuthor = cboAuthor.Items(nIndex).ToString)
                nIndex += 1
            End While
            If Not (bFound) Then
                cboAuthor.Items.Add(sAuthor)
            End If
        Next
        btnOK.Enabled = (txtTopic.Text <> String.Empty) Or (cboAuthor.Text <> String.Empty) Or _
        dtpStartDateMod.Checked Or dtpEndDateMod.Checked Or _
        dtpStartDateCre.Checked Or dtpEndDateCre.Checked
    End Sub
    Private Sub frm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:  init form
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

    Private Sub txtTitle_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTopic.TextChanged, _
        cboAuthor.SelectedIndexChanged, cboAuthor.TextChanged
        '********************************************************************************************
        'Description:  text changed - enable buttons
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        btnOK.Enabled = (txtTopic.Text <> String.Empty) Or (cboAuthor.Text <> String.Empty) Or _
                dtpStartDateMod.Checked Or dtpEndDateMod.Checked Or _
                dtpStartDateCre.Checked Or dtpEndDateCre.Checked
    End Sub

    Private Sub dtp_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpStartDateMod.ValueChanged, _
    dtpEndDateMod.ValueChanged, dtpStartDateCre.ValueChanged, dtpEndDateCre.ValueChanged
        '********************************************************************************************
        'Description:  Date setting changed, enable checkboxes and ok button - 
        '               We only need "check state changed", but this is what we get.
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        dtpStartTimeCre.Enabled = dtpStartDateCre.Checked
        dtpEndTimeCre.Enabled = dtpEndDateCre.Checked
        dtpStartTimeMod.Enabled = dtpStartDateMod.Checked
        dtpEndTimeMod.Enabled = dtpEndDateMod.Checked
        btnOK.Enabled = (txtTopic.Text <> String.Empty) Or (cboAuthor.Text <> String.Empty) Or _
                dtpStartDateMod.Checked Or dtpEndDateMod.Checked Or _
                dtpStartDateCre.Checked Or dtpEndDateCre.Checked
    End Sub

End Class
