
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
' Form/Module: mPresetCommon
'
' Description: Routines for Presets
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
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    06/20/07   gks     changes for paint shop computer
'    06/20/08   gks     Changes for estat descriptions
'    06/01/09   MSW     Convert to VB2008, get it working with the class change made for FluidMaint
'                       GetParamNames - removed ApplicatorType, use ColorChangeType instead
'    11/10/09   MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'                       parm name on the robot is used for resource lookup:
'                       for APP_CON_NAM= "FF", rsFF = "Paint", rsFF_CAP = "Paint cc/min", rsFF_UNITS ="
'                       supports 2nd shaping air, volume on CC screen
'    11/16/09   MSW     Convert to Rick's new FTP class
'    12/03/09   MSW     LoadOverRidesFromRobot, SaveOverRidesToRobot - Multiversion support
'    05/07/10   BTK     LoadEstatStepValues
'    08/23/10   MSW     GetPresetDataset - This still needed some work to get the table loaded
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    01/24/12   MSW     Applicator Updates                                            4.01.01.01
'    04/11/12   MSW     put the specific type of preset in the change log description 4.01.03.00
'    10/19/12   BTK&KDJ Bug Fixes for Presets by Style/Color. ValidatePresetDataRow/  4.01.03.01
'                       ValidatePresetDatatable - added Style number - was hard coded 
'                       to 1. GetPresetDataset - Added code to generate all presets 
'                       for each color and style when a new robot is selected, only 
'                       when there is no database table. Changed nStyle = 1 by default 
'                       for presets by color not style.
'    01/01/13   RJO     Preset descriptions were always for Style 1 regardless of     4.01.03.02
'                       selected Style for presets by Style/Color. Added Style 
'                       parameter to LoadCopyScreenSubParamBox.
'    04/10/13   MSW     Bug fix to generic preset descriptions in Function            4.01.03.03
'                       LoadCopyScreenSubParamBox. Did not work properly when more 
'                       than one style was selected as source.
'    04/16/13   MSW     Improve debug log details                                     4.01.05.00
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.05.01
'    05/16/14   BTK     Support degrade by style                                      4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.IO
Imports FSFTPLib ' ftp object from upstairs - was browsed to with vb6. add as reference with
'                  .net, dereference, copy interop to apps dir and reference interop
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Friend Module mPresetCommon

#Region " Declares "

    Friend Const gsPRESET_TABLE_PREFIX As String = "Presets "
    Friend Const gsFANUC_COLOR_COL As String = "Fanuc Color"
    Friend Const gsFANUC_STYLE_COL As String = "Style"
    Friend Const gsPRESET_COL As String = "Preset"
    Friend Const gsDESC_COL As String = "Description"
    Friend Const gsESTATDESC_COL As String = "Estat Description"    '6.20.08
    Friend Const gsVALVE_COL As String = "Valve"
    Friend Const gnSTEP_START_INDEX As Integer = 23
    Friend Const gnSTEP_OFFSET As Integer = 3
    Friend gStyleNameArray() As String
    Private moSWriter As StreamWriter = Nothing
    Private Const sLOGFILE As String = "C:\Temp\Presetcopy.log"
    Private Const bLOG As Boolean = False
    Private msFanucStyleIndex() As String = Nothing
    Private mViewAllRobot As clsArm = Nothing
    Private meScreen As frmMain.eScreenSelect = frmMain.eScreenSelect.FluidPresets
