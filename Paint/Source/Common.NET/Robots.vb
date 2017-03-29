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
' Form/Module: Robots.vb
'
' Description: Collection for robots, robot class, and robot common code
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    03/20/08   gks     Switch to SQL Database "Configuration"
'    02/11/09   gks     Add version
'    05/29/09   MSW	    switch to VB 2008
' 						clsArm - Remove mApplicator, always use CCtype
'						mPWRobotCommon - Add "select by cc type" to LoadRobotBoxFromCollection,
'						add LoadRobotBoxFromCollectionByCCTypeName
'    11/05/09   RJO	    mPWRobotCommon - Add IncludeOpeners to LoadRobotBoxFromCollection when
'                       when collection is Arm Collection
'    11/05/09   MSW     add NOT_NONE as color change type option
'    11/05/09   MSW     subLoadCheckListBox - Set the width before adding names to prevent a scrollbar
'    11/10/09   MSW     Add robot variable lookup through resource files.
'    11/19/09   RJO     Added as second LoadArmCollection with an IncludeOpeners property.
'    11/23/09   BTK     Added variables in mPWRobotCommon_subIndexScatteredAccess for dual shaping air.
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    10/14/11   MSW     Read from XML                                                   4.1.0.2
'    10/17/11   MSW     Add Dmon Config                                                 4.1.0.2
'    11/03/11   MSW     Support optional path for auto backups		                4.1.1.0
'    12/22/11   MSW     subInitParmConfig - Fix program name for version < 6.3          4.1.1.1
'    01/06/12   MSW     sGetResTxtFrmRobotTxt - Deal with some troubles adding in       4.1.1.2
'                       text from an old robot.
'    01/24/12   MSW     Applicator Updates                                            4.01.01.03
'    01/30/12   MSW     Make sure moColors gets inited by starting with nothing       4.01.01.04
'    02/15/12   MSW     Debug fixes from the recent applicator updates                4.01.01.05
'                       subInitParmConfig - Fix index problems filling in nColDetails
'    03/28/12   MSW     More DBs to XML, scattered access setup                       4.01.03.00
'    04/11/12   RJO     Added dispenser support for sealer applications               4.01.03.01
'    09/21/2012 AM      clsControllers::subInitCollection                             4.01.03.02
'                       Added to support no controllers on XML File
'    11/15/12   HGB     Multi-Zone updates to clsControllers Sub New and              4.01.03.03
'                       subInitCollection.
'    05/09/13   MSW     Add Hot Edit Logger DO[] for sealer                           4.01.05.00
'    05/28/13   MSW     Add vision controller item to controller class                4.01.05.01
'    07/19/13   MSW     Add some error details to ProgramName and VariableName        4.01.05.02
'    08/20/13   MSW     PSC remote Robot support                                      4.01.05.03
'    09/13/13   MSW     Disable cross-thread checking when triggering events          4.01.05.04
'                       PCDK doesn't deal with it well.
'                       Optional - mbRerouteEvents uses a timer to send the connection
'                       status from within the screen thread
'    09/20/13   DE      Modified clsController Property Version to handle Cultures    4.01.05.05
'                       that use "," in floating point numbers rather than ".".      
'    12/03/13   MSW     Cross thread started bombing out with checking                4.01.06.00
'                       disabled, so I'm mbRerouteEvents now.
'                       Add materials count to arm config.
'    01/06/14   MSW     Add an "All" option to LoadRobotBoxFromCollection,            4.01.06.01
'                       Support sysvar read and write
'    12/03/13   MSW     Added code to support materials in sealer applications        4.01.06.00
'    02/13/14   RJO     Add Current Color Change Status to mPWRobotCommon             4.01.06.02
'                       subIndexSactteredAccess
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'    04/03/14   MSW     Change smart-ass comment to descriptive                       4.01.07.01
'    03/24/14   DJM     Added HasManualCycles for sealer applications                 4.01.07.02
'    05/23/14   MSW     Property Application - Didn't work at all                     4.01.07.03
'    07/30/14   RJO     In subLoadCheckListBox .Multicolumn and .ScrollAlwaysVisible  4.01.07.04
'                       needed to be mutually exclusive.
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports FRRobot
Imports FRRobotNeighborhood
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
'********************************************************************************************
'Description: Robot Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsArms

    Inherits CollectionBase

#Region " Declares"

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsArms"
    '***** End Module Constants   **************************************************************

    '********** Event  Variables    *****************************************************************
    Friend Event BumpProgress()
    '********** End Event Variables *****************************************************************

#End Region
#Region " Properties"

    Default Friend Overloads Property Item(ByVal index As Integer) As clsArm
        '********************************************************************************************
        'Description: Get or set a robot by its index
        '
        'Parameters: index
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsArm)
        End Get
        Set(ByVal Value As clsArm)
            List(index) = Value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal Name As String) As clsArm
        '********************************************************************************************
        'Description: Get or set a robot by its name
        '
        'Parameters: Name
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            For Each O As clsArm In List
                If O.Name = Name Then
                    Return O
                    Exit For
                End If
            Next
            'opps
            Return Nothing

        End Get
        Set(ByVal Value As clsArm)

            For Each O As clsArm In List
                If O.Name = Value.Name Then
                    O = Value
                End If
            Next

        End Set
    End Property
    Default Friend Overloads ReadOnly Property Item(ByVal RobotNumber As Integer, ByVal Placeholder As Boolean) As clsArm
        '********************************************************************************************
        'Description: Get or set a robot by its name
        '
        'Parameters: Name
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            For Each O As clsArm In List
                If O.RobotNumber = RobotNumber Then
                    Return O
                    Exit For
                End If
            Next
            'opps
            Return Nothing

        End Get

    End Property
    Friend ReadOnly Property SelectedCount() As Integer
        '********************************************************************************************
        'Description: how many robots are selected?
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim nCount As Integer = 0
            For Each o As clsArm In List
                If o.Selected Then nCount += 1
            Next

            Return nCount
        End Get
    End Property

#End Region
#Region " Routines "

    Friend Function Add(ByVal value As clsArm) As Integer
        '********************************************************************************************
        'Description: Add a robot to collection
        '
        'Parameters: clsArm
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Add(value)
    End Function 'Add
    Friend Function Contains(ByVal value As clsArm) As Boolean
        '********************************************************************************************
        'Description: If value is not of type clsArm, this will return false.
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
    Friend Function IndexOf(ByVal value As clsArm) As Integer
        '********************************************************************************************
        'Description: Get Index of robot in collection
        '
        'Parameters: clsArm
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsArm)
        '********************************************************************************************
        'Description: Add a robot at specific location
        '
        'Parameters: position,clsArm
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsArm)
        '********************************************************************************************
        'Description: Remove a robot 
        '
        'Parameters: clsArm
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove

#End Region
#Region " Events "

    Friend Sub New()
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
    End Sub
    

#End Region

End Class 'clsArms
'********************************************************************************************
'Description: Robot Class
'
'
'Modification history:
'
' Date      By      Reason
' 05/29/09  MSW     Remove mApplicator, always use CCtype  
' 06/01/09  MSW     add CC Presets to robot class

'******************************************************************************************** 
Friend Class clsArm

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsArm"
    '***** End Module Constants   **************************************************************

    '******** Property Variables    *****************************************************************
    '9.5.07 remove unnecessary initializations per fxCop

    Private msName As String = String.Empty         'from xml file
    Private msFanucName As String = String.Empty    'from controller
    Private msIPAddress As String = "0.0.0.0"       'from host file
    Private mnZoneNumber As Integer = 1             'from controller
    Private mnStationNumber As Integer = 1 'CBZ 7/30/13
    Private mArmType As eArmType = eArmType.None
    Private mColorchg As eColorChangeType = eColorChangeType.NONE
    Private mnRobotNumber As Integer = 1
    Private msEstatIP As String = "0.0.0.0"
    Private msEstatName As String = String.Empty
    Private mnIndex As Integer ' = 0 'Removed Not used Geo 
    Private moProgram As FRRobot.FRCProgram ' = Nothing
    Private mbSysVar As Boolean
    Private moVariable As FRRobot.FRCVar ' = Nothing
    Private mnValve As Integer = 1
    Private mnArmNumber As Integer = 1
    Private mEstat As eEstatType = eEstatType.None
    Private moColors As clsSysColors = Nothing 'This nothing seems to be needed to make sure it gets loaded in some conditions
    Private msZoneName As String = String.Empty
    Private msDBPath As String = String.Empty
    Private msXMLPath As String = String.Empty
    Private mbSelected As Boolean ' = False
    Private mAlarmAssocData As clsAlarmAssocData ' = Nothing
    Private mbAlarmAssocDataEn As Boolean
    Private mController As clsController '= Nothing
    Private mbIsOpener As Boolean ' = False
    Private mnLastValve As Integer = 1
    Private msPLCPrefix As String
    Private msPaintToolAlias As String = String.Empty ' 12/10/09  MSW     Support PaintToolAlias property for 7.50 alarm screen changes
    Private mnDispensers As Integer '04/11/12 RJO
	Private mnMaterials As Integer '12/03/13 MSW
    Private mDispenserType As eDispenserType '04/11/12 RJO
    Private moTag1 As Object = Nothing
    Private moTag2 As Object = Nothing
    Private msScatteredAccessGroups() As String = Nothing
    Private mbHasManualCycles As Boolean
    Private mnPigSystems As Integer = 0 'NRU 161215 Piggable
    ''******** End Property Variables    *************************************************************

    ''******** Event    Variables    *****************************************************************
    ' main form can use to show progress
    Friend Event BumpProgress()
    '********** End Event Variables *****************************************************************
    Private mApplicator As clsApplicator = Nothing
