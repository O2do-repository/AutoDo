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
        var matchings = await _matchingService.GetAllMatchingsAsync();
        
        if (!matchings.Any())
        {
            return NotFound("Aucun matching trouvé.");
        }

        var dtos = matchings
            .Where(m => m.Profile?.Consultant != null)
            .Select(m => new DtoOutputMatching
            {
                JobTitle = m.Profile.JobTitle,
                ConsultantName = m.Profile.Consultant.Name,
                ConsultantSurname = m.Profile.Consultant.Surname,
                RfpReference = m.Rfp?.Reference,
                Score = m.Score,
                OfferDate = m.Rfp?.PublicationDate ?? DateTime.MinValue,
                Comment = m.Comment
            })
            .ToList();

        return Ok(dtos);
    }




}