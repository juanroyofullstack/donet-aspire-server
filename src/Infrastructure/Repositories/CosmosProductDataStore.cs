using Microsoft.Azure.Cosmos;
using NetAspireServer.Infrastructure.Configuration;

namespace NetAspireServer.Infrastructure.Repositories;

public sealed class CosmosProductDataStore : ICosmosProductDataStore
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private readonly SemaphoreSlim _containerLock = new(1, 1);
    private Container? _container;

    public CosmosProductDataStore(CosmosClient client, CosmosDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (!options.IsConfigured)
        {
            throw new InvalidOperationException("Cosmos DB is not configured. Please provide database name and container name.");
        }

        _client = client;
        _options = options;
    }

    public async Task<CosmosProductDocument> UpsertAsync(CosmosProductDocument document, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var response = await container.UpsertItemAsync(document, cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<IReadOnlyList<CosmosProductDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<CosmosProductDocument>(query);
        var results = new List<CosmosProductDocument>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.Resource);
        }

        return results;
    }

    public async Task<CosmosProductDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);

        try
        {
            var response = await container.ReadItemAsync<CosmosProductDocument>(
                id.ToString(),
                new PartitionKey(id.ToString()),
                cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            return _container;
        }

        await _containerLock.WaitAsync(cancellationToken);
        try
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
        finally
        {
            _containerLock.Release();
        }
    }
}

public sealed class InMemoryProductDataStore : ICosmosProductDataStore
{
    private readonly List<CosmosProductDocument> _documents = [];
    private readonly Lock _lock = new();

    public Task<CosmosProductDocument> UpsertAsync(CosmosProductDocument document, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
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
        }

        return Task.FromResult(document);
    }

    public Task<CosmosProductDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult(_documents.SingleOrDefault(item => item.Id == id));
        }
    }

    public Task<IReadOnlyList<CosmosProductDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<CosmosProductDocument>>(new List<CosmosProductDocument>(_documents).AsReadOnly());
        }
    }
}
