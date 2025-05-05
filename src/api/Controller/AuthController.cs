using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
    
    // Point d'entrée initial de l'authentification
    [HttpGet("login")]
    public IActionResult Login()
    {
        _logger.LogInformation("Démarrage de l'authentification GitHub");
        
        // Si l'utilisateur est déjà authentifié, on le redirige directement vers le frontend
        if (HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            return RedirectToFrontend();
        }
        
        // Sinon, on lance le processus d'authentification GitHub
        var callbackUrl = Url.Action("Callback", "Auth", null, Request.Scheme);
        return Challenge(
            new AuthenticationProperties { RedirectUri = callbackUrl }, 
            "GitHub"
        );
    }
    
    // Callback appelé après l'authentification GitHub
    [HttpGet("callback")]
    public IActionResult Callback()
    {
        _logger.LogInformation("Traitement du callback GitHub");
        
        var user = HttpContext.User;
        
        // Vérifier que l'utilisateur est bien authentifié
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Échec d'authentification dans le callback");
            return Unauthorized("L'authentification a échoué");
        }
        
        // Log des claims pour le débogage
        foreach (var claim in user.Claims)
        {
            _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
        }
        
        // Récupération du login GitHub
        var login = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(login))
        {
            _logger.LogWarning("Login GitHub non trouvé dans les claims");
            return Unauthorized("Identifiant utilisateur non disponible");
        }
        
        // Vérification des utilisateurs autorisés (liste blanche)
        var allowedUsers = Environment.GetEnvironmentVariable("ALLOWED_USERS");
        
        if (!string.IsNullOrEmpty(allowedUsers))
        {
            var authorizedUsers = allowedUsers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            if (!authorizedUsers.Contains(login, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Utilisateur {login} non autorisé");
                return Unauthorized("Votre compte GitHub n'est pas autorisé à accéder à cette application");
            }
        }
        
        // Tout est validé, redirection vers le frontend
        return RedirectToFrontend();
    }
    
    // Méthode utilitaire pour la redirection vers le frontend
    private IActionResult RedirectToFrontend()
    {
        var user = HttpContext.User;
        var login = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        // Construction de l'URL du frontend avec paramètres
        string frontendUrl = "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant";
        var queryParams = new List<string>();
        
        if (!string.IsNullOrEmpty(login))
            queryParams.Add($"login={Uri.EscapeDataString(login)}");
            
        if (!string.IsNullOrEmpty(email))
            queryParams.Add($"email={Uri.EscapeDataString(email)}");
            
        if (queryParams.Any())
            frontendUrl += "?" + string.Join("&", queryParams);
            
        _logger.LogInformation($"Redirection vers le frontend: {frontendUrl}");
        return Redirect(frontendUrl);
    }
    
    // Point de déconnexion (facultatif)
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/auth/login" },
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }
}