#End Region
#Region " Routines "
    Friend Property Screen() As frmMain.eScreenSelect
        Set(ByVal Value As frmMain.eScreenSelect)
            meScreen = Value
        End Set
        Get
            Return meScreen
        End Get
    End Property
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
        Select Case frmMain.ScreenSelect
            Case frmMain.eScreenSelect.CCPresets
                oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME_CC & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
            Case frmMain.eScreenSelect.EstatPresets
                oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME_ESTAT & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
            Case frmMain.eScreenSelect.FluidPresets
                oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
            Case Else
                oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
        End Select

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

        Select Case frmMain.ScreenSelect
            Case frmMain.eScreenSelect.CCPresets
                Select Case sSubScreen
                    Case "MultiView"
                        subLaunchHelp(gs_HELP_CCPRESETS_MV, frmMain.oIPC)
                    Case "Copy"
                        subLaunchHelp(gs_HELP_CCPRESETS_COPY, frmMain.oIPC)
                    Case Else
                        subLaunchHelp(gs_HELP_CCPRESETS, frmMain.oIPC)
                End Select
            Case frmMain.eScreenSelect.EstatPresets
                Select Case sSubScreen
                    Case "MultiView"
                        subLaunchHelp(gs_HELP_ESTATPRESETS_MV, frmMain.oIPC)
                    Case "Copy"
                        subLaunchHelp(gs_HELP_ESTATPRESETS_COPY, frmMain.oIPC)
                    Case Else
                        subLaunchHelp(gs_HELP_ESTATPRESETS, frmMain.oIPC)
                End Select
            Case frmMain.eScreenSelect.FluidPresets
                Select Case sSubScreen
                    Case "MultiView"
                        subLaunchHelp(gs_HELP_PRESETS_MV, frmMain.oIPC)
                    Case "Copy"
                        subLaunchHelp(gs_HELP_PRESETS_COPY, frmMain.oIPC)
                    Case Else
                        subLaunchHelp(gs_HELP_PRESETS, frmMain.oIPC)
                End Select
            Case Else
                subLaunchHelp(gs_HELP_PRESETS, frmMain.oIPC)
        End Select
    End Sub
    Private Function bGetPresets(ByRef oRobot As clsArm, ByRef oPresets As clsPresets, _
                                    ByVal sFanucNum As Integer, ByRef oCopy As frmCopy, _
                                    ByVal bSourcePresets As Boolean, _
                                    ByVal nStyle As Integer) As Boolean
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
            If (oPresets Is Nothing) Then
                If meScreen = frmMain.eScreenSelect.CCPresets Then
                    oPresets = oRobot.SystemColors(1, False).Presets
                    oPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
                    oPresets.FanucColor = 1
                    oPresets.ColorChangePresets = True
                Else
                    oPresets = oRobot.SystemColors(sFanucNum, False).Presets
                End If
            Else
                If oPresets.ColorChangePresets Then
                    oPresets = oRobot.SystemColors(1, False).Presets
                    oPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
                    oPresets.FanucColor = 1
                    oPresets.ColorChangePresets = True
                Else
                    oPresets = oRobot.SystemColors(sFanucNum, False).Presets
                End If
            End If

            Dim dsDesc As DataSet = New DataSet

            If bSourcePresets Then
                sTmp = gcsRM.GetString("csSELECTED_AS_SOURCE")
            Else
                sTmp = gcsRM.GetString("csSELECTED_AS_TARGET")
            End If
            If oPresets.ColorChangePresets Then
                oCopy.Status = oRobot.Controller.Name & " " & sTmp
            Else
                oCopy.Status = oRobot.Name & ", " & gcsRM.GetString("csCOLOR") & " " & _
                sFanucNum.ToString & "," & gpsRM.GetString("psSTYLE_CAP") & " " & nStyle.ToString & " " & sTmp
            End If
            If bSourcePresets Then
                sTmp = gpsRM.GetString("psLOADING_SOURCE")
            Else
                sTmp = gpsRM.GetString("psLOADING_TARGET")
            End If

            oCopy.Status = sTmp
            oPresets.StyleNumber = nStyle
            ' need to load both as using display name
            If frmMain.bLoadFromRobot(oRobot, oPresets) Then

                If bSourcePresets Then
                    sTmp = gpsRM.GetString("psLOADING_SOURCE_DESC")
                Else
                    sTmp = gpsRM.GetString("psLOADING_TARGET_DESC")
                End If

                oCopy.Status = sTmp

                If frmMain.bLoadFromGUI(oRobot.Controller.Zone, oRobot, oPresets, dsDesc) = False Then
                    'something failed bomb out... need continue to next here somehow
                    Return False
                End If
            Else
                'something failed bomb out... need continue to next here somehow
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
        ' 11/24/09  RJO     Modified call to LoadArmCollection for oTargetCollection to use new
        '                   version of LoadArmCollection with IncludeOpeners parameter.
        '********************************************************************************************
        Try

            Dim colControllersTo As New clsControllers(oZones, False)
            oTargetCollection = LoadArmCollection(colControllersTo, False) 'RJO 11/24/09
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
        ' 06/29/11  MSW     change to use name instead of index to select robots
        '********************************************************************************************
        Dim oSourceRobot As clsArm
        Dim sSourceFanucNum As String
        Dim oTargetRobot As clsArm
        Dim sTargetFanucNum As String
        Dim colTargetArms As clsArms = Nothing
        Dim sSrcRobot As String
        Dim sTgtRobot As String
        Dim sSourceStyleNum As String
        Dim sTargetStyleNum As String

        oCopy.Status = gcsRM.GetString("csCOPY_STARTED")

        Try

            If bLOG Then
                moSWriter = New StreamWriter(sLOGFILE, False) 'overwrite
                moSWriter.WriteLine("Copy operation started: " & Now)
                moSWriter.WriteLine(" ")
            End If

            'Color change copy, no color data.  Add one to the list so we can leave the in there
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                If colParamFrom.Count = 0 Then
                    colParamFrom.Add("0")
                End If
            End If
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                If colParamTo.Count = 0 Then
                    colParamTo.Add("0")
                End If
            End If
            If colStyleFrom Is Nothing Then
                colStyleFrom = New Collection(Of String)
                colStyleFrom.Add("1")
            End If
            If colStyleTo Is Nothing Then
                colStyleTo = New Collection(Of String)
                colStyleTo.Add("1")
            End If
            '  get target zone data
            For Each oZone As clsZone In colZoneTo
                If oZone.Selected Then
                    colZoneTo.CurrentZone = oZone.Name
                    ' get arms from selected zone(s) 1 zone at a time
                    bGetTargetArms(colZoneTo, colTargetArms, colRobotTo)

                    'the source is coming from a single zone
                    For Each oSourceRobot In colRobotFrom
                        If oSourceRobot.Selected Then
                            For Each sSourceFanucNum In colParamFrom
                                For Each sSourceStyleNum In colStyleFrom
                                    'get the source presets
                                    'set up new preset collection
                                    Dim oSourcePresets As New clsPresets
                                    oSourcePresets.ColorChangePresets = (meScreen = frmMain.eScreenSelect.CCPresets)

                                    If oSourcePresets.ColorChangePresets Then
                                        sSrcRobot = oSourceRobot.Controller.Name
                                    Else
                                        sSrcRobot = oSourceRobot.Name
                                    End If
                                    If bGetPresets(oSourceRobot, oSourcePresets, CInt(sSourceFanucNum), _
                                                oCopy, True, CInt(sSourceStyleNum)) Then
                                        'it worked
                                    Else
                                        'something failed bomb out... need continue to next here somehow
                                        If bLOG Then
                                            moSWriter.WriteLine("Source Robot: " & oSourceRobot.Name)
                                            moSWriter.WriteLine("   Load failed - fanuc color number " & sSourceFanucNum)
                                        End If
                                        Return False
                                    End If


                                    For Each oTargetRobot In colTargetArms
                                        ' if there is more than 1 source robot - set equal to source robot index
                                        ' and bail at bottom of for loop. Other wise loop thru all selected
                                        If colRobotFrom.SelectedCount > 1 Then
                                            ' robot index starts at 1 - should be selected from bGetTargetArms
                                            ' 06/29/11  MSW     change to use name instead of index to select robots
                                            oTargetRobot = colTargetArms(oSourceRobot.Name)
                                        End If

                                        If oTargetRobot.Selected Then
                                            ' loop thru colors - parameter collections are only loaded with parameters that
                                            ' are selected
                                            For Each sTargetFanucNum In colParamTo
                                                ' if there is more than 1 source param - set equal to source param index
                                                ' and bail at bottom of for loop. Other wise loop thru all 
                                                If colParamFrom.Count > 1 Then
                                                    ' align on index
                                                    sTargetFanucNum = sSourceFanucNum
                                                End If  'colParamFrom.Count > 1
                                                For Each sTargetStyleNum In colStyleTo
                                                    ' if there is more than 1 source param - set equal to source param index
                                                    ' and bail at bottom of for loop. Other wise loop thru all 
                                                    If colStyleFrom.Count > 1 Then
                                                        ' align on index
                                                        sTargetStyleNum = sSourceStyleNum
                                                    End If  'colParamFrom.Count > 1

                                                    'get existing target data
                                                    Dim oTargetPresets As New clsPresets
                                                    oTargetPresets.ColorChangePresets = (meScreen = frmMain.eScreenSelect.CCPresets)
                                                    If oTargetPresets.ColorChangePresets Then
                                                        sTgtRobot = oTargetRobot.Controller.Name
                                                    Else
                                                        sTgtRobot = oTargetRobot.Name
                                                    End If
                                                    If bGetPresets(oTargetRobot, oTargetPresets, CInt(sTargetFanucNum), _
                                                                oCopy, False, CInt(sTargetStyleNum)) Then

                                                        If bLOG Then
                                                            moSWriter.WriteLine("Source Robot: " & sSrcRobot)
                                                            moSWriter.WriteLine("   Parameter: " & oSourcePresets.DisplayName)
                                                            moSWriter.WriteLine(" ")
                                                            moSWriter.WriteLine("Target Robot: " & sTgtRobot)
                                                            moSWriter.WriteLine("   Parameter: " & oTargetPresets.DisplayName)
                                                            moSWriter.WriteLine(" ")
                                                        End If

                                                        ' have all data to do copy
                                                        oCopy.Status = gcsRM.GetString("csWRITING_TARGET")
                                                        If bActuallyDoTheCopy(oSourceRobot, oTargetRobot, _
                                                                            oSourcePresets, oTargetPresets, _
                                                                            colSubParamFrom, colSubParamTo, CopyType) = False Then

                                                            'something failed bomb out... need continue to next here somehow
                                                            oCopy.Status = gcsRM.GetString("csWRITE_FAIL")

                                                            If bLOG Then
                                                                moSWriter.WriteLine("Copy did not complete successfully")
                                                            End If

                                                            Return False

                                                        Else
                                                            oCopy.Status = gcsRM.GetString("csWRITE_SUCCESS")
                                                            oCopy.Status = String.Empty
                                                        End If

                                                    Else

                                                        If bLOG Then
                                                            moSWriter.WriteLine("Target Robot: " & oTargetRobot.Name)
                                                            moSWriter.WriteLine("   Load failed - fanuc color number " & sTargetFanucNum)
                                                        End If

                                                        'something failed bomb out... need continue to next here somehow
                                                        Return False
                                                    End If

                                                    'multiple from styles, just do 1 to
                                                    If colStyleFrom.Count > 1 Then Exit For
                                                Next
                                                ' multiple from colors - just do 1 color
                                                If colParamFrom.Count > 1 Then Exit For
                                            Next 'sTargetFanucNum
                                        End If  'oTargetRobot.Selected

                                        'multiple from selections - just do 1 robot
                                        If colRobotFrom.SelectedCount > 1 Then Exit For
                                    Next 'oTargetRobot
                                Next 'sSourceStyleNum
                            Next ' sSourceFanucNum
                        End If ' oSourceRobot.Selected
                    Next 'oSourceRobot In colRobotFrom
                End If 'oZone.Selected
            Next 'oZone

            If bLOG Then
                moSWriter.WriteLine(" ")
                moSWriter.WriteLine("Copy Done: " & Now)
                moSWriter.Close()
            End If

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            'MSW 4/16/13 - Add some details to error logging
            WriteEventToLog(frmMain.msSCREEN_NAME & ":DoCopy", ex.StackTrace & vbCrLf & ex.Message)


            If bLOG Then
                moSWriter.WriteLine("Error: " & ex.Message)
                moSWriter.Close()
            End If

            Return False

        End Try

        Return True

    End Function
    Private Function bActuallyDoTheCopy(ByRef rSourceRobot As clsArm, ByRef rTargetRobot As clsArm, _
                    ByRef rSourcePresets As clsPresets, ByRef rTargetPresets As clsPresets, _
                    ByRef rSourcePresetsToCopy As Collection(Of String), _
                    ByRef rTargetPresetsToCopy As Collection(Of String), _
                    ByVal CopyType As eCopyType) As Boolean
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
        '********************************************************************************************
        Dim sFrom As String = String.Empty
        Dim sTo As String = String.Empty
        Dim oFrom As clsPreset = Nothing
        Dim oTo As clsPreset = Nothing


        Dim bCopyParm(rSourcePresets.NumParms) As Boolean
        Dim bValue As Boolean = False
        For nParm As Integer = 0 To rSourcePresets.NumParms - 1
            bCopyParm(nParm) = (CopyType And CInt(2 ^ nParm)) > 0
            bValue = bValue Or bCopyParm(nParm)
        Next
        bCopyParm(rSourcePresets.NumParms) = ((CopyType And eCopyType.CopyDesc) = eCopyType.CopyDesc)
        Application.DoEvents()
        For Each sFrom In rSourcePresetsToCopy
            'source data
            oFrom = rSourcePresets(sFrom)



            For Each sTo In rTargetPresetsToCopy

                If rSourcePresetsToCopy.Count > 1 Then
                    ' more that 1 target = source exit for at bottom
                    oTo = rTargetPresets(oFrom.Index)
                Else
                    oTo = rTargetPresets(sTo)
                End If

                For nParm As Integer = 0 To rSourcePresets.NumParms - 1
                    If bCopyParm(nParm) Then
                        oTo.Param(nParm).Value = oFrom.Param(nParm).Value
                    End If
                Next
                'set desired values
                If bCopyParm(rSourcePresets.NumParms) Then
                    Select Case meScreen
                        Case frmMain.eScreenSelect.FluidPresets, frmMain.eScreenSelect.CCPresets

                            oTo.Description.Text = oFrom.Description.Text
                        Case frmMain.eScreenSelect.EstatPresets

                            oTo.EstatDescription.Text = oFrom.EstatDescription.Text
                    End Select

                End If

                ' more that 1 target = source 
                If rSourcePresetsToCopy.Count > 1 Then Exit For
            Next ' sTo
        Next ' sFrom

        ' save desc first - robot calls update
        If bCopyParm(rSourcePresets.NumParms) Then
            Dim dsDescriptions As DataSet = New DataSet
            If frmMain.bSaveToGUI(rTargetRobot, rTargetPresets, dsDescriptions) = False Then
                Return False
            End If
        End If


        'save it to robot
        If bValue Then
            If frmMain.bSaveToRobot(rTargetRobot, rTargetPresets) = False Then
                Return False
            End If
        End If


        'For SQL database - remove above eventually
        mPWCommon.SaveToChangeLog(rTargetRobot.Controller.Zone)


        Return True

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
        ' 11/24/09  RJO     Modified call to LoadArmCollection for oArms to use new version of 
        '                   LoadArmCollection with IncludeOpeners parameter.
        '********************************************************************************************
        Try
            Dim oZones As New clsZones(DatabasePath)
            oZones.CurrentZone = ZoneName
            Dim oControllers As clsControllers = New clsControllers(oZones, False)
            Dim oArms As clsArms = LoadArmCollection(oControllers, False)
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                rArm = oControllers.Item(RobotName).Arms.Item(0)
            Else
                rArm = oArms.Item(RobotName)
            End If
            mViewAllRobot = rArm

            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            'MSW 4/16/13 - Add some details to error logging
            WriteEventToLog(frmMain.msSCREEN_NAME & ":GetViewAllRobot", ex.StackTrace & vbCrLf & ex.Message)

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
        '********************************************************************************************
        Dim ParamName As String = rCboParamName.Text
        Dim SubParamName As String = rCboSubParamName.Text
        ' 06/01/09  MSW     CC preset support'Select CC or color presets
        Dim oPresets As clsPresets
        If meScreen = frmMain.eScreenSelect.CCPresets Then
            oPresets = mViewAllRobot.SystemColors(1, False).Presets
            oPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
            oPresets.FanucColor = 1
            oPresets.ColorChangePresets = True
        Else
            oPresets = mViewAllRobot.SystemColors(ParamName).Presets
            oPresets.DisplayName = ParamName
            oPresets.FanucColor = mViewAllRobot.SystemColors(ParamName).FanucNumber
            If frmMain.mbPresetsByStyle And (msFanucStyleIndex IsNot Nothing) Then
                oPresets.StyleNumber = CInt(msFanucStyleIndex(rCboSubParamName.SelectedIndex))
            End If
        End If

        Dim ds As New DataSet
        oPresets.EstatType = mViewAllRobot.EstatType
        If mViewAllRobot.Applicator Is Nothing Then
            mViewAllRobot.Applicator = frmMain.colApplicators(mViewAllRobot.ColorChangeType)
        End If
        If mViewAllRobot.IsOnLine Then
            If LoadPresetsFromRobot(mViewAllRobot, oPresets) Then
                If frmMain.bLoadFromGUI(mViewAllRobot.Controller.Zone, mViewAllRobot, oPresets, ds) Then
                    Return CreatePresetDataset(False, oPresets, mViewAllRobot, SubParamName)
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
    Friend Function LoadEstatStepValues(ByRef rRobot As clsArm, _
                                                        ByRef Values As String()) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Estat Unit
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/09  MSW     Convert to Rick's new FTP class
        ' 05/07/10  BTK     Changed to use .EstatName instead of .EstatIP.  If we do this then we need
        '                   second text file that is identical to one that is already there for the estat
        '                   parameters screen.  Changed the file name to match what the estat parameters
        '                   screen expects, sDevice & "_par_val.txt" instead of sDevice & "estat.txt"
        '********************************************************************************************
        'Dim oFTP As FSFTPLib.FSCFtp = New FSFTPLib.FSCFtp
        Dim oFTP As clsFSFtp = Nothing
        Const sFILE As String = "com_par.txt"
        'Const sUSER As String = "anonymous"
        'Const sPWD As String = ""
        'Const nPORT As Integer = 21
        Dim bRet As Boolean = True
        Dim sDevice As String = String.Empty
        Dim sPath As String = String.Empty
        Dim sPathAndFile As String = String.Empty
        Dim sTarget As String = String.Empty

        Try

            sDevice = rRobot.EstatName
            If sDevice = "0.0.0.0" Then sDevice = rRobot.EstatName


            'TODO  - check this for a paint shop computer or browsed to zone

            mPWCommon.GetDefaultFilePath(sPath, eDir.VBApps, String.Empty, String.Empty)

            If mPWCommon.GetDefaultFilePath(sPathAndFile, eDir.VBApps, String.Empty, sFILE) = False Then
                'file to send to unit does not exist, create it
                Dim s As StreamWriter
                s = New StreamWriter(sPathAndFile, False) 'overwrite
                s.WriteLine("COMMAND;0.01;PAR_VAL;")
                s.Close()
            End If

            If mPWCommon.PingDevice(sDevice) Then
                oFTP = New clsFSFtp(sDevice)
                If oFTP.Connected Then
                    'oFTP.Connect(sDevice, sUSER, sPWD, nPORT, False)
                    oFTP.PutFile(sPathAndFile, "1")
                    'oFTP.Put(sPathAndFile, "1", FSEFTPtransferFlags.fsFTPtransferFlagBinaryMode)
                    sTarget = sPath & sDevice & "_par_val.txt"
                    oFTP.GetFile("1", sTarget)
                    'oFTP.Get("1", sTarget, True, FSEFTPtransferFlags.fsFTPtransferFlagBinaryMode)
                    oFTP.Close()
                Else
                    bRet = False
                End If
            Else
                bRet = False
            End If
        Catch ex As Exception

            bRet = False
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            'MSW 4/16/13 - Add some details to error logging
            WriteEventToLog(frmMain.msSCREEN_NAME & ":LoadEstatStepValues", ex.StackTrace & vbCrLf & ex.Message)
        Finally

            'If Not oFTP Is Nothing Then
            '    If oFTP.IsFtpConnected Then oFTP.Disconnect()
            '    If oFTP.IsInternetOpen Then oFTP.Close()
            '    oFTP = Nothing
            '    GC.Collect()
            'End If
        End Try

        If bRet Then
            'break time
            Application.DoEvents()

            ' read file into collection
            Dim oReader As StreamReader
            Dim sText As String

            'Pass the file path and the file name to the StreamReader constructor.
            oReader = New StreamReader(sTarget)

            'Read the first line of text.
            sText = oReader.ReadToEnd

            'Close the file.
            oReader.Close()

            Dim sT() As String
            sT = Strings.Split(sText, ";")

            ' should have 53
            If sT.GetUpperBound(0) >= 53 Then
                'data we want starts at 23 and every third one after
                Dim OneWeWant As Integer = 23

                ' get with the 7 step program
                For i As Integer = 1 To 7
                    'rPresets.EstatStepValue(i%).Value = CInt(sT(OneWeWant))
                    'rPresets.EstatStepValue(i%).Update()
                    Values(i%) = sT(OneWeWant)
                    OneWeWant += 3
                Next


            Else
                bRet = False
            End If

        End If

        'rPresets.EstatStepValuesLoaded = bRet

        Return bRet

    End Function
    Friend Function LoadOverRidesFromRobot(ByRef rRobot As clsArm, _
                                            ByRef rPresets As clsPresets) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot - 
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     add support for color change presets
        ' 12/03/09  MSW     LoadOverRidesFromRobot, SaveOverRidesToRobot - Multiversion support
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing

        ' no error trap here - let calling routine catch it
        If rPresets.ColorChangePresets Then
            'Just reset the variables for color change presets
            For nCol As Integer = 1 To rPresets.NumParms
                rPresets.SetOverRidePercent(nCol - 1, 100)
            Next
            rPresets.SetEnableOverRide(False)
            rPresets.UpdateOverride()
        Else
            rRobot.ProgramName = "PAVRSYSC"
            Dim o As FRRobot.FRCVars = rRobot.ProgramVars
            Dim oo As FRRobot.FRCVars = DirectCast(o.Item("SYS_COLORS"), FRRobot.FRCVars)
            Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
            Dim oooo As FRRobot.FRCVars = _
                        DirectCast(ooo.Item(rPresets.FanucColor.ToString), FRRobot.FRCVars)
            Dim oOverrides As FRRobot.FRCVars = DirectCast(oooo.Item("PCT_OVERRIDE"), FRRobot.FRCVars)

            oOverrides.NoRefresh = True
            Dim nOffset As Integer = 0
            If (rRobot.Controller.Version >= 6.3) and (rRobot.Controller.Version < 7.3) Then 
                If rRobot.ArmNumber = 2 Then
                    nOffset = 6
                End If
            End If
            For nCol As Integer = 0 To rPresets.NumParms - 1
                oVar = DirectCast(oOverrides((rPresets.ScreenParm(nCol) + nOffset).ToString), FRRobot.FRCVar)
                rPresets.SetOverRidePercent(nCol, CInt(oVar.Value))
            Next
            oVar = DirectCast(oOverrides(ePresetParam.OverRideEnable), FRRobot.FRCVar)
            rPresets.SetEnableOverRide(CBool(oVar.Value))

            rPresets.UpdateOverride()

        End If


        Return True

    End Function
    Friend Function LoadPresetsFromRobot(ByRef rRobot As clsArm, _
                            ByRef rPresets As clsPresets, _
                            Optional ByRef ParmDetails() As tParmDetail = Nothing, _
                            Optional ByVal nNumParms As Integer = -1) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     add support for color change presets
        ' 11/24/09  MSW     get the right offset for equipment 2
        ' 12/17/09  MSW     Presets by style
        '********************************************************************************************

        Dim oVar As FRRobot.FRCVar = Nothing
        Dim sPathNode As String
        '''''''Coded up the local portion of presets by style, still needs the user interface, though
        Dim nJob As Integer = 1                 'Job we're reading presets for
        Dim nNumStyles As Integer = 1           'Number of styles allowed
        Dim bPresetsByStyle As Boolean = False  'Presets by style feature enabled on the robot
        '''''''
        Dim bV750Vars As Boolean = False        'Flag the version so we don't need to keep reading from the robot
        Dim nNumPresets As Integer = 40         'Number of presets, to support dynamic config in 7.50
        Dim nNumColors As Integer = 1           'Number of colors for preset config
        Dim nPresetOffset As Integer = 0        'position before the first preset we're getting in total nuber of presets
        Dim nPathNode As Integer = 0            'path node to read from in preset file 
        Dim nArrayOffset As Integer = 0         '1 less than the index for the preset array in path node where we start reading 

        rPresets.Clear()

        If ParmDetails Is Nothing Or nNumParms = -1 Then
            'This is by robot, don't need to reload it if the main screen keeps a copy
            Dim ParmSelect As mPWRobotCommon.eParmSelect = eParmSelect.All
            Select Case meScreen
                Case frmMain.eScreenSelect.CCPresets
                    ParmSelect = eParmSelect.CCPresets
                Case frmMain.eScreenSelect.EstatPresets
                    ParmSelect = eParmSelect.EstatPresets
                Case frmMain.eScreenSelect.FluidPresets
                    ParmSelect = eParmSelect.FluidPresets
            End Select
            subInitParmConfig(rRobot, ParmSelect, ParmDetails, nNumParms)
        End If
        rPresets.NumParms = nNumParms
        For nIdx As Integer = 0 To nNumParms - 1
            rPresets.ParameterName(nIdx) = ParmDetails(nIdx).sLblName
            rPresets.ScreenParm(nIdx) = ParmDetails(nIdx).nParmNum
            rPresets.ParameterMaxValue(nIdx) = CInt(ParmDetails(nIdx).nMax)
            rPresets.ParameterMinValue(nIdx) = CInt(ParmDetails(nIdx).nMin)
        Next

        If rRobot.Controller.Version >= 7.5 Then
            '[PAVROPTN] byOptn[1].byOp[78]	(Presets by JOB/STYLE, 0 = NO – 1 = YES)
            rRobot.ProgramName = "PAVROPTN"
            rRobot.VariableName = "byOptn[1].byOp[78]"
            bPresetsByStyle = (CInt(rRobot.VarValue) = 1)
            '[PAVRSYSC] num_presets	(Number of presets per color)
            rRobot.ProgramName = "PAVRSYSC"
            rRobot.VariableName = "num_presets"
            nNumPresets = CInt(rRobot.VarValue)
            '[PAVRSYSC] qty_prststyl	(Number of jobs/styles)
            rRobot.VariableName = "qty_prststyl"
            nNumStyles = CInt(rRobot.VarValue)
            '[PAVRSYSC] qty_sysc_clr	(Number of colors)
            rRobot.VariableName = "qty_sys_clr"
            nNumColors = CInt(rRobot.VarValue)
            bV750Vars = True
        End If
        If bPresetsByStyle Then
            nJob = rPresets.StyleNumber
            If nJob > nNumStyles Then
                MessageBox.Show(gpsRM.GetString("psINVALID_STYLE_1") & nJob.ToString & gpsRM.GetString("psINVALID_STYLE_2") & rRobot.Controller.Name & ".", _
                                gpsRM.GetString("psINVALID_STYLE"), MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If
        End If
        ' no error trap here - let calling routine catch it
        If rPresets.ColorChangePresets Then
            'Preset Data
            rRobot.ProgramName = "PAPSCCHG"
            sPathNode = "1"
        Else
            'Preset Data
            rRobot.ProgramName = "PAPS" & rRobot.ArmNumber.ToString
            If bV750Vars Then
                'Find the offset to the beginning of the preset table for this job and color
                nPresetOffset = ((nJob - 1) * (nNumColors * nNumPresets)) + ((rPresets.FanucColor - 1) * nNumPresets)
                nPathNode = (nPresetOffset \ gnMAX_ELM_NODE) + 1  'Node that contains the start of the table
                sPathNode = nPathNode.ToString
                nArrayOffset = nPresetOffset - ((nPathNode - 1) * gnMAX_ELM_NODE) 'Preset number in the node
            Else
                nPathNode = rPresets.FanucColor
                sPathNode = nPathNode.ToString
                nArrayOffset = 0
            End If
        End If

        Dim o As FRRobot.FRCVars = rRobot.ProgramVars
        Dim oo As FRRobot.FRCVars = DirectCast(o.Item("Preset_Data"), FRRobot.FRCVars)
        Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)

        Dim oooo As FRRobot.FRCVars = _
                    DirectCast(ooo.Item(sPathNode), FRRobot.FRCVars)
        Dim oPresets As FRRobot.FRCVars = DirectCast(oooo.Item("PRESETS"), FRRobot.FRCVars)

        oPresets.NoRefresh = True

        'get each preset column
        Dim oPresetData(rPresets.NumParms - 1) As FRRobot.FRCVars
        For nParm As Integer = 0 To rPresets.NumParms - 1
            oPresetData(nParm) = DirectCast(oPresets(rPresets.ScreenParm(nParm).ToString), FRRobot.FRCVars)
        Next

        'For older versions, use the data structure size to determine the number of presets
        If Not bV750Vars Then
            nNumPresets = oPresetData(0).Count
        End If

        For nPreset As Integer = 0 To nNumPresets - 1
            If (nPreset + nArrayOffset) >= gnMAX_ELM_NODE Then
                'Rolled over to the next path node
                nPathNode = nPathNode + 1
                sPathNode = nPathNode.ToString
                nArrayOffset = nPresetOffset - ((nPathNode - 1) * gnMAX_ELM_NODE)
                'Point the robot object vars at the new node
                oooo = DirectCast(ooo.Item(sPathNode), FRRobot.FRCVars)
                oPresets = DirectCast(oooo.Item("PRESETS"), FRRobot.FRCVars)

                oPresets.NoRefresh = True
                For nParm As Integer = 0 To rPresets.NumParms - 1
                    oPresetData(nParm) = DirectCast(oPresets(rPresets.ScreenParm(nParm).ToString), FRRobot.FRCVars)
                Next
            End If
            Dim oP As New clsPreset
            With oP
                .Index = nPreset + 1
                For nParm As Integer = 0 To rPresets.NumParms - 1
                    oVar = DirectCast(oPresetData(nParm)(, nPreset + nArrayOffset), FRRobot.FRCVar)
                    .Param(nParm).Value = CType(oVar.Value, Single)
                    .Param(nParm).MinValue = rPresets.ParameterMinValue(nParm)
                    .Param(nParm).MaxValue = rPresets.ParameterMaxValue(nParm)
                Next
            End With
            rPresets.Add(oP)
        Next

        Return True

    End Function
    Friend Function SaveOverRidesToRobot(ByRef rRobot As clsArm, _
                                                        ByRef rPresets As clsPresets) As Boolean
        '********************************************************************************************
        'Description:  Save  Override data to Robot
        '
        'Parameters: none
        'Returns:    True if save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     add support for color change presets
        ' 12/03/09  MSW     LoadOverRidesFromRobot, SaveOverRidesToRobot - Multiversion support
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing

        ' no error trap here - let calling routine catch it

        'something to save?
        If rPresets.OverRideChanged = False Then Return True
        If rPresets.ColorChangePresets Then Return True

        rRobot.ProgramName = "PAVRSYSC"
        Dim o As FRRobot.FRCVars = rRobot.ProgramVars
        Dim oo As FRRobot.FRCVars = DirectCast(o.Item("SYS_COLORS"), FRRobot.FRCVars)
        Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
        Dim oooo As FRRobot.FRCVars = _
                    DirectCast(ooo.Item(rPresets.FanucColor.ToString), FRRobot.FRCVars)
        Dim oOverrides As FRRobot.FRCVars = DirectCast(oooo.Item("PCT_OVERRIDE"), FRRobot.FRCVars)

        oOverrides.NoRefresh = True

        For nCol As Integer = 0 To rPresets.NumParms - 1

            If rPresets.OverRideChanged(nCol) Then
                Dim nOffset As Integer = 0
                If (rRobot.Controller.Version >= 6.3) and (rRobot.Controller.Version < 7.3) Then 
                    If rRobot.ArmNumber = 2 Then
                        nOffset = 6
                    End If
                End If
                'enum starts at 0 , var at 1
                oVar = DirectCast(oOverrides((rPresets.ScreenParm(nCol) + nOffset).ToString), FRRobot.FRCVar)
                oVar.Value = rPresets.OverRidePercent(nCol).Value

                ' update change log
                UpdateChangeLog(0, rPresets.OverRidePercent(nCol).Value.ToString, _
                                rPresets.OverRidePercent(nCol).OldValue.ToString, _
                                rPresets.ParameterName(nCol), _
                                rRobot.Name, rPresets.DisplayName, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)

            End If
        Next

        If rPresets.OverRideEnabled.Changed Then
            oVar = DirectCast(oOverrides(ePresetParam.OverRideEnable), FRRobot.FRCVar)
            If rPresets.OverRideEnabled.Value Then
                oVar.Value = 1
            Else
                oVar.Value = 0
            End If
            ' update change log
            UpdateChangeLog(0, rPresets.OverRideEnabled.Value.ToString, _
                             rPresets.OverRideEnabled.OldValue.ToString, _
                            String.Empty, _
                            rRobot.Name, rPresets.DisplayName, rRobot.ZoneNumber, rRobot.Controller.Zone.Name)
        End If

        oOverrides.Update()
        mPWCommon.SaveToChangeLog(frmMain.ActiveZone)

        rPresets.UpdateOverride()

        Return True

    End Function
    Friend Function SavePresetsToRobot(ByRef rRobot As clsArm, _
                                                        ByRef rPresets As clsPresets) As Boolean
        '********************************************************************************************
        'Description:  Save data to Robot
        '
        'Parameters: none
        'Returns:    True if save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/17/09  MSW     Presets by style
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim sName As String
        Dim sPathNode As String
        ' no error trap here - let calling routine catch it
        '''''''Coded up the local portion of presets by style, still needs the user interface, though
        Dim nJob As Integer = 1                 'Job we're reading presets for
        Dim nNumStyles As Integer = 1           'Number of styles allowed
        Dim bPresetsByStyle As Boolean = False  'Presets by style feature enabled on the robot
        '''''''
        Dim bV750Vars As Boolean = False        'Flag the version so we don't need to keep reading from the robot
        Dim nNumPresets As Integer = 40         'Number of presets, to support dynamic config in 7.50
        Dim nNumColors As Integer = 1           'Number of colors for preset config
        Dim nPresetOffset As Integer = 0        'position before the first preset we're getting in total nuber of presets
        Dim nPathNode As Integer = 0            'path node to read from in preset file 
        Dim nArrayOffset As Integer = 0         '1 less than the index for the preset array in path node where we start reading 
        Dim nPreset As Integer = 0              'Preset index

        'Get version config
        If rRobot.Controller.Version >= 7.5 Then
            '[PAVROPTN] byOptn[1].byOp[78]	(Presets by JOB/STYLE, 0 = NO – 1 = YES)
            rRobot.ProgramName = "PAVROPTN"
            rRobot.VariableName = "byOptn[1].byOp[78]"
            bPresetsByStyle = (CInt(rRobot.VarValue) = 1)
            '[PAVRSYSC] num_presets	(Number of presets per color)
            rRobot.ProgramName = "PAVRSYSC"
            rRobot.VariableName = "num_presets"
            nNumPresets = CInt(rRobot.VarValue)
            '[PAVRSYSC] qty_prststyl	(Number of jobs/styles)
            rRobot.VariableName = "qty_prststyl"
            nNumStyles = CInt(rRobot.VarValue)
            '[PAVRSYSC] qty_sysc_clr	(Number of colors)
            rRobot.VariableName = "qty_sys_clr"
            nNumColors = CInt(rRobot.VarValue)
            bV750Vars = True
        End If

        If bPresetsByStyle Then
            nJob = rPresets.StyleNumber
            If nJob > nNumStyles Then
                MessageBox.Show(gpsRM.GetString("psINVALID_STYLE_1") & nJob.ToString & gpsRM.GetString("psINVALID_STYLE_2") & rRobot.Controller.Name & ".", _
                                gpsRM.GetString("psINVALID_STYLE"), MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If
        End If

        If rPresets.ColorChangePresets Then
            rRobot.ProgramName = "PAPSCCHG"
            nPathNode = 0
            sPathNode = "1"
            sName = rRobot.Controller.Name
        Else
            rRobot.ProgramName = "PAPS" & rRobot.ArmNumber.ToString
            If bV750Vars Then
                'Find the offset to the beginning of the preset table for this job and color
                nPresetOffset = ((nJob - 1) * (nNumColors * nNumPresets)) + ((rPresets.FanucColor - 1) * nNumPresets)
                nPathNode = (nPresetOffset \ gnMAX_ELM_NODE) + 1  'Node that contains the start of the table
                sPathNode = nPathNode.ToString
                nArrayOffset = nPresetOffset - ((nPathNode - 1) * gnMAX_ELM_NODE) 'Preset number in the node
            Else
                nPathNode = rPresets.FanucColor
                sPathNode = nPathNode.ToString
                nArrayOffset = 0
            End If
            sName = rRobot.Name
        End If
        Dim o As FRRobot.FRCVars = rRobot.ProgramVars
        Dim oo As FRRobot.FRCVars = DirectCast(o.Item("Preset_Data"), FRRobot.FRCVars)
        Dim ooo As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
        Dim oooo As FRRobot.FRCVars = _
                        DirectCast(ooo.Item(sPathNode), FRRobot.FRCVars)
        Dim oPresets As FRRobot.FRCVars = DirectCast(oooo.Item("PRESETS"), FRRobot.FRCVars)

        oPresets.NoRefresh = True

        'enum is zero based oPresets is not
        Dim oPresetData(rPresets.NumParms - 1) As FRRobot.FRCVars
        For nParm As Integer = 0 To rPresets.NumParms - 1
            oPresetData(nParm) = DirectCast(oPresets(rPresets.ScreenParm(nParm).ToString), FRRobot.FRCVars)
        Next


        Dim oP As clsPreset

        For nPreset = 0 To rPresets.Count - 1
            If (nPreset + nArrayOffset) >= gnMAX_ELM_NODE Then
                oPresets.Update()
                'Rolled over to the next path node
                nPathNode = nPathNode + 1
                sPathNode = nPathNode.ToString
                nArrayOffset = nPresetOffset - ((nPathNode - 1) * gnMAX_ELM_NODE)
                'Point the robot object vars at the new node
                oooo = DirectCast(ooo.Item(sPathNode), FRRobot.FRCVars)
                oPresets = DirectCast(oooo.Item("PRESETS"), FRRobot.FRCVars)

                oPresets.NoRefresh = True
                For nParm As Integer = 0 To rPresets.NumParms - 1
                    oPresetData(nParm) = DirectCast(oPresets(rPresets.ScreenParm(nParm).ToString), FRRobot.FRCVars)
                Next

            End If
            oP = rPresets.Item(nPreset + 1)
            With oP
                For nParm As Integer = 0 To rPresets.NumParms - 1
                    If .Param(nParm).Changed Then
                        oVar = DirectCast(oPresetData(nParm)(, (nPreset + nArrayOffset)), FRRobot.FRCVar)
                        oVar.Value = .Param(nParm).Value
                        ' update change log
                        If bPresetsByStyle Then
                            UpdateChangeLog(oP.Index, .Param(nParm).Value.ToString, _
                                            .Param(nParm).OldValue.ToString, _
                                            rPresets.ParameterName(nParm), _
                                            sName, rPresets.DisplayName, rRobot.ZoneNumber, _
                                            rRobot.Controller.Zone.Name, rPresets.StyleNumber)
                        Else
                            UpdateChangeLog(oP.Index, .Param(nParm).Value.ToString, _
                                            .Param(nParm).OldValue.ToString, _
                                            rPresets.ParameterName(nParm), _
                                            sName, rPresets.DisplayName, rRobot.ZoneNumber, _
                                            rRobot.Controller.Zone.Name)

                        End If
                    End If
                Next

                ' this updates desc too, which should already be saved
                .Update()

            End With
        Next

        oPresets.Update()

        Return True

    End Function
    Friend Function LoadCopyScreenSubParamBox(ByRef oRobot As clsArm, ByRef clbBox As CheckedListBox, _
                                              ByVal ParamName As String, ByVal Style As Integer) As Boolean
        '********************************************************************************************
        'Description:  load preset box from preset copy screen - needs to match routine for colorchg
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Preset descriptions were always for Style 1 regardless of selected Style
        '                   for presets by Style/Color.
        ' 04/10/13  MSW     Bug fix for generic preset description.
        '********************************************************************************************
        Dim void As New DataSet
        Dim sPreset As String = gpsRM.GetString("psPRESET") & " " 'RJO 01/01/13

        Dim oPresets As clsPresets
        If meScreen = frmMain.eScreenSelect.CCPresets Then
            oPresets = oRobot.SystemColors(1, False).Presets
            oPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
            oPresets.FanucColor = 1
            oPresets.ColorChangePresets = True
        Else
            oPresets = oRobot.SystemColors(ParamName).Presets
            'have to set color
            oPresets.DisplayName = ParamName
            oPresets.FanucColor = oRobot.SystemColors(ParamName).FanucNumber

            'RJO 01/01/13 If a single style is selected, make sure we're using the proper descriptions
            If Style > 0 Then
                oPresets.StyleNumber = Style
            End If
        End If
        'it could end up adding in multiple copies.  clear it out first
        oPresets.Clear()
        If frmMain.bLoadFromGUI(oRobot.Controller.Zone, oRobot, oPresets, void) = False Then
            clbBox.Items.Clear()
            Return False
        End If

        clbBox.Items.Clear()
        clbBox.BeginUpdate()

        Select Case meScreen
            Case frmMain.eScreenSelect.FluidPresets, frmMain.eScreenSelect.CCPresets
                For Each o As clsPreset In oPresets
                    If Style = 0 Then
                        'No or multiple Styles selected. Use generic preset description. 'RJO 01/01/13
						'clbBox.Items.Add(sPreset & o.Index.ToString)
                        'MSW 4/10/13 Add the preset number at the beginning of the label
						clbBox.Items.Add(o.Index.ToString & " - " & sPreset & o.Index.ToString)
                    Else
                        clbBox.Items.Add(o.DisplayName)
                    End If
                Next
            Case frmMain.eScreenSelect.EstatPresets
                For Each o As clsPreset In oPresets
                    If Style = 0 Then
                        'No or multiple Styles selected. Use generic preset description. 'RJO 01/01/13
                        'clbBox.Items.Add(sPreset & o.Index.ToString)
                        'MSW 4/10/13 Add the preset number at the beginning of the label
						clbBox.Items.Add(o.Index.ToString & " - " & sPreset & o.Index.ToString)
                    Else
                        clbBox.Items.Add(o.EstatDisplayName)
                    End If
                Next
        End Select

        clbBox.EndUpdate()

        Return True

    End Function
    Friend Sub UpdateChangeLog(ByVal Item As Integer, ByRef NewValue As String, ByRef OldValue As String, _
                                                    ByRef ParamName As String, ByRef Device As String, _
                                                    ByRef Color As String, ByVal ZoneNumber As Integer, _
                                                    ByRef ZoneName As String, Optional ByVal nStyle As Integer = 0)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason

        ' 04/11/12  MSW     put the specific type of preset in the change log description
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        If Item > 0 Then
            ' its a preset
            sTmp = gpsRM.GetString("psPRESET")
            Select Case meScreen
                Case frmMain.eScreenSelect.CCPresets
                    sTmp = gpsRM.GetString("psCC_PRESETS")
                Case frmMain.eScreenSelect.EstatPresets
                    sTmp = gpsRM.GetString("psESTAT_PRESETS")
                Case frmMain.eScreenSelect.FluidPresets
                    sTmp = gpsRM.GetString("psPRESET")
            End Select
            sTmp = sTmp & " " & Item.ToString & " " & ParamName

        Else
            'its an override
            sTmp = ParamName & " " & gpsRM.GetString("psOVERRIDE")
        End If

        sTmp = sTmp & " " & gcsRM.GetString("csCHANGED_FROM") & " " & OldValue & " " & _
                 gcsRM.GetString("csTO") & " " & NewValue
        If nStyle > 0 Then
            sTmp = sTmp & ", " & gpsRM.GetString("psSTYLE_CAP") & " " & nStyle.ToString
        End If

        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, Color, sTmp, ZoneName)

    End Sub
    Friend Function CreatePresetDataset(ByVal AddDescColumn As Boolean, _
                                ByRef rPresets As clsPresets, ByRef rArm As clsArm, _
                                Optional ByRef sStyle As String = "") As DataSet
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
        If rPresets.ColorChangePresets Then
            DT.TableName = rArm.Controller.Name & ": " & rPresets.DisplayName
        Else
            If frmMain.mbPresetsByStyle Then
                DT.TableName = rArm.Name & ": " & rPresets.DisplayName & " - " & _
                    gpsRM.GetString("psSTYLE_CAP") & ": " & sStyle
            Else
                DT.TableName = rArm.Name & ": " & rPresets.DisplayName
            End If

        End If

        DT.Columns.Add(gsALL_INDEX)
        For nCol As Integer = 0 To rPresets.NumParms - 1
            DT.Columns.Add(rPresets.ParameterName(nCol))
        Next
        If AddDescColumn Then
            DT.Columns.Add(rPresets.ParameterName(ePresetParam.Description))
        End If

        Dim i% = 0
        For Each o As clsPreset In rPresets
            DR = DT.NewRow()
            DT.Rows.Add(DR)
            DT.Rows(i%).Item(gsALL_INDEX) = o.Index
            For nCol As Integer = 0 To rPresets.NumParms - 1
                DT.Rows(i%).Item(rPresets.ParameterName(nCol)) = _
                                                        o.Param(nCol).Value
            Next

            If AddDescColumn Then
                DT.Rows(i%).Item(rPresets.ParameterName(ePresetParam.Description)) = _
                                                    o.Description.Text
            End If

            i% += 1
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
        rCbo.Items.Clear()
        rCbo.Text = String.Empty
        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
        oStyles.LoadStyleBoxFromCollection(rCbo, False, True, colZones.ActiveZone.DegradeStyleRbtsReq, colZones.ActiveZone.NumberOfDegrades)
        msFanucStyleIndex = DirectCast(rCbo.Tag, String())
    End Function

    Friend Function ValidatePresetDatatable(ByVal MaxPresets As Integer, ByVal ValveNumber As Integer, _
          ByVal FanucNumber As Integer, ByRef dtPresets As DataTable, ByRef UpdateFile As Boolean, _
          ByVal StyleNumber As Integer) As Boolean
        '*********************************************************************************************
        'Description:  Check the preset description table to see that there are proper # 0f presets and
        '               no fields are null
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/19/12  BTK & KDJ Added Style number to ValidatePresetDatatable calls.
        '*********************************************************************************************

        Try
            Dim oRows As DataRowCollection = dtPresets.Rows()
            Dim nUB As Integer = oRows.Count - 1

            'when we recurse, the rows are only marked for deletion, need to account for this
            For i As Integer = 0 To dtPresets.Rows.Count - 1
                Dim r As DataRow = dtPresets.Rows(i)
                If r.RowState = DataRowState.Deleted Then nUB -= 1
            Next

            Select Case nUB
                Case Is > (MaxPresets - 1)
                    'whack the extras
                    For i As Integer = nUB To (MaxPresets) Step -1
                        Dim r As DataRow = dtPresets.Rows(i)
                        r.Delete()
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If ValidatePresetDatatable(MaxPresets, ValveNumber, FanucNumber, dtPresets, _
                                                                UpdateFile, StyleNumber) = False Then
                        Return False
                    End If

                Case Is < (MaxPresets - 1)
                    'Add the extras
                    For i As Integer = (nUB + 1) To (MaxPresets - 1)
                        Dim r As DataRow = dtPresets.NewRow
                        If ValidatePresetDataRow((i + 1), FanucNumber, ValveNumber, r, StyleNumber) Then
                            'changed
                            UpdateFile = True
                        End If

                        dtPresets.Rows.Add(r)

                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If ValidatePresetDatatable(MaxPresets, ValveNumber, FanucNumber, dtPresets, _
                                                                UpdateFile, StyleNumber) = False Then
                        Return False
                    End If

                Case Is = (MaxPresets - 1)
                    'this is the right number - should be 1 thru max_valves
                    'verify the data
                    For i As Integer = 0 To nUB
                        Dim r As DataRow = oRows(i)
                        If ValidatePresetDataRow((i + 1), FanucNumber, ValveNumber, r, StyleNumber) Then
                            'changed
                            UpdateFile = True
                        End If

                    Next
            End Select


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Private Function ValidatePresetDataRow(ByVal PresetNumber As Integer, _
                                            ByVal FanucNumber As Integer, _
                                            ByVal ValveNumber As Integer, _
                                            ByRef dRow As DataRow, _
                                            ByVal StyleNumber As Integer) As Boolean
        '*********************************************************************************************
        'Description:  Check datarow for integrity
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By        Reason
        ' 10/19/12  BTK & KDJ Added Style number argument. It was hard coded to 1.
        '*********************************************************************************************
        Dim bChanged As Boolean = False

        dRow.BeginEdit()

        If dRow.IsNull(gsFANUC_COLOR_COL) Then
            bChanged = True
            dRow(gsFANUC_COLOR_COL) = FanucNumber
        End If

        If CType(dRow(gsFANUC_COLOR_COL), Integer) <> FanucNumber Then
            bChanged = True
            dRow(gsFANUC_COLOR_COL) = FanucNumber
        End If

        'check for nulls
        If dRow.IsNull(gsPRESET_COL) Then
            bChanged = True
            dRow(gsPRESET_COL) = PresetNumber
        End If
        If CType(dRow(gsPRESET_COL), Integer) <> PresetNumber Then
            bChanged = True
            dRow(gsPRESET_COL) = PresetNumber
        End If

        If dRow.IsNull(gsDESC_COL) Then
            bChanged = True
            dRow(gsDESC_COL) = gcsRM.GetString("csNO_DESCRIPTION")
        End If

        If dRow.IsNull(gsESTATDESC_COL) Then
            bChanged = True
            dRow(gsESTATDESC_COL) = gcsRM.GetString("csNO_DESCRIPTION")
        End If

        If dRow.IsNull(gsVALVE_COL) Then
            bChanged = True
            dRow(gsVALVE_COL) = ValveNumber
        End If
        If CType(dRow(gsVALVE_COL), Integer) <> ValveNumber Then
            bChanged = True
            dRow(gsVALVE_COL) = ValveNumber
        End If

        'BTK & KDJ 10/19/12 Added Style number argument. It was hard coded to 1.
        If dRow.IsNull(gsFANUC_STYLE_COL) Then
            bChanged = True
            dRow(gsFANUC_STYLE_COL) = StyleNumber
        End If

        If bChanged Then
            dRow.EndEdit()
        Else
            dRow.CancelEdit()
        End If

        Return bChanged

    End Function
    Friend Function GetPresetDataset(ByRef oDB As clsSQLAccess, ByVal sTableName As String, _
                                     ByVal FanucColor As Integer, Optional ByVal nStyle As Integer = 1, _
                                     Optional ByVal nMaxPreset As Integer = 40, _
                                     Optional ByVal nMaxColor As Integer = 24, _
                                     Optional ByVal nMaxStyle As Integer = 1) As DataSet
        '********************************************************************************************
        'Description: This Routine gets a dataset for the requested table of preset info
        '
        'Parameters: a clsSQLAccess pointing to the proper place, name of table of interest 
        'Returns:    Dataset or nothing if problem
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/17/09  MSW     add presets by style
        ' 07/28/10  MSW     Hide error message when creating the database table
        ' 08/23/10  MSW     GetPresetDataset - This still needed some work to get the table loaded
        '10/19.12   BTK&KDJ Added code to generate all presets for each color and style
        '                   when a new robot is selected only when there is no database table.
        '                   Changed nStyle = 1 by default for presets by color not style.
        '********************************************************************************************
        Dim DT As New DataTable
        Dim bUpdate As Boolean

        Try
            With oDB
                .DBTableName = sTableName
                If nStyle > 0 Then
                    .SQLString = "SELECT * FROM [" & sTableName & "] AS p" & _
                                " WHERE [p].[" & gsFANUC_COLOR_COL & "] =" & _
                                FanucColor & " AND [p].[" & gsFANUC_STYLE_COL & "] =" & _
                                nStyle.ToString & " " & _
                                "ORDER BY [p].[" & gsPRESET_COL & "] ASC"
                Else
                    .SQLString = "SELECT * FROM [" & sTableName & "] AS p" & _
                                " WHERE [p].[" & gsFANUC_COLOR_COL & "] =" & _
                                FanucColor & " AND [p].[" & gsFANUC_STYLE_COL & "] =1 " & _
                                "ORDER BY [p].[" & gsPRESET_COL & "] ASC"

                End If

                Dim DS As New DataSet
                DS.Locale = mLanguage.FixedCulture

                Try

                    'this is to catch missing table
                    DS = .GetDataSet(False, , True)

                Catch ex As SqlClient.SqlException

                    Select Case ex.ErrorCode
                        Case -2146232060
                            'missing table (invalid object name)
                            'call stored procedure to create table
                            Dim s As SqlClient.SqlCommand = .GetStoredProcedureCommand("CreatePresetTable")
                            s.Parameters.Add(New SqlClient.SqlParameter("@TableName", _
                                SqlDbType.NVarChar)).Value = sTableName
                            Dim returnrows As Integer = -1
                            'MSW 8/23/10 This still needed some work to get the table loaded
                            Try
                                returnrows = s.ExecuteNonQuery()
                            Catch ex2 As Exception
                                returnrows = -1
                            End Try
                            'do it

                            If returnrows = -1 Then
                                'success

                                'this should be a developer only thing - no localization
                                MessageBox.Show("Created table '" & sTableName & "' in database ")

                                'BTK & KDJ 10/19.12 Added code to generate all presets for each color and style
                                '          when a new robot is selected only when there is no database table.

                                'Need to force SQL color and style to zero when updated all records.
                                .SQLString = "SELECT * FROM [" & sTableName & "] AS p" & _
                                " WHERE [p].[" & gsFANUC_COLOR_COL & "] =" & _
                                0 & " AND [p].[" & gsFANUC_STYLE_COL & "] =" & _
                                0 & " " & _
                                "ORDER BY [p].[" & gsPRESET_COL & "] ASC"

                                For iStyle As Integer = 1 To nMaxStyle
                                    For iColor As Integer = 1 To nMaxColor
                                        DS = .GetDataSet(False)
                                        DT = DS.Tables("[" & sTableName & "]")

                                        ValidatePresetDatatable(nMaxPreset, iColor, iColor, DT, bUpdate, iStyle)

                                        If bUpdate Then
                                            .UpdateDataSet(DS, sTableName)
                                        End If

                                    Next 'iColor
                                Next 'iStyle

                                'Once updating all records return the data for the selected style and color.
                                If nStyle > 0 Then
                                    .SQLString = "SELECT * FROM [" & sTableName & "] AS p" & _
                                                " WHERE [p].[" & gsFANUC_COLOR_COL & "] =" & _
                                                FanucColor & " AND [p].[" & gsFANUC_STYLE_COL & "] =" & _
                                                nStyle.ToString & " " & _
                                                "ORDER BY [p].[" & gsPRESET_COL & "] ASC"
                                Else
                                    .SQLString = "SELECT * FROM [" & sTableName & "] AS p" & _
                                                " WHERE [p].[" & gsFANUC_COLOR_COL & "] =" & _
                                                FanucColor & " AND [p].[" & gsFANUC_STYLE_COL & "] =1 " & _
                                                "ORDER BY [p].[" & gsPRESET_COL & "] ASC"

                                End If
                                DS = .GetDataSet(False)
                            Else
                                Return Nothing
                            End If
                        Case Else
                            Return Nothing
                    End Select

                End Try


                If DS.Tables.Contains("[" & sTableName & "]") Then
                    Return DS
                Else
                    Return Nothing
                End If

            End With


        Catch ex As Exception
            Return Nothing
        End Try

    End Function


#End Region
#Region " Properties "

#End Region

End Module
