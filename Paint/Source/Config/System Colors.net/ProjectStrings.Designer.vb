﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3620
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Class ProjectStrings
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("SystemColors.ProjectStrings", GetType(ProjectStrings).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        Friend Shared ReadOnly Property FormIcon() As System.Drawing.Icon
            Get
                Dim obj As Object = ResourceManager.GetObject("FormIcon", resourceCulture)
                Return CType(obj,System.Drawing.Icon)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to This color already exists, reverting to old value..
        '''</summary>
        Friend Shared ReadOnly Property psCOLOR_ALREADY_EXISTS() As String
            Get
                Return ResourceManager.GetString("psCOLOR_ALREADY_EXISTS", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Color Description.
        '''</summary>
        Friend Shared ReadOnly Property psCOLOR_DESC() As String
            Get
                Return ResourceManager.GetString("psCOLOR_DESC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Color doesn&apos;t exist.  The tricoat number should be the plant color number for the 2nd coat..
        '''</summary>
        Friend Shared ReadOnly Property psCOLOR_DOESNT_EXISTS() As String
            Get
                Return ResourceManager.GetString("psCOLOR_DOESNT_EXISTS", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Unable to verify SYSTEM COLOR TABLE - Check System colors.xml file.
        '''</summary>
        Friend Shared ReadOnly Property psERR_COLOR_TABLE() As String
            Get
                Return ResourceManager.GetString("psERR_COLOR_TABLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Unable to verify VALVE TABLE - Check System colors.xml file.
        '''</summary>
        Friend Shared ReadOnly Property psERR_VALVE_TABLE() As String
            Get
                Return ResourceManager.GetString("psERR_VALVE_TABLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Fanuc Color.
        '''</summary>
        Friend Shared ReadOnly Property psFANUCCOLOR() As String
            Get
                Return ResourceManager.GetString("psFANUCCOLOR", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Hardener Solvent.
        '''</summary>
        Friend Shared ReadOnly Property psHARD_SOLV_CAP() As String
            Get
                Return ResourceManager.GetString("psHARD_SOLV_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Hardener Ratio.
        '''</summary>
        Friend Shared ReadOnly Property psHARDENER_RATIO_CAP() As String
            Get
                Return ResourceManager.GetString("psHARDENER_RATIO_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Hardener Valve.
        '''</summary>
        Friend Shared ReadOnly Property psHARDENER_VALVE_CAP() As String
            Get
                Return ResourceManager.GetString("psHARDENER_VALVE_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Load Data From Database?.
        '''</summary>
        Friend Shared ReadOnly Property psLOAD_FROM_DATABASE() As String
            Get
                Return ResourceManager.GetString("psLOAD_FROM_DATABASE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Color.
        '''</summary>
        Friend Shared ReadOnly Property psPLANTCOLOR() As String
            Get
                Return ResourceManager.GetString("psPLANTCOLOR", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Resin Ratio.
        '''</summary>
        Friend Shared ReadOnly Property psRESIN_RATIO_CAP() As String
            Get
                Return ResourceManager.GetString("psRESIN_RATIO_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Resin Solvent.
        '''</summary>
        Friend Shared ReadOnly Property psRESIN_SOLV_CAP() As String
            Get
                Return ResourceManager.GetString("psRESIN_SOLV_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Above controller is not online, unable to save to this controller. Continue with save?.
        '''</summary>
        Friend Shared ReadOnly Property psROBOT_OFFLINE() As String
            Get
                Return ResourceManager.GetString("psROBOT_OFFLINE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Robots Required.
        '''</summary>
        Friend Shared ReadOnly Property psROBOTS_REQ() As String
            Get
                Return ResourceManager.GetString("psROBOTS_REQ", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Save Warning.
        '''</summary>
        Friend Shared ReadOnly Property psSAVE_WARNING() As String
            Get
                Return ResourceManager.GetString("psSAVE_WARNING", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to System Color Setup.
        '''</summary>
        Friend Shared ReadOnly Property psSCREENCAPTION() As String
            Get
                Return ResourceManager.GetString("psSCREENCAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Color \ Valve Setup.
        '''</summary>
        Friend Shared ReadOnly Property psTAB1CAPTION() As String
            Get
                Return ResourceManager.GetString("psTAB1CAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Robots Required.
        '''</summary>
        Friend Shared ReadOnly Property psTAB2CAPTION() As String
            Get
                Return ResourceManager.GetString("psTAB2CAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 2K Setup.
        '''</summary>
        Friend Shared ReadOnly Property psTAB3CAPTION() As String
            Get
                Return ResourceManager.GetString("psTAB3CAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tricoat Setup.
        '''</summary>
        Friend Shared ReadOnly Property psTAB4CAPTION() As String
            Get
                Return ResourceManager.GetString("psTAB4CAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tricoat Color Number.
        '''</summary>
        Friend Shared ReadOnly Property psTRICOATCOLOR() As String
            Get
                Return ResourceManager.GetString("psTRICOATCOLOR", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Using:.
        '''</summary>
        Friend Shared ReadOnly Property psUSING() As String
            Get
                Return ResourceManager.GetString("psUSING", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Valve.
        '''</summary>
        Friend Shared ReadOnly Property psVALVE() As String
            Get
                Return ResourceManager.GetString("psVALVE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Valve Description.
        '''</summary>
        Friend Shared ReadOnly Property psVALVE_DESC() As String
            Get
                Return ResourceManager.GetString("psVALVE_DESC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Valve Number.
        '''</summary>
        Friend Shared ReadOnly Property psVALVE_NUMBER() As String
            Get
                Return ResourceManager.GetString("psVALVE_NUMBER", resourceCulture)
            End Get
        End Property
    End Class
End Namespace