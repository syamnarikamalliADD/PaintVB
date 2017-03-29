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
' Description:
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    01/21/11   MSW     Adjust scrolling to make it a little more user-friendly
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    12/02/11   MSW     some more token management                                      4.1.1.0
'    02/15/12   MSW     force 32 bit build for PCDK, update print management            4.1.1.1
'    03/23/12   RJO     modifed for .NET Password and IPC                               4.01.02.00
'    04/11/12   MSW     Changed CommonStrings setup so it builds correctly              4.01.03.00
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances    4.01.03.01
'    12/13/12   MSW     Add some error handling improvements                            4.01.04.00
'    04/16/13   MSW     Add Canadian language files                                     4.01.05.00
'                       Standalone Changelog
'    06/05/13   MSW     Handle unfortunate link lengths                                 4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                              4.01.06.00
'    11/21/13   MSW     Handle some different ways of dividing the text in the resource 4.01.06.01
'                       file, support zones in the IO text
'********************************************************************************************


Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports clsPLCComm = PLCComm.clsPLCComm

'Imports Connstat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Friend Class frmMain

    '******** TODO List  ****************************************************************************
    'TODO - Clean out all the dead code
    'TODO - Write code for the Culture Porperty
    'TODO - Figure out how to dynamically size the treeview panel so there's no horizontal scroll bar.
    '       There doesn't appear to be any way to determine if the scroll bar is showing without 
    '       using the Windows API.
    '******** End TODO List  ************************************************************************

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "plc_io"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "View_PLCIO_Main.bmp"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const mnMAX_HOT_LINK_LEN As Integer = 100 '50 -PLC3 'This varies, depending on the PLC
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Private Const msIO_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".IOStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend glsRM As ResourceManager = Nothing   ' IO Description strings
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private msCulture As String = "en-US" 'Default to English
    Private colZones As clsZones = Nothing
    Private mbRemoteZone As Boolean = False
    Private mbEventBlocker As Boolean = False
    Private mbScreenLoaded As Boolean = False

    Private moFont As New Font("Arial", 10, FontStyle.Bold)
    Private mnActiveData() As Integer
    Private mnVerticalMargin As Integer = 10
    Private mnHorizontalMargin As Integer = 10
    Private mnMaxBBHeight As Integer = 0
    Private mnMaxBBWidth As Integer = 0
    Private moCurrentNode As TreeNode
    Private msPLCData() As String
    Private msPLCRefData() As String
    Private WithEvents moPLC As New clsPLCComm
    Private moPLC1 As clsPLCComm = Nothing
    Private moPLC2 As clsPLCComm = Nothing
    Private moPLC3 As clsPLCComm = Nothing
    Private moPLC4 As clsPLCComm = Nothing
    Private moPLC5 As clsPLCComm = Nothing
    Private moPLC6 As clsPLCComm = Nothing
    Private mInputTitleBackColor As Color = Color.Black
    Private mInputTitleForeColor As Color = Color.White
    Private mOutputTitleBackColor As Color = Color.Black
    Private mOutputTitleForeColor As Color = Color.White
    Private mAnalogTitleBackColor As Color = Color.Black
    Private mAnalogTitleForeColor As Color = Color.White
    Private mOtherTitleBackColor As Color = Color.Black
    Private mOtherTitleForeColor As Color = Color.White
    '******** End Form Variables    *****************************************************************

    '******** Form Structures    ********************************************************************
    Private Structure udsBBAttributes
        Public DisplayIndex As Integer
        Public ModuleType As String
        Public DataIndex As Integer
        Public DataLength As Integer
        Public Rack As String
        Public Slot As String
        Public ModuleNum As String
        Public URL As String
        Public AutoFill As Integer
        Public RackLabels As String
    End Structure

    Friend Structure udsBBItemData
        Public Address As String
        Public Description As String
        Public TagName As String
        Public AliasName As String
    End Structure
    '******** End Form Structures    ****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean = False
    Private mbWatchWindowVisible As Boolean = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************

    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12

    Private msZoneName As String = String.Empty
#End Region

#Region " Properties "

    Friend ReadOnly Property BBFont() As Font
        '********************************************************************************************
        'Description:  Exposes the Font used on the BibgoBoard items to the Watch Window.
        '
        'Parameters: none
        'Returns:    The Font
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moFont
        End Get

    End Property

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
            'Redo screen text, etc. for new culture
            'Code Here ->
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
            Return mPrivilegeGranted
        End Get

        Set(ByVal Value As ePrivilege)
            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted = mPrivilegeRequested Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                Control.CheckForIllegalCrossThreadCalls = True
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
                'hack me
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            Control.CheckForIllegalCrossThreadCalls = True
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
            If mbDataLoaded Then
                'just finished loading reset & refresh
                EditsMade = False
                subShowNewPage()
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
        End Set

    End Property

    Friend Property EditsMade() As Boolean
        '********************************************************************************************
        'Description:  Edit flag for form
        '
        'Parameters: set when somebody changed something
        'Returns:    True if active edits
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbEditsMade
        End Get

        Set(ByVal Value As Boolean)
            Dim bOld As Boolean = mbEditsMade
            mbEditsMade = Value
            If mbEditsMade <> bOld Then subEnableControls(True)
        End Set

    End Property

    Public Property LoggedOnUser() As String
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


    Friend Property WatchWindowVisible() As Boolean
        '********************************************************************************************
        'Description:  Is the Watch Window visible?
        '
        'Parameters: none
        'Returns:    True = Visible, False = Not Visible
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbWatchWindowVisible
        End Get

        Set(ByVal value As Boolean)
            mbWatchWindowVisible = value
        End Set

    End Property

#End Region

