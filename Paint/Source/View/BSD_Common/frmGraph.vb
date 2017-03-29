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
' Form/Module: frmGraph
'
' Description: Process screen graph pop-up window
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms
Public Class frmGraph

#Region " Declares "

    'Graph form maximized gives us almost 25 divisions
    'time per division = 1 to 10 seconds
    '4 samples per second, so 4 * 10 * 250 = 
    Private Const mnMaxPoints As Integer = 10000
    Private mnCurHead As Integer = 0
    Private mbLoopData As Boolean = False
    Private mnNumLines As Integer = 0
    Private msTitle As String = String.Empty
    Private mbRunning As Boolean = False
    Private mbUseYAxis1 As Boolean = False 'enable the 2nd Y axis
    Private mnAxisYMax(1) As Integer 'Max scale on each side
    Private Structure LineConfig
        Dim PenCfg As Pen
        Dim YAxis As Integer '0 = left, 1 = right
        Dim LineLabel As String
        Dim MaxScale As Integer
    End Structure
    Private mLineCfgs As LineConfig()
    Private mnLineData(0, mnMaxPoints - 1) As Integer
    Private msAxisLabels(1) As String
    Private mnGraphOffset As Integer = 0
#End Region
#Region " Properties "
    Friend Property NumLines() As Integer
        '********************************************************************************************
        'Description: Get or set the number of lines top graph.
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumLines
        End Get
        Set(ByVal value As Integer)
            mnNumLines = value
            ReDim mLineCfgs(mnNumLines - 1)
            mbLoopData = False
            mbUseYAxis1 = False
            mnAxisYMax(0) = 0
            mnAxisYMax(1) = 0
            mnGraphOffset = 0
            ReDim mnLineData(mnNumLines - 1, mnMaxPoints - 1)
            For nIndex As Integer = 0 To mnNumLines - 1
                mLineCfgs(nIndex).LineLabel = nIndex.ToString
                mLineCfgs(nIndex).YAxis = 0
                mLineCfgs(nIndex).PenCfg = New Pen(Color.Green, 2)
                mLineCfgs(nIndex).MaxScale = 100
                mnLineData(nIndex, 0) = 0
            Next
            mnCurHead = 1
        End Set
    End Property
    Friend Property LineLabel(ByVal nIndex As Integer) As String
        '********************************************************************************************
        'Description: Get or set a line label.
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mLineCfgs(nIndex).LineLabel
        End Get
        Set(ByVal value As String)
            mLineCfgs(nIndex).LineLabel = value
        End Set
    End Property
    Friend Property AxisUnitLabel(ByVal nIndex As Integer) As String
        '********************************************************************************************
        'Description: Get or set an axis label. - units only
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msAxisLabels(nIndex)
        End Get
        Set(ByVal value As String)
            msAxisLabels(nIndex) = value
        End Set
    End Property
    Friend Property Title() As String
        '********************************************************************************************
        'Description: Get or set a the window title
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msTitle
        End Get
        Set(ByVal value As String)
            msTitle = value
        End Set
    End Property
    Friend Property LineColor(ByVal nIndex As Integer) As Color
        '********************************************************************************************
        'Description: Get or set a line Color.
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mLineCfgs(nIndex).PenCfg.Color
        End Get
        Set(ByVal value As Color)
            mLineCfgs(nIndex).PenCfg.Color = value
        End Set
    End Property
    Friend Property YAxis(ByVal nIndex As Integer) As Integer
        '********************************************************************************************
        'Description: Get or set which axis a line is tied to.
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mLineCfgs(nIndex).YAxis
        End Get
        Set(ByVal value As Integer)
            mLineCfgs(nIndex).YAxis = value
        End Set
    End Property
    Friend Property MaxScale(ByVal nIndex As Integer) As Integer
        '********************************************************************************************
        'Description: Get or set the max value for this line.
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mLineCfgs(nIndex).MaxScale
        End Get
        Set(ByVal value As Integer)
            mLineCfgs(nIndex).MaxScale = value
        End Set
    End Property
#End Region
#Region " Routines "
    Friend Sub AddNodes(ByRef sData() As String)
        '********************************************************************************************
        'Description: Get new data
        '           
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbRunning Then
            For nIndex As Integer = 0 To sData.GetUpperBound(0)
                If mnCurHead > mnMaxPoints Then
                    mnCurHead = 0
                    mbLoopData = True
                End If
                'store integers, but value = Eng units * 10
                Try
                    If sData(nIndex) <> String.Empty Then
                        mnLineData(nIndex, mnCurHead) = CInt(10 * CType(sData(nIndex), Single))
                    Else
                        mnLineData(nIndex, mnCurHead) = 0
                    End If
                Catch ex As Exception
                    mnLineData(nIndex, mnCurHead) = 0
                End Try
            Next
            mnCurHead += 1
            Me.Refresh()
        End If
    End Sub
    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: format the screen
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnPlay.Text = gpsRM.GetString("psSTART")
        mbRunning = False
        lblSecPerDiv.Text = trkSecPerDiv.Value.ToString & gpsRM.GetString("psSecPerDiv")
        Me.Text = msTitle
        For nLine As Integer = 0 To mnNumLines - 1
            Dim oLabel As Label = DirectCast(tlpLegend.Controls("lblLegend" & nLine.ToString), Label)
            oLabel.ForeColor = mLineCfgs(nLine).PenCfg.Color
            oLabel.Text = mLineCfgs(nLine).LineLabel
            oLabel.Visible = True
            If mLineCfgs(nLine).YAxis = 1 Then
                mbUseYAxis1 = True
            End If
            If mLineCfgs(nLine).MaxScale > mnAxisYMax(mLineCfgs(nLine).YAxis) Then
                mnAxisYMax(mLineCfgs(nLine).YAxis) = mLineCfgs(nLine).MaxScale
            End If
            tlpLegend.ColumnStyles(nLine).SizeType = SizeType.Percent
            tlpLegend.ColumnStyles(nLine).Width = 100
        Next
        If mnNumLines < tlpLegend.ColumnCount Then
            For nCol As Integer = mnNumLines To tlpLegend.ColumnCount - 1
                tlpLegend.ColumnStyles(nCol).SizeType = SizeType.Absolute
                tlpLegend.ColumnStyles(nCol).Width = 0
            Next
        End If
        'YAxis scales
        lblYAxis0Bottom.Text = "0"
        lblYAxis1Bottom.Visible = mbUseYAxis1
        lblYAxis1Top.Visible = mbUseYAxis1
        lblYAxis1Bottom.Text = "0"
        lblYaxis0Top.Text = mnAxisYMax(0).ToString & " " & msAxisLabels(0)
        lblYAxis1Top.Text = mnAxisYMax(1).ToString & " " & msAxisLabels(1)

    End Sub
    Friend Shadows Sub Show()
        '********************************************************************************************
        'Description: start the screen
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.StartPosition = FormStartPosition.CenterScreen
        subInitFormText()
        MyBase.Show()
        Me.Focus()
    End Sub

