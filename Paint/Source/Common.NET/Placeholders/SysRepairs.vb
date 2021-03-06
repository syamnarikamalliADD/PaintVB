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
' Form/Module: SysStyles.vb
'
' Description: Collection for styles and the class for a style
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: RickO
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    04/05/12   MSW     1st Version                                                   4.01.03.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Friend Class clsSysRepairPanels

    Inherits CollectionBase

    Friend Function Clone() As clsSysRepairPanels
        '********************************************************************************************
        'Description: this routine is to make a new collection without going thru the database
        '               load for each style it is attached to. the description is copied, the 
        '               robots required is set when data is actually loaded
        '
        'Parameters:  
        'Returns:      
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Return Nothing

    End Function
    Friend Sub New(ByRef oZone As clsZone)
    End Sub

End Class

Friend Class clsDegradeRepairPanels

    Inherits CollectionBase
    Friend Sub Update()
    End Sub
    Friend Sub New(ByRef oZone As clsZone)
    End Sub

End Class 'clsDegradeRepairPanels