#Region " Routines "

    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lRet As DialogResult

        lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                            gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                            MessageBoxButtons.YesNoCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL")
                Return False
            Case Else
                Return True
        End Select

    End Function

    Private Function bValidateData() As Boolean
        '********************************************************************************************
        'Description:  Data Validate Routine
        '
        'Parameters: none
        'Returns:    false if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' temp hack to fire validate
        cboZone.Focus()

        Return True ' temp

    End Function

    Friend Function GetIOItemData(ByVal ResxTag As String, ByVal sRack As String, ByVal sSlot As String, ByVal sPoint As String) As udsBBItemData
        '********************************************************************************************
        'Description: Return the Address, Description, TagName, and Alias for the supplied Resource 
        '             File Tag String.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/11  MSW     add tokens to IO descriptions to maximize cut & paste and minimize typos
        ' 12/02/11  MSW     some more token management
        '********************************************************************************************

        Dim oIOItemData As udsBBItemData
        Dim sTemp1 As String = glsRM.GetString(ResxTag, DisplayCulture)
        If sTemp1 = String.Empty Then
            Debug.Print(ResxTag)
            mDebug.WriteEventToLog("PLC_IO", "IoStrings.RESX missing """ & ResxTag & """")
            sTemp1 = ResxTag
        End If
        If sTemp1.Contains("&") Then
            Debug.Print(sTemp1)
            Debug.Print(sTemp1)
        End If
        Dim nPoint As Integer = CType(sPoint, Integer)
        Dim sPointTrim As String = nPoint.ToString
        If sPoint.Length = 1 Then
            sPoint = "0" & sPoint
        End If
        Dim sPoint8 As String = (nPoint + 8).ToString("00")

        sTemp1 = sTemp1.Replace("#PT#", sPoint)
        sTemp1 = sTemp1.Replace("#PTT#", sPointTrim)
        sTemp1 = sTemp1.Replace("#PT8#", sPoint8)
        sTemp1 = sTemp1.Replace("#RCK#", sRack)
        sTemp1 = sTemp1.Replace("#SLT#", sSlot)
        Dim sTemp() As String = Strings.Split(sTemp1, "$")

        'initialize the structure
        oIOItemData.Address = String.Empty
        oIOItemData.Description = String.Empty
        oIOItemData.TagName = String.Empty
        oIOItemData.AliasName = String.Empty

        Try
            For nIndex As Integer = 0 To sTemp.GetUpperBound(0)
                Select Case nIndex
                    Case 0
                        oIOItemData.Address = sTemp(0)
                    Case 1
                        oIOItemData.Description = sTemp(1)
                    Case 2
                        oIOItemData.TagName = sTemp(2)
                    Case 3
                        oIOItemData.AliasName = sTemp(3)
                End Select

            Next 'nIndex

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: GetIOItemData, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: GetIOItemData, StackTrace: " & ex.StackTrace)
        End Try

        Return oIOItemData

    End Function

    Private Function GetModuleAttributes(ByVal sAttributes As String) As udsBBAttributes
        '********************************************************************************************
        'Description: Return the DisplayIndex, ModuleType, DataIndex, and DataLength for the supplied
        '             BingoBoard Tag String.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBBAttributes As udsBBAttributes
        Dim sTemp() As String = Strings.Split(sAttributes, ",")

        With oBBAttributes
            .DisplayIndex = CType(sTemp(0), Integer)
            .ModuleType = sTemp(1)
            .DataIndex = CType(sTemp(2), Integer)
            .DataLength = CType(sTemp(3), Integer)
            .Rack = sTemp(4)
            .Slot = sTemp(5)
            .ModuleNum = sTemp(6)
            .URL = sTemp(7)
            .AutoFill = 0
            .RackLabels = .Rack
        End With 'oBBAttributes

        Return oBBAttributes

    End Function

    Private Sub subChangeZone()
        '********************************************************************************************
        'Description: New Zone Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Progress = 10

        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboZone.Text = msOldZone
                Progress = 100
                Exit Sub
            End If
        End If  'EditsMade

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboZone.Text = msOldZone Then Exit Sub
        End If
        msOldZone = cboZone.Text
        colZones.CurrentZone = cboZone.Text

        Try
            Dim DS As New DataSet
            Dim DR As DataRow = Nothing
            Dim nDataLength As Integer = 0

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            mbScreenLoaded = False
            subClearScreen()
            subEnableControls(False)

            'Get configuration data
            DS.ReadXmlSchema(colZones.DatabasePath & "XML\PLC_IO.xsd")
            DS.ReadXml(colZones.DatabasePath & "XML\PLC_IO.xml")
            DR = DS.Tables("Config").Rows(0)

            If IsNumeric(DR.Item("DataLength").ToString) Then
                nDataLength = CType(DR.Item("Datalength"), Integer)
            End If
            If IsNumeric(DR.Item("HorizontalMargin").ToString) Then
                mnHorizontalMargin = CType(DR.Item("HorizontalMargin"), Integer)
            End If
            If IsNumeric(DR.Item("VerticalMargin").ToString) Then
                mnVerticalMargin = CType(DR.Item("VerticalMargin"), Integer)
            End If

            Call subSetTitleColors(mInputTitleBackColor, mInputTitleForeColor, DR.Item("InputTitleBackColor").ToString)
            Call subSetTitleColors(mOutputTitleBackColor, mOutputTitleForeColor, DR.Item("OutputTitleBackColor").ToString)
            Call subSetTitleColors(mAnalogTitleBackColor, mAnalogTitleForeColor, DR.Item("AnalogTitleBackColor").ToString)
            Call subSetTitleColors(mOtherTitleBackColor, mOtherTitleForeColor, DR.Item("OtherTitleBackColor").ToString)

            DR = Nothing
            DS.Dispose()

            Call subCreateHotLinks(nDataLength)
            Call subBuildTreeView(colZones.DatabasePath)
            Status(True) = gpsRM.GetString("psSELECT_RACK")
            pnlView.Height = pnlMain.Height

            subEnableControls(True)

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub

    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  Clear out the IO View Panel
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nCount As Integer = pnlView.Controls.Count
        'this should be handled better
        Dim bTmp As Boolean = mbEventBlocker

        pnlBrowserControl.Visible = False

        If nCount > 0 Then
            Try
                mbEventBlocker = True
                DataLoaded = False ' needs to be first
                subEnableControls(False)
                lblDescription.Visible = False

                'Clean-up just in case the tip is showing
                tmrToolTip.Enabled = False
                lblToolTip.Visible = False

                'Remove all of the BingoBoard Controls on pnlView
                For nIndex As Integer = (nCount - 1) To 0 Step -1
                    Dim o As Control = pnlView.Controls.Item(nIndex)
                    If TypeOf o Is BingoBoard.BingoBoard Then
                        Dim oBB As BingoBoard.BingoBoard = DirectCast(o, BingoBoard.BingoBoard)

                        oBB.Visible = False
                        RemoveHandler oBB.ItemClicked, AddressOf moBingoBoard_ItemClicked
                        RemoveHandler oBB.ItemEntered, AddressOf moBingoBoard_ItemEntered
                        Application.DoEvents()
                        oBB.Dispose()
                        oBB = Nothing
                        Application.DoEvents()
                    End If 'TypeOf o

                    o.Dispose()
                    o = Nothing

                Next 'nIndex

            Catch ex As Exception

            Finally
                mbEventBlocker = bTmp
                subEnableControls(True)
                pnlView.Height = pnlMain.Height
            End Try

        End If 'nCount > 0


    End Sub

    Private Sub subCreateHotLinks(ByVal DataLength As Integer)
        '********************************************************************************************
        'Description: Create hotlinks to the PLC Data Table and initialize the current and reference
        '             data arrays.
        '
        'Parameters: DataLength - How many PLC Data words to link up to.
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nHotLinks As Integer = (DataLength \ mnMAX_HOT_LINK_LEN)
        If DataLength Mod mnMAX_HOT_LINK_LEN > 0 Then
            nHotLinks = nHotLinks + 1
        End If
        ReDim msPLCData(DataLength - 1)
        ReDim msPLCRefData(DataLength - 1)

        Try
            'Kill any existing HotLinks
            moPLC1 = Nothing
            moPLC2 = Nothing
            moPLC3 = Nothing
            moPLC4 = Nothing
            moPLC5 = Nothing
            moPLC6 = Nothing
            For nIndex As Integer = 0 To (DataLength - 1)
                msPLCData(nIndex) = "0"
                msPLCRefData(nIndex) = "-" '"0"
            Next 'nIndex

            For nIndex As Integer = 1 To nHotLinks
                Dim oPLC As New clsPLCComm
                Dim nStartIndex As Integer = 0

                With oPLC
                    .ZoneName = colZones.ActiveZone.Name
                    .TagName = "IODataHotlink" & nIndex.ToString
                End With 'oPLC

                Select Case nIndex
                    Case 1
                        moPLC1 = oPLC
                        AddHandler moPLC1.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC1.NewData, AddressOf moPLC_NewData
                    Case 2
                        moPLC2 = oPLC
                        AddHandler moPLC2.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC2.NewData, AddressOf moPLC_NewData
                    Case 3
                        moPLC3 = oPLC
                        AddHandler moPLC3.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC3.NewData, AddressOf moPLC_NewData
                    Case 4
                        moPLC4 = oPLC
                        AddHandler moPLC4.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC4.NewData, AddressOf moPLC_NewData
                    Case 5
                        moPLC5 = oPLC
                        AddHandler moPLC5.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC5.NewData, AddressOf moPLC_NewData
                    Case 6
                        moPLC6 = oPLC
                        AddHandler moPLC6.ModuleError, AddressOf moPLC_ModuleError
                        AddHandler moPLC6.NewData, AddressOf moPLC_NewData
                    Case Else
                        'Just how big is this PLC?
                End Select
                '3/17/07 gks - need to read once to start hotlink
                Dim void As String() = oPLC.PLCData

                nStartIndex = (nIndex - 1) * mnMAX_HOT_LINK_LEN
                If Not IsNothing(void) Then
                    For n As Integer = 0 To void.GetUpperBound(0)
                        msPLCData(nStartIndex + n) = void(n)
                    Next
                End If
            Next 'nIndex

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subCreateHotLinks, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subCreateHotLinks, StackTrace: " & ex.StackTrace)
        End Try

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

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnMultiView.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            bRestOfControls = False
            pnlMain.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = (True And DataLoaded)
                    btnMultiView.Enabled = False
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    pnlMain.Enabled = True
                    btnMultiView.Enabled = True

                Case ePrivilege.Edit
                    btnSave.Enabled = (True And EditsMade)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    pnlMain.Enabled = True
                    btnMultiView.Enabled = True

                Case ePrivilege.Copy
                    btnCopy.Enabled = True
            End Select
        End If

        'restof controls here

    End Sub

    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Private Sub subGetIOResource(ByVal IO_Assembly_Name As String)
        '****************************************************************************************
        'Description: This routine sets up the IO Description string resource manager
        '
        'Parameters: Names or resource file
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sTmp As String = mLanguage.GetCultureString()

        Try

            If IO_Assembly_Name <> String.Empty Then
                glsRM = New ResourceManager(IO_Assembly_Name, [Assembly].GetExecutingAssembly())
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: frmMain, Routine: subGetIOResource, Error: " & ex.Message)
            Trace.WriteLine("Module: frmMain, Routine: subGetIOResource, StackTrace: " & ex.StackTrace)
        End Try

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

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            'mbScreenLoaded = False

            subProcessCommandLine()

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    If moPrivilege.ActionAllowed Then
            '        Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        Privilege = ePrivilege.None
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)
            cboZone.Left = pnlMain.Left

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            End If

            'mbScreenLoaded = True
            'If cboZone.Items.Count = 1 Then
            'this selects the zone in cases of just one zone. fires event to call subchangezone
            cboZone.Text = cboZone.Items(0).ToString
            msZoneName = cboZone.Text
            'End If

        Catch ex As Exception

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

    Private Sub subAddModule(ByVal DR As DataRow)
        '********************************************************************************************
        'Description:  Add a BingoBoard IO module display with configuration data from XML datafile.
        '
        'Parameters: IO Module Configuration Data
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/11  MSW     add tokens to IO descriptions to maximize cut & paste and minimize typos
        ' 04/03/12  MSW     Ran into some trouble with auto-generated PLC_IO.XML.  Make it a little more robust.
        ' 11/21/13  MSW     Handle some different ways of dividing the text in the resource file
        '******************************************************************************************** 
        Dim Culture As CultureInfo = DisplayCulture

        Dim sbbItemData() As String
        Dim sDelim As String

        Try
            Dim sRack As String = DR.Item("RackNumber").ToString
            Dim sSlot As String = DR.Item("SlotNumber").ToString
            Dim sModule As String = DR.Item("ModuleNumber").ToString
            Dim sTitle As String = DR.Item("ModuleType").ToString
            Dim nCount As Integer = CType(DR.Item("Density"), Integer)
            If nCount = 0 Then
                nCount = 1
            End If
            Dim bIsInput As Boolean = CType(DR.Item("PlcInput"), Boolean)
            Dim bAnalog As Boolean = CType(DR.Item("Analog"), Boolean)
            Dim sDataIndex As String = DR.Item("DataIndex").ToString
            Dim sDataLength As String = DR.Item("DataLength").ToString
            Dim sIndex As String = DR.Item("DisplayIndex").ToString
            Dim sURL As String = DR.Item("URL").ToString
            Dim nAutoFill As Integer = 0
            Try
                nAutoFill = CType(DR.Item("AutoFill"), Integer)
            Catch ex As Exception
            End Try
            Dim sRackLabels As String = Nothing
            Try
                sRackLabels = DR.Item("RackLabels").ToString
            Catch ex As Exception
            End Try
            Dim bEnableTT As Boolean = CType(DR.Item("EnableToolTips"), Boolean)
            Dim sFormat As String = String.Empty
            If sRackLabels Is Nothing OrElse sRackLabels = String.Empty Then
                sRackLabels = sRack
            End If

            Dim nAutoSlotStart As Integer = 1
            Dim nAutoSlotEnd As Integer = 1
            If nAutoFill > 0 Then
                nAutoSlotEnd = nAutoFill
            End If
            For nAutoSlot As Integer = nAutoSlotStart To nAutoSlotEnd
                Dim oBB As New BingoBoard.BingoBoard
                Dim sBBItemText() As String
                Dim sBBItemBit() As String
                ReDim sBBItemText(nCount - 1)
                ReDim sBBItemBit(nCount - 1)
                For nItem As Integer = 0 To sBBItemBit.GetUpperBound(0)
                    sBBItemBit(nItem) = nItem.ToString
                Next 'nItem

                sFormat = glsRM.GetString("lsSLOT", Culture) & sSlot & " " & glsRM.GetString("lsMODULE", Culture) & sModule & " - "
                sDelim = glsRM.GetString("lsDELIMITER", Culture)

                With oBB
                    .Name = "oBB" & sIndex
                    .Columns = 1
                    .AutosizeColumns = True
                    .AutoSize = True
                    .ItemOffColor = Color.Tomato 'Red was too hard to read
                    .ItemCount = nCount
                    .TitleText = sFormat & sTitle
                    .TitleFont = moFont
                    .ItemFont = moFont
                    .ItemBitIndex = sBBItemBit
                    .BorderStyle = BorderStyle.Fixed3D
                    .EnableToolTips = bEnableTT

                    If nCount <= 1 And (bAnalog = False) Then
                        'This is a Comm module or other Special Module
                        ReDim sbbItemData(0)
                        'Tag = <DisplayIndex>,<Type>,<DataIndex>,<DataLength>,<Rack>,<Slot>,<Module>,<URL>
                        .Tag = sIndex & ",X,0,0," & sRack & "," & sSlot & "," & sModule & "," & sURL
                        .TitleBackColor = mOtherTitleBackColor
                        .TitleColor = mOtherTitleForeColor
                        sBBItemText(0) = glsRM.GetString("lsEXPAND", Culture)
                        sbbItemData(0) = "1"
                    Else
                        If bAnalog Then
                            'This is an Analog I/O Module
                            ReDim sbbItemData(nCount - 1)

                            'Tag = <DisplayIndex>,<Type>,<DataIndex>,<DataLength>,<Rack>,<Slot>,<Module>,<URL>
                            .Tag = sIndex & ",A," & sDataIndex & "," & sDataLength & "," & sRack & "," & sSlot & "," & sModule & "," & sURL
                            .TitleBackColor = mAnalogTitleBackColor
                            .TitleColor = mAnalogTitleForeColor
                            If sSlot.Length = 1 Then
                                sSlot = "0" & sSlot
                            End If
                            For nItem As Integer = 0 To sBBItemText.GetUpperBound(0)
                                Dim oIOData As udsBBItemData

                                Dim sResxTag As String = "lsR" & sRackLabels & "S" & sSlot & "M" & sModule & "_"
                                Dim sDesc As String = String.Empty

                                If Strings.Len(sBBItemBit(nItem)) > 1 Then
                                    sResxTag = sResxTag & sBBItemBit(nItem)
                                Else
                                    sResxTag = sResxTag & "0" & sBBItemBit(nItem)
                                End If

                                oIOData = GetIOItemData(sResxTag, sRack, sSlot, sBBItemBit(nItem))
                                sBBItemText(nItem) = oIOData.Address & sDelim & "0"
                            Next 'nItem

                            For nItem As Integer = 0 To sbbItemData.GetUpperBound(0)
                                sbbItemData(nItem) = "-1"
                            Next 'nItem

                            .Locked = True

                        Else
                            'This is a Digital I/O Module
                            ReDim sbbItemData((nCount - 1) \ 16)

                            'Tag = <DisplayIndex>,<Type>,<DataIndex>,<DataLength>,<Rack>,<Slot>,<Module>,<URL>
                            .Tag = sIndex & ",D," & sDataIndex & "," & sDataLength & "," & sRack & "," & sSlot & "," & sModule & "," & sURL
                            If bIsInput Then
                                .TitleBackColor = mInputTitleBackColor
                                .TitleColor = mInputTitleForeColor
                            Else
                                .TitleBackColor = mOutputTitleBackColor
                                .TitleColor = mOutputTitleForeColor
                            End If

                            If sSlot.Length = 1 Then
                                sSlot = "0" & sSlot
                            End If
                            For nItem As Integer = 0 To sBBItemText.GetUpperBound(0)
                                Dim oItemData As udsBBItemData

                                Dim sResxTag As String = "lsR" & sRackLabels & "S" & sSlot & "M" & sModule & "_"

                                If Strings.Len(sBBItemBit(nItem)) > 1 Then
                                    sResxTag = sResxTag & sBBItemBit(nItem)
                                Else
                                    sResxTag = sResxTag & "0" & sBBItemBit(nItem)
                                End If

                                oItemData = GetIOItemData(sResxTag, sRack, sSlot, sBBItemBit(nItem))
                                ' 11/21/13  MSW     Handle some different ways of dividing the text in the resource file
                                If (oItemData.Address <> String.Empty) And (oItemData.Description = String.Empty) Then
                                    sBBItemText(nItem) = oItemData.Address
                                ElseIf (oItemData.Address = String.Empty) And (oItemData.Description <> String.Empty) Then
                                    sBBItemText(nItem) = oItemData.Description
                                Else
                                    sBBItemText(nItem) = oItemData.Address & sDelim & oItemData.Description
                                End If
                            Next 'nItem

                            'Special case to Handle 8-Bit Modules in 2 Slot addressing scheme
                            'If (nCount = 8) And (sModule = "1") Then
                            '    For nItem As Integer = 0 To sBBItemBit.GetUpperBound(0)
                            '        sBBItemBit(nItem) = (nItem + 8).ToString
                            '    Next 'nItem
                            '    .ItemBitIndex = sBBItemBit
                            'End If

                            For nItem As Integer = 0 To sbbItemData.GetUpperBound(0)
                                sbbItemData(nItem) = "0"
                            Next 'nItem 
                        End If 'bAnalog
                    End If 'nCount = 1

                    .ItemOffText = sBBItemText
                    .ItemOnText = sBBItemText
                    .ItemData = sbbItemData
                    If .Height > mnMaxBBHeight Then mnMaxBBHeight = .Height
                    If .Width > mnMaxBBWidth Then mnMaxBBWidth = .Width
                    .AutosizeColumns = False

                    AddHandler oBB.ItemClicked, AddressOf moBingoBoard_ItemClicked
                    AddHandler oBB.ItemEntered, AddressOf moBingoBoard_ItemEntered

                End With 'oBB

                pnlView.Controls.Add(oBB)

                If nCount > 1 Or bAnalog Then
                    'This is not a special module.
                    'Add the data words associated with this module to the list of active data to monitor
                    Dim nWords As Integer = sbbItemData.GetLength(0)
                    Dim nStart As Integer = 0
                    Dim bEmpty As Boolean = mnActiveData(0) < 0

                    If Not bEmpty Then
                        nStart = mnActiveData.GetLength(0)
                    End If

                    ReDim Preserve mnActiveData(nStart + nWords - 1)

                    For nWord As Integer = 1 To nWords
                        mnActiveData(nStart + nWord - 1) = CType(sDataIndex, Integer) + nWord - 1
                    Next

                End If
                If nAutoFill > 0 Then
                    Dim nSlot As Integer = CType(sSlot, Integer) + 1
                    Dim nDataIndex As Integer = CType(sDataIndex, Integer) + 1
                    Dim nIndex As Integer = CType(sIndex, Integer) + 1
                    sSlot = nSlot.ToString
                    sDataIndex = nDataIndex.ToString
                    sIndex = nIndex.ToString
                End If
            Next


        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subAddModule, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subAddModule, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subBuildRackView(ByVal DataFilePath As String, ByVal Rack As String)
        '********************************************************************************************
        'Description:  Build the selected Rack display with configuration data from XML datafile.
        '
        'Parameters: selected Rack
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Dim DS As New DataSet
        Dim DR As DataRow = Nothing
        ReDim mnActiveData(0)

        Try
            mbEventBlocker = True
            mnMaxBBHeight = 0
            mnMaxBBWidth = 0
            mnActiveData(0) = -1
            'Supress the flicker when we're building the display
            pnlView.Visible = False

            DS.ReadXmlSchema(DataFilePath & "XML\PLC_IO.xsd")
            DS.ReadXml(DataFilePath & "XML\PLC_IO.xml")

            For Each DR In DS.Tables("Modules").Rows
                If DR.Item("RackNumber").ToString = Rack Then
                    If DirectCast(DR.Item("View"), Boolean) = True Then
                        Call subAddModule(DR)
                    End If
                End If 'DR.Item("Rack")...
            Next 'DR

            Call subTrimActiveData()
            Call subPositionModules()
            pnlView.Visible = True
            Call subUpdateDisplay(True)

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subBuildRackView, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subBuildRackView, StackTrace: " & ex.StackTrace)
        Finally
            mbEventBlocker = False
        End Try

    End Sub

    Private Sub subBuildTreeView(ByVal DataFilePath As String)
        '********************************************************************************************
        'Description:  Build the Rack TreeView with configuration data from XML datafile.
        '
        'Parameters: Path to data file
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Dim DS As New DataSet
        Dim DR As DataRow = Nothing
        Dim Culture As CultureInfo = DisplayCulture

        Try
            mbEventBlocker = True
            lstSort.Items.Clear()

            DS.ReadXmlSchema(DataFilePath & "XML\PLC_IO.xsd")
            DS.ReadXml(DataFilePath & "XML\PLC_IO.xml")

            For Each DR In DS.Tables("Racks").Rows
                Dim bAdd As Boolean = DirectCast(DR.Item("View"), Boolean)

                If bAdd Then
                    Dim sRack As String = DirectCast(DR.Item("RackNumber"), String)
                    lstSort.Items.Add(sRack)
                End If
            Next 'DR

            DR = Nothing
            DS.Dispose()

            trvRacks.Size = New Size(3500, 546)

            With trvRacks
                'Create the Parent node
                .Nodes.Clear()
                .Font = moFont
                .ShowNodeToolTips = True
                .Nodes.Add(colZones.CurrentZone)
                .SelectedNode = .Nodes(0)

                For nItem As Integer = 0 To (lstSort.Items.Count - 1)
                    Dim sKey As String = lstSort.Items(nItem).ToString
                    Dim sName As String = glsRM.GetString("lsRACK" & sKey, Culture)
                    If sName = String.Empty Then
                        sName = glsRM.GetString("lsRACK", Culture) & " " & sKey
                    End If
                    Dim sTTKey As String = "lsR" & sKey & "_TT"

                    .SelectedNode.Nodes.Add(sKey, sName)
                    .SelectedNode.Nodes(sKey).Tag = sKey
                    .SelectedNode.Nodes(sKey).ToolTipText = glsRM.GetString(sTTKey, Culture)
                Next 'nItem

                .ExpandAll()

            End With 'trvRacks

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subBuildTreeView, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subBuildTreeView, StackTrace: " & ex.StackTrace)
        Finally
            mbEventBlocker = False
        End Try

    End Sub

    Private Sub subPositionModules()
        '********************************************************************************************
        'Description:  Arrange the BingoBoard controls on pnlView.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/21/11  MSW     Adjust scrolling to make it a little more user-friendly
        ' 03/29/11  MSW     For a single column, compress the heights so it's easier to read
        '********************************************************************************************
        Dim nModules As Integer = 0
        Dim nModsPerRow As Integer = 0
        Dim nCol As Integer = 0
        Dim nRow As Integer = 0
        Dim nHeight As Integer = 0

        Try
            'First, do some ciphering to figure out how many can fit in a row
            nModsPerRow = pnlView.Width \ (mnMaxBBWidth + mnHorizontalMargin + mnHorizontalMargin)
            If nModsPerRow = 0 Then
                nModsPerRow = 1
            End If
            'Now we need to assume that the XML file is set up correctly and the DisplayIndex for
            'each module in this rack is numbered sequentially, starting with 0.
            Dim nBBHeight(1) As Integer
            For Each oControl As Control In pnlView.Controls
                If TypeOf (oControl) Is BingoBoard.BingoBoard Then
                    Dim oBB As BingoBoard.BingoBoard = DirectCast(oControl, BingoBoard.BingoBoard)
                    Dim oAttributes As udsBBAttributes = GetModuleAttributes(CType(oBB.Tag, String))
                    Dim nIndex As Integer = CType(oAttributes.DisplayIndex, Integer)
                    If nIndex > nBBHeight.GetUpperBound(0) Then
                        ReDim Preserve nBBHeight(nIndex + 1)
                    End If
                    'make all of the BingoBoard controls the same size
                    oBB.Width = mnMaxBBWidth
                    oBB.ColumnWidth = mnMaxBBWidth
                    If nModsPerRow > 1 Then
                        oBB.Height = mnMaxBBHeight
                    Else
                        nBBHeight(nIndex) = oBB.Height
                    End If
                    'position the BibgoBoard control
                    nRow = (nIndex \ nModsPerRow) + 1
                    nCol = (nIndex Mod nModsPerRow) + 1
                    oBB.Top = (nRow * mnVerticalMargin) + ((nRow - 1) * mnMaxBBHeight)
                    oBB.Left = (nCol * mnHorizontalMargin) + ((nCol - 1) * mnMaxBBWidth)
                    'keep track of how many are displayed
                    nModules += 1
                End If
            Next 'oControl
            If nModsPerRow = 1 Then
                'For a single column, compress the heights so it's easier to read
                Dim nTop(nBBHeight.GetUpperBound(0) + 1) As Integer
                nTop(0) = mnVerticalMargin
                For nRowIndex As Integer = 1 To nBBHeight.GetUpperBound(0)
                    nTop(nRowIndex) = nTop(nRowIndex - 1) + mnVerticalMargin + nBBHeight(nRowIndex - 1)
                Next
                For Each oControl As Control In pnlView.Controls
                    If TypeOf (oControl) Is BingoBoard.BingoBoard Then
                        Dim oBB As BingoBoard.BingoBoard = DirectCast(oControl, BingoBoard.BingoBoard)
                        Dim oAttributes As udsBBAttributes = GetModuleAttributes(CType(oBB.Tag, String))
                        Dim nIndex As Integer = CType(oAttributes.DisplayIndex, Integer)
                        'position the BibgoBoard control
                        oBB.Top = nTop(nIndex)
                        'keep track of how many are displayed
                        nModules += 1
                    End If
                Next 'oControl
            End If
            'MSW  - leave pnlview alone.  Let it autoscroll instead of the main form
            ''Resize pnlView (if necessary)
            'nRow = (nModules \ nModsPerRow)
            'If nModules Mod nModsPerRow > 0 Then nRow += 1
            'nHeight = (nRow * mnMaxBBHeight) + ((nRow + 1) * mnVerticalMargin)
            'If nHeight < pnlTreeView.Height Then
            '    pnlView.Height = pnlTreeView.Height
            'Else
            '    pnlView.Height = nHeight
            'End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub subPrintData()
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Dim oReport As clsPrint = New clsPrint

        Me.Cursor = System.Windows.Forms.Cursors.AppStarting
        btnPrint.Enabled = False
        btnClose.Enabled = False

        Try

            Status = gcsRM.GetString("csPRINTING")


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)


        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            btnClose.Enabled = True

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
        Dim sSeparators As String = " "
        Dim sCommands As String = Microsoft.VisualBasic.Command()
        Dim sArgs() As String = sCommands.Split(sSeparators.ToCharArray)

    End Sub

    Private Sub subSaveData()
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select


        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Try

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            'If there's anything to save, save it here

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try


    End Sub

    Private Sub subSetTitleColors(ByRef BackColor As Color, ByRef ForeColor As Color, _
                                                                    ByVal RequestedColor As String)
        '********************************************************************************************
        'Description:  Set the Bingoboard Title colors for each module type.
        '
        'Parameters: BackColor - BackColor variable to set
        '            ForeColor - ForeColor variable to set
        '            ReqestedColor - Color to set BackColor to
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ForeColor = Color.Black

        Select Case RequestedColor.ToLower(CultureInfo.InvariantCulture)
            Case "brown"
                BackColor = Color.SandyBrown
            Case "red"
                BackColor = Color.Salmon
            Case "orange"
                BackColor = Color.Orange
            Case "yellow"
                BackColor = Color.Gold
            Case "green"
                BackColor = Color.LightGreen
            Case "blue"
                BackColor = Color.LightSkyBlue
            Case "violet"
                BackColor = Color.Plum
            Case "black"
                BackColor = Color.Black
                ForeColor = Color.White
            Case "gray"
                BackColor = Color.Silver
            Case Else
                'Just default to black
                BackColor = Color.Black
                ForeColor = Color.White
        End Select

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
                                       gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, mbUSEROBOTS, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

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
        'Should we call this from the treeview event???
        mbEventBlocker = False

    End Sub

    Private Sub subTrimActiveData()
        '********************************************************************************************
        'Description:  Get rid of any duplicates in the list.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTemp(0) As Integer
        Dim nWord As Integer = 0

        nTemp(0) = mnActiveData(0)
        For nIndex As Integer = 1 To mnActiveData.GetUpperBound(0)
            Dim bFound As Boolean = False

            For Each nWord In nTemp
                If nWord = mnActiveData(nIndex) Then
                    bFound = True
                    Exit For
                End If
            Next
            If Not bFound Then
                ReDim Preserve nTemp(nTemp.GetLength(0))

                nTemp(nTemp.GetUpperBound(0)) = mnActiveData(nIndex)
            End If
        Next 'nIndex

        ReDim mnActiveData(nTemp.GetUpperBound(0))

        For nIndex As Integer = 0 To nTemp.GetUpperBound(0)
            mnActiveData(nIndex) = nTemp(nIndex)
        Next

    End Sub

    Private Sub subUndoData()
        '********************************************************************************************
        'Description:  Undo Button Pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG"), msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then

            'This has been saved just in case we decide to allow a priviledged used to change the item texts.
        End If

    End Sub

    Private Sub subUpdateDisplay(ByVal Initialize As Boolean)
        '********************************************************************************************
        'Description:  PLC data or I/O Rack selection has changed. Determine if this affects  
        '              anything currently displayed and update if necessary.
        '
        'Parameters: Initiailize - True =  update all items that are currently visible
        '                          False = update only currently visible items that changed state
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/29/11  MSW     support single item with analog data
        '********************************************************************************************

        Try

            For Each nDataWord As Integer In mnActiveData
                If nDataWord >= 0 Then
                    If Initialize Or (msPLCData(nDataWord) <> msPLCRefData(nDataWord)) Then
                        'Figure out which Module's BingoBoard gets the new data
                        For Each oControl As Control In pnlView.Controls
                            Dim bFound As Boolean = False

                            If TypeOf oControl Is BingoBoard.BingoBoard Then
                                Dim oBB As BingoBoard.BingoBoard = DirectCast(oControl, BingoBoard.BingoBoard)
                                Dim Attributes As udsBBAttributes = GetModuleAttributes(oBB.Tag.ToString)
                                Dim nOffset As Integer = 0

                                For nOffset = 0 To (Attributes.DataLength - 1)
                                    If nDataWord = Attributes.DataIndex + nOffset Then
                                        bFound = True
                                        Exit For
                                    End If
                                Next 'nIndex

                                If bFound Then
                                    Dim sTemp() As String

                                    Select Case Attributes.ModuleType
                                        Case "A" 'Analog
                                            Dim sDelim As String = glsRM.GetString("lsDELIMITER", DisplayCulture)
                                            Dim nPtr As Integer = 0

                                            'The data goes in the item text
                                            sTemp = oBB.ItemOnText
                                            nPtr = Strings.InStr(sTemp(nOffset), sDelim)

                                            If nPtr > 0 Then
                                                sTemp(nOffset) = Strings.Left(sTemp(nOffset), nPtr + Strings.Len(sDelim) - 1) & msPLCData(nDataWord)
                                            Else
                                                sTemp(nOffset) = "<" & gcsRM.GetString("csERROR", DisplayCulture) & ">"
                                            End If

                                            oBB.SuspendLayout()
                                            oBB.ItemOnText = sTemp
                                            oBB.ItemOffText = sTemp
                                            'Prevent shrinkage
                                            oBB.Height = mnMaxBBHeight
                                            oBB.ResumeLayout()

                                        Case "D" 'Digital
                                            sTemp = oBB.ItemData
                                            sTemp(nOffset) = msPLCData(nDataWord)
                                            oBB.ItemData = sTemp

                                        Case Else
                                            'Houston, we have a problem
                                    End Select

                                End If 'bFound

                            End If 'TypeOf oControl Is ...
                        Next 'oControl
                    End If 'Initialize Or ...
                End If 'nDataWord >= 0
            Next 'nDataWord

            'Update the reference data
            Call subUpdateReference()

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subUpdateDisplay, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: subUpdateDisplay, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateReference()
        '********************************************************************************************
        'Description:  All visible displays have been updated with the current PLC data. Copy the  
        '              current data to the reference data array.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For nIndex As Integer = 0 To msPLCData.GetUpperBound(0)
            msPLCRefData(nIndex) = msPLCData(nIndex)
        Next

    End Sub

#End Region

#Region " Events "

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
                    subLaunchHelp(gs_HELP_VIEW_PLCIO, oIPC)
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Bmp)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

        'TODO - Add HotKeys and (invisible) Help button
        'If sKeyValue = String.Empty Then
        '    'Trap Main Menu HotKey presses
        '    If e.Alt And (Not (e.Control Or e.Shift)) Then
        '        For Each oMenuButton As Windows.Forms.ToolStripButton In tlsMain.Items
        '            If oMenuButton.Tag.ToString = e.KeyCode.ToString Then
        '                sKeyValue = oMenuButton.Name
        '                Exit For
        '            End If
        '        Next 'oMenuButton
        '    End If
        'End If 'sKeyValue = String.Empty

        'If sKeyValue <> String.Empty Then
        '    'Click the Menu Button
        '    If tlsMain.Items(sKeyValue).Enabled Then
        '        tlsMain.Items(sKeyValue).PerformClick()
        '    End If
        'End If

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

        subInitializeForm()

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

        If EditsMade Then
            e.Cancel = (bAskForSave() = False)
        End If

    End Sub

    Public Sub New()
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)
        Call subGetIOResource(msIO_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles cboZone.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboZone.Text <> msOldZone Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeZone()
        End If

    End Sub

    Private Sub frmMain_Layout(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/21/11  MSW     Adjust scrolling to make it a little more user-friendly
        '********************************************************************************************

        'todo find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100

        pnlMain.Height = nHeight
        pnlMain.Width = nWidth

        'Adjust the width of the View panel to fit the new screen size
        pnlView.Height = pnlMain.Height
        pnlView.Left = pnlTreeView.Right + 5
        'RV 09/12/16
        'pnlTreeView.Width = 165
        pnlView.Width = nWidth - pnlTreeView.Width - 5
        'Format the panel
        subPositionModules()
    End Sub

    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs)
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

    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs)
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

        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subPrintData()

            Case "btnUndo"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subUndoData()
                End Select

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

        End Select

    End Sub

    Private Sub mnuLast7_Click(ByVal sender As Object, _
                                                ByVal e As System.EventArgs) Handles mnuLast7.Click
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
                                            ByVal e As System.EventArgs) Handles mnuAllChanges.Click
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
                                            ByVal e As System.EventArgs) Handles mnuLast24.Click
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

    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '********************************************************************************************
        'Description:  call print routine
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subPrintData()

    End Sub
    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        ''********************************************************************************************
        ''Description:  this was needed to enable the menus for some reason
        ''                  now handled in dostatusbar 2
        ''
        ''Parameters: none
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        ''********************************************************************************************
        Dim cachePrivilege As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/23/12
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
        GC.Collect()
        System.Windows.Forms.Application.DoEvents()
        subInitializeForm()

    End Sub

    Private Sub btnCloseBrowser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseBrowser.Click
        '********************************************************************************************
        'Description: The user clicked the Browser Close button. Restore the current rack view
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subClearScreen()

        'Center lblDescription above pnlView
        lblDescription.Text = moCurrentNode.Text & glsRM.GetString("lsDELIMITER", DisplayCulture) & moCurrentNode.ToolTipText
        lblDescription.Visible = True
        lblDescription.Top = lblZone.Top
        lblDescription.Left = pnlView.Left + (pnlView.Width \ 2) - (lblDescription.Width \ 2)

        Status(False) = gpsRM.GetString("psBUILD_RACK", DisplayCulture) & moCurrentNode.Text
        Call subBuildRackView(colZones.DatabasePath, moCurrentNode.Tag.ToString)
        Status(True) = gpsRM.GetString("psSELECT_RACK", DisplayCulture)

    End Sub

    Private Sub moBingoBoard_ItemClicked(ByRef sender As BingoBoard.BingoBoard, ByVal Item As Integer, ByVal Mousebutton As System.Windows.Forms.MouseButtons)
        '********************************************************************************************
        'Description: The user clicked a BingoBoard Item.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Culture As CultureInfo = DisplayCulture

        Select Case Mousebutton
            Case Windows.Forms.MouseButtons.Left
                Dim oAttributes As udsBBAttributes = GetModuleAttributes(sender.Tag.ToString)

                Select Case oAttributes.ModuleType
                    Case "X" 'Comm Module
                        Dim oWB As New WebBrowser
                        Dim sDescription As String = glsRM.GetString("lsRACK", Culture) & " " & moCurrentNode.Tag.ToString & _
                                                     glsRM.GetString("lsDELIMITER", Culture) & sender.TitleText

                        'Show the module description and center above pnlView
                        Call subClearScreen()
                        lblDescription.Text = sDescription
                        lblDescription.Visible = True
                        lblDescription.Top = lblZone.Top
                        lblDescription.Left = pnlView.Left + (pnlView.Width \ 2) - (lblDescription.Width \ 2)

                        'Show a WebBrowser and navigate to the module web page
                        pnlView.Controls.Add(oWB)
                        With oWB
                            .Dock = DockStyle.Fill
                            .Navigate(oAttributes.URL)
                        End With

                        'Show the Browser control panel
                        With pnlBrowserControl
                            .Left = pnlView.Left + pnlMain.Left
                            .Width = pnlView.Width
                            .Visible = True
                        End With

                    Case Else 'I/O Module
                        'Ask the user if this item should be added to the WatchWindow
                        Dim lRet As DialogResult = MessageBox.Show(gpsRM.GetString("psWW_ADD_MSG", Culture), _
                                                                   gpsRM.GetString("psWW_CAPTION", Culture), _
                                                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                        If lRet = Windows.Forms.DialogResult.OK Then
                            If Not WatchWindowVisible Then
                                frmWatch.Show()
                                frmWatch.PLCData = msPLCData
                            End If
                            If Not frmWatch.ItemExists(sender, Item) Then
                                frmWatch.AddItem(sender, Item)
                            End If
                        End If 'lRet = Yes
                End Select

            Case Windows.Forms.MouseButtons.Right
                'Ask the user if they want to edit the description for this item

        End Select

    End Sub

    Private Sub moBingoBoard_ItemEntered(ByRef sender As BingoBoard.BingoBoard, ByVal Item As Integer)
        '********************************************************************************************
        'Description: The mousepointer has entered a BingoBoard Item.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'If tooltips are enabled for this module, show the tooltip related to this item
        If sender.EnableToolTips Then
            Dim oModData As udsBBAttributes = GetModuleAttributes(sender.Tag.ToString)
            Dim sItem As String = "_" & Strings.Format(Item, "00")
            Dim sSlot As String = String.Empty
            If oModData.Slot.Length = 1 Then
                sSlot = "0" & oModData.Slot
            Else
                sSlot = oModData.Slot
            End If

            Dim sResxTag As String = "lsR" & oModData.Rack & "S" & sSlot & "M" & oModData.ModuleNum & sItem
            Dim oTTData As udsBBItemData = GetIOItemData(sResxTag, oModData.Rack, oModData.Slot, Strings.Format(Item, "00"))
            Dim sToolTip As String = glsRM.GetString("lsTAG_NAME", DisplayCulture) & oTTData.TagName & ", "

            sToolTip = sToolTip & glsRM.GetString("lsALIAS", DisplayCulture) & oTTData.AliasName

            'Clean-up just in case the tip is showing
            tmrToolTip.Enabled = False
            lblToolTip.Visible = False

            'Show us your tip
            lblToolTip.Text = sToolTip
            lblToolTip.Left = Cursor.Position.X
            lblToolTip.Top = Cursor.Position.Y - 160
            lblToolTip.Visible = True
            tmrToolTip.Enabled = True
        End If

    End Sub

    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, ByVal sModule As String, ByVal AdditionalInfo As String)
        '********************************************************************************************
        'Description: An error has occurred in the clsPLCComm that is used to monitor PLC IO data.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMsg As String = nErrNum.ToString & "-" & sErrDesc & " occurred in  Module " & sModule & "."

        If AdditionalInfo <> String.Empty Then
            sMsg = sMsg & " Additional Info: " & AdditionalInfo
        End If

        Trace.WriteLine("Module: frmMain, Routine: moPLC_ModuleError, Error: " & sMsg)

    End Sub

    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, ByVal Data() As String)
        '********************************************************************************************
        'Description: The clsPLCComm that monitors the PLC IO data for this zone reported new data 
        '             has been read.
        '
        'Parameters: 
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

        Try
            Dim sIndex As String = Strings.Right(TagName, 1)
            Dim nStartIndex As Integer = 0

            If IsNumeric(sIndex) Then
                nStartIndex = (CType(sIndex, Integer) - 1) * mnMAX_HOT_LINK_LEN
            Else
                Trace.WriteLine("Module: " & msMODULE & ", Routine: moPLC_NewData, Error: Bad Tagname [" & TagName & "]")
            End If

            For nIndex As Integer = 0 To Data.GetUpperBound(0)
                msPLCData(nStartIndex + nIndex) = Data(nIndex)
            Next

            If pnlView.Controls.Count = 0 Then Exit Sub

            If Not mbEventBlocker Then
                Call subUpdateDisplay(False) '>>>RJO 05/31/07
                If mbWatchWindowVisible Then frmWatch.PLCData = msPLCData
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: " & msMODULE & ", Routine: moPLC_NewData, Error: " & ex.Message)
            Trace.WriteLine("Module: " & msMODULE & ", Routine: moPLC_NewData, StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub tmrToolTip_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrToolTip.Tick
        '********************************************************************************************
        'Description:  The tip has been showing long enough. Hide it.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrToolTip.Enabled = False
        lblToolTip.Visible = False

    End Sub

    Private Sub trvRacks_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvRacks.AfterSelect
        '********************************************************************************************
        'Description:  User selected a rack from the TreeView. Show the IO Display for the selected 
        '              Rack.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/29/10  RJO     Screen crashed if tree was colapsed or root node was selected.
        '********************************************************************************************

        If Not mbEventBlocker AndAlso e.Node.Tag IsNot Nothing Then
            Call subClearScreen()
            moCurrentNode = e.Node
            'Center lblDescription above pnlView
            lblDescription.Text = e.Node.Text & glsRM.GetString("lsDELIMITER", DisplayCulture) & e.Node.ToolTipText
            lblDescription.Visible = True
            lblDescription.Top = lblZone.Top
            lblDescription.Left = pnlView.Left + (pnlView.Width \ 2) - (lblDescription.Width \ 2)

            If moCurrentNode.Level > 0 Then 'RJO 12/29/2010
                Status(False) = gpsRM.GetString("psBUILD_RACK", DisplayCulture) & e.Node.Text
                Call subBuildRackView(colZones.DatabasePath, e.Node.Tag.ToString)
                Status(True) = gpsRM.GetString("psSELECT_RACK", DisplayCulture)
            End If
            mbScreenLoaded = True
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
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.Invoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Privilege = mPrivilegeRequested
        End If
    End Sub

    Private Sub moPassword_LogOut() Handles moPassword.Logout
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
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.Invoke(dLogin)
        Else
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
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
            Me.Invoke(dNewMessage, New Object() {Schema, DS})
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

    Private Sub btnTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTest.Click

        msPLCData(CType(txtDW.Text, Integer)) = txtValue.Text
        Call subUpdateDisplay(False)
        frmWatch.PLCData = msPLCData
        'If pnlTreeView.Width = 120 Then
        '    pnlTreeView.Width = 60
        'Else
        '    pnlTreeView.Width = 120
        'End If
    End Sub

End Class