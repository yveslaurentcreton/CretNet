using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CretNet.FluentValidation.DependencyInjection.Extensions;

public static class OptionsBuilderFluentValidationExtensions
{
    /// <summary>
    /// Adds Fluent Validation support to the specified options configuration.
    /// </summary>
    /// <param name="optionsBuilder">The options builder to configure.</param>
    /// <typeparam name="TOptions">The type of the options being configured.</typeparam>
    /// <returns>The same OptionsBuilder instance so that additional configuration calls can be chained.</returns>
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(serviceProvider =>
            new FluentValidationOptions<TOptions>(optionsBuilder.Name, serviceProvider));
        
        return optionsBuilder;
    }
}
