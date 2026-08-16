using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Configuration;
using NetAspireServer.Infrastructure.Repositories;

namespace NetAspireServer.Application.Tests;

public class CosmosProductRepositoryTests
{
    private sealed class FakeCosmosProductDataStore : ICosmosProductDataStore
    {
        private readonly List<CosmosProductDocument> _documents = [];

        public Task<CosmosProductDocument> UpsertAsync(CosmosProductDocument document, CancellationToken cancellationToken = default)
        {
            var existingIndex = _documents.FindIndex(item => item.Id == document.Id);
            if (existingIndex >= 0)
            {
                _documents[existingIndex] = document;
            }
            else
            {
                _documents.Add(document);
            }

            return Task.FromResult(document);
        }

        public Task<IReadOnlyList<CosmosProductDocument>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<CosmosProductDocument>>(_documents.AsReadOnly());
        }
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDataStoreIsNull()
    {
        var options = new CosmosDbOptions
        {
            DatabaseName = "netaspire",
            ContainerName = "products"
        };

        Assert.Throws<ArgumentNullException>(() => new CosmosProductRepository(null!, options));
    }

    [Fact]
    public void Constructor_ShouldThrowInvalidOperationException_WhenOptionsAreNotConfigured()
    {
        var dataStore = new FakeCosmosProductDataStore();
        var options = new CosmosDbOptions();

        Assert.Throws<InvalidOperationException>(() => new CosmosProductRepository(dataStore, options));
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAndReturnMappedProduct()
    {
        var dataStore = new FakeCosmosProductDataStore();
        var repository = new CosmosProductRepository(dataStore, new CosmosDbOptions
        {
            DatabaseName = "netaspire",
            ContainerName = "products"
        });
        var product = new Product(Guid.NewGuid(), "Laptop", 1299.99m);

        var result = await repository.AddAsync(product);

        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        var all = await repository.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapDocumentsToProducts()
    {
        var dataStore = new FakeCosmosProductDataStore();
        await dataStore.UpsertAsync(new CosmosProductDocument(Guid.NewGuid(), "Laptop", 1000m));
        await dataStore.UpsertAsync(new CosmosProductDocument(Guid.NewGuid(), "Mouse", 25.5m));
        var repository = new CosmosProductRepository(dataStore, new CosmosDbOptions
        {
            DatabaseName = "netaspire",
            ContainerName = "products"
        });

        var products = await repository.GetAllAsync();

        Assert.Equal(2, products.Count);
        Assert.Contains(products, product => product.Name == "Laptop");
        Assert.Contains(products, product => product.Name == "Mouse");
    }
}
