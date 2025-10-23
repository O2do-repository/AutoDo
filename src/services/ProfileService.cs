

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class ProfileService : IProfileService
{
    private readonly AutoDoDbContext _context;
    private readonly ITranslationService _translationService;


    public ProfileService(AutoDoDbContext context, ITranslationService translationService)
    {
        _context = context;
        _translationService = translationService;
    }

    // Get profile
    public List<Profile> GetAllProfiles()
    {
        return _context.Profiles.ToList();
    }
    public List<Profile> GetProfilesByConsultant(Guid consultantUuid)
    {
        var profiles = _context.Profiles
            .Where(p => p.ConsultantUuid == consultantUuid)
            .Include(p => p.Skills)
            .Include(p => p.Keywords)
            .ToList();

        if (!profiles.Any())
        {

            return new List<Profile>();
        }

        return profiles;
    }


    // Add a new profile 
    public async Task<Profile> AddProfile(Profile profile, List<Guid> skillUuids, List<Guid> keywordUuids)
{
    var consultant = await _context.Consultants
        .SingleOrDefaultAsync(c => c.ConsultantUuid == profile.ConsultantUuid);

    if (consultant == null)
        throw new Exception($"Le consultant avec UUID {profile.ConsultantUuid} n'existe pas.");

    // Traduction asynchrone
    profile.JobTitleFr = await _translationService.TranslateTextAsync(profile.JobTitle, "fr");
    profile.JobTitleEn = await _translationService.TranslateTextAsync(profile.JobTitle, "en");
    profile.JobTitleNl = await _translationService.TranslateTextAsync(profile.JobTitle, "nl");

    profile.ProfileUuid = Guid.NewGuid();
    profile.Consultant = consultant;

    if (skillUuids != null && skillUuids.Any())
    {
        profile.Skills = skillUuids.Select(id => new Skill { SkillUuid = id }).ToList();

        // On attache les Skills existants pour éviter qu'EF essaie de les insérer
        foreach (var skill in profile.Skills)
        {
            _context.Attach(skill);
        }
    }

    if (keywordUuids != null && keywordUuids.Any())
    {
        profile.Keywords = keywordUuids.Select(id => new Keyword { KeywordUuid = id }).ToList();

        foreach (var keyword in profile.Keywords)
        {
            _context.Attach(keyword);
        }
    }

    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    return profile;
}





    public async Task<Profile> UpdateProfile(Profile updatedProfile, List<Guid> skillUuids, List<Guid> keywordUuids)
    {
        var existingProfile = _context.Profiles
            .FirstOrDefault(p => p.ProfileUuid == updatedProfile.ProfileUuid);

        if (existingProfile == null)
            throw new Exception($"Le profil avec UUID {updatedProfile.ProfileUuid} n'existe pas.");

        // Mise à jour des champs simples
        if (existingProfile.JobTitle != updatedProfile.JobTitle)
        {
            existingProfile.JobTitle = updatedProfile.JobTitle;
            existingProfile.JobTitleFr = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "fr");
            existingProfile.JobTitleEn = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "en");
            existingProfile.JobTitleNl = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "nl");
        }

        existingProfile.Ratehour = updatedProfile.Ratehour;
        existingProfile.CV = updatedProfile.CV;
        existingProfile.CVDate = updatedProfile.CVDate;
        existingProfile.ExperienceLevel = updatedProfile.ExperienceLevel;

        // Mise à jour des Skills
        existingProfile.Skills = skillUuids.Select(id => new Skill { SkillUuid = id }).ToList();
        foreach (var skill in existingProfile.Skills)
        {
            _context.Attach(skill);
        }

        // Mise à jour des Keywords
        existingProfile.Keywords = keywordUuids.Select(id => new Keyword { KeywordUuid = id }).ToList();
        foreach (var keyword in existingProfile.Keywords)
        {
            _context.Attach(keyword);
        }

        _context.Profiles.Update(existingProfile);
        await _context.SaveChangesAsync();

        return existingProfile;
    }




    // Delete profile
    public void DeleteProfile(Guid profileUuid)
    {
        var existingProfile = _context.Profiles
            .FirstOrDefault(p => p.ProfileUuid == profileUuid);

        if (existingProfile == null)
        {
            throw new Exception($"Le profil avec UUID {profileUuid} n'existe pas.");
        }

        _context.Profiles.Remove(existingProfile);
        _context.SaveChanges();
    }
    public async Task<int> UpdateAllProfiles()
    {
        var profiles = _context.Profiles.ToList();

        foreach (var profile in profiles)
        {
            profile.JobTitleFr = await _translationService.TranslateTextAsync(profile.JobTitle, "fr");
            profile.JobTitleEn = await _translationService.TranslateTextAsync(profile.JobTitle, "en");
            profile.JobTitleNl = await _translationService.TranslateTextAsync(profile.JobTitle, "nl");

        }

        return await _context.SaveChangesAsync();
    }
        
    // public async Task<int> BackupOldProfileSkillAndKeywordData()
    // {
    //     var profiles = _context.Profiles.ToList();

    //     foreach (var profile in profiles)
    //     {
    //         if (profile.Skills != null && profile.Skills.Any())
    //             profile.SkillsBackupJson = JsonConvert.SerializeObject(profile.Skills);

    //         if (profile.Keywords != null && profile.Keywords.Any())
    //             profile.KeywordsBackupJson = JsonConvert.SerializeObject(profile.Keywords);
    //     }

    //     return await _context.SaveChangesAsync();
    // }

 }