'
'Test progarm to allow you to encrypt and decrypt strings with Crypto.dll v4.1.2.0 using the default passphrase "" or one of your chosing.
'
' Note: you will need to re-reference Crypto.dll in its current location.
Public Class Form1
    'Note: clsSQLAccess sBuildConnectionString
    '      builder.UserID = PAINTworksUser
    '      Builder.Password = Paint3762
    Private Sub btnEncrypt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEncrypt.Click
        txtOUT.Text = Crypto.Crypto.Encrypt(txtIN.Text)
    End Sub

    Private Sub btnSetPass_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetPass.Click
        Crypto.Crypto.SetPassphrase(txtPass.Text)
    End Sub

    Private Sub btnDecrypt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDecrypt.Click
        txtOUT.Text = Crypto.Crypto.Decrypt(txtIN.Text)
    End Sub
End Class
