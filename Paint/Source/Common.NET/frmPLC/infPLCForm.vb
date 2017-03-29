' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2010
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsNoPLCComm
'
' Description: Class to use for PaintWorks SA (No PLC)
' 
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: HGB
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'********************************************************************************************
'    09/30/13   MSW     Move PLC comm to a DLL                                     4.01.06.00
'********************************************************************************************
Public Interface IPLCComm
    '******* Events ****************************************************************************
    'when hotlink has new data 
    Event NewData(ByVal ZoneName As String, ByVal TagName As String, _
                         ByVal Data() As String)
    'Common Routines Error Event - keep format same for all routines. Raise event for errors
    ' to main application and let it figure what it wants to do with it.
    Event ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                             ByVal sModule As String, ByVal AdditionalInfo As String)
    '******* End Events ************************************************************************

    '******* Properties ****************************************************************************
    ReadOnly Property DefaultGroupName() As String
    Property PLCData() As String()
    Property PLCType() As ePLCType
    ReadOnly Property MaxDataPerReadWrite() As Integer
    Property TagName() As String
    ReadOnly Property XMLPath() As String
    Property RemotePath() As String
    Property ZoneName() As String
    '******* End Properties ****************************************************************************

    '******* Methods ****************************************************************************
    Function AreaNum(ByVal Area As String) As Integer
    Function FormatFromTimerValue(ByVal ValueIn As Integer, Optional ByVal sFormat As String = "0.000") As String
    Function FormatToTimerValue(ByVal ValueIn As Integer) As String
    Sub RemoveAllHotLinks()
    Sub RemoveHotLink(ByVal Tag As String, ByVal Zone As String)
    Function MemType(ByVal Type As String) As String
    Function TagParams(ByVal TagName As String) As String
    '******* End Methods ****************************************************************************

End Interface
