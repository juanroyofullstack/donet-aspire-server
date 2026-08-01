using NetAspireServer.Infrastructure.Configuration;

namespace NetAspireServer.Application.Tests;

public class CosmosDbOptionsTests
{
    [Fact]
    public void IsConfigured_ReturnsFalse_WhenRequiredValuesAreMissing()
    {
        var options = new CosmosDbOptions();

        Assert.False(options.IsConfigured);
    }

    [Fact]
    public void IsConfigured_ReturnsTrue_WhenRequiredValuesArePresent()
    {
        var options = new CosmosDbOptions
        {
            ConnectionString = "AccountEndpoint=https://example.documents.azure.com:443/;AccountKey=test-key;",
            DatabaseName = "netaspire",
            ContainerName = "products"
        };

        Assert.True(options.IsConfigured);
    }
}
