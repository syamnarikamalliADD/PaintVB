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
' Form/Module: CCToonUctrl
'
' Description: Interface template for CC user controls
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: 
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Public Interface CCToonUctrl
    Sub UpdateCartoons()
    Property ShowFeedBackLabels() As Boolean
    Property SharedValveStates() As Integer
    Property GroupValveStates() As Integer
    Property PaintColorName() As String
    Property SolvHeaderLabel() As String
    Property AirHeaderLabel() As String
    Sub SetSharedValveLabels(ByRef sNames As String())
    Sub SetGroupValveLabels(ByRef sNames As String())
    Sub SetAdditionalData(ByRef sData As String())
    Event GroupValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean)
    Event SharedValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean)
End Interface
