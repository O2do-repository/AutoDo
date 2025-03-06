using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DtoOutputProfile
{


    public int RateHour { get; set; }

    public string Cv { get; set; }

    public DateTime CvDate { get; set; }
    public string JobTitle { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public Experience ExperienceLevel { get; set; }

    public List<string> Skills { get; set; }
    public List<string> Keywords { get; set; } 
}