#End Region
#Region " Properties "
    Friend Property ScatteredAccessGroups() As String()
        Get
            Return msScatteredAccessGroups
        End Get
        Set(ByVal value As String())
            msScatteredAccessGroups = value
        End Set
    End Property
    Friend Property Tag() As Object
        '********************************************************************************************
        'Description: Store an object to pass around to everybody
        '
        'Parameters: none
        'Returns:    tag
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moTag1
        End Get
        Set(ByVal value As Object)
            moTag1 = value
        End Set
    End Property
    Friend Property Tag2() As Object
        '********************************************************************************************
        'Description: Store an object to pass around to everybody
        '
        'Parameters: none
        'Returns:    tag2
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moTag2
        End Get
        Set(ByVal value As Object)
            moTag2 = value
        End Set
    End Property
    Friend Property Applicator() As clsApplicator
        '********************************************************************************************
        'Description: Store an applicator object to pass around to everybody
        '
        'Parameters: none
        'Returns:    alarm associated data
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mApplicator
        End Get
        Set(ByVal value As clsApplicator)
            mApplicator = value
        End Set
    End Property
    Friend ReadOnly Property AlarmAssocData() As clsAlarmAssocData
        '********************************************************************************************
        'Description: Returns the alarm associated data class for this robot arm
        '
        'Parameters: none
        'Returns:    alarm associated data
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mAlarmAssocData
        End Get
    End Property
    Friend Property AlarmAssocDataEnable() As Boolean
        '********************************************************************************************
        'Description: Set attempts to initialize Alarm Associated Data Scattered Access for this arm.
        '             Get returns True is Scattered Access is set up for this arm.
        '
        'Parameters: none
        'Returns:    alarm associated data
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbAlarmAssocDataEn
        End Get
        Set(ByVal Enable As Boolean)
            If Enable = mbAlarmAssocDataEn Then Exit Property

            If Enable Then
                mbAlarmAssocDataEn = InitAlarmAssocDataSA()
            Else
                mAlarmAssocData = Nothing
                mbAlarmAssocDataEn = False
            End If
        End Set
    End Property

    Friend ReadOnly Property ArmType() As eArmType
        '********************************************************************************************
        'Description: What kind of Arm
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mArmType
        End Get

    End Property
    Friend Property ArmNumber() As Integer
        '********************************************************************************************
        'Description:  Robot number in controller - motion group number
        '
        'Parameters: none
        'Returns:    integer
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnArmNumber
        End Get
        Set(ByVal value As Integer)
            mnArmNumber = value
        End Set
    End Property
    Friend ReadOnly Property ColorChangeType() As eColorChangeType
        '********************************************************************************************
        'Description: What kind of color change is it
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mColorchg
        End Get

    End Property
    Friend Property Controller() As clsController
        '********************************************************************************************
        'Description: reference to controller
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mController
        End Get
        Set(ByVal value As clsController)
            mController = value
        End Set
    End Property
    Friend ReadOnly Property DatabasePath() As String
        '********************************************************************************************
        'Description: path to the database where the info was obtained
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
                'local machine
                GetDefaultFilePath(msDBPath, eDir.Database, String.Empty, String.Empty)
            End If
            Return msDBPath
        End Get
    End Property
    Friend ReadOnly Property Dispensers() As Integer
        '********************************************************************************************
        'Description: The number of Sealant Dispensers (0, 1, or 2) associated with this arm.
        '
        'Parameters:  none
        'Returns:     dispenser count
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/11/12  RJO     Added to support .NET Sealer screens
        '********************************************************************************************
        Get
            Return mnDispensers
        End Get
    End Property
    Friend ReadOnly Property DispenserType() As eDispenserType
        '********************************************************************************************
        'Description: The Sealant Dispenser type (None=0, ISD=1, IPS=2, Bedliner=3, Hem Flange=4, 
        '             Generic Dispenser w/Temp Control=5) associated with this arm.
        '
        'Parameters:  none
        'Returns:     dispenser type
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/11/12  RJO     Added to support .NET Sealer screens
        '********************************************************************************************
        Get
            Return mDispenserType
        End Get
    End Property
    Friend ReadOnly Property Materials() As Integer
        '********************************************************************************************
        'Description: The number of Sealant Materials (0, 1, or 2) associated with this arm.
        '
        'Parameters:  none
        'Returns:     dispenser count
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/03/13  MSW     Added to support .NET Sealer screens
        '********************************************************************************************
        Get
            Return mnMaterials
        End Get
    End Property
    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description: path to the xml directory where the info was obtained
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If msXMLPath = String.Empty Then
                'local machine
                GetDefaultFilePath(msXMLPath, eDir.XML, String.Empty, String.Empty)
            End If
            Return msXMLPath
        End Get
    End Property
    Friend ReadOnly Property EstatType() As eEstatType
        '********************************************************************************************
        'Description: type of estat unit
        '
        'Parameters:  
        'Returns:     enum value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mEstat
        End Get

    End Property
    Friend ReadOnly Property EstatName() As String
        '********************************************************************************************
        'Description: Name of estat unit
        '
        'Parameters:  
        'Returns:     name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msEstatName
        End Get
    End Property
    Friend ReadOnly Property EstatIP() As String
        '********************************************************************************************
        'Description: ip of estat unit
        '
        'Parameters:  
        'Returns:     name
        '
        'Modification history:
        '
        ' Date      By      Reason
        Get
            Return msEstatIP
        End Get
    End Property
    Friend ReadOnly Property PaintToolAlias() As String
        '********************************************************************************************
        'Description:  PaintTool alias "P1", "H1", etc 
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/10/09  MSW     Support PaintToolAlias property for 7.50 alarm screen changes
        '********************************************************************************************
        Get
            Return msPaintToolAlias
        End Get
    End Property
    Friend ReadOnly Property FanucName() As String
        '********************************************************************************************
        'Description:  FanucName for old stuff
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFanucName
        End Get
    End Property

    Friend ReadOnly Property Index() As Integer
        '********************************************************************************************
        'Description:  Position in robot collection
        '
        'Parameters: none
        'Returns:    index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' 3/19/09

        Get
            Index = mnIndex
        End Get
    End Property


    Friend ReadOnly Property IPAddress() As String
        '********************************************************************************************
        'Description: Internet address
        '
        'Parameters: none
        'Returns:    address
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            IPAddress = msIPAddress
        End Get
    End Property
    Friend ReadOnly Property IsOnLine() As Boolean
        '********************************************************************************************
        'Description:  Are we connected to frrobot?
        '
        'Parameters: none
        'Returns:    connect state
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mController.Robot Is Nothing Then
                Return False
            Else
                Try
                    Return mController.Robot.IsConnected
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Return False
                End Try
            End If
        End Get
    End Property
    Friend ReadOnly Property IsOpener() As Boolean
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
            Return mbIsOpener
        End Get
    End Property
    Friend ReadOnly Property HasManualCycles() As Boolean
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/24/14  DJM     Created property to support sealer applications
        '********************************************************************************************
        Get
            Return mbHasManualCycles
        End Get
    End Property
    Friend ReadOnly Property LastColorValve() As Integer
        Get
            '********************************************************************************************
            'Description:  highest color valve number
            '
            'Parameters: none
            'Returns:    number
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Return mnLastValve
        End Get
    End Property
    Friend ReadOnly Property Name() As String
        '********************************************************************************************
        'Description:  Name
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Name = msName
        End Get
    End Property
    Friend ReadOnly Property PLCTagPrefix() As String
        '********************************************************************************************
        'Description:  Name
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msPLCPrefix
        End Get
    End Property
    Friend Property ProgramName() As String
        '********************************************************************************************
        'Description:  Program name we want to read or write a variable in
        '
        'Parameters: none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mbSysVar Then
                Return "*SYSTEM*"
            Else
                If moProgram Is Nothing Then
                    Return String.Empty
                Else
                    Return moProgram.Name
                End If
            End If
        End Get
        Set(ByVal Value As String)
            Try
                If Value = "*SYSTEM*" Or Value = "*SYSVAR*" Then
                    mbSysVar = True
                    moProgram = Nothing
                Else
                    mbSysVar = False
                    If IsOnLine Then
                        Dim oList As FRRobot.FRCPrograms
                        oList = mController.Robot.Programs
                        moProgram = CType(oList.Item(Value), FRRobot.FRCProgram)
                    Else
                        moProgram = Nothing
                    End If
                End If
            Catch ex As Exception
                Dim sTmp As String = "Module: clsArm, Routine: ProgramName, Error: " & ex.Message
                Trace.WriteLine(sTmp)
                Trace.WriteLine("Module: clsArm, Routine: ProgramName, StackTrace: " & ex.StackTrace)
                mDebug.WriteEventToLog(Application.ProductName, Value & " - " & sTmp)
                Select Case Err.Number
                    Case FRRobot.FRERobotErrors.frRobotError
                        'one would hope this only comes up in development
                        MessageBox.Show(ex.Message & vbCrLf & "'" & Value & "'", msName, _
                                                MessageBoxButtons.OK, MessageBoxIcon.Error, _
                                                 MessageBoxDefaultButton.Button1, _
                                                  MessageBoxOptions.DefaultDesktopOnly)
                End Select
            End Try

        End Set
    End Property
    Friend ReadOnly Property ProgramVars() As FRRobot.FRCVars
        '********************************************************************************************
        'Description: Access to program variables after setting program
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
                If IsOnLine Then
                    If mbSysVar Then
                        Return mController.Robot.SysVariables
                    Else
                        If moProgram Is Nothing Then Return Nothing
                        ProgramVars = moProgram.Variables
                    End If
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Dim sTmp As String = "Module: clsArm, Routine: ProgramVars, Error: " & ex.Message
                Trace.WriteLine(sTmp)
                Trace.WriteLine("Module: clsArm, Routine: ProgramVars, StackTrace: " & ex.StackTrace)
                mDebug.WriteEventToLog(Application.ProductName, sTmp)

                Return Nothing
            End Try
        End Get
    End Property
    Friend ReadOnly Property RobotNumber() As Integer
        '********************************************************************************************
        'Description:  Robot number in booth e.g. first one on left = 1
        '
        'Parameters: none
        'Returns:    Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            RobotNumber = mnRobotNumber
        End Get
    End Property
    Friend ReadOnly Property PigSystems() As Integer 'NRU 161215 Piggable
        '********************************************************************************************
        'Description:  Robot number in booth e.g. first one on left = 1
        '
        'Parameters: none
        'Returns:    Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            PigSystems = mnPigSystems
        End Get
    End Property
    Friend Property Selected() As Boolean
        '********************************************************************************************
        'Description: a way to mark as selected when looking thru collection
        '
        'Parameters:  
        'Returns:     
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
    Friend ReadOnly Property SystemColors() As clsSysColors
        '********************************************************************************************
        'Description: this is the system color collection - you fill em
        '
        'Parameters: none
        'Returns:    collection
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moColors Is Nothing Then
                moColors = New clsSysColors(Me)
            End If
            Return moColors
        End Get
    End Property
    Friend Property VariableName() As String
        '********************************************************************************************
        'Description:  Variable name we want to read or write - this will fail if program not set
        '               first
        '
        'Parameters: none
        'Returns:    name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If (moProgram Is Nothing) Or (moVariable Is Nothing) Then
                Return String.Empty
            Else
                Return moVariable.VarName
            End If
        End Get
        Set(ByVal Value As String)
            Dim sProg As String = String.Empty
            Try
                If mbSysVar Then
                    sProg = "*SYSTEM*"
                    moVariable = CType(mController.Robot.SysVariables(Value), FRRobot.FRCVar)
                Else
                    If (moProgram Is Nothing) Then
                        moVariable = Nothing
                        sProg = String.Empty
                    Else
                        moVariable = CType(moProgram.Variables(Value), FRRobot.FRCVar)
                        sProg = "[" & moProgram.Name & "]"
                    End If
                End If
            Catch ex As Exception
                Dim sTmp As String = "Module: clsArm, Routine: VariableName, Error: " & ex.Message
                Trace.WriteLine(sTmp)
                Trace.WriteLine("Module: clsArm, Routine: VariableName, StackTrace: " & ex.StackTrace)
                mDebug.WriteEventToLog(Application.ProductName, sProg & Value & " - " & sTmp)

                Select Case Err.Number
                    Case FRRobot.FRERobotErrors.frInvVarName
                        'one would hope this only comes up in development
                        MessageBox.Show(ex.Message & vbCrLf & "'" & Value & "'", msName, _
                                                MessageBoxButtons.OK, MessageBoxIcon.Error, _
                                                 MessageBoxDefaultButton.Button1, _
                                                 MessageBoxOptions.DefaultDesktopOnly)
                End Select
            End Try

        End Set
    End Property
    Friend Property Valve() As Integer
        '********************************************************************************************
        'Description: Color valve selection
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Valve = mnValve
        End Get
        Set(ByVal Value As Integer)
            mnValve = Value
        End Set
    End Property
    Friend Property VarValue() As String
        '********************************************************************************************
        'Description: After the program and variable name are set use this to read/write the value
        '
        'Parameters: Value of a single variable
        'Returns:    Value of a single variable
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If moVariable Is Nothing Then
                Return String.Empty
            Else
                With moVariable
                    If .NoRefresh Then .Refresh()
                    If .IsInitialized Then
                        Return CStr(.Value)
                    Else
                        Return String.Empty
                    End If
                End With
            End If
        End Get
        Set(ByVal Value As String)
            If moVariable Is Nothing Then

            Else
                Try
                    moVariable.Value = Value
                Catch ex As Exception
                    Trace.WriteLine("Module: clsArm, Routine: VarValue, Error: " & ex.Message)
                    Trace.WriteLine("Module: clsArm, Routine: VarValue, StackTrace: " & ex.StackTrace)

                    Dim void As String = String.Empty
                    Dim sTmp As String = "Module: clsArm, Routine: VarValue, Error: "
                    mDebug.ShowErrorMessagebox(sTmp, ex, msName, void)

                End Try
            End If
        End Set
    End Property
    Friend ReadOnly Property ZoneNumber() As Integer
        '********************************************************************************************
        'Description: zone number for the old stuff
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnZoneNumber
        End Get
    End Property
    Friend ReadOnly Property StationNumber() As Integer
        '********************************************************************************************
        'Description: Station Number
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        'CBZ 7/30/13
        '********************************************************************************************
        Get
            Return mnStationNumber
        End Get
    End Property
#End Region
#Region " Routines "

    Private Function InitAlarmAssocDataSA() As Boolean
        '********************************************************************************************
        'Description: Set up Alarm Associated Data Scattered Access for this arm.
        '
        'Parameters: none
        'Returns:    True = Success, False = Fail 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sXMLPath As String = String.Empty
        Dim bSuccess As Boolean

        bSuccess = mPWCommon.GetDefaultFilePath(sXMLPath, eDir.XML, String.Empty, "RobotAlarmConfig.xml")

        If bSuccess Then
            mAlarmAssocData = New clsAlarmAssocData(mController.Robot, sXMLPath, ArmNumber, IsOpener)
            Return mAlarmAssocData.InitSuccess
        Else
            mAlarmAssocData = Nothing
            Return False
        End If

    End Function
    Private Sub subInitialize(ByVal drRobot As DataRow, ByVal Controller As clsController)
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '       ****** add virtural robot someday *******
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/10/09  MSW     Support PaintToolAlias property for 7.50 alarm screen changes
        ' 03/24/14  DJM     Added HasManaulCycles to support sealer applications
        '********************************************************************************************

        mController = Controller

        With drRobot
            msName = .Item(gsARM_COL_DNAME).ToString
            mnRobotNumber = CType(.Item(gsARM_COL_ROB_NUMBER), Integer)
            msFanucName = Controller.FanucName
            msIPAddress = Controller.IPAddress
            mnZoneNumber = Controller.Zone.ZoneNumber
            mnStationNumber = CType(.Item(gsARM_COL_STATION_NUMBER), Integer) 'CBZ 7/30/13
            mColorchg = CType(.Item(gsARM_COL_CC_TYPE), eColorChangeType)
            mnDispensers = CType(.Item(gsARM_COL_DISPENSERS), Integer) '04/11/12 RJO
            mDispenserType = CType(.Item(gsARM_COL_DISPENSER_TYPE), eDispenserType) '04/11/12 RJO
            mnMaterials = CType(.Item(gsARM_COL_MATERIALS), Integer) '12/03/13 MSW
            mbIsOpener = CType(.Item(gsARM_COL_ISOPENER), Boolean)
            mnArmNumber = CType(.Item(gsARM_COL_ARM_NUMBER), Integer)
            mEstat = CType(.Item(gsARM_COL_ES_TYPE), eEstatType)
            If mEstat = eEstatType.FB200 Then
                msEstatName = .Item(gsARM_COL_ES_HOST).ToString
                msEstatIP = mPWCommon.IPFromHostName(msEstatName)
            End If
            msPLCPrefix = .Item(gsARM_COL_PLC_TAG_PRFX).ToString
            mArmType = CType(.Item(gsARM_COL_ARM_TYPE), eArmType)
            If .Item(gsARM_PT_ALIAS) IsNot Nothing Then
                msPaintToolAlias = .Item(gsARM_PT_ALIAS).ToString
            Else
                msPaintToolAlias = String.Empty
            End If
            If .Item(gsARM_COL_HASMANUALCYCLES) IsNot Nothing Then
                mbHasManualCycles = CType(.Item(gsARM_COL_HASMANUALCYCLES), Boolean)
            Else
                mbHasManualCycles = False
            End If
            mnPigSystems = CType(.Item(gsARM_COL_PIG_SYSTEMS), Integer) 'NRU 161215 Piggable
            
        End With


        mnLastValve = Controller.Zone.MaxValves

        msDBPath = DatabasePath
        msXMLPath = DatabasePath & "XML\"

    End Sub
    Private Sub subInitialize(ByRef oArm As tArm, ByRef Controller As clsController)
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '       ****** add virtural robot someday *******
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/10/09  MSW     Support PaintToolAlias property for 7.50 alarm screen changes
        ' 03/24/14  DJM     Added HasManaulCycles for sealer applications
        '********************************************************************************************

        mController = Controller

        With oArm
            msName = .ArmDisplayName
            mnRobotNumber = .RobotNumber
            msFanucName = Controller.FanucName
            msIPAddress = Controller.IPAddress
            mnZoneNumber = Controller.Zone.ZoneNumber
            mnStationNumber = .StationNumber 'CBZ 7/30/13
            mColorchg = CType(.ColorChangeType, eColorChangeType)
            mnDispensers = .Dispensers '04/11/12 RJO
            mDispenserType = CType(.DispenserType, eDispenserType) '04/11/12 RJO
            mnMaterials = .Materials '12/03/13 MSW
            mbIsOpener = .IsOpener
            mnArmNumber = .ArmNumber
            mEstat = CType(.EstatType, eEstatType)
            msScatteredAccessGroups = Split(.ScatteredAccessGroups, ";")
            If mEstat = eEstatType.FB200 Then
                msEstatName = .EstatHostName
                msEstatIP = mPWCommon.IPFromHostName(msEstatName)
            End If
            msPLCPrefix = .PLCTagPrefix
            mArmType = CType(.ArmType, eArmType)
            If .PaintToolAlias IsNot Nothing Then
                msPaintToolAlias = .PaintToolAlias
            Else
                msPaintToolAlias = String.Empty
            End If
            mbHasManualCycles = .HasManualCycles
            mnPigSystems = .PigSystems 'NRU 161215 Piggable
        End With


        mnLastValve = Controller.Zone.MaxValves

        msDBPath = DatabasePath
        msXMLPath = DatabasePath & "XML\"

    End Sub
    Protected Sub subBumpProgress()
        '********************************************************************************************
        'Description: Pass on the Progress
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        RaiseEvent BumpProgress()
    End Sub

#End Region
#Region " Events "

    Friend Sub New(ByVal drController As DataRow, ByVal Controller As clsController)
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        subInitialize(drController, Controller)

    End Sub
    Friend Sub New(ByRef oArm As tArm, ByRef Controller As clsController)
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        subInitialize(oArm, Controller)

    End Sub

#End Region

    Private Sub clsArm_BumpProgress() Handles Me.BumpProgress

    End Sub
End Class 'clsArm
'********************************************************************************************
'Description: Controller Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsControllers

    Inherits CollectionBase

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsControllers"
    '***** End Module Constants   **************************************************************

    '********** Event  Variables    *****************************************************************
    ' robot neighborhood
    '9.5.07 remove initializations per fxCop
    Friend goRN As FRCRobotNeighborhood ' = Nothing
    Friend Event AlarmNotification(ByVal Alarm As Object, ByVal Name As String)
    Friend Event ConnectionStatusChange(ByVal Controller As clsController)
    Friend Event BumpProgress()
    Friend Event PacketEventReceiveNotification(ByRef PacketEvent As FRRobot.FRCPacket, ByRef oRobot As clsController) '04/05/11 HGB Packet Event support 
    Private mbRemote As Boolean = False    '    08/15/13   MSW     PSC remote Robot support
    Private msRemotePath As String = String.Empty
    '********** End Event Variables *****************************************************************

#End Region
#Region " Properties "

    Default Friend Overloads Property Item(ByVal index As Integer) As clsController
        '********************************************************************************************
        'Description: Get or set a robot by its index
        '
        'Parameters: index
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsController)
        End Get
        Set(ByVal Value As clsController)
            List(index) = Value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal Name As String) As clsController
        '********************************************************************************************
        'Description: Get or set a robot by its name
        '
        'Parameters: Name
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            For Each O As clsController In List
                If O.Name = Name Then
                    Return O
                    Exit For
                End If
            Next
            'opps
            Return Nothing
        End Get
        Set(ByVal Value As clsController)
            For Each O As clsController In List
                If O.Name = Value.Name Then
                    O = Value
                End If
            Next
        End Set
    End Property
    Friend ReadOnly Property SelectedCount() As Integer
        '********************************************************************************************
        'Description: how many robots are selected?
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim nCount As Integer = 0
            For Each o As clsController In List
                If o.Selected Then nCount += 1
            Next

            Return nCount
        End Get
    End Property

