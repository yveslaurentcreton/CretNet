using CretNet.Platform.Fluxor.Generators.Core;

namespace CretNet.Platform.Fluxor.Generators.Models;

internal class CnpEntityActionToGenerate
{
    public string NamespaceName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ShortClassName => ClassName.RemoveEnd("Action");
    public bool HasSuffix => ClassName.EndsWith("Action") && !ClassName.EndsWith(EntityClassName);
    public string ClassNameSuffix => HasSuffix ? "Action" : string.Empty;
    public string SuccessClassName => HasSuffix ? $"{ShortClassName}Success{ClassNameSuffix}" : $"{ClassName}Success";
    public string SuccessEventName => $"{ShortClassName}SuccessEvent";
    public string FailureClassName => HasSuffix ? $"{ShortClassName}Failure{ClassNameSuffix}" : $"{ClassName}Failure";
    public string FailureEventName => $"{ShortClassName}FailureEvent";
    public string? ActionLabelProperty { get; set; }
    public string? SuccessLabelProperty { get; set; }
    public string? FailureLabelProperty { get; set; }
    public bool HasPostActions { get; set; }
    public string? ResultNamespaceName { get; set; } = string.Empty;
    public string? ResultExpression { get; set; } = string.Empty;
    public string? EntityNamespaceName{ get; set; }
    public string EntityClassName { get; set; } = string.Empty;
    public EntityState EntityState { get; set; } = EntityState.Empty;
    public IDictionary<string, string> DependencyInjectionParameters { get; set; } = new Dictionary<string, string>();

    public bool IsPluralReturnType => ResultExpression?.StartsWith("IEnumerable<") == true;
        
    protected bool Equals(CnpEntityActionToGenerate other)
    {
        return NamespaceName == other.NamespaceName
               && ClassName == other.ClassName
               && ActionLabelProperty == other.ActionLabelProperty
               && SuccessLabelProperty == other.SuccessLabelProperty
               && FailureLabelProperty == other.FailureLabelProperty
               && HasPostActions == other.HasPostActions
               && ResultNamespaceName == other.ResultNamespaceName
               && ResultExpression == other.ResultExpression
               && EntityNamespaceName == other.EntityNamespaceName
               && EntityClassName == other.EntityClassName
               && EntityState.Equals(other.EntityState)
               && DependencyInjectionParameters.SequenceEqual(other.DependencyInjectionParameters);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CnpEntityActionToGenerate)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = NamespaceName.GetHashCode();
            hashCode = (hashCode * 397) ^ ClassName.GetHashCode();
            hashCode = (hashCode * 397) ^ (ActionLabelProperty != null ? ActionLabelProperty.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SuccessLabelProperty != null ? SuccessLabelProperty.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (FailureLabelProperty != null ? FailureLabelProperty.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ HasPostActions.GetHashCode();
            hashCode = (hashCode * 397) ^ (ResultNamespaceName != null ? ResultNamespaceName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ResultExpression != null ? ResultExpression.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EntityNamespaceName != null ? EntityNamespaceName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EntityClassName != null ? EntityClassName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EntityState != null ? EntityState.GetHashCode() : 0);
            return hashCode;
        }
    }
}