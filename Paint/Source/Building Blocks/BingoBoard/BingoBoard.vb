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
' Form/Module: BingoBoard
'
' Description: User configurable bingo board
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Ricko
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                       Version
'    04/11/12   MSW     SubUpdateStatus - Trap some odd errors so it doesn't bomb out at run time. 4.1.3.0
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'    04/14/14   MSW     From Fairfax sealer - support 32 bit data                     4.01.07.01
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.ComponentModel

Public Class BingoBoard

#Region " Declares "

    '******** Property Variables ****************************************************************
    Private mbAutosizeColumns As Boolean = True
    Private mbEnableToolTips As Boolean = False
    Private mbLocked As Boolean = False
    Private mbTitleVisible As Boolean = True
    Private mnColumns As Integer = 1
    Private mnColumnWidth As Integer = 150
    Private mnItemCount As Integer = 0
    Private mnItemHorizSpace As Integer = 5
    Private mnItemVertSpace As Integer = 0
    Private msItemBitIndex As String()
    Private msItemData As String()
    Private msItemOffText As String()
    Private msItemOnText As String()
    Private msItemToolTipText As String()
    Private mBackColor As System.Drawing.Color = Color.Black
    Private mItemErrorColor As System.Drawing.Color = Color.Silver
    Private mItemOffColor As System.Drawing.Color = Color.Red
    Private mItemOnColor As System.Drawing.Color = Color.Lime
    '******** End Property Variables ************************************************************

    '******** Events ****************************************************************************
    Public Event ItemClicked(ByRef sender As BingoBoard, ByVal Item As Integer, ByVal Mousebutton As System.Windows.Forms.MouseButtons)
    Public Event ItemEntered(ByRef sender As BingoBoard, ByVal Item As Integer)
    '******** End Events ************************************************************************

#End Region

