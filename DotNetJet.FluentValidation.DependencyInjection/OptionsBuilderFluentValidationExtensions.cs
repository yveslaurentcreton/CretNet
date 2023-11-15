using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotNetJet.FluentValidation.DependencyInjection;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(provider =>
        {
            var validator = provider.GetService<IValidator<TOptions>>();

            if (validator is null)
            {
                var scope = provider.CreateScope();
                validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
            }

            return new FluentValidationOptions<TOptions>(optionsBuilder.Name, validator);
        });
        
        return optionsBuilder;
    }
}