using System.Net.Sockets;
using Microsoft.Azure.Cosmos;
using NetAspireServer.Domain.Entities;
using NetAspireServer.Infrastructure.Configuration;
using NetAspireServer.Infrastructure.Repositories;

namespace NetAspireServer.IntegrationTests;

public class CosmosIntegrationTests
{
    private const string Endpoint = "https://localhost:8081/";
    private const string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    [Fact]
    public async Task AddAsync_And_GetAllAsync_ShouldPersistProductsInCosmos()
    {
        if (!await IsCosmosEmulatorAvailableAsync())
        {
            const string message = "The Cosmos DB emulator is not available at https://localhost:8081/.";
            if (string.Equals(Environment.GetEnvironmentVariable("REQUIRE_COSMOS_EMULATOR"), "true", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(message);
            }

            return;
        }

        var client = new CosmosClient(Endpoint, Key, new CosmosClientOptions
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway
        });

        var databaseName = "netaspire_integration_tests";
        var containerName = "products";
        var options = new CosmosDbOptions
        {
            DatabaseName = databaseName,
            ContainerName = containerName
        };

        var database = client.GetDatabase(databaseName);
        await database.DeleteAsync();

        var repository = new CosmosProductRepository(new CosmosProductDataStore(client, options), options);
        var product = new Product(Guid.NewGuid(), "Integration Laptop", 1234.56m);

        var created = await repository.AddAsync(product);
        var products = await repository.GetAllAsync();

        Assert.Equal(product.Id, created.Id);
        Assert.Contains(products, p => p.Id == created.Id && p.Name == "Integration Laptop");

        await database.DeleteAsync();
    }

    private static async Task<bool> IsCosmosEmulatorAvailableAsync()
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", 8081).WaitAsync(TimeSpan.FromSeconds(2));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
