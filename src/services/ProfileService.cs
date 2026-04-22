using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProfileService : IProfileService
{
    private readonly AutoDoDbContext _context;
    private readonly ITranslationService _translationService;
    private readonly IAiNormalizationService _aiService;

    public ProfileService(AutoDoDbContext context, ITranslationService translationService, IAiNormalizationService aiService)
    {
        _context = context;
        _translationService = translationService;
        _aiService = aiService;
    }

    public List<Profile> GetAllProfiles()
    {
        return _context.Profiles.ToList();
    }

    public List<Profile> GetProfilesByConsultant(Guid consultantUuid)
    {
        return _context.Profiles
            .Where(p => p.ConsultantUuid == consultantUuid)
            .Include(p => p.Skills)
            .Include(p => p.Keywords)
            .ToList();
    }

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

        // Charger les entités complètes depuis la DB EN PREMIER
        // (avant tout Attach, pour éviter que le tracker EF retourne des objets vides)
        var fullSkills = skillUuids != null && skillUuids.Any()
            ? await _context.Skills.Where(s => skillUuids.Contains(s.SkillUuid)).ToListAsync()
            : new List<Skill>();

        var fullKeywords = keywordUuids != null && keywordUuids.Any()
            ? await _context.Keywords.Where(k => keywordUuids.Contains(k.KeywordUuid)).ToListAsync()
            : new List<Keyword>();

        // Assigner directement les entités complètes (EF gère la table de jointure automatiquement)
        profile.Skills = fullSkills;
        profile.Keywords = fullKeywords;

        //  IA : normalisation
        try
        {
            var jobTitleInput = $"{profile.JobTitleFr} | {profile.JobTitleEn} | {profile.JobTitleNl}";

            var rawSkills = fullSkills
                .SelectMany(s => new[] { s.Name, s.NameFr, s.NameEn, s.NameNl })
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            var rawKeywords = fullKeywords
                .SelectMany(k => new[] { k.Name, k.NameFr, k.NameEn, k.NameNl })
                .Where(k => !string.IsNullOrWhiteSpace(k)).ToList();



            var normalized = await _aiService.NormalizeAsync(jobTitleInput, rawSkills, rawKeywords);

            profile.NormalizedJobTitle = normalized.NormalizedJobTitle;
            profile.NormalizedSkillsJson = JsonSerializer.Serialize(normalized.NormalizedSkills);
            profile.NormalizedKeywordsJson = JsonSerializer.Serialize(normalized.NormalizedKeywords);
            profile.LastNormalizationDate = DateTime.UtcNow;


        }
        catch (Exception ex)
        {
                        throw new InvalidOperationException(
                $"Échec de la normalisation IA pour le nouveau profil (Titre: {profile.JobTitle}). Voir l'exception interne pour les détails.", 
                ex);
        }

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        return profile;
    }

    public async Task<Profile> UpdateProfile(Profile updatedProfile, List<Guid> skillUuids, List<Guid> keywordUuids)
    {
        var existingProfile = await _context.Profiles
            .Include(p => p.Skills)
            .Include(p => p.Keywords)
            .FirstOrDefaultAsync(p => p.ProfileUuid == updatedProfile.ProfileUuid);

        if (existingProfile == null)
            throw new Exception($"Le profil avec UUID {updatedProfile.ProfileUuid} n'existe pas.");

        bool hasCriticalChange = false;

        if (existingProfile.JobTitle != updatedProfile.JobTitle)
        {
            existingProfile.JobTitle = updatedProfile.JobTitle;
            existingProfile.JobTitleFr = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "fr");
            existingProfile.JobTitleEn = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "en");
            existingProfile.JobTitleNl = await _translationService.TranslateTextAsync(updatedProfile.JobTitle, "nl");
            hasCriticalChange = true;
        }

        existingProfile.Ratehour = updatedProfile.Ratehour ?? existingProfile.Ratehour;
        existingProfile.CVDate = updatedProfile.CVDate ?? existingProfile.CVDate;
        existingProfile.CVDate = updatedProfile.CVDate;
        existingProfile.ExperienceLevel = updatedProfile.ExperienceLevel;

        var currentSkillIds = existingProfile.Skills.Select(s => s.SkillUuid).OrderBy(id => id);
        var newSkillIds = skillUuids?.OrderBy(id => id) ?? Enumerable.Empty<Guid>();

        var currentKeywordIds = existingProfile.Keywords.Select(k => k.KeywordUuid).OrderBy(id => id);
        var newKeywordIds = keywordUuids?.OrderBy(id => id) ?? Enumerable.Empty<Guid>();

        bool skillsChanged = !currentSkillIds.SequenceEqual(newSkillIds);
        bool keywordsChanged = !currentKeywordIds.SequenceEqual(newKeywordIds);

        // Charger les entités complètes une seule fois
        var fullSkills = skillUuids != null && skillUuids.Any()
            ? await _context.Skills.Where(s => skillUuids.Contains(s.SkillUuid)).ToListAsync()
            : new List<Skill>();

        var fullKeywords = keywordUuids != null && keywordUuids.Any()
            ? await _context.Keywords.Where(k => keywordUuids.Contains(k.KeywordUuid)).ToListAsync()
            : new List<Keyword>();

        if (skillsChanged || keywordsChanged)
        {
            hasCriticalChange = true;

            existingProfile.Skills.Clear();
            existingProfile.Keywords.Clear();

            // Commit intermédiaire pour vider les tables de jonction
            await _context.SaveChangesAsync();

            existingProfile.Skills.AddRange(fullSkills);
            existingProfile.Keywords.AddRange(fullKeywords);
        }

        if (hasCriticalChange)
        {
            try
            {
                var jobTitleInput = $"{existingProfile.JobTitleFr} | {existingProfile.JobTitleEn} | {existingProfile.JobTitleNl}";

                var rawSkills = fullSkills
                    .SelectMany(s => new[] { s.Name, s.NameFr, s.NameEn, s.NameNl })
                    .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                var rawKeywords = fullKeywords
                    .SelectMany(k => new[] { k.Name, k.NameFr, k.NameEn, k.NameNl })
                    .Where(k => !string.IsNullOrWhiteSpace(k)).ToList();



                var normalized = await _aiService.NormalizeAsync(jobTitleInput, rawSkills, rawKeywords);

                existingProfile.NormalizedJobTitle = normalized.NormalizedJobTitle;
                existingProfile.NormalizedSkillsJson = JsonSerializer.Serialize(normalized.NormalizedSkills);
                existingProfile.NormalizedKeywordsJson = JsonSerializer.Serialize(normalized.NormalizedKeywords);
                existingProfile.LastNormalizationDate = DateTime.UtcNow;


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Échec de la normalisation IA pour la mise à jour du profil (ID: {existingProfile.ProfileUuid}, Titre: {existingProfile.JobTitle}). Voir l'exception interne pour les détails.", 
                    ex);
                
            }
        }

        await _context.SaveChangesAsync();

        return existingProfile;
    }

    public void DeleteProfile(Guid profileUuid)
    {
        var existingProfile = _context.Profiles
            .FirstOrDefault(p => p.ProfileUuid == profileUuid);

        if (existingProfile == null)
            throw new Exception($"Le profil avec UUID {profileUuid} n'existe pas.");

        _context.Profiles.Remove(existingProfile);
        _context.SaveChanges();
    }

    public async Task UpdateAllProfilesNormalizationAsync()
    {
        var profiles = await _context.Profiles
            .Include(p => p.Skills)
            .Include(p => p.Keywords)
            .ToListAsync();

        foreach (var profile in profiles)
        {
            try
            {
                var fullSkills = profile.Skills ?? new List<Skill>();
                var fullKeywords = profile.Keywords ?? new List<Keyword>();

                var jobTitleInput = $"{profile.JobTitleFr} | {profile.JobTitleEn} | {profile.JobTitleNl}";

                var rawSkills = fullSkills
                    .SelectMany(s => new[] { s.Name, s.NameFr, s.NameEn, s.NameNl })
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                var rawKeywords = fullKeywords
                    .SelectMany(k => new[] { k.Name, k.NameFr, k.NameEn, k.NameNl })
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .ToList();

                var normalized = await _aiService.NormalizeAsync(jobTitleInput, rawSkills, rawKeywords);

                profile.NormalizedJobTitle = normalized.NormalizedJobTitle;
                profile.NormalizedSkillsJson = JsonSerializer.Serialize(normalized.NormalizedSkills);
                profile.NormalizedKeywordsJson = JsonSerializer.Serialize(normalized.NormalizedKeywords);
                profile.LastNormalizationDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                continue;
            }
        }
    }


 
}