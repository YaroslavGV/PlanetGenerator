using System;
using UnityEngine;

/// <summary>
/// An attribute used to validate an object, such as a ScriptableObject, 
/// Component, or other UnityEngine.Object, to implement an interface, including array and list.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class RequireInterfaceAttribute : PropertyAttribute
{
    [Range(0, 10)]
    public readonly Type type;

    public RequireInterfaceAttribute (Type value)
    {
        if (value.IsInterface == false)
            throw new Exception("Type must be an interface!");
        type = value;
    }
}
