
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
    [Required]
    public string JobTitleFr { get; set; }
    [Required]
    public string JobTitleEn { get; set; }
    [Required]
    public string JobTitleNl { get; set; }
    public Experience ExperienceLevel { get; set; }
    public List<Skill> Skills { get; set; }
    public List<Keyword> Keywords { get; set; }

    [JsonIgnore]
    public Consultant Consultant {get; set;}

    // Ajut
    /// Titre du poste normalisé en Anglais Standard (ex: "Senior .NET Developer")
    public string? NormalizedJobTitle { get; set; }


    /// Liste JSON des compétences normalisées (ex: ["CSharp", "Azure", "Agile"])
    public string? NormalizedSkillsJson { get; set; }
    public string? NormalizedKeywordsJson { get; set; }


    /// Date de la dernière normalisation (pour débogage et invalidation)
    public DateTime? LastNormalizationDate { get; set; }
}
