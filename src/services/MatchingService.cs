

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

        // 1. Matching sur l'expérience
        if (profile.ExperienceLevel == rfp.ExperienceLevel)
            score += 40;

        // 2. Matching sur le titre du job
        if (!string.IsNullOrEmpty(profile.JobTitle) && !string.IsNullOrEmpty(rfp.JobTitle))
        {
            string profileJobTitle = profile.JobTitle.ToLower();
            string rfpJobTitle = rfp.JobTitle.ToLower();

            if (profileJobTitle == rfpJobTitle)
                score += 40; // Correspondance exacte
            else if (profileJobTitle.Contains(rfpJobTitle) || rfpJobTitle.Contains(profileJobTitle))
                score += 20; // Correspondance partielle
        }

        // 3. Matching sur les skills 
        if (profile.Skills != null && rfp.Skills != null)
        {
            var profileSkillsLower = profile.Skills.Select(s => s.ToLower()).ToHashSet();
            var rfpSkillsLower = rfp.Skills.Select(s => s.ToLower()).ToHashSet();

            int matchingSkills = profileSkillsLower.Intersect(rfpSkillsLower).Count();
            if (matchingSkills > 0)
            {
                score += matchingSkills * 10; // 10 points par skill commun
            }
        }

        return Math.Min(score, 100); // On limite le score max à 100
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
        // Récupérer tous les profils en une seule requête
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

            // Supprimer les anciens matchings du profil liés aux nouvelles RFPs
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
