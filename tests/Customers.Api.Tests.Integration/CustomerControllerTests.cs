using System.Collections;
using System.Net;
using System.Net.Http.Json;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerControllerTests : IAsyncLifetime, IDisposable, IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly HttpClient _httpClient;
    
    public CustomerControllerTests(WebApplicationFactory<IApiMarker> appFactory)
    {
        //  Sync Setup
        _httpClient = appFactory.CreateClient();
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenCustomerIsCreated()
    {
        // Arrange
        var customer = new CustomerRequest
        {
            FullName = "John Doe",
            Email = "john@doe.com",
            GitHubUsername = "johndoe",
            DateOfBirth = new DateTime(1989, 12, 11)
        };
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(customer);
            
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("2EFD541F-3A80-414E-8FE5-524C93D58379", Skip = "An example of how a case can be skipped")]
    [InlineData("19C1DBD6-7B12-4A4A-8D56-51736E7670D2")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists_InlineDataParameters(string guidAsText)
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Contain("Not Found");
        problem.Status.Should().Be(404);
    }

    [Theory(Skip = "An example of how all of cases can be skipped")]
    [MemberData(nameof(Data))]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists_MemberDataParameters(string guidAsText)
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [ClassData(typeof(TestData))]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists_ClassDataParameters(string guidAsText)
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

public class TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] {"2EFD541F-3A80-414E-8FE5-524C93D58379"};
        yield return new object[] {"19C1DBD6-7B12-4A4A-8D56-51736E7670D2"};
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}