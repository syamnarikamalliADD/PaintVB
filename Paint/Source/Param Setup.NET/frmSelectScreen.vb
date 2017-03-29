Imports System.Windows.Forms

Public Class frmSelectScreen
    ReadOnly Property Screen() As String
        Get
            If dgvSelScreen.SelectedRows.Count > 0 Then
                Return (dgvSelScreen.SelectedRows.Item(0).Cells(gsPRMSET_COL_TBLNAME).Value.ToString)
            End If
            Return String.Empty
        End Get
    End Property
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmSelectScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        btnOK.Enabled = (dgvSelScreen.SelectedRows.Count > 0)
    End Sub

    Private Sub dgvSelScreen_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvSelScreen.SelectionChanged
        btnOK.Enabled = (dgvSelScreen.SelectedRows.Count > 0)
    End Sub
End Class
