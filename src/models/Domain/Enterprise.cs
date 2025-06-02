using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

public class Enterprise
{
    [Key]
    public Guid EnterpriseUuid { get; set; }
    public string Name { get; set; }

}