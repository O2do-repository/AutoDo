using System.ComponentModel.DataAnnotations;

public class Skill
{
    [Key]
    public Guid SkillUuid { get; set; }

    public string Name { get; set; }
    // Traductions
    public string NameFr { get; set; }
    public string NameEn { get; set; }
    public string NameNl { get; set; }
    
}
