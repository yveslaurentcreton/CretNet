namespace CretNet.Platform.Blazor.Models;

public class BreadcrumbItem
{
    public required string Text { get; set; }
    public string? NavigationUrl { get; set; }
}