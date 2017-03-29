' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: mPassword
'
' Description: temp thing for old password - Ralph!
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Friend Module mPassword

#Region " Declares "

    ' this enum is for password privilege levels
    Friend Enum ePrivilege As Integer
        None = CType(2 ^ 0, Integer)
        Edit = CType(2 ^ 1, Integer)
        Copy = CType(2 ^ 2, Integer)
        Delete = CType(2 ^ 3, Integer)
        Administrate = CType(2 ^ 4, Integer)
        Restore = CType(2 ^ 5, Integer)
        Execute = CType(2 ^ 6, Integer)
        Remote = CType(2 ^ 7, Integer)
    End Enum

    Friend Const gsPASSWORDFILE As String = "Security.MDB"

#End Region
#Region " Routines "

    Friend Sub InitPassword(ByRef rPassword As PWPassword.PasswordObject, _
                            ByRef rPrivilege As PWPassword.cPrivilegeObject, _
                            ByRef rUser As String, ByVal ScreenName As String)

        '****************************************************************************************
        'Description: fire up the old password
        '
        'Parameters: enum
        'Returns:   string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim sTmp As String = String.Empty

        'assuming that we want the database for passwords on local computer
        If mPWCommon.GetDefaultFilePath(sTmp, eDir.Database, String.Empty, gsPASSWORDFILE) Then
            rPassword.DatabaseLocation = sTmp

            rPassword.Initialize()

            rPrivilege = rPassword.PrivilegeObject

            ' set screen
            rPrivilege.ScreenName = ScreenName

            rUser = rPassword.UserName

            'standard priv
            If rPassword.UserName <> String.Empty Then
                rPrivilege.Privilege = "edit"
            End If

        Else

            MessageBox.Show(gcsRM.GetString("csUNABLE_TO_ACCESS_DB"), _
                            gcsRM.GetString("csPAINTWORKS"), _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, _
                            MessageBoxOptions.DefaultDesktopOnly)

        End If
    End Sub
    Friend Function PrivilegeStringToEnum(ByVal sPrivilege As String) As ePrivilege
        '****************************************************************************************
        'Description: for old object compatability
        '
        'Parameters: string
        'Returns:   enum
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Select Case LCase(sPrivilege)
            Case "edit"
                Return ePrivilege.Edit
            Case "administrate"
                Return ePrivilege.Administrate
            Case "copy"
                Return ePrivilege.Copy
            Case "delete"
                Return ePrivilege.Delete
            Case "execute"
                Return ePrivilege.Execute
            Case "restore"
                Return ePrivilege.Restore
            Case Else
                Return ePrivilege.None
        End Select
    End Function
    Friend Function PrivilegeEnumToString(ByVal ePrivilege As ePrivilege) As String
        '****************************************************************************************
        'Description: for old object compatability
        '
        'Parameters: enum
        'Returns:   string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Select Case ePrivilege
            Case ePrivilege.Edit
                Return "edit"
            Case ePrivilege.Administrate
                Return "administrate"
            Case ePrivilege.Copy
                Return "copy"
            Case ePrivilege.Delete
                Return "delete"
            Case ePrivilege.Execute
                Return "execute"
            Case ePrivilege.Restore
                Return "restore"
            Case ePrivilege.Remote
                Return "remote"
            Case Else
                Return "none"
        End Select
    End Function
    Friend Function CheckPassword(ByVal sArea As String, ByVal ePrivilege As ePrivilege, _
                                    ByVal rPassword As PWPassword.PasswordObject, _
                                    ByVal rPrivilege As PWPassword.cPrivilegeObject) As Boolean
        '****************************************************************************************
        'Description: Check password for desired privilege show login if necessary
        '
        'Parameters: area and privilege and who to blame
        'Returns:   true if OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            If rPassword.UserName = String.Empty Then
                'show login box
                rPassword.DisplayLogin()

                'this doesn't wait around
                Return False

            Else
                rPrivilege.ScreenName = sArea
                Dim oldPriv As String = rPrivilege.Privilege
                rPrivilege.Privilege = PrivilegeEnumToString(ePrivilege)
                If rPrivilege.ActionAllowed Then
                    Return True
                Else
                    rPrivilege.Privilege = oldPriv
                    Return False
                End If
            End If

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            'most all errors should be caught in subroutines
            mDebug.ShowErrorMessagebox(ex.Message, ex, System.Windows.Forms.Application.ProductName, _
                                    frmMain.Status, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End Try

    End Function

#End Region

End Module
