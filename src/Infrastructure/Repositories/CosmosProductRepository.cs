using Microsoft.Azure.Cosmos;
using NetAspireServer.Application.Interfaces;
using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Configuration;

namespace NetAspireServer.Infrastructure.Repositories;

public sealed class CosmosProductRepository : IProductRepository
{
    private readonly CosmosDbOptions _options;
    private readonly CosmosClient _client;
    private Container? _container;

    public CosmosProductRepository(CosmosClient client, CosmosDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!options.IsConfigured)
        {
            throw new InvalidOperationException("Cosmos DB is not configured. Please provide database name and container name.");
        }

        _client = client;
        _options = options;
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var document = new ProductDocument(product.Id, product.Name, product.Price);

        var response = await container.UpsertItemAsync(document, cancellationToken: cancellationToken);
        return new Product(response.Resource.Id, response.Resource.Name, response.Resource.Price);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<ProductDocument>(query);
        var products = new List<Product>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            products.AddRange(response.Resource.Select(item => new Product(item.Id, item.Name, item.Price)));
        }

        return products;
    }

    private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            return _container;
        }

        var database = await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName!, cancellationToken: cancellationToken);
        var containerProperties = new ContainerProperties(_options.ContainerName!, "/id");
        var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerProperties, cancellationToken: cancellationToken);
        _container = containerResponse.Container;

        return _container;
    }

    private sealed record ProductDocument(Guid Id, string Name, decimal Price);
}
