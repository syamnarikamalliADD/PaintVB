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
' Form/Module: frmMain
'
' Description: Schedule Setup Screen
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    11/12/09   MSW     add delegate declarations tohandle cross-thread calls
'    09/14/11   MSW     Assemble a standard version of everything                         4.01.00.00
'    11/22/11   RJO     Modified to use clsPrintHtml. Fixed bug in 
'                       btnAddFolderBrowse_Click.                         
'    12/02/11   MSW     Add DMON Cfg placeholder reference                                4.01.01.00
'    01/18/12   MSW     Clean up old printsettings object                                 4.01.01.01
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm   4.01.01.02
'    02/15/12   MSW     Force 32 bit build, print management udpates                      4.01.01.03
'    03/23/12   RJO     modified fro .NET Passwoed and IPC                                4.01.02.00
'    03/28/12   MSW     Move Schedule tables from SQL to XML                              4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                            4.01.03.01
'    04/16/13   MSW     Add Canadian language files                                       4.01.05.00
'                       Standalone Changelog
'    06/05/13   MSW     Read dates in fixed culture                                       4.01.05.01
'    07/09/13   MSW     Update and standardize logos                                      4.01.05.02
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up    4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs                                         4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                                  4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                  4.01.07.00
'    05/16/14   MSW     Add weekly reports to scheduler                                   4.01.07.01
'**************************************************************************************************



