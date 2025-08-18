using System.Text;
using System.Text.Json;

namespace CretNet.Platform.Fluxor;

public class ValidationProblemDetails
{
    public IDictionary<string, ICollection<string>>? Errors { get; set; }
    public string? Type { get; set; } = default!;
    public string? Title { get; set; } = default!;
    public int? Status { get; set; } = default!;
    public string? Detail { get; set; } = default!;
    public string? Instance { get; set; } = default!;
    public IDictionary<string, object> Extensions { get; set; } = default!;
    private IDictionary<string, object>? _additionalProperties;
    public IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ??= new Dictionary<string, object>(); }
        set => _additionalProperties = value;
    }
    
    public string FormatValidationErrors()
    {
        if (Errors is null || Errors.Count == 0)
            return "Api validation failed, but no details were provided.";

        var sb = new StringBuilder();
        sb.AppendLine("Api validation failed:");

        foreach (var error in Errors)
        {
            var field = error.Key;
            var messages = error.Value;
            
            foreach (var message in messages)
            {
                sb.AppendLine($"  - {field}: {message}");
            }
        }

        return sb.ToString();
    }
}

public static class ApiExceptionUtils
{
    public static bool TryExtractValidationDetails(Exception ex, out string? validationMessage)
    {
        validationMessage = null;

        var exType = ex.GetType();
        if (!exType.IsGenericType || exType.GetGenericTypeDefinition().Name != "ApiException`1")
            return false;

        var statusCodeProp = exType.GetProperty("StatusCode");
        var resultProp = exType.GetProperty("Result");

        if (statusCodeProp == null || resultProp == null)
            return false;

        var statusCode = (int?)statusCodeProp.GetValue(ex);
        if (statusCode != 400) return false;

        var resultObj = resultProp.GetValue(ex);
        if (resultObj == null) return false;

        try
        {
            var json = JsonSerializer.Serialize(resultObj);
            var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            validationMessage = problem.FormatValidationErrors();
            return true;
        }
        catch
        {
            return false;
        }
    }
}