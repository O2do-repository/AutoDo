using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer ")) return Unauthorized();

        var token = authHeader.Substring("Bearer ".Length).Trim();
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var userInfo = JsonSerializer.Deserialize<EasyAuthUser>(json);

            return Ok(new
            {
                login = userInfo?.UserDetails,
                provider = userInfo?.IdentityProvider,
                roles = userInfo?.UserRoles
            });
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpGet("token")]
    public IActionResult GetToken()
    {
        var principal = HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL"];
        if (string.IsNullOrEmpty(principal)) return Unauthorized();

        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(principal));
        var user = JsonSerializer.Deserialize<EasyAuthUser>(decoded);

        // Encode les infos utilisateur en base64 JSON → ce sera notre "token"
        var json = JsonSerializer.Serialize(user);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        // Redirection vers GitHub Pages avec token dans l'URL
        var redirectUrl = $"https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant?token={token}";
        return Redirect(redirectUrl);
    }

}

public class EasyAuthUser
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; } // e.g., GitHub username or email
    public string[] UserRoles { get; set; }
}
