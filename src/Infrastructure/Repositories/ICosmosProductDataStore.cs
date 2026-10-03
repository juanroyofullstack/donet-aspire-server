using Newtonsoft.Json;

namespace NetAspireServer.Infrastructure.Repositories;

public interface ICosmosProductDataStore
{
    Task<CosmosProductDocument> UpsertAsync(CosmosProductDocument document, CancellationToken cancellationToken = default);
    Task<CosmosProductDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CosmosProductDocument>> GetAllAsync(CancellationToken cancellationToken = default);
}

public sealed record CosmosProductDocument([property: JsonProperty("id")] Guid Id, string Name, decimal Price);
