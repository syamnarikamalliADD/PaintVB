' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2014
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsZDT.vb
'
' Description: data management for ZDT features on the BSD
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
'    04/15/14   MSW     ZDT Changes                                                   4.01.07.00
'    05/16/14   MSW     Fairfax updates                                               4.01.07.01
'    07/30/14   MSW     Data Reset and archive updates                                4.01.07.02
'    08/25/14   MSW     Improve cleanup to clear out extra logs                       4.01.07.03
'    10/10/14   MSW     subDisplayGrid - Clean up some wasteful code                  4.01.07.04
'********************************************************************************************
Imports System.IO
Imports System.Windows.Forms.Application
Imports clsPLCComm = PLCComm.clsPLCComm
Imports System.Xml
Imports System.Xml.XPath
Friend Module mZDT
    Private Const msMODULE As String = "mZDT"
    Friend mnNumTests As Integer = 32

    Friend Sub subCleanupLogFiles(ByRef sPath As String, ByRef nDaysToKeep As Integer)
        '********************************************************************************************
        'Description: clean up a log folder
        'Parameters: 
        '   sPath - log folder to clean up
        '   nDaysToKeep - How far back should it start deleting
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/14  MSW     Improve cleanup to clear out extra logs
        '******************************************************************************************** 
        Try
            'Debug.Print(sPath)
            Dim sDirs() As String = System.IO.Directory.GetDirectories(sPath, "????_??_??*")
            Array.Sort(sDirs)
            'Reformatting dates as numbers for easier math
            'Old enough to delete by Days To Keep setting
            Dim dtTooOld As DateTime = Now.AddDays(-1 * nDaysToKeep)
            Dim nTooOldYear As Integer = dtTooOld.Year
            Dim nTooOldMonth As Integer = dtTooOld.Month
            Dim nTooOldDay As Integer = dtTooOld.Day
            Dim nNumericTooOld As Integer = dtTooOld.Day + 100 * dtTooOld.Month + 10000 * dtTooOld.Year
            'A couple of days ago - Old enough to cleanup folders that are close together.
            Dim dtOldEnough As DateTime = Now.AddDays(-2)
            Dim nNumOldEnough As Integer = dtOldEnough.Day + 100 * dtOldEnough.Month + 10000 * dtOldEnough.Year
            'Variabls used in the loop to build the date for a specific folder
            Dim sParts1() As String = Nothing
            Dim sParts() As String = Nothing
            Dim nYear As Integer = 0
            Dim nMonth As Integer = 0
            Dim nDay As Integer = 0
            Dim nHourMin As Integer = 0
            Dim nNumDate As Long = 0
            'Track the last folder we looked at.
            Dim sPrev As String = String.Empty
            Dim nPrevDate As Long = 0
            Dim nPrevPrevDate As Long = 0
            For Each sDir As String In sDirs
                'Build a numeric date for this folder
                sParts1 = Split(sDir, "\")
                sParts = Split(sParts1(sParts1.GetUpperBound(0)), "_")
                nYear = CType(sParts(0), Integer)
                nMonth = CType(sParts(1), Integer)
                nDay = CType(sParts(2), Integer)
                nNumDate = nDay + 100 * nMonth + 10000 * nYear
                If (nNumDate < nNumericTooOld) Then
                    'Older than DaysToKeep, get rid of it
                    IO.Directory.Delete(sDir, True)
                    nPrevDate = 0
                    nPrevPrevDate = 0
                ElseIf (sParts.GetUpperBound(0) > 2) AndAlso (nNumDate < nNumOldEnough) Then
                    'Not that old, but old enough for some cleanup
                    'see if there are a bunch close together and get rid of some of them.
                    nNumDate = 10000 * nNumDate + CType(sParts(3), Integer)
                    If ((nNumDate - nPrevDate) < 300) AndAlso ((nPrevDate - nPrevPrevDate) < 300) Then
                        'Three folder less than 3 hours from each other.  Get rid of the last one.
                        If IO.Directory.Exists(sPrev) Then
                            IO.Directory.Delete(sPrev, True)
                        End If
                        'kept the first folder, only update the new one
                        nPrevDate = nNumDate
                        sPrev = sDir
                    Else
                        'Not close together, update the previous folder variables
                        nPrevPrevDate = nPrevDate
                        nPrevDate = nNumDate
                        sPrev = sDir
                    End If
                End If
            Next 'sDir
            If IO.File.Exists(sPath & "diag.xml") Then
                IO.File.Delete(sPath & "diag.xml")
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subCleanupLogFiles:" & sPath, _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Friend Sub subImportResults(ByRef colZDTArms As Collection, ByRef sFile As String, ByRef oTest As clsZDTTest, _
                                Optional ByRef bLookupText As Boolean = False)
        '********************************************************************************************
        'Description: read the last set of results from the output file
        '
        'Parameters: 
        '   colZDTArms - colection to load results into
        '   sFile - results file
        '   nTestIndex - index of test to update
        '   bLookupText - lookup the text in the resx file
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Dim sr As System.IO.StreamReader = Nothing
        If IO.File.Exists(sFile) Then
            'Status Format : RC_1,groupnum,nResult,sResult,sColor
            Try
                sr = System.IO.File.OpenText(sFile)
                Do While sr.Peek() >= 0
                    Dim sLine As String = sr.ReadLine()
                    Dim sColumns() As String = Split(sLine, ",")
                    If sColumns.GetUpperBound(0) >= 4 Then
                        For Each oArm As clsZDTArm In colZDTArms
                            If oArm.Enabled(oTest.nIndex) AndAlso _
                                oArm.sCntr = sColumns(0) AndAlso oArm.nGroup = CInt(sColumns(1)) Then
                                oArm.PCNumStatus(oTest.nIndex) = CInt(sColumns(2))
                                oArm.FromFile(oTest.nIndex) = (oArm.PCNumStatus(oTest.nIndex) <> 0)
                                Dim sTmp As String = sColumns(3)
                                If bLookupText Then
                                    sTmp = gpsRM.GetString(sColumns(3))
                                    If (sTmp Is Nothing) OrElse (sTmp = String.Empty) Then
                                        sTmp = sColumns(3)
                                    End If
                                End If
                                oArm.PCStatus(oTest.nIndex) = sTmp
                                oArm.PCColor(oTest.nIndex) = sColumns(4)
                            End If
                        Next
                    End If
                Loop
                sr.Close()
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subImportResults", _
                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End If

    End Sub
    Friend Sub subWriteResults(ByRef colZDTArms As Collection, ByRef sOutputFile As String, ByRef oTest As clsZDTTest)
        '********************************************************************************************
        'Description: Write results to output file 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim FileWriter As System.IO.StreamWriter = Nothing
        Try
            If IO.File.Exists(sOutputFile) Then
                IO.File.Delete(sOutputFile)
            End If
            FileWriter = New System.IO.StreamWriter(sOutputFile)

            For Each oArm As clsZDTArm In colZDTArms
                If oArm.Enabled(oTest.nIndex) Then
                    FileWriter.WriteLine(oArm.sCntr & "," & oArm.nGroup.ToString & "," & oArm.PCNumStatus(oTest.nIndex) & "," & oArm.PCStatus(oTest.nIndex) & "," & oArm.PCColor(oTest.nIndex))
                End If
            Next
            FileWriter.Close()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subWriteResults", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Friend Sub subZipResults(ByRef sFileName As String)
        '********************************************************************************************
        'Description:  zip up the results   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sZipUtil As String = String.Empty
        Dim sZDTPath As String = String.Empty
        Try
            If mPWCommon.GetDefaultFilePath(sZDTPath, eDir.ZDT, String.Empty, String.Empty) Then
            Else

            End If

            If Not (GetDefaultFilePath(sZipUtil, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_FILENAME)) Then
                sZipUtil = String.Empty
            Else
                If IO.File.Exists(sFileName) Then
                    IO.File.Delete(sFileName)
                End If
                Dim sCmd As String = sZipUtil & " " & gs_ZIP_ALL & " " & sZDTPath & "  " & """" & sFileName & """"
                'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                Shell(sCmd, AppWinStyle.Hide, True, 30000)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subZipResults", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

