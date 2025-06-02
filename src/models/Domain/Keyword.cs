using System.ComponentModel.DataAnnotations;

public class Keyword
{
    [Key]
    public Guid KeywordUuid { get; set; }
    public string Name { get; set; }
}