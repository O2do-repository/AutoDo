

using Microsoft.EntityFrameworkCore;

public class MatchingService :IMatchingService
{
    private readonly AutoDoDbContext _context;


    public MatchingService(AutoDoDbContext context)
    {
        _context = context;
    }
    public int CalculateMatchingScore(Profile profile, RFP rfp)
    {
        int score = 0;

        score += MatchingScoring.ScoreJobTitleMatch(profile, rfp);  // /20
        score += MatchingScoring.ScoreExperienceMatch(profile, rfp); // /20
        score += MatchingScoring.ScoreSkillsMatch(profile, rfp); // /40
        score += MatchingScoring.ScoreLocationMatch(rfp); // 20

        return Math.Min(score, 100);
    }
    

    public async Task<List<Matching>> MatchingsForProfileAsync(Profile profile)
    {
        var rfps = await _context.Rfps.ToListAsync();

        var newMatchings = rfps.Select(rfp => new Matching
        {
            MatchingUuid = Guid.NewGuid(),
            ProfileUuid = profile.ProfileUuid,
            RfpUuid = rfp.RFPUuid,
            Score = CalculateMatchingScore(profile, rfp),
            Comment = "",
            StatutMatching = StatutMatching.New
        }).ToList();

        // Supprimer les anciens matchings du profil (on écrase l'existant)
        _context.Matchings.RemoveRange(_context.Matchings.Where(m => m.ProfileUuid == profile.ProfileUuid));

        // Ajouter les nouveaux matchings
        _context.Matchings.AddRange(newMatchings);

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
        
        foreach (var profile in profiles)
        {
            var matchingsForProfile = rfps.Select(rfp => new Matching
            {
                MatchingUuid = Guid.NewGuid(),
                ProfileUuid = profile.ProfileUuid,
                RfpUuid = rfp.RFPUuid,
                Score = CalculateMatchingScore(profile, rfp),
                Comment = "",
                StatutMatching = StatutMatching.New
            }).ToList();

            var rfpUuids = rfps.Select(rfp => rfp.RFPUuid).ToHashSet();
            _context.Matchings.RemoveRange(_context.Matchings.Where(m => m.ProfileUuid == profile.ProfileUuid && rfpUuids.Contains(m.RfpUuid)));

            newMatchings.AddRange(matchingsForProfile);
        }

        if (newMatchings.Count > 0)
            _context.Matchings.AddRange(newMatchings);
        
        await _context.SaveChangesAsync();

        return newMatchings;
    }

 

}
