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
' Form/Module: frmWatch
'
' Description: Watch Window for PLC I/O screen
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
'********************************************************************************************

Imports Response = System.Windows.Forms.DialogResult
Imports BB = BingoBoard.BingoBoard

Friend Class frmWatch

#Region " Declares "

    '******** Form Constants   **********************************************************************
    Private Const msMODULE As String = "frmWatch"
    '******** End Form Constants   ******************************************************************

    '******** Form Variables   **********************************************************************
    Private msCulture As String = "en-US" 'Default to english
    Private mcolItemInfo As New Collection
    Private msPLCData() As String
    '******** End Form Variables   ******************************************************************

    '******** Form Structures    ********************************************************************
    Private Structure udsItemConfig
        Public ModuleType As String
        Public DataIndex As Integer
        Public DataBit As Integer
        Public ResxTag As String
        Public Rack As String
        Public Slot As String
        Public Point As String
    End Structure
    '******** End Form Structures    ****************************************************************

#End Region

#Region " Properties "

    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msCulture = value
            'Redo screen text, etc. for new culture
            'Code Here ->
        End Set

    End Property

    Friend WriteOnly Property PLCData() As String()
        '********************************************************************************************
        'Description:  Update the status of items in the Watch Window.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String())
            msPLCData = value
            Call subUpdateData(False)
        End Set

    End Property

#End Region

