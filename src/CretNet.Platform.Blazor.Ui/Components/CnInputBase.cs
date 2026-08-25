using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Shared plumbing for Cn inputs: uniform Value binding plus optional
/// EditForm integration. Pass <c>For</c> inside an EditForm and the control
/// shows its validation state inline (red border + message under the field),
/// like the FluentValidation setup on the other screens.
/// </summary>
public abstract class CnInputBase<TValue> : ComponentBase, IDisposable
{
    [CascadingParameter] protected EditContext? EditContext { get; set; }

    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<TValue?>>? For { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Subtle { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private FieldIdentifier _field;
    private EditContext? _subscribedTo;

    protected IReadOnlyList<string> ValidationMessages =>
        EditContext is not null && For is not null ? EditContext.GetValidationMessages(_field).ToList() : [];

    protected string? InvalidClass => ValidationMessages.Count > 0 ? "cn-input--invalid" : null;

    protected string? SubtleClass => Subtle ? "cn-input--subtle" : null;

    protected override void OnParametersSet()
    {
        if (For is not null)
            _field = CreateField(For);

        if (!ReferenceEquals(_subscribedTo, EditContext))
        {
            if (_subscribedTo is not null)
                _subscribedTo.OnValidationStateChanged -= OnValidationStateChanged;

            _subscribedTo = EditContext;

            if (_subscribedTo is not null)
                _subscribedTo.OnValidationStateChanged += OnValidationStateChanged;
        }
    }

    // FieldIdentifier.Create rejects UnaryExpressions, so binding a
    // non-nullable property (DateTime/Guid) to a nullable input crashed the
    // whole render tree (owner-hit on the payment dialog): the compiler
    // silently wraps such accessors in Convert(...). Unwrap conversions and
    // build the identifier from the inner member access instead.
    private static FieldIdentifier CreateField(Expression<Func<TValue?>> accessor)
    {
        var body = accessor.Body;
        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            body = unary.Operand;

        try
        {
            if (body is MemberExpression { Expression: not null } member)
            {
                var model = Expression.Lambda(member.Expression).Compile().DynamicInvoke();
                if (model is not null)
                    return new FieldIdentifier(model, member.Member.Name);
            }

            return FieldIdentifier.Create(accessor);
        }
        catch (ArgumentException)
        {
            // An exotic accessor should degrade to "no inline validation",
            // never take the page down.
            return new FieldIdentifier(accessor, string.Empty);
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e) => StateHasChanged();

    protected async Task SetValueAsync(TValue? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);

        if (EditContext is not null && For is not null)
            EditContext.NotifyFieldChanged(_field);
    }

    public void Dispose()
    {
        if (_subscribedTo is not null)
            _subscribedTo.OnValidationStateChanged -= OnValidationStateChanged;
    }
}
