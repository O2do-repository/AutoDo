public interface ISkillService{
    void DeleteSkill(Guid skillUuid);
    Task<Skill> AddSkill(Skill skill);
    List<Skill> GetAllSkills();
    Task TranslateAllSkills();
}
