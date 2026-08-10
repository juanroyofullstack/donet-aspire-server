namespace NetAspireServer.Api.Endpoints;

public static class SystemEndpoints
{
    private const string RootRoute = "/";
    private const string HealthRoute = "/health";
    private const string StatusTag = "System";
    private const string StatusOperationName = "GetStatus";
    private const string HealthOperationName = "GetHealth";
    private const string StatusValue = "ok";
    private const string StatusMessage = "NetAspireServer API is running.";

    public static void MapSystemEndpoints(this WebApplication app)
    {
        app.MapGet(RootRoute, () => Results.Ok(new { status = StatusValue, message = StatusMessage }))
            .WithName(StatusOperationName)
            .WithTags(StatusTag);

        app.MapGet(HealthRoute, () => Results.Ok(new { status = StatusValue }))
            .WithName(HealthOperationName)
            .WithTags(StatusTag);
    }
}