

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
            Comment = ""
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

    

}