#Region " Routines "

    Friend Sub AddItem(ByVal BingoBoard As BB, ByVal Item As Integer)
        '********************************************************************************************
        'Description:  Add the Item from the BingoBoard argument to the Watch Window
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItemCount As Integer = mcolItemInfo.Count
        Dim oItemConfig As udsItemConfig
        Dim sBitIndex() As String = BBWatch.ItemBitIndex
        Dim sData() As String = BBWatch.ItemData
        Dim sOffText() As String = BBWatch.ItemOffText
        Dim sOnText() As String = BBWatch.ItemOnText

        Try
            ReDim Preserve sBitIndex(nItemCount)
            ReDim Preserve sData(nItemCount)
            ReDim Preserve sOffText(nItemCount)
            ReDim Preserve sOnText(nItemCount)

            With BingoBoard
                Dim sTemp() As String = Strings.Split(CType(.Tag, String), ",")

                'Add information about this item to the collection
                oItemConfig.ModuleType = sTemp(1)
                oItemConfig.DataIndex = CType(sTemp(2), Integer)
                If oItemConfig.ModuleType = "A" Then oItemConfig.DataIndex += Item
                oItemConfig.DataBit = CType(.ItemBitIndex(Item), Integer)
                If sTemp(5).Length = 1 Then
                    sTemp(5) = "0" & sTemp(5)
                End If
                If .EnableToolTips Then
                    oItemConfig.ResxTag = "lsR" & sTemp(4) & "S" & sTemp(5) & "M" & sTemp(6) & "_" & Strings.Format(oItemConfig.DataBit, "00")
                Else
                    oItemConfig.ResxTag = String.Empty
                End If
                oItemConfig.Rack = sTemp(4)
                oItemConfig.Slot = sTemp(5)
                oItemConfig.Point = Strings.Format(oItemConfig.DataBit, "00")
                mcolItemInfo.Add(oItemConfig)
                'DEBUG>>>MessageBox.Show("DataIndex = " & oItemConfig.DataIndex.ToString)

                'Add this item to BBWatch
                Dim sItemBitIndex() As String = .ItemBitIndex
                Dim sItemData() As String = .ItemData
                Dim sItemOffText() As String = .ItemOffText
                Dim sItemOnText() As String = .ItemOnText
                Dim nBitIndex As Integer = CType(sItemBitIndex(Item), Integer)


                If oItemConfig.ModuleType = "A" Then
                    'Special case for Analog Modules so the Item will always be ItemOnColor
                    sData(nItemCount) = "-1"
                Else
                    sData(nItemCount) = msPLCData(oItemConfig.DataIndex)
                End If

                BBWatch.ItemCount = nItemCount + 1
                nBitIndex += (nItemCount * 32) 'NRU 161005 Bingo board resized to 32 bits changed * 16 to * 32
                sBitIndex(nItemCount) = nBitIndex.ToString(CultureInfo.InvariantCulture)
                BBWatch.ItemBitIndex = sBitIndex
                BBWatch.ItemData = sData
                sOffText(nItemCount) = sItemOffText(Item)
                BBWatch.ItemOffText = sOffText
                sOnText(nItemCount) = sItemOnText(Item)
                BBWatch.ItemOnText = sOnText

                'Resize to accomodate the new BingoBoard size
                Dim nCols As Integer = (nItemCount \ 32) + 1  'NRU 161005 Bingo board resized to 32 bits changed \ 16 to \ 32

                If BBWatch.Columns <> nCols Then BBWatch.Columns = nCols
                If (BBWatch.Width + 10) > Me.Width Then Me.Width = BBWatch.Width + 10
                If (BBWatch.Height + 40) > Me.Height Then Me.Height = BBWatch.Height + 40

            End With 'BingoBoard

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: AddItem, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: AddItem, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Function ItemExists(ByVal BingoBoard As BB, ByVal Item As Integer) As Boolean
        '********************************************************************************************
        'Description:  Check if the item already exists in the WatchWindow collection.
        '
        'Parameters: Bingoboard containing the selectem item, Item number
        'Returns:    True if the item is alreay shown on the WatchWindow, False if not
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bFound As Boolean = False

        If mcolItemInfo.Count = 0 Then Return bFound

        Try
            Dim oItemConfig As udsItemConfig
            Dim sTemp() As String = Strings.Split(CType(BingoBoard.Tag, String), ",")

            For nIndex As Integer = 1 To mcolItemInfo.Count
                Dim nDataIndex As Integer = CType(sTemp(2), Integer)
                Dim nBitIndex As Integer = CType(BingoBoard.ItemBitIndex(Item), Integer)

                oItemConfig = DirectCast(mcolItemInfo(nIndex), udsItemConfig)

                If oItemConfig.ModuleType = "A" Then
                    nDataIndex = nDataIndex + Item
                End If

                If (oItemConfig.DataIndex = nDataIndex) And (oItemConfig.DataBit = nBitIndex) Then
                    bFound = True
                    Exit For
                End If
            Next 'nIndex

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: ItemExists, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: ItemExists, StackTrace: " & ex.StackTrace)
        End Try

        Return bFound

    End Function

    Private Sub subRemoveItem(ByVal Item As Integer)
        '********************************************************************************************
        'Description:  Remove the specified Item from the Watch Window.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItemCount As Integer = mcolItemInfo.Count - 1

        Try
            If nItemCount = 0 Then
                Me.Close()
            Else
                Dim sBit() As String = BBWatch.ItemBitIndex
                Dim sData() As String = BBWatch.ItemData
                Dim sOffText() As String = BBWatch.ItemOffText
                Dim sOnText() As String = BBWatch.ItemOnText

                If Item < nItemCount Then
                    For nIndex As Integer = Item To (nItemCount - 1)
                        sBit(nIndex) = sBit(nIndex + 1)
                        'The item number has changed. If this is a digital I/O module, adjust the bit 
                        'index so it's pointing to the correct data word.
                        If CType(sBit(nIndex), Integer) > 15 Then
                            sBit(nIndex) = _
                                (CType(sBit(nIndex), Integer) - 16).ToString(CultureInfo.InvariantCulture)
                        End If
                        sData(nIndex) = sData(nIndex + 1)
                        sOffText(nIndex) = sOffText(nIndex + 1)
                        sOnText(nIndex) = sOnText(nIndex + 1)
                    Next 'nIndex
                End If

                ReDim Preserve sBit(nItemCount - 1)
                ReDim Preserve sData(nItemCount - 1)
                ReDim Preserve sOffText(nItemCount - 1)
                ReDim Preserve sOnText(nItemCount - 1)

                With BBWatch
                    .ItemCount = nItemCount
                    .ItemBitIndex = sBit
                    .ItemData = sData
                    .ItemOffText = sOffText
                    .ItemOnText = sOnText
                End With

                mcolItemInfo.Remove(Item + 1)
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subRemoveItem, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subRemoveItem, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateData(ByVal UpdateAll As Boolean)
        '********************************************************************************************
        'Description:  Update the status of the BingoBoard items.
        '
        'Parameters: UpdateAll - False = Only do the ones with changed data
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bUpdate As Boolean = False
        Dim nItem As Integer = 0
        Dim sData() As String = BBWatch.ItemData

        Try
            For nIndex As Integer = 1 To mcolItemInfo.Count
                Dim oItemInfo As udsItemConfig = DirectCast(mcolItemInfo.Item(nIndex), udsItemConfig)

                nItem = nIndex - 1

                Select Case oItemInfo.ModuleType
                    Case "A" 'Analog
                        Dim Culture As New System.Globalization.CultureInfo(msCulture)
                        Dim sOnText() As String = BBWatch.ItemOnText
                        Dim sNewData As String = frmMain.glsRM.GetString(oItemInfo.ResxTag, Culture)

                        'For Analog modules, use the ON Text string to display the value. Construct 
                        'a new ON Text string and compare the value to the current.
                        sNewData = Strings.Left(sNewData, Strings.InStr(sNewData, ",") - 1)
                        sNewData = sNewData & frmMain.glsRM.GetString("lsDELIMITER", Culture)
                        sNewData = sNewData & msPLCData(oItemInfo.DataIndex)

                        If UpdateAll Or (sOnText(nItem) <> sNewData) Then
                            sOnText(nItem) = sNewData
                            BBWatch.ItemOnText = sOnText
                        End If

                    Case "D" 'Digital
                        If UpdateAll Or (sData(nItem) <> msPLCData(oItemInfo.DataIndex)) Then
                            sData(nItem) = msPLCData(oItemInfo.DataIndex)
                            bUpdate = True
                        End If

                    Case Else
                        'Something's gone wrong
                End Select
            Next 'nIndex

            If bUpdate Then BBWatch.ItemData = sData

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subUpdateData, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subUpdateData, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub BBWatch_ItemClicked(ByRef sender As BingoBoard.BingoBoard, ByVal Item As Integer, ByVal Mousebutton As System.Windows.Forms.MouseButtons) Handles BBWatch.ItemClicked
        '********************************************************************************************
        'Description: The user clicked a BingoBoard Item.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Culture As CultureInfo = frmMain.DisplayCulture

        Select Case Mousebutton
            Case Windows.Forms.MouseButtons.Left
                'Ask the user if this item should be removed from the WatchWindow
                Dim lRet As DialogResult = MessageBox.Show(gpsRM.GetString("psWW_REMOVE_MSG", Culture), _
                                           gpsRM.GetString("psWW_CAPTION", Culture), _
                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

                If lRet = Windows.Forms.DialogResult.OK Then
                    Call subRemoveItem(Item)
                End If

            Case Windows.Forms.MouseButtons.Right
                'Show the ToolTip
                Dim oItemInfo As udsItemConfig = CType(mcolItemInfo(Item + 1), udsItemConfig)

                'Clean-up just in case the tip is showing
                tmrToolTip.Enabled = False
                lblToolTip.Visible = False

                If Strings.Len(oItemInfo.ResxTag) > 0 Then
                    Dim oTTData As frmMain.udsBBItemData = frmMain.GetIOItemData(oItemInfo.ResxTag, oItemInfo.Rack, oItemInfo.Slot, oItemInfo.Point)
                    Dim sToolTip As String = frmMain.glsRM.GetString("lsTAG_NAME", Culture) & oTTData.TagName & ", "

                    sToolTip = sToolTip & frmMain.glsRM.GetString("lsALIAS", Culture) & oTTData.AliasName

                    'Show the tip
                    lblToolTip.Text = sToolTip
                    'MessageBox.Show("X = " & (Cursor.Position.X - Me.Left).ToString & ", Y = " & (Cursor.Position.Y - (160 + Me.Top)).ToString)
                    lblToolTip.Left = Cursor.Position.X - Me.Left
                    lblToolTip.Top = Cursor.Position.Y - (Me.Top + 30)
                    lblToolTip.Visible = True
                    tmrToolTip.Enabled = True
                End If

        End Select

    End Sub

    Private Sub frmWatch_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description:  The form is being closed. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        frmMain.WatchWindowVisible = False

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description:  The form has just been instantiated. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim Culture As New System.Globalization.CultureInfo(msCulture)

        Try
            Me.Icon = CType(gpsRM.GetObject("WWIcon", Culture), Icon)
            Me.Text = gpsRM.GetString("psWW_CAPTION", Culture)
            frmMain.WatchWindowVisible = True
            BBWatch.ItemFont = frmMain.BBFont
            BBWatch.ItemOffColor = Color.Tomato
        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: New, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: New, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub tmrToolTip_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrToolTip.Tick
        '********************************************************************************************
        'Description:  The tip has been showing long enough. Hide it.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrToolTip.Enabled = False
        lblToolTip.Visible = False

    End Sub

#End Region

End Class