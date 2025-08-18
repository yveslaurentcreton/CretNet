using CretNet.Platform.Fluxor.Generators.Core;
using CretNet.Platform.Fluxor.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CretNet.Platform.Fluxor.Generators;

[Generator]
public class CnpEntityActionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "CretNet.Platform.Fluxor.Generators.CnpEntityActionAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) =>
                {
                    var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    return GetSemanticTarget(classSymbol, ctx.SemanticModel);
                })
            .Where(static (target) => target is not null)
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
    
    private static CnpEntityActionToGenerate? GetSemanticTarget(INamedTypeSymbol classSymbol, SemanticModel semanticModel)
    {
        var entityActionAttribute = Argument.GetArgument(semanticModel, classSymbol, "CretNet.Platform.Fluxor.Generators.CnpEntityActionAttribute");
        if (entityActionAttribute is null)
            return null;
        
        // Get action values
        var entityActionParameters = entityActionAttribute.Parameters.ToList();
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var hasPostActionsString = entityActionParameters.FirstOrDefault(x => x.Name.Equals("hasPostActions"))?.Value;
        var hasPostActions = hasPostActionsString is not null && bool.TryParse(hasPostActionsString, out var hasPostActionsResult) && hasPostActionsResult;
        
        // Get entity values
        var entityTypeParameter = entityActionParameters.First(x => x.Name.Equals("entityType"));
        var entityTypeSymbol = entityTypeParameter.TypeSymbol as INamedTypeSymbol;
        // Default fallbacks for safety
        string? resultNamespaceName = null;
        string? resultExpression = null;
        string entityNamespaceName = string.Empty;
        string entityClassName = string.Empty;
        if (entityTypeSymbol is not null)
        {
            if (entityTypeSymbol.TypeArguments.Length == 0)
            {
                resultNamespaceName = entityTypeSymbol.ContainingNamespace.ToDisplayString();
                resultExpression = entityTypeSymbol.Name;
                entityNamespaceName = resultNamespaceName;
                entityClassName = resultExpression;
            }
            else
            {
                var genericArg = entityTypeSymbol.TypeArguments[0] as INamedTypeSymbol; // single generic argument assumed
                var baseName = entityTypeSymbol.Name;
                var argName = genericArg?.Name ?? "object";
                resultNamespaceName = entityTypeSymbol.ContainingNamespace.ToDisplayString();
                resultExpression = $"{baseName}<{argName}>";
                entityNamespaceName = genericArg?.ContainingNamespace.ToDisplayString() ?? resultNamespaceName;
                entityClassName = argName;
            }
        }
        var isNullableString = entityActionParameters.FirstOrDefault(x => x.Name.Equals("isNullable"))?.Value;
        var isNullable = isNullableString is not null && bool.TryParse(isNullableString, out var isNullableResult) && isNullableResult;
        resultExpression = isNullable ? $"{resultExpression}?" : resultExpression;
        
        // Get state parameter
        var entityState = EntityState.GetEntityState(semanticModel, classSymbol);
        
        // Get dependency injection parameters
        var dependencyInjectionParameters = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(x => Argument.GetArgument(semanticModel, x, "CretNet.Platform.Fluxor.Generators.CnpInjectAttribute"))
            .Where(x => x is not null)
            .ToDictionary(x => $"{x!.ParentType}", x => x!.ParentName);
        
        // Get label parameters
        var actionLabelProperty = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic)
            .Select(x => Argument.GetArgument(semanticModel, x, "CretNet.Platform.Fluxor.Generators.CnpActionLabelAttribute"))
            .FirstOrDefault(x => x is not null)?.ParentName;
        var successLabelProperty = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic)
            .Select(x => Argument.GetArgument(semanticModel, x, "CretNet.Platform.Fluxor.Generators.CnpSuccessLabelAttribute"))
            .FirstOrDefault(x => x is not null)?.ParentName;
        var failureLabelProperty = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic)
            .Select(x => Argument.GetArgument(semanticModel, x, "CretNet.Platform.Fluxor.Generators.CnpFailureLabelAttribute"))
            .FirstOrDefault(x => x is not null)?.ParentName;
        
        return new CnpEntityActionToGenerate
        {
            NamespaceName = namespaceName,
            ClassName = className,
            ActionLabelProperty = actionLabelProperty,
            SuccessLabelProperty = successLabelProperty,
            FailureLabelProperty = failureLabelProperty,
            HasPostActions = hasPostActions,
            ResultNamespaceName = resultNamespaceName,
            ResultExpression = resultExpression,
            EntityNamespaceName = entityNamespaceName,
            EntityClassName = entityClassName,
            EntityState = entityState,
            DependencyInjectionParameters = dependencyInjectionParameters
        };
    }

    private void Execute(SourceProductionContext context, CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var namespaceName = cnpEntityActionToGenerate.NamespaceName;
        var className = cnpEntityActionToGenerate.ClassName;
        var fileName = $"{namespaceName}.{className}.g.cs";
        
        var source = Helpers.GetEmbededResource($"CretNet.Platform.Fluxor.Generators.Resources.CnpEntityAction.cs");

        var dependencyInjectionSource = GetDependencyInjectionSource(cnpEntityActionToGenerate);
        var usingsSource = GetUsings(cnpEntityActionToGenerate);
        var inheritanceSource = GetInheritanceSource(cnpEntityActionToGenerate);
        var reducersSource = cnpEntityActionToGenerate.EntityState.GetReducerSource(cnpEntityActionToGenerate.EntityClassName, cnpEntityActionToGenerate.IsPluralReturnType);
        var successMessage = GetSuccessMessage(cnpEntityActionToGenerate);
        var failureMessage = GetFailureMessage(cnpEntityActionToGenerate);
        var postActionsSource = GetPostActionsSource(cnpEntityActionToGenerate);
        
        source = source.Replace("[[UsingsSource]]", usingsSource);
        source = source.Replace("[[InheritanceSource]]", inheritanceSource);
        source = source.Replace("[[ReducersSource]]", reducersSource);
        source = source.Replace("[[NamespaceName]]", cnpEntityActionToGenerate.NamespaceName);
        source = source.Replace("[[ClassName]]", cnpEntityActionToGenerate.ClassName);
        source = source.Replace("[[ShortClassName]]", cnpEntityActionToGenerate.ShortClassName);
        source = source.Replace("[[SuccessClassName]]", cnpEntityActionToGenerate.SuccessClassName);
        source = source.Replace("[[FailureClassName]]", cnpEntityActionToGenerate.FailureClassName);
        source = source.Replace("[[SuccessEventName]]", cnpEntityActionToGenerate.SuccessEventName);
        source = source.Replace("[[FailureEventName]]", cnpEntityActionToGenerate.FailureEventName);
        source = source.Replace("[[ShortClassName]]", cnpEntityActionToGenerate.ShortClassName);
        source = source.Replace("[[ResultExpression]]", cnpEntityActionToGenerate.ResultExpression);
        source = source.Replace("[[EntityClassName]]", cnpEntityActionToGenerate.EntityClassName);
        source = source.Replace("[[StateNamespaceName]]", cnpEntityActionToGenerate.EntityState.NamespaceName);
        source = source.Replace("[[StateClassName]]", cnpEntityActionToGenerate.EntityState.ClassName);
        source = source.Replace("[[DependencyInjectionSource]]", dependencyInjectionSource);
        source = source.Replace("[[SuccessMessage]]", successMessage);
        source = source.Replace("[[FailureMessage]]", failureMessage);
        source = source.Replace("[[PostActionsSource]]", postActionsSource);

        context.AddSource(fileName, source);
    }
    
    private string GetDependencyInjectionSource(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var dependencyInjectionParameters = cnpEntityActionToGenerate.DependencyInjectionParameters;
        
        if (!dependencyInjectionParameters.Any())
            return string.Empty;
        
        var dependencyInjectionLines = dependencyInjectionParameters.Select(x => $"action.{x.Value} = _serviceProvider.GetRequiredService<{x.Key}>();");
        var dependencyInjectionSource = string.Join("\n        ", dependencyInjectionLines);
        
        return $"\n        {dependencyInjectionSource}\n";
    }
    
    private string GetUsings(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var usings = new List<string>
        {
            "using Microsoft.Extensions.DependencyInjection;",
            "using Fluxor;",
            "using CretNet.Platform.Fluxor;",
            "using CretNet.Platform.Blazor.Services;",
            "using System.Text.Json;",
            "using System.Linq;"
        };
        
        if (cnpEntityActionToGenerate.ResultNamespaceName is not null)
            usings.Add($"using {cnpEntityActionToGenerate.ResultNamespaceName};");
        
        if (cnpEntityActionToGenerate.EntityNamespaceName is not null)
            usings.Add($"using {cnpEntityActionToGenerate.EntityNamespaceName};");
        
        if (cnpEntityActionToGenerate.EntityState.NamespaceName is not null)
            usings.Add($"using {cnpEntityActionToGenerate.EntityState.NamespaceName};");
        
        return string.Join("\n", usings.Distinct());
    }
    
    private string GetInheritanceSource(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var inheritanceList = new List<string>
        {
            "ICnpEntityAction<[[ResultExpression]]>"
        };
        
        if (cnpEntityActionToGenerate.EntityState.GenerateCustomReducer)
            inheritanceList.Add("ICnpEntityActionReducer<[[ResultExpression]], [[StateClassName]]>");
        
        if (cnpEntityActionToGenerate.HasPostActions)
            inheritanceList.Add("ICnpEntityActionPostActions<[[ResultExpression]]>");

        var inheritanceSource = string.Empty;
        
        if (inheritanceList.Any())
            inheritanceSource = $" : {string.Join(", ", inheritanceList)}";
        
        return inheritanceSource;
    }
    
    private string GetSuccessMessage(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var className = cnpEntityActionToGenerate.ClassName;
        var entityClassName = cnpEntityActionToGenerate.EntityClassName;
        var actionLabelProperty = cnpEntityActionToGenerate.ActionLabelProperty;
        var successLabelProperty = cnpEntityActionToGenerate.SuccessLabelProperty;
        var isPluralReturnType = cnpEntityActionToGenerate.IsPluralReturnType;
        var entityDefinitionLabelProperty = isPluralReturnType ? "PluralLabel" : "Label";
        var entityIsGeneric = cnpEntityActionToGenerate.ResultExpression?.Contains($"<{entityClassName}>") == true;
        
        if (actionLabelProperty is null || successLabelProperty is null)
            return string.Empty;
        
        if (entityIsGeneric || isPluralReturnType)
            return $"\n        _toastService.Success({className}.{actionLabelProperty}, {className}.{successLabelProperty}, new {{ Entity = _entityDefinition?.{entityDefinitionLabelProperty} ?? string.Empty }});";
        
        return $"\n        _toastService.Success({className}.{actionLabelProperty}, {className}.{successLabelProperty}, new {{ Entity = _entityDefinition?.{entityDefinitionLabelProperty} ?? string.Empty, Identifier = _entityDefinition?.GetIdentifier(action.Entity) ?? string.Empty }});";
    }
    
    private string GetFailureMessage(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        var className = cnpEntityActionToGenerate.ClassName;
        var actionLabelProperty = cnpEntityActionToGenerate.ActionLabelProperty;
        var failureLabelProperty = cnpEntityActionToGenerate.FailureLabelProperty;
        var isPluralReturnType = cnpEntityActionToGenerate.IsPluralReturnType;
        var entityDefinitionLabelProperty = isPluralReturnType ? "PluralLabel" : "Label";
        
        if (actionLabelProperty is null)
            return string.Empty;
        
        if (failureLabelProperty is null)
            return $"\n        _toastService.Error({className}.{actionLabelProperty}, action.Exception.Message);";
        
        return $"\n        _toastService.Error({className}.{actionLabelProperty}, {className}.{failureLabelProperty}, new {{ Entity = _entityDefinition?.{entityDefinitionLabelProperty} ?? string.Empty, Error = action.Exception.Message }});";
    }
    
    private string GetPostActionsSource(CnpEntityActionToGenerate cnpEntityActionToGenerate)
    {
        if (!cnpEntityActionToGenerate.HasPostActions)
            return string.Empty;

        return $"\n        action.SourceAction.PostActions(action, dispatcher);";
    }
}