#Region " Properties "

    <Category("Layout"), Browsable(True), Description("Enables automatic column width resizing based on item text and font.")> _
    Public Property AutosizeColumns() As Boolean
        '********************************************************************************************
        'Description: If this property is set to True, we'll automatically size the widths of the 
        '             column(s) and subsequently the control itself based on the widest item text. 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbAutosizeColumns
        End Get

        Set(ByVal value As Boolean)
            mbAutosizeColumns = value
            Call subDoLayout()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("Sets the Background color of the control and all text.")> _
    Public Overrides Property BackColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The UserControl.BackColor property is overridden here so we can do our own thing.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mBackColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            mBackColor = value
            pnlBingoBoard.BackColor = value

            For Each oLabel As Label In pnlBingoBoard.Controls
                oLabel.BackColor = value
            Next 'oLabel
        End Set

    End Property

    <Category("Layout"), Browsable(True), Description("The number of columns to display.")> _
    Public Property Columns() As Integer
        '********************************************************************************************
        'Description: Gets/Sets the number of columns to be displayed.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnColumns
        End Get

        Set(ByVal value As Integer)
            If value >= 1 Then
                mnColumns = value
            Else
                mnColumns = 1
            End If
            Call subDoLayout()
        End Set

    End Property

    <Category("Layout"), Browsable(True), Description("The width of Item columns. AutosizeColumns = True overrides this property.")> _
    Public Property ColumnWidth() As Integer
        '********************************************************************************************
        'Description: Fixes the width of columns. AutosizeColumns = True overrides this property.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnColumnWidth
        End Get

        Set(ByVal value As Integer)
            mnColumnWidth = value
            If Not AutosizeColumns Then Call subDoLayout()
        End Set

    End Property

    <Category("Behavior"), Browsable(True), _
    Description("Enables Host application ToolTip display when the user clicks an Item.")> _
    Public Property EnableToolTips() As Boolean
        '********************************************************************************************
        'Description: Tells the host application if a ToolTip should be displayed when the user
        '             clicks an Item in the application specific manner.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbEnableToolTips
        End Get

        Set(ByVal value As Boolean)
            mbEnableToolTips = value
        End Set

    End Property

    <Category("Data"), Browsable(True), Description("The Data Bit Index (0-based) corresponding to each item to display.")> _
    Public Property ItemBitIndex() As String()
        '********************************************************************************************
        'Description: This property allows a dataview containing the bingoboard BitIndex, OffText and
        '             OnText to be passed in at runtime.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msItemBitIndex
        End Get

        Set(ByVal value As String())
            msItemBitIndex = value

            'Make sure we have enough ItemData
            Call subBuildItemDataArray()

            'If we have data, update the status
            If Not IsNothing(ItemData) Then
                Call subUpdateStatus()
            End If
        End Set

    End Property

    <Category("Layout"), Browsable(True), Description("The number of Items to display.")> _
    Public Property ItemCount() As Integer
        '********************************************************************************************
        'Description: The number of Items to display.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnItemCount
        End Get

        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            If mnItemCount <> value Then
                'Reformat the control for the new item count
                Call subItemCountChanged(mnItemCount, value)

                mnItemCount = value

                'Resize the control (if necessary) and locate all items
                Call subDoLayout()
            End If
        End Set

    End Property

    <Category("Data"), Browsable(True), Description("The 16-Bit Data word(s) that drive display.")> _
    Public Property ItemData() As String()
        '********************************************************************************************
        'Description: The data used to determine if the OffText or OnText for each item should be
        '             displayed based on the state of the Item's BitIndex bit in the data word. 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msItemData
        End Get

        Set(ByVal value As String())
            msItemData = value
            Call subUpdateStatus()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Foreground Color of Item text when an Error occurs.")> _
    Public Property ItemErrorColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The Text Color used to display BingoBoard Item Text when a data error occurs
        '             or the ItemBitIndex for this item hasn't been set.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mItemErrorColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            mItemErrorColor = value
            Call subItemColorChanged()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The font used to display BingoBoard Item text.")> _
    Public Property ItemFont() As Font
        '********************************************************************************************
        'Description: The font used to display BingoBoard Item Text.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblTemplate.Font
        End Get

        Set(ByVal value As Font)
            For Each oLabel As Label In pnlBingoBoard.Controls
                If Strings.Left(oLabel.Name, 7) = "lblItem" Then
                    oLabel.Font = value
                End If
            Next 'oLabel
            lblTemplate.Font = value
            If ItemCount > 0 Then Call subDoLayout()
        End Set

    End Property

    <Category("Layout"), Browsable(True), Description("The amount of space between Item Columns.")> _
    Public Property ItemHorizSpace() As Integer
        '********************************************************************************************
        'Description: The amount of space between Item Columns.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnItemHorizSpace
        End Get

        Set(ByVal value As Integer)
            mnItemHorizSpace = value
            Call subDoLayout()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Foreground Color of Item text in the OFF state.")> _
    Public Property ItemOffColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The Text Color used to display BingoBoard Item Text when the item is in the
        '             OFF state.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mItemOffColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            mItemOffColor = value
            Call subItemColorChanged()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Text to display when the Item is in the OFF state.")> _
    Public Property ItemOffText() As String()
        '********************************************************************************************
        'Description: The Text to display each BingoBoard Item when the item is in the OFF state.
        '"System.Drawing.Design.UITypeEditor, System.Drawing"
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msItemOffText
        End Get

        Set(ByVal value As String())
            msItemOffText = value
            If Locked Then
                If Not IsNothing(ItemData) Then
                    Call subUpdateStatus()
                End If
            Else
                Call subDoLayout()
            End If
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Foreground Color of Item text in the ON state.")> _
    Public Property ItemOnColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The Text Color used to display BingoBoard Item Text when the item is in the
        '             ON state.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mItemOnColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            mItemOnColor = value
            Call subItemColorChanged()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Text to display when the Item is in the ON state.")> _
    Public Property ItemOnText() As String()
        '********************************************************************************************
        'Description: The Text to display each BingoBoard Item when the item is in the ON state.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msItemOnText
        End Get

        Set(ByVal value As String())
            msItemOnText = value
            If Locked Then
                If Not IsNothing(ItemData) Then
                    Call subUpdateStatus()
                End If
            Else
                Call subDoLayout()
            End If
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Tooltip Text to display each BingoBoard Item.")> _
    Public Property ItemToolTipText() As String()
        '********************************************************************************************
        'Description: The Tooltip Text to display each BingoBoard Item.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msItemToolTipText
        End Get

        Set(ByVal value As String())
            msItemToolTipText = value
        End Set

    End Property

    <Category("Layout"), Browsable(True), Description("The amount of space between Item Rows.")> _
    Public Property ItemVertSpace() As Integer
        '********************************************************************************************
        'Description: The amount of space between Item Rows.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnItemVertSpace
        End Get

        Set(ByVal value As Integer)
            mnItemVertSpace = value
            Call subDoLayout()
        End Set

    End Property

    <Category("Behavior"), Browsable(True), Description("Lock out layout changes.")> _
    Public Property Locked() As Boolean
        '********************************************************************************************
        'Description: Locks out layou changes so ItemONText and/or ItemOFFText can be changed
        '             dynamically without causing the control to flicker.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbLocked
        End Get

        Set(ByVal value As Boolean)
            mbLocked = value
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Background Color of the Title text.")> _
    Public Property TitleBackColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The BingoBoard Title Text background color.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblTitle.BackColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            lblTitle.BackColor = value
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Foreground Color of the Title text.")> _
    Public Property TitleColor() As System.Drawing.Color
        '********************************************************************************************
        'Description: The Text Color used to display BingoBoard Title Text.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblTitle.ForeColor
        End Get

        Set(ByVal value As System.Drawing.Color)
            lblTitle.ForeColor = value
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The font used to display BingoBoard Title text.")> _
    Public Property TitleFont() As Font
        '********************************************************************************************
        'Description: The font used to display BingoBoard Title Text.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblTitle.Font
        End Get

        Set(ByVal value As Font)
            With lblTitle
                .Dock = DockStyle.None
                .AutoSize = True
                .Font = value
                If TitleVisible Then
                    If .Width > Me.Width Then Me.Width = .Width + Me.Padding.Left + Me.Padding.Right
                End If
                .Dock = DockStyle.Top
            End With 'lbltitle
            Call subDoLayout()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("The Title Text to display.")> _
    Public Property TitleText() As String
        '********************************************************************************************
        'Description: The Title Text to display.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblTitle.Text
        End Get

        Set(ByVal value As String)
            Dim nWidth As Integer = Me.Width

            Application.DoEvents()

            With lblTitle
                .Dock = DockStyle.None
                .AutoSize = True
                .Text = value

                If TitleVisible Then
                    If .Width > nWidth Then nWidth = .Width + Me.Padding.Left + Me.Padding.Right
                End If

                Me.Width = nWidth
                Application.DoEvents()
                .AutoSize = False
                .Dock = DockStyle.Top

            End With 'lblTitle
            Call subDoLayout()
        End Set

    End Property

    <Category("Appearance"), Browsable(True), Description("Determines whether the Title is visible or hidden.")> _
    Public Property TitleVisible() As Boolean
        '********************************************************************************************
        'Description: Show or hide the Title.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbTitleVisible
        End Get

        Set(ByVal value As Boolean)
            If value Then
                With lblTitle
                    .Dock = DockStyle.None
                    .AutoSize = True
                    If .Width > Me.Width Then Me.Width = .Width + Me.Padding.Left + Me.Padding.Right
                    .Dock = DockStyle.Top
                End With
            End If
            lblTitle.Visible = value
            mbTitleVisible = value
            Call subDoLayout()
        End Set

    End Property
