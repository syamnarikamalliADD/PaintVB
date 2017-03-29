Friend Class uctlLifeCycles
    Implements BSDForm

#Region " Declares "

    '**** Form Constants ****************************************
    Private Const mnGROUP_VALVES As Integer = 31
    Private Const mnSHARED_VALVES As Integer = 4
    Private Const mnCC_VALVES As Integer = 35 'group valves + shared valves
    Private Const mnDATA_WORDS As Integer = 66 'Change as needed to fit PLC data table
    Private Const mnVERTICAL_MARGIN As Integer = 3

    Private Const mnPRESET_LOWER_LIMIT As Integer = 0
    Private Const mnPRESET_UPPER_LIMIT As Integer = 2000000000
    Private Const mnTOLERANCE_LOWER_LIMIT As Integer = 0
    Private Const mnTOLERANCE_UPPER_LIMIT As Integer = 2000000
    '**** End Form Constants ************************************

    '**** Enumerations ******************************************
    Private Enum eLCData 'Change as needed to fit PLC data table
        CCValve1 = 0
        ColorValve1 = 36
        Regulator = 60
        HVHours = 61
        Pump1Liters = 62
        Pump2Liters = 63
    End Enum

    Private Enum eRWMode 'PLC Data Read/Write/Refresh mode
        All = 0
        Actual = 1
        Fault_Tol = 2
        Reset_All = 3
        Reset_Item = 4
    End Enum
    '**** End Enumerations **************************************

    '**** Structures ********************************************
    Private Structure udsLCItem
        Public Description As Label
        Public Current As Label
        Public Preset As FocusedTextBox.FocusedTextBox
        Public Tolerance As FocusedTextBox.FocusedTextBox
        Public Indicator As PictureBox
    End Structure
    '**** End Structures ****************************************

    '**** Form Variables ****************************************
    Private maCCValves(mnCC_VALVES - 1) As udsLCItem
    Private maColorValves() As udsLCItem
    Private mbMitsubishiPLC As Boolean
    Private mcboValves As New ComboBox
    Private mcolPainterArms As clsArms = Nothing
    Private mnSelectedItem As Integer
    Private mnValves As Integer
    Private moArm As clsArm = Nothing
    Private msCurrent(mnDATA_WORDS - 1) As String
    Private msFault(mnDATA_WORDS - 1) As String
    Private msTolerance(mnDATA_WORDS - 1) As String
    Private msSelectedItem As String
    Private mbUse2K As Boolean = False
    Private Const mnMaxHardener As Integer = 3
    Private Const mnStartHardener As Integer = 16
    '**** End Form Variables ************************************

    '**** Property Variables ************************************
    Private mbCheckPrivilege As Boolean
    Private mbEditsMade As Boolean
    Private mbIsRemote As Boolean
    Private mnLinkIndex As ePLCLink = ePLCLink.None
    Private mnRobotIndex As Integer
    Private mnSelectedRobotIndex As Integer
    Private msPlaceHolderData As String() = Nothing
    Private msSAData As String() = Nothing
    '**** End Property Variables ********************************

#End Region

#Region " Properties "

    Private Property EditsMade() As Boolean
        '********************************************************************************************
        'Description:  True = Unsaved edits have been made.
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbEditsMade
        End Get

        Set(ByVal value As Boolean)
            mbEditsMade = value
        End Set

    End Property

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
            Return "LifeCycles"
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

    Friend Property LinkIndex() As ePLCLink Implements BSDForm.LinkIndex
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
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

    Friend Property PLCData() As String() Implements BSDForm.PLCData
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles. 
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msPlaceHolderData
        End Get

        Set(ByVal value As String())
            If value Is Nothing Then Exit Property
            msPlaceHolderData = value
        End Set

    End Property

    Friend Property RobotIndex() As Integer Implements BSDForm.RobotIndex
        '********************************************************************************************
        'Description:  Required for "BSDForm" Interface. Not used for Life Cycles.
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
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
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

    Friend WriteOnly Property SelectedRobot() As String
        '********************************************************************************************
        'Description: The name of the selected robot arm.
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            Dim nRobotIndex As Integer

            For nRobotIndex = 0 To cboRobot.Items.Count - 1
                If cboRobot.Items(nRobotIndex) Is value Then Exit For
            Next

            moArm = mcolPainterArms.Item(nRobotIndex)
            SelectedRobotIndex = nRobotIndex

        End Set

    End Property

    Friend Property SelectedRobotIndex() As Integer
        '********************************************************************************************
        'Description: The index of the selected robot arm.
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnSelectedRobotIndex
        End Get

        Set(ByVal value As Integer)
            mnSelectedRobotIndex = value
            cboRobot.SelectedIndex = value
        End Set
    End Property
#End Region

