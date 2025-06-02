public interface ISkillService{
    void DeleteSkill(Guid skillUuid);
    Skill AddSkill(Skill skill);
    List<Skill> GetAllSkills();

}
