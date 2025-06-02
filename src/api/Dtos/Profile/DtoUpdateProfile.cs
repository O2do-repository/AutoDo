using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DtoUpdateProfile
{
    [Required]
    public Guid ProfileUuid { get; set; }
    
    [Required]
    public Guid ConsultantUuid { get; set; }

    [Required]
    public int RateHour { get; set; }

    [Required]
    [Url(ErrorMessage = "Le champ CV doit être une URL valide.")]
    public string Cv { get; set; }

    [Required]
    public DateTime CvDate { get; set; }

    [Required]
    public string JobTitle { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public Experience ExperienceLevel { get; set; }

    public List<string> Skills { get; set; }
    public List<string> Keywords { get; set; } 
}