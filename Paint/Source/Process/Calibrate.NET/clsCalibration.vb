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
' Form/Module: clsCalibration.vb
'
' Description: Calibration data
' 
'
' Dependancies:  mApplicators
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
' 05/29/09      MSW     New class to manage cal data
' 04/30/10      MSW     change cal status for "by color" to read from AF_DATA structure	
' 11/12/10      MSW     Add Scale to AA per_tol_band
' 11/17/10      MSW     Change scale from 100 to 10
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
'********************************************************************************************
'Description: calibration class
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsCalibration
#Region " Declares"

    '***** Module Constants  *******************************************************************
    Private Const msMODULE As String = "clsCalibration"
    '***** End Module Constants   **************************************************************

    Public Structure tCalPoint
        Dim CMD As clsSngValue
        Dim REF_OUT As clsIntValue
        Dim OUTPUT As clsIntValue
        Dim SCALE As clsSngValue
        Dim nOutletPressure As clsIntValue
        Dim nManifoldPressure As clsIntValue
    End Structure
    Private mCalTable() As tCalPoint     'Cal Table
    Private mnNumPoints As clsIntValue   'Usually 10, but technically it's adjustable 
    'Booleans enable the extra lines beyonf CMD and REF_OUT
    Private mbDyn As Boolean = False
    Private mb2KCal As Boolean = False
    Private mbManifoldPressure As Boolean = False
    Private mbOutletPressure As Boolean = False
    Private mfMinEngUnits As Single         'Max setpoint in engineering units
    Private mfMaxEngUnits As Single         'Min setpoint in engineering units
    Private mnMinCount As Integer           'Min counts
    Private mnMaxCount As Integer           'Max counts
    'Parameter details
    Private msParmName As String            'Parameter name
    Private msUnits As String               'Setpoint units
    Private mnParmNum As Integer = -1       'Parm number 
    Private mnValve As Integer = -1         'Valve number, if used
    Private mCntrType As eCntrType          'Control type
    Private mCalSource As clsIntValue       'Cal source
    Private mCalStatus As clsIntValue       'Cal status
    Private mDQCalStatus As clsIntValue     'Cal status for DQ because they couldn't just use the same constants
    Private mCalDate As clsIntValue         'Date
    Private mnChannel As Integer            'Used as a pointer from parm structure to ts or dq data
    'DQ PSI conversion vars and PSI Range
    Private mnDQMinCount As Integer
    Private mnDQMaxCount As Integer
    Private mnDQMinPSI As Integer
    Private mnDQMaxPSI As Integer
    Private mfDQCountToPSI As Single
    Private msColor As String
    Private mnZoneNumber As Integer
    Private msZoneName As String
    Private mnResMinCnt As Integer = 0
    Private mnHdrMinCnt As Integer = 0
    Private mnResScale As Single = 1.0
    Private mnHdrScale As Single = 1.0

    Public Structure tCalParam
        Dim IntValue As clsIntValue
        Dim SngValue As clsSngValue
        Dim txtValue As clsTextValue
        Dim BoolValue As clsBoolValue
        Dim nCol As Integer
        Dim nItem As Integer
        Dim Valuetype As TypeCode
        Dim CaptionText As String
    End Structure
    Private mCalParams() As tCalParam
    Private sColHeading() As String
    'Param Settings
#End Region

#Region " Properties"

    Friend Property CalParam(ByVal index As Integer) As tCalParam
        '********************************************************************************************
        'Description:  Calibration parameters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mCalParams Is Nothing Then
                Return Nothing
            Else
                Return mCalParams(index)
            End If
        End Get
        Set(ByVal value As tCalParam)
            mCalParams(index) = value
        End Set
    End Property
    Friend ReadOnly Property NumCalParams() As Integer
        '********************************************************************************************
        'Description:  number of cal params
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mCalParams Is Nothing Then
                Return 0
            Else
                Return mCalParams.GetUpperBound(0) + 1
            End If
        End Get
    End Property
    Friend ReadOnly Property ColLabel(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  Calibration parameter column labels
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If sColHeading Is Nothing Then
                Return String.empty
            Else
                Return sColHeading(index)
            End If
        End Get

    End Property
    Friend ReadOnly Property NumCols() As Integer
        '********************************************************************************************
        'Description:  number of colums of cal params
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If sColHeading Is Nothing Then
                Return 0
            Else
                Return sColHeading.GetUpperBound(0) + 1
            End If
        End Get
    End Property

    Friend Property CalStatus() As clsIntValue
        '********************************************************************************************
        'Description:  cas status object
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalStatus
        End Get
        Set(ByVal value As clsIntValue)
            mCalStatus = value
        End Set
    End Property
    Friend ReadOnly Property CalStatusText() As String
        Get
            If mCntrType = eCntrType.APP_CNTR_DQ Then
                CalStatusText = GetCalStatusText(mDQCalStatus.Value)
            Else
                CalStatusText = GetCalStatusText(mCalStatus.Value)
            End If
        End Get
    End Property
            
    Public Property changed() As Boolean
        '********************************************************************************************
        'Description:  Track changes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If mCalDate Is Nothing Then
                Return False
            End If
            If mCalDate.Changed Then
                Return True
            End If
            If mCalStatus.Changed Then
                Return True
            End If
            'If mCalSource.Changed Then
            '    Return True
            'End If
            If mnNumPoints.Changed Then
                Return True
            End If
            If Not (mCalParams Is Nothing) Then
                For Each CalParam As tCalParam In mCalParams
                    Select Case CalParam.Valuetype
                        Case TypeCode.Int32
                            If CalParam.IntValue.Changed Then
                                Return True
                            End If
                        Case TypeCode.Single
                            If CalParam.SngValue.Changed Then
                                Return True
                            End If
                        Case TypeCode.String
                            If CalParam.txtValue.Changed Then
                                Return True
                            End If
                        Case TypeCode.Boolean
                            If CalParam.BoolValue.Changed Then
                                Return True
                            End If
                    End Select
                Next
            End If

            For Each oTmp As tCalPoint In mCalTable
                If oTmp.CMD.Changed Then
                    Return True
                End If
                If oTmp.OUTPUT.Changed Then
                    Return True
                End If
                If mb2KCal Then
                    If oTmp.SCALE.Changed Then
                        Return True
                    End If
                End If
                If mbDyn Then
                    If oTmp.REF_OUT.Changed Then
                        Return True
                    End If
                End If
                If mbOutletPressure Then
                    If oTmp.nOutletPressure.Changed Then
                        Return True
                    End If
                End If
                If mbManifoldPressure Then
                    If oTmp.nManifoldPressure.Changed Then
                        Return True
                    End If
                End If
            Next
        End Get
        Set(ByVal value As Boolean)
            If value Then
                mCalDate.Update()
                mCalStatus.Update()
                mCalSource.Update()
                mnNumPoints.Update()
                If Not (mCalParams Is Nothing) Then
                    For Each CalParam As tCalParam In mCalParams
                        Select Case CalParam.Valuetype
                            Case TypeCode.Int32
                                CalParam.IntValue.Update()
                            Case TypeCode.Single
                                CalParam.SngValue.Update()
                            Case TypeCode.String
                                CalParam.txtValue.Update()
                            Case TypeCode.Boolean
                                CalParam.BoolValue.Update()
                        End Select
                    Next
                End If
                For Each oTmp As tCalPoint In mCalTable
                    oTmp.CMD.Update()
                    oTmp.OUTPUT.Update()
                    oTmp.REF_OUT.Update()
                    If mb2KCal Then
                        oTmp.SCALE.Update()
                    End If
                    If mbOutletPressure Then
                        oTmp.nOutletPressure.Update()
                    End If
                    If mbManifoldPressure Then
                        oTmp.nManifoldPressure.Update()
                    End If
                Next
            Else
                'Can't really set it false
            End If
        End Set
    End Property

    Public Property Color() As String
        '********************************************************************************************
        'Description:  Color name
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msColor
        End Get
        Set(ByVal value As String)
            msColor = value
        End Set
    End Property

    Public Property ZoneName() As String
        '********************************************************************************************
        'Description:  Zone name
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

    Public Property ZoneNumber() As Integer
        '********************************************************************************************
        'Description:  Zone name
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
        Set(ByVal value As Integer)
            mnZoneNumber = value
        End Set
    End Property
    Public Property Name() As String
        '********************************************************************************************
        'Description:  Parameter name
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msParmName
        End Get
        Set(ByVal value As String)
            msParmName = value
        End Set
    End Property
    Public Property Units() As String
        '********************************************************************************************
        'Description:  Parameter Units
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msUnits
        End Get
        Set(ByVal value As String)
            msUnits = value
        End Set
    End Property
    Public Property DQMaxPSI() As Integer
        '********************************************************************************************
        'Description:  max value for PSI Column
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnDQMaxPSI
        End Get
        Set(ByVal value As Integer)
            mnDQMaxPSI = value
        End Set
    End Property
    Public Property DQMinPSI() As Integer
        '********************************************************************************************
        'Description:  min value for PSI Column
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnDQMinPSI
        End Get
        Set(ByVal value As Integer)
            mnDQMinPSI = value
        End Set
    End Property
    Public Property ParmNum() As Integer
        '********************************************************************************************
        'Description:  Parameter Number
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnParmNum
        End Get
        Set(ByVal value As Integer)
            mnParmNum = value
        End Set
    End Property
    Public Property Valve() As Integer
        '********************************************************************************************
        'Description:  Parameter Number
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnValve
        End Get
        Set(ByVal value As Integer)
            mnValve = value
        End Set
    End Property
    Friend Property MinCount() As Integer
        '********************************************************************************************
        'Description:  min counts for parameter       '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMinCount
        End Get
        Set(ByVal value As Integer)
            mnMinCount = value
        End Set
    End Property
    Friend Property MaxCount() As Integer
        '********************************************************************************************
        'Description:  max counts for parameter
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxCount
        End Get
        Set(ByVal value As Integer)
            mnMaxCount = value
        End Set
    End Property
    Friend Property MinEngUnit() As Single
        '********************************************************************************************
        'Description:  min eng. units for parameter        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfMinEngUnits
        End Get
        Set(ByVal value As Single)
            mfMinEngUnits = value
        End Set
    End Property
    Friend Property MaxEngUnit() As Single
        '********************************************************************************************
        'Description:  max eng. units for parameter
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mfMaxEngUnits
        End Get
        Set(ByVal value As Single)
            mfMaxEngUnits = value
        End Set
    End Property
    Friend Property NumCalPoints() As Integer
        '********************************************************************************************
        'Description:  number of cal points
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumPoints.Value
        End Get
        Set(ByVal value As Integer)
            mnNumPoints.Value = value
            ReDim Preserve mCalTable(mnNumPoints.Value)
        End Set
    End Property
    Friend Property DynamicTable() As Boolean
        '********************************************************************************************
        'Description:  True for dynamic cal table (show dynamic line)
        '           accu... = true, everything else = false
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbDyn
        End Get
        Set(ByVal value As Boolean)
            mbDyn = value
        End Set
    End Property
    Friend Property IPC2KTable() As Boolean
        '********************************************************************************************
        'Description:  True for IPC 2k cal table 
        '           accu... = true, everything else = false
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mb2KCal
        End Get
        Set(ByVal value As Boolean)
            mb2KCal = value
        End Set
    End Property
    Friend Property ManifoldPressure() As Boolean
        '********************************************************************************************
        'Description:  True for DQ if it's using manifold pressure feedbcak
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbManifoldPressure
        End Get
        Set(ByVal value As Boolean)
            mbManifoldPressure = value
        End Set
    End Property
    Friend Property OutletPressure() As Boolean
        '********************************************************************************************
        'Description:  True for DQ if it's using outlet pressure feedbcak
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbOutletPressure
        End Get
        Set(ByVal value As Boolean)
            mbOutletPressure = value
        End Set
    End Property
    Friend Property CalSource() As eCalSource
        '********************************************************************************************
        'Description:  cal source for parameter       '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(mCalSource.Value, eCalSource)
        End Get
        Set(ByVal value As eCalSource)
            mCalSource.Value = CType(value, Integer)
        End Set
    End Property
    Friend Property CntrType() As eCntrType
        '********************************************************************************************
        'Description:  cal source for parameter       '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCntrType
        End Get
        Set(ByVal value As eCntrType)
            mCntrType = value
        End Set
    End Property
    Friend Property CalPoint(ByVal index As Integer) As tCalPoint
        '********************************************************************************************
        'Description:  cal point (index)      
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index)
        End Get
        Set(ByVal value As tCalPoint)
            mCalTable(index) = value
        End Set
    End Property
    Friend Property CMD(ByVal index As Integer) As clsSngValue
        '********************************************************************************************
        'Description:  cal point (index) Setpoint value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).CMD
        End Get
        Set(ByVal value As clsSngValue)
            mCalTable(index).CMD = value
        End Set
    End Property
    Friend Property SCALE(ByVal index As Integer) As clsSngValue
        '********************************************************************************************
        'Description:  cal point (index) SCALE value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).SCALE
        End Get
        Set(ByVal value As clsSngValue)
            mCalTable(index).SCALE = value
        End Set
    End Property
    Friend Property REF_OUT(ByVal index As Integer) As clsIntValue
        '********************************************************************************************
        'Description:  cal point (index) org value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).REF_OUT
        End Get
        Set(ByVal value As clsIntValue)
            mCalTable(index).REF_OUT = value
        End Set
    End Property
    Friend Property OUTPUT(ByVal index As Integer) As clsIntValue
        '********************************************************************************************
        'Description:  cal point (index) Dynamic value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).OUTPUT
        End Get
        Set(ByVal value As clsIntValue)
            mCalTable(index).OUTPUT = value
        End Set
    End Property
    Friend Property OutletPressurePSI(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  cal point (index) outlet pressure value
        '               stored as counts, convert to or fromm PSI
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim nTmp As Integer = (CInt((mCalTable(index).nOutletPressure.Value - mnDQMinCount) * mfDQCountToPSI) + mnDQMinPSI)
            If nTmp < 0 Then
                Return 0
            Else
                Return nTmp
            End If
        End Get
        Set(ByVal value As Integer)
            mCalTable(index).nOutletPressure.Value = CInt((value - mnDQMinPSI) / mfDQCountToPSI) + mnDQMinCount
        End Set
    End Property
    Friend Property ManifoldPressurePSI(ByVal index As Integer) As Integer
        '********************************************************************************************
        'Description:  cal point (index) outlet pressure value
        '               stored as counts, convert to or fromm PSI
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Dim nTmp As Integer = (CInt((mCalTable(index).nManifoldPressure.Value - mnDQMinCount) * mfDQCountToPSI) + mnDQMinPSI)
            If nTmp < 0 Then
                Return 0
            Else
                Return nTmp
            End If
        End Get
        Set(ByVal value As Integer)
            mCalTable(index).nManifoldPressure.Value = CInt((value - mnDQMinPSI) / mfDQCountToPSI) + mnDQMinCount
        End Set
    End Property
    Friend Property OutletPressure(ByVal index As Integer) As clsIntValue
        '********************************************************************************************
        'Description:  cal point (index) outlet pressure value
        '               stored as counts, convert to or fromm PSI
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).nOutletPressure
        End Get
        Set(ByVal value As clsIntValue)
            mCalTable(index).nOutletPressure = value
        End Set
    End Property
    Friend Property ManifoldPressure(ByVal index As Integer) As clsIntValue
        '********************************************************************************************
        'Description:  cal point (index) outlet pressure value
        '               stored as counts, convert to or fromm PSI
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCalTable(index).nManifoldPressure
        End Get
        Set(ByVal value As clsIntValue)
            mCalTable(index).nManifoldPressure = value
        End Set
    End Property
