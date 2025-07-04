using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CretNet.FluentValidation.DependencyInjection;

public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    private readonly string? _name;
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationOptions(string? name, IServiceProvider serviceProvider)
    {
        _name = name;
        _serviceProvider = serviceProvider;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (_name is not null && _name != name)
            return ValidateOptionsResult.Skip;

        ArgumentNullException.ThrowIfNull(options);
        
        using var scope = _serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
        var validationResult = validator.Validate(options);
        
        if (validationResult.IsValid)
            return ValidateOptionsResult.Success;
        
        var typeName = options.GetType().Name;
        var errors = validationResult.Errors.Select(x =>
            $"Options validation failed for '{typeName}.{x.PropertyName}' with the error: '{x.ErrorMessage}'.");
        
        return ValidateOptionsResult.Fail(errors);
    }
}
