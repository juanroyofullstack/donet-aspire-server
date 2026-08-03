namespace NetAspireServer.Api.Contracts.Products;

public sealed record ProductResponse(Guid Id, string Name, decimal Price);
