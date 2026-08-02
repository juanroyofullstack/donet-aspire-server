using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
});

var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator(emulator => emulator.WithContainerRuntimeArgs("--platform", "linux/amd64"));

cosmos.AddCosmosDatabase("productsdb")
    .AddContainer("products", "/id");

builder.AddProject<Projects.NetAspireServer_Api>("api")
    .WithReference(cosmos);

builder.Build().Run();
