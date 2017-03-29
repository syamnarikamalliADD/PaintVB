Imports System.Windows.Forms

Public Class frmTopic
    Property FormTitle() As String
        '********************************************************************************************
        'Description:  Access to the form title
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return Me.Text
        End Get
        Set(ByVal value As String)
            Me.Text = value
        End Set
    End Property
    Property Topic() As String
        '********************************************************************************************
        'Description:  Access to the topic text
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
            If value = String.Empty Then
                btnOK.Enabled = False
            Else
                btnOK.Enabled = True
            End If
            txtTopic.Text = value
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
        lblTopic.Text = gpsRM.GetString("psTOPIC_CAP")

        btnCancel.Enabled = True
        btnOK.Enabled = False
        btnOK.Text = gcsRM.GetString("csOK")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
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

    Private Sub txtTitle_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTopic.TextChanged
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
        btnOK.Enabled = (txtTopic.Text <> String.Empty)
    End Sub
End Class
