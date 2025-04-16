using System.ComponentModel.DataAnnotations;

public class DtoOutputEnterprise
{
    [Key]
    public Guid EnterpriseUuid { get; set; }
    [Required]
    public string Name { get; set; }
}