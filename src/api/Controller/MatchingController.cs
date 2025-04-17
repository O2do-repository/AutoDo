using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/matching")]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;

    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DtoOutputMatching>>> GetAllMatchings()
    {
        var matchings = await _matchingService.GetAllMatchingsFiltered();
        
    

        var dtos = matchings
            .Where(m => m.Profile?.Consultant != null)
            .Select(m => new DtoOutputMatching
            {
                JobTitle = m.Profile.JobTitle,
                ConsultantName = m.Profile.Consultant.Name,
                ConsultantSurname = m.Profile.Consultant.Surname,
                MatchingUuid = m.MatchingUuid,
                RfpReference = m.Rfp?.Reference,
                Score = m.Score,
                OfferDate = m.Rfp?.PublicationDate ?? DateTime.MinValue,
                Comment = m.Comment,
                StatutMatching = m.StatutMatching,
                ProfileUuid = m.ProfileUuid,
                RfpUuid = m.RfpUuid
            })
            .ToList();

        return Ok(dtos);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMatching(Guid id, [FromBody] DtoUpdateMatching dto)
    {
        if (dto == null)
        {
            return BadRequest("Données de matching invalides.");
        }

        try
        {
            var matching = new Matching
            {
                MatchingUuid = id,
                StatutMatching = dto.StatutMatching,
                Comment = dto.Comment,
                Score = dto.Score,
                ProfileUuid = dto.ProfileUuid,
                RfpUuid = dto.RfpUuid
            };

            var updatedMatching = await _matchingService.UpdateMatchingAsync(id, matching);

            return Ok(updatedMatching);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }

    }


}