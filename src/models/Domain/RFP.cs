
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class RFP
{
    [Key]
    public Guid RFPUuid { get; set; }  
    public string Reference { get; set; }

    public DateTime DeadlineDate { get; set; } = DateTime.Today.AddYears(1000);
    public string DescriptionBrut { get; set; }
    public Experience ExperienceLevel { get; set; } 
    public List<string> Skills { get; set; }
    public string JobTitle { get; set; }
    public string RfpUrl { get; set; }
    public string Workplace { get; set; }
    public DateTime PublicationDate { get; set; }
    public string RfpPriority {get;set;}
}
