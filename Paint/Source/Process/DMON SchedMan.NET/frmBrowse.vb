Imports System.Windows.Forms

Public Class frmBrowse
    Private mbAxisSetup As Boolean = False
    Private mbVariableSetup As Boolean = False
    Private mbIOSetup As Boolean = False
    Private mbRegisterSetup As Boolean = False
    Private moDmon As clsDMONCfg.tDmonItem = Nothing
    Private moDmonCfg As clsDMONCfg = Nothing
    Private moController As clsController = Nothing
    Private moRobot As FRRobot.FRCRobot = Nothing
    Private mnMaxGroup As Integer = 0
    Private mnMaxAxes() As Integer = Nothing
    Private mnodeSystem As TreeNode = Nothing
    Private mnodePrograms As TreeNode = Nothing
    Private moSysVars As FRRobot.FRCVars = Nothing
    Private moPrograms As FRRobot.FRCPrograms = Nothing
    Private Const msPROGRAMS As String = "PROGRAMS"
    Private Const msPROGRAMSTAG As String = "*PROGRAMS*"
    Private Const msSYSTEM As String = "*SYSTEM*"
    Private moType As clsDMONCfg.eTYPE = clsDMONCfg.eTYPE.None
    Private Const msMODULE As String = "frmBrowse"
    Private msFile As String = String.Empty
    Private msVar As String = String.Empty
    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: No fancy password stuff, just disable while loading from the robot controller
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tabMain.Enabled = bEnable
        btnOK.Enabled = bEnable
        btnCancel.Enabled = bEnable
    End Sub
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description: Accept the settings
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case tabMain.SelectedTab.Name
            Case TabVar.Name
                If moType <> clsDMONCfg.eTYPE.None Then
                    moDmon.TYPE.Value = moType
                    moDmon.PRG_NAME.Text = msFile
                    moDmon.VAR_NAME.Text = msVar
                    Me.DialogResult = Windows.Forms.DialogResult.OK
                Else
                    MessageBox.Show(gpsRM.GetString("psINV_VAR_SEL"), gpsRM.GetString("psINV_VAR_SEL_CAP"), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Case tabIO.Name
                moDmon.TYPE.Value = clsDMONCfg.eTYPE.IO
                If cboIOType.SelectedIndex >= 0 And _
                    lstIO.SelectedIndex >= 0 Then
                    Dim nTag() As Integer = CType(cboIOType.Tag, Integer())
                    moDmon.IO_TYPE.Value = nTag(cboIOType.SelectedIndex)
                    Dim nPort() As Integer = CType(lstIO.Tag, Integer())
                    moDmon.PORT_NUM.Value = nPort(lstIO.SelectedIndex)
                    Me.DialogResult = Windows.Forms.DialogResult.OK
                Else
                    MessageBox.Show(gpsRM.GetString("psINC_SEL"), gpsRM.GetString("psINC_SEL_CAP"), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Case tabReg.Name
                If lstReg.SelectedIndex >= 0 Then
                    moDmon.TYPE.Value = clsDMONCfg.eTYPE.Register
                    moDmon.PORT_NUM.Value = lstReg.SelectedIndex + 1
                    Me.DialogResult = Windows.Forms.DialogResult.OK
                Else
                    MessageBox.Show(gpsRM.GetString("psINC_SEL"), gpsRM.GetString("psINC_SEL_CAP"), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Case tabAxis.Name
                moDmon.TYPE.Value = clsDMONCfg.eTYPE.Axis
                moDmon.GROUP_NUM.Value = CType(numudGroup.Value, Integer)
                moDmon.AXIS_NUM.Value = CType(numudAxis.Value, Integer)
                Me.DialogResult = Windows.Forms.DialogResult.OK
        End Select
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub frmBrowse_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Init the form
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnOK.Text = gcsRM.GetString("csOK")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        btnVar.Text = gpsRM.GetString("psFIND")
        TabVar.Text = gpsRM.GetString("psVAR_TAB")
        tabIO.Text = gpsRM.GetString("psIO_TAB")
        tabReg.Text = gpsRM.GetString("psREG_TAB")
        tabAxis.Text = gpsRM.GetString("psAXIS_TAB")
        Me.Show()
        Select Case moDmon.TYPE.Value
            Case clsDMONCfg.eTYPE.Int, clsDMONCfg.eTYPE.Real
                tabMain.SelectedTab = TabVar
                subInitVariable()
                mbVariableSetup = True
            Case clsDMONCfg.eTYPE.IO
                tabMain.SelectedTab = tabIO
                subInitIO()
                mbIOSetup = True
            Case clsDMONCfg.eTYPE.Register
                tabMain.SelectedTab = tabReg
                subInitRegister()
                mbRegisterSetup = True
            Case clsDMONCfg.eTYPE.Axis
                tabMain.SelectedTab = tabAxis
                subInitAxis()
                mbAxisSetup = True
            Case Else
                tabMain.SelectedTab = TabVar
                subInitVariable()
                mbVariableSetup = True
        End Select
    End Sub

    Private Sub subInitVariable()
        '********************************************************************************************
        'Description: setup the IO tab
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Init treeview
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            lblVarDetail1.Text = String.Empty
            lblVarDetail2.Text = String.Empty
            trvwVar.Nodes.Clear()
            mnodeSystem = New TreeNode(msSYSTEM)
            mnodeSystem.Tag = msSYSTEM
            trvwVar.Nodes.Add(mnodeSystem)
            moSysVars = moRobot.SysVariables
            For nSysVar As Integer = 0 To moSysVars.Count - 1
                If TypeOf (moSysVars.Item(, nSysVar)) Is FRRobot.FRCVars Then
                    Try
                        Dim oVars As FRRobot.FRCVars = DirectCast(moSysVars.Item(, nSysVar), FRRobot.FRCVars)
                        If oVars IsNot Nothing Then
                            Dim newNode As New TreeNode(oVars.FieldName)
                            newNode.Tag = oVars.VarName
                            mnodeSystem.Nodes.Add(newNode)
                        End If
                    Catch ex As Exception
                    End Try
                ElseIf TypeOf (moSysVars.Item(, nSysVar)) Is FRRobot.FRCVar Then
                    Try
                        Dim oVar As FRRobot.FRCVar = DirectCast(moSysVars.Item(, nSysVar), FRRobot.FRCVar)
                        If oVar IsNot Nothing Then
                            Dim newNode As New TreeNode(oVar.FieldName)
                            newNode.Tag = oVar.VarName
                            mnodeSystem.Nodes.Add(newNode)
                        End If
                    Catch ex As Exception
                    End Try
                Else
                    Debug.Print("???")
                End If
            Next
            mnodePrograms = New TreeNode(msPROGRAMS)
            mnodePrograms.Tag = msPROGRAMSTAG
            trvwVar.Nodes.Add(mnodePrograms)
            moPrograms = moRobot.Programs
            For Each oProg As FRRobot.FRCProgram In moPrograms
                If oProg IsNot Nothing Then
                    Dim oVars As FRRobot.FRCVars = oProg.Variables
                    If oVars IsNot Nothing AndAlso oVars.Count > 0 Then
                        Dim newNode As New TreeNode(oProg.Name)
                        newNode.Tag = "[" & oProg.Name & "]"
                        mnodePrograms.Nodes.Add(newNode)
                    End If
                End If
            Next
            Dim sProg As String = moDmon.PRG_NAME.Text.ToUpper
            Dim sVar As String = moDmon.VAR_NAME.Text.ToUpper
            Dim nDollarSign As Integer = InStr(sVar, "$")
            If (nDollarSign = 1) Or sProg = msSYSTEM Then
                txtVar.Text = sVar
            Else
                txtVar.Text = "[" & sProg & "]" & sVar
            End If
            subFindVar()
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psVAL_ERR2") & ex.Message, _
                    gpsRM.GetString("psVAL_CAP"), _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnValidate_Click - Var", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        subEnableControls(True)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub subFindVar()
        '********************************************************************************************
        'Description: Find a variable in the treeview
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sText As String = txtVar.Text.ToUpper
        Dim nDollarSign As Integer = InStr(sText, "$")
        Dim nIndex As Integer = 0
        Dim oNode As TreeNode = Nothing
        Dim nNode As Integer = -1
        Dim sTmpTag1 As String = String.Empty
        Dim bProgramSeach As Boolean = False
        If nDollarSign > 0 Then
            'System variable
            If nDollarSign > 1 Then
                sText = sText.Substring(nDollarSign)
            End If
            sTmpTag1 = msSYSTEM
        Else
            'Karel Variable
            sTmpTag1 = msPROGRAMSTAG
            bProgramSeach = True

            Dim nBracket As Integer = InStr(sText, "[")
            If nBracket = 1 Then
                'Typical Karel var notation [prog]var
                'Reformat a bit to fit in search logic
                sText = sText.Substring(1)
                nBracket = InStr(sText, "]")
                sText = sText.Substring(0, nBracket - 1) & "." & sText.Substring(nBracket)
            End If
        End If
        For Each oTmpNode1 As TreeNode In trvwVar.Nodes
            Dim sTmpTag2 As String = DirectCast(oTmpNode1.Tag, String)
            If sTmpTag1 = sTmpTag2 Then
                oNode = oTmpNode1
                nNode = oTmpNode1.Index
                trvwVar.SelectedNode = oNode
                Exit For
            End If
        Next
        trvwVar.SelectedNode = oNode
        Dim sTextSplit() As String = Split(sText, ".")
        Dim bContinue As Boolean = False
        For Each sTmp As String In sTextSplit
            bContinue = False
            'check for brackets:
            Dim nBracket As Integer = InStr(sTmp, "[")
            Dim sTmp1 As String = sTmp
            Dim sIndex As String = String.Empty
            If bProgramSeach Then
                'Find a karel program - strip brackets from search string
                sTmp1.Replace("[", "")
                sTmp1.Replace("]", "")
            Else
                'variable name
                If nBracket > 0 Then
                    'Find an array
                    sTmp1 = sTmp.Substring(0, nBracket - 1)
                    sIndex = sTmp.Substring(nBracket, sTmp.Length - (nBracket + 1))
                End If
            End If
            For Each oTmpNode2 As TreeNode In oNode.Nodes
                If oTmpNode2.Text.ToUpper = sTmp1 Then
                    oNode = oTmpNode2
                    nNode = oTmpNode2.Index
                    trvwVar.SelectedNode = oNode
                    bContinue = True
                    Exit For
                End If
            Next
            'Special case for hidden programs
            If bProgramSeach Then
                bProgramSeach = False
                If bContinue = False Then
                    Dim oProg As FRRobot.FRCProgram = directcast(moRobot.Programs(sTmp1), FRRobot.FRCProgram)
                    If oProg IsNot Nothing Then
                        Dim oVars As FRRobot.FRCVars = oProg.Variables
                        If oVars IsNot Nothing AndAlso oVars.Count > 0 Then
                            Dim newNode As New TreeNode(oProg.Name)
                            newNode.Tag = "[" & oProg.Name & "]"
                            mnodePrograms.Nodes.Add(newNode)
                            oNode = newNode
                            nNode = newNode.Index
                            trvwVar.SelectedNode = oNode
                            bContinue = True
                        End If
                    End If
                End If
            End If
            'Extra round for arrays
            If bContinue And sIndex <> String.Empty Then
                bContinue = False
                For Each oTmpNode3 As TreeNode In oNode.Nodes
                    If oTmpNode3.Text = sIndex Then
                        oNode = oTmpNode3
                        nNode = oTmpNode3.Index
                        trvwVar.SelectedNode = oNode
                        bContinue = True
                        Exit For
                    End If
                Next
            End If

            If bContinue = False Then
                Exit For
            End If
        Next
    End Sub
    Private Sub btnVar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVar.Click
        '********************************************************************************************
        'Description: Find a variable in the treeview
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subFindVar()
    End Sub
    Private Sub subInitIO()
        '********************************************************************************************
        'Description: setup the IO tab
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Robot IO lookup
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            moDmonCfg.LoadIO_TYPEBox(cboIOType)
            If moDmon.IO_TYPE.Value > 0 Then
                frmMain.subSetCbo(cboIOType, moDmon.IO_TYPE)
            End If
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psVAL_IO_TYPE_VAL_ERR") & ex.Message, gpsRM.GetString("psVAL_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        subEnableControls(True)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub subInitRegister()
        '********************************************************************************************
        'Description: setup the register tab
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Robot Register lookup
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Dim oRegs As FRRobot.FRCVars = moRobot.RegNumerics
            Dim nNumRegs As Integer = oRegs.Count
            Dim sRegs(nNumRegs - 1) As String
            For nReg As Integer = 1 To nNumRegs
                Dim oRegVar As FRRobot.FRCVar = DirectCast(oRegs(nReg.ToString), FRRobot.FRCVar)
                Dim oRegReg As FRRobot.FRCRegNumeric = DirectCast(oRegVar.Value, FRRobot.FRCRegNumeric)
                sRegs(nReg - 1) = String.Format(gpsRM.GetString("psREG_CMT"), nReg.ToString, oRegReg.Comment)
            Next
            'load the listbox
            lstReg.Items.Clear()
            lstReg.Items.AddRange(sRegs)
            Dim nPort As Integer = moDmon.PORT_NUM.Value
            If (moDmon.TYPE.Value = CType(clsDMONCfg.eTYPE.Register, Integer)) And _
                (nPort > 0) And (nPort <= nNumRegs) Then
                lstReg.SelectedIndex = nPort - 1
            End If
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psBROWSE_REG_ERR") & ex.Message, gpsRM.GetString("psBROWSE_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        subEnableControls(True)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub subInitAxis()
        '********************************************************************************************
        'Description: setup the axis tab
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Robot Axis Range Check
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Dim oStruct As FRRobot.FRCVars = DirectCast(moRobot.SysVariables.Item("$SCR"), FRRobot.FRCVars)
            Dim oFRCVar As FRRobot.FRCVar = DirectCast(oStruct.Item("$NUM_GROUP"), FRRobot.FRCVar)
            mnMaxGroup = CType(oFRCVar.Value, Integer)
            ReDim mnMaxAxes(mnMaxGroup - 1)
            oStruct = DirectCast(moRobot.SysVariables.Item("$SCR_GRP"), FRRobot.FRCVars)
            For nGroup As Integer = 1 To mnMaxGroup
                Dim oStruct2 As FRRobot.FRCVars = DirectCast(oStruct.Item(nGroup.ToString), FRRobot.FRCVars)
                oFRCVar = DirectCast(oStruct2.Item("$NUM_AXES"), FRRobot.FRCVar)
                mnMaxAxes(nGroup - 1) = CType(oFRCVar.Value, Integer)
            Next
            numudGroup.Minimum = 1
            numudGroup.Minimum = mnMaxGroup
            Dim nTmp As Integer = moDmon.GROUP_NUM.Value
            If nTmp > mnMaxGroup Then
                nTmp = mnMaxGroup
            ElseIf nTmp < 1 Then
                nTmp = 1
            End If
            numudGroup.Minimum = 1
            numudGroup.Maximum = mnMaxGroup
            numudGroup.Value = nTmp
            nTmp = moDmon.AXIS_NUM.Value
            Dim nGroupTmp As Integer = CType(numudGroup.Value, Integer)
            If nTmp > mnMaxAxes(nGroupTmp - 1) Then
                nTmp = mnMaxAxes(nGroupTmp - 1)
            ElseIf nTmp < 1 Then
                nTmp = 1
            End If
            numudAxis.Minimum = 1
            numudAxis.Maximum = mnMaxAxes(nGroupTmp - 1)
            numudAxis.Value = nTmp

        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psBROWSE_AXIS_ERR") & ex.Message, gpsRM.GetString("psBROWSE_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        subEnableControls(True)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Friend Sub InitBrowseForm(ByRef oDmonCfg As clsDMONCfg, ByRef oDmon As clsDMONCfg.tDmonItem, _
                              ByRef oController As clsController)
        '********************************************************************************************
        'Description: setup the browse form
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moController = oController
        moRobot = moController.Robot
        moDmon = oDmon
        moDmonCfg = oDmonCfg
        mbAxisSetup = False
        mbVariableSetup = False
        mbIOSetup = False
        mbRegisterSetup = False
    End Sub

    Private Sub numudGroup_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles numudGroup.ValueChanged
        '********************************************************************************************
        'Description: setup update axis limits for the new group
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Robot Axis Range Check
        Try

            Dim nGroupTmp As Integer = CType(numudGroup.Value, Integer)
            If nGroupTmp > mnMaxGroup Then
                nGroupTmp = mnMaxGroup
            ElseIf nGroupTmp < 1 Then
                nGroupTmp = 1
            End If
            Dim nAxisTmp As Integer = CType(numudAxis.Value, Integer)
            numudAxis.Minimum = 1
            numudAxis.Maximum = mnMaxAxes(nGroupTmp - 1)
            'If the axis is out of bounds for the new group, adjust it
            If nAxisTmp > mnMaxAxes(nGroupTmp - 1) Then
                numudAxis.Value = mnMaxAxes(nGroupTmp - 1)
            ElseIf nAxisTmp < 1 Then
                numudAxis.Value = 1
            End If
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psBROWSE_AXIS_ERR") & ex.Message, gpsRM.GetString("psBROWSE_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub cboIOType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboIOType.SelectedIndexChanged
        '********************************************************************************************
        'Description: setup the IO tab
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Robot IO lookup

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        subEnableControls(False)
        Try

            Dim nOldPort As Integer = moDmon.PORT_NUM.Value
            If lstIO.SelectedIndex > 0 Then
                nOldPort = lstIO.SelectedIndex
            End If
            Dim nTag() As Integer = CType(cboIOType.Tag, Integer())
            Dim nIO_Type As Integer = nTag(cboIOType.SelectedIndex)
            If nIO_Type <= 0 Then
                subEnableControls(True)
                Me.Cursor = System.Windows.Forms.Cursors.Default
                Exit Sub
            End If
            Dim oIO_Type As clsDMONCfg.eIO_TYPE = CType(nIO_Type, clsDMONCfg.eIO_TYPE)
            Dim oFRCIOSignals As FRRobot.FRCIOSignals = frmMain.oGetIOSignals(moRobot, oIO_Type)
            lstIO.Items.Clear()
            Dim nCount As Integer = oFRCIOSignals.Count
            Dim sList(nCount - 1) As String
            Dim nPort(nCount - 1) As Integer

            Dim nPortNum As Integer = moDmon.PORT_NUM.Value
            Dim nPortFound As Integer = -1
            If oFRCIOSignals IsNot Nothing Then
                Select Case oIO_Type
                    Case clsDMONCfg.eIO_TYPE.IO_AI
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCAnalogIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCAnalogIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psAI_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_AO
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCAnalogIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCAnalogIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psAO_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_DI
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCDigitalIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCDigitalIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psDI_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_DO
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCDigitalIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCDigitalIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psDO_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_GI
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCGroupIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCGroupIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psGI_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_GO
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCGroupIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCGroupIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psGO_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_RI
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCRobotIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCRobotIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psRI_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_RO
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCRobotIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCRobotIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psRO_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_WI
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCWeldDigitalIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCWeldDigitalIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psWI_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case clsDMONCfg.eIO_TYPE.IO_WO
                        For nPortIndex As Integer = 0 To nCount - 1
                            Dim oIOSignal As FRRobot.FRCWeldDigitalIOSignal = CType(oFRCIOSignals.Item(, nPortIndex), FRRobot.FRCWeldDigitalIOSignal)
                            nPort(nPortIndex) = oIOSignal.LogicalNum
                            If nPort(nPortIndex) = nPortNum Then
                                nPortFound = nPortIndex
                            End If
                            sList(nPortIndex) = String.Format(gpsRM.GetString("psWO_CMT"), nPort(nPortIndex).ToString, oIOSignal.Comment)
                        Next
                    Case Else
                End Select
                lstIO.Items.AddRange(sList)
                lstIO.Tag = nPort
                If (moDmon.TYPE.Value = CType(clsDMONCfg.eTYPE.IO, Integer)) And _
                   (moDmon.IO_TYPE.Value = oIO_Type) And _
                   (nPortFound >= 0) Then
                    lstIO.SelectedIndex = nPortFound
                End If

            Else
                MessageBox.Show(gpsRM.GetString("psVAL_IO_TYPE_NOT_FOUND"), gpsRM.GetString("psVAL_CAP"), _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psVAL_IO_TYPE_VAL_ERR") & ex.Message, gpsRM.GetString("psVAL_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        subEnableControls(True)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub tabMain_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
        '********************************************************************************************
        'Description: Tab (item type) changed
        '
        'Parameters: type
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case tabMain.SelectedTab.Name
            Case TabVar.Name
                If mbVariableSetup = False Then
                    subInitVariable()
                    mbVariableSetup = True
                End If
            Case tabIO.Name
                If mbIOSetup = False Then
                    subInitIO()
                    mbIOSetup = True
                End If
            Case tabReg.Name
                If mbRegisterSetup = False Then
                    subInitRegister()
                    mbRegisterSetup = True
                End If
            Case tabAxis.Name
                If mbAxisSetup = False Then
                    subInitAxis()
                    mbAxisSetup = True
                End If
        End Select
    End Sub
    Private Function oValidVariable(ByRef sName As String, _
                                ByRef sFile As String, ByRef sVar As String, _
                                ByRef bSaveable As Boolean) As Object
        '********************************************************************************************
        'Description:  check for a valid variable
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Parse the name
        Try
            bSaveable = False

            If moRobot.IsConnected Then
                Dim oVar As FRRobot.FRCVar = Nothing
                Dim oVars As FRRobot.FRCVars = Nothing
                Dim oProg As FRRobot.FRCProgram = Nothing
                Dim sVarArray() As String = Nothing
                'Check for sys var first
                Dim nDollarSign As Integer = InStr(sName, "$")
                If (nDollarSign = 1) Then
                    'prep the sys vars a bit.
                    sFile = msSYSTEM
                    sVar = sName.Substring(nDollarSign - 1)
                    sVarArray = Split(sVar, ".")
                    oVars = moRobot.SysVariables
                Else
                    Dim nRightBracket As Integer = InStr(sName, "]")
                    If (nRightBracket > 1) AndAlso (sName.Substring(0, 1) = "[") Then
                        sFile = sName.Substring(1, (nRightBracket - 2))
                        sVar = sName.Substring(nRightBracket)
                        sVarArray = Split(sVar, ".")
                    Else
                        sFile = sName
                        sVar = String.Empty
                    End If
                    Dim oList As FRRobot.FRCPrograms
                    oList = moRobot.Programs
                    oProg = DirectCast(oList.Item(sFile), FRRobot.FRCProgram)
                    If oProg Is Nothing Then
                        Return Nothing
                    Else
                        If sVar = String.Empty Then
                            Return oProg
                        Else
                            oVars = oProg.Variables
                        End If
                    End If
                End If
                Dim sTmp As String = String.Empty
                Dim sTmpIdx As String = String.Empty
                Dim nLeftBracket As Integer = 0
                For nVar As Integer = 0 To sVarArray.GetUpperBound(0)
                    sTmpIdx = String.Empty
                    nLeftBracket = InStr(sVarArray(nVar), "[")
                    If nLeftBracket > 0 Then
                        'An Array or other complicated type
                        sTmp = sVarArray(nVar).Substring(0, nLeftBracket - 1)

                        'get the index
                        sTmpIdx = sVarArray(nVar).Substring(nLeftBracket, ((sVarArray(nVar).Length - nLeftBracket) - 1))
                    Else
                        sTmp = sVarArray(nVar)
                    End If
                    'Get the var or array first
                    If sTmp <> String.Empty Then
                        If TypeOf (oVars(sTmp)) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oVars(sTmp), FRRobot.FRCVar)
                        Else
                            oVars = DirectCast(oVars(sTmp), FRRobot.FRCVars)
                        End If
                    End If
                    If oVar Is Nothing And oVars IsNot Nothing Then
                        'Array
                        If sTmpIdx <> String.Empty Then
                            Dim sTmp2() As String = Split(sTmpIdx, ",")
                            For nVar2 As Integer = 0 To sTmp2.GetUpperBound(0)
                                Dim oTmpVar As Object = oVars(sTmp2(nVar2))
                                If oTmpVar IsNot Nothing Then
                                    If TypeOf (oTmpVar) Is FRRobot.FRCVars Then
                                        oVars = DirectCast(oTmpVar, FRRobot.FRCVars)
                                    ElseIf TypeOf (oTmpVar) Is FRRobot.FRCVar Then
                                        oVar = DirectCast(oTmpVar, FRRobot.FRCVar)
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
                If oVar IsNot Nothing Then
                    Return oVar
                ElseIf oVars IsNot Nothing Then
                    Return oVars
                ElseIf oProg IsNot Nothing Then
                    Return oProg
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
        Return Nothing
    End Function

    Private Sub trvwVar_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvwVar.AfterSelect
        '********************************************************************************************
        'Description:  item selected in browse window
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If trvwVar.SelectedNode.Nodes.Count > 0 Then
            'already has children expanded, don't need to do anything here
            Exit Sub
        End If

        Dim sTag As String = DirectCast(trvwVar.SelectedNode.Tag, String)
        Dim bSaveable As Boolean = False
        If sTag IsNot Nothing Then
            Select Case sTag 'Select one of the root nodes or any normal node
                Case msSYSTEM
                    'Nothing to do here
                Case msPROGRAMSTAG
                    'Nothing to do here
                Case Else
                    Dim oTmp As Object = oValidVariable(sTag, msFile, msVar, bSaveable)
                    Dim oVar As FRRobot.FRCVar = Nothing
                    Dim oVars As FRRobot.FRCVars = Nothing
                    Dim oProg As FRRobot.FRCProgram = Nothing
                    If oTmp IsNot Nothing Then
                        If TypeOf (oTmp) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oTmp, FRRobot.FRCVar)
                            'A variable is selected.  Display it
                            lblVarDetail1.Text = sTag
                            Select Case oVar.TypeCode
                                Case FRRobot.FRETypeCodeConstants.frRealType
                                    lblVarDetail2.Text = String.Format(gpsRM.GetString("psVAR_TYPE_SEL_OK"), oVar.TypeName)
                                    moType = clsDMONCfg.eTYPE.Real
                                Case FRRobot.FRETypeCodeConstants.frByteType, FRRobot.FRETypeCodeConstants.frBooleanType, FRRobot.FRETypeCodeConstants.frIntegerType, FRRobot.FRETypeCodeConstants.frShortType
                                    lblVarDetail2.Text = String.Format(gpsRM.GetString("psVAR_TYPE_SEL_OK"), oVar.TypeName)
                                    moType = clsDMONCfg.eTYPE.Int
                                Case Else
                                    lblVarDetail2.Text = String.Format(gpsRM.GetString("psVAR_TYPE_SEL_NOK"), oVar.TypeName)
                                    moType = clsDMONCfg.eTYPE.None
                            End Select
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVars Then
                            oVars = DirectCast(oTmp, FRRobot.FRCVars)
                            'A structure is selected, expand the treeview
                            lblVarDetail1.Text = sTag
                            lblVarDetail2.Text = gpsRM.GetString("psVARS_SEL")
                            moType = clsDMONCfg.eTYPE.None
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCProgram Then
                            oProg = DirectCast(oTmp, FRRobot.FRCProgram)
                            'A program is selected, expand the treeview
                            lblVarDetail1.Text = sTag
                            oVars = oProg.Variables
                            lblVarDetail2.Text = gpsRM.GetString("psPROG_SEL")
                            moType = clsDMONCfg.eTYPE.None
                        Else
                            lblVarDetail1.Text = sTag
                            lblVarDetail1.Text = String.Empty
                            moType = clsDMONCfg.eTYPE.None
                        End If
                        If oVars IsNot Nothing Then
                            If InStr("$", sTag) > 0 Then
                                sTag = String.Empty
                            Else
                                sTag = "[" & msFile & "]"
                            End If
                            For nVar As Integer = 0 To oVars.Count - 1
                                If TypeOf (oVars.Item(, nVar)) Is FRRobot.FRCVars Then
                                    Try
                                        Dim oVars2 As FRRobot.FRCVars = DirectCast(oVars.Item(, nVar), FRRobot.FRCVars)
                                        If oVars IsNot Nothing Then
                                            Dim newNode As New TreeNode(oVars2.FieldName)
                                            newNode.Tag = sTag & oVars2.VarName
                                            trvwVar.SelectedNode.Nodes.Add(newNode)
                                        End If
                                    Catch ex As Exception
                                    End Try
                                ElseIf TypeOf (oVars.Item(, nVar)) Is FRRobot.FRCVar Then
                                    Try
                                        Dim oVar2 As FRRobot.FRCVar = DirectCast(oVars.Item(, nVar), FRRobot.FRCVar)
                                        If oVar2 IsNot Nothing Then
                                            Dim newNode As New TreeNode(oVar2.FieldName)
                                            newNode.Tag = sTag & oVar2.VarName
                                            trvwVar.SelectedNode.Nodes.Add(newNode)
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Else
                                    Debug.Print("???")
                                End If
                            Next
                        End If
                    End If
                    'oVars = oValidVars(sTag)
            End Select
        Else
            ''sTag not set.  Find child nodes.

        End If

    End Sub


End Class

