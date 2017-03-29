'	Crypto class
'	--
'	Uses crypto API functions to encrypt and decrypt data. A passphrase 
'	string is used to create a 128-bit hash that is used to create a 
'	40-bit crypto key. The same key is required to encrypt and decrypt 
'	the data.
'    Date       By      Reason                                                        Version
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'********************************************************************************************
Imports System.Runtime.InteropServices
Imports System.Text


' Encrypts and decrypts data using the crypto APIs.
Public Class Crypto

    ' API functions
    Private Class WinApi
#Region " Crypto API imports "

        Private Const ALG_CLASS_HASH As Integer = (4 << 13)
        Private Const ALG_TYPE_ANY As Integer = 0
        Private Const ALG_CLASS_DATA_ENCRYPT As Integer = (3 << 13)
        Private Const ALG_TYPE_STREAM As Integer = (4 << 9)
        Private Const ALG_TYPE_BLOCK As Integer = (3 << 9)

        Private Const ALG_SID_DES As Integer = 1
        Private Const ALG_SID_RC4 As Integer = 1
        Private Const ALG_SID_RC2 As Integer = 2
        Private Const ALG_SID_MD5 As Integer = 3

        Public Const MS_DEF_PROV As String = "Microsoft Base Cryptographic Provider v1.0"

        Public Const PROV_RSA_FULL As Integer = 1
        Public Const CRYPT_VERIFYCONTEXT As Integer = &HF0000000
        Public Const CRYPT_EXPORTABLE As Integer = &H1

        Public Shared ReadOnly CALG_MD5 As Integer = ALG_CLASS_HASH Or ALG_TYPE_ANY Or ALG_SID_MD5
        Public Shared ReadOnly CALG_DES As Integer = ALG_CLASS_DATA_ENCRYPT Or ALG_TYPE_BLOCK Or ALG_SID_DES
        Public Shared ReadOnly CALG_RC2 As Integer = ALG_CLASS_DATA_ENCRYPT Or ALG_TYPE_BLOCK Or ALG_SID_RC2
        Public Shared ReadOnly CALG_RC4 As Integer = ALG_CLASS_DATA_ENCRYPT Or ALG_TYPE_STREAM Or ALG_SID_RC4

#If COMPACT_FRAMEWORK Then
	Private Const CryptDll As String = "coredll.dll"
	Private Const KernelDll As String = "coredll.dll" '
#Else
        Private Const CryptDll As String = "advapi32.dll"
        Private Const KernelDll As String = "kernel32.dll" '
#End If

        <DllImport(CryptDll)> _
        Public Shared Function CryptAcquireContext( _
         ByRef phProv As IntPtr, ByVal pszContainer As String, _
         ByVal pszProvider As String, ByVal dwProvType As Integer, _
         ByVal dwFlags As Integer) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptReleaseContext( _
         ByVal hProv As IntPtr, ByVal dwFlags As Integer) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptDeriveKey( _
         ByVal hProv As IntPtr, ByVal Algid As Integer, _
         ByVal hBaseData As IntPtr, ByVal dwFlags As Integer, _
         ByRef phKey As IntPtr) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptCreateHash( _
         ByVal hProv As IntPtr, ByVal Algid As Integer, _
         ByVal hKey As IntPtr, ByVal dwFlags As Integer, _
         ByRef phHash As IntPtr) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptHashData( _
         ByVal hHash As IntPtr, ByVal pbData() As Byte, ByVal dwDataLen As Integer, _
         ByVal dwFlags As Integer) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptEncrypt( _
         ByVal hKey As IntPtr, ByVal hHash As IntPtr, _
         ByVal Final As Boolean, ByVal dwFlags As Integer, _
         ByVal pbData() As Byte, ByRef pdwDataLen As Integer, _
         ByVal dwBufLen As Integer) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptDecrypt( _
         ByVal hKey As IntPtr, ByVal hHash As IntPtr, _
         ByVal Final As Boolean, ByVal dwFlags As Integer, _
         ByVal pbData() As Byte, ByRef pdwDataLen As Integer) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptDestroyHash(ByVal hHash As IntPtr) As Boolean
        End Function

        <DllImport(CryptDll)> _
        Public Shared Function CryptDestroyKey(ByVal hKey As IntPtr) As Boolean
        End Function

#End Region

