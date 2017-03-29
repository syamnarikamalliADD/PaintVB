' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmDevices
'
' Description: Show condition of devices for manual screens
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
'    Date       By      Reason                                                          Version
'    05/27/09   MSW     subLocatePanels - clean up the results, also call from the      4.0
'                       form resize routine
'    11/25/09   RJO     Sub Show - Created local arm collection of only Painter Arms
'                       because this form can't count on frmMain in the parent app to
'                       have an arm collection composed painter arms only.
'    11/10/10   MSW     give the form the ability to mask out items for multizone screens
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/24/12   MSW     Account for partial robot collections                         4.01.03.00
'    12/13/12   MSW     Add UseRobotNumber property for easier customizing            4.01.03.01
'    08/30/13   MSW     Add RobotDataStart property so the robots don't have to start 4.01.05.00
'                       on the next word after the booth data - for sealer
'    01/06/14   MSW     Disable the form controlbox                                   4.01.06.00
'*********************************************************************************************************

Friend Class frmDevices

#Region " Declares "

    Friend Const msAUX_ASSEMBLY_COMMON As String = frmMain.msSCREEN_NAME & ".Devices"

    'Moved indexex to declares
    Private mnRobots As Integer = 1
    Private mnPLCDataStart As Integer = 0
    Private mnRobotDataStart As Integer = -1
    Private mbUseRobotNumber As Boolean = True
    Private mnZoneItems As Integer = 1
    Private mnRobotItems As Integer = 1
    Private mbHideZoneItems() As Boolean
    Private mbHideRobotItems() As Boolean
    Private colArms As clsArms = Nothing
#End Region

#Region " Properties "

    Friend Property Robots() As Integer
        '********************************************************************************************
        'Description:  How many panels for arms
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnRobots
        End Get
        Set(ByVal value As Integer)
            mnRobots = value
        End Set
    End Property
    Friend Property PLCDataStart() As Integer
        '********************************************************************************************
        'Description:  Where does the BB data start in the hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnPLCDataStart '0 or ePLCMANUAL_ZONE_LINK.BBRobotWord
        End Get
        Set(ByVal value As Integer)
            mnPLCDataStart = value
        End Set
    End Property
    Friend Property RobotDataStart() As Integer
        '********************************************************************************************
        'Description:  Where does the BB data start in the hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnRobotDataStart '0 or ePLCMANUAL_ZONE_LINK.BBRobotWord
        End Get
        Set(ByVal value As Integer)
            mnRobotDataStart = value
        End Set
    End Property
    Friend Property UseRobotNumber() As Boolean
        '********************************************************************************************
        'Description:  Where does the BB data start in the hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseRobotNumber '0 or ePLCMANUAL_ZONE_LINK.BBRobotWord
        End Get
        Set(ByVal value As Boolean)
            mbUseRobotNumber = value
        End Set
    End Property
    Friend Property RobotItems() As Integer
        '********************************************************************************************
        'Description:  Where does the BB data start in the hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnRobotItems
        End Get
        Set(ByVal value As Integer)
            mnRobotItems = value
        End Set
    End Property
    Friend Property ZoneItems() As Integer
        '********************************************************************************************
        'Description:  Where does the BB data start in the hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnZoneItems
        End Get
        Set(ByVal value As Integer)
            mnZoneItems = value
        End Set
    End Property
    Friend Property HideZoneItems() As Boolean()
        '********************************************************************************************
        'Description:  Hide zone items for a specific zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbHideZoneItems
        End Get
        Set(ByVal value As Boolean())
            ReDim mbHideZoneItems(value.GetUpperBound(0))
            Array.Copy(value, mbHideZoneItems, value.GetUpperBound(0) + 1)
        End Set
    End Property
    Friend Property HideRobotItems() As Boolean()
        '********************************************************************************************
        'Description:  Hide robot items for a specific zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbHideRobotItems
        End Get
        Set(ByVal value As Boolean())
            ReDim mbHideRobotItems(value.GetUpperBound(0))
            Array.Copy(value, mbHideRobotItems, value.GetUpperBound(0) + 1)
        End Set
    End Property
#End Region