#End Region
#Region " Routines "

    Friend Function Add(ByVal value As clsController) As Integer
        '********************************************************************************************
        'Description: Add a robot to collection
        '
        'Parameters: clscontroller
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subAddHandlers(value)
        System.Windows.Forms.Application.DoEvents()
        RaiseEvent BumpProgress()
        Return List.Add(value)

    End Function 'Add
    Friend Function Contains(ByVal value As clsController) As Boolean
        '********************************************************************************************
        'Description: If value is not of type clscontroller, this will return false.
        '
        'Parameters: clscontroller
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Contains(value)
    End Function 'Contains
    Friend Function IndexOf(ByVal value As clsController) As Integer
        '********************************************************************************************
        'Description: Get Index of robot in collection
        '
        'Parameters: clscontroller
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsController)
        '********************************************************************************************
        'Description: Add a robot at specific location
        '
        'Parameters: position,clscontroller
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subAddHandlers(value)
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsController)
        '********************************************************************************************
        'Description: Remove a robot 
        '
        'Parameters: clscontroller
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subRemoveHandlers(value)
        List.Remove(value)

    End Sub
    Friend Sub RefreshConnectionStatus()
        '********************************************************************************************
        'Description: reraise the events
        '
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each o As clsController In List
            subConnectionStatusChange(o)
        Next
    End Sub
    Private Sub subAddHandlers(ByRef oRobot As clsController)
        '********************************************************************************************
        'Description: add event handlers as robots are added to collection
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        AddHandler oRobot.BumpProgress, AddressOf subBumpProgress
        AddHandler oRobot.ConnStatusChange, AddressOf subConnectionStatusChange
        AddHandler oRobot.AlarmNotification, AddressOf subAlarmNotification
        AddHandler oRobot.PacketEventReceiveNotification, AddressOf subPacketEventReceiveNotification

    End Sub
    Private Sub subInitCollection(ByRef ZoneCollection As clsZones, _
                                  ByVal LoadControllersFromAllZones As Boolean)
        '*****************************************************************************************
        'Description: load the robot collection
        '               This assumes that the arms as sorted by the sortorder column will be on 
        '               consecutive controllers. i.e. arm 1 on controller 1, arm 2 and 3 on 
        '               controller 2, arm 4 on controller 3 etc.
        '
        'Parameters:  Zone we are interested in
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Add LoadControllersFromAllZones, this is needed for multizone SA GUIs
        ' 08/15/13  MSW     PSC remote Robot support
        '*****************************************************************************************


        'get rid of old data if any
        If List.Count > 0 Then List.Clear()

        Try
            If Not LoadControllersFromAllZones Then
                If ZoneCollection.ActiveZone.IsRemoteZone AndAlso (ZoneCollection.ActiveZone.RobotsOnSameNet = False) Then
                    msRemotePath = "\\" & ZoneCollection.ActiveZone.ServerName
                    mbRemote = (msRemotePath <> "")
                Else
                    msRemotePath = String.Empty
                    mbRemote = False
                End If
                Dim tArmArray() As tArm = GetArmArray(ZoneCollection.ActiveZone)
                'Dim tArmArray() As tArm = GetArmArray(ZoneCollection)  ' NVSP 12/14/2016  -> Arms in all zones needs to be displayed
                '09/21/2012 AM Added to support no controllers on XML File
                If tArmArray Is Nothing Then Exit Sub

                Dim nContNum As Integer = -1
                For Each oArm As tArm In tArmArray
                    'DR = DT.Rows(i)
                    If oArm.Controller.ControllerNumber <> nContNum Then
                        ' new controller - it adds first arm
                        nContNum = oArm.Controller.ControllerNumber
                        Dim o As New clsController(oArm, ZoneCollection.ActiveZone, msRemotePath)
                        Dim z As clsZone = ZoneCollection.Item(ZoneCollection.ActiveZone.ZoneNumber)
                        o.ColorsByStyle = z.PresetsByStyle
                        Add(o)
                        Call o.subConnect()
                    Else
                        'additional arm on existing controller
                        Dim o As clsController = Me.Item(oArm.Controller.DisplayName)
                        Dim a As New clsArm(oArm, o)
                        'have to add to collection
                        o.Arms.Add(a)
                    End If
                Next
            Else
                Dim oZone As clsZone
                For Each oZone In ZoneCollection
                    Dim tArmArray() As tArm = GetArmArray(oZone)
                    If oZone.IsRemoteZone AndAlso (oZone.RobotsOnSameNet = False) Then
                        msRemotePath = "\\" & ZoneCollection.ActiveZone.ServerName
                        mbRemote = (msRemotePath <> "")
                    Else
                        msRemotePath = String.Empty
                        mbRemote = False
                    End If
                    Dim nContNum As Integer = -1
                    For Each oArm As tArm In tArmArray
                        'DR = DT.Rows(i)
                        If oArm.Controller.ControllerNumber <> nContNum Then
                            ' new controller - it adds first arm
                            nContNum = oArm.Controller.ControllerNumber
                            Dim o As New clsController(oArm, oZone, msRemotePath)
                            'Dim z As clsZone = ZoneCollection.Item(ZoneCollection.ActiveZone.ZoneNumber)
                            o.ColorsByStyle = oZone.PresetsByStyle
                            Add(o)
                            Call o.subConnect()
                        Else
                            'additional arm on existing controller
                            Dim o As clsController = Me.Item(oArm.Controller.DisplayName)
                            Dim a As New clsArm(oArm, o)
                            'have to add to collection
                            o.Arms.Add(a)
                        End If
                    Next
                Next

            End If
        Catch ex As Exception
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, Error: " & ex.Message)
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub subInitCollectionforProdReports(ByRef ZoneCollection As clsZones, _
                                 ByVal LoadControllersFromAllZones As Boolean)
        '*****************************************************************************************
        'Description: load the robot collection
        '               This assumes that the arms as sorted by the sortorder column will be on 
        '               consecutive controllers. i.e. arm 1 on controller 1, arm 2 and 3 on 
        '               controller 2, arm 4 on controller 3 etc.
        '
        'Parameters:  Zone we are interested in
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/16/16  NVSP     Add LoadControllersFromAllZones, this is needed for multizone SA GUIs
        '*****************************************************************************************


        'get rid of old data if any
        If List.Count > 0 Then List.Clear()

        Try
            If Not LoadControllersFromAllZones Then
                If ZoneCollection.ActiveZone.IsRemoteZone AndAlso (ZoneCollection.ActiveZone.RobotsOnSameNet = False) Then
                    msRemotePath = "\\" & ZoneCollection.ActiveZone.ServerName
                    mbRemote = (msRemotePath <> "")
                Else
                    msRemotePath = String.Empty
                    mbRemote = False
                End If

                Dim oZone As clsZone
                For Each oZone In ZoneCollection
                    Dim tArmArray() As tArm = GetArmArray(oZone)  ' NVSP 12/14/2016  -> Arms in all zones needs to be displayed
                    Dim nContNum As Integer = -1
                    If tArmArray Is Nothing Then Exit Sub
                    For Each oArm As tArm In tArmArray
                        'DR = DT.Rows(i)
                        If oArm.Controller.ControllerNumber <> nContNum Then
                            ' new controller - it adds first arm
                            nContNum = oArm.Controller.ControllerNumber
                            Dim o As New clsController(oArm, ZoneCollection.ActiveZone, msRemotePath)
                            Dim z As clsZone = ZoneCollection.Item(ZoneCollection.ActiveZone.ZoneNumber)
                            o.ColorsByStyle = z.PresetsByStyle
                            Add(o)
                            Call o.subConnect()
                        Else
                            'additional arm on existing controller
                            Dim o As clsController = Me.Item(oArm.Controller.DisplayName)
                            Dim a As New clsArm(oArm, o)
                            'have to add to collection
                            o.Arms.Add(a)
                        End If
                    Next
                Next
            Else
                Dim oZone As clsZone
                For Each oZone In ZoneCollection
                    Dim tArmArray() As tArm = GetArmArray(oZone)
                    If oZone.IsRemoteZone AndAlso (oZone.RobotsOnSameNet = False) Then
                        msRemotePath = "\\" & ZoneCollection.ActiveZone.ServerName
                        mbRemote = (msRemotePath <> "")
                    Else
                        msRemotePath = String.Empty
                        mbRemote = False
                    End If
                    Dim nContNum As Integer = -1
                    For Each oArm As tArm In tArmArray
                        'DR = DT.Rows(i)
                        If oArm.Controller.ControllerNumber <> nContNum Then
                            ' new controller - it adds first arm
                            nContNum = oArm.Controller.ControllerNumber
                            Dim o As New clsController(oArm, oZone, msRemotePath)
                            'Dim z As clsZone = ZoneCollection.Item(ZoneCollection.ActiveZone.ZoneNumber)
                            o.ColorsByStyle = oZone.PresetsByStyle
                            Add(o)
                            Call o.subConnect()
                        Else
                            'additional arm on existing controller
                            Dim o As clsController = Me.Item(oArm.Controller.DisplayName)
                            Dim a As New clsArm(oArm, o)
                            'have to add to collection
                            o.Arms.Add(a)
                        End If
                    Next
                Next

            End If
        Catch ex As Exception
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, Error: " & ex.Message)
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub subSetUpHood()
        '*****************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/21/08  gks     change to secret. shhhhh!
        ' 08/15/13  MSW     PSC remote Robot support
        '*****************************************************************************************
        'what's up in the neighborhood
        Try
            If mbRemote Then
                Dim oRemoteObject As Object = CreateObject("FRRobotNeighborhood.FRCRobotNeighborhood", msRemotePath)
                goRN = DirectCast(oRemoteObject, FRRobotNeighborhood.FRCRobotNeighborhood)
            Else
                goRN = New FRRobotNeighborhood.FRCRobotNeighborhood
            End If


            Dim oShhh As IRNInternal = CType(goRN, IRNInternal)
            Dim oRob As FRCRNRobots = oShhh.Robots0 ' goRN.Robots ' '64 bit hack

            goRN.PingDelay = 30                 '6/4/07 set everytime gks

            If oRob.Count = 0 Then
                'set up the neighborhood
                With goRN
                    .PingDelay = 30                 'Power Up Pause
                    .RSResponseTolerance = 2        'Power down pause. Occurs after Hearbeat Timeout
                    .DefaultKeepAliveDuration = 1   'Time in minutes
                    .DefaultKeepAliveEnable = True  '?
                    .DefaultHeartbeatPeriod = 5     'Time in seconds
                    .DefaultHeartbeatEnable = True  'Wont detect Power off unless enabled
                    .DefaultAutoReconnectEnable = True
                    .DefaultAutoReconnectNumRetries = 0 ' 0 = infinite
                    .DefaultAutoReconnectPeriod = 30 ' Time in seconds
                    .RSResponseTolerance = 40 ' time in seconds
                    ' for virtual
                    '    .StartModeAutoDelay
                    '    .VirtualRestartTolerance
                End With
            End If

        Catch ex As Exception
            'trouble in the hood
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, Error: " & ex.Message)
            Trace.WriteLine("Module: clsControllers, Routine: subInitCollection, StackTrace: " & ex.StackTrace)
        End Try


    End Sub
    Private Sub subRemoveHandlers(ByRef oRobot As clsController)
        '********************************************************************************************
        'Description: remove event handlers as robots are removed from collection
        '
        'Parameters: None
        'Returns:    number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            RemoveHandler oRobot.BumpProgress, AddressOf subBumpProgress
            RemoveHandler oRobot.ConnStatusChange, AddressOf subConnectionStatusChange
            RemoveHandler oRobot.AlarmNotification, AddressOf subAlarmNotification
            RemoveHandler oRobot.PacketEventReceiveNotification, AddressOf subPacketEventReceiveNotification '4/05/11 HGB Packet event support

        Catch ex As Exception
            'who said we wern't playin' slop?
        End Try

    End Sub

#End Region
#Region " Events "

    Private Sub subPacketEventReceiveNotification(ByRef PacketEvent As FRRobot.FRCPacket, ByRef oRobot As clsController)
        '********************************************************************************************
        'Description: Event packet received from a robot. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 4/05/11   HGB     Packet event support
        ' 09/13/13  MSW     Disable cross-thread checking when triggering events
        '********************************************************************************************
        'Control.CheckForIllegalCrossThreadCalls = False
        RaiseEvent PacketEventReceiveNotification(PacketEvent, oRobot)
        'Control.CheckForIllegalCrossThreadCalls = True
    End Sub
    Private Sub subAlarmNotification(ByVal Alarm As Object, ByVal Name As String)
        '********************************************************************************************
        'Description: New alarm received from a robot. Send it up the ladder
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/13  MSW     Disable cross-thread checking when triggering events
        '********************************************************************************************
        'Control.CheckForIllegalCrossThreadCalls = False
        RaiseEvent AlarmNotification(Alarm, Name)
        'Control.CheckForIllegalCrossThreadCalls = True
    End Sub
    Private Sub subBumpProgress()
        '********************************************************************************************
        'Description: Pass on the Progress
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '09/13/13   MSW     Disable cross-thread checking when triggering events
        '********************************************************************************************
        'Control.CheckForIllegalCrossThreadCalls = False
        RaiseEvent BumpProgress()
        'Control.CheckForIllegalCrossThreadCalls = True
    End Sub
    Private Sub subConnectionStatusChange(ByVal Controller As clsController)
        '********************************************************************************************
        'Description: Connection status of a robot changed
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/13  MSW     Disable cross-thread checking when triggering events
        '********************************************************************************************
        'Control.CheckForIllegalCrossThreadCalls = False
        RaiseEvent ConnectionStatusChange(Controller)
        'Trace.WriteLine("clsControllers connection status change event - " & Controller.Name _
        '           & " " & Controller.RCMConnectStatus.ToString & " " & System.DateTime.Now)
        'Control.CheckForIllegalCrossThreadCalls = True
    End Sub
    Friend Sub New(ByRef ZoneCollection As clsZones, ByVal LoadControllersFromAllZones As Boolean)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '08/17/07   gks     remove LoadControllersFromAllZones - it doesn't make sense
        '11/15/12   HGB     Add LoadControllersFromAllZones, this is needed for multizone SA GUIs
        '*****************************************************************************************
        MyBase.New()
        subSetUpHood()
        subInitCollection(ZoneCollection, LoadControllersFromAllZones)
    End Sub
    Friend Sub New(ByRef ZoneCollection As clsZones, ByVal LoadControllersFromAllZones As Boolean, ByVal ProdReports As Boolean)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/16/2016 NVSP    prod reports
        '*****************************************************************************************
        MyBase.New()
        subSetUpHood()
        subInitCollectionforProdReports(ZoneCollection, LoadControllersFromAllZones)
    End Sub
    Protected Overrides Sub Finalize()
        '*****************************************************************************************
        'Description: class destructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Try
            For Each o As clsController In List
                subRemoveHandlers(o)
            Next

            System.Windows.Forms.Application.DoEvents()

        Catch ex As Exception
            Trace.WriteLine("Module: clsControllers, Routine: Finalize, Error: " & ex.Message)
            Trace.WriteLine("Module: clsControllers, Routine: Finalize, StackTrace: " & ex.StackTrace)
        End Try

        MyBase.Finalize()
    End Sub

#End Region

End Class 'clsControllers
'********************************************************************************************
'Description: Controller object
'
'
'Modification history:
'
' Date      By      Reason
' 10/17/11  MSW     Add DMON Cfg
' 05/08/13  MSW     Add DO for hot edit logger
'******************************************************************************************** 
Friend Class clsController

