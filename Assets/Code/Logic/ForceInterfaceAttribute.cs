using System;
using UnityEngine;

/// <summary>
/// Use ForceInterfaceAttribute with Object, Monobehaviour, GameObject or Component field types only);
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ForceInterfaceAttribute : PropertyAttribute
{
    public readonly Type InterfaceType;

    public ForceInterfaceAttribute(Type interfaceType)
    {
        InterfaceType = interfaceType;
    }
}
