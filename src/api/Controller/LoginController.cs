using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("login")]
[Authorize]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var user = HttpContext.User;

            _logger.LogInformation($"User authenticated: {user?.Identity?.IsAuthenticated}");

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt: User not authenticated");
                return Unauthorized("Non authentifié");
            }

            // Log all claims
            foreach (var claim in user.Claims)
            {
                _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
            }

            var login = user.Claims.FirstOrDefault(c => c.Type.Contains("name", StringComparison.OrdinalIgnoreCase))?.Value;
            var email = user.Claims.FirstOrDefault(c => c.Type.Contains("email", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(login))
            {
                _logger.LogWarning("Impossible de récupérer le login utilisateur.");
                return Unauthorized("Identifiant utilisateur non reconnu.");
            }

            // Check allowed users
            var allowedUsersEnv = Environment.GetEnvironmentVariable("ALLOWED_USERS");
            if (!string.IsNullOrWhiteSpace(allowedUsersEnv))
            {
                _logger.LogInformation($"Allowed users: {allowedUsersEnv}");
                var allowedUsers = allowedUsersEnv
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (!allowedUsers.Contains(login, StringComparer.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"User {login} not in allowed users list");
                    return Unauthorized("Utilisateur non autorisé");
                }
            }
            else
            {
                _logger.LogInformation("No allowed users list configured");
            }

            string frontendUrl = "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant";

            if (!string.IsNullOrEmpty(login) || !string.IsNullOrEmpty(email))
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(login))
                    queryParams.Add($"login={Uri.EscapeDataString(login)}");
                if (!string.IsNullOrEmpty(email))
                    queryParams.Add($"email={Uri.EscapeDataString(email)}");

                frontendUrl += "?" + string.Join("&", queryParams);
            }

            _logger.LogInformation($"Redirecting to: {frontendUrl}");
            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in login process");
            return StatusCode(500, "Une erreur s'est produite lors du traitement de la connexion. Veuillez réessayer ultérieurement.");
        }
    }
}