Imports Response = System.Windows.Forms.DialogResult
Imports System.Xml
Imports System.Xml.XPath

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "Scheduler"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False
    Private Const mnEVENT_COUNT As Integer = 18
    'Private Const mnROWSPACE As Integer = 40 ' interval for rows of textboxes etc

    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private mcolZones As clsZones = Nothing

    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Private mbAddFoldersModified As Boolean
    Private mbAddExtraModified As Boolean
    Private mbPathDNE_Warn As Boolean
    Private msCurrentTabName As String = "TabPage1" ' always start on first page
    Private msSCREEN_DUMP_NAME As String = String.Empty 'changes with selected tab
    Private mPrintHtml As clsPrintHtml
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************

    '******** Structures    *************************************************************************
    Private Structure udsEvent
        Friend StartHour As Integer
        Friend StartMin As Integer
        Friend StartSec As Integer
        Friend EndHour As Integer
        Friend EndMin As Integer
        Friend EndSec As Integer
        Friend Enable As Boolean
        Friend Label As String
    End Structure

    Private Structure udsHistory
        Friend LastCleanup As String
        Friend LastRBackup As String
        Friend LastFBackup As String
    End Structure

    Private Structure udsMaint
        Friend StartHour As Integer
        Friend StartMin As Integer
        Friend StartSec As Integer
        Friend Cleanup As Boolean
        Friend RBackup As Boolean
        Friend FBackup As Boolean
        Friend Label As String
    End Structure

    Private Structure udsOptions
        Friend BackupPath As String
        Friend NewFolder As Boolean
        Friend BackupDB As Boolean
        Friend BackupRM As Boolean
        Friend BackupRS As Boolean
        Friend BackupRT As Boolean
        Friend BackupImage As Boolean
        Friend BackupAdd As Boolean
        Friend ExtraCopies As Boolean
        Friend Logfile As Boolean
        Friend NotifyStartShift As Boolean
        Friend NotifyEndShift As Boolean
        Friend WeeklyReports As Boolean '    05/16/14   MSW     Add weekly reports to scheduler 
    End Structure

    Private mEvents(mnEVENT_COUNT) As udsEvent
    Private mEventsRef(mnEVENT_COUNT) As udsEvent

    Private mHistory As udsHistory

    Private mMaint(7) As udsMaint
    Private mMaintRef(7) As udsMaint

    Private mOptions As udsOptions
    Private mOptionsRef As udsOptions
    '******** End Structures    *********************************************************************

    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject
    'Friend moPrivilege As PWPassword.cPrivilegeObject

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    '******** This is the old pw3 password object interop  ******************************************

    '********New program-to-program communication object******************************************
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '********************************************************************************************

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
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
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
           'Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)  ' This is confusing True but really false sub will look at privilege
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
                'hack me
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = True

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

        Dim oCulture As System.Globalization.CultureInfo = DisplayCulture

        Dim lRet As Response = MessageBox.Show(gcsRM.GetString("csSAVEMSG1", oCulture) & vbCrLf & _
                                               gcsRM.GetString("csSAVEMSG", oCulture), _
                                               gcsRM.GetString("csSAVE_CHANGES", oCulture), _
                                               MessageBoxButtons.YesNoCancel, _
                                               MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                Call subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                Return False
            Case Else
                Call subLoadData()
                Return True
        End Select

    End Function

    Private Function bCheckEditsMade() As Boolean
        '********************************************************************************************
        'Description:  Verify that at least one item was changed on the current tab. Return False if
        '              not.
        '
        'Parameters:  none
        'Returns:     True if EditsMade is valid.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case msCurrentTabName ' tabMain.SelectedTab.Name
            Case "TabPage1", "TabPage2", "TabPage3"
                For nEvent As Integer = 1 To mnEVENT_COUNT
                    If mEvents(nEvent).Enable <> mEventsRef(nEvent).Enable Then Return True
                    If mEvents(nEvent).EndHour <> mEventsRef(nEvent).EndHour Then Return True
                    If mEvents(nEvent).EndMin <> mEventsRef(nEvent).EndMin Then Return True
                    If mEvents(nEvent).EndSec <> mEventsRef(nEvent).EndSec Then Return True
                    If mEvents(nEvent).StartHour <> mEventsRef(nEvent).StartHour Then Return True
                    If mEvents(nEvent).StartMin <> mEventsRef(nEvent).StartMin Then Return True
                    If mEvents(nEvent).StartSec <> mEventsRef(nEvent).StartSec Then Return True
                Next 'nEvent

            Case "TabPage4"
                For nDay As Integer = 1 To 7
                    If mMaint(nDay).Cleanup <> mMaintRef(nDay).Cleanup Then Return True
                    If mMaint(nDay).FBackup <> mMaintRef(nDay).FBackup Then Return True
                    If mMaint(nDay).RBackup <> mMaintRef(nDay).RBackup Then Return True
                    If mMaint(nDay).StartHour <> mMaintRef(nDay).StartHour Then Return True
                    If mMaint(nDay).StartMin <> mMaintRef(nDay).StartMin Then Return True
                    If mMaint(nDay).StartSec <> mMaintRef(nDay).StartSec Then Return True
                Next 'nDay

            Case "TabPage5"
                If mOptions.BackupPath <> mOptionsRef.BackupPath Then Return True
                If mOptions.NewFolder <> mOptionsRef.NewFolder Then Return True
                If mOptions.BackupDB <> mOptionsRef.BackupDB Then Return True
                If mOptions.BackupRM <> mOptionsRef.BackupRM Then Return True
                If mOptions.BackupRS <> mOptionsRef.BackupRS Then Return True
                If mOptions.BackupRT <> mOptionsRef.BackupRT Then Return True
                If mOptions.BackupImage <> mOptionsRef.BackupImage Then Return True
                If mOptions.BackupAdd <> mOptionsRef.BackupAdd Then Return True
                If mOptions.ExtraCopies <> mOptionsRef.ExtraCopies Then Return True

                'TODO - Need a way to verify dgvFolders has changed. For now, trust EditsMade.
                Return True
        End Select

        'If it makes it to here...
        Return False

    End Function
    Private Function bSaveExtraCopyDataToXML() As Boolean
        '********************************************************************************************
        'Description:  Load Extra Copy Folders Grid.
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedExtraCopies"
        Const sXMLNODE As String = "ExtraCopy"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                If (IO.File.Exists(sXMLFilePath) = False) Then
                    IO.File.Create(sXMLFilePath)
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                Else
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    Catch ex As Exception
                        oXMLDoc = New XmlDocument
                        oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
                        oXMLDoc.AppendChild(oMainNode)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    End Try
                End If
                oMainNode.RemoveAll()
                If mbAddExtraModified Then
                    Call subUpdateChangeLog(mcolZones.ActiveZone.ZoneNumber)
                    mbAddExtraModified = False
                End If
                For Each oRow As DataGridViewRow In dgvExtraCopies.Rows
                    Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                    Dim oFromNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "From", Nothing)
                    oFromNode.InnerXml = oRow.Cells("ExtraFrom").Value.ToString
                    oNode.AppendChild(oFromNode)
                    Dim oToNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "To", Nothing)
                    oToNode.InnerXml = oRow.Cells("ExtraTo").Value.ToString
                    oNode.AppendChild(oToNode)
                    Dim oISNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "IncludeSubfolders", Nothing)
                    oISNode.InnerXml = oRow.Cells("ExtraSubfolders").Value.ToString
                    oNode.AppendChild(oISNode)


                    oMainNode.AppendChild(oNode)
                Next
                oXMLDoc.Save(sXMLFilePath)
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bSaveFolderDataToXML() As Boolean
        '********************************************************************************************
        'Description:  save Additional Backup Folders Grid.
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedAdditionalFolders"
        Const sXMLNODE As String = "AdditionalFolders"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                If (IO.File.Exists(sXMLFilePath) = False) Then
                    IO.File.Create(sXMLFilePath)
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                Else
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    Catch ex As Exception
                        oXMLDoc = New XmlDocument
                        oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
                        oXMLDoc.AppendChild(oMainNode)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    End Try
                End If
                oMainNode.RemoveAll()
                'record in change log
                If mbAddFoldersModified Then
                    Call subUpdateChangeLog(mcolZones.ActiveZone.ZoneNumber)
                    mbAddFoldersModified = False
                End If
                For Each oRow As DataGridViewRow In dgvFolders.Rows
                    Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                    Dim oBNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Name", Nothing)
                    oBNNode.InnerXml = oRow.Cells("BackupName").Value.ToString
                    oNode.AppendChild(oBNNode)
                    Dim oFPNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "FolderPath", Nothing)
                    oFPNode.InnerXml = oRow.Cells("FolderPath").Value.ToString
                    oNode.AppendChild(oFPNode)
                    Dim oISNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "IncludeSubfolders", Nothing)
                    oISNode.InnerXml = oRow.Cells("Subfolders").Value.ToString
                    oNode.AppendChild(oISNode)

                    oMainNode.AppendChild(oNode)
                Next
                oXMLDoc.Save(sXMLFilePath)
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    
    Private Function bSaveMaintDataToXML() As Boolean
        '********************************************************************************************
        'Description:  Load Maint Data
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedBackup"
        Const sXMLNODE As String = "Backup"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim oMaint As udsMaint = Nothing

        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                If (IO.File.Exists(sXMLFilePath) = False) Then
                    IO.File.Create(sXMLFilePath)
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                Else
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    Catch ex As Exception
                        oXMLDoc = New XmlDocument
                        oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
                        oXMLDoc.AppendChild(oMainNode)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    End Try
                End If
                oMainNode.RemoveAll()
                For nItem As Integer = 1 To 7 'days
                    oMaint = mMaint(nItem)

                    Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                    Dim oIDNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ID", Nothing)
                    oIDNode.InnerXml = nItem.ToString
                    oNode.AppendChild(oIDNode)

                    Dim oBNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartHour", Nothing)
                    oBNNode.InnerXml = oMaint.StartHour.ToString
                    oNode.AppendChild(oBNNode)
                    Dim oFPNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartMinute", Nothing)
                    oFPNode.InnerXml = oMaint.StartMin.ToString
                    oNode.AppendChild(oFPNode)
                    Dim oISNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartSecond", Nothing)
                    oISNode.InnerXml = oMaint.StartSec.ToString
                    oNode.AppendChild(oISNode)
                    Dim oCUNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "CleanupEnabled", Nothing)
                    oCUNode.InnerXml = oMaint.Cleanup.ToString
                    oNode.AppendChild(oCUNode)
                    Dim oBUNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "BackupEnabled", Nothing)
                    oBUNode.InnerXml = oMaint.FBackup.ToString
                    oNode.AppendChild(oBUNode)
                    Dim oRBNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "RobotBackupEnabled", Nothing)
                    oRBNode.InnerXml = oMaint.RBackup.ToString
                    oNode.AppendChild(oRBNode)

                    oMainNode.AppendChild(oNode)

                    If oMaint.StartHour <> mMaintRef(nItem).StartHour Or _
                        oMaint.StartMin <> mMaintRef(nItem).StartMin Or _
                        oMaint.StartSec <> mMaintRef(nItem).StartSec Then
                        'update change log
                        Dim sFrom As String = sGetTimeString(mMaintRef(nItem).StartHour, _
                                                             mMaintRef(nItem).StartMin, _
                                                             mMaintRef(nItem).StartSec)
                        Dim sTo As String = sGetTimeString(oMaint.StartHour, _
                                                           oMaint.StartMin, _
                                                           oMaint.StartSec)
                        Dim sParam As String = oMaint.Label & gpsRM.GetString("psSTART_TIME_TXT", _
                                                                              DisplayCulture)
                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    If oMaint.Cleanup <> mMaintRef(nItem).Cleanup Then
                        'update change log
                        Dim sFrom As String = mMaintRef(nItem).Cleanup.ToString
                        Dim sTo As String = oMaint.Cleanup.ToString
                        Dim sParam As String = oMaint.Label & gpsRM.GetString("psCLEANUP_TXT", _
                                                                              DisplayCulture)

                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    If oMaint.FBackup <> mMaintRef(nItem).FBackup Then
                        'update change log
                        Dim sFrom As String = mMaintRef(nItem).FBackup.ToString
                        Dim sTo As String = oMaint.FBackup.ToString
                        Dim sParam As String = oMaint.Label & gpsRM.GetString("psFBACKUP_TXT", _
                                                                              DisplayCulture)

                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    If oMaint.RBackup <> mMaintRef(nItem).RBackup Then
                        'update change log
                        Dim sFrom As String = mMaintRef(nItem).RBackup.ToString
                        Dim sTo As String = oMaint.RBackup.ToString
                        Dim sParam As String = oMaint.Label & gpsRM.GetString("psRBACKUP_TXT", _
                                                                              DisplayCulture)

                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If
                Next
                oXMLDoc.Save(sXMLFilePath)

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bSaveOptionDataToXML() As Boolean
        '********************************************************************************************
        'Description:  Load History Data
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        ' 05/16/14  MSW     Add weekly reports to scheduler 
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedOptions"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                
                If (IO.File.Exists(sXMLFilePath) = False) Then
                    IO.File.Create(sXMLFilePath)
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                Else
                    Try
                        oXMLDoc.Load(sXMLFilePath)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    Catch ex As Exception
                        oXMLDoc = New XmlDocument
                        oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
                        oXMLDoc.AppendChild(oMainNode)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    End Try
                End If
                oMainNode.RemoveAll()
                Dim oBPNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Path", Nothing)
                oBPNode.InnerXml = mOptions.BackupPath.ToString
                oMainNode.AppendChild(oBPNode)
                Dim oNFNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "NewFolder", Nothing)
                oNFNode.InnerXml = mOptions.NewFolder.ToString
                oMainNode.AppendChild(oNFNode)
                Dim oDBNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Database", Nothing)
                oDBNode.InnerXml = mOptions.BackupDB.ToString
                oMainNode.AppendChild(oDBNode)
                Dim oRMNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "RobotMaster", Nothing)
                oRMNode.InnerXml = mOptions.BackupRM.ToString
                oMainNode.AppendChild(oRMNode)
                Dim oRSNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "RobotScheduled", Nothing)
                oRSNode.InnerXml = mOptions.BackupRS.ToString
                oMainNode.AppendChild(oRSNode)
                Dim oRTNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "RobotTemp", Nothing)
                oRTNode.InnerXml = mOptions.BackupRT.ToString
                oMainNode.AppendChild(oRTNode)
                Dim oRINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "RobotImage", Nothing)
                oRINode.InnerXml = mOptions.BackupImage.ToString
                oMainNode.AppendChild(oRINode)
                Dim oAFNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "AdditionalFolders", Nothing)
                oAFNode.InnerXml = mOptions.BackupAdd.ToString
                oMainNode.AppendChild(oAFNode)
                Dim oECNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ExtraCopies", Nothing)
                oECNode.InnerXml = mOptions.ExtraCopies.ToString
                oMainNode.AppendChild(oECNode)
                Dim oLFNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Logfile", Nothing)
                oLFNode.InnerXml = mOptions.Logfile.ToString
                oMainNode.AppendChild(oLFNode)
                Dim oNSSNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "NotifyStartShift", Nothing)
                oNSSNode.InnerXml = mOptions.NotifyStartShift.ToString
                oMainNode.AppendChild(oNSSNode)
                Dim oNESNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "NotifyEndShift", Nothing)
                oNESNode.InnerXml = mOptions.NotifyEndShift.ToString
                oMainNode.AppendChild(oNESNode)
                '    05/16/14   MSW     Add weekly reports to scheduler 
                Dim oWRSNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "WeeklyReports", Nothing)
                oWRSNode.InnerXml = mOptions.WeeklyReports.ToString
                oMainNode.AppendChild(oWRSNode)

                oXMLDoc.Save(sXMLFilePath)

                If mOptions.BackupPath <> mOptionsRef.BackupPath Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupPath
                    Dim sTo As String = mOptions.BackupPath
                    Dim sParam As String = gpsRM.GetString("psBACK_PATH_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.NewFolder <> mOptionsRef.NewFolder Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.NewFolder.ToString
                    Dim sTo As String = mOptions.NewFolder.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_NEWFOLD_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupDB <> mOptionsRef.BackupDB Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupDB.ToString
                    Dim sTo As String = mOptions.BackupDB.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_DBBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupRM <> mOptionsRef.BackupRM Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupRM.ToString
                    Dim sTo As String = mOptions.BackupRM.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_RMBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupRS <> mOptionsRef.BackupRS Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupRS.ToString
                    Dim sTo As String = mOptions.BackupRS.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_RSBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupRT <> mOptionsRef.BackupRT Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupRT.ToString
                    Dim sTo As String = mOptions.BackupRT.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_RTBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupImage <> mOptionsRef.BackupImage Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupImage.ToString
                    Dim sTo As String = mOptions.BackupImage.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_IMGBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.BackupAdd <> mOptionsRef.BackupAdd Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.BackupAdd.ToString
                    Dim sTo As String = mOptions.BackupAdd.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_ADDBACK_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If

                If mOptions.ExtraCopies <> mOptionsRef.ExtraCopies Then
                    'update change log
                    Dim sFrom As String = mOptionsRef.ExtraCopies.ToString
                    Dim sTo As String = mOptions.ExtraCopies.ToString
                    Dim sParam As String = gpsRM.GetString("psCHK_EXT_COP_TXT", DisplayCulture)

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                End If



            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function

    Private Function bLoadExtraCopyDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load Extra Copy Folders Grid.
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedExtraCopies"
        Const sXMLNODE As String = "ExtraCopy"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                dgvExtraCopies.Rows.Clear()
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        dgvExtraCopies.Rows.Add()
                        With dgvExtraCopies.Rows(dgvExtraCopies.Rows.Count - 1)
                            .Cells("ExtraFrom").Value = oNode.Item("From").InnerXml
                            .Cells("ExtraTo").Value = oNode.Item("To").InnerXml
                            .Cells("ExtraSubfolders").Value = CType(oNode.Item("IncludeSubfolders").InnerXml, Boolean)
                        End With
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bLoadFolderDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load Additional Backup Folders Grid.
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedAdditionalFolders"
        Const sXMLNODE As String = "AdditionalFolders"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                dgvFolders.Rows.Clear()
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        dgvFolders.Rows.Add()
                        With dgvFolders.Rows(dgvFolders.Rows.Count - 1)
                            .Cells("BackupName").Value = oNode.Item("Name").InnerXml
                            .Cells("FolderPath").Value = oNode.Item("FolderPath").InnerXml
                            .Cells("Subfolders").Value = CType(oNode.Item("IncludeSubfolders").InnerXml, Boolean)
                        End With
                    Next
                Else

                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bLoadEventDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load Event data structure..
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "Schedules"
        Const sXMLNODE As String = "Schedule"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        Dim nID As Integer = CType(oNode.Item("ID").InnerXml, Integer)
                        Dim bZoneCheck As Boolean = True
                        Try
                            Dim sZoneTmp As String = oNode.Item("Zone").InnerXml
                            If sZoneTmp = "0" Or sZoneTmp = String.Empty Or sZoneTmp = oZone.ZoneNumber.ToString Then
                                bZoneCheck = True
                            Else
                                bZoneCheck = False
                            End If
                        Catch ex As Exception

                        End Try
                        If bZoneCheck Then
                            With mEventsRef(nID)
                                .StartHour = CType(oNode.Item("StartHour").InnerXml, Integer)
                                .StartMin = CType(oNode.Item("StartMinute").InnerXml, Integer)
                                .StartSec = CType(oNode.Item("StartSecond").InnerXml, Integer)
                                .EndHour = CType(oNode.Item("EndHour").InnerXml, Integer)
                                .EndMin = CType(oNode.Item("EndMinute").InnerXml, Integer)
                                .EndSec = CType(oNode.Item("EndSecond").InnerXml, Integer)
                                .Enable = CType(oNode.Item("Enabled").InnerXml, Boolean)
                            End With
                            mEvents(nID) = mEventsRef(nID)
                        End If
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

        Return True
    End Function
    Private Function bLoadHistDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load History Data
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedLastPerformed"
        Const sXMLNODE As String = "LastPerformedEntry"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        Dim nID As Integer = CType(oNode.Item("ID").InnerXml, Integer)
                        Dim sItem As String = oNode.Item("Item").InnerXml
                        Dim sTmpCulture As String = msCulture
                        Culture = "en-US"
                        Dim dtLast As DateTime = CType(oNode.Item("LastPerformed").InnerXml, DateTime)
                        Culture = sTmpCulture
                        Select Case sItem.ToLower
                            Case "cleanup"
                                mHistory.LastCleanup = dtLast.Date.ToShortDateString
                            Case "robot backup"
                                mHistory.LastRBackup = dtLast.Date.ToShortDateString
                            Case "file backup"
                                mHistory.LastFBackup = dtLast.Date.ToShortDateString
                        End Select
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bLoadMaintDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load Maint Data
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedBackup"
        Const sXMLNODE As String = "Backup"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        Dim nID As Integer = CType(oNode.Item("ID").InnerXml, Integer)

                        With mMaintRef(nID)
                            .StartHour = CType(oNode.Item("StartHour").InnerXml, Integer)
                            .StartMin = CType(oNode.Item("StartMinute").InnerXml, Integer)
                            .StartSec = CType(oNode.Item("StartSecond").InnerXml, Integer)
                            .Cleanup = CType(oNode.Item("CleanupEnabled").InnerXml, Boolean)
                            .FBackup = CType(oNode.Item("BackupEnabled").InnerXml, Boolean)
                            .RBackup = CType(oNode.Item("RobotBackupEnabled").InnerXml, Boolean)
                        End With

                        'TODO - Code to check and correct invalid Time data?
                        mMaint(nID) = mMaintRef(nID)

                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function
    Private Function bLoadOptionDataFromXML() As Boolean
        '********************************************************************************************
        'Description:  Load History Data
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move from SQL to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedOptions"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If oMainNode IsNot Nothing Then
                    With mOptionsRef
                        .BackupPath = oMainNode.Item("Path").InnerXml
                        .NewFolder = CType(oMainNode.Item("NewFolder").InnerXml, Boolean)
                        .BackupDB = CType(oMainNode.Item("Database").InnerXml, Boolean)
                        .BackupRM = CType(oMainNode.Item("RobotMaster").InnerXml, Boolean)
                        .BackupRS = CType(oMainNode.Item("RobotScheduled").InnerXml, Boolean)
                        .BackupRT = CType(oMainNode.Item("RobotTemp").InnerXml, Boolean)
                        .BackupImage = CType(oMainNode.Item("RobotImage").InnerXml, Boolean)
                        .BackupAdd = CType(oMainNode.Item("AdditionalFolders").InnerXml, Boolean)
                        .ExtraCopies = CType(oMainNode.Item("ExtraCopies").InnerXml, Boolean)
                        .Logfile = CType(oMainNode.Item("Logfile").InnerXml, Boolean)
                        .NotifyStartShift = CType(oMainNode.Item("NotifyStartShift").InnerXml, Boolean)
                        .NotifyEndShift = CType(oMainNode.Item("NotifyEndShift").InnerXml, Boolean)
                        '    05/16/14   MSW     Add weekly reports to scheduler 
                        If oMainNode.InnerXml.Contains("WeeklyReports") Then
                            .WeeklyReports = CType(oMainNode.Item("WeeklyReports").InnerXml, Boolean)
                        Else
                            .WeeklyReports = False
                        End If

                    End With

                    mOptions = mOptionsRef
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Return True
    End Function

    Private Function bLoadScheduleData() As Boolean
        '********************************************************************************************
        'Description:  Load Schedule data stored on GUI in database.
        '
        'Parameters:  none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean

        Try

            'Load Shift Events
            bSuccess = bLoadEventDataFromXML()
            If bSuccess Then
                'Load Maint Events
                bSuccess = bLoadMaintDataFromXML()

                If bSuccess Then
                    bSuccess = bLoadHistDataFromXML()
                End If
            End If

            If bSuccess Then
                'Load Maint Events
                bSuccess = bLoadOptionDataFromXML()
            End If

            If bSuccess Then
                ''Load Additional Backup Folders
                bSuccess = bLoadFolderDataFromXML()
            End If

            If bSuccess Then
                'Load Extra Copies
                bSuccess = bLoadExtraCopyDataFromXML()
            End If

            Return bSuccess

        Catch ex As Exception
            mDebug.ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

            'DB.Close()
            Return False
        End Try

    End Function

    Private Function bPrintdoc(ByVal bPrint As Boolean, ByVal bPrintAll As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/10  MSW     Move return(true) so status and enable get updated
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.AppStarting
        Dim bCancel As Boolean = False
        Try
            btnPrint.Enabled = False
            Status = gcsRM.GetString("csPRINTING", DisplayCulture)

            Progress = 5

            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

            mPrintHtml.HeaderRowsPerTable = 1

            Dim oTab As TabPage = tabMain.SelectedTab
            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION") & " - " & oTab.Text
            Dim sTitle(1) As String
            Dim sStatus As String = String.Empty

            sTitle(0) = oTab.Text
            sTitle(1) = String.Empty

            Dim sSubTitle(0) As String
            sSubTitle(0) = mcolZones.SiteName & " - " & mcolZones.ActiveZone.Name

            Progress = 10
            mPrintHtml.subStartDoc(Status, sPageTitle,False,bCancel)
            if bCancel = false then
              If bPrintAll Then
                  sTitle(0) = tabMain.TabPages.Item(0).Text
                  Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 1)
                  sSubTitle(0) = String.Empty
                  sTitle(0) = tabMain.TabPages.Item(1).Text
                  Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 2)
                  sTitle(0) = tabMain.TabPages.Item(2).Text
                  Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 3)
                  mPrintHtml.subAddPageBreak(sStatus)
                  sTitle(0) = tabMain.TabPages.Item(3).Text
                  Call subPrintMaint(mPrintHtml, sTitle, sSubTitle)
                  sTitle(0) = tabMain.TabPages.Item(4).Text
                  Call subPrintOptions(mPrintHtml, sTitle, sSubTitle)
              Else
                  Select Case oTab.Name
                      Case "TabPage1"
                          Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 1)
                      Case "TabPage2"
                          Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 2)
                      Case "TabPage3"
                          Call subPrintEvents(mPrintHtml, sTitle, sSubTitle, 3)
                      Case "TabPage4"
                          Call subPrintMaint(mPrintHtml, sTitle, sSubTitle)
                      Case "TabPage5"
                          Call subPrintOptions(mPrintHtml, sTitle, sSubTitle)
                      Case "TabPage6"
                          Call subPrintLog(mPrintHtml, sTitle, sSubTitle)
                  End Select
              End If
              '
              Progress = 80
              '
              Status = gcsRM.GetString("csPRINT_SENDING", DisplayCulture)
              mPrintHtml.subCloseFile(Status)
              If bPrint Then
                  mPrintHtml.subPrintDoc()
              End If
              Progress = 0
            end if
            Status = gcsRM.GetString("csREADY", DisplayCulture)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            Return (Not(bCancel))
        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

            Progress = 0
            Status = gcsRM.GetString("csREADY", DisplayCulture)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            Return False
        End Try

    End Function

    Private Function bValidateEventData(ByRef DS As DataSet) As Boolean
        '********************************************************************************************
        'Description:  Make sure we have the right number of Events and IDs are valid.
        '
        'Parameters:  Event Dataset
        'Returns:     True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DT As DataTable = New DataTable
        Dim bValid As Boolean = True

        Try
            DT.Locale = mLanguage.FixedCulture
            DT = DS.Tables("[" & gsSCHED_SHIFT_DS_TABLENAME & "]")

            If DT.Rows.Count < mnEVENT_COUNT Then
                bValid = False
            Else
                Dim bEvent(mnEVENT_COUNT) As Boolean

                For Each DR As DataRow In DT.Rows
                    Dim nID As Integer = CType(DR.Item(gsSCHED_SHIFT_ID), Integer)

                    If (nID < 1) Or (nID > mnEVENT_COUNT) Then
                        bValid = False
                        Exit For
                    End If

                    If bEvent(nID) Then
                        bValid = False
                        Exit For
                    Else
                        bEvent(nID) = True
                    End If
                Next
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bValidateEventData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return bValid

    End Function

    Private Function bValidateMaintData(ByRef DS As DataSet) As Boolean
        '********************************************************************************************
        'Description:  Make sure we have the right number of DataRows and IDs are valid.
        '
        'Parameters:  Maint Dataset
        'Returns:     True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DT As DataTable = New DataTable
        Dim bValid As Boolean = True

        Try
            DT.Locale = mLanguage.FixedCulture
            DT = DS.Tables("[" & gsSCHED_MAINT_DS_TABLENAME & "]")

            If DT.Rows.Count < 7 Then '1 row per weekday
                bValid = False
            Else
                Dim bEvent(7) As Boolean

                For Each DR As DataRow In DT.Rows
                    Dim nID As Integer = CType(DR.Item(gsSCHED_MAINT_ID), Integer)

                    If (nID < 1) Or (nID > 7) Then
                        bValid = False
                        Exit For
                    End If

                    If bEvent(nID) Then
                        bValid = False
                        Exit For
                    Else
                        bEvent(nID) = True
                    End If
                Next
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bValidateMaintData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return bValid

    End Function

    Private Function bSaveEventsToXML() As Boolean
        '********************************************************************************************
        'Description:  Save Event (shift times) data to XML Database
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Convert to XML
        '********************************************************************************************
        Dim oEvent As udsEvent = Nothing
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "Schedules"
        Const sXMLNODE As String = "Schedule"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            If (IO.File.Exists(sXMLFilePath) = False) Then
                IO.File.Create(sXMLFilePath)
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement(sXMLTABLE)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
            Else
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                Catch ex As Exception
                    oXMLDoc = New XmlDocument
                    oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
                    oXMLDoc.AppendChild(oMainNode)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                End Try
            End If
            oMainNode.RemoveAll()

            Try
                For nItem As Integer = 1 To mnEVENT_COUNT
                    oEvent = mEvents(nItem)
                    oNode = oXMLDoc.CreateElement(sXMLNODE)

                    'write all info
                    Dim oIDNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ID", Nothing)
                    oIDNode.InnerXml = nItem.ToString
                    oNode.AppendChild(oIDNode)

                    Dim oSHNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartHour", Nothing)
                    oSHNode.InnerXml = oEvent.StartHour.ToString
                    oNode.AppendChild(oSHNode)
                    Dim oSMNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartMinute", Nothing)
                    oSMNode.InnerXml = oEvent.StartMin.ToString
                    oNode.AppendChild(oSMNode)
                    Dim oSSNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "StartSecond", Nothing)
                    oSSNode.InnerXml = oEvent.StartSec.ToString
                    oNode.AppendChild(oSSNode)

                    Dim oEHNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "EndHour", Nothing)
                    oEHNode.InnerXml = oEvent.EndHour.ToString
                    oNode.AppendChild(oEHNode)
                    Dim oEMNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "EndMinute", Nothing)
                    oEMNode.InnerXml = oEvent.EndMin.ToString
                    oNode.AppendChild(oEMNode)
                    Dim oESNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "EndSecond", Nothing)
                    oESNode.InnerXml = oEvent.EndSec.ToString
                    oNode.AppendChild(oESNode)

                    Dim oEnableNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Enabled", Nothing)
                    oEnableNode.InnerXml = oEvent.Enable.ToString
                    oNode.AppendChild(oEnableNode)

                    Dim oZoneNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Zone", Nothing)
                    oZoneNode.InnerXml = oZone.ZoneNumber.ToString
                    oNode.AppendChild(oZoneNode)

                    oMainNode.AppendChild(oNode)

                    If oEvent.StartHour <> mEventsRef(nItem).StartHour Or _
                       oEvent.StartMin <> mEventsRef(nItem).StartMin Or _
                       oEvent.StartSec <> mEventsRef(nItem).StartSec Then
                        'update change log
                        Dim sFrom As String = sGetTimeString(mEventsRef(nItem).StartHour, _
                                                             mEventsRef(nItem).StartMin, _
                                                             mEventsRef(nItem).StartSec)
                        Dim sTo As String = sGetTimeString(oEvent.StartHour, _
                                                           oEvent.StartMin, _
                                                           oEvent.StartSec)
                        Dim sParam As String = oEvent.Label & gpsRM.GetString("psSTART_TIME_TXT", _
                                                                              DisplayCulture)
                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    If oEvent.EndHour <> mEventsRef(nItem).EndHour Or _
                       oEvent.EndMin <> mEventsRef(nItem).EndMin Or _
                       oEvent.EndSec <> mEventsRef(nItem).EndSec Then
                        'update change log
                        Dim sFrom As String = sGetTimeString(mEventsRef(nItem).EndHour, _
                                                             mEventsRef(nItem).EndMin, _
                                                             mEventsRef(nItem).EndSec)
                        Dim sTo As String = sGetTimeString(oEvent.EndHour, _
                                                           oEvent.EndMin, _
                                                           oEvent.EndSec)
                        Dim sParam As String = oEvent.Label & gpsRM.GetString("psEND_TIME_TXT", _
                                                                              DisplayCulture)
                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    If oEvent.Enable <> mEventsRef(nItem).Enable Then
                        'update change log
                        Dim sFrom As String = mEventsRef(nItem).Enable.ToString
                        Dim sTo As String = oEvent.Enable.ToString
                        Dim sParam As String = oEvent.Label & gpsRM.GetString("psENABLED_TXT", _
                                                                              DisplayCulture)

                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, oZone, sParam, String.Empty)
                    End If

                    oXMLDoc.Save(sXMLFilePath)

                Next 'nItem



                Return True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveEventsToSQLDB", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

    End Function


    Private Function bSaveToGUI() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Select Case msCurrentTabName
                Case "TabPage1", "TabPage2", "TabPage3"
                    Dim bSuccess As Boolean

                    bSuccess = bSaveEventsToXML()
                    'Let SchedMan know that the schedule changed
                    Call subSchedManUpdateReq()
                    Return bSuccess

                Case "TabPage4"
                    Dim bSuccess As Boolean

                    bSuccess = bSaveMaintDataToXML()
                    'Let SchedMan know that the schedule changed
                    Call subSchedManUpdateReq()
                    Return bSuccess

                Case "TabPage5"
                    Dim bSuccess As Boolean

                    bSuccess = bSaveOptionDataToXML()
                    If bSuccess Then
                        If bSaveFolderDataToXML() Then
                            Return bSaveExtraCopyDataToXML()
                        Else
                            Return False
                        End If
                    Else
                        Return False
                    End If
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveToGUI", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function


    Private Function GetSelectedDGVRow(ByRef oDGV As DataGridView) As Integer
        '********************************************************************************************
        'Description: Returns the selected Row index in dgvFolders. If no row selected, returns -1.
        '
        'Parameters: none
        'Returns:    Selected Row Index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nSelectedRow As Integer = -1

        If oDGV.Rows.Count > 0 Then
            For nIndex As Integer = 0 To (oDGV.Rows.Count - 1)
                If oDGV.Rows(nIndex).Selected Then
                    nSelectedRow = nIndex
                End If
            Next 'nIndex
        End If

        Return nSelectedRow

    End Function

    Private Function sGetTimeString(ByVal Hour As Integer, ByVal Minute As Integer, ByVal Second As Integer) As String
        '********************************************************************************************
        'Description: Converts 24 hour Hour, Minute, and Second to 12 hour time string for Change log
        '             reporting.
        '
        'Parameters: none
        'Returns:    Selected Row Index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bPM As Boolean = Hour > 11
        Dim sTime As String = String.Empty

        Hour = Hour Mod 12
        If Hour < 1 Then Hour = 12

        sTime = Hour.ToString.PadLeft(2, "0"c) & ":"
        sTime = sTime & Minute.ToString.PadLeft(2, "0"c) & ":"
        sTime = sTime & Second.ToString.PadLeft(2, "0"c) & " "

        If bPM Then
            sTime = sTime & "PM"
        Else
            sTime = sTime & "AM"
        End If

        Return sTime

    End Function

    Private Sub subAddMaintChkHandlers()
        '********************************************************************************************
        'Description: Handle all of the Maintenance Selection CheckBox control CheckedChanged events 
        '             with one event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oControl As Control In TabPage4.Controls
            If TypeOf oControl Is CheckBox Then
                Dim oCHK As CheckBox = DirectCast(oControl, CheckBox)

                AddHandler oCHK.CheckedChanged, AddressOf chkMaint_CheckedChanged
            End If
        Next 'oControl

    End Sub

    Private Sub subAddOptionChkHandlers()
        '********************************************************************************************
        'Description: Handle all of the Backup Option CheckBox control CheckedChanged events with one
        '             event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oControl As Control In TabPage5.Controls
            If TypeOf oControl Is CheckBox Then
                Dim oCHK As CheckBox = DirectCast(oControl, CheckBox)

                AddHandler oCHK.CheckedChanged, AddressOf chkOption_CheckedChanged
            End If
        Next 'oControl

    End Sub

    Private Sub subAddShiftChkHandlers()
        '********************************************************************************************
        'Description: Handle all of the Shift/Break CheckBox control CheckedChanged events with 
        '             one event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oControl As Control In TabPage1.Controls
            If TypeOf oControl Is CheckBox Then
                Dim oCHK As CheckBox = DirectCast(oControl, CheckBox)

                AddHandler oCHK.CheckedChanged, AddressOf chkShift_CheckedChanged
            End If
        Next 'oControl

        For Each oControl As Control In TabPage2.Controls
            If TypeOf oControl Is CheckBox Then
                Dim oCHK As CheckBox = DirectCast(oControl, CheckBox)

                AddHandler oCHK.CheckedChanged, AddressOf chkShift_CheckedChanged
            End If
        Next 'oControl

        For Each oControl As Control In TabPage3.Controls
            If TypeOf oControl Is CheckBox Then
                Dim oCHK As CheckBox = DirectCast(oControl, CheckBox)

                AddHandler oCHK.CheckedChanged, AddressOf chkShift_CheckedChanged
            End If
        Next 'oControl

    End Sub

    Private Sub subAddMaintDtpHandlers()
        '********************************************************************************************
        'Description: Handle all of the Maintenance DateTimePicker control ValueChanged events with 
        '             one event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oControl As Control In TabPage4.Controls
            If TypeOf oControl Is DateTimePicker Then
                Dim oDTP As DateTimePicker = DirectCast(oControl, DateTimePicker)

                AddHandler oDTP.ValueChanged, AddressOf dtpMaint_ValueChanged
            End If
        Next 'oControl

    End Sub

    Private Sub subAddShiftDtpHandlers()
        '********************************************************************************************
        'Description: Handle all of the Shift/Break DateTimePicker control ValueChanged events with 
        '             one event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oControl As Control In TabPage1.Controls
            If TypeOf oControl Is DateTimePicker Then
                Dim oDTP As DateTimePicker = DirectCast(oControl, DateTimePicker)

                AddHandler oDTP.ValueChanged, AddressOf dtpShift_ValueChanged
            End If
        Next 'oControl

        For Each oControl As Control In TabPage2.Controls
            If TypeOf oControl Is DateTimePicker Then
                Dim oDTP As DateTimePicker = DirectCast(oControl, DateTimePicker)

                AddHandler oDTP.ValueChanged, AddressOf dtpShift_ValueChanged
            End If
        Next 'oControl

        For Each oControl As Control In TabPage3.Controls
            If TypeOf oControl Is DateTimePicker Then
                Dim oDTP As DateTimePicker = DirectCast(oControl, DateTimePicker)

                AddHandler oDTP.ValueChanged, AddressOf dtpShift_ValueChanged
            End If
        Next 'oControl

    End Sub

    Private Sub subAddTabClickHandlers()
        '********************************************************************************************
        'Description: Handle all of the Tab Page Click events of interest withone event handler.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For Each oTab As TabPage In tabMain.TabPages
            If Not (oTab.Name = "TabPage6") Then
                AddHandler oTab.Click, AddressOf tabPage_Click
            End If
        Next 'oTab

    End Sub

    Private Sub subBuildGrid()
        '********************************************************************************************
        'Description: Initialize the "Additional Folders" grid on TabPage5
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            With dgvFolders
                'Default black forecolor
                .ForeColor = System.Drawing.Color.Black
                'Normal text in all cells + NoSort
                For nCol As Integer = 0 To (.Columns.Count - 1)
                    .Columns(nCol).DefaultCellStyle.Font = New Font(.Font, FontStyle.Regular)
                    .Columns(nCol).SortMode = DataGridViewColumnSortMode.NotSortable
                Next 'nCol
                'Globalize Column Names
                .Columns("BackupName").HeaderText = gpsRM.GetString("psFOLDER_NAME", DisplayCulture)
                .Columns("FolderPath").HeaderText = gpsRM.GetString("psFOLDER_LOCATION", DisplayCulture)
                .Columns("Subfolders").HeaderText = gpsRM.GetString("psSUBFOLDERS", DisplayCulture)
                'Adjust Column Widths
                .Columns("BackupName").Width = 120
                .Columns("FolderPath").Width = 322
                .Columns("Subfolders").Width = 78
                'Other customizations
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .MultiSelect = False
                .AllowUserToOrderColumns = False
            End With
            With dgvExtraCopies
                'Default black forecolor
                .ForeColor = System.Drawing.Color.Black
                'Normal text in all cells + NoSort
                For nCol As Integer = 0 To (.Columns.Count - 1)
                    .Columns(nCol).DefaultCellStyle.Font = New Font(.Font, FontStyle.Regular)
                    .Columns(nCol).SortMode = DataGridViewColumnSortMode.NotSortable
                Next 'nCol
                'Globalize Column Names
                .Columns("ExtraFrom").HeaderText = gpsRM.GetString("psFROM", DisplayCulture)
                .Columns("ExtraTo").HeaderText = gpsRM.GetString("psTO", DisplayCulture)
                .Columns("ExtraSubFolders").HeaderText = gpsRM.GetString("psSUBFOLDERS", DisplayCulture)
                'Adjust Column Widths
                .Columns("ExtraFrom").Width = 221
                .Columns("ExtraTo").Width = 221
                .Columns("ExtraSubFolders").Width = 78
                'Other customizations
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .MultiSelect = False
                .AllowUserToOrderColumns = False
            End With
        Catch ex As Exception

        End Try

    End Sub

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

            tabMain.Enabled = True

            If cboZone.Text = msOldZone Then Exit Sub
        End If
        msOldZone = cboZone.Text
        mcolZones.CurrentZone = cboZone.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            tabMain.TabPages.Item(0).Select()

            If mbScreenLoaded = True Then
                Call subLoadData()
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub

    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. Check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/10  RJO     If user logged in and had Copy privilege, controls would not be enabled.
        '                   Added ePrivilege.Copy to Case statement
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            bRestOfControls = False
            PnlMain.Enabled = False

        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = False
                    PnlMain.Enabled = True

                Case ePrivilege.Edit, ePrivilege.Copy '02/02/10 RJO
                    btnSave.Enabled = (True And EditsMade)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = (True And DataLoaded)
                    PnlMain.Enabled = (True And DataLoaded)

            End Select
        End If

        '*************************************************************************
        'rest of controls here
        If bRestOfControls Then
            Call subUnlockControls()
        Else
            Call subLockControls()
        End If

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
        Dim oCulture As CultureInfo = DisplayCulture

        Try

            With gpsRM
                ' set labels for tab #1 [Shift 1]
                TabPage1.Text = .GetString("psTAB1_TXT", oCulture)
                lblShift1StartTime.Text = .GetString("psSTART_TIME_TXT", oCulture)
                lblShift1EndTime.Text = .GetString("psEND_TIME_TXT", oCulture)
                lblShift1.Text = .GetString("psTAB1_TXT", oCulture)
                lblShift1Break1.Text = .GetString("psBREAK1_TXT", oCulture)
                lblShift1Break2.Text = .GetString("psBREAK2_TXT", oCulture)
                lblShift1Break3.Text = .GetString("psBREAK3_TXT", oCulture)
                lblShift1Break4.Text = .GetString("psBREAK4_TXT", oCulture)
                lblShift1Break5.Text = .GetString("psBREAK5_TXT", oCulture)
                chkShift1Enable.Text = .GetString("psSHIFT1_EN_TXT", oCulture)
                chkShift1Br1Enable.Text = .GetString("psBREAK1_EN_TXT", oCulture)
                chkShift1Br2Enable.Text = .GetString("psBREAK2_EN_TXT", oCulture)
                chkShift1BR3Enable.Text = .GetString("psBREAK3_EN_TXT", oCulture)
                chkShift1Br4Enable.Text = .GetString("psBREAK4_EN_TXT", oCulture)
                chkShift1Br5Enable.Text = .GetString("psBREAK5_EN_TXT", oCulture)

                ' set labels for tab #2 [Shift 2]
                TabPage2.Text = .GetString("psTAB2_TXT", oCulture)
                lblShift2StartTime.Text = .GetString("psSTART_TIME_TXT", oCulture)
                lblShift2EndTime.Text = .GetString("psEND_TIME_TXT", oCulture)
                lblShift2.Text = .GetString("psTAB2_TXT", oCulture)
                lblShift2Break1.Text = .GetString("psBREAK1_TXT", oCulture)
                lblShift2Break2.Text = .GetString("psBREAK2_TXT", oCulture)
                lblShift2Break3.Text = .GetString("psBREAK3_TXT", oCulture)
                lblShift2Break4.Text = .GetString("psBREAK4_TXT", oCulture)
                lblShift2Break5.Text = .GetString("psBREAK5_TXT", oCulture)
                chkShift2Enable.Text = .GetString("psSHIFT2_EN_TXT", oCulture)
                chkShift2Br1Enable.Text = .GetString("psBREAK1_EN_TXT", oCulture)
                chkShift2Br2Enable.Text = .GetString("psBREAK2_EN_TXT", oCulture)
                chkShift2Br3Enable.Text = .GetString("psBREAK3_EN_TXT", oCulture)
                chkShift2Br4Enable.Text = .GetString("psBREAK4_EN_TXT", oCulture)
                chkShift2Br5Enable.Text = .GetString("psBREAK5_EN_TXT", oCulture)

                ' set labels for tab #3 [Shift 3]
                TabPage3.Text = .GetString("psTAB3_TXT", oCulture)
                lblShift3StartTime.Text = .GetString("psSTART_TIME_TXT", oCulture)
                lblShift3EndTime.Text = .GetString("psEND_TIME_TXT", oCulture)
                lblShift3.Text = .GetString("psTAB3_TXT", oCulture)
                lblShift3Break1.Text = .GetString("psBREAK1_TXT", oCulture)
                lblShift3Break2.Text = .GetString("psBREAK2_TXT", oCulture)
                lblShift3Break3.Text = .GetString("psBREAK3_TXT", oCulture)
                lblShift3Break4.Text = .GetString("psBREAK4_TXT", oCulture)
                lblShift3Break5.Text = .GetString("psBREAK5_TXT", oCulture)
                chkShift3Enable.Text = .GetString("psSHIFT3_EN_TXT", oCulture)
                chkShift3Br1Enable.Text = .GetString("psBREAK1_EN_TXT", oCulture)
                chkShift3Br2Enable.Text = .GetString("psBREAK2_EN_TXT", oCulture)
                chkShift3Br3Enable.Text = .GetString("psBREAK3_EN_TXT", oCulture)
                chkShift3Br4Enable.Text = .GetString("psBREAK4_EN_TXT", oCulture)
                chkShift3Br5Enable.Text = .GetString("psBREAK5_EN_TXT", oCulture)

                ' set labels for tab #4 [Cleanup/Backup]
                TabPage4.Text = .GetString("psTAB4_TXT", oCulture)
                lblCleanup.Text = .GetString("psCLEANUP_TXT", oCulture)
                lblRBackup.Text = .GetString("psRBACKUP_TXT", oCulture)
                lblFBackup.Text = .GetString("psFBACKUP_TXT", oCulture)
                lblMaintStartTime.Text = .GetString("psSTART_TIME_TXT", oCulture)
                lblSunday.Text = .GetString("psSUNDAY_TXT", oCulture)
                lblMonday.Text = .GetString("psMONDAY_TXT", oCulture)
                lblTuesday.Text = .GetString("psTUESDAY_TXT", oCulture)
                lblWednesday.Text = .GetString("psWEDNESDAY_TXT", oCulture)
                lblThursday.Text = .GetString("psTHURSDAY_TXT", oCulture)
                lblFriday.Text = .GetString("psFRIDAY_TXT", oCulture)
                lblSaturday.Text = .GetString("psSATURDAY_TXT", oCulture)
                lblLastPerformed.Text = .GetString("psLBL_LAST_PERF_TXT", oCulture)

                Dim sEnabled As String = .GetString("psENABLED_TXT", oCulture)

                chkCleanup1.Text = sEnabled
                chkCleanup2.Text = sEnabled
                chkCleanup3.Text = sEnabled
                chkCleanup4.Text = sEnabled
                chkCleanup5.Text = sEnabled
                chkCleanup6.Text = sEnabled
                chkCleanup7.Text = sEnabled
                chkRBackup1.Text = sEnabled
                chkRBackup2.Text = sEnabled
                chkRBackup3.Text = sEnabled
                chkRBackup4.Text = sEnabled
                chkRBackup5.Text = sEnabled
                chkRBackup6.Text = sEnabled
                chkRBackup7.Text = sEnabled
                chkFBackup1.Text = sEnabled
                chkFBackup2.Text = sEnabled
                chkFBackup3.Text = sEnabled
                chkFBackup4.Text = sEnabled
                chkFBackup5.Text = sEnabled
                chkFBackup6.Text = sEnabled
                chkFBackup7.Text = sEnabled

                ' set labels for tab #5 [File Backup Options]
                TabPage5.Text = .GetString("psTAB5_TXT", oCulture)
                lblBackupPath.Text = .GetString("psBACK_PATH_TXT", oCulture)
                btnBrowse.Text = .GetString("psBTN_BROWSE_TXT", oCulture)
                chkNewFolder.Text = .GetString("psCHK_NEWFOLD_TXT", oCulture)
                chkBackupDB.Text = .GetString("psCHK_DBBACK_TXT", oCulture)
                chkBackupRM.Text = .GetString("psCHK_RMBACK_TXT", oCulture)
                chkBackupRS.Text = .GetString("psCHK_RSBACK_TXT", oCulture)
                chkBackupRT.Text = .GetString("psCHK_RTBACK_TXT", oCulture)
                chkBackupImage.Text = .GetString("psCHK_IMGBACK_TXT", oCulture)
                chkBackupAdd.Text = .GetString("psCHK_ADDBACK_TXT", oCulture)
                chkExtraCopies.Text = .GetString("psCHK_EXT_COP_TXT", oCulture)
                btnAdd.Text = .GetString("psBTN_ADD_TXT", oCulture)
                btnRemove.Text = .GetString("psBTN_REMOVE_TXT", oCulture)
                btnEdit.Text = .GetString("psBTN_EDIT_TXT", oCulture)
                btnAddExtra.Text = .GetString("psBTN_ADD_TXT", oCulture)
                btnRemoveExtra.Text = .GetString("psBTN_REMOVE_TXT", oCulture)
                btnEditExtra.Text = .GetString("psBTN_EDIT_TXT", oCulture)
                ' pnlAddFolders
                btnAddFolderAccept.Text = .GetString("psBTN_AF_ACCEPT_TXT", oCulture)
                btnAddFolderBrowse.Text = .GetString("psBTN_AF_BROWSE_TXT", oCulture)
                btnAddFolderCancel.Text = .GetString("psBTN_AF_CANCEL_TXT", oCulture)
                chkSubfolders.Text = .GetString("psCHK_AF_SUBFOLD_TXT", oCulture)
                lblAddFolderPanel.Text = .GetString("psLBL_AF_PANEL_TXT", oCulture)
                lblAddFolderName.Text = .GetString("psLBL_AF_NAME_TXT", oCulture)
                lblAddFolderPath.Text = .GetString("psLBL_AF_PATH_TXT", oCulture)
                lblAddFolderHint.Text = .GetString("psLBL_AF_HINT_TXT", oCulture)
                lblAddExtra.Text = .GetString("psLBL_AE_PANEL_TXT", oCulture)
                lblExtraFrom.Text = .GetString("psLBL_AE_FROM_TXT", oCulture)
                lblExtraTo.Text = .GetString("psLBL_AE_TO_TXT", oCulture)


                ' set labels for tab #6 [Event Log]
                TabPage6.Text = .GetString("psTAB6_TXT", oCulture)

                ' modify standard Print Dropdown menu item
                mnuPrint.Text = .GetString("psPRINT_TAB_MNU", oCulture)

            End With 'gpsRM

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitFormText", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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
        Dim lReply As Response = Response.OK
        'Dim cachePrivilege As mPassword.ePrivilege 'RJO 03/23/12

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING", DisplayCulture)
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()

            mcolZones = New clsZones(String.Empty)
            'Show the Zone combobox only if this computer connects to remote zones
            mbLocal = True
            For Each oZone As clsZone In mcolZones
                If oZone.IsRemoteZone Then
                    mbLocal = False
                    Exit For
                End If
            Next
            lblZone.Visible = Not mbLocal
            cboZone.Visible = Not mbLocal

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then

            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
            '    End If

            '    Privilege = cachePrivilege

            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION", DisplayCulture)

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)
            Call subInitFormText()
            Call subBuildGrid()
            Me.Show()
            'Handle user selection change events with common event handler routines
            Call subAddShiftDtpHandlers()
            Call subAddShiftChkHandlers()
            Call subAddMaintDtpHandlers()
            Call subAddMaintChkHandlers()
            Call subAddOptionChkHandlers()
            Call subAddTabClickHandlers()

            Progress = 70

            'subEnableControls(True)
            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            If (Not mbLocal) And (cboZone.Text = String.Empty) Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE", DisplayCulture)
            End If

            Progress = 98


            mbScreenLoaded = True
            If mbLocal Then
                cboZone.Text = cboZone.Items(0).ToString
                Call subChangeZone()
            End If

            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, Privilege, mcolZones.ActiveZone.IsRemoteZone)

        Catch ex As Exception

            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
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

    Private Sub subLoadData()
        '********************************************************************************************
        'Description:  Data Load Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        DataLoaded = False
        mbAddFoldersModified = False
        mbAddExtraModified = False

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA", DisplayCulture)

            Status = sTmp
            Progress = 50

            Call subLoadParamNames()
            DataLoaded = bLoadScheduleData()

            Progress = 98

            If DataLoaded Then
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

                'this resets edit flag
                DataLoaded = True

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csLOADFAILED", DisplayCulture)
                'load from DB failed
                MessageBox.Show(gcsRM.GetString("csLOADFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)

            End If

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub

    Private Sub subLoadParamNames()
        '********************************************************************************************
        'Description:  Set the Label property for each event for use when consructing a change log
        '              record.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oCulture As Globalization.CultureInfo = DisplayCulture
        Dim sShift As String = String.Empty

        'Shift 1
        sShift = gpsRM.GetString("psTAB1_TXT", oCulture)
        mEventsRef(1).Label = sShift & " "
        mEventsRef(2).Label = sShift & " " & gpsRM.GetString("psBREAK1_TXT", oCulture) & " "
        mEventsRef(3).Label = sShift & " " & gpsRM.GetString("psBREAK2_TXT", oCulture) & " "
        mEventsRef(4).Label = sShift & " " & gpsRM.GetString("psBREAK3_TXT", oCulture) & " "
        mEventsRef(5).Label = sShift & " " & gpsRM.GetString("psBREAK4_TXT", oCulture) & " "
        mEventsRef(6).Label = sShift & " " & gpsRM.GetString("psBREAK5_TXT", oCulture) & " "

        'Shift 2
        sShift = gpsRM.GetString("psTAB2_TXT", oCulture)
        mEventsRef(7).Label = sShift & " "
        mEventsRef(8).Label = sShift & " " & gpsRM.GetString("psBREAK1_TXT", oCulture) & " "
        mEventsRef(9).Label = sShift & " " & gpsRM.GetString("psBREAK2_TXT", oCulture) & " "
        mEventsRef(10).Label = sShift & " " & gpsRM.GetString("psBREAK3_TXT", oCulture) & " "
        mEventsRef(11).Label = sShift & " " & gpsRM.GetString("psBREAK4_TXT", oCulture) & " "
        mEventsRef(12).Label = sShift & " " & gpsRM.GetString("psBREAK5_TXT", oCulture) & " "

        'Shift 3
        sShift = gpsRM.GetString("psTAB3_TXT", oCulture)
        mEventsRef(13).Label = sShift & " "
        mEventsRef(14).Label = sShift & " " & gpsRM.GetString("psBREAK1_TXT", oCulture) & " "
        mEventsRef(15).Label = sShift & " " & gpsRM.GetString("psBREAK2_TXT", oCulture) & " "
        mEventsRef(16).Label = sShift & " " & gpsRM.GetString("psBREAK3_TXT", oCulture) & " "
        mEventsRef(17).Label = sShift & " " & gpsRM.GetString("psBREAK4_TXT", oCulture) & " "
        mEventsRef(18).Label = sShift & " " & gpsRM.GetString("psBREAK5_TXT", oCulture) & " "

        'Maint
        mMaintRef(1).Label = gpsRM.GetString("psSUNDAY_TXT", oCulture) & " "
        mMaintRef(2).Label = gpsRM.GetString("psMONDAY_TXT", oCulture) & " "
        mMaintRef(3).Label = gpsRM.GetString("psTUESDAY_TXT", oCulture) & " "
        mMaintRef(4).Label = gpsRM.GetString("psWEDNESDAY_TXT", oCulture) & " "
        mMaintRef(5).Label = gpsRM.GetString("psTHURSDAY_TXT", oCulture) & " "
        mMaintRef(6).Label = gpsRM.GetString("psFRIDAY_TXT", oCulture) & " "
        mMaintRef(7).Label = gpsRM.GetString("psSATURDAY_TXT", oCulture) & " "

    End Sub

    Private Sub subLockControls()
        '********************************************************************************************
        'Description:  Don't allow a user without edit privilege to change settings
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Dim oTab As TabPage = tabMain.SelectedTab

            Select Case oTab.Name
                Case "TabPage1", "TabPage2", "TabPage3", "TabPage4"
                    For Each oControl As Control In oTab.Controls
                        If (TypeOf oControl Is DateTimePicker) Or (TypeOf oControl Is CheckBox) Then
                            oControl.Enabled = False
                        End If
                    Next 'oControl
                Case "TabPage5"
                    For Each oControl As Control In oTab.Controls
                        If (TypeOf oControl Is TextBox) Or _
                           (TypeOf oControl Is CheckBox) Or _
                           (TypeOf oControl Is Button) Then
                            oControl.Enabled = False
                        End If
                        dgvFolders.Enabled = False
                    Next 'oControl
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subLockControls", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subLogChanges()
        '********************************************************************************************
        'Description:  Log changes to the database after a save
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath) 'Access Database
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone) 'SQL database

    End Sub

    Private Sub subPrintEvents(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, _
                               ByVal sSubTitle() As String, ByVal Shift As Integer)
        '********************************************************************************************
        'Description:  Event (Shift) Data Print Routine
        '
        'Parameters: Report, Shift number (1-3)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer
        Dim nStartIndex As Integer
        Dim sText(0) As String

        Try
            Progress = 10

            'Column Headers
            sText(0) = gpsRM.GetString("psITEM_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psSTART_TIME_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psEND_TIME_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psENABLED_TXT", DisplayCulture)
            sText(0) = Strings.Trim(sText(0))

            Select Case Shift
                Case 2
                    nStartIndex = 7
                Case 3
                    nStartIndex = 13
                Case Else
                    nStartIndex = 1
            End Select

            'Column Data
            For nEvent As Integer = nStartIndex To nStartIndex + 5
                nIndex += 1
                ReDim Preserve sText(nIndex)
                With mEvents(nEvent)
                    sText(nIndex) = .Label & vbTab & _
                                    sGetTimeString(.StartHour, .StartMin, .StartSec) & vbTab & _
                                    sGetTimeString(.EndHour, .EndMin, .EndSec) & vbTab & _
                                    .Enable.ToString
                    sText(nIndex) = Strings.Trim(sText(nIndex))
                End With
            Next 'nEvent

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 55
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subPrintLog(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, _
                            ByVal sSubTitle() As String)
        '********************************************************************************************
        'Description:  Maintenance Log Print Routine
        '
        'Parameters: Report
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sText(lstEventLog.Items.Count) As String

        Try
            Progress = 10

            'Column Header
            sText(0) = Strings.Trim(gpsRM.GetString("psEVENT_TXT", DisplayCulture))

            'Column Data
            For nEvent As Integer = 0 To lstEventLog.Items.Count - 1
                sText(nEvent + 1) = Strings.Trim(lstEventLog.Items(nEvent).ToString)
            Next 'nEvent

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 55
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subPrintMaint(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, _
                              ByVal sSubTitle() As String)
        '********************************************************************************************
        'Description:  Maintenance Schedule/Config Print Routine
        '
        'Parameters: Report
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sText(8) As String

        Try
            Progress = 10

            'Column Headers
            sText(0) = gpsRM.GetString("psDAY_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psCLEANUP_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psRBACKUP_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psFBACKUP_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psSTART_TIME_TXT", DisplayCulture)

            sText(0) = Strings.Trim(sText(0))

            'Column Data
            For nDay As Integer = 1 To 7
                With mMaint(nDay)
                    sText(nDay) = .Label & vbTab & .Cleanup.ToString & vbTab & _
                                  .RBackup.ToString & vbTab & .FBackup.ToString & vbTab & _
                                  sGetTimeString(.StartHour, .StartMin, .StartSec)
                End With

                sText(nDay) = Trim(sText(nDay))
            Next 'nDay

            sText(8) = gpsRM.GetString("psLBL_LAST_PERF_TXT", DisplayCulture) & vbTab & _
                       mHistory.LastCleanup & vbTab & mHistory.LastRBackup & vbTab & _
                       mHistory.LastFBackup

            sText(8) = Trim(sText(8))

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 55
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subPrintOptions(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, _
                                ByVal sSubTitle() As String)
        '********************************************************************************************
        'Description:  File Backup Options Print Routine
        '
        'Parameters: Report
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sStatus As String = String.Empty
        Dim sText(8) As String

        Try
            Progress = 10

            'Column Headers
            sText(0) = gpsRM.GetString("psITEM_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psVALUE_TXT", DisplayCulture)
            sText(0) = Strings.Trim(sText(0))

            'Column Data
            For nRow As Integer = 1 To 8
                Select Case nRow
                    Case 1
                        sText(nRow) = gpsRM.GetString("psBACK_PATH_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupPath
                    Case 2
                        sText(nRow) = gpsRM.GetString("psCHK_NEWFOLD_TXT", DisplayCulture) & vbTab & _
                                      mOptions.NewFolder.ToString
                    Case 3
                        sText(nRow) = gpsRM.GetString("psCHK_DBBACK_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupDB.ToString
                    Case 4
                        sText(nRow) = gpsRM.GetString("psCHK_RMBACK_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupRM.ToString
                    Case 5
                        sText(nRow) = gpsRM.GetString("psCHK_RSBACK_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupRS.ToString
                    Case 6
                        sText(nRow) = gpsRM.GetString("psCHK_RTBACK_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupRT.ToString
                    Case 7
                        sText(nRow) = gpsRM.GetString("psCHK_ADDBACK_TXT", DisplayCulture) & vbTab & _
                                      mOptions.BackupAdd.ToString
                    Case 8
                        sText(nRow) = gpsRM.GetString("psCHK_EXT_COP_TXT", DisplayCulture) & vbTab & _
                                      mOptions.ExtraCopies.ToString
                End Select

                sText(nRow) = Trim(sText(nRow))
            Next 'nRow

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 50

            If mOptions.BackupAdd Then
                ReDim sText(dgvFolders.Rows.Count)

                mPrintHtml.subAddHeading(sStatus, 3, gpsRM.GetString("psADD_FOLDERS_TXT", DisplayCulture))

                'Column Headers
                sText(0) = gpsRM.GetString("psFOLDER_NAME", DisplayCulture) & vbTab & _
                           gpsRM.GetString("psFOLDER_LOCATION", DisplayCulture) & vbTab & _
                           gpsRM.GetString("psSUBFOLDERS", DisplayCulture)

                sText(0) = Strings.Trim(sText(0))

                'Column Data
                For nRow As Integer = 1 To dgvFolders.Rows.Count
                    With dgvFolders.Rows(nRow - 1)
                        sText(nRow) = .Cells("BackupName").Value.ToString & vbTab & _
                                      .Cells("FolderPath").Value.ToString & vbTab & _
                                      .Cells("Subfolders").Value.ToString
                    End With

                    sText(nRow) = Trim(sText(nRow))
                Next 'nRow

                Progress = 60
                mPrintHtml.subAddObject(sText, Status)
                Progress = 70

            End If

            If mOptions.ExtraCopies Then
                ReDim sText(dgvExtraCopies.Rows.Count)

                mPrintHtml.subAddHeading(sStatus, 3, gpsRM.GetString("psCHK_EXT_COP_TXT", DisplayCulture))

                'Column Headers
                sText(0) = gpsRM.GetString("psFROM", DisplayCulture) & vbTab & _
                           gpsRM.GetString("psTO", DisplayCulture) & vbTab & _
                           gpsRM.GetString("psSUBFOLDERS", DisplayCulture)

                sText(0) = Strings.Trim(sText(0))

                'Column Data
                For nRow As Integer = 1 To dgvExtraCopies.Rows.Count
                    With dgvExtraCopies.Rows(nRow - 1)
                        sText(nRow) = .Cells("ExtraFrom").Value.ToString & vbTab & _
                                      .Cells("ExtraTo").Value.ToString & vbTab & _
                                      .Cells("ExtraSubFolders").Value.ToString
                    End With
                    sText(nRow) = Trim(sText(nRow))
                Next 'nRow
                Progress = 80
                mPrintHtml.subAddObject(sText, Status)
            End If

            Progress = 90
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
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
            End If
        Next

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
            Status = gcsRM.GetString("csSAVINGDATA", DisplayCulture)

            ' do save
            If bSaveToGUI() Then

                Call subLogChanges()
                Call subLoadData()
                ' save done
                EditsMade = False

                subShowNewPage(True)
                Status = gcsRM.GetString("csSAVE_DONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csSAVEFAILED", DisplayCulture)
                'save to DB failed
                MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If 'bSaveToGUI()

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub

    Private Sub subSchedManUpdateReq()
        '********************************************************************************************
        'Description: The schedule has changed. Send a message to SchedMan.exe to tell it to read the
        '             new schedule.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Call mWorksComm.SendFRWMMessage("update,0,0,0,0,0", "SchedMan")
        Dim sMessage(0) As String
        sMessage(0) = "update"
        oIPC.WriteControlMsg(gs_COM_ID_SCHEDMAN, sMessage)
    End Sub

    Private Sub subShowAddFolderPanel(ByRef Name As String, ByRef Path As String, ByVal Subfolders As Boolean, ByRef oPnl As Panel)
        '********************************************************************************************
        'Description: Lock down TabPage5, then show the Add/Edit Folder panel.
        '
        'Parameters: Name, Path, (include) Subfolders
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Lock down the form
        Call subLockControls()

        'Enable the Panel controls
        For Each oControl As Control In oPnl.Controls
            oControl.Enabled = True
        Next

        'Populate the fields
        Select Case oPnl.Name
            Case "pnlAddFolders"
                txtAddFolderName.Text = Name
                txtAddFolderPath.Text = Path
                chkSubfolders.Checked = Subfolders
            Case "pnlAddExtra"
                txtExtraFrom.Text = Name
                txtExtraTo.Text = Path
                chkExtraSubfolders.Checked = Subfolders
        End Select

        oPnl.Show()

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
        
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                        gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
                                
        End Try

    End Sub

    Private Sub subShowNewPage(ByVal BlockEvents As Boolean)
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
        mbEventBlocker = BlockEvents
        Dim bTmp As Boolean = False

        Select Case msCurrentTabName
            Case "TabPage1"   '[Shift 1 Tab]
                Call subShowTabPage1()
            Case "TabPage2"   '[Shift 2 Tab]
                Call subShowTabPage2()
            Case "TabPage3"   '[Shift 3 Tab]
                Call subShowTabPage3()
            Case "TabPage4"   '[Cleanup/Backup Tab]
                Call subShowTabPage4()
            Case "TabPage5"   '[File Backup Options Tab]
                Call subShowTabPage5()
            Case "TabPage6"   '[Maint Log Display Tab]
                Call subShowTabPage6()

        End Select

        subEnableControls(True)
        mbEventBlocker = False

    End Sub

    Private Function GetCheckBox(ByRef Container As TabPage, ByVal Index As Integer) As CheckBox
        '********************************************************************************************
        'Description:  Returns the CheckBox control that matches the Index
        '
        'Parameters: Container (Current TabPage), Index (Number in control's Tag field)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sIndex As String = Index.ToString

        For Each oControl As Control In Container.Controls
            If TypeOf oControl Is CheckBox Then
                If oControl.Tag.ToString = sIndex Then
                    Dim oCheckBox As CheckBox = DirectCast(oControl, CheckBox)

                    Return oCheckBox
                End If
            End If
        Next

        Return Nothing

    End Function

    Private Function GetDTPicker(ByRef Container As TabPage, ByVal Type As String, ByVal Index As Integer) As DateTimePicker
        '********************************************************************************************
        'Description:  Returns the DateTimePicker control that matches the Type and Index
        '
        'Parameters: Container (Current TabPage), Type ("Start" or "End"), 
        '            Index (Number in control's Tag field)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sIndex As String = Index.ToString

        For Each oControl As Control In Container.Controls
            If TypeOf oControl Is DateTimePicker Then
                If oControl.Tag.ToString = sIndex Then
                    If Strings.Right(oControl.Name, Strings.Len(Type)).ToLower = Type.ToLower Then
                        Dim oDTPicker As DateTimePicker = DirectCast(oControl, DateTimePicker)

                        Return oDTPicker
                    End If
                End If
            End If
        Next

        Return Nothing

    End Function

    Private Sub subShowTabPage1()
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
        Dim sHour As String = String.Empty
        Dim sMin As String = String.Empty
        Dim sSec As String = String.Empty
        Dim dtEvent As DateTime
        Dim oDTP As DateTimePicker
        Dim oCB As CheckBox

        TabPage1.Show()

        Try
            For nIndex As Integer = 1 To 6
                'Start Time
                sHour = mEvents(nIndex).StartHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).StartMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).StartSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage1, "Start", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'End Time
                sHour = mEvents(nIndex).EndHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).EndMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).EndSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage1, "End", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'Enable
                oCB = GetCheckBox(TabPage1, nIndex)

                If Not oCB Is Nothing Then
                    oCB.Checked = mEvents(nIndex).Enable
                End If

            Next 'nIndex

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_Shift1.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage1", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage2()
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
        Dim sHour As String = String.Empty
        Dim sMin As String = String.Empty
        Dim sSec As String = String.Empty
        Dim dtEvent As DateTime
        Dim oDTP As DateTimePicker
        Dim oCB As CheckBox

        TabPage2.Show()

        Try
            For nIndex As Integer = 7 To 12
                'Start Time
                sHour = mEvents(nIndex).StartHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).StartMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).StartSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage2, "Start", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'End Time
                sHour = mEvents(nIndex).EndHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).EndMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).EndSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage2, "End", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'Enable
                oCB = GetCheckBox(TabPage2, nIndex)

                If Not oCB Is Nothing Then
                    oCB.Checked = mEvents(nIndex).Enable
                End If

            Next 'nIndex

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_Shift2.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage2", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage3()
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
        Dim sHour As String = String.Empty
        Dim sMin As String = String.Empty
        Dim sSec As String = String.Empty
        Dim dtEvent As DateTime
        Dim oDTP As DateTimePicker
        Dim oCB As CheckBox

        TabPage3.Show()

        Try
            For nIndex As Integer = 13 To 18
                'Start Time
                sHour = mEvents(nIndex).StartHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).StartMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).StartSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage3, "Start", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'End Time
                sHour = mEvents(nIndex).EndHour.ToString.PadLeft(2, "0"c)
                sMin = mEvents(nIndex).EndMin.ToString.PadLeft(2, "0"c)
                sSec = mEvents(nIndex).EndSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage3, "End", nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                'Enable
                oCB = GetCheckBox(TabPage3, nIndex)

                If Not oCB Is Nothing Then
                    oCB.Checked = mEvents(nIndex).Enable
                End If

            Next 'nIndex

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_Shift3.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage3", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage4()
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
        Dim sHour As String = String.Empty
        Dim sMin As String = String.Empty
        Dim sSec As String = String.Empty
        Dim dtEvent As DateTime
        Dim oDTP As DateTimePicker
        Dim oCB As CheckBox

        TabPage4.Show()

        Try
            For nIndex As Integer = 1 To 7
                'Start Time
                sHour = mMaint(nIndex).StartHour.ToString.PadLeft(2, "0"c)
                sMin = mMaint(nIndex).StartMin.ToString.PadLeft(2, "0"c)
                sSec = mMaint(nIndex).StartSec.ToString.PadLeft(2, "0"c)

                dtEvent = CType("01/01/2000 " & sHour & ":" & sMin & ":" & sSec, DateTime)

                oDTP = GetDTPicker(TabPage4, "Start" & nIndex.ToString, nIndex)

                If Not oDTP Is Nothing Then
                    oDTP.Value = dtEvent
                End If

                'Cleanup Enable
                oCB = GetCheckBox(TabPage4, (nIndex * 10) + 1)

                With mMaint(nIndex)
                    If Not oCB Is Nothing Then
                        oCB.Checked = .Cleanup
                    End If

                    'Robot Backup Enable
                    oCB = GetCheckBox(TabPage4, (nIndex * 10) + 2)

                    If Not oCB Is Nothing Then
                        oCB.Checked = .RBackup
                    End If

                    'File Backup Enable
                    oCB = GetCheckBox(TabPage4, (nIndex * 10) + 3)

                    If Not oCB Is Nothing Then
                        oCB.Checked = .FBackup
                    End If

                    oDTP.Enabled = .Cleanup Or .RBackup Or .FBackup
                End With 'mMaint(nIndex)
            Next 'nIndex

            'show history
            lblLastCleanup.Text = mHistory.LastCleanup
            lblLastRBackup.Text = mHistory.LastRBackup
            lblLastFBackup.Text = mHistory.LastFBackup

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_Maint.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage4", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage5()
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

        TabPage5.Show()

        Try

            With mOptions
                txtBackupPath.Text = .BackupPath
                chkNewFolder.Checked = .NewFolder
                chkBackupDB.Checked = .BackupDB
                chkBackupRM.Checked = .BackupRM
                chkBackupRS.Checked = .BackupRS
                chkBackupRT.Checked = .BackupRT
                chkBackupImage.Checked = .BackupImage
                chkBackupAdd.Checked = .BackupAdd
                chkExtraCopies.Checked = .ExtraCopies
            End With

            'If there are any data rows, don't select the first row automatically
            If dgvFolders.Rows.Count > 0 Then dgvFolders.Rows(0).Selected = False

            mbPathDNE_Warn = False

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_Options.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage5", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage6()
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

        TabPage6.Show()

        Try
            Dim sPath As String = String.Empty

            If mPWCommon.GetDefaultFilePath(sPath, eDir.PAINTworks, String.Empty, "PWMaint.log") Then
                Dim objStreamReader As New IO.StreamReader(sPath)
                Dim sLine As String

                lstEventLog.Items.Clear()
                sLine = objStreamReader.ReadLine

                Do While Not sLine Is Nothing
                    lstEventLog.Items.Add(sLine)
                    sLine = objStreamReader.ReadLine
                Loop

                objStreamReader.Close()

            Else
                MessageBox.Show(gpsRM.GetString("psEVENTLOG_NOT_FOUND", DisplayCulture) & " [" & sPath & "]", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            msSCREEN_DUMP_NAME = "Utilities_Scheduler_EventLog.jpg"

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage6", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG", DisplayCulture), _
                                        msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then
            subLoadData()
        End If

    End Sub

    Private Sub subUnlockControls()
        '********************************************************************************************
        'Description:  The current user has edit privilege, unlock the controls to allow changes to 
        '              settings.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Dim oTab As TabPage = tabMain.SelectedTab

            'Enable all checkboxes
            For Each oControl As Control In oTab.Controls
                If TypeOf oControl Is CheckBox Then
                    oControl.Enabled = True
                End If
            Next 'oControl  

            Select Case oTab.Name
                Case "TabPage1", "TabPage2", "TabPage3"
                    'Set enables for DTPs based on CheckBox values
                    For Each oControl As Control In oTab.Controls
                        If TypeOf oControl Is CheckBox Then
                            Dim oCB As CheckBox = DirectCast(oControl, CheckBox)
                            Dim oDTP As DateTimePicker = GetDTPicker(oTab, "Start", CType(oCB.Tag, Integer))

                            oDTP.Enabled = oCB.Checked
                            oDTP = GetDTPicker(oTab, "End", CType(oCB.Tag, Integer))
                            oDTP.Enabled = oCB.Checked
                        End If
                    Next 'oControl

                Case "TabPage4"
                    'Enable DTPs for a given day if any checkbox for that day is checked
                    For Each oControl As Control In oTab.Controls
                        If TypeOf oControl Is DateTimePicker Then
                            Dim nDay As Integer = CType(oControl.Tag, Integer)

                            With mMaint(nDay)
                                oControl.Enabled = .Cleanup Or .FBackup Or .RBackup
                            End With
                        End If
                    Next 'oControl

                Case "TabPage5"
                    Dim bEnableAdd As Boolean = False
                    Dim bEnableExt As Boolean = False
                    'Enable textbox
                    txtBackupPath.Enabled = True
                    'Enable browse button
                    btnBrowse.Enabled = True
                    'Enable dgv and associated buttons if additional folders checkbox is checked
                    For Each oControl As Control In oTab.Controls
                        If TypeOf oControl Is CheckBox Then
                            If oControl.Name = "chkBackupAdd" Then
                                Dim oCB As CheckBox = DirectCast(oControl, CheckBox)

                                bEnableAdd = oCB.Checked
                            End If
                            If oControl.Name = "chkExtraCopies" Then
                                Dim oCB As CheckBox = DirectCast(oControl, CheckBox)

                                bEnableExt = oCB.Checked
                            End If
                        End If
                    Next 'oControl

                    dgvFolders.Enabled = bEnableAdd
                    btnAdd.Enabled = bEnableAdd
                    btnRemove.Enabled = bEnableAdd And dgvFolders.Rows.Count > 0
                    btnEdit.Enabled = bEnableAdd And dgvFolders.Rows.Count > 0

                    dgvExtraCopies.Enabled = bEnableExt
                    btnAddExtra.Enabled = bEnableExt
                    btnRemoveExtra.Enabled = bEnableExt And dgvExtraCopies.Rows.Count > 0
                    btnEditExtra.Enabled = bEnableExt And dgvExtraCopies.Rows.Count > 0

            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUnlockControls", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Overloads Sub subUpdateChangeLog(ByRef NewValue As String, _
                                        ByRef OldValue As String, ByRef ParamName As String, _
                                        ByRef oZone As clsZone, ByRef WhatChanged As String, _
                                        ByRef DeviceName As String)
        '********************************************************************************************
        'Description:  Build up the change text for a value change
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = WhatChanged & " "

        sTmp = sTmp & gcsRM.GetString("csCHANGED_FROM", DisplayCulture) & " " & OldValue & " " & _
                        gcsRM.GetString("csTO", DisplayCulture) & " " & NewValue

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, ParamName, sTmp, oZone.Name)

    End Sub

    Private Overloads Sub subUpdateChangeLog(ByVal ZoneNumber As Integer)
        '********************************************************************************************
        'Description:  build up the change text for Add folders changed event
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAddFolderChange As String = gpsRM.GetString("psADD_FOLDER_CHG_MSG", DisplayCulture)

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, ZoneNumber, _
                                                " ", " ", sAddFolderChange, mcolZones.ActiveZone.Name)

    End Sub

