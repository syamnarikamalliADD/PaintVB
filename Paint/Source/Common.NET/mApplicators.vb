' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: mApplicators
'
' Description: Applicator config, labels, no PLC dependencies
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    05/29/09   MSW     made the class to support manual operation and process screens
'    11/05/09   BTK     clsApplicators_subInitCollection Don't add an applicator for 
'                       an opener.		
'    11/10/09  	MSW	    Add app types
'    11/23/09   BTK     Added Fan2 to eParamID ENUM.
'    02/15/10   BTK     Added color change type for Versabell2 32 valves
'    03/15/11   MSW     Add IPC cal support
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    01/03/12   MSW     Changes for speed - move applicator tables to XML             4.01.01.01
'    01/24/12   MSW     Applicator Updates                                            4.01.01.02
'    02/15/12   MSW     Debug fixes from the recent applicator updates                4.01.01.03
'                       subInitializeAppl - Fix index problems on action/events and cc cycle names
'    04/05/12   MSW     Handle reading num parms from robot w/ volume parm for WB CC  4.01.03.00
'    04/23/12   MSW     adjust indexing for volume preset                             4.01.03.01
'    12/13/12   MSW     subInitializeAppl - fix problems reading long names from XML  4.01.03.02
'    04/22/13   MSW     subInitializeAppl - ignore stupid robot setup with minumum>0  4.01.05.00
'    07/19/13   MSW     subInitializeAppl - move the last change to the right spot,   4.01.05.01  
'                       fix up some of the variables for 6.3-6.4
'    09/30/13   MSW     clsApplicator-subInitializeAppl                               4.01.05.02
'                       Trim spaces for action and event names
'                       subInitializeAppl - handle empty title from the robot by
'                       copying the name
'    10/11/13   MSW     add versabell 3 labels                                    4.01.06.00
'    11/12/13   BTK     Added VB3 Single Canister color change type               4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath

'********************************************************************************************
'Description: Applicators module
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Module mApplicators

#Region " Declares "

    Friend Enum eParamID
        Flow = 0
        Atom = 1
        Fan = 2
        Estat = 3
        Fan2 = 4
        Time = 5
    End Enum
    Friend Enum eCntrType
        'From padbcnst.kl
        '-- Special Control types
        APP_CNTR_NO = 1     '--None
        APP_CNTR_AA = 2     '-- AccuAir    - Closed Loop Air
        APP_CNTR_AF = 3     '-- AccuFlow   - Closed Loop Fluid, with meter
        APP_CNTR_AS = 4     '-- AccuStat   - Closed Loop Fluid, with AccuStat gun
        APP_CNTR_IPC = 5    '-- IPC        - Integral Pump Control
        APP_CNTR_BS = 6     '-- External Bell Turbine Speed
        APP_CNTR_SS = 7     '-- External Bell Shaping Speed
        APP_CNTR_CHP = 8    '-- AccuChop   - Closed Loop Resin and Glass
        APP_CNTR_TS = 9     '-- Turbine Speed Control (Internal Bell Turbine Speed)
        APP_CNTR_ES = 10    '-- FANUC Estat Controller
        APP_CNTR_DQ = 11    '--DQ
        'APP_CNTR_MIN = 1    '-- min and max values, for range checking
        'APP_CNTR_MAX = 12    '-- max. control type
    End Enum
    Friend Enum eOutType
        'From padbcnst.kl
        '-- Special Control types
        APP_CON_DIS = 1     '--discrete
        APP_CON_BIN = 2     '-- binary
        APP_CON_ANA = 3     '-- analog
        APP_CON_DIR = 4     '-- direct
        'APP_CON_MIN = 1    '-- min out type
        'APP_CON_MAX = 4    '-- max out type
    End Enum
    Friend Enum eCalSource
        'From padbcnst.kl
        '-- calibration source constants
        CAL_SRC_CAL = 0     '-- calibration sequence
        CAL_SRC_SCAL = 1     '-- scaled
        CAL_SRC_COP = 2     '-- copied
        CAL_SRC_NR = 3     '-- not required  (binary output type may use this)
        CAL_SRC_NA = 4     '-- not available
        CAL_SRC_BC = 5     '-- by color
        CAL_SRC_NO = 6     '-- none
        CAL_SRC_IPC_CAL = 100
        'CAL_SRC_MIN = 0     '-- min and max values for range checking
        'CAL_SRC_MAX = 6
    End Enum
    Friend Enum eCalStatus
        'From padbcnst.kl
        '-- calibration status constants
        APP_CAL_NA = 0       '-- not available
        APP_CAL_NR = 1       '-- not required
        APP_CAL_NC = 2       '-- not complete
        APP_CAL_C = 3       '-- complete
        'APP_CAL_MIN = 0     '-- min and max values for range checking
        'APP_CAL_MAX = 3
    End Enum
    Friend Enum eAFCalStatus
        'From paevaflo.kl
        '-- calibration status constants
        AF_NOT_CAL = 0
        AF_CAL_OK = 1
        AF_CANT_UPR = 2
        AF_CANT_LOWR = 3
        AF_ADPTO_BIT = 16
        AF_ADAPT_OUT = 17
        AF_UPPER_LIM = 18
        AF_LOWER_LIM = 19
        AF_CAL_ABORT = 255
        AF_CAL_COPY_BIT = 32
        AF_CAL_COPY = 33
        AF_COPY_ER = 49
    End Enum
    Friend Enum eDQCalStatus
        'From paevDQ.kl
        '-- calibration status constants
        CAL_DONE = 0       '-- complete
        CAL_NOT_DONE = 1       '-- not complete
    End Enum
#End Region
End Module 'mApplicators
'********************************************************************************************
'Description: Applicators collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsApplicators

    Inherits CollectionBase

#Region " Declares"

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsApplicators"
    '***** End Module Constants   **************************************************************

    '********** Event  Variables    *****************************************************************
    Friend Event BumpProgress()
    '********** End Event Variables *****************************************************************

#End Region
#Region " Properties"
    Default Friend Overloads Property Item(ByVal index As Integer) As clsApplicator
        '********************************************************************************************
        'Description: Get or set an applicator by its index
        '
        'Parameters: index
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsApplicator)
        End Get
        Set(ByVal Value As clsApplicator)
            List(index) = Value
        End Set
    End Property
    Default Friend Overloads Property Item(ByVal Name As String) As clsApplicator
        '********************************************************************************************
        'Description: Get or set an applicator by its name
        '
        'Parameters: Name
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            For Each O As clsApplicator In List
                If O.Name = Name Then
                    Return O
                    Exit For
                End If
            Next
            'opps
            Return Nothing

        End Get
        Set(ByVal Value As clsApplicator)

            For Each O As clsApplicator In List
                If O.Name = Name Then
                    O = Value
                End If
            Next

        End Set
    End Property
    Default Friend Overloads Property Item(ByVal CCType As eColorChangeType) As clsApplicator
        '********************************************************************************************
        'Description: Get or set an applicator by its color change type
        '
        'Parameters: Name
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            For Each O As clsApplicator In List
                If O.ColorChangeType = CCType Then
                    Return O
                    Exit For
                End If
            Next
            'opps
            Return Nothing

        End Get
        Set(ByVal Value As clsApplicator)

            For Each O As clsApplicator In List
                If O.ColorChangeType = CCType Then
                    O = Value
                End If
            Next

        End Set
    End Property
#End Region
#Region " Routines "

    Friend Function Add(ByVal value As clsApplicator) As Integer
        '********************************************************************************************
        'Description: Add an appicator to collection
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
    Friend Function Contains(ByVal value As clsApplicator) As Boolean
        '********************************************************************************************
        'Description: If value is not of type clsApplicator, this will return false.
        '
        'Parameters: clsApplParam
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.Contains(value)
    End Function 'Contains
    Friend Function IndexOf(ByVal value As clsApplicator) As Integer
        '********************************************************************************************
        'Description: Get Index of applicator parameter
        '
        'Parameters: clsApplParam
        'Returns:     index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return List.IndexOf(value)
    End Function 'IndexOf
    Friend Sub Insert(ByVal index As Integer, ByVal value As clsApplicator)
        '********************************************************************************************
        'Description: Add an applicator at specific location
        '
        'Parameters: position,clsApplParam
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Insert(index, value)
    End Sub 'Insert
    Friend Sub Remove(ByVal value As clsApplicator)
        '********************************************************************************************
        'Description: Remove an applicator 
        '
        'Parameters: clsApplParam
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        List.Remove(value)

    End Sub 'Remove
    Private Sub subInitCollection(ByRef ArmCollection As clsArms, ByRef Zone As clsZone, Optional ByVal bReadFromRobot As Boolean = False)
        '*****************************************************************************************
        'Description: Applicator parameter data from the DB table
        '
        'Parameters:  Applicator parameter table DB name
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/05/09  BTK     Don't add an applicator for an opener.
        ' 03/31/10  RJO     Don't add an applicator for a painter that's not online because you 
        '                   won't get the configuration data from the controller.
        '*****************************************************************************************
        Dim Applicator As clsApplicator
        Dim bAdd As Boolean

        Try
            For Each o As clsArm In ArmCollection
                'If o.IsOnLine Then '03/31/10 RJO
                bAdd = (o.ColorChangeType <> eColorChangeType.NONE)
                For Each o2 As clsApplicator In List
                    If o2.ColorChangeType = o.ColorChangeType Then
                        bAdd = False
                    End If
                Next
                If o.IsOpener Then bAdd = False
                If bAdd Then
                    Applicator = New clsApplicator(o, Zone, bReadFromRobot)
                    List.Add(Applicator)
                End If
                'End If 'o.IsOnLine
            Next
        Catch ex As Exception
            Trace.WriteLine("Module: clsApplicators, Routine: subInitCollection, Error: " & ex.Message)
            Trace.WriteLine("Module: clsApplicators, Routine: subInitCollection, StackTrace: " & ex.StackTrace)
        End Try

    End Sub


#End Region
#Region " Events "

    Friend Sub New(ByRef ArmCollection As clsArms, ByRef Zone As clsZone, Optional ByVal bReadFromRobot As Boolean = False)
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
        subInitCollection(ArmCollection, Zone, bReadFromRobot)
    End Sub


#End Region

    Private Sub clsApplicators_BumpProgress() Handles Me.BumpProgress

    End Sub
End Class 'Applicators class

'********************************************************************************************
'Description: Applicator class
'           Configuration of applicator parameters for a specific color change type.
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsApplicator

