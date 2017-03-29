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
' Form/Module: uctlList
'
' Description: user control to list process data
'               basically, it handles the label casting so outside code can just acces by index
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick O.
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 06/17/09      msw     Onceover Cleanup
'********************************************************************************************
Public Class uctlList
    'Two labels exist at startup.  The 2nd one is used to set the spacing
    Private mnNumLabels As Integer = 2

    Friend Event Mouse_Click(ByVal oMe As Object)
    Friend Event Mouse_Down(ByVal oMe As Object)
    ReadOnly Property labels(ByVal index As Integer) As ControlCollection
        '********************************************************************************************
        'Description:  give access to the collection of labels
        '
        'Parameters:  
        'Returns:      controls collection - all the labels, plus the frame
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return Me.Controls
        End Get
    End Property
    ReadOnly Property label(ByVal index As Integer) As Label
        '********************************************************************************************
        'Description:  give access to an individual label
        '
        'Parameters:  label index
        'Returns:      individual label
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Return DirectCast(Me.Controls("lbl" & index.ToString), Label)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Property LabelVisible(ByVal index As Integer) As Boolean
        '********************************************************************************************
        'Description:  give access to label Visible properties
        '
        'Parameters:  label index - 0 sets all or returns property of lbl1
        'Returns:     a label's Visible property
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                    Return Nothing
                Else
                    Return olbl.Visible
                End If
            Else
                Return lbl1.Visible
            End If
        End Get
        Set(ByVal value As Boolean)
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                Else
                    olbl.Visible = value
                End If
            Else
                For Each olbl3 As Label In Me.Controls
                    olbl3.Visible = value
                Next
            End If
        End Set
    End Property
    Property LabelBackColor(ByVal index As Integer) As Color
        '********************************************************************************************
        'Description:  give access to label BackColor properties
        '
        'Parameters:  label index - 0 sets all or returns property of lbl1
        'Returns:     a label's BackColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                    Return Nothing
                Else
                    Return olbl.BackColor
                End If
            Else
                Return lbl1.BackColor
            End If
        End Get
        Set(ByVal value As Color)
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                Else
                    olbl.BackColor = value
                End If
            Else
                For Each olbl3 As Label In Me.Controls
                    olbl3.BackColor = value
                Next
            End If
        End Set
    End Property
    Property LabelForeColor(ByVal index As Integer) As Color
        '********************************************************************************************
        'Description:  give access to label ForeColor properties
        '
        'Parameters:  label index - 0 sets all or returns property of lbl1
        'Returns:     a label's ForeColor
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                    Return Nothing
                Else
                    Return olbl.ForeColor
                End If
            Else
                Return lbl1.ForeColor
            End If
        End Get
        Set(ByVal value As Color)
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                Else
                    olbl.ForeColor = value
                End If
            Else
                For Each olbl2 As Label In Me.Controls
                    olbl2.ForeColor = value
                Next
            End If
        End Set
    End Property
    Property LabelBorderStyle() As BorderStyle
        '********************************************************************************************
        'Description:  give access to label BorderStyle properties
        '
        'Parameters:  
        'Returns:    returns lbl1.BorderStyle, sets all of them
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return lbl1.BorderStyle
        End Get
        Set(ByVal value As BorderStyle)
            For Each olbl As Label In Me.Controls
                olbl.BorderStyle = value
            Next
        End Set
    End Property
    Property LabelTextAlign() As ContentAlignment
        '********************************************************************************************
        'Description:  give access to label BorderStyle properties
        '
        'Parameters:  
        'Returns:    returns lbl1.BorderStyle, sets all of them
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return lbl1.TextAlign
        End Get
        Set(ByVal value As ContentAlignment)
            For Each olbl As Label In Me.Controls
                olbl.TextAlign = value
            Next
        End Set
    End Property
    Property LabelHeight() As Integer
        '********************************************************************************************
        'Description:  give access to label BorderStyle properties
        '
        'Parameters:  
        'Returns:    returns lbl1.BorderStyle, sets all of them
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Return lbl1.Height
        End Get
        Set(ByVal value As Integer)
            Call subDoLayout(value)
        End Set
    End Property
    Property LabelSpacing() As Integer
        '********************************************************************************************
        'Description:  give access to label BorderStyle properties
        '
        'Parameters:  
        'Returns:    returns lbl1.BorderStyle, sets all of them
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If lbl2 Is Nothing Then
                Return 0
            Else
                Return lbl2.Top - (lbl1.Top + lbl1.Height)
            End If
            Return lbl1.Height
        End Get
        Set(ByVal value As Integer)
            Call subDoLayout(, value)
        End Set
    End Property
    Property LabelBorderStyle(ByVal index As Integer) As BorderStyle
        '********************************************************************************************
        'Description:  give access to label BorderStyle properties
        '
        'Parameters:  label index - 0 sets all or returns property of lbl1
        'Returns:     a label's BorderStyle
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                    Return Nothing
                Else
                    Return olbl.BorderStyle
                End If
            Else
                Return lbl1.BorderStyle
            End If
        End Get
        Set(ByVal value As BorderStyle)
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                Else
                    olbl.BorderStyle = value
                End If
            Else
                For Each olbl3 As Label In Me.Controls
                    olbl3.BorderStyle = value
                Next
            End If
        End Set
    End Property
    Property LabelText(ByVal index As Integer) As String
        '********************************************************************************************
        'Description:  give access to label text
        '
        'Parameters:  label index - 0 in the set will set them all, not valid for get
        'Returns:     a label's text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                    Return Nothing
                Else
                    Return olbl.Text
                End If
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As String)
            If index > 0 And index <= mnNumLabels Then
                Dim olbl As Label = DirectCast(Me.Controls("lbl" & index.ToString), Label)
                If olbl Is Nothing Then
                    Debug.Assert(False)
                Else
                    olbl.Text = value
                End If
            Else
                For Each olbl3 As Label In Me.Controls
                    olbl3.Text = value
                Next
            End If
        End Set
    End Property
    Property NumLabels() As Integer
        '********************************************************************************************
        'Description:  set the number of labels to display
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnNumLabels
        End Get
        Set(ByVal value As Integer)
            Dim nLabel As Integer
            Dim olbl As Label
            Dim nMinHeight As Integer = 0
            If mnNumLabels > value Then
                For nLabel = value + 1 To mnNumLabels
                    olbl = DirectCast(Me.Controls("lbl" & value.ToString), Label)
                    If Not (olbl Is Nothing) Then
                        Me.Controls.Remove(olbl)
                        olbl.Dispose()
                    End If
                Next
            ElseIf mnNumLabels < value Then
                For nLabel = mnNumLabels + 1 To value
                    olbl = New Label()
                    olbl.Name = "lbl" & nLabel.ToString
                    olbl.Font = Me.Font
                    olbl.Text = String.Empty
                    olbl.BorderStyle = lbl1.BorderStyle
                    olbl.BackColor = lbl1.BackColor
                    olbl.ForeColor = lbl1.ForeColor
                    olbl.TextAlign = lbl1.TextAlign
                    olbl.Size = lbl1.Size
                    olbl.Left = lbl1.Left
                    nMinHeight = lbl1.Top + (lbl2.Top - lbl1.Top) * (nLabel - 1)
                    olbl.Top = nMinHeight
                    olbl.Anchor = lbl1.Anchor
                    olbl.Visible = True
                    olbl.CreateControl()
                    olbl.Show()
                    olbl.Parent = Me
                    AddHandler olbl.Click, AddressOf Do_Mouse_Click
                    AddHandler olbl.MouseDown, AddressOf Do_Mouse_Down
                    Me.Controls.Add(olbl)
                Next
            End If
            Me.Height = nMinHeight + lbl1.Height + 3
            mnNumLabels = value
        End Set
    End Property

    Private Sub subDoLayout(Optional ByVal nHieght As Integer = 0, Optional ByVal nSpace As Integer = -1)
        If nHieght <= 0 Then
            nHieght = lbl1.Height
        End If
        If nSpace < 0 Then
            nSpace = lbl2.Top - (lbl1.Top + nHieght)
        End If
        Dim olbl As Label
        Dim nLastTop As Integer = 0
        For nLabel As Integer = 1 To mnNumLabels
            olbl = DirectCast(Me.Controls("lbl" & nLabel.ToString), Label)
            olbl.Height = nHieght
            olbl.Top = nLastTop + nSpace
            nLastTop = nLastTop + nSpace + olbl.Height
            olbl.Font = Me.Font
        Next
        Me.Height = nLastTop + nSpace

    End Sub
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Do_Mouse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbl1.Click, lbl2.Click, Me.Click
        '********************************************************************************************
        'Description: Mouse click event, pass out to the owner of the user control.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            RaiseEvent Mouse_Click(Me)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Private Sub Do_Mouse_Down(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbl1.mousedown, lbl2.mousedown, Me.mousedown
        '********************************************************************************************
        'Description: Mouse Down event, pass out to the owner of the user control.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            RaiseEvent Mouse_Down(Me)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
End Class
