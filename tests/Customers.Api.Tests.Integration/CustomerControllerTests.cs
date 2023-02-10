using System.Net;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerControllerTests
{
    [Fact]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExists()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001")
        };

        var response = await httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}