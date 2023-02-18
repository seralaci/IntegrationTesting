using System.Globalization;
using Bogus;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Customers.WebApp.Tests.Integration.Pages.Customer;

[Collection("Test collection")]
public class GetAllCustomerTests
{
    private readonly SharedTestContext _testContext;
    
    private readonly Faker<Models.Customer> _customerGenerator = new Faker<Models.Customer>()
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.GitHubUsername, SharedTestContext.ValidGitHubUser)
        .RuleFor(x => x.DateOfBirth, faker => DateOnly.FromDateTime(faker.Person.DateOfBirth.Date));

    public GetAllCustomerTests(SharedTestContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task GetAll_ContainsCustomer_WhenCustomerExists()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        var customer = await CreateCustomer(page);

        // Act
        await page.GotoAsync("/customers");

        var name = page.Locator("tbody>tr>td").Filter(new LocatorFilterOptions {HasTextString = customer.FullName});
        var email = page.Locator("tbody>tr>td").Filter(new LocatorFilterOptions {HasTextString = customer.Email});
        var githubUser = page.Locator("tbody>tr>td").Filter(new LocatorFilterOptions {HasTextString = customer.GitHubUsername});
        var dateOfBirth = page.Locator("tbody>tr>td").Filter(new LocatorFilterOptions {HasTextString = customer.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)});

        // Assert
        (await name.InnerTextAsync()).Should().Be(customer.FullName);
        (await email.InnerTextAsync()).Should().Be(customer.Email);
        (await githubUser.InnerTextAsync()).Should().Be(customer.GitHubUsername);
        (await dateOfBirth.InnerTextAsync()).Should().Be(customer.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
        
        await page.CloseAsync();
    }
    
    private async Task<Models.Customer> CreateCustomer(IPage page)
    {
        await page.GotoAsync("add-customer");
        var customer = _customerGenerator.Generate();

        await page.FillAsync("input[id=fullname]", customer.FullName);
        await page.FillAsync("input[id=email]", customer.Email);
        await page.FillAsync("input[id=github-username]", customer.GitHubUsername);
        await page.FillAsync("input[id=dob]", customer.DateOfBirth.ToString("yyyy-MM-dd"));

        await page.ClickAsync("button[type=submit]");

        return customer;
    }
}