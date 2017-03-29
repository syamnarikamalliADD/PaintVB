Imports System.Windows.Forms

Public Class frmImport
    Private mDT() As DataTable = Nothing
    Private mbIgnore As Boolean = False
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
    Friend Sub SetDTs(ByRef oDT() As DataTable)
        mDT = oDT
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
                    Return lblFrom.Text
                Case 3
                    Return lblTo.Text
                Case Else
                    Return String.Empty
            End Select
        End Get
        Set(ByVal value As String)
            Select Case nIndex
                Case 1
                    lblLabel1.Text = value
                Case 2
                    lblFrom.Text = value
                Case 3
                    lblTo.Text = value
                Case Else
                    '
            End Select
        End Set
    End Property
    ReadOnly Property FromItems() As Boolean()
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
            Dim bItems(chkFrom.Items.Count - 1) As Boolean
            For nItem As Integer = 0 To chkFrom.Items.Count - 1
                If chkFrom.GetItemChecked(nItem) Then
                    bItems(nItem) = True
                Else
                    bItems(nItem) = False
                End If
            Next
            Return bItems
        End Get
    End Property
    ReadOnly Property FromItem() As Integer
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
            Dim nReturn As Integer = -1
            For nItem As Integer = 0 To chkFrom.Items.Count - 1
                If chkFrom.GetItemChecked(nItem) Then
                    If nReturn = -1 Then
                        nReturn = nItem
                    Else
                        Return -1
                    End If
                End If
            Next
            Return nReturn
        End Get
    End Property
    ReadOnly Property ToItem() As Integer
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
            Dim nReturn As Integer = -1
            For nItem As Integer = 0 To chkTo.Items.Count - 1
                If chkTo.GetItemChecked(nItem) Then
                    If nReturn = -1 Then
                        nReturn = nItem
                    Else
                        Return -1
                    End If
                End If
            Next
            Return nReturn
        End Get
    End Property
    Public Sub SetFromList(ByRef sText() As String)
        '********************************************************************************************
        'Description: Add Item to chklist
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        chkFrom.Items.Clear()
        chkFrom.Items.AddRange(sText)
    End Sub
    Public Sub SetToList(ByRef oCbo As ComboBox)
        '********************************************************************************************
        'Description: Add Item to chklist
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        chkTo.Items.Clear()
        For nItem As Integer = 0 To oCbo.Items.Count - 1
            chkTo.Items.Add(oCbo.Items(nItem).ToString)
        Next
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
        lblFrom.Text = gpsRM.GetString("psFROM")
        lblTo.Text = gpsRM.GetString("psTO")
        btnOK.Text = gcsRM.GetString("csOK")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
        mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")
    End Sub

    Private Sub chkFrom_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles chkFrom.ItemCheck
        '********************************************************************************************
        'Description: check change on from box
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbIgnore Then
            Exit Sub
        End If
        mbIgnore = True
        Dim nCheckCount As Integer = chkFrom.CheckedItems.Count
        If e.NewValue = CheckState.Checked Then
            nCheckCount = nCheckCount + 1
        Else
            nCheckCount = nCheckCount - 1
        End If
        If chkTo.Items.Count = chkFrom.Items.Count Then
            For nItem As Integer = 0 To chkTo.Items.Count - 1
                If e.Index = nItem Then
                    chkTo.SetItemChecked(nItem, Not (chkFrom.GetItemChecked(nItem)))
                Else
                    chkTo.SetItemChecked(nItem, chkFrom.GetItemChecked(nItem))
                End If
            Next
        End If
        Application.DoEvents()
        chkTo.Enabled = (nCheckCount = 1)
        mbIgnore = False
    End Sub

    Private Sub chkFrom_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFrom.SelectedIndexChanged
        '********************************************************************************************
        'Description: load selected cycle in preview box
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If chkFrom.SelectedIndex > -1 Then
            dgPreview.DataSource = mDT(chkFrom.SelectedIndex)
        End If
    End Sub

    Private Sub chkTo_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles chkTo.ItemCheck
        '********************************************************************************************
        'Description: check change on To box
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbIgnore Then
            Exit Sub
        End If

        mbIgnore = True
        Dim nCheckCount As Integer = chkFrom.CheckedItems.Count
        If nCheckCount > 1 Then
            e.NewValue = chkFrom.GetItemCheckState(e.Index)
        Else
            For nItem As Integer = 0 To chkTo.Items.Count - 1
                If e.Index <> nItem Then
                    chkTo.SetItemChecked(nItem, False)
                End If
            Next
        End If
        Application.DoEvents()
        chkTo.Enabled = (nCheckCount = 1)
        mbIgnore = False
    End Sub

    Private Sub mnuSelectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        mbIgnore = True
        For nItem As Integer = 0 To chkFrom.Items.Count - 1
            chkFrom.SetItemChecked(nItem, True)
            chkTo.SetItemChecked(nItem, True)
        Next
        mbIgnore = False
    End Sub

    Private Sub mnuUnselectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuUnselectAll.Click
        mbIgnore = True
        For nItem As Integer = 0 To chkFrom.Items.Count - 1
            chkFrom.SetItemChecked(nItem, False)
            chkTo.SetItemChecked(nItem, False)
        Next
        mbIgnore = False
    End Sub
End Class