#Region " Declares "

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsController"
    '***** End Module Constants   **************************************************************

    '************ Property Variables    *************************************************************
    Private msName As String = String.Empty
    Private mConnStat As ConnStat = ConnStat.frRNUnknown
    '******** End Property Variables    *************************************************************
    '******** Property Variables    *****************************************************************
    '9.5.07 remove unnecessary initializations per fxCop

    Private msFanucName As String = String.Empty
    Private msIPAddress As String = "0.0.0.0"
    Private mnNumber As Integer ' = 0

    Private moRobot As FRRobot.FRCRobot ' = Nothing
    Private moProgram As FRRobot.FRCProgram '= Nothing
    Private moVariable As FRRobot.FRCVar ' = Nothing
    Private mnValve As Integer = 1
    Private mnArmNumber As Integer = 1
    Private moColors As clsSysColors ' = Nothing
    Private mnZoneNumber As Integer = 1
    Private mColorchg As eColorChangeType = eColorChangeType.NONE
    Private mEstat As eEstatType = eEstatType.None
    Private msDBPath As String = String.Empty
    Private mbSelected As Boolean ' = False
    Private mbControlledStartMemory As Boolean ' = False
    Private msEstatName(2) As String
    Private mbInternalBackupEnabled As Boolean
    Private mnHotEditLoggerDO As Integer = 0
    Private mbVisionController As Boolean = False
    Private msBackupPath As String
    Private mnNumArms As Integer = 1
    Private mArms As clsArms ' = Nothing
    'Private mAlarmAssocData As clsAlarmAssocData ' = Nothing
    Private mbStylesByColor As Boolean ' = False
    Private moZone As clsZone ' = Nothing
    Private mControllerType As eControllerType = eControllerType.None
    Private mfVersion As Single
    Private mDMONCfg As clsDMONCfg = Nothing

    '******** Event    Variables    *****************************************************************
    Private WithEvents moPacketEvent As FRCPacketEvent '04/05/11 HGB Packet event support
    Private WithEvents moRNConn As FRCRNRobot ' = Nothing 'FRCRobotManagerConnection
    ' 08/15/13  MSW     PSC remote Robot support
    Private moRNConnAlt As FRCRNRobot ' = Nothing 'FRCRobotManagerConnection
    Private WithEvents moAlarms As FRRobot.FRCAlarms '= Nothing
    Private moRNRobot As FRCRNRealRobot ' = Nothing
    Private moVRobot As FRCRNVirtualRobot ' = Nothing
    Private mbAlarmMonitorEnabled As Boolean ' = False
    Private mbShuttingDown As Boolean ' = False ' hoakey alert
    Friend Event AlarmNotification(ByVal Alarm As Object, ByVal Name As String)
    Friend Event ConnStatusChange(ByVal Controller As clsController)
    Friend Event BumpProgress()
    Friend Event PacketEventReceiveNotification(ByRef PacketEvent As FRRobot.FRCPacket, ByRef oRobot As clsController) 'HGB 04/05/11 Packet event support
    Private moTag1 As Object = Nothing
    Private moTag2 As Object = Nothing
    ' 08/15/13  MSW     PSC remote Robot support
    Private mbRemote As Boolean = False
    Private msRemotePath As String = String.Empty
    Private WithEvents tmrConnStatus As System.Windows.Forms.Timer

    ' 09/13/13  MSW     Disable cross-thread checking when triggering events
    'This is one of the alternatives to dealing with multithreading from
    'the VB6 based PCDK.
	'If mbRerouteEvents is false, events are still passed the same,
    ' but cross thread checks are disabled.  
    'If mbRerouteEvents is true, events are not passed in the thread the 
    ' event is called in from PCDK.  It uses a timer in the screen thread to
    ' check the robot status instead.
    ' 11/21/13  MSW     Cross thread started bombing out with checking 
    '  disabled, so I'm using this method now.
    ' 02/13/14  MSW   Switch cross-thread handling to BeginInvoke call
    Private mbRerouteEvents As Boolean = False
#End Region
#Region " Properties "

    Friend Property Tag() As Object
        '********************************************************************************************
        'Description: Store an object to pass around to everybody
        '
        'Parameters: none
        'Returns:    tag
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moTag1
        End Get
        Set(ByVal value As Object)
            moTag1 = value
        End Set
    End Property
    Friend Property Tag2() As Object
        '********************************************************************************************
        'Description: Store an object to pass around to everybody
        '
        'Parameters: none
        'Returns:    tag2
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moTag2
        End Get
        Set(ByVal value As Object)
            moTag2 = value
        End Set
    End Property
    Friend ReadOnly Property Arms() As clsArms
        '********************************************************************************************
        'Description:  robots that belong to this controller
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mArms
        End Get
    End Property
    Friend Overloads ReadOnly Property Alarm() As FRCAlarm
        '********************************************************************************************
        'Description: Returns the most recent alarm in the robot alarms collection
        '
        'Parameters: none
        'Returns:    alarm item 0
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Robot.Alarms.Item(0)
        End Get
    End Property
    Friend Overloads ReadOnly Property Alarm(ByVal Index As Integer) As FRCAlarm
        '********************************************************************************************
        'Description: Returns the selected alarm from the robot alarms collection
        '
        'Parameters: none
        'Returns:    selected alarm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If Robot.Alarms.Count > Index Then
                Return Robot.Alarms.Item(Index)
            Else
                Return Robot.Alarms.Item(Robot.Alarms.Count - 1)
            End If
        End Get
    End Property
    Friend ReadOnly Property Alarms() As FRCAlarms
        '********************************************************************************************
        'Description: Returns the alarm collection from the robot
        '
        'Parameters: none
        'Returns:    alarm collection
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Robot.Alarms
        End Get
    End Property
    Friend Property ColorsByStyle() As Boolean
        '********************************************************************************************
        'Description: This is part of a hack to get presets by style by color
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbStylesByColor
        End Get
        Set(ByVal value As Boolean)
            mbStylesByColor = value
        End Set
    End Property
    Friend Property DMONCfg() As clsDMONCfg
        '********************************************************************************************
        'Description: Store DMON cfg 
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/17/11  MSW     Add for DMON schedman
        '********************************************************************************************
        Get
            Return mDMONCfg
        End Get
        Set(ByVal value As clsDMONCfg)
            mDMONCfg = value
        End Set
    End Property
    Friend ReadOnly Property ControllerNumber() As Integer
        '********************************************************************************************
        'Description: Hey sailor - can I get your number?
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumber
        End Get
    End Property
    Friend ReadOnly Property ControllerType() As eControllerType
        '********************************************************************************************
        'Description: What kind is it?
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mControllerType
        End Get
    End Property
    Friend ReadOnly Property DatabasePath() As String
        '********************************************************************************************
        'Description: path to the database where the info was obtained
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
                'local machine
                GetDefaultFilePath(msDBPath, eDir.Database, String.Empty, String.Empty)
            End If
            Return msDBPath
        End Get
    End Property
    Friend ReadOnly Property FanucName() As String
        '********************************************************************************************
        'Description:  FanucName for old stuff
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFanucName
        End Get
    End Property
    Friend Property HeartbeatEnable() As Boolean
        '********************************************************************************************
        ' Description:  Gets/Setss the Heartbeat Enable for the connection.
        '
        ' Parameters:   (Set)Heartbeat Enable
        ' Returns:      (Get)Heartbeat Enable
        '
        ' Modification history:
        '
        ' By           Date             Reason
        ' MSW          08/15/13         PSC remote Robot support
        ' MSW          09/13/13         Potential cross-thread workaround
        '********************************************************************************************

        Get
            If mbRemote Or mbRerouteEvents Then
                Return moRNConnAlt.HeartbeatEnable
            Else
                Return moRNConn.HeartbeatEnable
            End If
        End Get

        Set(ByVal value As Boolean)
            If mbRemote Or mbRerouteEvents Then
                moRNConnAlt.HeartbeatEnable = value
            Else
                moRNConn.HeartbeatEnable = value
            End If
        End Set

    End Property

    Friend Property HeartbeatPeriod() As Integer
        '********************************************************************************************
        ' Description:  Gets/Setss the Heartbeat Period (in seconds) for the connection.
        '
        ' Parameters:   (Set)Heartbeat Period
        ' Returns:      (Get)Heartbeat Period
        '
        ' Modification history:
        '
        ' By           Date             Reason
        ' MSW          08/15/13         PSC remote Robot support
        ' MSW          09/13/13         Potential cross-thread workaround
        '********************************************************************************************

        Get
            If mbRemote Or mbRerouteEvents Then
                Return moRNConnAlt.HeartbeatPeriod
            Else
                Return moRNConn.HeartbeatPeriod
            End If
        End Get

        Set(ByVal value As Integer)
            If mbRemote Or mbRerouteEvents Then
                moRNConnAlt.HeartbeatPeriod = value
            Else
                moRNConn.HeartbeatPeriod = value
            End If

        End Set

    End Property
    Friend ReadOnly Property InControlledStart() As Boolean
        '********************************************************************************************
        'Description: Status of Robot Start Mode. this is called by a change in connection
        '             state of the robot connection manager.
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                If mbControlledStartMemory Then
                    'We're coming out of a controlled start. The robot won't be ready to tell us it's StartMode
                    'so we won't even ask. 
                    'Trace.WriteLine("--> We're coming out of a Controlled Start.")
                    'We?
                    Return False
                Else
                    'Trace.WriteLine("--> Startmode = " & moRobot.SysInfo.StartMode.ToString)
                    If moRobot.SysInfo.StartMode = FREStartModeConstants.frControlStart Then
                        mbControlledStartMemory = True
                        Return True
                    Else
                        Return False
                    End If
                End If 'mbControlledStartMemory
            Catch ex As Exception
                Trace.WriteLine("Module: clsController, Routine: InControlledStart, Error: " & ex.Message)
                Trace.WriteLine("Module: clsController, Routine: InControlledStart, StackTrace: " & ex.StackTrace)
                'mclsServerExceptionHandler.HandleGeneralException(ex, Me, True)
            End Try

        End Get
    End Property
    Friend ReadOnly Property HotEditLoggerDO() As Integer
        '********************************************************************************************
        'Description: Enables Hot Edit Logger and sets the DOUT to watch
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/16/13  MSW     Sealer updates
        '********************************************************************************************
        Get
            Return mnHotEditLoggerDO
        End Get
    End Property
    Friend ReadOnly Property VisionController() As Boolean
        '********************************************************************************************
        'Description: Enables vision logger 
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/16/13  MSW     Sealer updates
        '********************************************************************************************
        Get
            Return mbVisionController
        End Get
    End Property
    Friend ReadOnly Property InternalBackupEnabled() As Boolean
        '********************************************************************************************
        'Description: Returns True if the controller must backup to FRA:\ prior to the start of
        '             PWMaint Robot Backup. Note: This is set up by the controls engineer when the
        '             system is configured and should be set to false if PLC IO for Auto Backup is 
        '             not present in the controller IO map.
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbInternalBackupEnabled
        End Get

    End Property
    Friend ReadOnly Property BackupPath() As String
        '********************************************************************************************
        'Description: Returns a backup path if assigned.  Allows use of MD: to backup ls and va if desired
        '           ignored if internal backup is true, if missing, use old plan - FRA: if internal backup
        '           enabled, MDB if not enabled or failed, MD: if the controller is too old
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/03/11  MSW     Support optional path for auto backups
        '********************************************************************************************

        Get
            Return msBackupPath
        End Get

    End Property
    Friend ReadOnly Property IPAddress() As String
        Get
            Return msIPAddress
        End Get
    End Property
    Friend ReadOnly Property LeavingControlledStart() As Boolean
        '********************************************************************************************
        'Description: If we're leaving a cold start we'll need to kill te old robot and connect a new
        '             one.
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        Get
            Dim bStatus As Boolean = mbControlledStartMemory
            mbControlledStartMemory = False
            Return bStatus
        End Get
    End Property
    Friend Property MonitorAlarms() As Boolean
        '********************************************************************************************
        'Description: Turns Alarm Notification On or Off. Returns Alarm Notification status.
        '
        'Parameters:  
        'Returns:     Alarm Monitoring Enabled (True or False)
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbAlarmMonitorEnabled
        End Get
        Set(ByVal value As Boolean)
            Call subMonitorAlarms(value)
        End Set
    End Property
    Friend ReadOnly Property Name() As String
        '********************************************************************************************
        'Description:  Name
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Name = msName
        End Get
    End Property
    Friend ReadOnly Property Robot() As FRRobot.FRCRobot
        '********************************************************************************************
        'Description:  Access to robot server
        '
        'Parameters: none
        'Returns:    robot
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moRobot
        End Get
    End Property
    Friend Property RCMConnectStatus() As ConnStat
        '********************************************************************************************
        'Description: Status of connection to frrobot. this is called by a change in connection
        '               state of the robot connection manager
        '
        'Parameters: none
        'Returns:    state
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mConnStat
        End Get

        Set(ByVal Value As ConnStat)
            mConnStat = Value
            RaiseEvent ConnStatusChange(Me)
            'Trace.WriteLine(Me.Name & " " & Value.ToString)
        End Set
    End Property
    Friend Property Selected() As Boolean
        '********************************************************************************************
        'Description: a way to mark as selected when looking thru collection
        '
        'Parameters:  
        'Returns:     
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
    Friend ReadOnly Property Version() As Single
        '********************************************************************************************
        'Description: A way to try to keep up with derek's changes
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/20/13  DE      Modified to handle Cultures that use "," in floating point numbers rather 
        '                   than ".".
        '********************************************************************************************
        Get
            If mfVersion = 0 Then
                If RCMConnectStatus = FRERNConnectionStatusConstants.frRNConnected Then
                    Dim oVer As FRCVar = DirectCast(moRobot.SysVariables.Item("$Version"), FRCVar)
                    Dim sVer As String = Strings.Mid(oVer.Value.ToString, 2, 6)
                    'mfVersion = CSng(Trim(sVer)) 'DE 09/20/13
                    mfVersion = Single.Parse(Trim(sVer), mLanguage.FixedCulture)
                End If
            End If
            Return mfVersion
        End Get
    End Property
    Friend ReadOnly Property Application() As String
        '********************************************************************************************
        'Description: Application name
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/23/14  MSW     Application - Didn't work at all
        '********************************************************************************************
        Get
            Dim sApp As String = String.Empty
            If RCMConnectStatus = FRERNConnectionStatusConstants.frRNConnected Then
                Dim oApp As FRCVars = DirectCast(moRobot.SysVariables.Item("$APPLICATION"), FRCVars)
                Dim oApp1 As FRCVar = DirectCast(oApp(, 0), FRCVar)
                sApp = oApp1.Value.ToString
            End If
            Return sApp
        End Get
    End Property
    Friend ReadOnly Property Zone() As clsZone
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
            Return moZone
        End Get
    End Property

