using NetAspireServer.Application.Services;
using NetAspireServer.Infrastructure.Repositories;

namespace NetAspireServer.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnProductWithNameAndPrice()
    {
        var repository = new InMemoryProductRepository();
        var service = new ProductService(repository);

        var product = await service.CreateAsync("Laptop", 999.99m);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(999.99m, product.Price);
    }
}
