using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        var principal = HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL"];
        if (string.IsNullOrEmpty(principal)) return Unauthorized();

        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(principal));
        var userInfo = JsonSerializer.Deserialize<EasyAuthUser>(decoded);

        return Ok(new
        {
            login = userInfo?.UserDetails,
            provider = userInfo?.IdentityProvider,
            roles = userInfo?.UserRoles
        });
    }
}

public class EasyAuthUser
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; } // e.g., GitHub username or email
    public string[] UserRoles { get; set; }
}
