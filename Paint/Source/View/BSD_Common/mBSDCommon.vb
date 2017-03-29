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
' Form/Module: mBSDCommon
'
' Description: Common Routines for Paintworks Booth Status Display
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Matt White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    01/07/11   MSW     Put easy debug code in BSD
'    04/27/12   MSW     Add "held by estop" conv status, repair panel mask for style and option registers 4.01.03.01
'    03/05/14   MSW     ZDT Changes 03/05/14                                                              4.01.07.00 
       
'********************************************************************************************

Option Compare Binary
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Friend Module mBSDCommon

#Region " Declares "
    'Constants for pop up menu names
    Friend Const gsON As String = "On"
    Friend Const gsOFF As String = "Off"
    Friend Const gsALL As String = "All"
    Friend Const gsENABLE As String = "Enable"
    Friend Const gsDISABLE As String = "Disable"
    Friend Const gsBYPASS As String = "Bypass"
    Friend Const gnQUE_LABELS As Integer = 6 '5 RJO 10/02/14
    Friend gnEasyDebugActive As Integer = 0
    Friend gnEasyDebugLogNum() As Integer = Nothing
    Friend Const gbNumericVin As Boolean = True
    Friend Const gnVinChars As Integer = 9
    Friend mnRepairPanelMask As Integer = 0

    
    ' 03/05/14  MSW     ZDT Changes
    '********************************
    Friend oZDTBtn As Button
    Friend Const gnZdtStartingJobNumber As Integer = 90
    '********************************

    'Tags for the robot pop-up menus
    'BTK 03/04/10 Added so we can change the special move number we send to the robot based on controller
    'software version number.  If the software is not above 7.0 then bypass, clean in and clean out
    'numbers sent to the robot need to change.
    Public Enum eMnuFctn
        None = 0
        'The position labels use the value set here for the PLC write
        Home = 1
        Bypass = 2
        V643CleanIn = 2
        CleanIn = 3
        V643CleanOut = 3
        CleanOut = 4
        V643Bypass = 4
        Purge = 5
        Master = 6
        Special1 = 7
        Special2 = 8
        MaintIn = 9
        MaintOut = 10
        ClearPath = 15
        'The value isn't used except on the position, but once I assign some 
        'I don't know what VB will do, so I'm assigning all of them.
        Enable = 9
        Disable = 10
        All = 12
        Trigger = 13
        Applicator = 14
        Estat = 15
        MoveTo = 16
        Jog = 17
        RecCancel = 18
        RecContinue = 19
        Process = 20
        SuperPurge = 21
        ConvIntlk = 22
        RelTpAsEnablingDev = 23
        Isolate = 24
        EasyButton = 25
        IgnoreOpenerInput = 26
        MaintEnableDisable = 27
        MaintMoveTo = 28
        LifeCycles = 29
        Allp500 = 30
    End Enum
    Friend colZones As clsZones = Nothing
    Friend colStyles As clsSysStyles = Nothing
    Friend colImages As Collection = Nothing
    Private frmAxisJogFrm As frmAxisJog = Nothing
    Friend gbAsciiColor As Boolean = False
    Friend gnAsciiColorNumChar As Integer = 2
    'Tracking setup - available in properties, so each screen can set them
    'Set up some odd defaults so it'll be obvious during debug
    Private mfTrackingScale As Single = 1 'Pizza
    Private mnTrackingStartPos As Integer = 0
    Private mnTrackingQueueCountsBeforeShift As Integer = 100
    Private mnWaterHeaterNumber As Integer = 0

    Public mnMouseOverRobot As Integer = 0
    Public mnMouseDrawnOverRobot As Integer = 0

    Friend Structure tManDoor
        Dim Hinge As Point
        Dim Close As Point
        Dim Open As Point
        Dim Width As Integer
    End Structure
    Friend Structure tLine
        Dim StartPoint As Point
        Dim EndPoint As Point
        Dim Width As Integer
    End Structure
    Friend Enum ePLCLink
        None = -1
        Zone = 0
        Robot = 1
        Queue = 2
        Device = 3
        VinQueue = 4
        myQueue = 5 'NVSP 02/28/2017 My predetect queue
    End Enum
    Friend Enum eBooth  'zone plc link
        'Words
        InterlockWd = 0
        EstopPBWd = 1
        ManDoorWd = 2
        LimitSwStatWd = 3
        IntrStatWd = 4
        GhostInfoWd = 5
        ConveyorStatWd = 6
        ConvSpeedWd = 7
        JobCountWd = 8
        WaterHeaterStatWd = 9
        EntMuteCounter = 22
        EntBeamBrokeCounts = 24
        EntBeamMadeCounts = 25
        ExitMuteCounter = 23
        ExitBeamBrokeCounts = 26
        ExitBeamMadeCounts = 27
        PaintMixOutWd = 17
        PaintMixStatWd = 18
        'Interlock Bits
        MasterPower = 0
        FireBit = 1
        AirBit = 2
        PaintCirc = 3
        ProcessAirEnbBit = 4
        ConvPermBit = 5
        EstatMotionBit = 6
        EstatEnbBit = 7
        AutoModeBit = 8
        ManModeBit = 9
        TeachModeBit = 10
        AllRobotsBypassed = 11
        DegradeEnabled = 12
        T1ModeBit = 13 'New
        ConvRunSwitch = 14
        'RequestToEnterDisabled = 15
        BackupEncoderSwitch = 15
        SolventOKBit = 16
        RequestToEnterDisabled = 17
        EstatEnbSwitch = 18
        EnablingDev = 19
        'Limit Switch bits
        PartDetBit = 0
        'Intrusion bits
        EntIntMadeBit = 0
        EntMuteBit = 2
        EntFaultBit = 4
        ExitIntMadeBit = 8
        ExitMuteBit = 10
        ExitFaultBit = 12
        'Ghost Word Bits
        GhostBoothQClearBit = CInt(2 ^ 0)
        GhostAutoModeBit = CInt(2 ^ 1)
        GhostRobotsReady = CInt(2 ^ 2)
        GhostJogDisabled = CInt(2 ^ 3)
        GhostZoneOK = CInt(2 ^ 4)
        GhostNumBits = 5
        ZDTGhostRobotsOK = CInt(2 ^ 5)
        ZDTGhostZoneOK = CInt(2 ^ 6)
        ZDTNumBits = 7
        HoldAtSOC = CInt(2 ^ 5)
        EstatZoneOK = CInt(2 ^ 6)
        'WaterHeaterBits
        WaterHeaterEnabled = 0
        WaterHeaterMasterStartOn = 1
        WaterHeaterLevelOK = 2
        WaterHeaterTempOK = 3
        WaterHeater2Enabled = 4
        WaterHeater2MasterStartOn = 5
        WaterHeater2LevelOK = 6
        WaterHeater2TempOK = 7
        ''HAF 041514
        WaterHeater3Enabled = 8
        WaterHeater3MasterStartOn = 9
        WaterHeater3LevelOK = 10
        WaterHeater3TempOK = 11
        WaterHeater4Enabled = 12
        WaterHeater4MasterStartOn = 13
        WaterHeater4LevelOK = 14
        WaterHeater4TempOK = 15
        ''HAF 041514
    End Enum
    Friend Enum eConvStat
        Unknown = 0
        Running = 1
        HeldByOthers = 2
        EstatNotInRemote = 3
        HeldAtSOC = 4
        HeldByIntrusion = 5
        HeldByDoor = 6
        HeldByBoothIntlk = 7
        HeldByLimitSwStuck = 8
        CommError = 9
        HeldByGhostSim = 10
        JogFunctionEnabled = 11
        WaitingForCancCont = 12
        WaitForRobotHome = 13
        WaitForCC = 14
        RobotFaulted = 15
        HeldForQueueEdit = 16
        JobDataError = 17
        IncomingJobInManual = 18
        IncomingJob = 19
        HeldByFanuc = 20
        MaintRobMove = 22
        DataTransferErr = 26
        DataCommNextZone = 28
        RobotNotInProd = 30
        StyleIDMismatch = 31
        ConvRevSelected = 35
        ConvRevRunning = 36
        ConvHeldAtMIS = 33
        JOBS_TOO_CLOSE = 34
        HeldByEstop = 39
        HeldByMutingDuringCoverChange = 40

    End Enum
    Friend Enum eQueue
        WordsPerPosition = 15
        'pist detect Q pos = 5, so predetect 1 = position 4, and the PLC calls it queue 10
        PostDetQ1 = 5
        'Word indexes
        Status = 0
        'Status word values
        Stat_PosEmpty = 0
        Stat_Paint = 1
        Stat_NoPaint = 2
        Stat_EmptyCarrier = 3
        'Back to word #s
        Style = 1
        Color = 2
        Optn = 3
        PaintEnables = 4
        VinWd1 = 5
        VinLen = 1
        Carrier = 7
        Repair = 8
        Position = 9
        Invalid = 10
        Degrade = 11
        Tutone = 12 'RJO 10/02/14
    End Enum
    Friend Enum eRobot
        WordsPerRobot = 15
        Position = 0
        AtHomeBitVal = CInt(2 ^ 0)
        Status = 1
        RunningStat = 2
        RunningBitVal = CInt(2 ^ 0)
        MenuEnable = 3
        CycleTime = 4
        Carrier = 5
        Style = 6
        Color = 7
        Optn = 8
        Repair = 9
        CCCycle = 10
        CCTime = 11
        Spare = 12
        Vin1 = 13
        Vin2 = 14
        ' EstatKV = 8
        ' EstatUA = 9
    End Enum
    Friend Enum eMenuEnable
        'Menu Enable Bits
        RobotRunningBitVal = CInt(2 ^ 0)
        MoveHomeBitVal = CInt(2 ^ 1)
        MoveOtherBitVal = CInt(2 ^ 2)
        CancContBitVal = CInt(2 ^ 3)
        CancOnlyBitVal = CInt(2 ^ 4)
        ManFuncBitVal = CInt(2 ^ 5)
        ClearPathBitVal = CInt(2 ^ 6)
        AxisJogBitVal = CInt(2 ^ 7)
        ClearPathContinueBitVal = CInt(2 ^ 8)
        ConvIntlkBitVal = CInt(2 ^ 9)
        DegradeBitVal = CInt(2 ^ 10)
        MaintDisableEnbBitVal = CInt(2 ^ 11)
        RelTpAsEnablingDev = CInt(2 ^ 12)
        MaintMoveFromHomeBitVal = CInt(2 ^ 13)
        MaintMoveToHomeBitVal = CInt(2 ^ 14)
        DisableMoveAllInAuto = CInt(2 ^ 15)
        CoverChangeBitVAl = CInt(2 ^ 16)
        MovePurgeAllBitVAl = CInt(2 ^ 17)
        MoveHomeAllBitVAl = CInt(2 ^ 18)
        CancContAllBitVal = CInt(2 ^ 19)    'JZ 12152016 - Add cancel/continue all.
        CancOnlyAllBitVal = CInt(2 ^ 20)    'JZ 12152016 - Add cancel only all.
    End Enum
    Friend Enum eRobotStat
        Auto = CInt(2 ^ 0)
        Manual = CInt(2 ^ 1)
        Teach = CInt(2 ^ 2)
        ServoDiscOff = CInt(2 ^ 3)
        TPEStopOn = CInt(2 ^ 4)
        TPEnabled = CInt(2 ^ 5)
        SOPEStop = CInt(2 ^ 6)
        BypassLS = CInt(2 ^ 7)
        GhostInProg = CInt(2 ^ 8)
        MaintDoorClosed = CInt(2 ^ 9)
        TPLinkOn = CInt(2 ^ 10)
        PurgeOK = CInt(2 ^ 13)
        IPCPressMonOK = 12 'RJO 06/12/13
        IPCInletRegOK = 13 'RJO 06/12/13
        SA1ClosedLoop = 14 'RJO 06/12/13
        SA2ClosedLoop = 15 'RJO 06/12/13
    End Enum
    Friend Enum eRobotRunStat
        Running = CInt(2 ^ 0)
        Faulted = CInt(2 ^ 1)
        Bypassed = CInt(2 ^ 2)
        ColChgActive = CInt(2 ^ 3)
        InCycle = CInt(2 ^ 4)
        CancCont = CInt(2 ^ 5)
        WaitCanc = CInt(2 ^ 6)
        Selected = CInt(2 ^ 7)
        Triggeron = CInt(2 ^ 8)
        GunDisabled = CInt(2 ^ 9)
        ConvBypassed = CInt(2 ^ 11)
        AppDisabled = CInt(2 ^ 12)
        EstatDisabled = CInt(2 ^ 13)
        RunningOrBypassed = CInt(2 ^ 14)
        Isolated = CInt(2 ^ 15)
    End Enum
    Friend Enum eRobPos
        Home = CInt(2 ^ 0)
        Bypass = CInt(2 ^ 1)
        Cleanin = CInt(2 ^ 2)
        Cleanout = CInt(2 ^ 3)
        Purge = CInt(2 ^ 4)
        Master = CInt(2 ^ 5)
        Special1 = CInt(2 ^ 6)
        Special2 = CInt(2 ^ 7)
        Maint = CInt(2 ^ 8)
        Pos10 = CInt(2 ^ 9)
        Pos11 = CInt(2 ^ 10)
        Pos12 = CInt(2 ^ 11)
    End Enum
    Friend Enum eQueueEdit
        StatusWd = 0
        StyleWd = 8
        ColorWd = 9
        OptionWd = 10
        RepairWd = 17
        CarrierWd = 6
        VinWord = 11
        VinLength = 2
        CarrierType = 42
        Max = 49
    End Enum
    Friend Enum eMyQueueEdit
        StatusWd = 0
        paint = 4
        CarrierWd = 6
        StyleWd = 8
        ColorWd = 9
        OptionWd = 10
        Max = 49
    End Enum
    Friend Enum eQueueRead
        StatusWd = 0
        StyleWd = 8
        ColorWd = 9
        OptionWd = 10
        RepairWd = 17
        CarrierWd = 6
        VinWord = 11
        VinLength = 2
        CarrierType = 42
    End Enum
    Friend Enum eJobStatus
        Empty = 0
        Paint = 1
        NoPaint = 2
        Ghost = 4
    End Enum

#End Region

#Region " Properties "

    'Tracking setup
    'Each booth screen should set these when it starts up

    Property TrackingScale() As Single
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
        Get
            Return mfTrackingScale
        End Get
        Set(ByVal value As Single)
            mfTrackingScale = value
        End Set

    End Property

    Property TrackingStartPos() As Integer
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
        Get
            Return mnTrackingStartPos
        End Get
        Set(ByVal value As Integer)
            mnTrackingStartPos = value
        End Set

    End Property

    Property TrackingQueueCountsBeforeShift() As Integer
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
        Get
            Return mnTrackingQueueCountsBeforeShift
        End Get
        Set(ByVal value As Integer)
            mnTrackingQueueCountsBeforeShift = value
        End Set

    End Property

#End Region


#Region " ZDT "
    ' 03/05/14  MSW     ZDT Changes
    '********************************
    Friend Sub subUpdateZDT(ByRef sText As String, ByRef sColor As String)
        '********************************************************************************************
        'Description: Update ZDT button
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/05/14  MSW     ZDT Changes 03/05/14
        '********************************************************************************************
        If oZDTBtn IsNot Nothing Then
            oZDTBtn.Text = sText
            Select Case sColor
                Case "White", "Green"
                    oZDTBtn.BackColor = System.Drawing.SystemColors.Control
                Case "Red"
                    oZDTBtn.BackColor = System.Drawing.Color.Red
                Case "Yellow"
                    oZDTBtn.BackColor = System.Drawing.Color.Yellow
            End Select
        End If
    End Sub



    ' 03/05/14  MSW     ZDT Changes
    '********************************
    Friend Sub ZDTAlertClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: ZDT alert button pressed. show details
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/05/14  MSW     ZDT Changes 03/05/14
        '********************************************************************************************
        If frmMain.oZDT IsNot Nothing Then
            frmMiniGrid.Text = gpsRM.GetString("psZDT_TEST_STATUS")
            subSetUpToolStrip(frmMiniGrid.tlsMain)
            frmMain.oZDT.subDisplayGrid(frmMiniGrid.dgvMini, frmMiniGrid.mnuResetStatus)
            frmMiniGrid.ShowDialog()
        End If
    End Sub
#End Region


