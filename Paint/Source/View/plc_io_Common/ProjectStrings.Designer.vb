﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.5446
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
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("PLC_IO.ProjectStrings", GetType(ProjectStrings).Assembly)
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
        '''  Looks up a localized string similar to Building Rack View - .
        '''</summary>
        Friend Shared ReadOnly Property psBUILD_RACK() As String
            Get
                Return ResourceManager.GetString("psBUILD_RACK", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to PLC I/O Monitor.
        '''</summary>
        Friend Shared ReadOnly Property psSCREENCAPTION() As String
            Get
                Return ResourceManager.GetString("psSCREENCAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Select I/O Rack to View.
        '''</summary>
        Friend Shared ReadOnly Property psSELECT_RACK() As String
            Get
                Return ResourceManager.GetString("psSELECT_RACK", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Add this item to the Watch Window?.
        '''</summary>
        Friend Shared ReadOnly Property psWW_ADD_MSG() As String
            Get
                Return ResourceManager.GetString("psWW_ADD_MSG", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Watch Window.
        '''</summary>
        Friend Shared ReadOnly Property psWW_CAPTION() As String
            Get
                Return ResourceManager.GetString("psWW_CAPTION", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Remove this item from the Watch Window?.
        '''</summary>
        Friend Shared ReadOnly Property psWW_REMOVE_MSG() As String
            Get
                Return ResourceManager.GetString("psWW_REMOVE_MSG", resourceCulture)
            End Get
        End Property
        
        Friend Shared ReadOnly Property WCloseButton() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("WCloseButton", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        Friend Shared ReadOnly Property WebIcon() As System.Drawing.Icon
            Get
                Dim obj As Object = ResourceManager.GetObject("WebIcon", resourceCulture)
                Return CType(obj,System.Drawing.Icon)
            End Get
        End Property
        
        Friend Shared ReadOnly Property WWIcon() As System.Drawing.Icon
            Get
                Dim obj As Object = ResourceManager.GetObject("WWIcon", resourceCulture)
                Return CType(obj,System.Drawing.Icon)
            End Get
        End Property
    End Class
End Namespace
