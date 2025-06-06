using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("/rfp")] 
public class RfpController : ControllerBase
{
    private readonly IRfpService _rfpService;
    private readonly IMatchingService _matchingService;

    public RfpController(IRfpService rfpService, IMatchingService matchingService)
    {
        _rfpService = rfpService;
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredRfpsAsync()
    {
        try
        {
            var filteredRfps = await _rfpService.FilterRfpDeadlineNotReachedYet();
            return Ok(filteredRfps);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur est survenue.", error = ex.Message });
        }
    }
    [HttpPost("import/json")]
    public async Task<IActionResult> ImportFromRawJson([FromBody] List<DtoInputRFP> rfps)
    {
        if (rfps == null || rfps.Count == 0)
            return BadRequest("Le JSON reçu est vide ou invalide.");

        try
        {
            var mappedRfps = rfps.Select(dto => new RFP
            {
                Reference = dto.Reference,
                DeadlineDate = dto.DeadlineDate,
                DescriptionBrut = dto.DescriptionBrut,
                ExperienceLevel = dto.ExperienceLevel,
                Skills = dto.Skills,
                JobTitle = dto.JobTitle,
                RfpUrl = dto.RfpUrl,
                Workplace = dto.Workplace,
                PublicationDate = dto.PublicationDate,
                RfpPriority = dto.RfpPriority
            }).ToList();

            await _rfpService.ImportFromJsonData(mappedRfps);

            return Ok("Importation depuis JSON brut réussie !");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur d'importation : {ex.Message}");
        }
    }


}
