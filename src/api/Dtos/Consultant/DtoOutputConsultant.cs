using System.Text.Json.Serialization;

public class DtoOutputConsultant{
    public Guid ConsultantUuid { get; set; }
    public string Email { get; set; }
    public DateTime AvailabilityDate {get;set;}
    public DateTime ExpirationDateCI {get;set;}
    public bool Intern {get; set;} = true;
    public string Name { get; set; }
    public string Surname { get; set; }

    public string enterprise {get; set;}
    public string Phone { get; set; }
    public string Picture{get;set;}
    public string CopyCI {get;set;}
}