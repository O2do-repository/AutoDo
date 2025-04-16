using System.ComponentModel.DataAnnotations;

public class DtoOutputSkill
{
    [Key]
    public Guid SkillUuid { get; set; }
    [Required]
    public string Name { get; set; }
}