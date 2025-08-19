using CretNet.Platform.Fluxor.Generators.Core;
using Microsoft.CodeAnalysis;

namespace CretNet.Platform.Fluxor.Generators.Models;

public class EntityState
{
    public static EntityState Empty => new();
    
    public string? NamespaceName{ get; set; }
    public string? ClassName { get; set; }
    public string? EntityProperty { get; set; }
    public string? LoadingProperty { get; set; }
    public bool GenerateCustomReducer { get; set; }

    internal static EntityState GetEntityState(SemanticModel semanticModel, INamedTypeSymbol? classSymbol)
    {
        var argument = Argument.GetArgument(semanticModel, classSymbol, "CretNet.Platform.Fluxor.Generators.CnpEntityStateAttribute");
        var parameters = argument?.Parameters.ToList();
        var type = parameters?.FirstOrDefault(x => x.Name.Equals("stateType"))?.Value;
        var typeSymbol = type is null ? null : semanticModel.Compilation.GetTypeByMetadataName(type);
        var namespaceName = typeSymbol?.ContainingNamespace.ToDisplayString();
        var className = typeSymbol?.Name;
        var entityProperty = parameters?.FirstOrDefault(x => x.Name.Equals("entityProperty"))?.Value;
        var loadingProperty = parameters?.FirstOrDefault(x => x.Name.Equals("loadingProperty"))?.Value;
        var customReducerString = parameters?.FirstOrDefault(x => x.Name.Equals("customReducer"))?.Value;
        var customReducer = customReducerString is not null && bool.TryParse(customReducerString, out var stateCustomReducerResult) && stateCustomReducerResult;
        
        return new EntityState
        {
            NamespaceName = namespaceName,
            ClassName = className,
            EntityProperty = entityProperty,
            LoadingProperty = loadingProperty,
            GenerateCustomReducer = customReducer
        };
    }
    
    internal string GetReducerSource(string entityType, bool isPluralEntityType)
    {
        if (ClassName is null)
            return string.Empty;
        
        var actionReducerProperties = new List<string>();
        var successReducerProperties = new List<string>();
        var failureReducerProperties = new List<string>();
        
        if (EntityProperty is not null)
        {
            var defaultEntityValue = isPluralEntityType ? $"Enumerable.Empty<{entityType}>()" : "default";
            
            actionReducerProperties.Add($"{EntityProperty} = {defaultEntityValue}");
            successReducerProperties.Add($"{EntityProperty} = action.Entity");
        }

        if (!string.IsNullOrEmpty(LoadingProperty))
        {
            actionReducerProperties.Add($"{LoadingProperty} = true");
            successReducerProperties.Add($"{LoadingProperty} = false");
            failureReducerProperties.Add($"{LoadingProperty} = false");
        }

        var actionReducerSource = string.Join(",\n            ", actionReducerProperties);
        var successReducerSource = string.Join(",\n            ", successReducerProperties);
        var failureReducerSource = string.Join(",\n            ", failureReducerProperties);
        var customReducerSource = GenerateCustomReducer ? "\n        newState = action.SourceAction.Reduce(newState, action);\n" : string.Empty;
        
        var reducersSource = Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpEntityAction_Reducers.cs");
        reducersSource = reducersSource.Replace("[[ActionReducerSource]]", actionReducerSource);
        reducersSource = reducersSource.Replace("[[SuccessReducerSource]]", successReducerSource);
        reducersSource = reducersSource.Replace("[[CustomReducerSource]]", customReducerSource);
        reducersSource = reducersSource.Replace("[[FailureReducerSource]]", failureReducerSource);
        
        return $"\n{reducersSource}\n";
    }
    
    protected bool Equals(EntityState other)
    {
        return NamespaceName == other.NamespaceName
               && ClassName == other.ClassName
               && EntityProperty == other.EntityProperty
               && LoadingProperty == other.LoadingProperty
               && GenerateCustomReducer == other.GenerateCustomReducer;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((EntityState)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (NamespaceName != null ? NamespaceName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EntityProperty != null ? EntityProperty.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LoadingProperty != null ? LoadingProperty.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ GenerateCustomReducer.GetHashCode();
            return hashCode;
        }
    }
}