#Region " Routines "
    Friend Function GetVIN(ByVal VinData() As String, ByVal SeqNum As String) As String
        '********************************************************************************************
        'Description: Returns the VIN number assigned to the supplied sequence number.
        '
        'Parameters: VinData - Vin Data Queue, SeqNum - Job Sequence Number
        'Returns:    VIN Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sVin As String = String.Empty
        Dim nIndex As Integer = -1

        If Not VinData Is Nothing Then
            'Make sure we have the whole Vin Queue (100 words)
            If VinData.GetUpperBound(0) < 99 Then Return sVin
            'find the right one
            For nPtr As Integer = 0 To 90 Step 10
                If VinData(nPtr) = SeqNum Then
                    nIndex = nPtr + 1
                    Exit For
                End If
            Next 'nPtr

            If nIndex < 0 Then
                'Not Found
                Return sVin
            Else
                'cipher the VIN Number

                For nPtr As Integer = nIndex To (nIndex + 8)
                    Dim nData As Integer = CType(VinData(nPtr), Integer)
                    Dim nChar As Integer

                    'Get the right character
                    nChar = nData And &HFF

                    If (nChar > 31) And (nChar < 127) Then
                        sVin = sVin & Chr(nChar)
                    End If
                    'Get the left character
                    nChar = nData And &HFF00
                    nChar = nChar \ 256
                    If (nChar > 31) And (nChar < 127) Then
                        sVin = sVin & Chr(nChar)
                    End If
                Next

                sVin = Strings.Trim(sVin)

            End If

        End If

        Return sVin

    End Function

    Private Sub subDoEasyDebug(ByVal rMnuTag As eMnuFctn(), ByVal srobot As String)
        '********************************************************************************************
        'Description: start easy debug sequence
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     Add Easy Debug menu
        '********************************************************************************************
        Try
            Dim oRobot As clsArm = frmMain.colArms(srobot)
            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(gpsRM.GetString("psEASY_BUTTON_MESSAGE"), _
                                                                       gpsRM.GetString("psEASY_BUTTON"), MessageBoxButtons.YesNo)

            If lReply = DialogResult.No Then
                Exit Sub
            End If
            Dim nControllerNum As Integer = oRobot.Controller.ControllerNumber
            If gnEasyDebugLogNum Is Nothing Then
                ReDim gnEasyDebugLogNum(frmMain.colArms.Count)
            End If

            'Get then next log number
            oRobot.ProgramName = "pashell"
            oRobot.VariableName = "iLogNum"
            gnEasyDebugLogNum(nControllerNum) = CType(oRobot.VarValue, Integer)

            oRobot.ProgramName = "pashell"
            oRobot.VariableName = "bGetDebug"
            oRobot.VarValue = "True"
           
            gnEasyDebugActive = gnEasyDebugActive Or (CInt(2 ^ nControllerNum))

            frmWaitDebug.Text = gpsRM.GetString("psEASY_BUTTON")
            frmWaitDebug.lblText.Text = gpsRM.GetString("psWAIT_DEBUG_DATA")
            frmWaitDebug.tmrEasyButton.Enabled = True
            frmWaitDebug.ShowDialog()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: subDoEasyDebug", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub subGetEasyDebug(ByRef oCntr As clsController, ByVal nLogFile As Integer)
        '********************************************************************************************
        'Description: check easy debug files from robot
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/07/11  MSW     Put easy debug code in BSD
        '********************************************************************************************
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim oFTP As clsFSFtp = Nothing
        Try
            oFTP = New clsFSFtp(oCntr.FanucName)
            frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psCOPYING_DEBUG_DATA_FROM", oCulture), oCntr.Name)
            Application.DoEvents()
            With oFTP
                If .Connected Then
                    'Check for zip version first
                    .WorkingDir = "MD:"
                    Dim sDir() As String = .Directory("easydbg.zip", False, True)
                    Application.DoEvents()
                    'If (sDir IsNot Nothing) AndAlso (sDir(0) IsNot Nothing) AndAlso (sDir(0).ToLower = "easydbg.zip") Then
                    '    The robot zipped it up for us, do this the easy way
                    '    frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psEASYDBGZIP_FOUND", oCulture), oCntr.Name)
                    '    Application.DoEvents()
                    '    Save a zip
                    '    Dim sFileName As String = String.Empty
                    '    Dim oSFD As New SaveFileDialog
                    '    Try
                    '        With oSFD
                    '            Dim sPathTmp As String = String.Empty
                    '            If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                    '                oSFD.InitialDirectory = sPathTmp
                    '            End If
                    '            oSFD.CheckPathExists = True
                    '            oSFD.AddExtension = True
                    '            oSFD.OverwritePrompt = True
                    '            oSFD.FileName = colZones.CurrentZone & "_" & oCntr.FanucName & "_" & "EASYDBG.ZIP"
                    '            oSFD.DefaultExt = gpsRM.GetString("psZIP_EXT")
                    '            oSFD.Filter = gpsRM.GetString("psZIPMASK")
                    '            oSFD.FilterIndex = 1
                    '            Dim oVal As DialogResult = oSFD.ShowDialog
                    '            If (oVal = System.Windows.Forms.DialogResult.OK) Then
                    '                sFileName = oSFD.FileName
                    '            Else
                    '                sFileName = String.Empty
                    '            End If

                    '        End With
                    '        If sFileName <> String.Empty Then
                    '            If File.Exists(sFileName) Then
                    '                File.Delete(sFileName)
                    '            End If
                    '            frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psCOPYING_EASYDBGZIP_TO", oCulture), sFileName)
                    '            Application.DoEvents()
                    '            Dim sPathSplit() As String = Split(sFileName, "\")
                    '            Dim sFile As String = sPathSplit(sPathSplit.GetUpperBound(0))
                    '            Dim sPath As String = sFileName.Substring(0, (sFileName.Length - sFile.Length))
                    '            .DestDir = sPath
                    '            If (.GetFile(sDir(0), sFile) = False) Then
                    '                Dim sTmp As String = gpsRM.GetString("psFAILED_TO_COPY_DEBUG_DATA") & .ErrorMsg
                    '                frmMain.Status = sTmp
                    '                Application.DoEvents()
                    '                MessageBox.Show(sTmp, gcsRM.GetString("csERROR"), MessageBoxButtons.OK)
                    '            End If
                    '        End If
                    '    Catch ex As Exception
                    '        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: subGetEasyDebug", _
                    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    '        ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, frmMain.msSCREEN_NAME, _
                    '                frmMain.Status, MessageBoxButtons.OK)
                    '    End Try
                    'Else
                    'No zip, do it the hard way
                    .WorkingDir = "FR7:"
                    sDir = .Directory("log*", True, False)
                    Application.DoEvents()
                    If (sDir IsNot Nothing) Then
                        Dim sLogName As String = "log" & nLogFile.ToString
                        Dim bFound As Boolean = False
                        For Each sFolder As String In sDir
                            If sFolder.ToLower = sLogName Then
                                Dim sPath As String = "FR7:\" & sLogName
                                .WorkingDir = sPath
                                Dim sSubDir() As String = .Directory("*.*", False, True)
                                If sSubDir IsNot Nothing Then
                                    frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psEASYDBGFOLDER_FOUND", oCulture), sPath, oCntr.Name)
                                    bFound = True
                                    'Get a destination folder.
                                    Dim sFolderName As String = String.Empty
                                    Dim oFB As New FolderBrowserDialog
                                    Try
                                        With oFB
                                            .ShowNewFolderButton = True
                                            Dim sPathTmp As String = String.Empty
                                            If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                                                .SelectedPath = sPathTmp
                                            End If
                                            .Description = gpsRM.GetString("psSELECT_FOLDER")
                                            Dim oVal As DialogResult = .ShowDialog()
                                            If oVal = Windows.Forms.DialogResult.OK Then
                                                sFolderName = .SelectedPath
                                            Else
                                                sFolderName = String.Empty
                                            End If
                                        End With
                                        If sFolderName <> String.Empty Then
                                            sFolderName = sFolderName & "\" & oCntr.Name
                                            IO.Directory.CreateDirectory(sFolderName)
                                            sFolderName = sFolderName & "\" & sLogName
                                            IO.Directory.CreateDirectory(sFolderName)
                                            .DestDir = sFolderName
                                            For Each sFile As String In sSubDir
                                                frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psCOPYING_X_TO", oCulture), (sPath & "\" & sFile), sFolderName)
                                                Application.DoEvents()
                                                If (.GetFile(sFile, sFile) = False) Then
                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psFAILED_TO_COPY", oCulture), sFile) & .ErrorMsg
                                                    frmMain.Status = sTmp
                                                    Application.DoEvents()
                                                    MessageBox.Show(sTmp, gcsRM.GetString("csERROR"), MessageBoxButtons.OK)
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: subGetEasyDebug", _
                                                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, frmMain.msSCREEN_NAME, _
                                                frmMain.Status, MessageBoxButtons.OK)
                                    End Try
                                End If
                            End If
                        Next
                        If bFound = False Then
                            Dim sTmp As String = String.Format(gpsRM.GetString("psCAN_NOT_FIND_DEBUG_DATA_ON", oCulture), oCntr.Name)
                            frmMain.Status = sTmp
                            MessageBox.Show(sTmp, gcsRM.GetString("csERROR"), MessageBoxButtons.OK)
                        End If
                    Else
                        frmWaitDebug.lblText.Text = String.Format(gpsRM.GetString("psNO_EASYDBG_FOUND", oCulture), oCntr.Name)
                    End If
                End If
                'End If
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: subGetEasyDebug", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            If oFTP IsNot Nothing Then
                oFTP.Close()
            End If
        End Try
        frmWaitDebug.lblText.Text = gpsRM.GetString("psWAIT_DEBUG_DATA")
    End Sub
    Friend Sub CheckEasyDebug()
        '********************************************************************************************
        'Description: check easy debug status
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     Add Easy Debug menu
        '********************************************************************************************
        Try
            If gnEasyDebugActive = 0 Then
                frmWaitDebug.tmrEasyButton.Enabled = False
                Exit Sub
            End If
            'It gets real slow, so add some doevent calls
            For Each oCntr As clsController In frmMain.colControllers
                Application.DoEvents()
                Dim nCntrNum As Integer = oCntr.ControllerNumber
                If (gnEasyDebugActive And CInt(2 ^ nCntrNum)) > 0 Then
                    Application.DoEvents()
                    'Check easy debug for this controller
                    oCntr.Arms(0).ProgramName = "pashell"
                    oCntr.Arms(0).VariableName = "bGetDebug"
                    If oCntr.Arms(0).VarValue.ToLower = "false" Then
                        Application.DoEvents()
                        'It's done.  Clear the flag
                        gnEasyDebugActive = gnEasyDebugActive And Not (CInt(2 ^ nCntrNum))
                        'Move the easydebug copy out of the DMON logger and PLC
                        ''get the current status from the PLC
                        'frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "EasyButtonFlag"
                        'Application.DoEvents()
                        'Dim sData() As String
                        'sData = frmMain.mPLC.PLCData
                        'Dim nData As Integer = CInt(sData(0))
                        'Application.DoEvents()
                        ''Tell the PLC this controlller is ready
                        'nData = nData Or CInt(2 ^ oCntr.ControllerNumber)
                        'ReDim sData(0)
                        'Application.DoEvents()
                        'sData(0) = nData.ToString
                        'frmMain.mPLC.PLCData = sData
                        'Application.DoEvents()
                        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
                        frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psEASY_DEBUG_COMP_LBL", oCulture), gnEasyDebugLogNum(nCntrNum))
                        frmMultipleChoiceDlg.caption = gpsRM.GetString("psEASY_DEBUG_COMP_CAP", oCulture)
                        frmMultipleChoiceDlg.NumButtons = 2
                        frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psGET_DATA", oCulture)
                        frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psEXIT_BTN", oCulture)
                        frmMultipleChoiceDlg.ShowDialog()
                        Application.DoEvents() 'let the hotlink fire
                        Select Case frmMultipleChoiceDlg.SelectedButton
                            Case 0
                                'Get files from oCntr
                                subGetEasyDebug(oCntr, gnEasyDebugLogNum(nCntrNum))
                            Case 1
                                'Exit - do nothing
                        End Select

                    End If
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: CheckEasyDebug", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        Try
            If gnEasyDebugActive <> 0 Then
                frmWaitDebug.tmrEasyButton.Enabled = True
            Else
                frmWaitDebug.tmrEasyButton.Enabled = False
                frmWaitDebug.Close()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: CheckEasyDebug", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Friend Function ConveyorStatChange(ByVal vUCtl As UserControl, ByVal sData As String) As Color
        '********************************************************************************************
        'Description: Conveyor Status Label
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nData As Integer = CInt(sData)
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)
            Dim gpb As GroupBox = DirectCast(pnlTmp.Controls("gpbMode"), GroupBox)
            Dim lblStat As Label = DirectCast(gpb.Controls("lblConvStat"), Label)
            Dim oColor As Color = Color.Gray
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture

            Select Case nData
                Case eConvStat.Unknown
                    lblStat.Text = gpsRM.GetString("psUNKNOWN", oCulture)
                    oColor = Color.Gray
                Case eConvStat.Running
                    lblStat.Text = gpsRM.GetString("psCONV_RUNNING", oCulture)
                    oColor = Color.LimeGreen
                Case eConvStat.HeldByOthers
                    lblStat.Text = gpsRM.GetString("psHELD_BY_OTHERS", oCulture)
                    oColor = Color.Yellow
                Case eConvStat.EstatNotInRemote
                    lblStat.Text = gpsRM.GetString("psESTAT_NOT_IN_REMOTE", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldAtSOC
                    lblStat.Text = gpsRM.GetString("psHELD_BY_SOC", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByIntrusion
                    lblStat.Text = gpsRM.GetString("psHELD_BY_INTR", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByDoor
                    lblStat.Text = gpsRM.GetString("psHELD_BY_DOOR", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByBoothIntlk
                    lblStat.Text = gpsRM.GetString("psHELD_BY_BOOTH", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByLimitSwStuck
                    lblStat.Text = gpsRM.GetString("psHELD_BY_LIM_SW", oCulture)
                    oColor = Color.Red
                Case eConvStat.CommError
                    lblStat.Text = gpsRM.GetString("psCOMM_ERROR", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByGhostSim
                    lblStat.Text = gpsRM.GetString("psHELD_BY_GHOST", oCulture)
                    oColor = Color.Red
                Case eConvStat.JogFunctionEnabled
                    lblStat.Text = gpsRM.GetString("psHELD_BY_JOG", oCulture)
                    oColor = Color.Red
                Case eConvStat.WaitingForCancCont
                    lblStat.Text = gpsRM.GetString("psWAIT_CAN_CON", oCulture)
                    oColor = Color.Red
                Case eConvStat.WaitForRobotHome
                    lblStat.Text = gpsRM.GetString("psWAIT_HOME", oCulture)
                    oColor = Color.Red
                Case eConvStat.WaitForCC
                    lblStat.Text = gpsRM.GetString("psWAIT_COL_CHG", oCulture)
                    oColor = Color.Red
                Case eConvStat.RobotFaulted
                    lblStat.Text = gpsRM.GetString("psHELD_BY_ROBOTS", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldForQueueEdit
                    lblStat.Text = gpsRM.GetString("psHELD_BY_QEDIT", oCulture)
                    oColor = Color.Red
                Case eConvStat.IncomingJobInManual
                    lblStat.Text = gpsRM.GetString("psINC_JOB_MAN", oCulture)
                    oColor = Color.Red
                Case eConvStat.IncomingJob
                    lblStat.Text = gpsRM.GetString("psINC_JOB", oCulture)
                    oColor = Color.Red
                Case eConvStat.JobDataError
                    lblStat.Text = gpsRM.GetString("psJOB_DATA_ERR", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByFanuc
                    lblStat.Text = gpsRM.GetString("psHELD_BY_FANUC", oCulture)
                    oColor = Color.Red
                Case eConvStat.MaintRobMove
                    lblStat.Text = gpsRM.GetString("psMAINT_ROB_MOVE", oCulture)
                    oColor = Color.Red
                Case eConvStat.DataTransferErr
                    lblStat.Text = gpsRM.GetString("psDATA_TRANS_ERR", oCulture)
                    oColor = Color.Red
                Case eConvStat.DataCommNextZone
                    lblStat.Text = gpsRM.GetString("psDATA_ERR_OUT", oCulture)
                    oColor = Color.Red
                Case eConvStat.RobotNotInProd
                    lblStat.Text = gpsRM.GetString("psROB_NOT_PROD", oCulture)
                    oColor = Color.Red
                Case eConvStat.StyleIDMismatch
                    lblStat.Text = gpsRM.GetString("psSTYLEID_MISMATCH", oCulture)
                    oColor = Color.Red
                Case eConvStat.ConvRevSelected
                    lblStat.Text = gpsRM.GetString("psCONV_REV_SEL", oCulture)
                    oColor = Color.Red
                Case eConvStat.ConvRevRunning
                    lblStat.Text = gpsRM.GetString("psCONV_REV_RUN", oCulture)
                    oColor = Color.Red
                Case eConvStat.ConvHeldAtMIS
                    lblStat.Text = gpsRM.GetString("psCONV_HELD_MIS", oCulture)
                    oColor = Color.Red
                Case eConvStat.JOBS_TOO_CLOSE
                    lblStat.Text = gpsRM.GetString("psJOBS_TOO_CLOSE", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByEstop
                    lblStat.Text = gpsRM.GetString("psCONV_HELD_BY_ESTOP", oCulture)
                    oColor = Color.Red
                Case eConvStat.HeldByMutingDuringCoverChange
                    lblStat.Text = gpsRM.GetString("psCONV_HELD_BY_MUTING_IN_COVER_CHANGE", oCulture)
                    oColor = Color.Red

                Case Else
                    lblStat.Text = gpsRM.GetString("psUNKNOWN", oCulture)
                    oColor = Color.Gray
            End Select

            lblStat.BackColor = oColor
            Return oColor

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Function

    Friend Sub ConvSpeedConv(ByRef sData As String, _
                             Optional ByRef lblMetric As Label = Nothing, _
                             Optional ByRef lblEnglish As Label = Nothing)
        '********************************************************************************************
        'Description: Convert conveyor speed for display
        '
        'Parameters: conveyor speed word from PLC, metric and english label 
        'Returns:    sets the text in each label
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not (lblMetric Is Nothing) Then
            lblMetric.Text = Format$((CInt(sData) / 100), "#0.0")
        End If

        If Not (lblEnglish Is Nothing) Then
            lblEnglish.Text = Format$(((CInt(sData) * 0.0019685039)), "#0.0")
        End If

    End Sub

    Private Function CopyFunctionArray(ByRef pData As eMnuFctn()) As eMnuFctn()
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
        Dim eCopy As eMnuFctn()
        ReDim eCopy(pData.GetUpperBound(0))

        pData.CopyTo(eCopy, 0)
        Return eCopy

    End Function

    Friend Sub EnableRobotMenus(ByRef oMnu As ContextMenuStrip, ByRef sZoneData As String(), _
                                ByRef sRobotData As String(), ByVal nDataBlock As Integer, _
                                ByVal IsOpener As Boolean, Optional ByVal bEnableHomeAll As Boolean = False, _
                                Optional ByVal sRobotName As String = "")
        '********************************************************************************************
        'Description: Enable the pop-up menus based on booth and robot status
        '
        'Parameters: Menu object, booth and robot PLC data, Datablock #, True if opener
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/15/13  RJO     Modified Applicator subMenu items to reflect state of robot applicator.
        '                   Some sub-menu items were selectable even though the main menu item was
        '                   disabled.
        '********************************************************************************************
        Dim nBoothIntlk As Integer
        Dim nMenuEnable As Integer
        Dim nRobotStat As Integer
        Dim nRunStat As Integer
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim sAll As String = gpsRM.GetString("psALL", oCulture)

        If (sZoneData Is Nothing) Or (sRobotData Is Nothing) Then
            nBoothIntlk = 0
            nMenuEnable = 0
            nRobotStat = 0
            nRunStat = 0
        Else
            If sRobotName.Equals("B2") Or sRobotName.Equals("P2") Or sRobotName.Equals("C2") Then 'NVSP 12/06/2016 Changes for the menu to get enabled
                nBoothIntlk = CInt(sZoneData(eBooth.InterlockWd))
                nMenuEnable = CInt(sRobotData((eRobot.WordsPerRobot * 2 * (nDataBlock - 1)) + eRobot.MenuEnable))
                nRobotStat = CInt(sRobotData((eRobot.WordsPerRobot * 2 * (nDataBlock - 1)) + eRobot.Status))
                nRunStat = CInt(sRobotData((eRobot.WordsPerRobot * 2 * (nDataBlock - 1)) + eRobot.RunningStat))
            Else
                nBoothIntlk = CInt(sZoneData(eBooth.InterlockWd))
                nMenuEnable = CInt(sRobotData((eRobot.WordsPerRobot * (nDataBlock - 1)) + eRobot.MenuEnable))
                nRobotStat = CInt(sRobotData((eRobot.WordsPerRobot * (nDataBlock - 1)) + eRobot.Status))
                nRunStat = CInt(sRobotData((eRobot.WordsPerRobot * (nDataBlock - 1)) + eRobot.RunningStat))
            End If
            
        End If

        Dim nMask As Integer = CInt(gnBitVal(eBooth.AutoModeBit) Or gnBitVal(eBooth.ManModeBit) _
                                                    Or gnBitVal(eBooth.TeachModeBit))
        Dim nMode As Integer = nMask And nBoothIntlk
        Dim bAutoMode As Boolean = False
        Dim bManualMode As Boolean = False

        Select Case nMode
            Case gnBitVal(eBooth.AutoModeBit)
                bAutoMode = True
            Case gnBitVal(eBooth.ManModeBit)
                bManualMode = True
            Case gnBitVal(eBooth.TeachModeBit)
            Case Else
        End Select
        Dim bTempEnable As Boolean = Not (IsOpener Or colZones.CurrentZone.ToLower.Contains("teach"))
        'Openers only have a subset of the painter arm menus. Hide the stuff that doesn't apply.
        With oMnu
            .Items("mnuRobotName").Tag = nDataBlock
            If sRobotName <> "" Then
                .Items("mnuRobotName").Text = sRobotName
            End If
            .Items("mnuTriggerEnable").Visible = bTempEnable
            .Items("mnuAppEnable").Visible = bTempEnable
            .Items("mnuEstatEnable").Visible = bTempEnable
            .Items("mnuSeparator1").Visible = bTempEnable
            .Items("mnuProcess").Visible = bTempEnable
            .Items("mnuSeparator4").Visible = bTempEnable
            .Items("mnuLifeCycle").Visible = Not IsOpener
            .Items("mnuSeparator11").Visible = Not IsOpener
            .Items("mnuManFunc").Visible = bTempEnable
            .Items("mnuSeparator5").Visible = bTempEnable
        End With

        '******* Applicator menus *******************************************************************
        Dim oSubMnu As ToolStripMenuItem
        Dim sMnuName As String = "mnuTriggerEnable"

        bTempEnable = ((nRunStat And eRobotRunStat.GunDisabled) = eRobotRunStat.GunDisabled)
        oMnu.Items(sMnuName).Enabled = True
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.DropDownItems(sMnuName & "On").Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OnAll").Enabled = True
        oSubMnu.DropDownItems(sMnuName & "Off").Enabled = Not bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OffAll").Enabled = True
        'NRU 160921 One Robot
        oSubMnu.DropDownItems(sMnuName & "OnAll").Visible = False
        oSubMnu.DropDownItems(sMnuName & "OffAll").Visible = False

        sMnuName = "mnuAppEnable"
        bTempEnable = ((nRunStat And eRobotRunStat.AppDisabled) = eRobotRunStat.AppDisabled)
        oMnu.Items(sMnuName).Enabled = True
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.DropDownItems(sMnuName & "On").Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OnAll").Enabled = True
        oSubMnu.DropDownItems(sMnuName & "Off").Enabled = Not bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OffAll").Enabled = True
        'NRU 160921 One Robot
        oSubMnu.DropDownItems(sMnuName & "OnAll").Visible = False
        oSubMnu.DropDownItems(sMnuName & "OffAll").Visible = False

        sMnuName = "mnuEstatEnable"
        bTempEnable = ((nRunStat And eRobotRunStat.EstatDisabled) = eRobotRunStat.EstatDisabled)
        oMnu.Items(sMnuName).Enabled = True
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.DropDownItems(sMnuName & "On").Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OnAll").Enabled = True
        oSubMnu.DropDownItems(sMnuName & "Off").Enabled = Not bTempEnable
        oSubMnu.DropDownItems(sMnuName & "OffAll").Enabled = True
        'NRU 160921 One Robot
        oSubMnu.DropDownItems(sMnuName & "OnAll").Visible = False
        oSubMnu.DropDownItems(sMnuName & "OffAll").Visible = False
        '******* Applicator menus end ***************************************************************

        '******* Move menus *************************************************************************
        Dim nRobotType As Integer = DirectCast((frmMain.colArms(nDataBlock - 1).ArmType), Integer)

        bTempEnable = (bManualMode Or bAutoMode)
        Dim bTempEnable1 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.MoveHomeBitVal) = eMenuEnable.MoveHomeBitVal)
        Dim bTempEnable2 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.MoveOtherBitVal) = eMenuEnable.MoveOtherBitVal)
        Dim bTempEnable3 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.ClearPathBitVal) = eMenuEnable.ClearPathBitVal) And Not nRobotType = eArmType.P500
        Dim bTempEnable4 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.DisableMoveAllInAuto) = eMenuEnable.DisableMoveAllInAuto)
        'all robots are at home and can move to the same SP
        Dim bTempEnable5 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.MovePurgeAllBitVAl) = eMenuEnable.MovePurgeAllBitVAl)
        'all robots are in the same SP and can move home
        Dim bTempEnable6 As Boolean = bTempEnable And ((nMenuEnable And eMenuEnable.MoveHomeAllBitVAl) = eMenuEnable.MoveHomeAllBitVAl)

        sMnuName = "mnuMoveTo"
        oMnu.Items(sMnuName).Enabled = bTempEnable1 Or bTempEnable2 Or bTempEnable3 Or bTempEnable4
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)

        'If this menu is for an opener, hide the MoveTo choices that don't aspply
        oSubMnu.DropDownItems(sMnuName & "CleanOut").Visible = Not IsOpener
        oSubMnu.DropDownItems(sMnuName & "CleanIn").Visible = Not IsOpener
        oSubMnu.DropDownItems(sMnuName & "Purge").Visible = Not IsOpener
        oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Visible = Not IsOpener

        'Don't want move all for P1000s or P700s doing box interiors.
        'JZ 12132016 - Only one robot doesn't need to have move all.
        If frmMain.colArms.Count = 1 Then
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Visible = False
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Enabled = bTempEnable6
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Visible = False
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Visible = False
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Visible = False
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Visible = False
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Enabled = bTempEnable5
        Else
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Visible = True
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Enabled = bTempEnable6
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Visible = True
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Visible = True
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Visible = True
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Enabled = bTempEnable5
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Visible = True
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Enabled = bTempEnable5
        End If

        oSubMnu.DropDownItems("mnuMoveSeparator0").Visible = True
        'Uselsess move to item.
        oSubMnu.DropDownItems(sMnuName & "CleanOut").Visible = False
        oSubMnu.DropDownItems(sMnuName & "CleanOut").Enabled = False

        oSubMnu.DropDownItems(sMnuName & "Home").Enabled = bTempEnable1
        oSubMnu.DropDownItems(sMnuName & "Bypass").Enabled = bTempEnable2
        oSubMnu.DropDownItems(sMnuName & "Special1").Enabled = bTempEnable2
        oSubMnu.DropDownItems(sMnuName & "Special2").Enabled = bTempEnable2
        oSubMnu.DropDownItems(sMnuName & "CleanIn").Enabled = bTempEnable2
        oSubMnu.DropDownItems(sMnuName & "Purge").Enabled = bTempEnable2
        oSubMnu.DropDownItems(sMnuName & "ClearPath").Enabled = bTempEnable3
        oSubMnu.DropDownItems("mnuMoveSeparator1").Visible = Not nRobotType = eArmType.P500 And Not nRobotType = eArmType.P250 'NRU 160922 Added 250
        oSubMnu.DropDownItems(sMnuName & "ClearPath").Visible = Not nRobotType = eArmType.P500 And Not nRobotType = eArmType.P250 'NRU 160922 Added 250


        '******* Move menus end *********************************************************************

        '******* Jog menu ***************************************************************************
        bTempEnable = ((nMenuEnable And eMenuEnable.AxisJogBitVal) = eMenuEnable.AxisJogBitVal)
        oMnu.Items("mnuJog").Enabled = bTempEnable
        '******* Jog menu end ***********************************************************************

        '******* Cancel/Continue menu ***************************************************************
        bTempEnable1 = bAutoMode And ((nMenuEnable And eMenuEnable.CancContBitVal) = eMenuEnable.CancContBitVal)
        bTempEnable2 = bAutoMode And ((nMenuEnable And eMenuEnable.CancOnlyBitVal) = eMenuEnable.CancOnlyBitVal)
        bTempEnable3 = bAutoMode And ((nMenuEnable And eMenuEnable.CancOnlyAllBitVal) = eMenuEnable.CancOnlyAllBitVal) 'JZ 12152016 - Add concel only for all robots.
        bTempEnable4 = bAutoMode And ((nMenuEnable And eMenuEnable.CancContAllBitVal) = eMenuEnable.CancContAllBitVal)  'JZ 12152016 - Add concel/continue for all robots.
        sMnuName = "mnuCancCont"
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.Enabled = bTempEnable1 Or bTempEnable2 Or ((nMenuEnable And eMenuEnable.ClearPathContinueBitVal) = eMenuEnable.ClearPathContinueBitVal)
        'JZ 12142016 - Only one robot doesn't need to have cancel/continue all.
        If frmMain.colArms.Count = 1 Then
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Visible = False
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Enabled = bTempEnable3 Or bTempEnable4
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Visible = False
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Enabled = bTempEnable4 Or ((nMenuEnable And eMenuEnable.ClearPathContinueBitVal) = eMenuEnable.ClearPathContinueBitVal)
        Else
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Visible = True
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Enabled = bTempEnable3 Or bTempEnable4
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Visible = True
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Enabled = bTempEnable4 Or ((nMenuEnable And eMenuEnable.ClearPathContinueBitVal) = eMenuEnable.ClearPathContinueBitVal)
        End If
        oSubMnu.DropDownItems("mnuCancel").Enabled = bTempEnable1 Or bTempEnable2
        'oSubMnu.DropDownItems("mnuCancel" & gsALL).Enabled = bTempEnable1 Or bTempEnable2
        oSubMnu.DropDownItems("mnuContinue").Enabled = bTempEnable1 Or ((nMenuEnable And eMenuEnable.ClearPathContinueBitVal) = eMenuEnable.ClearPathContinueBitVal)
        'oSubMnu.DropDownItems("mnuContinue" & gsALL).Enabled = bTempEnable1
        '******* Cancel/Continue menu end ***********************************************************

        '******* Process menu ***********************************************************************
        oMnu.Items("mnuProcess").Enabled = True
        '******* Process menu end *******************************************************************

        '******* Life Cycles menu *******************************************************************
        oMnu.Items("mnuLifeCycle").Enabled = True
        '******* Life Cycles menu end ***************************************************************

        '******* Manual functions menu **************************************************************
        bTempEnable = bManualMode And ((nMenuEnable And eMenuEnable.ManFuncBitVal) = eMenuEnable.ManFuncBitVal)
        sMnuName = "mnuManFunc"
        oMnu.Items(sMnuName).Enabled = bTempEnable
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "SuperPurge").Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "SuperPurgeAll").Enabled = bTempEnable
        'JZ 12162016 - Only one robot doesn't need to have super purge all.
        If frmMain.colArms.Count = 1 Then
            oSubMnu.DropDownItems(sMnuName & "SuperPurgeAll").Visible = False
        Else
            oSubMnu.DropDownItems(sMnuName & "SuperPurgeAll").Visible = True
        End If
        '******* Manual functions menu end **********************************************************

        '******* Conveyor interlock menu ************************************************************
        bTempEnable = ((nMenuEnable And eMenuEnable.ConvIntlkBitVal) = eMenuEnable.ConvIntlkBitVal)
        bTempEnable1 = ((nRunStat And eRobotRunStat.ConvBypassed) = eRobotRunStat.ConvBypassed)
        sMnuName = "mnuConvIntlk"
        oMnu.Items(sMnuName).Enabled = bTempEnable
        oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        oSubMnu.Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "Bypass").Enabled = Not bTempEnable1
        oSubMnu.DropDownItems(sMnuName & "Enable").Enabled = bTempEnable1
        oSubMnu.DropDownItems(sMnuName & "BypassAll").Enabled = bTempEnable
        oSubMnu.DropDownItems(sMnuName & "EnableAll").Enabled = bTempEnable
        'JZ 12162016 - Only one robot doesn't need to have comveyor intlk bypass all.
        If frmMain.colArms.Count = 1 Then
            oSubMnu.DropDownItems(sMnuName & "BypassAll").Visible = False
            oSubMnu.DropDownItems(sMnuName & "EnableAll").Visible = False
        Else
            oSubMnu.DropDownItems(sMnuName & "BypassAll").Visible = True
            oSubMnu.DropDownItems(sMnuName & "EnableAll").Visible = True
        End If
        '******* Conveyor interlock menu end ********************************************************

        '******* Release TP As Enabling Device menu *************************************************
        bTempEnable = ((nMenuEnable And eMenuEnable.RelTpAsEnablingDev) = eMenuEnable.RelTpAsEnablingDev)
        oMnu.Items("mnuRelTpAsEnablingDev").Enabled = bTempEnable
        oMnu.Items("mnuRelTpAsEnablingDev").Visible = Not nRobotType = eArmType.P250 'NRU 160927 Added 250
        '******* Release TP As Enabling Device menu end *********************************************

        '******* Isolate Robot Arm menus ************************************************************
        'bTempEnable = True '((nRobotStat And eRobotStat.BypassLS) = eRobotStat.BypassLS) 'enable only when on Bypass LS
        bTempEnable1 = ((nRunStat And eRobotRunStat.Isolated) = eRobotRunStat.Isolated)
        bTempEnable = ((nRunStat And eRobotRunStat.ConvBypassed) = eRobotRunStat.ConvBypassed)
        oMnu.Items("mnuIsolate").Enabled = bTempEnable
        oSubMnu = DirectCast(oMnu.Items("mnuIsolate"), ToolStripMenuItem)
        oSubMnu.Enabled = bTempEnable
        oSubMnu.DropDownItems("mnuIsolateEnable").Enabled = bTempEnable And Not bTempEnable1
        oSubMnu.DropDownItems("mnuIsolateDisable").Enabled = bTempEnable1
        oSubMnu.Visible = Not nRobotType = eArmType.P250 'NRU 160927 Added 250
        '******* Isolate Robot Arm menus end ********************************************************

        '******* Easy button menu *******************************************************************
        'bTempEnable = ((nRunStat And eRobotRunStat.Running) = 0) 'enable only when faulted
        oMnu.Items("mnuEasyButton").Enabled = True
        '******* Easy button menu end ***************************************************************

        '******* Igonore Opener Input menu *******************************************************************
        bTempEnable = ((nRunStat And eRobotRunStat.InCycle) = eRobotRunStat.InCycle) 'enable only when on Bypass LS
        oMnu.Items("mnuIgnoreOpenerInput").Visible = IsOpener
        oMnu.Items("mnuSeparator9").Visible = IsOpener
        oMnu.Items("mnuIgnoreOpenerInput").Enabled = bTempEnable
        '******* Ignore Opener menu end ***************************************************************

        ''******* Maitenance Mode menu *****************************************************************
        'bTempEnable = nGetMaintModeEnableState(nDataBlock) And (nMenuEnable And eMenuEnable.MaintDisableEnbBitVal) = eMenuEnable.MaintDisableEnbBitVal
        'sMnuName = "mnuMaintMode"
        'oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
        'oSubMnu.Enabled = True
        'oSubMnu.Visible = Not frmMain.colArms(frmMain.colArms(nDataBlock - 1).ArmNumber).ArmType = eArmType.P200
        'oMnu.Items("mnuSeparator10").Visible = Not frmMain.colArms(frmMain.colArms(nDataBlock - 1).ArmNumber).ArmType = eArmType.P200
        'oSubMnu.DropDownItems("mnuMaintModeDisable").Enabled = bTempEnable
        'oSubMnu.DropDownItems("mnuMaintModeEnable").Enabled = Not bTempEnable

        'bTempEnable = ((nMenuEnable And eMenuEnable.MaintMoveFromHomeBitVal) = eMenuEnable.MaintMoveFromHomeBitVal) Or ((nMenuEnable And eMenuEnable.MaintMoveToHomeBitVal) = eMenuEnable.MaintMoveToHomeBitVal)
        'sMnuName = "mnuMaintMoveTo"
        'Dim oSubMnu2 As ToolStripMenuItem = DirectCast(oSubMnu.DropDownItems(sMnuName), ToolStripMenuItem)
        'oSubMnu2.Enabled = bTempEnable

        'bTempEnable = ((nMenuEnable And eMenuEnable.MaintMoveFromHomeBitVal) = eMenuEnable.MaintMoveFromHomeBitVal)
        'sMnuName = "mnuMaintPos"
        'oSubMnu2.DropDownItems(sMnuName).Enabled = bTempEnable

        'bTempEnable = ((nMenuEnable And eMenuEnable.MaintMoveToHomeBitVal) = eMenuEnable.MaintMoveToHomeBitVal)
        'sMnuName = "mnuMaintToHome"
        'oSubMnu2.DropDownItems(sMnuName).Enabled = bTempEnable

        '******* Maitenance Mode menu end *************************************************************

    End Sub

    Friend Sub EstopPBChange(ByRef vUCtl As UserControl, ByVal sData As String, _
                            ByVal HowMany As Integer, Optional ByVal vgpb As GroupBox = Nothing)
        '********************************************************************************************
        'Description: Estops for the booth
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim nData As Integer = CInt(sData)
            Dim p As PictureBox = Nothing
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)


            For i As Integer = 0 To HowMany - 1
                Dim sName As String = "picEstop" & Format(i, "00")
                p = DirectCast(pnlTmp.Controls(sName), PictureBox)
                If ((p Is Nothing) = True) And ((vgpb Is Nothing) = False) Then
                    p = DirectCast(vgpb.Controls(sName), PictureBox)
                End If
                If IsNothing(p) = False Then
                    Dim o As ImageList = DirectCast(colImages.Item(p.Tag), ImageList)

                    If (nData And gnBitVal(i)) = gnBitVal(i) Then
                        p.Image = o.Images.Item("e_out")
                    Else
                        p.Image = o.Images.Item("e_in")
                    End If
                End If  'IsNothing(p) = False
            Next


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Friend Sub subWaterHeaterGpbMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description: mousedown in a water heater box
        '
        'Parameters: event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'get the number at the end of the name, and force the menu for left-clicks
        Dim oGpb As GroupBox = DirectCast(sender, GroupBox)
        If oGpb IsNot Nothing Then
            mnWaterHeaterNumber = CType(oGpb.Name.Substring(oGpb.Name.Length - 1), Integer)
            If (mnWaterHeaterNumber > 0) And (e.Button = Windows.Forms.MouseButtons.Left) Then
                'Manually launch the menu
                Dim oPoint As Point = New Point(e.X, e.Y)
                oGpb.ContextMenuStrip.Show(oGpb.PointToScreen(oPoint))
            End If
        End If
    End Sub
    Friend Sub subWaterHeaterLblMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description: mousedown in a water heater box
        '
        'Parameters: event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'get the number at the end of the name, and force the menu for left-clicks
        Dim oLbl As Label = DirectCast(sender, Label)
        If oLbl IsNot Nothing Then
            mnWaterHeaterNumber = CType(oLbl.Name.Substring(oLbl.Name.Length - 1), Integer)
            If (mnWaterHeaterNumber > 0) And (e.Button = Windows.Forms.MouseButtons.Left) Then
                'Manually launch the menu
                Dim oPoint As Point = New Point(e.X, e.Y)
                oLbl.ContextMenuStrip.Show(oLbl.PointToScreen(oPoint))
            End If
        End If
    End Sub
    Friend Sub subWaterHeaterPicMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description: mousedown in a water heater box
        '
        'Parameters: event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'get the number at the end of the name, and force the menu for left-clicks
        Dim oPic As PictureBox = DirectCast(sender, PictureBox)
        If oPic IsNot Nothing Then
            mnWaterHeaterNumber = CType(oPic.Name.Substring(oPic.Name.Length - 1), Integer)
            If (mnWaterHeaterNumber > 0) And (e.Button = Windows.Forms.MouseButtons.Left) Then
                If e.Button = Windows.Forms.MouseButtons.Left Then
                    'Manually launch the menu
                    Dim oPoint As Point = New Point(e.X, e.Y)
                    oPic.ContextMenuStrip.Show(oPic.PointToScreen(oPoint))
                End If
            End If
        End If
    End Sub
    Friend Sub mnuOnOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: OnOff pop-up menu
        '
        'Parameters: event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Check on or off first
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim bOnOff As Boolean = False
        Select Case oMnu.Name
            Case "mnuOn"
                bOnOff = True
            Case "mnuOff"
                bOnOff = false
        End Select
        'Was a water heater box selected?
        If mnWaterHeaterNumber > 0 Then
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "WaterHeaterEnable"
            Dim sData() As String = frmMain.mPLC.PLCData
            If (sData IsNot Nothing) AndAlso sData(0) <> String.Empty Then
                Dim nData As Integer = CType(sData(0), Integer)
                If bOnOff Then
                    nData = nData And Not (CInt(2 ^ (mnWaterHeaterNumber + 7)))
                    nData = nData Or CInt(2 ^ (mnWaterHeaterNumber - 1))
                Else
                    nData = nData And Not (CInt(2 ^ (mnWaterHeaterNumber - 1)))
                    nData = nData Or CInt(2 ^ (mnWaterHeaterNumber + 7))
                End If
                'write to the PLC
                ReDim sData(0)
                sData(0) = nData.ToString
                frmMain.mPLC.PLCData = sData
            End If
        End If
    End Sub
    Private Sub chkValve_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: Valve selection checkbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim oChk As CheckBox = Nothing
        oChk = DirectCast(sender, CheckBox)
        Dim nValve As Integer = CInt(oChk.Name.Substring(8))
        frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "PaintMixManualRequest"
        Dim sData() As String = frmMain.mPLC.PLCData
        Dim bOnOff As Boolean = False
        'Check state is changed by click
        If (sData IsNot Nothing) AndAlso sData(0) <> String.Empty Then
            Dim nData As Integer = CType(sData(0), Integer)
            If ((nData And CInt(2 ^ (nValve))) <> 0) Then
                bOnOff = False
            Else
                bOnOff = True
            End If
            If bOnOff Then
                nData = nData Or CInt(2 ^ (nValve))
                oChk.CheckState = CheckState.Checked
            Else
                nData = nData And Not (CInt(2 ^ (nValve)))
                'See if it should be unchecked (not requested) or indeterminate (automatically requested)
                If (frmMain.msZoneData IsNot Nothing) AndAlso (frmMain.msZoneData.GetUpperBound(0) >= eBooth.PaintMixOutWd) Then
                    Dim nPaintMixOutWd As Integer = CInt(frmMain.msZoneData(eBooth.PaintMixOutWd))
                    Dim bOut As Boolean = ((nPaintMixOutWd And gnBitVal(nValve)) = gnBitVal(nValve))
                    If bOut Then
                        oChk.CheckState = CheckState.Indeterminate
                    Else
                        oChk.CheckState = CheckState.Unchecked
                    End If

                End If
            End If
            'write to the PLC
            ReDim sData(0)
            sData(0) = nData.ToString
            frmMain.mPLC.PLCData = sData
        End If
    End Sub
    Friend Sub InitCommonText(ByRef rUctl As UserControl, _
                              Optional ByVal nNumPreDetQPos As Integer = 0, _
                              Optional ByVal nNumPostDetQPos As Integer = 0, _
                              Optional ByRef oMnu As ContextMenuStrip = Nothing, _
                              Optional ByVal nNumPostQPanels As Integer = 0, _
                              Optional ByRef oMnuOnOff As ContextMenuStrip = Nothing)
        '********************************************************************************************
        'Description: Init labels, setup menus common to booth and queue screens
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/05/14  MSW     ZDT Changes 03/05/14
        '********************************************************************************************
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim oController As clsController = frmMain.colControllers.Item(0) 'NRU 160824 1 to 0
        Dim bV70 As Boolean

        'BTK 03/04/10 Added so we can change the special move number we send to the robot based on controller
        'software version number.  If the software is not above 7.0 then bypass, clean in and clean out
        'numbers sent to the robot need to change.
        Dim nPanels As Integer = colZones.ActiveZone.RepairPanels
        If nPanels > 0 Then
            mnRepairPanelMask = CInt(2 ^ nPanels) - 1
        Else
            mnRepairPanelMask = 0
        End If

        bV70 = True
        If oController.Version < 7 Then
            bV70 = False
        End If
        With gpsRM
            'Make some common resource calls only once to speed things up
            Dim sAll As String = .GetString("psALL", oCulture)
            Dim sDisable As String = .GetString("psDISABLE", oCulture)
            Dim sEnable As String = .GetString("psENABLE", oCulture)

            Dim pnlTmp As Panel = DirectCast(rUctl.Controls("pnlMain"), Panel)
            ' 03/24/14  MSW     ZDT Changes
            '********************************
            oZDTBtn = DirectCast(pnlTmp.Controls("btnZDTAlert"), Button)
            If oZDTBtn IsNot Nothing Then
                If frmMain.oZDT IsNot Nothing AndAlso frmMain.oZDT.Enabled Then
                    oZDTBtn.Visible = True
                    'Click handler for detail screen
                    AddHandler oZDTBtn.Click, AddressOf ZDTAlertClick
                    'Update text, color
                    subUpdateZDT(frmMain.oZDT.StatusText, frmMain.oZDT.StatusColor)
                Else
                    oZDTBtn.Visible = False
                End If
            End If
            '********************************

            'Top Banner left - Conveyor
            Dim pbTmp As GroupBox = DirectCast(pnlTmp.Controls("gpbStat01"), GroupBox)
            Dim lblTmp As Label = DirectCast(pbTmp.Controls("lblJobCountCap"), Label)
            lblTmp.Text = .GetString("psJOB_COUNT", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblJobCount"), Label)
            lblTmp.Text = "0"
            lblTmp = DirectCast(pbTmp.Controls("lblConvSpdCap"), Label)
            lblTmp.Text = .GetString("psCONV_SPEED", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblConvSpd"), Label)
            lblTmp.Text = "0"
            lblTmp = DirectCast(pbTmp.Controls("lblSpeedCap"), Label)
            lblTmp.Text = .GetString("psMM_S", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblConvSpd2"), Label)
            lblTmp.Text = "0"
            lblTmp = DirectCast(pbTmp.Controls("lblSpeedCap2"), Label)
            lblTmp.Text = .GetString("psFT_MIN", oCulture)
            'Top Banner Middle - Mode
            pbTmp = DirectCast(pnlTmp.Controls("gpbMode"), GroupBox)
            lblTmp = DirectCast(pbTmp.Controls("lblBoothModeCap"), Label)
            lblTmp.Text = .GetString("psBOOTHMODE", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblBoothMode"), Label)
            lblTmp.Text = .GetString("psUNKNOWN", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblConvStatCap"), Label)
            lblTmp.Text = .GetString("psCONV_STAT", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblConvStat"), Label)
            lblTmp.Text = .GetString("psUNKNOWN", oCulture)
            'Top Banner Right - Interlocks
            Dim sLbl As String = "lblIntlk"
            Dim sPic As String = "picIntlk"
            Dim sName As String = "psINTERLOCKCAP"
            Dim sTmp As String
            Dim bTeach As Boolean = InStr(colZones.ActiveZone.Name.ToLower, "teach") > 0
            Dim bShow As Boolean = True
            pbTmp = DirectCast(pnlTmp.Controls("gpbLED"), GroupBox)
            If Not (pbTmp Is Nothing) Then
                For i As Integer = 1 To 6

                    sTmp = .GetString(sName & i.ToString, oCulture)
                    pbTmp.Controls(sLbl & Format(i, "00")).Text = sTmp
                    If bTeach Then
                        Select Case i
                            Case 2, 3, 4 : bShow = False
                            Case Else : bShow = True
                        End Select
                    Else
                        bShow = True
                    End If
                    If Len(sTmp) > 0 Then
                        pbTmp.Controls(sLbl & Format(i, "00")).Visible = bShow
                        pbTmp.Controls(sPic & Format(i, "00")).Visible = bShow
                    End If
                Next
            End If
            'SOC box
            pbTmp = DirectCast(pnlTmp.Controls("gpbSOC"), GroupBox)
            pbTmp.Text = .GetString("psSOC", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblSOCEstop"), Label)
            lblTmp.Text = .GetString("psESTOP", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblSOCEstat"), Label)
            'lblTmp.Text = .GetString("psESTAT_DISC", oCulture)
            lblTmp = DirectCast(pbTmp.Controls("lblConvRunSS"), Label)
            lblTmp.Text = .GetString("psCONV", oCulture)
            'lblTmp = DirectCast(pbTmp.Controls("lblSOCProcess"), Label)
            'lblTmp.Text = .GetString("psPROC_AIR", oCulture)


            'Paint mix box
            Dim g As GroupBox = DirectCast(pnlTmp.Controls("gpbPaintMix"), GroupBox)
            If g IsNot Nothing Then
                g.Text = .GetString("psPAINTMIXBOX", oCulture)
                Dim oColors As xmlnodelist = Nothing
                Dim sColorValveName() As String = Nothing
                Dim bAsciiColor As Boolean = False
                Dim bColorsByStyle As Boolean = False
                Dim bUse2K As Boolean = False
                Dim bUseTricoat As Boolean = False
                Dim nPlantAsciiMaxLength As Integer = 0
                Dim bTwoCoats As Boolean = False
                If GetSystemColorInfoFromDB(colZones.ActiveZone, oColors, _
                                            sColorValveName, bAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, nPlantAsciiMaxLength, bTwoCoats) Then
                    'Valves


                    Dim nValve As Integer = 0
                    Dim bDone As Boolean = False
                    Dim oChk As CheckBox = Nothing
                    Dim nNumValves As Integer = sColorValveName.GetUpperBound(0) + 1
                    Do
                        oChk = DirectCast(g.Controls("chkValve" & nValve.ToString("00")), CheckBox)
                        If oChk IsNot Nothing Then
                            AddHandler oChk.Click, AddressOf chkValve_Click
                            If nValve < nNumValves Then
                                oChk.Text = sColorValveName(nValve)
                            End If
                        Else
                            bDone = True
                        End If
                        nValve = nValve + 1
                    Loop Until bDone
                End If
            End If

            'Conveyor Estop - No box
            lblTmp = DirectCast(pnlTmp.Controls("lblConvEstop"), Label)
            If lblTmp IsNot Nothing Then
                lblTmp.Text = .GetString("psCONV", oCulture)
            End If
            Dim picTmp As PictureBox = Nothing
            Dim pnlJobTmp As Panel = DirectCast(pnlTmp.Controls("pnlJob01"), Panel)
            If pnlJobTmp IsNot Nothing Then
                Dim nTmpInt As Integer = (pnlJobTmp.Top + (pnlJobTmp.Height \ 2))
                picTmp = DirectCast(pnlTmp.Controls("picArrow01"), PictureBox)
                nTmpInt = nTmpInt - (picTmp.Height \ 2)
                picTmp.Top = nTmpInt
                picTmp = DirectCast(pnlTmp.Controls("picArrow02"), PictureBox)
                picTmp.Top = nTmpInt
            End If
            'lblTmp = DirectCast(pnlJobTmp.Controls("lblColor01"), Label)
            'lblTmp.Width = 75


            'Water Heater box
            pbTmp = DirectCast(pnlTmp.Controls("gpbWaterHeater1"), GroupBox)
            Dim oPic As PictureBox
            If pbTmp IsNot Nothing Then
                pbTmp.Text = .GetString("psWATER_HEATER_1", oCulture)
                AddHandler pbTmp.MouseDown, AddressOf subWaterHeaterGpbMouseDown
                'lblTmp = DirectCast(pbTmp.Controls("lblWHEnab1"), Label)
                'lblTmp.Text = .GetString("psWH_ENABLE", oCulture)
                'AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHStart1"), Label)
                lblTmp.Text = .GetString("psWH_START", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHLevel1"), Label)
                lblTmp.Text = .GetString("psWH_LEVEL", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHTemp1"), Label)
                lblTmp.Text = .GetString("psWH_TEMP", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                'oPic = DirectCast(pbTmp.Controls("picWHEnable1"), PictureBox)
                'AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHStart1"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHLevel1"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHTemp1"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
            End If
            pbTmp = DirectCast(pnlTmp.Controls("gpbWaterHeater2"), GroupBox)
            If pbTmp IsNot Nothing Then
                pbTmp.Text = .GetString("psWATER_HEATER_2", oCulture)
                AddHandler pbTmp.MouseDown, AddressOf subWaterHeaterGpbMouseDown
                'lblTmp = DirectCast(pbTmp.Controls("lblWHEnab2"), Label)
                'lblTmp.Text = .GetString("psWH_ENABLE", oCulture)
                'AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHStart2"), Label)
                lblTmp.Text = .GetString("psWH_START", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHLevel2"), Label)
                lblTmp.Text = .GetString("psWH_LEVEL", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHTemp2"), Label)
                lblTmp.Text = .GetString("psWH_TEMP", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                'oPic = DirectCast(pbTmp.Controls("picWHEnable2"), PictureBox)
                'AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHStart2"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHLevel2"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHTemp2"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
            End If

            ''''''''''''''''''''''''''''HAF 041514
            pbTmp = DirectCast(pnlTmp.Controls("gpbWaterHeater3"), GroupBox)
            If pbTmp IsNot Nothing Then
                pbTmp.Text = .GetString("psWATER_HEATER_3", oCulture)
                AddHandler pbTmp.MouseDown, AddressOf subWaterHeaterGpbMouseDown
                'lblTmp = DirectCast(pbTmp.Controls("lblWHEnab2"), Label)
                'lblTmp.Text = .GetString("psWH_ENABLE", oCulture)
                'AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHStart3"), Label)
                lblTmp.Text = .GetString("psWH_START", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHLevel3"), Label)
                lblTmp.Text = .GetString("psWH_LEVEL", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHTemp3"), Label)
                lblTmp.Text = .GetString("psWH_TEMP", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                'oPic = DirectCast(pbTmp.Controls("picWHEnable2"), PictureBox)
                'AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHStart3"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHLevel3"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHTemp3"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
            End If
            pbTmp = DirectCast(pnlTmp.Controls("gpbWaterHeater4"), GroupBox)
            If pbTmp IsNot Nothing Then
                pbTmp.Text = .GetString("psWATER_HEATER_4", oCulture)
                AddHandler pbTmp.MouseDown, AddressOf subWaterHeaterGpbMouseDown
                'lblTmp = DirectCast(pbTmp.Controls("lblWHEnab2"), Label)
                'lblTmp.Text = .GetString("psWH_ENABLE", oCulture)
                'AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHStart4"), Label)
                lblTmp.Text = .GetString("psWH_START", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHLevel4"), Label)
                lblTmp.Text = .GetString("psWH_LEVEL", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                lblTmp = DirectCast(pbTmp.Controls("lblWHTemp4"), Label)
                lblTmp.Text = .GetString("psWH_TEMP", oCulture)
                AddHandler lblTmp.MouseDown, AddressOf subWaterHeaterLblMouseDown
                'oPic = DirectCast(pbTmp.Controls("picWHEnable2"), PictureBox)
                'AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHStart4"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHLevel4"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
                oPic = DirectCast(pbTmp.Controls("picWHTemp4"), PictureBox)
                AddHandler oPic.MouseDown, AddressOf subWaterHeaterPicMouseDown
            End If
            '''''HAF End
            'Dim oMnuOnOff As ContextMenuStrip = DirectCast(rUctl.Controls("mnuOnOff"), ContextMenuStrip)
            Dim oSubMnu As ToolStripMenuItem = Nothing
            If oMnuOnOff IsNot Nothing Then
                oSubMnu = DirectCast(oMnuOnOff.Items("mnuOn"), ToolStripMenuItem)
                oSubMnu.Text = .GetString("psMNU_ON", oCulture)
                AddHandler oSubMnu.Click, AddressOf mnuOnOff_Click
                oSubMnu = DirectCast(oMnuOnOff.Items("mnuOff"), ToolStripMenuItem)
                oSubMnu.Text = .GetString("psMNU_OFF", oCulture)
                AddHandler oSubMnu.Click, AddressOf mnuOnOff_Click
            End If


            'Fortress box
            'pbTmp = DirectCast(pnlTmp.Controls("gpbFortressLeft"), GroupBox)
            'pbTmp.Text = .GetString("psFORTRESS", oCulture)
            'pbTmp = DirectCast(pnlTmp.Controls("gpbFortressRight"), GroupBox)
            'pbTmp.Text = .GetString("psFORTRESS", oCulture)
            'Jello Boxes
            Dim sZone As String = colZones.ActiveZone.Name.ToLower
            Dim bEntInt As Boolean = True
            Dim bExitInt As Boolean = True
            'If InStr(sZone, "teach") > 0 Then
            '    bEntInt = False
            '    bExitInt = False
            'ElseIf (InStr(sZone, "clear") > 0) Or (InStr(sZone, "cc") > 0) Then
            '    bEntInt = False
            '    bExitInt = True
            'ElseIf (InStr(sZone, "prime") > 0) Or (InStr(sZone, "bcp") > 0) Then
            '    bEntInt = True
            '    bExitInt = True
            'Else
            '    bEntInt = False
            '    bExitInt = True
            'End If
            lblTmp = DirectCast(pnlTmp.Controls("lblEntIntData"), Label)
            If lblTmp IsNot Nothing Then
                lblTmp.Text = .GetString("psLBL_ENT_INT", oCulture)
                lblTmp.Visible = bEntInt
            End If
            lblTmp = DirectCast(pnlTmp.Controls("lblExitIntData"), Label)
            If lblTmp IsNot Nothing Then
                lblTmp.Text = .GetString("psLBL_EXIT_INT", oCulture)
                lblTmp.Visible = bExitInt
            End If
            Dim uctlTmp As uctlList = Nothing
            uctlTmp = DirectCast(pnlTmp.Controls("UctlEntInt"), uctlList)
            If uctlTmp IsNot Nothing Then
                uctlTmp.Visible = bEntInt
            End If
            uctlTmp = Nothing
            uctlTmp = DirectCast(pnlTmp.Controls("UctlExitInt"), uctlList)
            If uctlTmp IsNot Nothing Then
                uctlTmp.Visible = bExitInt
            End If
            'Predetect Q boxes
            Dim oUctlList As uctlList
            If nNumPreDetQPos > 0 Then
                For nLbl As Integer = 1 To (nNumPreDetQPos)
                    lblTmp = DirectCast(pnlTmp.Controls("lblPreQ" & nLbl.ToString), Label)
                    lblTmp.Text = .GetString("psQUE_POS", oCulture) & " " & nLbl.ToString
                    oUctlList = DirectCast(pnlTmp.Controls("UctlPreQ" & nLbl.ToString("00")), uctlList)
                    AddHandler oUctlList.Mouse_Click, AddressOf subEditQueue
                    oUctlList.LabelHeight = 15
                    oUctlList.LabelSpacing = 0
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        oUctlList.NumLabels = gnQUE_LABELS + 1
                    Else
                        oUctlList.NumLabels = gnQUE_LABELS - 1 'NRU 161007 No Tutone
                    End If
                    oUctlList.Size = New System.Drawing.Size(100, oUctlList.NumLabels * 16)
                Next
            End If
            'PostDetect Q boxes
            If nNumPostDetQPos > 0 Then
                For nLbl As Integer = 1 To (nNumPostDetQPos)
                    lblTmp = DirectCast(pnlTmp.Controls("lblPostQ" & nLbl.ToString), Label)
                    lblTmp.Text = .GetString("psQUE_POS", oCulture) & " " & nLbl.ToString
                    oUctlList = DirectCast(pnlTmp.Controls("UctlPostQ" & nLbl.ToString("00")), uctlList)
                    AddHandler oUctlList.Mouse_Click, AddressOf subEditQueue
                    oUctlList.LabelHeight = 15
                    oUctlList.LabelSpacing = 0
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        oUctlList.NumLabels = gnQUE_LABELS + 1
                    Else
                        oUctlList.NumLabels = gnQUE_LABELS
                    End If
                    oUctlList.Size = New System.Drawing.Size(85, oUctlList.NumLabels * 16)
                Next
            End If
            If nNumPostQPanels > 0 Then
                Dim bRepairEnable As Boolean = (colZones.ActiveZone.RepairPanels > 0)
                Dim sNum As String = String.Empty
                Dim pnl2Tmp As Panel = Nothing
                For nLbl As Integer = 1 To (nNumPostQPanels)
                    sNum = nLbl.ToString("00")
                    pnl2Tmp = DirectCast(pnlTmp.Controls("pnlJob" & sNum), Panel)
                    lblTmp = DirectCast(pnl2Tmp.Controls("lblRepair" & sNum), Label)
                    lblTmp.Visible = bRepairEnable
                Next
            End If
            If (InStr(colZones.ActiveZone.Name.ToLower(), "bcp") > 0) Then
                Dim tmpPic As PictureBox = Nothing
                tmpPic = DirectCast(pnlTmp.Controls("picDetectProx4"), PictureBox)
                If IsNothing(tmpPic) = False Then
                    tmpPic.Visible = False
                End If  'IsNothing(tmpPic) = False
                Dim tmpLabel As Label = Nothing
                tmpLabel = DirectCast(pnlTmp.Controls("lblDetect2"), Label)
                If IsNothing(tmpLabel) = False Then
                    tmpLabel.Visible = False
                End If  'IsNothing(tmpLabel) = False
            End If


            'Booth only stuff after this, check for the context menu, exit if it's not there
            'It looks like mnuRobotContext doesn't go in the collection, so Now it gets passed in.
            'Dim oMnu As ContextMenuStrip = DirectCast(rUctl.Controls("mnuRobotContext"), ContextMenuStrip)
            If oMnu Is Nothing Then
                Exit Sub
            End If
            'Pop-up menu setup
            'This isn't like the old way where it was built on the fly. The menus are defined in the 
            'booth and Queue usercontrols. This sets the text and adds tags to make it easier later.
            ''''''''''Applicator menus
            Dim eMnuTag(2) As eMnuFctn
            Dim sMnuName As String = "mnuTriggerEnable"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psTRIGGER", oCulture)
            eMnuTag(0) = eMnuFctn.Trigger
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsON).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Disable
            oSubMnu.DropDownItems(sMnuName & gsOFF).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF).Text = sDisable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Text = sDisable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Enable
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Text = sEnable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Click, AddressOf mnuRobotPopup_Click

            sMnuName = "mnuAppEnable"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psAPPLICATOR", oCulture)
            eMnuTag(0) = eMnuFctn.Applicator
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsON).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Disable
            oSubMnu.DropDownItems(sMnuName & gsOFF).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF).Text = sDisable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Text = sDisable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Enable
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Text = sEnable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Click, AddressOf mnuRobotPopup_Click

            sMnuName = "mnuEstatEnable"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psESTAT", oCulture)
            eMnuTag(0) = eMnuFctn.Estat
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsON).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Disable
            oSubMnu.DropDownItems(sMnuName & gsOFF).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF).Text = sDisable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Text = sDisable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsOFF & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Enable
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Text = sEnable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsON & gsALL).Click, AddressOf mnuRobotPopup_Click
            ''''''''''Applicator menus end

            ''''''''''Move menus
            oMnu.Items("mnuMoveTo").Text = .GetString("psMOVETO", oCulture)
            sMnuName = "mnuMoveTo"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psMOVETO", oCulture)
            eMnuTag(0) = eMnuFctn.MoveTo
            eMnuTag(1) = eMnuFctn.Home
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "Home").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Home").Text = .GetString("psHOME", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "Home").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Text = .GetString("psHOME", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "Home" & gsALL).Click, AddressOf mnuRobotPopup_Click
            If bV70 Then
                eMnuTag(1) = eMnuFctn.Bypass
            Else
                eMnuTag(1) = eMnuFctn.V643Bypass
            End If
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "Bypass").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Bypass").Text = .GetString("psBYPASS", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "Bypass").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Text = .GetString("psBYPASS", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "Bypass" & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Special1
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "Special1").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Special1").Text = .GetString("psSPECIAL1", oCulture) 'BCD 10-08-14
            'oSubMnu.DropDownItems(sMnuName & "Special1").Text = .GetString("psSPECIAL", oCulture) & "1"
            AddHandler oSubMnu.DropDownItems(sMnuName & "Special1").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Text = .GetString("psSPECIAL1", oCulture) & sAll 'BCD 10-08-14
            'oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Text = .GetString("psSPECIAL", oCulture) & "1" & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "Special1" & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Special2
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "Special2").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Special2").Text = .GetString("psSPECIAL2", oCulture) 'BCD 10-08-14
            'oSubMnu.DropDownItems(sMnuName & "Special2").Text = .GetString("psSPECIAL", oCulture) & "2"
            AddHandler oSubMnu.DropDownItems(sMnuName & "Special2").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Text = .GetString("psSPECIAL", oCulture) & sAll 'BCD 10-08-14
            'oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Text = .GetString("psSPECIAL", oCulture) & "2" & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "Special2" & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Text = .GetString("psPURGE", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Click, AddressOf mnuRobotPopup_Click
            If bV70 Then
                eMnuTag(1) = eMnuFctn.CleanOut
            Else
                eMnuTag(1) = eMnuFctn.V643CleanOut
            End If
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "CleanOut").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "CleanOut").Text = .GetString("psCLEANOUT", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "CleanOut").Click, AddressOf mnuRobotPopup_Click
            If bV70 Then
                eMnuTag(1) = eMnuFctn.CleanIn
            Else
                eMnuTag(1) = eMnuFctn.V643CleanIn
            End If
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "CleanIn").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "CleanIn").Text = .GetString("psCLEANIN", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "CleanIn").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Purge
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "Purge").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "Purge").Text = .GetString("psPURGE", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "Purge").Click, AddressOf mnuRobotPopup_Click
            'eMnuTag(1) = eMnuFctn.Master
            'eMnuTag(2) = eMnuFctn.None
            'oSubMnu.DropDownItems(sMnuName & "Master").Tag = CopyFunctionArray(eMnuTag)
            'oSubMnu.DropDownItems(sMnuName & "Master").Text = .GetString("psMASTER_POS", oCulture)
            'AddHandler oSubMnu.DropDownItems(sMnuName & "Master").Click, AddressOf mnuRobotPopup_Click
            'eMnuTag(1) = eMnuFctn.Purge
            'eMnuTag(2) = eMnuFctn.All
            'oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            'oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Text = .GetString("psPURGE", oCulture) & " " & sAll
            'AddHandler oSubMnu.DropDownItems(sMnuName & "Purge" & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.ClearPath
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "ClearPath").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "ClearPath").Text = .GetString("psCLEARPATH", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "ClearPath").Click, AddressOf mnuRobotPopup_Click


            ''''''''''Move menus end

            ''''''''''Jog Menu
            eMnuTag(0) = eMnuFctn.Jog
            eMnuTag(1) = eMnuFctn.None
            eMnuTag(2) = eMnuFctn.None
            oMnu.Items("mnuJog").Tag = CopyFunctionArray(eMnuTag)
            oMnu.Items("mnuJog").Text = .GetString("psJOG", oCulture)
            AddHandler oMnu.Items("mnuJog").Click, AddressOf mnuRobotPopup_Click

            ''''''''''Cancel/Continue menu
            sMnuName = "mnuCancCont"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psCANCCONT", oCulture)
            eMnuTag(0) = eMnuFctn.RecCancel
            eMnuTag(1) = eMnuFctn.None
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems("mnuCancel").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems("mnuCancel").Text = .GetString("psCANCEL", oCulture)
            AddHandler oSubMnu.DropDownItems("mnuCancel").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.All
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems("mnuCancel" & gsALL).Text = .GetString("psCANCEL", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems("mnuCancel" & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(0) = eMnuFctn.RecContinue
            eMnuTag(1) = eMnuFctn.None  'JZ 12152016 - Make the individual robot continue.
            oSubMnu.DropDownItems("mnuContinue").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems("mnuContinue").Text = .GetString("psCONTINUE", oCulture)
            AddHandler oSubMnu.DropDownItems("mnuContinue").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.All
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems("mnuContinue" & gsALL).Text = .GetString("psCONTINUE", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems("mnuContinue" & gsALL).Click, AddressOf mnuRobotPopup_Click

            ''''''''''Process menu
            eMnuTag(0) = eMnuFctn.Process
            eMnuTag(1) = eMnuFctn.None
            eMnuTag(2) = eMnuFctn.None
            oMnu.Items("mnuProcess").Tag = CopyFunctionArray(eMnuTag)
            oMnu.Items("mnuProcess").Text = .GetString("psPROC", oCulture)
            AddHandler oMnu.Items("mnuProcess").Click, AddressOf mnuRobotPopup_Click

            ''''''''''Life Cycles menu
            eMnuTag(0) = eMnuFctn.LifeCycles
            eMnuTag(1) = eMnuFctn.None
            eMnuTag(2) = eMnuFctn.None
            oMnu.Items("mnuLifeCycle").Tag = CopyFunctionArray(eMnuTag)
            oMnu.Items("mnuLifeCycle").Text = .GetString("psLC_TITLE", oCulture)
            AddHandler oMnu.Items("mnuLifeCycle").Click, AddressOf mnuRobotPopup_Click

            ''''''''''Manual functions menu
            sMnuName = "mnuManFunc"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psMANFUNC", oCulture)
            eMnuTag(0) = eMnuFctn.SuperPurge
            eMnuTag(1) = eMnuFctn.None
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & "SuperPurge").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "SuperPurge").Text = .GetString("psSUPERPURGE", oCulture)
            AddHandler oSubMnu.DropDownItems(sMnuName & "SuperPurge").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & "SuperPurge" & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & "SuperPurge" & gsALL).Text = .GetString("psSUPERPURGE", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & "SuperPurge" & gsALL).Click, AddressOf mnuRobotPopup_Click

            ''''''''''Conveyor interlock menu
            sMnuName = "mnuConvIntlk"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psCONVINTLK", oCulture)
            eMnuTag(0) = eMnuFctn.ConvIntlk
            eMnuTag(1) = eMnuFctn.Bypass
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsBYPASS).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsBYPASS).Text = .GetString("psBYPASS", oCulture)

            AddHandler oSubMnu.DropDownItems(sMnuName & gsBYPASS).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Enable
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsENABLE).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(2) = eMnuFctn.All
            oSubMnu.DropDownItems(sMnuName & gsENABLE & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsENABLE & gsALL).Text = sEnable & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsBYPASS & gsALL).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Bypass
            oSubMnu.DropDownItems(sMnuName & gsBYPASS & gsALL).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsBYPASS & gsALL).Text = .GetString("psBYPASS", oCulture) & " " & sAll
            AddHandler oSubMnu.DropDownItems(sMnuName & gsENABLE & gsALL).Click, AddressOf mnuRobotPopup_Click

            ''''''''''Release TP As Enabling Device menu
            eMnuTag(0) = eMnuFctn.RelTpAsEnablingDev
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oMnu.Items("mnuRelTpAsEnablingDev").Tag = CopyFunctionArray(eMnuTag)
            oMnu.Items("mnuRelTpAsEnablingDev").Text = .GetString("psREL_TP", oCulture)
            AddHandler oMnu.Items("mnuRelTpAsEnablingDev").Click, AddressOf mnuRobotPopup_Click

            ''''''''''Robot Arm Isolate menu
            sMnuName = "mnuIsolate"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psISOLATE_ROBOT", oCulture)
            eMnuTag(0) = eMnuFctn.Isolate
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsENABLE).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Disable
            oSubMnu.DropDownItems(sMnuName & gsDISABLE).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsDISABLE).Text = sDisable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsDISABLE).Click, AddressOf mnuRobotPopup_Click

            ''''''''''Easy Debug menu
            sMnuName = "mnuEasyButton"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psEASY_BUTTON")
            eMnuTag(0) = eMnuFctn.EasyButton
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.Tag = CopyFunctionArray(eMnuTag)
            AddHandler oSubMnu.Click, AddressOf mnuRobotPopup_Click

            ''''''''''Ignore Opener Input menu
            sMnuName = "mnuIgnoreOpenerInput"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psIGNORE_OPENER_INPUT")
            eMnuTag(0) = eMnuFctn.IgnoreOpenerInput
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.Tag = CopyFunctionArray(eMnuTag)
            AddHandler oSubMnu.Click, AddressOf mnuRobotPopup_Click

            '''''''''Maintenance mode menus
            sMnuName = "mnuMaintMode"
            oSubMnu = DirectCast(oMnu.Items(sMnuName), ToolStripMenuItem)
            oSubMnu.Text = .GetString("psMAINT_MODE", oCulture)
            eMnuTag(0) = eMnuFctn.MaintEnableDisable
            eMnuTag(1) = eMnuFctn.Enable
            eMnuTag(2) = eMnuFctn.None
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsENABLE).Text = sEnable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsENABLE).Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.Disable
            oSubMnu.DropDownItems(sMnuName & gsDISABLE).Tag = CopyFunctionArray(eMnuTag)
            oSubMnu.DropDownItems(sMnuName & gsDISABLE).Text = sDisable
            AddHandler oSubMnu.DropDownItems(sMnuName & gsDISABLE).Click, AddressOf mnuRobotPopup_Click

            sMnuName = "mnuMaintMoveTo"
            Dim oSubMnu2 As ToolStripMenuItem = DirectCast(oSubMnu.DropDownItems(sMnuName), ToolStripMenuItem)
            oSubMnu2 = DirectCast(oSubMnu.DropDownItems(sMnuName), ToolStripMenuItem)
            oSubMnu2.Text = .GetString("psMOVETO", oCulture)
            eMnuTag(0) = eMnuFctn.MaintMoveTo
            eMnuTag(1) = eMnuFctn.MaintIn
            eMnuTag(2) = eMnuFctn.None
            oSubMnu2.DropDownItems("mnuMaintPos").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu2.DropDownItems("mnuMaintPos").Text = .GetString("psMAINT", oCulture)
            AddHandler oSubMnu2.DropDownItems("mnuMaintPos").Click, AddressOf mnuRobotPopup_Click
            eMnuTag(1) = eMnuFctn.MaintOut
            eMnuTag(2) = eMnuFctn.None
            oSubMnu2.DropDownItems("mnuMaintToHome").Tag = CopyFunctionArray(eMnuTag)
            oSubMnu2.DropDownItems("mnuMaintToHome").Text = .GetString("psMAINT_TO_HOME", oCulture)
            AddHandler oSubMnu2.DropDownItems("mnuMaintToHome").Click, AddressOf mnuRobotPopup_Click

            ''''''''''End robot pop-up menu

            'Robot pics and controls
            Dim bRCDone(16) As Boolean
            Dim sApplStatText(3) As String 'RJO 06/12/13
            Dim nApplStatBits(3) As Integer 'RJO 06/12/13

            For nIndex As Integer = 1 To 4 'RJO 06/12/13
                sApplStatText(nIndex - 1) = .GetString("psAPPL_STATUS_" & nIndex.ToString, oCulture)
            Next

            nApplStatBits(0) = eRobotStat.IPCPressMonOK 'RJO 06/12/13
            nApplStatBits(1) = eRobotStat.IPCInletRegOK 'RJO 06/12/13
            nApplStatBits(2) = eRobotStat.SA1ClosedLoop 'RJO 06/12/13
            nApplStatBits(3) = eRobotStat.SA2ClosedLoop 'RJO 06/12/13

            For Each oArm As clsArm In frmMain.colArms
                Dim sDevName As String = oArm.RobotNumber.ToString
                Dim nRCNum As Integer = oArm.Controller.ControllerNumber
                Dim oDev As uctlDevice = DirectCast(pnlTmp.Controls("uctlDeviceR" & sDevName), uctlDevice)

                If Not oDev Is Nothing Then
                    'MSW 8/9/22 Add label details
                    oDev.DeviceName = oArm.Name & " (" & oArm.Controller.Name & " - Eq:" & oArm.ArmNumber & ")" 'TODO - This isn't localized
                    oDev.BypassProxTT = .GetString("psBYPASS_SW_TT", oCulture)
                    oDev.EstatTT = .GetString("psESTAT_STATUS_TT", oCulture)
                    oDev.ApplStatLEDTT = .GetString("psAPPL_STATUS_TT", oCulture) 'RJO 06/12/13
                    oDev.Visible = True
                    oDev.Enabled = True
                    'oDev.ContextMenuStrip = oMnu
                    oDev.RC_Number = oArm.Controller.ControllerNumber
                    oDev.IsOpener = oArm.IsOpener
                    oDev.Eq_Number = oArm.ArmNumber
                    'oDev.picAppicatorStatus.Visible = Not(oArm.IsOpener)
                    If oArm.ArmType = eArmType.P500 Then
                        sApplStatText(3) = String.Empty
                    Else
                        sApplStatText(3) = .GetString("psAPPL_STATUS_4", oCulture)
                    End If
                    oDev.ApplStatItems = sApplStatText 'RJO 06/12/13
                    oDev.ApplStatBits = nApplStatBits 'RJO 06/12/13
                End If

                If Not bRCDone(nRCNum) Then
                    Dim oRC As uctlRC = DirectCast(pnlTmp.Controls("uctlRC" & nRCNum.ToString), uctlRC)

                    bRCDone(nRCNum) = True

                    If Not oRC Is Nothing Then
                        'MSW 8/9/22 Add label details
                        Dim sLabel As String = oArm.Controller.Name & " - "
                        For Each oArmTmp As clsArm In oArm.Controller.Arms
                            sLabel = sLabel & oArmTmp.Name & ", "
                        Next
                        sLabel = Left(sLabel, sLabel.Length - 2)
                        oRC.RCName = sLabel 'Arm.Controller.Name 'TODO - This isn't localized
                        oRC.Visible = True
                        oRC.Enabled = True
                        oRC.EstopText = .GetString("psRC_ESTOP_LBL", oCulture)
                        oRC.EstopTT = .GetString("psRC_ESTOP_TT", oCulture)
                        oRC.PurgeText = .GetString("psRC_PURGE_LBL", oCulture)
                        oRC.PurgeTT = .GetString("psRC_PURGE_TT", oCulture)
                        oRC.ServoDiscText = .GetString("psRC_SERVO_LBL", oCulture)
                        oRC.ServoDiscTT = .GetString("psRC_SERVO_TT", oCulture)
                        oRC.TPText = .GetString("psRC_TP_LBL", oCulture)
                        oRC.TPTT = .GetString("psRC_TP_TT", oCulture)
                        oRC.ShowServoDisc = True
                        AddHandler oRC.MouseDown, AddressOf UctlRx_MouseDown
                    End If
                End If 'Not bRCDone(nRCNum)

                Dim oPic2 As PictureBox = DirectCast(pnlTmp.Controls("picR" & sDevName), PictureBox)

                If Not oPic2 Is Nothing Then
                    oPic2.Visible = True
                    oPic2.Enabled = True
                    oPic2.ContextMenuStrip = oMnu
                    AddHandler oPic2.MouseEnter, AddressOf picRx_MouseEnter
                    AddHandler oPic2.MouseLeave, AddressOf picRx_MouseLeave
                    AddHandler oPic2.MouseDown, AddressOf picRx_MouseDown
                End If

            Next 'oArm

        End With 'gpsRM

    End Sub

    Friend Sub InterlockChange(ByVal vUCtl As UserControl, ByVal sData As String)
        '********************************************************************************************
        'Description: do the interlock LED Cartoons -make sure they are named properly
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nData As Integer = CInt(sData)
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture

            ' Led's
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)
            Dim g As GroupBox = DirectCast(pnlTmp.Controls("gpbLed"), GroupBox)

            'Led 1
            Dim p As PictureBox = DirectCast(g.Controls("picIntlk01"), PictureBox)
            Dim o As ImageList = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.MasterPower)) = gnBitVal(eBooth.MasterPower) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'Led 2
            p = DirectCast(g.Controls("picIntlk02"), PictureBox)
            o = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.FireBit)) = gnBitVal(eBooth.FireBit) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'Led 3
            p = DirectCast(g.Controls("picIntlk03"), PictureBox)
            o = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.AirBit)) = gnBitVal(eBooth.AirBit) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'Led 4
            p = DirectCast(g.Controls("picIntlk04"), PictureBox)
            o = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.PaintCirc)) = gnBitVal(eBooth.PaintCirc) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'Led 5
            p = DirectCast(g.Controls("picIntlk05"), PictureBox)
            o = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.ConvPermBit)) = gnBitVal(eBooth.ConvPermBit) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'Led 6
            p = DirectCast(g.Controls("picIntlk06"), PictureBox)
            o = DirectCast(colImages.Item(p.Tag), ImageList)
            If (nData And gnBitVal(eBooth.SolventOKBit)) = gnBitVal(eBooth.SolventOKBit) Then
                p.Image = o.Images.Item("green")
            Else
                p.Image = o.Images.Item("red")
            End If

            'booth mode
            g = DirectCast(pnlTmp.Controls("gpbMode"), GroupBox)
            Dim l As Label = DirectCast(g.Controls("lblBoothMode"), Label)

            Dim nMask As Integer = CInt(gnBitVal(eBooth.AutoModeBit) Or gnBitVal(eBooth.ManModeBit) Or _
                                        gnBitVal(eBooth.TeachModeBit) Or gnBitVal(eBooth.T1ModeBit))
            Dim nMode As Integer = nMask And nData

            Select Case nMode
                Case gnBitVal(eBooth.AutoModeBit)
                    If (nData And gnBitVal(eBooth.DegradeEnabled)) = gnBitVal(eBooth.DegradeEnabled) Then
                        l.Text = gpsRM.GetString("psDEGRADE_MODE", oCulture)
                        l.BackColor = Color.Magenta
                    Else
                        l.Text = gpsRM.GetString("psAUTO_MODE", oCulture)
                        l.BackColor = Color.LimeGreen
                    End If
                Case gnBitVal(eBooth.ManModeBit)
                    l.Text = gpsRM.GetString("psMANUAL_MODE", oCulture)
                    l.BackColor = Color.Yellow
                Case gnBitVal(eBooth.TeachModeBit)
                    l.Text = gpsRM.GetString("psTEACH_MODE", oCulture)
                    l.BackColor = Color.Yellow
                Case gnBitVal(eBooth.T1ModeBit) ' + gnBitVal(eBooth.AutoModeBit)
                    l.Text = gpsRM.GetString("psT1_MODE", oCulture)
                    l.BackColor = Color.Orange
                Case Else
                    l.Text = gpsRM.GetString("psUNKNOWN_MODE", oCulture)
                    l.BackColor = Color.White
            End Select


            'SOC groupbox
            g = DirectCast(pnlTmp.Controls("gpbSOC"), GroupBox)
            'Estat disconnect
            p = DirectCast(g.Controls("picEstatDisc"), PictureBox)
            o = DirectCast(colImages.Item("imlLed"), ImageList)
            If (nData And gnBitVal(eBooth.EstatEnbBit)) = gnBitVal(eBooth.EstatEnbBit) Then
                p.Image = o.Images("green")
            Else
                p.Image = o.Images("red")
            End If


            ''process air enable
            'p = DirectCast(g.Controls("picProcessAir"), PictureBox)
            'o = DirectCast(colImages.Item("imlSelSwitch"), ImageList)
            'If (nData And gnBitVal(eBooth.ProcessAirEnbBit)) = gnBitVal(eBooth.ProcessAirEnbBit) Then
            '    p.Image = o.Images("green_left")
            'Else
            '    p.Image = o.Images("yellow_right")
            'End If

            'Conveyor Run switch
            p = DirectCast(g.Controls("picConvRunSS"), PictureBox)
            o = DirectCast(colImages.Item("imlSelSwitch"), ImageList)
            If (nData And gnBitVal(eBooth.ConvRunSwitch)) = gnBitVal(eBooth.ConvRunSwitch) Then
                p.Image = o.Images("yellow_left")
            Else
                p.Image = o.Images("green_right")
            End If

            ''Estat Motion Disconnect'HAF 
            'p = DirectCast(g.Controls("picEstatMotionDisc"), PictureBox)
            'o = DirectCast(colImages.Item("imlLed"), ImageList)
            'If (nData And gnBitVal(eBooth.EstatMotionBit)) = gnBitVal(eBooth.EstatMotionBit) Then
            '    p.Image = o.Images("green")
            'Else
            '    p.Image = o.Images("red")
            'End If

            'Process Enable switch
            p = DirectCast(g.Controls("picProcessSS"), PictureBox)
            If p IsNot Nothing Then
                o = DirectCast(colImages.Item("imlSelSwitch"), ImageList)
                If (nData And gnBitVal(eBooth.EstatEnbSwitch)) = gnBitVal(eBooth.EstatEnbSwitch) Then
                    p.Image = o.Images("green_left")
                Else
                    p.Image = o.Images("yellow_right")
                End If
            End If

            'Estat Enable Switch
            p = DirectCast(g.Controls("picEstatPowerSS"), PictureBox)
            If p IsNot Nothing Then
                o = DirectCast(colImages.Item("imlLed"), ImageList)
                If (nData And gnBitVal(eBooth.EstatEnbSwitch)) = gnBitVal(eBooth.EstatEnbSwitch) Then
                    p.Image = o.Images("green")
                Else
                    p.Image = o.Images("red")
                End If
            End If

            'HAF 140411
            'Enabling Device #1
            p = DirectCast(g.Controls("picEnablingDev"), PictureBox)
            If p IsNot Nothing Then
                o = DirectCast(colImages.Item("imlEnabDev"), ImageList)
                If (nData And gnBitVal(eBooth.EnablingDev)) = gnBitVal(eBooth.EnablingDev) Then
                    p.Image = o.Images("ok_left")
                Else
                    p.Image = o.Images("fault_left")
                End If
            End If

            'RequestToEnterDisabled = 15
            p = DirectCast(g.Controls("picReqToEnterSS"), PictureBox)
            If p IsNot Nothing Then
                o = DirectCast(colImages.Item("imlLed"), ImageList)
                If (nData And gnBitVal(eBooth.RequestToEnterDisabled)) = gnBitVal(eBooth.RequestToEnterDisabled) Then
                    p.Image = o.Images("green")
                Else
                    p.Image = o.Images("yellow")
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: InterlockChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Friend Sub PaintMixChange(ByVal vUCtl As UserControl, ByVal sPaintMixOutWd As String, ByVal sPaintMixStatWd As String, ByVal nMaxValve As Integer)
        '********************************************************************************************
        'Description: do the Paint mix LED Cartoons -make sure they are named properly
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nPaintMixOutWd As Integer = CInt(sPaintMixOutWd)
            Dim nPaintMixStatWd As Integer = CInt(sPaintMixStatWd)
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture

            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)
            Dim oPic As PictureBox = Nothing
            Dim oImgLst As ImageList = Nothing
            Dim oChk As CheckBox = Nothing
            Dim bStat As Boolean
            Dim bOut As Boolean
            Dim bManReq As Boolean
            Dim nManReqOld As Integer = 0
            'Paint mix box
            Dim g As GroupBox = DirectCast(pnlTmp.Controls("gpbPaintMix"), GroupBox)
            If g IsNot Nothing Then
                For nValve As Integer = 0 To nMaxValve - 1
                    oPic = DirectCast(g.Controls("picValve" & nValve.ToString("00")), PictureBox)
                    oImgLst = DirectCast(colImages.Item(oPic.Tag), ImageList)
                    bStat = ((nPaintMixStatWd And gnBitVal(nValve)) = gnBitVal(nValve))
                    bOut = ((nPaintMixOutWd And gnBitVal(nValve)) = gnBitVal(nValve))
                    bManReq = ((nPaintMixOutWd And gnBitVal(nValve + 16)) = gnBitVal(nValve + 16))
                    If bManReq Then
                        nManReqOld = nManReqOld + gnBitVal(nValve)
                    End If
                    If bStat Then
                        oPic.Image = oImgLst.Images.Item("green")
                    ElseIf bOut Then
                        oPic.Image = oImgLst.Images.Item("red")
                    Else
                        oPic.Image = oImgLst.Images.Item("gray")
                    End If

                    oChk = DirectCast(g.Controls("chkValve" & nValve.ToString("00")), CheckBox)
                    If bManReq Then
                        oChk.CheckState = CheckState.Checked
                    ElseIf bOut Then
                        oChk.CheckState = CheckState.Indeterminate
                    Else
                        oChk.CheckState = CheckState.Unchecked
                    End If

                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: PaintMixChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub WaterHeaterStatusChange(ByVal vUCtl As UserControl, ByVal sData As String)
        '********************************************************************************************
        'Description: do the water heater LED Cartoons -make sure they are named properly
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nData As Integer = CInt(sData)
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture

            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)
            Dim p As PictureBox = Nothing
            Dim o As ImageList = Nothing

            'Water Heater box 1
            Dim g As GroupBox = DirectCast(pnlTmp.Controls("gpbWaterHeater1"), GroupBox)

            If g IsNot Nothing Then
                p = DirectCast(g.Controls("picWHStart1"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeaterMasterStartOn)) = gnBitVal(eBooth.WaterHeaterMasterStartOn) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHLevel1"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeaterLevelOK)) = gnBitVal(eBooth.WaterHeaterLevelOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHTemp1"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeaterTempOK)) = gnBitVal(eBooth.WaterHeaterTempOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If
            End If

            'Water Heater box 2
            g = DirectCast(pnlTmp.Controls("gpbWaterHeater2"), GroupBox)

            If g IsNot Nothing Then
                p = DirectCast(g.Controls("picWHStart2"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater2MasterStartOn)) = gnBitVal(eBooth.WaterHeater2MasterStartOn) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHLevel2"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater2LevelOK)) = gnBitVal(eBooth.WaterHeater2LevelOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHTemp2"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater2TempOK)) = gnBitVal(eBooth.WaterHeater2TempOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If
            End If

            'Water Heater box 3
            g = DirectCast(pnlTmp.Controls("gpbWaterHeater3"), GroupBox)

            If g IsNot Nothing Then
                p = DirectCast(g.Controls("picWHStart3"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater3MasterStartOn)) = gnBitVal(eBooth.WaterHeater3MasterStartOn) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHLevel3"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater3LevelOK)) = gnBitVal(eBooth.WaterHeater3LevelOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHTemp3"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater3TempOK)) = gnBitVal(eBooth.WaterHeater3TempOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If
            End If

            'Water Heater box 4
            g = DirectCast(pnlTmp.Controls("gpbWaterHeater4"), GroupBox)

            If g IsNot Nothing Then
                p = DirectCast(g.Controls("picWHStart4"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater4MasterStartOn)) = gnBitVal(eBooth.WaterHeater4MasterStartOn) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHLevel4"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater4LevelOK)) = gnBitVal(eBooth.WaterHeater4LevelOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

                p = DirectCast(g.Controls("picWHTemp4"), PictureBox)
                o = DirectCast(colImages.Item(p.Tag), ImageList)

                If (nData And gnBitVal(eBooth.WaterHeater4TempOK)) = gnBitVal(eBooth.WaterHeater4TempOK) Then
                    p.Image = o.Images.Item("green")
                Else
                    p.Image = o.Images.Item("red")
                End If

            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: InterlockChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Friend Sub IntrusionStatusChange(ByRef vUCtl As UserControl, ByRef sData() As String, Optional ByRef oPen() As Pen = Nothing)
        '********************************************************************************************
        'Description: Intrusion status for the booth
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim nData As Integer = CInt(sData(eBooth.IntrStatWd))
            Dim imlPEC As ImageList = DirectCast(colImages.Item("imlPEC"), ImageList)
            Dim oColor As Color = Color.DarkGray
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)

            Dim p As PictureBox = Nothing
            Dim sName As String = "picDetectProx"

            p = DirectCast(pnlTmp.Controls(sName), PictureBox)
            If IsNothing(p) = False Then
                Dim o As ImageList = DirectCast(colImages.Item("imlProxSwitch"), ImageList)
                Dim sTag As String = DirectCast(p.Tag, String)
                Dim sTmp() As String = sTag.Split

                'Stuck  takes priority
                If (nData And gnBitVal(eBooth.PartDetBit + 8)) = gnBitVal(eBooth.PartDetBit + 8) Then
                    p.Image = o.Images.Item("stuck")
                ElseIf (nData And gnBitVal(eBooth.PartDetBit)) = gnBitVal(eBooth.PartDetBit) Then
                    p.Image = o.Images.Item("on")
                Else
                    p.Image = o.Images.Item("off")
                End If
            End If  'IsNothing(p) = False


            'Entrance intrusion
            Dim pTop As PictureBox = DirectCast(pnlTmp.Controls("picPECEntTop"), PictureBox)
            Dim pBottom As PictureBox = DirectCast(pnlTmp.Controls("picPECEntBottom"), PictureBox)

            'jello boxes
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
            Dim uctlTmp As uctlList = Nothing
            uctlTmp = DirectCast(pnlTmp.Controls("UctlEntInt"), uctlList)
            If uctlTmp IsNot Nothing Then
                If (nData And gnBitVal(eBooth.EntMuteBit)) = gnBitVal(eBooth.EntMuteBit) Then
                    uctlTmp.LabelText(0) = gpsRM.GetString("psMUTING_ON", oCulture)
                Else
                    uctlTmp.LabelText(0) = gpsRM.GetString("psMUTING_OFF", oCulture)
                End If
                uctlTmp.LabelText(1) = gpsRM.GetString("psMUTE_COUNT", oCulture) & sData(eBooth.EntMuteCounter)
                uctlTmp.LabelText(2) = gpsRM.GetString("psINT_BROKE", oCulture) & sData(eBooth.EntBeamBrokeCounts)
                uctlTmp.LabelText(3) = gpsRM.GetString("psINT_MADE", oCulture) & sData(eBooth.EntBeamMadeCounts)
            End If
            uctlTmp = Nothing
            uctlTmp = DirectCast(pnlTmp.Controls("UctlExitInt"), uctlList)
            If uctlTmp IsNot Nothing Then
                If (nData And gnBitVal(eBooth.ExitMuteBit)) = gnBitVal(eBooth.ExitMuteBit) Then
                    uctlTmp.LabelText(0) = gpsRM.GetString("psMUTING_ON", oCulture)
                Else
                    uctlTmp.LabelText(0) = gpsRM.GetString("psMUTING_OFF", oCulture)
                End If
                uctlTmp.LabelText(1) = gpsRM.GetString("psMUTE_COUNT", oCulture) & sData(eBooth.ExitMuteCounter)
                uctlTmp.LabelText(2) = gpsRM.GetString("psINT_BROKE", oCulture) & sData(eBooth.ExitBeamBrokeCounts)
                uctlTmp.LabelText(3) = gpsRM.GetString("psINT_MADE", oCulture) & sData(eBooth.ExitBeamMadeCounts)
            End If


            'We are on the queue screen bail.
            If pTop Is Nothing And pBottom Is Nothing Then Exit Sub

            If (nData And gnBitVal(eBooth.EntFaultBit)) = gnBitVal(eBooth.EntFaultBit) Then
                'Faulted
                pTop.Image = imlPEC.Images.Item("Fault_Down")
                pBottom.Image = imlPEC.Images.Item("Fault_Up")
                oColor = Color.Red
            ElseIf (nData And gnBitVal(eBooth.EntMuteBit)) = gnBitVal(eBooth.EntMuteBit) Then
                'Muted
                pTop.Image = imlPEC.Images.Item("Mute_Down")
                pBottom.Image = imlPEC.Images.Item("Mute_Up")
                oColor = Color.Yellow
            ElseIf (nData And gnBitVal(eBooth.EntIntMadeBit)) = gnBitVal(eBooth.EntIntMadeBit) Then
                'Made
                pTop.Image = imlPEC.Images.Item("On_Down")
                pBottom.Image = imlPEC.Images.Item("On_Up")
                oColor = Color.LimeGreen
            Else
                'Shouldn't be here
                pTop.Image = imlPEC.Images.Item("Off_Down")
                pBottom.Image = imlPEC.Images.Item("Off_Up")
                oColor = Color.DarkGray
            End If
            If ((oPen(0) Is Nothing) = False) Then
                oPen(0).Color = oColor
                If (nData And gnBitVal(eBooth.EntIntMadeBit)) = gnBitVal(eBooth.EntIntMadeBit) Then
                    oPen(0).DashStyle = Drawing2D.DashStyle.Solid
                Else
                    oPen(0).DashStyle = Drawing2D.DashStyle.DashDotDot
                End If
            End If
            'Exit intrusion
            pTop = DirectCast(pnlTmp.Controls("picPECExitTop"), PictureBox)
            pBottom = DirectCast(pnlTmp.Controls("picPECExitBottom"), PictureBox)
            If (nData And gnBitVal(eBooth.ExitFaultBit)) = gnBitVal(eBooth.ExitFaultBit) Then
                'Faulted
                pTop.Image = imlPEC.Images.Item("Fault_Down")
                pBottom.Image = imlPEC.Images.Item("Fault_Up")
                oColor = Color.Red
            ElseIf (nData And gnBitVal(eBooth.ExitMuteBit)) = gnBitVal(eBooth.ExitMuteBit) Then
                'Muted
                pTop.Image = imlPEC.Images.Item("Mute_Down")
                pBottom.Image = imlPEC.Images.Item("Mute_Up")
                oColor = Color.Yellow
            ElseIf (nData And gnBitVal(eBooth.ExitIntMadeBit)) = gnBitVal(eBooth.ExitIntMadeBit) Then
                'Made
                pTop.Image = imlPEC.Images.Item("On_Down")
                pBottom.Image = imlPEC.Images.Item("On_Up")
                oColor = Color.LimeGreen
            Else
                'Shouldn't be here
                pTop.Image = imlPEC.Images.Item("Off_Down")
                pBottom.Image = imlPEC.Images.Item("Off_Up")
                oColor = Color.DarkGray
            End If
            If ((oPen(1) Is Nothing) = False) Then
                oPen(1).Color = oColor
                If (nData And gnBitVal(eBooth.ExitIntMadeBit)) = gnBitVal(eBooth.ExitIntMadeBit) Then
                    oPen(1).DashStyle = Drawing2D.DashStyle.Solid
                Else
                    oPen(1).DashStyle = Drawing2D.DashStyle.DashDotDot
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: IntrusionStatusChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub LimitSwChange(ByVal vUCtl As UserControl, ByVal sStatus As String, _
                             ByVal HowMany As Integer)
        '********************************************************************************************
        'Description: Limit Switches in the booth
        '
        'Parameters: lots of
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nStatus As Integer = CInt(sStatus)
            Dim p As PictureBox = Nothing
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)

            For i As Integer = 0 To HowMany - 1
                Dim sName As String = "picLimitSw" & i.ToString

                p = DirectCast(pnlTmp.Controls(sName), PictureBox)
                If IsNothing(p) = False Then
                    Dim o As ImageList = DirectCast(colImages.Item("imlLimitSwitches"), ImageList)
                    Dim sTag As String = DirectCast(p.Tag, String)
                    Dim sTmp() As String = sTag.Split

                    'Stuck  takes priority
                    If (nStatus And gnBitVal(i + 8)) = gnBitVal(i + 8) Then
                        p.Image = o.Images.Item(sTmp(0) & "_on_stuck_" & sTmp(1))
                    ElseIf (nStatus And gnBitVal(i)) = gnBitVal(i) Then
                        p.Image = o.Images.Item(sTmp(0) & "_on_" & sTmp(1))
                    Else
                        p.Image = o.Images.Item(sTmp(0) & "_off")
                    End If
                End If  'IsNothing(p) = False
            Next 'i

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: LimitSwChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Friend Function nGetMaintModeEnableState(ByVal nRobotIndex As Integer) As Boolean
        '********************************************************************************************
        'Description: Checks if any robots are in maintenance mode.  Enables Menu if selected
        '             Robot is already in maintenance mode  
        '
        'Parameters: Robot selecting maitenance
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim s() As String

        Try

            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "MaintModeEnableStates"
            s = frmMain.mPLC.PLCData

            nGetMaintModeEnableState = False

            '1 or all
            If (CInt(s(0)) And CInt(2 ^ nRobotIndex)) > 0 Then
                nGetMaintModeEnableState = True
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: nGetMaintModeEnableState", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function
    Friend Function nGetAllMaintModeEnableState(ByVal nRobotIndex As Integer) As Boolean
        '********************************************************************************************
        'Description: Checks if any robots are in maintenance mode.  Enables Menu if selected
        '             Robot is already in maintenance mode or if no robots are in maintenance mode.
        '
        'Parameters: Robot selecting maitenance
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim s() As String

        Try

            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "MaintModeEnableStates"
            s = frmMain.mPLC.PLCData

            nGetAllMaintModeEnableState = False

            '1 or all
            If (CInt(s(0)) And CInt(2 ^ nRobotIndex)) > 0 Or (CInt(s(0)) = 0) Then
                nGetAllMaintModeEnableState = True
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: nGetMaintModeEnableState", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function



    Private Sub mnuRobotPopup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: Handle robot pop-up  menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Grab a couple local references from the menu
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim eMnuTag() As eMnuFctn = DirectCast(oMnu.Tag, eMnuFctn())
        'Figure out the robot that popped up
        Dim sRobot As String = String.Empty
        Dim nRobot As Integer = 0

        Try
            Dim omnuTop As ContextMenuStrip
            Dim oMnu2 As ToolStripMenuItem = DirectCast(oMnu.OwnerItem, ToolStripMenuItem)
            If oMnu2 Is Nothing Then
                '1st level menu, get the owner 
                omnuTop = DirectCast(oMnu.Owner, ContextMenuStrip)
            Else
                Dim oMnu3 As ToolStripDropDownItem = DirectCast(oMnu2.OwnerItem, ToolStripDropDownItem)
                If oMnu3 Is Nothing Then
                    'on a submenu, get grandpa
                    omnuTop = DirectCast(oMnu2.Owner, ContextMenuStrip)
                Else
                    'on a submenu of a submenu, get great grandpa
                    omnuTop = DirectCast(oMnu3.Owner, ContextMenuStrip)
                End If
            End If
            sRobot = omnuTop.Items("mnuRobotName").Text
            nRobot = CInt(omnuTop.Items("mnuRobotName").Tag)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            Debug.Print(sRobot & ": " & nRobot.ToString & " : " & eMnuTag(0).ToString & " " & eMnuTag(1).ToString & " " & eMnuTag(2).ToString)
        End Try

        'Handle each menu
        Try
            Dim nRC As Integer = frmMain.colArms.Item(nRobot - 1).Controller.ControllerNumber
            Dim nArm As Integer = frmMain.colArms.Item(nRobot - 1).ArmNumber

            Select Case eMnuTag(0)
                Case eMnuFctn.Applicator, eMnuFctn.Trigger, eMnuFctn.Estat
                    Call subPLCEnableToggle(eMnuTag, nRobot) '(eMnuTag, CType((nRC - 1) * 2 + nArm, Integer))
                Case eMnuFctn.MoveTo, eMnuFctn.MaintMoveTo
                    Call subMoveTo(eMnuTag, nRobot)
                Case eMnuFctn.Jog
                    frmAxisJogFrm = New frmAxisJog
                    frmAxisJogFrm.Show(sRobot)
                Case eMnuFctn.RecCancel, eMnuFctn.RecContinue
                    Call subWritePLCRequest(eMnuTag, nRobot)
                Case eMnuFctn.Process
                    Call frmMain.subLaunchProcessScreen(sRobot)
                Case eMnuFctn.LifeCycles
                    Call frmMain.subLaunchLifeCyclesScreen(sRobot)
                Case eMnuFctn.SuperPurge
                    Call subWritePLCRequest(eMnuTag, nRobot) ' CType((nRC - 1) * 2 + nArm, Integer))
                Case eMnuFctn.ConvIntlk
                    Call subPLCEnableToggle(eMnuTag, nRobot)
                Case eMnuFctn.RelTpAsEnablingDev
                    Call subPLCEnableToggle(eMnuTag, nRC)
                Case eMnuFctn.Isolate
                    Call subPLCEnableToggle(eMnuTag, nRobot)
                Case eMnuFctn.EasyButton
                    subDoEasyDebug(eMnuTag, sRobot)
                Case eMnuFctn.IgnoreOpenerInput
                    Call subReadBeforeWritePLCRequest(eMnuTag, nRobot)
                Case eMnuFctn.MaintEnableDisable
                    Call subPLCEnableToggle(eMnuTag, nRobot)
            End Select

        Catch ex As Exception

        End Try

    End Sub

    Friend Sub ProxSwChange(ByVal vUCtl As UserControl, ByVal sStatus As String, _
                            ByVal HowMany As Integer)
        '********************************************************************************************
        'Description: Prox Switches in the booth
        '
        'Parameters: lots of
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nStatus As Integer = CInt(sStatus)
            Dim p As PictureBox = Nothing
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)

            For i As Integer = 0 To HowMany - 1
                Dim sName As String = "picMuteProx" & i.ToString

                p = DirectCast(pnlTmp.Controls(sName), PictureBox)
                If IsNothing(p) = False Then
                    Dim o As ImageList = DirectCast(colImages.Item("imlProxSwitch"), ImageList)
                    'Stuck  takes priority
                    If (nStatus And gnBitVal(i + 8)) = gnBitVal(i + 8) Then
                        p.Image = o.Images.Item("stuck")
                    ElseIf (nStatus And gnBitVal(i)) = gnBitVal(i) Then
                        p.Image = o.Images.Item("on")
                    Else
                        p.Image = o.Images.Item("off")
                    End If
                End If  'IsNothing(p) = False
            Next 'i

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: ProxSwChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub QueuePositionChange(ByRef QPosData As String(), ByRef sNum As String, Optional ByRef uctlJob As uctlList = Nothing, Optional ByRef pnlJob As Panel = Nothing)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Try
            'Label order
            Const nLBL_CARRIER As Integer = 1
            Const nLBL_STYLE As Integer = 2
            Const nLBL_COLOR As Integer = 3
            Const nLBL_OPTION As Integer = 4
            Dim nLBL_VIN As Integer = -1
            Dim nLBL_TUTONE As Integer = 5 'RJO 10/2/14
            Dim nLBL_STATUS As Integer = 5 '6 'NRU 161007 No Tutone '5 RJO 10/02/14
            Dim nLBL_REPAIR As Integer = -1
            'If colZones.ActiveZone.RepairPanels > 0 Then
            '    nLBL_STATUS = 7
            '    nLBL_REPAIR = 5
            '    nLBL_VIN = 6
            'Else
            '    nLBL_STATUS = 5
            '    nLBL_REPAIR = -1
            '    nLBL_VIN = -1
            'End If

            Dim bDoPanel As Boolean = Not (pnlJob Is Nothing)
            Dim bDoList As Boolean = Not (uctlJob Is Nothing)
            Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture

            Dim nQueueShiftPoint As Integer = frmMain.gnQueueShiftPoint

            'is there a job?
            Dim nWord As Integer = CInt(QPosData(eQueue.Status))
            If (nWord = eQueue.Stat_PosEmpty) Then
                'no job
                If bDoPanel Then
                    If pnlJob.Visible = True Then pnlJob.Visible = False
                End If
                If bDoList Then
                    'This clears all the labels in the list
                    uctlJob.LabelText(0) = String.Empty
                    uctlJob.LabelText(nLBL_STATUS) = gpsRM.GetString("psNOCARRIER", oCulture)
                    uctlJob.LabelBackColor(nLBL_STATUS) = frmMain.BackColor
                End If
            Else
                'job in pos
                'nopaint
                If (CInt(QPosData(eQueue.Carrier)) = 1679) Then
                    Debug.Print("I got u bitch")
                End If
                Dim nPntEnab As Integer = CInt(QPosData(eQueue.PaintEnables))
                Dim bNoPaint As Boolean = (nPntEnab = 0) Or (nWord <> CInt(eQueue.Stat_Paint))

                'carrier
                Dim sCarrier As String = gpsRM.GetString("psCARRIER_LBL", oCulture) & " " & QPosData(eQueue.Carrier)
                'style
                Dim sStyle As String = String.Empty
                If colStyles.UseAscii Then
                    sStyle = gpsRM.GetString("psSTYLE_LBL", oCulture) & " " & mMathFunctions.CvIntegerToASCII(CInt(QPosData(eQueue.Style)), colStyles.PlantAsciiMaxLength)
                Else
                    sStyle = gpsRM.GetString("psSTYLE_LBL", oCulture) & " " & QPosData(eQueue.Style)
                End If

                'color
                Dim sColor As String = String.Empty
                If gbAsciiColor Then
                    sColor = gpsRM.GetString("psCOLOR_LBL", oCulture) & mMathFunctions.CvIntegerToASCII(CInt(QPosData(eQueue.Color)), gnAsciiColorNumChar)
                Else
                    sColor = gpsRM.GetString("psCOLOR_LBL", oCulture) & " " & QPosData(eQueue.Color) 'NRU 160929 Added space like all the rest
                End If
                'option
                Dim sOption As String = gpsRM.GetString("psOPTION_LBL", oCulture) & " " & QPosData(eQueue.Optn)
                'tutone RJO 10/02/14
                Dim sTutone As String = "Tutone: " & QPosData(eQueue.Tutone)
                'Repair
                Dim nPanels As Integer = (mnRepairPanelMask And CInt(QPosData(eQueue.Repair)))
                Dim sRepair As String = gpsRM.GetString("psREPAIR_LBL", oCulture) & " "
                If nPanels > 0 Then
                    sRepair = sRepair & gpsRM.GetString("psYES", oCulture)
                Else
                    sRepair = sRepair & gpsRM.GetString("psNO", oCulture)
                End If
                'Vin
                Dim sTmp As String
                Dim sVINpic As String = gpsRM.GetString("psVIN_LBL", oCulture) & " "
                Dim sVIN As String = String.Empty
                If gnVinChars < 7 Then
                    sVIN = sVINpic
                End If
                For nIdx As Integer = eQueue.VinLen + eQueue.VinWd1 - 1 To eQueue.VinWd1 Step -1
                    '''''''''''''''''''''''''''''''''''''''''
                    'Numeric vin  
                    If gbNumericVin Then
                        sTmp = QPosData(nIdx)
                    Else
                        'ASCII VIN
                        If IsNumeric(QPosData(nIdx)) Then
                            sTmp = mMathFunctions.CvIntegerToASCII(CInt(QPosData(nIdx)), 4)
                        Else
                            sTmp = String.Empty
                        End If
                    End If
                    '''''''''''''''''''''''''''''''''''''''''
                    '''''''''''''''''''''''''''''''''''''''''
                    sVIN = sVIN & sTmp
                Next

                If bDoPanel Then
                    Dim nPosQ As Integer = CInt(QPosData(eQueue.Position))
                    '16-bit hack
                    If nPosQ > 32767 Then
                        nPosQ = nPosQ - 65536
                    End If

                    Dim nPos As Integer = mnTrackingStartPos + (CInt(nPosQ * mfTrackingScale))
                    ''NVSP 03092017 GUI Carrior Graphic

                    If colZones.CurrentZoneNumber = 1 Then
                        pnlJob.Left = mnTrackingStartPos + CInt(nPosQ * mfTrackingScale)
                    ElseIf colZones.CurrentZoneNumber = 2 Then
                        pnlJob.Left = mnTrackingStartPos + CInt(nPosQ * mfTrackingScale) + 300
                    ElseIf colZones.CurrentZoneNumber = 3 Then
                        pnlJob.Left = mnTrackingStartPos + CInt(nPosQ * mfTrackingScale)
                    Else
                        pnlJob.Left = mnTrackingStartPos + CInt(nPosQ * mfTrackingScale) + 300
                    End If
                    Debug.Print("Panel Name" & pnlJob.Name)

                    Dim p As Panel = DirectCast(pnlJob.Controls("pnlNoPaint" & sNum), Panel)
                    p.Visible = bNoPaint

                    'carrier
                    Dim l As Label = DirectCast(pnlJob.Controls("lblCarrier" & sNum), Label)
                    l.Text = sCarrier
                    'style
                    l = DirectCast(pnlJob.Controls("lblStyle" & sNum), Label)
                    l.Text = sStyle
                    'color
                    l = DirectCast(pnlJob.Controls("lblColor" & sNum), Label)
                    l.Text = sColor
                    'option
                    l = DirectCast(pnlJob.Controls("lblOpt" & sNum), Label)
                    l.Text = sOption
                    'Tutone 'RJO 10/02/14 Hijacked Repair Label
                    l = DirectCast(pnlJob.Controls("lblRepair" & sNum), Label)
                    l.Text = sTutone
                    l.Visible = False 'NRU 160929 True to false

                    'Repair
                    'If nLBL_REPAIR >= 0 Then
                    '    l = DirectCast(pnlJob.Controls("lblRepair" & sNum), Label)
                    '    l.Text = sRepair
                    '    l.Visible = True
                    'End If
                    If nLBL_VIN >= 0 Then
                        l = DirectCast(pnlJob.Controls("lblVin" & sNum), Label)
                        l.Text = sVINpic & sVIN
                        l.Visible = True
                    End If
                    If pnlJob.Visible = False Then pnlJob.Visible = True
                    pnlJob.ResumeLayout(True)
                End If
                If bDoList Then

                    If bNoPaint Then
                        If (nWord = CInt(eQueue.Stat_EmptyCarrier)) Then
                            uctlJob.LabelText(nLBL_STATUS) = gpsRM.GetString("psEMPTYCARRIER", oCulture)
                            uctlJob.LabelBackColor(nLBL_STATUS) = frmMain.BackColor
                        Else
                            uctlJob.LabelText(nLBL_STATUS) = gpsRM.GetString("psNOPAINT", oCulture)
                            uctlJob.LabelBackColor(nLBL_STATUS) = Color.Yellow
                        End If
                    Else
                        uctlJob.LabelText(nLBL_STATUS) = gpsRM.GetString("psPAINT", oCulture)
                        uctlJob.LabelBackColor(nLBL_STATUS) = Color.LimeGreen
                    End If
                    If nLBL_CARRIER > 0 Then
                        uctlJob.LabelText(nLBL_CARRIER) = sCarrier
                    End If
                    uctlJob.LabelText(nLBL_STYLE) = sStyle
                    uctlJob.LabelText(nLBL_COLOR) = sColor
                    If nLBL_OPTION > 0 Then
                        uctlJob.LabelText(nLBL_OPTION) = sOption
                    End If

                    'uctlJob.LabelText(nLBL_TUTONE) = sTutone 'NRU 161007 No Tutone

                    If nLBL_REPAIR > 0 Then
                        uctlJob.LabelText(nLBL_REPAIR) = sRepair
                    End If
                    If nLBL_VIN > 0 Then
                        uctlJob.LabelText(nLBL_VIN) = sVIN
                    End If
                End If

            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: QueuePositionChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub RobotChange(ByVal vUCtl As UserControl, ByVal nDevice As Integer, _
                           ByVal sData As String(), Optional ByVal bMouseOver As Boolean = False)
        '********************************************************************************************
        'Description: Robot data passed in one robot at a time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/20/14  MSW     ZDT Changes
        '********************************************************************************************

        Try
            Dim nRobotPos As Integer = CInt(sData(eRobot.Position))
            Dim nRobotStat As Integer = CInt(sData(eRobot.Status))
            Dim nRunStat As Integer = CInt(sData(eRobot.RunningStat))
            Dim oDevice As uctlDevice = Nothing
            Dim oImageList As ImageList = Nothing
            Dim oRC As uctlRC = Nothing
            Dim oRobotPic As PictureBox = Nothing
            Dim sRCNumber As String = String.Empty
            Dim sName As String = String.Empty
            Dim bIsOpener As Boolean
            Dim pnlTmp As Panel = DirectCast(vUCtl.Controls("pnlMain"), Panel)

            'Device user control
            sName = "uctlDeviceR" & nDevice.ToString
            oDevice = DirectCast(pnlTmp.Controls(sName), uctlDevice)
            'Handle skipped robots
            If oDevice Is Nothing Then
                Exit Sub
            End If
            bIsOpener = oDevice.IsOpener
            sRCNumber = oDevice.RC_Number.ToString

            If Not bIsOpener Then oDevice.ApplicatorStatus = nRobotStat 'RJO 06/12/13

            'Robot picture
            sName = "picR" & nDevice.ToString
            oRobotPic = DirectCast(pnlTmp.Controls(sName), PictureBox)

            'RC user control
            sName = "uctlRC" & sRCNumber
            oRC = DirectCast(pnlTmp.Controls(sName), uctlRC)

            'position label
            ' 03/20/14  MSW     ZDT Changes
            Call subRobotPositionChange(oRobotPic, oDevice.PositionLabel, nRunStat, nRobotPos, bIsOpener, bMouseOver, nDevice)

            If oDevice.Eq_Number = 1 Then
                'sop estop
                oImageList = CType(colImages.Item("imlEstop_Cstop"), ImageList)

                If ((nRobotStat And eRobotStat.SOPEStop) = eRobotStat.SOPEStop) Then
                    oRC.Estop = oImageList.Images("e_in")
                Else
                    oRC.Estop = oImageList.Images("e_out")
                End If

                'teach pendant
                Dim sLink As String = String.Empty
                oImageList = CType(colImages.Item("imlPendant"), ImageList)

                If ((nRobotStat And eRobotStat.TPLinkOn) = eRobotStat.TPLinkOn) Then
                    sLink = "_link"
                End If
                If ((nRobotStat And eRobotStat.TPEStopOn) = eRobotStat.TPEStopOn) Then
                    oRC.TP = oImageList.Images("estop" & sLink)
                ElseIf ((nRobotStat And eRobotStat.TPEnabled) = eRobotStat.TPEnabled) Then
                    oRC.TP = oImageList.Images("enabled" & sLink)
                Else
                    oRC.TP = oImageList.Images("ok" & sLink)
                End If

                'purge
                oImageList = CType(colImages.Item("imlLed"), ImageList)

                If ((nRobotStat And eRobotStat.PurgeOK) = eRobotStat.PurgeOK) Then
                    oRC.Purge = oImageList.Images("green")
                Else
                    oRC.Purge = oImageList.Images("red")
                End If

                'servo disconnect
                oImageList = CType(colImages.Item("imlDisc"), ImageList)

                If ((nRobotStat And eRobotStat.ServoDiscOff) = eRobotStat.ServoDiscOff) Then
                    oRC.ServoDisc = oImageList.Images("on")
                Else
                    oRC.ServoDisc = oImageList.Images("s_off")
                End If


                'servo selector switch
                oImageList = CType(colImages.Item("imldisc"), ImageList)

                If ((nRobotStat And eRobotStat.ServoDiscOff) = eRobotStat.ServoDiscOff) Then
                    oRC.ServoDisc = oImageList.Images("on")
                Else
                    oRC.ServoDisc = oImageList.Images("s_off")
                End If

            End If

                'Bypass switch
                oImageList = CType(colImages.Item("imlProxSwitch"), ImageList)

                If ((nRunStat And eRobotRunStat.ConvBypassed) = eRobotRunStat.ConvBypassed) Then
                    oDevice.ShowBypassProx = True
                    oDevice.BypassProx = oImageList.Images("warn")
                    oDevice.BypassProxTT = gpsRM.GetString("psCONV_INTLK_BYP_TT", frmMain.DisplayCulture)
                Else
                    oDevice.ShowBypassProx = False
                End If

                'e-stat
                oImageList = CType(colImages.Item("imlEstat"), ImageList)
                If ((nRunStat And eRobotRunStat.EstatDisabled) = eRobotRunStat.EstatDisabled) Then
                    oDevice.Estat = oImageList.Images("disabled")
                Else
                    oDevice.Estat = oImageList.Images("on")
                End If


            'Dim p As PictureBox = Nothing
            'Dim gpbTmp1 As GroupBox = DirectCast(pnlTmp.Controls("gpbFortressLeft"), GroupBox)
            'Dim gpbTmp2 As GroupBox = DirectCast(pnlTmp.Controls("gpbFortressRight"), GroupBox)


            'sName = "picMaintLock" & nDevice

            'If nDevice Mod 2 = 0 Then
            '    p = DirectCast(gpbTmp2.Controls(sName), PictureBox)
            'Else
            '    p = DirectCast(gpbTmp1.Controls(sName), PictureBox)
            'End If
            'If IsNothing(p) = False And Not bIsOpener Then
            '    Dim o As ImageList = DirectCast(colImages.Item("imlMaintDoor"), ImageList)
            '    If ((nRobotStat And eRobotStat.MaintDoorClosed) = eRobotStat.MaintDoorClosed) Then
            '        p.Image = o.Images.Item("maint_lock_green")
            '    Else
            '        p.Image = o.Images.Item("maint_lock_red")
            '    End If
            'End If  'IsNothing(p) = False


        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: RobotChange", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Sub subRunGhost()
        '********************************************************************************************
        'Description: Run a ghost job
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 3/31/10   MSW     Support ascii color and style
        '********************************************************************************************
        Dim bContinue As Boolean = True
        'Check password
        Try
            'If CheckPassword(frmMain.msSCREEN_NAME, ePrivilege.Execute, frmMain.moPassword, frmMain.moPrivilege) Then 'RJO 03/23/12
            If frmMain.moPassword.CheckPassword(ePrivilege.Execute) Then
                'ready for ghost?
                If (CInt(frmMain.msZoneData(eBooth.GhostInfoWd)) And eBooth.GhostZoneOK) <> eBooth.GhostZoneOK Then
                    'Not ready, try the editstatus screen
                    bContinue = frmGhostStatus.Show(frmMain.msZoneData)
                    frmGhostStatus.Dispose()
                End If 'status form
                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
                If bContinue Then 'after GhostStatus and Password OK
                    'Start the ghost form
                    frmEdit.EnableTutone = False
                    frmEdit.EnableVin = False
                    frmEdit.EnableCarrier = False
                    frmEdit.EnableOption = True
                    '01/25/10 if database has repairs panels greater then 0 then enable the repair panes
                    'on the edit screen.
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        frmEdit.EnableRepair = True
                        frmEdit.Repair = 0
                    Else
                        frmEdit.EnableRepair = False
                        frmEdit.Repair = 0
                    End If
                    frmEdit.SimConveyor = False
                    frmEdit.Ghost = True
                    frmEdit.ViewOnly = False
                    'Conveyor held?
                    If (CInt(frmMain.msZoneData(eBooth.InterlockWd)) And gnBitVal(eBooth.ConvRunSwitch)) <> gnBitVal(eBooth.ConvRunSwitch) Then
                        frmEdit.EnableSimConveyor = False
                    Else
                        frmEdit.EnableSimConveyor = True
                    End If
                    frmEdit.subInitCbos()
                    bContinue = frmEdit.Show
                End If 'bContinue 'after GhostStatus and Password OK
            Else
                bContinue = False
            End If 'password
        Catch ex As Exception
            ShowErrorMessagebox("frmMain.subRunGhost - Unable to verify ready for ghost ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
            bContinue = False
        End Try
        Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
        Try
            If bContinue Then 'write the ghost
                'Write track sim setting
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostSimConv"
                Dim s(0) As String
                Dim bSim As Boolean = frmEdit.SimConveyor
                If frmEdit.SimConveyor Then
                    s(0) = "1"
                Else
                    s(0) = "0"
                End If
                frmMain.mPLC.PLCData = s
                'Build edit data array
                Dim sData(eQueueEdit.Max) As String
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sData(nItem) = "0"
                Next
                sData(eQueueEdit.StatusWd) = CInt(eJobStatus.Ghost).ToString
                If frmEdit.AsciiStyleEnable Then
                    sData(eQueueEdit.StyleWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiStyle, colStyles.PlantAsciiMaxLength).ToString
                Else
                    sData(eQueueEdit.StyleWd) = frmEdit.StyleNo.ToString
                End If
                sData(eQueueEdit.OptionWd) = frmEdit.OptionNo.ToString
                If frmEdit.AsciiColorEnable Then
                    sData(eQueueEdit.ColorWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiColor, gnAsciiColorNumChar).ToString
                Else
                    sData(eQueueEdit.ColorWd) = frmEdit.ColorNo.ToString
                End If
                If colZones.ActiveZone.RepairPanels > 0 Then
                    sData(eQueueRead.RepairWd) = frmEdit.Repair.ToString
                End If
                sData(eQueueEdit.CarrierWd) = "99"
                'Write edit data to PLC
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostQue"
                frmMain.mPLC.PLCData = sData
                Application.DoEvents()

                'Write edit Position to PLC
                s(0) = "11" 'Hard coded to position 11
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueueEditPosition"
                frmMain.mPLC.PLCData = s
                Application.DoEvents()
                'Write complete flag to PLC
                s(0) = "1"
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditComplete"
                frmMain.mPLC.PLCData = s
                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
                'Write Change log
                Dim sTmp As String = String.Empty
                Dim sStyle As String = String.Empty
                Dim sColor As String = String.Empty
                If frmEdit.AsciiColorEnable Then
                    sColor = frmEdit.AsciiColor
                Else
                    sColor = sData(eQueueEdit.ColorWd)
                End If
                If frmEdit.AsciiStyleEnable Then
                    sStyle = frmEdit.AsciiStyle
                Else
                    sStyle = sData(eQueueEdit.StyleWd)
                End If

                sTmp = gpsRM.GetString("psGHOST_JOB_INIT") & " " & _
                         gpsRM.GetString("psSTYLE_LBL") & " " & sStyle & ", " & _
                         gpsRM.GetString("psOPTION_LBL") & " " & sData(eQueueEdit.OptionWd)
                If bSim Then
                    sTmp = sTmp & "," & gpsRM.GetString("psCONV_SIMULATED")
                End If
                If colZones.ActiveZone.RepairPanels > 0 Then
                    If frmEdit.Repair > 0 Then
                        sTmp = sTmp & "," & gpsRM.GetString("psREPAIR_LBL") & " " & sData(eQueueEdit.RepairWd)
                    End If
                End If

                AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                            colZones.CurrentZoneNumber, gcsRM.GetString("csALL"), sColor, sTmp, colZones.CurrentZone)
                'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            End If
        Catch ex As Exception
            ShowErrorMessagebox("rmMain.subRunGhost - Unable to write ghost to PLC", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try
    End Sub


    Friend Sub subRunZDT()
        '********************************************************************************************
        'Description: Run a ZDT ghost job
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bContinue As Boolean = True
        'Check password
        Try
            'If CheckPassword(frmMain.msSCREEN_NAME, ePrivilege.Execute, frmMain.moPassword, frmMain.moPrivilege) Then 'RJO 03/23/12
            If frmMain.moPassword.CheckPassword(ePrivilege.Execute) Then
                'ready for ghost?
                If (CInt(frmMain.msZoneData(eBooth.GhostInfoWd)) And eBooth.ZDTGhostZoneOK) <> eBooth.ZDTGhostZoneOK Then
                    'Not ready, try the editstatus screen
                    bContinue = frmGhostStatus.Show(frmMain.msZoneData, True)
                    frmGhostStatus.Dispose()
                End If 'status form
                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
                If bContinue Then 'after GhostStatus and Password OK
                    'Start the ghost form
                    frmEdit.EnableTutone = False
                    frmEdit.EnableVin = False
                    frmEdit.EnableCarrier = False
                    frmEdit.EnableOption = False
                    '01/25/10 if database has repairs panels greater then 0 then enable the repair panes
                    'on the edit screen.
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        frmEdit.EnableRepair = True
                        frmEdit.Repair = 0
                    Else
                        frmEdit.EnableRepair = False
                        frmEdit.Repair = 0
                    End If
                    frmEdit.SimConveyor = False
                    frmEdit.Ghost = True
                    frmEdit.ViewOnly = False
                    'Conveyor held?
                    If (CInt(frmMain.msZoneData(eBooth.InterlockWd)) And gnBitVal(eBooth.ConvRunSwitch)) <> gnBitVal(eBooth.ConvRunSwitch) Then
                        frmEdit.EnableSimConveyor = False
                    Else
                        frmEdit.EnableSimConveyor = True
                    End If
                    frmEdit.subInitCbos(True)
                    bContinue = frmEdit.Show
                End If 'bContinue 'after GhostStatus and Password OK
            Else
                bContinue = False
            End If 'password
        Catch ex As Exception
            ShowErrorMessagebox("frmMain.subRunZDT - Unable to verify ready for ghost ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
            bContinue = False
        End Try
        Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
        Try
            If bContinue Then 'write the ghost
                'Write track sim setting
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostSimConv"
                Dim s(0) As String
                Dim bSim As Boolean = frmEdit.SimConveyor
                If frmEdit.SimConveyor Then
                    s(0) = "1"
                Else
                    s(0) = "0"
                End If
                frmMain.mPLC.PLCData = s
                'Build edit data array
                Dim sData(eQueueEdit.Max) As String
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sData(nItem) = "0"
                Next
                sData(eQueueEdit.StatusWd) = CInt(eJobStatus.Ghost).ToString
                If frmEdit.AsciiStyleEnable Then
                    sData(eQueueEdit.StyleWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiStyle, colStyles.PlantAsciiMaxLength).ToString
                Else
                    sData(eQueueEdit.StyleWd) = frmEdit.StyleNo.ToString
                End If
                sData(eQueueEdit.OptionWd) = frmEdit.OptionNo.ToString
                If frmEdit.AsciiColorEnable Then
                    sData(eQueueEdit.ColorWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiColor, gnAsciiColorNumChar).ToString
                Else
                    sData(eQueueEdit.ColorWd) = frmEdit.ColorNo.ToString
                End If
                If colZones.ActiveZone.RepairPanels > 0 Then
                    sData(eQueueRead.RepairWd) = frmEdit.Repair.ToString
                End If
                sData(eQueueEdit.CarrierWd) = "99"
                'Write edit data to PLC
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostQue"
                frmMain.mPLC.PLCData = sData
                Application.DoEvents()

                'Write edit Position to PLC
                s(0) = "11" 'Hard coded to position 11
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueueEditPosition"
                frmMain.mPLC.PLCData = s
                Application.DoEvents()
                'Write complete flag to PLC
                s(0) = "1"
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditComplete"
                frmMain.mPLC.PLCData = s
                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
                'Write Change log
                Dim sTmp As String = String.Empty
                Dim sStyle As String = String.Empty
                Dim sColor As String = String.Empty
                If frmEdit.AsciiColorEnable Then
                    sColor = frmEdit.AsciiColor
                Else
                    sColor = sData(eQueueEdit.ColorWd)
                End If
                If frmEdit.AsciiStyleEnable Then
                    sStyle = frmEdit.AsciiStyle
                Else
                    sStyle = sData(eQueueEdit.StyleWd)
                End If

                sTmp = gpsRM.GetString("psGHOST_JOB_INIT") & " " & _
                         gpsRM.GetString("psSTYLE_LBL") & " " & sStyle & ", " & _
                         gpsRM.GetString("psOPTION_LBL") & " " & sData(eQueueEdit.OptionWd)
                If bSim Then
                    sTmp = sTmp & "," & gpsRM.GetString("psCONV_SIMULATED")
                End If
                If colZones.ActiveZone.RepairPanels > 0 Then
                    If frmEdit.Repair > 0 Then
                        sTmp = sTmp & "," & gpsRM.GetString("psREPAIR_LBL") & " " & sData(eQueueEdit.RepairWd)
                    End If
                End If

                AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                            colZones.CurrentZoneNumber, gcsRM.GetString("csALL"), sColor, sTmp, colZones.CurrentZone)
                'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            End If
        Catch ex As Exception
            ShowErrorMessagebox("rmMain.subRunZDT - Unable to write ghost to PLC", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try
    End Sub
    Private Sub subEditQueue(ByVal sender As Object)
        '********************************************************************************************
        'Description: Queue Edit, click handler for queue control
        '
        'Parameters: queue object that was clicked
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim ndata As Integer
        Dim lReply As Windows.Forms.DialogResult = DialogResult.Retry
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim bViewOnly As Boolean = False
        Dim bContinue As Boolean = False
        Dim bCancel As Boolean = False
        Do
            ndata = CInt(frmMain.msZoneData(eBooth.InterlockWd))
            If (ndata And gnBitVal(eBooth.ConvRunSwitch)) = gnBitVal(eBooth.ConvRunSwitch) Then
                'lReply = DialogResult.OK
                bContinue = True
            Else
                frmMultipleChoiceDlg.label = gpsRM.GetString("psHOLD_CONV_FOR_EDIT", oCulture)
                frmMultipleChoiceDlg.caption = gpsRM.GetString("psQUEUE_EDIT", oCulture)
                frmMultipleChoiceDlg.NumButtons = 3
                frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psRETRY", oCulture)
                frmMultipleChoiceDlg.ButtonText(1) = gcsRM.GetString("csCANCEL", oCulture)
                frmMultipleChoiceDlg.ButtonText(2) = gpsRM.GetString("psVIEWONLY", oCulture)
                frmMultipleChoiceDlg.ShowDialog()
                'lReply = MessageBox.Show(gpsRM.GetString("psHOLD_CONV_FOR_EDIT", oCulture), _
                '    gpsRM.GetString("psQUEUE_EDIT", oCulture), MessageBoxButtons.RetryCancel)
                'If lReply = DialogResult.Cancel Then Exit Sub
                Application.DoEvents() 'let the hotlink fire
                Select Case frmMultipleChoiceDlg.SelectedButton
                    Case 0
                        'Retry
                    Case 1
                        bCancel = True
                    Case 2
                        bViewOnly = True
                        bContinue = True
                End Select
            End If
        Loop Until bContinue Or bCancel
        'Loop Until lReply = DialogResult.OK

        frmMain.Cursor = System.Windows.Forms.Cursors.Default

        Dim nQueuePos As Integer = 0
        Dim s(0) As String
        Dim sQData As String()
        Dim sChangeLogQPos As String = String.Empty

        Try
            'Check queue position#
            Dim oCtrl As Control = DirectCast(sender, Control)

            If oCtrl.Name.Substring(0, 8) = "UctlPreQ" Then
                'Predetect Queue
                nQueuePos = CInt(oCtrl.Name.Substring(8))
                'the double conversion on queue positino number should remove any leading 0s
                sChangeLogQPos = gpsRM.GetString("psPREDETECTQUEPOS", oCulture) & CInt(oCtrl.Name.Substring(8)).ToString
            ElseIf oCtrl.Name.Substring(0, 9) = "UctlPosQ" Then
                'Postdetect Queue
                nQueuePos = -1 * CInt(oCtrl.Name.Substring(9))
                'TODO - most places dont' allow this
                bContinue = False
                'the double conversion on queue positino number should remove any leading 0s
                sChangeLogQPos = gpsRM.GetString("psPOSDETECTQUEPOS", oCulture) & CInt(oCtrl.Name.Substring(9)).ToString
            Else
                nQueuePos = 0
                bContinue = False
            End If

            'bContinue = bContinue And CheckPassword(frmMain.msSCREEN_NAME, ePrivilege.Execute, frmMain.moPassword, frmMain.moPrivilege)
            bContinue = bContinue And frmMain.moPassword.CheckPassword(ePrivilege.Execute)

            If bContinue Then 'OK to start edit
                If bViewOnly = False Then
                    'Write "In Progress" flag to PLC
                    s(0) = "1"
                    frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditInProgress"
                    frmMain.mPLC.PLCData = s
                End If

                'Get current data from PLC
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueuePos" & nQueuePos.ToString
                sQData = frmMain.mPLC.PLCData

                'Launch the edit form
                frmEdit.AsciiColorEnable = True 'RJO 09/14/14
                frmEdit.EnableTutone = False
                frmEdit.VinDigits = gnVinChars
                frmEdit.VinForceNumeric = gbNumericVin
                frmEdit.EnableVin = False
                frmEdit.EnableCarrier = True
                frmEdit.EnableOption = True
                frmEdit.SimConveyor = False
                frmEdit.ViewOnly = bViewOnly
                '01/25/10 if database has repairs panels greater then 0 then enable the repair panes
                'on the edit screen.
                If colZones.ActiveZone.RepairPanels > 0 Then
                    frmEdit.EnableRepair = True
                    frmEdit.Repair = CInt(sQData(eQueueRead.RepairWd))
                Else
                    frmEdit.EnableRepair = False
                    frmEdit.Repair = 0
                End If

                frmEdit.Ghost = False
                frmEdit.subInitCbos()
                If CInt(sQData(eQueueRead.CarrierType)) = 3 Then
                    frmEdit.JobStatus = 3
                Else
                    frmEdit.JobStatus = CInt(sQData(eQueueRead.StatusWd))
                End If
                If frmEdit.AsciiStyleEnable Then
                    frmEdit.AsciiStyle = mMathFunctions.CvIntegerToASCII(CInt(sQData(eQueueRead.StyleWd)), colStyles.PlantAsciiMaxLength)
                Else
                    frmEdit.StyleNo = CInt(sQData(eQueueRead.StyleWd))
                End If
                frmEdit.OptionNo = CInt(sQData(eQueueRead.OptionWd))
                If frmEdit.AsciiColorEnable Then
                    frmEdit.AsciiColor = mMathFunctions.CvIntegerToASCII(CInt(sQData(eQueueRead.ColorWd)), gnAsciiColorNumChar)
                Else
                    frmEdit.ColorNo = CInt(sQData(eQueueRead.ColorWd))
                End If
                frmEdit.Carrier = sQData(eQueueRead.CarrierWd)
                frmEdit.QueuePos = nQueuePos
                Dim sVinOld As String = String.Empty
                Dim nChars As Integer = gnVinChars Mod 4
                If gbNumericVin Then
                    'Numeric vin  
                    sVinOld = sQData(eQueueRead.VinWord)
                Else
                    Dim sTmp As String = String.Empty
                    For nIdx As Integer = eQueueRead.VinLength + eQueueRead.VinWord - 1 To eQueueRead.VinWord Step -1
                        '''''''''''''''''''''''''''''''''''''''''
                        'ASCII VIN
                        If IsNumeric(sQData(nIdx)) Then
                            sTmp = mMathFunctions.CvIntegerToASCII(CInt(sQData(nIdx)), nChars)
                            nChars = 4 'only the first pass can be shorter
                        Else
                            sTmp = String.Empty
                        End If
                        '''''''''''''''''''''''''''''''''''''''''
                        '''''''''''''''''''''''''''''''''''''''''
                        sVinOld = sVinOld & sTmp
                    Next
                End If

                frmEdit.Vin = sVinOld
                Application.DoEvents()
                bContinue = frmEdit.Show
                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
            Else
                'This is just to get rid of warnings
                ReDim sQData(0)
                sQData(0) = String.Empty
            End If ''OK to start edit

                Application.DoEvents() 'MSW 8/25/09 allow it to redraw 

                If bContinue And (Not (bViewOnly)) Then 'write the ghost
                    'Build edit data array
                    Dim sData(eQueueEdit.Max) As String

                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        sData(nItem) = "0"
                    Next

                    sData(eQueueEdit.StatusWd) = frmEdit.JobStatus.ToString
                    If frmEdit.AsciiStyleEnable Then
                        sData(eQueueEdit.StyleWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiStyle, colStyles.PlantAsciiMaxLength).ToString
                    Else
                        sData(eQueueEdit.StyleWd) = frmEdit.StyleNo.ToString
                    End If
                    sData(eQueueEdit.OptionWd) = frmEdit.OptionNo.ToString
                    If frmEdit.AsciiColorEnable Then
                        sData(eQueueEdit.ColorWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiColor, gnAsciiColorNumChar).ToString
                    Else
                        sData(eQueueEdit.ColorWd) = frmEdit.ColorNo.ToString
                    End If
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        sData(eQueueRead.RepairWd) = frmEdit.Repair.ToString
                    End If
                    sData(eQueueEdit.CarrierWd) = frmEdit.Carrier
                    'VIN
                    Dim sVinNew As String = String.Empty
                    Dim sVinOld As String = frmEdit.Vin
                    If sVinOld Is String.Empty Then
                        sVinOld = "0"
                    End If
                    Dim sTmp As String = sVinOld
                    For nIdx As Integer = 0 To eQueueRead.VinLength - 1
                        If gbNumericVin Then
                            sData(eQueueEdit.VinWord) = sTmp.ToString
                        Else
                            'ASCII VIN
                            If sTmp.Length > 4 Then
                                sData(eQueueEdit.VinWord + nIdx) = mMathFunctions.CvASCIIToInteger(sTmp.Substring(sTmp.Length - 4), 4).ToString
                                sTmp = sTmp.Substring(0, sTmp.Length - 4)
                            ElseIf sTmp <> String.Empty Then
                                sData(eQueueEdit.VinWord + nIdx) = mMathFunctions.CvASCIIToInteger(sTmp, 4).ToString
                                sTmp = String.Empty
                            Else
                                sData(eQueueEdit.VinWord + nIdx) = "0"
                            End If
                            Dim nTmp As Integer = eQueueRead.VinLength - nIdx
                        End If
                    Next
                    frmEdit.Vin = sVinOld

                    frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditData"
                    frmMain.mPLC.PLCData = sData
                    'Write edit Position to PLC
                    s(0) = nQueuePos.ToString
                    frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueueEditPosition"
                    frmMain.mPLC.PLCData = s

                    Application.DoEvents()

                    System.Threading.Thread.Sleep(500)
                    Application.DoEvents()
                    'Write complete flag to PLC
                    s(0) = "1"
                    frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditComplete"
                    frmMain.mPLC.PLCData = s

                    Application.DoEvents() 'MSW 8/25/09 allow it to redraw 
                    'Write Change log
                    Dim sOld As String = String.Empty
                    Dim sNew As String = String.Empty
                    Dim sColor As String = String.Empty
                    If frmEdit.AsciiColorEnable Then
                        sColor = mMathFunctions.CvIntegerToASCII(CInt(sData(eQueueRead.ColorWd)), gnAsciiColorNumChar)
                    Else
                        sColor = sData(eQueueRead.ColorWd)
                    End If
                    sChangeLogQPos = sChangeLogQPos & ", " & gpsRM.GetString("psCARRIER", oCulture) & " # " & sQData(eQueueRead.CarrierWd) & " "
                    If sData(eQueueEdit.StyleWd) <> sQData(eQueueRead.StyleWd) Then
                        If frmEdit.AsciiStyleEnable Then
                            sOld = mMathFunctions.CvIntegerToASCII(CInt(sQData(eQueueRead.StyleWd)), colStyles.PlantAsciiMaxLength)
                            sNew = mMathFunctions.CvIntegerToASCII(CInt(sData(eQueueRead.StyleWd)), colStyles.PlantAsciiMaxLength)
                        Else
                            sOld = sQData(eQueueRead.StyleWd)
                            sNew = sData(eQueueRead.StyleWd)
                        End If
                        sTmp = sChangeLogQPos & _
                                     gpsRM.GetString("psSTYLE", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                     sOld & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                     sNew
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If
                    If colZones.ActiveZone.RepairPanels > 0 Then
                        sData(eQueueRead.RepairWd) = frmEdit.Repair.ToString
                    End If

                    If sData(eQueueEdit.OptionWd) <> sQData(eQueueRead.OptionWd) Then
                        sTmp = sChangeLogQPos & _
                                 gpsRM.GetString("psOPTION", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                 sQData(eQueueRead.OptionWd) & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                 sData(eQueueEdit.OptionWd)
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If

                    If sData(eQueueEdit.RepairWd) <> sQData(eQueueRead.RepairWd) Then
                        sTmp = sChangeLogQPos & _
                                 gpsRM.GetString("psREPAIR", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                 sQData(eQueueRead.RepairWd) & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                 sData(eQueueEdit.RepairWd)
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If
                    If sData(eQueueEdit.ColorWd) <> sQData(eQueueRead.ColorWd) Then
                        If frmEdit.AsciiColorEnable Then
                            sOld = mMathFunctions.CvIntegerToASCII(CInt(sQData(eQueueRead.ColorWd)), gnAsciiColorNumChar)
                        Else
                            sOld = sQData(eQueueRead.ColorWd)
                        End If
                        sTmp = sChangeLogQPos & _
                                 gpsRM.GetString("psCOLOR", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                 sOld & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                 sColor
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If

                    If sData(eQueueEdit.CarrierWd) <> sQData(eQueueRead.CarrierWd) Then
                        sTmp = sChangeLogQPos & _
                                 gpsRM.GetString("psCARRIER", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                 sQData(eQueueRead.CarrierWd) & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                 sData(eQueueEdit.CarrierWd)
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If

                    If sData(eQueueEdit.StatusWd) <> sQData(eQueueRead.StatusWd) Then
                        sTmp = sChangeLogQPos & _
                                     gpsRM.GetString("psJOB_STATUS", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                     sQData(eQueueRead.StatusWd) & " " & gcsRM.GetString("csTO", oCulture) & " " & _
                                     sData(eQueueEdit.StatusWd)
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                    colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sColor, sTmp, colZones.CurrentZone)
                    End If

                    If sData(eQueueEdit.VinWord) <> sQData(eQueueRead.VinWord) Then
                        sTmp = sChangeLogQPos & _
                                 gpsRM.GetString("psVIN", oCulture) & " " & gcsRM.GetString("csCHANGED_FROM", oCulture) & " " & _
                                 sVinOld & " " & gcsRM.GetString("csTO", oCulture) & " " & sVinNew
                        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                        colZones.CurrentZoneNumber, gcsRM.GetString("csALL", oCulture), sData(eQueueEdit.ColorWd), sTmp, colZones.CurrentZone)
                    End If

                'Call mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                    Call mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                End If

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("rmMain.subRunGhost - Unable to write ghost to PLC", _
                                            ex, frmMain.msSCREEN_NAME, frmMain.Status)
        Finally
            'Write "In Progress" flag to PLC
            s(0) = "0"
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditInProgress"
            frmMain.mPLC.PLCData = s

        End Try

        frmMain.Cursor = System.Windows.Forms.Cursors.Default

    End Sub
    Public Sub subEditMyQueue(ByVal sender As Object)
        '********************************************************************************************
        'Description: Queue Edit, click handler for queue control
        '
        'Parameters: queue object that was clicked
        'Returns:    none
        '
        'Modification history:
        '
        'Date      By      Reason
        'NVSP 02282017 Edit the queue and update the change log
        '********************************************************************************************

        Dim OriginalColor As String
        Dim OriginalStyle As String

        Dim lReply As Windows.Forms.DialogResult = DialogResult.Retry
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim bContinue As Boolean = True
        Dim bCancel As Boolean = False
        Dim Idx As Integer
        Dim sTmp As String
        Dim msXMLFilePath As String = String.Empty


        Dim sColorValveNames() As String = Nothing
        Dim bColorsByStyle As Boolean
        Dim bUse2K As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean

        Const sXMLFILE As String = "SystemStyles"
        Const sXMLTABLE As String = "SystemStyles"
        Const sXMLNODE As String = "Style"

        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oStyleColorList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Dim sXMLFilePath As String = String.Empty

        Dim mcolZones As clsZones = colZones


        frmMain.Cursor = System.Windows.Forms.Cursors.Default

        Dim nQueuePos As Integer = 0
        Dim s(0) As String
        Dim sQData As String()

        Dim sChangeLogQPos As String = String.Empty

        Try
            ' Check for permission to edit
            If frmMain.moPassword.UserName = Nothing Then
                Return
            Else
                frmMain.moPassword.CheckPassword(ePrivilege.Execute)
            End If

            'NVSP 02/28/2017 Mark the queue as being edited.
            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueueEditInProgress"
            s(0) = "1"
            frmMain.mPLC.PLCData = s


            ' Get the queue position number to edit
            Dim oCtrl As Control = DirectCast(sender, Control)
            If oCtrl.Name.StartsWith("Carrier") Then
                nQueuePos = CInt(oCtrl.Name.Replace("Carrier", ""))
            Else
                Return
            End If

            'Get current data from PLC
            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueuePos" & nQueuePos.ToString
            sQData = frmMain.mPLC.PLCData

            ' Configure the Edit window
            frmEdit.EnableCarrier = True
            'frmEdit.txtCarrier.ReadOnly = True
            frmEdit.EnableOption = True
            frmEdit.EnableTutone = False
            frmEdit.EnableVin = False
            frmEdit.SimConveyor = False
            frmEdit.ViewOnly = False
            frmEdit.subInitCbos()

            ' Set the job data in the edit window
            frmEdit.QueuePos = nQueuePos

            ' NVSP 02/28/2017 - Changed to grabbing the full job data so we can have access to some debug data too...
            frmEdit.txtCarrier.Text = sQData(eMyQueueEdit.CarrierWd)
            frmEdit.cboStyle.SelectedIndex = frmEdit.cboStyle.FindString(sQData(eMyQueueEdit.StyleWd))
            frmEdit.cboColor.SelectedIndex = frmEdit.cboColor.FindString(sQData(eMyQueueEdit.ColorWd))
            frmEdit.cboOption.SelectedIndex = frmEdit.cboOption.FindString(sQData(eMyQueueEdit.OptionWd))
            'frmEdit.cboJobStatus.SelectedIndex = CInt(sQData(0))

            Dim myJobStatus As String() = DirectCast(frmEdit.cboJobStatus.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To frmEdit.cboJobStatus.Items.Count - 1
                If CInt(myJobStatus(nVal)) = CInt(sQData(0)) Then
                    frmEdit.cboJobStatus.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                frmEdit.cboJobStatus.SelectedIndex = -1
                frmEdit.cboJobStatus.Text = String.Empty
            End If
           

            ' Show the edit window
            Application.DoEvents()
            Thread.Sleep(10)
            If Not frmEdit.Show Then Return
            Application.DoEvents()
            Thread.Sleep(10)


            ' Get the original color description for change log
            Dim FileExists As Boolean = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            If FileExists Then
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & mcolZones.ActiveZone.Name & "//" & sXMLNODE)
            End If

            Dim nStyle As Integer = CType(sQData(eMyQueueEdit.StyleWd), Integer)
            If nStyle > 0 And nStyle < 100 Then
                Dim styles As New clsSysStyle(nStyle, oNodeList(nStyle - 1), mcolZones.Item(mcolZones.ActiveZone.Name), False)
                OriginalStyle = styles.Description.Text
            Else
                OriginalStyle = "None"
            End If


            ' Get the original Color Description for change log
            Dim oColors As XmlNodeList = Nothing
            GetSystemColorInfoFromDB(mcolZones.ActiveZone, oColors, _
                                    sColorValveNames, gbAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, gnAsciiColorNumChar, bTwoCoats)

            Dim nColor As Integer = CType(sQData(eMyQueueEdit.ColorWd), Integer)
            If nColor > 0 And nColor < 100 Then
                OriginalColor = oColors(nColor - 1).Item(gsSYSC_COL_DESC).InnerText
            Else
                OriginalColor = "None"
            End If

            'Build edit data array
            Dim sData(eMyQueueEdit.Max) As String
            For nItem As Integer = 0 To sData.GetUpperBound(0)
                sData(nItem) = "0"
            Next

            ' Job Status
            Dim jobStatus As Integer = frmEdit.JobStatus
            'jobStatus += 1
            sData(eMyQueueEdit.StatusWd) = jobStatus.ToString

            ' Carrier Number
            sData(eMyQueueEdit.CarrierWd) = frmEdit.Carrier.ToString

            ' Style Number
            If frmEdit.AsciiStyleEnable Then
                sData(eMyQueueEdit.StyleWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiStyle, colStyles.PlantAsciiMaxLength).ToString
            Else
                sData(eMyQueueEdit.StyleWd) = frmEdit.StyleNo.ToString
            End If

            ' Color Number 
            If frmEdit.AsciiColorEnable Then
                sData(eMyQueueEdit.ColorWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiColor, gnAsciiColorNumChar).ToString
            Else
                sData(eMyQueueEdit.ColorWd) = frmEdit.ColorNo.ToString
            End If

            ' Option Number
            sData(eMyQueueEdit.OptionWd) = frmEdit.OptionNo.ToString

            ' Queue Edit Position
            'sData(eMyQueueEdit.QueuePositionWd) = nQueuePos.ToString

            ' Send data to the PLC
            Dim colzones As clsZones = mcolZones
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueEditData"
            frmMain.mPLC.PLCData = sData
            'Write edit Position to PLC
            s(0) = nQueuePos.ToString
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueueEditPosition"
            frmMain.mPLC.PLCData = s

            Application.DoEvents()

            System.Threading.Thread.Sleep(500)
            Application.DoEvents()
            'Write complete flag to PLC
            s(0) = "1"
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueEditComplete"
            frmMain.mPLC.PLCData = s

            Application.DoEvents()

            Dim msDBPath As String = String.Empty
            GetDefaultFilePath(msDBPath, eDir.Database, String.Empty, String.Empty)

            ' Update change log
            For Idx = 0 To 10

                GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, "SystemColors.XML")
                mPWCommon.SaveToChangeLog(colzones.ActiveZone)

                'Write change for each changed parameter 
                If sQData(Idx) <> sData(Idx) Then
                    sTmp = "Queue Edit " & gpsRM.GetString("psCARRIER") & " " & sData(eMyQueueEdit.CarrierWd) & ": "

                    Select Case Idx
                        Case 0
                            If sData(eMyQueueEdit.StatusWd) = "1" Then
                                sTmp = sTmp & gpsRM.GetString("psJOB_STATUS") & ": from [No Paint]  to  [Paint]"
                            Else
                                sTmp = sTmp & gpsRM.GetString("psJOB_STATUS") & ": from [Paint]  to  [No Paint]"
                            End If
                        Case 8
                            sTmp = sTmp & gpsRM.GetString("psSTYLE_LBL") & " from [" & sQData(eMyQueueEdit.StyleWd) & " - " _
                            & OriginalStyle & "]  to  [" & frmEdit.cboStyle.Text & "]"
                        Case 9
                            sTmp = sTmp & gpsRM.GetString("psCOLOR_LBL") & " from [" & sQData(eMyQueueEdit.ColorWd) & " - " _
                            & OriginalColor & "]  to  [" & frmEdit.cboColor.Text & "]"
                        Case 10
                            sTmp = sTmp & gpsRM.GetString("psOPTION_LBL") & " from " & sQData(Idx) & "  to  " & sData(eMyQueueEdit.OptionWd)
                    End Select

                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                       colzones.CurrentZoneNumber, gcsRM.GetString("csALL"), sData(eMyQueueEdit.ColorWd), sTmp, colzones.CurrentZone)
                    mPWCommon.SaveToChangeLog(colzones.ActiveZone)
                End If
            Next



        Catch ex As Exception
            ShowErrorMessagebox("Unable to complete the queue edit.", ex, frmMain.msSCREEN_NAME, frmMain.Status)

        Finally
            ' DAW 20151208 - Moved to finally block
            Try
                'NRU 150519 Unmark the queue as being edited.
                frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueueEditInProgress"
                s(0) = "0"
                frmMain.mPLC.PLCData = s

            Catch ex As Exception
                ShowErrorMessagebox("Unable to notifiy the PLC that the queue edit is complete.", ex, frmMain.msSCREEN_NAME, frmMain.Status)

            End Try
        End Try
        frmMain.Cursor = System.Windows.Forms.Cursors.Default

    End Sub
    Public Sub InsertJobToMyQueue(ByVal sender As Object, ByVal nqueposition As String)
        '********************************************************************************************
        'Description: Insert a job in a queue at desired location
        '
        'Parameters: queue object that was clicked
        'Returns:    none
        '
        'Modification history:
        '
        'Date      By      Reason
        'NVSP 03032017 
        '********************************************************************************************

        Dim OriginalColor As String
        Dim OriginalStyle As String

        Dim lReply As Windows.Forms.DialogResult = DialogResult.Retry
        Dim oCulture As Globalization.CultureInfo = frmMain.DisplayCulture
        Dim bContinue As Boolean = True
        Dim bCancel As Boolean = False
        Dim Idx As Integer
        Dim sTmp As String
        Dim msXMLFilePath As String = String.Empty


        Dim sColorValveNames() As String = Nothing
        Dim bColorsByStyle As Boolean
        Dim bUse2K As Boolean
        Dim bUseTricoat As Boolean
        Dim bTwoCoats As Boolean

        Const sXMLFILE As String = "SystemStyles"
        Const sXMLTABLE As String = "SystemStyles"
        Const sXMLNODE As String = "Style"

        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oStyleColorList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Dim sXMLFilePath As String = String.Empty

        Dim mcolZones As clsZones = colZones
        Dim s(0) As String
        Dim sQData As String()
        Dim sChangeLogQPos As String = String.Empty

        frmMain.Cursor = System.Windows.Forms.Cursors.Default

        Try
            ' Check for permission to edit
            If frmMain.moPassword.UserName = Nothing Then
                Return
            Else
                frmMain.moPassword.CheckPassword(ePrivilege.Execute)
            End If

            'NVSP 02/28/2017 Mark the queue as being edited.
            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueueEditInProgress"
            s(0) = "1"
            frmMain.mPLC.PLCData = s

            'Get current data from PLC
            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueuePos" & nqueposition
            sQData = frmMain.mPLC.PLCData

            'NVSP 02/28/2017 This triggers the queue copy by 50 positions and then flushes the current 50 positions in plc.

            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "PositiontoInsert"
            s(0) = nqueposition
            frmMain.mPLC.PLCData = s

            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "RoomToAddJob"
            s(0) = "1"
            
            frmMain.mPLC.PLCData = s

            ' Configure the Edit window
            frmEdit.EnableCarrier = True
            frmEdit.EnableOption = True
            frmEdit.EnableTutone = False
            frmEdit.EnableVin = False
            frmEdit.SimConveyor = False
            frmEdit.ViewOnly = False
            frmEdit.subInitCbos()

            ' Set the job data in the edit window
            frmEdit.QueuePos = CInt(nqueposition)
            frmEdit.txtCarrier.Text = sQData(eMyQueueEdit.CarrierWd)
            frmEdit.cboStyle.SelectedIndex = frmEdit.cboStyle.FindString(sQData(eMyQueueEdit.StyleWd))
            frmEdit.cboColor.SelectedIndex = frmEdit.cboColor.FindString(sQData(eMyQueueEdit.ColorWd))
            frmEdit.cboOption.SelectedIndex = frmEdit.cboOption.FindString(sQData(eMyQueueEdit.OptionWd))

            Dim myJobStatus As String() = DirectCast(frmEdit.cboJobStatus.Tag, String())
            Dim bFound As Boolean = False
            For nVal As Integer = 0 To frmEdit.cboJobStatus.Items.Count - 1
                If CInt(myJobStatus(nVal)) = CInt(sQData(0)) Then
                    frmEdit.cboJobStatus.SelectedIndex = nVal
                    bFound = True
                End If
            Next
            If bFound = False Then
                frmEdit.cboJobStatus.SelectedIndex = -1
                frmEdit.cboJobStatus.Text = String.Empty
            End If

            ' Show the edit window
            Application.DoEvents()
            Thread.Sleep(10)
            If Not frmEdit.Show Then Return
            Application.DoEvents()
            Thread.Sleep(10)

            ' Get the original color description for change log
            Dim FileExists As Boolean = GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML")
            If FileExists Then
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & mcolZones.ActiveZone.Name & "//" & sXMLNODE)
            End If

            Dim nStyle As Integer = CType(sQData(eMyQueueEdit.StyleWd), Integer)
            If nStyle > 0 And nStyle < 100 Then
                Dim styles As New clsSysStyle(nStyle, oNodeList(nStyle - 1), mcolZones.Item(mcolZones.ActiveZone.Name), False)
                OriginalStyle = styles.Description.Text
            Else
                OriginalStyle = "None"
            End If


            ' Get the original Color Description for change log
            Dim oColors As XmlNodeList = Nothing
            GetSystemColorInfoFromDB(mcolZones.ActiveZone, oColors, _
                                    sColorValveNames, gbAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, gnAsciiColorNumChar, bTwoCoats)

            Dim nColor As Integer = CType(sQData(eMyQueueEdit.ColorWd), Integer)
            If nColor > 0 And nColor < 100 Then
                OriginalColor = oColors(nColor - 1).Item(gsSYSC_COL_DESC).InnerText
            Else
                OriginalColor = "None"
            End If

            'Build edit data array
            Dim sData(eMyQueueEdit.Max) As String
            For nItem As Integer = 0 To sData.GetUpperBound(0)
                sData(nItem) = "0"
            Next

            ' Job Status
            sData(eMyQueueEdit.StatusWd) = frmEdit.JobStatus.ToString

            ' Carrier Number
            sData(eMyQueueEdit.CarrierWd) = frmEdit.Carrier.ToString

            ' Style Number
            If frmEdit.AsciiStyleEnable Then
                sData(eMyQueueEdit.StyleWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiStyle, colStyles.PlantAsciiMaxLength).ToString
            Else
                sData(eMyQueueEdit.StyleWd) = frmEdit.StyleNo.ToString
            End If

            ' Color Number 
            If frmEdit.AsciiColorEnable Then
                sData(eMyQueueEdit.ColorWd) = mMathFunctions.CvASCIIToInteger(frmEdit.AsciiColor, gnAsciiColorNumChar).ToString
            Else
                sData(eMyQueueEdit.ColorWd) = frmEdit.ColorNo.ToString
            End If

            ' Option Number
            sData(eMyQueueEdit.OptionWd) = frmEdit.OptionNo.ToString

            'Space Created and no more clearing
            frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "RoomToAddJob"
            s(0) = "0"
            frmMain.mPLC.PLCData = s

            ' Send data to the PLC
            Dim colzones As clsZones = mcolZones
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueEditData"
            frmMain.mPLC.PLCData = sData

            'Write edit Position to PLC
            s(0) = nqueposition
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueueEditPosition"
            frmMain.mPLC.PLCData = s

            Application.DoEvents()

            System.Threading.Thread.Sleep(500)
            Application.DoEvents()
            'Write complete flag to PLC
            s(0) = "1"
            frmMain.mPLC.TagName = "Z" & colzones.CurrentZoneNumber & "QueEditComplete"
            frmMain.mPLC.PLCData = s

            Application.DoEvents()

            Dim msDBPath As String = String.Empty
            GetDefaultFilePath(msDBPath, eDir.Database, String.Empty, String.Empty)

            ' Update change log
            For Idx = 0 To 10

                GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, "SystemColors.XML")
                mPWCommon.SaveToChangeLog(colzones.ActiveZone)

                'Write change for each changed parameter 
                If sQData(Idx) <> sData(Idx) Then
                    sTmp = "Queue Edit " & gpsRM.GetString("psCARRIER") & " " & sData(eMyQueueEdit.CarrierWd) & ": "

                    Select Case Idx
                        Case 0
                            If sData(eMyQueueEdit.StatusWd) = "1" Then
                                sTmp = sTmp & gpsRM.GetString("psJOB_STATUS") & ": from [No Paint]  to  [Paint]"
                            Else
                                sTmp = sTmp & gpsRM.GetString("psJOB_STATUS") & ": from [Paint]  to  [No Paint]"
                            End If
                        Case 8
                            sTmp = sTmp & gpsRM.GetString("psSTYLE_LBL") & " from [" & sQData(eMyQueueEdit.StyleWd) & " - " _
                            & OriginalStyle & "]  to  [" & frmEdit.cboStyle.Text & "]"
                        Case 9
                            sTmp = sTmp & gpsRM.GetString("psCOLOR_LBL") & " from [" & sQData(eMyQueueEdit.ColorWd) & " - " _
                            & OriginalColor & "]  to  [" & frmEdit.cboColor.Text & "]"
                        Case 10
                            sTmp = sTmp & gpsRM.GetString("psOPTION_LBL") & " from " & sQData(Idx) & "  to  " & sData(eMyQueueEdit.OptionWd)
                    End Select

                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                       colzones.CurrentZoneNumber, gcsRM.GetString("csALL"), sData(eMyQueueEdit.ColorWd), sTmp, colzones.CurrentZone)
                    mPWCommon.SaveToChangeLog(colzones.ActiveZone)
                End If
            Next

        Catch ex As Exception
            ShowErrorMessagebox("Unable to complete the queue edit.", ex, frmMain.msSCREEN_NAME, frmMain.Status)

        Finally
            Try
                frmMain.mPLC.TagName = "Z" & mcolZones.CurrentZoneNumber & "QueueEditInProgress"
                s(0) = "0"
                frmMain.mPLC.PLCData = s

            Catch ex As Exception
                ShowErrorMessagebox("Unable to notifiy the PLC that the queue edit is complete.", ex, frmMain.msSCREEN_NAME, frmMain.Status)

            End Try
        End Try
        frmMain.Cursor = System.Windows.Forms.Cursors.Default

    End Sub
    Private Sub subMoveTo(ByVal rMnuTag As eMnuFctn(), ByVal nRobot As Integer)
        '********************************************************************************************
        'Description: Handle move requests from the pop-up menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = String.Empty
        Dim nRobots As Integer
        Dim s(1) As String

        Try
            'Special Case for Cover Change (Special1 All) sequencing
            'If rMnuTag(1) = eMnuFctn.Special1 And rMnuTag(2) = eMnuFctn.All Then
            '    Dim sData(0) As String

            '    frmMain.mPLC.TagName = "Z1ToggleCoverChangePosReq"
            '    sData(0) = "1"
            '    frmMain.mPLC.PLCData = sData
            'Else
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "SpecialMoveWd"
            'Set the enum type to match the move numbers
            s(0) = CInt(rMnuTag(1)).ToString
            '1 or all
            Select Case rMnuTag(2)
                Case eMnuFctn.All
                    'All on set a bit for each robot
                    If rMnuTag(1) = eMnuFctn.Purge Then
                        nRobots = 0
                        For nRob As Integer = 1 To frmMain.colArms.Count
                            If (frmMain.colArms.Item(nRob - 1).IsOpener = False) Or (rMnuTag(1) = eMnuFctn.Special1) Then
                                nRobots = nRobots + CInt(2 ^ nRob)
                            End If
                        Next
                    Else   'If (rMnuTag(1) = eMnuFctn.Home) Or (rMnuTag(1) = eMnuFctn.Special2) Then
                        nRobots = 0
                        For nRob As Integer = 1 To frmMain.colArms.Count
                            nRobots = nRobots + CInt(2 ^ nRob)
                        Next
                    End If
                Case eMnuFctn.Allp500
                    'All on set a bit for each robot
                    nRobots = 0
                    For nRob As Integer = 1 To frmMain.colArms.Count
                        If frmMain.colArms.Item(nRob - 1).ArmType = eArmType.P500 Then
                            nRobots = nRobots + CInt(2 ^ nRob)
                        End If
                    Next
                Case Else
                    'individual robot
                    nRobots = CInt(2 ^ nRobot)
            End Select
            s(1) = nRobots.ToString
            'write to the PLC
            frmMain.mPLC.PLCData = s
            'End If

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub

    Private Sub subPLCEnableToggle(ByVal rMnuTag As eMnuFctn(), ByVal nRobot As Integer)
        '********************************************************************************************
        'Description: Handle enable/disable click from pop-up menu
        '   All the enabl/disable words 
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim sTag As String = String.Empty
            Dim nSetVal As Integer
            Dim s() As String
            Dim bOn As Boolean = False
            'Get the function first - app, trig or estat
            Select Case rMnuTag(0)
                Case eMnuFctn.Applicator
                    sTag = "AppEnableToggleRequest"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
                Case eMnuFctn.Trigger
                    sTag = "GunEnableToggleRequest"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
                Case eMnuFctn.Estat
                    sTag = "EstatEnableToggleRequest"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
                Case eMnuFctn.ConvIntlk
                    sTag = "ConvBypassRequest"
                    bOn = (rMnuTag(1) = eMnuFctn.Bypass)
                    If bOn Then
                        Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(gpsRM.GetString("psBYPASS_CONV_INTLK"), _
                                                                                   gpsRM.GetString("psCONV_INTLK"), _
                                                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If lReply = DialogResult.No Then
                            Exit Sub
                        End If
                    End If
                Case eMnuFctn.RelTpAsEnablingDev
                    sTag = "ReleaseTPRequest"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
                Case eMnuFctn.Isolate
                    sTag = "EnableMachineLock"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
                    If bOn Then
                        Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(gpsRM.GetString("psISOLATE_WARN") & vbCrLf & _
                                                                                   vbCrLf & gpsRM.GetString("psISOLATE_CONFIRM"), _
                                                                                   gpsRM.GetString("psISOLATE_ROBOT"), _
                                                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If lReply = DialogResult.No Then
                            Exit Sub
                        End If
                    End If
                Case eMnuFctn.MaintEnableDisable
                    sTag = "MaintModeEnableStates"
                    bOn = (rMnuTag(1) = eMnuFctn.Enable)
            End Select
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & sTag

            If bOn Then
                'On
                '1 or all
                If rMnuTag(2) = eMnuFctn.All Then
                    'All on set a bit for each robot
                    nSetVal = CInt(2 ^ (frmMain.colArms.Count + 1)) - 2
                Else
                    'individual robot.  
                    'New val = Old val OR the bit for selected robot
                    s = frmMain.mPLC.PLCData
                    nSetVal = CInt(s(0)) Or CInt(2 ^ (nRobot))
                End If
            Else
                'Off
                '1 or all
                If rMnuTag(2) = eMnuFctn.All Then
                    'All off - the easy one
                    nSetVal = 0
                Else
                    'individual robot.  
                    'New val = Old val AND NOT( the bit for selected robot)
                    s = frmMain.mPLC.PLCData
                    nSetVal = CInt(s(0)) And Not (CInt(2 ^ (nRobot)))
                End If
            End If
            'write to the PLC
            ReDim s(0)
            s(0) = nSetVal.ToString
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub

    Private Sub subRobotPositionChange(ByRef picR As PictureBox, ByRef lblL As Label, _
                                       ByVal nRStat As Integer, ByVal nRPos As Integer, _
                                       ByVal bIsopener As Boolean, Optional ByVal bMouseOver As Boolean = False, _
                                       Optional ByVal nDevice As Integer = 0)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/20/14  MSW     ZDT Changes
        '********************************************************************************************
        Dim sKey As String = "unknown"
        Dim sString As String = "psUNKNOWN"
        Dim bSelected As Boolean = ((nRStat And eRobotRunStat.Selected) = eRobotRunStat.Selected)
        Dim bAppDisabled As Boolean = ((nRStat And eRobotRunStat.AppDisabled) = eRobotRunStat.AppDisabled)
        Dim bTrigDisabled As Boolean = ((nRStat And eRobotRunStat.GunDisabled) = eRobotRunStat.GunDisabled)
        Dim bBypassed As Boolean = ((nRStat And eRobotRunStat.Bypassed) = eRobotRunStat.Bypassed)
        Dim bFaulted As Boolean = ((nRStat And eRobotRunStat.Faulted) = eRobotRunStat.Faulted)
        Dim bInCyc As Boolean = ((nRStat And eRobotRunStat.InCycle) = eRobotRunStat.InCycle)
        Dim bIsolated As Boolean = ((nRStat And eRobotRunStat.Isolated) = eRobotRunStat.Isolated)
        Dim bRunningOrBypassed As Boolean = ((nRStat And eRobotRunStat.RunningOrBypassed) = eRobotRunStat.RunningOrBypassed)

        If (nRPos And eRobPos.Master) = eRobPos.Master Then
            sKey = "master1"
            sString = "psPOS_MASTER"
        End If

        If Not bIsOpener Then
            If (nRPos And eRobPos.Purge) = eRobPos.Purge Then
                sKey = "purge"
                sString = "psPOS_PURGE"
            End If
            If (nRPos And eRobPos.Cleanin) = eRobPos.Cleanin Then
                sKey = "colchg"
                sString = "psPOS_CLNIN"
            End If
        End If

        If (nRPos And eRobPos.Special2) = eRobPos.Special2 Then
            sKey = "spec2"
            sString = "psPOS_SP2"
        End If

        If (nRPos And eRobPos.Special1) = eRobPos.Special1 Then
            sKey = "spec1"
            sString = "psPOS_SP1"
        End If

        If (nRPos And eRobPos.Home) = eRobPos.Home Then
            sKey = "home"
            sString = "psPOS_HOME"

            If Not bIsOpener Then
                If bTrigDisabled Then sKey = "home_trigoff"
                If bAppDisabled Then sKey = "home_appoff"
            End If
        End If

        If bBypassed Then
            sString = "psBYPASSED"
        End If

        If (nRPos And eRobPos.Bypass) = eRobPos.Bypass Then
            sKey = "bypass"
            sString = "psPOS_BYPASS"
        End If

        If bBypassed Then
            sString = "psBYPASSED"
        End If

        If bInCyc Then
            sKey = "cycle"
            sString = "psPOS_INCYC"

            If Not bIsOpener Then
                If ((nRStat And eRobotRunStat.Triggeron) = eRobotRunStat.Triggeron) Then sKey = "cycle_spray"
                If bTrigDisabled Then sKey = "cycle_trigoff"
                If bAppDisabled Then sKey = "cycle_appoff"
            End If
        End If

        If ((nRStat And eRobotRunStat.CancCont) = eRobotRunStat.CancCont) Then
            sKey = "canc_cont"
            sString = "psPOS_CANCCONT"
        End If

        If ((nRStat And eRobotRunStat.WaitCanc) = eRobotRunStat.WaitCanc) Then
            sKey = "canc_only"
            sString = "psPOS_CANC"
        End If

        If bFaulted And Not (bBypassed And (Not bRunningOrBypassed)) Then
            sKey = "fault"
            sString = "psPOS_FAULTED"
        End If

        'If bBypassed Then
        '    sString = "psBYPASSED"
        '    If (nRPos And eRobPos.Bypass) = eRobPos.Bypass Then
        '        sKey = "bypass"
        '        sString = "psPOS_BYPASS"
        '    End If
        'End If

        If (nRPos And eRobPos.Maint) = eRobPos.Maint Then
            sKey = "maint"
            sString = "psPOS_MAINT"
        End If

        If bIsolated Then sString = "psISOLATED"

        Dim sNormalSelectedMouseover As String

        If bSelected Then
            sNormalSelectedMouseover = "_Selected"
        Else
            sNormalSelectedMouseover = "_Normal"
        End If

        If bMouseOver Then
            sNormalSelectedMouseover = "_Mouseover"
        End If

        ' 03/20/14  MSW     ZDT Changes
        If nDevice > 0 Then
            If frmMain.oOVC IsNot Nothing Then
                Dim nCount As Integer = frmMain.oOVC.OVCCount(nDevice)
                If nCount > 0 Then
                    lblL.ForeColor = Color.Red
                    lblL.Text = String.Format(gpsRM.GetString("psOVC_RESET_DELAY", frmMain.DisplayCulture), nCount)
                Else
                    lblL.ForeColor = Color.Black
        lblL.Text = gpsRM.GetString(sString, frmMain.DisplayCulture)
                End If
            Else
                lblL.ForeColor = Color.Black
                lblL.Text = gpsRM.GetString(sString, frmMain.DisplayCulture)
            End If
        Else
            lblL.Text = gpsRM.GetString(sString, frmMain.DisplayCulture)
        End If

        If picR Is Nothing Then Exit Sub

        Dim o As ImageList = DirectCast(colImages.Item(picR.Tag.ToString & sNormalSelectedMouseover), ImageList)

        'set picture
        picR.Image = o.Images.Item(sKey)

    End Sub

    Private Sub subWritePLCRequest(ByVal rMnuTag As eMnuFctn(), ByVal nRobot As Integer)
        '********************************************************************************************
        'Description: Handle simple requests from the pop-up menus
        '   everything that does a write to a single word, for individual or all robots
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = String.Empty
        Dim nSetVal As Integer
        Dim s() As String
        Try
            'Get the function first - app, trig or estat
            Select Case rMnuTag(0)
                Case eMnuFctn.RecCancel
                    sTag = "CancelReqWd"
                Case eMnuFctn.RecContinue
                    sTag = "ContinueReqWd"
                Case eMnuFctn.SuperPurge
                    sTag = "SuperPurgeRequest"
                Case eMnuFctn.IgnoreOpenerInput
                    sTag = "IgnoreOpenerInputReqWd"
            End Select
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & sTag

            '1 or all
            If rMnuTag(1) = eMnuFctn.All Then
                'All on set a bit for each robot
                nSetVal = CInt(2 ^ (frmMain.colArms.Count + 1)) - 2
            Else
                'individual robot.  
                nSetVal = CInt(2 ^ (nRobot))
            End If
            'write to the PLC
            ReDim s(0)
            s(0) = nSetVal.ToString
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            ShowErrorMessagebox("Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub
    Private Sub subReadBeforeWritePLCRequest(ByVal rMnuTag As eMnuFctn(), ByVal nRobot As Integer)
        '********************************************************************************************
        'Description: Handle simple requests from the pop-up menus
        '   everything that does a write to a single word, for individual or all robots
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = String.Empty
        Dim nSetVal As Integer
        Dim s() As String
        Try
            'Get the function first - app, trig or estat
            Select Case rMnuTag(0)
                Case eMnuFctn.IgnoreOpenerInput
                    sTag = "IgnoreOpenerInputReqWd"
            End Select
            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & sTag

            'individual robot.  
            nSetVal = CInt(2 ^ (nRobot))

            'Read PLC data
            s = frmMain.mPLC.PLCData
            nSetVal = CInt(s(0)) Or (CInt(2 ^ (nRobot)))


            ReDim s(0)
            s(0) = nSetVal.ToString
            'write to the PLC
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            ShowErrorMessagebox("Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub
    Private Function IsItLine2() As Boolean
        '********************************************************************************************
        'Description: Line 1 or Line 2
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String = String.Empty

        Try


            Dim sPrefix As String = "Z" & colZones.CurrentZoneNumber.ToString

            frmMain.mPLC.TagName = sPrefix & "Line1OrLine2"
            Dim sTmp() As String = frmMain.mPLC.PLCData
            IsItLine2 = CBool(sTmp(0))


        Catch ex As Exception
            ShowErrorMessagebox("Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Function

    Friend Sub UpdateQueueData(ByVal rUctl As BSDForm, _
                               ByRef sQueueData As String(), ByRef sQueueRef As String(), _
                               ByVal nPOST_DETECT_Q1_POS_IN_DATA As Integer)
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
        Dim nUB As Integer = UBound(sQueueData)
        Dim bInit As Boolean = False
        'Get a variable to access control properties that aren't inherited from BSDForm
        Dim cUctl As UserControl = DirectCast(rUctl, UserControl)
        Try
            'queue info
            If sQueueRef Is Nothing Then
                bInit = True
            Else
                If UBound(sQueueRef) <> nUB Then bInit = True
            End If
            If bInit Then
                ReDim sQueueRef(nUB)
                For i As Integer = 0 To nUB
                    sQueueRef(i) = (Not CInt(sQueueData(i))).ToString
                Next
            End If

            '1 queue position block of data
            Dim sQData(eQueue.WordsPerPosition - 1) As String
            Dim nOffSet As Integer = 0
            Dim bChangeFound As Boolean = False
            Dim nPosNum As Integer = 1 - nPOST_DETECT_Q1_POS_IN_DATA
            Dim sNum As String = String.Empty
            Dim pnlMainTmp As Panel = DirectCast(cUctl.Controls("pnlMain"), Panel)

            For i As Integer = 0 To nUB
                sQData(nOffSet) = sQueueData(i)
                If sQueueData(i) <> sQueueRef(i) Then bChangeFound = True
                If nOffSet = (eQueue.WordsPerPosition - 1) Then
                    nOffSet = 0
                    If bChangeFound Then
                        Dim pnlTmp As Panel = Nothing
                        Dim uctlTmp As uctlList = Nothing
                        Dim uctlTmp1 As uctlList = Nothing
                        If nPosNum < 0 Then
                            sNum = (nPOST_DETECT_Q1_POS_IN_DATA - (-1 * nPosNum)).ToString("00")
                            '''Debug.Print("sNUM" & sNum)
                            '''Debug.Print("nPosNum" & nPosNum)
                            '''Debug.Print("nPOST_DETECT_Q1_POS_IN_DATA" & nPOST_DETECT_Q1_POS_IN_DATA)
                            uctlTmp = DirectCast(pnlMainTmp.Controls("UctlPreQ" & sNum), uctlList)
                        Else
                            sNum = (nPosNum + 1).ToString("00")
                            '''Debug.Print("sNUM:Else" & sNum)
                            '''Debug.Print("nPosNum:Else" & nPosNum)
                            '''Debug.Print("nPOST_DETECT_Q1_POS_IN_DATA:Else" & nPOST_DETECT_Q1_POS_IN_DATA)
                            uctlTmp = DirectCast(pnlMainTmp.Controls("UctlPostQ" & sNum), uctlList)
                            pnlTmp = DirectCast(pnlMainTmp.Controls("pnlJob" & sNum), Panel)
                        End If
                        If (IsNothing(pnlTmp) = False) Or (IsNothing(uctlTmp) = False) Then
                            mBSDCommon.QueuePositionChange(sQData, sNum, uctlTmp, pnlTmp)
                        End If
                    End If
                    nPosNum += 1
                    bChangeFound = False
                Else
                    nOffSet += 1
                End If

                sQueueRef(i) = sQueueData(i)

            Next 'i

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: mBSDCommon Routine: UpdateQueueData", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub picRx_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
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
        Dim oPic As PictureBox = DirectCast(sender, PictureBox)
        Dim nDeviceNumber As Integer = CType(oPic.Name.Substring(4), Integer)  'the device number after "picR"
        Dim oArm As clsArm = frmMain.colArms.Item(nDeviceNumber, False)

        Call mBSDCommon.EnableRobotMenus(oPic.ContextMenuStrip, frmMain.msZoneData, frmMain.msRobotData, nDeviceNumber, oArm.IsOpener, False, oArm.Name)

        If e.Button = Windows.Forms.MouseButtons.Left Then
            'Manually launch the menu
            Dim oPoint As Point = New Point(e.X, e.Y)

            oPic.ContextMenuStrip.Show(oPic.PointToScreen(oPoint))
        End If

    End Sub

    Private Sub UctlRx_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)


    End Sub

    Private Sub picRx_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: Tracks the mouse for mouseover picture status
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oPic As PictureBox = DirectCast(sender, PictureBox)

        mnMouseOverRobot = CType(oPic.Name.Substring(4), Integer) 'the device number after "picR"
        Call frmMain.gBSDFormCurrentScreen.UpdatePLCData(ePLCLink.Robot)

    End Sub

    Private Sub picRx_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: Tracks the mouse for mouseover picture status
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oPic As PictureBox = DirectCast(sender, PictureBox)
        Dim nDeviceNumber As Integer = CType(oPic.Name.Substring(4), Integer) 'the device number after "picR"

        If mnMouseOverRobot = nDeviceNumber Then
            mnMouseOverRobot = 0
            Call frmMain.gBSDFormCurrentScreen.UpdatePLCData(ePLCLink.Robot)
        End If

    End Sub

    Friend Sub conveyorBingBoard()
        '********************************************************************************************
        'Description: Open Conveyor Bing Board
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 9/24/14   BCD     Control what is displayed on Conveyor Bing Board according to mode.
        '********************************************************************************************
        Dim nBoothIntlk As Integer = CInt(frmMain.msZoneData(eBooth.InterlockWd))
        Dim nMask As Integer = CInt(gnBitVal(eBooth.AutoModeBit) Or gnBitVal(eBooth.ManModeBit) _
                                                    Or gnBitVal(eBooth.TeachModeBit)) 'for select case; references enumeration eBooth
        Dim nMode As Integer = nMask And nBoothIntlk
        frmDevices.PLCDataStart = 0
        frmDevices.ZoneItems = 11
        frmDevices.RobotItems = 10
        Dim bHide(16) As Boolean

        Select Case nMode

            Case gnBitVal(eBooth.TeachModeBit)
                'set everything in the array to false
                'making everything show up on the bingo board
                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                bHide(0) = True
                bHide(1) = True
                bHide(2) = True
                'bHide(3) = True
                bHide(4) = True
                bHide(5) = True
                bHide(6) = True
                bHide(7) = True
                bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                bHide(12) = True
                bHide(13) = True
                bHide(14) = True
                frmDevices.HideZoneItems = bHide

                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                'bHide(0) = True
                'bHide(1) = True
                'bHide(2) = True
                'bHide(3) = True
                'bHide(4) = True
                'bHide(5) = True
                'bHide(6) = True
                'bHide(7) = True
                'bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                'bHide(12) = True
                'bHide(13) = True
                'bHide(14) = True
                frmDevices.HideRobotItems = bHide

            Case gnBitVal(eBooth.ManModeBit)
                'set everything in the array to false
                'making everything show up on the bingo board
                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                'bHide(0) = True
                'bHide(1) = True
                'bHide(2) = True
                'bHide(3) = True
                'bHide(4) = True
                'bHide(5) = True
                'bHide(6) = True
                'bHide(7) = True
                'bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                'bHide(12) = True
                'bHide(13) = True
                'bHide(14) = True
                frmDevices.HideZoneItems = bHide

                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                'bHide(0) = True
                'bHide(1) = True
                'bHide(2) = True
                'bHide(3) = True
                'bHide(4) = True
                'bHide(5) = True
                'bHide(6) = True
                'bHide(7) = True
                'bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                'bHide(12) = True
                'bHide(13) = True
                'bHide(14) = True
                frmDevices.HideRobotItems = bHide

            Case gnBitVal(eBooth.AutoModeBit)
                'set everything in the array to false
                'making everything show up on the bingo board
                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                'bHide(0) = True
                'bHide(1) = True
                'bHide(2) = True
                'bHide(3) = True
                'bHide(4) = True
                'bHide(5) = True
                'bHide(6) = True
                'bHide(7) = True
                'bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                'bHide(12) = True
                'bHide(13) = True
                'bHide(14) = True
                frmDevices.HideZoneItems = bHide

                For nItem As Integer = 0 To 15
                    bHide(nItem) = False
                Next
                'uncomment anything you do not want to show up on the bingo board
                'bHide(0) = True
                'bHide(1) = True
                'bHide(2) = True
                'bHide(3) = True
                'bHide(4) = True
                'bHide(5) = True
                'bHide(6) = True
                'bHide(7) = True
                'bHide(8) = True
                'bHide(9) = True
                'bHide(10) = True
                'bHide(11) = True
                'bHide(12) = True
                'bHide(13) = True
                'bHide(14) = True
                frmDevices.HideRobotItems = bHide

        End Select
    End Sub
#End Region

End Module