#End Region

#Region " Routines "

    Private Function GetColumn(ByVal ItemsPerCol As Integer, ByVal Index As Integer) As Integer
        '********************************************************************************************
        'Description: Returns the 0-based column number this item (Index) belongs in.
        '
        'Parameters: Item Index 
        'Returns:    Column Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nCol As Integer = 0

        For nCol = 1 To Columns
            If Index < (ItemsPerCol * nCol) Then Exit For
        Next

        Return nCol - 1

    End Function

    Private Function GetMaxItemWidth() As Integer
        '********************************************************************************************
        'Description: Returns the Width of the widest Item label.
        '
        'Parameters: None 
        'Returns:    MaxItemWidth
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nMaxWidth As Integer = 0

        If ItemCount > 0 Then

            For Each oLabel As Label In pnlBingoBoard.Controls
                If Strings.Left(oLabel.Name, 7) = "lblItem" Then
                    Dim nIndex As Integer = CType(oLabel.Tag, Integer)

                    With oLabel
                        .Text = ItemOnText(nIndex)
                        If .Width > nMaxWidth Then
                            nMaxWidth = .Width
                        End If
                        .Text = ItemOffText(nIndex)
                        If .Width > nMaxWidth Then
                            nMaxWidth = .Width
                        End If
                    End With 'oLabel
                End If 'Strings.Left...
            Next 'oLabel

        Else
            nMaxWidth = GetTitleWidth()
        End If

        Return nMaxWidth

    End Function

    Private Function GetRow(ByVal ItemsPerCol As Integer, ByVal Index As Integer) As Integer
        '********************************************************************************************
        'Description: Returns the 0-based Row number this item (Index) belongs in.
        '
        'Parameters: Item Index 
        'Returns:    Row Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nCol As Integer = 0
        Dim nRow As Integer = 0

        For nCol = 1 To Columns
            If Index < (ItemsPerCol * nCol) Then Exit For
        Next

        nRow = Index - (ItemsPerCol * (nCol - 1))
        Return nRow

    End Function

    Private Function GetTitleWidth() As Integer
        '********************************************************************************************
        'Description: Returns the Width of the Title Text.
        '
        'Parameters: None 
        'Returns:    Title Width
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nTitleWidth As Integer = 0

        With lblTitle
            .Dock = DockStyle.None
            .AutoSize = True
            nTitleWidth = .Width
            .AutoSize = False
            .Dock = DockStyle.Top
        End With 'llblTitle

        Return nTitleWidth

    End Function

    Private Sub subBuildItemDataArray()
        '********************************************************************************************
        'Description: Use the ItemBitIndex array to guess how many ItemData words we'll need.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nDataWords As Integer = 0

        For nIndex As Integer = 0 To msItemBitIndex.GetUpperBound(0)
            If IsNumeric(msItemBitIndex(nIndex)) Then
                Dim nBit As Integer = CType(msItemBitIndex(nIndex), Integer)
                Dim nWord As Integer = nBit \ 32

                If nWord > nDataWords Then nDataWords = nWord
            End If
        Next 'nIndex

        ReDim msItemData(nDataWords)
        For nIndex As Integer = 0 To nDataWords
            msItemData(nIndex) = "0"
        Next 'sData

    End Sub

    Private Sub subDoLayout()
        '********************************************************************************************
        'Description: Something has changed that affects the layout and/or size of the control. Redo
        '             the layout of the control.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************
        Dim nItemsPerCol As Integer = ItemCount
        Dim nColWidth As Integer = ColumnWidth
        Dim n3DFudge As Integer = 0

        If Columns > 1 Then
            'Cipher the number of items in each column
            nItemsPerCol = ItemCount \ Columns
            If (ItemCount Mod Columns) > 0 Then nItemsPerCol += 1
        End If

        'Get the column width
        If AutosizeColumns Then nColWidth = GetMaxItemWidth()
        If TitleVisible Then
            Dim nTitlewidth As Integer = GetTitleWidth()

            If (Columns = 1) And (nTitlewidth > nColWidth) Then
                nColWidth = nTitlewidth
            End If
        End If

        'Size the control
        If Me.BorderStyle = Windows.Forms.BorderStyle.Fixed3D Then n3DFudge = 6
        Me.Height = Me.Padding.Top + (nItemsPerCol * (ItemVertSpace + lblTemplate.Height)) + Me.Padding.Bottom
        If TitleVisible Then
            Me.Height += (lblTitle.Height + ItemVertSpace + Me.Padding.Top + n3DFudge)
        Else
            Me.Height += (n3DFudge + 5) 'Fudge Factor
        End If
        Me.Width = Me.Padding.Left + (Columns * nColWidth) + ((Columns - 1) * ItemHorizSpace) + Me.Padding.Right + n3DFudge + 2 'Fudge Factor

        'Locate the Item Labels by index 
        Dim nTop As Integer = 0
        lblTitle.Top = 0

        If TitleVisible Then
            nTop += (lblTitle.Height + ItemVertSpace + Me.Padding.Top)
        Else
            nTop += 5 'More Fudge
        End If
        For Each oLabel As Label In pnlBingoBoard.Controls
            If Strings.Left(oLabel.Name, 7) = "lblItem" Then
                Dim nIndex As Integer = CType(oLabel.Tag, Integer)

                With oLabel
                    .Top = nTop + (GetRow(nItemsPerCol, nIndex) * (.Height + ItemVertSpace))
                    .Left = GetColumn(nItemsPerCol, nIndex) * (nColWidth + ItemHorizSpace)
                    .Text = ItemOffText(nIndex)
                End With 'oLabel
            End If 'Strings.Left...
        Next 'oLabel

        'If we have data, update the status
        If Not IsNothing(ItemData) Then
            Call subUpdateStatus()
        End If

    End Sub

    Private Sub subItemColorChanged()
        '********************************************************************************************
        'Description: A color assigned to one of the item text states has changed. Make sure all
        '             items currently in that state are shown in the new color.
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If ItemCount > 0 Then
            For Each oLabel As Label In pnlBingoBoard.Controls
                If Strings.Left(oLabel.Name, 7) = "lblItem" Then
                    Dim nIndex As Integer = CType(oLabel.Tag, Integer)

                    Select Case oLabel.Text
                        Case ItemOffText(nIndex)
                            oLabel.ForeColor = ItemOffColor

                        Case ItemOnText(nIndex)
                            oLabel.ForeColor = ItemOnColor

                        Case Else 'Error Text
                            oLabel.ForeColor = ItemErrorColor
                    End Select
                End If 'Strings.Left...
            Next 'oLabel

        End If 'ItemCount > 0

    End Sub

    Private Sub subItemCountChanged(ByVal OldCount As Integer, ByVal NewCount As Integer)
        '********************************************************************************************
        'Description: The number of items has changed. 
        '
        'Parameters: Old item count and Current item count 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Adjust the number of Item labels to match the new item count
        If OldCount < NewCount Then
            Dim myPad As Padding

            myPad.Left = 2 'Fudge Factor to help center the column
            'Add labels for the new Items
            For nItem As Integer = OldCount To (NewCount - 1)
                Dim oLabel As New Label

                With oLabel
                    .Name = "lblItem" & nItem.ToString
                    .AutoSize = True
                    .Anchor = lblTemplate.Anchor
                    .BackColor = BackColor
                    .ForeColor = ItemOffColor
                    .Font = ItemFont
                    .Padding = myPad
                    .Tag = nItem
                    .Text = "Item " & nItem.ToString & " OFF"
                End With 'oLabel

                pnlBingoBoard.Controls.Add(oLabel)
                AddHandler oLabel.MouseClick, AddressOf BingoBoard_ItemClicked
                AddHandler oLabel.MouseEnter, AddressOf BingoBoard_ItemEntered
            Next 'nItem

        Else

            'Delete the extra Item labels
            For nIndex As Integer = (pnlBingoBoard.Controls.Count - 1) To 0 Step -1
                If Strings.Left(pnlBingoBoard.Controls.Item(nIndex).Name, 7) = "lblItem" Then
                    Dim oLabel As Label = DirectCast(pnlBingoBoard.Controls.Item(nIndex), Label)

                    If CType(oLabel.Tag, Integer) > (NewCount - 1) Then
                        RemoveHandler oLabel.MouseClick, AddressOf BingoBoard_ItemClicked
                        RemoveHandler oLabel.MouseEnter, AddressOf BingoBoard_ItemEntered
                        pnlBingoBoard.Controls.Remove(oLabel)
                        oLabel.Dispose()
                        oLabel = Nothing
                    End If
                End If
            Next 'nIndex

        End If 'OldCount < NewCount

        'Redim the ItemOffText, ItemOnText, and ItemBitIndex arrays to match the new item count
        ReDim Preserve msItemBitIndex(NewCount - 1)
        ReDim Preserve msItemOffText(NewCount - 1)
        ReDim Preserve msItemOnText(NewCount - 1)
        ReDim Preserve msItemToolTipText(NewCount - 1)

        'Create default OffText, OnText, and BitIndex values for newly added items
        For nIndex As Integer = 0 To (NewCount - 1)
            If msItemBitIndex(nIndex) = String.Empty Then msItemBitIndex(nIndex) = "-1"
            If msItemOffText(nIndex) = String.Empty Then msItemOffText(nIndex) = "Item " & nIndex.ToString & " OFF"
            If msItemOnText(nIndex) = String.Empty Then msItemOnText(nIndex) = "Item " & nIndex.ToString & " ON"
            If msItemToolTipText(nIndex) = String.Empty Then msItemToolTipText(nIndex) = "Item " & nIndex.ToString & " ToolTip"
        Next 'nIndex

        'Make sure we have enough ItemData
        Call subBuildItemDataArray()

    End Sub

    Private Sub subUpdateStatus()
        '********************************************************************************************
        'Description: The data that drives the display has changed. Update the ItemText for each Item.
        '
        'Parameters: Data - Array of strings, one numeric string for each 16-bit data word
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/11/12  MSW     SubUpdateStatus - Trap some odd errors so it doesn't bomb out at run time.
        '********************************************************************************************

        For Each oLabel As Label In pnlBingoBoard.Controls
            If Strings.Left(oLabel.Name, 7) = "lblItem" Then
                Dim nIndex As Integer = CType(oLabel.Tag, Integer)
                Dim bError As Boolean = False
                Dim nBitIndex As Integer = -1

                If ItemBitIndex.length > 0 andalso  IsNumeric(ItemBitIndex(nIndex)) Then
                    nBitIndex = CType(ItemBitIndex(nIndex), Integer)
                End If

                If nBitIndex >= 0 Then
                    'Cipher the Data word # and the Bit Number (0-15) in that word to test
                    Dim nWord As Integer = nBitIndex \ 32
                    Dim nBit As Integer = nBitIndex Mod 32

                    If nWord <= ItemData.GetUpperBound(0) Then
                        If IsNumeric(ItemData(nWord)) Then
                            Dim nMaskedData As Integer = CType(ItemData(nWord), Integer) And 65535
                            If nMaskedData > 32767 Then nMaskedData -= 65536

                            Dim nData As Long = CType(nMaskedData, Long)

                            If (nData And CType(2 ^ nBit, Long)) > 0 Then
                                oLabel.Text = ItemOnText(nIndex)
                                oLabel.ForeColor = ItemOnColor
                            Else
                                oLabel.Text = ItemOffText(nIndex)
                                oLabel.ForeColor = ItemOffColor
                            End If
                        Else
                            'Bad Data
                            bError = True
                        End If
                    Else
                        'The data word for this bit doesn't exist
                        bError = True
                    End If

                        If bError Then
                            oLabel.Text = "<Data Error>"
                            oLabel.ForeColor = ItemErrorColor
                        End If
                    Else
                        'The ItemBitIndex hasn't been set for this Item
                        oLabel.Text = "<BitIndex Error>"
                        oLabel.ForeColor = ItemErrorColor
                    End If 'nBitIndex >= 0
                End If 'Strings.Left...
        Next 'oLabel

    End Sub

#End Region

#Region " Events "

    Private Sub BingoBoard_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description: An Item has been clicked. Raise an event to the parent.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLabel As Label = DirectCast(sender, Label)

        RaiseEvent ItemClicked(Me, CType(oLabel.Tag, Integer), e.Button)

    End Sub

    Private Sub BingoBoard_ItemEntered(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: An BingoBoard Item has been entered by the cursor. Raise an event to the parent.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLabel As Label = DirectCast(sender, Label)

        RaiseEvent ItemEntered(Me, CType(oLabel.Tag, Integer))

    End Sub

    Private Sub BingoBoard_PaddingChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PaddingChanged
        '********************************************************************************************
        'Description: Adjust the control to make everything fit with the new padding.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subDoLayout()

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description: Initialize the string arrays when the control is created.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim msItemBitIndex(4)
        ReDim msItemData(4)
        ReDim msItemOffText(4)
        ReDim msItemOnText(4)

        msItemBitIndex(0) = "-1"
        msItemData(0) = "0"
        msItemOffText(0) = "Item 0 OFF"
        msItemOnText(0) = "Item 0 ON"

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DoubleBuffered = True

    End Sub

#End Region


End Class
