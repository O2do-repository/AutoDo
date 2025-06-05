using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("matchingfeedback")]
public class MatchingFeedbackController : ControllerBase
{
    private readonly IMatchingFeedbackService _matchingFeedbackService;

    public MatchingFeedbackController(IMatchingFeedbackService matchingFeedbackService)
    {
        _matchingFeedbackService = matchingFeedbackService;
    }

    [HttpGet("{matchingUuid}")]
    public IActionResult GetMatchingFeedbackByMatchingId(Guid matchingUuid)
    {
        try
        {
            var feedback = _matchingFeedbackService.GetMatchingFeedbackByMatchingId(matchingUuid);

            if (feedback == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Aucun feedback trouvé pour ce matching."
                });
            }

            // Projection vers DTO
            var feedbackDto = new DtoOutputMatchingFeedback
            {
                MatchingFeedbackUuid = feedback.MatchingFeedbackUuid,
                MatchingUuid = feedback.MatchingUuid,
                TotalScore = feedback.TotalScore,
                JobTitleScore = feedback.JobTitleScore,
                ExperienceScore = feedback.ExperienceScore,
                SkillsScore = feedback.SkillsScore,
                LocationScore = feedback.LocationScore,
                JobTitleFeedback = feedback.JobTitleFeedback,
                ExperienceFeedback = feedback.ExperienceFeedback,
                SkillsFeedback = feedback.SkillsFeedback,
                LocationFeedback = feedback.LocationFeedback,
                CreatedAt = feedback.CreatedAt,
                LastUpdatedAt = feedback.LastUpdatedAt
            };

            return Ok(new
            {
                success = true,
                message = "MatchingFeedback récupéré avec succès.",
                data = feedbackDto
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erreur interne : " + ex.Message
            });
        }
    }
}
