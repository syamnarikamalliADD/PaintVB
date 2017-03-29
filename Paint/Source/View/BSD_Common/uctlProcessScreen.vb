Friend Class uctlProcessScreen
    Implements BSDForm


#Region " Declares "

    Private msBoothData As String() = Nothing
    Private msRobotData As String() = Nothing
    Private msQueueData As String() = Nothing
    Private msBoothRef As String() = Nothing
    Private msRobotRef As String() = Nothing
    Private msQueueRef As String() = Nothing
    Private mbIsRemote As Boolean = False
    Private mnQueueIndex As Integer = 0
    Private mnRobotIndex As Integer = 0
    Private mnLinkIndex As ePLCLink = ePLCLink.None
    Private msSAData As String() = Nothing
    Private mbCCValve As Boolean() = Nothing
    'Private mCCType As eColorChangeType = eColorChangeType.NONE
    Private meCCType As eColorChangeType
    Private mnStartRbtInx As Integer = 1

    '******** Cartoon Variables   *******************************************************************
    Private colImages As Collection = Nothing
    Private mAccustatToon As clsAccustatCartoon = Nothing
    '******** End Cartoon Variables   ****************************************************************
    Private mApplicator As clsApplicator
    Dim bInCycle() As Boolean
    '******** Cartoon Variables   *******************************************************************
    'CC type class from ColorChange.vb This'll get assigned a type-specific class inherited from clsColorChangeCartoon
    Private WithEvents mCCToon As clsColorChangeCartoon = Nothing
    'This'll get assigned to a cctype specific user-control that has the actual drawing.
    Friend uctrlCartoon As UserControl = Nothing
    '******** Cartoon Variables   *******************************************************************
    Private Const mnTOON_WIDTH As Integer = 316
    Private Const mnTOON_HEIGHT As Integer = 570
    Private mbInInit As Boolean = True
    Private mbCCCycleActive() As Boolean
    Private Const mnListSpacing As Integer = 125
    Private Const mnListOffset As Integer = 3
    Private mnGraphRobot As Integer = 0
    Private mnGraphParm As Integer = 0

    Private Enum eProcLbl
        FirstLabel = 1
        Carrier = 1
        JobName = 2
        PathName = 3
        Style = 4
        OptionNum = 5
        Color = 6
        VIN = 7
        CycleTime = 8
        PresetNum = 9
        PresetFlow = 10
        ActualFlow = 11
        PaintInCan = 11  'Accustat doesn't need actual flow
        FlowTotal = 12
        TPR = 13
        OutPressure = 13  'TPR for accustat, ICS, outpressure for IPC
        PresetAtom = 14
        PresetBS = 14
        ActualBellSpeed = 15
        PresetFan = 16
        PresetSA = 16
        SAManPressure = 17 'For DQ
        EstatPresetNum = 18 'FANUC Estat
        PresetEstat = 19 'FANUC Estat
        ActualKV = 20 'FANUC Estat
        ActualUA = 21 'FANUC Estat
        CCCycle = 22
        CCTime = 23
        LastLabel = 23
    End Enum