End Module
Friend Class clsZDTArm
    Private Const msMODULE As String = "clsZDTArm"
    Friend sArm As String
    Friend sCntr As String
    Friend nGroup As Integer
    Friend PLCStatus As Boolean
    Private bTestEnabled(mnNumTests - 1) As Boolean
    Private sRobotCfgDesc(mnNumTests - 1) As String
    Private nStatus(mnNumTests - 1) As Integer
    Private sStatus(mnNumTests - 1) As String
    Private sColor(mnNumTests - 1) As String
    Private nPCStatus(mnNumTests - 1) As Integer
    Private sPCStatus(mnNumTests - 1) As String
    Private sPCColor(mnNumTests - 1) As String
    Private bFromFile(mnNumTests - 1) As Boolean
    Friend dtDateTime(mnNumTests - 1) As DateTime
    Private nCfgFromRobot As Integer = 0
    Private Const nCfgFromRobotNotUsed As Integer = 0
    Private Const nCfgFromRobotNeeded As Integer = 1
    Private Const nCfgFromRobotLoaded As Integer = 2
    Friend nArmIdx As Integer
    Friend FetchData As Boolean
    Friend Const msGRAY As String = "Gray"
    Private moArm As clsArm
    Private moRobot As FRRobot.FRCRobot
    Private Const msZDTCFGPROG As String = "PAVRZDT"
    Private Const msZDTCFGVAR As String = "ZDT_CONF"
    Dim oStatusArray As FRRobot.FRCVars = Nothing
    Private mbChanged As Boolean = False
    Friend Property Changed() As Boolean
        Get
            Return mbChanged
        End Get
        Set(ByVal value As Boolean)
            mbChanged = value
        End Set
    End Property

    Friend Sub subDebugPrint()
        Debug.Print("sArm: " & sArm & ", sCntr: " & sCntr & ", nGroup: " & nGroup.ToString & ", nArmIdx: " & nArmIdx.ToString)
        Select Case nCfgFromRobot
            Case nCfgFromRobotNotUsed
                Debug.Print("   nCfgFromRobotNotUsed")
            Case nCfgFromRobotNeeded
                Debug.Print("   nCfgFromRobotNeeded")
            Case nCfgFromRobotLoaded
                Debug.Print("   nCfgFromRobotLoaded")
        End Select
        For nItem As Integer = 0 To mnNumTests - 1
            Debug.Print("   " & nItem.ToString("00") & " - " & sRobotCfgDesc(nItem) & "  Enabled: " & bTestEnabled(nItem).ToString)
        Next
    End Sub
    Friend Property RobotCfgDesc(ByVal nIndex As Integer) As String
        Get
            If nCfgFromRobot = nCfgFromRobotNeeded Then
                subGetConfigFromRobot()
            End If
            Return sRobotCfgDesc(nIndex)
        End Get
        Set(ByVal value As String)
            sRobotCfgDesc(nIndex) = value
        End Set
    End Property
    Friend Property Enabled(ByVal nIndex As Integer) As Boolean
        Get
            If nCfgFromRobot = nCfgFromRobotNeeded Then
                subGetConfigFromRobot()
            End If
            Return bTestEnabled(nIndex)
        End Get
        Set(ByVal value As Boolean)
            bTestEnabled(nIndex) = value
        End Set
    End Property
    Friend ReadOnly Property AnyTestEnabled() As Boolean
        Get
            Dim bEnabled As Boolean = False
            For nTestNo As Integer = 0 To mnNumTests - 1
                If bTestEnabled(nTestNo) Then
                    bEnabled = True
                    Exit For
                End If
            Next
            Return bEnabled
        End Get
    End Property
    Friend Property FromFile(ByVal nIndex As Integer) As Boolean
        Get
            Return bFromFile(nIndex)
        End Get
        Set(ByVal value As Boolean)
            bFromFile(nIndex) = value
        End Set
    End Property
    Friend Property NumStatus(ByVal nIndex As Integer) As Integer
        Get
            If bTestEnabled(nIndex) Then
                Return nStatus(nIndex)
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Integer)
            If bTestEnabled(nIndex) Then
                nStatus(nIndex) = value
            Else
                nStatus(nIndex) = -1
            End If
        End Set
    End Property
    Friend Property PCNumStatus(ByVal nIndex As Integer) As Integer
        Get
            If bTestEnabled(nIndex) Then
                Return nPCStatus(nIndex)
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Integer)
            If bTestEnabled(nIndex) Then
                nPCStatus(nIndex) = value
            Else
                nPCStatus(nIndex) = -1
            End If
        End Set
    End Property
    Friend Property Status(ByVal nIndex As Integer) As String
        Get
            If bTestEnabled(nIndex) Then
                Return sStatus(nIndex)
            Else
                Return gpsRM.GetString("psNA")
            End If
        End Get
        Set(ByVal value As String)
            If bTestEnabled(nIndex) Then
                sStatus(nIndex) = value
            Else
                sStatus(nIndex) = gpsRM.GetString("psNA")
            End If
        End Set
    End Property

    Friend Property PCStatus(ByVal nIndex As Integer) As String
        Get
            If bTestEnabled(nIndex) Then
                Return sPCStatus(nIndex)
            Else
                Return gpsRM.GetString("psNA")
            End If
        End Get
        Set(ByVal value As String)
            If bTestEnabled(nIndex) Then
                sPCStatus(nIndex) = value
            Else
                sPCStatus(nIndex) = gpsRM.GetString("psNA")
            End If
        End Set
    End Property
    Friend Property Color(ByVal nIndex As Integer) As String
        Get
            If bTestEnabled(nIndex) Then
                Return sColor(nIndex)
            Else
                Return gpsRM.GetString("psNA")
            End If
        End Get
        Set(ByVal value As String)
            If bTestEnabled(nIndex) Then
                sColor(nIndex) = value
            Else
                sColor(nIndex) = msGRAY
            End If
        End Set
    End Property
    Friend Property PCColor(ByVal nIndex As Integer) As String
        Get
            If bTestEnabled(nIndex) Then
                Return sPCColor(nIndex)
            Else
                Return gpsRM.GetString("psNA")
            End If
        End Get
        Set(ByVal value As String)
            If bTestEnabled(nIndex) Then
                sPCColor(nIndex) = value
            Else
                sPCColor(nIndex) = msGRAY
            End If
        End Set
    End Property
    Friend Sub subGetStatusFromRobot(ByRef sStatus As String, ByRef sColor As String, ByRef oColStatus As Collection)
        '********************************************************************************************
        'Description: read ZDT status var from robot
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If moArm.Controller.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
                If oStatusArray Is Nothing Then
                    Dim oList As FRRobot.FRCPrograms = moRobot.Programs
                    Dim oProg As FRRobot.FRCProgram = CType(oList.Item(msZDTCFGPROG), FRRobot.FRCProgram)
                    Dim oOuterArray As FRRobot.FRCVars = CType(oProg.Variables("ZDT_ITM_STAT"), FRRobot.FRCVars)
                    Dim oMidArray As FRRobot.FRCVars = CType(oOuterArray.Item(nGroup.ToString), FRRobot.FRCVars)
                    oStatusArray = CType(oMidArray.Item("ELEM"), FRRobot.FRCVars)
                End If
                oStatusArray.Refresh()
                Dim nMax As Integer = mnNumTests
                If oStatusArray.Count < nMax Then
                    nMax = oStatusArray.Count
                End If
                For nTestNo As Integer = 1 To nMax
                    If Enabled(nTestNo - 1) Then
                        Dim oStruct As FRRobot.FRCVars = CType(oStatusArray.Item(nTestNo.ToString), FRRobot.FRCVars)
                        Dim oSevVar As FRRobot.FRCVar = CType(oStruct.Item("ISEVERITY"), FRRobot.FRCVar)
                        Dim nStatus As Integer = CType(oSevVar.Value, Integer)

                        If nStatus <> 0 Then
                            If NumStatus(nTestNo - 1) <> nStatus Then
                                mbChanged = True
                            End If
                            Status(nTestNo - 1) = sRobotCfgDesc(nTestNo - 1)
                            Color(nTestNo - 1) = "Yellow"
                            sStatus = Status(nTestNo - 1)
                            sColor = Color(nTestNo - 1)
                            oColStatus.Add(sArm & ": " & sStatus)
                        Else
                            Status(nTestNo - 1) = gpsRM.GetString("psZDT_ARM_OK")
                            Color(nTestNo - 1) = "Green"
                        End If
                        NumStatus(nTestNo - 1) = nStatus
                    End If
                Next
                nCfgFromRobot = nCfgFromRobotLoaded
            Else
                oStatusArray = Nothing
                nCfgFromRobot = nCfgFromRobotNeeded
            End If
        Catch ex As Exception
            oStatusArray = Nothing
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetStatusFromRobot", _
                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Friend Sub subGetConfigFromRobot()
        '********************************************************************************************
        'Description: read ZDT config from robot
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If moArm.Controller.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
                Dim oList As FRRobot.FRCPrograms = moRobot.Programs
                Dim oProg As FRRobot.FRCProgram = CType(oList.Item(msZDTCFGPROG), FRRobot.FRCProgram)
                Dim oArray As FRRobot.FRCVars = CType(oProg.Variables(msZDTCFGVAR), FRRobot.FRCVars)
                oArray.Refresh()
                Dim nMax As Integer = mnNumTests
                If oArray.Count < nMax Then
                    nMax = oArray.Count
                End If
                For nTestNo As Integer = 1 To nMax
                    If bTestEnabled(nTestNo - 1) Then
                    Dim oStruct As FRRobot.FRCVars = CType(oArray.Item(nTestNo.ToString), FRRobot.FRCVars)
                    Dim oDescVar As FRRobot.FRCVar = CType(oStruct.Item("SDESC"), FRRobot.FRCVar)
                    Dim oEnabVar As FRRobot.FRCVar = CType(oStruct.Item("BENABLE"), FRRobot.FRCVar)
                    If oEnabVar.IsInitialized Then
                        bTestEnabled(nTestNo - 1) = bTestEnabled(nTestNo - 1) And CType(oEnabVar.Value, Boolean)
                    Else
                        bTestEnabled(nTestNo - 1) = False
                    End If
                    If oDescVar.IsInitialized Then
                    sRobotCfgDesc(nTestNo - 1) = CType(oDescVar.Value, String)
                    Else
                        sRobotCfgDesc(nTestNo - 1) = String.Empty
                    End If
                    End If
                Next
                nCfgFromRobot = nCfgFromRobotLoaded
            Else
                nCfgFromRobot = nCfgFromRobotNeeded
            End If
        Catch ex As Exception
            If ex.Message <> "Program does not exist" Then
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetConfigFromRobot", _
                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End If
            For nTestNo As Integer = 1 To mnNumTests
                bTestEnabled(nTestNo - 1) = False
                sRobotCfgDesc(nTestNo - 1) = String.Empty
            Next
            nCfgFromRobot = nCfgFromRobotNotUsed
        End Try
    End Sub

    Friend Sub New(ByRef oZone As clsZone, ByRef oArm As clsArm, ByRef oZDTConfig As clsZDTConfig, _
                   Optional ByRef bConnect As Boolean = False)
        '********************************************************************************************
        'Description: New ZDT arm class  - holds status data for one arm
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bEnable As Boolean = False
        Try
            moArm = oArm
            sArm = oArm.Name
            sCntr = oArm.Controller.FanucName
            nGroup = oArm.ArmNumber
            moRobot = oArm.Controller.Robot
            nArmIdx = oArm.RobotNumber
            FetchData = False
            nCfgFromRobot = nCfgFromRobotNotUsed
            PLCStatus = False
            For nTest As Integer = 0 To mnNumTests - 1
                sStatus(nTest) = gpsRM.GetString("psNA")
                nStatus(nTest) = -1
                sColor(nTest) = msGRAY
                bTestEnabled(nTest) = False
                bFromFile(nTest) = False
                sRobotCfgDesc(nTest) = String.Empty
                If oZDTConfig.colZDTTests IsNot Nothing Then
                    For Each oTest As clsZDTTest In oZDTConfig.colZDTTests
                        If oTest.nIndex = nTest Then
                            For Each sTmp As String In oTest.colArms
                                If sTmp = sArm Then
                                    bTestEnabled(nTest) = True
                                    bEnable = True
                                    Exit For
                                End If
                            Next
                            Exit For
                        End If
                    Next
                End If
            Next
            If bConnect And bEnable Then
                nCfgFromRobot = nCfgFromRobotNeeded
                subGetConfigFromRobot()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
