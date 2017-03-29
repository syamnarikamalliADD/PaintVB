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
' Form/Module: frmMain
'
' Description: DmonViewer
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
'    01/24/2012 MSW     first draft                                                   4.01.01.00
'    03/23/2012 RJO     modifed for .NET Password and IPC                             4.01.02.00
'    04/11/12   MSW     Changed CommonStrings setup so it builds correctly            4.01.03.00
'    04/27/12   MSW     Fix screen shot names for chart form                          4.01.03.01
'    05/01/12   MSW     Update export support so ODS opens in Excel 2010              4.01.03.02
'    05/08/12   MSW     Work on XLSX output  - excel export w/o linkning to excel     4.01.03.03
'    06/07/12   MSW     Adjust export support to remove excel links, support csv,xlsx,ods     4.01.04.00
'    10/05/12   MSW     Fix up colors in item config                                  4.01.04.01
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.04.02
'    11/19/12   MSW     Work on running standalone                                    4.01.04.03
'    02/27/13   MSW     publish standalone version                                    4.01.04.04
'                       Fix up the import/export a bit for xlsx and ods files
'                       Add an import/export for the graph config (DmonItems.xml) - standalone only
'                       Add the option to remember added folders, remove folders from the list
'    03/28/13   MSW     Deal with duplicate column names                              4.01.04.05
'    04/16/13   MSW     Add Canadian language files                                     4.01.05.00
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.01
'    09/30/13   MSW     Save screenshots as jpegs                                       4.01.06.00
'    12/04/13   MSW     subRefreshFolders - remove missing folders from the list.       4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    05/16/14   MSW     Avoid Foreign numbers                                           4.01.07.01
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports System.Windows.Forms
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip
Imports System
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath
Imports System.Net
Imports System.Resources
Imports System.Windows.Forms.Application

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "DmonViewer"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME As String = "View_DmonViewer_Main.jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    '******** End Form Constants    *****************************************************************

    '***** XML Setup ***************************************************************************
    'Name of the config file
    Private Const msXMLFILENAME As String = "DmonItems.xml"
    Private msXMLFilePath As String = String.Empty
    '*****  End XML Setup **********************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private msLastPage As String = String.Empty
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Private mnMaxProgress As Integer = 100
    Public gPrintHtml As clsPrintHtml
    Private mnMaxID As Integer = 0
    Private mnSelectedRow As Integer = 0
    Private mbDontUpdate As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnFindNextIndex As Integer = -1
    Private mbSearchBack As Boolean = False
    Private mbInAskForSave As Boolean = False
    'Dim sDate As String = Format(Now, "yyyyMMdd HHmmss")
    ''dig out the file name from production data
    'Dim sFileName As String = msDeviceName & gsDMON_DELIMITER & _
    '                        sDate & gsDMON_DELIMITER & _
    '                        sStyle & gsDMON_DELIMITER & _
    '                        sColor & gsDMON_DELIMITER & _
    '                        ProdData(mnOptionPtr) & gsDMON_DELIMITER
    '    sFileName = sFileName & sVinStr & gsDMON_DELIMITER & moZone.ZoneNumber & ".dt"
    Public Structure tFileInfo
        Dim sFileName As String
        Dim sDevice As String
        Dim dtDate As Date
        Dim sStyle As String
        Dim sColor As String
        Dim sOption As String
        Dim sVIN As String
        Dim sZone As String
        Dim sLocation As String
    End Structure
    Public Structure tItemInfo
        Dim fMin As Single
        Dim fMax As Single
        Dim sUnits As String
        Dim sName As String
        Dim sType As String 'probably should be enum, but this allows the file read/write to stay flexible
        Dim fScale As Single
        Dim fOffset As Single
        Dim fPlotMin As Single
        Dim fPlotMax As Single
        Dim sMultiPlex As String
        Dim nMultiPlexStart As Integer
        Dim nMultiPlexCount As Integer
        Dim oColor As Color
    End Structure
    Friend moItemInfo As tItemInfo() = Nothing
    Private msDMONData As String = ""
    Private msDMONArchive As String = ""
    Public Structure tFolderInfo
        Dim sPath As String
        Dim bZip As Boolean
        Dim dtDate As Date
        Dim oFileInfo() As tFileInfo
    End Structure
    Private mtFolderInfo() As tFolderInfo = Nothing
    Friend gnCOL_ZONE As Integer = 0
    Friend gnCOL_DEVICE As Integer = 1
    Friend gnCOL_STYLE As Integer = 2
    Friend gnCOL_COLOR As Integer = 3
    Friend gnCOL_OPTION As Integer = 4
    Friend gnCOL_DATE As Integer = 5
    Friend gnCOL_VIN As Integer = 6
    Friend gnCOL_FILENAME As Integer = 7
    Friend gnCOL_LOCATION As Integer = 8
    Friend mnChkItem As Integer = -1
#End Region
#Region " Properties "
    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                String.Empty)

        End Set

    End Property

    Friend ReadOnly Property DisplayCulture() As System.Globalization.CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return New System.Globalization.CultureInfo(msCulture)
        End Get

    End Property
    Friend Property Privilege() As ePrivilege
        '********************************************************************************************
        'Description:  What can a user do - for now take privilege from password object and massage 
        '               it to work - need this property for future 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            If UsePaintWorksDB Then
                Return mPrivilegeGranted
            Else
                Return ePrivilege.Edit
            End If


        End Get
        Set(ByVal Value As ePrivilege)

            If UsePaintWorksDB Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                'Control.CheckForIllegalCrossThreadCalls = False

                mPrivilegeRequested = Value

                'handle logout
                If mPrivilegeRequested = ePrivilege.None Then
                    mPrivilegeGranted = ePrivilege.None
                    subEnableControls(True)
                    DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                    'this is needed with old password object raising events
                    'and can hopefully be removed when password is redone
                    'Control.CheckForIllegalCrossThreadCalls = True
                    Exit Property
                End If

                'prevent recursion
                If mPrivilegeGranted = mPrivilegeRequested Then
                    'this is needed with old password object raising events
                    'and can hopefully be removed when password is redone
                    'Control.CheckForIllegalCrossThreadCalls = True
                    Exit Property
                End If


                'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/23/12
                If moPassword.CheckPassword(Value) Then
                    'passed
                    mPrivilegeGranted = mPrivilegeRequested
                    'if privilege changed may have to enable controls
                    subEnableControls(True)
                    DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                Else
                    'denied
                    If moPassword.UserName <> String.Empty Then _
                            mPrivilegeRequested = mPrivilegeGranted
                    DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                End If

                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                'Control.CheckForIllegalCrossThreadCalls = True
            Else
                ' Do Nothing
            End If

        End Set
    End Property
    Friend Property DataLoaded() As Boolean
        '********************************************************************************************
        'Description:  Data loaded flag for form
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            'if not loaded disable all controls
            subEnableControls(Value)
        End Set
    End Property
    Friend Property LoggedOnUser() As String
        '********************************************************************************************
        'Description:  Who's Logged on
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msUserName
        End Get
        Set(ByVal Value As String)
            msUserName = Value
        End Set
    End Property
    Friend Property Progress() As Integer
        '********************************************************************************************
        'Description:  run the progress bar
        '
        'Parameters: 1 to 100 percent
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        ' 09/30/13  MSW     Use the control's min and max instead of literal constants
        '********************************************************************************************
        Set(ByVal Value As Integer)
            Try
                If Value < tspProgress.Minimum Then Value = tspProgress.Minimum
                If Value > tspProgress.Maximum Then Value = tspProgress.Minimum
                mnProgress = Value
                tspProgress.Value = mnProgress
                If mnProgress > 0 And mnProgress < 100 Then
                    lblSpacer.Width = gtSSSize.SpaceLabelInvisSize
                    tspProgress.Visible = True
                Else
                    lblSpacer.Width = gtSSSize.SpaceLabelVisbleSize
                    tspProgress.Visible = False
                End If
                stsStatus.Invalidate()
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Progress", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
        Get
            Return mnProgress
        End Get
    End Property
    Friend Property Status(Optional ByVal StatusStripOnly As Boolean = False) As String
        '********************************************************************************************
        'Description:  write status messages to listbox and statusbar
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
		' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        '********************************************************************************************
        Get
            Return stsStatus.Text
        End Get
        Set(ByVal Value As String)
            Try
                If StatusStripOnly = False Then
                    mPWCommon.AddToStatusBox(lstStatus, Value)
                End If
                stsStatus.Items("lblStatus").Text = Strings.Replace(Value, vbTab, "  ")
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property

