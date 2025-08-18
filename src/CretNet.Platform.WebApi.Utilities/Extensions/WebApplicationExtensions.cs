using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void MapPing(this IEndpointRouteBuilder app)
    {
        app.MapGet("/ping", () => new PingResponse("Pong"))
            .Produces<PingResponse>();
    }
}

public record PingResponse(string Content);