#End Region
#Region " Routines"
    Friend Sub DrawGraph(ByVal rPanel As Panel, ByVal e As System.Windows.Forms.PaintEventArgs)
        '********************************************************************************************
        'Description:  draw a cal table
        '
        'Parameters: panel object and paint event
        'Returns:
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/05/10  MSW     Prevent math errors from badc cal data
        '********************************************************************************************
        Try

            Const nNUM_LINES As Integer = 5
            'Available panel space
            Dim nPnlHeight As Integer = rPanel.Height
            Dim nPnlWidth As Integer = rPanel.Width
            'Grid dimensions
            Dim nGrphHeight As Integer = CInt(nPnlHeight * 0.8)
            Dim nGrphWidth As Integer = CInt(nPnlWidth * 0.8)
            Dim ptOrigin As Point = New Point(CInt(nPnlWidth * 0.1), CInt(nPnlHeight * 0.9))
            Dim nRight As Integer = ptOrigin.X + nGrphWidth
            Dim nTop As Integer = ptOrigin.Y - nGrphHeight
            'Scale units to pixels or whatever unit we're drawing in
            Dim nXMax As Integer = CInt(mCalTable(mCalTable.GetUpperBound(0)).OUTPUT.Value)
            If mbDyn Or mb2KCal Then
                If nXMax < mCalTable(mCalTable.GetUpperBound(0)).REF_OUT.Value Then
                    nXMax = mCalTable(mCalTable.GetUpperBound(0)).REF_OUT.Value
                End If
            End If
            Dim nYMax As Integer = CInt(mCalTable(mCalTable.GetUpperBound(0)).CMD.Value)
            If mb2KCal Then
                If nYMax < mCalTable(mCalTable.GetUpperBound(0)).SCALE.Value Then
                    nYMax = CInt(mCalTable(mCalTable.GetUpperBound(0)).SCALE.Value)
                End If
            End If
            Dim nX2Max As Integer = 0
            If mbManifoldPressure Then
                If nX2Max < DQCountsToPSI(mCalTable(mCalTable.GetUpperBound(0)).nManifoldPressure.Value) Then
                    nX2Max = DQCountsToPSI(mCalTable(mCalTable.GetUpperBound(0)).nManifoldPressure.Value)
                End If
            End If
            If mbOutletPressure Then
                If nX2Max < DQCountsToPSI(mCalTable(mCalTable.GetUpperBound(0)).nOutletPressure.Value) Then
                    nX2Max = DQCountsToPSI(mCalTable(mCalTable.GetUpperBound(0)).nOutletPressure.Value)
                End If
            End If
            If nYMax < 1 Then
                nYMax = 1
            End If
            If nXMax < 1 Then
                nXMax = 1
            End If
            Dim fXScale As Double = nGrphWidth / nXMax
            Dim fYScale As Double = nGrphHeight / nYMax
            'Grid pens
            Dim oPenBorder As Pen = New Pen(Drawing.Color.Black, 3)
            Dim oPengrid As Pen = New Pen(Drawing.Color.Black, 1)
            Dim oPenCounts As Pen = New Pen(Drawing.Color.Black, 1)
            Dim oPenPSIs As Pen = New Pen(Drawing.Color.Black, 1)
            'Grid lines, go for about 5 of them 
            'This'll get rounded spacing

            Dim nVertGridLineSpacing As Integer = CInt(nYMax / 250) * 50
            Dim nHorzGridLineSpacing As Integer = CInt(nXMax / 250) * 50
            Dim nX2Spacing As Integer = 30
            If nX2Max < 100 Then
                nX2Spacing = 20
            ElseIf nX2Max < 150 Then
                nX2Spacing = 30
            Else
                nX2Spacing = CInt(nX2Max / 250) * 50
            End If
            Dim fX2Scale As Double = fXScale * nHorzGridLineSpacing / nX2Spacing
            'Find where the last grid line is, adjust the grid size to match
            Dim x As Single = ptOrigin.X + CInt(nHorzGridLineSpacing * fXScale * nNUM_LINES)
            Dim y As Single = ptOrigin.Y - CInt(nVertGridLineSpacing * fYScale * nNUM_LINES)
            If x > nRight Then
                nRight = CInt(x)
            End If
            If y < nTop Then
                nTop = CInt(y)
            End If
            'Draw the axis
            e.Graphics.DrawLine(oPenBorder, ptOrigin.X, ptOrigin.Y, nRight, ptOrigin.Y)
            e.Graphics.DrawLine(oPenBorder, ptOrigin.X, ptOrigin.Y, ptOrigin.X, nTop)
            ' Create a font object for the scales.
            Dim aFont As New System.Drawing.Font("Arial", 8)
            Dim sText As String = String.Empty
            Dim txtsize As SizeF
            Dim lblLeft As Integer = ptOrigin.X
            For nLine As Integer = 1 To nNUM_LINES
                x = ptOrigin.X + CInt(nHorzGridLineSpacing * fXScale * nLine)
                y = ptOrigin.Y - CInt(nVertGridLineSpacing * fYScale * nLine)
                'vertical gridline
                e.Graphics.DrawLine(oPengrid, x, ptOrigin.Y, x, nTop)
                'x-axis labels
                sText = (nHorzGridLineSpacing * nLine).ToString
                txtsize = e.Graphics.MeasureString(sText, aFont)

                If mbManifoldPressure Or mbManifoldPressure Then
                    'color code the x - axis labels for DQ
                    e.Graphics.DrawString(sText, aFont, Brushes.Green, (x - txtsize.Width / 2), (ptOrigin.Y + 2))
                    'psi labels
                    sText = (nX2Spacing * nLine).ToString
                    txtsize = e.Graphics.MeasureString(sText, aFont)
                    e.Graphics.DrawString(sText, aFont, Brushes.Blue, (x - txtsize.Width / 2), (nTop - txtsize.Height))
                Else
                    e.Graphics.DrawString(sText, aFont, Brushes.Black, (x - txtsize.Width / 2), (ptOrigin.Y + 2))
                End If
                'horizontal gridline
                e.Graphics.DrawLine(oPengrid, ptOrigin.X, y, nRight, y)
                'Y-axis labels
                sText = (nVertGridLineSpacing * nLine).ToString
                txtsize = e.Graphics.MeasureString(sText, aFont)
                e.Graphics.DrawString(sText, aFont, Brushes.Black, ptOrigin.X - txtsize.Width - 2, (y - txtsize.Height / 2))
                If (lblLeft > (ptOrigin.X - txtsize.Width - 2)) Then
                    lblLeft = CInt(ptOrigin.X - txtsize.Width - 2)
                End If
            Next
            'Counts label at the bottom
            If mb2KCal Then
                sText = gpsRM.GetString("psPSI")
            Else
                sText = gpsRM.GetString("psCOUNTS")
            End If
            aFont = New System.Drawing.Font("Arial", 11, FontStyle.Bold)
            txtsize = e.Graphics.MeasureString(sText, aFont)
            If mbManifoldPressure Or mbOutletPressure Then
                'color code the axis labels for DQ
                e.Graphics.DrawString(sText, aFont, Brushes.Green, (ptOrigin.X + nRight - txtsize.Width) / 2, ptOrigin.Y + txtsize.Height)
            Else
                e.Graphics.DrawString(sText, aFont, Brushes.Black, (ptOrigin.X + nRight - txtsize.Width) / 2, ptOrigin.Y + txtsize.Height)
            End If
            'Parm name at the top
            sText = msParmName
            aFont = New System.Drawing.Font("Arial", 13, FontStyle.Bold)
            txtsize = e.Graphics.MeasureString(sText, aFont)
            e.Graphics.DrawString(sText, aFont, Brushes.Black, (ptOrigin.X + nRight - txtsize.Width) / 2, 0)
            If mbManifoldPressure Or mbManifoldPressure Then
                'psi label
                sText = gpsRM.GetString("psPSI")
                aFont = New System.Drawing.Font("Arial", 11, FontStyle.Bold)
                txtsize = e.Graphics.MeasureString(sText, aFont)
                e.Graphics.DrawString(sText, aFont, Brushes.Blue, ((ptOrigin.X + nRight - txtsize.Width) / 2), (nTop - txtsize.Height))
            End If
            'units on the y-axis.  
            sText = msUnits
            aFont = New System.Drawing.Font("Arial", 11, FontStyle.Bold)
            txtsize = e.Graphics.MeasureString(sText, aFont)
            'This part gets goofy.  First save the existing coordinate system
            Dim mtrxTmp As Matrix = e.Graphics.Transform
            'Move the origin to where the label will be
            e.Graphics.TranslateTransform((lblLeft - (txtsize.Height / 2)), CSng((ptOrigin.Y + nTop) / 2))
            'rotate back 90 degrees
            e.Graphics.RotateTransform(-90)
            'draw the word sideways
            e.Graphics.DrawString(sText, aFont, Brushes.Black, ((0 - txtsize.Width) / 2), (0 - txtsize.Width / 2))
            'restore the old coordinates
            e.Graphics.Transform = mtrxTmp

            Dim lineStd As PointF()
            ReDim lineStd(mCalTable.GetUpperBound(0))
            Dim lineOrg As PointF()
            Dim lineMP As PointF()
            Dim lineOP As PointF()
            'If mbDyn Then
            ReDim lineOrg(mCalTable.GetUpperBound(0))
            'End If
            'If mbManifoldPressure Then
            ReDim lineMP(mCalTable.GetUpperBound(0))
            'End If
            'If mbOutletPressure Then
            ReDim lineOP(mCalTable.GetUpperBound(0))
            'End If
            For nPoint As Integer = 0 To mCalTable.GetUpperBound(0)
                With mCalTable(nPoint)
                    lineStd(nPoint).X = ptOrigin.X + CSng(.OUTPUT.Value * fXScale)
                    lineStd(nPoint).Y = ptOrigin.Y - CSng(.CMD.Value * fYScale)
                    If mbDyn Then
                        lineOrg(nPoint).X = ptOrigin.X + CSng(.REF_OUT.Value * fXScale)
                        lineOrg(nPoint).Y = ptOrigin.Y - CSng(.CMD.Value * fYScale)
                    End If
                    If mb2KCal Then
                        lineOrg(nPoint).X = ptOrigin.X + CSng(.REF_OUT.Value * fXScale)
                        lineOrg(nPoint).Y = ptOrigin.Y - CSng(.SCALE.Value * fYScale)
                    End If
                    If mbManifoldPressure Then
                        lineMP(nPoint).X = ptOrigin.X + CSng(DQCountsToPSI(.nManifoldPressure.Value) * fX2Scale)
                        lineMP(nPoint).Y = ptOrigin.Y - CSng(.CMD.Value * fYScale)
                    End If
                    If mbOutletPressure Then
                        lineOP(nPoint).X = ptOrigin.X + CSng(DQCountsToPSI(.nOutletPressure.Value) * fX2Scale)
                        lineOP(nPoint).Y = ptOrigin.Y - CSng(.CMD.Value * fYScale)
                    End If
                End With
            Next
            Dim oPenLine As Pen = New Pen(Drawing.Color.Green, 2)
            e.Graphics.DrawLines(oPenLine, lineStd)
            If mbDyn Or mb2KCal Then
                oPenLine.Color = Drawing.Color.DarkCyan
                e.Graphics.DrawLines(oPenLine, lineOrg)
            End If
            If mbManifoldPressure Then
                oPenLine.Color = Drawing.Color.Blue
                e.Graphics.DrawLines(oPenLine, lineMP)
            End If
            If mbOutletPressure Then
                oPenLine.Color = Drawing.Color.Blue
                If mbManifoldPressure Then ' separate the ines
                    oPenLine.DashStyle = DashStyle.Dot
                End If
                e.Graphics.DrawLines(oPenLine, lineOP)
            End If
        Catch ex As Exception
            Dim sStatus As String = String.Empty
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE, _
                    sStatus, MessageBoxButtons.OK)
        End Try

    End Sub

    Friend Function LoadFromRobot(ByRef Arm As clsArm, _
                          Optional ByVal nParam As Integer = -1, _
                          Optional ByVal nValve As Integer = -1) As Boolean
        '********************************************************************************************
        'Description:  Load a cal table
        '
        'Parameters: Applicator config, robot, parameter number, and valve number
        'Returns:    true = success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/30/10  MSW     change cal status for "by color" to read from AF_DATA structure
        ' 11/12/10  MSW     Add Scale to AA per_tol_band
        ' 11/17/10  MSW     Change scale from 100 to 10
        '********************************************************************************************
        'Check optional parms, valve and param# could already be set
        Try
            If nParam = -1 Then
                nParam = mnParmNum
            Else
                mnParmNum = nParam
            End If
            If nValve = -1 Then
                nValve = mnValve
            Else
                mnValve = nValve
            End If
            If Arm.IsOnLine = False Then
                Return False
            End If
            'Init the value class objects
            mCalSource = New clsIntValue()
            mCalStatus = New clsIntValue()
            mCalDate = New clsIntValue()
            mnNumPoints = New clsIntValue()
            mCalParams = Nothing
            'get con_prm_desc structure
            Dim sProg As String
            Dim sVar As String
            Dim nParamOnRobot As Integer 'To account for dualarm indexing.  Still start at 0, though
            If Arm.Controller.Version >= 7.3 Then
                'NGIP parm config
                nParamOnRobot = nParam
            Else
                'Dualarm or older parm config
                If Arm.ArmNumber > 1 Then
                    nParamOnRobot = nParam + 6
                Else
                    nParamOnRobot = nParam
                End If
            End If
            'Where is the cal table
            sProg = "PAVRPARM"
            'Find our way to the parm data
            Arm.ProgramName = sProg
            Dim oFile As FRRobot.FRCVars = Arm.ProgramVars
            'First get con_parm_desc structure
            'Muiltiarm variable setup varies by version
            Dim oStruct As FRRobot.FRCVars
            Dim oStruct3 As FRRobot.FRCVars
            If Arm.Controller.Version >= 7.3 Then
                'NGIP parm config
                Dim oTmp As FRRobot.FRCVars = _
                        DirectCast(oFile.Item("TPARMEQ"), FRRobot.FRCVars)
                Dim oTmp1 As FRRobot.FRCVars = _
                        DirectCast(oTmp.Item(Arm.ArmNumber.ToString()), FRRobot.FRCVars)
                oStruct = DirectCast(oTmp1.Item("CON_PRM_DESC"), FRRobot.FRCVars)
                oStruct3 = DirectCast(oTmp1.Item("CON_PRM_DES3"), FRRobot.FRCVars)
            Else
                'Dualarm or older parm config
                oStruct = DirectCast(oFile.Item("CON_PRM_DESC"), FRRobot.FRCVars)
                oStruct3 = DirectCast(oFile.Item("CON_PRM_DES3"), FRRobot.FRCVars)
            End If

            'Parm data
            Dim oParmData As FRRobot.FRCVars = _
                        DirectCast(oStruct.Item(, (nParamOnRobot)), FRRobot.FRCVars)
            Dim oParmData3 As FRRobot.FRCVars = _
                        DirectCast(oStruct3.Item(, (nParamOnRobot)), FRRobot.FRCVars)
            'read from the robot once
            oParmData.NoRefresh = True
            oParmData3.NoRefresh = True

            Dim oVar As FRRobot.FRCVar
            'get applicator settings
            oVar = CType(oParmData.Item("APP_CON_MIN"), FRRobot.FRCVar)
            mfMinEngUnits = CInt(oVar.Value)
            oVar = CType(oParmData.Item("APP_CON_MAX"), FRRobot.FRCVar)
            mfMaxEngUnits = CInt(oVar.Value)
            oVar = CType(oParmData.Item("APP_MIN_OUT"), FRRobot.FRCVar)
            mnMinCount = CInt(oVar.Value)
            oVar = CType(oParmData.Item("APP_MAX_OUT"), FRRobot.FRCVar)
            mnMaxCount = CInt(oVar.Value)
            oVar = CType(oParmData.Item("APP_MAX_OUT"), FRRobot.FRCVar)
            mnMaxCount = CInt(oVar.Value)
            oVar = CType(oParmData.Item("CAL_SOURCE"), FRRobot.FRCVar)
            mCalSource.Value = CInt(oVar.Value)
            oVar = CType(oParmData.Item("APP_CON_TYPE"), FRRobot.FRCVar)
            Dim nOutType As Integer = CInt(oVar.Value)

            If nOutType = CInt(eOutType.APP_CON_DIR) Then
                'Special case for IPC cal
                'Adding a couple checks to verify IPC cal feature, otherwise default to no cal
                Try
                    Arm.ProgramName = "pavroptn"
                    Dim sPrefix2 As String = String.Empty
                    If Arm.Controller.Version >= 7.3 Then
                        sPrefix2 = "BYOPTN[" & Arm.ArmNumber.ToString & "]."
                    Else
                        sPrefix2 = ""
                    End If
                    Arm.VariableName = sPrefix2 & "BYOP[40]"
                    Dim nVal As Integer = CInt(Arm.VarValue)
                    If nVal = 1 Then
                        mCalSource.Value = CInt(eCalSource.CAL_SRC_IPC_CAL)
                    End If
                    'I don't know why he didn't set the variable to cal by color, but he didn't, so we'll make our own value to use on this screen.
                Catch ex As Exception

                End Try
            End If
            oVar = CType(oParmData.Item("CAL_UNITS"), FRRobot.FRCVar)
            msUnits = oVar.Value.ToString
            oVar = CType(oParmData3.Item("APP_CNTR_TYP"), FRRobot.FRCVar)
            mCntrType = CType(oVar.Value, eCntrType)
            oVar = CType(oParmData.Item("APP_CON_TITL"), FRRobot.FRCVar)
            msParmName = oVar.Value.ToString
            If msParmName.StartsWith("Eq") Then
                msParmName = msParmName.Substring(4)
            Else
                'The robots parm names don't always have the same format. This handles the typical setup
                If msParmName.Contains("#" & Arm.ArmNumber.ToString) Then
                    msParmName = msParmName.Substring(0, msParmName.Length - 3)
                Else
                    If msParmName = "Fluid Flow2" Then
                        msParmName = "Fluid Flow"
                    End If
                End If
            End If
            Select Case mCntrType
                Case eCntrType.APP_CNTR_DQ
                    mbOutletPressure = True
                    mbManifoldPressure = True
                    mbDyn = False
                    mb2KCal = False
                Case eCntrType.APP_CNTR_AF, eCntrType.APP_CNTR_AS
                    mbOutletPressure = False
                    mbManifoldPressure = False
                    mbDyn = True
                    mb2KCal = False
                Case eCntrType.APP_CNTR_AA 'MSW 11/05/09 new shaping air
                    mbOutletPressure = False
                    mbManifoldPressure = False
                    mbDyn = Not (Arm.Controller.Version >= 7.3)
                    mb2KCal = False
                Case Else
                    mbOutletPressure = False
                    mbManifoldPressure = False
                    mbDyn = False
                    mb2KCal = False
            End Select
            mb2KCal = (eCalSource.CAL_SRC_IPC_CAL = CalSource)

            'Map a cal table var based on calibration type
            Dim oCal As FRRobot.FRCVars
            Select Case CalSource
                Case eCalSource.CAL_SRC_NO, eCalSource.CAL_SRC_NR, eCalSource.CAL_SRC_NA
                    'No cal table
                    Return False
                Case eCalSource.CAL_SRC_BC, eCalSource.CAL_SRC_IPC_CAL
                    If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                        'psi to counts scale
                        sProg = "PAVRIPC"
                        Arm.ProgramName = sProg
                        oFile = Arm.ProgramVars
                        'TIPCEQCFG[1].TPMPCFG[1].TSENSOR[2].PSI_TO_CNTS
                        'TIPCEQCFG[1].TPMPCFG[1].TSENSOR[1].TIO.MIN_CNTS
                        sVar = "TIPCEQCFG"
                        Dim oTIPCEQCFG As FRRobot.FRCVars = DirectCast(oFile.Item(sVar), FRRobot.FRCVars)
                        Dim o2 As FRRobot.FRCVars = DirectCast(oTIPCEQCFG.Item(Arm.ArmNumber.ToString), FRRobot.FRCVars)
                        sVar = "TPMPCFG"
                        Dim o3 As FRRobot.FRCVars = DirectCast(o2.Item(sVar), FRRobot.FRCVars)
                        'Read this whole structure
                        o3.NoRefresh = True
                        Dim oResinPmpCfg As FRRobot.FRCVars = DirectCast(o3.Item("1"), FRRobot.FRCVars)
                        Dim oHardenerPmpCfg As FRRobot.FRCVars = DirectCast(o3.Item("2"), FRRobot.FRCVars)
                        sVar = "TSENSOR"
                        Dim oResinSensors As FRRobot.FRCVars = DirectCast(oResinPmpCfg.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerSensors As FRRobot.FRCVars = DirectCast(oHardenerPmpCfg.Item(sVar), FRRobot.FRCVars)
                        Dim oResinInSensor As FRRobot.FRCVars = DirectCast(oResinSensors.Item("1"), FRRobot.FRCVars)
                        Dim oHardenerInSensor As FRRobot.FRCVars = DirectCast(oHardenerSensors.Item("1"), FRRobot.FRCVars)
                        Dim oResinOutSensor As FRRobot.FRCVars = DirectCast(oResinSensors.Item("2"), FRRobot.FRCVars)
                        Dim oHardenerOutSensor As FRRobot.FRCVars = DirectCast(oHardenerSensors.Item("2"), FRRobot.FRCVars)
                        sVar = "PSI_TO_CNTS"
                        Dim oResinScale As FRRobot.FRCVar = DirectCast(oResinOutSensor.Item(sVar), FRRobot.FRCVar)
                        Dim oHardenerScale As FRRobot.FRCVar = DirectCast(oHardenerOutSensor.Item(sVar), FRRobot.FRCVar)
                        sVar = "TIO"
                        Dim oResinInSensIO As FRRobot.FRCVars = DirectCast(oResinInSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerInSensIO As FRRobot.FRCVars = DirectCast(oHardenerInSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oResinOutSensIO As FRRobot.FRCVars = DirectCast(oResinOutSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerOutSensIO As FRRobot.FRCVars = DirectCast(oHardenerOutSensor.Item(sVar), FRRobot.FRCVars)
                        sVar = "MIN_CNTS"
                        Dim oResinOutSensMinCnt As FRRobot.FRCVar = DirectCast(oResinOutSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim oHardenerOutSensMinCnt As FRRobot.FRCVar = DirectCast(oHardenerOutSensIO.Item(sVar), FRRobot.FRCVar)

                        mnResMinCnt = CInt(oResinOutSensMinCnt.Value)
                        mnHdrMinCnt = CInt(oHardenerOutSensMinCnt.Value)
                        mnResScale = CType(oResinScale.Value, Single)
                        mnHdrScale = CType(oHardenerScale.Value, Single)


                        'fault parameters

                        sVar = "MAX_REQUNITS"
                        Dim oVarVal As FRRobot.FRCVar = DirectCast(oResinInSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim nResWarnPct As Integer = CInt(100 * CType(oVarVal.Value, Single))
                        oVarVal = DirectCast(oResinOutSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim nResFaultPct As Integer = CInt(100 * CType(oVarVal.Value, Single))
                        oVarVal = DirectCast(oHardenerInSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim nHdrWarnPct As Integer = CInt(100 * CType(oVarVal.Value, Single))
                        oVarVal = DirectCast(oHardenerOutSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim nHdrFaultPct As Integer = CInt(100 * CType(oVarVal.Value, Single))

                        ReDim mCalParams(3)
                        ReDim sColHeading(1)
                        sColHeading(0) = gpsRM.GetString("psRESIN")
                        sColHeading(1) = gpsRM.GetString("psHARDENER")
                        'nResWarnPct
                        mCalParams(0).Valuetype = TypeCode.Int32
                        mCalParams(0).IntValue = New clsIntValue
                        mCalParams(0).nCol = 0
                        mCalParams(0).nItem = 0
                        mCalParams(0).CaptionText = gpsRM.GetString("psWRNPCT")
                        mCalParams(0).IntValue.Value = nResWarnPct
                        mCalParams(0).IntValue.MinValue = 10
                        mCalParams(0).IntValue.MaxValue = 100
                        'nResFaultPct
                        mCalParams(1).Valuetype = TypeCode.Int32
                        mCalParams(1).IntValue = New clsIntValue
                        mCalParams(1).nCol = 0
                        mCalParams(1).nItem = 1
                        mCalParams(1).CaptionText = gpsRM.GetString("psFLTPCT")
                        mCalParams(1).IntValue.Value = nResFaultPct
                        mCalParams(1).IntValue.MinValue = 15
                        mCalParams(1).IntValue.MaxValue = 100
                        'nHdrWarnPct
                        mCalParams(2).Valuetype = TypeCode.Int32
                        mCalParams(2).IntValue = New clsIntValue
                        mCalParams(2).nCol = 1
                        mCalParams(2).nItem = 0
                        mCalParams(2).CaptionText = gpsRM.GetString("psWRNPCT")
                        mCalParams(2).IntValue.Value = nHdrWarnPct
                        mCalParams(2).IntValue.MinValue = 10
                        mCalParams(2).IntValue.MaxValue = 100
                        'nHdrFaultPct
                        mCalParams(3).Valuetype = TypeCode.Int32
                        mCalParams(3).IntValue = New clsIntValue
                        mCalParams(3).nCol = 1
                        mCalParams(3).nItem = 1
                        mCalParams(3).CaptionText = gpsRM.GetString("psFLTPCT")
                        mCalParams(3).IntValue.Value = nHdrFaultPct
                        mCalParams(3).IntValue.MinValue = 15
                        mCalParams(3).IntValue.MaxValue = 100
                    End If

                    'by color - pavrdbcv
                    sProg = "PAVRDBCV"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars

                    If nValve < 1 Then
                        nValve = 1
                    End If
                    sVar = "COLOR_DATA"
                    'Muiltiarm
                    If Arm.ArmNumber > 1 Then
                        'COLOR_DATA{Equip Num}
                        sVar = sVar & Arm.ArmNumber.ToString
                    End If
                    'COLOR_DATA{Equip Num}.NODEDATA[{VALVE_NO}]
                    'sVar = sVar & ".NODEDATA[" & nValve.ToString & "]"
                    '01/20/10 BTK The code didn't work when sVar was defined like it is in the line
                    'above.  Needed to break out the data.
                    'Valve data
                    Dim o As FRRobot.FRCVars = _
                        DirectCast(oFile.Item(sVar), FRRobot.FRCVars)
                    Dim oo As FRRobot.FRCVars = _
                            DirectCast(o.Item("NODEDATA"), FRRobot.FRCVars)
                    Dim oValveData As FRRobot.FRCVars = _
                                DirectCast(oo.Item(nValve.ToString), FRRobot.FRCVars)
                    'read from the robot once
                    oValveData.NoRefresh = True
                    'Cal Data - to be read later
                    oCal = DirectCast(oValveData.Item("CONTROL_CAL"), FRRobot.FRCVars)
                    'Cal size
                    If mb2KCal Then
                        mnNumPoints.Value = 5
                    Else
                        oVar = CType(oValveData.Item("CAL_SIZE"), FRRobot.FRCVar)
                        mnNumPoints.Value = CInt(oVar.Value)
                    End If

                    ReDim mCalTable(mnNumPoints.Value - 1)
                    'Cal status
                    If (mCntrType = eCntrType.APP_CNTR_AF) Or (mCntrType = eCntrType.APP_CNTR_AS) Then
                        Dim oAF_DATA As FRRobot.FRCVars = _
                        CType(oValveData.Item("AF_DATA"), FRRobot.FRCVars)
                        oVar = CType(oAF_DATA.Item("CAL_STATUS"), FRRobot.FRCVar)
                        mCalStatus.Value = CInt(oVar.Value)
                    Else
                        oVar = CType(oValveData.Item("CAL_STATUS"), FRRobot.FRCVar)
                        mCalStatus.Value = CInt(oVar.Value)
                    End If
                    'Cal status
                    oVar = CType(oValveData.Item("CAL_DATE"), FRRobot.FRCVar)
                    mCalDate.Value = CInt(oVar.Value)

                Case eCalSource.CAL_SRC_CAL, eCalSource.CAL_SRC_COP, eCalSource.CAL_SRC_SCAL
                    oCal = DirectCast(oParmData.Item("CONTROL_CAL"), FRRobot.FRCVars)
                    'Cal size
                    oVar = CType(oParmData.Item("APP_CAL_SIZE"), FRRobot.FRCVar)
                    mnNumPoints.Value = CInt(oVar.Value)
                    ReDim mCalTable(mnNumPoints.Value - 1)
                    'Cal status
                    oVar = CType(oParmData.Item("CAL_STATUS"), FRRobot.FRCVar)
                    mCalStatus.Value = CInt(oVar.Value)
                    'Cal status
                    oVar = CType(oParmData.Item("CAL_DATE"), FRRobot.FRCVar)
                    mCalDate.Value = CInt(oVar.Value)
                Case Else
                    'New features to program
                    Debug.Assert(False)
                    Return False
            End Select
            'Load the table
            Select Case CntrType
                Case eCntrType.APP_CNTR_DQ
                    'DQ cal data
                    sProg = "PAVRDQ"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars
                    Dim oTmp As FRRobot.FRCVars

                    'Find out if it's using each sensor
                    oStruct = DirectCast(oFile.Item("TDQ_IO"), FRRobot.FRCVars)
                    oTmp = DirectCast(oStruct.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item("OP_AI"), FRRobot.FRCVar)
                    mbOutletPressure = (CInt(oVar.Value) > 0)
                    oVar = CType(oTmp.Item("MP_AI"), FRRobot.FRCVar)
                    mbManifoldPressure = (CInt(oVar.Value) > 0)

                    'Get variables from the setup structure
                    oStruct = DirectCast(oFile.Item("TDQ_SETUP"), FRRobot.FRCVars)
                    oTmp = DirectCast(oStruct.Item("RCNTSTOPSI"), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVar)
                    mfDQCountToPSI = CSng(oVar.Value)
                    oTmp = DirectCast(oStruct.Item("IDQMINUNITS"), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVar)
                    mnDQMinPSI = CInt(oVar.Value)
                    oTmp = DirectCast(oStruct.Item("IDQMAXUNITS"), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVar)
                    mnDQMaxPSI = CInt(oVar.Value)
                    oTmp = DirectCast(oStruct.Item("IDQMINCNTS"), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVar)
                    mnDQMinCount = CInt(oVar.Value)
                    oTmp = DirectCast(oStruct.Item("IDQMAXCNTS"), FRRobot.FRCVars)
                    oVar = CType(oTmp.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVar)
                    mnDQMaxCount = CInt(oVar.Value)

                    'editable parameters                
                    ReDim sColHeading(0)
                    sColHeading(0) = gpsRM.GetString("psDQ_SETTINGS")
                    ReDim mCalParams(1)
                    'Grace time
                    mCalParams(0).Valuetype = TypeCode.Int32
                    mCalParams(0).IntValue = New clsIntValue
                    mCalParams(0).nCol = 0
                    mCalParams(0).nItem = 0
                    mCalParams(0).CaptionText = gpsRM.GetString("psDQ_GRACE_TIME")
                    oVar = DirectCast(oStruct.Item("IGRACETIME"), FRRobot.FRCVar)
                    mCalParams(0).IntValue.Value = CInt(oVar.Value)
                    mCalParams(0).IntValue.MinValue = 0
                    mCalParams(0).IntValue.MaxValue = 9999
                    'step delay
                    mCalParams(1).Valuetype = TypeCode.Int32
                    mCalParams(1).IntValue = New clsIntValue
                    mCalParams(1).nCol = 0
                    mCalParams(1).nItem = 1
                    mCalParams(1).CaptionText = gpsRM.GetString("psDQ_CAL_STEP_TIME")
                    oVar = DirectCast(oStruct.Item("ICALSTEPDEL"), FRRobot.FRCVar)
                    mCalParams(1).IntValue.Value = CInt(oVar.Value)
                    mCalParams(1).IntValue.MinValue = 1000
                    mCalParams(1).IntValue.MaxValue = 9999
                    Dim nCol As Integer = 0
                    'Manifold pressure
                    If mbManifoldPressure Then
                        nCol += 1
                        ReDim Preserve sColHeading(nCol)
                        sColHeading(nCol) = gpsRM.GetString("psDQ_MP")
                        ReDim Preserve mCalParams(2 * nCol + 1)
                        'Grace time
                        mCalParams(2 * nCol).Valuetype = TypeCode.Int32
                        mCalParams(2 * nCol).IntValue = New clsIntValue
                        mCalParams(2 * nCol).nCol = nCol
                        mCalParams(2 * nCol).nItem = 0
                        mCalParams(2 * nCol).CaptionText = gpsRM.GetString("psDQ_WARNING")
                        oVar = DirectCast(oStruct.Item("IMPWARNING"), FRRobot.FRCVar)
                        mCalParams(2 * nCol).IntValue.Value = CInt(oVar.Value)
                        mCalParams(2 * nCol).IntValue.MinValue = 1
                        mCalParams(2 * nCol).IntValue.MaxValue = 15
                        'step delay
                        mCalParams(2 * nCol + 1).Valuetype = TypeCode.Int32
                        mCalParams(2 * nCol + 1).IntValue = New clsIntValue
                        mCalParams(2 * nCol + 1).nCol = nCol
                        mCalParams(2 * nCol + 1).nItem = 1
                        mCalParams(2 * nCol + 1).CaptionText = gpsRM.GetString("psDQ_ALARM")
                        oVar = DirectCast(oStruct.Item("IMPALARM"), FRRobot.FRCVar)
                        mCalParams(2 * nCol + 1).IntValue.Value = CInt(oVar.Value)
                        mCalParams(2 * nCol + 1).IntValue.MinValue = 16
                        mCalParams(2 * nCol + 1).IntValue.MaxValue = 30
                    End If
                    'Outlet pressure
                    If mbOutletPressure Then
                        nCol += 1
                        ReDim Preserve sColHeading(nCol)
                        sColHeading(nCol) = gpsRM.GetString("psDQ_OP")
                        ReDim Preserve mCalParams(2 * nCol + 1)
                        'Grace time
                        mCalParams(2 * nCol).Valuetype = TypeCode.Int32
                        mCalParams(2 * nCol).IntValue = New clsIntValue
                        mCalParams(2 * nCol).nCol = nCol
                        mCalParams(2 * nCol).nItem = 0
                        mCalParams(2 * nCol).CaptionText = gpsRM.GetString("psDQ_WARNING")
                        oVar = DirectCast(oStruct.Item("IOPWARNING"), FRRobot.FRCVar)
                        mCalParams(2 * nCol).IntValue.Value = CInt(oVar.Value)
                        mCalParams(2 * nCol).IntValue.MinValue = 1
                        mCalParams(2 * nCol).IntValue.MaxValue = 15
                        'step delay
                        mCalParams(2 * nCol + 1).Valuetype = TypeCode.Int32
                        mCalParams(2 * nCol + 1).IntValue = New clsIntValue
                        mCalParams(2 * nCol + 1).nCol = nCol
                        mCalParams(2 * nCol + 1).nItem = 1
                        mCalParams(2 * nCol + 1).CaptionText = gpsRM.GetString("psDQ_ALARM")
                        oVar = DirectCast(oStruct.Item("IOPALARM"), FRRobot.FRCVar)
                        mCalParams(2 * nCol + 1).IntValue.Value = CInt(oVar.Value)
                        mCalParams(2 * nCol + 1).IntValue.MinValue = 16
                        mCalParams(2 * nCol + 1).IntValue.MaxValue = 30
                    End If

                    'get the cal table structure
                    oStruct = DirectCast(oFile.Item("TDQ_CAL_TAB"), FRRobot.FRCVars)
                    Dim oChannel As FRRobot.FRCVars = DirectCast(oStruct.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVars)
                    oChannel.NoRefresh = True 'Do one read from the robot for the whole structure
                    'Cal status
                    oVar = CType(oChannel.Item("ICALSTATUS"), FRRobot.FRCVar)
                    mDQCalStatus = New clsIntValue
                    mDQCalStatus.Value = CInt(oVar.Value)
                    'For some reason, DQ uses a different structure to set of constants.
                    'Convert this over so the screen can use the same interface
                    Select Case CType(mDQCalStatus.Value, eDQCalStatus)
                        Case eDQCalStatus.CAL_DONE
                            mCalStatus.Value = eCalStatus.APP_CAL_C
                        Case Else
                            mCalStatus.Value = eCalStatus.APP_CAL_NC
                    End Select
                    mCalStatus.Update()  'Make sure this isn't tracked as a change
                    'Table
                    Dim oFlow As FRRobot.FRCVars = DirectCast(oChannel.Item("RFLOWVALUES"), FRRobot.FRCVars)
                    Dim oComVal As FRRobot.FRCVars = DirectCast(oChannel.Item("ICOMMVALUES"), FRRobot.FRCVars)
                    Dim oOPCount As FRRobot.FRCVars = DirectCast(oChannel.Item("IOPCNTVALUES"), FRRobot.FRCVars)
                    Dim oMPCount As FRRobot.FRCVars = DirectCast(oChannel.Item("IMPCNTVALUES"), FRRobot.FRCVars)
                    Dim oOPPSI As FRRobot.FRCVars = DirectCast(oChannel.Item("IOPPSIVALUES"), FRRobot.FRCVars)
                    Dim oMPPSI As FRRobot.FRCVars = DirectCast(oChannel.Item("IMPPSIVALUES"), FRRobot.FRCVars)
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        oVar = CType(oFlow.Item(, nCalPoint), FRRobot.FRCVar)
                        mCalTable(nCalPoint).CMD = New clsSngValue
                        mCalTable(nCalPoint).CMD.Value = CSng(oVar.Value)
                        mCalTable(nCalPoint).CMD.MinValue = mfMinEngUnits
                        mCalTable(nCalPoint).CMD.MaxValue = mfMaxEngUnits
                        oVar = CType(oComVal.Item(, nCalPoint), FRRobot.FRCVar)
                        mCalTable(nCalPoint).OUTPUT = New clsIntValue
                        mCalTable(nCalPoint).OUTPUT.Value = CInt(oVar.Value)
                        mCalTable(nCalPoint).OUTPUT.MinValue = mnMinCount
                        mCalTable(nCalPoint).OUTPUT.MaxValue = mnMaxCount
                        oVar = CType(oOPCount.Item(, nCalPoint), FRRobot.FRCVar)
                        mCalTable(nCalPoint).nOutletPressure = New clsIntValue
                        mCalTable(nCalPoint).nOutletPressure.Value = CInt(oVar.Value)
                        mCalTable(nCalPoint).nOutletPressure.MinValue = mnDQMinCount
                        mCalTable(nCalPoint).nOutletPressure.MaxValue = mnDQMaxCount
                        oVar = CType(oMPCount.Item(, nCalPoint), FRRobot.FRCVar)
                        mCalTable(nCalPoint).nManifoldPressure = New clsIntValue
                        mCalTable(nCalPoint).nManifoldPressure.Value = CInt(oVar.Value)
                        mCalTable(nCalPoint).nManifoldPressure.MinValue = mnDQMinCount
                        mCalTable(nCalPoint).nManifoldPressure.MaxValue = mnDQMaxCount
                    Next
                    Debug.Print(CType(mDQCalStatus.Value, eDQCalStatus).ToString)
                    Debug.Print("Outlet: " & mbOutletPressure.ToString)
                    Debug.Print("Manifold: " & mbManifoldPressure.ToString)
                Case eCntrType.APP_CNTR_AA
                    'Normal cal table
                    Dim oCalPoint As FRRobot.FRCVars
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        oCalPoint = DirectCast(oCal.Item(, nCalPoint), FRRobot.FRCVars)
                        oVar = CType(oCalPoint.Item("CMD"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).CMD = New clsSngValue
                        mCalTable(nCalPoint).CMD.Value = CSng(oVar.Value)
                        mCalTable(nCalPoint).CMD.MinValue = mfMinEngUnits
                        mCalTable(nCalPoint).CMD.MaxValue = mfMaxEngUnits
                        oVar = CType(oCalPoint.Item("OUTPUT"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).OUTPUT = New clsIntValue
                        mCalTable(nCalPoint).OUTPUT.Value = CInt(oVar.Value)
                        mCalTable(nCalPoint).OUTPUT.MinValue = mnMinCount
                        mCalTable(nCalPoint).OUTPUT.MaxValue = mnMaxCount
                        oVar = CType(oCalPoint.Item("REF_OUT"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).REF_OUT = New clsIntValue
                        mCalTable(nCalPoint).REF_OUT.Value = CInt(oVar.Value)
                        mCalTable(nCalPoint).REF_OUT.MinValue = mnMinCount
                        mCalTable(nCalPoint).REF_OUT.MaxValue = mnMaxCount
                        oVar = CType(oCalPoint.Item("SCALE"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).SCALE = New clsSngValue
                        mCalTable(nCalPoint).SCALE.Value = CSng(oVar.Value)
                        mCalTable(nCalPoint).SCALE.MinValue = mfMinEngUnits
                        mCalTable(nCalPoint).SCALE.MaxValue = mfMaxEngUnits
                    Next
                    'editable parameters  
                    'AA data
                    sProg = "pavrtsgl"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars

                    'Get the stucture for this channel
                    Dim oTmp1 As FRRobot.FRCVars = DirectCast(oFile.Item("ts_chan_io"), FRRobot.FRCVars)
                    Dim oTmp2 As FRRobot.FRCVars
                    Dim nChanCount As Integer = oTmp1.Count
                    Dim nChanStart As Integer = (Arm.ArmNumber - 1) * (nChanCount \ 2)
                    Dim nChan As Integer = 0
                    For nChan = nChanStart To oTmp1.Count - 1
                        oTmp2 = DirectCast(oTmp1.Item(, nChan), FRRobot.FRCVars)
                        oVar = CType(oTmp2.Item("CON_OUT_PRM"), FRRobot.FRCVar)
                        If CInt(oVar.Value) = (nParamOnRobot + 1) Then
                            Exit For
                        End If
                    Next
                    Dim oTmp3 As FRRobot.FRCVars = DirectCast(oFile.Item("ts_chan_prm"), FRRobot.FRCVars)

                    'Get variables from the setup structure
                    oStruct = DirectCast(oTmp3.Item(, nChan), FRRobot.FRCVars)

                    'Get the current status structure
                    sProg = "patsctl"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars
                    oTmp1 = DirectCast(oFile.Item("ts_chan_dyn"), FRRobot.FRCVars)
                    Dim oStruct2 As FRRobot.FRCVars = DirectCast(oTmp1.Item(, nChan), FRRobot.FRCVars)

                    ReDim sColHeading(0)
                    sColHeading(0) = gpsRM.GetString("psAA_SETTINGS")
                    ReDim mCalParams(4)
                    'Selected mode
                    mCalParams(0).Valuetype = TypeCode.Boolean
                    mCalParams(0).BoolValue = New clsBoolValue
                    mCalParams(0).nCol = 0
                    mCalParams(0).nItem = 0
                    mCalParams(0).CaptionText = gpsRM.GetString("psAA_OP_MODE")
                    oVar = DirectCast(oStruct.Item("op_mode"), FRRobot.FRCVar)
                    mCalParams(0).BoolValue.Value = (CInt(oVar.Value) = 2)


                    'current mode
                    mCalParams(1).Valuetype = TypeCode.Boolean
                    mCalParams(1).BoolValue = New clsBoolValue
                    mCalParams(1).nCol = 0
                    mCalParams(1).nItem = 1
                    mCalParams(1).CaptionText = gpsRM.GetString("psAA_CUR_MODE")
                    oVar = DirectCast(oStruct2.Item("cur_op_mode"), FRRobot.FRCVar)
                    mCalParams(1).BoolValue.Value = (CInt(oVar.Value) = 2)


                    mCalParams(2).Valuetype = TypeCode.Int32
                    mCalParams(2).IntValue = New clsIntValue
                    mCalParams(2).nCol = 1
                    mCalParams(2).nItem = 0
                    mCalParams(2).CaptionText = gpsRM.GetString("psAA_MAX_OUT_MS")
                    oVar = DirectCast(oStruct.Item("max_out_ms"), FRRobot.FRCVar)
                    mCalParams(2).IntValue.Value = CInt(oVar.Value)
                    mCalParams(2).IntValue.MinValue = 0
                    mCalParams(2).IntValue.MaxValue = 15000

                    mCalParams(3).Valuetype = TypeCode.Single
                    mCalParams(3).SngValue = New clsSngValue
                    mCalParams(3).nCol = 1
                    mCalParams(3).nItem = 1
                    mCalParams(3).CaptionText = gpsRM.GetString("psAA_PER_TOL_BAND")
                    oVar = DirectCast(oStruct.Item("per_tol_band"), FRRobot.FRCVar)
                    mCalParams(3).SngValue.Value = CSng(oVar.Value) / 10 ' 11/17/10  MSW     Change scale from 100 to 10

                    mCalParams(3).SngValue.MinValue = 0
                    mCalParams(3).SngValue.MaxValue = 99.99
                    mCalParams(3).SngValue.FormatString = "0.00"

                    mCalParams(4).Valuetype = TypeCode.Int32
                    mCalParams(4).IntValue = New clsIntValue
                    mCalParams(4).nCol = 2
                    mCalParams(4).nItem = 1
                    mCalParams(4).CaptionText = gpsRM.GetString("psAA_ZERO_SPD_MS")
                    oVar = DirectCast(oStruct.Item("zero_spd_ms"), FRRobot.FRCVar)
                    mCalParams(4).IntValue.Value = CInt(oVar.Value)
                    mCalParams(4).IntValue.MinValue = 0
                    mCalParams(4).IntValue.MaxValue = 5000

                Case Else
                    'Normal cal table
                    Dim oCalPoint As FRRobot.FRCVars
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        oCalPoint = DirectCast(oCal.Item(, nCalPoint), FRRobot.FRCVars)
                        oVar = CType(oCalPoint.Item("CMD"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).CMD = New clsSngValue
                        mCalTable(nCalPoint).CMD.Value = CSng(oVar.Value)
                        mCalTable(nCalPoint).CMD.MinValue = mfMinEngUnits
                        mCalTable(nCalPoint).CMD.MaxValue = mfMaxEngUnits
                        oVar = CType(oCalPoint.Item("OUTPUT"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).OUTPUT = New clsIntValue
                        If mb2KCal Then
                            mCalTable(nCalPoint).OUTPUT.Value = CInt((CInt(oVar.Value) - mnResMinCnt) * mnResScale)
                            mCalTable(nCalPoint).OUTPUT.MinValue = CInt(mfMinEngUnits)
                            mCalTable(nCalPoint).OUTPUT.MaxValue = CInt(mfMaxEngUnits)
                        Else
                            mCalTable(nCalPoint).OUTPUT.Value = CInt(oVar.Value)
                            mCalTable(nCalPoint).OUTPUT.MinValue = mnMinCount
                            mCalTable(nCalPoint).OUTPUT.MaxValue = mnMaxCount
                        End If
                        oVar = CType(oCalPoint.Item("REF_OUT"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).REF_OUT = New clsIntValue
                        If mb2KCal Then
                            mCalTable(nCalPoint).REF_OUT.Value = CInt((CInt(oVar.Value) - mnHdrMinCnt) * mnHdrScale)
                            mCalTable(nCalPoint).REF_OUT.MinValue = CInt(mfMinEngUnits)
                            mCalTable(nCalPoint).REF_OUT.MaxValue = CInt(mfMaxEngUnits)
                        Else
                            mCalTable(nCalPoint).REF_OUT.Value = CInt(oVar.Value)
                            mCalTable(nCalPoint).REF_OUT.MinValue = mnMinCount
                            mCalTable(nCalPoint).REF_OUT.MaxValue = mnMaxCount
                        End If
                        oVar = CType(oCalPoint.Item("SCALE"), FRRobot.FRCVar)
                        mCalTable(nCalPoint).SCALE = New clsSngValue
                        mCalTable(nCalPoint).SCALE.Value = CSng(oVar.Value)
                        mCalTable(nCalPoint).SCALE.MinValue = mfMinEngUnits
                        mCalTable(nCalPoint).SCALE.MaxValue = mfMaxEngUnits
                    Next
            End Select
            Return True
        Catch ex As Exception
            Dim sStatus As String = String.Empty
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msMODULE, _
                    sStatus, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Friend Function SaveToRobot(ByRef Arm As clsArm, _
                        Optional ByVal bWriteAll As Boolean = False, _
                        Optional ByVal nParam As Integer = -1, _
                        Optional ByVal nValve As Integer = -1) As Boolean
        '********************************************************************************************
        'Description:  Save a cal table
        '
        'Parameters: Applicator, robot, optional parameter number, and optional valve number
        'Returns:    true = success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/12/10  MSW     Add Scale to AA per_tol_band
        ' 11/17/10  MSW     Change scale from 100 to 10
        ' 06/28/11  MSW     Fix IPC cal copy - sys col and valve were mixed up, and bWriteAll was getting ignored
        '********************************************************************************************
        Try
            If nParam = -1 Then
                nParam = mnParmNum
            Else
                mnParmNum = nParam
            End If
            If nValve = -1 Then
                nValve = mnValve
            Else
                mnValve = nValve
            End If
            If Arm.IsOnLine = False Then
                Return False
            End If
            'get con_prm_desc structure
            Dim sProg As String
            Dim sVar As String
            Dim nParamOnRobot As Integer 'To account for dualarm indexing.  Still start at 0, though
            If Arm.Controller.Version >= 7.3 Then
                'NGIP parm config
                nParamOnRobot = nParam
            Else
                'Dualarm or older parm config
                If Arm.ArmNumber > 1 Then
                    nParamOnRobot = nParam + 6
                Else
                    nParamOnRobot = nParam
                End If
            End If
            Dim sItem As String = String.Empty
            'Where is the cal table
            sProg = "PAVRPARM"
            'Find our way to the parm data
            Arm.ProgramName = sProg
            Dim oFile As FRRobot.FRCVars = Arm.ProgramVars
            'First get con_parm_desc structure
            'Muiltiarm variable setup varies by version
            Dim oStruct As FRRobot.FRCVars
            Dim oStruct3 As FRRobot.FRCVars
            If Arm.Controller.Version >= 7.3 Then
                'NGIP parm config
                Dim oTmp As FRRobot.FRCVars = _
                        DirectCast(oFile.Item("TPARMEQ"), FRRobot.FRCVars)
                Dim oTmp1 As FRRobot.FRCVars = _
                        DirectCast(oTmp.Item(Arm.ArmNumber.ToString()), FRRobot.FRCVars)
                oStruct = DirectCast(oTmp1.Item("CON_PRM_DESC"), FRRobot.FRCVars)
                oStruct3 = DirectCast(oTmp1.Item("CON_PRM_DES3"), FRRobot.FRCVars)
            Else
                'Dualarm or older parm config
                oStruct = DirectCast(oFile.Item("CON_PRM_DESC"), FRRobot.FRCVars)
                oStruct3 = DirectCast(oFile.Item("CON_PRM_DES3"), FRRobot.FRCVars)
            End If

            'Parm data
            Dim oParmData As FRRobot.FRCVars = _
                        DirectCast(oStruct.Item(, (nParamOnRobot)), FRRobot.FRCVars)
            Dim oParmData3 As FRRobot.FRCVars = _
                        DirectCast(oStruct3.Item(, (nParamOnRobot)), FRRobot.FRCVars)

            Dim oVar As FRRobot.FRCVar
            'Map a cal table var based on calibration type
            Dim oCal As FRRobot.FRCVars
            Dim oValveData As FRRobot.FRCVars = Nothing
            Dim oPathA As FRRobot.FRCVars = Nothing
            Dim oPathB As FRRobot.FRCVars = Nothing
            Select Case CalSource
                Case eCalSource.CAL_SRC_NO, eCalSource.CAL_SRC_NR, eCalSource.CAL_SRC_NA
                    'No cal table
                    Return False
                Case eCalSource.CAL_SRC_BC, eCalSource.CAL_SRC_IPC_CAL
                    'IPC cal fault limits
                    If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                        'psi to counts scale
                        sProg = "PAVRIPC"
                        Arm.ProgramName = sProg
                        oFile = Arm.ProgramVars
                        'TIPCEQCFG[1].TPMPCFG[1].TSENSOR[2].PSI_TO_CNTS
                        'TIPCEQCFG[1].TPMPCFG[1].TSENSOR[1].TIO.MIN_CNTS
                        sVar = "TIPCEQCFG"
                        Dim oTIPCEQCFG As FRRobot.FRCVars = DirectCast(oFile.Item(sVar), FRRobot.FRCVars)
                        Dim o2 As FRRobot.FRCVars = DirectCast(oTIPCEQCFG.Item(Arm.ArmNumber.ToString), FRRobot.FRCVars)
                        sVar = "TPMPCFG"
                        Dim o3 As FRRobot.FRCVars = DirectCast(o2.Item(sVar), FRRobot.FRCVars)
                        'Read this whole structure
                        o3.NoRefresh = True
                        Dim oResinPmpCfg As FRRobot.FRCVars = DirectCast(o3.Item("1"), FRRobot.FRCVars)
                        Dim oHardenerPmpCfg As FRRobot.FRCVars = DirectCast(o3.Item("2"), FRRobot.FRCVars)
                        sVar = "TSENSOR"
                        Dim oResinSensors As FRRobot.FRCVars = DirectCast(oResinPmpCfg.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerSensors As FRRobot.FRCVars = DirectCast(oHardenerPmpCfg.Item(sVar), FRRobot.FRCVars)
                        Dim oResinInSensor As FRRobot.FRCVars = DirectCast(oResinSensors.Item("1"), FRRobot.FRCVars)
                        Dim oHardenerInSensor As FRRobot.FRCVars = DirectCast(oHardenerSensors.Item("1"), FRRobot.FRCVars)
                        Dim oResinOutSensor As FRRobot.FRCVars = DirectCast(oResinSensors.Item("2"), FRRobot.FRCVars)
                        Dim oHardenerOutSensor As FRRobot.FRCVars = DirectCast(oHardenerSensors.Item("2"), FRRobot.FRCVars)
                        sVar = "PSI_TO_CNTS"
                        Dim oResinScale As FRRobot.FRCVar = DirectCast(oResinOutSensor.Item(sVar), FRRobot.FRCVar)
                        Dim oHardenerScale As FRRobot.FRCVar = DirectCast(oHardenerOutSensor.Item(sVar), FRRobot.FRCVar)
                        sVar = "TIO"
                        Dim oResinInSensIO As FRRobot.FRCVars = DirectCast(oResinInSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerInSensIO As FRRobot.FRCVars = DirectCast(oHardenerInSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oResinOutSensIO As FRRobot.FRCVars = DirectCast(oResinOutSensor.Item(sVar), FRRobot.FRCVars)
                        Dim oHardenerOutSensIO As FRRobot.FRCVars = DirectCast(oHardenerOutSensor.Item(sVar), FRRobot.FRCVars)
                        sVar = "MIN_CNTS"
                        Dim oResinOutSensMinCnt As FRRobot.FRCVar = DirectCast(oResinOutSensIO.Item(sVar), FRRobot.FRCVar)
                        Dim oHardenerOutSensMinCnt As FRRobot.FRCVar = DirectCast(oHardenerOutSensIO.Item(sVar), FRRobot.FRCVar)

                        mnResMinCnt = CInt(oResinOutSensMinCnt.Value)
                        mnHdrMinCnt = CInt(oHardenerOutSensMinCnt.Value)
                        mnResScale = CType(oResinScale.Value, Single)
                        mnHdrScale = CType(oHardenerScale.Value, Single)
                        'fault parameters
                        sVar = "MAX_REQUNITS"
                        Dim oVarVal As FRRobot.FRCVar = Nothing
                        If mCalParams(0).IntValue.Changed Or bWriteAll Then
                            oVarVal = DirectCast(oResinInSensIO.Item(sVar), FRRobot.FRCVar)
                            oVarVal.Value = mCalParams(0).IntValue.Value / 100
                            If mCalParams(0).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psRES_WRN_PCT")
                                UpdateChangeLog(sItem, mCalParams(0).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalParams(1).IntValue.Changed Or bWriteAll Then
                            oVarVal = DirectCast(oResinOutSensIO.Item(sVar), FRRobot.FRCVar)
                            oVarVal.Value = mCalParams(1).IntValue.Value / 100
                            If mCalParams(1).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psRES_FLT_PCT")
                                UpdateChangeLog(sItem, mCalParams(1).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalParams(2).IntValue.Changed Or bWriteAll Then
                            oVarVal = DirectCast(oHardenerInSensIO.Item(sVar), FRRobot.FRCVar)
                            oVarVal.Value = mCalParams(2).IntValue.Value / 100
                            If mCalParams(2).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psHDR_WRN_PCT")
                                UpdateChangeLog(sItem, mCalParams(2).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalParams(3).IntValue.Changed Or bWriteAll Then
                            oVarVal = DirectCast(oHardenerOutSensIO.Item(sVar), FRRobot.FRCVar)
                            oVarVal.Value = mCalParams(3).IntValue.Value / 100
                            If mCalParams(3).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psHDR_FLT_PCT")
                                UpdateChangeLog(sItem, mCalParams(3).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                    End If

                    'by color - pavrdbcv
                    sProg = "PAVRDBCV"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars

                    If nValve < 1 Then
                        nValve = 1
                    End If
                    sVar = "COLOR_DATA"
                    'Muiltiarm
                    If Arm.ArmNumber > 1 Then
                        'COLOR_DATA{Equip Num}
                        sVar = sVar & Arm.ArmNumber.ToString
                    End If
                    oPathA = DirectCast(oFile.Item(sVar), FRRobot.FRCVars)
                    Debug.Print(oPathA.Count.ToString & " - " & oPathA.VarName & " - " & oPathA.Size)
                    sVar = "NODEDATA"
                    oPathB = DirectCast(oPathA.Item(sVar), FRRobot.FRCVars)
                    Debug.Print(oPathB.Count.ToString & " - " & oPathB.VarName & " - " & oPathB.Size)
                    'COLOR_DATA{Equip Num}.NODEDATA[{VALVE_NO}]

                    sVar = "[" & nValve.ToString & "]"
                    'Valve data
                    oValveData = _
                                DirectCast(oPathB.Item(nValve.ToString), FRRobot.FRCVars)
                    'Cal Data - to be read later
                    oCal = DirectCast(oValveData.Item("CONTROL_CAL"), FRRobot.FRCVars)
                    'Cal size
                    If mnNumPoints.Changed Or bWriteAll Then
                        oVar = CType(oValveData.Item("CAL_SIZE"), FRRobot.FRCVar)
                        oVar.Value = mnNumPoints.Value
                        If mnNumPoints.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_SIZE")
                            UpdateChangeLog(sItem, mnNumPoints, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                    'Cal status
                    If mCalStatus.Changed Or bWriteAll Then
                        If (mCntrType = eCntrType.APP_CNTR_AF) Or (mCntrType = eCntrType.APP_CNTR_AS) Then
                            Dim oAF_DATA As FRRobot.FRCVars = _
                            CType(oValveData.Item("AF_DATA"), FRRobot.FRCVars)
                            oVar = CType(oAF_DATA.Item("CAL_STATUS"), FRRobot.FRCVar)
                            oVar.Value = mCalStatus.Value
                        End If
                        oVar = CType(oValveData.Item("CAL_STATUS"), FRRobot.FRCVar)
                        oVar.Value = mCalStatus.Value
                        If mCalStatus.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_STATUS")
                            Dim sOld As String = GetCalStatusText(mCalStatus.OldValue)
                            Dim sNew As String = GetCalStatusText(mCalStatus.Value)
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalStatus.Update()
                        End If
                    End If
                    'Cal Date
                    If mCalDate.Changed Or bWriteAll Then
                        oVar = CType(oValveData.Item("CAL_DATE"), FRRobot.FRCVar)
                        oVar.Value = mCalDate.Value
                        If mCalDate.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_DATE")
                            UpdateChangeLog(sItem, mCalDate, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                Case eCalSource.CAL_SRC_CAL, eCalSource.CAL_SRC_COP, eCalSource.CAL_SRC_SCAL
                    msColor = String.Empty 'Don't save a color to the change log
                    oCal = DirectCast(oParmData.Item("CONTROL_CAL"), FRRobot.FRCVars)
                    'Cal size
                    If mnNumPoints.Changed Or bWriteAll Then
                        oVar = CType(oParmData.Item("APP_CAL_SIZE"), FRRobot.FRCVar)
                        oVar.Value = mnNumPoints.Value
                        If mnNumPoints.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_SIZE")
                            UpdateChangeLog(sItem, mnNumPoints, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                    'Cal status
                    If mCalStatus.Changed Or bWriteAll Then
                        oVar = CType(oParmData.Item("CAL_STATUS"), FRRobot.FRCVar)
                        oVar.Value = mCalStatus.Value
                        If mCalStatus.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_STATUS")
                            Dim sOld As String
                            Dim sNew As String
                            If mCntrType = eCntrType.APP_CNTR_DQ Then
                                sOld = GetCalStatusText(mDQCalStatus.OldValue)
                                sNew = GetCalStatusText(mDQCalStatus.Value)
                            Else
                                sOld = GetCalStatusText(mCalStatus.OldValue)
                                sNew = GetCalStatusText(mCalStatus.Value)
                            End If
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalStatus.Update()
                        End If
                    End If
                    'Cal Date
                    If mCalDate.Changed Or bWriteAll Then
                        oVar = CType(oParmData.Item("CAL_DATE"), FRRobot.FRCVar)
                        oVar.Value = mCalDate.Value
                        If mCalDate.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psCAL_DATE")
                            UpdateChangeLog(sItem, mCalDate, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                Case Else
                    'New features to program
                    Debug.Assert(False)
                    Return False
            End Select
            'Load the table
            Select Case CntrType
                Case eCntrType.APP_CNTR_DQ
                    'DQ cal data
                    sProg = "PAVRDQ"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars

                    'editable parameters                
                    'Get variables from the setup structure
                    oStruct = DirectCast(oFile.Item("TDQ_SETUP"), FRRobot.FRCVars)
                    'Grace time
                    If mCalParams(0).IntValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("IGRACETIME"), FRRobot.FRCVar)
                        oVar.Value = mCalParams(0).IntValue.Value
                        If mCalParams(0).IntValue.Changed Then
                            sItem = msParmName & " " & mCalParams(0).CaptionText
                            UpdateChangeLog(sItem, mCalParams(0).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                    'step delay
                    If mCalParams(1).IntValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("ICALSTEPDEL"), FRRobot.FRCVar)
                        oVar.Value = mCalParams(1).IntValue.Value
                        If mCalParams(1).IntValue.Changed Then
                            sItem = msParmName & " " & mCalParams(1).CaptionText
                            UpdateChangeLog(sItem, mCalParams(1).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                        End If
                    End If
                    Dim nCol As Integer = 0
                    'Manifold pressure
                    If mbManifoldPressure Then
                        nCol += 1
                        'Grace time
                        If mCalParams(2 * nCol).IntValue.Changed Or bWriteAll Then
                            oVar = DirectCast(oStruct.Item("IMPWARNING"), FRRobot.FRCVar)
                            oVar.Value = mCalParams(2 * nCol).IntValue.Value
                            If mCalParams(2 * nCol).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psMANIFOLD_PRES") & " " & mCalParams(2 * nCol).CaptionText
                                UpdateChangeLog(sItem, mCalParams(2 * nCol).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        'step delay
                        If mCalParams(2 * nCol + 1).IntValue.Changed Or bWriteAll Then
                            oVar = DirectCast(oStruct.Item("IMPALARM"), FRRobot.FRCVar)
                            oVar.Value = mCalParams(2 * nCol + 1).IntValue.Value
                            If mCalParams(2 * nCol + 1).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psMANIFOLD_PRES") & " " & mCalParams(2 * nCol + 1).CaptionText
                                UpdateChangeLog(sItem, mCalParams(2 * nCol + 1).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                    End If
                    'Outlet pressure
                    If mbOutletPressure Then
                        nCol += 1
                        'Grace time
                        If mCalParams(2 * nCol).IntValue.Changed Or bWriteAll Then
                            oVar = DirectCast(oStruct.Item("IOPWARNING"), FRRobot.FRCVar)
                            oVar.Value = mCalParams(2 * nCol).IntValue.Value
                            If mCalParams(2 * nCol).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psDQ_MP") & " " & mCalParams(2 * nCol).CaptionText
                                UpdateChangeLog(sItem, mCalParams(2 * nCol).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        'step delay
                        If mCalParams(2 * nCol + 1).IntValue.Changed Or bWriteAll Then
                            oVar = DirectCast(oStruct.Item("IOPALARM"), FRRobot.FRCVar)
                            oVar.Value = mCalParams(2 * nCol + 1).IntValue.Value
                            If mCalParams(2 * nCol + 1).IntValue.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psDQ_OP") & " " & mCalParams(2 * nCol + 1).CaptionText
                                UpdateChangeLog(sItem, mCalParams(2 * nCol + 1).IntValue, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                    End If

                    'get the cal table structure
                    oStruct = DirectCast(oFile.Item("TDQ_CAL_TAB"), FRRobot.FRCVars)
                    Dim oChannel As FRRobot.FRCVars = DirectCast(oStruct.Item(, (Arm.ArmNumber - 1)), FRRobot.FRCVars)
                    oChannel.NoRefresh = True 'Do one read from the robot for the whole structure
                    'Cal status
                    If mDQCalStatus.Changed Or bWriteAll Then
                        oVar = CType(oChannel.Item("ICALSTATUS"), FRRobot.FRCVar)
                        oVar.Value = mDQCalStatus.Value
                        mDQCalStatus.Update() ' it gets logged in the common variable, so no changelog
                    End If
                    mCalStatus.Update()  'Make sure this isn't tracked as a change
                    'Table
                    Dim oFlow As FRRobot.FRCVars = DirectCast(oChannel.Item("RFLOWVALUES"), FRRobot.FRCVars)
                    Dim oComVal As FRRobot.FRCVars = DirectCast(oChannel.Item("ICOMMVALUES"), FRRobot.FRCVars)
                    Dim oOPCount As FRRobot.FRCVars = DirectCast(oChannel.Item("IOPCNTVALUES"), FRRobot.FRCVars)
                    Dim oMPCount As FRRobot.FRCVars = DirectCast(oChannel.Item("IMPCNTVALUES"), FRRobot.FRCVars)
                    Dim oOPPSI As FRRobot.FRCVars = DirectCast(oChannel.Item("IOPPSIVALUES"), FRRobot.FRCVars)
                    Dim oMPPSI As FRRobot.FRCVars = DirectCast(oChannel.Item("IMPPSIVALUES"), FRRobot.FRCVars)
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        If mCalTable(nCalPoint).CMD.Changed Or bWriteAll Then
                            oVar = CType(oFlow.Item(, nCalPoint), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).CMD.Value
                            If mCalTable(nCalPoint).CMD.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psSETPOINT") & " (" & msUnits & ")"
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).CMD, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).OUTPUT.Changed Or bWriteAll Then
                            oVar = CType(oComVal.Item(, nCalPoint), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).OUTPUT.Value
                            If mCalTable(nCalPoint).OUTPUT.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psOUTPUT_COUNTS")
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).OUTPUT, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).nOutletPressure.Changed Or bWriteAll Then
                            oVar = CType(oOPCount.Item(, nCalPoint), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).nOutletPressure.Value
                            If mCalTable(nCalPoint).nOutletPressure.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psOUTLET_PRES")
                                Dim sOld As String = DQCountsToPSI(mCalTable(nCalPoint).nOutletPressure.OldValue).ToString
                                Dim sNew As String = DQCountsToPSI(mCalTable(nCalPoint).nOutletPressure.Value).ToString
                                UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                                mCalTable(nCalPoint).nOutletPressure.Update()
                            End If
                        End If
                        If mCalTable(nCalPoint).nManifoldPressure.Changed Or bWriteAll Then
                            oVar = CType(oMPCount.Item(, nCalPoint), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).nManifoldPressure.Value
                            If mCalTable(nCalPoint).nManifoldPressure.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psMANIFOLD_PRES")
                                Dim sOld As String = DQCountsToPSI(mCalTable(nCalPoint).nManifoldPressure.OldValue).ToString
                                Dim sNew As String = DQCountsToPSI(mCalTable(nCalPoint).nManifoldPressure.Value).ToString
                                UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                                mCalTable(nCalPoint).nManifoldPressure.Update()
                            End If
                        End If
                    Next
                    oChannel.Update()
                Case eCntrType.APP_CNTR_AA
                    'Normal cal table
                    Dim oCalPoint As FRRobot.FRCVars
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        oCalPoint = DirectCast(oCal.Item(, nCalPoint), FRRobot.FRCVars)
                        If mCalTable(nCalPoint).CMD.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("CMD"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).CMD.Value
                            If mCalTable(nCalPoint).CMD.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psSETPOINT") & " (" & msUnits & ")"
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).CMD, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).OUTPUT.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("OUTPUT"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).OUTPUT.Value
                            If mCalTable(nCalPoint).OUTPUT.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psREFERENCE") & " " & gpsRM.GetString("psOUTPUT_COUNTS")
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).OUTPUT, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).REF_OUT.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("REF_OUT"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).REF_OUT.Value
                            If mCalTable(nCalPoint).REF_OUT.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psREFERENCE") & " " & gpsRM.GetString("psOUTPUT_COUNTS")
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).REF_OUT, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).SCALE.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("SCALE"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).SCALE.Value
                            If mCalTable(nCalPoint).SCALE.Changed Then
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psSCALE") & " (" & msUnits & ")"
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).SCALE, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                    Next
                    oParmData.Update()
                    oParmData3.Update()
                    If oValveData Is Nothing = False Then
                        oValveData.Update()
                    End If

                    'editable parameters  
                    'AA data
                    sProg = "pavrtsgl"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars

                    'Get the stucture for this channel
                    Dim oTmp1 As FRRobot.FRCVars = DirectCast(oFile.Item("ts_chan_io"), FRRobot.FRCVars)
                    Dim oTmp2 As FRRobot.FRCVars
                    Dim nChanCount As Integer = oTmp1.Count
                    Dim nChanStart As Integer = (Arm.ArmNumber - 1) * (nChanCount \ 2)
                    Dim nChan As Integer = 0
                    For nChan = nChanStart To oTmp1.Count - 1
                        oTmp2 = DirectCast(oTmp1.Item(, nChan), FRRobot.FRCVars)
                        oVar = CType(oTmp2.Item("CON_OUT_PRM"), FRRobot.FRCVar)
                        If CInt(oVar.Value) = (nParamOnRobot + 1) Then
                            Exit For
                        End If
                    Next
                    Dim oTmp3 As FRRobot.FRCVars = DirectCast(oFile.Item("ts_chan_prm"), FRRobot.FRCVars)

                    'Get variables from the setup structure
                    oStruct = DirectCast(oTmp3.Item(, nChan), FRRobot.FRCVars)

                    'Get the current status structure
                    sProg = "patsctl"
                    Arm.ProgramName = sProg
                    oFile = Arm.ProgramVars
                    oTmp1 = DirectCast(oFile.Item("ts_chan_dyn"), FRRobot.FRCVars)
                    Dim oStruct2 As FRRobot.FRCVars = DirectCast(oTmp1.Item(, nChan), FRRobot.FRCVars)

                    'Selected mode
                    If mCalParams(0).BoolValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("op_mode"), FRRobot.FRCVar)
                        If mCalParams(0).BoolValue.Value Then
                            oVar.Value = 2
                        Else
                            oVar.Value = 3
                        End If
                        If mCalParams(0).BoolValue.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psAA_OP_MODE")
                            Dim sOld As String = mCalParams(0).BoolValue.OldValue.ToString
                            Dim sNew As String = mCalParams(0).BoolValue.Value.ToString
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalParams(0).BoolValue.Update()
                        End If
                    End If

                    'current mode
                    If mCalParams(1).BoolValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct2.Item("cur_op_mode"), FRRobot.FRCVar)
                        If mCalParams(1).BoolValue.Value Then
                            oVar.Value = 2
                        Else
                            oVar.Value = 3
                        End If
                        If mCalParams(1).BoolValue.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psAA_CUR_MODE")
                            Dim sOld As String = mCalParams(1).BoolValue.OldValue.ToString
                            Dim sNew As String = mCalParams(1).BoolValue.Value.ToString
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalParams(1).BoolValue.Update()
                        End If
                    End If

                    If mCalParams(2).IntValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("max_out_ms"), FRRobot.FRCVar)
                        oVar.Value = mCalParams(2).IntValue.Value
                        If mCalParams(2).IntValue.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psAA_MAX_OUT_MS")
                            Dim sOld As String = mCalParams(2).IntValue.OldValue.ToString
                            Dim sNew As String = mCalParams(2).IntValue.Value.ToString
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalParams(2).IntValue.Update()
                        End If
                    End If

                    If mCalParams(3).SngValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("per_tol_band"), FRRobot.FRCVar)
                        oVar.Value = mCalParams(3).SngValue.Value * 10 ' 11/17/10  MSW     Change scale from 100 to 10

                        If mCalParams(3).SngValue.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psAA_PER_TOL_BAND")
                            Dim sOld As String = mCalParams(3).SngValue.OldValue.ToString
                            Dim sNew As String = mCalParams(3).SngValue.Value.ToString
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalParams(3).SngValue.Update()
                        End If
                    End If

                    If mCalParams(4).IntValue.Changed Or bWriteAll Then
                        oVar = DirectCast(oStruct.Item("zero_spd_ms"), FRRobot.FRCVar)
                        oVar.Value = mCalParams(4).IntValue.Value
                        If mCalParams(4).IntValue.Changed Then
                            sItem = msParmName & " " & gpsRM.GetString("psAA_ZERO_SPD_MS")
                            Dim sOld As String = mCalParams(4).IntValue.OldValue.ToString
                            Dim sNew As String = mCalParams(4).IntValue.Value.ToString
                            UpdateChangeLog(sItem, sNew, sOld, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            mCalParams(4).IntValue.Update()
                        End If
                    End If
                Case Else
                    'Normal cal table
                    Dim oCalPoint As FRRobot.FRCVars
                    For nCalPoint As Integer = 0 To mnNumPoints.Value - 1
                        oCalPoint = DirectCast(oCal.Item(, nCalPoint), FRRobot.FRCVars)
                        Debug.Print(mCalTable(nCalPoint).CMD.Value.ToString & " " & mCalTable(nCalPoint).OUTPUT.Value.ToString & " " & mCalTable(nCalPoint).REF_OUT.Value.ToString & " " & mCalTable(nCalPoint).SCALE.Value.ToString)
                        If mCalTable(nCalPoint).CMD.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("CMD"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).CMD.Value
                            If mCalTable(nCalPoint).CMD.Changed Then
                                If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                                    sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psRESIN") & " " & gpsRM.GetString("psSETPOINT") & " (" & msUnits & ")"
                                Else
                                    sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psSETPOINT") & " (" & msUnits & ")"
                                End If
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).CMD, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).OUTPUT.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("OUTPUT"), FRRobot.FRCVar)
                            If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                                oVar.Value = CInt((mCalTable(nCalPoint).OUTPUT.Value / mnResScale) + mnResMinCnt)
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psRESIN") & " " & gpsRM.GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
                            Else
                                oVar.Value = mCalTable(nCalPoint).OUTPUT.Value
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & " " & gpsRM.GetString("psOUTPUT_COUNTS")
                            End If
                            If mCalTable(nCalPoint).OUTPUT.Changed Then
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).OUTPUT, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).REF_OUT.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("REF_OUT"), FRRobot.FRCVar)
                            If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                                oVar.Value = CInt((mCalTable(nCalPoint).REF_OUT.Value / mnHdrScale) + mnHdrMinCnt)
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psHARDENER") & " " & gpsRM.GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
                            Else
                                oVar.Value = mCalTable(nCalPoint).REF_OUT.Value
                                sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psREFERENCE")
                            End If
                            If mCalTable(nCalPoint).REF_OUT.Changed Then
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).REF_OUT, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                        If mCalTable(nCalPoint).SCALE.Changed Or bWriteAll Then
                            oVar = CType(oCalPoint.Item("SCALE"), FRRobot.FRCVar)
                            oVar.Value = mCalTable(nCalPoint).SCALE.Value
                            If mCalTable(nCalPoint).SCALE.Changed Then
                                If CalSource = eCalSource.CAL_SRC_IPC_CAL Then
                                    sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psHARDENER") & " " & gpsRM.GetString("psSETPOINT") & " (" & msUnits & ")"
                                Else
                                    sItem = msParmName & " " & gpsRM.GetString("psCAL_POINT") & (nCalPoint + 1).ToString & " " & gpsRM.GetString("psSCALE")
                                End If
                                UpdateChangeLog(sItem, mCalTable(nCalPoint).SCALE, Arm.Name, msColor, mnZoneNumber, msZoneName)
                            End If
                        End If
                    Next
                    oParmData.Update()
                    oParmData3.Update()
                    If oValveData Is Nothing = False Then
                        oValveData.Update()
                    End If
            End Select
            Return True
        Catch ex As Exception
            Dim sStatus As String = String.Empty
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msMODULE, _
                    sStatus, MessageBoxButtons.OK)
            Return False
        End Try
    End Function
    Public Function GetCalStatusText(ByVal CalStatus As Integer) As String

        Select Case mCntrType
            Case eCntrType.APP_CNTR_AF
                Select Case CalStatus
                    Case eAFCalStatus.AF_NOT_CAL
                        GetCalStatusText = gpsRM.GetString("psNOT_COMP")
                    Case eAFCalStatus.AF_CAL_OK
                        GetCalStatusText = gpsRM.GetString("psCOMPLETE")
                    Case eAFCalStatus.AF_CANT_UPR
                        GetCalStatusText = gpsRM.GetString("psAF_CANT_UPR")
                    Case eAFCalStatus.AF_CANT_LOWR
                        GetCalStatusText = gpsRM.GetString("psAF_CANT_LOWR")
                    Case eAFCalStatus.AF_ADAPT_OUT
                        GetCalStatusText = gpsRM.GetString("psAF_ADAPT_OUT")
                    Case eAFCalStatus.AF_UPPER_LIM
                        GetCalStatusText = gpsRM.GetString("psAF_UPPER_LIM")
                    Case eAFCalStatus.AF_LOWER_LIM
                        GetCalStatusText = gpsRM.GetString("psAF_LOWER_LIM")
                    Case eAFCalStatus.AF_CAL_ABORT
                        GetCalStatusText = gpsRM.GetString("psABORTED")
                    Case eAFCalStatus.AF_CAL_COPY
                        GetCalStatusText = gpsRM.GetString("psCOPIED")
                    Case eAFCalStatus.AF_COPY_ER
                        GetCalStatusText = gpsRM.GetString("psCOPY_ERR")
                    Case Else
                        GetCalStatusText = gcsRM.GetString("csERROR")
                End Select

            Case eCntrType.APP_CNTR_DQ
                Select Case CalStatus
                    Case eDQCalStatus.CAL_DONE
                        GetCalStatusText = gpsRM.GetString("psCOMPLETE")
                    Case Else
                        GetCalStatusText = gpsRM.GetString("psNOT_COMP")
                End Select
            Case Else
                Select Case CalStatus
                    Case eCalStatus.APP_CAL_C
                        Select Case mCalSource.Value
                            Case eCalSource.CAL_SRC_SCAL
                                GetCalStatusText = gpsRM.GetString("psSCALED")
                            Case eCalSource.CAL_SRC_COP
                                GetCalStatusText = gpsRM.GetString("psCOPIED")
                            Case Else
                                GetCalStatusText = gpsRM.GetString("psCOMPLETE")
                        End Select
                    Case Else
                        GetCalStatusText = gpsRM.GetString("psNOT_COMP")
                End Select
        End Select
    End Function
    Friend Function DQCountsToPSI(ByVal Counts As Integer) As Integer
        '********************************************************************************************
        'Description:  convert counts to psi
        '           
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (CInt((Counts - mnDQMinCount) * mfDQCountToPSI) + mnDQMinPSI)
    End Function
    Friend Function DQPSIToCounts(ByVal PSI As Integer) As Integer
        '********************************************************************************************
        'Description:  convert counts to psi
        '           
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return CInt((PSI - mnDQMinPSI) / mfDQCountToPSI) + mnDQMinCount
    End Function
#End Region
End Class
