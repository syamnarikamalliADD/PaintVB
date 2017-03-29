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
' Form/Module: mZip
'
' Description: zip dll interface class
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: VB6 code converter
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    07/08/2010 MSW     Ran VB6 code through converter and poked at it till it worked
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Imports System.IO
Public Class mZip

    'UPGRADE_WARNING: Structure ZIPUSERFUNCTIONS may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Function ZpInit Lib "zip32.dll" (ByRef Zipfun As ZIPUSERFUNCTIONS) As Integer '-- Set Zip Callbacks
    'UPGRADE_WARNING: Structure ZPOPT may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Function ZpSetOptions Lib "zip32.dll" (ByRef Opts As ZPOPT) As Integer '-- Set Zip Options
    Private Declare Function ZpGetOptions Lib "zip32.dll" () As ZPOPT '-- Used To Check Encryption Flag Only
    'UPGRADE_WARNING: Structure ZIPnames may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Function ZpArchive Lib "zip32.dll" (ByVal argc As Integer, ByVal funame As String, ByRef argv As ZIPnames) As Integer '-- Real Zipping Action
    'UPGRADE_WARNING: Structure USERFUNCTION may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    'UPGRADE_WARNING: Structure DCLIST may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    'UPGRADE_WARNING: Structure UNZIPnames may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    'UPGRADE_WARNING: Structure UNZIPnames may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Function Wiz_SingleEntryUnzip Lib "unzip32.dll" (ByVal ifnc As Integer, ByRef ifnv As UNZIPnames, ByVal xfnc As Integer, ByRef xfnv As UNZIPnames, ByRef dcll As DCLIST, ByRef Userf As USERFUNCTION) As Integer
    'UPGRADE_WARNING: Structure UZPVER may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Sub UzpVersion2 Lib "unzip32.dll" (ByRef uzpv As UZPVER)
    Private Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Integer) As Integer
    'PW_VERSION 4.00.0000
    '
    ' Form/Module: mZip.bas
    '
    ' Description: This module is a free replacement for the WinZip
    '              command-line functions used in Paintworks applications.
    '              See mZipReadMe.txt for details of how to use this module.
    '
    ' Dependencies: Windows\System32\zip32.dll, Windows\System32\unzip32.dll
    '
    ' Author: Various Authors - see comment sections below.
    '
    ' Modification history:
    '
    ' Version       By          Date
    '*************************************************************************
    ' 3.06.0000     RJO     01/30/08
    'Combined and modified two original sample code modules to accomodate
    'Paintworks applications. The sample code, ugly as it is, seems to
    'work so I ain't touchin' it! There's a lot of extra stuff in here that
    'we might be able to use, someday.
    '

    '*************************************************************************
    ' The comment section below is an acknowledgement to the authors of the
    ' sample code for using Info-Zip Zip32.dll. This code is graciously
    ' provided free of charge with only one request - that the comments remain.
    '**************************************************************************

    '---------------------------------------------------------------
    '-- Please Do Not Remove These Comments!!!
    '---------------------------------------------------------------
    '-- Sample VB 5 code to drive zip32.dll
    '-- Contributed to the Info-ZIP project by Mike Le Voi
    '--
    '-- Contact Me at: mlevoi@modemss.brisnet.org.au
    '--
    '-- Visit my home page at: http://modemss.brisnet.org.au/~mlevoi
    '--
    '-- Use this code at your own risk. Nothing implied or warranted
    '-- to work on your machine :-)
    '---------------------------------------------------------------
    '--
    '-- The Source Code Is Freely Available From Info-ZIP At:
    '-- http://www.cdrom.com/pub/infozip/infozip.html
    '--
    '-- A Very Special Thanks To Mr. Mike Le Voi
    '-- And Mr. Mike White Of The Info-ZIP
    '-- For Letting Me Use And Modify His Orginal
    '-- Visual Basic 5.0 Code! Thank You Mike Le Voi.
    '---------------------------------------------------------------
    '--
    '-- Contributed To The Info-ZIP Project By Raymond L. King
    '-- Modified June 21, 1998
    '-- By Raymond L. King
    '-- Custom Software Designers
    '--
    '-- Contact Me At: king@ntplx.net
    '-- ICQ 434355
    '-- Or Visit Our Home Page At: http://www.ntplx.net/~king
    '--
    '---------------------------------------------------------------
    '
    ' This is the original example with some small changes. Only
    ' use with the original Zip32.dll (Zip 2.3).  Do not use this VB
    ' example with Zip32z64.dll (Zip 3.0).
    '
    ' 4/29/2004 Ed Gordon

    '---------------------------------------------------------------
    ' Usage notes:
    '
    ' This code uses Zip32.dll.  You DO NOT need to register the
    ' DLL to use it.  You also DO NOT need to reference it in your
    ' VB project.  You DO have to copy the DLL to your SYSTEM
    ' directory, your VB project directory, or place it in a directory
    ' on your command PATH.
    '
    ' A bug has been found in the Zip32.dll when called from VB.  If
    ' you try to pass any values other than NULL in the ZPOPT strings
    ' Date, szRootDir, or szTempDir they get converted from the
    ' VB internal wide character format to temporary byte strings by
    ' the calling interface as they are supposed to.  However when
    ' ZpSetOptions returns the passed strings are deallocated unless the
    ' VB debugger prevents it by a break between ZpSetOptions and
    ' ZpArchive.  When Zip32.dll uses these pointers later it
    ' can result in unpredictable behavior.  A kluge is available
    ' for Zip32.dll, just replacing api.c in Zip 2.3, but better to just
    ' use the new Zip32z64.dll where these bugs are fixed.  However,
    ' the kluge has been added to Zip 2.31.  To determine the version
    ' of the dll you have right click on it, select the Version tab,
    ' and verify the Product Version is at least 2.31.
    '
    ' Another bug is where -R is used with some other options and can
    ' crash the dll.  This is a bug in how zip processes the command
    ' line and should be mostly fixed in Zip 2.31.  If you run into
    ' problems try using -r instead for recursion.  The bug is fixed
    ' in Zip 3.0 but note that Zip 3.0 creates dll zip32z64.dll and
    ' it is not compatible with older VB including this example.  See
    ' the new VB example code included with Zip 3.0 for calling
    ' interface changes.
    '
    ' Note that Zip32 is probably not thread safe.  It may be made
    ' thread safe in a later version, but for now only one thread in
    ' one program should use the DLL at a time.  Unlike Zip, UnZip is
    ' probably thread safe, but an exception to this has been
    ' found.  See the UnZip documentation for the latest on this.
    '
    ' All code in this VB project is provided under the Info-Zip license.
    '
    ' If you have any questions please contact Info-Zip at
    ' http://www.info-zip.org.
    '
    ' 4/29/2004 EG (Updated 3/1/2005 EG)
    '
    '---------------------------------------------------------------

    '*************************************************************************
    ' The comment section below is an acknowledgement to the authors of the
    ' sample code for using Info-Zip Unzip32.dll. This code is graciously
    ' provided free of charge with only one request - that the comments remain.
    '**************************************************************************

    '-- Please Do Not Remove These Comment Lines!
    '----------------------------------------------------------------
    '-- Sample VB 5 / VB 6 code to drive unzip32.dll
    '-- Contributed to the Info-ZIP project by Mike Le Voi
    '--
    '-- Contact Me at: mlevoi@modemss.brisnet.org.au
    '--
    '-- Visit my home page at: http://modemss.brisnet.org.au/~mlevoi
    '--
    '-- Use this code at your own risk. Nothing implied or warranted
    '-- to work on your machine :-)
    '----------------------------------------------------------------
    '--
    '-- This Source Code Is Freely Available From The Info-ZIP Project
    '-- Web Server At:
    '-- ftp://ftp.info-zip.org/pub/infozip/infozip.html
    '--
    '-- A Very Special Thanks To Mr. Mike Le Voi
    '-- And Mr. Mike White
    '-- And The Fine People Of The Info-ZIP Group
    '-- For Letting Me Use And Modify Their Original
    '-- Visual Basic 5.0 Code! Thank You Mike Le Voi.
    '-- For Your Hard Work In Helping Me Get This To Work!!!
    '---------------------------------------------------------------
    '--
    '-- Contributed To The Info-ZIP Project By Raymond L. King.
    '-- Modified June 21, 1998
    '-- By Raymond L. King
    '-- Custom Software Designers
    '--
    '-- Contact Me At: king@ntplx.net
    '-- ICQ 434355
    '-- Or Visit Our Home Page At: http://www.ntplx.net/~king
    '--
    '---------------------------------------------------------------
    '--
    '-- Modified August 17, 1998
    '-- by Christian Spieler
    '-- (implemented sort of a "real" user interface)
    '-- Modified May 11, 2003
    '-- by Christian Spieler
    '-- (use late binding for referencing the common dialog)
    '--
    '---------------------------------------------------------------

    '*************************************************************************
    ' Declarations
    '*************************************************************************
    Delegate Sub UZDLLPrntCallBack()
    Delegate Sub UZDLLRepCallBack()
    Delegate Sub UZDLLPassCallBack()
    Delegate Sub UZReceiveDLLMessageCallBack()
    Delegate Sub UZDLLServCallBack()

    '*************************************************************************
    ' Declarations for Zip32.dll
    '*************************************************************************

    '-- C Style argv
    '-- Holds The Zip Archive Filenames
    ' Max for this just over 8000 as each pointer takes up 4 bytes and
    ' VB only allows 32 kB of local variables and that includes function
    ' parameters.  - 3/19/2004 EG
    Public Structure ZIPnames
        <VBFixedArray(7999)> Dim zFiles() As String

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim zFiles(7999)
        End Sub
    End Structure

    '-- Call Back "String"
    Public Structure ZipCBChar
        <VBFixedArray(4096)> Dim ch() As Byte

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim ch(4096)
        End Sub
    End Structure

    '-- ZPOPT Is Used To Set The Options In The ZIP32.DLL
    Public Structure ZPOPT
        'UPGRADE_NOTE: Date was upgraded to Date_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Date_Renamed As String ' US Date (8 Bytes Long) "12/31/98"?
        Dim szRootDir As String ' Root Directory Pathname (Up To 256 Bytes Long)
        Dim szTempDir As String ' Temp Directory Pathname (Up To 256 Bytes Long)
        Dim fTemp As Integer ' 1 If Temp dir Wanted, Else 0
        Dim fSuffix As Integer ' Include Suffixes (Not Yet Implemented!)
        Dim fEncrypt As Integer ' 1 If Encryption Wanted, Else 0
        Dim fSystem As Integer ' 1 To Include System/Hidden Files, Else 0
        Dim fVolume As Integer ' 1 If Storing Volume Label, Else 0
        Dim fExtra As Integer ' 1 If Excluding Extra Attributes, Else 0
        Dim fNoDirEntries As Integer ' 1 If Ignoring Directory Entries, Else 0
        Dim fExcludeDate As Integer ' 1 If Excluding Files Earlier Than Specified Date, Else 0
        Dim fIncludeDate As Integer ' 1 If Including Files Earlier Than Specified Date, Else 0
        Dim fVerbose As Integer ' 1 If Full Messages Wanted, Else 0
        Dim fQuiet As Integer ' 1 If Minimum Messages Wanted, Else 0
        Dim fCRLF_LF As Integer ' 1 If Translate CR/LF To LF, Else 0
        Dim fLF_CRLF As Integer ' 1 If Translate LF To CR/LF, Else 0
        Dim fJunkDir As Integer ' 1 If Junking Directory Names, Else 0
        Dim fGrow As Integer ' 1 If Allow Appending To Zip File, Else 0
        Dim fForce As Integer ' 1 If Making Entries Using DOS File Names, Else 0
        Dim fMove As Integer ' 1 If Deleting Files Added Or Updated, Else 0
        Dim fDeleteEntries As Integer ' 1 If Files Passed Have To Be Deleted, Else 0
        Dim fUpdate As Integer ' 1 If Updating Zip File-Overwrite Only If Newer, Else 0
        Dim fFreshen As Integer ' 1 If Freshing Zip File-Overwrite Only, Else 0
        Dim fJunkSFX As Integer ' 1 If Junking SFX Prefix, Else 0
        Dim fLatestTime As Integer ' 1 If Setting Zip File Time To Time Of Latest File In Archive, Else 0
        Dim fComment As Integer ' 1 If Putting Comment In Zip File, Else 0
        Dim fOffsets As Integer ' 1 If Updating Archive Offsets For SFX Files, Else 0
        Dim fPrivilege As Integer ' 1 If Not Saving Privileges, Else 0
        Dim fEncryption As Integer ' Read Only Property!!!
        Dim fRecurse As Integer ' 1 (-r), 2 (-R) If Recursing Into Sub-Directories, Else 0
        Dim fRepair As Integer ' 1 = Fix Archive, 2 = Try Harder To Fix, Else 0
        Dim flevel As Byte ' Compression Level - 0 = Stored 6 = Default 9 = Max
    End Structure

    '-- This Structure Is Used For The ZIP32.DLL Function Callbacks
    Public Structure ZIPUSERFUNCTIONS
        Dim ZDLLPrnt As Integer ' Callback ZIP32.DLL Print Function
        Dim ZDLLCOMMENT As Integer ' Callback ZIP32.DLL Comment Function
        Dim ZDLLPASSWORD As Integer ' Callback ZIP32.DLL Password Function
        Dim ZDLLSERVICE As Integer ' Callback ZIP32.DLL Service Function
    End Structure

    '-- Local Declarations
    Public ZOPT As ZPOPT
    Public ZUSER As ZIPUSERFUNCTIONS

    '-- This Assumes ZIP32.DLL Is In Your \Windows\System Directory!
    '-- (alternatively, a copy of ZIP32.DLL needs to be located in the program
    '-- directory or in some other directory listed in PATH.)




    '-------------------------------------------------------
    '-- Public Variables For Setting The ZPOPT Structure...
    '-- (WARNING!!!) You Must Set The Options That You
    '-- Want The ZIP32.DLL To Do!
    '-- Before Calling VBZip32!
    '--
    '-- NOTE: See The Above ZPOPT Structure Or The VBZip32
    '--       Function, For The Meaning Of These Variables
    '--       And How To Use And Set Them!!!
    '-- These Parameters Must Be Set Before The Actual Call
    '-- To The VBZip32 Function!
    '-------------------------------------------------------
    Public zDate As String
    Public zRootDir As String
    Public zTempDir As String
    Public zSuffix As Short
    Public zEncrypt As Short
    Public zSystem As Short
    Public zVolume As Short
    Public zExtra As Short
    Public zNoDirEntries As Short
    Public zExcludeDate As Short
    Public zIncludeDate As Short
    Public zVerbose As Short
    Public zQuiet As Short
    Public zCRLF_LF As Short
    Public zLF_CRLF As Short
    Public zJunkDir As Short
    Public zRecurse As Short
    Public zGrow As Short
    Public zForce As Short
    Public zMove As Short
    Public zDelEntries As Short
    Public zUpdate As Short
    Public zFreshen As Short
    Public zJunkSFX As Short
    Public zLatestTime As Short
    Public zComment As Short
    Public zOffsets As Short
    Public zPrivilege As Short
    Public zEncryption As Short
    Public zRepair As Short
    Public zLevel As Short

    '-- Public Program Variables
    Public zArgc As Short ' Number Of Files To Zip Up
    Public zZipFileName As String ' The Zip File Name ie: Myzip.zip
    'UPGRADE_WARNING: Arrays in structure zZipFileNames may need to be initialized before they can be used. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
    Public zZipFileNames As ZIPnames ' File Names To Zip Up
    Public zZipInfo As String ' Holds The Zip File Information


    '*************************************************************************
    ' Declarations for Unzip32.dll
    '*************************************************************************
    '-- C Style argv
    Public Structure UNZIPnames
        <VBFixedArray(999)> Dim uzFiles() As String

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim uzFiles(999)
        End Sub
    End Structure

    '-- Callback Large "String"
    Public Structure UNZIPCBChar
        <VBFixedArray(32800)> Dim ch() As Byte

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim ch(32800)
        End Sub
    End Structure

    '-- Callback Small "String"
    Public Structure UNZIPCBCh
        <VBFixedArray(256)> Dim ch() As Byte

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim ch(256)
        End Sub
    End Structure

    '-- UNZIP32.DLL DCL Structure
    Public Structure DCLIST
        Dim ExtractOnlyNewer As Integer ' 1 = Extract Only Newer/New, Else 0
        Dim SpaceToUnderscore As Integer ' 1 = Convert Space To Underscore, Else 0
        Dim PromptToOverwrite As Integer ' 1 = Prompt To Overwrite Required, Else 0
        Dim fQuiet As Integer ' 2 = No Messages, 1 = Less, 0 = All
        Dim ncflag As Integer ' 1 = Write To Stdout, Else 0
        Dim ntflag As Integer ' 1 = Test Zip File, Else 0
        Dim nvflag As Integer ' 0 = Extract, 1 = List Zip Contents
        Dim nfflag As Integer ' 1 = Extract Only Newer Over Existing, Else 0
        Dim nzflag As Integer ' 1 = Display Zip File Comment, Else 0
        Dim ndflag As Integer ' 1 = Honor Directories, Else 0
        Dim noflag As Integer ' 1 = Overwrite Files, Else 0
        Dim naflag As Integer ' 1 = Convert CR To CRLF, Else 0
        Dim nZIflag As Integer ' 1 = Zip Info Verbose, Else 0
        Dim C_flag As Integer ' 1 = Case Insensitivity, 0 = Case Sensitivity
        Dim fPrivilege As Integer ' 1 = ACL, 2 = Privileges
        Dim Zip As String ' The Zip Filename To Extract Files
        Dim ExtractDir As String ' The Extraction Directory, NULL If Extracting To Current Dir
    End Structure

    '-- UNZIP32.DLL Userfunctions Structure
    Private Structure USERFUNCTION
        Dim UZDLLPrnt As Integer ' Pointer To Apps Print Function
        Dim UZDLLSND As Integer ' Pointer To Apps Sound Function
        Dim UZDLLREPLACE As Integer ' Pointer To Apps Replace Function
        Dim UZDLLPASSWORD As Integer ' Pointer To Apps Password Function
        Dim UZDLLMESSAGE As Integer ' Pointer To Apps Message Function
        Dim UZDLLSERVICE As Integer ' Pointer To Apps Service Function (Not Coded!)
        Dim TotalSizeComp As Integer ' Total Size Of Zip Archive
        Dim TotalSize As Integer ' Total Size Of All Files In Archive
        Dim CompFactor As Integer ' Compression Factor
        Dim NumMembers As Integer ' Total Number Of All Files In The Archive
        Dim cchComment As Short ' Flag If Archive Has A Comment!
    End Structure

    '-- UNZIP32.DLL Version Structure
    Private Structure UZPVER
        Dim structlen As Integer ' Length Of The Structure Being Passed
        Dim flag As Integer ' Bit 0: is_beta  bit 1: uses_zlib
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        <VBFixedString(10), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst:=10)> Public beta() As Char ' e.g., "g BETA" or ""
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        'UPGRADE_NOTE: Date was upgraded to Date_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        <VBFixedString(20), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst:=20)> Public Date_Renamed() As Char ' e.g., "4 Sep 95" (beta) or "4 September 1995"
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        <VBFixedString(10), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst:=10)> Public zlib() As Char ' e.g., "1.0.5" or NULL
        <VBFixedArray(4)> Dim unzip() As Byte ' Version Type Unzip
        <VBFixedArray(4)> Dim zipinfo() As Byte ' Version Type Zip Info
        Dim os2dll As Integer ' Version Type OS2 DLL
        <VBFixedArray(4)> Dim windll() As Byte ' Version Type Windows DLL

        'UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            'UPGRADE_WARNING: Lower bound of array unzip was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim unzip(4)
            'UPGRADE_WARNING: Lower bound of array zipinfo was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim zipinfo(4)
            'UPGRADE_WARNING: Lower bound of array windll was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim windll(4)
        End Sub
    End Structure

    '-- This Assumes UNZIP32.DLL Is In Your \Windows\System Directory!


    '-- Private Variables For Structure Access
    Private UZDCL As DCLIST
    Private UZUSER As USERFUNCTION
    'UPGRADE_WARNING: Arrays in structure UZVER may need to be initialized before they can be used. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
    Private UZVER As UZPVER

    '-- Public Variables For Setting The
    '-- UNZIP32.DLL DCLIST Structure
    '-- These Must Be Set Before The Actual Call To VBUnZip32
    Public uExtractOnlyNewer As Short ' 1 = Extract Only Newer/New, Else 0
    Public uSpaceUnderScore As Short ' 1 = Convert Space To Underscore, Else 0
    Public uPromptOverWrite As Short ' 1 = Prompt To Overwrite Required, Else 0
    Public uQuiet As Short ' 2 = No Messages, 1 = Less, 0 = All
    Public uWriteStdOut As Short ' 1 = Write To Stdout, Else 0
    Public uTestZip As Short ' 1 = Test Zip File, Else 0
    Public uExtractList As Short ' 0 = Extract, 1 = List Contents
    Public uFreshenExisting As Short ' 1 = Update Existing by Newer, Else 0
    Public uDisplayComment As Short ' 1 = Display Zip File Comment, Else 0
    Public uHonorDirectories As Short ' 1 = Honor Directories, Else 0
    Public uOverWriteFiles As Short ' 1 = Overwrite Files, Else 0
    Public uConvertCR_CRLF As Short ' 1 = Convert CR To CRLF, Else 0
    Public uVerbose As Short ' 1 = Zip Info Verbose
    Public uCaseSensitivity As Short ' 1 = Case Insensitivity, 0 = Case Sensitivity
    Public uPrivilege As Short ' 1 = ACL, 2 = Privileges, Else 0
    Public uZipFileName As String ' The Zip File Name
    Public uExtractDir As String ' Extraction Directory, Null If Current Directory

    '-- Public Program Variables
    Public uZipNumber As Integer ' Zip File Number
    Public uNumberFiles As Integer ' Number Of Files
    Public uNumberXFiles As Integer ' Number Of Extracted Files
    Public uZipMessage As String ' For Zip Message
    Public uZipInfo As String ' For Zip Information
    'UPGRADE_WARNING: Arrays in structure uZipNames may need to be initialized before they can be used. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
    Public uZipNames As UNZIPnames ' Names Of Files To Unzip
    'UPGRADE_WARNING: Arrays in structure uExcludeNames may need to be initialized before they can be used. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
    Public uExcludeNames As UNZIPnames ' Names Of Zip Files To Exclude
    Public uVbSkip As Short ' For DLL Password Function

    '*************************************************************************
    ' Declarations for Zip & UnZip Error Codes
    '*************************************************************************

    Public Const ZE_OK As Short = 0 ' Success (No Error)
    Public Const ZE_EOF As Short = 2 ' Unexpected End Of Zip File Error
    Public Const ZE_FORM As Short = 3 ' Zip File Structure Error
    Public Const ZE_MEM As Short = 4 ' Out Of Memory Error
    Public Const ZE_LOGIC As Short = 5 ' Internal Logic Error
    Public Const ZE_BIG As Short = 6 ' Entry Too Large To Split Error
    Public Const ZE_NOTE As Short = 7 ' Invalid Comment Format Error
    Public Const ZE_TEST As Short = 8 ' Zip Test (-T) Failed Or Out Of Memory Error
    Public Const ZE_ABORT As Short = 9 ' User Interrupted Or Termination Error
    Public Const ZE_TEMP As Short = 10 ' Error Using A Temp File
    Public Const ZE_READ As Short = 11 ' Read Or Seek Error
    Public Const ZE_NONE As Short = 12 ' Nothing To Do Error
    Public Const ZE_NAME As Short = 13 ' Missing Or Empty Zip File Error
    Public Const ZE_WRITE As Short = 14 ' Error Writing To A File
    Public Const ZE_CREAT As Short = 15 ' Could't Open To Write Error
    Public Const ZE_PARMS As Short = 16 ' Bad Command Line Argument Error
    Public Const ZE_OPEN As Short = 18 ' Could Not Open A Specified File To Read Error

    '*************************************************************************
    ' Declarations for Windows API Functions
    '*************************************************************************


    '*************************************************************************
    ' Zip & Unzip Shared Functions
    '*************************************************************************
    Public Function DllsPresent() As Boolean
        '********************************************************************************************
        'Description: Returns true if the required DLLs are present in the System32 folder.
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim bOK As Boolean
        Dim sTmp As String
        Dim sZipPath As String
        Dim lSize As Integer
        Dim sSysDir As String

        bOK = True

        sSysDir = New String(vbNullChar.Chars(0), 50)
        lSize = GetSystemDirectory(sSysDir, 50)
        sSysDir = Left(sSysDir, lSize)

        sZipPath = sSysDir & "\zip32.dll"
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        sTmp = Dir(sZipPath)

        If LCase(sTmp) <> "zip32.dll" Then bOK = False

        If bOK Then
            sZipPath = sSysDir & "\unzip32.dll"
            'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            sTmp = Dir(sZipPath)

            If LCase(sTmp) <> "unzip32.dll" Then bOK = False
        End If

        DllsPresent = bOK

    End Function

    Public Function FnPtr(ByVal lp As Integer) As Integer
        '********************************************************************************************
        'Description: Puts A Function Pointer In A Structure For Use With Callbacks...
        '
        'Parameters: the pointer
        'Returns: the pointer
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        FnPtr = lp

    End Function

    Private Function GetFiles(ByVal ListFileName As String) As String()
        '********************************************************************************************
        'Description: Extracts the files to zip from a text file and returns a FileName array
        '
        'Parameters: ListFileName - includes full path
        'Returns: File list array
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Return Nothing
    End Function

    Public Function szTrim(ByRef szString As String) As String
        '********************************************************************************************
        'Description: ASCIIZ To String Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim pos As Integer

        pos = InStr(szString, vbNullChar)

        Select Case pos
            Case Is > 1
                szTrim = Trim(Left(szString, pos - 1))
            Case 1
                szTrim = ""
            Case Else
                szTrim = Trim(szString)
        End Select

    End Function


    '*************************************************************************
    ' Unzip Functions and Subroutines
    '*************************************************************************

    Private Sub subProcessUnZipFileList(ByVal ListFileName As String)
        '********************************************************************************************
        'Description: Load uZipNames.uzFiles array from list file.
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim nFileCount As Integer
        Dim sFileName As String
        Dim sListFile As String

        sListFile = Right(ListFileName, Len(ListFileName) - 1)

        Dim fileReader As System.IO.StreamReader
        fileReader = My.Computer.FileSystem.OpenTextFileReader(sListFile)
        Dim stringReader As String
        stringReader = fileReader.ReadLine()

        nFileCount = 0

        Do While Not fileReader.EndOfStream
            sFileName = fileReader.ReadLine
            uZipNames.uzFiles(nFileCount) = Trim(sFileName)
            nFileCount = nFileCount + 1
        Loop

        uNumberFiles = nFileCount

    End Sub

    Public Function UZDLLPass(ByRef p As UNZIPCBCh, ByVal n As Integer, ByRef m As UNZIPCBCh, ByRef Name As UNZIPCBCh) As Short
        '********************************************************************************************
        'Description: Callback For UNZIP32.DLL - Password Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Dim prompt As String
        Dim xx As Integer
        Dim szpassword As String

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        UZDLLPass = 1

        If uVbSkip = 1 Then Exit Function

        '-- Get The Zip File Password
        szpassword = InputBox("Please Enter The Password!")

        '-- No Password So Exit The Function
        If Len(szpassword) = 0 Then
            uVbSkip = 1
            Exit Function
        End If

        '-- Zip File Password So Process It
        For xx = 0 To 255
            If m.ch(xx) = 0 Then
                Exit For
            Else
                prompt = prompt & Chr(m.ch(xx))
            End If
        Next

        For xx = 0 To n - 1
            p.ch(xx) = 0
        Next

        For xx = 0 To Len(szpassword) - 1
            p.ch(xx) = Asc(Mid(szpassword, xx + 1, 1))
        Next

        p.ch(xx) = 0 ' Put Null Terminator For C

        UZDLLPass = 0

    End Function

    Public Function UZDLLPrnt(ByRef fname As UNZIPCBChar, ByVal x As Integer) As Integer
        '********************************************************************************************
        'Description: Callback For UNZIP32.DLL - Print Message Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        s0 = ""

        '-- Gets The UNZIP32.DLL Message For Displaying.
        For xx = 0 To x - 1
            If fname.ch(xx) = 0 Then Exit For
            s0 = s0 & Chr(fname.ch(xx))
        Next

        '-- Assign Zip Information
        If Mid(s0, 1, 1) = vbLf Then s0 = vbNewLine ' Damn UNIX :-)
        uZipInfo = uZipInfo & s0

        UZDLLPrnt = 0

    End Function

    Public Function UZDLLRep(ByRef fname As UNZIPCBChar) As Integer
        '********************************************************************************************
        'Description: Callback For UNZIP32.DLL - Report Function To Overwrite Files. This Function
        '             Will Display A MsgBox Asking The User If They Would Like To Overwrite The Files.
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        UZDLLRep = 100 ' 100 = Do Not Overwrite - Keep Asking User
        s0 = ""

        For xx = 0 To 255
            If fname.ch(xx) = 0 Then Exit For
            s0 = s0 & Chr(fname.ch(xx))
        Next

        '-- This Is The MsgBox Code
        xx = MsgBox("Overwrite " & s0 & "?", CType(MsgBoxStyle.Exclamation + MsgBoxStyle.YesNoCancel, MsgBoxStyle), "VBUnZip32 - File Already Exists!")

        If xx = MsgBoxResult.No Then Exit Function

        If xx = MsgBoxResult.Cancel Then
            UZDLLRep = 104 ' 104 = Overwrite None
            Exit Function
        End If

        UZDLLRep = 102 ' 102 = Overwrite, 103 = Overwrite All

    End Function

    Public Function UZDLLServ(ByRef mname As UNZIPCBChar, ByVal x As Integer) As Integer
        '********************************************************************************************
        'Description: Callback For UNZIP32.DLL - DLL Service Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        ' Parameter x contains the size of the extracted archive entry.
        ' This information may be used for some kind of progress display...

        s0 = ""

        '-- Get Zip32.DLL Message For processing
        For xx = 0 To UBound(mname.ch)
            If mname.ch(xx) = 0 Then Exit For
            s0 = s0 & Chr(mname.ch(xx))
        Next

        ' At this point, s0 contains the message passed from the DLL
        ' It is up to the developer to code something useful here :)

        UZDLLServ = 0 ' Setting this to 1 will abort the zip!

    End Function

    Public Sub UZReceiveDLLMessage(ByVal ucsize As Integer, ByVal csiz As Integer, ByVal cfactor As Short, ByVal mo As Short, ByVal dy As Short, ByVal yr As Short, ByVal hh As Short, ByVal mm As Short, ByVal c As Byte, ByRef fname As UNZIPCBCh, ByRef meth As UNZIPCBCh, ByVal crc As Integer, ByVal fCrypt As Byte)
        '********************************************************************************************
        'Description: Callback For UNZIP32.DLL - Receive Message Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer
        Dim strout As New VB6.FixedLengthString(80)

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        '------------------------------------------------
        '-- This Is Where The Received Messages Are
        '-- Printed Out And Displayed.
        '-- You Can Modify Below!
        '------------------------------------------------

        strout.Value = Space(80)

        '-- For Zip Message Printing
        If uZipNumber = 0 Then
            Mid(strout.Value, 1, 50) = "Filename:"
            Mid(strout.Value, 53, 4) = "Size"
            Mid(strout.Value, 62, 4) = "Date"
            Mid(strout.Value, 71, 4) = "Time"
            uZipMessage = strout.Value & vbNewLine
            strout.Value = Space(80)
        End If

        s0 = ""

        '-- Do Not Change This For Next!!!
        For xx = 0 To 255
            If fname.ch(xx) = 0 Then Exit For
            s0 = s0 & Chr(fname.ch(xx))
        Next

        '-- Assign Zip Information For Printing
        Mid(strout.Value, 1, 50) = Mid(s0, 1, 50)
        Mid(strout.Value, 51, 7) = Right("        " & CStr(ucsize), 7)
        Mid(strout.Value, 60, 3) = Right("0" & Trim(CStr(mo)), 2) & "/"
        Mid(strout.Value, 63, 3) = Right("0" & Trim(CStr(dy)), 2) & "/"
        Mid(strout.Value, 66, 2) = Right("0" & Trim(CStr(yr)), 2)
        Mid(strout.Value, 70, 3) = Right(Str(hh), 2) & ":"
        Mid(strout.Value, 73, 2) = Right("0" & Trim(CStr(mm)), 2)

        ' Mid(strout, 75, 2) = Right$(" " & CStr(cfactor), 2)
        ' Mid(strout, 78, 8) = Right$("        " & CStr(csiz), 8)
        ' s0 = ""
        'For xx = 0 To 255
        'If meth.ch(xx) = 0 Then Exit For
        '    s0 = s0 & Chr$(meth.ch(xx))
        'Next 'xx

        '-- Do Not Modify Below!!!
        uZipMessage = uZipMessage & strout.Value & vbNewLine
        uZipNumber = uZipNumber + 1

    End Sub

    Public Function VBUnZip32(ByVal UnZipFileName As String, ByVal UnZipFileFolder As String, Optional ByVal UnZipFileList As String = "", Optional ByVal OverWrite As Boolean = True, Optional ByVal PreserveDirectories As Boolean = True) As Integer
        Dim mZip As Object
        '********************************************************************************************
        'Description: Main UNZIP32.DLL UnZip32 Function. This Is Where It All Happens.
        '             (WARNING!) Do Not Change This Function!!!
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        ' RJO         02/07/08      I cheated and added all of the parameters and initialization code
        '                           to this function.
        '********************************************************************************************
        Dim retcode As Integer
        'Dim MsgStr As String

        If Right(UnZipFileFolder, 1) <> "\" Then UnZipFileFolder = UnZipFileFolder & "\"

        'Set UnZip Options based on defaults and VBUnZip32 arguments

        '-- Init Global Message Variables
        uZipInfo = ""
        uZipNumber = 0 ' Holds The Number Of Zip Files

        '-- Select UNZIP32.DLL Options - Change As Required!
        uPromptOverWrite = 0 ' 1 = Prompt To Overwrite
        If OverWrite Then
            uOverWriteFiles = 1 ' 1 = Always Overwrite Files
        Else
            uOverWriteFiles = 0
        End If
        uDisplayComment = 0 ' 1 = Display comment ONLY!!!

        '-- Change The Next Line To Do The Actual Unzip!
        'UPGRADE_WARNING: Couldn't resolve default property of object mZip.uExtractList. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        mZip.uExtractList = 0 ' 1 = List Contents Of Zip 0 = Extract
        If PreserveDirectories Then
            uHonorDirectories = 1 ' 1 = Honour Zip Directories
        Else
            uHonorDirectories = 0 ' 1 = Honour Zip Directories
        End If

        uCaseSensitivity = 1 ' 1 = Case insensitivity, 0 = Case Sensitivity

        '-- Select Filenames
        If UnZipFileList = "" Then
            '-- Select All Files
            uZipNames.uzFiles(0) = vbNullString
            uNumberFiles = 0
        Else
            If Left(UnZipFileList, 1) = "@" Then
                '-- Select a list of Files
                Call subProcessUnZipFileList(UnZipFileList)
            Else
                '-- Select a single file
                uZipNames.uzFiles(0) = UnZipFileList
                uNumberFiles = 1
            End If
        End If 'UnZipFileList = ""

        '-- Select Filenames To Exclude From Processing
        ' Note UNIX convention!
        '   vbxnames.s(0) = "VBSYX/VBSYX.MID"
        '   vbxnames.s(1) = "VBSYX/VBSYX.SYX"
        '   numx = 2

        '-- Or Just Select All Files
        uExcludeNames.uzFiles(0) = vbNullString
        uNumberXFiles = 0

        '-- Change The Next 2 Lines As Required!
        '-- These Should Point To Your Directory
        uZipFileName = UnZipFileName
        uExtractDir = UnZipFileFolder
        If uExtractDir <> "" Then uExtractList = 0 ' unzip if dir specified


        '-- Set The UNZIP32.DLL Options
        '-- (WARNING!) Do Not Change
        UZDCL.ExtractOnlyNewer = uExtractOnlyNewer ' 1 = Extract Only Newer/New
        UZDCL.SpaceToUnderscore = uSpaceUnderScore ' 1 = Convert Space To Underscore
        UZDCL.PromptToOverwrite = uPromptOverWrite ' 1 = Prompt To Overwrite Required
        UZDCL.fQuiet = uQuiet ' 2 = No Messages 1 = Less 0 = All
        UZDCL.ncflag = uWriteStdOut ' 1 = Write To Stdout
        UZDCL.ntflag = uTestZip ' 1 = Test Zip File
        UZDCL.nvflag = uExtractList ' 0 = Extract 1 = List Contents
        UZDCL.nfflag = uFreshenExisting ' 1 = Update Existing by Newer
        UZDCL.nzflag = uDisplayComment ' 1 = Display Zip File Comment
        UZDCL.ndflag = uHonorDirectories ' 1 = Honour Directories
        UZDCL.noflag = uOverWriteFiles ' 1 = Overwrite Files
        UZDCL.naflag = uConvertCR_CRLF ' 1 = Convert CR To CRLF
        UZDCL.nZIflag = uVerbose ' 1 = Zip Info Verbose
        UZDCL.C_flag = uCaseSensitivity ' 1 = Case insensitivity, 0 = Case Sensitivity
        UZDCL.fPrivilege = uPrivilege ' 1 = ACL 2 = Priv
        UZDCL.Zip = uZipFileName ' ZIP Filename
        UZDCL.ExtractDir = uExtractDir ' Extraction Directory, NULL If Extracting
        ' To Current Directory

        ' The delegates (function pointers) enable asynchronous calls from the password object.


        '-- Set Callback Addresses
        '-- (WARNING!!!) Do Not Change
        'UPGRADE_WARNING: Add a delegate for AddressOf UZDLLPrnt Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        UZUSER.UZDLLPrnt = FnPtr(AddressOf UZDLLPrnt)
        UZUSER.UZDLLSND = 0 '-- Not Supported
        'UPGRADE_WARNING: Add a delegate for AddressOf UZDLLRep Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        UZUSER.UZDLLREPLACE = FnPtr(AddressOf UZDLLRep)
        'UPGRADE_WARNING: Add a delegate for AddressOf UZDLLPass Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        UZUSER.UZDLLPASSWORD = FnPtr(AddressOf UZDLLPass)
        'UPGRADE_WARNING: Add a delegate for AddressOf UZReceiveDLLMessage Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        UZUSER.UZDLLMESSAGE = FnPtr(AddressOf UZReceiveDLLMessage)
        'UPGRADE_WARNING: Add a delegate for AddressOf UZDLLServ Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        UZUSER.UZDLLSERVICE = FnPtr(AddressOf UZDLLServ)

        '-- Set UNZIP32.DLL Version Space
        '-- (WARNING!!!) Do Not Change
        With UZVER
            .structlen = Len(UZVER)
            .beta = New String(" ".Chars(0), 9) & vbNullChar.Chars(0)
            .Date_Renamed = Space(19) & vbNullChar.Chars(0)
            .zlib = Space(9) & vbNullChar.Chars(0)
        End With

        '-- Get Version
        'Call UzpVersion2(UZVER)

        '--------------------------------------
        '-- You Can Change This For Displaying
        '-- The Version Information!
        '--------------------------------------
        'MsgStr$ = "DLL Date: " & szTrim(UZVER.Date)
        'MsgStr$ = MsgStr$ & vbNewLine$ & "Zip Info: " & Hex$(UZVER.zipinfo(1)) & "." & _
        ''Hex$(UZVER.zipinfo(2)) & Hex$(UZVER.zipinfo(3))
        'MsgStr$ = MsgStr$ & vbNewLine$ & "DLL Version: " & Hex$(UZVER.windll(1)) & "." & _
        ''Hex$(UZVER.windll(2)) & Hex$(UZVER.windll(3))
        'MsgStr$ = MsgStr$ & vbNewLine$ & "--------------"
        '-- End Of Version Information.

        '-- Go UnZip The Files! (Do Not Change Below!!!)
        '-- This Is The Actual UnZip Routine
        retcode = Wiz_SingleEntryUnzip(uNumberFiles, uZipNames, uNumberXFiles, uExcludeNames, UZDCL, UZUSER)
        '---------------------------------------------------------------
        '-- Return The Function Code
        VBUnZip32 = retcode

        '-- If There Is An Error Display A MsgBox!
        'If retcode <> 0 Then MsgBox retcode

        '-- You Can Change This As Needed!
        '-- For Compression Information
        'MsgStr$ = MsgStr$ & vbNewLine & "Only Shows If uExtractList = 1 List Contents"
        'MsgStr$ = MsgStr$ & vbNewLine & "--------------"
        'MsgStr$ = MsgStr$ & vbNewLine & "Comment         : " & UZUSER.cchComment
        'MsgStr$ = MsgStr$ & vbNewLine & "Total Size Comp : " & UZUSER.TotalSizeComp
        'MsgStr$ = MsgStr$ & vbNewLine & "Total Size      : " & UZUSER.TotalSize
        'MsgStr$ = MsgStr$ & vbNewLine & "Compress Factor : %" & UZUSER.CompFactor
        'MsgStr$ = MsgStr$ & vbNewLine & "Num Of Members  : " & UZUSER.NumMembers
        'MsgStr$ = MsgStr$ & vbNewLine & "--------------"

        'frmMain.txtMsgOut.Text = frmMain.txtMsgOut.Text & MsgStr$ & vbNewLine

    End Function


    '*************************************************************************
    ' Zip Functions and Subroutines
    '*************************************************************************

    Private Sub subProcessZipFileList(ByVal ListFileName As String)
        Dim TristateUseDefault As Object
        Dim ForReading As Object
        '********************************************************************************************
        'Description: Load zZipFileNames.zZipFileNames array from list file.
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        'UPGRADE_ISSUE: FileSystemObject object was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"'
        Dim oFSO As FileSystemObject
        'UPGRADE_ISSUE: TextStream object was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"'
        Dim oTS As TextStream
        Dim nFileCount As Integer
        Dim sFileName As String
        Dim sListFile As String

        sListFile = Right(ListFileName, Len(ListFileName) - 1)
        oFSO = New FileSystemObject
        'UPGRADE_WARNING: Couldn't resolve default property of object oFSO.OpenTextFile. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        oTS = oFSO.OpenTextFile(sListFile, ForReading, False, TristateUseDefault)

        nFileCount = 0

        'UPGRADE_WARNING: Couldn't resolve default property of object oTS.AtEndOfStream. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Do While Not oTS.AtEndOfStream
            'UPGRADE_WARNING: Couldn't resolve default property of object oTS.ReadLine. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            sFileName = oTS.ReadLine
            zZipFileNames.zFiles(nFileCount) = Trim(sFileName)
            nFileCount = nFileCount + 1
        Loop

        zArgc = nFileCount

    End Sub

    Public Function VBZip32(ByVal ZipFileName As String, ByVal ZipFileList As String, Optional ByVal SavePath As Boolean = False, Optional ByVal RecurseSubDirs As Boolean = False) As Integer
        '********************************************************************************************
        'Description: Main ZIP32.DLL Function. This Is Where It All Happens.
        '             (WARNING!) Do Not Change This Function!!!
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        ' RJO         02/07/08      I cheated and added all of the parameters and initialization code
        '                           to this function.
        '********************************************************************************************
        Dim retcode As Integer

        'Set Zip Options based on defaults and VBZip32 arguments

        '-- Set Options - Only The Common Ones Are Shown Here
        '-- These Must Be Set Before Calling The VBZip32 Function
        zDate = vbNullString
        'zDate = "2005-1-31"
        'zExcludeDate = 1
        'zIncludeDate = 0
        If SavePath Then
            zJunkDir = 0 ' 1 = Throw Away Path Names
        Else
            zJunkDir = 1 ' 1 = Throw Away Path Names
        End If
        If RecurseSubDirs Then
            zRecurse = 1 ' 1 = Recurse -r, 2 = Recurse -R 2 = Most Useful : ) 'RJO Re-he-healy?
        Else
            zRecurse = 0
        End If

        zUpdate = 0 ' 1 = Update Only If Newer
        zFreshen = 0 ' 1 = Freshen - Overwrite Only
        zLevel = Asc(CStr(9)) ' Compression Level (0 - 9)
        zEncrypt = 0 ' Encryption = 1 For Password Else 0
        zComment = 0 ' Comment = 1 if required

        '-- Some I added to play around with 'RJO
        zVerbose = 0
        zQuiet = 0

        zZipFileName = ZipFileName

        If Left(ZipFileList, 1) = "@" Then
            'Call sub to load zZipFileNames array from file
            Call subProcessZipFileList(ZipFileList)
            zRecurse = 0
        Else
            'ReDim zZipFileNames.zFiles(0)
            zArgc = 1 ' Number Of Elements Of mynames Array
            zZipFileNames.zFiles(0) = ZipFileList
        End If

        zRootDir = "" ' This Affects The Stored Path Name

        On Error Resume Next '-- Nothing Will Go Wrong :-)

        retcode = 0

        '-- Set Address Of ZIP32.DLL Callback Functions
        '-- (WARNING!) Do Not Change!!!
        'UPGRADE_WARNING: Add a delegate for AddressOf ZDLLPrnt Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        ZUSER.ZDLLPrnt = FnPtr(AddressOf ZDLLPrnt)
        'UPGRADE_WARNING: Add a delegate for AddressOf ZDLLPass Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        ZUSER.ZDLLPASSWORD = FnPtr(AddressOf ZDLLPass)
        'UPGRADE_WARNING: Add a delegate for AddressOf ZDLLComm Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        ZUSER.ZDLLCOMMENT = FnPtr(AddressOf ZDLLComm)
        'UPGRADE_WARNING: Add a delegate for AddressOf ZDLLServ Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
        ZUSER.ZDLLSERVICE = FnPtr(AddressOf ZDLLServ)

        '-- Set ZIP32.DLL Callbacks
        retcode = ZpInit(ZUSER)
        If retcode = 0 Then
            MsgBox("Zip32.dll did not initialize.  Is it in the current directory " & "or on the command path?", MsgBoxStyle.OkOnly, "VB Zip")
            Exit Function
        End If

        '-- Setup ZIP32 Options
        '-- (WARNING!) Do Not Change!
        ZOPT.Date_Renamed = zDate ' "12/31/79"? US Date?
        ZOPT.szRootDir = zRootDir ' Root Directory Pathname
        ZOPT.szTempDir = zTempDir ' Temp Directory Pathname
        ZOPT.fSuffix = zSuffix ' Include Suffixes (Not Yet Implemented)
        ZOPT.fEncrypt = zEncrypt ' 1 If Encryption Wanted
        ZOPT.fSystem = zSystem ' 1 To Include System/Hidden Files
        ZOPT.fVolume = zVolume ' 1 If Storing Volume Label
        ZOPT.fExtra = zExtra ' 1 If Including Extra Attributes
        ZOPT.fNoDirEntries = zNoDirEntries ' 1 If Ignoring Directory Entries
        ZOPT.fExcludeDate = zExcludeDate ' 1 If Excluding Files Earlier Than A Specified Date
        ZOPT.fIncludeDate = zIncludeDate ' 1 If Including Files Earlier Than A Specified Date
        ZOPT.fVerbose = zVerbose ' 1 If Full Messages Wanted
        ZOPT.fQuiet = zQuiet ' 1 If Minimum Messages Wanted
        ZOPT.fCRLF_LF = zCRLF_LF ' 1 If Translate CR/LF To LF
        ZOPT.fLF_CRLF = zLF_CRLF ' 1 If Translate LF To CR/LF
        ZOPT.fJunkDir = zJunkDir ' 1 If Junking Directory Names
        ZOPT.fGrow = zGrow ' 1 If Allow Appending To Zip File
        ZOPT.fForce = zForce ' 1 If Making Entries Using DOS Names
        ZOPT.fMove = zMove ' 1 If Deleting Files Added Or Updated
        ZOPT.fDeleteEntries = zDelEntries ' 1 If Files Passed Have To Be Deleted
        ZOPT.fUpdate = zUpdate ' 1 If Updating Zip File-Overwrite Only If Newer
        ZOPT.fFreshen = zFreshen ' 1 If Freshening Zip File-Overwrite Only
        ZOPT.fJunkSFX = zJunkSFX ' 1 If Junking SFX Prefix
        ZOPT.fLatestTime = zLatestTime ' 1 If Setting Zip File Time To Time Of Latest File In Archive
        ZOPT.fComment = zComment ' 1 If Putting Comment In Zip File
        ZOPT.fOffsets = zOffsets ' 1 If Updating Archive Offsets For SFX Files
        ZOPT.fPrivilege = zPrivilege ' 1 If Not Saving Privelages
        ZOPT.fEncryption = zEncryption ' Read Only Property!
        ZOPT.fRecurse = zRecurse ' 1 or 2 If Recursing Into Subdirectories
        ZOPT.fRepair = zRepair ' 1 = Fix Archive, 2 = Try Harder To Fix
        ZOPT.flevel = zLevel ' Compression Level - (0 To 9) Should Be 0!!!

        '-- Set ZIP32.DLL Options
        retcode = ZpSetOptions(ZOPT)

        '-- Go Zip It Them Up!
        retcode = ZpArchive(zArgc, zZipFileName, zZipFileNames)

        '-- Return The Function Code
        VBZip32 = retcode

    End Function

    Public Function ZDLLComm(ByRef s1 As ZipCBChar) As Short
        '********************************************************************************************
        'Description: Callback For ZIP32.DLL - DLL Comment Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim xx As Integer
        Dim szcomment As String

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        ZDLLComm = 1
        szcomment = InputBox("Enter the comment")
        If szcomment = "" Then Exit Function

        For xx = 0 To Len(szcomment) - 1
            s1.ch(xx) = Asc(Mid(szcomment, xx + 1, 1))
        Next  'xx

        s1.ch(xx) = CByte(Chr(0)) ' Put null terminator for C

    End Function

    Public Function ZDLLPass(ByRef p As ZipCBChar, ByVal n As Integer, ByRef m As ZipCBChar, ByRef Name As ZipCBChar) As Short
        '********************************************************************************************
        'Description: Callback For ZIP32.DLL - DLL Password Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim prompt As String
        Dim xx As Integer
        Dim szpassword As String

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        ZDLLPass = 1

        '-- If There Is A Password Have The User Enter It!
        '-- This Can Be Changed
        szpassword = InputBox("Please Enter The Password!")

        '-- The User Did Not Enter A Password So Exit The Function
        If szpassword = "" Then Exit Function

        '-- User Entered A Password So Proccess It
        For xx = 0 To 255
            If m.ch(xx) = 0 Then
                Exit For
            Else
                prompt = prompt & Chr(m.ch(xx))
            End If
        Next

        For xx = 0 To n - 1
            p.ch(xx) = 0
        Next

        For xx = 0 To Len(szpassword) - 1
            p.ch(xx) = Asc(Mid(szpassword, xx + 1, 1))
        Next

        p.ch(xx) = CByte(Chr(0)) ' Put Null Terminator For C

        ZDLLPass = 0

    End Function

    Public Function ZDLLPrnt(ByRef fname As ZipCBChar, ByVal x As Integer) As Integer
        '********************************************************************************************
        'Description: Callback For ZIP32.DLL - DLL Print Function
        '
        'Parameters:
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        s0 = ""

        '-- Get Zip32.DLL Message For processing
        For xx = 0 To x
            If fname.ch(xx) = 0 Then
                Exit For
            Else
                s0 = s0 & Chr(fname.ch(xx))
            End If
        Next

        '----------------------------------------------
        '-- This Is Where The DLL Passes Back Messages
        '-- To You! You Can Change The Message Printing
        '-- Below Here!
        '----------------------------------------------

        '-- Display Zip File Information
        zZipInfo = zZipInfo & s0
        'frmMain.Print s0;

        System.Windows.Forms.Application.DoEvents()

        ZDLLPrnt = 0

    End Function

    Public Function ZDLLServ(ByRef mname As ZipCBChar, ByVal x As Integer) As Integer
        '********************************************************************************************
        'Description: Callback For ZIP32.DLL - DLL Service Function
        '
        'Parameters: x is the size of the file
        'Returns:
        '
        ' Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim s0 As String
        Dim xx As Integer

        '-- Always Put This In Callback Routines!
        On Error Resume Next

        s0 = ""
        '-- Get Zip32.DLL Message For processing
        For xx = 0 To 4096
            If mname.ch(xx) = 0 Then
                Exit For
            Else
                s0 = s0 & Chr(mname.ch(xx))
            End If
        Next
        ' Form1.Print "-- " & s0 & " - " & x & " bytes"

        ' This is called for each zip entry.
        ' mname is usually the null terminated file name and x the file size.
        ' s0 has trimmed file name as VB string.

        ' At this point, s0 contains the message passed from the DLL
        ' It is up to the developer to code something useful here :)
        ZDLLServ = 0 ' Setting this to 1 will abort the zip!

    End Function
End Class
