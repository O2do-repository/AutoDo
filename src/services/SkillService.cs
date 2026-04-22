using Microsoft.Extensions.Logging;

public class SkillService : ISkillService
{
    private readonly AutoDoDbContext _context;
    private readonly ITranslationService _translationService;
    public SkillService(AutoDoDbContext context, ITranslationService translationService)
    {
        _context = context;
        _translationService = translationService;
    }

    // Get skill
    public List<Skill> GetAllSkills()
    {
        return _context.Skills.ToList();
    }

    // Add a new skill 
    public async Task<Skill> AddSkill(Skill skill)
    {
        var existingSkill = _context.Skills
            .SingleOrDefault(k => k.Name.ToLower() == skill.Name.ToLower());

        if (existingSkill != null)
        {
            throw new InvalidOperationException($"La compétence '{skill.Name}' existe déjà.");
        }

        skill.SkillUuid = Guid.NewGuid();

        // Nom d'origine
        var originalName = skill.Name;

        // Traductions automatiques
        try
        {
            skill.NameEn = await _translationService.TranslateTextAsync(originalName, "en");
            skill.NameFr = await _translationService.TranslateTextAsync(originalName, "fr");
            skill.NameNl = await _translationService.TranslateTextAsync(originalName, "nl");
        }
        catch (Exception ex)
        {
            skill.NameEn = originalName;
            skill.NameFr = originalName;
            skill.NameNl = originalName;
        }

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return skill;
    }

    // Delete skill
    public void DeleteSkill(Guid skillUuid)
    {
        var existingSkill = _context.Skills
            .SingleOrDefault(s => s.SkillUuid == skillUuid);

        if (existingSkill == null)
        {
            throw new Exception($"Le skill avec UUID {skillUuid} n'existe pas.");
        }

        _context.Skills.Remove(existingSkill);
        _context.SaveChanges();
    }
    
    public async Task TranslateAllSkills()
    {
        var skills = _context.Skills.ToList();

        foreach (var skill in skills)
        {
            var originalName = skill.Name;

            try
            {
                skill.NameEn = await _translationService.TranslateTextAsync(originalName, "en");
                skill.NameFr = await _translationService.TranslateTextAsync(originalName, "fr");
                skill.NameNl = await _translationService.TranslateTextAsync(originalName, "nl");
            }
            catch (Exception ex)
            {
                // Optionnel : log l'erreur, ou garde les champs tels quels
                skill.NameEn = originalName;
                skill.NameFr = originalName;
                skill.NameNl = originalName;
            }
        }

        await _context.SaveChangesAsync();
    }
    public async Task<(int Added, int Skipped, List<string> SkippedNames)> AddBulkSkills(IEnumerable<string> names)
    {
        var added = 0;
        var skipped = 0;
        var skippedNames = new List<string>();

        var existingNames = _context.Skills
            .Select(s => s.Name.ToLower())
            .ToHashSet();

        foreach (var name in names.Where(n => !string.IsNullOrWhiteSpace(n)))
        {
            if (existingNames.Contains(name.ToLower()))
            {
                skipped++;
                skippedNames.Add(name);
                continue;
            }

            var skill = new Skill
            {
                SkillUuid = Guid.NewGuid(),
                Name = name,
                NameEn = name,
                NameFr = name,
                NameNl = name
            };

            _context.Skills.Add(skill);
            existingNames.Add(name.ToLower());
            added++;
        }

        await _context.SaveChangesAsync();
        return (added, skipped, skippedNames);
    }
}
