' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2010
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsNoPLCComm
'
' Description: Class to use for PaintWorks SA (No PLC)
' 
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: HGB
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'********************************************************************************************
' 03/22/2010    HGB     Initial Code
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Array
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO

Friend Class clsPLCComm
    '***** TODO List ***************************************************************************
    '***** End TODO List ***********************************************************************

#Region "  Declares  "

    '***** XML Setup ***************************************************************************
    'Name of the config file as it is specific to plc and comm form
    Private Const msXMLFILENAME As String = "NoPLCComm.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsNoPLCComm"
    '***** End Module Constants ****************************************************************

    '***** Property Vars ***********************************************************************
    Private msTagName As String = String.Empty
    Private msZoneName As String = String.Empty
    Private msRemoteComputer As String = String.Empty
    Private msReadData() As String
    Private mbRead As Boolean '= False
    '***** End Property Vars *******************************************************************

    '******* Events ****************************************************************************
    'when hotlink has new data 
    Friend Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                         ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Friend Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events ************************************************************************

    '***** Working Vars ************************************************************************
    Private mbOTRWInProg As Boolean ' = False
    Private mLinks As New clsLinks
    Private mnHotLinks As Integer ' = 0 'How many HotLinks have been created.
    Private mnNoPLCReturnCode As Integer ' = 0
    '***** End Working Vars ********************************************************************

    '***** Enums *******************************************************************************

    '***** End Enums ***************************************************************************

#End Region

#Region " Properties "

    Friend ReadOnly Property DefaultGroupName() As String
        '********************************************************************************************
        'Description: The opc group name - typically the project name
        '             Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return Me.Parent.Name
        End Get

    End Property

    Friend Property PLCData() As String()
        '********************************************************************************************
        'Description:  Read or write PLC Data - All data in and out is string array, one word data
        '              is done as string(0).  Link is a hotlink if poll rate is > 0 if zero it is a
        '              one time read/write. Hotlink data returned via newdata event, but this must
        '              be called to start hotlink
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
          Return Nothing
        End Get

        Set(ByVal value As String())
        End Set

    End Property

    'Friend Property PLCType() As ePLCType
    '    '********************************************************************************************
    '    'Description:  NoPLC.ocx is capable of reading/writing data to 3 PLC types (PLC-5, SLC-500, 
    '    '              and Logix 5000). This property is set by default to Logix 5000 as the other 
    '    '              types are the exception.
    '    '              Note: Not used for NoPLC. Maintained for compatibility
    '    '
    '    'Parameters: none
    '    'Returns:    PLC Type
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************

    '    Get
    '        Return mPLCType
    '    End Get

    '    Set(ByVal value As ePLCType)
    '        mPLCType = value
    '    End Set

    'End Property

    Friend Property TagName() As String
        '********************************************************************************************
        'Description:  TagName - identifier into tag file
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msTagName
        End Get

        Set(ByVal value As String)
            msTagName = value
        End Set

    End Property


    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  return where we are looking for taginfo
        '              Note: Not used for NoPLC. Maintained for compatibility
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msXMLFilePath
        End Get

    End Property

    Friend WriteOnly Property Zone() As clsZone
        '********************************************************************************************
        'Description:  This is to take the place of Zonename - set this with current zone this 
        '              carries dbpath etc with it. should already be checked for available etc.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As clsZone)
        End Set

    End Property

    Private ReadOnly Property ZoneName() As String
        '********************************************************************************************
        'Description:  Zone name 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msZoneName
        End Get

    End Property

#End Region

#Region " Routines "
    Friend Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String
        '****************************************************************************************
        'Description: This function takes a integer from a hotlink etc. and formats it 
        '               for the time base that this particular PLC uses
        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'assuming all 0.001 timebase
        Dim fTmp As Single = CSng(ValueIn / 1000)
        Return Format(fTmp, sFormat)

    End Function
    Friend Function FormatToTimerValue(ByVal ValueIn As Integer) As String
        '****************************************************************************************
        'Description: This function takes a integer from a textbox etc and formats it 
        '               for the value we need to send to plc
        'Parameters: Integer
        'Returns:   formatted string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'assuming all 0.001 timebase
        Dim nTmp As Integer = ValueIn * 1000
        Return nTmp.ToString

    End Function

    Friend Sub RemoveAllHotLinks()
        '********************************************************************************************
        'Description:  Dispose of all hotlinks.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Friend Sub RemoveHotLink(ByVal Tag As String, ByVal Zone As clsZone)
        '********************************************************************************************
        'Description:  Dispose of a hotlink we're no longer using.
        '
        'Parameters: Tag name, Zone name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub


#End Region

#Region " Functions "


#End Region

#Region " Events "


    Public Sub New()
        '****************************************************************************************
        'Description: Initialize
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub


#End Region

    Private Class clsLink
        '********************************************************************************************
        'Description:  This class is to keep track of associated link info for hotlinks
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

#Region "  Declares "
        Private msAddress As String = String.Empty
        Private msControlName As String = String.Empty
        Private mnBit As Short ' = 0
        Private mbBusy As Boolean ' = False
        Private mnElapsedTime As Integer ' = 0
        Private msIP_Address As String = String.Empty
        Private mnLength As Short ' = 0
        Private msMemType As String = String.Empty
        Private mnPollRate As Integer ' = 0
        Private msRefData() As String
        Private mnReturnCode As Integer ' = 0
        Private msSlot As String = String.Empty
        Private msTagName As String = String.Empty
        Private msType As String = String.Empty
        Private msZoneName As String = String.Empty
