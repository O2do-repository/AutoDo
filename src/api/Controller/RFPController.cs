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
    [HttpPost("import")]
    public async Task<IActionResult> ImportRfpFromJson()
    {
        try
        {
            await _rfpService.ImportRfpAndGenerateMatchings();

            return Ok("Importation des RFPs et génération des matchings réussie !");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erreur : {ex.Message}");
        }
    }
    [HttpPost("import/json")]
    public async Task<IActionResult> ImportFromRawJson([FromBody] List<RFP> rfps)
    {
        if (rfps == null || rfps.Count == 0)
            return BadRequest("Le JSON reçu est vide ou invalide.");

        try
        {
            await _rfpService.ImportFromJsonData(rfps);
            return Ok("Importation depuis JSON brut réussie !");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur d'importation : {ex.Message}");
        }
    }

}
