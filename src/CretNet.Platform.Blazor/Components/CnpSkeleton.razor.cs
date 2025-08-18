using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSkeleton
{
    [Parameter] public string? Height { get; set; }
    [Parameter] public string? Width { get; set; }
    [Parameter] public CnpSkeletonType Type { get; set; } = CnpSkeletonType.Text;
    
    public SkeletonShape FluentType => Type.ToFluent();
}

public enum CnpSkeletonType
{
    Text,
    Rectangle,
    Circle
}

public static class CnpSkeletonExtensions
{
    public static SkeletonShape ToFluent(this CnpSkeletonType cnpSkeletonType)
    {
        return cnpSkeletonType switch
        {
            CnpSkeletonType.Text => SkeletonShape.Rect,
            CnpSkeletonType.Rectangle => SkeletonShape.Rect,
            CnpSkeletonType.Circle => SkeletonShape.Circle,
            _ => throw new ArgumentOutOfRangeException(nameof(cnpSkeletonType), cnpSkeletonType, null)
        };
    }
}