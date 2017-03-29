Friend Class uctlQueueScreenRight

    Implements BSDForm

#Region " Declares "


    Private msBoothData As String() = Nothing
    Private msRobotData As String() = Nothing
    Private msQueueData As String() = Nothing
    Private msBoothRef As String() = Nothing
    Private msRobotRef As String() = Nothing
    Private msQueueRef As String() = Nothing
    Private mbIsRemote As Boolean = False
    Private mnRobotIndex As Integer = 0
    Private mnLinkIndex As ePLCLink = ePLCLink.None
    Private msSAData As String() = Nothing

    Private Const mnNumEstops As Integer = 1
    Private Const mnNumLimits As Integer = 3
    Private Const mnPOST_DETECT_Q1_POS_IN_DATA As Integer = 11

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
            Return "QueueRight"
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

        Call subSetToolTips()

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
                nUB = UBound(msRobotData)
                If msRobotRef Is Nothing Then
                    bInit = True
                Else
                    If UBound(msRobotRef) <> nUB Then bInit = True
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

    Private Sub uctlQueueRight_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
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

End Class
