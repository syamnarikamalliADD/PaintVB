Imports System.Windows.Forms

Public Class frmFind
    Private mbPrevious As Boolean = False
    ReadOnly Property Backwards() As Boolean
        '********************************************************************************************
        'Description:  Backwards status.  
        '
        'Parameters: none
        'Returns:    True if previous button was pressed 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return mbPrevious
        End Get
    End Property

    ReadOnly Property CaseSensitive() As Boolean
        '********************************************************************************************
        'Description:  case sensitive is selected.  
        '
        'Parameters: none
        'Returns:    True if case sensitive is selected 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return chkCaseSens.Checked
        End Get
    End Property
    Property SearchAllDocs() As Boolean
        '********************************************************************************************
        'Description:  search all selected.  
        '
        'Parameters: none
        'Returns:    True if search all is selected 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Set(ByVal value As Boolean)
            rdoAllDocs.Checked = value
        End Set
        Get
            Return rdoAllDocs.Checked
        End Get
    End Property
    Property SearchThisDoc() As Boolean
        '********************************************************************************************
        'Description:  search all selected.  
        '
        'Parameters: none
        'Returns:    True if search all is selected 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Set(ByVal value As Boolean)
            rdoThisDoc.Checked = value
        End Set
        Get
            Return rdoThisDoc.Checked
        End Get
    End Property

    Property SearchTopics() As Boolean
        '********************************************************************************************
        'Description:  search topics.  
        '
        'Parameters: none
        'Returns:    True if topic search is selected 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Set(ByVal value As Boolean)
            rdoTopics.Checked = value
        End Set
        Get
            Return rdoTopics.Checked
        End Get
    End Property

    Property SearchText() As String
        '********************************************************************************************
        'Description:  Access to the search text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return txtSearchText.Text
        End Get
        Set(ByVal value As String)
            If value = String.Empty Then
                btnPrev.Enabled = False
                btnNext.Enabled = False
            Else
                btnPrev.Enabled = True
                btnNext.Enabled = True
            End If
            txtSearchText.Text = value
        End Set
    End Property
    Private Sub btnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrev.Click
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
        mbPrevious = True
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
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
        mbPrevious = False
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
        Me.Text = gpsRM.GetString("psFIND_FRM_CAP")
        lblFind.Text = gpsRM.GetString("psFIND_FRM_CAP")
        btnCancel.Enabled = True
        btnNext.Enabled = (txtSearchText.Text <> String.Empty)
        btnNext.Text = gpsRM.GetString("psNEXT_BTN")
        btnPrev.Enabled = (txtSearchText.Text <> String.Empty)
        btnPrev.Text = gpsRM.GetString("psPREV_BTN")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        rdoAllDocs.Text = gpsRM.GetString("psSEARCH_ALL_DOCS")
        rdoThisDoc.Text = gpsRM.GetString("psSEARCH_THIS_DOC")
        rdoTopics.Text = gpsRM.GetString("psSEARCH_TOPICS")
        chkCaseSens.Text = gpsRM.GetString("psCASE_SENS")
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
        txtSearchText.Select()
    End Sub

    Private Sub txtTitle_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearchText.TextChanged
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
        btnNext.Enabled = (txtSearchText.Text <> String.Empty)
        btnPrev.Enabled = (txtSearchText.Text <> String.Empty)
    End Sub

    Private Sub rdoThisDoc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoThisDoc.CheckedChanged

    End Sub
End Class
