Friend Class uctlCCExtRight
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

    'mandoors have to be drawn in paint event below is needed to keep track of where
    Private Const mnDOORS As Integer = 2
    Private Const mnSILHOUETTES As Integer = 4
    Private Const mnSILHOUETTE_START_BIT As Integer = 8
    Private tDoors(mnDOORS) As tManDoor
    Private tSilhouettes(mnSILHOUETTES) As tManDoor
    Private mnManDoorState As Integer = 0
    'Intrusion Lines - maintain a copy of the pens
    Private oPenPEC(1) As Pen
    Dim tIntLines(1) As tLine
    'Conveyor line
    Private oPenConv As Pen = New Pen(Color.DarkGray, 6)
    Private Const mnNumEstops As Integer = 3
    Private Const mnNumLimits As Integer = 3
    Private Const mnNumMaintLocks As Integer = 0
    Private Const mnPOST_DETECT_Q1_POS_IN_DATA As Integer = 11

#End Region

#Region " Properties"

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
            Return "CCExtRight"
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

    Private Sub subCleanUpRobotLabels(ByVal rArm As clsArm) Implements BSDForm.subCleanUpRobotLabels
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

        'add car cartoons
        If colImages.Contains("imlJob") Then
            'could be from another zone
            colImages.Remove("imlJob")
        End If

        colImages.Add(imlJob, "imlJob")

        'Shapes and lines are a bit akward now.  We'll a store a pen setting for each light curtain, then use it 
        'when the form repaints
        For nLine As Integer = tIntLines.GetLowerBound(0) To tIntLines.GetUpperBound(0)
            tIntLines(nLine).Width = 2
            oPenPEC(nLine) = New Pen(Color.DarkGray, tIntLines(nLine).Width)
            oPenPEC(nLine).DashStyle = Drawing2D.DashStyle.DashDotDot
        Next

        tIntLines(0).StartPoint = New Point(picPECEntTop.Left + CInt(picPECEntTop.Width / 2), picPECEntTop.Top + picPECEntTop.Height)
        tIntLines(0).EndPoint = New Point(picPECEntBottom.Left + CInt(picPECEntBottom.Width / 2), picPECEntBottom.Top)
        tIntLines(1).StartPoint = New Point(picPECExitTop.Left + CInt(picPECExitTop.Width / 2), picPECExitTop.Top + picPECExitTop.Height)
        tIntLines(1).EndPoint = New Point(picPECExitBottom.Left + CInt(picPECExitBottom.Width / 2), picPECExitBottom.Top)

        mBSDCommon.TrackingScale = 0.5 '0.365
        mBSDCommon.TrackingStartPos = 26 '-159
        mBSDCommon.TrackingQueueCountsBeforeShift = 75

        Call subSetToolTips()

    End Sub

    Private Sub subDoManDoors(ByVal e As System.Windows.Forms.PaintEventArgs)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Add offset for scrollbars
        '********************************************************************************************
        Dim nScrollX As Integer = pnlMain.AutoScrollPosition.X
        Dim nScrollY As Integer = pnlMain.AutoScrollPosition.Y
        Dim oRed As New Pen(Color.Red, 4)
        Dim oGreen As New Pen(Color.LimeGreen, 4)
        Const nStart As Integer = 0
        'this is for the designer mode
        If gnBitVal Is Nothing Then Exit Sub

        For i As Integer = nStart To nStart + (mnDOORS - 1)
            Dim nDoor As Integer = (i - nStart) + 1 'cause we skip door 0
            If (mnManDoorState And gnBitVal(i)) = gnBitVal(i) Then
                'door closed
                e.Graphics.DrawLine(oGreen, tDoors(nDoor).Hinge.X + nScrollX, tDoors(nDoor).Hinge.Y + nScrollY, _
                                    tDoors(nDoor).Close.X + nScrollX, tDoors(nDoor).Close.Y + nScrollY)
            Else
                'door open
                e.Graphics.DrawLine(oRed, tDoors(nDoor).Hinge.X + nScrollX, tDoors(nDoor).Hinge.Y + nScrollY, _
                                    tDoors(nDoor).Open.X + nScrollX, tDoors(nDoor).Open.Y + nScrollY)
            End If
        Next

    End Sub

    Private Sub subDoSilhouettes(ByVal e As System.Windows.Forms.PaintEventArgs)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Add offset for scrollbars
        '********************************************************************************************
        Dim nScrollX As Integer = pnlMain.AutoScrollPosition.X
        Dim nScrollY As Integer = pnlMain.AutoScrollPosition.Y
        Dim oRed As New Pen(Color.Red, 4)
        Dim oGreen As New Pen(Color.LimeGreen, 4)
        Const nStart As Integer = mnSILHOUETTE_START_BIT
        'this is for the designer mode
        If gnBitVal Is Nothing Then Exit Sub

        For i As Integer = nStart To nStart + (mnSILHOUETTES - 1)
            Dim nSilhouette As Integer = (i - nStart) + 1 'cause we skip door 0
            If (mnManDoorState And gnBitVal(i)) = gnBitVal(i) Then
                'Silhouette OK
                e.Graphics.DrawLine(oGreen, tSilhouettes(nSilhouette).Hinge.X + nScrollX, tSilhouettes(nSilhouette).Hinge.Y + nScrollY, _
                                    tSilhouettes(nSilhouette).Close.X + nScrollX, tSilhouettes(nSilhouette).Close.Y + nScrollY)
            Else
                'Silhouette displaced
                e.Graphics.DrawLine(oRed, tSilhouettes(nSilhouette).Hinge.X + nScrollX, tSilhouettes(nSilhouette).Hinge.Y + nScrollY, _
                                    tSilhouettes(nSilhouette).Open.X + nScrollX, tSilhouettes(nSilhouette).Open.Y + nScrollY)
            End If
        Next
    End Sub

    Private Sub subDoIntrusionLines(ByVal e As System.Windows.Forms.PaintEventArgs)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Add offset for scrollbars
        '********************************************************************************************

        Dim nScrollX As Integer = pnlMain.AutoScrollPosition.X
        Dim nScrollY As Integer = pnlMain.AutoScrollPosition.Y
        'Manually draw the intrusion line between 
        For nLine As Integer = tIntLines.GetLowerBound(0) To tIntLines.GetUpperBound(0)
            'Dim nLine As Integer = 1
            If oPenPEC(nLine) Is Nothing Then Exit Sub
            e.Graphics.DrawLine(oPenPEC(nLine), tIntLines(nLine).StartPoint.X + nScrollX, tIntLines(nLine).StartPoint.Y + nScrollY, _
                                tIntLines(nLine).EndPoint.X + nScrollX, tIntLines(nLine).EndPoint.Y + nScrollY)
        Next

    End Sub

    Private Sub subDoConveyorLine(ByVal e As System.Windows.Forms.PaintEventArgs)
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

        Dim nPos As Integer = picArrow01.Top + CInt(picArrow01.Height / 2)
        e.Graphics.DrawLine(oPenConv, 0, nPos, Me.Width, nPos)

    End Sub

    Private Sub subSetUpManDoors()
        '********************************************************************************************
        'Description: set points to draw the lines to and from
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'head off exceptions
        If mnDOORS = 0 Then
            Exit Sub
        End If

        'manually set up doors
        For i As Integer = 1 To mnDOORS
            Dim oDoorLbl As Label = DirectCast(pnlMain.Controls("lblDoor" & i.ToString), Label)
            tDoors(i).Width = 4

            With oDoorLbl
                Select Case i
                    Case 1
                        'Hinge Lower Left
                        tDoors(i).Hinge = New Point(.Left + 2, .Bottom - 2)
                        'Close Lower Right
                        tDoors(i).Close = New Point(.Right - 2, .Bottom - 2)
                        'Open Upper Right
                        tDoors(i).Open = New Point(.Right - 2, .Top + 2)
                    Case 2
                        'Hinge Upper Left
                        tDoors(i).Hinge = New Point(.Left + 2, .Top + 2)
                        'Close Upper Right
                        tDoors(i).Close = New Point(.Right - 2, .Top + 2)
                        'Open Lower Right
                        tDoors(i).Open = New Point(.Right - 2, .Bottom - 2)
                End Select
            End With 'oDoorLbl
        Next 'i

    End Sub

    Private Sub subSetUpSilhouettes()
        '********************************************************************************************
        'Description: set points to draw the lines to and from
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'head off exceptions
        If mnSILHOUETTES = 0 Then
            Exit Sub
        End If

        'manually set up doors
        For i As Integer = 1 To mnSILHOUETTES
            Dim oGateLbl As Label = DirectCast(pnlMain.Controls("lblGate" & i.ToString), Label)
            tSilhouettes(i).Width = 4

            With oGateLbl
                Select Case i
                    Case 1, 3
                        'Hinge Upper Left
                        tSilhouettes(i).Hinge = New Point(.Left + 2, .Top + 2)
                        'Close Lower Left
                        tSilhouettes(i).Close = New Point(.Left + 2, .Bottom - 2)
                        'Open Lower Right
                        tSilhouettes(i).Open = New Point(.Right - 2, .Bottom - 2)
                    Case 2, 4
                        'Hinge Lower Left 
                        tSilhouettes(i).Hinge = New Point(.Left + 2, .Bottom - 2)
                        'Close Upper Left
                        tSilhouettes(i).Close = New Point(.Left + 2, .Top + 2)
                        'Open Upper Right
                        tSilhouettes(i).Open = New Point(.Right - 2, .Top + 2)
                End Select
            End With 'oGateLbl
        Next 'i

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
            .SetToolTip(picEstop01, gpsRM.GetString("psESTOP") & " #1")
            .SetToolTip(picEstop02, gpsRM.GetString("psESTOP") & " #2")
            '.SetToolTip(picMaintLock1, gpsRM.GetString("psMAINT_LOCK_TT") & "1")
            '.SetToolTip(picMaintLock2, gpsRM.GetString("psMAINT_LOCK_TT") & "2")
            '.SetToolTip(picMaintLock5, gpsRM.GetString("psMAINT_LOCK_TT") & "3")
            '.SetToolTip(picMaintLock6, gpsRM.GetString("psMAINT_LOCK_TT") & "4")
            '.SetToolTip(picMaintLock9, gpsRM.GetString("psMAINT_LOCK_TT") & "5")
            '.SetToolTip(picMaintLock10, gpsRM.GetString("psMAINT_LOCK_TT") & "6")
            '.SetToolTip(picMaintLock11, gpsRM.GetString("psMAINT_LOCK_TT") & "7")
            '.SetToolTip(picMaintLock12, gpsRM.GetString("psMAINT_LOCK_TT") & "8")
            .SetToolTip(picLimitSw0, gpsRM.GetString("psMUTE_SW_TT") & "1")
            .SetToolTip(picLimitSw1, gpsRM.GetString("psMUTE_SW_TT") & "3")
            .SetToolTip(picLimitSw2, gpsRM.GetString("psMUTE_SW_TT") & "2")
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

        Call InitCommonText(Me, 8, 0, mnuRobotContext, 6, mnuOnOff)

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
        Dim bUpdateIntBox As Boolean

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

                bUpdateIntBox = False

                For i As Integer = 0 To nUB
                    If msBoothData(i) <> msBoothRef(i) Then
                        Select Case i
                            Case eBooth.InterlockWd
                                Call mBSDCommon.InterlockChange(Me, msBoothData(i))
                            Case eBooth.ConveyorStatWd
                                oPenConv.Color = mBSDCommon.ConveyorStatChange(Me, msBoothData(i))
                                Me.Refresh() 'This draws intrusion, doors and conveyor line
                            Case eBooth.JobCountWd
                                lblJobCount.Text = msBoothData(i)
                            Case eBooth.EstopPBWd
                                Call mBSDCommon.EstopPBChange(Me, msBoothData(i), mnNumEstops, gpbSOC)
                            Case eBooth.ManDoorWd
                                mnManDoorState = CInt(msBoothData(i))
                                Me.Refresh() 'This draws intrusion, doors and conveyor line
                            Case eBooth.LimitSwStatWd
                                Call mBSDCommon.LimitSwChange(Me, msBoothData(eBooth.LimitSwStatWd), _
                                    mnNumLimits)
                            Case eBooth.ConvSpeedWd
                                Call mBSDCommon.ConvSpeedConv(msBoothData(i), lblConvSpd, lblConvSpd2)
                            Case eBooth.IntrStatWd, _
                                eBooth.EntMuteCounter, eBooth.EntBeamBrokeCounts, eBooth.EntBeamMadeCounts, _
                                eBooth.ExitMuteCounter, eBooth.ExitBeamBrokeCounts, eBooth.ExitBeamMadeCounts
                                Call mBSDCommon.IntrusionStatusChange(Me, msBoothData, oPenPEC)
                                Me.Refresh() 'This draws intrusion, doors and conveyor line
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
                        If mnMouseDrawnOverRobot <> mnMouseOverRobot Then
                            If (mnMouseOverRobot = nRobNum) Or (mnMouseDrawnOverRobot = nRobNum) Then
                                bChangeFound = True
                            End If
                        End If
                        If bChangeFound Then
                            mBSDCommon.RobotChange(Me, nRobNum, sRData, (mnMouseOverRobot = nRobNum))
                        End If
                        nRobNum += 1
                        bChangeFound = False
                    Else
                        nOffSet += 1
                    End If

                    msRobotRef(i) = msRobotData(i)

                Next
                mnMouseDrawnOverRobot = mnMouseOverRobot
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

        ' Add any initialization after the InitializeComponent() call.
        Call subSetUpManDoors()
        Call subSetUpSilhouettes()

    End Sub

    Private Sub uctlPrimeRight_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        '********************************************************************************************
        'Description:  Hijack the Paint event to update Line cartoons
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subDoManDoors(e)
        Call subDoSilhouettes(e)
        Call subDoIntrusionLines(e)
        Call subDoConveyorLine(e)

    End Sub

#End Region


End Class
