using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
});

builder.AddProject<Projects.NetAspireServer_Api>("api");

builder.Build().Run();
