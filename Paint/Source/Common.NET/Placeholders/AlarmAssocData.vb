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
' Form/Module: AlarmAssocData.vb
'
' Description: Placeholder for above class when not needed
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    09/05/07   gks     changed properties to shared on advice of fxCop
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports FRRobot

Friend Class clsAlarmAssocData
#Region " Declares "

#End Region
#Region " Properties "

    Friend Property InitSuccess() As Boolean
        '*****************************************************************************************
        'Description: Feedback to the parent
        '
        'Parameters: 
        'Returns:       True if we hooked up successfully, False if not
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
        End Set

    End Property
    Friend Shared Property Style() As String
        '*****************************************************************************************
        'Description: Current Style from Robot Controller
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set
    End Property
    Friend Shared Property Color() As String
        '*****************************************************************************************
        'Description: Current Color from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set
    End Property
    Friend Shared Property Valve() As String
        '*****************************************************************************************
        'Description: Current Color Valve from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set

    End Property
    Friend Shared Property JobName() As String
        '*****************************************************************************************
        'Description: Current Job Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set

    End Property
    Friend Shared Property Process() As String
        '*****************************************************************************************
        'Description: Current Process Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set
    End Property
    Friend Shared Property Node() As Integer
        '*****************************************************************************************
        'Description: Current Process Name from Robot Controller
        '
        'Parameters: Robot
        'Returns: None 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return 0
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property

#End Region
#Region " Routines "
 
#End Region
#Region " Events "
    Friend Sub New(ByRef oRobot As FRCRobot, ByVal XMLPath As String, ByVal ArmNumber As Integer, _
                   ByVal IsOpener As Boolean)
        '*****************************************************************************************
        'Description: class constructor
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        MyBase.New()
        'fxCop correction
        Dim void As FRCRobot = oRobot
    End Sub
   
#End Region
End Class
