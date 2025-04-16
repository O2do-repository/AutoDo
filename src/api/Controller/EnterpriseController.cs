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

        return Ok(outputEnterprises);
    }
    [HttpPost]
    public IActionResult CreateEnterprise([FromBody] DtoInputEnterprise enterpriseDto)
    {
        if (enterpriseDto == null)
        {
            throw new ArgumentException("Enterprise data is invalid");
        }

        try
        {
            var enterprise = new Enterprise
            {
                EnterpriseUuid = Guid.NewGuid(),
                Name = enterpriseDto.Name,
            };

            var newEnterprise = _enterpriseService.AddEnterprise(enterprise);

            return Ok(newEnterprise);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }


    [HttpDelete("{enterpriseUuid}")]
    public IActionResult DeleteEnterprise(Guid enterpriseUuid)
    {
        try
        {
            _enterpriseService.DeleteEnterprise(enterpriseUuid);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
