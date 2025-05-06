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
        if (string.IsNullOrWhiteSpace(JwtSecretKey) || string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience))
        {
            return StatusCode(500, "JWT configuration is missing.");
        }

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

        // Cookie accessible côté serveur uniquement
        // Response.Cookies.Append("autodo_token", jwt, new CookieOptions
        // {
        //     HttpOnly = true,
        //     Secure = true,
        //     SameSite = SameSiteMode.None, // Pour que le frontend GitHub Pages puisse l'envoyer
        //     Expires = DateTimeOffset.UtcNow.AddHours(1)
        // });

        // Rediriger vers ton app avec le token dans l’URL
        var redirectUrl = $"https://o2do-repository.github.io/AutoDo/#/auth-redirect?token={jwt}";
        return Redirect(redirectUrl);
    }



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