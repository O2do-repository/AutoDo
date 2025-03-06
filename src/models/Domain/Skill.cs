using System.ComponentModel.DataAnnotations;

public class Skill
{
    [Key]
    public Guid SkillUuid { get; set; }
    public string Name { get; set; }

}