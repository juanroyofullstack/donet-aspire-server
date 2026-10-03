using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Repositories;
using Newtonsoft.Json.Linq;

namespace NetAspireServer.Application.Tests;

public sealed class InMemoryProductRepositoryBehaviorTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnSnapshot_WhenRepositoryChangesAfterRead()
    {
        var repository = new InMemoryProductRepository();
        await repository.AddAsync(new Product(Guid.NewGuid(), "Laptop", 999m));

        var snapshot = await repository.GetAllAsync();
        await repository.AddAsync(new Product(Guid.NewGuid(), "Mouse", 25m));

        Assert.Single(snapshot);
        Assert.Equal(2, (await repository.GetAllAsync()).Count);
    }

    [Fact]
    public async Task Operations_ShouldHonorCancelledToken()
    {
        var repository = new InMemoryProductRepository();
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            repository.AddAsync(new Product(Guid.NewGuid(), "Laptop", 999m), cancellationSource.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => repository.GetAllAsync(cancellationSource.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => repository.GetByIdAsync(Guid.NewGuid(), cancellationSource.Token));
    }

    [Fact]
    public void CosmosProductDocument_ShouldSerializeLowercaseId_ForCosmos()
    {
        var document = new CosmosProductDocument(Guid.NewGuid(), "Laptop", 999m);

        var json = JObject.FromObject(document);

        Assert.NotNull(json["id"]);
        Assert.Null(json["Id"]);
    }

    [Fact]
    public async Task InMemoryProductDataStore_ShouldReturnDocumentById()
    {
        var dataStore = new InMemoryProductDataStore();
        var document = new CosmosProductDocument(Guid.NewGuid(), "Laptop", 999m);
        await dataStore.UpsertAsync(document);

        var result = await dataStore.GetByIdAsync(document.Id);

        Assert.Equal(document, result);
    }

    [Fact]
    public async Task InMemoryProductDataStore_ShouldHonorCancelledToken()
    {
        var dataStore = new InMemoryProductDataStore();
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => dataStore.GetAllAsync(cancellationSource.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => dataStore.GetByIdAsync(Guid.NewGuid(), cancellationSource.Token));
    }
}
