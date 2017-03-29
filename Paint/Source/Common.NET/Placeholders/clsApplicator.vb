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
' Form/Module: clsApplicator- placeholder
'
' Description: !! This is just a place holder - Robot.vb needs this if it's not using the applicator
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    01/19/12   MSW     1st                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Friend Class clsApplicator
    Friend ReadOnly Property Changed() As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Sub Update()

    End Sub
    Friend Sub New(ByVal void As clsArm)
        MyBase.New()
    End Sub

 

    'nColDetails(nCol - 1).nMax = oApplicator.MaxEngUnit(nCol - 1)
    'nColDetails(nCol - 1).nMin = oApplicator.MinEngUnit(nCol - 1)
    'nColDetails(nCol - 1).sLblName = oApplicator.ParamName(nCol - 1)
    'nColDetails(nCol - 1).sLblCap = oApplicator.ParamNameCAP(nCol - 1)
    'nColDetails(nCol - 1).sUnits = oApplicator.ParamUnits(nCol - 1)
    Friend ReadOnly Property MaxEngUnit(ByVal index As Integer) As Integer
        Get
            Return 0
        End Get
    End Property
    Friend ReadOnly Property MinEngUnit(ByVal index As Integer) As Integer
        Get
            Return 0
        End Get
    End Property
    Friend ReadOnly Property ParamName(ByVal index As Integer) As String
        Get
            Return String.Empty
        End Get
    End Property
    Friend ReadOnly Property ParamNameCAP(ByVal index As Integer) As String
        Get
            Return String.Empty
        End Get
    End Property
    Friend ReadOnly Property ParamUnits(ByVal index As Integer) As String
        Get
            Return String.Empty
        End Get
    End Property
End Class