#End Region
#Region " Properties "

    Friend ReadOnly Property FormName() As String Implements BSDForm.FormName
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
            Return "Process"
        End Get
    End Property
    Friend Property IsRemoteZone() As Boolean Implements BSDForm.IsRemoteZone
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
            Return mbIsRemote
        End Get
        Set(ByVal value As Boolean)
            mbIsRemote = value
        End Set
    End Property
    Friend Property PLCData() As String() Implements BSDForm.PLCData
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
            Select Case mnLinkIndex
                Case ePLCLink.Zone
                    Return msBoothData
                Case ePLCLink.Robot
                    Return msRobotData
                Case ePLCLink.Queue
                    Return msQueueData
                Case Else
                    Return msBoothData
            End Select
        End Get
        Set(ByVal value As String())
            If value Is Nothing Then Exit Property
            Select Case mnLinkIndex
                Case ePLCLink.Zone
                    msBoothData = value
                Case ePLCLink.Robot
                    msRobotData = value
                Case ePLCLink.Queue
                    msQueueData = value
                Case Else
                    Debug.Assert(False)
            End Select
            subUpdatePLCData()
        End Set
    End Property
    Friend Property RobotIndex() As Integer Implements BSDForm.RobotIndex
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
            Return mnRobotIndex
        End Get
        Set(ByVal value As Integer)
            mnRobotIndex = value
        End Set
    End Property
    Friend Property ScatteredAccessData() As String() Implements BSDForm.ScatteredAccessData
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
            Return msSAData
        End Get
        Set(ByVal value As String())
            msSAData = value
        End Set
    End Property
    Friend Property LinkIndex() As ePLCLink Implements BSDForm.LinkIndex
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
            Return mnLinkIndex
        End Get
        Set(ByVal value As ePLCLink)
            mnLinkIndex = value
        End Set
    End Property
    Property SelectedRobot() As String
        '********************************************************************************************
        'Description: robot selected in cboRobot  
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return cboRobot.Text
        End Get
        Set(ByVal value As String)
            If value = String.Empty Then
                cboRobot.SelectedIndex = 0
            ElseIf value = gcsRM.GetString("csNONE") Then
                cboRobot.SelectedIndex = 0
            Else
                cboRobot.Text = value
            End If
            'cboRobot will call subNewRobotSelected() when it gets changed above
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub Initialize(Optional ByVal sParam As String = "") Implements BSDForm.Initialize
        '********************************************************************************************
        'Description:  Setup screen - putting text in the resource file for captions makes the 
        '               labels visible
        '
        'Parameters:  
        'Returns:      sParam - for this screen it'll be the robot select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbInInit = True
        mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, frmMain.colArms, False, , , True)
        ReDim mbCCCycleActive(frmMain.colArms.Count)
        Dim nUB As Integer = cboRobot.Items.Count
        For Each oCntl As Control In pnlProcess.Controls
            If oCntl.Name <> "uctlR1" Then
                pnlProcess.Controls.Remove(oCntl)
                oCntl.Dispose()
            End If
        Next
        For Each oCntl As Control In pnlRobotNames.Controls
            If oCntl.Name <> "lblR1" Then
                pnlRobotNames.Controls.Remove(oCntl)
                oCntl.Dispose()
            End If
        Next

        For Each oArm As clsArm In frmMain.colArms
            'See if there's a control there for this robot
            Dim oUctlList As uctlList = DirectCast(pnlProcess.Controls("uctlR" & oArm.RobotNumber.ToString), uctlList)
            If oUctlList Is Nothing Then
                'Not there yet, make a new one.
                oUctlList = New uctlList
                'Position   
                oUctlList.Name = "uctlR" & oArm.RobotNumber.ToString    'Name for access later
                Controls.Add(oUctlList)
                pnlProcess.Controls.Add(oUctlList)  'Add it to the panel's control collection

            End If
            oUctlList.LabelTextAlign = ContentAlignment.MiddleCenter
            oUctlList.Width = 120
            oUctlList.LabelHeight = uctlR1.LabelHeight
            oUctlList.LabelSpacing = uctlR1.LabelSpacing
            oUctlList.Top = uctlR1.Top
            oUctlList.Left = (mnListSpacing) * (frmMain.colArms.IndexOf(oArm)) + mnListOffset
            oUctlList.NumLabels = eProcLbl.LastLabel '  How many items to display, constant in this form
            oUctlList.ContextMenuStrip = mnuProcessPopUp
            AddHandler oUctlList.Mouse_Down, AddressOf UctlRx_MouseDown
            'See if there's a label there for this robot
            Dim oLabel As Label = DirectCast(pnlRobotNames.Controls("lblR" & oArm.RobotNumber.ToString), Label)
            If oLabel Is Nothing Then
                'Not there yet, make a new one.
                oLabel = New Label
                'Position   
                oLabel.Name = "lblR" & oArm.RobotNumber.ToString    'Name for access later
                Controls.Add(oLabel)
                pnlRobotNames.Controls.Add(oLabel)  'Add it to the panel's control collection
            End If
            oLabel.Text = oArm.Name          'Name at the top of the labels
            oLabel.Font = lblR1.Font
            oLabel.TextAlign = lblR1.TextAlign
            oLabel.Height = lblR1.Height
            oLabel.Width = oUctlList.Width
            oLabel.Left = oUctlList.Left
            oLabel.Tag = oUctlList.Left
            oLabel.ContextMenuStrip = mnuProcessPopUp
            AddHandler oLabel.MouseDown, AddressOf lblRx_MouseDown
            'Turn on scattered access for each robot
            frmMain.ScatteredAccessEnableArm(frmMain.colArms.IndexOf(oArm)) = True
        Next

        Dim sText As String
        Dim bTmp As Boolean

        lblViewRobotCap.Text = gpsRM.GetString("psROB_BOX_PROC_CAP")

        ReDim bInCycle(nUB)
        For j As Integer = 0 To nUB
            bInCycle(j) = False
        Next
        uctlLabels.NumLabels = eProcLbl.LastLabel   '  How many items to display, constant in this form
        For nLabel As Integer = 1 To eProcLbl.LastLabel
            sText = "psPROC_CAP" & Format(nLabel, "00")
            uctlLabels.LabelText(nLabel) = gpsRM.GetString(sText)
            bTmp = (uctlLabels.LabelText(nLabel) <> String.Empty)
            uctlLabels.LabelVisible(nLabel) = bTmp
            For Each oUctlList As uctlList In pnlProcess.Controls
                oUctlList.LabelVisible(nLabel) = bTmp
            Next
        Next
        'Init Pop-up menus
        mnuDetailView.Text = gpsRM.GetString("psDetailView")
        mnuGraphParm1.Text = gpsRM.GetString("psGraphParm1")
        mnuGraphParm2.Text = gpsRM.GetString("psGraphParm2")
        mnuGraphParm3.Text = gpsRM.GetString("psGraphParm3")
        mnuGraphParm4.Text = gpsRM.GetString("psGraphParm4")
        mnuGraphParm5.Text = gpsRM.GetString("psGraphParm5")
        mbInInit = False
        SelectedRobot = sParam
        subFormatScreenLayout()

    End Sub
    Friend Sub MakeCarMove() Implements BSDForm.MakeCarMove
        '********************************************************************************************
        'Description:  just here for implements
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Private Sub subInitCartoon()
        '********************************************************************************************
        'Description: Add the proper image lists to the collection to do cartoons
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        SetCCType(colZones, Me.Controls, uctrlCartoon, mCCToon, frmMain.colArms(cboRobot.Text))
        If ((uctrlCartoon Is Nothing) = False) And ((mCCToon Is Nothing) = False) Then
            'Set the size and location on the form.
            uctrlCartoon.Size = New Size(mnTOON_WIDTH, mnTOON_HEIGHT)
            subFormatScreenLayout()
        End If
    End Sub
    Private Sub subNewRobotSelected()
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


        If cboRobot.Text = gcsRM.GetString("csNONE") Then
            If Not (uctrlCartoon Is Nothing) Then
                uctrlCartoon.Visible = False
            End If
        Else
            Dim o As clsArm = frmMain.colArms(cboRobot.Text)

            If Not o.IsOpener Then
                meCCType = o.ColorChangeType
                subInitCartoon()
                If Not (uctrlCartoon Is Nothing) Then
                    uctrlCartoon.Visible = True
                End If
            Else
                If Not (uctrlCartoon Is Nothing) Then
                    uctrlCartoon.Visible = False
                End If
            End If
        End If

        subFormatScreenLayout()

    End Sub
    Sub subCleanUpRobotLabels(ByVal rArm As clsArm) Implements BSDForm.subCleanUpRobotLabels
        '********************************************************************************************
        'Description:  
        '       called by frmMian.tmrSA_Tick
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oUctlList As uctlList = DirectCast(pnlProcess.Controls("uctlR" & rArm.RobotNumber.ToString), uctlList)
        If Not (oUctlList Is Nothing) Then
            oUctlList.LabelText(0) = String.Empty
        End If
        If cboRobot.Text = rArm.Name Then
            If Not (mCCToon Is Nothing) Then
                With mCCToon
                    ' Update class module with valve integer array
                    .SharedValveStates = 0
                    .GroupValveStates = 0
                End With
            End If
        End If
    End Sub
    Public Sub subUpdateSAData() Implements BSDForm.subUpdateSAData
        '********************************************************************************************
        'Description:  
        '       called by frmMian.tmrSA_Tick
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/12/10  MSW     Missing Scale on uA
        '********************************************************************************************
        Try
            'msSAData = msSAData
            'mnRobotIndex = colArms.IndexOf(oArm)

            'the msSAData should look like this....
            '		(0)	"EQ1: Requested Bell Speed"	String
            '		(1)	"0"	String
            '		(2)	"EQ1: Requested Shaping Air"	String
            '		(3)	""	String
            'ect...
            Dim sSAVal As String
            Dim fVal As Single
            Dim oArm As clsArm = frmMain.colArms(mnRobotIndex)
            Dim Applicator As clsApplicator = frmMain.colApplicators.Item(oArm.ColorChangeType)
            Dim nEquipNumber As Integer = oArm.ArmNumber
            Dim nRobotIdx As Integer = mnRobotIndex
            Dim oUctlList As uctlList = DirectCast(pnlProcess.Controls("uctlR" & oArm.RobotNumber.ToString), uctlList)
            Dim sOutletPressure As String = String.Empty
            Dim sFlow As String = String.Empty
            Dim sFlowAct As String = String.Empty
            'Paint flow and totals
            '        PresetNum = 7      
            '        PresetFlow = 8
            '        ActualFlow = 9
            '        PaintInCan = 9  'Accustat doesn't need actual flow
            '        FlowTotal = 10
            '        TPR = 11       'TODO
            '        OutPressure = 11  'TPR for accustat, ICS, outpressure for IPC
            'Preset Number
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PresetNum) > 0 Then
                oUctlList.LabelText(eProcLbl.PresetNum) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PresetNum))
            End If
            'Setpoint, they should all have this
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow) > 0 Then
                sFlow = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow))
                oUctlList.LabelText(eProcLbl.PresetFlow) = sFlow
            End If
            'actual flow, paint in can, app specific
            Select Case oArm.ColorChangeType
                'Flow Rate 
                Case eColorChangeType.ACCUSTAT, eColorChangeType.AQUABELL ' Paint in can for accustat
                    If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintInCan) > 0 Then
                        sFlowAct = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintInCan))
                        oUctlList.LabelText(eProcLbl.PaintInCan) = sFlowAct

                    End If
                    'Any other special cases, add here
                Case Else
                    'If there's an actual flow, use it
                    If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow) > 0 Then
                        sFlow = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow))
                        oUctlList.LabelText(eProcLbl.ActualFlow) = sFlow
                        sFlowAct = sFlow
                    End If
            End Select

            ' Total Flow
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.TotFlow) > 0 Then
                'round floating point to 0.1
                Dim fTmp As Single = CType(msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.TotFlow)), Single)
                fTmp = CType(CInt(10.0 * fTmp), Single) / 10
                oUctlList.LabelText(eProcLbl.FlowTotal) = fTmp.ToString
            End If

            ' Outlet Pressure
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure) > 0 Then
                sOutletPressure = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure))
                oUctlList.LabelText(eProcLbl.OutPressure) = sOutletPressure
            End If


            'Atomizing air or bell speed
            '        PresetAtom = 12
            '        PresetBS = 12
            '        ActualBellSpeed = 13
            'Requested
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqBSAA) > 0) Then
                oUctlList.LabelText(eProcLbl.PresetAtom) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqBSAA))
            End If
            'Actual
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActBSAA) > 0 Then 'Feedback
                sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActBSAA))
                fVal = CSng(sSAVal)
                If Applicator.CntrType(eParamID.Atom) = eCntrType.APP_CNTR_DQ Then
                    'This is a DQ feedback, scale it
                    fVal = CSng(sSAVal)
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                End If
                sSAVal = fVal.ToString("##0.0")
                oUctlList.LabelText(eProcLbl.ActualBellSpeed) = sSAVal
            End If

            'Fan or shaping air (1)
            '        PresetFan = 14
            '        PresetSA = 14
            '        SAManPressure = 15
            '        ActualSA = 15
            'Requested
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA) > 0) Then
                oUctlList.LabelText(eProcLbl.PresetSA) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA))
            End If
            'Actual
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA) > 0 Then 'Feedback
                sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA))
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_DQ Then
                    'This is a DQ feedback, scale it
                    fVal = CSng(sSAVal)
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                    sSAVal = fVal.ToString("##0.0")
                End If
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA Then
                    'This is a DQ feedback, scale it
                    fVal = CSng(sSAVal)
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                    sSAVal = fVal.ToString("##0")
                End If
                'oUctlList.LabelText(eProcLbl.ActualSA) = sSAVal
            End If

            'Shaping air (2)
            '        PresetSA2 = 16
            '        ActualSA2 = 17
            'Requested
            'If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA2) > 0) Then
            '    oUctlList.LabelText(eProcLbl.PresetSA2) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA2))
            'End If
            ''Actual
            'If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA2) > 0 Then 'Feedback
            '    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA2))
            '    If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_DQ Then
            '        'This is a DQ feedback, scale it
            '        fVal = CSng(sSAVal)
            '        fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
            '        If fVal < 0 Then
            '            fVal = 0
            '        End If
            '        sSAVal = fVal.ToString("##0.0")
            '    End If
            '    If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA Then
            '        'This is a DQ feedback, scale it
            '        fVal = CSng(sSAVal)
            '        fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
            '        If fVal < 0 Then
            '            fVal = 0
            '        End If
            '        sSAVal = fVal.ToString("##0")
            '    End If
            '    oUctlList.LabelText(eProcLbl.ActualSA2) = sSAVal
            'End If

            'Honda Estats
            '        HondaEstatPresetNum = 18
            '        HondaEstatPreset = 19
            '        HondaActualKV = 20 
            '        HondaActualUA = 21
            'Estat preset num
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.EstatPresetNum) > 0) Then
                oUctlList.LabelText(eProcLbl.EstatPresetNum) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.EstatPresetNum))
            End If
            'Requested KV
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES) > 0) Then
                oUctlList.LabelText(eProcLbl.PresetEstat) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES))
            End If
            'Estat KV
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV) > 0 Then
                sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV))
                fVal = CSng(sSAVal)
                fVal = (fVal - Applicator.KVOffset) * Applicator.KVScale
                If fVal < 0 Then
                    fVal = 0
                End If
                oUctlList.LabelText(eProcLbl.ActualKV) = fVal.ToString("##0.0")
            End If
            'Estat uA
            If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA) > 0 Then
                sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA))
                fVal = CSng(sSAVal)
                fVal = (fVal - Applicator.uAOffset) * Applicator.uAScale ' 04/12/10  MSW     Missing Scale on uA
                'kludge for Honda Estat
                'Should be able to this in the DB
                'fVal = (fVal - Applicator.uAOffset) * Applicator.uAScale
                oUctlList.LabelText(eProcLbl.ActualUA) = fVal.ToString("##0")
            End If

            'FANUC Estats
            '        EstatPresetNum = 16
            '        PresetEstat = 17
            '        ActualKV = 18
            '        ActualUA = 19
            'Estat KV
            'If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV) > 0 Then
            '    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV))
            '    fVal = CSng(sSAVal)
            '    fVal = (fVal - Applicator.KVOffset) * Applicator.KVScale
            '    If fVal < 0 Then
            '        fVal = 0
            '    End If
            '    oUctlList.LabelText(eProcLbl.ActualKV) = fVal.ToString("###")
            'End If

            ''Estat uA
            'If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA) > 0 Then
            '    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA))
            '    fVal = CSng(sSAVal)
            '    fVal = (fVal - Applicator.uAOffset) * Applicator.uAScale
            '    oUctlList.LabelText(eProcLbl.ActualUA) = fVal.ToString("###")
            'End If
            ''requested - detail view only
            'If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES) > 0) Then
            '    oUctlList.LabelText(eProcLbl.PresetEstat) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES))
            'End If
            ''Estat preset num
            'If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.EstatPresetNum) > 0) Then
            '    oUctlList.LabelText(eProcLbl.EstatPresetNum) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.EstatPresetNum))
            'End If

            'CC cycle
            '        CCCycle = 22
            '        CCTime = 23
            'CC cycle name
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCCycleName) > 0) Then
                If mbCCCycleActive(nRobotIdx) Then
                    oUctlList.LabelText(eProcLbl.CCCycle) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCCycleName))
                Else
                    oUctlList.LabelText(eProcLbl.CCCycle) = String.Empty
                End If
            End If
            'CC cycle time
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.LastCCTime) > 0) Then
                oUctlList.LabelText(eProcLbl.CCTime) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.LastCCTime))
            End If

            'Job data
            '        Color = 4
            '        JobName = 2
            '        PathName = 3
            'Color
            sSAVal = String.Empty
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor) > 0) Then
                sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor))
            End If
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor) > 0) Then
                sSAVal = sSAVal & " - " & msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName))
            End If
            If sSAVal <> String.Empty Then
                oUctlList.LabelText(eProcLbl.Color) = sSAVal
            End If
            'Job
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentJob) > 0) Then
                oUctlList.LabelText(eProcLbl.JobName) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentJob))
            End If
            'Process
            If (frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentPath) > 0) Then
                oUctlList.LabelText(eProcLbl.PathName) = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentPath))
            End If

            If mnGraphRobot = oArm.RobotNumber Then
                Dim sGraphData As String() = Nothing
                Select Case mnGraphParm
                    Case 1 'Paint
                        ReDim sGraphData(2)
                        sGraphData(0) = oUctlList.LabelText(eProcLbl.PresetFlow)
                        sGraphData(1) = oUctlList.LabelText(eProcLbl.ActualFlow)
                        sGraphData(2) = oUctlList.LabelText(eProcLbl.OutPressure)
                    Case 2 'Bell/atom air
                        ReDim sGraphData(1)
                        sGraphData(0) = oUctlList.LabelText(eProcLbl.PresetBS)
                        sGraphData(1) = oUctlList.LabelText(eProcLbl.ActualBellSpeed)
                    Case 3 'Fan/Shaping air
                        ReDim sGraphData(1)
                        sGraphData(0) = oUctlList.LabelText(eProcLbl.PresetSA)
                        sGraphData(1) = oUctlList.LabelText(eProcLbl.SAManPressure)
                    Case 4 'Estat
                        ReDim sGraphData(1)
                        sGraphData(0) = oUctlList.LabelText(eProcLbl.ActualKV)
                        sGraphData(1) = oUctlList.LabelText(eProcLbl.ActualUA)
                    Case 5 'Fan/Shaping air 2
                        'ReDim sGraphData(1)
                        'sGraphData(0) = oUctlList.LabelText(eProcLbl.PresetSA2)
                        'sGraphData(1) = oUctlList.LabelText(eProcLbl.ActualSA2)
                End Select
                If Not (sGraphData Is Nothing) Then
                    frmGraph.AddNodes(sGraphData)
                End If
            End If
            'More stuff only for the robot selected in cboRobot
            If oArm.Name = cboRobot.Text Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Color change valves 
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' 4 shared valves init to 0
                Dim nSharedValves() As Integer = {0, 0, 0, 0}
                'shared valve 1 TRIGGER
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Trigger_Shared0) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Trigger_Shared0))
                    nSharedValves(0) = CInt(sSAVal)
                End If
                'shared valve 2 Spare 1
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Shared1) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Shared1))
                    nSharedValves(1) = CInt(sSAVal)
                End If
                'shared valve 3 Hardner 
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HE_Shared2) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HE_Shared2))
                    nSharedValves(2) = CInt(sSAVal)
                End If
                'shared valve 4 Color Enable 
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CE_Shared3) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CE_Shared3))
                    nSharedValves(3) = CInt(sSAVal)
                End If

                'Color Valves
                Dim sCCValves As String = Nothing
                Dim sCCValves2 As String = Nothing
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves1) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves1))
                    sCCValves = sSAVal
                End If
                If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves2) > 0 Then
                    sSAVal = msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves2))
                    sCCValves2 = sSAVal
                End If

                Dim nSharedValvesState As Integer = nSharedValves(0) + 2 * nSharedValves(1) + 4 * nSharedValves(2) + 8 * nSharedValves(3)
                Dim nGroupValvesState As Integer = CInt(sCCValves) + 65536 * CInt(sCCValves2)
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Update the fluid diagrams
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If Not (mCCToon Is Nothing) And Not (uctrlCartoon Is Nothing) Then
                    With mCCToon
                        ' Update class module with valve integer array
                        .SharedValveStates = nSharedValvesState
                        .GroupValveStates = nGroupValvesState

                        'color header label
                        sSAVal = ""
                        If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor) > 0 Then

                            sSAVal = gpsRM.GetString("psSYSCOL") & msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor))
                        End If
                        If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName) > 0 Then
                            sSAVal = sSAVal & " - " & msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName))
                        End If
                        If frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentValve) > 0 Then
                            If sSAVal <> "" Then
                                sSAVal = sSAVal & ", "
                            End If
                            sSAVal = sSAVal & gpsRM.GetString("psVALVE") & msSAData(frmMain.gtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentValve))
                        End If
                        If sSAVal <> "" Then
                            mCCToon.PaintHeader = sSAVal
                        End If

                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Applicator/CC type specific labels
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        Select Case oArm.ColorChangeType
                            Case eColorChangeType.ACCUSTAT
                            Case eColorChangeType.SINGLE_PURGE
                            Case eColorChangeType.VERSABELL2
                                Dim sLabels As String()
                                ReDim sLabels(2)
                                sLabels(0) = "lblFlow"
                                sLabels(1) = sFlow
                                .subUpdateAdditionalParams(sLabels)

                            Case eColorChangeType.VERSABELL2_PLUS
                                Dim sLabels As String()
                                ReDim sLabels(4)
                                sLabels(0) = "lblOutPressure"
                                sLabels(1) = sOutletPressure
                                sLabels(2) = "lblFlow"
                                sLabels(3) = sFlow
                                .subUpdateAdditionalParams(sLabels)
                        End Select
                        ' update cartoons
                        .subUpdateValveCartoon()
                    End With
                End If
            End If
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - uctlProcessScreen.subUpdateSAData", _
                    ex, frmMain.msSCREEN_NAME & "_Process", frmMain.Status)
        End Try


    End Sub
    Public Sub InitPLCData() Implements BSDForm.InitPLCData
        '********************************************************************************************
        'Description: Clear out saved copy of PLC data so it'll process it all next
        '               time data is sent
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msBoothRef = Nothing
        msRobotRef = Nothing
        msQueueRef = Nothing
    End Sub
    Private Sub subUpdatePLCData()
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
        Dim nBoxPtr As Integer = 0
        Dim sName As String = String.Empty

        Try

            Select Case LinkIndex
                Case ePLCLink.Zone
                    ''wilmington specific same for all robots
                    'For nBoxPtr = 1 To 4
                    '    Dim o As GroupBox = DirectCast(pnlProcess.Controls( _
                    '                                    "gpbR" & Format(nBoxPtr, "00")), GroupBox)

                    '    'sName = "lblR" & Format(nBoxPtr, "00") & Format(CInt(eProcLbl.Style), "00")
                    '    o.Controls(sName).Text = msPLCData(eBooth.StyleWord)

                    '    'vin#
                    '    Dim s(2) As String
                    '    s(0) = msPLCData(eBooth.VINWord)
                    '    s(1) = msPLCData(eBooth.VINWord + 1)
                    '    s(2) = msPLCData(eBooth.VINWord + 2)

                    '    sName = "lblR" & Format(nBoxPtr, "00") & Format(CInt(eProcLbl.VIN), "00")
                    '    o.Controls(sName).Text = mPlantSpec.DecipherVINNumber(s, 3)
                    'Next

                Case ePLCLink.Robot
                    Dim nOffset As Integer = 0
                    Dim nStat As Integer = 0
                    Dim nTmp As Integer = 0

                    nOffset = 0

                    For Each oArm As clsArm In frmMain.colArms
                        Dim oUctlList As uctlList = DirectCast(pnlProcess.Controls("uctlR" & oArm.RobotNumber.ToString), uctlList)

                        'status bits
                        nOffset = ((oArm.RobotNumber - 1) * CInt(eRobot.WordsPerRobot))
                        If (nOffset + eRobot.WordsPerRobot - 1) > UBound(msRobotData) Then
                            Exit For
                        End If


                        nStat = CInt(msRobotData(nOffset + CInt(eRobot.RunningStat)))
                        bInCycle(oArm.RobotNumber) = ((nStat And eRobotRunStat.InCycle) = eRobotRunStat.InCycle)

                        mbCCCycleActive(oArm.RobotNumber - 1) = ((nStat And eRobotRunStat.ColChgActive) = eRobotRunStat.ColChgActive)
                        If mbCCCycleActive(oArm.RobotNumber - 1) Then
                            'ScatteredAccessData access will come back for it.
                        Else
                            oUctlList.LabelText(eProcLbl.CCCycle) = String.Empty
                        End If

                        'Cycle time
                        nTmp = CInt(msRobotData(eRobot.CycleTime + nOffset))
                        oUctlList.LabelText(eProcLbl.CycleTime) = (nTmp / 10).ToString("0.0")

                        'Carrier
                        oUctlList.LabelText(eProcLbl.Carrier) = msRobotData(eRobot.Carrier + nOffset)

                        'Style
                        If colStyles.UseAscii Then
                            oUctlList.LabelText(eProcLbl.Style) = mMathFunctions.CvIntegerToASCII(CInt(msRobotData(eRobot.Style + nOffset)), colStyles.PlantAsciiMaxLength)
                        Else
                            oUctlList.LabelText(eProcLbl.Style) = msRobotData(eRobot.Style + nOffset)
                        End If

                        'Option
                        oUctlList.LabelText(eProcLbl.OptionNum) = msRobotData(eRobot.Optn + nOffset)

                        'Color
                        'If oArm.IsOpener Then
                        '    oUctlList.LabelText(eProcLbl.Color) = String.Empty
                        'Else
                        '    oUctlList.LabelText(eProcLbl.Color) = msRobotData(eRobot.Color + nOffset)
                        'End If

                        'VIN
                        oUctlList.LabelText(eProcLbl.VIN) = (msRobotData(eRobot.Vin2 + nOffset)) & (msRobotData(eRobot.Vin1 + nOffset))

                        ''PLC Estat
                        'Dim sKV As String = msPLCData(eRobot.EstatKV + nOffset)
                        'Dim sUA As String = msPLCData(eRobot.EstatUA + nOffset)

                        'sName = "lblR" & Format(nBoxPtr, "00") & Format(CInt(eProcLbl.ActualKV), "00")
                        'o.Controls(sName).Text = sKV

                        'sName = "lblR" & Format(nBoxPtr, "00") & Format(CInt(eProcLbl.ActualUA), "00")
                        'o.Controls(sName).Text = sUA

                    Next
                Case ePLCLink.Queue
                    'Nothing for the queue data on this screen
            End Select



        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - uctlProcessScreen.subUpdatePLCData", _
                                ex, frmMain.msSCREEN_NAME & "_Process", frmMain.Status)
        End Try



    End Sub
    Friend Sub PrivilegeChange(ByVal NewPrivilege As mPassword.ePrivilege) _
                                                       Implements BSDForm.PrivilegeChange
        '********************************************************************************************
        'Description: password privilege changed - called from main form
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub
    Public Overloads Sub Show(ByVal StartData As String()) Implements BSDForm.Show
        '********************************************************************************************
        'Description:  Show me
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        LinkIndex = ePLCLink.Queue
        PLCData = frmMain.GetCachedData(ePLCLink.Queue)
        LinkIndex = ePLCLink.Robot
        PLCData = frmMain.GetCachedData(ePLCLink.Robot)
        LinkIndex = ePLCLink.Zone
        PLCData = frmMain.GetCachedData(ePLCLink.Zone)
        System.Windows.Forms.Application.DoEvents()

        Me.Show()
    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description: arrange the panels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const nSHIM As Integer = 5  ' A little space to leave on the edges
        Const nSCROLLWIDTH As Integer = 20 'width of a vertical scrollbar
        If mbInInit Then Exit Sub
        Dim bShowToon As Boolean = False
        If Not (uctrlCartoon Is Nothing) Then
            bShowToon = uctrlCartoon.Visible
        End If
        Dim nWidth As Integer
        Dim nHeight As Integer = frmMain.tscMain.ContentPanel.Height - (pnlProcess.Top + nSHIM)
        'If the form isn't too small, place the cartoon so it's in view
        Dim nToonLeft As Integer = frmMain.Width - mnTOON_WIDTH - nSHIM
        'Leave at least 1 row of data to the left of the cartoon
        Dim nToonMinLeft As Integer = pnlProcess.Left + mnListSpacing + mnListOffset
        If nHeight < uctlR1.Height + nSCROLLWIDTH Then 'There's probably a vert scrollbar in pnlProcess
            nToonMinLeft = nToonMinLeft + nSCROLLWIDTH
        End If
        If nToonLeft < nToonMinLeft Then
            nToonLeft = nToonMinLeft
        End If
        'Move cboRobot and label to keep a position relative to the cartoon
        If bShowToon Then
            nWidth = nToonLeft - (pnlProcess.Left + nSHIM)
        Else
            nWidth = frmMain.tscMain.ContentPanel.Width - (pnlProcess.Left + nSHIM)
        End If
        If nWidth > pnlProcess.PreferredSize.Width + nSCROLLWIDTH Then
            nWidth = pnlProcess.PreferredSize.Width + nSCROLLWIDTH
            nToonLeft = pnlProcess.Left + nWidth + nSHIM
        End If
        If Not (uctrlCartoon Is Nothing) Then
            uctrlCartoon.Location = New Point(nToonLeft, 0)
        End If
        cboRobot.Left = nToonLeft - (cboRobot.Width + nSHIM)
        lblViewRobotCap.Left = cboRobot.Left - (lblViewRobotCap.Width + nSHIM)
        'SA data columns get the rest of the space
        pnlProcess.Width = nWidth
        pnlRobotNames.Width = nWidth
        If (nHeight > (uctlR1.Height + nSCROLLWIDTH + nSHIM)) Then
            pnlProcess.Height = uctlR1.Height + nSCROLLWIDTH + nSHIM
            pnlLabels.Height = uctlR1.Height + nSCROLLWIDTH + nSHIM
        Else
            pnlProcess.Height = nHeight
            pnlLabels.Height = nHeight
        End If
        'Use autoscroll values from pnlProcess to move the row and column labels.
        uctlLabels.Top = pnlProcess.AutoScrollPosition.Y + uctlR1.Top
        For Each oLabel As Label In pnlRobotNames.Controls
            'Stored the starting position in the tags
            oLabel.Left = CInt(oLabel.Tag) + pnlProcess.AutoScrollPosition.X
        Next
        Me.Refresh()
    End Sub
