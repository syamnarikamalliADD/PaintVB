﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.4963
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
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Job_Setup.ProjectStrings", GetType(ProjectStrings).Assembly)
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
        '''  Looks up a localized string similar to Conveyor Photoeye ID Position.
        '''</summary>
        Friend Shared ReadOnly Property psConveyorIDPosition() As String
            Get
                Return ResourceManager.GetString("psConveyorIDPosition", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Copy.
        '''</summary>
        Friend Shared ReadOnly Property psCOPY() As String
            Get
                Return ResourceManager.GetString("psCOPY", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Degrade.
        '''</summary>
        Friend Shared ReadOnly Property psDEGRADE() As String
            Get
                Return ResourceManager.GetString("psDEGRADE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to down degrade number .
        '''</summary>
        Friend Shared ReadOnly Property psDEGRADE_CHGLOG() As String
            Get
                Return ResourceManager.GetString("psDEGRADE_CHGLOG", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Degrade Number.
        '''</summary>
        Friend Shared ReadOnly Property psDEGRADE_NUMBER_CAP() As String
            Get
                Return ResourceManager.GetString("psDEGRADE_NUMBER_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Description.
        '''</summary>
        Friend Shared ReadOnly Property psDESC_CAP() As String
            Get
                Return ResourceManager.GetString("psDESC_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Option must be unique. Returning to original value..
        '''</summary>
        Friend Shared ReadOnly Property psDUPLICATE_OPTION() As String
            Get
                Return ResourceManager.GetString("psDUPLICATE_OPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Style must be unique. Returning to original value..
        '''</summary>
        Friend Shared ReadOnly Property psDUPLICATE_STYLE() As String
            Get
                Return ResourceManager.GetString("psDUPLICATE_STYLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Style/Option combination must be unique. Returning to original value..
        '''</summary>
        Friend Shared ReadOnly Property psDUPLICATE_STYLE_OPT() As String
            Get
                Return ResourceManager.GetString("psDUPLICATE_STYLE_OPT", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Entrance Start Counts.
        '''</summary>
        Friend Shared ReadOnly Property psENT_START_CAP() As String
            Get
                Return ResourceManager.GetString("psENT_START_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Could Not Validate System Repair Panels Table.
        '''</summary>
        Friend Shared ReadOnly Property psERR_REPAIRS_TABLE() As String
            Get
                Return ResourceManager.GetString("psERR_REPAIRS_TABLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Could Not Validate System Styles Table.
        '''</summary>
        Friend Shared ReadOnly Property psERR_STYLES_TABLE() As String
            Get
                Return ResourceManager.GetString("psERR_STYLES_TABLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Exit Start Counts.
        '''</summary>
        Friend Shared ReadOnly Property psEXIT_START_CAP() As String
            Get
                Return ResourceManager.GetString("psEXIT_START_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Fanuc Option.
        '''</summary>
        Friend Shared ReadOnly Property psFANUC_OPTION_CAP() As String
            Get
                Return ResourceManager.GetString("psFANUC_OPTION_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to FANUC Style.
        '''</summary>
        Friend Shared ReadOnly Property psFANUC_STYLE_CAP() As String
            Get
                Return ResourceManager.GetString("psFANUC_STYLE_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Item.
        '''</summary>
        Friend Shared ReadOnly Property psITEM_CAP() As String
            Get
                Return ResourceManager.GetString("psITEM_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Mute Length Counts.
        '''</summary>
        Friend Shared ReadOnly Property psMUTE_LEN_CAP() As String
            Get
                Return ResourceManager.GetString("psMUTE_LEN_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Option.
        '''</summary>
        Friend Shared ReadOnly Property psOPTION() As String
            Get
                Return ResourceManager.GetString("psOPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Page.
        '''</summary>
        Friend Shared ReadOnly Property psPAGE_CAP() As String
            Get
                Return ResourceManager.GetString("psPAGE_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Panel.
        '''</summary>
        Friend Shared ReadOnly Property psPANEL() As String
            Get
                Return ResourceManager.GetString("psPANEL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Panel.
        '''</summary>
        Friend Shared ReadOnly Property psPANEL_CAP() As String
            Get
                Return ResourceManager.GetString("psPANEL_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Photoeye Pattern.
        '''</summary>
        Friend Shared ReadOnly Property psPhotoeyePattern() As String
            Get
                Return ResourceManager.GetString("psPhotoeyePattern", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Option.
        '''</summary>
        Friend Shared ReadOnly Property psPLANT_OPTION_CAP() As String
            Get
                Return ResourceManager.GetString("psPLANT_OPTION_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Plant Style.
        '''</summary>
        Friend Shared ReadOnly Property psPLANT_STYLE_CAP() As String
            Get
                Return ResourceManager.GetString("psPLANT_STYLE_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Register Init.
        '''</summary>
        Friend Shared ReadOnly Property psREGISTER_INIT_CAP() As String
            Get
                Return ResourceManager.GetString("psREGISTER_INIT_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to repair panel.
        '''</summary>
        Friend Shared ReadOnly Property psREPAIR_PANEL() As String
            Get
                Return ResourceManager.GetString("psREPAIR_PANEL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Database .
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_DB() As String
            Get
                Return ResourceManager.GetString("psRESTORE_DB", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Data Restored From: .
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_FROM() As String
            Get
                Return ResourceManager.GetString("psRESTORE_FROM", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to This will load data from: .
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_MSG() As String
            Get
                Return ResourceManager.GetString("psRESTORE_MSG", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to to the screen. You can then review and.
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_MSG1() As String
            Get
                Return ResourceManager.GetString("psRESTORE_MSG1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to save the data. Do you want to continue?.
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_MSG2() As String
            Get
                Return ResourceManager.GetString("psRESTORE_MSG2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to PLC.
        '''</summary>
        Friend Shared ReadOnly Property psRESTORE_PLC() As String
            Get
                Return ResourceManager.GetString("psRESTORE_PLC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Robot Required For Panel.
        '''</summary>
        Friend Shared ReadOnly Property psROB_REQ_FOR_PANEL() As String
            Get
                Return ResourceManager.GetString("psROB_REQ_FOR_PANEL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Robot Down.
        '''</summary>
        Friend Shared ReadOnly Property psROBOT_DOWN_CAP() As String
            Get
                Return ResourceManager.GetString("psROBOT_DOWN_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Robots Required.
        '''</summary>
        Friend Shared ReadOnly Property psROBS_REQD_CAP() As String
            Get
                Return ResourceManager.GetString("psROBS_REQD_CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Job Setup.
        '''</summary>
        Friend Shared ReadOnly Property psSCREENCAPTION() As String
            Get
                Return ResourceManager.GetString("psSCREENCAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Style.
        '''</summary>
        Friend Shared ReadOnly Property psSTYLE() As String
            Get
                Return ResourceManager.GetString("psSTYLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to System Style.
        '''</summary>
        Friend Shared ReadOnly Property psSYS_STYLE() As String
            Get
                Return ResourceManager.GetString("psSYS_STYLE", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Style Numbers.
        '''</summary>
        Friend Shared ReadOnly Property psTAB1CAP() As String
            Get
                Return ResourceManager.GetString("psTAB1CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Option Numbers.
        '''</summary>
        Friend Shared ReadOnly Property psTAB2CAP() As String
            Get
                Return ResourceManager.GetString("psTAB2CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Repair Panels.
        '''</summary>
        Friend Shared ReadOnly Property psTAB3CAP() As String
            Get
                Return ResourceManager.GetString("psTAB3CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Degrade Numbers.
        '''</summary>
        Friend Shared ReadOnly Property psTAB4CAP() As String
            Get
                Return ResourceManager.GetString("psTAB4CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to PEC Intrusion.
        '''</summary>
        Friend Shared ReadOnly Property psTAB5CAP() As String
            Get
                Return ResourceManager.GetString("psTAB5CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Style ID.
        '''</summary>
        Friend Shared ReadOnly Property psTAB6CAP() As String
            Get
                Return ResourceManager.GetString("psTAB6CAP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Byte Me.
        '''</summary>
        Friend Shared ReadOnly Property psTest() As String
            Get
                Return ResourceManager.GetString("psTest", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Refill Required.
        '''</summary>
        Friend Shared ReadOnly Property psTWO_COATS_CAP() As String
            Get
                Return ResourceManager.GetString("psTWO_COATS_CAP", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
