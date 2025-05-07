using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/consultant")]
public class ConsultantController : ControllerBase
{
    private readonly IConsultantService _consultantService;

    public ConsultantController(IConsultantService consultantService)
    {
        _consultantService = consultantService;
    }

    [HttpGet]
    public IActionResult GetAllConsultants()
    {
        var consultants = _consultantService.GetAllConsultants();

        var outputConsultants = consultants.Select(consultant => new DtoOutputConsultant
        {
            ConsultantUuid = consultant.ConsultantUuid,
            Email = consultant.Email,
            AvailabilityDate = consultant.AvailabilityDate,
            ExpirationDateCI = consultant.ExpirationDateCI,
            Intern = consultant.Intern,
            Name = consultant.Name,
            Surname = consultant.Surname,
            enterprise = consultant.enterprise,
            Phone = consultant.Phone,
            CopyCI = consultant.CopyCI,
            Picture = consultant.Picture
        }).ToList();

        return Ok(new {
            success = true,
            message = "Liste des consultants récupérée avec succès.",
            data = outputConsultants
        });
    }

    [HttpPost]
    public IActionResult CreateConsultant([FromBody] DtoInputConsultant consultantDto)
    {
        if (consultantDto == null)
        {
            return BadRequest(new { success = false, message = "Les données envoyées sont invalides." });
        }

        try
        {
            var consultant = new Consultant
            {
                ConsultantUuid = Guid.NewGuid(),
                Email = consultantDto.Email,
                AvailabilityDate = consultantDto.AvailabilityDate,
                ExpirationDateCI = consultantDto.ExpirationDateCI,
                Intern = consultantDto.Intern,
                Name = consultantDto.Name,
                Surname = consultantDto.Surname,
                enterprise = consultantDto.enterprise,
                Phone = consultantDto.Phone,
                CopyCI = consultantDto.CopyCI,
                Picture = consultantDto.Picture
            };

            var newConsultant = _consultantService.AddConsultant(consultant);

            return CreatedAtAction(nameof(GetConsultantById), new { id = newConsultant.ConsultantUuid }, new {
                success = true,
                message = "Consultant créé avec succès.",
                data = newConsultant
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                success = false,
                message = "Erreur lors de la création du consultant.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetConsultantById(Guid id)
    {
        var consultant = _consultantService.GetConsultantById(id);
        if (consultant == null)
        {
            return NotFound(new { success = false, message = "Consultant non trouvé." });
        }
        return Ok(new { success = true, message = "Consultant récupéré avec succès.", data = consultant });
    }

    [HttpDelete("{consultantUuid}")]
    public IActionResult DeleteConsultant(Guid consultantUuid)
    {
        try
        {
            var consultant = _consultantService.GetConsultantById(consultantUuid);
            if (consultant == null)
            {
                return NotFound(new { success = false, message = "Consultant non trouvé." });
            }

            _consultantService.DeleteConsultant(consultantUuid);
            return Ok(new { success = true, message = "Consultant supprimé avec succès." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                success = false,
                message = "Erreur lors de la suppression du consultant.",
                details = ex.Message
            });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConsultant([FromBody] DtoUpdateConsultant ConsultantDto)
    {
        if (ConsultantDto == null)
        {
            return BadRequest(new { success = false, message = "Données de consultant invalides." });
        }

        try
        {
            var existingConsultant = _consultantService.GetConsultantById(ConsultantDto.ConsultantUuid);
            if (existingConsultant == null)
            {
                return NotFound(new { success = false, message = "Consultant non trouvé." });
            }

            var updatedConsultant = new Consultant
            {
                ConsultantUuid = ConsultantDto.ConsultantUuid,
                Email = ConsultantDto.Email,
                AvailabilityDate = ConsultantDto.AvailabilityDate,
                ExpirationDateCI = ConsultantDto.ExpirationDateCI,
                Intern = ConsultantDto.Intern,
                Name = ConsultantDto.Name,
                Surname = ConsultantDto.Surname,
                CopyCI = ConsultantDto.CopyCI,
                Picture = ConsultantDto.Picture,
                enterprise = ConsultantDto.enterprise,
                Phone = ConsultantDto.Phone
            };

            var result = _consultantService.UpdateConsultant(updatedConsultant);

            return Ok(new {
                success = true,
                message = "Consultant mis à jour avec succès.",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                success = false,
                message = "Erreur lors de la mise à jour du consultant.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }
}
