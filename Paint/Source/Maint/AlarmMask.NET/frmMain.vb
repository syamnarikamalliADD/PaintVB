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
' Description: Alarm Mask Utility
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    08/30/12   RJO     Assemble a standard version compatible with Paintworks.NET    4.01.00.00
'    12/13/12   MSW     subProcessCommandLine - handle spaces in the path name        4.01.04.00
'    02/26/13   RJO     Changed sAlarmNumber to Integer in btnAdd_Click event code    4.01.04.01
'                       so it will format properly in the Status label.
'    07/09/13   MSW     Update and standardize logos, PLCComm.Dll                     4.01.05.00
'**************************************************************************************************

Imports Response = System.Windows.Forms.DialogResult

Public Class frmMain

#Region " Declares "

    Private Structure udtDisable
        Dim FacilityName As String
        Dim AlarmNumber() As String
    End Structure

    Private mDisable(0) As udtDisable
    Private mbEdits As Boolean = False
    Private mbPathError As Boolean
    Private msXMLPath As String = String.Empty

#End Region

#Region " Routines "

    Private Function DiscardEdits() As Boolean
        '********************************************************************************************
        'Description: Warn user that unsaved edits will be lost.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblStatus.Text = "Warning: Edits present!"

        Dim lResult As Windows.Forms.DialogResult

        lResult = MessageBox.Show("All edits to masked alarm list will be lost. Are you sure that you want to continue?", "Edits Present Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
        If lResult = Windows.Forms.DialogResult.Yes Then
            Return True
            mbEdits = False
        Else
            Return False
        End If
        lblStatus.Text = String.Empty

    End Function

    Private Function FormatAlarm(ByVal FacilityName As String, ByVal AlarmNumber As String) As String
        '********************************************************************************************
        'Description: Make sure masked alarm mnemonic is in the proper format.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMnemonic As String = String.Empty
        Dim nAlarmNumber As Integer = 0
        Dim bNumeric As Boolean = False

        sMnemonic = Strings.Trim(FacilityName.ToUpper)
        sMnemonic = Strings.Left(sMnemonic, 4)
        If Strings.Len(sMnemonic) < 4 Then sMnemonic = sMnemonic & Strings.Space(4 - Strings.Len(sMnemonic))
        bNumeric = IsNumeric(AlarmNumber)
        If bNumeric Then
            AlarmNumber = Strings.Left(Strings.Trim(AlarmNumber), 3)
            nAlarmNumber = CType(AlarmNumber, Integer)
        Else
            If Strings.Trim(AlarmNumber) <> "*" Then
                bNumeric = True
            End If
        End If

        If bNumeric Then
            sMnemonic = sMnemonic & "-" & Strings.Format(nAlarmNumber, "000")
        Else
            sMnemonic = sMnemonic & "-" & Strings.Trim(AlarmNumber)
        End If
        Return sMnemonic

    End Function

    Private Sub subAddAlarmNumber(ByRef ThisAlarm As udtDisable, ByVal AlarmNumber As String)
        '********************************************************************************************
        'Description: Mask an alarm number belonging to the selected Facility Code.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If AlarmNumber = "*" Then
            'Mask all alarms for this Facility code
            ReDim ThisAlarm.AlarmNumber(0)
            ThisAlarm.AlarmNumber(0) = AlarmNumber
            lblStatus.Text = "Masking " & ThisAlarm.FacilityName & "-*"
        Else
            'if all alarms aren't masked for this Facility Code, add this alarm to the list
            If ThisAlarm.AlarmNumber(0) <> "*" Then
                Dim nLen As Integer = ThisAlarm.AlarmNumber.GetLength(0)
                ReDim Preserve ThisAlarm.AlarmNumber(nLen)
                ThisAlarm.AlarmNumber(nLen) = AlarmNumber
                lblStatus.Text = "Masking " & ThisAlarm.FacilityName & "-" & AlarmNumber
            Else
                lblStatus.Text = String.Empty
                Dim lResult As Windows.Forms.DialogResult
                lResult = MessageBox.Show("All alarms for Facility Name " & ThisAlarm.FacilityName & _
                                          " are currently masked. Are you sure that you only want to mask " & _
                                          ThisAlarm.FacilityName & "-" & AlarmNumber & "?", "Warning", _
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If lResult = Windows.Forms.DialogResult.Yes Then
                    ThisAlarm.AlarmNumber(0) = AlarmNumber
                    lblStatus.Text = "Masking " & ThisAlarm.FacilityName & "-" & AlarmNumber
                End If
            End If
        End If

    End Sub

    Private Sub subAddMaskedAlarm(ByVal FacilityName As String, ByVal AlarmNumber As String, Optional ByVal UpdateList As Boolean = True)
        '********************************************************************************************
        'Description: Add a masked alarm to the list.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim myEntry As udtDisable
        Dim bFacFound As Boolean = False

        'First, check if mDisable is empty
        If mDisable(0).FacilityName = String.Empty Then
            mDisable(0).FacilityName = FacilityName.ToUpper
            mDisable(0).AlarmNumber(0) = AlarmNumber
            lblStatus.Text = "Masking " & mDisable(0).FacilityName & "-" & AlarmNumber
        Else
            Dim nIndex As Integer = 0
            For nIndex = 0 To mDisable.GetUpperBound(0)
                myEntry = mDisable(nIndex)
                If FacilityName.ToUpper = myEntry.FacilityName Then
                    Dim sAlarm As String = String.Empty
                    Dim bMasked As Boolean = False
                    bFacFound = True
                    For Each sAlarm In myEntry.AlarmNumber
                        If (sAlarm = AlarmNumber) Then
                            lblStatus.Text = myEntry.FacilityName & "-" & AlarmNumber & " already masked!"
                            bMasked = True
                            Exit For
                        End If
                    Next
                    If Not bMasked Then
                        'Facility Code exisits - add masked alarm
                        Call subAddAlarmNumber(myEntry, AlarmNumber)
                        'Update the array
                        mDisable(nIndex) = myEntry
                    End If
                    Exit For
                End If
            Next
            If Not bFacFound Then
                'Facility Code does not exisit - add an element to mDisable
                Dim nLen As Integer = mDisable.GetLength(0)
                ReDim Preserve mDisable(nLen)
                ReDim mDisable(nLen).AlarmNumber(0)
                mDisable(nLen).FacilityName = FacilityName.ToUpper
                mDisable(nLen).AlarmNumber(0) = AlarmNumber
                lblStatus.Text = "Masking " & mDisable(nLen).FacilityName & "-" & AlarmNumber
            End If
        End If
        If UpdateList Then Call subListMaskedAlarms()

    End Sub

    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Initialize frmMain.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim mDisable(0).AlarmNumber(0)

        mDisable(0).FacilityName = String.Empty
        mDisable(0).AlarmNumber(0) = String.Empty
        lstMaskedAlarms.Items.Clear()

        btnAdd.Enabled = False
        btnCheck.Enabled = False
        btnRemove.Enabled = False
        btnWrite.Enabled = False

        Call subProcessCommandLine() 'Path should be in the command line arguments RJO 08/30/12

        If msXMLPath = String.Empty Then
            'Try the registry (legacy code)
            msXMLPath = CType(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\FANUC\PAINTworks", "DBPath", Nothing), String)

            If msXMLPath = String.Empty Then
                'Not in the registry either - Ask the user.
                FolderBrowserDialog1.Description = "Please select the folder that contains the MaskedAlarms.xml file."
                FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer

                If FolderBrowserDialog1.ShowDialog() = Response.OK Then
                    msXMLPath = FolderBrowserDialog1.SelectedPath
                End If
            Else
                'Complete the registry path
                If Strings.Right(msXMLPath, 1) <> "\" Then msXMLPath = msXMLPath & "\"
                msXMLPath = msXMLPath & "XML\"
            End If
        End If

        If Strings.Right(msXMLPath, 1) <> "\" Then msXMLPath = msXMLPath & "\"

        If Not IO.File.Exists(msXMLPath & "MaskedAlarms.xml") Then
            MessageBox.Show("Could not find file '" & msXMLPath & "MaskedAlarms.xml'. Exiting screen.", _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            mbPathError = True
        Else
            Dim sFacNames() As String = Strings.Split("ACAL,AG,APSH,ARC,ASBN,CALB,CALM,CD,CMND,CNTR,COND,COPT,CPMO,CUST,DICT,DJOG,DMDR,DMER,DNET,DX," & _
                                      "ELOG,ELSE,FILE,FLEX,FLPY,FRCE,FRSY,HOST,HRTL,IBSS,INTP,ISD,JOG,LANG,LECO,LNTK,LSTP,MACR,MARL," & _
                                      "MCTL,MEMO,MENT,MHND,MIGE,MOTN,MUPS,OPTN,OS,PALL,PALT,PMON,PAIN,PLCE,PLCR,PLCZ,PNT1,PNT2,PRIO,PROF,PROG,PWD,PWRK," & _
                                      "QMGR,RIPE,ROUT,RPC,RPM,RTCP,SCIO,SEAL,SENS,SHAP,SP,SPOT,SPRM,SRIO,SRVO,SSPC,SVGN,SYST,TAST,TCPP," & _
                                      "TG,THSR,TJOG,TMAT,TOOL,TPIF,TRAK,VARS,VISN,WEAV,WMAP,WNDW,XMLF", ",")
            For Each sFacility As String In sFacNames
                cboFacilityName.Items.Add(sFacility)
            Next

            Call subReadMaskedAlarms()
        End If

    End Sub

    Private Sub subListMaskedAlarms()
        '********************************************************************************************
        'Description: Populate the list of Masked Alarms.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim ThisEntry As udtDisable

        lstMaskedAlarms.Items.Clear()
        For Each ThisEntry In mDisable
            Dim sAlarm As String = String.Empty

            For Each sAlarm In ThisEntry.AlarmNumber
                lstMaskedAlarms.Items.Add(FormatAlarm(ThisEntry.FacilityName, sAlarm))
            Next
        Next

    End Sub

    Private Sub subParseAlarm(ByVal Mnemonic As String, ByRef FacilityName As String, ByRef AlarmNumber As String)
        '********************************************************************************************
        'Description: Splits the alarm Mnemonic argument into its component parts.
        '
        'Parameters: Mnemonic
        'Returns:    Facility Name and Alarm Number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nDelimPos As Integer = Strings.InStr(Mnemonic, "-")

        FacilityName = Strings.Left(Mnemonic, nDelimPos - 1)
        AlarmNumber = Strings.Right(Mnemonic, Strings.Len(Mnemonic) - nDelimPos)

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

        ' 12/13/12  MSW     subProcessCommandLine - handle spaces in the path name
        '********************************************************************************************
        Dim sPathArg As String = "/path="

        For nParm As Integer = 0 To My.Application.CommandLineArgs.Count - 1

            If My.Application.CommandLineArgs(nParm).ToLower.StartsWith(sPathArg) Then
                msXMLPath = My.Application.CommandLineArgs(nParm).Remove(0, sPathArg.Length)
                If (nParm < (My.Application.CommandLineArgs.Count - 1)) AndAlso _
                     (My.Application.CommandLineArgs(nParm + 1).Substring(0, 1) <> "/") Then
                    msXMLPath = msXMLPath & " " & My.Application.CommandLineArgs(nParm + 1)
                    If (nParm < (My.Application.CommandLineArgs.Count - 2)) AndAlso _
                     (My.Application.CommandLineArgs(nParm + 2).Substring(0, 1) <> "/") Then
                        msXMLPath = msXMLPath & " " & My.Application.CommandLineArgs(nParm + 1)
                    End If
                End If
            End If
        Next

    End Sub

    Private Sub subReadMaskedAlarms()
        '********************************************************************************************
        'Description: Read the list of masked alarms From MaskedAlarms.xml file and display.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nAlarms As Integer = 0
        Dim nAlarm As Integer = 0
        Dim sFacilityName As String = String.Empty
        Dim sAlarmNumber As String = String.Empty
        Dim objMaskedAlarms As New DataSet

        If mbEdits Then
            If Not DiscardEdits() Then Exit Sub
        End If

        ReDim mDisable(0)
        ReDim mDisable(0).AlarmNumber(0)
        lstMaskedAlarms.Items.Clear()
        lblStatus.Text = String.Empty

        With objMaskedAlarms
            .ReadXmlSchema(msXMLPath & "MaskedAlarms.xsd")
            .ReadXml(msXMLPath & "MaskedAlarms.xml")

            nAlarms = .Tables("Alarm").Rows.Count
            For nAlarm = 0 To nAlarms - 1
                sFacilityName = CType(.Tables("Alarm").Rows(nAlarm)("FacilityName"), String)
                sAlarmNumber = CType(.Tables("Alarm").Rows(nAlarm)("AlarmNumber"), String)
                Call subAddMaskedAlarm(sFacilityName, sAlarmNumber)
            Next
            .Dispose()
        End With

        Call subListMaskedAlarms()

        btnAdd.Enabled = True
        btnCheck.Enabled = True
        btnWrite.Enabled = True

        lblStatus.Text = "Masked alarms have been read from XML file."

    End Sub

    Private Sub subWriteMaskedAlarms()
        '********************************************************************************************
        'Description: Write the list of masked alarms to MaskedAlarms.xml file.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nAlarms As Integer = 0
        Dim nAlarm As Integer = 0
        Dim nDelimPos As Integer = 0
        Dim objMaskedAlarms As New DataSet

        lblStatus.Text = String.Empty
        With objMaskedAlarms
            .ReadXmlSchema(msXMLPath & "MaskedAlarms.xsd")

            nAlarms = lstMaskedAlarms.Items.Count - 1
            For nAlarm = 0 To nAlarms
                Dim newRow As DataRow = .Tables("Alarm").NewRow

                nDelimPos = Strings.InStr(CType(lstMaskedAlarms.Items(nAlarm), String), "-")
                newRow("FacilityName") = Strings.Left(CType(lstMaskedAlarms.Items(nAlarm), String), nDelimPos - 1)
                newRow("AlarmNumber") = Strings.Right(CType(lstMaskedAlarms.Items(nAlarm), String), Strings.Len(lstMaskedAlarms.Items(nAlarm)) - nDelimPos)
                .Tables("Alarm").Rows.Add(newRow)
            Next

            .WriteXml(msXMLPath & "MaskedAlarms.xml")
            .Dispose()
        End With
        lblStatus.Text = "Masked alarms have been written to XML file."
        mbEdits = False

    End Sub

    Private Function ValidateAlarm() As Boolean
        '********************************************************************************************
        'Description: Make sure what they entered is valid
        '
        'Parameters: none
        'Returns:    True if valid entry
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If (cboFacilityName.Text = String.Empty) Or (Strings.Len(cboFacilityName.Text) > 4) Or (IsNumeric(cboFacilityName.Text)) Then
            MessageBox.Show("Please enter a valid Facility Name.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            lblStatus.Text = "Invalid Entry"
            Return False
        End If

        If IsNumeric(txtAlarmNumber.Text) Or (txtAlarmNumber.Text = "*") Then
            lblStatus.Text = String.Empty
            Return True
        Else
            MessageBox.Show("Alarm # must be a number from 0 to 999 or an asterisk (*).", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            lblStatus.Text = "Invalid Entry"
            Return False
        End If

    End Function

#End Region

#Region " Events "

    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        '********************************************************************************************
        'Description: Add Masked Alarm to List button was clicked.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/26/13  RJO     Changed sAlarmNumber to Integer so it will format properly
        '********************************************************************************************

        If ValidateAlarm() Then
            Call subAddMaskedAlarm(cboFacilityName.Text, txtAlarmNumber.Text)
            Dim sAlarmNumber As String = txtAlarmNumber.Text
            If sAlarmNumber <> "*" Then sAlarmNumber = Strings.Format(CType(sAlarmNumber, Integer), "000")
            lblStatus.Text = cboFacilityName.Text.ToUpper & "-" & sAlarmNumber & " has been added to masked alarms list."
            mbEdits = True
        End If

    End Sub

    Private Sub btnCheck_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCheck.Click
        '********************************************************************************************
        'Description: Check Alarm Status button was clicked.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim myEntry As udtDisable
        Dim bMasked As Boolean = False

        If ValidateAlarm() Then
            lblStatus.Text = String.Empty
            'First check if the Facility Name is in the mDisable Array
            For Each myEntry In mDisable
                If myEntry.FacilityName = cboFacilityName.Text.ToUpper Then
                    Dim sAlarm As String = String.Empty
                    'It's there, now see if the specified alarm is masked.
                    For Each sAlarm In myEntry.AlarmNumber
                        If (sAlarm = "*") Or (sAlarm = txtAlarmNumber.Text) Then
                            bMasked = True
                            Exit For
                        End If
                    Next 'sAlarm
                End If
            Next 'myEntry
            lblStatus.Text = cboFacilityName.Text.ToUpper & "-" & txtAlarmNumber.Text & ": Masked = " & bMasked.ToString
        End If

    End Sub

    Private Sub btnRead_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead.Click
        '********************************************************************************************
        'Description: Read Masked Alarms from XML File button was clicked.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subReadMaskedAlarms()

    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        '********************************************************************************************
        'Description: Remove Masked Alarm from List button was clicked.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblStatus.Text = String.Empty

        If lstMaskedAlarms.SelectedIndex >= 0 Then
            Dim sAlarm As String = lstMaskedAlarms.Items(lstMaskedAlarms.SelectedIndex).ToString
            Dim sFacility As String = String.Empty
            Dim sNumber As String = String.Empty
            ReDim mDisable(0)
            ReDim mDisable(0).AlarmNumber(0)

            lstMaskedAlarms.Items.RemoveAt(lstMaskedAlarms.SelectedIndex)
            lstMaskedAlarms.Refresh()
            're-construct mDisable()
            For nAlarm As Integer = 0 To (lstMaskedAlarms.Items.Count - 1)
                Call subParseAlarm(lstMaskedAlarms.Items(nAlarm).ToString, sFacility, sNumber)
                Call subAddMaskedAlarm(sFacility, sNumber, False)
            Next

            lblStatus.Text = sAlarm & " has been removed from masked alarm list."
            btnRemove.Enabled = False
            mbEdits = True
        End If

    End Sub

    Private Sub btnWrite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWrite.Click
        '********************************************************************************************
        'Description: Write Masked Alarms to XML File button was clicked.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subWriteMaskedAlarms()

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Check for edits on Form Close.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If mbEdits Then
            If Not DiscardEdits() Then e.Cancel = True
        End If

    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Check for edits on Form Close.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subInitializeForm()

    End Sub

    Private Sub frmMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: If the Masked Alarms xml file could not be read, bail out here.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If mbPathError Then Me.Close()

    End Sub

    Private Sub lstMaskedAlarms_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstMaskedAlarms.SelectedValueChanged
        '********************************************************************************************
        'Description: Masked Alarm has been selected from list.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFacilityName As String = String.Empty
        Dim sAlarmNumber As String = String.Empty

        If lstMaskedAlarms.SelectedIndex >= 0 Then
            Call subParseAlarm(CType(lstMaskedAlarms.Items(lstMaskedAlarms.SelectedIndex), String), sFacilityName, sAlarmNumber)
            cboFacilityName.Text = sFacilityName
            txtAlarmNumber.Text = sAlarmNumber
            btnRemove.Enabled = True
        End If

    End Sub

#End Region

End Class
