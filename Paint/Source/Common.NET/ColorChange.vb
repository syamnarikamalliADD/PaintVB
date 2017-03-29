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
' Form/Module: ColorChange.vb
'
' Description: This holds the pieces parts for color change info
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
'   03/17/07    gks     Onceover Cleanup
'   08/15/07    gks     typed collections - imagelist still needs work
'   05/29/09    MSW     switch to VB 2008
'                       Revised clsColorChangeCartoon and its children to fit cartoons in user controls.  
'                       Split group and shared valves for 32 valve support
'                       Moved cartoon classes out of here into ColorChangeCartoons.vb
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    01/03/12   MSW     Changes for speed - load fewer details during copy            4.01.01.01
'    01/10/12   MSW     More speed improvements                                       4.01.01.02
'                       Make better use of FRCVars.NoRefresh
'                       improve handling of empty names
'    01/24/12   MSW     Applicator Updates                                            4.01.01.03
'    07/31/12   JBW     Bug fix in clsColorChange ActionName and EventName porperties 4.01.01.04
'                       to allow access to the last Action or Event in the list.
'    09/30/13   MSW     Remove mnMAX_CYCLES                                           4.01.05.05
'********************************************************************************************

Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports FRRobot

'********************************************************************************************
'Description: Top level color change Class
'
'
'Modification history:
'
' Date      By      Reason
' 01/03/12  MSW     Changes for speed - load fewer details during copy
' 09/30/13  MSW     Remove mnMAX_CYCLES
'********************************************************************************************
Friend Class clsColorChange

#Region " Declares"

    'Private Const mnMAX_CYCLES As Integer = 10 'mProcessDeclares.gnMAX_COLORCHANGE_CYCLES
    Private Const mnDEF_CYCLES As Integer = 8

    Private mnValve As Integer = 0
    Private msValve As String = "-"
    Private moCycles As Collection(Of clsColorChangeCycle) = Nothing
    Private mRobot As clsArm = Nothing
    'Action and event names
    Private msActions() As String = Nothing
    Private msEvents() As String = Nothing
    Private mnNumActions As Integer
    Private mnNumEvents As Integer
    Private moPushVol As clsSngValue
    Private mbPushByVol As Boolean
    Private mbSimpleData As Boolean
