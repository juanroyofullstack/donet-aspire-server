namespace NetAspireServer.Infrastructure.Repositories;

public interface ICosmosProductDataStore
{
    Task<CosmosProductDocument> UpsertAsync(CosmosProductDocument document, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CosmosProductDocument>> GetAllAsync(CancellationToken cancellationToken = default);
}

public sealed record CosmosProductDocument(Guid Id, string Name, decimal Price);
