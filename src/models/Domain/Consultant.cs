using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Consultant
{
    [Key]
    public Guid ConsultantUuid { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public DateTime AvailabilityDate {get;set;}
    public DateTime ExpirationDateCI {get;set;}
    [Required]
    public bool Intern {get; set;} = false;
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    public Enterprise enterprise {get; set;}
    [Required]
    public string Phone { get; set; }
    public string Picture{get;set;}
    [Required]
    public string CopyCI {get;set;}
    // Un consultant peut avoir plusieurs profils
    public List<Profile> Profiles { get; set; }
}

internal class requiredAttribute : Attribute
{
}