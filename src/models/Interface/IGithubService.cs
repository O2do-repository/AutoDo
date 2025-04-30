public interface IGitHubService
{
    Task<bool> IsMemberOfOrg(string username);
}