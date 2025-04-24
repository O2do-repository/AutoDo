using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/enterprise")]
public class EnterpriseController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;

    public EnterpriseController(IEnterpriseService enterpriseService)
    {
        _enterpriseService = enterpriseService;
    }

    [HttpGet]
    public IActionResult GetAllEnterprises()
    {
        var enterprises = _enterpriseService.GetAllEnterprises();

        var outputEnterprises = enterprises.Select(e => new DtoOutputEnterprise
        {
            EnterpriseUuid = e.EnterpriseUuid,
            Name = e.Name,
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Liste des entreprises récupérée avec succès.",
            data = outputEnterprises
        });
    }

    [HttpPost]
    public IActionResult CreateEnterprise([FromBody] DtoInputEnterprise enterpriseDto)
    {
        if (enterpriseDto == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Les données de l'entreprise sont invalides."
            });
        }

        try
        {
            var enterprise = new Enterprise
            {
                EnterpriseUuid = Guid.NewGuid(),
                Name = enterpriseDto.Name,
            };

            var newEnterprise = _enterpriseService.AddEnterprise(enterprise);

            return StatusCode(201, new
            {
                success = true,
                message = "Entreprise créée avec succès.",
                data = newEnterprise
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Erreur lors de la création de l'entreprise.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpDelete("{enterpriseUuid}")]
    public IActionResult DeleteEnterprise(Guid enterpriseUuid)
    {
        try
        {


            _enterpriseService.DeleteEnterprise(enterpriseUuid);

            return Ok(new
            {
                success = true,
                message = "Entreprise supprimée avec succès."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erreur lors de la suppression de l'entreprise.",
                details = ex.Message
            });
        }
    }
}
