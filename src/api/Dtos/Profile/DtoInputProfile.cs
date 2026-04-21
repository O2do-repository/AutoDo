using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DtoInputProfile
{
    [Required]
    public Guid ConsultantUuid { get; set; }

    public int? RateHour { get; set; }

    [Required]
    [Url(ErrorMessage = "Le champ CV doit être une URL valide.")]
    public string Cv { get; set; }

    public DateTime? CvDate { get; set; }

    [Required]
    public string JobTitle { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public Experience ExperienceLevel { get; set; }

    public List<Guid> SkillUuids { get; set; }
    public List<Guid> KeywordUuids { get; set; }
}