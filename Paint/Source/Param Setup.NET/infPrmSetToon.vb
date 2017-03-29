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
' Form/Module: param setup screen ToonUctrl for dispense status
'
' Description: Interface template for ToonUctrl
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
Module infPrmSetToon

    Public Interface PrmSetToon
        Sub SetItem(ByVal itemConfig As frmMain.tItemCfg, ByRef oData As Object)
        Sub Init()
    End Interface

End Module