#Region " Declares"

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsApplicator"
    '***** End Module Constants   **************************************************************

    '********** Structures *****************************************************************
    Public Structure tApplParam
        Dim sParmTitle As String             'Parameter name
        Dim sParmName As String             'Parameter name
        Dim sParmNameCAP As String             'Parameter name
        Dim sUnits As String                'Setpoint units
        Dim nParmNum As Integer             'Parm number 
        Dim bUseCounts As Boolean           'Enable counts for manual flow test, cal table
        Dim bForceInteger As Boolean        'Force integer setpoint (estat steps)
        Dim fMinEngUnits As Single          'Max setpoint in engineering units
        Dim fMaxEngUnits As Single          'Min setpoint in engineering units
        Dim nMinCount As Integer            'Min counts
        Dim nMaxCount As Integer            'Max counts
        Dim fBellEnabMinEngUnits As Single  'Max setpoint with bell enabled
        Dim fBellEnabMaxEngUnits As Single  'Min setpoint with bell enabled
        Dim nBellEnabMinCount As Integer    'Min counts with bell enabled
        Dim nBellEnabMaxCount As Integer    'Max counts with bell enabled
        Dim CntrType As eCntrType          'Control type
        Dim CalSource As eCalSource        'Cal source
        Dim OutType As eOutType             'Output type
    End Structure
    '********** End Structures *****************************************************************

    '********** Variables *****************************************************************
    Private msApplName As String        'Applicator name
    Private msApplNameLong As String        'Applicator name - longer if needed
    Private meColorChangeType As eColorChangeType
    Private mnNumParms As Integer     'Typically 4
    Private mtParams() As tApplParam  'Parameter setup
    Private msApplParamDB As String
    Private mnDQFeedbackkOffset As Integer    'DQ psi = (AIN[DQ] - mnDQFeedbackkOffset) * mfDQFeedbackkScale
    Private mfDQFeedbackkScale As Single      '
    Private mnKVFeedbackOffset As Integer    'KV psi = (AIN[KV] - mnKVFeedbackOffset) * mfKVFeedbackScale
    Private mfKVFeedbackScale As Single      '
    Private mnUAFeedbackOffset As Integer    'uA = (AIN[uA] - mnUAFeedbackOffset) * mfUAFeedbackScale
    Private mfUAFeedbackScale As Single      '
    Private mbEnabBellFeatures As Boolean
    Private mnNumCycles As Integer
    Private msCCCycles() As String
    'Private nNumSATags As Integer     'Number of feedback tags in scattered access
    'Private sSATag() As String        'Scattered access tags
    'Private sSALabel() As String      'Labels for scattegered access tags
    Private msCCActions() As String = Nothing
    Private msCCEvents() As String = Nothing
    Private mnNumCCActions As Integer
    Private mnNumCCEvents As Integer
    Private mnNumSharedValves As Integer
    Private mnNumGroupValves As Integer
    Private msSharedValves() As String = Nothing
    Private msGroupValves() As String = Nothing
    Private msSharedValvesCAP() As String = Nothing
    Private msGroupValvesCAP() As String = Nothing
    '********** End Variables *****************************************************************