Friend Class clsZDTTest
    Private Const msMODULE As String = "clsZDTTest"
    Friend sName As String
    Friend nIndex As Integer
    Friend sArms As String
    Friend colArms As Collection
    Friend sFolder As String
    Friend sGetFile As String
    Friend sCallProg As String
    Friend nDaysToKeep As Integer = 365

    Friend Sub New(ByRef oNode As XmlNode)
        '********************************************************************************************
        'Description: New ZDT test class  - holds status data for one test
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' <ZDTTest>
        '   <Name>J1</Name>
        '   <Index>0</Index>
        '   <Arms>P1,P2</Arms>
        '   <Folder>J1</Folder>
        '   <GetFile>pavrgrg#.va</GetFile>
        '   <CallProg>%ProgramFiles%\FANUC\GearDiag\GearDiag.exe</CallProg>
        '   <ReadVars>False<ReadVars>
        '</ZDTTest>
        Try

            Dim sCNTR As String = oNode.Item("Name").InnerXml
            sName = oNode.Item("Name").InnerXml

            Try
                sName = oNode.Item("Name").InnerXml
            Catch ex As Exception
                sName = String.Empty
            End Try
            Try
                nIndex = CType(oNode.Item("Index").InnerXml, Integer)
            Catch ex As Exception
                nIndex = 0
            End Try
            Try
                sArms = oNode.Item("Arms").InnerXml
                Dim sArmsSplit() As String = Split(sArms, ",")
                colArms = New Collection
                For Each sArm As String In sArmsSplit
                    colArms.Add(sArm.Trim)
                Next
            Catch ex As Exception
                sArms = String.Empty
            End Try
            Try
                sFolder = oNode.Item("Folder").InnerXml
            Catch ex As Exception
                sFolder = String.Empty
            End Try
            Try
                sGetFile = oNode.Item("GetFile").InnerXml
            Catch ex As Exception
                sGetFile = String.Empty
            End Try
            Try
                sCallProg = oNode.Item("CallProg").InnerXml
            Catch ex As Exception
                sCallProg = String.Empty
            End Try
            Try
                nDaysToKeep = CType(oNode.Item("DaysToKeep").InnerXml, Integer)
            Catch ex As Exception
                nDaysToKeep = 365
            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

End Class
Friend Class clsZDTFileLog
    Private Const msMODULE As String = "clsZDTFileLog"
    Friend msCntr As String = String.Empty
    Friend msFiles As String() = Nothing
    Friend mnPLCBitMask As Integer = 0
    Friend mbGetFilesActive As Boolean = False
    Friend mnDaysToKeep As Integer = 365
    Friend WithEvents mbgFileLogWorker As New System.ComponentModel.BackgroundWorker

    Friend Sub New(ByRef sCntr As String, ByRef nDaysToKeep As Integer)
        '********************************************************************************************
        'Description: New
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/14  MSW     Improve cleanup to clear out extra logs
        '********************************************************************************************
        msCntr = sCntr
        mnDaysToKeep = nDaysToKeep
    End Sub
    Friend Sub subGetFilesBG(ByRef sLogPath As String)
        '********************************************************************************************
        'Description: Get files from the controller
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbGetFilesActive = True
        mbgFileLogWorker.RunWorkerAsync(sLogPath)
    End Sub

    Private Sub mbgFileLogWorker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles mbgFileLogWorker.DoWork
        '********************************************************************************************
        'Description: Get files from the controller
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subGetFiles(e.Argument.ToString)
        e.Result = True
    End Sub
    Friend Sub subGetFiles(ByRef sLogPath As String)
        '********************************************************************************************
        'Description: Get files from the controller
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            Dim sCntrPath As String = sLogPath.ToString & msCntr
            If (IO.Directory.Exists(sCntrPath) = False) Then
                IO.Directory.CreateDirectory(sCntrPath)
            End If
            Dim oFTP As clsFSFtp
            Try
                oFTP = New clsFSFtp(msCntr)
                Try
                    Dim sDateFolder As String = sCntrPath & "\" & Date.Now.Year.ToString("0000") & "_" & Date.Now.Month.ToString("00") & "_" & Date.Now.Day.ToString("00") & "_" & Date.Now.Hour.ToString("00") & Date.Now.Minute.ToString("00")
                    If (IO.Directory.Exists(sDateFolder) = False) Then
                        IO.Directory.CreateDirectory(sDateFolder)
                    End If
                    With oFTP
                        If .Connected Then
                            .DestDir = sDateFolder
                            For Each sFileSpec As String In msFiles
                                Try
                                    Dim sFile As String = String.Empty
                                    If sFileSpec.Contains(":") Then
                                        Dim sSplit() As String = Split(sFileSpec, ":")
                                        .WorkingDir = sSplit(0) & ":"
                                        sFile = sSplit(1)
                                    Else
                                        .WorkingDir = "MD:"
                                        sFile = sFileSpec
                                    End If
                                    If .GetFile(sFile, sFile) Then
                                        'Debug.Print(msCntr & ":  Got " & sFile)
                                    Else
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                                   "ERROR: Could not get file: " & msCntr & "\" & sFileSpec & " to " & sDateFolder & "   -   FTP ERROR: " & .ErrorMsg)
                                    End If
                                Catch ex As Exception

                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                               "ERROR: Could not get file: " & msCntr & "\" & sFileSpec & " to " & sDateFolder & "   -   Error: " & ex.Message & vbCrLf & ex.StackTrace)
                                End Try
                            Next
                        End If
                    End With
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                Finally
                    oFTP.Close()
                End Try
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            Finally
                oFTP = Nothing
            End Try
                Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub mbgFileLogWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles mbgFileLogWorker.RunWorkerCompleted
        '********************************************************************************************
        'Description: Done getting files from the controller
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbGetFilesActive = False
    End Sub
