using System;

namespace CretNet.Platform.Fluxor.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class CnpActionAttribute : Attribute
{
    public bool HasEffect { get; }
    
    public CnpActionAttribute(bool hasEffect = false)
    {
        HasEffect = hasEffect;
    }
}