#End Region
#Region " Properties"
    Friend Property NumGroupValves() As Integer
        '********************************************************************************************
        'Description: Number of Group Valves
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumGroupValves
        End Get
        Set(ByVal value As Integer)
            mnNumGroupValves = value
        End Set
    End Property
    Friend Property GroupValves() As String()
        '********************************************************************************************
        'Description: Group Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msGroupValves
        End Get
        Set(ByVal value As String())
            msGroupValves = value
            mnNumGroupValves = msGroupValves.GetUpperBound(0)
        End Set
    End Property

    Friend Property GroupValve(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  Group Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msGroupValves.GetUpperBound(0) Then
                Return msGroupValves(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msGroupValves.GetUpperBound(0) Then
                msGroupValves(index) = value
            End If
        End Set
    End Property
    Friend Property GroupValvesCAP() As String()
        '********************************************************************************************
        'Description: Group Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msGroupValvesCAP
        End Get
        Set(ByVal value As String())
            msGroupValvesCAP = value
            mnNumGroupValves = msGroupValvesCAP.GetUpperBound(0)
        End Set
    End Property

    Friend Property GroupValveCAP(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  Group Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msGroupValvesCAP.GetUpperBound(0) Then
                Return msGroupValvesCAP(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msGroupValvesCAP.GetUpperBound(0) Then
                msGroupValvesCAP(index) = value
            End If
        End Set
    End Property
    Friend Property NumSharedValves() As Integer
        '********************************************************************************************
        'Description: Number of Shared Valves
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumSharedValves
        End Get
        Set(ByVal value As Integer)
            mnNumSharedValves = value
        End Set
    End Property
    Friend Property SharedValves() As String()
        '********************************************************************************************
        'Description: Shared Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msSharedValves
        End Get
        Set(ByVal value As String())
            msSharedValves = value
            mnNumSharedValves = msSharedValves.GetUpperBound(0)
        End Set
    End Property

    Friend Property SharedValve(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  Shared Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msSharedValves.GetUpperBound(0) Then
                Return msSharedValves(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msSharedValves.GetUpperBound(0) Then
                msSharedValves(index) = value
            End If
        End Set
    End Property
    Friend Property SharedValvesCAP() As String()
        '********************************************************************************************
        'Description: Shared Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msSharedValvesCAP
        End Get
        Set(ByVal value As String())
            msSharedValvesCAP = value
            mnNumSharedValves = msSharedValvesCAP.GetUpperBound(0)
        End Set
    End Property

    Friend Property SharedValveCAP(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  Shared Valves Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msSharedValvesCAP.GetUpperBound(0) Then
                Return msSharedValvesCAP(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msSharedValvesCAP.GetUpperBound(0) Then
                msSharedValvesCAP(index) = value
            End If
        End Set
    End Property




    Friend Property Name() As String
        '********************************************************************************************
        'Description:  Applicator name for display
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msApplName
        End Get
        Set(ByVal value As String)
            msApplName = value
        End Set
    End Property
    Friend Property NumCCCycles() As Integer
        '********************************************************************************************
        'Description: Number of CC Cycles
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumCycles
        End Get
        Set(ByVal value As Integer)
            mnNumCycles = value
        End Set
    End Property
    Friend Property CCCycleNames() As String()
        '********************************************************************************************
        'Description: CC Action Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msCCCycles
        End Get
        Set(ByVal value As String())
            msCCCycles = value
            mnNumCycles = msCCCycles.GetUpperBound(0)
        End Set
    End Property

    Friend Property CCCycleName(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  CC Action Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/30/12  MSW     Fix an indexing problem
        '********************************************************************************************
        Get
            If index > 0 And index <= (msCCCycles.GetUpperBound(0) + 1) Then
                Return msCCCycles(index - 1)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index > 0 And index <= (msCCCycles.GetUpperBound(0) + 1) Then
                msCCCycles(index - 1) = value
            End If
        End Set
    End Property
    Friend Property NumCCActions() As Integer
        '********************************************************************************************
        'Description: Number of CC Actions
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumCCActions
        End Get
        Set(ByVal value As Integer)
            mnNumCCActions = value
        End Set
    End Property
    Friend Property CCActions() As String()
        '********************************************************************************************
        'Description: CC Action Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msCCActions
        End Get
        Set(ByVal value As String())
            msCCActions = value
            mnNumCCActions = msCCActions.GetUpperBound(0)
        End Set
    End Property

    Friend Property CCAction(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  CC Action Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msCCActions.GetUpperBound(0) Then
                Return msCCActions(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msCCActions.GetUpperBound(0) Then
                msCCActions(index) = value
            End If
        End Set
    End Property
    Friend Property NumnCCEvents() As Integer
        '********************************************************************************************
        'Description: Number of CC Events
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumCCEvents
        End Get
        Set(ByVal value As Integer)
            mnNumCCEvents = value
        End Set
    End Property
    Friend Property CCEvents() As String()
        '********************************************************************************************
        'Description: CC Event Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msCCEvents
        End Get
        Set(ByVal value As String())
            msCCEvents = value
            mnNumCCEvents = msCCEvents.GetUpperBound(0)
        End Set
    End Property

    Friend Property CCEvent(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  CC Event Names
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index >= 0 And index <= msCCEvents.GetUpperBound(0) Then
                Return msCCEvents(index)
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            If index >= 0 And index <= msCCEvents.GetUpperBound(0) Then
                msCCEvents(index) = value
            End If
        End Set
    End Property

    Friend Property DQOffset() As Integer
        '********************************************************************************************
        'Description: offset to subtract from DQ feedback AIN before scaling
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnDQFeedbackkOffset
        End Get
        Set(ByVal value As Integer)
            mnDQFeedbackkOffset = value
        End Set
    End Property
    Friend Property DQScale() As Single
        '********************************************************************************************
        'Description: DQ feedback scale ain (minus offset) to psi
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfDQFeedbackkScale
        End Get
        Set(ByVal value As Single)
            mfDQFeedbackkScale = value
        End Set
    End Property
    Friend Property KVOffset() As Integer
        '********************************************************************************************
        'Description: offset to subtract from KV feedback AIN before scaling
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnKVFeedbackOffset
        End Get
        Set(ByVal value As Integer)
            mnKVFeedbackOffset = value
        End Set
    End Property
    Friend Property KVScale() As Single
        '********************************************************************************************
        'Description: KV feedback scale ain (minus offset) to KV
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfKVFeedbackScale
        End Get
        Set(ByVal value As Single)
            mfKVFeedbackScale = value
        End Set
    End Property
    Friend Property uAOffset() As Integer
        '********************************************************************************************
        'Description: offset to subtract from DQ feedback AIN before scaling
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnUAFeedbackOffset
        End Get
        Set(ByVal value As Integer)
            mnUAFeedbackOffset = value
        End Set
    End Property
    Friend Property uAScale() As Single
        '********************************************************************************************
        'Description: uA feedback scale ain (minus offset) to uA
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfUAFeedbackScale
        End Get
        Set(ByVal value As Single)
            mfUAFeedbackScale = value
        End Set
    End Property

    Friend Property LongName() As String
        '********************************************************************************************
        'Description:  Applicator name for display
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msApplNameLong
        End Get
        Set(ByVal value As String)
            msApplNameLong = value
        End Set
    End Property
    Friend Property ColorChangeType() As eColorChangeType
        '********************************************************************************************
        'Description:  parameter number on robot
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return meColorChangeType
        End Get
        Set(ByVal value As eColorChangeType)
            meColorChangeType = value
        End Set
    End Property
    Friend Property NumParms() As Integer
        '********************************************************************************************
        'Description:  pnumber of fluid parameters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumParms
        End Get
        Set(ByVal value As Integer)
            mnNumParms = value
        End Set
    End Property
    Friend ReadOnly Property BellApplicator() As Boolean
        '********************************************************************************************
        'Description:  True for bell applicators
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnabBellFeatures
        End Get
    End Property
    Friend Property CalSource(ByVal index As Integer) As eCalSource
        '********************************************************************************************
        'Description:  cal source for parameter(index)        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).CalSource
        End Get
        Set(ByVal value As eCalSource)
            mtParams(index).CalSource = value
        End Set
    End Property
    Friend Property CntrType(ByVal index As Integer) As eCntrType
        '********************************************************************************************
        'Description:  cal source for parameter(index)        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).CntrType
        End Get
        Set(ByVal value As eCntrType)
            mtParams(index).CntrType = value
        End Set
    End Property
    Friend Property MinCount(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  min counts for parameter(index)        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).nMinCount
        End Get
        Set(ByVal value As Integer)
            mtParams(index).nMinCount = value
        End Set
    End Property
    Friend Property MaxCount(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  max counts for parameter(index)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).nMaxCount
        End Get
        Set(ByVal value As Integer)
            mtParams(index).nMaxCount = value
        End Set
    End Property
    Friend Property MinEngUnit(ByVal index As Integer) As Single
        '********************************************************************************************
        'Description:  min eng. units for parameter(index)        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).fMinEngUnits
        End Get
        Set(ByVal value As Single)
            mtParams(index).fMinEngUnits = value
        End Set
    End Property
    Friend Property MaxEngUnit(ByVal index As Integer) As Single
        '********************************************************************************************
        'Description:  max eng. units for parameter(index)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).fMaxEngUnits
        End Get
        Set(ByVal value As Single)
            mtParams(index).fMaxEngUnits = value
        End Set
    End Property
    Friend ReadOnly Property SelectedMin(ByVal index As Integer, ByVal bEngUnits As Boolean, ByVal bBeakerMode As Boolean) As Single
        '********************************************************************************************
        'Description:  Get minimum value based on parameters
        '
        'Parameters: bEngUnits - True =  get min in engineering unites, false = get min in counts
        '            bBeakerMode - False = get min with bell spin enabled, true = get min in beaker mode
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If bEngUnits Then
                If Not (bBeakerMode) And mbEnabBellFeatures Then
                    Return mtParams(index).fBellEnabMinEngUnits
                Else
                    Return mtParams(index).fMinEngUnits
                End If
            Else
                If Not (bBeakerMode) And mbEnabBellFeatures Then
                    Return CSng(mtParams(index).nBellEnabMinCount)
                Else
                    Return CSng(mtParams(index).nMinCount)
                End If
            End If
        End Get
    End Property
    Friend ReadOnly Property SelectedMax(ByVal index As Integer, ByVal bEngUnits As Boolean, ByVal bBeakerMode As Boolean) As Single
        '********************************************************************************************
        'Description:  Get maximum value based on parameters
        '
        'Parameters: bEngUnits - True =  get max in engineering unites, false = get max in counts
        '            bBeakerMode - False = get max with bell spin enabled, true = get max in beaker mode
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If bEngUnits Then
                If Not (bBeakerMode) Then
                    Return mtParams(index).fBellEnabMaxEngUnits
                Else
                    Return mtParams(index).fMaxEngUnits
                End If
            Else
                If Not (bBeakerMode) Then
                    Return CSng(mtParams(index).nBellEnabMaxCount)
                Else
                    Return CSng(mtParams(index).nMaxCount)
                End If
            End If
        End Get
    End Property
    Friend Property MinCountBellEnab(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  min counts for parameter(index) with the bell enabled    '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).nBellEnabMinCount
        End Get
        Set(ByVal value As Integer)
            mtParams(index).nBellEnabMinCount = value
        End Set
    End Property
    Friend Property MaxCountBellEnab(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  max counts for parameter(index) with the bell enabled
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).nBellEnabMaxCount
        End Get
        Set(ByVal value As Integer)
            mtParams(index).nBellEnabMaxCount = value
        End Set
    End Property
    Friend Property MinEngUnitBellEnab(ByVal index As Integer) As Single
        '********************************************************************************************
        'Description:  min eng. units for parameter(index) with the bell enabled        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).fBellEnabMinEngUnits
        End Get
        Set(ByVal value As Single)
            mtParams(index).fBellEnabMinEngUnits = value
        End Set
    End Property
    Friend Property MaxEngUnitBellEnab(ByVal index As Integer) As Single
        '********************************************************************************************
        'Description:  max eng. units for parameter(index) with the bell enabled
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).fBellEnabMaxEngUnits
        End Get
        Set(ByVal value As Single)
            mtParams(index).fBellEnabMaxEngUnits = value
        End Set
    End Property

    Friend Property UseCounts(ByVal index As Integer) As Boolean
        '********************************************************************************************
        'Description:  enable counts on a manual flow test
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).bUseCounts
        End Get
        Set(ByVal value As Boolean)
            mtParams(index).bUseCounts = value
        End Set
    End Property
    Friend Property ForceInteger(ByVal index As Integer) As Boolean
        '********************************************************************************************
        'Description:  force an integer value on a manual flow test
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).bForceInteger
        End Get
        Set(ByVal value As Boolean)
            mtParams(index).bForceInteger = value
        End Set
    End Property
    Friend ReadOnly Property ParamName(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  parameter name read from robobts.resx
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sParmName
        End Get
    End Property
    Friend ReadOnly Property ParamNameCAP(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  parameter name read from robobts.resx
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sParmNameCAP
        End Get
    End Property
    Friend ReadOnly Property ParamTitle(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  parameter name read from robobts.resx
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sParmTitle
        End Get
    End Property

    Friend ReadOnly Property ParamUnits(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  parameter units read from robobts.resx
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sUnits
        End Get
    End Property
    Friend ReadOnly Property FlowTestLabel(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  return a label with units for the flow test box
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sParmTitle & " (" & mtParams(index).sUnits & ")"
        End Get
    End Property
    Friend ReadOnly Property FlowTestCountsLabel(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  return a label with units for the flow test box
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mtParams(index).sParmTitle & " (" & grsRM.GetString("rsCOUNTS") & ")"
        End Get
    End Property
#End Region
#Region " Routines"

    Private Sub subInitializeAppl(ByVal CCType As eColorChangeType, ByRef Zone As clsZone, _
                                  Optional ByRef Arm As clsArm = Nothing, Optional ByVal bReadFromRobot As Boolean = False)
        '********************************************************************************************
        'Description:   Initialize an applicator for a ColorChangeType,
        '               open the applicator specific table to get  each ApplParam
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW	    Add app types
        ' 04/01/10  msw     Fix where fBellEnabMinEngUnits was calculated.  .fMaxEngUnits and  .fMinEngUnits were swapped
        ' 03/15/11  MSW     Add IPC cal support
        ' 01/03/12  MSW     Changes for speed - move applicator tables to XML
        ' 01/19/12  MSW     Take all the info from the robot and write to the DB, give the option to skip the read from the robot
        ' 02/01/12  MSW     Debug fixes from the recent applicator updates                4.01.01.03
        '                   subInitializeAppl - Fix index problems on action/events and cc cycle names
        ' 04/23/12  MSW     adjust indexing for volume
        ' 12/13/12  MSW     subInitializeAppl - fix problems reading long names from XML
        ' 04/22/13  MSW     subInitializeAppl - ignore stupid robot setup with minumum > 0
        ' 09/17/13  MSW     Trim spaces for action and event names
        ' 09/30/13  MSW     subInitializeAppl - handle empty title from the robot by copying the name
        '********************************************************************************************
        Try
            meColorChangeType = CCType
            msApplName = GetCCTypeName(meColorChangeType)
            msApplNameLong = GetCCTypeLongName(meColorChangeType)
        Catch ex As Exception
            Dim sTmp As String = String.Empty
            ShowErrorMessagebox(gcsRM.GetString("csERRO"), ex, msMODULE, _
            sTmp, MessageBoxButtons.OK)
            Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - name, Error: " & ex.Message)
            Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - name, StackTrace: " & ex.StackTrace)
        End Try

        Dim oCCTypeMainNode As XmlNode = Nothing
        Dim oCCTypeNode As XmlNode = Nothing
        Dim oCCTypeNodeList As XmlNodeList = Nothing
        Dim oCCTypeXMLDoc As New XmlDocument
        Dim sCCTypeXMLFilePath As String = String.Empty

        'Get data table for parameter setup:
        Try
            If GetDefaultFilePath(sCCTypeXMLFilePath, eDir.XML, String.Empty, gsCCTYPE_XMLTABLE & ".XML") Then
                oCCTypeXMLDoc.Load(sCCTypeXMLFilePath)
                oCCTypeMainNode = oCCTypeXMLDoc.SelectSingleNode("//" & gsCCTYPE_XMLTABLE)
                oCCTypeNodeList = oCCTypeMainNode.SelectNodes("//" & gsCCTYPE_XMLNODE & "[" & gsCCTYPE_COL_ID & "='" & CInt(meColorChangeType).ToString & "']")
                Try
                    If oCCTypeNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", sCCTypeXMLFilePath & " not found.")
                    Else
                        oCCTypeNode = oCCTypeNodeList(0)
                        Try
                            msApplParamDB = oCCTypeNodeList(0).Item(gsCCTYPE_COL_APPL_TABLE).InnerXml
                            mnDQFeedbackkOffset = CType(oCCTypeNodeList(0).Item(gsCCTYPE_DQ_OFFSET).InnerXml, Integer)
                            mfDQFeedbackkScale = CType(oCCTypeNodeList(0).Item(gsCCTYPE_DQ_SCALE).InnerXml, Single)
                            mnKVFeedbackOffset = CType(oCCTypeNodeList(0).Item(gsCCTYPE_KV_OFFSET).InnerXml, Integer)
                            mfKVFeedbackScale = CType(oCCTypeNodeList(0).Item(gsCCTYPE_KV_SCALE).InnerXml, Single)
                            mnUAFeedbackOffset = CType(oCCTypeNodeList(0).Item(gsCCTYPE_UA_OFFSET).InnerXml, Integer)
                            mfUAFeedbackScale = CType(oCCTypeNodeList(0).Item(gsCCTYPE_UA_SCALE).InnerXml, Single)
                        Catch ex As Exception
                            ShowErrorMessagebox(sCCTypeXMLFilePath & "  : Invalid Data", ex, "Module: clsApplicator, Routine: subInitializeAppl", String.Empty)
                        End Try
                        Try
                            mbEnabBellFeatures = CType(oCCTypeNodeList(0).Item(gsCCTYPE_ENABLE_BELL).InnerXml, Boolean)
                        Catch ex As Exception
                            mbEnabBellFeatures = False
                            ShowErrorMessagebox(sCCTypeXMLFilePath & "  : Invalid " & gsCCTYPE_ENABLE_BELL, ex, "clsApplicator:subInitializeAppl", String.Empty)
                        End Try
                        Try
                            mnNumCycles = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_CYCLES).InnerXml, Integer)
                        Catch ex As Exception
                            mnNumCycles = 10
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msCCCycles = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_CYCLE_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            mnNumCycles = 10
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            mnNumCCActions = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_ACTIONS).InnerXml, Integer)
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msCCActions = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_ACTION_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            mnNumCCEvents = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_EVENTS).InnerXml, Integer)
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msCCEvents = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_EVENT_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            mnNumSharedValves = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_SHARED_VALVES).InnerXml, Integer)
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msSharedValves = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_SHARED_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msSharedValvesCAP = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_SHARED_NAMES_CAP).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            mnNumGroupValves = CType(oCCTypeNodeList(0).Item(gsCCTYPE_NUM_GROUP_VALVES).InnerXml, Integer)
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msGroupValves = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_GROUP_NAMES).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                        Try
                            msGroupValvesCAP = Split(CType(oCCTypeNodeList(0).Item(gsCCTYPE_GROUP_NAMES_CAP).InnerXml, String), ";")
                        Catch ex As Exception
                            bReadFromRobot = (Arm IsNot Nothing)
                        End Try
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XML Data: " & sCCTypeXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XPath syntax: " & sCCTypeXMLFilePath & " - " & ex.Message)
        End Try

        Dim oParmMainNode As XmlNode = Nothing
        Dim oParmNode As XmlNode = Nothing
        Dim oAppl As XmlNode = Nothing
        Dim oParmNodeList As XmlNodeList = Nothing
        Dim oApplList As XmlNodeList = Nothing
        Dim oParmXMLDoc As New XmlDocument
        Dim sParmXMLFilePath As String = String.Empty
        'Open parameter list table
        Try
            If GetDefaultFilePath(sParmXMLFilePath, eDir.XML, String.Empty, gsAPPL_PARM_XMLTABLE & ".XML") Then
                oParmXMLDoc.Load(sParmXMLFilePath)
                oParmMainNode = oParmXMLDoc.SelectSingleNode("//" & gsAPPL_PARM_XMLTABLE)
                oApplList = oParmMainNode.ChildNodes
                For Each oNode As XmlNode In oApplList
                    If oNode.Name = msApplParamDB Then
                        oAppl = oNode
                    End If
                Next
                ' 12/13/12  MSW     subInitializeAppl - fix problems reading long names from XML
                'SelectNodes stopped working for this case.  Don't know why, but we'll just search through it
                oParmNodeList = oAppl.ChildNodes 'oParmMainNode.SelectNodes("//" & msApplParamDB & "//" & gsAPPL_PARM_XMLNODE)
                Try
                    If oParmNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", sParmXMLFilePath & ":" & msApplParamDB & " not found.")
                    Else
                        mnNumParms = oParmNodeList.Count
                        ReDim mtParams(mnNumParms)
                        For nNode As Integer = 0 To mnNumParms - 1
                            With oParmNodeList(nNode)

                                Dim nParm As Integer = CType(.Item(gsAPPLPARM_PARMNUM).InnerXml, Integer)
                                mtParams(nParm - 1).nParmNum = nParm
                                Try
                                    mtParams(nParm - 1).sParmName = .Item(gsAPPLPARM_PARMNAME).InnerXml
                                Catch ex As Exception
                                    mtParams(nParm - 1).sParmName = String.Empty
                                End Try
                                Try
                                    mtParams(nParm - 1).sParmNameCAP = .Item(gsAPPLPARM_PARMNAMECAP).InnerXml
                                Catch ex As Exception
                                    mtParams(nParm - 1).sParmNameCAP = String.Empty
                                End Try
                                Try
                                    mtParams(nParm - 1).sParmTitle = .Item(gsAPPLPARM_PARMTITLE).InnerXml
                                Catch ex As Exception
                                    mtParams(nParm - 1).sParmTitle = String.Empty
                                End Try
                                'Trim() isn't really needed, but it handles small typos in the database.
                                If mtParams(nParm - 1).sParmName Is Nothing Then
                                    mtParams(nParm - 1).sParmName = String.Empty
                                End If
                                If mtParams(nParm - 1).sParmNameCAP Is Nothing Then
                                    mtParams(nParm - 1).sParmNameCAP = String.Empty
                                End If
                                If mtParams(nParm - 1).sParmTitle Is Nothing Then
                                    mtParams(nParm - 1).sParmTitle = String.Empty
                                End If
                                mtParams(nParm - 1).sUnits = .Item(gsAPPLPARM_UNITS).InnerXml
                                If mtParams(nParm - 1).sUnits Is Nothing Then
                                    mtParams(nParm - 1).sUnits = String.Empty
                                End If
                                mtParams(nParm - 1).bUseCounts = CType(.Item(gsAPPLPARM_USE_COUNTS).InnerXml, Boolean)
                                mtParams(nParm - 1).bForceInteger = CType(.Item(gsAPPLPARM_FORCE_INT).InnerXml, Boolean)
                                mtParams(nParm - 1).nMinCount = CType(.Item(gsAPPLPARM_MIN_CNT).InnerXml, Integer)
                                mtParams(nParm - 1).nMaxCount = CType(.Item(gsAPPLPARM_MAX_CNT).InnerXml, Integer)
                                mtParams(nParm - 1).fMinEngUnits = CType(.Item(gsAPPLPARM_MIN_ENG).InnerXml, Single)
                                mtParams(nParm - 1).fMaxEngUnits = CType(.Item(gsAPPLPARM_MAX_ENG).InnerXml, Single)
                                mtParams(nParm - 1).nBellEnabMinCount = CType(.Item(gsAPPLPARM_MIN_CNT_BE).InnerXml, Integer)
                                mtParams(nParm - 1).nBellEnabMaxCount = CType(.Item(gsAPPLPARM_MAX_CNT_BE).InnerXml, Integer)
                                mtParams(nParm - 1).fBellEnabMinEngUnits = CType(.Item(gsAPPLPARM_MIN_ENG_BE).InnerXml, Single)
                                mtParams(nParm - 1).fBellEnabMaxEngUnits = CType(.Item(gsAPPLPARM_MAX_ENG_BE).InnerXml, Single)
                                Try
                                    mtParams(nParm - 1).CntrType = CType(.Item(gsAPPLPARM_CNTR_TYPE).InnerXml, eCntrType)
                                Catch ex As Exception
                                    mtParams(nParm - 1).CntrType = eCntrType.APP_CNTR_NO
                                    bReadFromRobot = (Arm IsNot Nothing)
                                End Try
                                Try
                                    mtParams(nParm - 1).CalSource = CType(.Item(gsAPPLPARM_CAL_SOURCE).InnerXml, eCalSource)
                                Catch ex As Exception
                                    mtParams(nParm - 1).CalSource = eCalSource.CAL_SRC_NO
                                    bReadFromRobot = (Arm IsNot Nothing)
                                End Try
                                Try
                                    mtParams(nParm - 1).OutType = CType(.Item(gsAPPLPARM_OUT_TYPE).InnerXml, eOutType)
                                Catch ex As Exception
                                    mtParams(nParm - 1).OutType = eOutType.APP_CON_ANA
                                    bReadFromRobot = (Arm IsNot Nothing)
                                End Try
                            End With
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XML Data: " & sParmXMLFilePath & ":" & msApplParamDB & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XPath syntax: " & sParmXMLFilePath & ":" & msApplParamDB & " - " & ex.Message)
        End Try

        If bReadFromRobot AndAlso Not (Arm Is Nothing) AndAlso Arm.IsOnLine Then
            'Get details from a robot if possible
            Dim bResave As Boolean = True ' we won't save if there's a read error
            Try ' Parm Details
                Dim sName As String
                Dim nParmNum As Integer
                Dim sParmNum As String
                Dim sPrefix As String
                Dim nParmOffset As Integer
                Const nMAX_PARM_ROBOT As Integer = 6 'Offset halfway into parm array
                Dim sTmp As String

                Try
                    'Handle dual shaping air setup when reading from robot
                    Dim nTmpNumParm As Integer = oAppl.ChildNodes.Count
                    If (Arm.Controller.Version >= 7.3) Then
                        Arm.ProgramName = "PAPARMDB"
                        Arm.VariableName = "num_con_parm"
                        nTmpNumParm = CInt(Arm.VarValue)
                        'Handle WB extra column for volume preset
                        'PAVROPTN	byOptn[1].byOp[101]
                        Arm.ProgramName = "PAVROPTN"
                        If Arm.Controller.Version >= 7.3 Then
                            sName = "byOptn[" & Arm.ArmNumber.ToString & "].byOp[101]"
                        Else
                            sName = "byOp[101]"
                        End If
                        Arm.VariableName = sName
                        mnNumParms = nTmpNumParm + CInt(Arm.VarValue)
                    ElseIf (Arm.Controller.Version >= 6.3) Then
                        'if dual arm - PAVROPTN	byOp[75]
                        'By the way, if you're bombing out here on an old (version<6.3) robot, congratulations.
                        'You're the first one.  
                        Arm.ProgramName = "PAVROPTN"
                        Arm.VariableName = "byOp[75]"
                        Dim nTmp As Integer = CInt(Arm.VarValue)
                        If nTmp > 0 Then
                            'Dual Arm  - The variables aren't as clear
                            Arm.ProgramName = "PAPARMDB"
                            Arm.VariableName = "num_prm_msk"
                            nTmp = CInt(Arm.VarValue)
                            nTmpNumParm = 1
                            While (((nBitMask(nTmpNumParm + 1) And nTmp)) > 0)
                                nTmpNumParm += 1
                            End While
                        Else
                            'Not dual arm
                            Arm.ProgramName = "PAPARMDB"
                            Arm.VariableName = "num_con_parm"
                            nTmpNumParm = CInt(Arm.VarValue)
                        End If
                    Else
                        'Old controller
                        Arm.ProgramName = "PAPARMDB"
                        Arm.VariableName = "num_con_parm"
                        nTmpNumParm = CInt(Arm.VarValue)
                    End If

                    ReDim Preserve mtParams(mnNumParms)

                Catch ex As Exception

                End Try

                Arm.ProgramName = "PAVRPARM"
                If Arm.Controller.Version >= 7.3 Then
                    sPrefix = "TPARMEQ[" & Arm.ArmNumber.ToString & "]."
                    nParmOffset = 0
                Else
                    sPrefix = ""
                    nParmOffset = (Arm.ArmNumber - 1) * nMAX_PARM_ROBOT
                End If
                For nParmNum = 0 To mnNumParms
                    sParmNum = CStr(nParmNum + 1 + nParmOffset)
                    With mtParams(nParmNum)
                        .nParmNum = nParmNum + 1
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_CON_MIN"
                        Arm.VariableName = sName
                        .fMinEngUnits = CSng(Arm.VarValue)
                        'MSW 7/18/13 ignore stupid robot setup
                        If .fMinEngUnits > 0 Then
                            .fMinEngUnits = 0
                        End If
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_MIN_OUT"
                        Arm.VariableName = sName
                        .nMinCount = CInt(Arm.VarValue)
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_CON_MAX"
                        Arm.VariableName = sName
                        .fMaxEngUnits = CSng(Arm.VarValue)
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_MAX_OUT"
                        Arm.VariableName = sName
                        .nMaxCount = CInt(Arm.VarValue)

                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_CON_TYPE"
                        Arm.VariableName = sName
                        .OutType = CType(CInt(Arm.VarValue), eOutType)
                        'Min bell speed values - use database unless it's outside the range from robot
                        If (.nBellEnabMinCount < .nMinCount) Or (.nBellEnabMinCount > .nMaxCount) Then
                            If mbEnabBellFeatures And (nParmNum = eParamID.Atom Or nParmNum = eParamID.Fan) Then
                                .nBellEnabMinCount = CInt(.nMinCount + 0.25 * (.nMaxCount - .nMinCount))
                            Else
                                .nBellEnabMinCount = .nMinCount
                            End If
                        End If
                        If (.fBellEnabMinEngUnits < .fMinEngUnits) Or (.fBellEnabMinEngUnits > .fMaxEngUnits) Then
                            If mbEnabBellFeatures And (nParmNum = eParamID.Atom Or nParmNum = eParamID.Fan) Then
                                .fBellEnabMinEngUnits = CInt(.fMinEngUnits + 0.25 * (.fMaxEngUnits - .fMinEngUnits))
                            Else
                                .fBellEnabMinEngUnits = .fMinEngUnits
                            End If
                        End If
                        'Just read the max w/ bell from the robot for now.
                        'If (.nBellEnabMaxCount < .nMinCount) Or (.nBellEnabMaxCount > .nMaxCount) Then
                        .nBellEnabMaxCount = .nMaxCount
                        'End If
                        'If (.fBellEnabMaxEngUnits < .fMaxEngUnits) Or (.fBellEnabMaxEngUnits > .fMaxEngUnits) Then
                        .fBellEnabMaxEngUnits = .fMaxEngUnits
                        'End If

                        'Units and name - May need to take these out and use the database for multilanguage
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].CAL_UNITS"
                        Arm.VariableName = sName
                        sTmp = Arm.VarValue
                        'Special case - The robot lists "NA" for estat step units.  We'll keep "Step" or whatever it translates to.
                        If sTmp <> "NA" And sTmp <> String.Empty Then
                            .sUnits = sGetResTxtFrmRobotTxt(sTmp, , CCType)
                        End If

                        'Parm name
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_CON_NAM"
                        Arm.VariableName = sName
                        sTmp = Arm.VarValue
                        If sTmp <> String.Empty Then
                            .sParmName = sGetResTxtFrmRobotTxt(sTmp, , CCType)
                        End If
                        If sTmp <> String.Empty Then
                            .sParmNameCAP = sGetResTxtFrmRobotTxt(sTmp & "_CAP", , CCType)
                        End If

                        'Parm title
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].APP_CON_TITL"
                        Arm.VariableName = sName
                        sTmp = Arm.VarValue
                        ' 09/30/13  MSW     subInitializeAppl - handle empty title from the robot by copying the name
                        If sTmp = String.Empty Then
                            .sParmTitle = .sParmName
                        Else
                            If sTmp.StartsWith("Eq") Then
                                sTmp = sTmp.Substring(4)
                            Else
                                'The robots parm names don't always have the same format. This handles the typical setup
                                If sTmp.Contains("#" & Arm.ArmNumber.ToString) Then
                                    sTmp = sTmp.Substring(0, sTmp.Length - 3)
                                Else
                                    If sTmp = "Fluid Flow2" Then
                                        sTmp = "Fluid Flow"
                                    End If
                                End If
                            End If

                            If .sParmTitle = String.Empty And sTmp <> String.Empty Then
                                .sParmTitle = sGetResTxtFrmRobotTxt(sTmp, , CCType)
                            End If
                        End If

                        'Calibration
                        sName = sPrefix & "CON_PRM_DESC[" & sParmNum & "].CAL_SOURCE"
                        Arm.VariableName = sName
                        .CalSource = CType(Arm.VarValue, eCalSource)
                        sName = sPrefix & "CON_PRM_DES3[" & sParmNum & "].APP_CNTR_TYP"
                        Arm.VariableName = sName
                        .CntrType = CType(Arm.VarValue, eCntrType)
                        'Use cal source to enable cal table
                        Select Case .CalSource
                            Case eCalSource.CAL_SRC_BC, eCalSource.CAL_SRC_CAL, eCalSource.CAL_SRC_COP, eCalSource.CAL_SRC_SCAL
                                Select Case .OutType
                                    Case eOutType.APP_CON_ANA, eOutType.APP_CON_BIN
                                        .bUseCounts = True
                                    Case eOutType.APP_CON_DIR
                                        'Special case for IPC cal
                                        'Adding a couple checks to verify IPC cal feature, otherwise default to no cal
                                        Try
                                            If Arm.Controller.Version >= 6.3 Then
                                                Arm.ProgramName = "pavroptn"
                                                Dim sPrefix2 As String = String.Empty
                                                If Arm.Controller.Version >= 7.3 Then
                                                    sPrefix2 = "BYOPTN[" & Arm.ArmNumber.ToString & "]."
                                                Else
                                                    sPrefix2 = ""
                                                End If
                                                'MSW 9/14/11 - change from IPC to 2k
                                                '!rob_ipc_use = 40
                                                '!rob_2ksp_use = 62
                                                'Arm.VariableName = sPrefix2 & "BYOP[40]"
                                                Arm.VariableName = sPrefix2 & "BYOP[62]"
                                                Dim nVal As Integer = CInt(Arm.VarValue)
                                                If nVal = 1 Then
                                                    .bUseCounts = True
                                                    'I don't know why he didn't set the variable to cal by color, but he didn't, so we'll make our own value to use on this screen.
                                                    .CalSource = eCalSource.CAL_SRC_IPC_CAL
                                                Else
                                                    .bUseCounts = False
                                                End If
                                            Else
                                                .bUseCounts = False
                                            End If
                                            Arm.ProgramName = "PAVRPARM"
                                        Catch ex As Exception
                                            .bUseCounts = False
                                        End Try
                                    Case Else
                                        .bUseCounts = False
                                End Select
                            Case eCalSource.CAL_SRC_NA, eCalSource.CAL_SRC_NO, eCalSource.CAL_SRC_NR
                                .bUseCounts = False
                            Case Else
                                .bUseCounts = False
                        End Select
                    End With
                Next
            Catch ex As Exception
                Dim sTmp As String = String.Empty
                ShowErrorMessagebox(gcsRM.GetString("csERRO"), ex, msMODULE, _
                sTmp, MessageBoxButtons.OK)
                Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - Read Parm Details from robot, Error: " & ex.Message)
                Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - Read Parm Details from robot, StackTrace: " & ex.StackTrace)
                bResave = False
            End Try

            'CC Cycle names
            Dim oVar As FRRobot.FRCVar = Nothing
            Dim oProgVars As FRRobot.FRCVars = Nothing
            Try
                Dim oo As FRRobot.FRCVars
                If Arm.Controller.Version > 6.3 Then
                    Arm.ProgramName = "PAVRCCEX"
                    oProgVars = Arm.ProgramVars
                    Dim ooo As FRRobot.FRCVars = DirectCast(oProgVars.Item("TCCEXCMOS"), FRRobot.FRCVars)
                    oo = DirectCast(ooo.Item("CC_CYC_NAM"), FRRobot.FRCVars)
                Else
                    Arm.ProgramName = "PAVRCCIO"
                    oProgVars = Arm.ProgramVars
                    oo = DirectCast(oProgVars.Item("CC_CYC_NAM"), FRRobot.FRCVars)
                End If
                oo.NoRefresh = True
                mnNumCycles = oo.Count
                ReDim msCCCycles(mnNumCycles - 1)
                For nCycle As Integer = 0 To oo.Count - 1
                    oVar = DirectCast(oo.Item(, nCycle), FRRobot.FRCVar)
                    If oVar.IsInitialized Then
                        Dim sTmpStr As String = oVar.Value.ToString
                        If sTmpStr = String.Empty Then
                            mnNumCycles = nCycle
                            ReDim Preserve msCCCycles(mnNumCycles)
                            Exit For
                        End If
                        msCCCycles(nCycle) = sGetResTxtFrmRobotTxt(sTmpStr, , CCType)
                    Else
                        mnNumCycles = nCycle
                        ReDim Preserve msCCCycles(mnNumCycles)
                        Exit For
                    End If
                Next
                oo.NoRefresh = False
            Catch ex As Exception
                Dim sTmp As String = String.Empty
                ShowErrorMessagebox(gcsRM.GetString("csERRO"), ex, msMODULE, _
                sTmp, MessageBoxButtons.OK)
                Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - CC Cycle names from robot, Error: " & ex.Message)
                Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - CC Cycle names from robot, StackTrace: " & ex.StackTrace)
                bResave = False
            End Try

            'CC Actions and events
            Dim oActionArray As FRRobot.FRCVars = Nothing
            Dim oEventArray As FRRobot.FRCVars = Nothing
            Dim i As Integer
            Try
                If Arm.IsOnLine Then
                    If Arm.Controller.Version > 6.3 Then
                        Arm.ProgramName = "PAVRCCEX"
                        oProgVars = Arm.ProgramVars
                        Dim oStructure As FRRobot.FRCVars = DirectCast(oProgVars.Item("TCCEXCMOS"), FRRobot.FRCVars)
                        oActionArray = DirectCast(oStructure.Item("ACTION_NAM"), FRRobot.FRCVars)
                        oEventArray = DirectCast(oStructure.Item("EVENT_NAM"), FRRobot.FRCVars)
                    Else
                        Arm.ProgramName = "PAVRCCIO"
                        oProgVars = Arm.ProgramVars
                        oActionArray = DirectCast(oProgVars.Item("ACTION_NAM"), FRRobot.FRCVars)
                        oEventArray = DirectCast(oProgVars.Item("EVENT_NAM"), FRRobot.FRCVars)
                    End If

                    ReDim msCCActions(oActionArray.Count + 1)
                    ReDim msCCEvents(oEventArray.Count + 1)
                    msCCActions(0) = grsRM.GetString("rsNO_ACTION")
                    msCCEvents(0) = grsRM.GetString("rsNO_EVENT")
                    ' 09/30/13  MSW     Trim spaces for action and event names
                    For i = 1 To oActionArray.Count
                        oVar = DirectCast(oActionArray.Item(, i - 1), FRRobot.FRCVar)
                        If oVar.IsInitialized AndAlso (oVar.Value.ToString.Trim <> String.Empty) Then
                            msCCActions(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString.Trim, , CCType)
                        Else
                            msCCActions(i) = String.Empty
                            mnNumCCActions = i
                            ReDim Preserve msCCActions(i - 1)
                            Exit For
                        End If
                    Next
                    For i = 1 To oEventArray.Count
                        oVar = DirectCast(oEventArray.Item(, i - 1), FRRobot.FRCVar)
                        If oVar.IsInitialized AndAlso (oVar.Value.ToString.Trim <> String.Empty) Then
                            msCCEvents(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString.Trim, , CCType)
                        Else
                            msCCEvents(i) = String.Empty
                            mnNumCCEvents = i
                            ReDim Preserve msCCEvents(i - 1)
                            Exit For
                        End If
                    Next

                End If
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
            End Try
            mnNumCCActions = msCCActions.GetUpperBound(0)
            mnNumCCEvents = msCCEvents.GetUpperBound(0)


            'CC Valves
            Dim oSharedArray As FRRobot.FRCVars = Nothing
            Dim oGroupArray As FRRobot.FRCVars = Nothing

            Try
                If Arm.IsOnLine Then
                    If Arm.Controller.Version > 6.3 Then
                        Arm.ProgramName = "PAVRCCEX"
                        oProgVars = Arm.ProgramVars
                        Dim oStructure As FRRobot.FRCVars = DirectCast(oProgVars.Item("TCCEXCMOS"), FRRobot.FRCVars)
                        oSharedArray = DirectCast(oStructure.Item("DVALVE_NAM"), FRRobot.FRCVars)
                        oGroupArray = DirectCast(oStructure.Item("GVALVE_NAM"), FRRobot.FRCVars)
                    Else
                        Arm.ProgramName = "PAVRCCIO"
                        oProgVars = Arm.ProgramVars
                        oSharedArray = DirectCast(oProgVars.Item("DVALVE_NAM"), FRRobot.FRCVars)
                        oGroupArray = DirectCast(oProgVars.Item("GVALVE_NAM"), FRRobot.FRCVars)
                    End If
                    mnNumSharedValves = oSharedArray.Count
                    mnNumGroupValves = oGroupArray.Count
                    ReDim msSharedValves(mnNumSharedValves - 1)
                    ReDim msGroupValves(mnNumGroupValves - 1)
                    ReDim msSharedValvesCAP(mnNumSharedValves - 1)
                    ReDim msGroupValvesCAP(mnNumGroupValves - 1)
                    For i = 0 To mnNumSharedValves - 1
                        oVar = DirectCast(oSharedArray.Item(, i), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            msSharedValves(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString, , CCType)
                            msSharedValvesCAP(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString & "_CAP", , CCType)
                        Else
                            msSharedValves(i) = String.Empty
                            msSharedValvesCAP(i) = String.Empty
                        End If
                    Next
                    For i = 0 To mnNumGroupValves - 1
                        oVar = DirectCast(oGroupArray.Item(, i), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            msGroupValves(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString, , CCType)
                            msGroupValvesCAP(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString & "_CAP", , CCType)
                        Else
                            msGroupValves(i) = String.Empty
                            msGroupValvesCAP(i) = String.Empty
                        End If
                    Next

                End If
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
            End Try

            If bResave Then
                'Save changes from robot to CC type DB
                If oCCTypeNode IsNot Nothing Then
                    Try
                        oCCTypeNode = oCCTypeNodeList(0) 'Should only be one match!!!
                        If oCCTypeNode.Item(gsCCTYPE_COL_APPL_TABLE) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_COL_APPL_TABLE, Nothing)
                            oNewNode.InnerXml = msApplParamDB
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_COL_APPL_TABLE).InnerXml = msApplParamDB
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_DQ_OFFSET) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_DQ_OFFSET, Nothing)
                            oNewNode.InnerXml = mnDQFeedbackkOffset.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_DQ_OFFSET).InnerXml = mnDQFeedbackkOffset.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_DQ_SCALE) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_DQ_SCALE, Nothing)
                            oNewNode.InnerXml = mfDQFeedbackkScale.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_DQ_SCALE).InnerXml = mfDQFeedbackkScale.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_KV_OFFSET) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_KV_OFFSET, Nothing)
                            oNewNode.InnerXml = mnKVFeedbackOffset.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_KV_OFFSET).InnerXml = mnKVFeedbackOffset.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_KV_SCALE) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_KV_SCALE, Nothing)
                            oNewNode.InnerXml = mfKVFeedbackScale.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_KV_SCALE).InnerXml = mfKVFeedbackScale.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_UA_OFFSET) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_UA_OFFSET, Nothing)
                            oNewNode.InnerXml = mnUAFeedbackOffset.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_UA_OFFSET).InnerXml = mnUAFeedbackOffset.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_UA_SCALE) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_UA_SCALE, Nothing)
                            oNewNode.InnerXml = mfUAFeedbackScale.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_UA_SCALE).InnerXml = mfUAFeedbackScale.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_ENABLE_BELL) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_ENABLE_BELL, Nothing)
                            oNewNode.InnerXml = mbEnabBellFeatures.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_ENABLE_BELL).InnerXml = mbEnabBellFeatures.ToString
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_NUM_CYCLES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_NUM_CYCLES, Nothing)
                            oNewNode.InnerXml = mnNumCycles.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_NUM_CYCLES).InnerXml = mnNumCycles.ToString
                        End If
                        Dim sTmp As String = String.Empty
                        If mnNumCycles > 0 Then
                            For nItem As Integer = 0 To mnNumCycles - 1
                                sTmp = sTmp & msCCCycles(nItem)
                                If nItem <> mnNumCycles Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_CYCLE_NAMES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_CYCLE_NAMES, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_CYCLE_NAMES).InnerXml = sTmp
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_NUM_ACTIONS) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_NUM_ACTIONS, Nothing)
                            oNewNode.InnerXml = mnNumCCActions.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_NUM_ACTIONS).InnerXml = mnNumCCActions.ToString
                        End If
                        sTmp = String.Empty
                        If mnNumCCActions > 0 Then
                            For nItem As Integer = 0 To mnNumCCActions
                                sTmp = sTmp & msCCActions(nItem)
                                If nItem <> mnNumCCActions Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_ACTION_NAMES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_ACTION_NAMES, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_ACTION_NAMES).InnerXml = sTmp
                        End If


                        If oCCTypeNode.Item(gsCCTYPE_NUM_EVENTS) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_NUM_EVENTS, Nothing)
                            oNewNode.InnerXml = mnNumCCEvents.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_NUM_EVENTS).InnerXml = mnNumCCEvents.ToString
                        End If
                        sTmp = String.Empty
                        If mnNumCCEvents > 0 Then
                            For nItem As Integer = 0 To mnNumCCEvents
                                sTmp = sTmp & msCCEvents(nItem)
                                If nItem <> mnNumCCEvents Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_EVENT_NAMES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_EVENT_NAMES, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_EVENT_NAMES).InnerXml = sTmp
                        End If

                        If oCCTypeNode.Item(gsCCTYPE_NUM_SHARED_VALVES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_NUM_SHARED_VALVES, Nothing)
                            oNewNode.InnerXml = mnNumSharedValves.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_NUM_SHARED_VALVES).InnerXml = mnNumSharedValves.ToString
                        End If
                        sTmp = String.Empty
                        If mnNumSharedValves > 0 Then
                            For nItem As Integer = 1 To mnNumSharedValves
                                sTmp = sTmp & msSharedValves(nItem - 1)
                                If nItem <> mnNumSharedValves Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_SHARED_NAMES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_SHARED_NAMES, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_SHARED_NAMES).InnerXml = sTmp
                        End If
                        sTmp = String.Empty
                        If mnNumSharedValves > 0 Then
                            For nItem As Integer = 1 To mnNumSharedValves
                                sTmp = sTmp & msSharedValvesCAP(nItem - 1)
                                If nItem <> mnNumSharedValves Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_SHARED_NAMES_CAP) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_SHARED_NAMES_CAP, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_SHARED_NAMES_CAP).InnerXml = sTmp
                        End If

                        If oCCTypeNode.Item(gsCCTYPE_NUM_GROUP_VALVES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_NUM_GROUP_VALVES, Nothing)
                            oNewNode.InnerXml = mnNumGroupValves.ToString
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_NUM_GROUP_VALVES).InnerXml = mnNumGroupValves.ToString
                        End If
                        sTmp = String.Empty
                        If mnNumGroupValves > 0 Then
                            For nItem As Integer = 1 To mnNumGroupValves
                                sTmp = sTmp & msGroupValves(nItem - 1)
                                If nItem <> mnNumGroupValves Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_GROUP_NAMES) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_GROUP_NAMES, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_GROUP_NAMES).InnerXml = sTmp
                        End If
                        sTmp = String.Empty
                        If mnNumGroupValves > 0 Then
                            For nItem As Integer = 1 To mnNumGroupValves
                                sTmp = sTmp & msGroupValvesCAP(nItem - 1)
                                If nItem <> mnNumGroupValves Then
                                    sTmp = sTmp & ";"
                                End If
                            Next
                        End If
                        If oCCTypeNode.Item(gsCCTYPE_GROUP_NAMES_CAP) Is Nothing Then
                            Dim oNewNode As XmlNode = oCCTypeXMLDoc.CreateNode(XmlNodeType.Element, gsCCTYPE_GROUP_NAMES_CAP, Nothing)
                            oNewNode.InnerXml = sTmp
                            oCCTypeNode.AppendChild(oNewNode)
                        Else
                            oCCTypeNode.Item(gsCCTYPE_GROUP_NAMES_CAP).InnerXml = sTmp
                        End If

                        Dim oIOStream As System.IO.StreamWriter = New System.IO.StreamWriter(sCCTypeXMLFilePath)
                        Dim oWriter As XmlTextWriter = New XmlTextWriter(oIOStream)
                        oWriter.Formatting = Formatting.Indented
                        oCCTypeXMLDoc.WriteTo(oWriter)
                        oWriter.Close()
                        oIOStream.Close()

                    Catch ex As Exception
                        Dim sTmp As String = String.Empty
                        ShowErrorMessagebox(gcsRM.GetString("csERRO"), ex, msMODULE, _
                        sTmp, MessageBoxButtons.OK)
                        Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - Save robot data to DB, Error: " & ex.Message)
                        Trace.WriteLine("Module: clsApplicator, Routine: subInitializeAppl - Save robot data to DB, StackTrace: " & ex.StackTrace)
                        bResave = False
                    End Try
                End If
            End If

            'Save changes from robot to param DB
            'Dim oParmMainNode As XmlNode = Nothing
            'Dim oParmNode As XmlNode = Nothing
            'Dim oParmNodeList As XmlNodeList = Nothing
            'Dim oParmXMLDoc As New XmlDocument
            'Dim sParmXMLFilePath As String = String.Empty
            oApplList = oParmMainNode.ChildNodes
            For Each oNode As XmlNode In oApplList
                If oNode.Name = msApplParamDB Then
                    oAppl = oNode
                End If
            Next
            ' 12/13/12  MSW     subInitializeAppl - fix problems reading long names from XML
            'SelectNodes stopped working for this case.  Don't know why, but we'll just search through it
            If oAppl.ChildNodes.Count > mnNumParms Then
                oAppl.RemoveChild(oAppl.ChildNodes(oAppl.ChildNodes.Count - 1))
            End If
            oParmNodeList = oAppl.ChildNodes 'oParmMainNode.SelectNodes("//" & msApplParamDB & "//" & gsAPPL_PARM_XMLNODE)
            Try
                If oParmNodeList.Count = 0 Then
                    mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", sParmXMLFilePath & ":" & msApplParamDB & " not found.")
                Else
                    For nNode As Integer = 0 To mnNumParms - 1
                        If nNode >= oParmNodeList.Count Then
                            Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPL_PARM_XMLNODE, Nothing)
                            oAppl.AppendChild(oNewNode)
                            oParmNodeList = oAppl.ChildNodes
                        End If
                        With oParmNodeList(nNode)

                            If .Item(gsAPPLPARM_PARMNUM) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_PARMNUM, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).nParmNum.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_PARMNUM).InnerXml = mtParams(nNode).nParmNum.ToString
                            End If
                            If .Item(gsAPPLPARM_PARMNAME) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_PARMNAME, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).sParmName
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_PARMNAME).InnerXml = mtParams(nNode).sParmName
                            End If
                            If .Item(gsAPPLPARM_PARMNAMECAP) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_PARMNAMECAP, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).sParmNameCAP
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_PARMNAMECAP).InnerXml = mtParams(nNode).sParmNameCAP
                            End If
                            If .Item(gsAPPLPARM_PARMTITLE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_PARMTITLE, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).sParmTitle
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_PARMTITLE).InnerXml = mtParams(nNode).sParmTitle
                            End If
                            If .Item(gsAPPLPARM_UNITS) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_UNITS, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).sUnits
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_UNITS).InnerXml = mtParams(nNode).sUnits
                            End If
                            If .Item(gsAPPLPARM_USE_COUNTS) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_USE_COUNTS, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).bUseCounts.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_USE_COUNTS).InnerXml = mtParams(nNode).bUseCounts.ToString
                            End If
                            If .Item(gsAPPLPARM_FORCE_INT) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_FORCE_INT, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).bForceInteger.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_FORCE_INT).InnerXml = mtParams(nNode).bForceInteger.ToString
                            End If
                            If .Item(gsAPPLPARM_MIN_CNT) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MIN_CNT, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).nMinCount.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MIN_CNT).InnerXml = mtParams(nNode).nMinCount.ToString
                            End If
                            If .Item(gsAPPLPARM_MAX_CNT) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MAX_CNT, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).nMaxCount.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MAX_CNT).InnerXml = mtParams(nNode).nMaxCount.ToString
                            End If
                            If .Item(gsAPPLPARM_MIN_ENG) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MIN_ENG, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).fMinEngUnits.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MIN_ENG).InnerXml = mtParams(nNode).fMinEngUnits.ToString
                            End If
                            If .Item(gsAPPLPARM_MAX_ENG) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MAX_ENG, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).fMaxEngUnits.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MAX_ENG).InnerXml = mtParams(nNode).fMaxEngUnits.ToString
                            End If

                            If .Item(gsAPPLPARM_MIN_CNT_BE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MIN_CNT_BE, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).nBellEnabMinCount.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MIN_CNT_BE).InnerXml = mtParams(nNode).nBellEnabMinCount.ToString
                            End If
                            If .Item(gsAPPLPARM_MAX_CNT_BE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MAX_CNT_BE, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).nBellEnabMaxCount.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MAX_CNT_BE).InnerXml = mtParams(nNode).nBellEnabMaxCount.ToString
                            End If

                            If .Item(gsAPPLPARM_MIN_ENG_BE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MIN_ENG_BE, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).fBellEnabMinEngUnits.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MIN_ENG_BE).InnerXml = mtParams(nNode).fBellEnabMinEngUnits.ToString
                            End If
                            If .Item(gsAPPLPARM_MAX_ENG_BE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_MAX_ENG_BE, Nothing)
                                oNewNode.InnerXml = mtParams(nNode).fBellEnabMaxEngUnits.ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_MAX_ENG_BE).InnerXml = mtParams(nNode).fBellEnabMaxEngUnits.ToString
                            End If

                            If .Item(gsAPPLPARM_CNTR_TYPE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_CNTR_TYPE, Nothing)
                                oNewNode.InnerXml = CInt(mtParams(nNode).CntrType).ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_CNTR_TYPE).InnerXml = CInt(mtParams(nNode).CntrType).ToString
                            End If

                            If .Item(gsAPPLPARM_CAL_SOURCE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_CAL_SOURCE, Nothing)
                                oNewNode.InnerXml = CInt(mtParams(nNode).CalSource).ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_CAL_SOURCE).InnerXml = CInt(mtParams(nNode).CalSource).ToString
                            End If

                            If .Item(gsAPPLPARM_OUT_TYPE) Is Nothing Then
                                Dim oNewNode As XmlNode = oParmXMLDoc.CreateNode(XmlNodeType.Element, gsAPPLPARM_OUT_TYPE, Nothing)
                                oNewNode.InnerXml = CInt(mtParams(nNode).OutType).ToString
                                .AppendChild(oNewNode)
                            Else
                                .Item(gsAPPLPARM_OUT_TYPE).InnerXml = CInt(mtParams(nNode).OutType).ToString
                            End If
                        End With
                    Next
                    Dim oIOStream As System.IO.StreamWriter = New System.IO.StreamWriter(sParmXMLFilePath)
                    Dim oWriter As XmlTextWriter = New XmlTextWriter(oIOStream)
                    oWriter.Formatting = Formatting.Indented
                    oParmXMLDoc.WriteTo(oWriter)
                    oWriter.Close()
                    oIOStream.Close()
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog("Module: clsApplicator, Routine: subInitializeAppl", "Invalid XML Data: " & sParmXMLFilePath & ":" & msApplParamDB & " - " & ex.Message)
            End Try
        End If '        If bReadFromRobot AndAlso Not (Arm Is Nothing) AndAlso Arm.IsOnLine Then
    End Sub
    Public Function GetCCTypeName(ByVal CCType As eColorChangeType) As String
        '********************************************************************************************
        'Description:   Return the name for a cc type
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW	    Add app types
        ' 11/12/13  BTK     Added VB3 Single Canister
        '********************************************************************************************
        Dim sApplName As String = ""
        Select Case (CCType)
            Case eColorChangeType.SINGLE_PURGE
                sApplName = grsRM.GetString("rsGUN")
            Case eColorChangeType.DUAL_PURGE
                sApplName = grsRM.GetString("rsGUN")
            Case eColorChangeType.MODIFIED_DUAL_PURGE
                sApplName = grsRM.GetString("rsGUN")
            Case eColorChangeType.GUN_2K
                sApplName = grsRM.GetString("rsGUN")
            Case eColorChangeType.GUN_2K_PIG
                sApplName = grsRM.GetString("rsGUN_2K_PIG")
            Case eColorChangeType.GUN_2K_Mica   'JZ 12062016 - Piggable stack only.
                sApplName = grsRM.GetString("rsGUN_2K_MICA")
            Case eColorChangeType.SINGLE_PURGE_BELL
                sApplName = grsRM.GetString("rsBELL")
            Case eColorChangeType.BELL_2K
                sApplName = grsRM.GetString("rsBELL")
            Case eColorChangeType.ACCUSTAT
                sApplName = grsRM.GetString("rsACCUSTAT")
            Case eColorChangeType.AQUABELL
                sApplName = grsRM.GetString("rsAQUABELL")
            Case eColorChangeType.SERVOBELL
                sApplName = grsRM.GetString("rsSERVOBELL")
            Case eColorChangeType.VERSABELL
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_WB
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_2K
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.POWDER
                sApplName = grsRM.GetString("rsPOWDER")
            Case eColorChangeType.VERSABELL_2K
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_2K_1PLUS1
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_2K_MULTIRESIN
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_PLUS
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_PLUS_WB
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2_WB
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.HONDA_1K
                sApplName = grsRM.GetString("rsHONDA_1K")
            Case eColorChangeType.HONDA_WB
                sApplName = grsRM.GetString("rsHONDA_WB")
            Case eColorChangeType.VERSABELL3_DUAL_WB
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL3_WB
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL3
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL3P1000 'RJO 07/16/14
                sApplName = grsRM.GetString("rsVERSABELL")
        End Select
        Return sApplName
    End Function
    Public Function GetCCTypeLongName(ByVal CCType As eColorChangeType) As String
        '********************************************************************************************
        'Description:   Return the name for a cc type
        '
        'Parameters: datarow
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW	    Add app types
        ' 02/15/10  BTK     Added color change type for Versabell2 32 valves
        ' 11/12/13  BTK     Added VB3 Single Canister
        '********************************************************************************************
        Dim sApplName As String = ""
        Select Case (CCType)
            Case eColorChangeType.SINGLE_PURGE
                sApplName = grsRM.GetString("rsGUN_SINGLE_PURGE")
            Case eColorChangeType.DUAL_PURGE
                sApplName = grsRM.GetString("rsGUN_DUAL_PURGE")
            Case eColorChangeType.MODIFIED_DUAL_PURGE
                sApplName = grsRM.GetString("rsGUN_MOD_DUAL_PURGE")
            Case eColorChangeType.GUN_2K
                sApplName = grsRM.GetString("rsGUN_2K")
            Case eColorChangeType.GUN_2K_PIG
                sApplName = grsRM.GetString("rsGUN_2K_PIG")
            Case eColorChangeType.GUN_2K_Mica
                sApplName = grsRM.GetString("rsGUN_2K_MICA")
            Case eColorChangeType.SINGLE_PURGE_BELL
                sApplName = grsRM.GetString("rsBELL")
            Case eColorChangeType.BELL_2K
                sApplName = grsRM.GetString("rsBELL2K")
            Case eColorChangeType.ACCUSTAT
                sApplName = grsRM.GetString("rsACCUSTAT")
            Case eColorChangeType.AQUABELL
                sApplName = grsRM.GetString("rsAQUABELL")
            Case eColorChangeType.SERVOBELL
                sApplName = grsRM.GetString("rsSERVOBELL")
            Case eColorChangeType.VERSABELL
                sApplName = grsRM.GetString("rsVERSABELL")
            Case eColorChangeType.VERSABELL2
                sApplName = grsRM.GetString("rsVERSABELL2")
            Case eColorChangeType.VERSABELL2_WB
                sApplName = grsRM.GetString("rsVERSABELL2_WB")
            Case eColorChangeType.VERSABELL2_2K
                sApplName = grsRM.GetString("rsVERSABELL2_2K")
            Case eColorChangeType.POWDER
                sApplName = grsRM.GetString("rsPOWDER")
            Case eColorChangeType.VERSABELL_2K
                sApplName = grsRM.GetString("rsVERSABELL_2K")
            Case eColorChangeType.VERSABELL2_2K_1PLUS1
                sApplName = grsRM.GetString("rsVERSABELL2_2K_1PLUS1")
            Case eColorChangeType.VERSABELL2_2K_MULTIRESIN
                sApplName = grsRM.GetString("rsVERSABELL2_2K_MULTIRESIN")
            Case eColorChangeType.VERSABELL2_PLUS
                sApplName = grsRM.GetString("rsVERSABELL2_PLUS")
            Case eColorChangeType.VERSABELL2_WB
                sApplName = grsRM.GetString("rsVERSABELL2_WB")
            Case eColorChangeType.VERSABELL2_PLUS_WB
                sApplName = grsRM.GetString("rsVERSABELL2_PLUS_WB")
            Case eColorChangeType.HONDA_1K
                sApplName = grsRM.GetString("rsHONDA_1K")
            Case eColorChangeType.HONDA_WB
                sApplName = grsRM.GetString("rsHONDA_WB")
            Case eColorChangeType.VERSABELL2_32 ' 02/15/10  BTK     Added color change type for Versabell2 32 valves
                sApplName = grsRM.GetString("rsVERSABELL2")
            Case eColorChangeType.VERSABELL3_DUAL_WB
                sApplName = grsRM.GetString("rsVERSABELL3_DUAL_WB")
            Case eColorChangeType.VERSABELL3_WB
                sApplName = grsRM.GetString("rsVERSABELL3_WB")
            Case eColorChangeType.VERSABELL3
                sApplName = grsRM.GetString("rsVERSABELL3")
            Case eColorChangeType.VERSABELL3P1000 'RJO 07/16/14
                sApplName = grsRM.GetString("rsVERSABELL3P1000")
            Case eColorChangeType.GUN_1K 'NRU 160922
                sApplName = grsRM.GetString("rsGUN_1K")
        End Select
        Return sApplName
    End Function
    Friend Function LoadParameterBox(ByRef rCbo As ComboBox, Optional ByVal bCal As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  Load combo with parameter names
        '
        'Parameters: rCBO : combobox to load, bCal - true = only load parameters with cal tables
        'Returns:    true for success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        Dim nTmp As Integer()
        ReDim nTmp(0)
        For nParmNum As Integer = 0 To mnNumParms
            With mtParams(nParmNum)
                If Not bCal Or .bUseCounts Then
                    rCbo.Items.Add(.sParmName)
                    nTmp(UBound(nTmp)) = .nParmNum
                    ReDim Preserve nTmp(UBound(nTmp) + 1)
                End If
            End With
        Next

        rCbo.Tag = nTmp

        Return False
    End Function
#End Region
#Region " Events "

    Friend Sub New(ByVal CCType As eColorChangeType, ByRef Zone As clsZone, Optional ByVal bReadFromRobot As Boolean = False)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: Color change type
        'Returns:  Applicator
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        subInitializeAppl(CCType, Zone, , bReadFromRobot)
    End Sub
    Friend Sub New(ByRef Arm As clsArm, ByRef Zone As clsZone, Optional ByVal bReadFromRobot As Boolean = False)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: Robot arm
        'Returns:  Applicator
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        MyBase.New()
        subInitializeAppl(Arm.ColorChangeType, Zone, Arm, bReadFromRobot)
    End Sub

#End Region



End Class 'Applicator class
'********************************************************************************************
'Description: Applicator Common Code
'Routines that would be in mPWRobotCommon if they didn't need clsApplicators defined
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend module ApplicatorCommon
    Friend Function LoadRobotBoxFromCollectionByCCTypeName(ByRef rCbo As ComboBox, ByRef ArmCollection As clsArms, _
            ByVal AddAll As Boolean, ByRef ApplCollection As clsApplicators, ByVal CCType As String) As Boolean

        '********************************************************************************************
        'Description:  Load Robot combo with arm names
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/01/09   MSW     Add in the select by cc type name
        '11/05/09   MSW     Check for openers
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        If ArmCollection Is Nothing Then Return False

        Dim nTmp As Integer()
        ReDim nTmp(0)
        Dim bAdd As Boolean
        Dim bAddAllRobots As Boolean = (CCType = gcsRM.GetString("csALL"))
        rCbo.Items.Clear()

        If AddAll Then
            rCbo.Items.Add(gcsRM.GetString("csALL"))
            nTmp(0) = 0
            ReDim Preserve nTmp(1)
        End If

        For Each o As clsArm In ArmCollection
            bAdd = Not (o.IsOpener)
            If bAdd And Not (bAddAllRobots) Then
                Dim a As clsApplicator = ApplCollection.Item(o.ColorChangeType)
                If (a.Name <> (CCType)) And (a.LongName <> (CCType) And "" <> (CCType)) Then
                    bAdd = False
                End If
            End If
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


    Friend Function LoadRobotBoxFromCollectionByCCTypeName(ByRef rLst As CheckedListBox, ByRef ArmCollection As clsArms, _
            ByVal AddAll As Boolean, ByRef ApplCollection As clsApplicators, ByVal CCType As String) As Boolean
        '********************************************************************************************
        'Description:  Load Robot checked Listbox with Controller names that match a cc type name
        '           It can be either the long or short cc type name
        'Parameters: box to load, controller collection and a void so we can overload
        'Returns:    true if ok
        '
        'Modification history:
        '
        ' Date      By      Reason
        '05/01/09   MSW     Add in the select by cc type
        '11/05/09   MSW     Check for openers
        '********************************************************************************************    
        If rLst Is Nothing Then Return False
        If ArmCollection Is Nothing Then Return False

        Try
            Dim sNames() As String
            Dim nTmp As Integer()
            Dim bAdd As Boolean
            Dim nCount As Integer = 0
            Dim bAddAllRobots As Boolean = (CCType = gcsRM.GetString("csALL"))
            ReDim Preserve nTmp(0)
            ReDim Preserve sNames(0)
            For Each o As clsArm In ArmCollection
                bAdd = Not (o.IsOpener)
                If bAdd And Not (bAddAllRobots) Then
                    Dim a As clsApplicator = ApplCollection.Item(o.ColorChangeType)
                    If (a.Name <> (CCType)) And (a.LongName <> (CCType)) Then
                        bAdd = False
                    End If
                End If
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
    Public Function GetAFCalStatusText(ByVal CalStatus As Integer) As String

        Select Case CalStatus
            Case eAFCalStatus.AF_NOT_CAL
                GetAFCalStatusText = gpsRM.GetString("psNOT_COMP")
            Case eAFCalStatus.AF_CAL_OK
                GetAFCalStatusText = gpsRM.GetString("psCOMPLETE")
            Case eAFCalStatus.AF_CANT_UPR
                GetAFCalStatusText = gpsRM.GetString("psAF_CANT_UPR")
            Case eAFCalStatus.AF_CANT_LOWR
                GetAFCalStatusText = gpsRM.GetString("psAF_CANT_LOWR")
            Case eAFCalStatus.AF_ADAPT_OUT
                GetAFCalStatusText = gpsRM.GetString("psAF_ADAPT_OUT")
            Case eAFCalStatus.AF_UPPER_LIM
                GetAFCalStatusText = gpsRM.GetString("psAF_UPPER_LIM")
            Case eAFCalStatus.AF_LOWER_LIM
                GetAFCalStatusText = gpsRM.GetString("psAF_LOWER_LIM")
            Case eAFCalStatus.AF_CAL_ABORT
                GetAFCalStatusText = gpsRM.GetString("psABORTED")
            Case eAFCalStatus.AF_CAL_COPY
                GetAFCalStatusText = gpsRM.GetString("psCOPIED")
            Case eAFCalStatus.AF_COPY_ER
                GetAFCalStatusText = gpsRM.GetString("psCOPY_ERR")
            Case Else
                GetAFCalStatusText = gcsRM.GetString("csERROR")
        End Select
    End Function

End Module