#End Region

#Region " Properties "

        Friend Property Address() As String
            '********************************************************************************************
            'Description: The PLC Data Table address string
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msAddress
            End Get

            Set(ByVal value As String)
                msAddress = value
            End Set

        End Property

        Friend Property Bit() As Short
            '********************************************************************************************
            'Description: The bit number within the PLC Dat word for Boolean Data Type.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnBit
            End Get

            Set(ByVal value As Short)
                mnBit = value
            End Set

        End Property

        Friend Property Busy() As Boolean
            '********************************************************************************************
            'Description: True if a read or write operation is in progress.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mbBusy
            End Get

            Set(ByVal value As Boolean)
                mbBusy = value
            End Set

        End Property

        Friend Property ControlName() As String
            '********************************************************************************************
            'Description: Name of the Asabtcp Control associated with this link
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msControlName
            End Get

            Set(ByVal value As String)
                msControlName = value
            End Set

        End Property

        Friend Property ElapsedTime() As Integer
            '********************************************************************************************
            'Description: The elapsed time (in milliseconds) a read or write operation has been in 
            '             progress.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Get
                Return mnElapsedTime
            End Get

            Set(ByVal value As Integer)
                mnElapsedTime = value
            End Set

        End Property

        Friend Property IP_Address() As String
            '********************************************************************************************
            'Description: The IP Address of the PLC that data is being read from/written to.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msIP_Address
            End Get

            Set(ByVal value As String)
                msIP_Address = value
            End Set

        End Property

        Friend Property Length() As Short
            '********************************************************************************************
            'Description: The number of PLC Data Table words being read/written
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnLength
            End Get

            Set(ByVal value As Short)
                If value < 1 Then Exit Property
                mnLength = value
            End Set

        End Property

        Friend Property MemType() As String
            '********************************************************************************************
            'Description: Internal data type specifier
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msMemType
            End Get

            Set(ByVal value As String)
                msMemType = value
            End Set

        End Property

        Friend Property PollRate() As Integer
            '********************************************************************************************
            'Description: In the case of a HotLink, the data read Poll Rate (in milliseconds)
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mnPollRate
            End Get

            Set(ByVal value As Integer)
                If value < 0 Then Exit Property
                mnPollRate = value
            End Set

        End Property

        Friend Property RefData() As String()
            '********************************************************************************************
            'Description: In the case of a HotLink, this is the data that was returned on the last
            '             "Complete" event. It is used to determine if a data change has occurred.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msRefData
            End Get

            Set(ByVal value As String())
                msRefData = value
            End Set

        End Property


        Friend Property TagName() As String
            '********************************************************************************************
            'Description: The TagName.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msTagName
            End Get

            Set(ByVal value As String)
                msTagName = value
            End Set

        End Property

        Friend Property Type() As String
            '********************************************************************************************
            'Description: The data type specified in the Tag file.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msType
            End Get

            Set(ByVal value As String)
                msType = value
            End Set

        End Property

        Friend Property ZoneName() As String
            '********************************************************************************************
            'Description: The name of the precess zone that this PLC data is associated with.
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return msZoneName
            End Get

            Set(ByVal value As String)
                msZoneName = value
            End Set

        End Property

#End Region

#Region " Events "


        Friend Sub New()
            '********************************************************************************************
            'Description: 
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

        End Sub

#End Region

    End Class  'clsLink

    Private Class clsLinks
        '********************************************************************************************
        'Description:  This class is to hold the collection of Links
        '
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Inherits CollectionBase

#Region " Declares"

#End Region

#Region " Properties "

        Friend Overloads ReadOnly Property Count() As Integer
            '********************************************************************************************
            'Description: How many links are in the collection
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return List.Count
            End Get

        End Property

#End Region

#Region " Routines "

        Friend Function Add(ByRef oLink As clsLink) As Integer
            '********************************************************************************************
            'Description: Add a new link to the collection
            '
            'Parameters:
            'Returns:    The index in the collection of the added link   
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim index As Integer = 0

            Try
                index = List.Add(oLink)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Add", ex.Message & vbCrLf & ex.StackTrace)
            End Try

            Return index

        End Function

        Friend Function Item(ByVal ControlName As String) As clsLink
            '********************************************************************************************
            'Description: Fetch the Link item that belongs to the named control
            '
            'Parameters: ControlName - Link identifier
            'Returns:    clsLink associated with the named control
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                Dim oLink As clsLink

                For Each oLink In List
                    If oLink.ControlName = ControlName Then
                        Return oLink
                    End If
                Next
                'not found
                Return Nothing

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Item", ex.Message & vbCrLf & ex.StackTrace)
                Return Nothing
            End Try

        End Function

        Friend Sub Remove(ByVal Link As clsLink)
            '********************************************************************************************
            'Description: Remove the Link from the list
            '
            'Parameters: Link - Link to be removed
            'Returns:    none
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Try
                MyBase.List.Remove(Link)

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":clsLinks:Remove", ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub

#End Region

#Region " Events "

        Friend Sub New()
            '********************************************************************************************
            'Description: 
            '
            'Parameters:
            'Returns:    
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************


        End Sub


#End Region

    End Class  'clsLinks

End Class 'clsPLCComm

