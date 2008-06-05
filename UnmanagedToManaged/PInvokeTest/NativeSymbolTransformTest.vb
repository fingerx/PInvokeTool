﻿' Copyright (c) Microsoft Corporation.  All rights reserved.
'The following code was generated by Microsoft Visual Studio 2005.
'The test owner should check each test for validity.
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports PInvoke
Imports PInvoke.Transform

'''<summary>
'''This is a test class for PInvoke.NativeSymbolTransform and is intended
'''to contain all PInvoke.NativeSymbolTransform Unit Tests
'''</summary>
<TestClass()> _
Public Class NativeSymbolTransformTest

    Private Function Print(ByVal ns As NativeSymbol) As String
        If ns Is Nothing Then
            Return "<Nothing>"
        End If

        Dim str As String = ns.Name
        For Each child As NativeSymbol In ns.GetChildren()
            str &= "(" & Print(child) & ")"
        Next

        Return str
    End Function

    Private Sub VerifyTree(ByVal ns As NativeSymbol, ByVal str As String)
        Dim realStr As String = Print(ns)
        Assert.AreEqual(str, realStr)
    End Sub

    Private testContextInstance As TestContext

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property

    <TestMethod()> _
    Public Sub CollapseNamed()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeNamedType( _
                "char", _
                New NativeBuiltinType(BuiltinType.NativeChar))))
        VerifyTree(s1, "s1(m1(char(char)))")

        Dim transform As New NativeSymbolTransform
        transform.CollapseNamedTypes(s1)
        VerifyTree(s1, "s1(m1(char))")
    End Sub

    ''' <summary>
    ''' Collapsing a null type shouldn't do anything
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub CallapseNamed2()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeNamedType("char")))
        VerifyTree(s1, "s1(m1(char))")

        Dim transform As New NativeSymbolTransform
        transform.CollapseNamedTypes(s1)
        VerifyTree(s1, "s1(m1(char))")
    End Sub

    <TestMethod()> _
    Public Sub CollapseTypedef1()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeTypeDef( _
                "PCHAR", _
                New NativePointer(New NativeBuiltinType(BuiltinType.NativeChar)))))
        VerifyTree(s1, "s1(m1(PCHAR(*(char))))")

        Dim transform As New NativeSymbolTransform
        transform.CollapseTypedefs(s1)
        VerifyTree(s1, "s1(m1(*(char)))")
    End Sub

    ''' <summary>
    ''' Collapsing a null typedef shouldn't do anything
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub CollapseTypedef2()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeTypeDef("foo")))
        VerifyTree(s1, "s1(m1(foo))")

        Dim transform As New NativeSymbolTransform
        transform.CollapseNamedTypes(s1)
        VerifyTree(s1, "s1(m1(foo))")
    End Sub

    ''' <summary>
    ''' Renaming a type is fun
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub TypeRename1()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeBuiltinType(BuiltinType.NativeChar)))

        Dim transform As New NativeSymbolTransform()
        transform.RenameTypeSymbol(s1, "s1", "s2")
        VerifyTree(s1, "s2(m1(char))")
    End Sub

    ''' <summary>
    ''' Rename through a typedef
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub TypeRename2()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeBuiltinType(BuiltinType.NativeChar)))
        Dim td As New NativeTypeDef("foo", s1)

        Dim transform As New NativeSymbolTransform()
        transform.RenameTypeSymbol(td, "s1", "s2")
        VerifyTree(td, "foo(s2(m1(char)))")
    End Sub

    ''' <summary>
    ''' Don't rename members
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub TypeRename3()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "s1", _
            New NativeBuiltinType(BuiltinType.NativeChar)))

        Dim transform As New NativeSymbolTransform()
        transform.RenameTypeSymbol(s1, "s1", "s2")
        VerifyTree(s1, "s2(s1(char))")
    End Sub

    ''' <summary>
    ''' Make sure named types get renamed as well
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub TypeRename4()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember( _
            "m1", _
            New NativeNamedType("s1", New NativeBuiltinType(BuiltinType.NativeByte))))

        Dim transform As New NativeSymbolTransform()
        transform.RenameTypeSymbol(s1, "s1", "s2")
        VerifyTree(s1, "s2(m1(s2(byte)))")
    End Sub

End Class