#End Region
#Region " Events "
    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, _
                        ByVal e As System.EventArgs) Handles cboRobot.SelectedIndexChanged
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
        If cboRobot.SelectedIndex < 0 Then Exit Sub
        subNewRobotSelected()
    End Sub


    Private Sub uctlProcessScreen_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
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
        subFormatScreenLayout()
    End Sub


    Private Sub pnlProcess_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles pnlProcess.Scroll
        '********************************************************************************************
        'Description: pnlProcess scrollbars are used to move the labels in pnlLabels and pnlRobotNames
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        uctlLabels.Top = pnlProcess.AutoScrollPosition.Y
        For Each oLabel As Label In pnlRobotNames.Controls
            'Stored the starting position in the tags
            oLabel.Left = CInt(oLabel.Tag) + pnlProcess.AutoScrollPosition.X
        Next
        Me.Refresh()
    End Sub
#End Region
    Private Sub UctlRx_MouseDown(ByVal sender As Object)
        '********************************************************************************************
        'Description:Prep the pop-up menu
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oUctl As uctlList = DirectCast(sender, uctlList)
        ' take the number after "UctlR"
        Dim sNum As String = oUctl.Name.Substring(5)
        Dim oLabel As Label = DirectCast(pnlRobotNames.Controls("lblR" & sNum), Label)
        mnuRobotName.Text = oLabel.Text
        mnuRobotName.Tag = sNum

    End Sub

    Private Sub lblRx_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description:Prep the pop-up menu
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLabel As Label = DirectCast(sender, Label)
        ' take the number after "lblR"
        Dim sNum As String = oLabel.Name.Substring(4)
        mnuRobotName.Text = oLabel.Text
        mnuRobotName.Tag = sNum

    End Sub

    Private Sub mnuDetailView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDetailView.Click
        '********************************************************************************************
        'Description:pop-up menu click handler
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            cboRobot.Text = cboRobot.Items(CInt(mnuRobotName.Tag)).ToString
        Catch ex As Exception
            'not too worried about it
        End Try

    End Sub

    Private Sub mnuGraphParm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuGraphParm1.Click
        '********************************************************************************************
        'Description:pop-up menu click handler - parm 1 paint flow
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGraphRobot = CInt(mnuRobotName.Tag)
        mnGraphParm = 1
        Application.DoEvents()
        If mApplicator Is Nothing Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        ElseIf mApplicator.ColorChangeType <> frmMain.colArms(mnuRobotName.Text).ColorChangeType Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        End If
        frmGraph.NumLines = 3
        frmGraph.Title = mnuRobotName.Text & " " & gpsRM.GetString("psGraphTitle1")
        frmGraph.LineColor(0) = Color.Blue
        frmGraph.LineColor(1) = Color.Green
        frmGraph.LineColor(2) = Color.Red
        frmGraph.LineLabel(0) = gpsRM.GetString("psPROC_CAP08") 'Requested Flow
        frmGraph.LineLabel(1) = gpsRM.GetString("psPROC_CAP09") 'Actual Flow
        frmGraph.LineLabel(2) = gpsRM.GetString("psPROC_CAP11") 'Outlet Pressure
        frmGraph.AxisUnitLabel(0) = mApplicator.ParamUnits(eParamID.Flow) 'grsRM.GetString("rsCCMIN")
        frmGraph.AxisUnitLabel(1) = grsRM.GetString("rsPSI")
        frmGraph.YAxis(2) = 1 ' Put pressure labels on the right
        'The default max flow is kind of silly, so we'll default to a lower range here  
        'If it goes to high, the graph will handle it.
        Dim nTmp As Integer = CInt(mApplicator.MaxEngUnit(eParamID.Flow))
        If mApplicator.BellApplicator Then
            If nTmp > 500 Then
                nTmp = 500
            End If
        Else
            If nTmp > 1000 Then
                nTmp = 1000
            End If
        End If
        frmGraph.MaxScale(0) = nTmp
        frmGraph.MaxScale(1) = nTmp
        frmGraph.MaxScale(2) = 100
        frmGraph.Show()
    End Sub

    Private Sub mnuGraphParm2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuGraphParm2.Click
        '********************************************************************************************
        'Description:pop-up menu click handler - parm 2 bell speed/atom air
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGraphRobot = CInt(mnuRobotName.Tag)
        mnGraphParm = 2
        Application.DoEvents()
        If mApplicator Is Nothing Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        ElseIf mApplicator.ColorChangeType <> frmMain.colArms(mnuRobotName.Text).ColorChangeType Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        End If
        frmGraph.NumLines = 2
        frmGraph.Title = mnuRobotName.Text & " " & gpsRM.GetString("psGraphTitle2")
        frmGraph.LineColor(0) = Color.Blue
        frmGraph.LineColor(1) = Color.Green
        frmGraph.LineLabel(0) = gpsRM.GetString("psPROC_CAP12") 'Requested Bell Speed
        frmGraph.LineLabel(1) = gpsRM.GetString("psPROC_CAP13") 'Actual Bell Speed
        frmGraph.AxisUnitLabel(0) = mApplicator.ParamUnits(eParamID.Atom) ' grsRM.GetString("rsKRPM")
        frmGraph.MaxScale(0) = CInt(mApplicator.MaxEngUnit(eParamID.Atom))
        frmGraph.MaxScale(1) = CInt(mApplicator.MaxEngUnit(eParamID.Atom))
        frmGraph.Show()

    End Sub

    Private Sub mnuGraphParm3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuGraphParm3.Click
        '********************************************************************************************
        'Description:pop-up menu click handler - parm 3 fan/shaping air
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGraphRobot = CInt(mnuRobotName.Tag)
        mnGraphParm = 3
        Application.DoEvents()
        If mApplicator Is Nothing Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        ElseIf mApplicator.ColorChangeType <> frmMain.colArms(mnuRobotName.Text).ColorChangeType Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        End If
        frmGraph.NumLines = 2
        frmGraph.Title = mnuRobotName.Text & " " & gpsRM.GetString("psGraphTitle3")
        frmGraph.LineColor(0) = Color.Blue
        frmGraph.LineColor(1) = Color.Green
        frmGraph.LineLabel(0) = gpsRM.GetString("psPROC_CAP14") 'Requested Shaping Air
        frmGraph.LineLabel(1) = gpsRM.GetString("psPROC_CAP15") 'Actual Shaping Air
        'frmGraph.YAxis(1) = 1 'Put pressure on a different scale
        frmGraph.AxisUnitLabel(0) = mApplicator.ParamUnits(eParamID.Fan) ' grsRM.GetString("rsLPM")
        frmGraph.AxisUnitLabel(1) = mApplicator.ParamUnits(eParamID.Fan) ' grsRM.GetString("rsPSI")
        frmGraph.MaxScale(0) = CInt(mApplicator.MaxEngUnit(eParamID.Fan))
        frmGraph.MaxScale(1) = CInt(mApplicator.MaxEngUnit(eParamID.Fan)) '100
        frmGraph.Show()
    End Sub

    Private Sub mnuGraphParm4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuGraphParm4.Click
        '********************************************************************************************
        'Description:pop-up menu click handler - parm 4 estats
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGraphRobot = CInt(mnuRobotName.Tag)
        mnGraphParm = 4
        Application.DoEvents()
        If mApplicator Is Nothing Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        ElseIf mApplicator.ColorChangeType <> frmMain.colArms(mnuRobotName.Text).ColorChangeType Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        End If
        frmGraph.NumLines = 2
        frmGraph.Title = mnuRobotName.Text & " " & gpsRM.GetString("psGraphTitle4")
        frmGraph.LineColor(0) = Color.Blue
        frmGraph.LineColor(1) = Color.Red
        frmGraph.LineLabel(0) = gpsRM.GetString("psPROC_CAP18") 'Actual KV
        frmGraph.LineLabel(1) = gpsRM.GetString("psPROC_CAP19") 'Actual uA
        frmGraph.AxisUnitLabel(0) = grsRM.GetString("rsKV") & "," & grsRM.GetString("rsUA")
        frmGraph.MaxScale(0) = 100
        frmGraph.MaxScale(1) = 100
        frmGraph.Show()
    End Sub


    Private Sub mnuGraphParm5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuGraphParm5.Click
        '********************************************************************************************
        'Description:pop-up menu click handler - parm 5 shaping air 2 (outer)
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGraphRobot = CInt(mnuRobotName.Tag)
        mnGraphParm = 5
        Application.DoEvents()
        If mApplicator Is Nothing Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        ElseIf mApplicator.ColorChangeType <> frmMain.colArms(mnuRobotName.Text).ColorChangeType Then
            mApplicator = New clsApplicator(frmMain.colArms(mnuRobotName.Text), colZones.ActiveZone)
        End If
        frmGraph.NumLines = 2
        frmGraph.Title = mnuRobotName.Text & " " & gpsRM.GetString("psGraphTitle5")
        frmGraph.LineColor(0) = Color.Blue
        frmGraph.LineColor(1) = Color.Green
        frmGraph.LineLabel(0) = gpsRM.GetString("psPROC_CAP16") 'Requested Shaping Air
        frmGraph.LineLabel(1) = gpsRM.GetString("psPROC_CAP17") 'Actual Shaping Air
        'frmGraph.YAxis(1) = 1 'Put pressure on a different scale
        frmGraph.AxisUnitLabel(0) = mApplicator.ParamUnits(eParamID.Fan2) ' grsRM.GetString("rsLPM")
        frmGraph.AxisUnitLabel(1) = mApplicator.ParamUnits(eParamID.Fan2) ' grsRM.GetString("rsPSI")
        frmGraph.MaxScale(0) = CInt(mApplicator.MaxEngUnit(eParamID.Fan2))
        frmGraph.MaxScale(1) = CInt(mApplicator.MaxEngUnit(eParamID.Fan2)) '100
        frmGraph.Show()
    End Sub
End Class