using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("login")]
public class LoginController : ControllerBase
{
    private readonly IGitHubService _githubService;

    public LoginController(IGitHubService githubService)
    {
        _githubService = githubService;
    }

    [HttpGet("complete")]
    public async Task<IActionResult> CompleteLogin([FromQuery] string redirect = "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant")
    {
        try
        {
            var username = User.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Redirect("https://o2do-repository.github.io/AutoDo/#/login?error=no-user");
            }

            var isAuthorized = await _githubService.IsMemberOfOrg(username);
            if (!isAuthorized)
            {
                return Redirect("https://o2do-repository.github.io/AutoDo/#/login?error=not-in-org");
            }

            return Redirect(redirect);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur dans /login/complete : " + ex.Message);
            return Redirect("https://o2do-repository.github.io/AutoDo/#/login?error=server");
        }
    }

}
