namespace NetAspireServer.Api.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { status = "ok", message = "NetAspireServer API is running." }))
            .WithName("GetStatus")
            .WithTags("System");

        app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
            .WithName("GetHealth")
            .WithTags("System");
    }
}