#End Region
#Region " Properties "

    Friend ReadOnly Property ActionName(ByVal nIndex As Integer) As String
        '********************************************************************************************
        'Description: action names
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            'JBW 07/31/12 Changed to <= instead of strictly < than
            If nIndex <= mnNumActions Then
                Return msActions(nIndex)
            Else
                Return String.Empty
            End If
        End Get
    End Property
    Friend ReadOnly Property EventName(ByVal nIndex As Integer) As String
        '********************************************************************************************
        'Description: Event names
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            'JBW 07/31/12 changed to a <= compare instead of strictly < than
            If nIndex <= mnNumEvents Then
                Return msEvents(nIndex)
            Else
                Return String.Empty
            End If
        End Get
    End Property
    Friend ReadOnly Property NumActions() As Integer
        '********************************************************************************************
        'Description: number of actions
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumActions
        End Get
    End Property
    Friend ReadOnly Property NumEvents() As Integer
        '********************************************************************************************
        'Description: number of events
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumEvents
        End Get
    End Property
    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: But I can change...honest..
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As clsColorChangeCycle
            If moPushVol.Changed Then Return True

            For Each o In moCycles
                If o.Changed Then Return True
            Next

            Return False

        End Get
    End Property

    Friend ReadOnly Property Cycle(ByVal CycleNumber As Integer) As clsColorChangeCycle
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim o As clsColorChangeCycle
            For Each o In moCycles
                If o.CycleNumber = CycleNumber Then
                    Return o
                End If
            Next

            Return Nothing
        End Get
    End Property
    Friend Property NumberOfCycles() As Integer
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moCycles.Count
        End Get
        Set(ByVal Value As Integer)
            'setting this initializes everything to get ready to load data
            Dim i%

            If Value < 1 Then Exit Property
            'If Value > mnMAX_CYCLES Then Exit Property

            If moCycles.Count > 0 Then
                'get rid of old data
                For i% = (moCycles.Count - 1) To 0 Step -1
                    moCycles.RemoveAt(i%)
                Next
                moCycles = New Collection(Of clsColorChangeCycle)
            End If

            For i% = 1 To Value
                Dim o As New clsColorChangeCycle
                o.CycleNumber = i%
                moCycles.Add(o)
            Next
            'Refresh the cycle names
            subGetCCInfo(True)
        End Set
    End Property
    Friend ReadOnly Property Robot() As clsArm
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mRobot
        End Get
    End Property
    Friend Property Valve() As Integer
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnValve
        End Get
        Set(ByVal Value As Integer)
            If Value > 0 Then
                mnValve = Value
            End If
        End Set
    End Property
    Friend Property PushOutVolume() As clsSngValue
        '********************************************************************************************
        'Description: pushout volume
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moPushVol
        End Get
        Set(ByVal Value As clsSngValue)
            moPushVol = Value
        End Set
    End Property
    Friend Property ShowPushoutVolume(Optional ByVal nCycle As Integer = 0) As Boolean
        '********************************************************************************************
        'Description: pushout volume available for this version
        '           only tells if the data is available because the option is supported, 
        '           does not check the enable
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW     Run robot names through resource file
        '********************************************************************************************
        Get
            If nCycle = 0 Then
                'Don't care if it's pushout just if the robot supports pushout by colume
                Return mbPushByVol
            Else
                'see if we should display it for this cycle
                If nCycle = 1 Then 'So far pushout is always cycle one.  If it changes we'll need to deal with it here
                    Return mbPushByVol
                Else
                    Return False
                End If
            End If
        End Get
        Set(ByVal Value As Boolean)
            mbPushByVol = Value
        End Set
    End Property
    Friend Property ValveDescription() As String
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msValve
        End Get
        Set(ByVal Value As String)
            msValve = Value
        End Set
    End Property