End Class
Friend Class clsZDTConfig
    Private Const msMODULE As String = "clsZDTConfig"
    Friend colZDTTests As Collection
    Friend colFileLog As Collection
    Friend mnLogDaysToKeep As Integer = 365
    Friend mnPLCBitMask As Integer = 0
    Friend msZDTPath As String = String.Empty
    Friend msLogFilePath As String = String.Empty

    Friend Sub subGetLogFiles(ByRef sLogPath As String)
        '********************************************************************************************
        'Description: read files from the robot controllers
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/14  MSW     Improve cleanup to clear out extra logs
        '******************************************************************************************** 
        Dim oStart As DateTime = Now
        Try
            For Each oFileLog As clsZDTFileLog In colFileLog
                Try
                    oFileLog.subGetFiles(sLogPath)
                    subCleanupLogFiles(sLogPath.ToString & oFileLog.msCntr, oFileLog.mnDaysToKeep)
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetLogFiles", _
                               "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Next
            Dim bNotDone As Boolean = True
            Do While bNotDone
                bNotDone = False
                Application.DoEvents()
                For Each oFileLog As clsZDTFileLog In colFileLog
                    If oFileLog.mbgFileLogWorker.IsBusy Then
                        bNotDone = True
                    End If
                Next
            Loop
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetLogFiles", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        'Debug.Print("Fetch Files" & (Now - oStart).ToString)
    End Sub
    Friend Sub New(ByRef oZone As clsZone, ByRef oArms As clsArms, _
                    Optional ByVal bLoggerOnly As Boolean = False)
        '********************************************************************************************
        'Description: read test configs from XML file 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/14  MSW     Improve cleanup to clear out extra logs
        '********************************************************************************************
        Try
            If mPWCommon.GetDefaultFilePath(msZDTPath, eDir.ZDT, String.Empty, String.Empty) Then
                If (IO.Directory.Exists(msZDTPath) = False) Then
                    IO.Directory.CreateDirectory(msZDTPath)
                End If
                msLogFilePath = msZDTPath & "Log\"
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Const sFILENAME As String = "ZDTConfig"
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sFILENAME & ".XML") Then
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & "ZDTTests")
                oNodeList = oMainNode.SelectNodes("//" & "ZDTTest")
                If oNodeList.Count > 0 Then
                    colZDTTests = New Collection
                    For Each oNode As XmlNode In oNodeList
                        Dim oZDTTest As New clsZDTTest(oNode)
                        If oZDTTest.sName <> String.Empty AndAlso ((bLoggerOnly = False) Or (oZDTTest.sFolder <> String.Empty)) Then
                            colZDTTests.Add(oZDTTest)
                        End If
                    Next
                End If
                'Load file logging details

                oNodeList = oMainNode.SelectNodes("//" & "FileLog")
                If oNodeList.Count > 0 Then
                    Dim sByArm As String = String.Empty
                    Dim sByController As String = String.Empty
                    Dim sController As String = String.Empty
                    Dim nBit As Integer = 0
                    colFileLog = New Collection
                    For Each oNode As XmlNode In oNodeList
                        Try
                            sController = oNode.Item("Controller").InnerXml
                        Catch ex As Exception
                            sController = String.Empty
                        End Try
                        Try
                            nBit = CType(oNode.Item("PLCBit").InnerXml, Integer)
                        Catch ex As Exception
                            nBit = 0
                        End Try
                        Try
                            sByArm = oNode.Item("ByArm").InnerXml
                        Catch ex As Exception
                            sByArm = String.Empty
                        End Try
                        Try
                            sByController = oNode.Item("ByController").InnerXml
                        Catch ex As Exception
                            sByController = String.Empty
                        End Try
                        Try
                            mnLogDaysToKeep = CType(oNode.Item("DaysToKeep").InnerXml, Integer)
                        Catch ex As Exception
                            mnLogDaysToKeep = 365
                        End Try
                    'build up a collection of files to log per controller based on arm configs
                    Dim sFilesByArm As String() = Split(sByArm, ",")
                    Dim sFilesByController As String() = Split(sByController, ",")
                    For Each oArm As clsArm In oArms
                        If oArm.Controller.FanucName = sController Then
                            Dim bAddController As Boolean = True
                            Dim oFileLog As clsZDTFileLog = Nothing
                            For Each oTempFileLog As clsZDTFileLog In colFileLog
                                If oTempFileLog.msCntr = oArm.Controller.FanucName Then
                                    bAddController = False
                                    oFileLog = oTempFileLog
                                End If
                            Next
                            If bAddController Then
                                    oFileLog = New clsZDTFileLog(oArm.Controller.FanucName, mnLogDaysToKeep)
                                oFileLog.mnPLCBitMask = nBitMask(nBit)
                                mnPLCBitMask = mnPLCBitMask Or oFileLog.mnPLCBitMask
                                'Add files by controller
                                For Each sTmp As String In sFilesByController
                                    If sTmp <> String.Empty Then
                                        If oFileLog.msFiles Is Nothing Then
                                            ReDim oFileLog.msFiles(0)
                                        Else
                                            ReDim Preserve oFileLog.msFiles(oFileLog.msFiles.GetUpperBound(0) + 1)
                                        End If
                                        oFileLog.msFiles(oFileLog.msFiles.GetUpperBound(0)) = sTmp
                                    End If
                                Next
                            End If
                            'Add files by arm
                            For Each sTmp As String In sFilesByArm
                                If sTmp <> String.Empty Then
                                    If oFileLog.msFiles Is Nothing Then
                                        ReDim oFileLog.msFiles(0)
                                    Else
                                        ReDim Preserve oFileLog.msFiles(oFileLog.msFiles.GetUpperBound(0) + 1)
                                    End If
                                    oFileLog.msFiles(oFileLog.msFiles.GetUpperBound(0)) = sTmp.Replace("#", oArm.ArmNumber.ToString)
                                End If
                            Next
                            'Add new controller to collection
                            If bAddController Then
                                colFileLog.Add(oFileLog)
                            End If
                        End If
                    Next
                    Next
                End If

            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

End Class

