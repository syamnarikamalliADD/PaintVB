' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: Zones.vb
'
' Description: Collection for zone, zone class
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 03/17/07      gks     Onceover Cleanup
' 05/07/07      gks     Point to new tables "Setup Info"  "Zone Info"
' 03/20/08      gks     Switch to SQL Database "Configuration"
' 03/26/09      rjo     Added StandAlone Property to clsZones
' 10/23/09      BTK     Added mnRobotsRequiredStartingBit so we can tell config screens where the
'                       first robot starts in the PLC, generally zero or one.
' 09/27/11      MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
' 10/07/11      MSW     Read from XML                                                   4.1.1.0
' 04/11/12      MSW     remove old routine that reads from sql                          4.1.3.0
' 09/18/12      AM      Added MIS Computer mbMISComputer to use where is needed         4.1.3.02
' 04/12/13      RJO     Added AreaVolumeEnabled and LearnedVolumeEnabled to support     4.1.3.03
'                       Sealer screens
' 05/06/13      MSW     Remove some mazda custom changes that snuck in here             4.01.05.00
' 06/05/13      MSW     Vision logger support - provide VIN routines here               4.01.05.01
' 08/20/13      MSW     PSC remote Robot support                                        4.01.05.02
' 10/25/13      BTK     Added TricoatByStyle flag for tricoat robots required by style. 4.01.05.03
' 02/17/14      kdp     Added StationName support option                                4.01.07.00
' 04/21/14      MSW     cleanup station support                                         4.01.07.01
' 05/16/14      BTK     Support degrade by style                                        4.01.07.02
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath
'********************************************************************************************
'Description: Zone Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsZones
    Inherits CollectionBase


#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsZones"
    '***** End Module Constants   **************************************************************

    '9.5.07 remove unnecessary initializations per fxCop
    Private msSiteName As String = String.Empty
    Private msDBPath As String = String.Empty
    Private msHomeDBPath As String = String.Empty
    Private msCurrentZone As String = String.Empty
    Private mbAsciiColor As Boolean ' = False
    Private mbAsciiStyle As Boolean ' = False
    Private msInitialZone As String = String.Empty
    Private mbPaintShopComputer As Boolean ' = False
    Private mbStandAlone As Boolean
    Private mbSaveAccessData As Boolean
    Private mnNumVINWords As Integer ' = 0
    Private mbMISComputer As Boolean = False '09/18/12 AM Added for MIS computers

#End Region
#Region " Properties"

    Friend ReadOnly Property ActiveZone() As clsZone
        '********************************************************************************************
        'Description: The current zone class - the one to use
        '
        'Parameters:  
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                Dim o As clsZone = Item(CurrentZone)
                Return o
            Catch ex As Exception
                Return Nothing
            End Try
        End Get
    End Property
    'Friend ReadOnly Property AsciiColor() As Boolean
    '    '********************************************************************************************
    '    'Description: Specifies whether Plant Color Identifiers are ASCII strings
    '    '
    '    'Parameters: None 
    '    'Returns: True if Plant Colors are ASCII, False if numeric
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    Get
    '        Return mbAsciiColor
    '    End Get
    'End Property
    'Friend ReadOnly Property AsciiStyle() As Boolean
    '    '********************************************************************************************
    '    'Description: Specifies whether Plant Style Identifiers are ASCII strings
    '    '
    '    'Parameters: None 
    '    'Returns: True if Plant Style Identifiers are ASCII, False if numeric
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    Get
    '        Return mbAsciiStyle
    '    End Get
    'End Property
    Friend Property CurrentZone() As String
        '********************************************************************************************
        'Description: Name of current zone
        '
        'Parameters: None 
        'Returns: Zone Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/07/07   gks     set database path when changing zones
        '06/26/07   gks     Remove all the set this and that - use zone properties instead
        '********************************************************************************************
        Get
            If msCurrentZone = String.Empty Then msCurrentZone = msInitialZone
            CurrentZone = msCurrentZone
        End Get
        Set(ByVal value As String)

            For Each z As clsZone In List
                If z.Name = value Then
                    msCurrentZone = value
                    Exit For
                End If
            Next

        End Set

    End Property
    Friend ReadOnly Property CurrentZoneNumber() As Integer
        '********************************************************************************************
        'Description: for legacy
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ZoneNumberFromName(msCurrentZone)
        End Get

    End Property
    Friend ReadOnly Property DatabasePath() As String
        '********************************************************************************************
        'Description: Where this collection was loaded from - this should be set in the new event - 
        '             programs should use the database path from the active zone and not this as those
        '             zones could be on different computers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If msDBPath = String.Empty Then
                'should already be loaded, but just in case - comes from this machine
                GetDefaultFilePath(msDBPath, eDir.Database, String.Empty, String.Empty)
            End If
            Return msDBPath
        End Get
    End Property
    Friend ReadOnly Property HomeDBPath() As String
        '********************************************************************************************
        'Description: This is to keep track of the database path on this machine
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            'click your heels together 3 times and say "There's no place like home"...
            Return msHomeDBPath
        End Get
    End Property
    Friend ReadOnly Property InitialZone() As String
        '********************************************************************************************
        'Description: Zone name - to take the place of initial zone number
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msInitialZone
        End Get
    End Property
    Default Friend Overloads Property Item(ByVal ZoneNumber As Integer) As clsZone
        '********************************************************************************************
        'Description: Get or set a zone by its number
        '
        'Parameters: index
        'Returns:    clsZone
        '
        'Modification history:
        '
        ' Date      By      Reason
        '2/20/07    gks     This got it by index - changed to return by zone number
        '********************************************************************************************
        Get
            For Each o As clsZone In List
                If o.ZoneNumber = ZoneNumber Then
                    Return o
                End If
            Next
            'opps -  the 0 zone... nyuck nyuck
            Return Nothing

        End Get
        Set(ByVal value As clsZone)

            For Each o As clsZone In List
                If o.ZoneNumber = value.ZoneNumber Then
                    o = value
                    CurrentZone = value.Name
                End If
            Next

        End Set
    End Property
    Default Friend Overloads Property Item(ByVal vName As String) As clsZone
        '********************************************************************************************
        'Description: Get or set a zone by its name
        '
        'Parameters: name
        'Returns:    clsZone
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            For Each o As clsZone In List
                If o.Name = vName Then
                    Return o
                End If
            Next
            'opps
            Return Nothing
        End Get
        Set(ByVal value As clsZone)
            For Each o As clsZone In List
                If o.Name = value.Name Then
                    o = value
                    CurrentZone = value.Name
                    Exit For
                End If
            Next
        End Set
    End Property
    Friend ReadOnly Property SiteName() As String
        '********************************************************************************************
        'Description: Where am i?
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '5/7/07     gks     made read only
        '********************************************************************************************
        Get
            Return msSiteName
        End Get
    End Property
    Friend ReadOnly Property PaintShopComputer() As Boolean
        '********************************************************************************************
        'Description: Am I All Knowing?
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbPaintShopComputer
        End Get
    End Property
    Friend ReadOnly Property StandAlone() As Boolean
        '********************************************************************************************
        'Description: Do I only talk to FANUC Robots?
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbStandAlone
        End Get
    End Property
    Friend ReadOnly Property MISComputer() As Boolean
        '********************************************************************************************
        'Description: Am I All Knowing?
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbMISComputer
        End Get
    End Property
    Friend ReadOnly Property SaveAccessData() As Boolean
        '********************************************************************************************
        'Description: True to save more to access DBs for projhects with more old screens
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbSaveAccessData
        End Get
    End Property
    Friend ReadOnly Property NumberOfVINWords() As Integer
        '********************************************************************************************
        'Description: How many plc words make up the vin number
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumVINWords
        End Get
    End Property