#End Region

#Region " Form Events "

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
        '    04/20/12   MSW     frmMain_KeyDown-Add excape key handler 
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_UTILITIES_SCHEDULER, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)

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
        '********************************************************************************************
        'TODO - find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (PnlMain.Left * 2)

        If nHeight < 100 Then nHeight = 100
        If nWidth < 100 Then nWidth = 100

        PnlMain.Height = nHeight
        PnlMain.Width = nWidth

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
            Dim sCultureArg As String = "/culture="

            'If a culture string has been passed in, set the current culture (display language)
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    Culture = s.Remove(0, sCultureArg.Length)
                    Exit For
                End If
            Next

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description:  I feel like a new form!
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
                                      String.Empty)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

#End Region

#Region " Control Events "

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click, btnAddExtra.Click
        '********************************************************************************************
        'Description:  User wants to Add a folder to be backed up.
        '
        'Parameters: sender, eventargs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBtn As Button = DirectCast(sender, Button)
        Dim oDGV As DataGridView = Nothing
        Dim oPnl As Panel = Nothing
        Select Case oBtn.Name
            Case "btnAdd"
                oDGV = dgvFolders
                oPnl = pnlAddFolders
            Case "btnAddExtra"
                oDGV = dgvExtraCopies
                oPnl = pnlAddExtra
        End Select
        Dim nRow As Integer = GetSelectedDGVRow(oDGV)


        oPnl.Tag = "Add"
        Call subShowAddFolderPanel(String.Empty, String.Empty, False, oPnl)

    End Sub

    Private Sub btnAddFolderAccept_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddFolderAccept.Click, btnAddExtraAccept.Click
        '********************************************************************************************
        'Description:  User has accepted an edit to the Additional Folders Table
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim oBtn As Button = DirectCast(sender, Button)
            Dim oDGV As DataGridView = Nothing
            Dim oPnl As Panel = Nothing
            Dim oTxt1 As TextBox = Nothing
            Dim oLbl1 As Label = Nothing
            Dim oTxt2 As TextBox = Nothing
            Dim oLbl2 As Label = Nothing
            Dim bExtra As Boolean = False
            Dim oChk As CheckBox = Nothing
            Dim sCol1 As String = Nothing
            Dim sCol2 As String = Nothing
            Dim sCol3 As String = Nothing
            Select Case oBtn.Name
                Case "btnAddFolderAccept"
                    oDGV = dgvFolders
                    oPnl = pnlAddFolders
                    oTxt1 = txtAddFolderName
                    oTxt2 = txtAddFolderPath
                    oLbl1 = lblAddFolderName
                    oLbl2 = lblAddFolderPath
                    bExtra = False
                    oChk = chkSubfolders
                    sCol1 = "BackupName"
                    sCol2 = "FolderPath"
                    sCol3 = "Subfolders"
                Case "btnAddExtraAccept"
                    oDGV = dgvExtraCopies
                    oPnl = pnlAddExtra
                    oTxt1 = txtExtraFrom
                    oTxt2 = txtExtraTo
                    oLbl1 = lblExtraFrom
                    oLbl2 = lblExtraTo
                    bExtra = True
                    oChk = chkExtraSubfolders
                    sCol1 = "ExtraFrom"
                    sCol2 = "ExtraTo"
                    sCol3 = "ExtraSubfolders"
            End Select

            'Check for zero length strings
            If oTxt1.Text = String.Empty Then
                MessageBox.Show(gpsRM.GetString("psCANNOT_BE_BLANK_MSG", DisplayCulture) & _
                                                "[" & oLbl1.Text & "]", oPnl.Text, _
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If oTxt2.Text = String.Empty Then
                MessageBox.Show(gpsRM.GetString("psCANNOT_BE_BLANK_MSG", DisplayCulture) & _
                                                "[" & oLbl2.Text & "]", oPnl.Text, _
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            'Check if the folder path exists
            If bExtra Then
                If Not IO.Directory.Exists(oTxt1.Text) Then
                    If MessageBox.Show(gpsRM.GetString("psPATH_DNE_MSG", DisplayCulture) & _
                                       "[" & oLbl1.Text & "]", oPnl.Text, _
                                       MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = Response.Cancel Then
                        Exit Sub
                    End If
                End If

            End If

            If Not IO.Directory.Exists(oTxt2.Text) Then
                If MessageBox.Show(gpsRM.GetString("psPATH_DNE_MSG", DisplayCulture) & _
                                   "[" & oLbl2.Text & "]", oPnl.Text, _
                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = Response.Cancel Then
                    Exit Sub
                End If
            End If

            'update the grid
            Dim nRow As Integer

            oDGV.Enabled = True

            Select Case oPnl.Tag.ToString
                Case "Add"
                    nRow = oDGV.Rows.Count
                    oDGV.Rows.Add()

                Case "Edit"
                    nRow = GetSelectedDGVRow(oDGV)
            End Select

            With oDGV.Rows(nRow)
                .Cells(sCol1).Value = oTxt1.Text
                .Cells(sCol2).Value = oTxt2.Text
                .Cells(sCol3).Value = oChk.Checked
            End With
            If bExtra Then
                mbAddExtraModified = True
            Else
                mbAddFoldersModified = True
            End If
            EditsMade = True

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnAddFolderAccept_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        'Just hide both panels so it's outside the error trap.
        pnlAddFolders.Hide()
        pnlAddExtra.Hide()
        tabMain.Enabled = True
        Call subUnlockControls()

    End Sub

    Private Sub btnAddFolderBrowse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddFolderBrowse.Click, _
    btnExtraFromBrowse.Click, btnExtraToBrowse.Click
        '********************************************************************************************
        'Description:  Allow user to browse for Additional Folder to Backup Location
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/22/11  RJO     Wrong button name in case statement.
        '********************************************************************************************
        Dim oBtn As Button = DirectCast(sender, Button)
        Dim oTxt1 As TextBox = Nothing

        Select Case oBtn.Name
            Case "btnAddFolderBrowse" '"btnAddFolderAccept" 'RJO 11/22/11
                oTxt1 = txtAddFolderPath
            Case "btnExtraFromBrowse"
                oTxt1 = txtExtraFrom
            Case "btnExtraToBrowse"
                oTxt1 = txtExtraTo
        End Select

        FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer

        If FolderBrowserDialog1.ShowDialog() = Response.OK Then
            oTxt1.Text = FolderBrowserDialog1.SelectedPath
        End If

    End Sub

    Private Sub btnAddFolderCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddFolderCancel.Click, btnAddExtraCancel.Click
        '********************************************************************************************
        'Description:  User has cancelled an edit to the Additional Folders Table
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        pnlAddFolders.Hide()
        pnlAddExtra.Hide()
        tabMain.Enabled = True
        Call subUnlockControls()

    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        '********************************************************************************************
        'Description:  Allow user to browse for Backup Folder Location
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer
        If FolderBrowserDialog1.ShowDialog() = Response.OK Then
            txtBackupPath.Text = FolderBrowserDialog1.SelectedPath
        End If

    End Sub

    Private Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click, btnEditExtra.Click
        '********************************************************************************************
        'Description:  User wants to Edit a folder to be backed up.
        '
        'Parameters: sender, eventargs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBtn As Button = DirectCast(sender, Button)
        Dim oDGV As DataGridView = Nothing
        Dim oPnl As Panel = Nothing
        Dim sCol1 As String = Nothing
        Dim sCol2 As String = Nothing
        Dim sCol3 As String = Nothing
        Select Case oBtn.Name
            Case "btnEdit"
                oDGV = dgvFolders
                oPnl = pnlAddFolders
                sCol1 = "BackupName"
                sCol2 = "FolderPath"
                sCol3 = "Subfolders"
            Case "btnEditExtra"
                oDGV = dgvExtraCopies
                oPnl = pnlAddExtra
                sCol1 = "ExtraFrom"
                sCol2 = "ExtraTo"
                sCol3 = "ExtraSubfolders"
        End Select
        Dim nRow As Integer = GetSelectedDGVRow(oDGV)

        If nRow < 0 Then
            'Tell user to select a row to edit
            MessageBox.Show(gpsRM.GetString("psSEL_FOLDER_EDIT_MSG", DisplayCulture), _
                            tabMain.SelectedTab.Text, MessageBoxButtons.OK, _
                            MessageBoxIcon.Exclamation)
        Else
            oPnl.Tag = "Edit"
            With oDGV.Rows(nRow)
                Call subShowAddFolderPanel(.Cells(sCol1).Value.ToString, _
                                           .Cells(sCol2).Value.ToString, _
                                           CType(.Cells(sCol3).Value, Boolean), oPnl)
            End With
        End If

    End Sub

    Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click, btnRemoveExtra.Click
        '********************************************************************************************
        'Description:  User wants to Remove a folder from the table of folders to be backed up.
        '
        'Parameters: sender, eventargs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBtn As Button = DirectCast(sender, Button)
        Dim oDGV As DataGridView = Nothing
        Dim oPnl As Panel = Nothing
        Dim sCol1 As String = Nothing
        Dim sCol2 As String = Nothing
        Select Case oBtn.Name
            Case "btnRemove"
                oDGV = dgvFolders
                oPnl = pnlAddFolders
            Case "btnRemoveExtra"
                oDGV = dgvExtraCopies
                oPnl = pnlAddExtra
        End Select
        Dim nRow As Integer = GetSelectedDGVRow(oDGV)

        If nRow < 0 Then
            'Tell user to select a row to remove
            MessageBox.Show(gpsRM.GetString("psSEL_FOLDER_REM_MSG", DisplayCulture), _
                            tabMain.SelectedTab.Text, MessageBoxButtons.OK, _
                            MessageBoxIcon.Exclamation)
        Else
            'double check
            If MessageBox.Show(gpsRM.GetString("psFOLDER_REM_VERIFY_MSG", DisplayCulture), _
                               tabMain.SelectedTab.Text, MessageBoxButtons.OKCancel, _
                               MessageBoxIcon.Question) = Response.OK Then

                oDGV.Rows.RemoveAt(nRow)
                Select Case oBtn.Name
                    Case "btnRemove"
                        mbAddFoldersModified = True
                    Case "btnRemoveExtra"
                        mbAddExtraModified = True
                End Select
                Call subUnlockControls()

            End If
        End If

    End Sub

    Private Sub cboZone_SelectionChangeCommitted1(ByVal sender As Object, ByVal e As System.EventArgs) _
                                        Handles cboZone.SelectionChangeCommitted
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
            Application.DoEvents()
            subChangeZone()
        End If

    End Sub

    Private Sub chkMaint_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  A Maintenance selection was changed by the user. Update the corresponding 
        '              enable in the mMaint array.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then

            Try
                Dim oCHK As CheckBox = DirectCast(sender, CheckBox)
                Dim nDay As Integer = CType(oCHK.Tag, Integer) \ 10
                Dim nType As Integer = CType(oCHK.Tag, Integer) Mod 10
                Dim bEnabled As Boolean

                Select Case nType
                    Case 1
                        mMaint(nDay).Cleanup = oCHK.Checked
                    Case 2
                        mMaint(nDay).RBackup = oCHK.Checked
                    Case 3
                        mMaint(nDay).FBackup = oCHK.Checked
                End Select

                'Set the DateTimePicker for nDay Enabled State 
                bEnabled = mMaint(nDay).Cleanup Or mMaint(nDay).RBackup Or mMaint(nDay).FBackup

                For Each oControl As Control In TabPage4.Controls
                    If TypeOf oControl Is DateTimePicker Then
                        Dim oDTP As DateTimePicker = DirectCast(oControl, DateTimePicker)

                        If CType(oDTP.Tag, Integer) = nDay Then
                            oDTP.Enabled = bEnabled
                            Exit For
                        End If

                    End If
                Next 'oControl

                EditsMade = True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: chkMaint_CheckedChanged", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub chkOption_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Backup option selection was changed by the user. Update the corresponding 
        '              enable in the mOptions.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then

            Try
                Dim oCHK As CheckBox = DirectCast(sender, CheckBox)

                With mOptions
                    Select Case oCHK.Name
                        Case "chkNewFolder"
                            .NewFolder = oCHK.Checked
                        Case "chkBackupDB"
                            .BackupDB = oCHK.Checked
                        Case "chkBackupRM"
                            .BackupRM = oCHK.Checked
                        Case "chkBackupRS"
                            .BackupRS = oCHK.Checked
                        Case "chkBackupRT"
                            .BackupRT = oCHK.Checked
                        Case "chkBackupImage"
                            .BackupImage = oCHK.Checked
                        Case "chkBackupAdd"
                            .BackupAdd = oCHK.Checked
                        Case "chkExtraCopies"
                            .ExtraCopies = oCHK.Checked

                            'Set enables for Additional Folders associated controls
                            btnAdd.Enabled = .BackupAdd
                            btnRemove.Enabled = .BackupAdd And dgvFolders.Rows.Count > 0
                            btnEdit.Enabled = .BackupAdd And dgvFolders.Rows.Count > 0
                            dgvFolders.Enabled = .BackupAdd
                    End Select
                End With 'mOptions

                EditsMade = True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: chkOption_CheckedChanged", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub chkShift_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Shift/Break enable was changed by the user. Update the corresponding enable
        '              in the mEvents array.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then

            Try
                Dim oCHK As CheckBox = DirectCast(sender, CheckBox)
                Dim nIndex As Integer = CType(oCHK.Tag, Integer)
                Dim oTab As TabPage = tabMain.SelectedTab

                mEvents(nIndex).Enable = oCHK.Checked

                'Modify associated controls enable properties accordingly
                If nIndex = 0 Then  'JZ 12072016
                    'The shift has been Enabled/Disabled
                    If Not oCHK.Checked Then
                        'Breaks are invalid - uncheck all
                        mbEventBlocker = True
                        For Each oControl As Control In oTab.Controls
                            If TypeOf oControl Is CheckBox Then
                                Dim oCheckBox As CheckBox = DirectCast(oControl, CheckBox)
                                Dim nCbIndex As Integer = CType(oCheckBox.Tag, Integer)

                                oCheckBox.Checked = False
                                mEvents(nCbIndex).Enable = False
                            End If
                        Next 'oControl
                        mbEventBlocker = False
                    End If

                    'Set DateTimePicker Control enables
                    For Each oControl As Control In oTab.Controls
                        If TypeOf oControl Is DateTimePicker Then
                            Dim oDateTimePicker As DateTimePicker = DirectCast(oControl, DateTimePicker)
                            Dim nDtpIndex As Integer = CType(oDateTimePicker.Tag, Integer)

                            oDateTimePicker.Enabled = mEvents(nDtpIndex).Enable
                        End If
                    Next 'oControl

                Else
                    'A Break has been Enabled/Disabled
                    Dim oDTP As DateTimePicker = GetDTPicker(oTab, "Start", nIndex)

                    oDTP.Enabled = mEvents(nIndex).Enable
                    oDTP = GetDTPicker(oTab, "End", nIndex)
                    oDTP.Enabled = mEvents(nIndex).Enable
                End If

                EditsMade = True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: chkShift_CheckedChanged", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub dtpMaint_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Maintenance start time was changed by the user. Update the corresponding Time 
        '              vaules in the mMaint array.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then

            Try
                Dim oDTP As DateTimePicker = DirectCast(sender, DateTimePicker)
                Dim nDay As Integer = CType(oDTP.Tag, Integer)

                With mMaint(nDay)
                    .StartHour = oDTP.Value.Hour
                    .StartMin = oDTP.Value.Minute
                    .StartSec = oDTP.Value.Second
                End With 'mMaint(nDay)

                EditsMade = True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: dtpMaint_ValueChanged", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub dtpShift_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Shift/Break time was changed by the user. Update the corresponding Time vaules
        '              in the mEvents array.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then

            Try
                Dim oDTP As DateTimePicker = DirectCast(sender, DateTimePicker)
                Dim nIndex As Integer = CType(oDTP.Tag, Integer)

                With mEvents(nIndex)
                    If Strings.Right(oDTP.Name, 3) = "End" Then
                        .EndHour = oDTP.Value.Hour
                        .EndMin = oDTP.Value.Minute
                        .EndSec = oDTP.Value.Second
                    Else
                        .StartHour = oDTP.Value.Hour
                        .StartMin = oDTP.Value.Minute
                        .StartSec = oDTP.Value.Second
                    End If
                End With 'mEvents(nIndex)

                EditsMade = True

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: dtpShift_ValueChanged", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub tabMain_Selected(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlEventArgs) Handles tabMain.Selected
        '********************************************************************************************
        'Description:  a tab page was just selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        msCurrentTabName = e.TabPage.Name

        Select Case msCurrentTabName
            Case "TabPage1"
                'Add any special init code here
            Case "TabPage2"
                'Add any special init code here
            Case "TabPage3"
                'Add any special init code here
            Case "TabPage4"
                'Add any special init code here
            Case "TabPage5"
                'Add any special init code here
        End Select

        Call subShowNewPage(True)

    End Sub

    Private Sub tabMain_Selecting(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  A tab page is being selected. Make sure there are no unsaved edits on the
        '              current tab before switching to the selected tab.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If EditsMade And bCheckEditsMade() Then
            e.Cancel = (bAskForSave() = False)
        ElseIf pnlAddFolders.Visible Then
            e.Cancel = True
        Else
            EditsMade = False
        End If

    End Sub

    Private Sub tabPage_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Common event handler to show the Login form if the user clicks on a TabPage
        '              but doesn't have the Edit privilege.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then
            If Privilege = ePrivilege.Edit Then
                Exit Sub
            Else
                Privilege = ePrivilege.Edit
            End If
        End If

    End Sub

    Private Sub TabPage5_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabPage5.MouseEnter
        '********************************************************************************************
        'Description:  The mousepointer has entered TabPage5. If the user has edit privilege, verify
        '              the Backup Folder Path.
        '
        'Parameters: sender, EventArgs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If (Not mbPathDNE_Warn) And txtBackupPath.Enabled Then
            'Alert the user if the backup folder path does not exist (one time only)
            If Not IO.Directory.Exists(txtBackupPath.Text) Then
                MessageBox.Show(gpsRM.GetString("psPATH_DNE_MSG", DisplayCulture) & _
                                "[" & txtBackupPath.Text & "]", TabPage5.Text, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtBackupPath.Select()
            End If
            mbPathDNE_Warn = True
        End If

    End Sub

    Private Sub txtBackupPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBackupPath.TextChanged
        '********************************************************************************************
        'Description:  User changed the Backup Folder Path.
        '
        'Parameters: sender, eventargs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then
            mOptions.BackupPath = txtBackupPath.Text
            EditsMade = True
        End If

    End Sub

#End Region

#Region " Temp Stuff for Old Password Object "

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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
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

#Region " Main Menu Button Events "

    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim cachePrivilege As ePrivilege

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
                If mbRemoteZone = False Then
                    mnuRemote.Enabled = True
                End If
            Else
                mnuRemote.Enabled = False
            End If
            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

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

    Private Sub mnuLast24_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs) Handles mnuLast24.Click
        '********************************************************************************************
        'Description:  show changes for last 24 hours
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

        subInitializeForm()

    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description:  show page setup dialog 
        '
        'Parameters: none
        'Returns:    Print settings to use in printing
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False, False) Then
                mPrintHtml.subShowPageSetup()
            End If
        End If

    End Sub

    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                        Handles mnuPrint.Click
        '********************************************************************************************
        'Description:  call print routine for current tab only
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            Call bPrintdoc(True, False)
        End If

    End Sub

    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description:  print the current tab data to a file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False, False) Then
                mPrintHtml.subSaveAs()
            End If
        End If

    End Sub

    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description:  offer options for printout table setup.
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPrintHtml.subShowOptions()

    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description:  Show the print preview for current tab only
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False, False) Then
                mPrintHtml.subShowPrintPreview()
            End If
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

            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If 'SetRemoteServer()

        mbEventBlocker = False

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

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True, Not tabMain.SelectedTab Is TabPage6)

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

#End Region

#Region " Log In, Log Out Events "

    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                  Handles mnuLogin.Click
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

    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuLogOut.Click
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

#End Region

#Region " Debug Stuff "

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'For debug
    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        'For debug
    End Sub

#End Region

End Class