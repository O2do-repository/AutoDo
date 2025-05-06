using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private static string JwtSecretKey => Environment.GetEnvironmentVariable("JWTSETTINGS_SECRET");
    private static string Issuer => Environment.GetEnvironmentVariable("JWTSETTINGS_ISSUER");
    private static string Audience => Environment.GetEnvironmentVariable("JWTSETTINGS_AUDIENCE");

    [HttpGet("token")]
    public IActionResult GetToken()
    {
        var principal = HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL"];
        if (string.IsNullOrEmpty(principal)) return Unauthorized();

        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(principal));
        var user = JsonSerializer.Deserialize<EasyAuthUser>(decoded);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserDetails ?? ""),
            new Claim("provider", user.IdentityProvider ?? ""),
            new Claim("userid", user.UserId ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        // On définit le cookie pour les requêtes côté serveur
        Response.Cookies.Append("autodo_token", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        // On retourne aussi le token pour le frontend
        return Ok(new { token = jwt });
    }

    // Cette route utilisera JWT (pas EasyAuth)
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized();

        var name = User.Identity.Name;
        var provider = User.FindFirst("provider")?.Value;
        var userId = User.FindFirst("userid")?.Value;

        return Ok(new { login = name, provider, userId });
    }
}

public class EasyAuthUser
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; } // GitHub username/email
    public string[] UserRoles { get; set; }
}