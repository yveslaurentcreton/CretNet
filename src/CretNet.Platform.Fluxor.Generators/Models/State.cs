using CretNet.Platform.Fluxor.Generators.Core;
using Microsoft.CodeAnalysis;

namespace CretNet.Platform.Fluxor.Generators.Models;

public class State
{
    public static State Empty => new();
    
    public string? NamespaceName{ get; set; }
    public string? ClassName { get; set; }
    public bool GenerateCustomReducer { get; set; }

    internal static State GetState(SemanticModel semanticModel, INamedTypeSymbol? classSymbol)
    {
        var argument = Argument.GetArgument(semanticModel, classSymbol, "CretNet.Platform.Fluxor.Generators.CnpStateAttribute");
        var parameters = argument?.Parameters.ToList();
        var type = parameters?.FirstOrDefault(x => x.Name.Equals("stateType"))?.Value;
        var typeSymbol = type is null ? null : semanticModel.Compilation.GetTypeByMetadataName(type);
        var namespaceName = typeSymbol?.ContainingNamespace.ToDisplayString();
        var className = typeSymbol?.Name;
        var customReducerString = parameters?.FirstOrDefault(x => x.Name.Equals("customReducer"))?.Value;
        var customReducer = customReducerString is not null && bool.TryParse(customReducerString, out var stateCustomReducerResult) && stateCustomReducerResult;
        
        return new State
        {
            NamespaceName = namespaceName,
            ClassName = className,
            GenerateCustomReducer = customReducer
        };
    }
    
    internal string GetReducerSource()
    {
        if (ClassName is null)
            return string.Empty;
        
        var customReducerSource = GenerateCustomReducer ? "\n        newState = action.Reduce(newState);\n" : string.Empty;
        
        var reducersSource = Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpAction_Reducers.cs");
        reducersSource = reducersSource.Replace("[[CustomReducerSource]]", customReducerSource);
        
        return $"\n{reducersSource}\n";
    }

    protected bool Equals(State other)
    {
        return NamespaceName == other.NamespaceName
               && ClassName == other.ClassName
               && GenerateCustomReducer == other.GenerateCustomReducer;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((State)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (NamespaceName != null ? NamespaceName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ GenerateCustomReducer.GetHashCode();
            return hashCode;
        }
    }
}