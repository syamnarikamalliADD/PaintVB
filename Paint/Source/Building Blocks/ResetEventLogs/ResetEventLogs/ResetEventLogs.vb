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
' Form/Module: ResetEventLogs
'
' Description: console app to clear out event logs
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                       Version
'    11/26/13   MSW     New                                                          4.01.06.01
'    04/03/14   MSW     Mark a version, split up folders for building block versions 4.01.07.00
'********************************************************************************************
Module ResetEventLogs

    Sub Main()
        Try

            EventLog.Delete("PAINTworks")
            EventLog.Delete("Application")
            Console.WriteLine("Complete")
        Catch ex As Exception
            Console.WriteLine("Error")
        End Try
        Console.ReadKey()
    End Sub


End Module
