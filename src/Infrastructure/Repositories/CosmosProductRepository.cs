using NetAspireServer.Application.Interfaces;
using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Configuration;

namespace NetAspireServer.Infrastructure.Repositories;

public sealed class CosmosProductRepository : IProductRepository
{
    private readonly CosmosDbOptions _options;
    private readonly ICosmosProductDataStore _dataStore;

    public CosmosProductRepository(ICosmosProductDataStore dataStore, CosmosDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(dataStore);

        if (!options.IsConfigured)
        {
            throw new InvalidOperationException("Cosmos DB is not configured. Please provide database name and container name.");
        }

        _dataStore = dataStore;
        _options = options;
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var document = new CosmosProductDocument(product.Id, product.Name, product.Price);
        var response = await _dataStore.UpsertAsync(document, cancellationToken);

        return new Product(response.Id, response.Name, response.Price);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _dataStore.GetAllAsync(cancellationToken);

        return documents
            .Select(item => new Product(item.Id, item.Name, item.Price))
            .ToList();
    }
}
