using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("login")]
[Authorize]
public class LoginController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var user = HttpContext.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
            return Unauthorized("Non authentifié");

        var login = user.Claims.FirstOrDefault(c => c.Type.Contains("name"))?.Value;
        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        var allowedUsersEnv = Environment.GetEnvironmentVariable("ALLOWED_USERS");
        if (!string.IsNullOrWhiteSpace(allowedUsersEnv))
        {
            var allowedUsers = allowedUsersEnv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!allowedUsers.Contains(login, StringComparer.OrdinalIgnoreCase))
                return Unauthorized("Utilisateur non autorisé");
        }

        var frontendUrl = $"https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant?login={login}&email={email}";
        return Redirect(frontendUrl);
    }

}