#End Region
#Region " Routines "
    Public Sub UpdateStatus(ByVal nprogress As Integer, ByVal sStatus As String)
        Status(True) = sStatus
        Progress = nprogress
    End Sub



    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  Clear out textboxes reset colors etc. here
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblFolders.Text = gpsRM.GetString("psFOLDERS")
        lblFiles.Text = gpsRM.GetString("psDMON_FILES")
        dgFiles.Columns.Clear()
        dgFiles.Rows.Clear()

        dgFiles.Columns.Add("colZone", gpsRM.GetString("psCOL_ZONE"))
        dgFiles.Columns.Add("colDevice", gpsRM.GetString("psCOL_DEVICE"))
        dgFiles.Columns.Add("colStyle", gpsRM.GetString("psCOL_STYLE"))
        dgFiles.Columns.Add("colColor", gpsRM.GetString("psCOL_COLOR"))
        dgFiles.Columns.Add("colOption", gpsRM.GetString("psCOL_OPTION"))
        dgFiles.Columns.Add("colDate", gpsRM.GetString("psCOL_DATE"))
        dgFiles.Columns.Add("colVIN", gpsRM.GetString("psCOL_VIN"))
        dgFiles.Columns.Add("colFileName", gpsRM.GetString("psCOL_FILENAME"))
        dgFiles.Columns.Add("colLocation", gpsRM.GetString("psCOL_LOCATION"))
        dgFiles.Columns(gnCOL_ZONE).ValueType = GetType(Integer)
        dgFiles.Columns(gnCOL_DEVICE).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_STYLE).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_COLOR).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_OPTION).ValueType = GetType(Integer)
        dgFiles.Columns(gnCOL_DATE).ValueType = GetType(Date)
        dgFiles.Columns(gnCOL_VIN).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_FILENAME).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_LOCATION).ValueType = GetType(String)
        dgFiles.Columns(gnCOL_ZONE).Width = 50
        dgFiles.Columns(gnCOL_DEVICE).Width = 50
        dgFiles.Columns(gnCOL_STYLE).Width = 50
        dgFiles.Columns(gnCOL_COLOR).Width = 50
        dgFiles.Columns(gnCOL_OPTION).Width = 50
        dgFiles.Columns(gnCOL_DATE).Width = 110
        dgFiles.Columns(gnCOL_VIN).Width = 50
        dgFiles.Columns(gnCOL_FILENAME).Width = 220
        dgFiles.Columns(gnCOL_LOCATION).Width = 220
        Application.DoEvents()
    End Sub
    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        btnClose.Enabled = True
        Dim bDataLoaded As Boolean = DataLoaded
        If bEnable = False Then
            bRestOfControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    bRestOfControls = False
                Case Else
                    bRestOfControls = bDataLoaded
            End Select
        End If
        If bEnable Then
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End If
    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description:  new data selected - set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

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
        mbEditsMade = False
        DataLoaded = False
        With gpsRM
            btnRefresh.Text = .GetString("psREFRESH")
            btnOpen.Text = .GetString("psOPEN")
            btnRefresh.ToolTipText = .GetString("psREFRESH")
            btnOpen.ToolTipText = .GetString("psOPEN")
            mnuAddFolder.Text = .GetString("psADD_FOLDER")
            mnuAddZip.Text = .GetString("psADD_ZIP")
            mnuOpenDmonFile.Text = .GetString("psOPEN_DMON_FILE")
            btnHide.ToolTipText = .GetString("psHIDE")
            btnShow.ToolTipText = .GetString("psSHOW")
            lblZone.Text = .GetString("psCOL_ZONE")
            lblDevice.Text = .GetString("psCOL_DEVICE")
            btnApplyFilter.Text = .GetString("psAPPLY_FILTER")
            btnClearFilters.Text = .GetString("psCLEAR_FILTERS")
            lblStyle.Text = .GetString("psCOL_STYLE")
            lblOption.Text = .GetString("psCOL_OPTION")
            lblColor.Text = .GetString("psCOL_COLOR")
            lblVin.Text = .GetString("psCOL_VIN")
            lblAfter.Text = .GetString("psAFTER_CAP")
            lblBefore.Text = .GetString("psBEFORE_CAP")
            cboZone.Text = "*"
            cboDevice.Text = "*"
            cboStyle.Text = "*"
            cboOption.Text = "*"
            cboColor.Text = "*"
            cboZone.Items.Add("*")
            cboDevice.Items.Add("*")
            cboStyle.Items.Add("*")
            cboOption.Items.Add("*")
            cboColor.Items.Add("*")
            txtVin.Text = "*"
            chkAfter.Checked = False
            chkBefore.Checked = False
            lblFolders.Text = .GetString("psFOLDERS")
            lblFiles.Text = .GetString("psDMON_FILES")
            mnuRemoveItem.Text = .GetString("psREMOVE_ITEM")
            mnuImportCfg.Text = .GetString("psIMPORT_CFG")
            mnuExportCfg.Text = .GetString("psEXPORT_CFG")
            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) And _
               (mPWCommon.UsePaintWorksDB = False) Then
                mnuImportCfg.Visible = True
                mnuExportCfg.Visible = True
            Else
                mnuImportCfg.Visible = False
                mnuExportCfg.Visible = False
            End If
        End With
        mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
        mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")
    End Sub

    Private Sub subReadDmonItemsFromXML(ByRef oItemInfo() As tItemInfo)
        '********************************************************************************************
        'Description:  Read Dmon Items from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = "//DmonItem"
        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            oXMLDoc.Load(msXMLFilePath)

            oMainNode = oXMLDoc.SelectSingleNode("//DmonItems")
            oNodeList = oMainNode.SelectNodes(sPath)
            Try
                If oNodeList.Count = 0 Then
                    mDebug.WriteEventToLog(msSCREEN_NAME & ":subReadDmonItemsFromXML", "DmonItems not found.")
                Else
                    ReDim oItemInfo(oNodeList.Count - 1)
                    For nItem As Integer = 0 To oNodeList.Count - 1
                        oNode = oNodeList(nItem)
                        Try
                            oItemInfo(nItem).sName = oNode.Item("Name").InnerXml
                        Catch ex As Exception
                            oItemInfo(nItem).sName = String.Empty
                        End Try
                        Try
                            oItemInfo(nItem).sUnits = oNode.Item("Units").InnerXml
                        Catch ex As Exception
                            oItemInfo(nItem).sUnits = String.Empty
                        End Try
                        Try
                            oItemInfo(nItem).sType = oNode.Item("DataType").InnerXml
                        Catch ex As Exception
                            oItemInfo(nItem).sType = gpsRM.GetString("psFLOAT")
                        End Try
                        Try
                            oItemInfo(nItem).fOffset = CType((oNode.Item("Offset").InnerXml), Single)
                        Catch ex As Exception
                            oItemInfo(nItem).fOffset = 0
                        End Try
                        Try
                            oItemInfo(nItem).fScale = CType((oNode.Item("Scale").InnerXml), Single)
                        Catch ex As Exception
                            oItemInfo(nItem).fScale = 1
                        End Try
                        Try
                            oItemInfo(nItem).fPlotMin = CType((oNode.Item("Min").InnerXml), Single)
                        Catch ex As Exception
                            oItemInfo(nItem).fPlotMin = 0
                        End Try
                        Try
                            oItemInfo(nItem).fPlotMax = CType((oNode.Item("Max").InnerXml), Single)
                        Catch ex As Exception
                            oItemInfo(nItem).fPlotMax = 8000
                        End Try
                        Try
                            oItemInfo(nItem).sMultiPlex = oNode.Item("MultiPlex").InnerXml
                        Catch ex As Exception
                            oItemInfo(nItem).sName = String.Empty
                        End Try
                        Dim sColor As String = oNode.Item("Color").InnerXml
                        oItemInfo(nItem).oColor = frmChart.StringToColor(sColor)
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & ":subReadDmonItemsFromXML", "Invalid XML Data: [" & sPath & "] - " & ex.Message)
            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & ":subReadDmonItemsFromXML", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
        End Try
    End Sub

    Public Sub subSaveDmonItemsToXML(ByRef oItemInfo() As tItemInfo)
        '********************************************************************************************
        'Description:  Save Dmon Item config to XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = "//DmonItem"
        Dim sTopic As String = String.Empty
        Dim oNode As XmlNode = Nothing
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            If (IO.File.Exists(msXMLFilePath) = False) Then
                IO.File.Create(msXMLFilePath)
                oXMLDoc = New XmlDocument
                oMainNode = oXMLDoc.CreateElement("DmonItems")
                oXMLDoc.AppendChild(oMainNode)
                oMainNode = oXMLDoc.SelectSingleNode("//DmonItems")
            Else
                Try
                    oXMLDoc.Load(msXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//DmonItems")
                Catch ex As Exception
                    oXMLDoc = New XmlDocument
                    oMainNode = oXMLDoc.CreateElement("DmonItems")
                    oXMLDoc.AppendChild(oMainNode)
                    oMainNode = oXMLDoc.SelectSingleNode("//DmonItems")
                End Try
            End If
            For Each oItem As tItemInfo In oItemInfo
                Dim sColor As String = frmChart.ColorToString(oItem.oColor)
                oNodeList = oMainNode.SelectNodes(sPath & "[Name='" & oItem.sName & "']")
                If oNodeList.Count = 0 Then
                    oNode = oXMLDoc.CreateElement("DmonItem")
                    Dim oNameNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Name", Nothing)
                    Dim oUnitsNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Units", Nothing)
                    Dim oIntegerNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "DataType", Nothing)
                    Dim oOffsetNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Offset", Nothing)
                    Dim oScaleNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Scale", Nothing)
                    Dim oMinNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Min", Nothing)
                    Dim oMaxNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Max", Nothing)
                    Dim oMPlxNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "MultiPlex", Nothing)
                    Dim oColorNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Color", Nothing)

                    oNameNode.InnerXml = oItem.sName
                    oUnitsNode.InnerXml = oItem.sUnits
                    oIntegerNode.InnerXml = oItem.sType
                    oOffsetNode.InnerXml = oItem.fOffset.ToString
                    oScaleNode.InnerXml = oItem.fScale.ToString
                    oMinNode.InnerXml = oItem.fPlotMin.ToString
                    oMaxNode.InnerXml = oItem.fPlotMax.ToString
                    oMPlxNode.InnerXml = oItem.sMultiPlex
                    oColorNode.InnerXml = sColor

                    oNode.AppendChild(oNameNode)
                    oNode.AppendChild(oUnitsNode)
                    oNode.AppendChild(oIntegerNode)
                    oNode.AppendChild(oOffsetNode)
                    oNode.AppendChild(oScaleNode)
                    oNode.AppendChild(oMinNode)
                    oNode.AppendChild(oMaxNode)
                    oNode.AppendChild(oMPlxNode)
                    oNode.AppendChild(oColorNode)
                    oMainNode.AppendChild(oNode)
                Else
                    oNode = oNodeList(0) 'Should only be one match!!!
                    If oNode.Item("Name") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Name", Nothing)
                        oNewNode.InnerXml = oItem.sName
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Name").InnerXml = oItem.sName
                    End If
                    If oNode.Item("Units") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Units", Nothing)
                        oNewNode.InnerXml = oItem.sUnits
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Units").InnerXml = oItem.sUnits
                    End If
                    If oNode.Item("DataType") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "DataType", Nothing)
                        oNewNode.InnerXml = oItem.sType
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("DataType").InnerXml = oItem.sType
                    End If
                    If oNode.Item("Offset") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Offset", Nothing)
                        oNewNode.InnerXml = oItem.fOffset.ToString
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Offset").InnerXml = oItem.fOffset.ToString
                    End If
                    If oNode.Item("Scale") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Scale", Nothing)
                        oNewNode.InnerXml = oItem.fScale.ToString
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Scale").InnerXml = oItem.fScale.ToString
                    End If
                    If oNode.Item("Min") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Min", Nothing)
                        oNewNode.InnerXml = oItem.fPlotMin.ToString
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Min").InnerXml = oItem.fPlotMin.ToString
                    End If
                    If oNode.Item("Max") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Max", Nothing)
                        oNewNode.InnerXml = oItem.fPlotMax.ToString
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Max").InnerXml = oItem.fPlotMax.ToString
                    End If
                    If oNode.Item("MultiPlex") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "MultiPlex", Nothing)
                        oNewNode.InnerXml = oItem.sMultiPlex
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("MultiPlex").InnerXml = oItem.sMultiPlex
                    End If
                    If oNode.Item("Color") Is Nothing Then
                        Dim oNewNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Color", Nothing)
                        oNewNode.InnerXml = sColor
                        oNode.AppendChild(oNewNode)
                    Else
                        oNode.Item("Color").InnerXml = sColor
                    End If
                End If
            Next
            Dim oIOStream As System.IO.StreamWriter = New System.IO.StreamWriter(msXMLFilePath)
            Dim oWriter As XmlTextWriter = New XmlTextWriter(oIOStream)
            oWriter.Formatting = Formatting.Indented
            oXMLDoc.WriteTo(oWriter)
            oWriter.Close()
            oIOStream.Close()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & ":subSaveDmonItemsToXML", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
        End Try
        subReadDmonItemsFromXML(moItemInfo)
    End Sub
    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Try

            Status = gcsRM.GetString("csINITALIZING")

            Progress = 1

            DataLoaded = False
            mbScreenLoaded = False

            subProcessCommandLine()

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    Privilege = ePrivilege.Copy ' extra for buttons etc.
            '    If Privilege <> ePrivilege.Copy Then
            '        'didn't have clearance
            '        Privilege = ePrivilege.Edit
            '        If moPrivilege.ActionAllowed Then
            '            Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '        Else
            '            Privilege = ePrivilege.None
            '        End If
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'MSW 11/15/12 Make DMON viewer work as a standalone
            If UsePaintWorksDB Then
                'init new IPC and new Password 'RJO 03/23/12
                oIPC = New Paintworks_IPC.clsInterProcessComm
                moPassword = New clsPWUser
                Progress = 50

                colZones = New clsZones(String.Empty)
            Else
                btnFunction.Enabled = False
            End If

            mScreenSetup.InitializeForm(Me)
            subInitFormText()

            Application.DoEvents()

            Progress = 70

            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) Then
                subReadDmonItemsFromXML(moItemInfo)
            End If
            If UsePaintWorksDB Then
                gPrintHtml = New clsPrintHtml(msSCREEN_NAME, , , True, False, False, False, True, True)
            Else
                gPrintHtml = New clsPrintHtml(String.Empty, , , True, False, False, False, True, True)
            End If

            
            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_DOC")

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            ''''''''''''''''''''
            subEnableControls(False)
            'Draw the screen before we get to the slow stuff
            Application.DoEvents()

            subClearScreen()
            subRefreshFolders(True)
            mbEventBlocker = True
            If chkFolders.Items.Count > 0 Then
                chkFolders.SetItemChecked(0, True)
            End If
            subRefreshFiles()
            DataLoaded = True
            Application.DoEvents()
            mbEventBlocker = False
            subEnableControls(True)

            Me.stsStatus.Refresh()

            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.AbortRetryIgnore)

            Select Case lReply
                Case Response.Abort
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    End
                Case Response.Ignore
                Case Response.Retry
                    subInitializeForm()
            End Select
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub


    Private Sub subProcessCommandLine()
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sCultureArg As String = "/culture="

        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In My.Application.CommandLineArgs
            If s.ToLower.StartsWith(sCultureArg) Then
                Culture = s.Remove(0, sCultureArg.Length)
                Exit For
            End If
        Next

    End Sub

    Private Sub subShowNewPage()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True


        mbEventBlocker = False

    End Sub

