using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotNetJet.FluentValidation.DependencyInjection
{
    public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string? _name;

        public FluentValidationOptions(string? name, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _name = name;
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (_name is not null && _name != name)
                return ValidateOptionsResult.Skip;

            ArgumentNullException.ThrowIfNull(options);
        
            using var scope = _serviceProvider.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
            var results = validator.Validate(options);
            if (results.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var typeName = options.GetType().Name;
            var errors = new List<string>();
            foreach (var result in results.Errors)
            {
                errors.Add($"Fluent validation failed for '{typeName}.{result.PropertyName}' with the error: '{result.ErrorMessage}'.");
            }

            return ValidateOptionsResult.Fail(errors);
        }
    }
}