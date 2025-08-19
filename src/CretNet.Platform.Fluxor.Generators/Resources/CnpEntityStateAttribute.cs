using System;

#nullable enable

namespace CretNet.Platform.Fluxor.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class CnpEntityStateAttribute : Attribute
{
    public Type StateType { get; }
    public string? EntityProperty { get; }
    public string? LoadingProperty { get; }
    public bool CustomReducer { get; }

    public CnpEntityStateAttribute(Type stateType, string? entityProperty = null, string? loadingProperty = null, bool customReducer = false)
    {
        StateType = stateType;
        EntityProperty = entityProperty;
        LoadingProperty = loadingProperty;
        CustomReducer = customReducer;
    }
    
    public CnpEntityStateAttribute(Type stateType, string? loadingProperty = null, bool customReducer = false)
        : this(stateType, null, loadingProperty, customReducer)
    {
    }
    
    public CnpEntityStateAttribute(Type stateType, bool customReducer = false)
        : this(stateType, null, null, customReducer)
    {
    }
}