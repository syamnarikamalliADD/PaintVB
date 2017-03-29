Imports System.Windows.Forms

Public Class frmWaitDebug

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     Add Easy Debug menu
        '********************************************************************************************

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel

        tmrEasyButton.Enabled = False
        mBSDCommon.gnEasyDebugActive = 0

        Me.Close()

    End Sub

    Private Sub tmrEasyButton_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrEasyButton.Tick
        '********************************************************************************************
        'Description: check easy debug status
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     Add Easy Debug menu
        '********************************************************************************************

        Call mBSDCommon.CheckEasyDebug()

    End Sub
End Class
