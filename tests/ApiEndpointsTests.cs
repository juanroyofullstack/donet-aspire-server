using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NetAspireServer.Api.Contracts.Products;

namespace NetAspireServer.Application.Tests;

public sealed class ApiEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedResourceThatCanBeRetrieved()
    {
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/products", new CreateProductRequest("Laptop", 999.99m));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);

        var getResponse = await client.GetAsync(createResponse.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Name, retrieved.Name);
        Assert.Equal(created.Price, retrieved.Price);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/products", new CreateProductRequest(" ", -1m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed class ApiFactory : WebApplicationFactory<Program>;