#End Region
#Region " Routines "

    Public Sub subGetCCInfo(Optional ByVal bNoSetNumCycles As Boolean = False)
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 2/11/09   gks     add version check - redo
        ' 11/10/09  MSW     Run robot names through resource file
        ' 01/03/12  MSW     Changes for speed - load fewer details during copy
        ' 01/06/12  MSW     Make better use of FRCVars.NoRefresh for speed improvements
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim o As FRRobot.FRCVars = Nothing
        Dim oo As FRRobot.FRCVars = Nothing
        Dim sTmp() As String
        Dim i As Integer
        Try
            If mRobot.Applicator IsNot Nothing Then
                If NumberOfCycles = 0 And bNoSetNumCycles = False Then
                    NumberOfCycles = mRobot.Applicator.NumCCCycles
                End If

                For i = 1 To NumberOfCycles
                    Cycle(i).CycleName = mRobot.Applicator.CCCycleName(i)
                Next
                msActions = mRobot.Applicator.CCActions
                msEvents = mRobot.Applicator.CCEvents
                mnNumActions = msActions.GetUpperBound(0)
                mnNumEvents = msEvents.GetUpperBound(0)
            Else
                If mRobot.IsOnLine Then
                    If mRobot.Controller.Version > 6.4 Then
                        mRobot.ProgramName = "PAVRCCEX"
                        o = mRobot.ProgramVars
                        Dim ooo As FRRobot.FRCVars = DirectCast(o.Item("TCCEXCMOS"), FRRobot.FRCVars)
                        oo = DirectCast(ooo.Item("CC_CYC_NAM"), FRRobot.FRCVars)
                    Else
                        mRobot.ProgramName = "PAVRCCIO"
                        o = mRobot.ProgramVars
                        oo = DirectCast(o.Item("CC_CYC_NAM"), FRRobot.FRCVars)
                    End If
                    oo.NoRefresh = True
                    ReDim sTmp(oo.Count)

                    For i = 0 To oo.Count - 1
                        oVar = DirectCast(oo.Item(, i), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            Dim sTmpStr As String = oVar.Value.ToString
                            If sTmpStr = String.Empty Then
                                Exit For
                            End If
                            sTmp(i + 1) = sGetResTxtFrmRobotTxt(sTmpStr, mRobot)
                        Else
                            Exit For
                        End If
                    Next
                    oo.NoRefresh = False
                    If NumberOfCycles = 0 And bNoSetNumCycles = False Then
                        NumberOfCycles = i
                    End If


                    For i = 1 To NumberOfCycles
                        If i > sTmp.GetUpperBound(0) Then
                            Cycle(i).CycleName = String.Empty
                        Else
                            Cycle(i).CycleName = sTmp(i)
                        End If

                    Next

                    If mbSimpleData Then
                        Exit Sub
                    End If

                    subGetActionEventNames(mRobot, msActions, msEvents)
                    mnNumActions = msActions.GetUpperBound(0)
                    mnNumEvents = msEvents.GetUpperBound(0)
                End If

            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Friend Sub Update()
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As clsColorChangeCycle
        moPushVol.Update()
        For Each o In moCycles
            o.Update()
        Next
    End Sub

#End Region
#Region " Events "

    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            For i As Integer = moCycles.Count To 1 Step -1
                moCycles.RemoveAt(i% - 1)
            Next
            moCycles = Nothing

        Catch ex As Exception
            Trace.WriteLine("Module: clsColorChange, Routine: Finalize, Error: " & ex.Message)
            Trace.WriteLine("Module: clsColorChange, Routine: Finalize, StackTrace: " & ex.StackTrace)
        Finally
            MyBase.Finalize()
        End Try

    End Sub
    Friend Sub New(ByVal vRobot As clsArm, Optional ByVal bSimpleData As Boolean = False)
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/12  MSW     Changes for speed - load fewer details during copy
        '********************************************************************************************
        MyBase.New()
        mbSimpleData = bSimpleData
        mRobot = vRobot
        moPushVol = New clsSngValue
        moCycles = New Collection(Of clsColorChangeCycle)
        mnValve = 1
        msValve = "-"
        subGetCCInfo()

    End Sub

#End Region

End Class  'clsColorChange
'********************************************************************************************
'Description: Class to hold a Color change cycle
'
'
'Modification history:
'
' Date      By      Reason
' 09/30/13  MSW     Remove mnMAX_CYCLE
'********************************************************************************************
Friend Class clsColorChangeCycle

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop
    Private Const mnMAX_STEPS As Integer = 100
    'Private Const mnMAX_CYCLE As Integer = 10

    Private mnNumStepsOld As Integer ' = 0
    Private moSteps As Collection(Of clsCCStep)
    Private mnCycNumber As Integer ' = 0
    Private msCycName As String = String.Empty

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: Did it change?
        '
        'Parameters:  none
        'Returns:     true if changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/22/10  MSW     Adding steps to an empty cycle counts for a change
        '********************************************************************************************
        Get
            Dim o As clsCCStep

            For Each o In moSteps
                If o.Changed Then Return True
            Next

            If (mnNumStepsOld <> moSteps.Count) Then Return True

            Return False

        End Get
    End Property
    Friend Property CycleName() As String
        '********************************************************************************************
        'Description: Whats your name
        '
        'Parameters:  none
        'Returns:     text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msCycName
        End Get
        Set(ByVal Value As String)
            msCycName = Value
        End Set

    End Property
    Friend Property CycleNumber() As Integer
        '********************************************************************************************
        'Description: cycle number
        '
        'Parameters:  none
        'Returns:     number
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/13  MSW     Remove mnMAX_CYCLE
        '********************************************************************************************
        Get
            Return mnCycNumber
        End Get
        Set(ByVal Value As Integer)
            'If (Value < 1) Or (Value > mnMAX_CYCLE) Then Exit Property
            If (Value < 1) Then Exit Property
            mnCycNumber = Value
        End Set

    End Property
    Friend Property NumberOfSteps() As Integer
        '********************************************************************************************
        'Description: How many Steps in this cycle
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moSteps.Count
        End Get
        Set(ByVal Value As Integer)
            If (Value < 0) Or (Value > mnMAX_STEPS) Then Exit Property

            If mnNumStepsOld = 0 Then
                'first time set log value for "old"
                mnNumStepsOld = Value
            End If
            subAdjustNumberOfSteps(Value)
        End Set

    End Property
    Friend ReadOnly Property NumberOfStepsOld() As Integer
        '********************************************************************************************
        'Description: how many there used to be
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumStepsOld
        End Get

    End Property
    Default Friend ReadOnly Property Steps(ByVal StepNumber As Integer) As clsCCStep
        '********************************************************************************************
        'Description: how to get a step
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            For Each o As clsCCStep In moSteps
                If o.StepNumber = StepNumber Then
                    Return o
                End If
            Next
            ' missed it or bad step num
            Return Nothing

        End Get

    End Property

#End Region
#Region " Routines "
    Friend Function InsertStep(ByVal InsertAfter As Integer) As Boolean
        '********************************************************************************************
        'Description: add a step
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/27/10  RJO     Action and Event values were uninitialized. A new selected value for these
        '                   items would not be saved
        '********************************************************************************************

        Dim o As clsCCStep

        If InsertAfter < 0 Then Return False
        If moSteps.Count = mnMAX_STEPS Then Return False

        Try

            For Each o In moSteps
                If o.StepNumber > InsertAfter Then
                    o.StepNumber = o.StepNumber + 1
                End If
            Next

            o = New clsCCStep
            o.StepNumber = InsertAfter + 1
            o.StepAction.Value = 0
            o.StepEvent.Value = 0
            moSteps.Add(o)

            Return True

        Catch ex As Exception
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: InsertStep, Error: " & ex.Message)
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: InsertStep, StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function
    Friend Function RemoveStep(ByVal StepNumber As Integer) As Boolean
        '********************************************************************************************
        'Description: Remove a step
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/10  RJO     This crashed and burned if you remove any step but the last from a cycle.
        '                   When a step is deleted, you have to start at the beginning of the 
        '                   collection before you go off re-numbering steps because the item count
        '                   has changed.
        '********************************************************************************************

        Dim i%
        Dim o As clsCCStep

        If StepNumber < 0 Then Return False
        If StepNumber > moSteps.Count Then Return False

        Try

            'For i% = 0 To moSteps.Count - 1
            '    o = CType(moSteps(i%), clsCCStep)
            '    If o.StepNumber > StepNumber Then
            '        o.StepNumber = o.StepNumber - 1
            '    End If
            '    If o.StepNumber = StepNumber Then
            '        moSteps.RemoveAt(i%)
            '    End If
            'Next

            '>>> RJO 02/02/10
            For i% = 0 To moSteps.Count - 1
                o = CType(moSteps(i%), clsCCStep)

                If o.StepNumber = StepNumber Then
                    moSteps.RemoveAt(i%)
                    Exit For
                End If
            Next

            For i% = 0 To moSteps.Count - 1
                o = CType(moSteps(i%), clsCCStep)

                If o.StepNumber > StepNumber Then
                    o.StepNumber = o.StepNumber - 1
                End If
            Next
            '<<<

            Return True

        Catch ex As Exception
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: RemoveStep, Error: " & ex.Message)
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: RemoveStep, StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function
    Private Sub subAdjustNumberOfSteps(ByVal NewNumberOfSteps As Integer)
        '********************************************************************************************
        'Description: add or subtract steps
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim i%
        Dim o As clsCCStep

        If moSteps.Count = 0 Then

            ' brand new fill collection
            For i% = 1 To NewNumberOfSteps
                o = New clsCCStep
                o.StepNumber = i%
                moSteps.Add(o)
            Next

        Else

            If NewNumberOfSteps < moSteps.Count Then
                'number being reduced
                For i% = moSteps.Count - 1 To 0 Step -1
                    o = CType(moSteps(i%), clsCCStep)
                    If o.StepNumber > NewNumberOfSteps Then
                        moSteps.RemoveAt(i%)
                    End If
                Next
            End If

            If NewNumberOfSteps > moSteps.Count Then
                'number being Increased
                For i% = moSteps.Count To NewNumberOfSteps - 1
                    o = New clsCCStep
                    o.StepNumber = i% + 1
                    moSteps.Add(o)
                Next
            End If

        End If
    End Sub
    Friend Sub Update()
        '********************************************************************************************
        'Description: Make old = new
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim o As clsCCStep

        For Each o In moSteps
            o.Update()
        Next
        mnNumStepsOld = moSteps.Count

    End Sub
#End Region
#Region " Events "
    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description: goodbye
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim i%
        Try
            For i% = moSteps.Count - 1 To 0 Step -1
                moSteps.RemoveAt(i%)
            Next
            moSteps = Nothing
        Catch ex As Exception
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: Finalize, Error: " & ex.Message)
            Trace.WriteLine("Module: clsColorChangeCycle, Routine: Finalize, StackTrace: " & ex.StackTrace)
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Friend Sub New()
        '********************************************************************************************
        'Description: hello world
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        MyBase.New()
        moSteps = New Collection(Of clsCCStep)
    End Sub
#End Region
End Class 'clsColorChangeCycle
'********************************************************************************************
'Description: a Color Change Step
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsCCStep

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop
    Private mnIndex As Integer ' = 0
    Private msName As String = String.Empty
    Private Const mnOff As Integer = 0
    Private Const mnON As Integer = 1
    Private Const mnLASTSTATE As Integer = 2
    Private Const mnNUM_DOUT As Integer = 4
    Private Const mnNUM_GOUT As Integer = 32
    Private Const mnNUM_VALVES As Integer = mnNUM_DOUT + mnNUM_GOUT

    Private moDOUTState As clsIntValue = Nothing
    Private moDOUTDC As clsIntValue = Nothing
    Private moGOUTState As clsIntValue = Nothing
    Private moGOUTDC As clsIntValue = Nothing

    Private moDuration As clsSngValue = Nothing
    Private moPreset As clsIntValue = Nothing
    Private moStepAction As clsIntValue = Nothing
    Private moStepEvent As clsIntValue = Nothing

#End Region
#Region " Properties "

    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'Description: did it change? 
        '
        'Parameters:  none
        'Returns:    true if changes
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get


            If moDuration.Changed() Then Return True
            If moPreset.Changed() Then Return True
            If moStepAction.Changed() Then Return True
            If moStepEvent.Changed() Then Return True

            If moDOUTState.Changed() Then Return True
            If moDOUTDC.Changed() Then Return True
            If moGOUTState.Changed() Then Return True
            If moGOUTDC.Changed() Then Return True
            Return False
        End Get

    End Property
    Friend Property Duration() As clsSngValue
        '********************************************************************************************
        'Description: How long is it?
        '
        'Parameters:  none
        'Returns:    Not as long as you think
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moDuration
        End Get

        Set(ByVal Value As clsSngValue)
            moDuration = value
        End Set
    End Property
    Friend Property DoutDC() As clsIntValue
        '********************************************************************************************
        'Description: digital output last state info
        '
        'Parameters:  none
        'Returns:   state value 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moDOUTDC
        End Get
        Set(ByVal Value As clsIntValue)
            moDOUTDC = Value
        End Set
    End Property
    Friend Property DoutState() As clsIntValue
        '********************************************************************************************
        'Description: digital output  state info
        '
        'Parameters:  none
        'Returns:   state value 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moDOUTState
        End Get
        Set(ByVal Value As clsIntValue)
            moDOUTState = Value
        End Set
    End Property
    Friend Property GoutDC() As clsIntValue
        '********************************************************************************************
        'Description: grout output last state info
        '
        'Parameters:  none
        'Returns:   state value 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moGOUTDC
        End Get
        Set(ByVal Value As clsIntValue)
            moGOUTDC = Value
        End Set
    End Property
    Friend Property GoutState() As clsIntValue
        '********************************************************************************************
        'Description: grout output   state info
        '
        'Parameters:  none
        'Returns:   state value 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moGOUTState
        End Get
        Set(ByVal Value As clsIntValue)
            moGOUTState = Value
        End Set
    End Property
    Friend Property Preset() As clsIntValue
        '********************************************************************************************
        'Description: preset number
        '
        'Parameters:  none
        'Returns:   number 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moPreset
        End Get
        Set(ByVal Value As clsIntValue)
            moPreset = Value
        End Set
    End Property
    Friend Property StepName() As String
        '********************************************************************************************
        'Description: what to call it
        '
        'Parameters:  none
        'Returns:   text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msName
        End Get
        Set(ByVal Value As String)
            msName = Value
        End Set
    End Property
    Friend Property StepNumber() As Integer
        '********************************************************************************************
        'Description: step number
        '
        'Parameters:  none
        'Returns:   number 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value > 0 Then mnIndex = Value
        End Set
    End Property
    Friend Property StepAction() As clsIntValue
        '********************************************************************************************
        'Description: What's it doing
        '
        'Parameters:  none
        'Returns:   number 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moStepAction
        End Get
        Set(ByVal Value As clsIntValue)
            moStepAction = Value
        End Set
    End Property
    Friend Property StepEvent() As clsIntValue
        '********************************************************************************************
        'Description: Whats it waiting for
        '
        'Parameters:  none
        'Returns:   number 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return moStepEvent
        End Get
        Set(ByVal Value As clsIntValue)
            moStepEvent = Value
        End Set
    End Property
    Friend ReadOnly Property GroupValveState(ByVal nValveNo As Integer) As Integer
        '********************************************************************************************
        'Description: individual valve state 
        '
        'Parameters:  valve no starting at  0
        'Returns:   0=off, 1=on, 2=DC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Dim nBitmask As Integer = CType(2 ^ (nValveNo), Integer)
            If ((moGOUTState.Value And nBitmask) > 0) Then
                Return 1
            ElseIf ((moGOUTDC.Value And nBitmask) > 0) Then
                Return 2
            Else
                Return 0
            End If
        End Get
    End Property
    Friend ReadOnly Property SharedValveState(ByVal nValveNo As Integer) As Integer
        '********************************************************************************************
        'Description: individual valve state 
        '
        'Parameters:  valve no starting at  0
        'Returns:   0=off, 1=on, 2=DC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Dim nBitmask As Integer = CType(2 ^ (nValveNo), Integer)
            If ((moDOUTState.Value And nBitmask) > 0) Then
                Return 1
            ElseIf ((moDOUTDC.Value And nBitmask) > 0) Then
                Return 2
            Else
                Return 0
            End If
        End Get
    End Property
#End Region
#Region " Routines "

    Friend Sub Update()
        '********************************************************************************************
        'Description: make old = new
        '
        'Parameters:  none
        'Returns:   number 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        moDuration.Update()
        moPreset.Update()
        moStepAction.Update()
        moStepEvent.Update()
        moDOUTState.Update()
        moDOUTDC.Update()
        moGOUTState.Update()
        moGOUTDC.Update()
    End Sub

#End Region
#Region "Events"

    Friend Sub New()
        '********************************************************************************************
        'Description: hello world
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        MyBase.New()

        moStepAction = New clsIntValue
        moStepEvent = New clsIntValue
        moPreset = New clsIntValue
        moDuration = New clsSngValue

        moDOUTState = New clsIntValue
        moDOUTDC = New clsIntValue
        moGOUTState = New clsIntValue
        moGOUTDC = New clsIntValue
    End Sub


#End Region


End Class 'clsCCStep
'********************************************************************************************
'Description: commom routines
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Module mCCCommon

#Region " Routines "
    Friend Function LoadApplBoxFromCollection(ByRef rCbo As ComboBox, _
            ByRef ApplCollection As clsApplicators, ByVal AddAll As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Load applicator combo with applicator names
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/05/09  MSW     Switch to long name to keep them different
        '********************************************************************************************    
        If rCbo Is Nothing Then Return False
        If ApplCollection Is Nothing Then Return False

        Dim nTmp As eColorChangeType()
        Dim i As Integer = 0
        ReDim nTmp(i)

        rCbo.Items.Clear()

        If AddAll Then
            rCbo.Items.Add(gcsRM.GetString("csALL"))
            nTmp(i) = eColorChangeType.NOT_SELECTED
            i += 1
            ReDim Preserve nTmp(i)
        End If


        For Each o As clsApplicator In ApplCollection
            If (rCbo.Items.Contains(o.LongName) = False) Then
                rCbo.Items.Add(o.LongName)
                nTmp(i) = o.ColorChangeType
                i += 1
                ReDim Preserve nTmp(i)
            End If
        Next

        rCbo.Tag = nTmp

    End Function

    Friend Sub LoadCCCycleBox(ByVal vColorchange As clsColorChange, ByRef rCombo As ComboBox, Optional ByVal bLoadLists As Boolean = False)
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 8/10/10   MSW     Load cycle lists
        '********************************************************************************************
        Dim nTmp As Integer()
        ReDim nTmp(vColorchange.NumberOfCycles + 2)

        rCombo.Items.Clear()

        For i As Integer = 1 To vColorchange.NumberOfCycles
            rCombo.Items.Add(vColorchange.Cycle(i).CycleName)
            nTmp(i - 1) = vColorchange.Cycle(i).CycleNumber
        Next
        If bLoadLists Then
            rCombo.Items.Add(gpsRM.GetString("psSTANDARD_CYCLE_LIST"))
            nTmp(vColorchange.NumberOfCycles) = 11
            rCombo.Items.Add(gpsRM.GetString("psFULL_CYCLE_LIST"))
            nTmp(vColorchange.NumberOfCycles + 1) = 12
            rCombo.Items.Add(gpsRM.GetString("psCLEAN0_CYCLE_LIST"))
            nTmp(vColorchange.NumberOfCycles + 2) = 13
        End If
        'this is so we can get the cycle number for the plc later - and not use listindex
        rCombo.Tag = nTmp
    End Sub

    Friend Sub LoadCCCycleBox(ByRef oApplicator As clsApplicator, ByRef rCombo As ComboBox, Optional ByVal bLoadLists As Boolean = False)
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 8/10/10   MSW     Load cycle lists
        '********************************************************************************************
        Dim nTmp As Integer()
        ReDim nTmp(oApplicator.NumCCCycles - 1)

        rCombo.Items.Clear()

        For i As Integer = 1 To oApplicator.NumCCCycles
            rCombo.Items.Add(oApplicator.CCCycleName(i))
            nTmp(i - 1) = i
        Next
        If bLoadLists Then
            ReDim nTmp(oApplicator.NumCCCycles + 2)
            rCombo.Items.Add(gpsRM.GetString("psSTANDARD_CYCLE_LIST"))
            nTmp(oApplicator.NumCCCycles) = 11
            rCombo.Items.Add(gpsRM.GetString("psFULL_CYCLE_LIST"))
            nTmp(oApplicator.NumCCCycles + 1) = 12
            rCombo.Items.Add(gpsRM.GetString("psCLEAN0_CYCLE_LIST"))
            nTmp(oApplicator.NumCCCycles + 2) = 13
        End If
        'this is so we can get the cycle number for the plc later - and not use listindex
        rCombo.Tag = nTmp
    End Sub
    Friend Sub subGetActionEventNames(ByRef rRobot As clsArm, ByRef sActions() As String, ByRef sEvents() As String)
        '********************************************************************************************
        'Description: get action and event names
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW     Run robot names through resource file
        ' 01/06/12  MSW     improve handling of empty names
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim oProgVars As FRRobot.FRCVars = Nothing
        Dim oActionArray As FRRobot.FRCVars = Nothing
        Dim oEventArray As FRRobot.FRCVars = Nothing
        Dim i As Integer

        Try
            If rRobot.IsOnLine Then
                If rRobot.Controller.Version > 6.3 Then
                    rRobot.ProgramName = "PAVRCCEX"
                    oProgVars = rRobot.ProgramVars
                    Dim oStructure As FRRobot.FRCVars = DirectCast(oProgVars.Item("TCCEXCMOS"), FRRobot.FRCVars)
                    oActionArray = DirectCast(oStructure.Item("ACTION_NAM"), FRRobot.FRCVars)
                    oEventArray = DirectCast(oStructure.Item("EVENT_NAM"), FRRobot.FRCVars)
                Else
                    rRobot.ProgramName = "PAVRCCIO"
                    oProgVars = rRobot.ProgramVars
                    oActionArray = DirectCast(oProgVars.Item("ACTION_NAM"), FRRobot.FRCVars)
                    oEventArray = DirectCast(oProgVars.Item("EVENT_NAM"), FRRobot.FRCVars)
                End If

                ReDim sActions(oActionArray.Count + 1)
                ReDim sEvents(oEventArray.Count + 1)
                sActions(0) = grsRM.GetString("rsNO_ACTION")
                sEvents(0) = grsRM.GetString("rsNO_EVENT")
                For i = 1 To oActionArray.Count
                    oVar = DirectCast(oActionArray.Item(, i - 1), FRRobot.FRCVar)
                    If oVar.IsInitialized AndAlso (sActions(i) <> String.Empty) Then
                        sActions(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString, rRobot)
                    Else
                        sActions(i) = String.Empty
                    End If
                Next
                For i = 1 To oEventArray.Count
                    oVar = DirectCast(oEventArray.Item(, i - 1), FRRobot.FRCVar)
                    If oVar.IsInitialized AndAlso (sEvents(i) <> String.Empty) Then
                        sEvents(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString, rRobot)
                    Else
                        sEvents(i) = String.Empty
                    End If
                Next

            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub subGetValveLabels(ByRef rRobot As clsArm, ByRef sShared() As String, ByRef sGroup() As String, Optional ByVal b_CAP As Boolean = False)
        '********************************************************************************************
        'Description: get avalve labels
        '
        'Parameters:  none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/10/09  MSW     Run robot names through resource file
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim oProgVars As FRRobot.FRCVars = Nothing
        Dim oSharedArray As FRRobot.FRCVars = Nothing
        Dim oGroupArray As FRRobot.FRCVars = Nothing
        Dim i As Integer

        Try
            If rRobot.IsOnLine Then
                If rRobot.Controller.Version > 6.3 Then
                    rRobot.ProgramName = "PAVRCCEX"
                    oProgVars = rRobot.ProgramVars
                    Dim oStructure As FRRobot.FRCVars = DirectCast(oProgVars.Item("TCCEXCMOS"), FRRobot.FRCVars)
                    oSharedArray = DirectCast(oStructure.Item("DVALVE_NAM"), FRRobot.FRCVars)
                    oGroupArray = DirectCast(oStructure.Item("GVALVE_NAM"), FRRobot.FRCVars)
                Else
                    rRobot.ProgramName = "PAVRCCIO"
                    oProgVars = rRobot.ProgramVars
                    oSharedArray = DirectCast(oProgVars.Item("DVALVE_NAM"), FRRobot.FRCVars)
                    oGroupArray = DirectCast(oProgVars.Item("GVALVE_NAM"), FRRobot.FRCVars)
                End If
                Dim sSuffix As String = String.Empty
                If b_CAP Then
                    sSuffix = "_CAP"
                End If
                ReDim sShared(oSharedArray.Count)
                ReDim sGroup(oGroupArray.Count)
                For i = 0 To oSharedArray.Count - 1
                    oVar = DirectCast(oSharedArray.Item(, i), FRRobot.FRCVar)
                    If oVar.IsInitialized Then
                        sShared(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString & sSuffix, rRobot)
                    Else
                        sShared(i) = String.Empty
                    End If
                Next
                For i = 0 To oGroupArray.Count - 1
                    oVar = DirectCast(oGroupArray.Item(, i), FRRobot.FRCVar)
                    If oVar.IsInitialized Then
                        sGroup(i) = sGetResTxtFrmRobotTxt(oVar.Value.ToString & sSuffix, rRobot)
                    Else
                        sGroup(i) = String.Empty
                    End If
                Next

            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
#End Region


End Module  'mCCCommon

