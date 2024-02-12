using System;
using System.Collections.Generic;
using UnityEngine;


public static class SoapTypeUtils
{
    private static readonly Dictionary<string, string> BuiltInTypes = new Dictionary<string, string>
    {
        { "byte", "System.Byte" },
        { "sbyte", "System.SByte" },
        { "char", "System.Char" },
        { "decimal", "System.Decimal" },
        { "double", "System.Double" },
        { "uint", "System.UInt32" },
        { "nint", "System.IntPtr" },
        { "nuint", "System.UIntPtr" },
        { "long", "System.Int64" },
        { "ulong", "System.UInt64" },
        { "short", "System.Int16" },
        { "ushort", "System.UInt16" },
        { "int", "System.Int32" },
        { "float", "System.Single" },
        { "string", "System.String" },
        { "object", "System.Object" },
        { "bool", "System.Boolean" }
    };
    
    public static bool IsBuiltInType(string typeName)
    {
        if (BuiltInTypes.TryGetValue(typeName, out var qualifiedName))
            typeName = qualifiedName;

        Type type = Type.GetType(typeName);
        if (type?.Namespace != null && type.Namespace.StartsWith("System"))
            return true;

        return false;
    }
    
    /// <summary>
    /// Checks if a type name is valid.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static bool IsTypeNameValid(string typeName)
    {
        var valid = System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(typeName);
        return valid;
    }
    
    /// <summary>
    /// Checks if a type is serializable or is serialized by Unity.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSerializable(Type type)
    {
        var isSerializable = false;
        isSerializable |= type.IsSerializable;
        isSerializable |= type.Namespace == "UnityEngine";
        isSerializable |= type.IsSubclassOf(typeof(MonoBehaviour));
        return isSerializable;
    }
}