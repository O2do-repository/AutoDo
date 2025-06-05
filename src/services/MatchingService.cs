

using Microsoft.EntityFrameworkCore;

public class MatchingService :IMatchingService
{
    private readonly AutoDoDbContext _context;


    public MatchingService(AutoDoDbContext context)
    {
        _context = context;
    }
    public MatchingFeedback CalculateMatchingFeedback(Profile profile, RFP rfp, Guid matchingUuid)
    {
        var (jobScore, jobFeedback) = MatchingScoring.ScoreJobTitleMatch(profile, rfp);
        var (expScore, expFeedback) = MatchingScoring.ScoreExperienceMatch(profile, rfp);
        var (skillScore, skillFeedback) = MatchingScoring.ScoreSkillsMatch(profile, rfp);
        var (locScore, locFeedback) = MatchingScoring.ScoreLocationMatch(rfp);

        int total = jobScore + expScore + skillScore + locScore;

        return new MatchingFeedback
        {
            MatchingFeedbackUuid = Guid.NewGuid(),
            MatchingUuid = matchingUuid,
            TotalScore = Math.Min(total, 100),
            JobTitleScore = jobScore,
            ExperienceScore = expScore,
            SkillsScore = skillScore,
            LocationScore = locScore,
            JobTitleFeedback = jobFeedback,
            ExperienceFeedback = expFeedback,
            SkillsFeedback = skillFeedback,
            LocationFeedback = locFeedback,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
    }

    

    public async Task<List<Matching>> MatchingsForProfileAsync(Profile profile)
    {
        var rfps = await _context.Rfps.ToListAsync();

        var newMatchings = new List<Matching>();
        var newFeedbacks = new List<MatchingFeedback>();

        foreach (var rfp in rfps)
        {
            var matchingUuid = Guid.NewGuid();

            var feedback = CalculateMatchingFeedback(profile, rfp, matchingUuid);

            var matching = new Matching
            {
                MatchingUuid = matchingUuid,
                ProfileUuid = profile.ProfileUuid,
                RfpUuid = rfp.RFPUuid,
                Score = feedback.TotalScore,
                Comment = "", 
                StatutMatching = StatutMatching.New
            };

            newMatchings.Add(matching);
            newFeedbacks.Add(feedback);
        }

        // Supprimer les anciens matchings et feedbacks du profil
        var oldMatchings = _context.Matchings.Where(m => m.ProfileUuid == profile.ProfileUuid).ToList();
        var oldMatchingUuids = oldMatchings.Select(m => m.MatchingUuid).ToList();

        var oldFeedbacks = _context.MatchingFeedbacks.Where(fb => oldMatchingUuids.Contains(fb.MatchingUuid)).ToList();

        _context.MatchingFeedbacks.RemoveRange(oldFeedbacks);
        _context.Matchings.RemoveRange(oldMatchings);

        // Ajouter les nouveaux
        _context.Matchings.AddRange(newMatchings);
        _context.MatchingFeedbacks.AddRange(newFeedbacks);

        await _context.SaveChangesAsync();

        return newMatchings;
    }

    public async Task<List<Matching>> GetAllMatchingsAsync()
    {
        return await _context.Matchings
            .Include(m => m.Rfp)
            .Include(m => m.Profile)
                .ThenInclude(p => p.Consultant) 
            .ToListAsync();
    }
    public async Task<List<Matching>> GetAllMatchingsFiltered()
    {
        return await _context.Matchings
            .Where(m => m.Score != 0 && m.StatutMatching != StatutMatching.Rejected)
            .Include(m => m.Rfp)
            .Include(m => m.Profile)
                .ThenInclude(p => p.Consultant) 
            .OrderByDescending(m => m.Score) 
            .ToListAsync();
    }

    public async Task<Matching> UpdateMatchingAsync(Guid id,Matching updatedMatching)
    {
        var matching = await _context.Matchings.SingleOrDefaultAsync(m => m.MatchingUuid == id);

        if (matching == null)
        {
            throw new Exception($"Le matching avec UUID {updatedMatching.MatchingUuid} n'existe pas.");
        }

        
        matching.StatutMatching = updatedMatching.StatutMatching;
        matching.Comment = updatedMatching.Comment;
        matching.MatchingUuid = id;
        matching.Score = updatedMatching.Score;
        matching.ProfileUuid = updatedMatching.ProfileUuid;
        matching.RfpUuid = updatedMatching.RfpUuid;


        await _context.SaveChangesAsync();
        return matching;
    }
    public async Task<List<Matching>> MatchingsForRfpsAsync(List<RFP> rfps)
    {
        var profiles = await _context.Profiles.ToListAsync();

        var newMatchings = new List<Matching>();
        var newFeedbacks = new List<MatchingFeedback>();

        foreach (var profile in profiles)
        {
            foreach (var rfp in rfps)
            {
                var matchingUuid = Guid.NewGuid();
                var feedback = CalculateMatchingFeedback(profile, rfp, matchingUuid);

                var matching = new Matching
                {
                    MatchingUuid = matchingUuid,
                    ProfileUuid = profile.ProfileUuid,
                    RfpUuid = rfp.RFPUuid,
                    Score = feedback.TotalScore,
                    Comment = "",
                    StatutMatching = StatutMatching.New
                };

                newMatchings.Add(matching);
                newFeedbacks.Add(feedback);
            }

            // Supprimer les anciens matchings et feedbacks du profil pour les RFPs concernés
            var rfpUuids = rfps.Select(r => r.RFPUuid).ToList();

            var oldMatchings = _context.Matchings
                .Where(m => m.ProfileUuid == profile.ProfileUuid && rfpUuids.Contains(m.RfpUuid))
                .ToList();

            var oldMatchingUuids = oldMatchings.Select(m => m.MatchingUuid).ToList();

            var oldFeedbacks = _context.MatchingFeedbacks
                .Where(fb => oldMatchingUuids.Contains(fb.MatchingUuid))
                .ToList();

            _context.MatchingFeedbacks.RemoveRange(oldFeedbacks);
            _context.Matchings.RemoveRange(oldMatchings);
        }

        // Ajouter les nouveaux
        _context.Matchings.AddRange(newMatchings);
        _context.MatchingFeedbacks.AddRange(newFeedbacks);

        await _context.SaveChangesAsync();

        return newMatchings;
    }

 

}
