public class DtoOutputConsultant{
    public Guid ConsultantUuid { get; set; }
    public string Email { get; set; }
    public DateTime AvailabilityDate {get;set;}
    public DateTime ExpirationDateCI {get;set;}
    public bool Intern {get; set;} = true;
    public string Name { get; set; }
    public string Surname { get; set; }
    public Enterprise enterprise {get; set;}
    public string Phone { get; set; }
}