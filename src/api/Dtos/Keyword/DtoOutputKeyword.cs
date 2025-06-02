using System.ComponentModel.DataAnnotations;

public class DtoOutputKeyword
{
    [Key]
    public Guid KeywordUuid { get; set; }
    [Required]
    public string Name { get; set; }
}