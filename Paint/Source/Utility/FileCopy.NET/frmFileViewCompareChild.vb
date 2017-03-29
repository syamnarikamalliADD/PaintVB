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
' Description: File view/Compare MDI child window
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
' 05/13/2010    MSW     first draft
'********************************************************************************************

Public Class frmFileViewCompareChild
    Private msFileName As String
    Private msTitle As String
    Private msFileDate As String = String.Empty
    Private mbFolderView As Boolean = False
    Friend Enum eCompareSelect
        Base
        Compare
        None
    End Enum

    Private meCompareSelect As eCompareSelect = eCompareSelect.None

    Friend Property BaseCompareSelect() As eCompareSelect
        Get
            Return meCompareSelect
        End Get
        Set(ByVal value As eCompareSelect)
            meCompareSelect = value
            Select Case meCompareSelect
                Case eCompareSelect.None
                    Me.Text = msTitle
                Case eCompareSelect.Base
                    Me.Text = msTitle & gpsRM.GetString("ps_BASE")
                Case eCompareSelect.Compare
                    Me.Text = msTitle & gpsRM.GetString("ps_COMPARE")
            End Select
        End Set
    End Property
    Friend Property Title() As String
        '********************************************************************************************
        'Description:  access the title bar text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return msTitle
        End Get
        Set(ByVal value As String)
            msTitle = value
            Select Case meCompareSelect
                Case eCompareSelect.None
                    Me.Text = msTitle
                Case eCompareSelect.Base
                    Me.Text = msTitle & gpsRM.GetString("ps_BASE")
                Case eCompareSelect.Compare
                    Me.Text = msTitle & gpsRM.GetString("ps_COMPARE")
            End Select
        End Set
    End Property
    Friend Property FolderView() As Boolean
        '********************************************************************************************
        'Description:  select floder view to list files
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Get
            Return mbFolderView
        End Get
        Set(ByVal value As Boolean)
            mbFolderView = value
            mlstvwFolder.Visible = mbFolderView
            mRtbText.Visible = Not (mbFolderView)

        End Set
    End Property
    Friend ReadOnly Property TextBox() As RichTextBox
        '********************************************************************************************
        'Description:  access the listview
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return mRtbText
        End Get

    End Property

    Friend ReadOnly Property Filelist() As ListView
        '********************************************************************************************
        'Description:  access the richtextbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return mlstvwFolder
        End Get

    End Property

    Friend Property FileName() As String
        '********************************************************************************************
        'Description:  get the current file or pathname
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        Get
            Return msFileName
        End Get
        Set(ByVal value As String)
            msFileName = value
        End Set
    End Property
    Friend Sub subLoadFile(ByVal sPath As String)
        '********************************************************************************************
        'Description:  load a text file onto the form
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        If mbFolderView Then
            mlstvwFolder.Items.Clear()
            If IO.Directory.Exists(sPath) Then
                Dim sFiles() As String = IO.Directory.GetFiles(sPath)
                If sFiles.Length > 0 Then
                    For Each sFile As String In sFiles
                        Dim sFileSplit() As String = Split(sFile, "\")
                        mlstvwFolder.Items.Add(sFileSplit(sFileSplit.GetUpperBound(0)))
                    Next
                Else
                    mlstvwFolder.Items.Add(gpsRM.GetString("psEMPTY_FOLDER"))
                End If
            Else
                mlstvwFolder.Items.Add(gpsRM.GetString("psDIR_DOES_NOT_EXIST"))
            End If
        Else
            Dim oFile As New IO.FileInfo(sPath)
            mRtbText.LoadFile(sPath, RichTextBoxStreamType.PlainText)
            msFileDate = oFile.LastWriteTime.ToString
        End If
        subupdatestatus()
    End Sub
    Friend Sub subSaveFile(ByVal sPath As String)
        '********************************************************************************************
        'Description:  save a text file from the form
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/12/10  MSW	    first draft
        '********************************************************************************************  
        mRtbText.SaveFile(sPath, RichTextBoxStreamType.PlainText)
    End Sub

    Private Sub frmFileViewCompareChild_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description:  save a text file from the form
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/12/10  MSW	    first draft
        '********************************************************************************************  
        Select Case meCompareSelect
            Case eCompareSelect.Base
                frmFileViewCompare.moBaseForm = Nothing
            Case eCompareSelect.Compare
                frmFileViewCompare.moCompForm = Nothing
        End Select
    End Sub

    Private Sub frmFileViewCompareChild_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:  save a text file from the form
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/12/10  MSW	    first draft
        '********************************************************************************************  
        stsStatus.Visible = frmFileViewCompare.mnuToolbar.Checked
        mlstvwFolder.Location = mRtbText.Location
        mlstvwFolder.Size = mRtbText.Size
        mlstvwFolder.Visible = mbFolderView
    End Sub
    Private Sub subupdatestatus()
        '********************************************************************************************
        'Description:  update status line
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        If mbFolderView Then
            lblStatus.Text = mlstvwFolder.Items.Count & gpsRM.GetString("ps_FILES") 
        Else
            'Dim pt As Point = mRtbText.Cursor.Position
            Dim nChar As Integer = mRtbText.SelectionStart 'mRtbText.GetCharIndexFromPosition(pt)
            Dim nLine As Integer = mRtbText.GetLineFromCharIndex(nChar)
            Dim nCol As Integer = nChar - mRtbText.GetFirstCharIndexFromLine(nLine)
            ' "Date Modified: " & date & ", ", Line: " & curline & "/" & numlines & ", ", Column: "
            lblStatus.Text = gpsRM.GetString("ps_FVC_CHILD_STAT1") & msFileDate & _
            gpsRM.GetString("ps_FVC_CHILD_STAT2") & (nLine + 1).ToString & "/" & (mRtbText.Lines.GetUpperBound(0) + 1) & _
            gpsRM.GetString("ps_FVC_CHILD_STAT3") & nCol.ToString
        End If
    End Sub

    Private Sub mRtbText_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mRtbText.SelectionChanged
        '********************************************************************************************
        'Description:  update status line
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        If frmFileViewCompare.mbIgnoreEvents = False Then
            subupdatestatus()
        End If
    End Sub
    Friend Sub subResizertb()
        '********************************************************************************************
        'Description:  manually resize the rtb to account for the status bar
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        'Got the offsets from where they ended up at design time.  It seems like the form should provide a client size.
        mRtbText.Width = Me.Width - 8
        mlstvwFolder.Width = Me.Width - 8
        If stsStatus.Visible Then
            mRtbText.Height = (Me.Height - 36) - stsStatus.Height
            mlstvwFolder.Height = (Me.Height - 36) - stsStatus.Height
        Else
            mRtbText.Height = Me.Height - 36
            mlstvwFolder.Height = Me.Height - 36
        End If
    End Sub
    Private Sub frmFileViewCompareChild_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        '********************************************************************************************
        'Description:  manually resize the rtb to account for the status bar
        '
        'Parameters: path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subResizertb()
    End Sub
    
End Class