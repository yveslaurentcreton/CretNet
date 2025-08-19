using System;

namespace CretNet.Platform.Fluxor.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class CnpEntityActionAttribute : Attribute
{
    public Type EntityType { get; }
    public bool HasPostActions { get; }
    public bool IsNullable { get; }

    public CnpEntityActionAttribute(Type entityType, bool hasPostActions = false, bool isNullable = false)
    {
        EntityType = entityType;
        HasPostActions = hasPostActions;
        IsNullable = isNullable;
    }
}