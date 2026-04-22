public interface ISkillService{
    void DeleteSkill(Guid skillUuid);
    Task<Skill> AddSkill(Skill skill);
    List<Skill> GetAllSkills();
    Task TranslateAllSkills();
    Task<(int Added, int Skipped, List<string> SkippedNames)> AddBulkSkills(IEnumerable<string> names);

}
