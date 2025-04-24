using System.Text.Json.Serialization;

public class DtoOutputMatching
{
    public Guid MatchingUuid {get;set;}
    public string JobTitle {get;set;}
    public string ConsultantName { get; set; }
    public string ConsultantSurname { get; set; }
    public string RfpReference { get; set; }
    public string RfpUrl{get;set;}
    public int Score { get; set; }
    public DateTime OfferDate { get; set; }
    public string Comment { get; set; }
    public Guid ProfileUuid {get;set;}
    public Guid RfpUuid {get;set;}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatutMatching StatutMatching{get; set;}
}
