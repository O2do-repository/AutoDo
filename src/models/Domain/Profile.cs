
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Profile
{
    [Key]
    public Guid ProfileUuid { get; set; }
    [ForeignKey("ConsultantUuid")]
    public Guid ConsultantUuid{get;set;}
    public int Ratehour { get; set; }
    [Required]
    public string CV { get; set; }
    [Required]
    public DateTime CVDate { get; set; }
    [Required]
    public string JobTitle { get; set; }
    public Experience ExperienceLevel { get; set; }
    public List<String> Skills { get; set; }
    public List<String> Keywords { get; set; }
    [JsonIgnore]
    public Consultant Consultant {get; set;}
}
