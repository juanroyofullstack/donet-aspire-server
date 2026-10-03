using NetAspireServer.Application.Interfaces;
using NetAspireServer.Domain.Entities;

namespace NetAspireServer.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];
    private readonly Lock _lock = new();

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _products.Add(product);
        }

        return Task.FromResult(product);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult(_products.SingleOrDefault(product => product.Id == id));
        }
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<Product>>(new List<Product>(_products).AsReadOnly());
        }
    }
}
