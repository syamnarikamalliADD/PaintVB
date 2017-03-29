Friend Class uctlQueueScreenLeft
    Implements BSDForm

#Region " Declares "

    Private msBoothData As String() = Nothing
    Private msRobotData As String() = Nothing
    Private msQueueData As String() = Nothing
    Private msPreDetectQueueData As String() = Nothing
    Private msBoothRef As String() = Nothing
    Private msRobotRef As String() = Nothing
    Private msQueueRef As String() = Nothing
    Private mbIsRemote As Boolean = False
    Private mnRobotIndex As Integer = 0
    Private mnLinkIndex As ePLCLink = ePLCLink.None
    Private msSAData As String() = Nothing

    Private Const mnNumEstops As Integer = 1
    Private Const mnNumLimits As Integer = 2
    Private Const mnNumMaintLocks As Integer = 8
    Private Const mnPOST_DETECT_Q1_POS_IN_DATA As Integer = 11

    'NVSP 02/28/2017 Get descriptions for mouseover
    Private oMoControllers As clsControllers = Nothing
    Private oMoArms As clsArms = Nothing
    Private oMoStyles As clsSysStyles = Nothing
    Private oMoColors As clsSysColors = Nothing
    Private oMoOptions As clsSysOptions = Nothing

    'NVSP 02/28/2017
    'Communication vars
    Friend WithEvents EditFrm As frmEdit = Nothing
    Friend msMyQueueData As String() = Nothing

    Dim mNumCarriers As Integer = 0

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
            Return "QueueLeft"
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
                Case ePLCLink.myQueue
                    Return msPreDetectQueueData
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
                Case ePLCLink.myQueue
                    msPreDetectQueueData = value
                Case Else
                    Debug.Assert(False)
            End Select
            UpdatePLCData()
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

#End Region

