Imports System.Windows.Forms

Public Class frmBrowse
    Private msCntr As String
    Private msPath As String
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description:  Pressed OK - exit form
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
        'Description:  Pressed Cancel - exit form
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
    Friend Property Path() As String
        '********************************************************************************************
        'Description:  Get or set the path in the combo box
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return cboPath.Text
        End Get
        Set(ByVal value As String)
            cboPath.Text = value
        End Set
    End Property
    Friend Property Controller() As String
        Get
            Return msPath
        End Get
        Set(ByVal value As String)
            msCntr = value
            msPath = msCntr

        End Set
    End Property
    Private Sub frmBrowse_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
        cboPath.SelectedIndex = -1
        cboPath.Items.Clear()
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        btnOK.Text = gcsRM.GetString("csOK")
        With gpsRM
            Me.Text = .GetString("psBROWSE_FOR_FOLDER")
            cboPath.Items.Add(.GetString("psMDB"))
            cboPath.Items.Add(.GetString("psMD"))
            cboPath.Items.Add(.GetString("psFRA"))
            cboPath.Items.Add(.GetString("psRD"))
            cboPath.Items.Add(.GetString("psMC"))
            cboPath.Items.Add(.GetString("psUD1"))
            cboPath.Items.Add(.GetString("psUT1"))
            cboPath.Items.Add(.GetString("psFR"))
            cboPath.SelectedIndex = 0
            btnRefresh.Text = .GetString("psREFRESH")
        End With
        subRefreshDirectoryList()
    End Sub
    Private Sub subRefreshDirectoryList()
        '********************************************************************************************
        'Description:  Get a folder list for the current path
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lstFolders.Clear()
        Dim oFTP As clsFSFtp = Nothing
        Try
            oFTP = New clsFSFtp(msCntr)
            Application.DoEvents()
            With oFTP
                If .Connected Then
                    Dim sTmpDevName As String = Replace(cboPath.Text, "/", "\")
                    Dim sDevName() As String = Split(sTmpDevName, "\")
                    msPath = String.Empty
                    For nFolderDepth As Integer = 0 To sDevName.GetUpperBound(0)
                        If sDevName(nFolderDepth) <> String.Empty Then
                            .WorkingDir = sDevName(nFolderDepth)
                            msPath = msPath & sDevName(nFolderDepth) & "\"
                        End If
                    Next
                    Dim sFolders() As String = .Directory("*", True, False)
                    For Each sFolder As String In sFolders
                        lstFolders.Items.Add(sFolder)
                    Next
                End If
            End With
        Catch ex As Exception
            If oFTP IsNot Nothing Then
                oFTP.Close()
            End If
        End Try
    End Sub
    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        '********************************************************************************************
        'Description:  Manually refresh the folder list when the button is pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subRefreshDirectoryList()
    End Sub

    Private Sub cboPath_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboPath.KeyDown
        '********************************************************************************************
        'Description:  Refresh the folder list when the enter is pressed is pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If e.KeyCode = Keys.Enter Then
            subRefreshDirectoryList()
        End If
    End Sub

    Private Sub cboPath_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPath.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Refresh the folder list when the enter is pressed is pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subRefreshDirectoryList()
    End Sub

    Private Sub lstFolders_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstFolders.SelectedIndexChanged
        msPath = msPath & lstFolders.SelectedItems.Item(0).Text & "\"
        cboPath.Text = msPath
        subRefreshDirectoryList()
    End Sub
End Class
