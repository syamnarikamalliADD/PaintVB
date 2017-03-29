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
' Form/Module: mRegVal
' Description: Routines to read and write Robot Registers
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 09/30/10      RJO     Needed a way to get around late binding to read/write Robot   4.00.00.00
'                       registers. Option Strict Must be Off to do this.
' 06/24/13      MSW     Added a version of each routine to pass individual robot      4.00.05.00
'                       register items instead of the whole group
'***************************************************************************************************

Option Compare Text
Option Explicit On
Option Strict Off

Module mRegVal

    Friend Function ReadNumReg(ByRef RegVars As FRRobot.FRCVars, ByVal RegNum As Integer) As String
        '********************************************************************************************
        'Description: Return the value of a Robot Numeric Register
        '
        'Parameters: Robot RegVars, Register number to read  
        'Returns: Value of Robot Numeric Register    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oVar As FRRobot.FRCRegNumeric = RegVars.Item(RegNum).Value
        Return ReadNumReg(oVar)

    End Function

    Friend Function ReadNumReg(ByRef oVar As FRRobot.FRCRegNumeric) As String
        '********************************************************************************************
        'Description: Return the value of a Robot Numeric Register
        '
        'Parameters: Robot register object
        'Returns: Value of Robot Numeric Register    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sVal As String = String.Empty

        If oVar.Type = FRRobot.FRETypeCodeConstants.frRealType Then
            'floating point
            sVal = oVar.RegFloat.ToString
        Else
            'integer
            sVal = oVar.RegLong.ToString
        End If

        Return sVal

    End Function

    Friend Function ReadStringReg(ByRef RegVars As FRRobot.FRCVars, ByVal RegNum As Integer) As String
        '********************************************************************************************
        'Description: Return the value of a Robot String Register
        '
        'Parameters: Robot RegVars, Register number to read  
        'Returns: Value of Robot String Register    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oVar As FRRobot.FRCRegString = RegVars.Item(RegNum).Value

        ReadStringReg = oVar.Value

    End Function

    Friend Function ReadStringReg(ByRef oVar As FRRobot.FRCRegString) As String
        '********************************************************************************************
        'Description: Return the value of a Robot String Register
        '
        'Parameters: Robot register object
        'Returns: Value of Robot String Register    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ReadStringReg = oVar.Value

    End Function

    Friend Function WriteNumReg(ByRef RegVars As FRRobot.FRCVars, ByVal RegNum As Integer, _
                                ByVal Value As String) As Boolean
        '********************************************************************************************
        'Description:  Write Value to a Robot Numeric Register
        '
        'Parameters: Robot register object, Value to write 
        'Returns: True if success    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oVar As FRRobot.FRCRegNumeric = RegVars.Item(RegNum).Value

        Return WriteNumReg(oVar, Value)

    End Function
    Friend Function WriteNumReg(ByRef oVar As FRRobot.FRCRegNumeric, _
                                ByVal Value As String) As Boolean
        '********************************************************************************************
        'Description:  Write Value to a Robot Numeric Register
        '
        'Parameters: Robot RegVars, Register number to write, Value to write 
        'Returns: True if success    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean = True

        Try
            If oVar.Type = FRRobot.FRETypeCodeConstants.frRealType Then
                'floating point
                oVar.RegFloat = CType(Value, Single)
            Else
                'integer
                oVar.RegLong = CType(Value, Integer)
            End If

        Catch ex As Exception
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Friend Function WriteStringReg(ByRef RegVars As FRRobot.FRCVars, ByVal RegNum As Integer, _
                            ByVal Value As String) As Boolean
        '********************************************************************************************
        'Description:  Write Value to a Robot String Register
        '
        'Parameters: Robot register object, Value to write 
        'Returns: True if success    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oVar As FRRobot.FRCRegString = RegVars.Item(RegNum).Value

        Return WriteStringReg(oVar, Value)

    End Function
    Friend Function WriteStringReg(ByVal oVar As FRRobot.FRCRegString, _
                            ByVal Value As String) As Boolean
        '********************************************************************************************
        'Description:  Write Value to a Robot String Register
        '
        'Parameters: Robot RegVars, Register number to write, Value to write 
        'Returns: True if success    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean = True

        Try
            oVar.Value = Value

        Catch ex As Exception
            bSuccess = False
        End Try

        Return bSuccess

    End Function

End Module
