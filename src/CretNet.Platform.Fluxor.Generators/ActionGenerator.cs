using CretNet.Platform.Fluxor.Generators.Core;
using CretNet.Platform.Fluxor.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CretNet.Platform.Fluxor.Generators;

[Generator]
public class CnpActionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "CretNet.Platform.Fluxor.Generators.CnpActionAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) =>
                {
                    var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    return GetSemanticTarget(classSymbol, ctx.SemanticModel);
                })
            .Where(static target => target is not null)
            .Collect()
            .SelectMany((items, _) => items.Distinct());
        
        context.RegisterSourceOutput(classes,
            (ctx, source) =>
            {
                if (source is null)
                    return;
                
                Execute(ctx, source);
            });
    }

    private static CnpActionToGenerate? GetSemanticTarget(INamedTypeSymbol classSymbol, SemanticModel semanticModel)
    {
        var actionAttribute = Argument.GetArgument(semanticModel, classSymbol, "CretNet.Platform.Fluxor.Generators.CnpActionAttribute");
        if (actionAttribute is null)
            return null;

        // Get action values
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var hasEffectString = actionAttribute.Parameters.FirstOrDefault(x => x.Name.Equals("hasEffect"))?.Value;
        var hasEffect = hasEffectString is not null && bool.TryParse(hasEffectString, out var hasEffectResult) && hasEffectResult;
        
        // Get state values
        var state = State.GetState(semanticModel, classSymbol);

        // Get dependency injection parameters
        var dependencyInjectionParameters = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(x => Argument.GetArgument(semanticModel, x, "CretNet.Platform.Fluxor.Generators.CnpInjectAttribute"))
            .Where(x => x is not null)
            .ToDictionary(x => $"{x!.ParentType}", x => x!.ParentName);

        return new CnpActionToGenerate
        {
            NamespaceName = namespaceName,
            ClassName = className,
            HasEffect = hasEffect,
            State = state,
            DependencyInjectionParameters = dependencyInjectionParameters
        };
    }

    private void Execute(SourceProductionContext context, CnpActionToGenerate cnpActionToGenerate)
    {
        var namespaceName = cnpActionToGenerate.NamespaceName;
        var className = cnpActionToGenerate.ClassName;
        var fileName = $"{namespaceName}.{className}.g.cs";
        
        var source = Helpers.GetEmbededResource($"CretNet.Platform.Fluxor.Generators.Resources.CnpAction.cs");

        var dependencyInjectionSource = GetDependencyInjectionSource(cnpActionToGenerate);
        var effectSource = GetEffectSource(cnpActionToGenerate);
        var usingsSource = GetUsings(cnpActionToGenerate);
        var inheritanceSource = GetInheritanceSource(cnpActionToGenerate);
        var reducersSource = cnpActionToGenerate.State.GetReducerSource();
        
        source = source.Replace("[[UsingsSource]]", usingsSource);
        source = source.Replace("[[InheritanceSource]]", inheritanceSource);
        source = source.Replace("[[ReducersSource]]", reducersSource);
        source = source.Replace("[[NamespaceName]]", cnpActionToGenerate.NamespaceName);
        source = source.Replace("[[ClassName]]", cnpActionToGenerate.ClassName);
        source = source.Replace("[[ShortClassName]]", cnpActionToGenerate.ShortClassName);
        source = source.Replace("[[PostEventName]]", cnpActionToGenerate.PostEventName);
        source = source.Replace("[[StateNamespaceName]]", cnpActionToGenerate.State.NamespaceName);
        source = source.Replace("[[StateClassName]]", cnpActionToGenerate.State.ClassName);
        source = source.Replace("[[DependencyInjectionSource]]", dependencyInjectionSource);
        source = source.Replace("[[EffectSource]]", effectSource);

        context.AddSource(fileName, source);
    }
    
    private string GetDependencyInjectionSource(CnpActionToGenerate cnpActionToGenerate)
    {
        var dependencyInjectionParameters = cnpActionToGenerate.DependencyInjectionParameters;
        
        if (!dependencyInjectionParameters.Any())
            return string.Empty;
        
        var dependencyInjectionLines = dependencyInjectionParameters.Select(x => $"action.{x.Value} = _serviceProvider.GetRequiredService<{x.Key}>();");
        var dependencyInjectionSource = string.Join("\n        ", dependencyInjectionLines);
        
        return $"\n        {dependencyInjectionSource}\n";
    }
    
    private string GetEffectSource(CnpActionToGenerate cnpActionToGenerate)
    {
        if (!cnpActionToGenerate.HasEffect)
            return string.Empty;
        
        return "\n        await action.Effect(dispatcher);";
    }
    
    private string GetUsings(CnpActionToGenerate cnpActionToGenerate)
    {
        var usings = new List<string>
        {
            "using Microsoft.Extensions.DependencyInjection;",
            "using Fluxor;"
        };
        
        if (cnpActionToGenerate.State.NamespaceName is not null)
            usings.Add($"using {cnpActionToGenerate.State.NamespaceName};");
        
        return string.Join("\n", usings.Distinct());
    }
    
    private string GetInheritanceSource(CnpActionToGenerate cnpActionToGenerate)
    {
        var inheritanceList = new List<string>
        {
            "ICnpAction"
        };
        
        if (cnpActionToGenerate.HasEffect)
            inheritanceList.Add("ICnpActionEffect");
        
        if (cnpActionToGenerate.State.GenerateCustomReducer)
            inheritanceList.Add("ICnpActionReducer<[[StateClassName]]>");
        
        var inheritanceSource = string.Empty;
        
        if (inheritanceList.Any())
            inheritanceSource = $": {string.Join(", ", inheritanceList)}";
        
        return inheritanceSource;
    }
}