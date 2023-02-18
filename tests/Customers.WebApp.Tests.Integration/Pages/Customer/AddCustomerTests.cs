using System.Globalization;
using Bogus;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Customers.WebApp.Tests.Integration.Pages.Customer;

[Collection("Test collection")]
public class AddCustomerTests
{
    private readonly SharedTestContext _testContext;
    
    private readonly Faker<Models.Customer> _customerGenerator = new Faker<Models.Customer>()
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.GitHubUsername, SharedTestContext.ValidGitHubUser)
        .RuleFor(x => x.DateOfBirth, faker => DateOnly.FromDateTime(faker.Person.DateOfBirth.Date));

    public AddCustomerTests(SharedTestContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async Task Create_CreateCustomer_WhenDataIsValid()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        await page.GotoAsync("add-customer");
        var customer = _customerGenerator.Generate();

        // Act
        await page.FillAsync("input[id=fullname]", customer.FullName);
        await page.FillAsync("input[id=email]", customer.Email);
        await page.FillAsync("input[id=github-username]", customer.GitHubUsername);
        await page.FillAsync("input[id=dob]", customer.DateOfBirth.ToString("yyyy-MM-dd"));

        await page.ClickAsync("button[type=submit]");

        // Assert
        var linkElement = page.Locator("article>p>a").First;
        var link = await linkElement.GetAttributeAsync("href");
        await page.GotoAsync(link!);

        (await page.Locator("p[id=fullname-field]").InnerTextAsync()).Should().Be(customer.FullName);
        (await page.Locator("p[id=email-field]").InnerTextAsync()).Should().Be(customer.Email);
        (await page.Locator("p[id=github-username-field]").InnerTextAsync()).Should().Be(customer.GitHubUsername);
        (await page.Locator("p[id=dob-field]").InnerTextAsync()).Should().Be(customer.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

        await page.CloseAsync();
    }

    [Fact]
    public async Task Create_ShowsError_WhenEmailIsInvalid()
    {
        // Arrange
        var page = await _testContext.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            BaseURL = SharedTestContext.AppUrl
        });
        await page.GotoAsync("add-customer");
        var customer = _customerGenerator.Generate();
        
        // Act
        await page.FillAsync("input[id=fullname]", customer.FullName);
        await page.FillAsync("input[id=email]", "invalidEmail");
        await page.FillAsync("input[id=github-username]", customer.GitHubUsername);
        await page.FillAsync("input[id=dob]", customer.DateOfBirth.ToString("yyyy-MM-dd"));
        
        // Assert
        var element = page.Locator("li.validation-message").First;
        var text = await element.InnerTextAsync();
        text.Should().Be("Invalid email format");
    }
}