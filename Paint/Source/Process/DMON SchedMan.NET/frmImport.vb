Imports System.Windows.Forms

Public Class frmImport
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Property label(ByVal nIndex As Integer) As String
        '********************************************************************************************
        'Description: access to the labels that need to change
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Select Case nIndex
                Case 1
                    Return lblLabel1.Text
                Case 2
                    Return lblLabel2.Text
                Case 3
                    Return lblLabel3.Text
                Case Else
                    Return String.Empty
            End Select
        End Get
        Set(ByVal value As String)
            Select Case nIndex
                Case 1
                    lblLabel1.Text = value
                Case 2
                    lblLabel2.Text = value
                Case 3
                    lblLabel3.Text = value
                Case Else
                    '
            End Select
        End Set
    End Property
    Property Zone() As String
        '********************************************************************************************
        'Description: import source zone
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblZone.Text
        End Get
        Set(ByVal value As String)
            lblZone.Text = value
        End Set
    End Property
    Property Device() As String
        '********************************************************************************************
        'Description: import source device
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblDevice.Text
        End Get
        Set(ByVal value As String)
            lblDevice.Text = value
        End Set
    End Property
    Property MaxToItems() As Integer
        '********************************************************************************************
        'Description: max import destination item number
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(numudItem.Maximum, Integer)
        End Get
        Set(ByVal value As Integer)
            numudItem.Maximum = value
        End Set
    End Property
    Property FromItem() As Integer
        '********************************************************************************************
        'Description: import source item number
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return cboItems.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            cboItems.SelectedIndex = value
        End Set
    End Property
    Property ToItem() As Integer
        '********************************************************************************************
        'Description: import destination item number
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(numudItem.Value, Integer)
        End Get
        Set(ByVal value As Integer)
            numudItem.Value = value
        End Set
    End Property
    Public Sub ClearCboItems()
        '********************************************************************************************
        'Description: clear items in cbo
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        cboItems.Items.Clear()
    End Sub

    Public Sub AddCboItem(ByVal sText As String, ByVal nIndex As Integer)
        '********************************************************************************************
        'Description: Add Item to CBO
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboItems.Items.Count = 0 Then
            Dim nTag(0) As Integer
            nTag(0) = nIndex
            cboItems.Items.Add(sText)
            cboItems.Tag = nTag
        Else
            Dim nTag() As Integer = DirectCast(cboItems.Tag, Integer())
            ReDim Preserve nTag(nTag.Length)
            nTag(nTag.Length - 1) = nIndex
            cboItems.Items.Add(sText)
            cboItems.Tag = nTag
        End If
    End Sub
    Private Sub frmImport_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Init the form
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        lblSource.Text = gpsRM.GetString("psIMPORT_DATA_SOURCE")
        lblZoneLbl.Text = gcsRM.GetString("csZONE_CAP")
        lblDeviceLbl.Text = gcsRM.GetString("csROBOT_CAP")
        btnOK.Text = gcsRM.GetString("csOK")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        lblLabel3.Visible = False
        numudItem.Visible = False
        numudItem.Minimum = 1
    End Sub


    Private Sub cboItems_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboItems.SelectionChangeCommitted
        '********************************************************************************************
        'Description: an item is selected for import.  Show "To:" selection if an individual item is selected
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If cboItems.SelectedIndex <= 0 Then
            lblLabel3.Visible = False
            numudItem.Visible = False
        Else
            lblLabel3.Visible = True
            numudItem.Visible = True
            numudItem.Value = cboItems.SelectedIndex
        End If
    End Sub

End Class

