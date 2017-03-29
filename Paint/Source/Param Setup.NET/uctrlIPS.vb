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
' Form/Module: uctrlIPS.vb
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
Friend Class uctrlIPS

    Implements PrmSetToon
    Private Const msMODULE As String = "uctrlIPS.vb"
    Private Const mnINIT As Integer = -123
    Private mnValveAIn As Integer = mnINIT
    Private mnValveBIn As Integer = mnINIT
    Private mnValveAOut As Integer = mnINIT
    Private mnValveBOut As Integer = mnINIT
    Private mnGun1 As Integer = mnINIT
    Private mnGun2 As Integer = mnINIT
    Private mnGun3 As Integer = mnINIT
    Private mnGun4 As Integer = mnINIT
    Private mnGun5 As Integer = mnINIT
    Private mnGun6 As Integer = mnINIT
    Private mnOverTravelA As Integer = mnINIT
    Private mnOverTravelB As Integer = mnINIT
    Private mnPosA As Integer = mnINIT
    Private mnPosB As Integer = mnINIT
    Private mnVolA As Integer = mnINIT
    Private mnVolB As Integer = mnINIT
    Private mnNumGuns As Integer = mnINIT
    Private mnGunPres As Integer = mnINIT
    Private mnSupplyPres As Integer = mnINIT
    Private mnMtrAPres As Integer = mnINIT
    Private mnMtrBPres As Integer = mnINIT
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
            Dim oAInColor As System.Drawing.Color
            Dim oBInColor As System.Drawing.Color
            Dim oAOutColor As System.Drawing.Color
            Dim oBOutColor As System.Drawing.Color
            Dim oGunColor As System.Drawing.Color
            If mnValveAIn <= 0 And mnValveBIn <= 0 Then
                oSupplyColor = Color.Black
                oAInColor = Color.Black
                oBInColor = Color.Black
            ElseIf mnValveAIn > 0 And mnValveBIn <= 0 Then
                oSupplyColor = Color.Yellow
                oAInColor = Color.Yellow
                oBInColor = Color.Black
            ElseIf mnValveAIn <= 0 And mnValveBIn > 0 Then
                oSupplyColor = Color.Yellow
                oAInColor = Color.Black
                oBInColor = Color.Yellow
            ElseIf mnValveAIn > 0 And mnValveBIn > 0 Then
                oSupplyColor = Color.Yellow
                oAInColor = Color.Yellow
                oBInColor = Color.Yellow
            End If
            If mnValveAOut <= 0 And mnValveBOut <= 0 Then
                oGunColor = Color.Black
                oAOutColor = Color.Black
                oBOutColor = Color.Black
            ElseIf mnValveAOut > 0 And mnValveBOut <= 0 Then
                oGunColor = Color.Green
                oAOutColor = Color.Green
                oBOutColor = Color.Black
            ElseIf mnValveAOut <= 0 And mnValveBOut > 0 Then
                oGunColor = Color.Green
                oAOutColor = Color.Black
                oBOutColor = Color.Green
            ElseIf mnValveAOut > 0 And mnValveBOut > 0 Then
                oSupplyColor = Color.Red
                oAOutColor = Color.Red
                oBOutColor = Color.Red
            End If
            'Supply lines
            lblSupplyLine0.BackColor = oSupplyColor
            lblSupplyLine1.BackColor = oSupplyColor
            lblSupplyLine2.BackColor = oSupplyColor
            lblSupplyLine3.BackColor = oSupplyColor
            lblSupplyLine4.BackColor = oSupplyColor
            lblSupplyLine5.BackColor = oSupplyColor
            lblSupplyLineB.BackColor = oSupplyColor
            lblSupplyPressureLine.BackColor = oSupplyColor
            'Line A in
            lblAInLine.BackColor = oAInColor
            'Line B In
            lblBInLine.BackColor = oBInColor
            'Line A Out
            lblAOutLine.BackColor = oAOutColor
            lblMtrAPressureLine.BackColor = oAOutColor
            'Line B Out
            lblBOutLine.BackColor = oBOutColor
            lblMtrBPressureLine.BackColor = oBOutColor

            'Output Lines
            lblTrigLine0.BackColor = oGunColor
            lblTrigLine1.BackColor = oGunColor
            lblTrigLine2.BackColor = oGunColor
            lblTrigLine3.BackColor = oGunColor
            lblTrigLineB.BackColor = oGunColor
            lblGunPresLine0.BackColor = oGunColor
            lblGunPresLine1.BackColor = oGunColor
            lblGunPresLine2.BackColor = oGunColor

            If mnValveAIn > 0 And mnValveAOut > 0 Then
                lblCanAInner.BackColor = Color.Red
            ElseIf mnValveAIn <= 0 And mnValveAOut <= 0 Then
                lblCanAInner.BackColor = Color.LightGray
            ElseIf mnValveAIn > 0 And mnValveAOut <= 0 Then
                lblCanAInner.BackColor = Color.Yellow
            ElseIf mnValveAIn <= 0 And mnValveAOut > 0 Then
                lblCanAInner.BackColor = Color.Green
            End If
            If mnValveBIn > 0 And mnValveBOut > 0 Then
                lblCanBInner.BackColor = Color.Red
            ElseIf mnValveBIn <= 0 And mnValveBOut <= 0 Then
                lblCanBInner.BackColor = Color.LightGray
            ElseIf mnValveBIn > 0 And mnValveBOut <= 0 Then
                lblCanBInner.BackColor = Color.Yellow
            ElseIf mnValveBIn <= 0 And mnValveBOut > 0 Then
                lblCanBInner.BackColor = Color.Green
            End If
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
            Const nMeter0Height As Integer = 56
            Const nMeter100Height As Integer = 137

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
                Case "psMtrAPosition", "psMtrPosition"
                    'Const nMeter0Height As Integer = 56
                    'Const nMeter1000Height As Integer = 137
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnPosA Then
                        mnPosA = nTmp
                        lblMeterAPosition.Text = nTmp.ToString & "%"
                        If nTmp < 0 Then
                            nTmp = 0
                        End If
                        If nTmp > 100 Then
                            nTmp = 100
                        End If
                        lblPistonA.Height = nMeter0Height + CInt(nTmp * (nMeter100Height - nMeter0Height) / 100)
                    End If
                Case "psMtrBPosition"
                    'Const nMeter0Height As Integer = 56
                    'Const nMeter1000Height As Integer = 137
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnPosB Then
                        If mnPosB = mnINIT Then
                            lblMeterBPosition.Visible = True
                            lblMeterBPositionLabel.Visible = True
                            lblPistonB.Visible = True
                            lblCanBInner.Visible = True
                            lblCanBOuter.Visible = True
                            lblCylB.Visible = True
                            lblBInLine.Visible = True
                            lblBOutLine.Visible = True
                            lblSupplyLineB.Visible = True
                            lblTrigLineB.Visible = True
                            picValveBIn.Visible = True
                            picValveBOut.Visible = True
                            lblAIn.Visible = True
                            lblBIn.Visible = True
                            lblAOut.Visible = True
                            lblBOut.Visible = True
                        End If
                        mnPosB = nTmp
                        lblMeterBPosition.Text = nTmp.ToString & "%"
                        If nTmp < 0 Then
                            nTmp = 0
                        End If
                        If nTmp > 100 Then
                            nTmp = 100
                        End If
                        lblPistonB.Height = nMeter0Height + CInt(nTmp * (nMeter100Height - nMeter0Height) / 100)
                    End If
                Case "psMtrAVolumeLeft", "psMtrVolumeLeft"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnVolA Then
                        If mnVolA = mnINIT Then
                            lblMeterAVolume.Visible = True
                        End If
                        mnVolA = nTmp
                        lblMeterAVolume.Text = nTmp.ToString & "cc"
                    End If
                Case "psMtrBVolumeLeft"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnVolB Then
                        If mnVolB = mnINIT Then
                            lblMeterBVolume.Visible = True
                        End If
                        mnVolB = nTmp
                        lblMeterBVolume.Text = nTmp.ToString & "cc"
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
                Case "psMtrAPressure", "psMtrPressure"
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnMtrAPres Then
                        lblMtrADispensePressure.Text = nTmp.ToString
                        If mnMtrAPres = mnINIT Then
                            pnlMtrADispensePressure.Visible = True
                            lblMtrAPressureLine.Visible = True
                            lblMtrADispensePressureLabel.Visible = True
                        End If
                        mnMtrAPres = nTmp
                    End If
                Case "psMtrBPressure"
                        Dim nTmp As Integer = CType(oData.ToString, Integer)
                        If nTmp <> mnMtrBPres Then
                            lblMtrBDispensePressure.Text = nTmp.ToString
                        If mnMtrBPres = mnINIT Then
                            pnlMtrBDispensePressure.Visible = True
                            lblMtrBPressureLine.Visible = True
                            lblMtrBDispensePressureLabel.Visible = True
                        End If
                            mnMtrBPres = nTmp
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
                Case "psValveAIn", "psValveIn"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveAIn Then
                        mnValveAIn = nTmp
                        If nTmp > 0 Then
                            picValveAIn.Image = DirectCast(gpsRM.GetObject("Valve_L_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveAIn.Image = DirectCast(gpsRM.GetObject("Valve_L", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psValveBIn"
                        ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveBIn Then
                        If mnValveBOut = mnINIT Then
                            lblMeterBPosition.Visible = True
                            lblMeterBPositionLabel.Visible = True
                            lblPistonB.Visible = True
                            lblCanBInner.Visible = True
                            lblCanBOuter.Visible = True
                            lblCylB.Visible = True
                            lblBInLine.Visible = True
                            lblBOutLine.Visible = True
                            lblSupplyLineB.Visible = True
                            lblTrigLineB.Visible = True
                            picValveBIn.Visible = True
                            picValveBOut.Visible = True
                            lblAIn.Visible = True
                            lblBIn.Visible = True
                            lblAOut.Visible = True
                            lblBOut.Visible = True
                        End If
                        mnValveBIn = nTmp
                        If nTmp > 0 Then
                            picValveBIn.Image = DirectCast(gpsRM.GetObject("Valve_R_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveBIn.Image = DirectCast(gpsRM.GetObject("Valve_R", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psValveAOut", "psValveOut"
                    ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveAOut Then
                        mnValveAOut = nTmp
                        If nTmp > 0 Then
                            picValveAOut.Image = DirectCast(gpsRM.GetObject("Valve_R_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveAOut.Image = DirectCast(gpsRM.GetObject("Valve_R", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psValveBOut"
                        ''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnValveBOut Then
                        If mnValveBOut = mnINIT Then
                            lblMeterBPosition.Visible = True
                            lblMeterBPositionLabel.Visible = True
                            lblPistonB.Visible = True
                            lblCanBInner.Visible = True
                            lblCanBOuter.Visible = True
                            lblCylB.Visible = True
                            lblBInLine.Visible = True
                            lblBOutLine.Visible = True
                            lblSupplyLineB.Visible = True
                            lblTrigLineB.Visible = True
                            picValveBIn.Visible = True
                            picValveBOut.Visible = True
                            lblAIn.Visible = True
                            lblBIn.Visible = True
                            lblAOut.Visible = True
                            lblBOut.Visible = True
                        End If
                        mnValveBOut = nTmp
                        If nTmp > 0 Then
                            picValveBOut.Image = DirectCast(gpsRM.GetObject("Valve_L_On", mLanguage.FixedCulture), Image)
                        Else
                            picValveBOut.Image = DirectCast(gpsRM.GetObject("Valve_L", mLanguage.FixedCulture), Image)
                        End If
                        subUpdateLines()
                    End If
                Case "psMtrAOverTravel", "psMtrOverTravel"
                    '<CustomCBO>psOTPOS;4096;psOTNEG;8192;psOTBOTH;12288;psOTOK;0</CustomCBO>
                    '<CustomColors>Red;4096;Red;8192;Red;12288;Green;0</CustomColors>
                    Dim nTmp As Integer = CType(oData.ToString, Integer)
                    If nTmp <> mnOverTravelA Then
                        mnOverTravelA = nTmp
                        Select Case nTmp
                            Case 4096
                                lblOverTravelA1.Visible = True
                                lblOverTravelA2.Visible = False
                            Case 8192
                                lblOverTravelA1.Visible = False
                                lblOverTravelA2.Visible = True
                            Case 12288
                                lblOverTravelA1.Visible = True
                                lblOverTravelA2.Visible = True
                            Case 0
                                lblOverTravelA1.Visible = False
                                lblOverTravelA2.Visible = False
                            Case Else
                                mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: SetItem", itemConfig.Tag & ":  " & oData.ToString & " - Unexpected value")
                        End Select
                    End If
                Case "psMtrBOverTravel"
                        '<CustomCBO>psOTPOS;4096;psOTNEG;8192;psOTBOTH;12288;psOTOK;0</CustomCBO>
                        '<CustomColors>Red;4096;Red;8192;Red;12288;Green;0</CustomColors>
                        Dim nTmp As Integer = CType(oData.ToString, Integer)
                        If nTmp <> mnOverTravelB Then
                            mnOverTravelB = nTmp
                            Select Case nTmp
                                Case 4096
                                    lblOverTravelB1.Visible = True
                                    lblOverTravelB2.Visible = False
                                Case 8192
                                    lblOverTravelB1.Visible = False
                                    lblOverTravelB2.Visible = True
                                Case 12288
                                    lblOverTravelB1.Visible = True
                                    lblOverTravelB2.Visible = True
                                Case 0
                                    lblOverTravelB1.Visible = False
                                    lblOverTravelB2.Visible = False
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
            lblMtrADispensePressureLabel.Text = gpsRM.GetString("psDispPresCAP")
            lblMtrADispensePressureUnits.Text = gpsRM.GetString("pspsi")
            lblMtrBDispensePressureLabel.Text = gpsRM.GetString("psDispPresCAP")
            lblMtrBDispensePressureUnits.Text = gpsRM.GetString("pspsi")
            lblSupplyPressureLabel.Text = gpsRM.GetString("psSupplyPresCAP")
            lblSupplyPressureUnits.Text = gpsRM.GetString("pspsi")
            lblOverTravelA1.Text = gpsRM.GetString("psOvertravelCAP")
            lblOverTravelA2.Text = gpsRM.GetString("psOvertravelCAP")
            lblMeterAPositionLabel.Text = gpsRM.GetString("psMeterPosCAP")
            lblMeterBPositionLabel.Text = gpsRM.GetString("psMeterPosCAP")
            lblVolumeLabel.Text = gpsRM.GetString("psVolumeDispCAP")
            lblBIn.Text = gpsRM.GetString("psValveBCAP")
            lblAOut.Text = gpsRM.GetString("psValveACAP")
            lblAIn.Text = gpsRM.GetString("psValveACAP")
            lblBOut.Text = gpsRM.GetString("psValveBCAP")
            lblCylA.Text = gpsRM.GetString("psMeterACAP")
            lblCylB.Text = gpsRM.GetString("psMeterBCAP")
        Catch ex As Exception
            mDebug.WriteEventToLog("Module: " & msMODULE & ", Routine: Init", ex.Message)
        End Try
    End Sub
End Class

