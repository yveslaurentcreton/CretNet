using System;

namespace CretNet.Platform.Fluxor.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class CnpStateAttribute : Attribute
{
    public Type StateType { get; }
    public bool CustomReducer { get; set; }

    public CnpStateAttribute(Type stateType, bool customReducer = false)
    {
        StateType = stateType;
        CustomReducer = customReducer;
    }
}