#Region " Routines "

    Friend Sub Initialize(Optional ByVal sParam As String = "") Implements BSDForm.Initialize
        '********************************************************************************************
        'Description:  Set up the control for it's first use.
        '
        'Parameters:  sParam = Selected robot arm name
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mbMitsubishiPLC = (BSD.colZones.ActiveZone.PLCType = ePLCType.Mitsubishi)
        mcolPainterArms = mPWRobotCommon.LoadArmCollection(frmMain.colControllers, False)

        Call subInitFormText()

        Call mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, mcolPainterArms, False, , , False, False)

        Call subSetToolTips()

        Call subInitCCValves()
        Call subInitColorValves()

        'Set Tags for other LC items. This is used to determine which item was right-clicked.
        lblEstatAct1.Tag = eLCData.HVHours
        ftbEstatFlt1.Tag = eLCData.HVHours
        ftbEstatTol1.Tag = eLCData.HVHours

        lblPumpAct1.Tag = eLCData.Pump1Liters
        ftbPumpFlt1.Tag = eLCData.Pump1Liters
        ftbPumpTol1.Tag = eLCData.Pump1Liters

        lblPumpAct2.Tag = eLCData.Pump2Liters
        ftbPumpFlt2.Tag = eLCData.Pump2Liters
        ftbPumpTol2.Tag = eLCData.Pump2Liters

        lblRegAct1.Tag = eLCData.Regulator
        ftbRegFlt1.Tag = eLCData.Regulator
        ftbRegTol1.Tag = eLCData.Regulator

        SelectedRobot = sParam

        Call subEnableControls(frmMain.LoggedOnUser <> String.Empty)

    End Sub

    Public Sub InitPLCData() Implements BSDForm.InitPLCData
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
        '               
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub
    Public Sub UpdatePLCData(Optional ByVal pLink As ePLCLink = ePLCLink.None) Implements BSDForm.UpdatePLCData
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
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
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Friend Sub PrivilegeChange(ByVal NewPrivilege As mDeclares.ePrivilege) _
                                                   Implements BSDForm.PrivilegeChange
        '********************************************************************************************
        'Description: Password privilege changed - called from main form
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbCheckPrivilege Then Call subEnableControls(frmMain.LoggedOnUser <> String.Empty)

    End Sub

    Private Function sGetLEDColor(ByVal Index As Integer) As String
        '********************************************************************************************
        'Description: Returns LED Color based on Current, Warning and Fault values.
        '
        'Parameters: Index - LifeCycles data array pointer
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nFault As Integer = CType(msFault(Index), Integer)
        Dim nWarn As Integer = nFault - CType(msTolerance(Index), Integer)

        'Assume unassigned if Fault value is set to zero
        If nFault = 0 Then Return "gray"

        Select Case CType(msCurrent(Index), Integer)
            Case Is >= nFault
                Return "red"
            Case Is >= nWarn
                Return "yellow"
            Case Else
                Return "green"
        End Select

    End Function

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

        Me.Show()

    End Sub

    Private Sub subCheckLimits(ByRef oItem As udsLCItem)
        '********************************************************************************************
        'Description: Check the value of the PLC data for Fault and Tolerance and determine if they 
        '             are within the upper and lower limits. If not, adjust the values and write them
        '             to the Focused Text Box.
        '
        'Parameters:  oItem - Life Cycles Item structure
        'Returns:     oItem    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = CType(oItem.Preset.Tag, Integer)
        Dim nFaultValue As Integer = CType(msFault(nItem), Integer)
        Dim nTolValue As Integer = CType(msTolerance(nItem), Integer)
        Dim bInRange As Boolean
        Dim sMsg As String = String.Empty
        Dim sValue As String = String.Empty

        Select Case nFaultValue
            Case Is > mnPRESET_UPPER_LIMIT
                sMsg = oItem.Description.Text & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                       gpsRM.GetString("psLC_VAL_ABOVE_MAX")
                sValue = mnPRESET_UPPER_LIMIT.ToString
            Case Is < mnPRESET_LOWER_LIMIT
                sMsg = oItem.Description.Text & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                       gpsRM.GetString("psLC_VAL_BELOW_MIN")
                sValue = mnPRESET_LOWER_LIMIT.ToString
            Case Else
                bInRange = True
        End Select

        If Not bInRange Then
            MessageBox.Show(sMsg, gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            oItem.Preset.Text = sValue
            EditsMade = True
        End If

        bInRange = False

        Select Case nTolValue
            Case Is > mnTOLERANCE_UPPER_LIMIT
                sMsg = oItem.Description.Text & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                       gpsRM.GetString("psLC_VAL_ABOVE_MAX")
                sValue = mnTOLERANCE_UPPER_LIMIT.ToString
            Case Is < mnTOLERANCE_LOWER_LIMIT
                sMsg = oItem.Description.Text & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                       gpsRM.GetString("psLC_VAL_BELOW_MIN")
                sValue = mnTOLERANCE_LOWER_LIMIT.ToString
            Case Else
                bInRange = True
        End Select

        If Not bInRange Then
            MessageBox.Show(sMsg, gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            oItem.Tolerance.Text = sValue
            EditsMade = True
        End If

    End Sub

    Friend Sub subCleanUpRobotLabels(ByVal rArm As clsArm) Implements BSDForm.subCleanUpRobotLabels
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Private Sub subConfigureControls()
        '********************************************************************************************
        'Description: Show the appropriate controls for this robot. gpbEstatCable, gpbPump, all of  
        '             the controls for Pump2, and gpbRegulator are NOT visible by default.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case moArm.ColorChangeType
            Case eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL, _
                 eColorChangeType.VERSABELL2, eColorChangeType.VERSABELL2_32, _
                 eColorChangeType.VERSABELL2_PLUS, eColorChangeType.VERSABELL3, eColorChangeType.VERSABELL3P1000
                gpbEstatCable.Visible = True
                gpbPump.Visible = False
                gpbRegulator.Visible = False

            Case eColorChangeType.HONDA_WB, eColorChangeType.VERSABELL2_WB, _
                 eColorChangeType.VERSABELL2_PLUS_WB, eColorChangeType.VERSABELL2_2K_1PLUS1, _
                 eColorChangeType.VERSABELL2_2K_MULTIRESIN, eColorChangeType.VERSABELL_2K, _
                 eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL3_WB, eColorChangeType.VERSABELL3_DUAL_WB
                gpbEstatCable.Visible = True
                gpbRegulator.Visible = False
                gpbPump.Visible = False

            Case Else
                'If you don't see it here, add your color change type to this case statement

        End Select

    End Sub

    Private Sub subCopyArray(ByVal SourceArray() As String, ByRef DestArray() As String)
        '********************************************************************************************
        'Description: Copy the string values in the SourceArray to the DestArray. Assume DestArray is
        '             properly dimensioned.
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'TODO - Error handling
        Dim nSourceUBound As Integer = SourceArray.GetUpperBound(0)
        Dim nUBound As Integer = DestArray.GetUpperBound(0)

        For nIndex As Integer = 0 To nUBound
            If nIndex <= nSourceUBound Then
                DestArray(nIndex) = SourceArray(nIndex)
            Else
                DestArray(nIndex) = "0"
            End If
        Next

    End Sub

    Private Sub subEnableContextMenu(ByRef oMnu As ContextMenuStrip)
        '********************************************************************************************
        'Description: Enable the pop-up menus based on EditsMade status. 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        oMnu.Items("mnuSaveAll").Enabled = EditsMade

    End Sub

    Private Sub subEnableControls(ByVal Enable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls based on user having "Execute" privilege. 
        '
        'Parameters: Enable - True = Check Privilege, False = Just disable controls
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer

        'Block the PrivilegeChange event while we're doing this 
        mbCheckPrivilege = True

        If Enable Then
            Dim oPrivilegeMem As ePrivilege = frmMain.Privilege

            frmMain.Privilege = ePrivilege.Execute

            If frmMain.Privilege <> ePrivilege.Execute Then
                Enable = False
            End If

            frmMain.Privilege = oPrivilegeMem
        End If

        'Set Enabled on all controls individually so we don't loose control of
        'vertical slider(s)
        For nIndex = 0 To maCCValves.GetUpperBound(0)
            maCCValves(nIndex).Current.Enabled = Enable
            maCCValves(nIndex).Preset.Enabled = Enable
            maCCValves(nIndex).Tolerance.Enabled = Enable
        Next

        For nIndex = 0 To maColorValves.GetUpperBound(0)
            maColorValves(nIndex).Current.Enabled = Enable
            maColorValves(nIndex).Preset.Enabled = Enable
            maColorValves(nIndex).Tolerance.Enabled = Enable
        Next

        lblEstatAct1.Enabled = Enable
        ftbEstatFlt1.Enabled = Enable
        ftbEstatTol1.Enabled = Enable

        lblPumpAct1.Enabled = Enable
        ftbPumpFlt1.Enabled = Enable
        ftbPumpTol1.Enabled = Enable
        lblPumpAct2.Enabled = Enable
        ftbPumpFlt2.Enabled = Enable
        ftbPumpTol2.Enabled = Enable

        lblRegAct1.Enabled = Enable
        ftbRegFlt1.Enabled = Enable
        ftbRegTol1.Enabled = Enable

        'Unblock the PrivilegeChange event
        mbCheckPrivilege = False

    End Sub

    Private Sub subGetCCValveLabels()
        '********************************************************************************************
        'Description: Get Color Changer Valve labels from the selected robot's controller if possible,
        '             otherwise use generic names.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer

        If moArm.IsOnLine Then
            Dim sSharedValveNames(mnSHARED_VALVES) As String
            Dim sGroupValveNames(mnGROUP_VALVES) As String

            Call mCCCommon.subGetValveLabels(moArm, sSharedValveNames, sGroupValveNames, True)

            For nIndex = 0 To maCCValves.GetUpperBound(0)
                If nIndex < 4 Then
                    maCCValves(nIndex).Description.Text = sSharedValveNames(nIndex)
                Else
                    maCCValves(nIndex).Description.Text = sGroupValveNames(nIndex - 4)
                End If
            Next
        Else
            Dim sGroupValve As String = gpsRM.GetString("psLC_GROUP_VALVE")
            Dim sSharedValve As String = gpsRM.GetString("psLC_SHARED_VALVE")

            For nIndex = 0 To maCCValves.GetUpperBound(0)
                If nIndex < 4 Then
                    maCCValves(nIndex).Description.Text = sSharedValve & (nIndex + 1).ToString
                Else
                    maCCValves(nIndex).Description.Text = sGroupValve & (nIndex - 3).ToString
                End If
            Next
        End If

    End Sub

    Private Sub subGetColorValveLabels()
        '********************************************************************************************
        'Description: Get Color Valve labels from the selected robot's controller if possible, 
        '             otherwise use the names in the database.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If moArm.IsOnLine Then
            Dim oSysColors As clsSysColors = New clsSysColors(moArm)

            If oSysColors Is Nothing Then Exit Sub 'TODO - Should show message box then close screen


            For nIndex As Integer = 0 To mcboValves.Items.Count - 1
                Dim sDesc As String = mcboValves.Items(nIndex).ToString
                Dim sColorDesc As String = gpsRM.GetString("psUNASSIGNED")

                For Each oColor As clsSysColor In oSysColors

                    If oColor.Valve.Number.Value = nIndex + 1 Then
                        sColorDesc = " - " & oColor.DisplayName
                        Exit For
                    End If
                Next 'oColor

                maColorValves(nIndex).Description.Text = sDesc & sColorDesc
            Next
            mbUse2K = oSysColors.Use2K

        Else

            For nIndex As Integer = 0 To maColorValves.GetUpperBound(0)
                maColorValves(nIndex).Description.Text = mcboValves.Items(nIndex).ToString
            Next

            mbUse2K = False
        End If

    End Sub

    Private Sub subInitCCValves()
        '********************************************************************************************
        'Description: Populate the Color Changer Valves GroupBox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sDesc As String
        Dim sGroupValve As String = gpsRM.GetString("psLC_GROUP_VALVE")
        Dim sSharedValve As String = gpsRM.GetString("psLC_SHARED_VALVE")
        Dim nOffset As Integer = lblCCValve1.Top - lblCCValveAct1.Top

        For nValve As Integer = 1 To mnCC_VALVES
            Dim nTag As Integer = eLCData.CCValve1 + nValve - 1

            If nValve < 5 Then
                sDesc = sSharedValve & nValve.ToString
            Else
                sDesc = sGroupValve & (nValve - 4).ToString
            End If

            Select Case nValve
                Case 1
                    lblCCValve1.Text = sDesc
                    lblCCValve1.TextAlign = ContentAlignment.MiddleLeft
                    lblCCValveAct1.Tag = nTag
                    ftbCCValveFlt1.Tag = nTag
                    ftbCCValveTol1.Tag = nTag

                    With maCCValves(0)
                        .Description = lblCCValve1
                        .Current = lblCCValveAct1
                        .Preset = ftbCCValveFlt1
                        .Tolerance = ftbCCValveTol1
                        .Indicator = picCCValve1
                    End With

                Case Else
                    'Create new description label for additional valve
                    Dim oLabel1 As New Label

                    With oLabel1
                        .Font = lblCCValve1.Font
                        .BorderStyle = lblCCValve1.BorderStyle
                        .AutoSize = False
                        .Size = lblCCValve1.Size
                        .Name = "lblCCValve" & nValve.ToString
                        .TextAlign = lblCCValve1.TextAlign
                        .Text = sDesc
                        pnlCCValves.Controls.Add(oLabel1)
                        .Top = lblCCValveAct1.Top + ((nValve - 1) * (lblCCValveAct1.Height + mnVERTICAL_MARGIN)) + nOffset
                        .Left = lblCCValve1.Left
                        .Visible = True
                    End With

                    'Create new actual Life Cycle value label for additional valve
                    Dim oLabel2 As New Label

                    With oLabel2
                        .Font = lblCCValveAct1.Font
                        .BorderStyle = lblCCValveAct1.BorderStyle
                        .AutoSize = False
                        .Size = lblCCValveAct1.Size
                        .Name = "lblCCValveAct" & nValve.ToString
                        .ContextMenuStrip = mnuReset
                        .TextAlign = lblCCValveAct1.TextAlign
                        .Text = lblCCValveAct1.Text
                        pnlCCValves.Controls.Add(oLabel2)
                        .Top = oLabel1.Top - nOffset
                        .Left = lblCCValveAct1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oLabel2.MouseDown, AddressOf ActualLabel_MouseDown
                    End With

                    'Create new actual Life Cycle Fault value textbox for additional valve
                    Dim oTextBox1 As New FocusedTextBox.FocusedTextBox

                    With oTextBox1
                        .BorderStyle = ftbCCValveFlt1.BorderStyle
                        .AutoSize = False
                        .Font = ftbCCValveFlt1.Font
                        .Size = ftbCCValveFlt1.Size
                        .Name = "ftbCCValveFlt" & nValve.ToString
                        .NumericOnly = True
                        .MaxLength = 10
                        .ForeColor = Color.Black
                        .ContextMenuStrip = mnuReset
                        .TextAlign = ftbCCValveFlt1.TextAlign
                        .Text = ftbCCValveFlt1.Text
                        pnlCCValves.Controls.Add(oTextBox1)
                        .Top = oLabel1.Top - nOffset
                        .Left = ftbCCValveFlt1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oTextBox1.MouseDown, AddressOf Ftb_MouseDown
                        AddHandler oTextBox1.Validated, AddressOf Ftb_Validated
                        AddHandler oTextBox1.Validating, AddressOf Ftb_Validating
                    End With

                    'Create new actual Life Cycle Fault Tolerance value textbox for additional valve
                    Dim oTextBox2 As New FocusedTextBox.FocusedTextBox

                    With oTextBox2
                        .BorderStyle = ftbCCValveTol1.BorderStyle
                        .AutoSize = False
                        .Font = ftbCCValveTol1.Font
                        .Size = ftbCCValveTol1.Size
                        .Name = "ftbCCValveTol" & nValve.ToString
                        .NumericOnly = True
                        .MaxLength = 6
                        .ForeColor = Color.Black
                        .ContextMenuStrip = mnuReset
                        .TextAlign = ftbCCValveTol1.TextAlign
                        .Text = ftbCCValveTol1.Text
                        pnlCCValves.Controls.Add(oTextBox2)
                        .Top = oLabel1.Top - nOffset
                        .Left = ftbCCValveTol1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oTextBox2.MouseDown, AddressOf Ftb_MouseDown
                        AddHandler oTextBox2.Validated, AddressOf Ftb_Validated
                        AddHandler oTextBox2.Validating, AddressOf Ftb_Validating
                    End With

                    'Create new Life Cycle Status indicator for additional valve
                    Dim oPic As New PictureBox

                    With oPic
                        .Image = picCCValve1.Image
                        .Size = picCCValve1.Size
                        .SizeMode = picCCValve1.SizeMode
                        .Tag = picCCValve1.Tag
                        .Name = "picCCValve" & nValve.ToString
                        pnlCCValves.Controls.Add(oPic)
                        .Top = oLabel1.Top - nOffset
                        .Left = picCCValve1.Left
                        .Visible = True
                    End With

                    With maCCValves(nValve - 1)
                        .Description = oLabel1
                        .Current = oLabel2
                        .Preset = oTextBox1
                        .Tolerance = oTextBox2
                        .Indicator = oPic
                    End With

            End Select

        Next

    End Sub

    Private Sub subInitColorValves()
        '********************************************************************************************
        'Description: Populate the Color Valves GroupBox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/09/12 MSW change from nValve >= nHardenerStart to > 
        '********************************************************************************************
        Dim sColorValve As String = gpsRM.GetString("psLC_COLOR_VALVE")
        Dim sHardenerValve As String = gpsRM.GetString("psLC_HDR_VALVE")
        Dim bUse2K As Boolean = True
        Dim sVoid(0) As String
        Dim nOffset As Integer = lblColorValve1.Top - lblColorValveAct1.Top

        'Get a list of Color Valves for this zone
        If Not mSysColorCommon.LoadValveBoxFromDB(mcboValves, mBSDCommon.colZones, sVoid, False, , , bUse2K) Then
            Exit Sub 'TODO - Should show message box then close screen
        End If
        mbUse2K = bUse2K

        mnValves = mcboValves.Items.Count
        Dim nHardenerStart As Integer = mnValves
        If bUse2K Then
            mnValves = mnValves + mnMaxHardener
        End If
        ReDim maColorValves(mnValves - 1)

        For nValve As Integer = 1 To mnValves
            Dim nTag As Integer = eLCData.ColorValve1 + nValve - 1
            If nValve > nHardenerStart Then '11/09/12 MSW change from nValve >= nHardenerStart to > 
                nTag = nTag + mnStartHardener - nHardenerStart
            End If
            Select Case nValve
                Case 1
                    lblColorValve1.Text = sColorValve & nValve.ToString
                    lblColorValve1.TextAlign = ContentAlignment.MiddleLeft
                    lblColorValveAct1.Tag = nTag
                    ftbColorValveFlt1.Tag = nTag
                    ftbColorValveTol1.Tag = nTag

                    With maColorValves(0)
                        .Description = lblColorValve1
                        .Current = lblColorValveAct1
                        .Preset = ftbColorValveFlt1
                        .Tolerance = ftbColorValveTol1
                        .Indicator = picColorValve1
                    End With

                Case Else
                    'Create new labels and textboxes for each additional valve

                    'Create new description label for additional valve
                    Dim oLabel1 As New Label

                    With oLabel1
                        .Font = lblColorValve1.Font
                        .BorderStyle = lblColorValve1.BorderStyle
                        .AutoSize = False
                        .Size = lblColorValve1.Size
                        .Name = "lblColorValve" & nValve.ToString
                        .TextAlign = lblColorValve1.TextAlign
                        If bUse2K AndAlso nValve > nHardenerStart Then
                            .Text = sHardenerValve & (nValve - nHardenerStart).ToString
                        Else
                            .Text = sColorValve & nValve.ToString
                        End If
                        pnlColorValves.Controls.Add(oLabel1)
                        .Top = lblColorValveAct1.Top + ((nValve - 1) * (lblColorValveAct1.Height + mnVERTICAL_MARGIN)) + nOffset
                        .Left = lblColorValve1.Left
                        .Visible = True
                    End With

                    'Create new actual Life Cycle value label for additional valve
                    Dim oLabel2 As New Label

                    With oLabel2
                        .Font = lblColorValveAct1.Font
                        .BorderStyle = lblColorValveAct1.BorderStyle
                        .AutoSize = False
                        .Size = lblColorValveAct1.Size
                        .Name = "lblColorValveAct" & nValve.ToString
                        .ContextMenuStrip = mnuReset
                        .TextAlign = lblColorValveAct1.TextAlign
                        .Text = lblColorValveAct1.Text
                        pnlColorValves.Controls.Add(oLabel2)
                        .Top = oLabel1.Top - nOffset
                        .Left = lblColorValveAct1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oLabel2.MouseDown, AddressOf ActualLabel_MouseDown
                    End With

                    'Create new actual Life Cycle Fault value textbox for additional valve
                    Dim oTextBox1 As New FocusedTextBox.FocusedTextBox

                    With oTextBox1
                        .BorderStyle = ftbColorValveFlt1.BorderStyle
                        .AutoSize = False
                        .Font = ftbColorValveFlt1.Font
                        .Size = ftbColorValveFlt1.Size
                        .Name = "ftbColorValveFlt" & nValve.ToString
                        .NumericOnly = True
                        .MaxLength = 10
                        .ForeColor = Color.Black
                        .ContextMenuStrip = mnuReset
                        .TextAlign = ftbColorValveFlt1.TextAlign
                        .Text = ftbColorValveFlt1.Text
                        pnlColorValves.Controls.Add(oTextBox1)
                        .Top = oLabel1.Top - nOffset
                        .Left = ftbColorValveFlt1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oTextBox1.MouseDown, AddressOf Ftb_MouseDown
                        AddHandler oTextBox1.Validated, AddressOf Ftb_Validated
                        AddHandler oTextBox1.Validating, AddressOf Ftb_Validating
                    End With

                    'Create new actual Life Cycle Fault Tolerance value textbox for additional valve
                    Dim oTextBox2 As New FocusedTextBox.FocusedTextBox

                    With oTextBox2
                        .BorderStyle = ftbColorValveTol1.BorderStyle
                        .AutoSize = False
                        .Font = ftbColorValveTol1.Font
                        .Size = ftbColorValveTol1.Size
                        .Name = "ftbColorValveTol" & nValve.ToString
                        .NumericOnly = True
                        .MaxLength = 6
                        .ForeColor = Color.Black
                        .ContextMenuStrip = mnuReset
                        .TextAlign = ftbColorValveTol1.TextAlign
                        .Text = ftbColorValveTol1.Text
                        pnlColorValves.Controls.Add(oTextBox2)
                        .Top = oLabel1.Top - nOffset
                        .Left = ftbColorValveTol1.Left
                        .Tag = nTag
                        .Visible = True
                        AddHandler oTextBox2.MouseDown, AddressOf Ftb_MouseDown
                        AddHandler oTextBox2.Validated, AddressOf Ftb_Validated
                        AddHandler oTextBox2.Validating, AddressOf Ftb_Validating
                    End With

                    'Create new Life Cycle Status indicator for additional valve
                    Dim oPic As New PictureBox

                    With oPic
                        .Image = picColorValve1.Image
                        .Size = picColorValve1.Size
                        .SizeMode = picColorValve1.SizeMode
                        .Tag = picColorValve1.Tag
                        .Name = "picColorValve" & nValve.ToString
                        pnlColorValves.Controls.Add(oPic)
                        .Top = oLabel1.Top - nOffset
                        .Left = picColorValve1.Left
                        .Visible = True
                    End With

                    With maColorValves(nValve - 1)
                        .Description = oLabel1
                        .Current = oLabel2
                        .Preset = oTextBox1
                        .Tolerance = oTextBox2
                        .Indicator = oPic
                    End With

            End Select

        Next 'nValve

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
        Dim sTemp As String

        With gpsRM
            lblTitle.Text = .GetString("psLC_TITLE")

            gpbCCValves.Text = .GetString("psLC_CCV_GRP_CAP")
            gpbColorValves.Text = .GetString("psLC_COLV_GRP_CAP")
            gpbEstatCable.Text = .GetString("psLC_EST_GRP_CAP")
            gpbPump.Text = .GetString("psLC_PUMP_GRP_CAP")
            gpbRegulator.Text = .GetString("psLC_REG_GRP_CAP")

            sTemp = .GetString("psLC_ACT_CYC_CAP")
            lblCCValveActCap.Text = sTemp
            lblColorValveActCap.Text = sTemp
            lblEstatActCap.Text = .GetString("psLC_ACT_HR_CAP")
            lblPumpActCap.Text = .GetString("psLC_ACT_LTR_CAP")
            lblRegActCap.Text = sTemp

            sTemp = .GetString("psLC_FLT_CAP")
            lblCCValveFltCap.Text = sTemp
            lblColorValveFltCap.Text = sTemp
            lblEstatFltCap.Text = sTemp
            lblPumpFltCap.Text = sTemp
            lblRegFltCap.Text = sTemp

            sTemp = .GetString("psLC_TOL_CAP")
            lblCCValveTolCap.Text = sTemp
            lblColorValveTolCap.Text = sTemp
            lblEstatTolCap.Text = sTemp
            lblPumpTolCap.Text = sTemp
            lblRegTolCap.Text = sTemp

            lblPump1.Text = .GetString("psLC_PUMP1_CAP")
            lblPump2.Text = .GetString("psLC_PUMP2_CAP")

            'Context menu
            mnuReset.Items("mnuResetSingle").Text = .GetString("psLC_RESET_SINGLE")
            mnuReset.Items("mnuResetAll").Text = .GetString("psLC_RESET_ALL")
            mnuReset.Items("mnuSaveAll").Text = .GetString("psLC_SAVE_ALL")
        End With

        lblViewRobotCap.Text = gcsRM.GetString("csROBOT")

    End Sub

    Private Sub subNewRobotSelected()
        '********************************************************************************************
        'Description: Retrieve and display life cycle data for the selected robot arm.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subConfigureControls()

        'Get CC Valve labels
        Call subGetCCValveLabels()

        'Get Color Valve Labels
        Call subGetColorValveLabels()

        'Show data read from PLC
        Call subRefresh(eRWMode.All)

    End Sub

    Private Sub subReadLCDataFromPLC(ByVal Mode As eRWMode)
        '********************************************************************************************
        'Description: Read life cycle data for the selected robot arm and format for display.
        '
        'Parameters: Mode All, Actual only, Fault and Tolerance only
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = moArm.Controller.FanucName & "A" & moArm.ArmNumber.ToString
        Dim sData() As String

        With frmMain.mPLC

            If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                .TagName = sTag & "LifeCyclesCurrent"
                sData = .PLCData

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordFromMitsubishi(sData)

                    Call subCopyArray(sDWordData, msCurrent)
                Else
                    Call subCopyArray(sData, msCurrent)
                End If

                Application.DoEvents()
            End If

            If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                .TagName = sTag & "LifeCyclesFaultPreset"
                sData = .PLCData

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordFromMitsubishi(sData)

                    Call subCopyArray(sDWordData, msFault)
                Else
                    Call subCopyArray(sData, msFault)
                End If

                Application.DoEvents()

                .TagName = sTag & "LifeCyclesTolerance"
                sData = .PLCData

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordFromMitsubishi(sData)

                    Call subCopyArray(sDWordData, msTolerance)
                Else
                    Call subCopyArray(sData, msTolerance)
                End If

                For nIndex As Integer = 0 To mnDATA_WORDS - 1
                    Dim nPreset As Integer = CType(msFault(nIndex), Integer)
                    Dim nWarn As Integer = CType(msTolerance(nIndex), Integer)

                    msTolerance(nIndex) = (nPreset - nWarn).ToString
                Next
            End If

        End With

    End Sub

    Private Sub subRefresh(ByVal Mode As eRWMode)
        '********************************************************************************************
        'Description: Refresh the data displayed on this screen
        '
        'Parameters: Mode All, Actual only, Fault and Tolerance only
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer
        Dim oImagelist As ImageList = DirectCast(BSD.colImages.Item("imlLED"), ImageList)

        'Get Life Cycle data from PLC
        Call subReadLCDataFromPLC(Mode)

        'Display CC valve LC Data
        For nIndex = 0 To maCCValves.GetUpperBound(0)
            With maCCValves(nIndex)
                If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                    .Current.Text = msCurrent(eLCData.CCValve1 + nIndex)
                End If

                If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                    .Preset.Text = msFault(eLCData.CCValve1 + nIndex)
                    .Preset.ForeColor = Color.Black
                    .Tolerance.Text = msTolerance(eLCData.CCValve1 + nIndex)
                    .Tolerance.ForeColor = Color.Black
                    Call subCheckLimits(maCCValves(nIndex))
                End If

                .Indicator.Image = oImagelist.Images(sGetLEDColor(eLCData.CCValve1 + nIndex))
            End With
        Next

        'Display Color valve LC Data
        For nIndex = 0 To maColorValves.GetUpperBound(0)
            With maColorValves(nIndex)
                Dim nCVIndex As Integer = eLCData.ColorValve1 + nIndex
                If mbUse2K AndAlso nIndex > (maColorValves.GetUpperBound(0) - mnMaxHardener) Then
                    nCVIndex = eLCData.ColorValve1 + mnStartHardener + nIndex - (maColorValves.GetUpperBound(0) - mnMaxHardener) - 1
                End If
                If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                    .Current.Text = msCurrent(nCVIndex)
                End If

                If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                    .Preset.Text = msFault(nCVIndex)
                    .Preset.ForeColor = Color.Black
                    .Tolerance.Text = msTolerance(nCVIndex)
                    .Tolerance.ForeColor = Color.Black
                    Call subCheckLimits(maColorValves(nIndex))
                End If

                .Indicator.Image = oImagelist.Images(sGetLEDColor(nCVIndex))
            End With
        Next

        'Display Estat Cable LC Data
        If gpbEstatCable.Visible Then
            If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                lblEstatAct1.Text = msCurrent(eLCData.HVHours)
            End If

            If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                Dim oItem As udsLCItem
                Dim oLabel As New Label

                oLabel.Text = gpsRM.GetString("psLC_ESTAT_CABLE")
                ftbEstatFlt1.Text = msFault(eLCData.HVHours)
                ftbEstatFlt1.ForeColor = Color.Black
                ftbEstatTol1.Text = msTolerance(eLCData.HVHours)
                ftbEstatTol1.ForeColor = Color.Black

                With oItem
                    .Current = Nothing
                    .Description = oLabel
                    .Indicator = Nothing
                    .Preset = ftbEstatFlt1
                    .Tolerance = ftbEstatTol1
                End With

                Call subCheckLimits(oItem)
            End If

            picEstat1.Image = oImagelist.Images(sGetLEDColor(eLCData.HVHours))
        End If

        'Display Pump LC Data
        If gpbPump.Visible Then
            If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                lblPumpAct1.Text = msCurrent(eLCData.Pump1Liters)
            End If
            If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                Dim oItem As udsLCItem

                ftbPumpFlt1.Text = msFault(eLCData.Pump1Liters)
                ftbPumpFlt1.ForeColor = Color.Black
                ftbPumpTol1.Text = msTolerance(eLCData.Pump1Liters)
                ftbPumpTol1.ForeColor = Color.Black

                With oItem
                    .Current = Nothing
                    .Description = lblPump1
                    .Indicator = Nothing
                    .Preset = ftbPumpFlt1
                    .Tolerance = ftbPumpTol1
                End With

                Call subCheckLimits(oItem)
            End If

            picPump1.Image = oImagelist.Images(sGetLEDColor(eLCData.Pump1Liters))
            If lblPump2.Visible Then
                If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                    lblPumpAct2.Text = msCurrent(eLCData.Pump2Liters)
                End If

                If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                    Dim oItem As udsLCItem

                    ftbPumpFlt2.Text = msFault(eLCData.Pump2Liters)
                    ftbPumpFlt2.ForeColor = Color.Black
                    ftbPumpTol2.Text = msTolerance(eLCData.Pump2Liters)
                    ftbPumpTol2.ForeColor = Color.Black

                    With oItem
                        .Current = Nothing
                        .Description = lblPump2
                        .Indicator = Nothing
                        .Preset = ftbPumpFlt2
                        .Tolerance = ftbPumpTol2
                    End With

                    Call subCheckLimits(oItem)
                End If

                picPump2.Image = oImagelist.Images(sGetLEDColor(eLCData.Pump2Liters))
            End If
        End If

        'Display Regulator LC Data
        If gpbRegulator.Visible Then
            If Mode = eRWMode.All Or Mode = eRWMode.Actual Then
                lblRegAct1.Text = msCurrent(eLCData.Regulator)
            End If

            If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                Dim oItem As udsLCItem
                Dim oLabel As New Label

                oLabel.Text = gpsRM.GetString("psLC_REGULATOR")
                ftbRegFlt1.Text = msFault(eLCData.Regulator)
                ftbRegFlt1.ForeColor = Color.Black
                ftbRegTol1.Text = msTolerance(eLCData.Regulator)
                ftbRegTol1.ForeColor = Color.Black

                With oItem
                    .Current = Nothing
                    .Description = oLabel
                    .Indicator = Nothing
                    .Preset = ftbRegFlt1
                    .Tolerance = ftbRegTol1
                End With

                Call subCheckLimits(oItem)
            End If

            picReg1.Image = oImagelist.Images(sGetLEDColor(eLCData.Regulator))
        End If

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
        Dim sToolTip As String = gpsRM.GetString("psLC_GRP_TT")

        With ttHint
            .IsBalloon = True
            'Set hint Tooltips for group boxes
            .SetToolTip(gpbCCValves, sToolTip)
            .SetToolTip(gpbColorValves, sToolTip)
            .SetToolTip(gpbEstatCable, sToolTip)
            .SetToolTip(gpbPump, sToolTip)
            .SetToolTip(gpbRegulator, sToolTip)
        End With

    End Sub

    Public Sub subUpdateSAData() Implements BSDForm.subUpdateSAData
        '********************************************************************************************
        'Description: Required for "BSDForm" Interface. Not used for Life Cycles.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Private Sub subWriteLCDataToPLC(ByVal Mode As eRWMode, Optional ByVal nItem As Integer = 0)
        '********************************************************************************************
        'Description: Write changed life cycle data for the selected robot arm to the PLC.
        '
        'Parameters: Mode All, Actual only, Fault and Tolerance only
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = moArm.Controller.FanucName & "A" & moArm.ArmNumber.ToString

        With frmMain.mPLC
            If Mode = eRWMode.Reset_Item Then
                Dim sItem(0) As String
                sItem(0) = nItem.ToString
                .TagName = sTag & "LifeCyclesResetItem"

                .PLCData = sItem

                Application.DoEvents()
            End If
            
            If Mode = eRWMode.All Or Mode = eRWMode.Actual Or Mode = eRWMode.Reset_All Then

                .TagName = sTag & "LifeCyclesCurrent"

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordToMitsubishi(msCurrent)

                    .PLCData = sDWordData
                Else
                    .PLCData = msCurrent
                End If

                Application.DoEvents()
            End If

            If Mode = eRWMode.All Or Mode = eRWMode.Fault_Tol Then
                Dim sData(mnDATA_WORDS - 1) As String

                .TagName = sTag & "LifeCyclesFaultPreset"

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordToMitsubishi(msFault)

                    .PLCData = sDWordData
                Else
                    .PLCData = msFault
                End If

                Application.DoEvents()

                .TagName = sTag & "LifeCyclesTolerance"

                For nIndex As Integer = 0 To mnDATA_WORDS - 1
                    Dim nPreset As Integer = CType(msFault(nIndex), Integer)
                    Dim nWarn As Integer = CType(msTolerance(nIndex), Integer)

                    sData(nIndex) = (nPreset - nWarn).ToString
                Next

                If mbMitsubishiPLC Then
                    Dim sDWordData() As String = sDwordToMitsubishi(sData)

                    .PLCData = sDWordData
                Else
                    .PLCData = sData
                End If

            End If

        End With

    End Sub

#End Region

#Region " Events "

    Private Sub ActualLabel_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                                  Handles lblCCValveAct1.MouseDown, lblColorValveAct1.MouseDown, _
                                          lblEstatAct1.MouseDown, lblPumpAct1.MouseDown, _
                                          lblPumpAct2.MouseDown, lblRegAct1.MouseDown
        '********************************************************************************************
        'Description: The user has clicked an "Actual" label control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim oLabel As Label = DirectCast(sender, Label)

            mnSelectedItem = CType(oLabel.Tag, Integer)
            'Set msSelectedItem. Used for Change Log.
            Select Case Strings.Left(oLabel.Name.ToLower, 5)
                Case "lblcc"
                    msSelectedItem = maCCValves(mnSelectedItem).Description.Text & " " & _
                                     gpsRM.GetString("psLC_ACT_CYC_CAP")
                Case "lblCo"
                    msSelectedItem = maColorValves(mnSelectedItem - eLCData.ColorValve1).Description.Text & " " & _
                                     gpsRM.GetString("psLC_ACT_CYC_CAP")
                Case "lbles"
                    msSelectedItem = gpsRM.GetString("psLC_ESTAT_CABLE") & " " & _
                                     gpsRM.GetString("psLC_ACT_HR_CAP")
                Case "lblpu"
                    If Strings.Right(oLabel.Name, 1) = "1" Then
                        msSelectedItem = lblPump1.Text
                    Else
                        msSelectedItem = lblPump2.Text
                    End If
                    msSelectedItem = msSelectedItem & " " & gpsRM.GetString("psLC_ACT_LTR_CAP")
                Case "lblre"
                    msSelectedItem = gpsRM.GetString("psLC_REGULATOR") & " " & _
                                     gpsRM.GetString("psLC_ACT_CYC_CAP")
            End Select

            Call subEnableContextMenu(mnuReset)
        End If

    End Sub

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
        SelectedRobot = cboRobot.Items(cboRobot.SelectedIndex).ToString
        Call subNewRobotSelected()

    End Sub

    Private Sub Ftb_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                              Handles ftbCCValveFlt1.MouseDown, ftbCCValveTol1.MouseDown, _
                                      ftbColorValveFlt1.MouseDown, ftbColorValveTol1.MouseDown, _
                                      ftbEstatFlt1.MouseDown, ftbEstatTol1.MouseDown, ftbPumpFlt1.MouseDown, _
                                      ftbPumpTol1.MouseDown, ftbPumpFlt2.MouseDown, ftbPumpTol2.MouseDown, _
                                      ftbRegFlt1.MouseDown, ftbRegTol1.MouseDown
        '********************************************************************************************
        'Description: The user has clicked a "Fault" or "Tolerance" Focused TextBox control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)

            mnSelectedItem = CType(oFTB.Tag, Integer)
            Call subEnableContextMenu(mnuReset)
        End If

    End Sub

    Private Sub Ftb_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
                              ftbCCValveFlt1.Validated, ftbColorValveFlt1.Validated, _
                              ftbEstatFlt1.Validated, ftbPumpFlt1.Validated, ftbPumpFlt2.Validated, _
                              ftbRegFlt1.Validated, ftbCCValveTol1.Validated, ftbColorValveTol1.Validated, _
                              ftbEstatTol1.Validated, ftbPumpTol1.Validated, ftbPumpTol2.Validated, _
                              ftbRegTol1.Validated
        '********************************************************************************************
        'Description: Generic Validated routine for all Focused TextBoxes.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sOrgValue As String = String.Empty

        If Strings.InStr(oFTB.Name, "Flt") > 0 Then
            'Fault Preset
            sOrgValue = msFault(CType(oFTB.Tag, Integer))
        Else
            'Tolerance
            sOrgValue = msTolerance(CType(oFTB.Tag, Integer))
        End If

        If oFTB.Text = sOrgValue Then
            oFTB.ForeColor = Color.Black
        Else
            EditsMade = True
        End If

    End Sub

    Private Sub Ftb_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _
                               ftbCCValveFlt1.Validating, ftbColorValveFlt1.Validating, _
                               ftbEstatFlt1.Validating, ftbPumpFlt1.Validating, ftbPumpFlt2.Validating, _
                               ftbRegFlt1.Validating, ftbCCValveTol1.Validating, ftbColorValveTol1.Validating, _
                               ftbEstatTol1.Validating, ftbPumpTol1.Validating, ftbPumpTol2.Validating, _
                               ftbRegTol1.Validating
        '********************************************************************************************
        'Description: Generic Validating routine for all Focused TextBoxes.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)

        'if no edit priv and bad data this locks us up
        If frmMain.Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        'no value?
        If Strings.Len(oFTB.Text) = 0 Then
            MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            oFTB.Undo()
            e.Cancel = True
            Exit Sub
        End If

        'check limits
        Dim bInRange As Boolean
        Dim nValue As Integer = CType(oFTB.Text, Integer)
        Dim sAdjValue As String = String.Empty
        Dim sMsg As String = String.Empty

        If Strings.InStr(oFTB.Name, "Flt") > 0 Then
            'Fault Preset
            Select Case nValue
                Case Is > mnPRESET_UPPER_LIMIT
                    sMsg = gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                           gcsRM.GetString("csMAXIMUM_EQ") & mnPRESET_UPPER_LIMIT.ToString
                    sAdjValue = mnPRESET_UPPER_LIMIT.ToString
                Case Is < mnPRESET_LOWER_LIMIT
                    sMsg = gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                           gcsRM.GetString("csMINIMUM_EQ") & mnPRESET_LOWER_LIMIT.ToString
                    sAdjValue = mnPRESET_LOWER_LIMIT.ToString
                Case Else
                    bInRange = True
            End Select
        Else
            'Tolerance
            Select Case nValue
                Case Is > mnTOLERANCE_UPPER_LIMIT
                    sMsg = gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                           gcsRM.GetString("csMAXIMUM_EQ") & mnTOLERANCE_UPPER_LIMIT.ToString
                    sAdjValue = mnTOLERANCE_UPPER_LIMIT.ToString
                Case Is < mnTOLERANCE_LOWER_LIMIT
                    sMsg = gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                           gcsRM.GetString("csMINIMUM_EQ") & mnTOLERANCE_LOWER_LIMIT.ToString
                    sAdjValue = mnTOLERANCE_LOWER_LIMIT.ToString
                Case Else
                    bInRange = True
            End Select
        End If

        If Not bInRange Then
            MessageBox.Show(sMsg, gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            oFTB.Text = sAdjValue
        End If

    End Sub

    Private Sub mnuResetAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuResetAll.Click
        '********************************************************************************************
        'Description: The user has clicked "Reset All" on the Context Menu Strip
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Reply As DialogResult

        Reply = MessageBox.Show(gpsRM.GetString("psPLC_RESET_WARN"), gcsRM.GetString("csWARNING"), _
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
        If Reply = DialogResult.OK Then
            'Set all items in msCurrent to zero, write to PLC, then refresh Actual values
            For nIndex As Integer = 0 To msCurrent.GetUpperBound(0)
                msCurrent(nIndex) = "0"
            Next

            Call subWriteLCDataToPLC(eRWMode.Reset_All)
            Application.DoEvents()
            Call subReadLCDataFromPLC(eRWMode.Actual)
            Call subRefresh(eRWMode.Actual)

            'Record this action in change log
            Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                       BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                       gcsRM.GetString("csALL"), _
                                                       gpsRM.GetString("psLC_RESET_ALL"), _
                                                       BSD.colZones.CurrentZone)
            Call mPWCommon.SaveToChangeLog(colZones.ActiveZone)
        End If

    End Sub
    Private Sub mnuResetSingle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuResetSingle.Click
        '********************************************************************************************
        'Description: The user has clicked "Reset" on the Context Menu Strip
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/09/12  MSW     change from mnureset to mnuresetsingle
        '********************************************************************************************
        'Dim sData(0) As String
        'Dim sTag As String = "Z" & BSD.colZones.CurrentZoneNumber & "LifeCycle"
        ''Tell the PLC to set the selected item to zero, then refresh Actual values
        'With frmMain.mPLC
        '    .TagName = sTag & "RobotToReset"
        '    sData(0) = (SelectedRobotIndex + 1).ToString
        '    .PLCData = sData

        '    Application.DoEvents()

        '    .TagName = sTag & "ToReset"
        '    sData(0) = (mnSelectedItem + 1).ToString
        '    .PLCData = sData

        '    Application.DoEvents()
        'End With

        Call subWriteLCDataToPLC(eRWMode.Reset_Item, (mnSelectedItem + 1))
        Application.DoEvents()
        Call subReadLCDataFromPLC(eRWMode.Actual)
        Call subRefresh(eRWMode.Actual)

        'Record this action in change log
        Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                   BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                   msSelectedItem, _
                                                   gpsRM.GetString("psLC_RESET_SINGLE"), _
                                                   BSD.colZones.CurrentZone)
        Call mPWCommon.SaveToChangeLog(colZones.ActiveZone)
    End Sub

    Private Sub mnuSaveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveAll.Click
        '********************************************************************************************
        'Description: The user has clicked "Save All" on the Context Menu Strip
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nDataWord As Integer
        Dim nItem As Integer
        Dim sSelectedItem As String = String.Empty
        Dim sSpecific As String = String.Empty

        'Record each Item that changed in change log
        'CCValves
        For nItem = 0 To maCCValves.GetUpperBound(0)
            nDataWord = CType(maCCValves(nItem).Preset.Tag, Integer)
            sSelectedItem = maCCValves(nItem).Description.Text

            If maCCValves(nItem).Preset.Text <> msFault(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & maCCValves(nItem).Preset.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msFault(nDataWord) = maCCValves(nItem).Preset.Text
            End If

            If maCCValves(nItem).Tolerance.Text <> msTolerance(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & maCCValves(nItem).Tolerance.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msTolerance(nDataWord) = maCCValves(nItem).Tolerance.Text
            End If
        Next

        'ColorValves
        For nItem = 0 To maColorValves.GetUpperBound(0)
            nDataWord = CType(maColorValves(nItem).Preset.Tag, Integer)
            sSelectedItem = maColorValves(nItem).Description.Text

            If maColorValves(nItem).Preset.Text <> msFault(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & maColorValves(nItem).Preset.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msFault(nDataWord) = maColorValves(nItem).Preset.Text
            End If

            If maColorValves(nItem).Tolerance.Text <> msTolerance(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & maColorValves(nItem).Tolerance.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msTolerance(nDataWord) = maColorValves(nItem).Tolerance.Text
            End If
        Next

        'Estat Cable
        If gpbEstatCable.Visible Then
            nDataWord = CType(ftbEstatFlt1.Tag, Integer)
            sSelectedItem = gpsRM.GetString("psLC_ESTAT_CABLE")

            If ftbEstatFlt1.Text <> msFault(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbEstatFlt1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msFault(nDataWord) = ftbEstatFlt1.Text
            End If

            If ftbEstatTol1.Text <> msTolerance(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbEstatTol1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msTolerance(nDataWord) = ftbEstatTol1.Text
            End If
        End If

        'Pumps
        If gpbPump.Visible Then
            nDataWord = CType(ftbPumpFlt1.Tag, Integer)
            sSelectedItem = lblPump1.Text

            If ftbPumpFlt1.Text <> msFault(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbPumpFlt1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msFault(nDataWord) = ftbPumpFlt1.Text
            End If

            If ftbPumpTol1.Text <> msTolerance(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbPumpTol1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msTolerance(nDataWord) = ftbPumpTol1.Text
            End If

            If lblPump2.Visible Then
                nDataWord = CType(ftbPumpFlt2.Tag, Integer)
                sSelectedItem = lblPump2.Text

                If ftbPumpFlt2.Text <> msFault(nDataWord) Then
                    sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                                gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                                msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbPumpFlt2.Text
                    Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                               BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                               sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                    msFault(nDataWord) = ftbPumpFlt2.Text
                End If

                If ftbPumpTol2.Text <> msTolerance(nDataWord) Then
                    sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                                gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                                msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbPumpTol2.Text
                    Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                               BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                               sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                    msTolerance(nDataWord) = ftbPumpTol2.Text
                End If
            End If 'lblPump2.Visible
        End If 'gpbPump.Visible

        'Regulator
        If gpbRegulator.Visible Then
            nDataWord = CType(ftbRegFlt1.Tag, Integer)
            sSelectedItem = gpsRM.GetString("psLC_REGULATOR")

            If ftbRegFlt1.Text <> msFault(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_FLT_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msFault(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbRegFlt1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msFault(nDataWord) = ftbRegFlt1.Text
            End If

            If ftbRegTol1.Text <> msTolerance(nDataWord) Then
                sSpecific = gpsRM.GetString("psLC_TITLE") & " " & gpsRM.GetString("psLC_TOL_CAP") & " " & _
                            gcsRM.GetString("csVALUE") & " " & gcsRM.GetString("csCHANGED_FROM") & " " & _
                            msTolerance(nDataWord) & " " & gcsRM.GetString("csTO") & " " & ftbRegTol1.Text
                Call mPWCommon.AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                           BSD.colZones.CurrentZoneNumber, moArm.Name, _
                                                           sSelectedItem, sSpecific, BSD.colZones.CurrentZone)
                msTolerance(nDataWord) = ftbRegTol1.Text
            End If
        End If 'gpbRegulator.Visible

        'Write to PLC
        Call subWriteLCDataToPLC(eRWMode.Fault_Tol)
        'Read from PLC
        Call subReadLCDataFromPLC(eRWMode.Fault_Tol)
        'Refresh
        Call subRefresh(eRWMode.Fault_Tol)
        'Update the change log
        Call mPWCommon.SaveToChangeLog(BSD.colZones.ActiveZone)

        EditsMade = False

    End Sub

#End Region

#Region " Mitsubishi PLC Specific Functions "

    Private Function sDwordFromMitsubishi(ByVal PLCData() As String) As String()
        '********************************************************************************************
        'Description: Registers used as DWords in the Misubishi PLC are not read as DWords by the
        '             ActEasyIF control. Each Dword is read as 2 unsigned integers, Least significant 
        '             first - Most significant second.
        '
        'Parameters: DWord Data as read from the PLC
        'Returns:    DWord values
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sReturnData(0) As String
        Dim nOutputDataLength As Integer = PLCData.GetLength(0) \ 2

        If nOutputDataLength = 0 Then
            sReturnData(0) = "0"
            Return sReturnData
        Else
            ReDim sReturnData(nOutputDataLength - 1)
            Dim nOffset As Integer
            Dim lDword As Long
            Dim lTemp As Long

            For nIndex As Integer = 0 To nOutputDataLength - 1
                lDword = 0

                'Least significant word
                Try
                    lDword = CType(PLCData(nOffset), Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                nOffset += 1
                lTemp = 0

                'Most significant word
                Try
                    lTemp = CType(PLCData(nOffset), Long) * CType(2 ^ 16, Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                lDword += lTemp
                sReturnData(nIndex) = lDword.ToString
                nOffset += 1
            Next

            Return sReturnData

        End If

    End Function

    Private Function sDwordToMitsubishi(ByVal DwordData() As String) As String()
        '********************************************************************************************
        'Description: Registers used as DWords in the Misubishi PLC are not stored as DWords by the
        '             ActEasyIF control. Each Dword is stored as 2 unsigned integers, Least significant 
        '             first - Most significant second.
        '
        'Parameters: DWord values
        'Returns:    DWord Data formatted to be written to the PLC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sReturnData(0) As String
        Dim nOutputDataLength As Integer = DwordData.GetLength(0) * 2

        If nOutputDataLength = 0 Then
            sReturnData(0) = "0"
            Return sReturnData
        Else
            ReDim sReturnData(nOutputDataLength - 1)
            Dim nOffset As Integer
            Dim lDword As Long
            Dim lTemp As Long

            For nIndex As Integer = 0 To DwordData.GetUpperBound(0)
                lDword = 0

                Try
                    lDword = CType(DwordData(nIndex), Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                lTemp = lDword And &HFFFF
                sReturnData(nOffset) = lTemp.ToString
                nOffset += 1
                sReturnData(nOffset) = ((lDword - lTemp) \ CType(2 ^ 16, Long)).ToString
                nOffset += 1
            Next

            Return sReturnData

        End If

    End Function
#End Region


End Class
