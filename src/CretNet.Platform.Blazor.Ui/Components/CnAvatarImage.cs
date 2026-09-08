using Microsoft.AspNetCore.Components.Forms;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>Browser-side photo preparation shared by host profile upload flows.</summary>
public static class CnAvatarImage
{
    /// <summary>Resizes the selected image to a bounded PNG before the host uploads it.
    /// Host applications own persistence and presentation of upload errors.</summary>
    public static async Task<byte[]> ReadPngAsync(
        IBrowserFile file,
        int maxWidth = 256,
        int maxHeight = 256,
        long maxBytes = 512 * 1024,
        CancellationToken cancellationToken = default)
    {
        var resized = await file.RequestImageFileAsync("image/png", maxWidth, maxHeight);
        await using var stream = resized.OpenReadStream(maxBytes, cancellationToken);
        await using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        return buffer.ToArray();
    }
}
