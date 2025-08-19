using CretNet.Platform.Fluxor.Generators.Core;
using Microsoft.CodeAnalysis;

namespace CretNet.Platform.Fluxor.Generators;

[Generator]
public class GeneralGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attributes
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpInjectAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpInjectAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpActionAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpActionAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpStateAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpStateAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpEntityActionAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpEntityActionAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpEntityStateAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpEntityStateAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpActionLabelAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpActionLabelAttribute.cs")));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpSuccessLabelAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpSuccessLabelAttribute.cs")));
                
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CretNet.Platform.Fluxor.Generators.CnpFailureLabelAttribute.g.cs",
            Helpers.GetEmbededResource("CretNet.Platform.Fluxor.Generators.Resources.CnpFailureLabelAttribute.cs")));
    }
}