#Region " Error reporting imports "

        Public Const FORMAT_MESSAGE_FROM_SYSTEM As Integer = &H1000

        <DllImport(KernelDll)> _
        Public Shared Function GetLastError() As Integer
        End Function

        <DllImport(KernelDll)> _
        Public Shared Function FormatMessage( _
         ByVal dwFlags As Integer, ByVal lpSource As String, _
         ByVal dwMessageId As Integer, ByVal dwLanguageId As Integer, _
         ByVal lpBuffer As StringBuilder, ByVal nSize As Integer, _
         ByVal Arguments() As String) As Integer
        End Function

#End Region
    End Class


    'Private Const sPassphrase As String = "bdm3dp@ssphras3"
    Private Shared msPassphrase As String = "bdm3dp@ssphras3"

    Public Shared Sub SetPassphrase(ByVal Passphrase As String)
        msPassphrase = Passphrase
    End Sub
    ' DECRYPT
    Public Shared Function Decrypt(ByVal sEncryptedStr As String) As String
        ' Convert the sEncryptedStr passed in into a byte array
        Dim i As Int32
        Dim sBytes As String() = sEncryptedStr.Split(" ")
        Dim yEncBytes() As Byte = New Byte(sBytes.Length - 1) {}
        For i = sBytes.GetLowerBound(0) To sBytes.GetUpperBound(0)
            yEncBytes(i) = Convert.ToByte(sBytes(i))
        Next

        ' Decrypt the byte array of the encrypted string
        Dim yDecrypted As Byte() = Decrypt(msPassphrase, yEncBytes)

        ' Convert the decrypted byte array into the decrypted string and return it
        Return ASCIIEncoding.ASCII.GetString(yDecrypted, 0, yDecrypted.Length)
    End Function



    ' PRIVATE SHARED DECRYPT
    Private Shared Function Decrypt(ByVal passphrase As String, ByVal data() As Byte) As Byte()
        ' Decrypt data. Use passphrase to generate the encryption key. 
        ' Returns a byte array that contains the decrypted data.

        ' make a copy of the encrypted data
        Dim dataCopy As Byte() = CType(data.Clone(), Byte())

        ' holds the decrypted data
        Dim buffer As Byte() = Nothing

        ' crypto handles
        Dim hProv As IntPtr = IntPtr.Zero
        Dim hKey As IntPtr = IntPtr.Zero

        Try
            ' get crypto provider, specify the provider (3rd argument)
            ' instead of using default to ensure the same provider is 
            ' used on client and server
            If Not WinApi.CryptAcquireContext(hProv, Nothing, WinApi.MS_DEF_PROV, WinApi.PROV_RSA_FULL, WinApi.CRYPT_VERIFYCONTEXT) Then
                Failed("CryptAcquireContext")
            End If

            ' generate encryption key from the passphrase
            hKey = GetCryptoKey(hProv, passphrase)

            ' decrypt the data
            Dim dataLength As Integer = dataCopy.Length
            If Not WinApi.CryptDecrypt(hKey, IntPtr.Zero, True, 0, dataCopy, dataLength) Then
                Failed("CryptDecrypt")
            End If

            ' copy to a buffer that is returned to the caller
            ' the decrypted data size might be less then
            ' the encrypted size
            buffer = New Byte(dataLength - 1) {}
            System.Buffer.BlockCopy(dataCopy, 0, buffer, 0, dataLength)
        Finally
            ' release crypto handles
            If Not hKey.Equals(IntPtr.Zero) Then
                WinApi.CryptDestroyKey(hKey)
            End If

            If Not hProv.Equals(IntPtr.Zero) Then
                WinApi.CryptReleaseContext(hProv, 0)
            End If
        End Try

        Return buffer
    End Function



    ' ENCRYPT
    Public Shared Function Encrypt(ByVal sPlainStr As String) As String
        ' Convert the sPlainStr passed in into a byte array
        Dim yPlain As Byte() = System.Text.Encoding.ASCII.GetBytes(sPlainStr)

        ' Encrypt the array of bytes of the plain string
        Dim yEncrypted As Byte() = Encrypt(msPassphrase, yPlain)

        ' Return the encrypted byte array as a string to be stored
        Dim sEncrypted As String = String.Empty
        Dim yByte As Byte
        For Each yByte In yEncrypted
            ' Build up a string of the encrypted bytes
            sEncrypted = sEncrypted & " " & yByte.ToString
        Next
        Return sEncrypted.Trim
    End Function



    ' PRIVATE SHARED ENCRYPT
    Private Shared Function Encrypt(ByVal passphrase As String, ByVal data() As Byte) As Byte()
        ' Encrypt data. Use passphrase to generate the encryption key. 
        ' Returns a byte array that contains the encrypted data.

        ' holds encrypted data
        Dim buffer As Byte() = Nothing

        ' crypto handles
        Dim hProv As IntPtr = IntPtr.Zero
        Dim hKey As IntPtr = IntPtr.Zero

        Try
            ' get crypto provider, specify the provider (3rd argument)
            ' instead of using default to ensure the same provider is 
            ' used on client and server
            If Not WinApi.CryptAcquireContext(hProv, Nothing, WinApi.MS_DEF_PROV, WinApi.PROV_RSA_FULL, WinApi.CRYPT_VERIFYCONTEXT) Then
                Failed("CryptAcquireContext")
            End If

            ' generate encryption key from passphrase
            hKey = GetCryptoKey(hProv, passphrase)

            ' determine how large of a buffer is required
            ' to hold the encrypted data
            Dim dataLength As Integer = data.Length
            Dim bufLength As Integer = data.Length

            If Not WinApi.CryptEncrypt(hKey, IntPtr.Zero, True, 0, Nothing, dataLength, bufLength) Then
                Failed("CryptEncrypt")
            End If

            ' allocate and fill buffer with encrypted data
            buffer = New Byte(dataLength - 1) {}
            System.Buffer.BlockCopy(data, 0, buffer, 0, data.Length)

            dataLength = data.Length
            bufLength = buffer.Length
            If Not WinApi.CryptEncrypt(hKey, IntPtr.Zero, True, 0, buffer, dataLength, bufLength) Then
                Failed("CryptEncrypt")
            End If
        Finally
            ' release crypto handles
            If Not hKey.Equals(IntPtr.Zero) Then
                WinApi.CryptDestroyKey(hKey)
            End If

            If Not hProv.Equals(IntPtr.Zero) Then
                WinApi.CryptReleaseContext(hProv, 0)
            End If
        End Try

        Return buffer
    End Function



    ' PRIVATE SHARED GET CRYPTO KEY
    Private Shared Function GetCryptoKey(ByVal hProv As IntPtr, ByVal passphrase As String) As IntPtr
        ' Create a crypto key form a passphrase. This key is 
        ' used to encrypt and decrypt data.

        ' crypto handles
        Dim hHash As IntPtr = IntPtr.Zero
        Dim hKey As IntPtr = IntPtr.Zero

        Try
            ' create 128 bit hash object
            If Not WinApi.CryptCreateHash(hProv, WinApi.CALG_MD5, IntPtr.Zero, 0, hHash) Then
                Failed("CryptCreateHash")
            End If

            ' add passphrase to hash
            Dim keyData As Byte() = ASCIIEncoding.ASCII.GetBytes(passphrase)
            If Not WinApi.CryptHashData(hHash, keyData, CType(keyData.Length, Integer), 0) Then
                Failed("CryptHashData")
            End If

            ' create 40 bit crypto key from passphrase hash
            If Not WinApi.CryptDeriveKey(hProv, WinApi.CALG_RC2, hHash, WinApi.CRYPT_EXPORTABLE, hKey) Then
                Failed("CryptDeriveKey")
            End If

        Finally
            ' release hash object
            If Not hHash.Equals(IntPtr.Zero) Then
                WinApi.CryptDestroyHash(hHash)
            End If
        End Try

        Return hKey
    End Function



    ' PRIVATE SHARED FAILED
    Private Shared Sub Failed(ByVal command As String)
        ' Throws SystemException with GetLastError information.
        Dim lastError As Integer = WinApi.GetLastError()
        Dim sb As New StringBuilder(500)

        Try
            ' get message for last error
            WinApi.FormatMessage(WinApi.FORMAT_MESSAGE_FROM_SYSTEM, Nothing, lastError, 0, sb, 500, Nothing)
        Catch
            ' error calling FormatMessage
            sb.Append("N/A.")
        End Try

        Throw New SystemException( _
         String.Format("{0} failed." + ControlChars.Cr + ControlChars.Lf + _
         "Last error - 0x{1:x}." + ControlChars.Cr + ControlChars.Lf + _
         "Error message - {2}", command, lastError, sb.ToString()))
    End Sub



    ' NEW
    Private Sub New()
        ' All static methods
    End Sub
End Class
