using NetAspireServer.Application.Interfaces;
using NetAspireServer.Domain.Entities;

namespace NetAspireServer.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Product>>(_products.AsReadOnly());
    }
}
