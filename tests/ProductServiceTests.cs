using NetAspireServer.Application.Services;
using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Repositories;

namespace NetAspireServer.Application.Tests;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateProduct_WhenDataIsValid()
    {
        var productId = Guid.NewGuid();

        var product = new Product(productId, "Laptop", 999.99m);

        Assert.Equal(productId, product.Id);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(999.99m, product.Price);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenNameIsEmpty(string name)
    {
        Assert.Throws<ArgumentException>(() => new Product(Guid.NewGuid(), name, 100m));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenPriceIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Product(Guid.NewGuid(), "Laptop", -1m));
    }
}

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

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStoredProducts()
    {
        var repository = new InMemoryProductRepository();
        var service = new ProductService(repository);

        await service.CreateAsync("Laptop", 999.99m);
        await service.CreateAsync("Mouse", 29.99m);

        var products = await service.GetAllAsync();

        Assert.Equal(2, products.Count);
        Assert.Collection(products,
            product =>
            {
                Assert.Equal("Laptop", product.Name);
                Assert.Equal(999.99m, product.Price);
            },
            product =>
            {
                Assert.Equal("Mouse", product.Name);
                Assert.Equal(29.99m, product.Price);
            });
    }
}

public class InMemoryProductRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistProduct()
    {
        var repository = new InMemoryProductRepository();
        var product = new Product(Guid.NewGuid(), "Keyboard", 80m);

        var result = await repository.AddAsync(product);

        Assert.Same(product, result);
        var all = await repository.GetAllAsync();
        Assert.Single(all);
        Assert.Equal(product.Id, all[0].Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        var repository = new InMemoryProductRepository();

        var products = await repository.GetAllAsync();

        Assert.Empty(products);
    }
}
