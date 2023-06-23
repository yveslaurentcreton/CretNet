using FluentValidation;
using Microsoft.Extensions.Options;

namespace DotNetJet.FluentValidation.DependencyInjection
{
    public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;
        private readonly string? _name;

        public FluentValidationOptions(string? name, IValidator<TOptions> validator)
        {
            _name = name;
            _validator = validator;
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (_name is not null && _name != name)
                return ValidateOptionsResult.Skip;

            ArgumentNullException.ThrowIfNull(options);
            
            var validationResult = _validator.Validate(options);
            
            if (validationResult.IsValid)
                return ValidateOptionsResult.Success;
            
            var typeName = options.GetType().Name;
            var errors = validationResult.Errors.Select(x =>
                $"Options validation failed for '{typeName}.{x.PropertyName}' with the error: '{x.ErrorMessage}'.");
            
            return ValidateOptionsResult.Fail(errors);
        }
    }
}