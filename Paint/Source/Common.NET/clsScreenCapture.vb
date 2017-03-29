' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsScreenCapture
'
' Description: Provides functions to capture the entire screen, or a particular window, 
'              and save it to a file.
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
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports System
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Drawing.Imaging


Namespace ScreenShot

    Friend Class ScreenCapture

        Friend Function CaptureScreen() As Image
            '********************************************************************************************
            'Description: Creates an Image object containing a screen shot of the entire desktop.
            '
            'Parameters: none
            'Returns:    Desktop Image
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Return CaptureWindow(User32.GetDesktopWindow())

        End Function

        Friend Function CaptureWindow(ByVal handle As IntPtr) As Image
            '********************************************************************************************
            'Description: Creates an Image object containing a screen shot of a specific window.
            '
            'Parameters: none
            'Returns:    Window Image
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Const SRCCOPY As Integer = &HCC0020
            Const CAPTUREBLT As Integer = &H40000000

            Dim hdcSrc As IntPtr = User32.GetWindowDC(handle) 'Get te hDC of the target window
            Dim windowRect As New User32.RECT                 'Get the size

            User32.GetWindowRect(handle, windowRect)

            Dim width As Integer = windowRect.right - windowRect.left
            Dim height As Integer = windowRect.bottom - windowRect.top
            Dim hdcDest As IntPtr = GDI32.CreateCompatibleDC(hdcSrc) 'Create a device context we can copy to
            'Create a bitmap we can copy it to, using GetDeviceCaps to get the width/height
            Dim hBitmap As IntPtr = GDI32.CreateCompatibleBitmap(hdcSrc, width, height)
            Dim hOld As IntPtr = GDI32.SelectObject(hdcDest, hBitmap) 'Select the bitmap object

            'Bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY Or CAPTUREBLT)
            'Restore selection
            GDI32.SelectObject(hdcDest, hOld)
            'Clean up 
            GDI32.DeleteDC(hdcDest)
            User32.ReleaseDC(handle, hdcSrc)

            Dim img As Image = Image.FromHbitmap(hBitmap) 'Get a .NET image object for it

            'Free up the Bitmap object
            GDI32.DeleteObject(hBitmap)

            Return img

        End Function

        Friend Sub CaptureWindowToFile(ByVal handle As IntPtr, ByVal filename As String, ByVal format As ImageFormat)
            '********************************************************************************************
            'Description: Captures a screen shot of a specific window, and saves it to a file.
            '
            'Parameters: Window Handle, File Name, File Format
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim img As Image = CaptureWindow(handle)

            img.Save(filename, format)

        End Sub

        Friend Sub CaptureScreenToFile(ByVal filename As String, ByVal format As ImageFormat)
            '********************************************************************************************
            'Description: Captures a screen shot of the entire desktop, and saves it to a file.
            '
            'Parameters: File Name, File Format
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim img As Image = CaptureScreen()

            img.Save(filename, format)

        End Sub

        Friend Function CaptureDeskTopRectangle(ByVal CapRect As Rectangle, ByVal CapRectWidth As Integer, ByVal CapRectHeight As Integer) As Bitmap
            '********************************************************************************************
            'Description: Returns BitMap of the region of the desktop, similar to CaptureWindow, but can 
            '             be used to create a snapshot of the desktop when no handle is present, by
            '             passing in a rectangle.
            '
            'Parameters: File Name, File Format
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            'Grabs snapshot of entire desktop, then crops it using the passed in rectangle's coordinates
            Dim SC As New ScreenShot.ScreenCapture
            Dim bmpImage As New Bitmap(SC.CaptureScreen)
            Dim bmpCrop As New Bitmap(CapRectWidth, CapRectHeight, bmpImage.PixelFormat)
            Dim recCrop As New Rectangle(CapRect.X, CapRect.Y, CapRectWidth, CapRectHeight)
            Dim gphCrop As Graphics = Graphics.FromImage(bmpCrop)
            Dim recDest As New Rectangle(0, 0, CapRectWidth, CapRectHeight)

            gphCrop.DrawImage(bmpImage, recDest, recCrop.X, recCrop.Y, recCrop.Width, _
                              recCrop.Height, GraphicsUnit.Pixel)
            Return bmpCrop

        End Function


        Private Class GDI32
            '********************************************************************************************
            ' Helper class containing Gdi32 API functions
            '********************************************************************************************

            ' BitBlt dwRop parameter
            Declare Function BitBlt Lib "gdi32.dll" ( _
                ByVal hDestDC As IntPtr, _
                ByVal x As Int32, _
                ByVal y As Int32, _
                ByVal nWidth As Int32, _
                ByVal nHeight As Int32, _
                ByVal hSrcDC As IntPtr, _
                ByVal xSrc As Int32, _
                ByVal ySrc As Int32, _
                ByVal dwRop As Int32) As Int32

            Declare Function CreateCompatibleBitmap Lib "gdi32.dll" ( _
                ByVal hdc As IntPtr, _
                ByVal nWidth As Int32, _
                ByVal nHeight As Int32) As IntPtr

            Declare Function CreateCompatibleDC Lib "gdi32.dll" ( _
                ByVal hdc As IntPtr) As IntPtr

            Declare Function DeleteDC Lib "gdi32.dll" ( _
                ByVal hdc As IntPtr) As Int32

            Declare Function DeleteObject Lib "gdi32.dll" ( _
                ByVal hObject As IntPtr) As Int32

            Declare Function SelectObject Lib "gdi32.dll" ( _
                ByVal hdc As IntPtr, _
                ByVal hObject As IntPtr) As IntPtr

        End Class 'GDI32

        Private Class User32
            '********************************************************************************************
            ' Helper class containing User32 API functions
            '********************************************************************************************
            <StructLayout(LayoutKind.Sequential)> _
            Public Structure RECT
                Public left As Integer
                Public top As Integer
                Public right As Integer
                Public bottom As Integer
            End Structure 'RECT

            Declare Function GetDesktopWindow Lib "user32.dll" () As IntPtr

            Declare Function GetWindowDC Lib "user32.dll" ( _
                ByVal hwnd As IntPtr) As IntPtr

            Declare Function ReleaseDC Lib "user32.dll" ( _
                ByVal hwnd As IntPtr, _
                ByVal hdc As IntPtr) As Int32

            Declare Function GetWindowRect Lib "user32.dll" ( _
                ByVal hwnd As IntPtr, _
                ByRef lpRect As RECT) As Int32

        End Class 'User32

    End Class 'ScreenCapture 

End Namespace 'ScreenShot