#Region " Routines "

    Private Sub subLocatePanels()
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/27/09  MSW     check for having more space than required, if so center the robot panel.
        '********************************************************************************************
        Const Border As Integer = 25
        Dim pt As Point
        Dim nWidth As Integer = Me.Width - (Border * 2)
        Dim nHeight As Integer = Me.Height - (Border * 2) - tlsMain.Height

        Dim nWidth2 As Integer = bbZone.Width \ 2
        pt.Y = Border
        pt.X = Border + (nWidth \ 2 - nWidth2)
        bbZone.Location = pt

        'locate flow layout panel
        Dim nTop As Integer = bbZone.Top
        nTop = bbZone.Height + nTop + Border
        pt.Y = nTop

        Dim szPref As New Size
        szPref = flpMain.PreferredSize
        szPref.Width = szPref.Width + flpMain.Margin.Horizontal
        szPref.Height = szPref.Height + flpMain.Margin.Vertical
        Dim szAvailable As New Size(nWidth, (nHeight - pt.Y))
        If (szPref.Height > szAvailable.Height) Or (szPref.Width > szAvailable.Width) Then
            If (((szPref.Width \ 2) + Border) < szAvailable.Width) Then
                szAvailable.Width = (szPref.Width \ 2) + Border
                pt.X = Border + (nWidth - szAvailable.Width) \ 2
            Else
                pt.X = Border
            End If
            flpMain.AutoScroll = True
            flpMain.Size = szAvailable
        Else
            flpMain.AutoScroll = False
            flpMain.Size = szPref
            pt.X = Border + (nWidth - szPref.Width) \ 2
        End If
            flpMain.Location = pt

    End Sub
    Friend Overloads Sub Show(ByVal sPLCData As String(), ByRef oColarms As clsArms)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/24/12  MSW     Account for partial robot collections
        '********************************************************************************************
        Dim oBBoard As BingoBoard.BingoBoard
        colArms = oColarms
        btnStatus.Visible = False

        'set up bingo boards
        Dim sName As String
        Dim sVal(0) As String

        sVal(0) = "0"

        Dim sData(mnZoneItems - 1) As String

        With bbZone

            .Columns = 1
            .AutosizeColumns = True
            .ItemOffColor = Color.Tomato
            .ItemCount = mnZoneItems
            .TitleFont = New Font("Arial", 10, FontStyle.Bold)
            .TitleText = gasRM.GetString("dsZoneTitle")
            .ItemFont = New Font("Arial", 10, FontStyle.Bold)

            For i As Integer = 0 To mnZoneItems - 1
                sData(i) = i.ToString
            Next
            .ItemBitIndex = sData

            .BorderStyle = BorderStyle.Fixed3D

            sData = Nothing
            ReDim sData(mnZoneItems)

            For i As Integer = 1 To mnZoneItems - 1 'JZ 12122016 - No intrusion in ADAC.
                If mbHideZoneItems Is Nothing OrElse (mbHideZoneItems.GetUpperBound(0) < i) _
                        OrElse (mbHideZoneItems(i) = False) Then
                    sName = "dsZoneOn" & Format(i + 1, "00")
                    sData(i) = gasRM.GetString(sName)
                Else
                    sData(i) = String.Empty
                End If
            Next
            .ItemOnText = sData

            sData = Nothing
            ReDim sData(mnZoneItems - 1)

            For i As Integer = 1 To mnZoneItems - 1 'JZ 12122016 - No intrusion in ADAC.
                If mbHideZoneItems Is Nothing OrElse (mbHideZoneItems.GetUpperBound(0) < i) _
                        OrElse (mbHideZoneItems(i) = False) Then
                    sName = "dsZoneOff" & Format(i + 1, "00")
                    sData(i) = gasRM.GetString(sName)
                Else
                    sData(i) = String.Empty
                End If
            Next
            .ItemOffText = sData

            .ItemData = sVal

        End With

        'Dim colDevZones As New clsZones(String.Empty) 'RJO 11/25/09
        'Dim colDevControllers As New clsControllers(colDevZones, False) 'RJO 11/25/09
        'Dim colDevArms As clsArms = LoadArmCollection(colDevControllers, False) 'RJO 11/25/09

        For nBB As Integer = 1 To mnRobots

            'If nBB > frmMain.colArms.Count Then Exit For
            If nBB > colArms.Count Then Exit For 'RJO 11/25/09

            sName = "bbR" & Format(nBB, "00")

            oBBoard = DirectCast(flpMain.Controls(sName), BingoBoard.BingoBoard)
            If oBBoard Is Nothing Then
                oBBoard = New BingoBoard.BingoBoard
                oBBoard.Name = sName
                oBBoard.BorderStyle = BorderStyle.Fixed3D
                flpMain.Controls.Add(oBBoard)
            End If

            sData = Nothing
            ReDim sData(mnRobotItems - 1)

            With oBBoard
                .ItemCount = mnRobotItems
                .Columns = 1
                .AutosizeColumns = True
                '.TitleText = frmMain.colArms(nBB - 1).Name
                .TitleText = colArms(nBB - 1).Name 'RJO 11/25/09

                For i As Integer = 0 To mnRobotItems - 1
                    sData(i) = i.ToString
                Next
                .ItemBitIndex = sData

                sData = Nothing
                ReDim sData(mnRobotItems - 1)

                For i As Integer = 0 To mnRobotItems - 1
                    If mbHideRobotItems Is Nothing OrElse (mbHideRobotItems.GetUpperBound(0) < i) _
                            OrElse (mbHideRobotItems(i) = False) Then
                        sName = "dsRobotOn" & Format(i + 1, "00")
                        sData(i) = gasRM.GetString(sName)
                    Else
                        sData(i) = String.Empty
                    End If
                Next
                .ItemOnText = sData

                sData = Nothing
                ReDim sData(mnRobotItems - 1)

                For i As Integer = 0 To mnRobotItems - 1
                    If mbHideRobotItems Is Nothing OrElse (mbHideRobotItems.GetUpperBound(0) < i) _
                            OrElse (mbHideRobotItems(i) = False) Then
                        sName = "dsRobotOff" & Format(i + 1, "00")
                        sData(i) = gasRM.GetString(sName)
                    Else
                        sData(i) = String.Empty
                    End If
                Next
                .ItemOffText = sData

                .ItemFont = New Font("Arial", 10, FontStyle.Bold)
                .TitleFont = New Font("Arial", 10, FontStyle.Bold)
                .ItemOffColor = Color.Tomato

                .ItemData = sVal

                .Visible = True
            End With

        Next

        Me.Text = gasRM.GetString("dsDeviceScreen")

        UpdateHotlinkData(sPLCData)

        subLocatePanels()

        Me.ShowDialog()

    End Sub
    Friend Sub UpdateHotlinkData(ByVal vData As String())
        '********************************************************************************************
        'Description:  update labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/24/12  MSW     Account for partial robot collections
        '********************************************************************************************
        Dim sWord(0) As String
        Dim nOffset As Integer ' = 0
        Dim sName As String ' = String.Empty
        Dim nBox As Integer = 1

        If vData Is Nothing Then Exit Sub

        Dim nUB As Integer = UBound(vData)

        Try

            'Geo4
            sWord(0) = vData(mnPLCDataStart)
            If sWord(0) = "-" Then
                sWord(0) = "0"
            End If
            bbZone.ItemData = sWord

            For i As Integer = 1 To Me.Robots
                If mnRobotDataStart = -1 Then
                    If mbUseRobotNumber Then
                        nOffset = colArms(i - 1).RobotNumber + mnPLCDataStart
                    Else
                        nOffset = i + mnPLCDataStart
                    End If
                Else
                    If mbUseRobotNumber Then
                        nOffset = colArms(i - 1).RobotNumber + mnRobotDataStart - 1
                    Else
                        nOffset = i + mnRobotDataStart - 1
                    End If
                End If

                sWord(0) = vData(nOffset)
                If sWord(0) = "-" Then
                    sWord(0) = "0"
                End If
                sName = "bbR" & Format(nBox, "00")
                Dim o As BingoBoard.BingoBoard = _
                                DirectCast(flpMain.Controls.Item(sName), BingoBoard.BingoBoard)
                o.ItemData = sWord
                nBox += 1
                If nBox > flpMain.Controls.Count Then Exit For
            Next

        Catch ex As Exception

        End Try



    End Sub

#End Region

#Region " Events "

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Hide()
    End Sub
    Public Sub New()
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'for language support
        mLanguage.GetAuxResourceManager(msAUX_ASSEMBLY_COMMON)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mScreenSetup.InitializeForm(Me)

    End Sub

#End Region

    Private Sub frmDevices_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        '********************************************************************************************
        'Description:  form resize
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/27/09  MSW     Add the routine, call subLocatePanels
        '********************************************************************************************
        subLocatePanels()
    End Sub

End Class