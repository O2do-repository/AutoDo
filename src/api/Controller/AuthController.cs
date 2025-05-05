using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(principal));
        var userInfo = JsonSerializer.Deserialize<EasyAuthUser>(decoded);

        return Ok(new
        {
            login = userInfo?.UserDetails,
            provider = userInfo?.IdentityProvider,
            roles = userInfo?.UserRoles
        });
    }

    [HttpGet("redirect")]
    public IActionResult RedirectToFrontend([FromQuery] string return_to = null)
    {
        var principalHeader = HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
        if (string.IsNullOrEmpty(principalHeader))
            return Unauthorized("En-tête d'authentification manquant");

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(principalHeader));
            var principal = JsonSerializer.Deserialize<EasyAuthPrincipal>(decoded);

            string login = null, email = null;

            foreach (var claim in principal.Claims)
            {
                if (claim.Type == "name") login = claim.Value;
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                    email = claim.Value;
            }

            string frontendUrl = return_to ??
                "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant";

            var query = new List<string>();
            if (!string.IsNullOrEmpty(login)) query.Add($"login={Uri.EscapeDataString(login)}");
            if (!string.IsNullOrEmpty(email)) query.Add($"email={Uri.EscapeDataString(email)}");

            if (query.Any()) frontendUrl += (frontendUrl.Contains("?") ? "&" : "?") + string.Join("&", query);

            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Erreur lors de la redirection", ex.Message });
        }
    }
}

public class EasyAuthUser
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; } // e.g., GitHub username
    public string[] UserRoles { get; set; }
}

public class EasyAuthPrincipal
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; }
    public string[] UserRoles { get; set; }
    public List<EasyAuthClaim> Claims { get; set; }
}

public class EasyAuthClaim
{
    public string Typ { get; set; }
    public string typ { get; set; }
    public string Type => Typ ?? typ;
    public string Value { get; set; }
}
