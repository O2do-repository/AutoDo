using System.Text.Json.Serialization;

public class DtoInputMatching
{
    public string JobTitle {get;set;}
    public string ConsultantName { get; set; }
    public string ConsultantSurname { get; set; }
    public string RfpReference { get; set; }
    public int Score { get; set; }
    public DateTime PublicationDate { get; set; }
    public string Comment { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatutMatching StatutMatching{get; set;}
}
