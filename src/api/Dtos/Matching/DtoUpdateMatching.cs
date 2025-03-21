using System.Text.Json.Serialization;

public class DtoUpdateMatching
{    public string Comment { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatutMatching StatutMatching{get; set;}
    public Guid ProfileUuid {get;set;}
    public Guid RfpUuid {get;set;}
    public Guid MatchingUuid {get;set;}
    public int Score {get;set;}
}
