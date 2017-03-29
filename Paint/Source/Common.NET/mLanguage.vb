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
' Form/Module: mLanguage
'
' Description: Multi language functions
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
'    11/03/08   rjo     Added DisplayCultureString Property
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Resources
Imports System.Reflection
Imports System.Globalization
Imports System.Globalization.CultureInfo
Imports System.Threading

Friend Module mLanguage

#Region " Declares"

    '9.5.07 remove unnecessary initializations per fxCop
    Friend gcsRM As ResourceManager ' = Nothing   ' Common strings
    Friend gpsRM As ResourceManager ' = Nothing   ' Project Strings
    Friend grsRM As ResourceManager ' = Nothing   ' Robot Strings
    Friend gasRM As ResourceManager ' = Nothing    ' For misc as needed
    Private msCultureString As String = "en-US"   'Default to English

#End Region
#Region " Properties"
    Friend ReadOnly Property FixedCulture() As CultureInfo
        Get
            Return CultureInfo.InvariantCulture
        End Get
    End Property
    Friend ReadOnly Property CurrentCulture() As CultureInfo
        Get
            Return CultureInfo.CurrentCulture
        End Get
    End Property
    Friend Property DisplayCultureString() As String
        '********************************************************************************************
        'Description:  This property contains the culture string for the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msCultureString
        End Get
        Set(ByVal value As String)
            msCultureString = value
        End Set
    End Property
#End Region
#Region " Routines "

    Friend Function GetCultureString() As String
        '****************************************************************************************
        'Description: This Function gets Current culture string from the xml file
        '
        'Parameters: none
        'Returns:   Culture string  e.g. "en-US"
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sTmp As String = "en-US"
        ''Dim xNode As Xml.XmlNode
        ''Dim sPath As String = String.Empty
        ''Dim sXPath As String = "CurrentCultureString"

        Try
            ''If GetDefaultFilePath(sPath, mPW4Common.eDir.XML, gsLANGUAGEFILE) Then
            ''    If GetXmlNode(sPath, sXPath, gsXMLPREFIX, gsDEFAULTURN, xNode) = mPW4Common.fRet.Success Then
            ''        sTmp = xNode.InnerText
            ''    End If
            ''End If
            sTmp = DisplayCultureString

            Return sTmp
        Catch ex As Exception
            'if all else fails return english
            Trace.WriteLine("Module: mLanguage, Routine: GetCultureString, Error: " & ex.Message)
            Trace.WriteLine("Module: mLanguage, Routine: GetCultureString, StackTrace: " & ex.StackTrace)
            Return "en-US"
        End Try

    End Function
    Friend Function GetResourceManagers(ByVal Common_Assembly_Name As String, _
           ByVal Local_Assembly_Name As String, ByVal Robot_Assembly_Name As String) As Boolean
        '****************************************************************************************
        'Description: This Function sets up the resource managers
        '
        'Parameters: Names or resource files
        'Returns:   True if success
        '
        'Modification history:
        '
        ' Date          By      Reason
        ' 02/15/2010    RJO     Make sure the culture is Not a Neutral culture before assigning it
        '                       to Thread.CurrentThread.CurrentCulture
        '*****************************************************************************************
        Dim sTmp As String = GetCultureString()

        Try
            Dim oCulture As New CultureInfo(sTmp) '02/15/2010 RJO

            If Not oCulture.IsNeutralCulture Then '02/15/2010 RJO
                Thread.CurrentThread.CurrentCulture = oCulture
            End If
            Thread.CurrentThread.CurrentUICulture = New CultureInfo(sTmp)
            If Common_Assembly_Name <> String.Empty Then
                gcsRM = New ResourceManager(Common_Assembly_Name, [Assembly].GetExecutingAssembly())
            End If
            If Local_Assembly_Name <> String.Empty Then
                gpsRM = New ResourceManager(Local_Assembly_Name, [Assembly].GetExecutingAssembly())
            End If
            If Robot_Assembly_Name <> String.Empty Then
                grsRM = New ResourceManager(Robot_Assembly_Name, [Assembly].GetExecutingAssembly())
            End If

            Return True

        Catch ex As Exception
            Trace.WriteLine("Module: mLanguage, Routine: GetResourceManagers, Error: " & ex.Message)
            Trace.WriteLine("Module: mLanguage, Routine: GetResourceManagers, StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function
    Friend Function GetAuxResourceManager(ByVal Aux_Assembly_Name As String) As Boolean
        '****************************************************************************************
        'Description: This Function sets up the aux resource manager
        '
        'Parameters: Names or resource files
        'Returns:   True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/21/2011    RJO     Make sure the culture is Not a Neutral culture before assigning it
        '                       to Thread.CurrentThread.CurrentCulture
        '*****************************************************************************************
        Dim sTmp As String = GetCultureString()

        Try
            Dim oCulture As New CultureInfo(sTmp) '09/21/2011 RJO
 
            If Not oCulture.IsNeutralCulture Then '09/21/2011 RJO
                Thread.CurrentThread.CurrentCulture = oCulture
            End If
            Thread.CurrentThread.CurrentUICulture = New CultureInfo(sTmp)
            If Aux_Assembly_Name <> String.Empty Then
                gasRM = New ResourceManager(Aux_Assembly_Name, [Assembly].GetExecutingAssembly())
            End If

            Return True

        Catch ex As Exception
            Trace.WriteLine("Module: mLanguage, Routine: GetAuxResourceManager, Error: " & ex.Message)
            Trace.WriteLine("Module: mLanguage, Routine: GetAuxResourceManager, StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function

#End Region

End Module
