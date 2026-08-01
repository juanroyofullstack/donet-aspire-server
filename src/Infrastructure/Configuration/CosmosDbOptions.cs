namespace NetAspireServer.Infrastructure.Configuration;

public sealed class CosmosDbOptions
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? ContainerName { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ConnectionString) &&
        !string.IsNullOrWhiteSpace(DatabaseName) &&
        !string.IsNullOrWhiteSpace(ContainerName);
}
