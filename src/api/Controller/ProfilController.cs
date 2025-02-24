using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/profil")]
public class ProfilController : ControllerBase
{
    private readonly IProfilService _profilService;

    public ProfilController(IProfilService profilService)
    {
        _profilService = profilService;
    }

    [HttpGet]
    public IActionResult GetAllProfils()
    {
        var profils = _profilService.GetAllProfils();
        return Ok(profils);
    }
}