#End Region

    Private Sub btnPlay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlay.Click
        '********************************************************************************************
        'Description: start the screen
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbRunning = Not (mbRunning)
        If mbRunning Then
            btnPlay.Text = gpsRM.GetString("psSTOP")
        Else
            btnPlay.Text = gpsRM.GetString("psSTART")
        End If
    End Sub

    Private Sub trkSecPerDiv_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles trkSecPerDiv.Scroll
        '********************************************************************************************
        'Description: Adjust the time scale
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        lblSecPerDiv.Text = trkSecPerDiv.Value.ToString & gpsRM.GetString("psSecPerDiv")
        Me.Refresh()
    End Sub

    Private Sub pnlGraph_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlGraph.Paint
        '********************************************************************************************
        'Description: Draw the graph
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oPen As New Pen(Color.LightGray, 1)
        Dim nX0(0) As Integer
        Dim nX1(0) As Integer
        Dim nY0(0) As Integer
        Dim nY1(0) As Integer
        Const nSamplesPerSecond As Integer = 4
        Const nPixelsPerDiv As Integer = 50
        Dim nNumDiv As Integer = 5
        If pnlGraph.Height > 300 Then
            nNumDiv = 10
        End If
        'Draw horizontal grid lines
        nX0(0) = 0
        nX1(0) = pnlGraph.Width
        For nLine As Integer = 0 To nNumDiv
            nY0(0) = CInt(nLine * pnlGraph.Height / nNumDiv)
            e.Graphics.DrawLine(oPen, nX0(0), nY0(0), nX1(0), nY0(0))
        Next
        nY0(0) = 0
        nY1(0) = pnlGraph.Height
        nX0(0) = mnGraphOffset
        Do While nX0(0) < pnlGraph.Width
            e.Graphics.DrawLine(oPen, nX0(0), nY0(0), nX0(0), nY1(0))
            nX0(0) = nX0(0) + nPixelsPerDiv
        Loop
        '   num samples =                Divisions           *           sec/div    * samples/sec            
        Dim nSamplesToDraw As Integer = CInt(pnlGraph.Width / nPixelsPerDiv) * trkSecPerDiv.Value * nSamplesPerSecond
        'Range in data array
        Dim nStop As Integer = mnCurHead - 1
        Dim nStart As Integer = mnCurHead - nSamplesToDraw
        Dim nPixelsPerSample As Integer = CInt(pnlGraph.Width / nSamplesToDraw)
        Dim nArrayIndex As Integer
        'Cover loop around 
        If nStart <= 0 Then
            If mbLoopData Then
                nArrayIndex = mnMaxPoints + nStart - 1
            Else
                nStart = 1
                nArrayIndex = 0
                'Wait for it to start recording
                If nStart = nStop Then
                    Exit Sub
                End If
            End If
        Else
                nArrayIndex = nStart - 1
        End If
        Dim nLines As Integer = mnLineData.GetUpperBound(0)
        ReDim nX0(nLines)
        ReDim nY0(nLines)
        ReDim nX1(nLines)
        ReDim nY1(nLines)
        Dim fVertScale(nLines) As Double
        Dim nIndex2 As Integer = nArrayIndex
        For nline As Integer = 0 To nLines
            If mLineCfgs(nline).MaxScale = 0 Then
                mLineCfgs(nline).MaxScale = 10
            End If
            fVertScale(nline) = (pnlGraph.Height / mLineCfgs(nline).MaxScale) / 10
            nX0(nline) = 0
            nY0(nline) = pnlGraph.Height - CInt(mnLineData(nline, nArrayIndex) * fVertScale(nline))
            nX1(nline) = 0
            nY1(nline) = 0
            nIndex2 = nArrayIndex
            For nIndex As Integer = nStart To nStop
                nIndex2 += 1
                If nIndex2 = mnMaxPoints Then
                    nIndex2 = 0
                End If
                nX1(nline) = nX0(nline) + nPixelsPerSample
                nY1(nline) = pnlGraph.Height - CInt(mnLineData(nline, nIndex2) * fVertScale(nline))
                e.Graphics.DrawLine(mLineCfgs(nline).PenCfg, nX0(nline), nY0(nline), nX1(nline), nY1(nline))
                nX0(nline) = nX1(nline)
                nY0(nline) = nY1(nline)
            Next
        Next
    End Sub

    Private Sub frmGraph_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Refresh()
    End Sub
End Class