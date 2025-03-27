
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class RFP
{
    [Key]
    public Guid RFPUuid { get; set; }  
    [JsonProperty("Id")]
    public string Reference { get; set; }

    [JsonProperty("SubmissionDeadline")]
    public DateTime DeadlineDate { get; set; } = DateTime.Today.AddYears(1000);

    [JsonProperty("Description")]
    public string DescriptionBrut { get; set; }

    [JsonProperty("Experience")]
    public Experience ExperienceLevel { get; set; } 

    [JsonProperty("Skills")]
    public List<string> Skills { get; set; }

    [JsonProperty("Name")]
    public string JobTitle { get; set; }

    [JsonProperty("Url")]
    public string RfpUrl { get; set; }

    [JsonProperty("PlaceToWork")]
    public string Workplace { get; set; }

    [JsonProperty("PublicationDate")]
    public DateTime PublicationDate { get; set; }
    [JsonProperty("RfpPriority")]
    public string RfpPriority {get;set;}
}
