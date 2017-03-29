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
' Form/Module: mMathFunctions
'
' Description: Math and Bit functions
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick O.
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    04/01/10   MSW     add  string length to ascii conversion routines
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    07/19/13   MSW     Add nBitMask function                                         4.01.05.00
'    04/21/14   MSW     GetBitState - Change to long data type                        4.01.07.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Friend Module mMathFunctions

#Region " Declares "

    Friend gnBitVal() As Integer
    Private mbUse32Bit As Boolean = True

#End Region

#Region " Properties "

    Friend Property Use32Bit() As Boolean
        '********************************************************************************************
        'Description:  Allow the program to specify 16 or 32 bit math
        '              Note: Default is 32-Bit
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Use32Bit = mbUse32Bit
        End Get
        Set(ByVal Value As Boolean)
            mbUse32Bit = Value
        End Set
    End Property

#End Region

#Region " Routines "

    Friend Function CvASCIIToInteger(ByVal AsciiWord As String, Optional ByVal StrLen As Integer = 4) As Integer
        '********************************************************************************************
        ' Description: This function converts a 1-4 character string and returns an integer with an
        '              ASCII representation of the rightmost character in bits 0-6 and the leftmost '
        '              character in bits 24-30.
        '
        ' Parameters:  1-4 character string (Example "RSTU")
        ' Returns:     32-bit integer (Example returns 1381192789)
        '
        ' Modification history:
        '
        ' Date      By      Reason
        ' 04/01/10  MSW     add  string length
        '********************************************************************************************
        Dim sTemp As String = Strings.Trim(AsciiWord)
        Dim chChar() As Char = Nothing
        Dim nValue As Integer = 0
        Dim nIndex As Integer = 0

        If Strings.Len(sTemp) = 0 Then Return nValue
        If Strings.Len(sTemp) < StrLen Then sTemp = "    " & sTemp
        If Strings.Len(sTemp) > StrLen Then sTemp = Strings.Right(sTemp, StrLen)
        chChar = sTemp.ToCharArray

        For nIndex = (Strings.Len(sTemp) - 1) To 0 Step -1
            nValue = nValue + (Strings.Asc(chChar((Strings.Len(sTemp) - nIndex) - 1)) * CType(2 ^ (nIndex * 8), Integer))
        Next

        Return nValue

    End Function

    Friend Function CvBin(ByVal DataIn As Integer, ByVal StrLen As Integer) As String
        '********************************************************************************************
        'Description: Converts a Long to a variable length binary string. Example: 327,16 returns
        '             "0000000101000111"
        '
        'Parameters: DataIn - Number to convert
        '            StrLen - length of output string
        'Returns:   Binary String
        '
        ' Modification history:
        '
        ' Date      By      Reason
        ' 06/06/00  MP      Fixed bug where this routine would return all zeros if the sign bit was
        '                   set in DataIn (i.e. DataIn was negative).
        ' 05/22/07  Geo     .net
        ' 09/05/07  gks     recommended change from fxCop - untested
        '********************************************************************************************
        'Dim OutString As String
        Dim OutString As New System.Text.StringBuilder(String.Empty)
        Dim x As Integer
        Dim Mask As Integer


        'OutString = ""
        For x = 0 To StrLen - 1
            If x < 31 Then
                Mask = CInt(2 ^ x)
                If CBool(DataIn And Mask) Then
                    'OutString = "1" & OutString
                    OutString.Insert(0, "1")
                Else
                    'OutString = "0" & OutString
                    OutString.Insert(0, "0")
                End If
            Else
                If DataIn < 0 Then
                    'OutString = "1" & OutString
                    OutString.Insert(0, "1")
                Else
                    'OutString = "0" & OutString
                    OutString.Insert(0, "0")
                End If
            End If
        Next  'X
        CvBin = OutString.ToString

    End Function
    Friend Function CvInteger(ByVal DataString As String, _
                            Optional ByVal bForce32Bit As Boolean = True) As Integer 'Double
        '********************************************************************************************
        'Description: Converts binary string to an integer. Example: input string "1010101" returns
        '             85.
        '
        'Parameters: DataString - string of 1's and 0's
        'Returns:   Long Integer value
        '
        ' Modification history:
        '
        ' By          Date          Reason
        ' almost straight from VB6 - Just for old times sake....(yes it did return a double)
        '********************************************************************************************

        Dim NewVal As Integer 'long
        Dim StrLen As Integer
        Dim Loopie As Integer
        Dim b32Bit As Boolean


        NewVal = 0
        StrLen = Len(DataString)

        If (StrLen > 16) Or bForce32Bit Then
            b32Bit = True
        End If

        For Loopie = 0 To (StrLen - 1)

            If Mid$(DataString, StrLen - Loopie, 1) = "1" Then

                If b32Bit Then
                    If Loopie < 31 Then
                        NewVal = CInt(NewVal + 2 ^ Loopie)
                    Else
                        NewVal = CInt(NewVal + ((2 ^ Loopie) - 4294967296.0#))
                    End If
                Else
                    If Loopie < 15 Then
                        NewVal = CInt(NewVal + 2 ^ Loopie)
                    Else
                        NewVal = CInt(NewVal + ((2 ^ Loopie) - 65536))
                    End If
                End If 'b32Bit

            End If 'Mid(DataString, StrLen - Loopie, 1) = "1"

        Next 'Loopie

        CvInteger = NewVal

    End Function

    Friend Function CvIntegerToASCII(ByVal nNumToConvert As Integer, Optional ByVal StrLen As Integer = 4) As String
        '********************************************************************************************
        ' Description: This function will convert 1-4 character ascii stored in an integer value
        '              to a 1-4 character string value. 
        ' Parameters:  integer  leftmost char in bits 24-30, rightmost char in bits (0-6)
        '              Example(1381192789)
        ' Returns:     1-4 character string (Example parameter returns "RSTU"
        '
        ' Modification history:
        '
        ' Date      By      Reason
        ' 04/01/10  MSW     add  string length
        '********************************************************************************************
        ' 6/7/07   Geo      .net

        Dim nChar As Integer = 0
        Dim sTmp As String = String.Empty

        ' Geo 6/7/07 sub was faulting with over load so send a string of zeros if numtoconvert = 0
        If nNumToConvert = 0 Then
            For nIdx As Integer = 1 To StrLen
                sTmp = sTmp & "0"
            Next
            Return sTmp
        End If
        'We could do this in a fancy loop but this is easier to understand
        If StrLen >= 4 Then 'Leftmost char (bits 24-30)
            nChar = (nNumToConvert And &H7F000000) \ 16777216 '2^24
            'Mask out control chars (0-33)
            If nChar > 33 Then sTmp = Strings.Chr(nChar).ToString
        End If
        If StrLen >= 3 Then 'Char 2 (bits 16-22)
            nChar = (nNumToConvert And &H7F0000) \ 65536 '2^16
            If nChar > 33 Then sTmp = sTmp & Strings.Chr(nChar).ToString
        End If
        If StrLen >= 2 Then 'Char 3 (bits 8-14)
            nChar = (nNumToConvert And &H7F00) \ 256 '2^8
            If nChar > 33 Then sTmp = sTmp & Strings.Chr(nChar).ToString
        End If
        'Rightmost char (bits 0-6)
        nChar = nNumToConvert And &H7F
        If nChar > 33 Then sTmp = sTmp & Strings.Chr(nChar).ToString

        Return sTmp

    End Function

    Friend Sub FillBitValArray()
        '********************************************************************************************
        'Description:  fill the arrays to cut down on the math
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nBit As Integer

        If Use32Bit Then
            ReDim gnBitVal(31)
            For nBit = 0 To 30
                gnBitVal(nBit) = CType(2 ^ nBit, Integer)
            Next
            gnBitVal(31) = -2147483648
        Else 'Use 16-bit
            ReDim gnBitVal(15)
            For nBit = 0 To 14
                gnBitVal(nBit) = CType(2 ^ nBit, Integer)
            Next
            gnBitVal(15) = -32767
        End If

    End Sub

    Friend Function GetBitState(ByVal Word As Integer, ByVal Bit As Integer) As Integer
        '********************************************************************************************
        'Description:   Returns the status of the specified Bit in an integer.
        '
        'Parameters:    Word - PLC data table word value
        '               Bit - bit number to test
        'Returns:       Bit State - 1=ON, 0=OFF
        '
        ' Modification history:
        '
        ' By          Date          Reason
        ' MSW         04/21/14      GetBitState - Change to long data type
        '********************************************************************************************
        'Geo        6/12/07         .net conversion

        Dim lValue As Long
        If Bit < 31 Then
            If CBool(Word And CType(2 ^ Bit, Integer)) Then
                GetBitState = 1
            Else
                GetBitState = 0
            End If
        Else
            lValue = Word And (CType(2 ^ Bit, Long) - 4294967296)
            If lValue <> 0 Then
                GetBitState = 1
            Else
                GetBitState = 0
            End If
        End If

    End Function

    Friend Function ReSetBit(ByVal Word As Integer, ByVal Bit As Integer, _
                         Optional ByVal b32Bit As Boolean = False) As Integer
        '********************************************************************************************
        'Description: Resets (to 0) the specified bit in the Word argument.
        '
        'Parameters: Word - PLC Data Table word
        '            Bit - bit number to reset
        'Returns:    Value of Word with specified bit reset (FALSE)
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '09/19/02   AM      Modified to support ContolLogix (32 bit) PLC.
        '10/03/03   RJO     Added optional b32Bit param to force 32 bit mode because ControlLogix
        '                   also supports 16 bit integers. Modified 16 bit code to return a
        '                   negative number if sign bit is set.
        '********************************************************************************************

        'Create an inverted mask of the bit
        If b32Bit Then
            '32 bit
            If Bit < 31 Then
                ReSetBit = Word And (Not CInt(2 ^ Bit) And &HFFFFFFFF)
            Else
                ReSetBit = CInt(Word And (Not ((CInt(2 ^ Bit) - 4294967296) And &HFFFFFFFF)))
            End If
        Else
            '16 bit
            ReSetBit = CInt(Word And (Not CInt(2 ^ Bit) And &HFFFF&))
            If ReSetBit > 32767 Then ReSetBit = ReSetBit - 65536
        End If

    End Function

    Friend Function SetBit(ByVal Word As Integer, ByVal Bit As Integer, _
                           Optional ByVal b32Bit As Boolean = False) As Integer
        '********************************************************************************************
        'Description: Sets (to 1) the specified bit in the Word argument.
        '
        'Parameters: Word - PLC Data Table word
        '            Bit - bit number to set
        'Returns:    Value of Word with specified bit set (TRUE)
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '09/19/02   AM      Modified to support ContolLogix (32 bit) PLC.
        '10/03/03   RJO     Added optional b32Bit param to force 32 bit mode because ControlLogix
        '                   also supports 16 bit integers. Modified 16 bit code to return a
        '                   negative number if sign bit is set.
        '********************************************************************************************


        If b32Bit Then
            If Bit < 31 Then
                SetBit = Word Or CInt(2 ^ Bit)
            Else
                SetBit = CInt(Word Or (CInt(2 ^ Bit) - 4294967296))  '100000000000000000000000000000000
            End If
        Else '16 bit
            SetBit = Word Or CInt(2 ^ Bit)
            If SetBit > 32767 Then SetBit = SetBit - 65536
        End If

    End Function
    Friend Function nBitMask(ByVal nBit As Integer) As Integer
       '********************************************************************************************
        'Description: Provide the integer value of a bit
        '
        'Parameters: nBit - bit number starting with 0
        '         
        'Returns:    Value of Word with specified bit set (TRUE)
        '
        ' Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case nBit
            Case 0
                nBitMask = 1
            Case 1
                nBitMask = 2
            Case 2
                nBitMask = 4
            Case 3
                nBitMask = 8
            Case 4
                nBitMask = 16
            Case 5
                nBitMask = 32
            Case 6
                nBitMask = 64
            Case 7
                nBitMask = 128
            Case 8
                nBitMask = 256
            Case 9
                nBitMask = 512
            Case 10
                nBitMask = 1024
            Case 11
                nBitMask = 2048
            Case 12
                nBitMask = 4096
            Case 13
                nBitMask = 8192
            Case 14
                nBitMask = 16384
            Case 15
                nBitMask = 32768
            Case Else
                nBitMask = CInt(2 ^ nBit)
        End Select
    End Function
#End Region

End Module