#End Region
#Region " Routines "

    Private Sub subInitialize(ByRef drController As DataRow, ByRef oZone As clsZone, Optional ByRef sRemotePath As String = "")
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '       ****** add virtural robot someday *******
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/18/07  gks     Changes for read from xml, restructure
        ' 03/24/08  gks     Change to read from SQL database. This routine will handle the first
        '                   arm on the controller, other arms added elsewhere
        ' 08/15/13  MSW     PSC remote Robot support
        '********************************************************************************************

        msDBPath = oZone.DatabasePath
        moZone = oZone

        With drController
            'Can this go away???
            mnZoneNumber = oZone.ZoneNumber

            msFanucName = .Item(gsCONT_COL_HNAME).ToString
            msIPAddress = IPFromHostName(msFanucName)
            mnNumber = CType(.Item(gsCONT_COL_NUMBER), Integer)
            mbInternalBackupEnabled = CType(.Item(gsCONT_COL_IBE), Boolean)
            msBackupPath = String.Empty
            msName = .Item(gsCONT_COL_DNAME).ToString
            mControllerType = CType(.Item(gsCONT_COL_TYPE), eControllerType)

            mArms = New clsArms
            Dim o As New clsArm(drController, Me)
            mArms.Add(o)
            mnNumArms = mArms.Count
            msRemotePath = sRemotePath
            If msRemotePath <> "" Then
                mbRemote = True
            End If
        End With
    End Sub
    Private Sub subInitialize(ByRef oArm As tArm, ByRef oZone As clsZone, Optional ByRef sRemotePath As String = "")
        '********************************************************************************************
        'Description: When a new robot is requested, its info is passed in in the form of a datarow
        '   from the database
        '       ****** add virtural robot someday *******
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/18/07  gks     Changes for read from xml, restructure
        ' 03/24/08  gks     Change to read from SQL database. This routine will handle the first
        '                   arm on the controller, other arms added elsewhere
        ' 11/03/11  MSW     Support optional path for auto backups
        ' 08/15/13  MSW     PSC remote Robot support
        '********************************************************************************************

        msDBPath = oZone.DatabasePath
        moZone = oZone

        'With drController
        'Can this go away???
        mnZoneNumber = oZone.ZoneNumber

        msFanucName = oArm.Controller.HostName  '.Item(gsCONT_COL_HNAME).ToString
        msIPAddress = IPFromHostName(msFanucName)
        mnNumber = oArm.Controller.ControllerNumber 'CType(.Item(gsCONT_COL_NUMBER), Integer)
        mbInternalBackupEnabled = oArm.Controller.InternalBackup 'CType(.Item(gsCONT_COL_IBE), Boolean)
        mnHotEditLoggerDO = oArm.Controller.HotEditLoggerDO
        mbVisionController = oArm.Controller.VisionController
        msBackupPath = oArm.Controller.BackupPath
        msName = oArm.Controller.DisplayName '.Item(gsCONT_COL_DNAME).ToString
        mControllerType = CType(oArm.Controller.ControllerType, eControllerType)

        mArms = New clsArms
        Dim o As New clsArm(oArm, Me)
        mArms.Add(o)
        mnNumArms = mArms.Count
        msRemotePath = sRemotePath
        If msRemotePath <> "" Then
            mbRemote = True
        End If
        'End With
    End Sub
    Friend Sub subConnect()
        '********************************************************************************************
        'Description:  hook this thing up
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/20/07  gks     Added oBot per Whiteys email when using paint pro
        ' 07/21/08  gks     change to secret. Shhhh!
        ' 04/21/09  msw     Rearrange the error handling so it doesn't give up when it runs into old
        '                   robots in the robot neighborhood.
        ' 08/05/09  MSW     remove a robot if there's an address conflict
        ' 11/02/11  MSW     a little more error handling to deal with missing connections better
        ' 08/15/13  MSW     PSC remote Robot support
        ' 09/13/13  MSW     Potential cross-thread workaround
        '********************************************************************************************
        Dim RN As FRCRobotNeighborhood
        If mbRemote Then
            Dim oRemoteObject As Object = CreateObject("FRRobotNeighborhood.FRCRobotNeighborhood", msRemotePath)

            RN = DirectCast(oRemoteObject, FRRobotNeighborhood.FRCRobotNeighborhood)
        Else

            RN = New FRRobotNeighborhood.FRCRobotNeighborhood
        End If

        Dim oShhh As IRNInternal = CType(RN, IRNInternal)
        Dim RNConns As FRCRNRobots = oShhh.Robots0 ' RN.Robots 'oShhh.Robots0 '64 bit hack

        Dim RBT As FRCRNRealRobot
        Dim bExists As Boolean = False
        Dim oBot As Object
        Dim bIPAddressOK As Boolean
        Try
            'is it there already?
            'For Each RBT In RNConns
            For Each oBot In RNConns
                Try
                    If TypeOf oBot Is FRCRNRealRobot Then
                        RBT = DirectCast(oBot, FRCRNRealRobot)
                        If (RBT.Name = msName) Then
                            Try
                                bIPAddressOK = (RBT.IPAddress = msIPAddress)
                            Catch ex As Exception
                                bIPAddressOK = True
                            End Try
                            If (bIPAddressOK) Then
                                ' this is needed for development using the same robot multiple times
                                moRNRobot = DirectCast(RBT, FRCRNRealRobot)
                                If mbRemote Or mbRerouteEvents Then
                                    moRNConnAlt = DirectCast(RNConns.Item("\" & msName), FRCRNRobot)
                                Else
                                    moRNConn = DirectCast(RNConns.Item("\" & msName), FRCRNRobot)
                                End If
                                bExists = True
                                Exit For
                            Else
                                If msIPAddress <> "0.0.0.0" Then
                                    'I guess it needs to be removed
                                    Dim sName As String = RBT.Name
                                    RBT = Nothing
                                    RNConns.CancelKeepAlive()
                                    RNConns.Remove(sName)
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    'Ignore this robot if it gives any trouble
                    ' just go to the next one
                End Try
            Next 'oBot
            If bExists Then
                'RCMConnectStatus = moRNConn.ConnectionStatus
            Else
                'add the robot to neighborhood
                Dim sHostName As String = msIPAddress
                If sHostName = String.Empty Then sHostName = msFanucName

                ' Changed to Fanuc name which is the same as host name Geo. 3/19/09
                '  moRNRobot = RNConns.AddRealRobot(msFanucName, sHostName)
                '  moRNConn = DirectCast(RNConns.Item("\" & msFanucName), FRCRNRobot)
                If msIPAddress <> "0.0.0.0" Then
                    moRNRobot = RNConns.AddRealRobot(msName, sHostName)
                    If mbRemote Or mbRerouteEvents Then
                        moRNConnAlt = DirectCast(RNConns.Item("\" & msName), FRCRNRobot)
                    Else
                        moRNConn = DirectCast(RNConns.Item("\" & msName), FRCRNRobot)
                    End If

                End If
            End If

            moRobot = DirectCast(moRNRobot.RobotServer, FRCRobot)

            If moRNConn Is Nothing = False Then
                RCMConnectStatus = moRNConn.ConnectionStatus
            ElseIf moRNConnAlt Is Nothing = False Then
                RCMConnectStatus = moRNConnAlt.ConnectionStatus
                If tmrConnStatus Is Nothing Then
                    tmrConnStatus = New System.Windows.Forms.Timer
                    tmrConnStatus.Interval = 500
                    tmrConnStatus.Enabled = True
                End If
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: clsController, Routine: subConnect, Error: " & ex.Message)
            Trace.WriteLine("Module: clsController, Routine: subConnect, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub tmrConnStatus_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrConnStatus.Tick
        '********************************************************************************************
        'Description:  Update connection status   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/15/13  MSW     PSC remote Robot support
        ' 11/21/13  MSW     Add some error traps
        '********************************************************************************************
        Try
            If RCMConnectStatus <> moRNConnAlt.ConnectionStatus Then
                RCMConnectStatus = moRNConnAlt.ConnectionStatus
            End If
        Catch ex As Exception
            RCMConnectStatus = FRERNConnectionStatusConstants.frRNUnknown
            mDebug.WriteEventToLog("Module: Robots, Routine: tmrConnStatus_Tick", ex.Message)
        End Try
    End Sub
    Friend Sub subCreatePacketEvent(ByVal RequestCode As Byte, ByVal SubSystemCode As FREPacketEventConstants)
        '********************************************************************************************
        'Description:  Create a packcet event object   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/05/11  HGB      Created packet event support
        '********************************************************************************************

        moPacketEvent = Nothing

        Try
            moPacketEvent = moRobot.CreatePacketEvent(SubSystemCode, RequestCode)
        Catch ex As Exception
            moPacketEvent = Nothing
        End Try
    End Sub

    Friend Sub subDestroyPacketEvent(ByVal RequestCode As Byte, ByVal SubSystemCode As FREPacketEventConstants)
        '********************************************************************************************
        'Description:  Create a packcet event object   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/05/11  HGB      Created packet event support
        '********************************************************************************************
        ' SubSystemCode and Requestcode will be used in the future to find the proper packet event object 
        ' in a collection

        moPacketEvent = Nothing

    End Sub

    Private Sub subMonitorAlarms(ByVal Enable As Boolean)
        '********************************************************************************************
        'Description:  Turn Alarm notification On or Off
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If Enable Then
                'mAlarmAssocData = New clsAlarmAssocData(moRobot)
                For Each oArm As clsArm In Arms
                    oArm.AlarmAssocDataEnable = True
                Next
                moAlarms = DirectCast(moRobot.Alarms, FRCAlarms)
                If Not IsNothing(moAlarms) Then
                    'If there are currently any active alarms, add them to the grid
                    If (Not IsNothing(moAlarms.Item(0))) And moAlarms.ActiveAlarms Then
                        Dim nItem As Integer = 0
                        Dim nReset As Integer = 0
                        'Figure out how many alarms are active
                        For nItem = 0 To (moAlarms.Count - 1)
                            If moAlarms.Item(nItem).ErrorFacility = 0 Then
                                nReset = nItem
                                Exit For
                            End If
                        Next
                        'Send each active alarm to AlarmMan
                        For nItem = nReset To 0 Step -1
                            moAlarms_AlarmNotify(moAlarms.Item(nItem))
                        Next
                    End If
                End If ' Not IsNothing(moAlarms)
            Else
                moAlarms = Nothing
                For Each oArm As clsArm In Arms
                    oArm.AlarmAssocDataEnable = False
                Next
            End If
        Catch ex As Exception
            Trace.WriteLine("Module: clsController, Routine: subMonitorAlarms, Error: " & ex.Message)
            Trace.WriteLine("Module: clsController, Routine: subMonitorAlarms, StackTrace: " & ex.StackTrace)
            moAlarms = Nothing
            Enable = False
        Finally
            mbAlarmMonitorEnabled = Enable
        End Try

    End Sub

#End Region
#Region " Events "

    Private Sub moPacketEvent_Receive(ByRef Packet As FRRobot.FRCPacket) Handles moPacketEvent.Receive

        '********************************************************************************************
        'Description: A new packet event was sent by this robot controller. Pass it up to the 
        '             Controllers collection.
        '
        'Parameters: Data Packet
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04-23-2010 HGB     Created Packet event support
        '********************************************************************************************
        RaiseEvent PacketEventReceiveNotification(Packet, Me)

    End Sub

    Private Sub moAlarms_AlarmNotify(ByVal Alarm As Object) Handles moAlarms.AlarmNotify
        '********************************************************************************************
        'Description: A new alarm was generated by this robot controller. Pass it up to the 
        '             Controllers collection.
        '
        'Parameters: Alarm
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        RaiseEvent AlarmNotification(Alarm, Name)

    End Sub
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub moRNConn_OnConnectionStatusChange(ByVal NewStatus As ConnStat) _
                                                    Handles moRNConn.OnConnectionStatusChange
        '********************************************************************************************
        'Description: todo fix why this crashes on exit
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/15/13  MSW     PSC remote Robot support
        ' 09/13/13  MSW     Potential cross-thread workaround
        '********************************************************************************************
        If mbShuttingDown Then Exit Sub
        Try
            If mbRemote Or mbRerouteEvents Then
                RCMConnectStatus = moRNConnAlt.ConnectionStatus
            Else
                RCMConnectStatus = moRNConn.ConnectionStatus
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub New(ByRef oArm As tArm, ByRef oZone As clsZone, Optional ByRef sRemotePath As String = "")
        '********************************************************************************************
        'Description: When a new controller is requested, its info is passed in in the form of a 
        '   datarow from the database
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        msDBPath = oZone.DatabasePath
        subInitialize(oArm, oZone, sRemotePath)
    End Sub
    Friend Sub New(ByRef drController As DataRow, ByRef oZone As clsZone, Optional ByRef sRemotePath As String = "")
        '********************************************************************************************
        'Description: When a new controller is requested, its info is passed in in the form of a 
        '   datarow from the database
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        msDBPath = oZone.DatabasePath
        subInitialize(drController, oZone, sRemotePath)
    End Sub
    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description: Goodbye
        '
        'Parameters: 
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/20/07  gks     getting crashes- comment out- dunno why
        '********************************************************************************************
        Try
            ''moAlarms = Nothing
            ''mbShuttingDown = True
            ''moRNRobot = Nothing
            ''moVRobot = Nothing

            ''Application.DoEvents()
            GC.Collect()

        Catch ex As Exception
            Trace.WriteLine("Module: clsController, Routine: Finalize, Error: " & ex.Message)
            Trace.WriteLine("Module: clsController, Routine: Finalize, StackTrace: " & ex.StackTrace)
        End Try

        MyBase.Finalize()
    End Sub

#End Region

End Class 'clsController
'********************************************************************************************
'Description: calibration tables
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsCalData

#Region " Declares "

    Private mfFlow(10) As clsSngValue
    Private mnDynCnt(10) As clsIntValue
    Private mnRefCnt(10) As clsIntValue
    Private mfParmData(10) As clsSngValue
    Private msParmName(10) As String
    '9.5.07 remove initialization per fxCop
    Private mfFlowMin As Single ' = 0
    Private mfFlowMax As Single = 1000
    Private mnXducMax As Integer = 8000
    Private mnXducMin As Integer = 1600
    Private mnCalStatus As Integer

#End Region
#Region " Properties "

    Friend Property ParameterName(ByVal Index As eCalParameters) As String
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
            If Index >= 0 And Index < 11 Then
                Return msParmName(Index)
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As String)
            If Index >= 0 And Index < 11 Then
                msParmName(Index) = value
            End If
        End Set
    End Property
    Friend ReadOnly Property ParameterData(ByVal Index As eCalParameters) As clsSngValue
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
            If Index >= 0 And Index < 11 Then
                Return mfParmData(Index)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Friend ReadOnly Property FlowValue(ByVal Index As Integer) As clsSngValue
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
            If Index > 0 And Index < 11 Then
                Return mfFlow(Index)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Friend ReadOnly Property ReferenceCount(ByVal Index As Integer) As clsIntValue
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
            If Index > 0 And Index < 11 Then
                Return mnRefCnt(Index)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Friend ReadOnly Property DynamicCount(ByVal Index As Integer) As clsIntValue
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
            If Index > 0 And Index < 11 Then
                Return mnDynCnt(Index)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Friend ReadOnly Property Changed() As Boolean
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
            For i As Integer = 0 To 10
                If mfFlow(i).Changed Then Return True
                If mnDynCnt(i).Changed Then Return True
                If mnRefCnt(i).Changed Then Return True
                If mfParmData(i).Changed Then Return True
            Next
            Return False
        End Get
    End Property
    Friend Property FlowMinimumValue() As Single
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
            Return mfFlowMin
        End Get
        Set(ByVal value As Single)
            mfFlowMin = value
        End Set
    End Property
    Friend Property FlowMaximumValue() As Single
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
            Return mfFlowMax
        End Get
        Set(ByVal value As Single)
            mfFlowMax = value
        End Set
    End Property
    Friend Property TransducerMinCount() As Integer
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
            Return mnXducMin
        End Get
        Set(ByVal value As Integer)
            mnXducMin = value
        End Set
    End Property
    Friend Property TransducerMaxCount() As Integer
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
            Return mnXducMax
        End Get
        Set(ByVal value As Integer)
            mnXducMax = value
        End Set
    End Property
    Friend Property CalStatusValue() As Integer
        Set(ByVal value As Integer)
            mnCalStatus = value
        End Set
        Get
            Return mnCalStatus
        End Get
    End Property
 

#End Region
#Region " Routines "

    Friend Sub Update()
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
        For i As Integer = 0 To 10
            mfFlow(i).Update()
            mnDynCnt(i).Update()
            mnRefCnt(i).Update()
            mfParmData(i).Update()
        Next
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
        Dim i%
        For i% = 0 To 10
            mfFlow(i%) = New clsSngValue
            mnDynCnt(i%) = New clsIntValue
            mnRefCnt(i%) = New clsIntValue
            mfParmData(i%) = New clsSngValue
            msParmName(i%) = String.Empty
        Next
    End Sub

#End Region

End Class 'clsCalData


'********************************************************************************************
'Description: Robot Common Code
'
'
'Modification history:
'
' Date      By      Reason
' 11/10/09  MSW     Add robot variable lookup through resource files.
'******************************************************************************************** 
Friend Module mPWRobotCommon

    'This is used to hold parameter/preset column details.
    Friend Structure tParmDetail
        Dim nParmNum As Integer
        Dim sLblCap As String
        Dim sLblName As String
        Dim sUnits As String
        Dim nMax As Single
        Dim nMin As Single
    End Structure
    Friend Enum eParmSelect
        FluidPresets
        EstatPresets
        CCPresets
        All
    End Enum
    Friend Sub subInitParmConfig(ByRef rRobot As clsArm, ByVal ParmSelect As eParmSelect, _
                                        ByRef nColDetails() As tParmDetail, _
                                        ByRef nNumParms As Integer, _
                                        Optional ByRef oApplicator As clsApplicator = Nothing)
        '********************************************************************************************
        'Description:  Load preset config from Robot
        '
        'Parameters: robot, Screen type(cc,fluid,estat), labels, num parms
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/22/11  MSW     Fix program name for version < 6.3 
        ' 01/19/12  MSW     pass in applicator object for quicker settings
        ' 02/01/12  MSW     Debug fixes from the recent applicator updates
        '                   subInitParmConfig - Fix index problems filling in nColDetails
        '********************************************************************************************
        Dim oArr As FRRobot.FRCVars = Nothing
        If ParmSelect = eParmSelect.CCPresets Then
            'Get parameter order
            If rRobot.Controller.Version >= 6.3 Then
                rRobot.ProgramName = "PAVRCCEX"
                Dim oPrg As FRRobot.FRCVars = rRobot.ProgramVars
                Dim oArr2D As FRRobot.FRCVars = DirectCast(oPrg.Item("TCCEXCMOS"), FRRobot.FRCVars)
                oArr = DirectCast(oArr2D.Item("CC_TO_PAR"), FRRobot.FRCVars)
            Else
                rRobot.ProgramName = "PAVRCCIO"
                Dim oPrg As FRRobot.FRCVars = rRobot.ProgramVars
                oArr = DirectCast(oPrg.Item("CC_TO_PAR"), FRRobot.FRCVars)
            End If
        Else
            'Get parameter order
            rRobot.ProgramName = "PAPARMDB"
            Dim oPrg As FRRobot.FRCVars = rRobot.ProgramVars
            Dim oArr2D As FRRobot.FRCVars = DirectCast(oPrg.Item("SCRN_TO_PAR"), FRRobot.FRCVars)
            Select Case ParmSelect
                Case eParmSelect.FluidPresets
                    oArr = DirectCast(oArr2D.Item(1), FRRobot.FRCVars)
                Case eParmSelect.EstatPresets
                    oArr = DirectCast(oArr2D.Item(2), FRRobot.FRCVars)
            End Select
        End If
        nNumParms = oArr.Count
        ReDim nColDetails(nNumParms - 1)
        For nItem As Integer = 1 To oArr.Count
            Dim oVarTmp As FRRobot.FRCVar = DirectCast(oArr.Item(nItem), FRRobot.FRCVar)
            nColDetails(nItem - 1).nParmNum = CType(oVarTmp.Value, Integer)
            If nColDetails(nItem - 1).nParmNum < 1 Then
                nNumParms = nItem - 1
                Exit For
            End If
        Next
        ReDim Preserve nColDetails(nNumParms - 1)
        If ParmSelect <> eParmSelect.CCPresets Then
            If ((rRobot.Controller.Version >= 6.3) And (rRobot.Controller.Version < 7.3) And _
                (rRobot.Controller.Arms.Count > 1)) Then
                'Dual arm stuffs two arrays in one
                nNumParms = nNumParms \ 2
            End If
        End If
        If oApplicator Is Nothing And rRobot.Applicator IsNot Nothing Then
            oApplicator = rRobot.Applicator
        End If
        If oApplicator IsNot Nothing Then
            For nCol As Integer = 1 To nNumParms
                nColDetails(nCol - 1).nMax = oApplicator.MaxEngUnit(nColDetails(nCol - 1).nParmNum - 1)
                nColDetails(nCol - 1).nMin = oApplicator.MinEngUnit(nColDetails(nCol - 1).nParmNum - 1)
                nColDetails(nCol - 1).sLblName = oApplicator.ParamName(nColDetails(nCol - 1).nParmNum - 1)
                nColDetails(nCol - 1).sLblCap = oApplicator.ParamNameCAP(nColDetails(nCol - 1).nParmNum - 1)
                nColDetails(nCol - 1).sUnits = oApplicator.ParamUnits(nColDetails(nCol - 1).nParmNum - 1)
            Next
        Else
            'Get parm details
            rRobot.ProgramName = "PAVRPARM"
            Dim oProg As FRRobot.FRCVars = rRobot.ProgramVars
            Dim oStruct1 As FRRobot.FRCVars
            If rRobot.Controller.Version >= 7.3 Then
                Dim oEQ As FRRobot.FRCVars = DirectCast(oProg.Item("TPARMEQ"), FRRobot.FRCVars)
                oStruct1 = DirectCast(oEQ.Item(rRobot.ArmNumber), FRRobot.FRCVars)
            Else
                oStruct1 = oProg
            End If
            Dim sTmp1 As String
            Dim sTmp2 As String
            Dim sTmp3 As String
            For nCol As Integer = 1 To nNumParms
                Dim nParm As Integer = nColDetails(nCol - 1).nParmNum
                Dim oStruct2 As FRRobot.FRCVars = DirectCast(oStruct1.Item("Con_Prm_Desc"), FRRobot.FRCVars)
                Dim ooo As FRRobot.FRCVars = DirectCast(oStruct2.Item(nParm), FRRobot.FRCVars)

                Dim oVar As FRRobot.FRCVar = DirectCast(ooo.Item("App_Con_Max"), FRRobot.FRCVar)
                nColDetails(nCol - 1).nMax = CInt(oVar.Value)

                oVar = DirectCast(ooo.Item("App_Con_Min"), FRRobot.FRCVar)
                nColDetails(nCol - 1).nMin = CInt(oVar.Value)

                'Use robot labels for resource lookup
                ' This is the short form in the robot (FF,AA, ...)
                oVar = DirectCast(ooo.Item("APP_CON_NAM"), FRRobot.FRCVar)
                sTmp1 = "rs" & oVar.Value.ToString
                sTmp2 = grsRM.GetString(sTmp1)
                If sTmp2 = String.Empty Then
                    nColDetails(nCol - 1).sLblName = sTmp1
                    Debug.Print("Missing resource string: " & sTmp1)
                Else
                    nColDetails(nCol - 1).sLblName = sTmp2
                End If
                sTmp3 = sTmp1 & "_CAP"
                sTmp2 = grsRM.GetString(sTmp3)
                If sTmp2 = String.Empty Then
                    nColDetails(nCol - 1).sLblCap = sTmp3
                    Debug.Print("Missing resource string: " & sTmp3)
                Else
                    nColDetails(nCol - 1).sLblCap = sTmp2
                End If
                sTmp3 = sTmp1 & "_UNITS"
                sTmp2 = grsRM.GetString(sTmp3)
                If sTmp2 = String.Empty Then
                    nColDetails(nCol - 1).sUnits = sTmp3
                    Debug.Print("Missing resource string: " & sTmp3)
                Else
                    nColDetails(nCol - 1).sUnits = sTmp2
                End If
            Next
        End If
    End Sub

    Friend Function LoadRobotBoxFromCollection(ByRef rCbo As ComboBox, _
                ByRef ArmCollection As clsArms, ByVal AddAll As Boolean, _
                Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                Optional ByVal AddNone As Boolean = False, _
                Optional ByVal IncludeOpeners As Boolean = True) As Boolean

        '********************************************************************************************
        'Description:  Load Robot combo with arm names
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/01/09   MSW     Add in the select by cc type
        '11/05/09   RJO     Add in IncludeOpeners
        '11/05/09   MSW     Add not none as color change type option
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        If ArmCollection Is Nothing Then Return False

        Dim nTmp As Integer()
        ReDim nTmp(0)
        Dim bAdd As Boolean

        rCbo.Items.Clear()

        If AddNone Then
            rCbo.Items.Add(gcsRM.GetString("csNONE"))
            nTmp(0) = 0
            ReDim Preserve nTmp(1)
        End If

        If AddAll Then
            rCbo.Items.Add(gcsRM.GetString("csALL"))
            nTmp(0) = 0
            ReDim Preserve nTmp(1)
        End If

        For Each o As clsArm In ArmCollection
            bAdd = True
            If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                If (CCType1 = eColorChangeType.NOT_NONE) Then
                    If o.ColorChangeType = eColorChangeType.NONE Then
                        bAdd = False
                    End If
                Else
                    If (o.ColorChangeType <> CCType1) And (o.ColorChangeType <> CCType2) Then
                        bAdd = False
                    End If
                End If
            End If

            If (bAdd AndAlso o.IsOpener) Then bAdd = IncludeOpeners

            If bAdd Then
                rCbo.Items.Add(o.Name)
                'Removed o.Index not being set 
                'nTmp(UBound(nTmp)) = o.Index
                nTmp(UBound(nTmp)) = o.RobotNumber
                ReDim Preserve nTmp(UBound(nTmp) + 1)
            End If
        Next

        rCbo.Tag = nTmp

    End Function
    Friend Function LoadRobotBoxFromCollection(ByRef rCbo As ToolStripComboBox, _
                ByRef ArmCollection As clsArms, ByVal AddAll As Boolean, _
                Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                Optional ByVal IncludeOpeners As Boolean = True) As Boolean

        '********************************************************************************************
        'Description:  Load ToolStrip Robot combo with arm names
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/01/09   MSW     Add in the select by cc type
        '11/05/09   RJO     Add in IncludeOpeners
        '11/05/09   MSW     Add not none as color change type option
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        If ArmCollection Is Nothing Then Return False

        Dim nTmp As Integer()
        ReDim nTmp(0)
        Dim bAdd As Boolean

        rCbo.Items.Clear()

        If AddAll Then
            rCbo.Items.Add(gcsRM.GetString("csALL"))
            nTmp(0) = 0
            ReDim Preserve nTmp(1)
        End If

        For Each o As clsArm In ArmCollection
            bAdd = True
            If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                If (CCType1 = eColorChangeType.NOT_NONE) Then
                    If o.ColorChangeType = eColorChangeType.NONE Then
                        bAdd = False
                    End If
                Else
                    If (o.ColorChangeType <> CCType1) And (o.ColorChangeType <> CCType2) Then
                        bAdd = False
                    End If
                End If
            End If

            If (bAdd AndAlso o.IsOpener) Then bAdd = IncludeOpeners

            If bAdd Then
                rCbo.Items.Add(o.Name)
                'Removed o.Index not being set 
                'nTmp(UBound(nTmp)) = o.Index
                nTmp(UBound(nTmp)) = o.RobotNumber
                ReDim Preserve nTmp(UBound(nTmp) + 1)
            End If
        Next

        rCbo.Tag = nTmp

    End Function
    Friend Function LoadRobotBoxFromCollection(ByRef rCbo As ComboBox, _
                ByRef ControllerCollection As clsControllers, ByVal AddAll As Boolean, _
                Optional ByVal IncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        'Description:  Load Robot combo with arm names
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/05/10  MSW     Add IncludeOpeners check for controller box
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        If ControllerCollection Is Nothing Then Return False

        Dim nTmp As Integer()
        Dim i As Integer = 0
        ReDim nTmp(i)

        rCbo.Items.Clear()

        If AddAll Then
            rCbo.Items.Add(gcsRM.GetString("csALL"))
            nTmp(i) = i
            i += 1
            ReDim Preserve nTmp(i)
        End If

        Dim bAdd As Boolean = False
        For Each o As clsController In ControllerCollection
            If IncludeOpeners Then
                bAdd = True
            Else
                bAdd = False
                For Each oArm As clsArm In o.Arms
                    If (oArm.IsOpener = False) Then
                        bAdd = True
                        Exit For
                    End If
                Next
            End If
            If bAdd Then
                rCbo.Items.Add(o.Name)
                nTmp(i) = o.ControllerNumber
                i += 1
                ReDim Preserve nTmp(i)
            End If
        Next

        rCbo.Tag = nTmp

    End Function
    Friend Function LoadRobotBoxFromCollection(ByRef rLst As CheckedListBox, _
                ByVal ControllerCollection As clsControllers, ByVal Void As Boolean, _
                Optional ByVal bIncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        'Description:  Load Robot checked Listbox with Controller names 
        '
        'Parameters: box to load, controller collection and a void so we can overload
        'Returns:    true if ok
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/14/10  MSW     support skipping opener controllers
        '********************************************************************************************    
        If rLst Is Nothing Then Return False
        If ControllerCollection Is Nothing Then Return False

        Try
            Dim sNames() As String
            ReDim sNames(ControllerCollection.Count - 1)
            Dim nOpenerControllers As Integer = 0
            For i As Integer = 0 To ControllerCollection.Count - 1
                Dim bOpeners As Boolean = True
                For Each oArm As clsArm In ControllerCollection.Item(i).Arms
                    If oArm.IsOpener = False Then
                        bOpeners = False
                        Exit For
                    End If
                Next
                If bOpeners Then
                    nOpenerControllers += 1
                Else
                    sNames(i - nOpenerControllers) = ControllerCollection.Item(i).Name
                End If
            Next
            If nOpenerControllers > 0 Then
                ReDim Preserve sNames((ControllerCollection.Count - 1) - nOpenerControllers)
            End If
            subLoadCheckListBox(rLst, sNames)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function
    Friend Sub subLoadCheckListBox(ByRef rLst As CheckedListBox, ByVal sNames As String())
        '********************************************************************************************
        'Description:  Load Robot Listbox with names - This routine is a total kludge, the 
        '               checked list box has a mind of its own, and seems to use the last thing 
        '               loaded to size the columns - then the overall size needs to be forced.
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/05/09  MSW     Set the width before adding names to prevent a scrollbar
        ' 04/09/10  RJO     Kludged the kludge. More than 10 robots causes real estate problems on the
        '                   copy screen.
        ' 07/29/10  MSW     change + 25 to + 30 - 3 robot 2 row box wasn't working
        ' 07/30/14  RJO     .Multicolumn and .ScrollAlwaysVisible needed to be mutually exclusive.
        '                   No scroll bar showed up when there were 12 robots in the list.
        '********************************************************************************************    
        Dim nLargeWidth As Integer = 0
        Dim nWidth As Integer = 0
        Dim sName As String = String.Empty
        Dim g As Graphics = rLst.CreateGraphics

        With rLst
            .BeginUpdate()
            .Items.Clear()

            'RJO 04/09/10
            If rLst.Parent.Name = "gpbFrom" Or rLst.Parent.Name = "gpbTo" Then
                If sNames.GetUpperBound(0) < 10 Then
                    .MultiColumn = True
                    .ScrollAlwaysVisible = False
                Else 'RJO 07/30/14
                    .MultiColumn = False
                    .ScrollAlwaysVisible = True
                End If
            Else
                .MultiColumn = True
                .ScrollAlwaysVisible = False
                .Width = 1000
            End If

            For i As Integer = 0 To UBound(sNames)

                sName = sNames(i)
                '' Determine the width of the items in the list to get the best column width setting.
                nWidth = CInt(g.MeasureString(sName, .Font).Width)
                If nWidth > nLargeWidth Then nLargeWidth = nWidth

                .Items.Add(sName)

            Next

            g.Dispose()
            .ColumnWidth = nLargeWidth + 20

            If .MultiColumn Then 'RJO 04/09/10
                Dim sz As Size = .Size
                'This will decide if it needs to be wide enough to get all the robots in 1 or 2 rows
                'MSW change + 25 to + 30 - 3 robot 2 row box wasn't working
                If sz.Height > (.Font.Height * 2) Then
                    sz.Width = CType((nLargeWidth + 20) * (UBound(sNames) + 1) / 2, Integer) + 45
                Else
                    sz.Width = CType((nLargeWidth + 20) * (UBound(sNames) + 1), Integer) + 45
                End If
                .Size = sz
                .HorizontalScrollbar = False
                .ScrollAlwaysVisible = False
            End If

            .EndUpdate()
        End With

    End Sub
    Friend Function GetCycleNamesFromDB(ByRef rZone As clsZone, ByVal CCType As eColorChangeType) As String()
        '********************************************************************************************
        'Description:  get cc cyle names from the database
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/23/12  MSW     Move from DB to XML
        '********************************************************************************************
        Dim oCCTypeMainNode As XmlNode = Nothing
        Dim oCCTypeNode As XmlNode = Nothing
        Dim oCCTypeNodeList As XmlNodeList = Nothing
        Dim oCCTypeXMLDoc As New XmlDocument
        Dim sCCTypeXMLFilePath As String = String.Empty
        Dim nNumCycles As Integer
        Dim sCCCycles() As String = Nothing

        'Get data table for parameter setup:
        Try
            If GetDefaultFilePath(sCCTypeXMLFilePath, eDir.XML, String.Empty, gsCCTYPE_XMLTABLE & ".XML") Then
                oCCTypeXMLDoc.Load(sCCTypeXMLFilePath)
                oCCTypeMainNode = oCCTypeXMLDoc.SelectSingleNode("//" & gsCCTYPE_XMLTABLE)
                oCCTypeNodeList = oCCTypeMainNode.SelectNodes("//" & gsCCTYPE_XMLNODE & "[" & gsCCTYPE_COL_ID & "='" & CInt(CCType).ToString & "']")
                Try
                    If oCCTypeNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", sCCTypeXMLFilePath & " not found.")
                    Else
                        oCCTypeNode = oCCTypeNodeList(0)
                        Try
                            nNumCycles = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_CYCLES).InnerXml, Integer)
                        Catch ex As Exception
                            nNumCycles = 10
                        End Try
                        Try
                            sCCCycles = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_CYCLE_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            'Just throw something in there until the applicator database gets built
                            ReDim sCCCycles(nNumCycles - 1)
                            For nItem As Integer = 1 To nNumCycles
                                sCCCycles(nItem - 1) = "Cycle " & nNumCycles.ToString
                            Next
                        End Try
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XML Data: " & sCCTypeXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            Dim sTmp As String = String.Empty
            ShowErrorMessagebox(gcsRM.GetString("csERRO"), ex, "Color Change Cycle Names", _
            sTmp, MessageBoxButtons.OK)
            Trace.WriteLine("Module: Robots, Routine: GetCycleNamesFromDB - table name, Error: " & ex.Message)
            Trace.WriteLine("Module: Robots, Routine: GetCycleNamesFromDB - table name, StackTrace: " & ex.StackTrace)
        End Try
        Return sCCCycles

    End Function
    Friend Function LoadRobotBoxFromCollection(ByRef rLst As CheckedListBox, _
                                               ByVal ArmCollection As clsArms, ByVal Void As Boolean, _
                                               Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                                               Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                                               Optional ByVal IncludeOpeners As Boolean = True, _
                                               Optional ByVal AddAll As Boolean = False, Optional ByVal nStation As Integer = 0) As Boolean
        '********************************************************************************************
        'Description:  Load Robot checked Listbox with Arm names 
        '
        'Parameters: box to load, controller collection and a void so we can overload
        'Returns:    true if ok
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/01/09   MSW     Add in the select by cc type
        '11/05/09   RJO     Add in IncludeOpeners
        '11/05/09   MSW     Add not none as color change type option
        '3/10/14    CBZ add stations 
        '********************************************************************************************    
        If rLst Is Nothing Then Return False
        If ArmCollection Is Nothing Then Return False

        Try
            Dim sNames() As String
            Dim nTmp As Integer()
            Dim bAdd As Boolean
            Dim nCount As Integer = 0
            ReDim Preserve nTmp(0)
            ReDim Preserve sNames(0)
            If AddAll Then
                nCount = nCount + 1
                ReDim Preserve nTmp(nCount - 1)
                ReDim Preserve sNames(nCount - 1)
                sNames(nCount - 1) = gcsRM.GetString("csALL")
                nTmp(nCount - 1) = 0
            End If
            For Each o As clsArm In ArmCollection
                bAdd = True
                If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                    If (CCType1 = eColorChangeType.NOT_NONE) Then
                        If o.ColorChangeType = eColorChangeType.NONE Then
                            bAdd = False
                        End If
                    Else
                        If (o.ColorChangeType <> CCType1) And (o.ColorChangeType <> CCType2) Then
                            bAdd = False
                        End If
                    End If
                End If

                If (nStation <> 0) Then
                    If Not (o.StationNumber = nStation) Then
                        bAdd = False
                    End If
                End If

                If (bAdd AndAlso o.IsOpener) Then bAdd = IncludeOpeners

                If bAdd Then
                    nCount = nCount + 1
                    ReDim Preserve nTmp(nCount - 1)
                    ReDim Preserve sNames(nCount - 1)
                    sNames(nCount - 1) = o.Name
                    nTmp(nCount - 1) = o.RobotNumber
                End If
            Next
            subLoadCheckListBox(rLst, sNames)
            rLst.Tag = nTmp 'Robot number index
            Return True

        Catch ex As Exception
            Return False
        End Try


    End Function

    Friend Function LoadArmCollection(ByVal ControllerCollection As clsControllers) As clsArms
        '********************************************************************************************
        'Description:  load collection of arms
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim o As New clsArms
        For Each oC As clsController In ControllerCollection
            For Each oA As clsArm In oC.Arms
                o.Add(oA)
            Next
        Next
        Return o
    End Function

    Friend Function LoadArmCollection(ByVal ControllerCollection As clsControllers, ByVal IncludeOpeners As Boolean) As clsArms
        '********************************************************************************************
        'Description:  load collection of arms
        '
        'Parameters: ControllerCollection, IncludeOpeners
        'Returns:    Collection of arms
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/19/09  RJO     frmCopy requires an arm collection without openers for some copy functions.
        '********************************************************************************************    
        Dim o As New clsArms

        For Each oC As clsController In ControllerCollection
            For Each oA As clsArm In oC.Arms
                If IncludeOpeners Or (Not oA.IsOpener) Then
                    o.Add(oA)
                End If
            Next
        Next
        Return o
    End Function
    Friend Sub UrbanRenewal()
        '********************************************************************************************
        'Description: Clean up the Hood
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '07/21/08   gks     change to use secret
        '********************************************************************************************
        Dim RN As New FRCRobotNeighborhood
        Dim oShhh As IRNInternal = CType(RN, IRNInternal)
        Dim RNConns As FRCRNRobots = oShhh.Robots0 'RN.Robots ' oShhh.Robots0 '64
        Dim o As FRCRNRobot
        RNConns.CancelKeepAlive()
        For Each o In RN.Robots
            o.AutoReconnectEnable = False
            o.KeepAliveEnable = False
            o = Nothing
        Next

    End Sub
    Public Sub subIndexScatteredAccess(ByRef sData As String(), _
                 ByVal nEquipNumber As Integer, ByRef nScatteredAccessIndexes As Integer())
        '********************************************************************************************
        'Description:   Generate an index array for the scattered access data
        '               Use the IndexOf to search for the tag
        '               Data should be in the (tag index + 1) location of the 1 dimentional array.
        '
        'Parameters:    sData - SA data converted into 1 dimension array
        '               nEquipNumber - select "Eq#" in the SA tag name
        '               nScatteredAccessIndexes - array to fill with indexes for sData
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/23/09  BTK     Added variables for dual shaping air.
        '********************************************************************************************

        'Dim sEQPrefix As String = "EQ" & nEquipNumber & ":" & " "
        Dim sEQPrefix As String = ""

        'Actual Flow Rate
        nScatteredAccessIndexes(eSAIndex.ActFlow) = Array.IndexOf(sData, sEQPrefix & gsSA_FLOW_RATE) + 1
        If nScatteredAccessIndexes(eSAIndex.ActFlow) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_FLOW_RATE)

        'Accuflow Cal Active 
        nScatteredAccessIndexes(eSAIndex.AFCalActive) = Array.IndexOf(sData, sEQPrefix & gsSA_AF_CAL_ACTIVE) + 1
        If nScatteredAccessIndexes(eSAIndex.AFCalActive) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_AF_CAL_ACTIVE)

        '  Paint AOUT
        nScatteredAccessIndexes(eSAIndex.PaintAout) = Array.IndexOf(sData, sEQPrefix & gsSA_PAINT_AOUT) + 1
        If nScatteredAccessIndexes(eSAIndex.PaintAout) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_PAINT_AOUT)

        'Requested Flow
        nScatteredAccessIndexes(eSAIndex.ReqFlow) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_FLOW) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqFlow) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_REQUESTED_FLOW)

        ' Total Flow
        nScatteredAccessIndexes(eSAIndex.TotFlow) = Array.IndexOf(sData, sEQPrefix & gsSA_FLOW_TOTAL) + 1
        If nScatteredAccessIndexes(eSAIndex.TotFlow) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_FLOW_TOTAL)

        ' Pump 1 & 2 Flow
        nScatteredAccessIndexes(eSAIndex.ReqFlowP1) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_FLOW_PUMP_1) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqFlowP1) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_REQUESTED_FLOW_PUMP_1)
        nScatteredAccessIndexes(eSAIndex.ReqFlowP2) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_FLOW_PUMP_2) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqFlowP2) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_REQUESTED_FLOW_PUMP_2)

        'Paint Pressures
        nScatteredAccessIndexes(eSAIndex.ResinInletPressure) = Array.IndexOf(sData, sEQPrefix & gsSA_RESIN_INLET_PRESS) + 1
        If nScatteredAccessIndexes(eSAIndex.ResinInletPressure) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_RESIN_INLET_PRESS)
        nScatteredAccessIndexes(eSAIndex.ResinOutletPressure) = Array.IndexOf(sData, sEQPrefix & gsSA_RESIN_OUTLET_PRESS) + 1
        If nScatteredAccessIndexes(eSAIndex.ResinOutletPressure) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_RESIN_OUTLET_PRESS)
        nScatteredAccessIndexes(eSAIndex.HardenerInletPressure) = Array.IndexOf(sData, sEQPrefix & gsSA_HARDENER_INLET_PRESS) + 1
        If nScatteredAccessIndexes(eSAIndex.HardenerInletPressure) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_HARDENER_INLET_PRESS)
        nScatteredAccessIndexes(eSAIndex.HardenerOutletPressure) = Array.IndexOf(sData, sEQPrefix & gsSA_HARDENER_OUTLET_PRESS) + 1
        If nScatteredAccessIndexes(eSAIndex.HardenerOutletPressure) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_HARDENER_OUTLET_PRESS)

        'Canister stuff
        nScatteredAccessIndexes(eSAIndex.PaintInCan) = Array.IndexOf(sData, sEQPrefix & gsSA_PAINT_IN_CAN) + 1
        If nScatteredAccessIndexes(eSAIndex.PaintInCan) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_PAINT_IN_CAN)
        nScatteredAccessIndexes(eSAIndex.CanisterPos) = Array.IndexOf(sData, sEQPrefix & gsSA_CAN_POS) + 1
        If nScatteredAccessIndexes(eSAIndex.CanisterPos) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CAN_POS)
        nScatteredAccessIndexes(eSAIndex.CanisterTorque) = Array.IndexOf(sData, sEQPrefix & gsSA_CAN_TORQUE) + 1
        If nScatteredAccessIndexes(eSAIndex.CanisterTorque) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CAN_TORQUE)

        'Honda WB "S-Unit" dock devce  RJO 01/25/10
        nScatteredAccessIndexes(eSAIndex.SUnitPos) = Array.IndexOf(sData, sEQPrefix & gsSA_SUNIT_POS) + 1
        If nScatteredAccessIndexes(eSAIndex.SUnitPos) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_SUNIT_POS)
        nScatteredAccessIndexes(eSAIndex.SUnitTorque) = Array.IndexOf(sData, sEQPrefix & gsSA_SUNIT_FORCE) + 1
        If nScatteredAccessIndexes(eSAIndex.SUnitTorque) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_SUNIT_FORCE)
        nScatteredAccessIndexes(eSAIndex.SUnitPos) = Array.IndexOf(sData, sEQPrefix & gsSA_SUNIT_POS) + 1
        If nScatteredAccessIndexes(eSAIndex.SUnitPos) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_SUNIT_POS)

        'Atom/Turbine
        nScatteredAccessIndexes(eSAIndex.ReqBSAA) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_BELL_SPEED) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqBSAA) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ACTUAL_TURBINE_SPEED)
        nScatteredAccessIndexes(eSAIndex.ActBSAA) = Array.IndexOf(sData, sEQPrefix & gsSA_ACTUAL_TURBINE_SPEED) + 1
        If nScatteredAccessIndexes(eSAIndex.ActBSAA) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ACTUAL_TURBINE_SPEED)

        'Fan/Shape
        nScatteredAccessIndexes(eSAIndex.ReqSAFA) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_SHAPING_AIR) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqSAFA) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_REQUESTED_SHAPING_AIR)
        nScatteredAccessIndexes(eSAIndex.ActSAFA) = Array.IndexOf(sData, sEQPrefix & gsSA_ACTUAL_SHAPING_AIR) + 1
        If nScatteredAccessIndexes(eSAIndex.ActSAFA) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ACTUAL_SHAPING_AIR)

        ' 11/23/09  BTK     Added variables for dual shaping air.
        'Fan/Shape2
        nScatteredAccessIndexes(eSAIndex.ReqSAFA2) = Array.IndexOf(sData, sEQPrefix & gsSA_REQUESTED_SHAPING_AIR2) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqSAFA2) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_REQUESTED_SHAPING_AIR2)
        nScatteredAccessIndexes(eSAIndex.ActSAFA2) = Array.IndexOf(sData, sEQPrefix & gsSA_ACTUAL_SHAPING_AIR2) + 1
        If nScatteredAccessIndexes(eSAIndex.ActSAFA2) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ACTUAL_SHAPING_AIR2)

        'Estat Setpoint
        nScatteredAccessIndexes(eSAIndex.ReqES) = Array.IndexOf(sData, sEQPrefix & gsSA_ESTAT_REQUESTED_KV) + 1
        If nScatteredAccessIndexes(eSAIndex.ReqES) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ESTAT_ACTUAL_KV)

        'Estat KV
        nScatteredAccessIndexes(eSAIndex.ActKV) = Array.IndexOf(sData, sEQPrefix & gsSA_ESTAT_ACTUAL_KV) + 1
        If nScatteredAccessIndexes(eSAIndex.ActKV) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ESTAT_ACTUAL_KV)

        'Estat uA
        nScatteredAccessIndexes(eSAIndex.ActUA) = Array.IndexOf(sData, sEQPrefix & gsSA_ESTAT_ACTUAL_uA) + 1
        If nScatteredAccessIndexes(eSAIndex.ActUA) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ESTAT_ACTUAL_uA)

        'Preset Numbers
        nScatteredAccessIndexes(eSAIndex.PresetNum) = Array.IndexOf(sData, sEQPrefix & gsSA_FLUID_PRESET_NUM) + 1
        If nScatteredAccessIndexes(eSAIndex.PresetNum) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_FLUID_PRESET_NUM)
        nScatteredAccessIndexes(eSAIndex.EstatPresetNum) = Array.IndexOf(sData, sEQPrefix & gsSA_ESTAT_PRESET_NUMBER) + 1
        If nScatteredAccessIndexes(eSAIndex.EstatPresetNum) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_ESTAT_PRESET_NUMBER)

        'shared valve 1 TRIGGER
        nScatteredAccessIndexes(eSAIndex.Trigger_Shared0) = Array.IndexOf(sData, sEQPrefix & gsSA_TRIGGER) + 1
        If nScatteredAccessIndexes(eSAIndex.Trigger_Shared0) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_TRIGGER)

        'shared valve 2 Spare 1
        nScatteredAccessIndexes(eSAIndex.Shared1) = Array.IndexOf(sData, sEQPrefix & gsSA_SPARE_1) + 1
        If nScatteredAccessIndexes(eSAIndex.Shared1) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_SPARE_1)

        'shared valve 3 Hardner 
        nScatteredAccessIndexes(eSAIndex.HE_Shared2) = Array.IndexOf(sData, sEQPrefix & gsSA_HARDENER) + 1
        If nScatteredAccessIndexes(eSAIndex.HE_Shared2) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_HARDENER)

        'shared valve 4 Color Enable 
        nScatteredAccessIndexes(eSAIndex.CE_Shared3) = Array.IndexOf(sData, sEQPrefix & gsSA_COLOR_ENABLE) + 1
        If nScatteredAccessIndexes(eSAIndex.CE_Shared3) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_COLOR_ENABLE)

        'Color Valves
        nScatteredAccessIndexes(eSAIndex.CCValves1) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_VALVES) + 1
        If nScatteredAccessIndexes(eSAIndex.CCValves1) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_VALVES)
        nScatteredAccessIndexes(eSAIndex.CCValves2) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_VALVES2) + 1
        If nScatteredAccessIndexes(eSAIndex.CCValves2) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_VALVES2)

        'Bell startup
        nScatteredAccessIndexes(eSAIndex.BellStartupComplete) = Array.IndexOf(sData, sEQPrefix & gsSA_BELL_STARTUP_COMPLETE) + 1
        If nScatteredAccessIndexes(eSAIndex.BellStartupComplete) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_BELL_STARTUP_COMPLETE)

        'Pump Running
        nScatteredAccessIndexes(eSAIndex.P1Running) = Array.IndexOf(sData, sEQPrefix & gsSA_PUMP_1_RUNNING) + 1
        If nScatteredAccessIndexes(eSAIndex.P1Running) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_PUMP_1_RUNNING)
        nScatteredAccessIndexes(eSAIndex.P2Running) = Array.IndexOf(sData, sEQPrefix & gsSA_PUMP_2_RUNNING) + 1
        If nScatteredAccessIndexes(eSAIndex.P2Running) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_PUMP_2_RUNNING)

        Dim i As Integer
        'CC times
        For i = 1 To 10
            nScatteredAccessIndexes(eSAIndex.LastCCTime + i) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_TIME_PRE & i.ToString & gsSA_CC_TIME_POST) + 1
            If nScatteredAccessIndexes(eSAIndex.LastCCTime + i) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_TIME_PRE & i.ToString & gsSA_CC_TIME_POST)
        Next
        'Current cycle name
        nScatteredAccessIndexes(eSAIndex.CCCycleName) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_CYCLE_NAME) + 1
        If nScatteredAccessIndexes(eSAIndex.CCCycleName) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_CYCLE_NAME)

        'Current Color Change Status 'RJO 02/13/14
        nScatteredAccessIndexes(eSAIndex.CCStatus) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_STATUS) + 1
        If nScatteredAccessIndexes(eSAIndex.CCStatus) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_STATUS)

        'Current cycle number
        nScatteredAccessIndexes(eSAIndex.CCCycleNumber) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_CYCLE_NUMBER) + 1
        If nScatteredAccessIndexes(eSAIndex.CCCycleNumber) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_CYCLE_NUMBER)

        'Last cycle time
        nScatteredAccessIndexes(eSAIndex.LastCCTime) = Array.IndexOf(sData, sEQPrefix & gsSA_CC_CYCLE_TIME) + 1
        If nScatteredAccessIndexes(eSAIndex.LastCCTime) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CC_CYCLE_TIME)

        'Current color
        nScatteredAccessIndexes(eSAIndex.CurrentColor) = Array.IndexOf(sData, sEQPrefix & gsSA_CURRENT_COLOR) + 1
        If nScatteredAccessIndexes(eSAIndex.CurrentColor) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CURRENT_COLOR)

        'Current color name
        nScatteredAccessIndexes(eSAIndex.CurrentColorName) = Array.IndexOf(sData, sEQPrefix & gsSA_CURRENT_COLOR_NAME) + 1
        If nScatteredAccessIndexes(eSAIndex.CurrentColorName) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CURRENT_COLOR_NAME)

        'Current valve
        nScatteredAccessIndexes(eSAIndex.CurrentValve) = Array.IndexOf(sData, sEQPrefix & gsSA_CURRENT_VALVE) + 1
        If nScatteredAccessIndexes(eSAIndex.CurrentValve) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CURRENT_VALVE)

        'Current job
        nScatteredAccessIndexes(eSAIndex.CurrentJob) = Array.IndexOf(sData, sEQPrefix & gsSA_CURRENT_JOB) + 1
        If nScatteredAccessIndexes(eSAIndex.CurrentJob) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CURRENT_JOB)

        'Current path
        nScatteredAccessIndexes(eSAIndex.CurrentPath) = Array.IndexOf(sData, sEQPrefix & gsSA_CURRENT_PATH) + 1
        If nScatteredAccessIndexes(eSAIndex.CurrentPath) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CURRENT_PATH)

        'DQ Calibration variables
        nScatteredAccessIndexes(eSAIndex.DQCalActive) = Array.IndexOf(sData, sEQPrefix & gsSA_DQ_CAL_ACTIVE) + 1
        If nScatteredAccessIndexes(eSAIndex.DQCalActive) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_DQ_CAL_ACTIVE)

        nScatteredAccessIndexes(eSAIndex.DQOutput) = Array.IndexOf(sData, sEQPrefix & gsSA_DQ_OUTPUT) + 1
        If nScatteredAccessIndexes(eSAIndex.DQOutput) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_DQ_OUTPUT)

        nScatteredAccessIndexes(eSAIndex.DQCalStatus) = Array.IndexOf(sData, sEQPrefix & gsSA_DQ_CAL_STATUS) + 1
        If nScatteredAccessIndexes(eSAIndex.DQCalStatus) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_DQ_CAL_STATUS)

        ' 11/23/09  BTK     Added variables for dual shaping air.
        'DQ2 Calibration variables
        nScatteredAccessIndexes(eSAIndex.DQ2Output) = Array.IndexOf(sData, sEQPrefix & gsSA_DQ2_OUTPUT) + 1
        If nScatteredAccessIndexes(eSAIndex.DQ2Output) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_DQ2_OUTPUT)

        nScatteredAccessIndexes(eSAIndex.DQ2CalStatus) = Array.IndexOf(sData, sEQPrefix & gsSA_DQ2_CAL_STATUS) + 1
        If nScatteredAccessIndexes(eSAIndex.DQ2CalStatus) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_DQ2_CAL_STATUS)

        'IC scale cal
        nScatteredAccessIndexes(eSAIndex.ICCalActive) = Array.IndexOf(sData, sEQPrefix & gsSA_IC_CAL_ACTIVE) + 1
        If nScatteredAccessIndexes(eSAIndex.ICCalActive) = 0 Then
            Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_IC_CAL_ACTIVE)
            Debug.Print(" -" & sData(0) & "- ")
            Debug.Print(" -" & sEQPrefix & gsSA_IC_CAL_ACTIVE & "- ")
        End If
        nScatteredAccessIndexes(eSAIndex.ICCalStatus) = Array.IndexOf(sData, sEQPrefix & gsSA_IC_CAL_STATUS) + 1
        If nScatteredAccessIndexes(eSAIndex.ICCalStatus) = 0 Then
            Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_IC_CAL_STATUS)
        End If

        'IPC scale cal
        nScatteredAccessIndexes(eSAIndex.IPCCalActive) = Array.IndexOf(sData, sEQPrefix & gsSA_IPC_CAL_ACTIVE) + 1
        If nScatteredAccessIndexes(eSAIndex.IPCCalActive) = 0 Then
            Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_IPC_CAL_ACTIVE)
            Debug.Print(" -" & sData(0) & "- ")
            Debug.Print(" -" & sEQPrefix & gsSA_IPC_CAL_ACTIVE & "- ")
        End If

        'Honda S-Unit (Dock) cal Status
        nScatteredAccessIndexes(eSAIndex.SUnitCalStatus) = Array.IndexOf(sData, sEQPrefix & gsSA_SUNIT_CAL_STATUS) + 1
        If nScatteredAccessIndexes(eSAIndex.SUnitCalStatus) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_SUNIT_CAL_STATUS)

        'Line Status 'RJO 04/21/11
        nScatteredAccessIndexes(eSAIndex.PushedOut) = Array.IndexOf(sData, sEQPrefix & gsSA_PUSHED_OUT) + 1
        If nScatteredAccessIndexes(eSAIndex.PushedOut) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_PUSHED_OUT)

        nScatteredAccessIndexes(eSAIndex.CleanedOut) = Array.IndexOf(sData, sEQPrefix & gsSA_CLEANED_OUT) + 1
        If nScatteredAccessIndexes(eSAIndex.CleanedOut) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_CLEANED_OUT)

        nScatteredAccessIndexes(eSAIndex.Filled) = Array.IndexOf(sData, sEQPrefix & gsSA_FILLED) + 1
        If nScatteredAccessIndexes(eSAIndex.Filled) = 0 Then Debug.Print("Could not find SA Variables " & sEQPrefix & gsSA_FILLED)

    End Sub
    Friend Function sGetResTxtFrmRobotTxt(ByVal sText As String, _
                                          Optional ByRef rRobot As clsArm = Nothing, _
                                          Optional ByVal rCCType As eColorChangeType = eColorChangeType.NOT_SELECTED) As String
        '********************************************************************************************
        'Description: get text from resource file using text from the robot controller
        '               if there's nothing in the resource file, return the text from the robot controller
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/20/10  MSW     Add more punctuation replacement for 2K
        ' 01/06/12  MSW     Deal with some troubles adding in text from an old robot.
        '                   The GetString is case-dependent, but the resx is not.  add a hacked up workaround.
        '                   ex.  "rsRESERVED" is in, so you can't add "rsReserved"  
        ' 01/19/12  MSW     Allow direct passing of cc type instead of needing a robot object
        '********************************************************************************************
        sText = Replace(sText, " ", "_")
        sText = Replace(sText, "-", "_")
        sText = Replace(sText, "/", "_")
        sText = Replace(sText, "\", "_")
        Dim sTmpIn As String = "rs" & Trim(sText)

        Dim sTmpOut As String = grsRM.GetString(sTmpIn)
        If sTmpOut = Nothing Then
            sTmpIn = "rs" & Trim(sText.ToUpper)
            sTmpOut = grsRM.GetString(sTmpIn)
        End If
        If sTmpOut = String.Empty Then
            Debug.Print(sTmpIn)
            Return sTmpIn
        Else
            Dim nWB As Integer = InStr(sTmpOut, "#WB")
            Dim nIPC As Integer = InStr(sTmpOut, "#IPC")
            Dim n2K As Integer = InStr(sTmpOut, "#2K")
            Dim nDef As Integer = InStr(sTmpOut, "#DEF")
            Dim bWB As Boolean = False
            Dim b2K As Boolean = False
            Dim bIPC As Boolean = False
            If rCCType = eColorChangeType.NOT_SELECTED And rRobot IsNot Nothing Then
                rCCType = rRobot.ColorChangeType
            End If

            If rCCType <> eColorChangeType.NOT_SELECTED Then
                Select Case rCCType
                    Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
                         eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2_2K, _
                         eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN
                        b2K = True
                    Case eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL2, eColorChangeType.VERSABELL2_32, eColorChangeType.VERSABELL2_PLUS, _
                        eColorChangeType.VERSABELL3
                        bIPC = True
                    Case eColorChangeType.HONDA_WB, eColorChangeType.SERVOBELL, eColorChangeType.VERSABELL2_PLUS_WB, eColorChangeType.VERSABELL2_WB, _
                            eColorChangeType.VERSABELL3_WB, eColorChangeType.VERSABELL3_DUAL_WB
                        bWB = True
                End Select
            End If
            If nWB <> 0 Or nIPC <> 0 Or n2K <> 0 Or nDef <> 0 Then
                Dim nEnd As Integer = 0
                If bWB And (nWB > 0) Then
                    nEnd = InStr((nWB + 4), sTmpOut, "#")
                    If nEnd = 0 Then nEnd = sTmpOut.Length + 1
                    nWB = nWB + 3
                    sTmpOut = sTmpOut.Substring(nWB, (nEnd - nWB) - 1)
                ElseIf b2K And (n2K > 0) Then
                    nEnd = InStr((n2K + 4), sTmpOut, "#")
                    If nEnd = 0 Then nEnd = sTmpOut.Length + 1
                    n2K = n2K + 3
                    sTmpOut = sTmpOut.Substring(n2K, (nEnd - n2K) - 1)
                ElseIf bIPC And (nIPC > 0) Then
                    nEnd = InStr((nIPC + 5), sTmpOut, "#")
                    If nEnd = 0 Then nEnd = sTmpOut.Length + 1
                    nIPC = nIPC + 4
                    sTmpOut = sTmpOut.Substring(nIPC, (nEnd - nIPC) - 1)
                ElseIf (nDef > 0) Then
                    nEnd = InStr((nDef + 5), sTmpOut, "#")
                    If nEnd = 0 Then nEnd = sTmpOut.Length + 1
                    nDef = nDef + 4
                    sTmpOut = sTmpOut.Substring(nDef, (nEnd - nDef) - 1)
                End If
            End If
            Return sTmpOut
        End If
    End Function
    'Friend Function LoadColorChangeFromRobotnot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange, _
    '                                         Optional ByRef op As UpdateStatus = Nothing, _
    '                                         Optional ByVal ProgressMin As Integer = 0, _
    '                                            Optional ByVal ProgressMax As Integer = 100, _
    '                                            Optional ByRef sCycleName As String = "") As Boolean
    '    '********************************************************************************************
    '    'Description:  Load data stored on Robot
    '    'NRU 161103
    '    'Parameters: none
    '    'Returns:    True if load success
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    ' 12/22/11  MSW     Updates for speed
    '    '********************************************************************************************
    '    Dim oVar As FRRobot.FRCVar = Nothing
    '    'Dim sVar As String
    '    Dim nStep As Integer
    '    ' no error trap here - let calling routine catch it
    '    Dim sProgName As String = String.Empty

    '    Try

    '        rColorChange.PushOutVolume.Value = 0

    '    Catch ex As Exception
    '        ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, frmMain.msSCREEN_NAME, _
    '                            frmMain.Status, MessageBoxButtons.OK)
    '    End Try



    '    rColorChange.NumberOfCycles = 8

    '    For nCycle As Integer = 1 To rColorChange.NumberOfCycles

    '        With rColorChange.Cycle(nCycle)




    '            .NumberOfSteps = 1


    '            For nStep = 1 To .NumberOfSteps
    '                'assing a variable to the step structure

    '                .Steps(nStep).StepEvent.Value = 0


    '                .Steps(nStep).GoutState.Value = 0


    '                .Steps(nStep).GoutDC.Value = 0


    '                .Steps(nStep).DoutState.Value = 0

    '                .Steps(nStep).DoutDC.Value = 0


    '                .Steps(nStep).StepAction.Value = 0


    '                .Steps(nStep).Preset.Value = 0

    '                .Steps(nStep).Duration.Value = 0

    '            Next

    '        End With
    '    Next
    '    rColorChange.Update()
    '    Return True

    'End Function
End Module
