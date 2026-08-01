using NetAspireServer.Application.Interfaces;
using NetAspireServer.Domain.Entities;

namespace NetAspireServer.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> CreateAsync(string name, decimal price, CancellationToken cancellationToken = default)
    {
        var product = new Product(Guid.NewGuid(), name, price);
        return await _productRepository.AddAsync(product, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetAllAsync(cancellationToken);
    }
}
