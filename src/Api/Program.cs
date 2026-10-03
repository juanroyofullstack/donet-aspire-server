using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;
using NetAspireServer.Api.Endpoints;
using NetAspireServer.Application.Interfaces;
using NetAspireServer.Application.Services;
using NetAspireServer.Infrastructure.Configuration;
using NetAspireServer.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("cosmos")))
{
    var acceptUntrustedEmulatorCertificate = builder.Configuration.GetValue<bool>("CosmosDb:AcceptUntrustedEmulatorCertificate");
    builder.AddAzureCosmosClient("cosmos", configureClientOptions: clientOptions =>
    {
        if (acceptUntrustedEmulatorCertificate)
        {
            clientOptions.ConnectionMode = ConnectionMode.Gateway;
            clientOptions.HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        }
    });
}

builder.Services.Configure<CosmosDbOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton<ICosmosProductDataStore>(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    var cosmosClient = sp.GetService<CosmosClient>();

    return options.IsConfigured && cosmosClient is not null
        ? new CosmosProductDataStore(cosmosClient, options)
        : new InMemoryProductDataStore();
});

builder.Services.AddSingleton<IProductRepository>(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    var dataStore = sp.GetRequiredService<ICosmosProductDataStore>();

    return options.IsConfigured && sp.GetService<CosmosClient>() is not null
        ? new CosmosProductRepository(dataStore, options)
        : new InMemoryProductRepository();
});
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapSystemEndpoints();
app.MapProductEndpoints();

app.Run();

public partial class Program
{
}
