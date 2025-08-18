using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CretNet.Platform.Fluxor.Generators.Models;

public class Argument
{
    public string ParentType { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    
    public IEnumerable<ArgumentParameter> Parameters { get; set; } = Enumerable.Empty<ArgumentParameter>();

    public static Argument? GetArgument(SemanticModel semanticModel, ISymbol? symbol, string attributeName)
    {
        var attributeSymbol = semanticModel.Compilation.GetTypeByMetadataName(attributeName);
        var attributeData = symbol?.GetAttributes().FirstOrDefault(x => attributeSymbol?.Equals(x.AttributeClass, SymbolEqualityComparer.Default) == true);
        
        if (attributeData is null)
            return null;

        if (symbol is IPropertySymbol propertySymbol)
        {
            return new Argument
            {
                ParentType = propertySymbol.Type.ToDisplayString(),
                ParentName = propertySymbol.Name,
                Parameters = ArgumentParameter.DetermineParameters(attributeData)
            };
        }
        
        if (symbol is IMethodSymbol methodSymbol)
        {
            return new Argument
            {
                ParentType = methodSymbol.ReturnType.ToDisplayString(),
                ParentName = methodSymbol.Name,
                Parameters = ArgumentParameter.DetermineParameters(attributeData)
            };
        }
        
        if (symbol is INamedTypeSymbol)
        {
            return new Argument
            {
                ParentType = string.Empty,
                ParentName = string.Empty,
                Parameters = ArgumentParameter.DetermineParameters(attributeData)
            };
        }
        
        return null;
    }
}

public class ArgumentParameter
{
    public string TypeNamespace { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty;
    public ITypeSymbol? TypeSymbol { get; set; }

    public static IEnumerable<ArgumentParameter> DetermineParameters(AttributeData attributeData)
    {
        var attributeSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
        var parameterSymbols = attributeData.AttributeConstructor?.Parameters.ToList();
        var typedConstants = attributeData.ConstructorArguments.ToList();
        var argumentSyntaxes = attributeSyntax?.ArgumentList?.Arguments.ToList();
        
        if (parameterSymbols is null || argumentSyntaxes is null)
            return Enumerable.Empty<ArgumentParameter>();
        
        var actionLabelParameters = parameterSymbols
            .Zip(typedConstants, (x, y) => new { ParameterSymbol = x, TypedConstant = y })
            .Zip(argumentSyntaxes, (x, y) => new { x.ParameterSymbol, x.TypedConstant, ArgumentSyntax = y })
            .ToList();
        
        return actionLabelParameters.Select(x => new ArgumentParameter
        {
            TypeNamespace = x.ParameterSymbol.Type.ContainingNamespace.ToDisplayString(),
            Type = x.ParameterSymbol.Type.ToDisplayString(),
            Name = x.ParameterSymbol.Name,
            Value = x.TypedConstant.Value?.ToString() ?? string.Empty,
            Expression = x.ArgumentSyntax.Expression.ToString(),
            TypeSymbol = x.TypedConstant.Kind == TypedConstantKind.Type ? x.TypedConstant.Value as ITypeSymbol : null
        });
    }
}