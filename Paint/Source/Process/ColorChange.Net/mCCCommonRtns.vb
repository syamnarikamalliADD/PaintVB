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
' Form/Module: mCCCommonRtns
'
' Description: Routines for Color Change
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'   Date        By      Reason                                                        Version
'   06/02/09    MSW     1st PW4 version
'   01/01/13    RJO     Added (unused) Style param to LoadCopyScreenSubParamBox for 
'                       compatibility with frmCopy v4.01.01.03
'   01/12/13    MSW     Fix loop range for group valve changelog
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel


Module mCCCommonRtns

#Region " Declares "

    Friend Const gsVALVE_COL As String = "Valve"
    Friend gStyleNameArray() As String
    Private moSWriter As StreamWriter = Nothing
    Private Const sLOGFILE As String = "C:\Temp\CCcopy.log"
    Delegate Sub UpdateStatus(ByVal progress As Integer, ByVal sStatus As String)
    Private mViewAllRobot As clsArm = Nothing
#End Region

#Region " Robot Communication Routines "
    Friend Sub subScreenDump(ByRef oForm As Form, ByVal sSubScreen As String)
        '********************************************************************************************
        'Description: screen capture request from copy or multiview
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        'Screen Dump request
        Dim oSC As New ScreenShot.ScreenCapture
        Dim sDumpPath As String = String.Empty

        If sSubScreen <> String.Empty Then
            sSubScreen = "_" & sSubScreen
        End If
        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)

        oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)

    End Sub

    Friend Sub subShowHelp(ByRef sSubScreen As String)
        '********************************************************************************************
        'Description: Help Screen request from copy or multiview
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Help Screen request
        Select Case sSubScreen
            Case "Copy"
                subLaunchHelp(gs_HELP_COLORCHANGE_COPY, frmMain.oIPC)
            Case "MultiView"
                subLaunchHelp(gs_HELP_COLORCHANGE_MV, frmMain.oIPC)
            Case Else
                subLaunchHelp(gs_HELP_COLORCHANGE, frmMain.oIPC)
        End Select
    End Sub
    Private Function bGetColorChange(ByRef oRobot As clsArm, ByRef oColorChange As clsColorChange, _
                                    ByVal sFanucNum As Integer, ByRef oCopy As frmCopy, _
                                    ByVal bSourceCC As Boolean, Optional ByRef sCycle As String = "") As Boolean
        '********************************************************************************************
        'Description:  get the preset info for robot passed in
        '
        'Parameters: 
        'Returns:    True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        Try


            'Dim dsDesc As DataSet = New DataSet

            If sCycle <> String.Empty Then
                sTmp = gpsRM.GetString("psLOADING_CYCLE") & " " & sCycle
            Else
                If bSourceCC Then
                    sTmp = gpsRM.GetString("psLOADING_SOURCE")
                Else
                    sTmp = gpsRM.GetString("psLOADING_TARGET")
                End If
            End If

            oCopy.Status = sTmp
            If oRobot.IsOnLine Then
                If LoadColorChangeFromRobot(oRobot, oColorChange, , , , sCycle) Then

                    If frmMain.bLoadFromGUI(oRobot.Controller.Zone, oRobot, oColorChange) = False Then
                        'something failed bomb out... need continue to next here somehow
                        Return False
                    End If
                Else
                    'something failed bomb out... need continue to next here somehow
                    Return False
                End If
            Else
                Return False
            End If


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Private Function bGetTargetArms(ByRef oZones As clsZones, ByRef oTargetCollection As clsArms, _
                                    ByRef oRefCollection As clsArms) As Boolean
        '********************************************************************************************
        'Description:  get the arms for the passed in zone, and select based on input collection
        '
        'Parameters: 
        'Returns:    True if copy success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim colControllersTo As New clsControllers(oZones, False)
            oTargetCollection = LoadArmCollection(colControllersTo, False)
            'could be different names, different number of arms etc - just blindly using index
            For i As Integer = 0 To oRefCollection.Count - 1
                If i > oTargetCollection.Count - 1 Then Exit For
                If oRefCollection.Item(i).Selected Then oTargetCollection.Item(i).Selected = True
            Next

            'make sure system colors are loaded in target collection as these are new clsarms
            For i As Integer = 0 To oTargetCollection.Count - 1
                If oTargetCollection.Item(i).Selected Then
                    ' if i ever catch the guy who wrote this @#!@$...
                    oTargetCollection.Item(i).SystemColors.Load(oTargetCollection.Item(i))
                End If
            Next

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function DoCopy(ByRef colZoneFrom As clsZones, ByRef colZoneTo As clsZones, _
                                ByRef colRobotFrom As clsArms, ByRef colRobotTo As clsArms, _
                                ByRef colParamFrom As Collection(Of String), _
                                ByRef colParamTo As Collection(Of String), _
                                ByRef colSubParamFrom As Collection(Of String), _
                                ByRef colSubParamTo As Collection(Of String), _
                                ByVal CopyType As eCopyType, ByRef oCopy As frmCopy, _
                                Optional ByRef colStyleFrom As Collection(Of String) = Nothing, _
                                Optional ByRef colStyleTo As Collection(Of String) = Nothing) As Boolean
        '********************************************************************************************
        'Description:  Copy Data Selected from copy screen
        '
        'Parameters: collections of item selected
        'Returns:    True if copy success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/21/07  gks     This was originally set up to use the display name to locate the colors
        '                   to try to get away from the index thing. worked fine until the paint shop
        '                   computer tried to copy across mods where system colors had different 
        '                   display names - now trying fanuc number
        ' 11/18/09  MSW     -Don't need to convert target valve to color number, it's already done
        ' 03/30/11  MSW     Speed up copy of all cycles by skipping the load from the target
        ' 06/27/11  MSW     Enable pushout by volume based on source when we don't load the target
        ' 12/22/11  MSW     Updates for speed
        '********************************************************************************************
        Dim oSourceRobot As clsArm
        Dim sSourceValveNum As String
        Dim oTargetRobot As clsArm
        Dim sTargetValveNum As String
        Dim colTargetArms As clsArms = Nothing
        Dim sSrcRobot As String
        Dim sTgtRobot As String

        oCopy.Status = gcsRM.GetString("csCOPY_STARTED")

        Try



            '  get target zone data
            For Each oZone As clsZone In colZoneTo
                If oZone.Selected Then
                    colZoneTo.CurrentZone = oZone.Name
                    ' get arms from selected zone(s) 1 zone at a time
                    bGetTargetArms(colZoneTo, colTargetArms, colRobotTo)

                    'the source is coming from a single zone
                    For Each oSourceRobot In colRobotFrom


                        If oSourceRobot.Selected Then
                            If oSourceRobot.Applicator Is Nothing Then
                                oSourceRobot.Applicator = frmMain.colApplicators(oSourceRobot.ColorChangeType)
                            End If
                            For Each sSourceValveNum In colParamFrom

                                'Source color change data
                                Dim oSourceColorChange As New clsColorChange(oSourceRobot, True)
                                'turn the valve number into fanuc color and assign the sys color setup
                                Dim nSourceFanuColor As Integer = CInt(sSourceValveNum)
                                oSourceColorChange = oSourceRobot.SystemColors(nSourceFanuColor, True).Valve.ColorChange
                                oSourceColorChange.ValveDescription = oSourceRobot.SystemColors(nSourceFanuColor, True).Valve.DisplayName
                                oSourceColorChange.Valve = oSourceRobot.SystemColors(nSourceFanuColor, True).Valve.Number.Value
                                '
                                Debug.Print("Src - Valve : " & sSourceValveNum & ", FANUC Color : " & nSourceFanuColor & ", : " & oSourceColorChange.ValveDescription)
                                sSrcRobot = oSourceRobot.Name
                                oCopy.Status = oSourceRobot.Name & ", " & gcsRM.GetString("csCOLOR") & _
                                       " " & oSourceColorChange.ValveDescription & " " & gcsRM.GetString("csSELECTED_AS_SOURCE")

                                'Load CC data cycles
                                If oSourceColorChange.NumberOfCycles > colSubParamFrom.Count Then
                                    For Each sSrcCycle As String In colSubParamFrom
                                        If bGetColorChange(oSourceRobot, oSourceColorChange, CInt(sSourceValveNum), _
                                                    oCopy, True, sSrcCycle) Then
                                            'it worked
                                        Else
                                            'something failed bomb out... need continue to next here somehow
                                            Return False
                                        End If
                                    Next
                                Else
                                    'Load all together
                                    If bGetColorChange(oSourceRobot, oSourceColorChange, CInt(sSourceValveNum), _
                                                oCopy, True, String.Empty) Then
                                        'it worked
                                    Else
                                        'something failed bomb out... need continue to next here somehow
                                        Return False
                                    End If
                                End If
                                For Each oTargetRobot In colTargetArms
                                    ' if there is more than 1 source robot - set equal to source robot index
                                    ' and bail at bottom of for loop. Other wise loop thru all selected
                                    If colRobotFrom.SelectedCount > 1 Then
                                        ' robot index starts at 1 - should be selected from bGetTargetArms
                                        oTargetRobot = colTargetArms(oSourceRobot.RobotNumber - 1)
                                    End If

                                    If oTargetRobot.Selected Then
                                        ' loop thru colors - parameter collections are only loaded with parameters that
                                        ' are selected

                                        If oTargetRobot.Applicator Is Nothing Then
                                            oTargetRobot.Applicator = frmMain.colApplicators(oSourceRobot.ColorChangeType)
                                        End If
                                        For Each sTargetValveNum In colParamTo
                                            ' if there is more than 1 source param - set equal to source param index
                                            ' and bail at bottom of for loop. Other wise loop thru all 
                                            If colParamFrom.Count > 1 Then
                                                ' align on index
                                                sTargetValveNum = sSourceValveNum
                                            End If  'colParamFrom.Count > 1

                                            'get existing target data

                                            Dim oTargetColorChange As New clsColorChange(oTargetRobot, True)
                                            'turn the valve number into fanuc color and assign the sys color setup
                                            'It comes in as a system color number
                                            'Dim nTargetFanuColor As Integer = oTargetRobot.SystemColors.FanucNumberFromValveNumber(CInt(sTargetValveNum))
                                            Dim nTargetFanuColor As Integer = CInt(sTargetValveNum)
                                            oTargetColorChange = oTargetRobot.SystemColors(nTargetFanuColor, True).Valve.ColorChange
                                            oTargetColorChange.ValveDescription = oTargetRobot.SystemColors(nTargetFanuColor, True).Valve.DisplayName
                                            oTargetColorChange.Valve = oTargetRobot.SystemColors(nTargetFanuColor, True).Valve.Number.Value
                                            '
                                            oCopy.Status = oTargetRobot.Name & ", " & gcsRM.GetString("csCOLOR") & _
                                                   " " & oTargetColorChange.ValveDescription & " " & gcsRM.GetString("csSELECTED_AS_TARGET")

                                            Debug.Print("Tgt - Valve : " & sTargetValveNum & ", FANUC Color : " & nTargetFanuColor & ", : " & oTargetColorChange.ValveDescription)
                                            sTgtRobot = oTargetRobot.Name
                                            'Speed up copy of all cycles by skipping the load from the target
                                            'If oTargetColorChange.NumberOfCycles > colSubParamFrom.Count Then
                                            '    'Load CC data cycles
                                            '    'For nCycle As Integer = 1 To oTargetColorChange.NumberOfCycles
                                            '    '    Debug.Print(oTargetColorChange.Cycle(nCycle).CycleName & oTargetColorChange.Cycle(nCycle).NumberOfSteps.ToString)
                                            '    'Next
                                            '    For Each sTgtCycle As String In colSubParamFrom
                                            '        'It's not a typo, it's using the "From" list for cycle names because it should always be the same 
                                            '        If bGetColorChange(oTargetRobot, oTargetColorChange, CInt(sTargetValveNum), _
                                            '                    oCopy, False, sTgtCycle) Then
                                            '            'it worked
                                            '        Else
                                            '            'something failed bomb out... need continue to next here somehow
                                            '            Return False
                                            '        End If
                                            '    Next
                                            '    'For nCycle As Integer = 1 To oTargetColorChange.NumberOfCycles
                                            '    '    Debug.Print(oTargetColorChange.Cycle(nCycle).CycleName & oTargetColorChange.Cycle(nCycle).NumberOfSteps.ToString)
                                            '    'Next
                                            'Else
                                            'Enable pushout by volume based on source when we don't laod the target
                                            oTargetColorChange.ShowPushoutVolume(0) = oSourceColorChange.ShowPushoutVolume(0)

                                            'End If
                                            ' have all data to do copy
                                            oCopy.Status = gcsRM.GetString("csWRITING_TARGET")
                                            If bActuallyDoTheCopy(oSourceRobot, oTargetRobot, _
                                                                oSourceColorChange, oTargetColorChange, _
                                                                colSubParamFrom, colSubParamTo, CopyType, oCopy) = False Then

                                                'something failed bomb out... need continue to next here somehow
                                                oCopy.Status = gcsRM.GetString("csWRITE_FAIL")


                                                Return False

                                            Else
                                                oCopy.Status = oTargetRobot.Name & ", " & gcsRM.GetString("csCOLOR") & _
                                                       " " & oTargetColorChange.ValveDescription & " " & gcsRM.GetString("csWRITE_SUCCESS")
                                                oCopy.Status = String.Empty
                                            End If


                                            ' multiple from colors - just do 1 color
                                            If colParamFrom.Count > 1 Then Exit For
                                        Next 'sTargetValveNum
                                    End If  'oTargetRobot.Selected

                                    'multiple from selections - just do 1 robot
                                    If colRobotFrom.SelectedCount > 1 Then Exit For

                                Next 'oTargetRobot

                            Next ' sSourceValveNum
                        End If ' oSourceRobot.Selected
                    Next 'oSourceRobot In colRobotFrom
                End If 'oZone.Selected
                'changelog
                'mPWCommon.SaveToChangeLog(rTargetRobot.DatabasePath)
                'For SQL database - remove above eventually
                mPWCommon.SaveToChangeLog(oZone)

            Next 'oZone


        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            mDebug.WriteEventToLog(ex.Message, frmMain.msSCREEN_NAME)


            Return False

        End Try

        Return True

    End Function
    Private Function bActuallyDoTheCopy(ByRef rSourceRobot As clsArm, ByRef rTargetRobot As clsArm, _
                    ByRef rSourceColorChange As clsColorChange, ByRef rTargetColorChange As clsColorChange, _
                    ByRef rSourceColorChangeToCopy As Collection(Of String), _
                    ByRef rTargetColorChangeToCopy As Collection(Of String), _
                    ByVal CopyType As eCopyType, ByRef oCopy As frmCopy) As Boolean
        '********************************************************************************************
        'Description:  Copy the data we just loaded
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason      
        '5/8/07     gks     Copy by param 
        ' 12/22/11  MSW     Updates for speed
        '********************************************************************************************
        Dim nStep As Integer
        Dim nCycleNum As Integer
        rTargetColorChange.PushOutVolume = rSourceColorChange.PushOutVolume
        Dim bAll As Boolean = (rTargetColorChange.NumberOfCycles = rTargetColorChangeToCopy.Count)
        If rTargetRobot.IsOnLine Then
            For nCycleNum = 1 To rSourceColorChange.NumberOfCycles
                Debug.Print(rSourceColorChange.Cycle(nCycleNum).CycleName)
                If rSourceColorChangeToCopy.Contains(rSourceColorChange.Cycle(nCycleNum).CycleName) Then
                    'Do the copy on this cycle

                    'Dim dsDesc As DataSet = New DataSet

                    oCopy.Status = gpsRM.GetString("psSAVING_CYCLE") & " " & nCycleNum.ToString & " - " & rSourceColorChange.Cycle(nCycleNum).CycleName & _
                        " " & gcsRM.GetString("csTO") & " " & rTargetRobot.Name & ", " & gcsRM.GetString("csCOLOR") & " " & rTargetColorChange.ValveDescription

                    'set desired values
                    rTargetColorChange.Cycle(nCycleNum).NumberOfSteps = rSourceColorChange.Cycle(nCycleNum).NumberOfSteps
                    For nStep = 1 To rSourceColorChange.Cycle(nCycleNum).NumberOfSteps
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).Duration = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).Duration
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).DoutDC = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).DoutDC
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).DoutState = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).DoutState
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).GoutDC = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).GoutDC
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).GoutState = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).GoutState
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).Preset = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).Preset
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).StepAction = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).StepAction
                        rTargetColorChange.Cycle(nCycleNum).Steps(nStep).StepEvent = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).StepEvent
                    Next
                    'reset the change status so it doesn't log a change for each bit
                    rTargetColorChange.Cycle(nCycleNum).Update()
                    If bAll = False Then
                        If SaveColorChangeToRobot(rTargetRobot, rTargetColorChange, , , , , True, rTargetColorChange.Cycle(nCycleNum).CycleName) Then
                            UpdateChangeLogCopy(rTargetColorChange.Cycle(nCycleNum).CycleName, rSourceRobot.Name, rTargetRobot.Name, rSourceColorChange.ValveDescription, _
                                                    rTargetColorChange.ValveDescription, _
                                                    rTargetRobot.ZoneNumber, rTargetRobot.Controller.Zone.Name)
                        Else
                            Return False
                        End If
                    End If
                End If
            Next

            If bAll Then
                If SaveColorChangeToRobot(rTargetRobot, rTargetColorChange, , , , , True, String.Empty) Then
                    UpdateChangeLogCopy(gpsRM.GetString("psALL_CYCLES"), rSourceRobot.Name, rTargetRobot.Name, rSourceColorChange.ValveDescription, _
                                            rTargetColorChange.ValveDescription, _
                                            rTargetRobot.ZoneNumber, rTargetRobot.Controller.Zone.Name)
                Else
                    Return False
                End If
            End If

            Return True
        Else
            Return False
        End If


    End Function
    Friend Function GetViewAllRobot(ByRef rArm As clsArm, ByVal ZoneName As String, _
                        ByVal RobotName As String, ByVal DatabasePath As String) As Boolean
        '********************************************************************************************
        'Description:  get a robot for the viewall screen
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/28/09  RJO     Modified call to LoadArmCollection to not include openers
        '********************************************************************************************
        Try
            Dim oZones As New clsZones(DatabasePath)
            oZones.CurrentZone = ZoneName

            Dim oControllers As clsControllers = New clsControllers(oZones, False)
            Dim oArms As clsArms = LoadArmCollection(oControllers, False)
            rArm = oArms.Item(RobotName)
            mViewAllRobot = rArm
            mViewAllRobot.Applicator = frmMain.colApplicators(mViewAllRobot.ColorChangeType)
            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            mDebug.WriteEventToLog(ex.Message, frmMain.msSCREEN_NAME)
            Return False

        End Try

    End Function
    Friend Function GetViewAllData(ByVal ZoneName As String, ByVal RobotName As String, _
                                    ByVal rCboParamName As ComboBox, ByVal rCboSubParamName As ComboBox, _
                                    ByVal DatabasePath As String) As DataSet
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: Zone name, Robot name, valve description, cycle name, DB path, arm
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Pass a robot object into the cc class so it can get the valves
        ' 10/12/10  MSW     switcvh to byvalve calls for selecting a system color
        '********************************************************************************************
        Dim ParamName As String = rCboParamName.Text
        Dim SubParamName As String = rCboSubParamName.Text
        Dim oColorChange As clsColorChange = mViewAllRobot.SystemColors.ByValve(ParamName).Valve.ColorChange
        oColorChange.Valve = mViewAllRobot.SystemColors.ByValve(ParamName).Valve.Number.Value
        oColorChange.ValveDescription = ParamName
        Dim lCCtoon As clsColorChangeCartoon
        Dim oCCToon As UserControl = Nothing
        Select Case mViewAllRobot.ColorChangeType
            Case eColorChangeType.ACCUSTAT

                lCCtoon = New clsGunCartoon
            Case eColorChangeType.SINGLE_PURGE
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsGunCartoon
            Case eColorChangeType.VERSABELL2
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsVersabellCartoon
            Case eColorChangeType.VERSABELL2_2K
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsVersabell2_2kCartoon
            Case eColorChangeType.VERSABELL2_PLUS
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsVersabell2plusCartoon
            Case eColorChangeType.VERSABELL2_PLUS_WB
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsVersabell2plusWBCartoon
            Case eColorChangeType.HONDA_1K
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsHonda1KCartoon
            Case eColorChangeType.HONDA_WB
                'And the app specific class from ColorChange.vb
                lCCtoon = New clsHondaWBCartoon
            Case Else
                lCCtoon = New clsGunCartoon
        End Select
        lCCtoon.Initialize(oCCToon, DatabasePath, mViewAllRobot, frmMain.mApplicator)
        If mViewAllRobot.IsOnLine Then
            If LoadColorChangeFromRobot(mViewAllRobot, oColorChange, , , , SubParamName) Then
                If frmMain.bLoadFromGUI(mViewAllRobot.Controller.Zone, mViewAllRobot, oColorChange) Then
                    Return CreateColorChangeDataset(False, oColorChange, mViewAllRobot, lCCtoon, SubParamName)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            MessageBox.Show(gcsRM.GetString("csCOULD_NOT_CONNECT"), _
                    mViewAllRobot.Name, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return Nothing
        End If

    End Function
    Friend Function LoadPresetsFromRobot(ByRef rRobot As clsArm, _
                    ByRef rPresets As clsPresets, _
                    Optional ByRef ParmDetails() As tParmDetail = Nothing, Optional ByVal nNumParms As Integer = -1) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/04/09  MSW     copied from presets program for CC screen, cut out some extras
        ' 01/06/12  MSW     Make better use of FRCVars.NoRefresh for speed improvements
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim nNumPresets As Integer = 40
        If ParmDetails Is Nothing Or nNumParms = -1 Then
            'This is by robot, don't need to reload it if the main screen keeps a copy
            Dim ParmSelect As mPWRobotCommon.eParmSelect = eParmSelect.CCPresets
            subInitParmConfig(rRobot, ParmSelect, ParmDetails, nNumParms, frmMain.mApplicator)
        End If
        rPresets.NumParms = nNumParms
        rRobot.ProgramName = "PAPSCCHG"
        Dim o As FRRobot.FRCVars = rRobot.ProgramVars
        o.NoRefresh = True 'Read the whole file from the robot
        'Dim oo As FRRobot.FRCVars = DirectCast(o.Item("Preset_Data"), FRRobot.FRCVars)
        'Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
        Dim oo As FRRobot.FRCVars = DirectCast(o.Item(, 0), FRRobot.FRCVars)
        Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item(, 0), FRRobot.FRCVars)
        Dim oooo As FRRobot.FRCVars = DirectCast(ooo.Item(, 0), FRRobot.FRCVars)
        Dim oPresets As FRRobot.FRCVars = DirectCast(oooo.Item("PRESETS"), FRRobot.FRCVars)
        'oPresets.NoRefresh = True 'Read the whole file from the robot
        Dim nMaxParm As Integer = nNumParms - 1
        'get each preset column
        Dim oPresetData(nMaxParm) As FRRobot.FRCVars
        For nParm As Integer = 0 To nMaxParm
            rPresets.ParameterName(nParm) = ParmDetails(nParm).sLblName
            rPresets.ScreenParm(nParm) = ParmDetails(nParm).nParmNum
            oPresetData(nParm) = DirectCast(oPresets(rPresets.ScreenParm(nParm).ToString), FRRobot.FRCVars)
        Next

        If rRobot.Controller.Version >= 7.5 Then
            '[PAVRSYSC] num_presets	(Number of presets per color)
            rRobot.ProgramName = "PAVRSYSC"
            rRobot.VariableName = "num_presets"
            nNumPresets = CInt(rRobot.VarValue)
        Else
            'For older versions, use the data structure size to determine the number of presets
            nNumPresets = oPresetData(0).Count
        End If

        For nPreset As Integer = 0 To nNumPresets - 1

            Dim oP As New clsPreset
            With oP
                .Index = nPreset + 1
                For nParm As Integer = 0 To nMaxParm
                    oVar = DirectCast(oPresetData(nParm)(, nPreset), FRRobot.FRCVar)
                    .Param(nParm).Value = CType(oVar.Value, Single)
                Next
            End With
            rPresets.Add(oP)
        Next
        o.NoRefresh = False

        Return True

    End Function

    Friend Function LoadColorChangeFromRobot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange, _
                                             Optional ByRef op As UpdateStatus = Nothing, _
                                             Optional ByVal ProgressMin As Integer = 0, _
                                                Optional ByVal ProgressMax As Integer = 100, _
                                                Optional ByRef sCycleName As String = "") As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/22/11  MSW     Updates for speed
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim sVar As String
        Dim nStep As Integer
        ' no error trap here - let calling routine catch it
        Dim sProgName As String = String.Empty
        'Pushout by volume.  Get it done first, it's in a different file
        If rRobot.Controller.Version >= 7.3 Then
            rColorChange.ShowPushoutVolume = True
            Try
                'Get pushout volume
                rRobot.ProgramName = "PAVRDBCV"

                'COLOR_DATA{eq#>1}.NODEDATA[valve#].HDWE.PUSH_VOL
                sVar = "COLOR_DATA"
                If rRobot.ArmNumber > 1 Then
                    sVar = sVar & rRobot.ArmNumber.ToString
                End If
                sVar = sVar & ".NODEDATA[" & rColorChange.Valve.ToString & "].HDWE.PUSH_VOL"
                rRobot.VariableName = sVar
                rColorChange.PushOutVolume.Value = CSng(rRobot.VarValue)
                Debug.Print(rColorChange.PushOutVolume.Value.ToString)
            Catch ex As Exception
                ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, frmMain.msSCREEN_NAME, _
                                    frmMain.Status, MessageBoxButtons.OK)
            End Try
        Else
            rColorChange.ShowPushoutVolume = False
        End If
        ''select the file based on version, valve, and arm
        If rRobot.Controller.Version >= 7.5 Then
            '7.5 goes to 3 digit valve number - still format for 2 digits for valve 1-99
            Select Case rRobot.ArmNumber
                Case 1
                    sProgName = "CHGCL" & rColorChange.Valve.ToString("00")
                Case Else
                    sProgName = "CH" & rRobot.ArmNumber.ToString & "CL" & rColorChange.Valve.ToString("00")
            End Select
        Else
            Select Case rRobot.ArmNumber
                Case 1
                    sProgName = "CHGCOL" & rColorChange.Valve.ToString("00")
                Case Else
                    sProgName = "CH" & rRobot.ArmNumber.ToString & "COL" & rColorChange.Valve.ToString("00")
            End Select
        End If

        'Get the program object for now, declare the cycle data vars for the loop
        rRobot.ProgramName = sProgName
        Dim oProgVars As FRRobot.FRCVars = rRobot.ProgramVars
        'Cycle count nvaries by option, but the file only has cycle header and step vars, so we'll use that
        Dim nCycleCount As Integer = CInt(oProgVars.Count / 2)
        'Take the number of cycles for the minimum data available
        If nCycleCount < rColorChange.NumberOfCycles Then
            rColorChange.NumberOfCycles = nCycleCount
        End If
        'Reset this here, it gets changed by .NumberOfCycles
        rRobot.ProgramName = sProgName
        Dim oCycleData As FRRobot.FRCVars
        Dim oStep As FRRobot.FRCVars
        Dim oHeaderVar As FRRobot.FRCVars
        Dim bAll As Boolean = (sCycleName = String.Empty)
        If bAll Then
            oProgVars.NoRefresh = True 'Read the whole file from the robot
        End If
        For nCycle As Integer = 1 To rColorChange.NumberOfCycles

            With rColorChange.Cycle(nCycle)
                If bAll OrElse (sCycleName = rColorChange.Cycle(nCycle).CycleName) Then
                    If Not (op Is Nothing) Then
                        'update status
                        '    rForm.progress = ProgressMin + nCycle * (ProgressMax - ProgressMin) \ nCycleCount
                        op.Invoke(ProgressMin + nCycle * (ProgressMax - ProgressMin) \ nCycleCount, _
                                  (gpsRM.GetString("psLOADING_CYCLE") & nCycle.ToString))
                        frmMain.Refresh()
                    End If
                    'Number of steps
                    sVar = "C_CHG_HED_" & nCycle.ToString("00")
                    oHeaderVar = DirectCast(oProgVars.Item(sVar), FRRobot.FRCVars)
                    If bAll = False Then
                        oHeaderVar.NoRefresh = True
                    End If
                    oVar = DirectCast(oHeaderVar.Item("CYCLE_STEPS"), FRRobot.FRCVar)
                    If (sCycleName <> String.Empty) Then
                        oProgVars.NoRefresh = True 'Read the whole file from the robot
                    End If
                    'Check uninit
                    If oVar.IsInitialized Then
                        .NumberOfSteps = CInt(oVar.Value)
                    Else
                        sVar = "C_CHG_HED_" & nCycle.ToString("00") & ".CYCLE_STEPS"
                        rRobot.VariableName = sVar
                        oVar.Value = 1
                        oVar.Update()
                        .NumberOfSteps = 1
                    End If
                    sVar = "C_CHG_CYC_" & nCycle.ToString("00")
                    oCycleData = DirectCast(oProgVars.Item(sVar), FRRobot.FRCVars)
                    If bAll = False Then
                        oCycleData.NoRefresh = True
                    End If
                    For nStep = 1 To .NumberOfSteps
                        'assing a variable to the step structure
                        oStep = DirectCast(oCycleData.Item(nStep), FRRobot.FRCVars)

                        'Event
                        oVar = DirectCast(oStep.Item("STEP_EVENT"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).StepEvent.Value = CInt(oVar.Value)

                        'oVar = DirectCast(oStep.Item("EVENT_TYPE"), FRRobot.FRCVar)

                        'GOUT state
                        oVar = DirectCast(oStep.Item("G_OUT_STATE"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        End If
                        .Steps(nStep).GoutState.Value = CInt(oVar.Value)

                        'GOUT don't change
                        oVar = DirectCast(oStep.Item("G_OUT_DC"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        End If
                        .Steps(nStep).GoutDC.Value = CInt(oVar.Value)

                        'Shared state
                        oVar = DirectCast(oStep.Item("D_OUT_STATE"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).DoutState.Value = CInt(oVar.Value)

                        'Shared don't change
                        oVar = DirectCast(oStep.Item("D_OUT_DC"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).DoutDC.Value = CInt(oVar.Value)

                        'Action
                        oVar = DirectCast(oStep.Item("STEP_ACTION"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).StepAction.Value = CInt(oVar.Value)

                        'Preset
                        oVar = DirectCast(oStep.Item("PRESET_SEL"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).Preset.Value = CInt(oVar.Value)

                        'time
                        oVar = DirectCast(oStep.Item("DUR_TIME"), FRRobot.FRCVar)
                        If Not (oVar.IsInitialized) Then
                            oVar.Value = 0
                        ElseIf CInt(oVar.Value) = 255 Then 'Uninit byte
                            oVar.Value = 0
                        End If
                        .Steps(nStep).Duration.Value = (CSng(oVar.Value) / 10)

                    Next
                End If
            End With
        Next
        rColorChange.Update()
        Return True

    End Function
    Friend Function ValveChanged(ByVal nValve As Integer, ByVal nOldDC As Integer, ByVal nNewDC As Integer, _
                                 ByVal nOldState As Integer, ByVal nNewState As Integer, _
                                 ByRef sOld As String, ByRef sNew As String) As Boolean
        Dim nBitMask As Integer = CType(2 ^ (nValve - 1), Integer)
        Dim nOldValveState As Integer
        If ((nOldState And nBitMask) > 0) Then
            'select each valve that's on in the step data
            nOldValveState = CheckState.Checked
        ElseIf ((nOldDC And nBitMask) > 0) Then
            nOldValveState = CheckState.Indeterminate
        Else
            nOldValveState = CheckState.Unchecked
        End If
        Dim nNewValveState As Integer
        If ((nNewState And nBitMask) > 0) Then
            'select each valve that's on in the step data
            nNewValveState = CheckState.Checked
        ElseIf ((nNewDC And nBitMask) > 0) Then
            nNewValveState = CheckState.Indeterminate
        Else
            nNewValveState = CheckState.Unchecked
        End If
        If nNewValveState <> nOldValveState Then
            Select Case nNewValveState
                Case CheckState.Checked
                    sNew = gpsRM.GetString("psON")
                Case CheckState.Unchecked
                    sNew = gpsRM.GetString("psOFF")
                Case CheckState.Indeterminate
                    sNew = gpsRM.GetString("psDC")
            End Select
            Select Case nOldValveState
                Case CheckState.Checked
                    sOld = gpsRM.GetString("psON")
                Case CheckState.Unchecked
                    sOld = gpsRM.GetString("psOFF")
                Case CheckState.Indeterminate
                    sOld = gpsRM.GetString("psDC")
            End Select
            Return True
        Else
            Return False
        End If
    End Function
    Friend Function SaveColorChangeToRobot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange, _
                                                Optional ByRef op As UpdateStatus = Nothing, _
                                                Optional ByVal ProgressMin As Integer = 0, _
                                                Optional ByVal ProgressMax As Integer = 100, _
                                                Optional ByRef rCCToon As clsColorChangeCartoon = Nothing, _
                                                Optional ByVal bSaveAll As Boolean = False, _
                                                Optional ByRef sCycleName As String = "") As Boolean
        '********************************************************************************************
        'Description:  Save data to Robot
        '
        'Parameters: none
        'Returns:    True if save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/22/11  MSW     Updates for speed
        ' 01/12/13  MSW     Fix loop range for group valve changelog
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim sVar As String
        Dim nStep As Integer
        ' no error trap here - let calling routine catch it
        Dim sProgName As String = String.Empty
        Dim nValve As Integer
        Dim sOld As String = String.Empty
        Dim sNew As String = String.Empty
        Dim bAll As Boolean = (sCycleName = String.Empty)
        Dim bSaveAllThisCycle As Boolean = False
        'select the file based on version, valve, and arm
        If rRobot.Controller.Version >= 7.5 Then
            '7.5 goes to 3 digit valve number - still format for 2 digits for valve 1-99
            Select Case rRobot.ArmNumber
                Case 1
                    sProgName = "CHGCL" & rColorChange.Valve.ToString("00")
                Case Else
                    sProgName = "CH" & rRobot.ArmNumber.ToString & "CL" & rColorChange.Valve.ToString("00")
            End Select
        Else
            Select Case rRobot.ArmNumber
                Case 1
                    sProgName = "CHGCOL" & rColorChange.Valve.ToString("00")
                Case Else
                    sProgName = "CH" & rRobot.ArmNumber.ToString & "COL" & rColorChange.Valve.ToString("00")
            End Select
        End If
        'Get the program object for now, declare the cycle data vars for the loop
        rRobot.ProgramName = sProgName
        Dim oProgVars As FRRobot.FRCVars = rRobot.ProgramVars
        If bAll Then
            oProgVars.NoUpdate = True
        End If
        Dim oCycleData As FRRobot.FRCVars
        Dim oStep As FRRobot.FRCVars
        'Cycle count varies by option, This file only has cycle header and step vars, so we'll use that,
        'compare to the list data and take the smallerr one if they don't match
        Dim nCycleCount As Integer = CInt(oProgVars.Count / 2)
        'Take the number of cycles for the minimum data available
        If nCycleCount < rColorChange.NumberOfCycles Then
            rColorChange.NumberOfCycles = nCycleCount
        End If
        For nCycle As Integer = 1 To rColorChange.NumberOfCycles
            bSaveAllThisCycle = bSaveAll

            If bAll OrElse rColorChange.Cycle(nCycle).CycleName = sCycleName Then
                With rColorChange.Cycle(nCycle)

                    If Not (op Is Nothing) Then
                        '    rForm.progress = ProgressMin + nCycle * (ProgressMax - ProgressMin) \ nCycleCount
                        op.Invoke(ProgressMin + nCycle * (ProgressMax - ProgressMin) \ nCycleCount, _
                                  (gpsRM.GetString("psSAVING_CYCLE") & nCycle.ToString))
                        frmMain.Refresh()
                    End If

                    'Save pushout volume if required
                    If rColorChange.ShowPushoutVolume(nCycle) And (rRobot.Controller.Version >= 7.3) And _
                    (rColorChange.PushOutVolume.Changed Or bSaveAllThisCycle) Then
                        Try
                            'Get pushout volume
                            rRobot.ProgramName = "PAVRDBCV"
                            'COLOR_DATA{eq#>1}.NODEDATA[valve#].HDWE.PUSH_VOL
                            sVar = "COLOR_DATA"
                            If rRobot.ArmNumber > 1 Then
                                sVar = sVar & rRobot.ArmNumber.ToString
                            End If
                            sVar = sVar & ".NODEDATA[" & rColorChange.Valve.ToString & "].HDWE.PUSH_VOL"
                            rRobot.VariableName = sVar
                            Debug.Print(rColorChange.PushOutVolume.Value.ToString)

                            rRobot.VarValue = rColorChange.PushOutVolume.Value.ToString
                            If rColorChange.PushOutVolume.Changed Then
                                UpdateChangeLog(.CycleName, 0, gpsRM.GetString("psPUSH_VOL"), _
                                                rColorChange.PushOutVolume.Value.ToString, _
                                                rColorChange.PushOutVolume.OldValue.ToString, _
                                                rRobot.Name, rColorChange.ValveDescription, _
                                                rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                            End If
                            rColorChange.PushOutVolume.Update()
                        Catch ex As Exception
                            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, frmMain.msSCREEN_NAME, _
                                                frmMain.Status, MessageBoxButtons.OK)
                        End Try
                    End If
                    rRobot.ProgramName = sProgName
                    'Normal cycle data
                    If .NumberOfSteps <> .NumberOfStepsOld Or bSaveAllThisCycle Then
                        bSaveAllThisCycle = True ' save everything if the steps have been rearranged
                        sVar = "C_CHG_HED_" & nCycle.ToString("00") & ".CYCLE_STEPS"
                        rRobot.VariableName = sVar
                        rRobot.VarValue = .NumberOfSteps.ToString
                        ' update change log
                        If .NumberOfSteps <> .NumberOfStepsOld Then
                            UpdateChangeLog(.CycleName, 0, gpsRM.GetString("psNUM_STEPS"), .NumberOfSteps.ToString, .NumberOfStepsOld.ToString, _
                                            rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                        End If
                    End If

                    sVar = "C_CHG_CYC_" & nCycle.ToString("00")
                    oCycleData = DirectCast(oProgVars.Item(sVar), FRRobot.FRCVars)
                    If bAll = False Then
                        oCycleData.NoUpdate = True
                    End If
                    For nStep = 1 To .NumberOfSteps

                        oStep = DirectCast(oCycleData.Item(nStep), FRRobot.FRCVars)

                        'Event
                        If .Steps(nStep).StepEvent.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("STEP_EVENT"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).StepEvent.Value
                            ' update change log
                            If .Steps(nStep).StepEvent.Changed Then
                                UpdateChangeLog(.CycleName, nStep, gpsRM.GetString("psEVENT"), _
                                                    rColorChange.EventName(.Steps(nStep).StepEvent.Value), _
                                                    rColorChange.EventName(.Steps(nStep).StepEvent.OldValue), _
                                                    rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                            End If
                        End If

                        'Shared Valves
                        If .Steps(nStep).DoutState.Changed Or .Steps(nStep).DoutDC.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("D_OUT_STATE"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).DoutState.Value
                            oVar = DirectCast(oStep.Item("D_OUT_DC"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).DoutDC.Value
                            ' update change log only if there are details
                            If ((rCCToon Is Nothing) = False) And (.Steps(nStep).DoutState.Changed Or .Steps(nStep).DoutDC.Changed) Then
                                For nValve = 1 To rCCToon.NumberOfSharedValves
                                    If ValveChanged(nValve, .Steps(nStep).DoutDC.OldValue, .Steps(nStep).DoutDC.Value, _
                                                    .Steps(nStep).DoutState.OldValue, .Steps(nStep).DoutState.Value, _
                                                    sOld, sNew) Then
                                        UpdateChangeLog(.CycleName, nStep, rCCToon.sSharedValveNames(nValve - 1), sNew, sOld, _
                                                        rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                                    End If
                                Next
                            End If
                        End If
                        'Group Valves
                        If .Steps(nStep).GoutState.Changed Or .Steps(nStep).GoutDC.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("G_OUT_STATE"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).GoutState.Value
                            oVar = DirectCast(oStep.Item("G_OUT_DC"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).GoutDC.Value
                            ' update change log only if there are details
                            If ((rCCToon Is Nothing) = False) And (.Steps(nStep).GoutState.Changed Or .Steps(nStep).GoutDC.Changed) Then
                                For nValve = 1 To rCCToon.NumberOfGroupValves  'MSW 1/12/13 this was NumberOfSharedValves
                                    If ValveChanged(nValve, .Steps(nStep).GoutDC.OldValue, .Steps(nStep).GoutDC.Value, _
                                                    .Steps(nStep).GoutState.OldValue, .Steps(nStep).GoutState.Value, _
                                                    sOld, sNew) Then
                                        UpdateChangeLog(.CycleName, nStep, rCCToon.sGroupValveNames(nValve - 1), sNew, sOld, _
                                                        rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                                    End If
                                Next
                            End If
                        End If
                        'oVar = DirectCast(oStep.Item("EVENT_TYPE"), FRRobot.FRCVar)

                        'Action
                        If .Steps(nStep).StepAction.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("STEP_ACTION"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).StepAction.Value
                            ' update change log
                            If .Steps(nStep).StepAction.Changed Then
                                UpdateChangeLog(.CycleName, nStep, gpsRM.GetString("psACTION"), _
                                                rColorChange.ActionName(.Steps(nStep).StepAction.Value), _
                                                rColorChange.ActionName(.Steps(nStep).StepAction.OldValue), _
                                                rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                            End If
                        End If


                        'Preset
                        If .Steps(nStep).Preset.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("PRESET_SEL"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).Preset.Value
                            ' update change log
                            If .Steps(nStep).Preset.Changed Then
                                UpdateChangeLog(.CycleName, nStep, gpsRM.GetString("psPRESET"), _
                                                    .Steps(nStep).Preset.Value.ToString, _
                                                    .Steps(nStep).Preset.OldValue.ToString, _
                                                    rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                            End If
                        End If

                        'Duration
                        If .Steps(nStep).Duration.Changed Or bSaveAllThisCycle Then
                            oVar = DirectCast(oStep.Item("DUR_TIME"), FRRobot.FRCVar)
                            oVar.Value = .Steps(nStep).Duration.Value * 10
                            ' update change log
                            If .Steps(nStep).Duration.Changed Then
                                UpdateChangeLog(.CycleName, nStep, gpsRM.GetString("psTIME"), _
                                                .Steps(nStep).Duration.Value.ToString, _
                                                .Steps(nStep).Duration.OldValue.ToString, _
                                                rRobot.Name, rColorChange.ValveDescription, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
                            End If
                        End If
                    Next

                    If bAll = False Then
                        oCycleData.Update()
                        oCycleData.NoUpdate = False
                    End If
                    .Update()

                End With
            End If
        Next
        If bAll Then
            oProgVars.Update()
            oProgVars.NoUpdate = False
        End If

        Return True

    End Function
    Friend Function LoadCopyScreenSubParamBox(ByRef oRobot As clsArm, _
                            ByRef clbBox As CheckedListBox, ByVal ParamName As String, ByVal style As Integer) As Boolean
        '********************************************************************************************
        'Description:  load preset box from preset copy screen - needs to match routine for colorchg
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Added (unused) Style param for compatibility with frmCopy v4.01.01.03
        '********************************************************************************************
        'Dim void As New DataSet

        'Dim oColorChange As clsColorChange = oRobot.SystemColors(0).Valve.ColorChange

        'oColorChange.subGetCCInfo()

        clbBox.Items.Clear()
        clbBox.BeginUpdate()
        'For nCycle As Integer = 1 To oColorChange.NumberOfCycles
        '    clbBox.Items.Add(oColorChange.Cycle(nCycle).CycleName)
        'Next
        For nCycle As Integer = 1 To frmMain.mApplicator.NumCCCycles
            clbBox.Items.Add(frmMain.mApplicator.CCCycleName(nCycle))
        Next

        clbBox.EndUpdate()

        Return True

    End Function
    Friend Sub UpdateChangeLog(ByRef sCycle As String, ByVal nStep As Integer, ByRef sItem As String, _
                               ByRef NewValue As String, ByRef OldValue As String, _
                               ByRef Device As String, ByRef Color As String, _
                               ByVal ZoneNumber As Integer, ByRef ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        sTmp = sCycle
        If nStep > 0 Then
            sTmp = sTmp & gpsRM.GetString("psSTEP_CL") & nStep.ToString
        End If

        sTmp = sTmp & " " & sItem & " " & gcsRM.GetString("csCHANGED_FROM") & " " & OldValue & " " & _
                 gcsRM.GetString("csTO") & " " & NewValue
        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, Color, sTmp, ZoneName)

    End Sub
    Friend Sub UpdateChangeLogCopy(ByVal sCycle As String, _
                               ByVal sSrcDevice As String, _
                               ByVal Device As String, ByVal ColorFrom As String, ByVal ColorTo As String, _
                               ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        sTmp = gcsRM.GetString("csCOPIED") & " " & sCycle & " " & gcsRM.GetString("csFROM") & " " & _
                sSrcDevice & ", " & gcsRM.GetString("csCOLOR") & " " & ColorFrom & " " & gcsRM.GetString("csTO") & " " & _
                Device & ", " & gcsRM.GetString("csCOLOR") & " " & ColorTo
        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, ColorTo, sTmp, ZoneName)

    End Sub
    Friend Function CreateColorChangeDataset(ByVal AddDescColumn As Boolean, _
                                ByRef rColorChange As clsColorChange, ByRef rArm As clsArm, _
                                ByRef rCCtoon As clsColorChangeCartoon, _
                                ByRef sCycleName As String) As DataSet
        '********************************************************************************************
        'Description:  create a dataset from the preset data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DT As DataTable = New DataTable
        Dim DR As DataRow = Nothing
        Dim DS As DataSet = New DataSet
        Const nTIME_IDX As Integer = 0
        Const nPRESET_IDX As Integer = 1
        Const nACTION_IDX As Integer = 2
        Const nEVENT_IDX As Integer = 3
        Dim nCycle As Integer
        For nCycle = 1 To rColorChange.NumberOfCycles
            If rColorChange.Cycle(nCycle).CycleName = sCycleName Then
                Exit For
            End If
        Next

        'Title
        DT.TableName = rArm.Name & "   " & gpsRM.GetString("psVALVE") & " " & rColorChange.ValveDescription & "   " & sCycleName
        'label Column
        DT.Columns.Add(gsALL_INDEX)
        For nRow As Integer = 1 To rCCtoon.NumberOfSharedValves + rCCtoon.NumberOfGroupValves + nEVENT_IDX + 1
            DR = DT.NewRow()
            DT.Rows.Add(DR)
        Next

        'Labels 
        DT.Rows(nTIME_IDX).Item(gsALL_INDEX) = gpsRM.GetString("psTIME")
        DT.Rows(nPRESET_IDX).Item(gsALL_INDEX) = gpsRM.GetString("psPRESET")
        DT.Rows(nACTION_IDX).Item(gsALL_INDEX) = gpsRM.GetString("psACTION")
        DT.Rows(nEVENT_IDX).Item(gsALL_INDEX) = gpsRM.GetString("psEVENT")
        Dim nValve As Integer
        Dim nBitMask As Integer
        Dim nValveState As Integer
        For nValve = 1 To rCCtoon.NumberOfSharedValves
            DT.Rows(nEVENT_IDX + nValve).Item(gsALL_INDEX) = rCCtoon.SharedValveNames(nValve - 1)
        Next
        For nValve = 1 To rCCtoon.NumberOfGroupValves
            DT.Rows(nEVENT_IDX + rCCtoon.NumberOfSharedValves + nValve).Item(gsALL_INDEX) = rCCtoon.GroupValveNames(nValve - 1)
        Next
        For nStep As Integer = 1 To rColorChange.Cycle(nCycle).NumberOfSteps
            DT.Columns.Add(nStep.ToString)
            DT.Rows(nTIME_IDX).Item(nStep.ToString) = rColorChange.Cycle(nCycle).Steps(nStep).Duration.Value.ToString
            DT.Rows(nPRESET_IDX).Item(nStep.ToString) = rColorChange.Cycle(nCycle).Steps(nStep).Preset.Value.ToString
            DT.Rows(nACTION_IDX).Item(nStep.ToString) = rColorChange.ActionName(rColorChange.Cycle(nCycle).Steps(nStep).StepAction.Value)
            DT.Rows(nEVENT_IDX).Item(nStep.ToString) = rColorChange.EventName(rColorChange.Cycle(nCycle).Steps(nStep).StepEvent.Value)
            For nValve = 1 To rCCtoon.NumberOfSharedValves
                nBitMask = CType(2 ^ (nValve - 1), Integer)
                nValveState = 0
                If ((rColorChange.Cycle(nCycle).Steps(nStep).DoutState.Value And nBitMask) > 0) Then
                    'select each valve that's on in the step data
                    nValveState = 1
                ElseIf ((rColorChange.Cycle(nCycle).Steps(nStep).DoutDC.Value And nBitMask) > 0) Then
                    nValveState = 2
                Else
                    nValveState = 0
                End If

                DT.Rows(nEVENT_IDX + nValve).Item(nStep.ToString) = nValveState
            Next
            For nValve = 1 To rCCtoon.NumberOfGroupValves
                nBitMask = CType(2 ^ (nValve - 1), Integer)
                nValveState = 0
                If ((rColorChange.Cycle(nCycle).Steps(nStep).GoutState.Value And nBitMask) > 0) Then
                    'select each valve that's on in the step data
                    nValveState = 1
                ElseIf ((rColorChange.Cycle(nCycle).Steps(nStep).GoutDC.Value And nBitMask) > 0) Then
                    nValveState = 2
                Else
                    nValveState = 0
                End If
                DT.Rows(nEVENT_IDX + rCCtoon.NumberOfSharedValves + nValve).Item(nStep.ToString) = nValveState
            Next
        Next
        DS.Tables.Add(DT)
        Return DS

    End Function


    Friend Function LoadMultiScreenSubParameterBox(ByRef rCbo As ComboBox, ByRef colZones As clsZones, _
                ByVal ParamName As String, ByVal bAddAll As Boolean) As Boolean
        '********************************************************************************************
        'Description: This Routine is called from frmAll when needed
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Don't care about the color yet, just get the cycle data
        'Dim oColorChange As clsColorChange = mViewAllRobot.SystemColors(0).Valve.ColorChange
        'oColorChange.subGetCCInfo() 'refresh the cycle names
        'mCCCommon.LoadCCCycleBox(oColorChange, rCbo)
        mCCCommon.LoadCCCycleBox(mViewAllRobot.Applicator, rCbo)

    End Function
#End Region

End Module
