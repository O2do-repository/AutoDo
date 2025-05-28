using System.Text.Json.Serialization;

public class DtoOutputConsultant
{
    public Guid ConsultantUuid { get; set; }
    public string Email { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime AvailabilityDate { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime ExpirationDateCI { get; set; }
    public bool Intern { get; set; } = true;
    public string Name { get; set; }
    public string Surname { get; set; }

    public string enterprise { get; set; }
    public string Phone { get; set; }
    public string Picture { get; set; }
    public string CopyCI { get; set; }
    public string Comment{ get; set; }
}