#Region " Routines "

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
    End Sub

    Friend Sub Initialize(Optional ByVal sParam As String = "") Implements BSDForm.Initialize
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

        subInitFormText()

        'NVSP 02/28/2017 Added for my queue display stuff
        MyQueueEventHandler()
        'NVSP 02/28/2017 Get all the stupid collections for descriptions in masterqueue mouseover
        oMoControllers = New clsControllers(colZones, False)
        oMoArms = LoadArmCollection(oMoControllers, False)
        oMoColors = New clsSysColors(oMoArms(0)) 'Just use the first robot wherever
        oMoOptions = New clsSysOptions(colZones.ActiveZone)
        oMoStyles = New clsSysStyles(colZones.ActiveZone, False)

        'NVSP 03/03/2017 Changes for inserting a job at queue location
        cboInsertJobLocation.Items.Clear()
        Dim sName As String = ""
        cboInsertJobLocation.Items.Add(sName)
        'NVSP This populates the dropdown box
        For nQueueCounter As Integer = 1 To 159
            sName = ""
            sName = "Position" & nQueueCounter.ToString
            cboInsertJobLocation.Items.Add(sName)
        Next

        ''Dim sTag(159) As String
        ''For nQueueCounter As Integer = 1 To 159
        ''    sTag(nQueueCounter) = nQueueCounter.ToString
        ''Next
        ''cboInsertJobLocation.Tag = sTag

        Call subSetToolTips()

    End Sub
    Private Sub MyQueueEventHandler()
        '********************************************************************************************
        'Description: Set all buttons to accept the same event handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 20170228  NVSP     Accept any length of number after the carrier label name
        '********************************************************************************************
        For Each Ctrl As Control In pnlMain.Controls
            If Ctrl.Name.StartsWith("Carrier") Then
                AddHandler Ctrl.Click, AddressOf ButtonPress
                'NVSP 02/28/2017 Mouseover
                AddHandler Ctrl.MouseEnter, AddressOf MouseOver
                'NVSP 02/28/2017 Enable this line to make hover data disappear when you're not hovering
                'over a carrier anymore.
                AddHandler Ctrl.MouseLeave, AddressOf MouseExit

                'NVSP 02/28/2017 - Record the largest carrier number found for update loop
                Dim nnn As Integer = 0
                nnn = Int32.Parse(Ctrl.Name.Replace("Carrier", ""))
                If nnn > mNumCarriers Then
                    mNumCarriers = nnn
                End If

            End If
        Next

    End Sub
    Private Sub ButtonPress(ByVal sender As System.Object, ByVal e As System.EventArgs)

        '********************************************************************************************
        'Description: Event handler for button presses on Master Queue
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 20170228  NVSP Launch the subedit queue
        '********************************************************************************************

        'frmMain.mPLC.Zone = colZones.ActiveZone
        frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditInProgress"
        Dim s(0) As String
        s = frmMain.mPLC.PLCData

        If s(0) = "0" Then
            subEditMyQueue(sender)
        Else
            MsgBox("Someone is already editing the queue, try again when they are done.")
        End If

    End Sub
    Private Sub MouseOver(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Skip any problems with mouseover, don't crash the screen because of failures here.
        On Error Resume Next

        'NVSP 02/28/2017 Added Mouseover function
        Dim nQueuePos As Integer
        Dim bContinue As Boolean

        HoverDataVisible(True)
        'Check queue position number
        Dim oCtrl As Control = DirectCast(sender, Control)

        ' NVSP 02/28/2017 - startswith
        If oCtrl.Name.StartsWith("Carrier") Then
            'Predetect Queue
            nQueuePos = CInt(oCtrl.Name.Substring(7)) - 1
            'the double conversion on queue position number should remove any leading 0s
            bContinue = True
        Else
            nQueuePos = 0
            bContinue = False
        End If
        If Not bContinue Then Exit Sub

        Dim strQueueData As String() = frmMain.GetCachedData(ePLCLink.myQueue)

        Me.txtCarrier.Text = strQueueData(nQueuePos * 15 + 7)
        Me.txtStyle.Text = strQueueData(nQueuePos * 15 + 1)
        Me.txtColor.Text = strQueueData(nQueuePos * 15 + 2)
        Me.txtOption.Text = strQueueData(nQueuePos * 15 + 3)
        If CInt(Me.txtColor.Text) > 0 Then
            Me.txtColorDesc.Text = oMoColors(CInt(Me.txtColor.Text) - 1).Description.Text
        Else
            Me.txtColorDesc.Text = "No Data"
        End If
        If CInt(Me.txtStyle.Text) > 0 Then
            Me.txtStyleDesc.Text = oMoStyles(CInt(Me.txtStyle.Text) - 1).Description.Text
        Else
            Me.txtStyleDesc.Text = "No Data"
        End If
        If CInt(Me.txtOption.Text) > 0 Then
            Me.txtOptionDesc.Text = oMoOptions(CInt(Me.txtOption.Text)).Description.Text
        Else
            Me.txtOptionDesc.Text = "No Data"
        End If

        If strQueueData(nQueuePos * 15) = "1" Then Me.txtPaint.Text = "Yes" Else Me.txtPaint.Text = "No"

        On Error GoTo 0
    End Sub
    Private Sub MouseExit(ByVal sender As System.Object, ByVal ByVale As System.EventArgs)
        'NVSP 02/28/2017 Disappear abilities...
        HoverDataVisible(False)
    End Sub
    Private Sub HoverDataVisible(ByVal bVisible As Boolean)
        'NVSP 02/28/2017 Disappear abilities...
        Me.lblCarrier.Visible = bVisible
        Me.lblStyle.Visible = bVisible
        Me.lblColorNum.Visible = bVisible
        Me.lblOption.Visible = bVisible
        Me.lblPaint.Visible = bVisible
        Me.txtCarrier.Visible = bVisible
        Me.txtStyle.Visible = bVisible
        Me.txtColor.Visible = bVisible
        Me.txtOption.Visible = bVisible
        Me.txtPaint.Visible = bVisible
        Me.txtColorDesc.Visible = bVisible
        Me.txtOptionDesc.Visible = bVisible
        Me.txtStyleDesc.Visible = bVisible
    End Sub

    Private Sub subSetToolTips()
        '********************************************************************************************
        'Description: Attach tooltip as required
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With ttDesc
            .IsBalloon = True
            .SetToolTip(picEstop00, gpsRM.GetString("psESTOP"))
            .SetToolTip(picConvRunSS, gpsRM.GetString("psCONV_ENAB"))
            .SetToolTip(picEstatDisc, gpsRM.GetString("psESTAT_DISC"))
            .SetToolTip(picMaintLock1, gpsRM.GetString("psMAINT_LOCK_TT") & "1")
            .SetToolTip(picMaintLock2, gpsRM.GetString("psMAINT_LOCK_TT") & "2")
            .SetToolTip(picMaintLock3, gpsRM.GetString("psMAINT_LOCK_TT") & "3")
            .SetToolTip(picMaintLock4, gpsRM.GetString("psMAINT_LOCK_TT") & "4")
            .SetToolTip(picMaintLock5, gpsRM.GetString("psMAINT_LOCK_TT") & "5")
            .SetToolTip(picMaintLock6, gpsRM.GetString("psMAINT_LOCK_TT") & "6")
            .SetToolTip(picMaintLock7, gpsRM.GetString("psMAINT_LOCK_TT") & "7")
            .SetToolTip(picMaintLock8, gpsRM.GetString("psMAINT_LOCK_TT") & "8")
        End With

    End Sub

    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: load text for form labels etc
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call InitCommonText(Me, 6, 5)
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

    Public Sub UpdatePLCData(Optional ByVal pLink As ePLCLink = ePLCLink.None) Implements BSDForm.UpdatePLCData
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
        Dim nUB As Integer
        Dim bInit As Boolean = False

        If pLink = ePLCLink.None Then
            pLink = LinkIndex
        End If
        Select Case pLink

            Case ePLCLink.Zone
                'zone info
                nUB = msBoothData.GetUpperBound(0)
                If msBoothRef Is Nothing Then
                    bInit = True
                Else
                    If msBoothRef.GetUpperBound(0) <> nUB Then bInit = True
                End If
                If bInit Then
                    ReDim msBoothRef(nUB)
                    For i As Integer = 0 To nUB
                        msBoothRef(i) = (Not CInt(msBoothData(i))).ToString
                    Next
                End If

                For i As Integer = 0 To nUB
                    If msBoothData(i) <> msBoothRef(i) Then
                        Select Case i
                            Case eBooth.InterlockWd
                                Call mBSDCommon.InterlockChange(Me, msBoothData(i))
                            Case eBooth.ConveyorStatWd
                                'oPenConv.Color = 
                                mBSDCommon.ConveyorStatChange(Me, msBoothData(i))
                            Case eBooth.JobCountWd
                                lblJobCount.Text = msBoothData(i)
                            Case eBooth.EstopPBWd
                                Call mBSDCommon.EstopPBChange(Me, msBoothData(i), mnNUmEstops, gpbSOC)
                            Case eBooth.ManDoorWd
                                'mnManDoorState = CInt(msBoothData(i))
                                'Me.Refresh()
                            Case eBooth.LimitSwStatWd
                                Call mBSDCommon.ProxSwChange(Me, msBoothData(eBooth.LimitSwStatWd), _
                                    mnNumLimits)
                            Case eBooth.ConvSpeedWd
                                Call mBSDCommon.ConvSpeedConv(msBoothData(i), lblConvSpd, lblConvSpd2)
                            Case eBooth.IntrStatWd, _
                                eBooth.EntMuteCounter, eBooth.EntBeamBrokeCounts, eBooth.EntBeamMadeCounts, _
                                eBooth.ExitMuteCounter, eBooth.ExitBeamBrokeCounts, eBooth.ExitBeamMadeCounts
                                Call mBSDCommon.IntrusionStatusChange(Me, msBoothData)
                            Case eBooth.WaterHeaterStatWd
                                Call mBSDCommon.WaterHeaterStatusChange(Me, msBoothData(i))
                            Case Else
                                'Nothing
                        End Select
                    End If

                    msBoothRef(i) = msBoothData(i)

                Next

                '-----------------------------------------------------------------------------------------------
            Case ePLCLink.Robot
                'robot info
                If msRobotData Is Nothing Then Exit Sub
                nUB = msRobotData.GetUpperBound(0)
                If msRobotRef Is Nothing Then
                    bInit = True
                Else
                    If msRobotRef.GetUpperBound(0) <> nUB Then bInit = True
                End If
                If bInit Then
                    ReDim msRobotRef(nUB)
                    For i As Integer = 0 To nUB
                        msRobotRef(i) = (Not CInt(msRobotData(i))).ToString
                    Next
                End If

                '1 robot block of data
                Dim sRData(eRobot.WordsPerRobot - 1) As String
                Dim nOffSet As Integer = 0
                Dim bChangeFound As Boolean = False
                Dim nRobNum As Integer = 1

                'For each robot, call the update if data changed or the mouseover status changed
                For i As Integer = 0 To nUB
                    sRData(nOffSet) = msRobotData(i)
                    If msRobotData(i) <> msRobotRef(i) Then bChangeFound = True
                    If nOffSet = (eRobot.WordsPerRobot - 1) Then
                        nOffSet = 0
                        If bChangeFound Then
                            mBSDCommon.RobotChange(Me, nRobNum, sRData)
                        End If
                        nRobNum += 1
                        bChangeFound = False
                    Else
                        nOffSet += 1
                    End If

                    msRobotRef(i) = msRobotData(i)

                Next
            Case ePLCLink.Queue
                Call mBSDCommon.UpdateQueueData(Me, msQueueData, msQueueRef, mnPOST_DETECT_Q1_POS_IN_DATA)
        End Select

    End Sub

    Public Sub subUpdateSAData() Implements BSDForm.subUpdateSAData
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

    End Sub

    Friend Sub MakeCarMove() Implements BSDForm.MakeCarMove
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

    End Sub

    Friend Overloads Sub Show(ByVal StartData As String()) Implements BSDForm.Show
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

    Friend Sub PrivilegeChange(ByVal NewPrivilege As mDeclares.ePrivilege) _
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

#End Region

#Region " Events "

    Friend Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    Private Sub uctlBoothLeft_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
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

    End Sub
#End Region

    '********************************************************************************************
    'Description: The following functions are to display the style, option, color and carrior
    '             upon a radio button click for a better user interface. 
    '
    'Parameters: 
    'Returns:    
    '
    'Modification history:
    'NVSP 03/02/2017
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub ByColor_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByColor.CheckedChanged
        Dim nQueueIterator As Integer
        Dim strCompleteQueueData As String() = frmMain.GetCachedData(ePLCLink.myQueue)
        For Each Ctrl As Control In pnlMain.Controls
            If Ctrl.Name.StartsWith("Carrier") Then
                nQueueIterator = CInt(Ctrl.Name.Substring(7)) - 1
                Ctrl.Text = strCompleteQueueData(nQueueIterator * 15 + 2)
            End If
        Next
    End Sub

    Private Sub ByStyle_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByStyle.CheckedChanged
        Dim nQueueIterator As Integer
        Dim strCompleteQueueData As String() = frmMain.GetCachedData(ePLCLink.myQueue)
        For Each Ctrl As Control In pnlMain.Controls
            If Ctrl.Name.StartsWith("Carrier") Then
                nQueueIterator = CInt(Ctrl.Name.Substring(7)) - 1
                Ctrl.Text = strCompleteQueueData(nQueueIterator * 15 + 1)
            End If
        Next
    End Sub

    Private Sub ByOption_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByOption.CheckedChanged
        Dim nQueueIterator As Integer
        Dim strCompleteQueueData As String() = frmMain.GetCachedData(ePLCLink.myQueue)
        For Each Ctrl As Control In pnlMain.Controls
            If Ctrl.Name.StartsWith("Carrier") Then
                nQueueIterator = CInt(Ctrl.Name.Substring(7)) - 1
                Ctrl.Text = strCompleteQueueData(nQueueIterator * 15 + 3)
            End If
        Next
    End Sub

    Private Sub ByCarrior_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByCarrior.CheckedChanged
        Dim nQueueIterator As Integer
        Dim strCompleteQueueData As String() = frmMain.GetCachedData(ePLCLink.myQueue)
        For Each Ctrl As Control In pnlMain.Controls
            If Ctrl.Name.StartsWith("Carrier") Then
                nQueueIterator = CInt(Ctrl.Name.Substring(7)) - 1
                Ctrl.Text = strCompleteQueueData(nQueueIterator * 15 + 7)
            End If
        Next
    End Sub

    'NVSP 03/03/2017 Changes for inserting a job at queue location
    Private Sub cboInsertJobLocation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboInsertJobLocation.SelectedIndexChanged
        ''MsgBox(cboInsertJobLocation.SelectedIndex)

        frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditInProgress"
        Dim s(0) As String
        s = frmMain.mPLC.PLCData

        If s(0) = "0" Then
            InsertJobToMyQueue(sender, cboInsertJobLocation.SelectedIndex.ToString)
        Else
            MsgBox("Someone is already editing the queue, try again when they are done.")
        End If
    End Sub
End Class
