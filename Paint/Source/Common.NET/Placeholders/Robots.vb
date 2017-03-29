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
' Form/Module: Robots.vb
'
' Description: Placeholder version
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
'    09/13/13   MSW     New                                                           4.01.05.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

#Region " Namespace "
#End Region
'********************************************************************************************
'Description: Preset Collection
'
'
'Modification history:
'
' Date      By      Reason
'******************************************************************************************** 
Friend Class clsController
    Friend ReadOnly Property Name() As String
         Get
            return string.empty
        End Get
    End Property
    Friend Sub New()
    End Sub

  
	End class

Friend Class clsControllers

    Inherits CollectionBase
    Default Friend Overloads Property Item(ByVal index As Integer) As clsController
        '********************************************************************************************
        'Description: Get or set a robot by its index
        '
        'Parameters: index
        'Returns:    clsArm
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(List(index), clsController)
        End Get
        Set(ByVal Value As clsController)
            List(index) = Value
        End Set
    End Property

    Friend Sub New()
  End Sub

End Class
'********************************************************************************************
