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

    [Theory]
    [InlineData("2EFD541F-3A80-414E-8FE5-524C93D58379")]
    [InlineData("19C1DBD6-7B12-4A4A-8D56-51736E7670D2")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists_InlineDataParameters(string guidAsText)
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists_MemberDataParameters(string guidAsText)
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

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

    public static IEnumerable<object[]> Data { get; } = new[]
    {
        new[] {"2EFD541F-3A80-414E-8FE5-524C93D58379"},
        new[] {"19C1DBD6-7B12-4A4A-8D56-51736E7670D2"}
    };
}