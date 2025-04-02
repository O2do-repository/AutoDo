using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Consultant
{
    [Key]
    public Guid ConsultantUuid { get; set; }
    public string Email { get; set; }
    public DateTime AvailabilityDate {get;set;}
    public DateTime ExpirationDateCI {get;set;}
    public bool Intern {get; set;} = false;
    public string Name { get; set; }
    public string Surname { get; set; }
    public Enterprise enterprise {get; set;}
    public string Phone { get; set; }
    public string Picture{get;set;}
    public string CopyCI {get;set;}
    // Un consultant peut avoir plusieurs profils
    public List<Profile> Profiles { get; set; }
}