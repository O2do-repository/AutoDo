using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public GitHubService(IConfiguration configuration)
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("AutoDoApp");
        _configuration = configuration;
    }

    public async Task<bool> IsMemberOfOrg(string username)
    {
        // Utiliser le token d'accès pour l'API GitHub
        var token = _configuration["GitHub:AccessToken"];
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var url = $"https://api.github.com/orgs/O2do/members/{username}";
        var res = await _http.GetAsync(url);
        
        // GitHub répond avec NoContent (204) si l'utilisateur est membre, NotFound (404) sinon
        return res.StatusCode == HttpStatusCode.NoContent;
    }
}
