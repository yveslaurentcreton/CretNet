using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotNetJet.FluentValidation.DependencyInjection;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            provider => new FluentValidationOptions<TOptions>(optionsBuilder.Name, provider.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}