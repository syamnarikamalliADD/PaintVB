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
' Form/Module: uctrlISD.vb
'
' Description: ISD cartoon
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: 
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    02/21/14   MSW     1st version                                                4.01.07.00
'********************************************************************************************
Friend Class uctrlISD

    Implements PrmSetToon
    Private Const msMODULE As String = "uctrlISD.vb"
    Private Const mnINIT As Integer = -123
    Private mnValveA As Integer = mnINIT
    Private mnValveB As Integer = mnINIT
    Private mnGun1 As Integer = mnINIT
    Private mnGun2 As Integer = mnINIT
    Private mnGun3 As Integer = mnINIT
    Private mnGun4 As Integer = mnINIT
    Private mnGun5 As Integer = mnINIT
    Private mnGun6 As Integer = mnINIT
    Private mnOverTravel As Integer = mnINIT
    Private mnPos As Integer = mnINIT
    Private mnNumGuns As Integer = mnINIT
    Private mnGunPres As Integer = mnINIT
    Private mnDispPres As Integer = mnINIT
    Private mnSupplyPres As Integer = mnINIT
    Private mnVolume As Integer = mnINIT
    Private Sub subUpdateLines()
        '********************************************************************************************
        'Description: update sealer line colors
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oSupplyColor As System.Drawing.Color
            Dim oAFillBSprayColor As System.Drawing.Color
            Dim oBFillASprayColor As System.Drawing.Color
            Dim oGunColor As System.Drawing.Color
            If mnValveA = 0 And mnValveB = 0 Then
                oSupplyColor = Color.Black
                oAFillBSprayColor = Color.Black
                oBFillASprayColor = Color.Black
                oGunColor = Color.Black
            ElseIf mnValveA > 0 And mnValveB = 0 Then
                oSupplyColor = Color.Yellow
                oAFillBSprayColor = Color.Yellow
                oBFillASprayColor = Color.Green
                oGunColor = Color.Green
            ElseIf mnValveA = 0 And mnValveB > 0 Then
                oSupplyColor = Color.Yellow
                oAFillBSprayColor = Color.Green
                oBFillASprayColor = Color.Yellow
                oGunColor = Color.Green
            ElseIf mnValveA > 0 And mnValveB > 0 Then
                oSupplyColor = Color.Yellow
                oAFillBSprayColor = Color.Red
                oBFillASprayColor = Color.Red
                oGunColor = Color.Green
            End If
            'Supply lines
            lblSupplyLine0.BackColor = oSupplyColor
            lblSupplyLine1.BackColor = oSupplyColor
            lblSupplyLine2.BackColor = oSupplyColor
            lblSupplyLine3.BackColor = oSupplyColor
            lblSupplyLine4.BackColor = oSupplyColor
            lblSupplyLine5.BackColor = oSupplyColor
            lblSupplyPressureLine.BackColor = oSupplyColor
            'Fill A/ Spray B
            lblAFillBSpray0.BackColor = oAFillBSprayColor
            lblAFillBSpray1.BackColor = oAFillBSprayColor
            lblAFillBSpray2.BackColor = oAFillBSprayColor
            lblAFillBSpray3.BackColor = oAFillBSprayColor
            pnlAFillBSpray.BackColor = oAFillBSprayColor
            'Fill B/ Spray A
            lblBFillASpray0.BackColor = oBFillASprayColor
            lblBFillASpray1.BackColor = oBFillASprayColor
            lblBFillASpray2.BackColor = oBFillASprayColor
            lblBFillASpray3.BackColor = oBFillASprayColor
            pnlBFillASpray.BackColor = oBFillASprayColor
            'Output Lines
            lblTrigLine0.BackColor = oGunColor
            lblTrigLine1.BackColor = oGunColor
            lblTrigLine2.BackColor = oGunColor
            lblTrigLine3.BackColor = oGunColor
            lblDispensePressureLine.BackColor = oGunColor
            lblGunPresLine0.BackColor = oGunColor
            lblGunPresLine1.BackColor = oGunColor
            lblGunPresLine2.BackColor = oGunColor
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: Init", ex.Message)
        End Try
    End Sub
    Public Sub SetItem(ByVal itemConfig As frmMain.tItemCfg, ByRef oData As Object) Implements infPrmSetToon.PrmSetToon.SetItem
        '********************************************************************************************
        'Description: data for the cartoon
        '               Update data as it comes in.  Show optional items if data comes in
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case itemConfig.Tag
                Case "psSlNumGuns"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    'Design time settings, 6 guns
                    'Me.pnlGun1.Location = New System.Drawing.Point(217, 6)
                    'Me.pnlGun2.Location = New System.Drawing.Point(239, 6)
                    'Me.pnlGun3.Location = New System.Drawing.Point(261, 6)
                    'Me.pnlGun4.Location = New System.Drawing.Point(283, 6)
                    'Me.pnlGun5.Location = New System.Drawing.Point(305, 6)
                    'Me.pnlGun6.Location = New System.Drawing.Point(327, 6)
                    'Me.pnlGun1.Size = New System.Drawing.Size(21, 21)
                    'Me.lblTrigLine1.Location = New System.Drawing.Point(226, 28)
                    'Me.lblTrigLine1.Size = New System.Drawing.Size(113, 2)
                    'Me.lblTrigLine0.Location = New System.Drawing.Point(281, 30)
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnNumGuns Then
                        Dim nTrigSpace As Integer = pnlGun2.Left - pnlGun1.Left 'Space between gun drawings
                        Dim nHalfTrigSpace As Integer = nTrigSpace \ 2
                        Dim nLine0 As Integer = Me.lblTrigLine0.Left
                        mnNumGuns = nTmp
                        lblTrigLine1.Width = 3 + (nTmp - 1) * nTrigSpace
                        lblTrigLine1.Left = nLine0 - ((nTmp - 1) * nHalfTrigSpace)
                        pnlGun1.Left = (nLine0 - 9) - ((nTmp - 1) * nHalfTrigSpace)
                        pnlGun2.Left = pnlGun1.Left + nTrigSpace
                        pnlGun3.Left = pnlGun2.Left + nTrigSpace
                        pnlGun4.Left = pnlGun3.Left + nTrigSpace
                        pnlGun5.Left = pnlGun4.Left + nTrigSpace
                        pnlGun6.Left = pnlGun5.Left + nTrigSpace
                        Select Case nTmp
                            Case 1
                                pnlGun1.Visible = True
                                pnlGun2.Visible = False
                                pnlGun3.Visible = False
                                pnlGun4.Visible = False
                                pnlGun5.Visible = False
                                pnlGun6.Visible = False
                            Case 2
                                pnlGun1.Visible = True
                                pnlGun2.Visible = True
                                pnlGun3.Visible = False
                                pnlGun4.Visible = False
                                pnlGun5.Visible = False
                                pnlGun6.Visible = False
                            Case 3
                                pnlGun1.Visible = True
                                pnlGun2.Visible = True
                                pnlGun3.Visible = True
                                pnlGun4.Visible = False
                                pnlGun5.Visible = False
                                pnlGun6.Visible = False
                            Case 4
                                pnlGun1.Visible = True
                                pnlGun2.Visible = True
                                pnlGun3.Visible = True
                                pnlGun4.Visible = True
                                pnlGun5.Visible = False
                                pnlGun6.Visible = False
                            Case 5
                                pnlGun1.Visible = True
                                pnlGun2.Visible = True
                                pnlGun3.Visible = True
                                pnlGun4.Visible = True
                                pnlGun5.Visible = True
                                pnlGun6.Visible = False
                            Case 6
                                pnlGun1.Visible = True
                                pnlGun2.Visible = True
                                pnlGun3.Visible = True
                                pnlGun4.Visible = True
                                pnlGun5.Visible = True
                                pnlGun6.Visible = True
                        End Select
                    End If
                Case "psDispStatus"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                Case "psDispMode"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                Case "psMeterDirection"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                Case "psMeterPosition"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnPos Then
                        mnPos = nTmp
                        lblMeterPosition.Text = nTmp.ToString & "%"
                        If nTmp < 0 Then
                            nTmp = 0
                        End If
                        If nTmp > 100 Then
                            nTmp = 100
                        End If
                        tlpCylinder.RowStyles.Item(0).Height = nTmp
                        tlpCylinder.RowStyles.Item(1).Height = 100 - nTmp
                    End If
                Case "psSupplyPressure"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnSupplyPres Then
                        lblSupplyPressure.Text = nTmp.ToString
                        If mnSupplyPres = mnINIT Then
                            pnlSupplyPressure.Visible = True
                            lblSupplyPressureLine.Visible = True
                            lblSupplyPressureLabel.Visible = True
                        End If
                        mnSupplyPres = nTmp
                    End If
                Case "psDispPressure"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnDispPres Then
                        lblDispensePressure.Text = nTmp.ToString
                        If mnDispPres = mnINIT Then
                            pnlDispensePressure.Visible = True
                            lblDispensePressureLine.Visible = True
                            lblDispensePressureLabel.Visible = True
                        End If
                        mnDispPres = nTmp
                    End If
                Case "psGunPressure"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnGunPres Then
                        lblGunPressure.Text = nTmp.ToString
                        If mnGunPres = mnINIT Then
                            pnlGunPressure.Visible = True
                            lblGunPresLine0.Visible = True
                            lblGunPresLine1.Visible = True
                            lblGunPresLine2.Visible = True
                            lblGunPressureLabel.Visible = True
                        End If
                        mnGunPres = nTmp
                    End If
                Case "psVolumeDispensed"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnVolume Then
                        lblVolume.Text = nTmp.ToString
                        If mnVolume = mnINIT Then
                            lblVolume.Visible = True
                            lblVolumeLabel.Visible = True
                        End If
                        mnVolume = nTmp
                    End If
                Case "psValveA"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveA Then
                        mnValveA = nTmp
                        If nTmp > 0 Then
                            picValveA1.Image = DirectCast(gpsRM.GetObject("Valve_R_On", mLanguage.FixedCulture), Image)
                            picValveA2.Image = DirectCast(gpsRM.GetObject("Valve_R_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveA1.Image = DirectCast(gpsRM.GetObject("Valve_R", mLanguage.FixedCulture), Image)
                            picValveA2.Image = DirectCast(gpsRM.GetObject("Valve_R", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psValveB"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveB Then
                        mnValveB = nTmp
                        If nTmp > 0 Then
                            picValveB1.Image = DirectCast(gpsRM.GetObject("Valve_L_On", mLanguage.FixedCulture), Image)
                            picValveB2.Image = DirectCast(gpsRM.GetObject("Valve_L_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveB1.Image = DirectCast(gpsRM.GetObject("Valve_L", mLanguage.FixedCulture), Image)
                            picValveB2.Image = DirectCast(gpsRM.GetObject("Valve_L", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psOverTravel"
                    '<CustomCBO>psOTPOS;4096;psOTNEG;8192;psOTBOTH;12288;psOTOK;0</CustomCBO>
                    '<CustomColors>Red;4096;Red;8192;Red;12288;Green;0</CustomColors>
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnOverTravel Then
                        mnOverTravel = nTmp
                        Select Case nTmp
                            Case 4096
                                lblOverTravel1.Visible = True
                                lblOverTravel2.Visible = False
                            Case 8192
                                lblOverTravel1.Visible = False
                                lblOverTravel2.Visible = True
                            Case 12288
                                lblOverTravel1.Visible = True
                                lblOverTravel2.Visible = True
                            Case 0
                                lblOverTravel1.Visible = False
                                lblOverTravel2.Visible = False
                            Case Else
                                mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: SetItem", itemConfig.Tag & ":  " & oData.ToString & " - Unexpected value")
                        End Select
                    End If
                Case "psGun1Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun1 Then
                        mnGun1 = nTmp
                        If nTmp > 0 Then
                            pnlGun1.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun1.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
                Case "psGun2Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun2 Then
                        mnGun2 = nTmp
                        If nTmp > 0 Then
                            pnlGun2.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun2.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
                Case "psGun3Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun3 Then
                        mnGun3 = nTmp
                        If nTmp > 0 Then
                            pnlGun3.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun3.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
                Case "psGun4Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun4 Then
                        mnGun4 = nTmp
                        If nTmp > 0 Then
                            pnlGun4.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun4.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
                Case "psGun5Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun5 Then
                        mnGun5 = nTmp
                        If nTmp > 0 Then
                            pnlGun5.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun5.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
                Case "psGun6Status"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim bTmp As Boolean = CType(oData.ToString, Boolean)
                    Dim nTmp As Integer = 0
                    If bTmp Then
                        nTmp = 1
                    End If
                    If nTmp <> mnGun6 Then
                        mnGun6 = nTmp
                        If nTmp > 0 Then
                            pnlGun6.BackgroundImage = DirectCast(gpsRM.GetObject("GunOn", mLanguage.FixedCulture), Image)
                        Else
                            pnlGun6.BackgroundImage = DirectCast(gpsRM.GetObject("GunOff", mLanguage.FixedCulture), Image)
                        End If
                    End If
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: SetItem", itemConfig.Tag & ":  " & oData.ToString & " - " & ex.Message)
        End Try

    End Sub
    Public Sub Init() Implements infPrmSetToon.PrmSetToon.Init
        '********************************************************************************************
        'Description: init labels
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            lblGunPressureLabel.Text = gpsRM.GetString("psGunPresCAP")
            lblGunPressureUnits.Text = gpsRM.GetString("pspsi")
            lblSupplyPressureLabel.Text = gpsRM.GetString("psSupplyPresCAP")
            lblSupplyPressureUnits.Text = gpsRM.GetString("pspsi")
            lblDispensePressureLabel.Text = gpsRM.GetString("psDispPresCAP")
            lblDispensePressureUnits.Text = gpsRM.GetString("pspsi")
            lblOverTravel1.Text = gpsRM.GetString("psOvertravelCAP")
            lblOverTravel2.Text = gpsRM.GetString("psOvertravelCAP")
            lblMeterPositionLabel.Text = gpsRM.GetString("psMeterPosCAP")
            lblVolumeLabel.Text = gpsRM.GetString("psVolumeDispCAP")
            lblA1.Text = gpsRM.GetString("psValveACAP")
            lblA2.Text = gpsRM.GetString("psValveACAP")
            lblB1.Text = gpsRM.GetString("psValveBCAP")
            lblB2.Text = gpsRM.GetString("psValveBCAP")
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: Init", ex.Message)
        End Try
    End Sub
End Class

