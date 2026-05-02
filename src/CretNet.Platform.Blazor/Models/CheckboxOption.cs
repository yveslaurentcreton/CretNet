namespace CretNet.Platform.Blazor.Models;

/// <summary>
/// One option in a <c>CnpBindCheckboxGroup&lt;TQuery, TValue&gt;</c>.
/// </summary>
/// <param name="Value">The value pushed onto the query field when the checkbox is enabled.</param>
/// <param name="Label">User-visible label.</param>
public sealed record CheckboxOption<TValue>(TValue Value, string Label);
