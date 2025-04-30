using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _http;
    private readonly string _accessToken;

    public GitHubService(IConfiguration config)
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("AutoDoApp");
        _accessToken = config["GitHub:AccessToken"]; 
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<bool> IsMemberOfOrg(string username)
    {
        var url = $"https://api.github.com/orgs/O2do/members/{username}";
        var res = await _http.GetAsync(url);
        return res.StatusCode == HttpStatusCode.NoContent;
    }
}
