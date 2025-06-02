using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
public class DtoInputRFP
{
    public string Reference { get; set; }
    public DateTime DeadlineDate { get; set; }
    public string DescriptionBrut { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public Experience ExperienceLevel { get; set; } 
    public List<string> Skills { get; set; }
    public string JobTitle { get; set; }
    public string RfpUrl { get; set; }
    public string Workplace { get; set; }
    public DateTime PublicationDate { get; set; }
    public string RfpPriority {get;set;}
}
