using CretNet.Platform.Fluxor.Generators.Core;

namespace CretNet.Platform.Fluxor.Generators.Models;

internal class CnpActionToGenerate
{
    protected bool Equals(CnpActionToGenerate other)
    {
        return NamespaceName == other.NamespaceName
               && ClassName == other.ClassName
               && HasEffect == other.HasEffect
               && State.Equals(other.State)
               && DependencyInjectionParameters.SequenceEqual(other.DependencyInjectionParameters);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CnpActionToGenerate)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = NamespaceName.GetHashCode();
            hashCode = (hashCode * 397) ^ ClassName.GetHashCode();
            hashCode = (hashCode * 397) ^ HasEffect.GetHashCode();
            hashCode = (hashCode * 397) ^ (State != null ? State.GetHashCode() : 0);
            return hashCode;
        }
    }

    public string NamespaceName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ShortClassName => ClassName.RemoveEnd("Action");
    public string PostEventName => $"{ShortClassName}PostEvent";
    public bool HasEffect { get; set; }
    public State State { get; set; } = State.Empty;
    public IDictionary<string, string> DependencyInjectionParameters { get; set; } = new Dictionary<string, string>();
}