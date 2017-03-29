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
' Form/Module: frmAxisJog
'
' Description: Axis jog
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
'    3/31/10    MSW     Support ascii color and style
'    9/14/12    RJO     Bug fix to subSetRepairPanelNames
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms
Imports Response = System.Windows.Forms.DialogResult


Friend Class frmEdit


#Region " Declares "
    Private mbGhost As Boolean = False
    Private mbEnableSim As Boolean = True
    Private mbEnableVin As Boolean = True
    Private mbEnableCarrier As Boolean = True
    Private mnVinDigits As Integer = 8
    Private mnCarrierDigits As Integer = 8
    Private mbCarrierNumeric As Boolean = True
    Private mbVinNumeric As Boolean = True
    Private msCarrier As String = String.Empty
    Private msVin As String = String.Empty
    Private mnQueuePos As Integer = 0
    Private mbIgnoreEdits As Boolean = False
    Private mbAsciiColor As Boolean = False
    Private mbAsciiStyle As Boolean = False
    Private mbCarrierChanged As Boolean = False
    Private mbEdit As Boolean = True
    Private mbEnableRepair As Boolean = False
#End Region
#Region " Properties "
    Property ViewOnly() As Boolean
        Get
            Return Not (mbEdit)
        End Get
        Set(ByVal value As Boolean)
            mbEdit = Not (value)
            btnAccept.Visible = mbEdit
            If mbEdit Then
                btnCancel.Text = gcsRM.GetString("csCANCEL")
            Else
                btnCancel.Text = gcsRM.GetString("csCLOSE")
            End If
            txtCarrier.Enabled = mbEdit
            txtVin.Enabled = mbEdit
            cboStyle.Enabled = mbEdit
            cboOption.Enabled = mbEdit
            cboTutone.Enabled = mbEdit
            cboColor.Enabled = mbEdit
            cboJobStatus.Enabled = mbEdit
            tlpRepair.Enabled = mbEdit
        End Set
    End Property
    Friend Property QueuePos() As Integer
        Get
            Return mnQueuePos
        End Get
        Set(ByVal value As Integer)
            mnQueuePos = value
            If Not (mbGhost) Then
                If value > 0 Then
                    lblBanner.Text = gpsRM.GetString("psEDIT_PRE_QUEUE_POS") & mnQueuePos.ToString
                Else
                    lblBanner.Text = gpsRM.GetString("psEDIT_POS_QUEUE_POS") & (-1 * mnQueuePos).ToString
                End If
            End If

        End Set
    End Property
    Friend Property Ghost() As Boolean
        '********************************************************************************************
        'Description: Configure for ghost or queue edit
        '
        'Parameters: true = ghost edit
        'Returns:    true for ghost edit
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbGhost
        End Get
        Set(ByVal value As Boolean)
            mbGhost = value
            dgSimSpeeds.Visible = value
            picGhost1.Visible = value
            picGhost2.Visible = value
            mbEnableSim = True
            chkSimConveyor.Visible = value And mbEnableSim
            dgSimSpeeds.Visible = value And mbEnableSim
            lblVin.Visible = Not (value) And mbEnableVin
            txtVin.Visible = Not (value) And mbEnableVin
            lblCarrier.Visible = Not (value) And mbEnableCarrier
            txtCarrier.Visible = Not (value) And mbEnableCarrier
            lblJobStatus.Visible = Not (value)
            cboJobStatus.Visible = Not (value)
            If value Then
                lblBanner.Text = gpsRM.GetString("psENTER_GHOST_JOB")
                If mbEnableSim Then
                    subUpdateSimSpeedGrid()
                End If
            Else
                If mnQueuePos > 0 Then
                    lblBanner.Text = gpsRM.GetString("psEDIT_PRE_QUEUE_POS") & mnQueuePos.ToString
                Else
                    lblBanner.Text = gpsRM.GetString("psEDIT_POS_QUEUE_POS") & (-1 * mnQueuePos).ToString
                End If
            End If

        End Set
    End Property
    Friend Property Repair() As Integer
        '********************************************************************************************
        'Description: Repair panel checkboxes
        '
        'Parameters: Repair panel word as integer
        'Returns:    Repair panel word as integer
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Dim nRepair As Integer = 0
            Dim nBitMask As Integer = 0
            For Each chkTmp As CheckBox In tlpRepair.Controls
                If chkTmp.Checked Then
                    nBitMask = CInt(2 ^ CInt(chkTmp.Name.Substring(9)))
                    nRepair += nBitMask
                End If
            Next
            Return nRepair
        End Get
        Set(ByVal value As Integer)
            Dim nBitMask As Integer = 0
            For Each chkTmp As CheckBox In tlpRepair.Controls
                nBitMask = CInt(2 ^ CInt(chkTmp.Name.Substring(9)))
                If (nBitMask And value) = nBitMask Then
                    chkTmp.Checked = True
                Else
                    chkTmp.Checked = False
                End If

            Next
        End Set
    End Property
    Friend Property EnableSimConveyor() As Boolean
        '********************************************************************************************
        'Description: Enable the sim conveyor check box
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnableSim
        End Get
        Set(ByVal value As Boolean)
            mbEnableSim = value
            chkSimConveyor.Visible = mbEnableSim And mbGhost
            dgSimSpeeds.Visible = mbEnableSim And mbGhost
            If mbEnableSim And mbGhost Then
                subUpdateSimSpeedGrid()
            End If
        End Set
    End Property
    Friend Property EnableRepair() As Boolean
        '********************************************************************************************
        'Description: Enable the repair panel check boxes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnableRepair
        End Get
        Set(ByVal value As Boolean)
            mbEnableRepair = value
            gpbRepairPanels.Visible = mbEnableRepair
            tlpRepair.Visible = mbEnableRepair
            If value Then
                subSetRepairPanelNames()
            End If
            For Each chkTmp As CheckBox In tlpRepair.Controls
                chkTmp.Visible = value
            Next
        End Set
    End Property
    Friend Property SimConveyor() As Boolean
        '********************************************************************************************
        'Description: Conveyor sim checkbox
        '
        'Parameters: true = set sim conveyor checked
        'Returns:    true if sim conveyor is checked
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return chkSimConveyor.Checked
        End Get
        Set(ByVal value As Boolean)
            chkSimConveyor.Checked = value
        End Set
    End Property
    Friend Property EnableOption() As Boolean
        '********************************************************************************************
        'Description: Enable the option cbo
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblOption.Visible
        End Get
        Set(ByVal value As Boolean)
            cboOption.Visible = value
            lblOption.Visible = value
            If value Then
                tlpCbos.ColumnStyles.Item(3).SizeType = SizeType.Percent
                tlpCbos.ColumnStyles.Item(3).Width = CSng(tlpCbos.ColumnStyles.Item(2).Width / 1.5)
            Else
                tlpCbos.ColumnStyles.Item(3).SizeType = SizeType.Absolute
                tlpCbos.ColumnStyles.Item(3).Width = 0
            End If
        End Set
    End Property
    Friend Property EnableTutone() As Boolean
        '********************************************************************************************
        'Description: Enable the option cbo
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblTutone.Visible
        End Get
        Set(ByVal value As Boolean)
            lblTutone.Visible = value
            cboTutone.Visible = value
            If value Then
                tlpCbos.ColumnStyles.Item(4).SizeType = SizeType.Percent
                tlpCbos.ColumnStyles.Item(4).Width = tlpCbos.ColumnStyles.Item(2).Width
            Else
                tlpCbos.ColumnStyles.Item(4).SizeType = SizeType.Absolute
                tlpCbos.ColumnStyles.Item(4).Width = 0
            End If
        End Set
    End Property
    Friend Property EnableVin() As Boolean
        '********************************************************************************************
        'Description: Enable the Vin textbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnableVin
        End Get
        Set(ByVal value As Boolean)
            mbEnableVin = value
            lblVin.Visible = value
            txtVin.Visible = value
            If value Then
                tlpCbos.ColumnStyles.Item(0).SizeType = SizeType.Percent
                tlpCbos.ColumnStyles.Item(0).Width = tlpCbos.ColumnStyles.Item(2).Width
            Else
                tlpCbos.ColumnStyles.Item(0).SizeType = SizeType.Absolute
                tlpCbos.ColumnStyles.Item(0).Width = 0
            End If
        End Set
    End Property
    Friend Property EnableCarrier() As Boolean
        '********************************************************************************************
        'Description: Enable the Carrier textbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEnableCarrier
        End Get
        Set(ByVal value As Boolean)
            mbEnableCarrier = value
            lblCarrier.Visible = value
            txtCarrier.Visible = value
            If value Then
                tlpCbos.ColumnStyles.Item(1).SizeType = SizeType.Percent
                tlpCbos.ColumnStyles.Item(1).Width = tlpCbos.ColumnStyles.Item(2).Width / 2
            Else
                tlpCbos.ColumnStyles.Item(1).SizeType = SizeType.Absolute
                tlpCbos.ColumnStyles.Item(1).Width = 0
            End If
        End Set
    End Property
    Friend Property Vin() As String
        '********************************************************************************************
        'Description: Vin edit value
        '
        'Parameters: Vin edit value
        'Returns:    Vin edit value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            subValidateTextBox(txtVin)
            Return txtVin.Text
        End Get
        Set(ByVal value As String)
            txtVin.Text = value
        End Set
    End Property
    Friend Property Carrier() As String
        '********************************************************************************************
        'Description: Carrier edit value
        '
        'Parameters: Carrier edit value
        'Returns:    Carrier edit value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            subValidateTextBox(txtCarrier)
            Return txtCarrier.Text
        End Get
        Set(ByVal value As String)
            txtCarrier.Text = value
        End Set
    End Property
    Friend Property VinForceNumeric() As Boolean
        '********************************************************************************************
        'Description: True = force numeric vin value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbVinNumeric
        End Get
        Set(ByVal value As Boolean)
            mbVinNumeric = value
        End Set
    End Property
    Friend Property CarrierForceNumeric() As Boolean
        '********************************************************************************************
        'Description: True = force numeric Carrier value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbCarrierNumeric
        End Get
        Set(ByVal value As Boolean)
            mbCarrierNumeric = value
        End Set
    End Property
    Friend Property VinDigits() As Integer
        '********************************************************************************************
        'Description: Number of vin digits or characters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnVinDigits
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            mnVinDigits = value
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property CarrierDigits() As Integer
        '********************************************************************************************
        'Description: Number of carrier digits or characters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnCarrierDigits
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            mnCarrierDigits = value
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property AsciiStyleEnable() As Boolean
        '********************************************************************************************
        'Description: ascii color setting
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            Return mbAsciiStyle
        End Get
        Set(ByVal value As Boolean)
            mbAsciiStyle = value
        End Set
    End Property
    Friend Property AsciiStyle() As String
        '********************************************************************************************
        'Description: ascii color value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            Dim sTmp As String() = DirectCast(cboStyle.Tag, String())
            If cboStyle.SelectedIndex >= 0 Then
                Return sTmp(cboStyle.SelectedIndex)
            Else
                Return "0"
            End If
        End Get
        Set(ByVal value As String)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboStyle.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboStyle.Items.Count - 1
                If (sTmp(nVal) = value) Then
                    cboStyle.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboStyle.SelectedIndex = -1
                cboStyle.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property StyleNo() As Integer
        '********************************************************************************************
        'Description: Style No - index to the CBO (starting with 0)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            If mbAsciiStyle Then
                Return -1
            End If
            Dim sTmp As String() = DirectCast(cboStyle.Tag, String())
            If cboStyle.SelectedIndex >= 0 Then
                Return CInt(sTmp(cboStyle.SelectedIndex))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboStyle.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboStyle.Items.Count - 1
                If CInt(sTmp(nVal)) = value Then
                    cboStyle.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboStyle.SelectedIndex = -1
                cboStyle.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property OptionNo() As Integer
        '********************************************************************************************
        'Description: Option No - index to the CBO (starting with 0)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim sTmp As String() = DirectCast(cboOption.Tag, String())
            If cboOption.SelectedIndex >= 0 Then
                Return CInt(sTmp(cboOption.SelectedIndex))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboOption.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboOption.Items.Count - 1
                If CInt(sTmp(nVal)) = value Then
                    cboOption.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboOption.SelectedIndex = -1
                cboOption.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property TutoneNo() As Integer
        '********************************************************************************************
        'Description: Tutone No - index to the CBO (starting with 0)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim sTmp As String() = DirectCast(cboTutone.Tag, String())
            If cboTutone.SelectedIndex >= 0 Then
                Return CInt(sTmp(cboTutone.SelectedIndex))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboTutone.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboTutone.Items.Count - 1
                If CInt(sTmp(nVal)) = value Then
                    cboTutone.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboTutone.SelectedIndex = -1
                cboTutone.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property AsciiColorEnable() As Boolean
        '********************************************************************************************
        'Description: ascii color setting
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            Return mbAsciiColor
        End Get
        Set(ByVal value As Boolean)
            mbAsciiColor = value
        End Set
    End Property
    Friend Property AsciiColor() As String
        '********************************************************************************************
        'Description: ascii color value
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            Dim sTmp As String() = DirectCast(cboColor.Tag, String())
            If cboColor.SelectedIndex >= 0 Then
                Return sTmp(cboColor.SelectedIndex)
            Else
                Return "0"
            End If
        End Get
        Set(ByVal value As String)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboColor.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboColor.Items.Count - 1
                If (sTmp(nVal) = value) Then
                    cboColor.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboColor.SelectedIndex = -1
                cboColor.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property ColorNo() As Integer
        '********************************************************************************************
        'Description: Color No - index to the CBO (starting with 0)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Get
            If mbAsciiColor Then
                Return -1
            End If
            Dim sTmp As String() = DirectCast(cboColor.Tag, String())
            If cboColor.SelectedIndex >= 0 Then
                Return CInt(sTmp(cboColor.SelectedIndex))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboColor.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboColor.Items.Count - 1
                If CInt(sTmp(nVal)) = value Then
                    cboColor.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboColor.SelectedIndex = -1
                cboColor.Text = String.Empty
            End If
            mbIgnoreEdits = False
        End Set
    End Property
    Friend Property JobStatus() As Integer
        '********************************************************************************************
        'Description: Job Status - index to the CBO (starting with 0)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim sTmp As String() = DirectCast(cboJobStatus.Tag, String())
            If cboJobStatus.SelectedIndex >= 0 Then
                Return CInt(sTmp(cboJobStatus.SelectedIndex))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            mbIgnoreEdits = True
            Dim sTmp As String() = DirectCast(cboJobStatus.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To cboJobStatus.Items.Count - 1
                If CInt(sTmp(nVal)) = value Then
                    cboJobStatus.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                cboJobStatus.SelectedIndex = -1
                cboJobStatus.Text = String.Empty
            End If
            mbCarrierChanged = False
            mbIgnoreEdits = False
        End Set
    End Property
#End Region
#Region " Routines"
    Private Sub subUpdateSimSpeedGrid()
        '********************************************************************************************
        'Description: Update grid with sim speeds
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Sim speed grid - Sim Speeds are by controller
        Dim nControllers As Integer = frmMain.colControllers.Count

        With dgSimSpeeds
            .RowCount = nControllers
            .ColumnCount = 1
            .Columns.Item(0).HeaderText = gpsRM.GetString("psCONV_SIM_SPEED")
            For nController As Integer = 1 To nControllers
                .Rows.Item(nController - 1).HeaderCell.Value = frmMain.colControllers.Item(nController - 1).Name

                For Each oArm As clsArm In frmMain.colArms
                    If oArm.Controller.ControllerNumber = nController Then
                        oArm.ProgramName = "pavrlntr"
                        oArm.VariableName = "sim_conv_spd"
                        .Rows.Item(nController - 1).Cells(0).Value = oArm.VarValue
                        Exit For
                    End If
                Next 'oArm

            Next 'nController

            .Columns.Item(0).Width = dgSimSpeeds.Width \ 2
            .RowHeadersWidth = dgSimSpeeds.Width \ 2

        End With 'dgSimSpeeds

    End Sub
    Friend Sub subInitFormText()
        '********************************************************************************************
        'Description: Init Common text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnAccept.Text = gpsRM.GetString("psACCEPT")
        If mbEdit Then
            btnCancel.Text = gcsRM.GetString("csCANCEL")
        Else
            btnCancel.Text = gcsRM.GetString("csCLOSE")
        End If
        chkSimConveyor.Text = gpsRM.GetString("psSIM_CONV")
        lblVin.Text = gpsRM.GetString("psVIN")
        lblCarrier.Text = gpsRM.GetString("psCARRIER")
        lblStyle.Text = gpsRM.GetString("psSTYLE")
        lblOption.Text = gpsRM.GetString("psOPTION")
        lblTutone.Text = gpsRM.GetString("psTUTONE")
        lblColor.Text = gcsRM.GetString("csCOLOR")
        lblJobStatus.Text = gpsRM.GetString("psJOB_STATUS")
        gpbRepairPanels.Text = gpsRM.GetString("psREPAIR_PANELS")
        For Each chkTmp As CheckBox In tlpRepair.Controls
            'It may have already been set
            If chkTmp.Text = chkTmp.Name Then
                chkTmp.Text = gpsRM.GetString("psREPAIR_PANEL") & chkTmp.Name.Substring(9)
            End If
        Next
    End Sub

    Friend Sub subInitCbos(Optional ByVal bZDT As Boolean = False)
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Change to only load enables styles, colors and options
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************

        cboColor.Items.Clear()
        cboColor.Text = String.Empty
        LoadColorBoxFromDB(cboColor, colZones, False, True, mbAsciiColor)
        cboStyle.Items.Clear()
        cboStyle.Text = String.Empty
        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
        If bZDT Then
            Dim nStyle As Integer = 0
            Do While nStyle < oStyles.Count
                If oStyles.Item(nStyle).FanucNumber.Value < gnZdtStartingJobNumber Then
                    oStyles.RemoveAt(nStyle)
                Else
                    nStyle += 1
                End If
            Loop
        End If
        mbAsciiStyle = oStyles.UseAscii
        oStyles.LoadStyleBoxFromCollection(cboStyle, True, , colZones.ActiveZone.DegradeStyleRbtsReq, colZones.ActiveZone.NumberOfDegrades)
        cboOption.Items.Clear()
        cboOption.Text = String.Empty
        Dim oOptions As New clsSysOptions(colZones.ActiveZone)
        oOptions.LoadOptionBoxFromCollection(cboOption, True)
        cboJobStatus.Items.Clear()
        cboJobStatus.Items.Add(gpsRM.GetString("psPAINT"))
        cboJobStatus.Items.Add(gpsRM.GetString("psNOPAINT"))
        'cboJobStatus.Items.Add(gpsRM.GetString("psEMPTYCARRIER"))
        cboJobStatus.Items.Add(gpsRM.GetString("psNOCARRIER"))
        'Dim sTag(3) As String
        Dim sTag(2) As String
        sTag(0) = CInt(eQueue.Stat_Paint).ToString
        sTag(1) = CInt(eQueue.Stat_NoPaint).ToString
        'sTag(2) = CInt(eQueue.Stat_EmptyCarrier).ToString
        'sTag(3) = CInt(eQueue.Stat_PosEmpty).ToString
        sTag(2) = CInt(eQueue.Stat_PosEmpty).ToString
        cboJobStatus.Tag = sTag

        'Dim oRepair As New clsSysRepair(colZones.ActiveZone)
        'oRepair.LoadOptionBoxFromCollection(cboStyle)
    End Sub
    Friend Shadows Function Show() As Boolean
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
        btnAccept.Enabled = False
        Return MyBase.ShowDialog() = Windows.Forms.DialogResult.OK
    End Function
    Private Function subValidateTextBox(ByRef txtTmp As TextBox) As Boolean
        Dim nMaxDigits As Integer = 0
        Dim bNumeric As Boolean = False
        mbIgnoreEdits = True
        Select Case txtTmp.Name
            Case "txtVin"
                nMaxDigits = mnVinDigits
                bNumeric = mbVinNumeric
            Case "txtCarrier"
                nMaxDigits = mnCarrierDigits
                bNumeric = mbCarrierNumeric
        End Select
        Dim sTest As String = txtTmp.Text
        Dim bNoGood As Boolean = False
        If sTest.Length > nMaxDigits Then
            bNoGood = True
        End If
        If bNumeric Then
            Try
                If (String.IsNullOrEmpty(sTest)) OrElse Not (IsNumeric(sTest)) OrElse (CInt(sTest).ToString <> sTest) Then
                    bNoGood = True
                End If
            Catch ex As Exception
                bNoGood = True
            End Try
        End If
        Select Case txtTmp.Name
            Case "txtVin"
                If bNoGood Then
                    txtTmp.Text = msVin
                Else
                    msVin = sTest
                End If
            Case "txtCarrier"
                If bNoGood Then
                    txtTmp.Text = msCarrier
                Else
                    msCarrier = sTest
                End If
        End Select
        mbIgnoreEdits = False
        'We force a good value, so no need to disable the accept button
        'Return (bNoGood = False)
        Return True
    End Function
#End Region
#Region " Events "

    Private Sub btnAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAccept.Click
        '********************************************************************************************
        'Description: Edits or Ghost accepted
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description: Edits or Ghost canceled
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
    Private Sub txtBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCarrier.TextChanged, txtVin.TextChanged
        '********************************************************************************************
        'Description: Vin or text box changed
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbIgnoreEdits Then Exit Sub
        subValidateAll()
    End Sub


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        mbIgnoreEdits = True
        InitializeComponent()
        mbIgnoreEdits = False
        ' Add any initialization after the InitializeComponent() call.

    End Sub
#End Region


    Private Sub subValidateAll()
        '********************************************************************************************
        'Description: see if it's OK to enable btnAccept
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim bEnable As Boolean = True
            If cboJobStatus.Visible Then
                If cboJobStatus.SelectedIndex < 0 Then
                    bEnable = False
                Else
                    'If they're just removing a job from the queue, ignore the rest
                    Dim sTag() As String = DirectCast(cboJobStatus.Tag, String())
                    If (CInt(sTag(cboJobStatus.SelectedIndex)) = CInt(eQueue.Stat_PosEmpty)) Or _
                       (CInt(sTag(cboJobStatus.SelectedIndex)) = CInt(eQueue.Stat_EmptyCarrier)) Then
                        btnAccept.Enabled = mbCarrierChanged
                        Exit Sub
                    End If
                End If
            End If
            If mbEnableVin Then
                bEnable = bEnable And subValidateTextBox(txtVin)
            End If
            If mbEnableCarrier Then
                bEnable = bEnable And subValidateTextBox(txtCarrier)
            End If
            If cboStyle.Visible Then
                If cboStyle.SelectedIndex < 0 Then
                    bEnable = False
                End If
            End If
            If cboColor.Visible Then
                If cboColor.SelectedIndex < 0 Then
                    bEnable = False
                End If
            End If
            If cboOption.Visible Then
                If cboOption.SelectedIndex < 0 Then
                    bEnable = False
                End If
            End If
            If cboTutone.Visible Then
                If cboTutone.SelectedIndex < 0 Then
                    bEnable = False
                End If
            End If
            btnAccept.Enabled = bEnable
        Catch ex As Exception
            btnAccept.Enabled = False
        End Try
    End Sub
    Private Sub cbo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStyle.SelectedIndexChanged, _
            cboColor.SelectedIndexChanged, cboOption.SelectedIndexChanged, _
             cboJobStatus.SelectedIndexChanged, cboTutone.SelectedIndexChanged
        '********************************************************************************************
        'Description: any cbo changed
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oCbo As ComboBox = DirectCast(sender, ComboBox)
        If oCbo.Name = cboJobStatus.Name Then
            mbCarrierChanged = True 'track for btnAccept.Enable
        End If
        If mbEnableRepair AndAlso oCbo.Name = cboStyle.Name Then
            subSetRepairPanelNames()
        End If
        If mbIgnoreEdits Then Exit Sub
        subValidateAll()
    End Sub
    Private Sub subSetRepairPanelNames()
        '********************************************************************************************
        'Description: set repair panel names
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/14/12  RJO     Original version failed to load descriptions because (1.) RepairPanelDescriptions
        '                   was never loaded and therefore was Nothing and also (2.) Only valid Styles are 
        '                   listed in the combo box so unless ALL styles are valid, there's a good likelyhood 
        '                   that cboStyle.SelectedIndex will not point to the right index in colStyles.
        '********************************************************************************************
        Try
            If cboStyle.SelectedIndex > -1 Then
                If colStyles.Item(cboStyle.SelectedIndex).RepairPanelDescriptions Is Nothing Then 'RJO 09/14/12
                    colStyles.LoadRepairPanelDescriptions()
                End If

                Dim sPlantStyles() As String = DirectCast(cboStyle.Tag, String()) 'RJO 09/14/12
                Dim oRepairPanels() As clsTextValue = colStyles.Item(sPlantStyles(cboStyle.SelectedIndex), False).RepairPanelDescriptions 'RJO 09/14/12
                'Dim oRepairPanels() As clsTextValue = colStyles.Item(cboStyle.SelectedIndex).RepairPanelDescriptions 'RJO 09/14/12

                For nIndex As Integer = 0 To oRepairPanels.GetUpperBound(0)
                    Dim chkTmp As CheckBox = DirectCast(tlpRepair.Controls("chkRepair" & nIndex.ToString), CheckBox)

                    chkTmp.Text = oRepairPanels(nIndex).Text
                Next  'nIndex
            End If

        Catch ex As Exception

        End Try
    End Sub
    Private Sub chkRepair_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRepair0.CheckedChanged, _
            chkRepair1.CheckedChanged, chkRepair2.CheckedChanged, chkRepair3.CheckedChanged, chkRepair4.CheckedChanged, _
            chkRepair5.CheckedChanged, chkRepair6.CheckedChanged, chkRepair7.CheckedChanged, chkRepair8.CheckedChanged, _
            chkRepair9.CheckedChanged, chkRepair10.CheckedChanged, chkRepair11.CheckedChanged, chkRepair12.CheckedChanged, _
            chkRepair13.CheckedChanged, chkRepair14.CheckedChanged, chkRepair15.CheckedChanged
        '********************************************************************************************
        'Description: repair checkboxes changed
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbIgnoreEdits Then Exit Sub
        subValidateAll()
    End Sub
End Class