#End Region
#Region " Events "

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: intercept the close event        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If gPrintHtml.Busy Then
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                    Handles MyBase.Load
        '********************************************************************************************
        'Description: Runs after class constructor (new)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally

        End Try

    End Sub
    Private Sub subShowChangeLog(ByVal nIndex As Integer)
        '********************************************************************************************
        'Description:  show the change log form
        '
        'Parameters: how many changes to show
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Standalone ChangeLog                                          4.01.05.00
        '********************************************************************************************

        Try
        
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                       gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
                                
        End Try

    End Sub
    Private Sub mnuLast7_Click(ByVal sender As Object, _
                                                ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub
    Private Sub mnuAllChanges_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub
    Private Sub mnuLast24_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))


    End Sub
    Private Sub subParseDMONFileName(ByRef sFileName As String, ByRef oFileInfo As tFileInfo)
        '********************************************************************************************
        'Description:  Fill out DMON file details from file name
        '
        'Parameters: filname, oFileInfo structure
        'Returns:    fill in oFileInfo.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Dim sDate As String = Format(Now, "yyyyMMdd HHmmss")
        ''dig out the file name from production data
        'Dim sFileName As String = msDeviceName & gsDMON_DELIMITER & _
        '                        sDate & gsDMON_DELIMITER & _
        '                        sStyle & gsDMON_DELIMITER & _
        '                        sColor & gsDMON_DELIMITER & _
        '                        ProdData(mnOptionPtr) & gsDMON_DELIMITER
        '    sFileName = sFileName & sVinStr & gsDMON_DELIMITER & moZone.ZoneNumber & ".dt"
        Try
            oFileInfo.sDevice = String.Empty
            oFileInfo.sStyle = String.Empty
            oFileInfo.sColor = String.Empty
            oFileInfo.sOption = String.Empty
            oFileInfo.sVIN = String.Empty
            oFileInfo.sZone = String.Empty
            oFileInfo.dtDate = IO.File.GetCreationTime(sFileName)
            Dim sPathSplit() As String = Split(sFileName, "\")
            oFileInfo.sFileName = sPathSplit(sPathSplit.GetUpperBound(0))
            oFileInfo.sLocation = sFileName.Substring(0, sFileName.Length - oFileInfo.sFileName.Length)

            Dim sSplit1() As String = Split(oFileInfo.sFileName, ".")

            If sSplit1.GetUpperBound(0) >= 1 Then
                Dim sTmpName As String = Replace(sSplit1(0), "--", "-")
                Dim sSplit2() As String = Split(sTmpName, "-", , )
                If sSplit2.GetUpperBound(0) > 3 Then
                    oFileInfo.sDevice = sSplit2(0)
                    Dim sDate() As String = Split(sSplit2(1), " ")
                    If sDate.GetUpperBound(0) = 1 AndAlso sDate(0).Length = 8 AndAlso sDate(1).Length = 6 Then
                        Dim nYear As Integer = CType(sDate(0).Substring(0, 4), Integer)
                        Dim nMonth As Integer = CType(sDate(0).Substring(4, 2), Integer)
                        Dim nDay As Integer = CType(sDate(0).Substring(6, 2), Integer)
                        Dim nHour As Integer = CType(sDate(1).Substring(0, 2), Integer)
                        Dim nMinute As Integer = CType(sDate(1).Substring(2, 2), Integer)
                        Dim nSecond As Integer = CType(sDate(1).Substring(4, 2), Integer)
                        oFileInfo.dtDate = New Date(nYear, nMonth, nDay, nHour, nMinute, nSecond)
                    End If
                    oFileInfo.sStyle = sSplit2(2)
                    oFileInfo.sColor = sSplit2(3)
                    oFileInfo.sOption = sSplit2(4)
                    If sSplit2.GetUpperBound(0) > 4 Then
                        oFileInfo.sVIN = sSplit2(5)
                        If sSplit2.GetUpperBound(0) > 5 Then
                            oFileInfo.sZone = sSplit2(6)
                        End If
                    End If
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Function bGetFileInfo(ByRef oFolderInfo As tFolderInfo) As Boolean
        '********************************************************************************************
        'Description:  Fill out file list for folder
        '
        'Parameters: FolderInfo structure
        'Returns:    fill in FileInfo array in FolderInfo.  Return true if OK, false if there's an error.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With oFolderInfo
            Dim lDate As Date = IO.File.GetLastWriteTime(.sPath)
            If .bZip Then
                'Update
                Dim oZip As ZipFile = Nothing
                Try
                    oZip = ZipFile.Read(.sPath)
                    Dim nCount As Integer = oZip.Count
                    ReDim .oFileInfo(nCount - 1)
                    Dim nIndex As Integer = -1
                    For Each oEntry As ZipEntry In oZip.Entries
                        If oEntry.FileName.Substring(oEntry.FileName.Length - 3).ToLower = ".dt" Then
                            nIndex = nIndex + 1
                            subParseDMONFileName(oEntry.FileName, .oFileInfo(nIndex))
                            .oFileInfo(nIndex).sLocation = .sPath
                            .oFileInfo(nIndex).dtDate = oEntry.LastModified
                        End If
                    Next
                    ReDim Preserve .oFileInfo(nIndex)
                    oZip = Nothing
                Catch ex As Exception
                Finally
                    If oZip IsNot Nothing Then
                        oZip.Dispose()
                        oZip = Nothing
                    End If
                End Try
            Else
                Dim sFiles() As String = IO.Directory.GetFiles(.sPath)
                Dim nMax As Integer = sFiles.GetUpperBound(0)
                ReDim .oFileInfo(nMax)
                For nIndex As Integer = 0 To nMax
                    subParseDMONFileName(sFiles(nIndex), .oFileInfo(nIndex))
                    .oFileInfo(nIndex).sLocation = .sPath
                    .oFileInfo(nIndex).dtDate = IO.File.GetLastWriteTime(sFiles(nIndex))
                Next
            End If
            .dtDate = lDate
        End With
        Return True
    End Function
    Private Sub subCheckCboList(ByRef oCbo As ComboBox, ByRef sText As String)
        '********************************************************************************************
        'Description:  Check for text in cbo, add if needed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each oItem As Object In oCbo.Items
            If oItem.ToString = sText Then
                Exit Sub
            End If
        Next
        oCbo.Items.Add(sText)
    End Sub
    Private Function bFilter(ByVal sMask As String, ByVal sTest As String) As Boolean
        '********************************************************************************************
        'Description:  check file parameter verses filter mask
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        sMask = sMask.Trim
        sTest = sTest.Trim
        If sTest Like sMask Then
            Return True
        End If
        Return False
    End Function
    Private Sub subDisplayFolder(ByVal nIndex As Integer, ByVal bFillCboLists As Boolean)
        '********************************************************************************************
        'Description:  get file list for a folder or zip checked in chkFolder
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If mtFolderInfo(nIndex).oFileInfo IsNot Nothing Then
            For Each oFile As tFileInfo In mtFolderInfo(nIndex).oFileInfo
                If bFillCboLists Then
                    subCheckCboList(cboZone, oFile.sZone)
                    subCheckCboList(cboDevice, oFile.sDevice)
                    subCheckCboList(cboStyle, oFile.sStyle)
                    subCheckCboList(cboOption, oFile.sOption)
                    subCheckCboList(cboColor, oFile.sColor)
                    If (chkAfter.Checked = False) Then
                        If (dtAfterDate.Value.Date > oFile.dtDate.Date) OrElse _
                            ((dtAfterDate.Value.Date = oFile.dtDate.Date) AndAlso _
                            (dtAfterDate.Value.TimeOfDay > oFile.dtDate.TimeOfDay)) Then
                            dtAfterDate.Value = oFile.dtDate
                            dtAfterTime.Value = oFile.dtDate
                        End If
                    End If
                    If chkBefore.Checked = False Then
                        If (dtBeforeDate.Value.Date < oFile.dtDate.Date) OrElse _
                            ((dtBeforeDate.Value.Date = oFile.dtDate.Date) AndAlso _
                            (dtBeforeDate.Value.TimeOfDay < oFile.dtDate.TimeOfDay)) Then
                            dtBeforeDate.Value = oFile.dtDate
                            dtBeforeTime.Value = oFile.dtDate
                        End If
                    End If
                End If
                Dim bAdd As Boolean = True
                bAdd = bFilter(cboZone.Text, oFile.sZone) AndAlso _
                       bFilter(cboDevice.Text, oFile.sDevice) AndAlso _
                       bFilter(cboStyle.Text, oFile.sStyle) AndAlso _
                       bFilter(cboColor.Text, oFile.sColor) AndAlso _
                       bFilter(cboOption.Text, oFile.sOption) AndAlso _
                       bFilter(txtvin.Text, oFile.sVIN)
                If bAdd Then
                    bAdd = bFilter("*.dt", oFile.sFileName) Or _
                           bFilter("*.xlsx", oFile.sFileName) Or _
                           bFilter("*.ods", oFile.sFileName) Or _
                           bFilter("*.csv", oFile.sFileName)
                End If
                If bAdd Then

                    If chkAfter.Checked Then
                        Dim dtStartDate As DateTime = dtAfterDate.Value.Date + dtAfterTime.Value.TimeOfDay
                        bAdd = (oFile.dtDate >= dtStartDate)
                    Else

                    End If
                    If bAdd AndAlso chkBefore.Checked Then
                        Dim dtEndDate As DateTime = dtBeforeDate.Value.Date + dtBeforeTime.Value.TimeOfDay
                        bAdd = (oFile.dtDate <= dtEndDate)
                    End If

                End If
                If bAdd Then
                    dgFiles.Rows.Add()
                    Dim nLastRow As Integer = dgFiles.RowCount - 1
                    dgFiles.Rows(nLastRow).Cells(gnCOL_ZONE).Value = oFile.sZone
                    dgFiles.Rows(nLastRow).Cells(gnCOL_DEVICE).Value = oFile.sDevice
                    dgFiles.Rows(nLastRow).Cells(gnCOL_STYLE).Value = oFile.sStyle
                    dgFiles.Rows(nLastRow).Cells(gnCOL_COLOR).Value = oFile.sColor
                    dgFiles.Rows(nLastRow).Cells(gnCOL_OPTION).Value = oFile.sOption
                    dgFiles.Rows(nLastRow).Cells(gnCOL_DATE).Value = oFile.dtDate
                    dgFiles.Rows(nLastRow).Cells(gnCOL_VIN).Value = oFile.sVIN
                    dgFiles.Rows(nLastRow).Cells(gnCOL_FILENAME).Value = oFile.sFileName
                    dgFiles.Rows(nLastRow).Cells(gnCOL_LOCATION).Value = oFile.sLocation
                End If
            Next
        End If

        lblFiles.Text = gpsRM.GetString("psDMON_FILES") & " (" & dgFiles.RowCount.ToString & ")"


    End Sub
    Private Sub subProcessFolder(ByVal nIndex As Integer, Optional ByVal bLoadList As Boolean = True)
        '********************************************************************************************
        'Description:  get file list for a folder or zip checked in chkFolder
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Dim nFolderInfoIndex As Integer = -1
        Dim bNew As Boolean = False
        Dim sItem As String = chkFolders.Items(nIndex).ToString
        If mtFolderInfo Is Nothing Then
            ReDim mtFolderInfo(0)
            bNew = True
            nFolderInfoIndex = 0
        Else
            For nTmp As Integer = 0 To mtFolderInfo.GetUpperBound(0)
                If sItem = mtFolderInfo(nTmp).sPath Then
                    nFolderInfoIndex = nTmp
                    Exit For
                End If
            Next
            If nFolderInfoIndex = -1 Then
                nFolderInfoIndex = mtFolderInfo.GetUpperBound(0) + 1
                ReDim Preserve mtFolderInfo(nFolderInfoIndex)
                bNew = True
            End If
        End If
        With mtFolderInfo(nFolderInfoIndex)
            If bNew Then
                .sPath = sItem
                Dim sExtTmp1() As String = Split(sItem, ".")
                If sExtTmp1(sExtTmp1.GetUpperBound(0)).ToLower = "zip" Then
                    .bZip = True
                End If
            End If
        End With
        If bLoadList Then
            If bGetFileInfo(mtFolderInfo(nFolderInfoIndex)) = False Then
                chkFolders.SetItemChecked(nIndex, False)
            End If
        End If

        subDisplayFolder(nFolderInfoIndex, bLoadList)

        lblFiles.Text = gpsRM.GetString("psDMON_FILES") & " (" & dgFiles.RowCount.ToString & ")"
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  Check all folders
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        For nIndex As Integer = 0 To chkFolders.Items.Count - 1
            chkFolders.SetItemChecked(nIndex, True)
        Next
        subRefreshFolders()
        subRefreshFiles()
        mbEventBlocker = False
    End Sub
    Private Sub mnuUnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnselectAll.Click
        '********************************************************************************************
        'Description:  Uncheck all folders
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        For nIndex As Integer = 0 To chkFolders.Items.Count - 1
            chkFolders.SetItemChecked(nIndex, False)
        Next
        subRefreshFolders()
        subRefreshFiles()
        mbEventBlocker = False
    End Sub


    Private Sub chkFolders_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles chkFolders.ItemCheck
        '********************************************************************************************
        'Description:  folder checklist changed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        If e.NewValue = CheckState.Checked Then
            'Add items to file list.  No need to clear anything out
            subProcessFolder(e.Index)
        Else
            'Refresh the list
            subRefreshFiles(e.Index)
        End If
    End Sub

    Private Sub btnApplyFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyFilter.Click
        '********************************************************************************************
        'Description:  refresh file list display
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subFilterFiles()
    End Sub
    Private Sub subFilterFiles()
        '********************************************************************************************
        'Description:  refresh file list display
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nCount As Integer = chkFolders.Items.Count - 1

        dgFiles.Rows.Clear()
        For nItem As Integer = 0 To nCount
            If chkFolders.GetItemChecked(nItem) Then
                subProcessFolder(nItem, False)
            End If
        Next

    End Sub
    Private Sub subRefreshFiles(Optional ByVal nSkipIndex As Integer = -1)
        '********************************************************************************************
        'Description:  refresh file list
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nCount As Integer = chkFolders.Items.Count - 1

        dgFiles.Rows.Clear()
        For nItem As Integer = 0 To nCount
            If nSkipIndex <> nItem AndAlso chkFolders.GetItemChecked(nItem) Then
                subProcessFolder(nItem)
            End If
        Next

    End Sub
    Private Sub subRefreshFolders(Optional ByVal bInit As Boolean = False)
        '********************************************************************************************
        'Description:  refresh folder list
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/04/13  MSW     subRefreshFolders - remove missing folders from the list.
        '********************************************************************************************
        Dim sSourcePath As String = String.Empty

        mPWCommon.GetDefaultFilePath(msDMONData, eDir.DmonData, String.Empty, String.Empty)
        mPWCommon.GetDefaultFilePath(msDMONArchive, eDir.DmonArchive, String.Empty, String.Empty)
        Dim nCount As Integer = chkFolders.Items.Count - 1
        Dim bFound As Boolean = False
        If msDMONData <> String.Empty AndAlso IO.Directory.Exists(msDMONData) Then
            Dim sFileList() As String = IO.Directory.GetFiles(msDMONData, "*.dt", IO.SearchOption.TopDirectoryOnly)
            For nItem As Integer = 0 To nCount
                If msDMONData = chkFolders.Items(nItem).ToString Then
                    bFound = True
                End If
            Next
            If bFound = False Then
                chkFolders.Items.Add(msDMONData)
            End If
        End If
        If bInit Then
            Dim oNames As String() = Application.UserAppDataRegistry.GetValueNames
            If oNames.GetUpperBound(0) >= 0 Then
                For nItem As Integer = 0 To oNames.GetUpperBound(0)
                    Dim sTmp As String = Application.UserAppDataRegistry.GetValue(oNames(nItem)).ToString
                    'Check for missing folder
                    If IO.Directory.Exists(sTmp) Then
                        Dim sFileList() As String = IO.Directory.GetFiles(sTmp, "*.dt", IO.SearchOption.TopDirectoryOnly)
                        For nValue As Integer = 0 To nCount
                            If sTmp = chkFolders.Items(nValue).ToString Then
                                bFound = True
                            End If
                        Next
                        If bFound = False Then
                            chkFolders.Items.Add(sTmp)
                            chkFolders.SetItemChecked(chkFolders.Items.Count - 1, True)
                        End If
                    Else
                        'Just remove any missing folder from the list
                        Application.UserAppDataRegistry.DeleteValue(oNames(nItem))
                    End If
                Next
            End If
        End If
        If msDMONArchive <> String.Empty AndAlso IO.Directory.Exists(msDMONArchive) Then
            Dim sZipList() As String = IO.Directory.GetFiles(msDMONArchive, "*.zip", IO.SearchOption.TopDirectoryOnly)
            For Each sTmp As String In sZipList
                nCount = chkFolders.Items.Count - 1
                bFound = False
                For nItem As Integer = 0 To nCount
                    If sTmp = chkFolders.Items(nItem).ToString Then
                        bFound = True
                    End If
                Next
                If bFound = False Then
                    chkFolders.Items.Add(sTmp)
                End If
            Next
        End If
        DataLoaded = True

    End Sub

    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
               ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subEnableControls(False)

        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                Exit Sub

            Case "btnRefresh"
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                ''''''''''''''''''''
                subEnableControls(False)
                subClearScreen()
                subRefreshFolders()
                subRefreshFiles()
                subEnableControls(True)
                Me.Cursor = System.Windows.Forms.Cursors.Default
            Case "btnShow"
                splMain.Panel1Collapsed = False
                btnHide.Visible = True
                btnShow.Visible = False
            Case "btnHide"
                splMain.Panel1Collapsed = True
                btnHide.Visible = False
                btnShow.Visible = True

        End Select

        subEnableControls(True)
        tlsMain.Refresh()


    End Sub
    Private Sub frmMain_Closing(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Try
            If colArms Is Nothing Then Exit Sub
            If colArms.Count = 0 Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next

        Catch ex As Exception

        End Try

    End Sub
    Public Sub New()
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)
        mbEventBlocker = True
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mbEventBlocker = False
    End Sub

    Private Sub frmMain_Layout(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '   <System.Diagnostics.DebuggerStepThrough()>
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'The panel objects handle everything with this screen
    End Sub

    Private Sub colRobots_BumpProgress() Handles colArms.BumpProgress
        '********************************************************************************************
        'Description:  bump me!
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Progress > 0 Then Progress += 5

    End Sub
    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogin.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If LoggedOnUser = String.Empty Then
            If Privilege = ePrivilege.None Then
                Privilege = ePrivilege.Edit
            End If
        End If

    End Sub
    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogOut.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If LoggedOnUser <> String.Empty Then
            moPassword.LogUserOut()
        End If

    End Sub
    Private Sub mnuRemote_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                   Handles mnuRemote.Click
        '********************************************************************************************
        'Description:  Select a remote zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim bTmp As Boolean = mbRemoteZone
        mbEventBlocker = True
        If SetRemoteServer() Then
            mbRemoteZone = True
            moPassword = Nothing

            GC.Collect()
            System.Windows.Forms.Application.DoEvents()
            mPWRobotCommon.UrbanRenewal()
            System.Windows.Forms.Application.DoEvents()
            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If
        mbEventBlocker = False
    End Sub
    Private Sub mnuLocal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLocal.Click
        '********************************************************************************************
        'Description:  Select a local zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbRemoteZone = False
        moPassword = Nothing
        RemotePWPath = String.Empty

        GC.Collect()
        System.Windows.Forms.Application.DoEvents()
        mPWRobotCommon.UrbanRenewal()
        System.Windows.Forms.Application.DoEvents()
        subInitializeForm()

    End Sub
    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '                  now handled in dostatusbar 2
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim cachePrivilege As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then
            '    cachePrivilege = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
            '    End If
            'End If

            If moPassword.Privilege = ePrivilege.None Then
                cachePrivilege = ePrivilege.None
            Else
                If moPassword.ActionAllowed Then
                    cachePrivilege = moPassword.Privilege
                Else
                    cachePrivilege = ePrivilege.None
                End If
            End If

            Privilege = ePrivilege.Remote

            If Privilege = ePrivilege.Remote Then
                If colZones.PaintShopComputer = False Then
                    bAllow = True
                End If
            End If

            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If

        DoStatusBar2(stsStatus, LoggedOnUser, Privilege, bRem, bAllow)


    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_VIEW_DMON, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME


                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                Case Keys.Escape
                    Me.Close()
                Case Else

            End Select
        End If
    End Sub
#End Region
#Region " Temp stuff for old password object "

    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            Privilege = ePrivilege.Copy ' extra for buttons etc.
            If Privilege <> ePrivilege.Copy Then
                'didn't have clearance
                Privilege = ePrivilege.Edit
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If

    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.LogOut
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If

    End Sub

    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles oIPC.NewMessage
        '********************************************************************************************
        'Description:  A new message has been received from another Paintworks Application
        '
        'Parameters: DS - PWUser Dataset, ProcessName Paintworks screen process name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Dim DR As DataRow = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)

            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    'TODO - Handle language change requests here.
                    'Call subDoControlAction(DR)
                Case oIPC.PASSWORD_MSG_SCHEMA.ToLower
                    Call moPassword.ProcessPasswordMessage(DR)
                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                                           "Unrecognized schema [" & Schema & "].")
            End Select
        End If 'Me.InvokeRequired

    End Sub

#End Region

    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (DataLoaded) Then
            Dim lRet As DialogResult

            lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                                gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                                MessageBoxButtons.YesNoCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

            Select Case lRet
                Case Response.Yes 'Response.Yes
                    mbInAskForSave = True
                    mbInAskForSave = False
                    Return True
                Case Response.Cancel
                    'false aborts closing form , changing zone etc
                    Status = gcsRM.GetString("csSAVE_CANCEL")
                    Return False
                Case Else
                    Return True
            End Select
        Else
            Return True
        End If
    End Function


    Private Sub dgFiles_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgFiles.ColumnHeaderMouseClick
        '********************************************************************************************
        'Description:  sort the table
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim newColumn As DataGridViewColumn = _
            dgFiles.Columns(e.ColumnIndex)
        Dim oldColumn As DataGridViewColumn = dgFiles.SortedColumn
        Dim direction As System.ComponentModel.ListSortDirection
        ' If oldColumn is null, then the DataGridView is not currently sorted.

        If oldColumn IsNot Nothing Then

            ' Sort the same column again, reversing the SortOrder.
            If oldColumn Is newColumn Then
                'Don't know why this is needed, but it works
                If e.Button = Windows.Forms.MouseButtons.Right Then
                    If dgFiles.SortOrder = SortOrder.Ascending Then
                        direction = System.ComponentModel.ListSortDirection.Descending
                    Else
                        direction = System.ComponentModel.ListSortDirection.Ascending
                    End If
                Else
                    If dgFiles.SortOrder = SortOrder.Ascending Then
                        direction = System.ComponentModel.ListSortDirection.Ascending
                    Else
                        direction = System.ComponentModel.ListSortDirection.Descending
                    End If
                End If
            Else
                ' Sort a new column and remove the old SortGlyph.
                direction = System.ComponentModel.ListSortDirection.Ascending
                oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None
            End If
        Else
            direction = System.ComponentModel.ListSortDirection.Ascending
        End If

        ' Sort the selected column.
        dgFiles.Sort(newColumn, direction)
        If direction = System.ComponentModel.ListSortDirection.Ascending Then
            newColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending
        Else
            newColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending
        End If


    End Sub

    Private Sub cbo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
        cboDevice.KeyPress, cboZone.KeyPress, cboColor.KeyPress, cboOption.KeyPress, cboStyle.KeyPress, _
        txtvin.KeyPress
        '********************************************************************************************
        'Description:  Apply filter if enter is pressed
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            subFilterFiles()
        End If
    End Sub

    Private Sub btnClearFilters_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearFilters.Click
        '********************************************************************************************
        'Description:  Clear Filters
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        cboZone.Text = "*"
        cboDevice.Text = "*"
        cboStyle.Text = "*"
        cboOption.Text = "*"
        cboColor.Text = "*"
        txtvin.Text = "*"
        chkAfter.Checked = False
        chkBefore.Checked = False
        subFilterFiles()
    End Sub

    Private Sub mnuAddZip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAddZip.Click
        '********************************************************************************************
        'Description:  Add a zip file to the folder list
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFileName As String = String.Empty
        Try
            Dim o As New OpenFileDialog
            With o
                'Zip Files|*.Zip
                .Filter = gpsRM.GetString("psOPEN_ZIP_FILE_FLTR")
                .Title = gpsRM.GetString("psOPEN_ZIP_FILE_CAP")
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".ZIP"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFileName = .FileName
                Else
                    Exit Sub
                End If
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuAddZip_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub

        End Try
        For nItem As Integer = 0 To chkFolders.Items.Count - 1
            If sFileName = chkFolders.Items(nItem).ToString Then
                Exit Sub
            End If
        Next
        chkFolders.Items.Add(sFileName)
        chkFolders.SetItemChecked(chkFolders.Items.Count - 1, True)

    End Sub

    Private Sub mnuAddFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAddFolder.Click
        '********************************************************************************************
        'Description:  Add a folder to the folder list
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFolderName As String = String.Empty
        Try
            Dim oFB As New FolderBrowserDialog
            With oFB
                .ShowNewFolderButton = False
                Dim sPathTmp As String = String.Empty
                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                    .SelectedPath = sPathTmp
                End If
                .Description = gpsRM.GetString("psSELECT_FOLDER")
                Dim oVal As DialogResult = .ShowDialog()
                If oVal = Windows.Forms.DialogResult.OK Then
                    sFolderName = .SelectedPath
                Else
                    Exit Sub
                End If
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuAddFolder_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub
        End Try
        Dim oReturn As DialogResult = MessageBox.Show(gpsRM.GetString("psREMEMBER_FOLDER"), gpsRM.GetString("psDMONVIEWER"), _
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        Select Case oReturn
            Case Windows.Forms.DialogResult.Yes
                Dim oNames As String() = Application.UserAppDataRegistry.GetValueNames
                Dim sNewName As String = "Folder1"
                Dim bSkip As Boolean = False
                If oNames.GetUpperBound(0) >= 0 Then
                    Try
                        Dim nName As Integer = CType(oNames(oNames.GetUpperBound(0)).Substring(6), Integer)
                        sNewName = "Folder" & (nName + 1).ToString
                    Catch ex As Exception
                        sNewName = "Folder999"
                    End Try
                    For nItem As Integer = 0 To oNames.GetUpperBound(0)
                        Dim sTmp As String = Application.UserAppDataRegistry.GetValue(oNames(nItem)).ToString
                        If sTmp.ToLower = sFolderName Then
                            bSkip = True
                            Exit For
                        End If
                    Next
                    If bSkip = False Then
                        Dim bNameOK As Boolean = False
                        Do While bNameOK = False
                            bNameOK = True
                            For nValue As Integer = 0 To oNames.GetUpperBound(0)
                                If oNames(nValue) = sNewName Then
                                    bNameOK = False
                                    Dim nName As Integer = CType(sNewName.Substring(6), Integer)
                                    sNewName = "Folder" & (nName + 1).ToString
                                    Exit For
                                End If
                            Next
                        Loop
                    End If
                End If
                If bSkip = False Then
                    Application.UserAppDataRegistry.SetValue(sNewName, sFolderName)
                End If
            Case Windows.Forms.DialogResult.No
            Case Else
                Exit Sub
        End Select
        For nItem As Integer = 0 To chkFolders.Items.Count - 1
            If sFolderName = chkFolders.Items(nItem).ToString Then
                Exit Sub
            End If
        Next
        chkFolders.Items.Add(sFolderName)
        chkFolders.SetItemChecked(chkFolders.Items.Count - 1, True)

    End Sub

    Private Sub mnuOpenDmonFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOpenDmonFile.Click
        '********************************************************************************************
        'Description:  Open a DMON file directly from a file open dialog
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/13  MSW     Deal with duplicate column names
        '********************************************************************************************
        Dim sFileName As String = String.Empty
        Try
            Dim o As New OpenFileDialog
            With o
                'All Files|*.*|DMON file|*.DT
                .Filter = gpsRM.GetString("psOPEN_DMON_FILE_FLTR") & "|" & sGetImportExportFilter()
                .Title = gpsRM.GetString("psOPEN_DMON_FILE_CAP")
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".DT"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFileName = .FileName
                Else
                    Exit Sub
                End If
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuOpenDmonFile_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub
        End Try

        subOpenDt(sFileName)

    End Sub
    Friend Sub subOpenDt(ByRef sFileName As String)
        'MSW 5/6/14 avoid Foreign numbers
        Dim odtItemData As New DataTable
        Dim odtScheduleData As New DataTable
        Dim odtTable As New DataTable
        Dim odtTmp As DataTable = Nothing
        Dim sr As System.IO.StreamReader = Nothing
        Dim nLastColumn As Integer = 0
        Const nSTART As Integer = 0
        Const nITEMS As Integer = 1
        Const nSCHEDULE As Integer = 2
        Const nDATA As Integer = 3
        Dim oFileInfo As New tFileInfo
        Dim oItemInfo() As tItemInfo = Nothing
        'MSW 5/6/14 avoid Foreign numbers
        Dim sCurrentCulture As String = My.Application.Culture.Name
        My.Application.ChangeCulture("en-US")

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            'process the file
            If sFileName <> String.Empty Then
                subParseDMONFileName(sFileName, oFileInfo)
                Dim sSplit() As String = Split(sFileName, ".")
                Dim sExt As String = sSplit(sSplit.GetUpperBound(0))
                If sExt.ToLower = "dt" Then
                    'Open the stream and read it back.
                    sr = System.IO.File.OpenText(sFileName)
                    Dim nStep As Integer = nSTART
                    Do While sr.Peek() >= 0

                        Dim sLine As String = sr.ReadLine()
                        'deal with edits from a spreadsheet, may convert to csv
                        sLine = sLine.Replace(",", vbTab)
                        Dim sText() As String = Split(sLine, vbTab)
                        If sText.GetUpperBound(0) >= 0 Then
                            Select Case nStep
                                Case nSTART
                                    Select Case sText(0).Trim
                                        Case "SCHEDULE ITEM"
                                            'Read some Item Data
                                            nStep = nITEMS
                                            odtItemData.Rows.Add()
                                        Case "Number"
                                            'Still have two possibilities
                                            If sText.GetUpperBound(0) >= 1 Then
                                                Select Case sText(1).Trim
                                                    Case "Item"
                                                        'Read some schedule Data
                                                        nStep = nSCHEDULE
                                                        odtTmp = odtScheduleData
                                                    Case "Tick"
                                                        'Read the main data
                                                        nStep = nDATA
                                                        odtTmp = odtTable
                                                        ReDim oItemInfo(sText.GetUpperBound(0))
                                                        For nCol As Integer = 0 To sText.GetUpperBound(0)
                                                            Dim sTmp As String = sText(nCol)
                                                            Dim nLen As Integer = InStr(sTmp, "[")
                                                            Dim nEndUnits As Integer = InStr(sTmp, "]")
                                                            If nLen > 0 Then
                                                                oItemInfo(nCol).sName = sTmp.Substring(0, nLen - 1).Trim
                                                                If nEndUnits > 0 Then
                                                                    oItemInfo(nCol).sUnits = sTmp.Substring(nLen, (nEndUnits - nLen) - 1)
                                                                Else
                                                                    oItemInfo(nCol).sUnits = String.Empty
                                                                End If
                                                            Else
                                                                oItemInfo(nCol).sName = sTmp
                                                                oItemInfo(nCol).sUnits = String.Empty
                                                            End If
                                                            oItemInfo(nCol).fScale = 1
                                                            oItemInfo(nCol).fOffset = 0
                                                            oItemInfo(nCol).sType = gpsRM.GetString("psINTEGER")
                                                            oItemInfo(nCol).fMin = 99999
                                                            oItemInfo(nCol).fMax = -99999
                                                            oItemInfo(nCol).sMultiPlex = String.Empty
                                                        Next
                                                    Case Else
                                                        odtTmp = Nothing
                                                End Select ' sText(1).Trim
                                                'Use this row to set the column names
                                                nLastColumn = sText.GetUpperBound(0)
                                                If odtTmp IsNot Nothing Then
                                                    For nCol As Integer = 0 To nLastColumn
                                                        'MSW 3/28/13 Deal with duplicate column names
                                                        If odtTmp.Columns.Contains(sText(nCol)) Then
                                                            odtTmp.Columns.Add(sText(nCol) & "_C" & nCol.ToString)
                                                        Else
                                                            odtTmp.Columns.Add(sText(nCol))
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        Case Else
                                            'Skip it
                                    End Select 'sText(0).Trim
                                Case nITEMS
                                    'If there are two columns use column 1 as a column name, use column 2 as data
                                    If (sText.GetUpperBound(0) >= 1) AndAlso (sText(0) <> String.Empty) Then
                                        If (odtItemData.Columns.Contains(sText(0)) = False) Then
                                            'Add the column
                                            odtItemData.Columns.Add(sText(0))
                                        End If
                                        odtItemData.Rows(odtItemData.Rows.Count - 1).Item(sText(0)) = sText(1)
                                    Else
                                        nStep = nSTART
                                    End If
                                Case nSCHEDULE
                                    If odtScheduleData IsNot Nothing AndAlso (sText.GetUpperBound(0) >= 1) AndAlso (sText(0) <> String.Empty) Then
                                        odtScheduleData.Rows.Add()
                                        Dim nRow As Integer = odtTmp.Rows.Count - 1
                                        For nCount As Integer = 0 To nLastColumn
                                            If sText.GetUpperBound(0) < nCount Then
                                                odtScheduleData.Rows(nRow).Item(nCount) = String.Empty
                                            Else
                                                odtScheduleData.Rows(nRow).Item(nCount) = sText(nCount)
                                            End If
                                        Next
                                    Else
                                        nStep = nSTART
                                    End If
                                Case nDATA
                                    If odtTable IsNot Nothing AndAlso (sText.GetUpperBound(0) >= 1) AndAlso (sText(0) <> String.Empty) Then
                                        odtTable.Rows.Add()
                                        Dim nRow As Integer = odtTable.Rows.Count - 1
                                        For nCount As Integer = 0 To nLastColumn
                                            If sText.GetUpperBound(0) >= nCount Then
                                                If IsNumeric(sText(nCount)) Then
                                                    Dim fTmp As Single = CType(sText(nCount), Single)
                                                    If fTmp > oItemInfo(nCount).fMax Then
                                                        oItemInfo(nCount).fMax = fTmp
                                                    End If
                                                    If fTmp < oItemInfo(nCount).fMin Then
                                                        oItemInfo(nCount).fMin = fTmp
                                                    End If
                                                    Dim nDecimalPoint As Integer = InStr(sText(nCount), ".")
                                                    If nDecimalPoint > 0 Then
                                                        'Dim nInt1 As integer= CType(sText(nCount).Substring(0,nDecimalPoint-1) integer)
                                                        Dim nInt2 As Integer = CType(sText(nCount).Substring(nDecimalPoint), Integer)
                                                        If nInt2 <> 0 Then
                                                            oItemInfo(nCount).sType = gpsRM.GetString("psFLOAT")
                                                        End If
                                                    End If
                                                    odtTable.Rows(nRow).Item(nCount) = fTmp
                                                Else
                                                    odtTable.Rows(nRow).Item(nCount) = 0
                                                End If
                                            End If
                                        Next
                                    Else
                                        nStep = nSTART
                                    End If
                            End Select ' nStep
                        End If
                    Loop
                    sr.Close()
                Else
                    Dim sTitleReq As String = String.Empty
                    Dim sTableStart(2) As String
                    sTableStart(0) = "Item"
                    sTableStart(1) = "$DMONITEM number"
                    sTableStart(2) = "Number"

                    Dim sHeader() As String = Nothing
                    Dim oDTs() As DataTable = Nothing
                    GetDTFromCSV(sTitleReq, sTableStart, sHeader, oDTs, sFileName, , , , False, True, True)

                    Debug.Print(sHeader.GetUpperBound(0).ToString)
                    Debug.Print(oDTs.GetUpperBound(0).ToString)

                    For nTable As Integer = 0 To sHeader.GetUpperBound(0)
                        If InStr(sHeader(nTable), gpsRM.GetString("psTABLE")) > 0 Then
                            If nTable <= oDTs.GetUpperBound(0) Then
                                odtTable = oDTs(nTable)
                            End If
                        ElseIf InStr(sHeader(nTable), gpsRM.GetString("psSCHEDULE")) > 0 Then
                            If nTable <= oDTs.GetUpperBound(0) Then
                                odtScheduleData = oDTs(nTable)
                            End If
                        ElseIf InStr(sHeader(nTable), gpsRM.GetString("psITEMS")) > 0 Then
                            If nTable <= oDTs.GetUpperBound(0) Then
                                odtItemData = oDTs(nTable)
                            End If
                        ElseIf InStr(sHeader(nTable), gpsRM.GetString("psCONFIG")) > 0 Then
                            ReDim oItemInfo(oDTs(nTable).Rows.Count - 1)
                            Dim nNameCol As Integer = -1
                            Dim nUnitCol As Integer = -1
                            Dim nTypeCol As Integer = -1
                            Dim nScaleCol As Integer = -1
                            Dim nOffsetCol As Integer = -1
                            Dim nMinDataCol As Integer = -1
                            Dim nMaxDataCol As Integer = -1
                            Dim nMinDrawCol As Integer = -1
                            Dim nMaxDrawCol As Integer = -1
                            Dim nMultiCol As Integer = -1
                            Dim nColorCol As Integer = -1
                            For nCol As Integer = 0 To oDTs(nTable).Columns.Count - 1
                                With oDTs(nTable).Columns(nCol)
                                    If .Caption = gpsRM.GetString("psNAME") Then
                                        nNameCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psUNITS") Then
                                        nUnitCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psTYPE") Then
                                        nTypeCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psSCALE") Then
                                        nScaleCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psOFFSET") Then
                                        nOffsetCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psMINDATA") Then
                                        nMinDataCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psMAXDATA") Then
                                        nMaxDataCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psMINDRAW") Then
                                        nMinDrawCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psMAXDRAW") Then
                                        nMaxDrawCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psMULTIPLEX") Then
                                        nMultiCol = nCol
                                    ElseIf .Caption = gpsRM.GetString("psCOLOR") Then
                                        nColorCol = nCol
                                    End If
                                End With
                            Next
                            Dim nMultiStart As Integer = 0
                            For nItem As Integer = 0 To oDTs(nTable).Rows.Count - 1
                                With oDTs(nTable).Rows(nItem)
                                    If nNameCol >= 0 Then
                                        oItemInfo(nItem).sName = .Item(nNameCol).ToString
                                    End If
                                    If nUnitCol >= 0 Then
                                        oItemInfo(nItem).sUnits = .Item(nUnitCol).ToString
                                    End If
                                    If nTypeCol >= 0 Then
                                        oItemInfo(nItem).sType = .Item(nTypeCol).ToString
                                    End If
                                    If nScaleCol >= 0 Then
                                        oItemInfo(nItem).fScale = CType(.Item(nScaleCol), Single)
                                    End If
                                    If nOffsetCol >= 0 Then
                                        oItemInfo(nItem).fOffset = CType(.Item(nOffsetCol), Single)
                                    End If
                                    If nMinDataCol >= 0 Then
                                        oItemInfo(nItem).fMin = CType(.Item(nMinDataCol), Single)
                                    End If
                                    If nMaxDataCol >= 0 Then
                                        oItemInfo(nItem).fMax = CType(.Item(nMaxDataCol), Single)
                                    End If
                                    If nMinDrawCol >= 0 Then
                                        oItemInfo(nItem).fPlotMin = CType(.Item(nMinDrawCol), Single)
                                    End If
                                    If nMaxDrawCol >= 0 Then
                                        oItemInfo(nItem).fPlotMax = CType(.Item(nMaxDrawCol), Single)
                                    End If
                                    If nMultiCol >= 0 Then
                                        oItemInfo(nItem).sMultiPlex = .Item(nMultiCol).ToString
                                        Dim sMulti() As String = Split(oItemInfo(nItem).sMultiPlex, ";")
                                        oItemInfo(nItem).nMultiPlexCount = sMulti.GetUpperBound(0) + 1
                                        oItemInfo(nItem).nMultiPlexStart = nMultiStart
                                        nMultiStart = nMultiStart + oItemInfo(nItem).nMultiPlexCount
                                    End If
                                    If nColorCol >= 0 Then
                                        oItemInfo(nItem).oColor = frmChart.StringToColor(.Item(nColorCol).ToString)
                                    Else
                                        oItemInfo(nItem).oColor = Color.Black
                                    End If
                                End With
                            Next
                        End If
                    Next
                End If

                Dim nColIdx As Integer = oItemInfo.GetUpperBound(0)
                Do While (oItemInfo(nColIdx).sName = "") AndAlso (oItemInfo(nColIdx).fMin > oItemInfo(nColIdx).fMax)
                    odtTable.Columns.RemoveAt(nColIdx)
                    nColIdx = nColIdx - 1
                    ReDim Preserve oItemInfo(nColIdx)
                Loop

                'Merge saved item configs with info from table
                For nItem As Integer = 0 To oItemInfo.GetUpperBound(0)
                    'For Each oReadItem As tItemInfo In oItemInfo
                    If (oItemInfo(nItem).fMax <= 1) AndAlso (oItemInfo(nItem).fMin >= 0) AndAlso _
                        (oItemInfo(nItem).sUnits.Trim = String.Empty) AndAlso _
                        (oItemInfo(nItem).sType = gpsRM.GetString("psINTEGER")) Then
                        oItemInfo(nItem).sType = gpsRM.GetString("psBIT")
                    End If
                    oItemInfo(nItem).fPlotMax = oItemInfo(nItem).fMax
                    oItemInfo(nItem).fPlotMin = oItemInfo(nItem).fMin
                    If moItemInfo IsNot Nothing Then
                        For Each oSavedItem As tItemInfo In moItemInfo
                            If oItemInfo(nItem).sName.Trim.ToLower = oSavedItem.sName.Trim.ToLower Then
                                'Matching Item, update scaling
                                If oSavedItem.fPlotMax > oItemInfo(nItem).fPlotMax Then
                                    oItemInfo(nItem).fPlotMax = oSavedItem.fPlotMax
                                End If
                                If oSavedItem.fPlotMin < oItemInfo(nItem).fPlotMin Then
                                    oItemInfo(nItem).fPlotMin = oSavedItem.fPlotMin
                                End If
                                If oSavedItem.fScale <> 0 Then
                                    oItemInfo(nItem).fScale = oSavedItem.fScale
                                End If
                                oItemInfo(nItem).fOffset = oSavedItem.fOffset
                                oItemInfo(nItem).sUnits = oSavedItem.sUnits
                                oItemInfo(nItem).sType = oSavedItem.sType
                                oItemInfo(nItem).sMultiPlex = oSavedItem.sMultiPlex
                                oItemInfo(nItem).oColor = oSavedItem.oColor
                            End If
                        Next
                    End If
                Next

                'A litle cleanup
                While (odtTable.Columns.Count > (oItemInfo.GetUpperBound(0) + 1))
                    odtTable.Columns.RemoveAt(odtTable.Columns.Count - 1)
                End While

                Dim bDelete As Boolean = True
                While bDelete
                    For nRow As Integer = 0 To odtScheduleData.Rows.Count - 1
                        If odtScheduleData.Rows(nRow).Item(odtScheduleData.Columns.Count - 1).ToString.Trim <> "" Then
                            bDelete = False
                            Exit For
                        End If
                    Next
                    If bDelete Then
                        odtScheduleData.Columns.RemoveAt(odtScheduleData.Columns.Count - 1)
                    End If
                End While

                bDelete = True
                While bDelete
                    For nRow As Integer = 0 To odtItemData.Rows.Count - 1
                        If odtItemData.Rows(nRow).Item(odtItemData.Columns.Count - 1).ToString.Trim <> "" Then
                            bDelete = False
                            Exit For
                        End If
                    Next
                    If bDelete Then
                        If (odtItemData.Columns.Count > 0) Then
                            odtItemData.Columns.RemoveAt(odtItemData.Columns.Count - 1)
                        Else
                            bDelete = False
                        End If
                    End If
                End While

                'Open the chart
                Dim mfrmChart As New frmChart
                mfrmChart.ItemDT = odtItemData
                mfrmChart.ScheduleDT = odtScheduleData
                mfrmChart.TableDT = odtTable
                mfrmChart.FileInfo = oFileInfo
                mfrmChart.ItemInfo = oItemInfo

                Me.Cursor = System.Windows.Forms.Cursors.Default
                mfrmChart.Show()
            End If

        Catch ex As Exception

        Finally
            If (sr IsNot Nothing) Then
                sr.Close()
            End If
            Me.Cursor = System.Windows.Forms.Cursors.Default
            'MSW 5/6/14 avoid Foreign numbers
            My.Application.ChangeCulture(sCurrentCulture)
        End Try


    End Sub


    Private Sub dgFiles_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgFiles.CellDoubleClick
        '********************************************************************************************
        'Description: select a file
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nRow As Integer = e.RowIndex
        If nRow < 0 Then
            Exit Sub
        End If
        Dim oDR As DataGridViewRow = dgFiles.Rows(nRow)
        Dim sLocation As String = oDR.Cells(gnCOL_LOCATION).Value.ToString
        Dim sFileName As String = oDR.Cells(gnCOL_FILENAME).Value.ToString
        Dim sOpenPath As String = String.Empty
        Dim sExtTmp1() As String = Split(sLocation, ".")
        If sExtTmp1(sExtTmp1.GetUpperBound(0)).ToLower = "zip" Then
            'Zip
            sOpenPath = sGetTmpFileName(sFileName, String.Empty)
            Dim oZip As ZipFile = Nothing
            Try
                oZip = ZipFile.Read(sLocation)
                For Each oZipEntry As ZipEntry In oZip.Entries
                    If oZipEntry.FileName = sFileName Then
                        oZipEntry.Extract(sOpenPath, ExtractExistingFileAction.OverwriteSilently)
                        oZip.Dispose()
                        Exit For
                    End If
                Next
                sOpenPath = sOpenPath & "\" & sFileName
            Catch ex As Exception
            Finally
                If oZip IsNot Nothing Then
                    oZip.Dispose()
                    oZip = Nothing
                End If
            End Try
        Else
            If sLocation.Substring(sLocation.Length - 1) <> "\" Then
                sLocation = sLocation & "\"
            End If
            sOpenPath = sLocation & sFileName
        End If

        subOpenDt(sOpenPath)

    End Sub

    Private Sub chkFolders_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles chkFolders.MouseDown
        '********************************************************************************************
        'Description: identify the item selected for the pop-up menu
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            mnChkItem = -1
            For nItem As Integer = 0 To chkFolders.Items.Count - 1
                Dim oRect As Rectangle = chkFolders.GetItemRectangle(nItem)
                If (e.Location.X > oRect.Location.X) And _
                   (e.Location.Y > oRect.Location.Y) And _
                   (e.Location.X < (oRect.Location.X + oRect.Width)) And _
                   (e.Location.Y < (oRect.Location.Y + oRect.Height)) Then
                    mnChkItem = nItem
                    Exit For
                End If
            Next
            mnuRemoveItem.Enabled = mnChkItem >= 0
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: chkFolders_MouseDown", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Exit Sub
        End Try
    End Sub

    Private Sub mnuRemoveItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuRemoveItem.Click
        '********************************************************************************************
        'Description: remove the selected item from the checklist
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mnChkItem > -1 Then
                Dim sName As String = chkFolders.Items(mnChkItem).ToString
                chkFolders.Items.RemoveAt(mnChkItem)
                Try
                    Dim oNames As String() = Application.UserAppDataRegistry.GetValueNames
                    If oNames.GetUpperBound(0) >= 0 Then
                        For nItem As Integer = 0 To oNames.GetUpperBound(0)
                            Dim sTmp As String = Application.UserAppDataRegistry.GetValue(oNames(nItem)).ToString
                            If sTmp.ToLower = sName.ToLower Then
                                'This folder is saved in the registry.  See if it should be kept
                                Dim oReturn As DialogResult = MessageBox.Show(gpsRM.GetString("psREMOVE_FOLDER"), gpsRM.GetString("psDMONVIEWER"), _
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                                Select Case oReturn
                                    Case Windows.Forms.DialogResult.Yes
                                        Application.UserAppDataRegistry.DeleteValue(oNames(nItem))
                                    Case Else
                                End Select
                            End If
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuRemoveItem_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                            Status, MessageBoxButtons.OK)
                End Try
                subRefreshFiles()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuRemoveItem_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub
        End Try
    End Sub

    Private Sub mnuImportCfg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportCfg.Click
        '********************************************************************************************
        'Description: Import a config file (DMONItems.XML)
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFileName As String = String.Empty
        Try
            Dim o As New OpenFileDialog
            With o
                'XML Files|*.XML
                .Filter = gpsRM.GetString("psOPEN_XML_FILE_FLTR")
                .Title = gpsRM.GetString("psOPEN_XML_FILE_CAP")
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".XML"
                .FileName = msXMLFilePath
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFileName = .FileName
                Else
                    Exit Sub
                End If
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuAddZip_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub

        End Try
        If sFileName <> String.Empty Then
            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) Then
                File.Copy(sFileName, msXMLFilePath, True)
                subReadDmonItemsFromXML(moItemInfo)
            End If
        End If
    End Sub

    Private Sub mnuExportCfg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuExportCfg.Click
        '********************************************************************************************
        'Description: Export a config file (DMONItems.XML)
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFileName As String = String.Empty
        Try
            Dim o As New SaveFileDialog
            With o
                'XML Files|*.XML
                .Filter = gpsRM.GetString("psOPEN_XML_FILE_FLTR")
                .Title = gcsRM.GetString("csSAVE_FILE_DLG_CAP")
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".XML"
                .FileName = msXMLFilePath
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFileName = .FileName
                Else
                    Exit Sub
                End If
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuAddZip_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
            Exit Sub
        End Try
        If sFileName <> String.Empty Then
            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) Then
                File.Copy(msXMLFilePath, sFileName, True)
            End If
        End If
    End Sub
    Private Sub dgFiles_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dgFiles.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Return, Keys.Space
                '********************************************************************************************
                'Description: select a file
                'Parameters: none
                'Returns:    true to continue
                '
                'Modification history:
                '
                ' Date      By      Reason
                '********************************************************************************************
                Dim oDR As DataGridViewRow = dgFiles.SelectedRows(0)
                Dim sLocation As String = oDR.Cells(gnCOL_LOCATION).Value.ToString
                Dim sFileName As String = oDR.Cells(gnCOL_FILENAME).Value.ToString
                Dim sOpenPath As String = String.Empty
                Dim sExtTmp1() As String = Split(sLocation, ".")
                If sExtTmp1(sExtTmp1.GetUpperBound(0)).ToLower = "zip" Then
                    'Zip
                    sOpenPath = sGetTmpFileName(sFileName, String.Empty)
                    Dim oZip As ZipFile = Nothing
                    Try
                        oZip = ZipFile.Read(sLocation)
                        For Each oZipEntry As ZipEntry In oZip.Entries
                            If oZipEntry.FileName = sFileName Then
                                oZipEntry.Extract(sOpenPath, ExtractExistingFileAction.OverwriteSilently)
                                oZip.Dispose()
                                Exit For
                            End If
                        Next
                        sOpenPath = sOpenPath & "\" & sFileName
                    Catch ex As Exception
                    Finally
                        If oZip IsNot Nothing Then
                            oZip.Dispose()
                            oZip = Nothing
                        End If
                    End Try

                Else
                    If sLocation.Substring(sLocation.Length - 1) <> "\" Then
                        sLocation = sLocation & "\"
                    End If
                    sOpenPath = sLocation & sFileName
                End If

                subOpenDt(sOpenPath)
        End Select
    End Sub
End Class