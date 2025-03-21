using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DtoInputProfile
{
    [Required]
    public Guid ConsultantUuid { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Le taux horaire ne peut pas être négatif.")]
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