using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("login")]
//[Authorize]
public class LoginController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var user = HttpContext.User;
        var name = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = user.FindFirst(ClaimTypes.Email)?.Value;
        var login = user.Claims.FirstOrDefault(c => c.Type.Contains("name"))?.Value;

        var allowedUsersEnv = Environment.GetEnvironmentVariable("ALLOWED_USERS");

        if (!string.IsNullOrWhiteSpace(allowedUsersEnv))
        {
            var allowedUsers = allowedUsersEnv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!allowedUsers.Contains(login, StringComparer.OrdinalIgnoreCase))
            {
                return Unauthorized("Utilisateur non autorisé");
            }
        }

        // Pour debug : afficher tous les claims
        var allClaims = user.Claims.Select(c => new { c.Type, c.Value });

        return Ok(new
        {
            Login = login,
            Email = email,
            NameIdentifier = name,
            Claims = allClaims
        });
    }
    [HttpGet("redirect")]
    public IActionResult RedirectToFrontend()
    {
        return Redirect("https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant");
    }
}
