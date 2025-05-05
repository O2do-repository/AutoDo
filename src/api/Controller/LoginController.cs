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
            
            // Log authentication status
            _logger.LogInformation($"User authenticated: {user?.Identity?.IsAuthenticated}");
            
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt: User not authenticated");
                return Unauthorized("Non authentifié");
            }
            
            // Extract user information
            var login = user.Claims.FirstOrDefault(c => c.Type.Contains("name"))?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            
            _logger.LogInformation($"User login attempt: {login}, Email: {email}");
            
            // Check if user is in allowed list
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
            
            // Utilisez simplement une chaîne pour l'URL au lieu d'un objet Uri
            string frontendUrl = "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant";
            
            // Ajoutez les paramètres si login et email ne sont pas null
            if (!string.IsNullOrEmpty(login) || !string.IsNullOrEmpty(email))
            {
                frontendUrl += "?";
                
                if (!string.IsNullOrEmpty(login))
                {
                    frontendUrl += $"login={Uri.EscapeDataString(login)}";
                }
                
                if (!string.IsNullOrEmpty(email))
                {
                    if (!string.IsNullOrEmpty(login))
                    {
                        frontendUrl += "&";
                    }
                    frontendUrl += $"email={Uri.EscapeDataString(email)}";
                }
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