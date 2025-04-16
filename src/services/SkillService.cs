public class SkillService : ISkillService
{
    private readonly AutoDoDbContext _context;

    public SkillService(AutoDoDbContext context)
    {
        _context = context;
    }
    
    // Get skill
    public List<Skill> GetAllSkills()
    {
        return _context.Skills.ToList();
    }
    
    // Add a new skill 
    public Skill AddSkill(Skill skill)
    {
        skill.SkillUuid = Guid.NewGuid();

        _context.Skills.Add(skill);
        _context.SaveChanges();

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
}
