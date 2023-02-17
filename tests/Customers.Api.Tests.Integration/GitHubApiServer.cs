using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customers.Api.Tests.Integration;

public class GitHubApiServer : IDisposable
{
    private WireMockServer _server;

    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupUser(string username)
    {
        _server
            .Given(Request
                .Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response
                .Create()
                .WithBody(GenerateGitHubUserResponseBody(username))
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(200));
    }
    
    public void SetupThrottledUser(string username)
    {
        _server
            .Given(Request
                .Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response
                .Create()
                .WithBody(@"{
                                ""message"": ""API rate limit exceeded for xxx.xxx.xxx.xxx. (But here's the good news: Authenticated requests get a higher rate limit. Check out the documentation for more details.)"",
                                ""documentation_url"": ""https://docs.github.com/rest/overview/resources-in-the-rest-api#rate-limiting""
                            }")
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(403));
    }
    
    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    private static string GenerateGitHubUserResponseBody(string username)
    {
        return $@"{{
                    ""login"": ""{username}"",
                    ""id"": 99999,
                    ""node_id"": ""MDQ6VXNlcjg3MDA1MQ=="",
                    ""avatar_url"": ""https://avatars.githubusercontent.com/u/870051?v=4"",
                    ""gravatar_id"": """",
                    ""url"": ""https://api.github.com/users/{username}"",
                    ""html_url"": ""https://github.com/{username}"",
                    ""followers_url"": ""https://api.github.com/users/{username}/followers"",
                    ""following_url"": ""https://api.github.com/users/{username}/following{{/other_user}}"",
                    ""gists_url"": ""https://api.github.com/users/{username}/gists{{/gist_id}}"",
                    ""starred_url"": ""https://api.github.com/users/{username}/starred{{/owner}}{{/repo}}"",
                    ""subscriptions_url"": ""https://api.github.com/users/{username}/subscriptions"",
                    ""organizations_url"": ""https://api.github.com/users/{username}/orgs"",
                    ""repos_url"": ""https://api.github.com/users/{username}/repos"",
                    ""events_url"": ""https://api.github.com/users/{username}/events{{/privacy}}"",
                    ""received_events_url"": ""https://api.github.com/users/{username}/received_events"",
                    ""type"": ""User"",
                    ""site_admin"": false,
                    ""name"": ""John Doe"",
                    ""company"": null,
                    ""blog"": """",
                    ""location"": ""Budapest, Hungary"",
                    ""email"": null,
                    ""hireable"": null,
                    ""bio"": null,
                    ""twitter_username"": null,
                    ""public_repos"": 19,
                    ""public_gists"": 0,
                    ""followers"": 1,
                    ""following"": 0,
                    ""created_at"": ""2011-06-23T09:07:48Z"",
                    ""updated_at"": ""2023-02-11T16:14:51Z""
                }}";
    }
}