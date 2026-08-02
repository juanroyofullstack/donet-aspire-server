using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;
using NetAspireServer.Application.Interfaces;
using NetAspireServer.Application.Services;
using NetAspireServer.Infrastructure.Configuration;
using NetAspireServer.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("cosmos")))
{
    builder.AddAzureCosmosClient("cosmos");
}

builder.Services.Configure<CosmosDbOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton<IProductRepository>(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    var cosmosClient = sp.GetService<CosmosClient>();

    return options.IsConfigured && cosmosClient is not null
        ? new CosmosProductRepository(cosmosClient, options)
        : new InMemoryProductRepository();
});
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new { status = "ok", message = "NetAspireServer API is running." }));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/products", async (ProductService service) =>
{
    var products = await service.GetAllAsync();
    return Results.Ok(products);
});

app.MapPost("/products", async (CreateProductRequest request, ProductService service) =>
{
    var product = await service.CreateAsync(request.Name, request.Price);
    return Results.Created($"/products/{product.Id}", product);
});

app.Run();

public sealed record CreateProductRequest(string Name, decimal Price);