Friend Class clsZDTMonitor
    Private Const msMODULE As String = "clsZDTMonitor"
    Private Const msDefaultColor As String = "White"
    Friend mcolZDTArms As Collection
    Private msPLCData As String()
    Friend Event Update(ByRef sText As String, ByRef sColor As String)
    '********************************
    'ZDT Changes 03/03/14
    Friend WithEvents mZDTWatcher As FileSystemWatcher
    Friend msFetchProgram As String = "ZDTGetData.exe"
    Friend msFetchProgramPath As String = String.Empty
    Private msStatusText As String = gpsRM.GetString("psZDT_BUTTON_OK")
    Private mcolStatusText As New Collection
    Private mbFileAlertActive As Boolean = False
    Private mbFileAlertChanged As Boolean = False

    Private mnStatusPointer As Integer = 1
    Private mnStatusCounter As Integer = 0
    Private msStatusColor As String = msDefaultColor
    Private mbFlash As Boolean = False
    Private mbFlashToggle As Boolean = False
    Private WithEvents tmrRefreshStatus As System.Windows.Forms.Timer
    Private WithEvents tmrFlash As System.Windows.Forms.Timer
    Private mbRefreshStatus As Boolean = False
    Private WithEvents moPLC As New clsPLCComm
    Friend moZDTConfig As clsZDTConfig
    Private mbInitDisplay As Boolean = True
    Private moZone As clsZone = Nothing

    Private mbChanged As Boolean = False
    Friend Property Changed() As Boolean
        Get
            If mbFileAlertChanged Then
                Return True
            End If
            For Each oZDTArm As clsZDTArm In mcolZDTArms
                If oZDTArm.Changed Then
                    Return True
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            If value = False Then
                mbFileAlertChanged = False
                For Each oZDTArm As clsZDTArm In mcolZDTArms
                    oZDTArm.Changed = False
                Next
                If mbFlash Then
                    mbFlash = False
                    mbFlashToggle = False
                    RaiseEvent Update(msStatusText, msStatusColor)
                End If
            End If
        End Set
    End Property
    Friend Sub subArchiveTestData(ByRef oGrid As System.Windows.Forms.DataGridView, _
                              ByRef nColumn As Integer, ByRef nRow As Integer)
        '********************************************************************************************
        'Description: reset the test status at the 
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            If nColumn >= oGrid.ColumnCount Then
                Exit Sub
            End If
            If nRow >= oGrid.RowCount Then
                Exit Sub
            End If
            Dim oDR As DialogResult = MessageBox.Show(gpsRM.GetString("psARCHIVE_TEST_DATA"), gpsRM.GetString("psARCHIVE_TEST_DATA_CAP"), MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            Select Case oDR
                Case Windows.Forms.DialogResult.Yes
            For Each oZDTTest As clsZDTTest In moZDTConfig.colZDTTests
                'Find the test first
                If oZDTTest.sName = oGrid.Columns(nColumn).Name Then
                    'Does it have a folder?
                    If oZDTTest.sFolder <> String.Empty Then
                        Dim sPath As String = moZDTConfig.msZDTPath & oZDTTest.sFolder & "\"
                        Dim sArchivePath As String = moZDTConfig.msZDTPath & oZDTTest.sFolder & "_Archive\"
                        'See if there's a current folder
                        If (IO.Directory.Exists(sPath)) Then
                            If (IO.Directory.Exists(sArchivePath) = False) Then
                                'No archive folder yet, create it.
                                IO.Directory.CreateDirectory(sArchivePath)
                            End If
                            'find the robot we're archiving
                            For Each oArm As clsZDTArm In mcolZDTArms
                                If (oArm.sArm = oGrid.Rows(nRow).HeaderCell.Value.ToString) Then
                                    Dim sPathTemp As String = sPath & oArm.sCntr & "\"
                                    If IO.Directory.Exists(sPathTemp) Then
                                        'Zip files
                                        Dim sArchiveFile As String = sArchivePath & oArm.sArm & "_" & oZDTTest.sFolder & "_Archive_" & _
                                            Date.Now.Year.ToString("0000") & "_" & Date.Now.Month.ToString("00") & "_" & Date.Now.Day.ToString("00") & _
                                                    "_" & Date.Now.Hour.ToString("00") & Date.Now.Minute.ToString("00") & ".Zip"
                                        Dim sZipUtil As String = String.Empty
                                        If (GetDefaultFilePath(sZipUtil, mPWCommon.eDir.VBApps, String.Empty, gs_ZIP_UTIL_FILENAME)) Then
                                            If IO.File.Exists(sArchiveFile) Then
                                                IO.File.Delete(sArchiveFile)
                                            End If
                                            Dim sCmd As String = sZipUtil & " " & gs_ZIP_ALL & " " & sPathTemp & "  " & """" & sArchiveFile & """"
                                            'Shell out to a utility program for simple zip functions so we don't need the DLL linked to every project
                                            Shell(sCmd, AppWinStyle.Hide, True, 30000)
                                        End If
                                        'Delete files
                                        subCleanupLogFiles(sPathTemp, -1) 'Clean up everything older than tomorrow
                                        sPathTemp = sPathTemp & "Group" & oArm.nGroup.ToString & "\"
                                        If IO.Directory.Exists(sPathTemp) Then
                                            subCleanupLogFiles(sPathTemp, -1)
                                        End If
                                                Try
                                                    IO.Directory.Delete(sPathTemp, True)
                                                Catch ex As Exception
                                                End Try
                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
            Next
                Case Else
                    Exit Sub
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subArchiveTestData", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Friend Sub subResetStatus(ByRef oGrid As System.Windows.Forms.DataGridView, _
                              ByRef oMnu As ContextMenuStrip, _
                              ByRef nColumn As Integer, ByRef nRow As Integer)
        '********************************************************************************************
        'Description: reset the test status at the 
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            If nColumn >= oGrid.ColumnCount Then
                Exit Sub
            End If
            If nRow >= oGrid.RowCount Then
                Exit Sub
            End If
            subUpdateZDTStatus()
            'Check status from file
            For Each oTest As clsZDTTest In moZDTConfig.colZDTTests
                If oTest.sName = oGrid.Columns(nColumn).Name Then
                    For Each oZDTArm As clsZDTArm In mcolZDTArms
                        If (oZDTArm.sArm = oGrid.Rows(nRow).HeaderCell.Value.ToString) Then
                            oZDTArm.PCNumStatus(oTest.nIndex) = 0
                            oZDTArm.PCStatus(oTest.nIndex) = gpsRM.GetString("psZDT_ARM_OK")
                            oZDTArm.PCColor(oTest.nIndex) = "Green"
                            oZDTArm.FromFile(oTest.nIndex) = False
                        End If
                    Next
                    subWriteResults(mcolZDTArms, moZDTConfig.msZDTPath & oTest.sName & "Results.csv", oTest)
                End If
            Next
            'Update display
            subUpdateZDTStatus()
            subDisplayGrid(oGrid, oMnu)
            If mbFlashToggle Then
                RaiseEvent Update(msStatusText, msDefaultColor)
            Else
                RaiseEvent Update(msStatusText, msStatusColor)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subResetStatus", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                    ByVal Data() As String) Handles moPLC.NewData
        '********************************************************************************************
        'Description: The frmPLCComm that monitors the "ZDTLogger" status array
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            PLCData = Data
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: moPLC_NewData", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Friend ReadOnly Property Enabled() As Boolean
        '********************************************************************************************
        'Description: True if any tests are set up in the DB
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Get
            Try
                If moZDTConfig.colZDTTests IsNot Nothing Then
                    Return moZDTConfig.colZDTTests.Count > 0
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property
    Friend ReadOnly Property StatusText() As String
        '********************************************************************************************
        'Description: Holds the current status text for the button. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msStatusText
        End Get
    End Property
    Friend ReadOnly Property StatusColor() As String
        '********************************************************************************************
        'Description: Holds the current color for the button. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msStatusColor
        End Get
    End Property
    Public Sub subDisplayGrid(ByRef oGrid As System.Windows.Forms.DataGridView, ByRef oMnu As ContextMenuStrip)
        '********************************************************************************************
        'Description: Display the grid with details. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/10/14  MSW     subDisplayGrid - Clean up some wasteful code
        '********************************************************************************************
        oGrid.ColumnHeadersVisible = True
        oGrid.RowHeadersVisible = True
        If oGrid.ColumnCount = 0 Then
        For Each oTest As clsZDTTest In moZDTConfig.colZDTTests
            Dim sTmp As String = gpsRM.GetString("psZDT_" & oTest.sName & "_COL_CAP")
            If sTmp = String.Empty Then
                sTmp = oTest.sName
            End If
            oGrid.Columns.Add(oTest.sName, sTmp)
        Next
        End If
        'oGrid.RowCount = 0
        Dim nTest As Integer = 0
        Dim nRow As Integer = 0
        For Each oZDTArm As clsZDTArm In mcolZDTArms
            If oGrid.RowCount <= nRow Then
            oGrid.Rows.Add()
            oGrid.Rows(nRow).HeaderCell.Value = oZDTArm.sArm
            End If
            For Each oTest As clsZDTTest In moZDTConfig.colZDTTests
                Dim sColor As String = "Gray"
                Dim bTag(1) As Boolean
                bTag(0) = False
                bTag(1) = False
                If oZDTArm.FromFile(oTest.nIndex) Then
                    oGrid.Rows(nRow).Cells(oTest.sName).Value = oZDTArm.PCStatus(oTest.nIndex)
                    sColor = oZDTArm.PCColor(oTest.nIndex)
                    oGrid.Rows(nRow).Cells(oTest.sName).ContextMenuStrip = oMnu
                    bTag(0) = True
                Else
                    oGrid.Rows(nRow).Cells(oTest.sName).Value = oZDTArm.Status(oTest.nIndex)
                    sColor = oZDTArm.Color(oTest.nIndex)
                    bTag(0) = False
                    If oTest.sFolder = String.Empty Then
                    oGrid.Rows(nRow).Cells(oTest.sName).ContextMenuStrip = Nothing
                    Else
                        oGrid.Rows(nRow).Cells(oTest.sName).ContextMenuStrip = oMnu
                    End If
                End If
                If oTest.sFolder <> String.Empty Then
                    Dim sPath As String = moZDTConfig.msZDTPath & oTest.sFolder & "\" & oZDTArm.sCntr & "\"
                    If Directory.Exists(sPath) Then
                        Dim sTmp() As String = Directory.GetDirectories(sPath)
                        If sTmp IsNot Nothing AndAlso sTmp.Length > 0 Then
                            bTag(1) = True
                        Else
                            sTmp = Directory.GetFiles(sPath)
                            If sTmp IsNot Nothing AndAlso sTmp.Length > 0 Then
                        bTag(1) = True
                    End If
                End If
                    End If
                End If
                oGrid.Rows(nRow).Cells(oTest.sName).Tag = bTag
                Dim oBackColor As System.Drawing.Color = System.Drawing.Color.Gray
                Dim oForeColor As System.Drawing.Color = System.Drawing.Color.Black
                Select Case sColor
                    Case "White", "Green"
                        oBackColor = System.Drawing.Color.Green
                        oForeColor = System.Drawing.Color.Black
                    Case "Red"
                        oBackColor = System.Drawing.Color.Red
                        oForeColor = System.Drawing.Color.Black
                    Case "Yellow"
                        oBackColor = System.Drawing.Color.Yellow
                        oForeColor = System.Drawing.Color.Black
                    Case "Gray"
                        oBackColor = System.Drawing.Color.Gray
                        oForeColor = System.Drawing.Color.Black
                    Case Else
                End Select
                oGrid.Rows(nRow).Cells(oTest.sName).Style.BackColor = oBackColor
                oGrid.Rows(nRow).Cells(oTest.sName).Style.SelectionBackColor = oBackColor
                oGrid.Rows(nRow).Cells(oTest.sName).Style.ForeColor = oForeColor
                oGrid.Rows(nRow).Cells(oTest.sName).Style.SelectionForeColor = oForeColor
            Next
            nRow += 1
        Next
        If oGrid.Visible Then
            Changed = False
        End If
    End Sub
    Private Sub mZDTWatcher_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles mZDTWatcher.Changed
        '********************************************************************************************
        'Description: New data in the result file.  Update the status. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbRefreshStatus = True
    End Sub

    Friend Property PLCData() As String()
        '********************************************************************************************
        'Description: Update ZDT status from the PLC 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msPLCData
        End Get
        Set(ByVal value As String())
            Try
                msPLCData = value
                subUpdateZDTStatus()

                If Not (mbFlash) Then
                    If Changed Then
                        mbFlash = True
                    End If
                End If
                'Update display
                If mbFlashToggle Then
                    RaiseEvent Update(msStatusText, msDefaultColor)
                Else
                    RaiseEvent Update(msStatusText, msStatusColor)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Property: PLCData", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Set
    End Property
    Private Sub subUpdateZDTStatus()
        '********************************************************************************************
        'Description: Update ZDT status
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Clear
        Try
            mcolStatusText.Clear()
            msStatusText = gpsRM.GetString("psZDT_BUTTON_OK")
            msStatusColor = "Green"
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subUpdateZDTStatus(1)", _
           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Dim nTest As Integer = 0
        'Check PLC Status
        Try
            For Each oZDTArm As clsZDTArm In mcolZDTArms
                Dim sData As String = msPLCData(oZDTArm.nArmIdx)
                Dim nData As Integer = CInt(sData)
                Dim bPLCStatus As Boolean = ((nData And nBitMask(0)) <> 0)
                If bPLCStatus Then
                    'Fetch details from robot variables
                    oZDTArm.subGetStatusFromRobot(msStatusText, msStatusColor, mcolStatusText)
                Else
                    If oZDTArm.PLCStatus Or mbInitDisplay Then
                        'First pass with PLC alert off.  Clear the status
                        For Each oTest As clsZDTTest In moZDTConfig.colZDTTests
                            oZDTArm.NumStatus(oTest.nIndex) = 0
                            oZDTArm.Status(oTest.nIndex) = gpsRM.GetString("psZDT_ARM_OK")
                            oZDTArm.Color(oTest.nIndex) = "Green"
                        Next
                    End If
                End If
                oZDTArm.PLCStatus = bPLCStatus
            Next
            mbInitDisplay = False
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subUpdateZDTStatus(2)", _
           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        'Check status from file
        Dim bTmpAlert As Boolean = False
        If moZDTConfig.colZDTTests IsNot Nothing Then
            For Each oTest As clsZDTTest In moZDTConfig.colZDTTests
                If IO.File.Exists(moZDTConfig.msZDTPath & "\" & oTest.sName & "Results.csv") Then
                    subImportResults(mcolZDTArms, moZDTConfig.msZDTPath & "\" & oTest.sName & "Results.csv", oTest, True)
                    For Each oZDTArm As clsZDTArm In mcolZDTArms
                        If oZDTArm.Enabled(oTest.nIndex) AndAlso oZDTArm.PCNumStatus(oTest.nIndex) <> 0 Then
                            msStatusText = oZDTArm.PCStatus(oTest.nIndex)
                            msStatusColor = oZDTArm.PCColor(oTest.nIndex)
                            mcolStatusText.Add(msStatusText)
                            bTmpAlert = True
                            Exit For
                        End If
                    Next
                End If
            Next
        End If
        If bTmpAlert <> mbFileAlertActive Then
            mbFileAlertChanged = bTmpAlert
            mbFileAlertActive = bTmpAlert
        End If
        If mcolStatusText.Count = 0 Then
            mcolStatusText.Add(msStatusText)
        End If
        If mnStatusPointer > mcolStatusText.Count Then
            mnStatusPointer = 1
        End If
        msStatusText = mcolStatusText(mnStatusPointer).ToString
    End Sub

    Private Sub tmrFlash_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrFlash.Tick
        '********************************************************************************************
        'Description:  flash the button for Tom   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrFlash.Enabled = False
        Try
            If mnStatusCounter > 3 Then
                mnStatusCounter = 1
                If mnStatusPointer >= mcolStatusText.Count Then
                    mnStatusPointer = 1
                Else
                    mnStatusPointer += 1
                End If
                msStatusText = mcolStatusText(mnStatusPointer).ToString
            Else
                mnStatusCounter += 1
            End If
            If mbFlash Then
                If Changed Then
                    mbFlashToggle = Not (mbFlashToggle)
                    If mbFlashToggle Then
                        RaiseEvent Update(msStatusText, msDefaultColor)
                    Else
                        RaiseEvent Update(msStatusText, msStatusColor)
                    End If
                Else
                    mbFlash = False
                    mbFlashToggle = False
                    RaiseEvent Update(msStatusText, msStatusColor)
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine:tmrFlash_Tick", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        tmrFlash.Enabled = True
    End Sub
    Private Sub tmrRefreshStatus_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefreshStatus.Tick
        '********************************************************************************************
        'Description:  Check for a new results file   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefreshStatus.Enabled = False
        Try
            'If mbRefreshStatus Then
            mbRefreshStatus = False
            subUpdateZDTStatus()
            'Update display
            If Not (mbFlash) Then
                If Changed Then
                    mbFlash = True
                End If
            End If
            If mbFlashToggle Then
                RaiseEvent Update(msStatusText, msDefaultColor)
            Else
                RaiseEvent Update(msStatusText, msStatusColor)
            End If
            'End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: tmrRefreshStatus_Tick", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        tmrRefreshStatus.Enabled = True
    End Sub
    Friend Function bSendFileReqToPLC() As Boolean
        '********************************************************************************************
        'Description: send file request bits to PLC
        '
        'Parameters: None
        'Returns:    True if it's sent, false if not required
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If moZDTConfig.mnPLCBitMask <> 0 Then
                Dim s(0) As String
                s(0) = moZDTConfig.mnPLCBitMask.ToString
                With moPLC
                    .ZoneName = moZone.Name
                    .TagName = "Z" & moZone.ZoneNumber.ToString & "ZDTFileReq"
                    .PLCData = s
                End With
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: bSendFileReqToPLC", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Friend Sub New(ByRef oZone As clsZone, ByRef oArms As clsArms)
        '********************************************************************************************
        'Description: New ZDT management class
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            moZone = oZone
            moZDTConfig = New clsZDTConfig(oZone, oArms)
            If moZDTConfig.colZDTTests IsNot Nothing AndAlso moZDTConfig.colZDTTests.Count > 0 Then

                mcolZDTArms = New Collection
                'Build robot data structure
                For Each oarm As clsArm In oArms
                    Dim oZDTArm As clsZDTArm = New clsZDTArm(oZone, oarm, moZDTConfig, True)
                    If oZDTArm.AnyTestEnabled Then
                    mcolZDTArms.Add(oZDTArm)
                    End If
                    'oZDTArm.subDebugPrint()
                Next
                mZDTWatcher = New FileSystemWatcher
                With mZDTWatcher
                    .Path = moZDTConfig.msZDTPath
                    'Watch for changes in File Size
                    .NotifyFilter = NotifyFilters.Size Or NotifyFilters.CreationTime Or NotifyFilters.LastWrite  'NotifyFilters.LastWrite  'RJO 09/07/12
                    .Filter = "*.*"
                    .IncludeSubdirectories = False
                    .EnableRaisingEvents = True
                End With

                If tmrRefreshStatus Is Nothing Then
                    tmrRefreshStatus = New System.Windows.Forms.Timer
                    tmrRefreshStatus.Interval = 5000
                    tmrRefreshStatus.Enabled = True
                End If
                If tmrFlash Is Nothing Then
                    tmrFlash = New System.Windows.Forms.Timer
                    tmrFlash.Interval = 1000
                    tmrFlash.Enabled = True
                End If
                If mPWCommon.GetDefaultFilePath(msFetchProgramPath, eDir.VBApps, String.Empty, msFetchProgram) Then
                    'Good
                End If
                msStatusText = gpsRM.GetString("psZDT_BUTTON_OK")
                msStatusColor = "Green"

                With moPLC
                    .ZoneName = oZone.Name
                    .TagName = "Z" & oZone.ZoneNumber & "ZDTData"
                    PLCData = .PLCData
                End With
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
Class clsZDTLogger

    Private Const msMODULE As String = "clsZDTLogger"
    Private mcolZDTArms As Collection
    Private WithEvents tmrRefreshLogStatus As System.Windows.Forms.Timer
    Private mbRefreshStatus As Boolean = False
    Private WithEvents moPLC As New clsPLCComm
    Private WithEvents mZDTLogWatcher As FileSystemWatcher
    Private msPLCData() As String
    Private mnCurrentTest As Integer = -1
    Private mnCurrentRobots As Integer = 0
    Private mcolCheckXML As Collection
    Private moZDTConfig As clsZDTConfig
    Private mdtLogDateTime As DateTime = Nothing
    Private mdtFileReqDateTime As DateTime = Nothing
    Private moZone As clsZone

    Private Sub subUpdateResultsFromXML(ByRef sFileName As String, ByRef oTest As clsZDTTest)
        '********************************************************************************************
        'Description: read updates from XML file 
        '
        'Parameters: sFileName - xml file to read from
        '           oTest - ZDT test to update
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Try
            Dim nTestIndex As Integer = oTest.nIndex
            oXMLDoc.Load(sFileName)
            oMainNode = oXMLDoc.SelectSingleNode("//" & "Diag")
            oNodeList = oMainNode.SelectNodes("//" & "Monitor")
            If oNodeList.Count > 0 Then
                Dim nMax As Integer = oNodeList.Count - 1
                For Each oNode As XmlNode In oNodeList
                    Dim sKey As String = String.Empty
                    Dim nGroup As Integer = 0
                    Dim dtDateTime As DateTime = DateTime.MinValue
                    Dim nFlag As Integer = 0
                    For Each oAttr As XmlAttribute In oNode.Attributes
                        Select Case oAttr.Name
                            Case "Key"
                                sKey = oAttr.Value.ToString
                            Case "Group"
                                nGroup = CType(oAttr.Value.ToString.Substring(5), Integer)
                            Case "DateTime"
                                dtDateTime = CType(oAttr.Value.ToString, DateTime)
                            Case "Flag"
                                nFlag = CType(oAttr.Value.ToString, Integer)
                        End Select
                    Next

                    For Each oArm As clsZDTArm In mcolZDTArms
                        If (sFileName.Contains("\" & oArm.sCntr & "\")) AndAlso _
                            sFileName.Contains(oArm.sCntr) AndAlso _
                            (nGroup = oArm.nGroup) Then
                            'This data is for this arm
                            'check if this test is newer
                            If (oArm.dtDateTime(nTestIndex).CompareTo(dtDateTime) <= 0) Then
                                'Newer results, save them
                                If nFlag = 0 Then
                                    oArm.PCNumStatus(nTestIndex) = 0
                                    oArm.PCStatus(nTestIndex) = "psZDT_ARM_OK"
                                    oArm.PCColor(nTestIndex) = "Green"
                                Else
                                    oArm.PCNumStatus(nTestIndex) = 1
                                    oArm.PCStatus(nTestIndex) = "psZDT_ARM_ALERT"
                                    oArm.PCColor(nTestIndex) = "Yellow"
                                End If
                            End If
                            Exit For
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subUpdateResultsFromXML", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrRefreshLogStatus_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefreshLogStatus.Tick
        '********************************************************************************************
        'Description:  Check for a new results file   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefreshLogStatus.Enabled = False
        Try
            Dim sName As String = String.Empty
            Do While mcolCheckXML.Count > 0
                'Skip over repeats
                If sName <> mcolCheckXML.Item(mcolCheckXML.Count).ToString Then
                    sName = mcolCheckXML.Item(mcolCheckXML.Count).ToString
                    Dim sPath As String = moZDTConfig.msZDTPath & sName
                    If IO.File.Exists(sPath) Then
                        Try
                            'FullPathc:\paint\ZDT\J1\RC_1\diag.xml
                            'Name=J1\RC_1\diag.xml
                            'ChangeType=Changed()
                            For Each oZDTTest As clsZDTTest In moZDTConfig.colZDTTests
                                If sName.StartsWith(oZDTTest.sFolder & "\", StringComparison.CurrentCultureIgnoreCase) Then
                                    'Found the test folder.  
                                    ' Can only go so far with the data based setup.  This part needs 
                                    ' to be coded for any test that uses it
                                    Dim sOutputFile As String = moZDTConfig.msZDTPath & "\" & oZDTTest.sName & "Results.csv"
                                    'Load the saved status for this test
                                    subImportResults(mcolZDTArms, sOutputFile, oZDTTest)
                                    'Update the status from PDE
                                    subUpdateResultsFromXML(sPath, oZDTTest)
                                    'Save the results
                                    subWriteResults(mcolZDTArms, sOutputFile, oZDTTest)
                                    ''Zip the results
                                    'subZipResults(msZDTPath, oZDTTest.sName)
                                End If
                            Next
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: mZDTLogWatcher_Changed", _
                                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                            sName = String.Empty 'Prevent clearing multiple entries if it fails
                        End Try
                    Else
                        sName = String.Empty 'Prevent clearing multiple entries if it fails

                    End If
                End If
                mcolCheckXML.Remove(mcolCheckXML.Count)
            Loop
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: tmrRefreshLogStatus_Tick", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try


        Try
            If moZDTConfig.colFileLog IsNot Nothing Then
                If moZDTConfig.mnPLCBitMask <> 0 Then
                    If Now > mdtFileReqDateTime Then
                        Dim s(0) As String
                        s(0) = moZDTConfig.mnPLCBitMask.ToString
                        With moPLC
                            .ZoneName = moZone.Name
                            .TagName = "Z" & moZone.ZoneNumber.ToString & "ZDTFileReq"
                            .PLCData = s
                        End With
                        mdtFileReqDateTime = mdtFileReqDateTime.AddHours(12)
                    Else
                        'This looks redundant, but it makes sure that the PLC pulse gets a scan before the getfile on the first pass
                        If Now > mdtLogDateTime Then
                            moZDTConfig.subGetLogFiles(moZDTConfig.msLogFilePath)
                            mdtLogDateTime = mdtLogDateTime.AddHours(12)
                        End If
                    End If
                Else
                    If Now > mdtLogDateTime Then
                        moZDTConfig.subGetLogFiles(moZDTConfig.msLogFilePath)
                        mdtLogDateTime = mdtLogDateTime.AddHours(12)
                    End If
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: tmrRefreshLogStatus_Tick", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        tmrRefreshLogStatus.Enabled = True
    End Sub

    Private Sub mZDTLogWatcher_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles mZDTLogWatcher.Changed
        '********************************************************************************************
        'Description: New data in the result file.  Update the status. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Not (mcolCheckXML.Contains(e.Name)) Then
            mcolCheckXML.Add(e.Name, e.Name)
        End If
    End Sub

    Private Sub subGetFiles(ByRef oZDTTest As clsZDTTest, ByRef sVRFileName As String, ByRef sTestPath As String)
        '********************************************************************************************
        'Description: read files from the robot controllers
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            For Each oArm As clsZDTArm In mcolZDTArms
                Try
                    If oArm.FetchData Then
                        For Each sArm As String In oZDTTest.colArms
                            If sArm = oArm.sArm Then
                                Dim sCntrPath As String = sTestPath & "\" & oArm.sCntr
                                If (IO.Directory.Exists(sCntrPath) = False) Then
                                    IO.Directory.CreateDirectory(sCntrPath)
                                End If
                                Dim oFTP As clsFSFtp
                                Try
                                    oFTP = New clsFSFtp(oArm.sCntr)
                                    Try
                                        Dim sArmPath As String = sCntrPath & "\Group" & oArm.nGroup
                                        If (IO.Directory.Exists(sArmPath) = False) Then
                                            IO.Directory.CreateDirectory(sArmPath)
                                        End If
                                        Dim sDateFolder As String = sArmPath & "\" & Date.Now.Year.ToString("0000") & "_" & Date.Now.Month.ToString("00") & "_" & Date.Now.Day.ToString("00")
                                        If (IO.Directory.Exists(sDateFolder) = False) Then
                                            IO.Directory.CreateDirectory(sDateFolder)
                                        End If
                                        With oFTP
                                            If .Connected Then
                                                .WorkingDir = "MD:"
                                                .DestDir = sDateFolder
                                                Dim stmpFile As String = sVRFileName.Replace("#", oArm.nGroup.ToString)
                                                If .GetFile(stmpFile, stmpFile) Then
                                                    'Good
                                                Else
                                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                                            "ERROR: Could not get file: " & oArm.sCntr & "\MD\" & sVRFileName & " to " & sDateFolder & "   -    FTP ERROR: " & .ErrorMsg)
                                                End If
                                            End If
                                        End With
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                                    Finally
                                        oFTP.Close()
                                    End Try
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                                               "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                                Finally
                                    oFTP = Nothing
                                End Try

                            End If
                        Next
                        oArm.FetchData = False
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                               "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subGetFiles", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub



    Private Sub subFetchData(ByVal nTest As Integer)
        '********************************************************************************************
        'Description: Fetch ZDT files from the robot 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sVRFileName As String = String.Empty
        Dim sTestPath As String = String.Empty
        Dim sCmdPath As String = String.Empty
        Try
            Dim bGetFiles As Boolean = False
            Dim bCallProg As Boolean = False
            Dim oZDTTest As clsZDTTest = Nothing
            For Each oTmpZDTTest As clsZDTTest In moZDTConfig.colZDTTests
                If nTest = oTmpZDTTest.nIndex Then
                    oZDTTest = oTmpZDTTest
                    If oZDTTest.sGetFile <> String.Empty AndAlso oZDTTest.sGetFile <> String.Empty Then
                        sVRFileName = oZDTTest.sGetFile '"pavrgrg#.va"
                        sTestPath = moZDTConfig.msZDTPath & oZDTTest.sName
                        bGetFiles = True
                    End If
                    If oZDTTest.sCallProg <> String.Empty Then
                        sCmdPath = Environment.ExpandEnvironmentVariables("""" & oZDTTest.sCallProg & """" & " " & """" & sTestPath & """")
                        bCallProg = True
                    End If
                    Exit For
                End If
            Next
            If bGetFiles Then
                'Go get the VRs from the robots
                subGetFiles(oZDTTest, sVRFileName, sTestPath)
            End If
            If bCallProg Then
                'Call PDE blackbox program
                Try
                    'Debug.Print(sCmdPath)
                    Shell(sCmdPath, AppWinStyle.Hide, False)
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subFetchData-shell", _
                               "Error: " & ex.Message & vbCrLf & ex.StackTrace)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subFetchData", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try


    End Sub
    Friend Property PLCData() As String()
        '********************************************************************************************
        'Description: Update ZDT status from the PLC 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msPLCData
        End Get
        Set(ByVal value As String())
            Try
                msPLCData = value
                If msPLCData IsNot Nothing Then
                    For nTest As Integer = msPLCData.GetLowerBound(0) To msPLCData.GetUpperBound(0)
                        Try
                            If IsNumeric(msPLCData(nTest)) Then
                                Dim nTestActive As Integer = CInt(PLCData(nTest))
                                If nTestActive <> 0 Then
                                    mnCurrentTest = nTest
                                    mnCurrentRobots = nTestActive
                                    'Mark robots as busy
                                    For Each oArm As clsZDTArm In mcolZDTArms
                                        If ((mnCurrentRobots And nBitMask(oArm.nArmIdx)) <> 0) Then
                                            oArm.FetchData = True
                                        End If
                                    Next
                                    Exit For
                                End If
                                If ((nTestActive = 0) And (mnCurrentTest = nTest)) Then
                                    'Test complete
                                    'Fetch Data
                                    subFetchData(mnCurrentTest)
                                    'Reset the logger
                                    mnCurrentTest = -1
                                    mnCurrentRobots = 0
                                    Exit For
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Property: PLCData", _
           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Set
    End Property
    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                    ByVal Data() As String) Handles moPLC.NewData
        '********************************************************************************************
        'Description: The frmPLCComm that monitors the "ZDTLogger" status array
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Try
            PLCData = Data
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: moPLC_NewData", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub


    Friend Sub New(ByRef oZones As clsZones, ByRef oArms As clsArms)
        '********************************************************************************************
        'Description: New ZDT logger class
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/14  MSW     Improve cleanup to clear out extra logs
        '********************************************************************************************
        Try

            moZDTConfig = New clsZDTConfig(oZones.ActiveZone, oArms)
            If moZDTConfig.colFileLog Is Nothing AndAlso moZDTConfig.colZDTTests Is Nothing Then
                Exit Sub
            End If
            mcolZDTArms = New Collection
            moZone = oZones.ActiveZone
            'Build robot data structure
            For Each oarm As clsArm In oArms
                Dim oZDTArm As clsZDTArm = New clsZDTArm(oZones.ActiveZone, oarm, moZDTConfig)
                mcolZDTArms.Add(oZDTArm)
                'oZDTArm.subDebugPrint()
            Next

            mcolCheckXML = New Collection
            Dim nHour As Integer = Now.Hour
            If nHour >= 12 Then
                nHour = 12
            Else
                nHour = 0
            End If
            mdtLogDateTime = New DateTime(Now.Year, Now.Month, Now.Day, nHour, 0, 0)
            mdtFileReqDateTime = mdtLogDateTime.AddMinutes(-2)
            If moZDTConfig.colZDTTests IsNot Nothing Then
                For Each oZDTTest As clsZDTTest In moZDTConfig.colZDTTests
                    Dim sPath As String = moZDTConfig.msZDTPath & oZDTTest.sFolder & "\"
                    If oZDTTest.sFolder <> String.Empty Then
                        If (IO.Directory.Exists(sPath) = False) Then
                            IO.Directory.CreateDirectory(sPath)
                        End If
                        subCleanupLogFiles(sPath, oZDTTest.nDaysToKeep)
                        For Each oArm As clsZDTArm In mcolZDTArms
                            Dim sPathTemp As String = sPath & oArm.sCntr & "\"
                            If IO.Directory.Exists(sPathTemp) Then
                                subCleanupLogFiles(sPathTemp, oZDTTest.nDaysToKeep)
                                sPathTemp = sPathTemp & "Group" & oArm.nGroup.ToString & "\"
                                If IO.Directory.Exists(sPathTemp) Then
                                    subCleanupLogFiles(sPathTemp, oZDTTest.nDaysToKeep)
                                End If
                            End If
                        Next
                    End If
                Next
                mZDTLogWatcher = New FileSystemWatcher
                With mZDTLogWatcher
                    .Path = moZDTConfig.msZDTPath
                    'Watch for changes in File Size
                    .NotifyFilter = NotifyFilters.Size Or NotifyFilters.CreationTime Or NotifyFilters.LastWrite
                    .Filter = "*.XML"
                    .IncludeSubdirectories = True
                    .EnableRaisingEvents = True
                End With
                DoEvents()
                With moPLC
                    .ZoneName = oZones.CurrentZone
                    .TagName = "Z" & oZones.ActiveZone.ZoneNumber & "ZDTLogger"
                    PLCData = .PLCData

                    DoEvents()
                End With
            End If
            'If moZDTConfig.colFileLog IsNot Nothing Then
            '    subCleanupLogFiles(moZDTConfig.msLogFilePath, moZDTConfig.mnLogDaysToKeep)
            '    For Each oArm As clsZDTArm In mcolZDTArms
            '        Dim sPathTemp As String = moZDTConfig.msLogFilePath & oArm.sCntr & "\"
            '        If IO.Directory.Exists(sPathTemp) Then
            '            subCleanupLogFiles(sPathTemp, moZDTConfig.mnLogDaysToKeep)
            '        End If
            '    Next
            'End If
            If tmrRefreshLogStatus Is Nothing Then
                tmrRefreshLogStatus = New System.Windows.Forms.Timer
                tmrRefreshLogStatus.Interval = 1000
                tmrRefreshLogStatus.Enabled = True
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
Friend Class clsOVCArm
    Private Const msMODULE As String = "clsOVCArm"
    Private moArm As clsArm
    Private mnRobotNumber As Integer = 0
    Private mnEqNumber As Integer = 0
    Private moVar As FRRobot.FRCVar = Nothing
    Private moRobot As FRRobot.FRCRobot
    Private mnCount As Integer = 0
    Private mbChanged As Boolean = True
    Friend Property Changed() As Boolean
        Get
            Return mbChanged
        End Get
        Set(ByVal value As Boolean)
            mbChanged = value
        End Set
    End Property
    Friend ReadOnly Property RobotNumber() As Integer
        Get
            Return mnRobotNumber
        End Get
    End Property
    Friend Sub subRefresh()
        '********************************************************************************************
        'Description: Read counter from robot
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nNew As Integer = mnCount
        Try
            If moArm.Controller.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
                If moVar Is Nothing Then
                    moVar = CType(moRobot.SysVariables("$mor_grp[" & mnEqNumber.ToString & "].$sv_int_dat3[8]"), FRRobot.FRCVar)
                End If
                If moVar.IsInitialized Then
                    nNew = CType(moVar.Value, Integer)

                Else
                    nNew = 0
                End If

            Else
                moVar = Nothing
                nNew = 0
            End If
        Catch ex As Exception
            moVar = Nothing
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: subRefresh", _
                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        If nNew <> mnCount Then
            mbChanged = True
        End If
        mnCount = nNew
    End Sub

    Friend ReadOnly Property OVCCount() As Integer
        '********************************************************************************************
        'Description: Read counter from robot
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnCount
        End Get
    End Property
    Friend Sub New(ByRef oZone As clsZone, ByRef rArm As clsArm)
        '********************************************************************************************
        'Description: New OVC arm class  - holds status data for one arm
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            moArm = rArm
            mnRobotNumber = moArm.RobotNumber
            mnEqNumber = moArm.ArmNumber
            moRobot = moArm.Controller.Robot
            subRefresh()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
Friend Class clsOVCMonitor

    Private Const msMODULE As String = "clsOVCMonitor"
    Private colOVCArms As Collection
    Friend Event Changed()
    Private WithEvents tmrRefreshStatus As System.Windows.Forms.Timer
    Private Const mnInactiveScanTime As Integer = 2000
    Private Const mnActiveScanTime As Integer = 1000
    Friend ReadOnly Property OVCCount(ByVal nRobotNumber As Integer) As Integer
        '********************************************************************************************
        'Description:  Get an individual robot's OVC count   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                For Each oOVCArm As clsOVCArm In colOVCArms
                    If oOVCArm.RobotNumber = nRobotNumber Then
                        Return oOVCArm.OVCCount
                    End If
                Next
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: OVCCount", _
                           "Error: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
            Return Nothing
        End Get
    End Property
    Private Sub tmrRefreshStatus_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefreshStatus.Tick
        '********************************************************************************************
        'Description:  Check for variable changes   
        '
        'Parameters: none
        'Returns:    Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefreshStatus.Enabled = False
        Try
            Dim bChanged As Boolean = False
            Dim bActive As Boolean = False
            For Each oOVCArm As clsOVCArm In colOVCArms
                oOVCArm.subRefresh()
                bChanged = bChanged Or oOVCArm.Changed
                bActive = bActive Or (oOVCArm.OVCCount > 0)
                oOVCArm.Changed = False
            Next
            If bActive Then
                tmrRefreshStatus.Interval = mnActiveScanTime
            Else
                tmrRefreshStatus.Interval = mnInactiveScanTime
            End If
            If bChanged Then
                RaiseEvent Changed()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: tmrRefreshStatus_Tick", _
                       "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        tmrRefreshStatus.Enabled = True
    End Sub

    Friend Sub New(ByRef oZone As clsZone, ByRef oArms As clsArms)
        '********************************************************************************************
        'Description: New ZDT management class
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            colOVCArms = New Collection
            'Build robot data structure
            For Each oarm As clsArm In oArms
                Select Case oarm.Controller.ControllerType
                    Case eControllerType.R30iA, eControllerType.R30iB
                        Dim oOVCArm As clsOVCArm = New clsOVCArm(oZone, oarm)
                        colOVCArms.Add(oOVCArm)
                    Case Else
                End Select
            Next
            If tmrRefreshStatus Is Nothing Then
                tmrRefreshStatus = New System.Windows.Forms.Timer
                tmrRefreshStatus.Interval = mnInactiveScanTime
                tmrRefreshStatus.Enabled = True
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & ":" & msMODULE & ", Routine: New", _
                                   "Error: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class

