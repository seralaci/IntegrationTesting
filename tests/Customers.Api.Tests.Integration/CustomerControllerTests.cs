using System.Net;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerControllerTests : IAsyncLifetime, IDisposable
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:5001")
    };

    public CustomerControllerTests()
    {
        //  Sync Setup
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists()
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public Task InitializeAsync()
    {
        // Async Setup
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // Async cleanup
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Sync cleanup
        _httpClient.Dispose();
    }
}