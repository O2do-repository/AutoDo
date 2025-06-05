

using Microsoft.EntityFrameworkCore;

public class MatchingFeedbackService : IMatchingFeedbackService
{
    private readonly AutoDoDbContext _context;


    public MatchingFeedbackService(AutoDoDbContext context)
    {
        _context = context;
    }


    public MatchingFeedback GetMatchingFeedbackByMatchingId(Guid matchingUuid)
    {
        var matchingFeedback = _context.MatchingFeedbacks.SingleOrDefault(mf => mf.MatchingUuid == matchingUuid);

        if (matchingFeedback == null)
        {
            throw new Exception($"Aucun Feedback disponible pour ce matching.");
        }

        return matchingFeedback;
    }


 

}