#End Region
#Region " Routines "

    Friend Function Add(ByRef value As clsZone) As Integer
        '********************************************************************************************
        'Description: Add a zone to collection
        '
        'Parameters: clsZone
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Add(value)
    End Function

    Friend Function Contains(ByVal value As clsZone) As Boolean
        '********************************************************************************************
        'Description: If value is not of type clsZone, this will return false.
        '
        'Parameters: clsArm
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Contains(value)
    End Function 'Contains
    Friend Function IndexOf(ByVal value As clsZone) As Integer
        '********************************************************************************************
        'Description: Get Index of zone in collection
        '
        'Parameters: clszone
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Remove(ByVal value As clsZone)
        '********************************************************************************************
        'Description: Remove a zone 
        '
        'Parameters: clsZone
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Private Sub subGetZoneDBPaths()
        '********************************************************************************************
        'Description: get a database path for each zone in the collection
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            For Each z As clsZone In List
                If z.IsRemoteZone Then
                    If z.IsAvailable Then
                        Dim sPath As String = String.Empty
                        Dim RemotePWServer As String = "\\" & z.ServerName & z.ShareName
                        If GetDefaultFilePath(sPath, eDir.Database, RemotePWServer, String.Empty) Then
                            z.DatabasePath = sPath
                        Else
                            ''this just keeps getting worse...
                            'Throw New System.IO.FileNotFoundException( _
                            '        gcsRM.GetString("csCANT_ACCESS_DB_FILES_ZONE"), gsCONFIG_DBNAME)

                        End If
                    Else


                    End If
                Else
                    'should be the one we just looked up
                    z.DatabasePath = DatabasePath
                End If
            Next

        Catch exFile As IO.FileNotFoundException
            'we did it
            ShowErrorMessagebox(gcsRM.GetString("csWARNING"), exFile, msMODULE, String.Empty)
        Catch ex As Exception
            'it did us
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Friend Function SetCurrentZone(ByVal ZoneName As String, _
                                                    ByVal SkipProgramCheck As Boolean) As Boolean
        '********************************************************************************************
        'Description: point the zone collection to the desired zone NOTE: debugger does not stop 
        '               here on error. add a breakpoint if necessary
        '
        'Parameters: check to see if this program is running on remote computer
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     PSC remote Robot support 
        '********************************************************************************************
        Dim sOldZone As String = CurrentZone

        Try
            If ZoneName = String.Empty Then Return False

            CurrentZone = ZoneName
            'is zone online?
            If PaintShopComputer Then
                If ActiveZone.IsAvailable = False Then
                    MessageBox.Show(gcsRM.GetString("csSELECTED_ZONE_UNAVAILABLE"), _
                        gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                            MessageBoxOptions.DefaultDesktopOnly)
                    If sOldZone <> String.Empty Then CurrentZone = sOldZone
                    Return False
                Else

                    If SkipProgramCheck Then Return True

                    If ActiveZone.IsRemoteZone Then
                        'is this already running on the other computer?
                        'if you are not logged on as someone setup for access to wmi on remote computer
                        ' this will err out with various errors

                        Try

                            Dim remoteByName As Process() = Process.GetProcessesByName( _
                                            My.Application.Info.AssemblyName, ActiveZone.ServerName)

                            If remoteByName Is Nothing = False Then
                                'should only be 1 I thingk
                                If UBound(remoteByName) > -1 Then
                                    MessageBox.Show(gcsRM.GetString("csALREADY_RUNNING_REMOTE"), _
                                                My.Application.Info.AssemblyName, MessageBoxButtons.OK, _
                                                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                                                MessageBoxOptions.DefaultDesktopOnly)
                                    Return False
                                End If
                            End If 'remoteByName Is Nothing = False 

                        Catch ex As Exception

                            MessageBox.Show(gcsRM.GetString("csUNABLE_DETERMINE_RUNNING"), _
                                                My.Application.Info.AssemblyName, MessageBoxButtons.OK, _
                                                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                                                MessageBoxOptions.DefaultDesktopOnly)

                        End Try
                    End If 'ActiveZone.IsRemoteZone
                End If 'ActiveZone.IsAvailable = False 
            End If 'PaintShopComputer

            Return True

        Catch ex As Exception
            If sOldZone <> String.Empty Then CurrentZone = sOldZone
            Return False
        End Try


    End Function
    Friend Function SetCurrentZone(ByVal ZoneName As String) As Boolean
        '********************************************************************************************
        'Description: point the zone collection to the desired zone
        '               check to see if this program is running on remote computer
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return SetCurrentZone(ZoneName, False)

    End Function
    Friend Function ZoneNameFromNumber(ByVal ZoneNumber As Integer) As String
        '********************************************************************************************
        'Description: get the zone name given a number
        '
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each o As clsZone In List
            If o.ZoneNumber = ZoneNumber Then
                Return o.Name
            End If
        Next
        ' oops
        Return String.Empty

    End Function
    Friend Function ZoneNumberFromName(ByVal ZoneName As String) As Integer
        '********************************************************************************************
        'Description: get the zone number given a name
        '
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each o As clsZone In List
            If o.Name = ZoneName Then
                Return o.ZoneNumber
            End If
        Next
        ' opps
        Return 0

    End Function
    Private Sub subReadXMLSetupInfo()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        '********************************************************************************************
        Const sSETUP_XMLTABLE As String = "Setup"
        Const sSETUP_XMLNODE As String = "SetupInfo"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty

        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, gsSETUP_TABLENAME & ".XML") Then
                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sSETUP_XMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sSETUP_XMLNODE)

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("clsZones:subReadXMLSetupInfo", sXMLFilePath & " not found.")
                    Else
                        Try
                            msSiteName = oNodeList(0).Item("Site").InnerXml
                        Catch ex As Exception
                            msSiteName = "Site_Name"
                            ShowErrorMessagebox("Setup Data.XML : Invalid Site", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            msInitialZone = oNodeList(0).Item("InitialZone").InnerXml
                        Catch ex As Exception
                            msInitialZone = String.Empty
                            ShowErrorMessagebox("Setup Data.XML : Invalid InitialZone", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            mnNumVINWords = CType(oNodeList(0).Item("NumberOfVINWords").InnerXml, Integer)
                        Catch ex As Exception
                            mnNumVINWords = 0
                            ShowErrorMessagebox("Setup Data.XML : Invalid NumberOfVINWords", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            mbPaintShopComputer = CType(oNodeList(0).Item("PaintShopComputer").InnerXml, Boolean)
                        Catch ex As Exception
                            mbPaintShopComputer = False
                            ShowErrorMessagebox("Setup Data.XML : Invalid mbPaintShopComputer", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            mbStandAlone = CType(oNodeList(0).Item("StandAlone").InnerXml, Boolean)
                        Catch ex As Exception
                            mbStandAlone = False
                            ShowErrorMessagebox("Setup Data.XML : Invalid StandAlone", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            mbSaveAccessData = CType(oNodeList(0).Item("SaveAccessData").InnerXml, Boolean)
                        Catch ex As Exception
                            mbStandAlone = False
                            ShowErrorMessagebox("Setup Data.XML : Invalid SaveAccessData", ex, msMODULE, String.Empty)
                        End Try
                        Try
                            mbMISComputer = CType(oNodeList(0).Item("MISComputer").InnerXml, Boolean)
                        Catch ex As Exception
                            mbMISComputer = False
                            ShowErrorMessagebox("Setup Data.XML : Invalid mbMISComputer", ex, msMODULE, String.Empty)
                        End Try
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsZones:subReadXMLSetupInfo", "Invalid XML Data: [" & sXMLFilePath & "] - " & ex.Message)
                    ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                End Try

            Else
                MessageBox.Show("Cannot Find " & gsSETUP_TABLENAME & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)

            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("clsZones:subReadXMLSetupInfo", "Invalid XPath syntax: [" & sXMLFilePath & "] - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
        End Try
    End Sub
    Private Sub subReadXMLZoneInfo()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        '********************************************************************************************
        Const gsZONE_XMLTABLE As String = "Zones"
        Const gsZONE_XMLNODE As String = "ZoneInfo"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim bRemoteZone As Boolean = False
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, gsZONE_TABLENAME & ".XML") Then

                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & gsZONE_XMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & gsZONE_XMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("clsZones:subReadXMLZoneInfo", sXMLFilePath & " not found.")
                    Else
                        Dim bError As Boolean = False
                        For nNode As Integer = 0 To oNodeList.Count - 1
                            Try
                                bRemoteZone = CType(oNodeList(nNode).Item("RemoteZone").InnerXml, Boolean)
                            Catch ex As Exception
                                bRemoteZone = False
                            End Try
                            If mbPaintShopComputer OrElse (bRemoteZone = False) Then
                                Dim o As New clsZone(oNodeList, nNode)
                                Add(o)
                            End If
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("clsZones:subReadXMLSetupInfo", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    ShowErrorMessagebox("Invalid XML Data: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
                End Try
            Else

                MessageBox.Show("Cannot Find " & gsZONE_TABLENAME & ".XML", msMODULE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("clsZones:subReadXMLSetupInfo", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)

        End Try
    End Sub
#End Region
#Region " Events"

    Friend Sub New(ByVal sDatabasePath As String)
        '********************************************************************************************
        'Description: Make a new collection of zones. if sDatabasePath is empty it will use the 
        '               tables on this machine, if not it will follow that path
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        '********************************************************************************************
        MyBase.New()

        'keep track of the path on local computer
        GetDefaultFilePath(msHomeDBPath, eDir.Database, String.Empty, String.Empty)

        'this path is to the Zone Info table we will use
        If sDatabasePath <> String.Empty Then
            msDBPath = sDatabasePath
        Else
            'this is the normal path - reading zone info table from this computer
            msDBPath = msHomeDBPath
        End If

        subReadXMLSetupInfo()
        subReadXMLZoneInfo()
        
        'Call bLoadCollection()
        'some of the Zones just loaded may be remote to this machine
        'set each zones dbPath so we know where to look for stuff
        Call subGetZoneDBPaths()

        Me.CurrentZone = InitialZone

    End Sub
    

#End Region

 
End Class 'clsZones
'********************************************************************************************
'Description: a single zone. you are now in the zone.
'
'
'Modification history:
'
' Date      By      Reason
' 10/23/09  BTK     Added mnRobotsRequiredStartingBit so we can tell config screens where the
'                   first robot starts in the PLC, generally zero or one.
' 04/12/13  RJO     Added AreaVolumeEnabled and LearnedVolumeEnabled to support Sealer screens
' 05/17/13  MSW     Add conversion for VIN from display to PLCData and back.  In the long run, 
'                   get all the programs that mess with the VIN to use this instead of doing 
'                   it a bunch of different ways
'10/25/13   BTK     Tricoat By Style
'******************************************************************************************** 
Friend Class clsZone

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop
    Private msName As String = String.Empty
    Private mnMaxStyles As Integer = 1
    Private mnZoneNumber As Integer = 1
    Private mnNumStations As Integer = 0
    Private msStationName() As String = Nothing   'kdp
    Private msIPAddress As String = "127.0.0.1"
    Private msServerName As String = "localhost"
    Private mbSelected As Boolean ' = False
    '2/17/07 had to add for colors by style
    Private mnMaxColors As Integer = 1            ' this is how many in controller
    Private mnEffectiveColors As Integer = 1      ' this is how many real world colors there are
    Private mbPresetsByStyle As Boolean ' = False ' this is a hack
    Private mnMaxOptions As Integer = 1
    Private mnMaxValves As Integer = 1
    Private mnRepairPanels As Integer ' = 0
    Private mEstatType As eEstatType = eEstatType.None
    Private mPLCType As ePLCType = ePLCType.None
    Private msPLCHostName As String = "localhost"
    Private msPLCIPAddress As String = "127.0.0.1"
    Private mbEnableColor As Boolean ' = False
    Private mbRemoteZone As Boolean ' = False
    Private msDB_Path As String = String.Empty
    Private msShareName As String = String.Empty
    Private mbSplitShell As Boolean  '4/8/08
    Private mbDegradeEnabled As Boolean  '7/3/08
    Private mbIntrusionSetupEnabled As Boolean '8/10/09
    Private msLineDirection As String '8/11/09
    Private mbAreaVolEnabled As Boolean 'RJO 04/12/13
    Private mbLearnedVolEnabled As Boolean 'RJO 04/12/13
    Private mbVisionEnabled As Boolean
    Private mbVisionChangeLogEnabled As Boolean
    Private mbStyleIDEnabled As Boolean 'BTK 02/09/10
    Private mnMaxPEIDs As Integer 'JBW 04/19/11
    Private mnRobotsRequiredStartingBit As Integer '10/23/09 Where do the robots start
    Private mbDegradeRepair As Boolean
    Private mbRobotsOnSameNet As Boolean
    'Centralize VIN handling code
    Private msVinType As String
    Private mnVinWords As Integer
    Private mnVinSubType As Integer = 1
    '10/25/13 BTK Tricoat By Style
    Private mbTricoatByStyle As Boolean
    '3/6/14 CBZ add sealer flag
    Private mbIsSealer As Boolean ' = False
    '10/30/13 BTK Degrade Style Robots Required
    Private mbDegradeStyleRbtsReq As Boolean
    Private mnNumberOfDegrades As Integer
    
#End Region
#Region " Properties"

    Friend ReadOnly Property VinType() As String
        '********************************************************************************************
        'Description: Vin type
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msVinType
        End Get
    End Property
    Friend Property VinWords() As Integer
        '********************************************************************************************
        'Description: Number of words in PLC data used for the vin 
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnVinWords
        End Get
        Set(ByVal value As Integer)
            mnVinWords = value
        End Set
    End Property
    Friend ReadOnly Property AreaVolumeEnabled() As Boolean
        '********************************************************************************************
        'Description: Enables Sealer Volumes screen Area Volumes tab
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbAreaVolEnabled
        End Get
    End Property
    Friend Property DatabasePath() As String
        '********************************************************************************************
        'Description: path to database for this zone 
        '
        'Parameters:  none
        'Returns:   Text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msDB_Path
        End Get
        Set(ByVal value As String)
            msDB_Path = value
        End Set
    End Property
    Friend Property LineDirection() As String
        '********************************************************************************************
        'Description: conveyor line direction for choosing BSD screens 
        '
        'Parameters:  none
        'Returns:   Text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msLineDirection
        End Get
        Set(ByVal value As String)
            msLineDirection = value
        End Set
    End Property
    Friend ReadOnly Property DegradeEnabled() As Boolean
        '********************************************************************************************
        'Description: Needed for Colors by Style Hack = to number of colors otherwise
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '7/3/08     gks     new
        '********************************************************************************************
        Get
            Return mbDegradeEnabled
        End Get
    End Property
    Friend ReadOnly Property DegradeRepair() As Boolean
        '********************************************************************************************
        'Description: Needed for Colors by Style Hack = to number of colors otherwise
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/07/11     msw     new
        '********************************************************************************************
        Get
            Return mbDegradeRepair
        End Get
    End Property
    Friend ReadOnly Property DegradeStyleRbtsReq() As Boolean
        '********************************************************************************************
        'Description: Needed for degrade style robots req.
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/30/13 BTK Degrade Style Robots Required
        '********************************************************************************************
        Get
            Return mbDegradeStyleRbtsReq
        End Get
    End Property

    Friend ReadOnly Property RobotsOnSameNet() As Boolean
        '********************************************************************************************
        'Description: For remote zones, true = direct robot connection, false = use RPC connection to
        '             remote PC
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     PSC remote Robot support 
        '********************************************************************************************
        Get
            Return mbRobotsOnSameNet
        End Get
    End Property

    Friend ReadOnly Property IntrusionSetupEnabled() As Boolean
        '********************************************************************************************
        'Description: enable intrusion setup screen
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '8/10/09    msw     add option for intrusion setup
        '********************************************************************************************
        Get
            Return mbIntrusionSetupEnabled
        End Get
    End Property
    Friend ReadOnly Property EffectiveColors() As Integer
        '********************************************************************************************
        'Description: Needed for Colors by Style Hack = to number of colors otherwise
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnEffectiveColors
        End Get
    End Property
    Friend ReadOnly Property IPAddress() As String
        '********************************************************************************************
        'Description: Zones internet Address
        '
        'Parameters:  none
        'Returns:    address
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msIPAddress
        End Get
    End Property
    Friend ReadOnly Property IsAvailable() As Boolean
        '********************************************************************************************
        'Description: ping zone computer and see if it answers
        '
        'Parameters:  none
        'Returns:    true if available
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If ServerName = String.Empty Then Return True 'local zone
            Dim sTmp As String = ServerName
            'may be local
            If mbRemoteZone = False Then sTmp = "localhost"

            If My.Computer.Network.Ping(sTmp) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Friend ReadOnly Property IsRemoteZone() As Boolean
        '********************************************************************************************
        'Description: is zone on this computer?
        '
        'Parameters:  none
        'Returns:    true if not local
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbRemoteZone
        End Get
    End Property
    Friend ReadOnly Property LearnedVolumeEnabled() As Boolean
        '********************************************************************************************
        'Description: Enables Sealer Volume screen Learned Volumes tab
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbLearnedVolEnabled
        End Get
    End Property
    Friend ReadOnly Property MaxStyles() As Integer
        '********************************************************************************************
        'Description: How Many Styles
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxStyles
        End Get
    End Property
    Friend ReadOnly Property NumberOfDegrades() As Integer
        '********************************************************************************************
        'Description: How Many Styles
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumberOfDegrades
        End Get
    End Property
    Friend ReadOnly Property MaxColors() As Integer
        '********************************************************************************************
        'Description: Needed for Colors by Style Hack - this is how many in robot - not effective
        '               colors if colors by style
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxColors
        End Get
    End Property
    Friend ReadOnly Property MaxValves() As Integer
        '********************************************************************************************
        'Description: how many valves
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxValves
        End Get
    End Property
    Friend ReadOnly Property MaxOptions() As Integer
        '********************************************************************************************
        'Description: PLC data . Do I have another option?
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxOptions
        End Get
    End Property
    Friend ReadOnly Property RepairPanels() As Integer
        '********************************************************************************************
        'Description: PLC data
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnRepairPanels
        End Get
    End Property
    Friend ReadOnly Property Name() As String
        '********************************************************************************************
        'Description: Zone Name
        '
        'Parameters:  none
        'Returns:   Text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msName
        End Get
    End Property
    Friend ReadOnly Property StationName(ByVal StationNumber As Integer) As String
        '********************************************************************************************
        'Description: Zone Name
        '
        'Parameters:  none
        'Returns:   Text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msStationName(StationNumber)
        End Get
    End Property

    Friend ReadOnly Property PresetsByStyle() As Boolean
        '********************************************************************************************
        'Description: Needed for Colors by Style Hack
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbPresetsByStyle
        End Get
    End Property
    Friend Property Selected() As Boolean
        '********************************************************************************************
        'Description: is this zone selected
        '
        'Parameters:  none
        'Returns:    true if selected
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbSelected
        End Get
        Set(ByVal value As Boolean)
            mbSelected = value
        End Set
    End Property
    Friend ReadOnly Property UseSplitShell() As Boolean
        '********************************************************************************************
        'Description: TODO - for the by arm or by controller stuff
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbSplitShell
        End Get
    End Property
    Friend ReadOnly Property ZoneNumber() As Integer
        '********************************************************************************************
        'Description: Zone Number For Old Stuff
        '
        'Parameters:  none
        'Returns:    true if selected
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnZoneNumber
        End Get
    End Property
    Friend ReadOnly Property NumStations() As Integer
        '********************************************************************************************
        'Description: Number of Stations
        '
        'Parameters:  none
        'Returns:    true if selected
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumStations
        End Get
    End Property
    Friend ReadOnly Property PLCType() As ePLCType
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
        Get
            Return mPLCType
        End Get
    End Property
    Friend ReadOnly Property PLCTagPrefix() As String
        '********************************************************************************************
        'Description: use for reading plc tags
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return "Z" & mnZoneNumber.ToString(mLanguage.FixedCulture)
        End Get
    End Property
    Friend ReadOnly Property PLCHostName() As String
        Get
            Return msPLCHostName
        End Get
    End Property
    Friend ReadOnly Property PLCIPAddress() As String
        Get
            Return msPLCIPAddress
        End Get
    End Property
    Friend ReadOnly Property EstatType() As eEstatType
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
        Get
            Return mEstatType
        End Get
    End Property
    Friend ReadOnly Property RemotePath() As String
        '********************************************************************************************
        'Description: path to the remote share name
        '
        'Parameters:  none
        'Returns:    name - add your own \\ later
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If IsRemoteZone Then
                If Left(msShareName, 1) <> "\" Then msShareName = "\" & msShareName
                Return "\\" & msServerName & "\" & msShareName
            Else
                Return String.Empty
            End If
        End Get
    End Property
    Friend ReadOnly Property ServerName() As String
        '********************************************************************************************
        'Description: this is the computer or host name
        '
        'Parameters:  none
        'Returns:    name - add your own \\ later
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msServerName
        End Get
    End Property
    Friend ReadOnly Property EnableColor() As Boolean
        '********************************************************************************************
        'Description: for car color cartoons someday
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnableColor
        End Get
    End Property

    Friend ReadOnly Property TricoatByStyle() As Boolean
        '********************************************************************************************
        'Description: Needed for Tricoat by Style
        '
        'Parameters:  none
        'Returns:   number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        Get
            Return mbTricoatByStyle
        End Get
    End Property

    Friend ReadOnly Property IsSealer() As Boolean
        '********************************************************************************************
        'Description: Is a Sealer Cell
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '3/6/14 CBZ add sealer flag
        '********************************************************************************************
        Get
            Return mbIsSealer
        End Get
    End Property

    Friend ReadOnly Property ShareName() As String
        '********************************************************************************************
        'Description: name of share used to access paint directory on remote computer
        '
        'Parameters:  none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If Left(msShareName, 1) <> "\" Then msShareName = "\" & msShareName
            Return msShareName
        End Get
    End Property
    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description: path to XML files folder for this zone 
        '
        'Parameters:  none
        'Returns:   Text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msDB_Path & "XML\"
        End Get
    End Property
    Friend ReadOnly Property VisionChangeLogEnabled() As Boolean
        '********************************************************************************************
        'Description: for vision someday
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbVisionChangeLogEnabled
        End Get
    End Property
    Friend ReadOnly Property VisionEnabled() As Boolean
        '********************************************************************************************
        'Description: for vision someday
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbVisionEnabled
        End Get
    End Property
    Friend ReadOnly Property StyleIDEnabled() As Boolean
        '********************************************************************************************
        'Description: for vision someday
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbStyleIDEnabled
        End Get
    End Property

    Friend ReadOnly Property maxPEIDs() As Integer
        '********************************************************************************************
        'Description: for vision someday
        '
        'Parameters:  
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/11  JBW     For zone setup for body ID photo eyes
        '********************************************************************************************
        Get
            Return mnMaxPEIDs
        End Get
    End Property


#End Region
#Region " Events "

    Friend Sub New(NodeList As XmlNodeList, nNode as integer)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        ' 04/12/13  RJO     Added initialization for AreaVolumeEnabled and LearnedVolumeEnabled
        ' 08/20/13  MSW     PSC remote Robot support 
        ' 10/30/13  BTK     Degrade Style Robots Required
        '********************************************************************************************
        MyBase.New()

        ' to allow a dummy zone
        If NodeList Is Nothing Then Exit Sub

        With NodeList(nNode)
            Try
                msName = .Item("ZoneName").InnerXml
            Catch ex As Exception
                msName = "Zone"
                ShowErrorMessagebox("Zone Info.XML : Invalid ZoneName", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnZoneNumber = CType(.Item("ZoneNumber").InnerXml, Integer)
            Catch ex As Exception
                mnZoneNumber = 1
                ShowErrorMessagebox("Zone Info.XML : Invalid ZoneNumber", ex, "ClsZone", String.Empty)
            End Try
            If NodeList(nNode).InnerXml.Contains("NumStations") Then
                Try
                    mnNumStations = CType(.Item("NumStations").InnerXml, Integer)
                Catch ex As Exception
                    mnNumStations = 0
                    mDebug.WriteEventToLog("clsZone, Routine: New - NumStations", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Else
                mnNumStations = 0
            End If

            'kdp  Added Station Names 02/06/14
            If mnNumStations > 0 Then
                For nWord As Integer = 0 To mnNumStations - 1
                    ReDim Preserve msStationName(nWord)
                    If NodeList(nNode).InnerXml.Contains("StationName") Then
                        Dim sName As String = "StationName"
                        Try
                            sName = "StationName" & CType(nWord + 1, Integer).ToString
                            msStationName(nWord) = .Item(sName).InnerXml
                        Catch ex As Exception
                            msStationName(nWord) = msName & " " & nWord + 1
                            mDebug.WriteEventToLog("clsZone, Routine: New - " & sName, _
                                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                        End Try
                    Else
                        msStationName(nWord) = msName & " " & nWord + 1
                    End If
                Next
            End If


            Try
                mnMaxStyles = CType(.Item("MaxStyles").InnerXml, Integer)
            Catch ex As Exception
                mnMaxStyles = 30
                ShowErrorMessagebox("Zone Info.XML : Invalid MaxStyles", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnMaxColors = CType(.Item("MaxColors").InnerXml, Integer)
            Catch ex As Exception
                mnMaxColors = 30
                ShowErrorMessagebox("Zone Info.XML : Invalid MaxColors", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnMaxValves = CType(.Item("MaxValves").InnerXml, Integer)
            Catch ex As Exception
                mnMaxValves = 8
                ShowErrorMessagebox("Zone Info.XML : Invalid MaxValves", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnMaxOptions = CType(.Item("MaxOptions").InnerXml, Integer)
            Catch ex As Exception
                mnMaxOptions = 10
                ShowErrorMessagebox("Zone Info.XML : Invalid MaxOptions", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnRepairPanels = CType(.Item("MaxRepairPanels").InnerXml, Integer)
            Catch ex As Exception
                mnRepairPanels = 0
                ShowErrorMessagebox("Zone Info.XML : Invalid MaxRepairPanels", ex, "ClsZone", String.Empty)
            End Try
            Try
                msServerName = .Item("HostName").InnerXml
                msIPAddress = mPWCommon.IPFromHostName(msServerName)
            Catch ex As Exception
                msServerName = String.Empty
                msIPAddress = String.Empty
                ShowErrorMessagebox("Zone Info.XML : Invalid HostName", ex, "ClsZone", String.Empty)
            End Try
            Try
                msPLCHostName = .Item("PLCHostName").InnerXml
                If msPLCHostName <> String.Empty Then
                    msPLCIPAddress = mPWCommon.IPFromHostName(msPLCHostName)
                End If
            Catch ex As Exception
                msPLCIPAddress = String.Empty
                ShowErrorMessagebox("Zone Info.XML : Invalid PLCHostName", ex, "ClsZone", String.Empty)
            End Try
            Try
                mPLCType = CType(CType(.Item("PLCType").InnerXml, Integer), ePLCType)
            Catch ex As Exception
                mPLCType = ePLCType.None
                ShowErrorMessagebox("Zone Info.XML : Invalid PLCType", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbEnableColor = CType(.Item("EnableColor").InnerXml, Boolean)
            Catch ex As Exception
                mbEnableColor = False
                ShowErrorMessagebox("Zone Info.XML : Invalid EnableColor", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbRemoteZone = CType(.Item("RemoteZone").InnerXml, Boolean)
            Catch ex As Exception
                mbRemoteZone = False
                ShowErrorMessagebox("Zone Info.XML : Invalid RemoteZone", ex, "ClsZone", String.Empty)
            End Try
            Try
                msShareName = .Item("ShareName").InnerXml
            Catch ex As Exception
                msShareName = String.Empty
                ShowErrorMessagebox("Zone Info.XML : Invalid ShareName", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbSplitShell = CType(.Item("SplitShell").InnerXml, Boolean)
            Catch ex As Exception
                mbSplitShell = False
                ShowErrorMessagebox("Zone Info.XML : Invalid SplitShell", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbDegradeEnabled = CType(.Item("EnableDegrade").InnerXml, Boolean)
            Catch ex As Exception
                mbDegradeEnabled = False
                ShowErrorMessagebox("Zone Info.XML : Invalid EnableDegrade", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbIntrusionSetupEnabled = CType(.Item("EnableIntrusionSetup").InnerXml, Boolean)
            Catch ex As Exception
                mbIntrusionSetupEnabled = False
                ShowErrorMessagebox("Zone Info.XML : Invalid EnableIntrusionSetup", ex, "ClsZone", String.Empty)
            End Try
            Try
                msLineDirection = CType(.Item("LineDirection").InnerXml, String)
            Catch ex As Exception
                msLineDirection = String.Empty
                ShowErrorMessagebox("Zone Info.XML : Invalid LineDirection", ex, "ClsZone", String.Empty)
            End Try
            Try
                mnRobotsRequiredStartingBit = CType(.Item("RobotsRequiredStartingBit").InnerXml, Integer)
            Catch ex As Exception
                mnRobotsRequiredStartingBit = 1
                ShowErrorMessagebox("Zone Info.XML : Invalid RobotsRequiredStartingBit", ex, "ClsZone", String.Empty)
            End Try
            Try
                mbStyleIDEnabled = CType(.Item("EnableStyleIDSetup").InnerXml, Boolean)
            Catch ex As Exception
                mbStyleIDEnabled = False
                mDebug.WriteEventToLog("clsZone, Routine: New - EnableStyleIDSetup", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mbDegradeRepair = CType(.Item("DegradeRepair").InnerXml, Boolean)
            Catch ex As Exception
                mbDegradeRepair = False
                mDebug.WriteEventToLog("clsZone, Routine: New - DegradeRepair", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mbAreaVolEnabled = CType(.Item("EnableAreaVol").InnerXml, Boolean)
            Catch ex As Exception
                mbAreaVolEnabled = False
                mDebug.WriteEventToLog("clsZone, Routine: New - EnableAreaVol", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mbLearnedVolEnabled = CType(.Item("EnableLearnedVol").InnerXml, Boolean)
            Catch ex As Exception
                mbLearnedVolEnabled = False
                mDebug.WriteEventToLog("clsZone, Routine: New - EnableLearnedVol", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mbVisionEnabled = CType(.Item("EnableVision").InnerXml, Boolean)
            Catch ex As Exception
                mbVisionEnabled = False
                mDebug.WriteEventToLog("clsZone, Routine: New - EnableVision", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mbVisionChangeLogEnabled = CType(.Item("VisionChangeLogEnabled").InnerXml, Boolean)
            Catch ex As Exception
                mbVisionChangeLogEnabled = False
                mDebug.WriteEventToLog("clsZone, Routine: New - VisionChangeLogEnabled", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                msVinType = CType(.Item("VinType").InnerXml, String)

                If msVinType.ToLower.StartsWith("ascii") AndAlso (msVinType.Length > 5) Then
                    mnVinSubType = CInt(msVinType.Substring(5))
                ElseIf msVinType.ToLower.StartsWith("numbertext") AndAlso (msVinType.Length > 10) Then
                    mnVinSubType = CInt(msVinType.Substring(10))
                Else
                    mnVinSubType = 1
                End If
            Catch ex As Exception
                msVinType = "Number"
                mnVinSubType = 1
                mDebug.WriteEventToLog("clsZone, Routine: New - VinType", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Try
                mnVinWords = CType(.Item("VinWords").InnerXml, Integer)
            Catch ex As Exception
                mnVinWords = 1
                mDebug.WriteEventToLog("clsZone, Routine: New - VinWords", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            If NodeList(nNode).InnerXml.Contains("RobotsOnSameNet") Then
                Try
                    mbRobotsOnSameNet = CType(.Item("RobotsOnSameNet").InnerXml, Boolean)
                Catch ex As Exception
                    mbRobotsOnSameNet = True
                    mDebug.WriteEventToLog("clsZone, Routine: New - RobotsOnSameNet", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Else
                mbRobotsOnSameNet = True
            End If
            If NodeList(nNode).InnerXml.Contains("EnableTricoatByStyle") Then
                Try
                    mbTricoatByStyle = CType(.Item("EnableTricoatByStyle").InnerXml, Boolean)
                Catch ex As Exception
                    mbTricoatByStyle = False
                    mDebug.WriteEventToLog("clsZone, Routine: New - Enable Tricoat By Style", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Else
                mbTricoatByStyle = False
            End If

            '3/6/14 CBZ add sealer flag
            If NodeList(nNode).InnerXml.Contains("IsSealer") Then
                Try
                    mbIsSealer = CType(.Item("IsSealer").InnerXml, Boolean)
                Catch ex As Exception
                    mbIsSealer = False
                    mDebug.WriteEventToLog("clsZone, Routine: New - Is Sealer Cell", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Else
                mbIsSealer = False
            End If
            '10/30/13 BTK Degrade Style Robots Required 
            If NodeList(nNode).InnerXml.Contains("DegradeStyleRbtsReq") Then
                Try
                    mbDegradeStyleRbtsReq = CType(.Item("DegradeStyleRbtsReq").InnerXml, Boolean)
                Catch ex As Exception
                    mbDegradeStyleRbtsReq = False
                    mDebug.WriteEventToLog("clsZone, Routine: New - DegradeStyleRbtsReq", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
                Try
                    mnNumberOfDegrades = CType(.Item("NumberOfDegrades").InnerXml, Integer)
                Catch ex As Exception
                    mnNumberOfDegrades = 0
                    mDebug.WriteEventToLog("clsZone, Routine: New - NumberOfDegrades", _
                                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                    If mbDegradeStyleRbtsReq Then
                        ShowErrorMessagebox("Zone Info.XML : Invalid Number Of Degrades", ex, "ClsZone", String.Empty)
                    End If
                End Try
            Else
                mnNumberOfDegrades = 0
                mbDegradeStyleRbtsReq = False
            End If
        End With
    End Sub

    Friend Sub New(ByVal vDataRow As DataRow)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        ' 05/27/11  MSW     Add vision 
        ' 08/20/13  MSW     PSC remote Robot support
        ' 10/25/13  BTK     Tricoat By Style
        ' 10/30/13  BTK     Degrade Style Robots Required
        '********************************************************************************************
        MyBase.New()

        ' to allow a dummy zone
        If vDataRow Is Nothing Then Exit Sub

        With vDataRow
            msName = .Item(gsZONE_NAME).ToString
            mnZoneNumber = CType(.Item("Zone Number"), Integer)
            mnNumStations = CType(.Item("Num Stations"), Integer)
            mnMaxStyles = CType(.Item(gsZONE_COL_MAXSTY), Integer)
            mnMaxColors = CType(.Item(gsZONE_COL_MAXCOL), Integer)
            mnMaxValves = CType(.Item("Max Valves"), Integer)
            mnMaxOptions = CType(.Item("Max Options"), Integer)
            mnRepairPanels = CType(.Item("Max Repair Panels"), Integer)
            msServerName = .Item("Host Name").ToString
            msIPAddress = mPWCommon.IPFromHostName(msServerName)
            msPLCHostName = .Item("PLC Host Name").ToString
            If msPLCHostName <> String.Empty Then
                msPLCIPAddress = mPWCommon.IPFromHostName(msPLCHostName)
            End If
            mPLCType = CType(CType(.Item("PLC Type"), Integer), ePLCType)
            mbEnableColor = CType(.Item("Enable Color"), Boolean)
            mbRemoteZone = CType(.Item("Remote Zone"), Boolean)
            msShareName = .Item("Share Name").ToString
            mbSplitShell = CType(.Item("Split Shell"), Boolean)
            mbDegradeEnabled = CType(.Item("Enable Degrade"), Boolean)
            mbIntrusionSetupEnabled = CType(.Item("Enable Intrusion Setup"), Boolean)
            msLineDirection = CType(.Item("Line Direction"), String)
            mnRobotsRequiredStartingBit = CType(.Item("Robots Required Starting Bit"), Integer)
            mbStyleIDEnabled = CType(.Item("Enable Style ID Setup"), Boolean)
            Try
                mbDegradeRepair = CType(.Item("Degrade Repair"), Boolean)
            Catch ex As Exception
                mbDegradeRepair = False
            End Try
            If .Item("Enable Vision") IsNot Nothing Then
                mbVisionEnabled = CType(.Item("Enable Vision"), Boolean)
            Else
                mbVisionEnabled = False
            End If
            If .Item("VisionChangeLogEnabled") IsNot Nothing Then
                mbVisionChangeLogEnabled = CType(.Item("VisionChangeLogEnabled"), Boolean)
            Else
                mbVisionChangeLogEnabled = False
            End If
            If .Item("RobotsOnSameNet") IsNot Nothing Then
                mbRobotsOnSameNet = CType(.Item("RobotsOnSameNet"), Boolean)
            Else
                mbRobotsOnSameNet = True
            End If

            ' 10/25/13  BTK     Tricoat By Style
            Try
                mbTricoatByStyle = CType(.Item("EnableTricoatByStyle"), Boolean)
            Catch ex As Exception
                mbTricoatByStyle = False
            End Try

            '3/6/14 CBZ add sealer flag
            Try
                mbIsSealer = CType(.Item("IsSealer"), Boolean)
            Catch ex As Exception
                mbIsSealer = False
            End Try

            '10/30/13 BTK Degrade Style Robots Required DegradeStyleRbtsReq
            Try
                mbDegradeStyleRbtsReq = CType(.Item("DegradeStyleRbtsReq"), Boolean)
            Catch ex As Exception
                mbDegradeRepair = False
            End Try

            Try
                mnNumberOfDegrades = CType(.Item("NumberOfDegrades"), Integer)
            Catch ex As Exception
                mnNumberOfDegrades = 0
            End Try
            'stuff we dont need here???
            'mEstatType = CType(.Item("Estat Type"), eEstatType)
            'mbPresetsByStyle = CType(.Item(gsZONE_COL_BYSTYLE), Boolean)

            'If mbPresetsByStyle Then
            '    If mnMaxStyles <> 0 Then
            '        mnEffectiveColors = CInt(mnMaxColors / mnMaxStyles)
            '    Else
            '        mnEffectiveColors = 0
            '    End If
            'Else
            '    mnEffectiveColors = mnMaxColors
            'End If

        End With
    End Sub

    Friend ReadOnly Property RobotsRequiredStartingBit() As Integer
        '********************************************************************************************
        'Description: Determine where the robots start.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************
        Get
            Return mnRobotsRequiredStartingBit
        End Get
    End Property
    Friend Function sGetVinString(ByRef sData() As String, Optional ByVal nStartIdx As Integer = 0) As String
        '********************************************************************************************
        'Description: Convert PLC data to a VIN in string form
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason

        '********************************************************************************************
        Dim nData(mnVinWords - 1) As Integer
        For nItem As Integer = 0 To mnVinWords - 1
            Try
                nData(nItem) = CInt(sData(nItem + nStartIdx))
            Catch ex As Exception
                nData(nItem) = 0
            End Try
        Next
        Return sGetVinString(nData)
    End Function
    Friend Function sGetVinString(ByRef nData() As Integer, Optional ByVal nStartIdx As Integer = 0) As String
        '********************************************************************************************
        'Description: Convert PLC data to a VIN in string form
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason

        '********************************************************************************************
        Dim sReturn As String = String.Empty
        Dim nTmp As Decimal 'VINs can get big
        Try
            Select Case msVinType.ToLower
                Case "int", "integer"
                    'Simple number, one or two words - assumes two words are 16 bit combined for 32 bits
                    'lower index is LSW, higher index is MSW
                    Select Case mnVinWords
                        Case 1
                            nTmp = nData(nStartIdx)
                        Case 2
                            nTmp = nData(nStartIdx) + (nData(nStartIdx + 1) * (UShort.MaxValue + 1))
                        Case Else
                            nTmp = 0
                    End Select
                    sReturn = nTmp.ToString
                Case "numbertext", "numbertext1", "numbertext2", "numbertext3", "numbertext4"
                    'Simple numbers, but combine as text.  Lower index on the left
                    Dim sFormat As String = "0"
                    Select Case mnVinSubType
                        Case 2
                            sFormat = "00"
                        Case 3
                            sFormat = "000"
                        Case 4
                            sFormat = "0000"
                        Case Else
                            sFormat = "0"
                    End Select
                    sReturn = nData(nStartIdx).ToString(sFormat)
                    If mnVinWords > 1 Then
                        For nWord As Integer = (nStartIdx + 1) To (nStartIdx + mnVinWords - 1)
                            sReturn = sReturn & nData(nWord).ToString(sFormat)
                        Next
                    End If
                Case "ascii", "ascii1", "ascii2", "ascii3", "ascii4"
                    'ascii character for each PLC word, lower index on the left
                    sReturn = mMathFunctions.CvIntegerToASCII(nData(nStartIdx), mnVinSubType)
                    If mnVinWords > 1 Then
                        For nWord As Integer = (nStartIdx + 1) To (nStartIdx + mnVinWords - 1)
                            sReturn = sReturn & mMathFunctions.CvIntegerToASCII(nData(nWord), mnVinSubType)
                        Next
                    End If
                Case Else
                    sReturn = nData(nStartIdx).ToString
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog("clsZone, Routine: sGetVinString", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Return sReturn
    End Function
    Friend Sub GetVinArray(ByRef sVIN As String, ByRef sData() As String, Optional ByVal nStartIdx As Integer = 0)
        '********************************************************************************************
        'Description: Convert VIN in string form to an array of integers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTmp() As Integer = nGetVinArray(sVIN)
        For nWord As Integer = 0 To mnVinWords - 1
            sData(nStartIdx + nWord) = nTmp(nWord).ToString
        Next
    End Sub
    Friend Sub GetVinArray(ByRef sVIN As String, ByRef nData() As Integer, Optional ByVal nStartIdx As Integer = 0)
        '********************************************************************************************
        'Description: Convert VIN in string form to an array of integers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTmp() As Integer = nGetVinArray(sVIN)
        For nWord As Integer = 0 To mnVinWords - 1
            nData(nStartIdx + nWord) = nTmp(nWord)
        Next
    End Sub
    Friend Function nGetVinArray(ByRef sVIN As String) As Integer()
        '********************************************************************************************
        'Description: Convert VIN in string form to an array of integers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nReturn(mnVinWords - 1) As Integer
        For Each nWord As Integer In nReturn
            nWord = 0
        Next
        Dim nTmp As Decimal 'VINs can get big
        Try
            If IsNumeric(sVIN) Then
                Select Case msVinType.ToLower
                    Case "int", "integer"
                        'Simple number, one or two words - assumes two words are 16 bit combined for 32 bits
                        'lower index is LSW, higher index is MSW
                        nReturn(0) = CInt(sVIN)
                        If mnVinWords > 0 Then
                            nReturn(1) = nReturn(0) \ (UShort.MaxValue + 1)
                            nReturn(0) = nReturn(0) And UShort.MaxValue
                        End If
                    Case "numbertext", "numbertext1", "numbertext2", "numbertext3", "numbertext4"
                        'Simple numbers, but combine as text.  Lower index on the left
                        Dim nStrLen As Integer = mnVinSubType * mnVinWords
                        If nStrLen > sVIN.Length Then
                            Do While nStrLen > sVIN.Length
                                sVIN = "0" & sVIN
                            Loop
                        Else
                            sVIN = sVIN.Substring(sVIN.Length - nStrLen, nStrLen)
                        End If
                        For nWord As Integer = 0 To mnVinWords - 1
                            Dim sTmp As String = sVIN.Substring(nWord * mnVinSubType, mnVinSubType)
                            nReturn(nWord) = CInt(nTmp)
                        Next
                    Case "ascii", "ascii1", "ascii2", "ascii3", "ascii4"
                        'ascii character for each PLC word, lower index on the left
                        Dim nStrLen As Integer = mnVinSubType * mnVinWords
                        If nStrLen > sVIN.Length Then
                            Do While nStrLen > sVIN.Length
                                sVIN = "0" & sVIN
                            Loop
                        Else
                            sVIN = sVIN.Substring(sVIN.Length - nStrLen, nStrLen)
                        End If
                        For nWord As Integer = 0 To mnVinWords - 1
                            Dim sTmp As String = sVIN.Substring(nWord * mnVinSubType, mnVinSubType)
                            nReturn(nWord) = mMathFunctions.CvASCIIToInteger(sTmp, mnVinSubType)
                        Next
                    Case Else
                        nReturn(0) = CInt(sVIN)
                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("clsZone, Routine: nGetVinArray", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Return nReturn

    End Function
#End Region

End Class 'clsZone
