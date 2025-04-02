using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DtoInputConsultant{

    [Required]
    public string Email { get; set; }
    public DateTime AvailabilityDate {get;set;}
    public DateTime ExpirationDateCI {get;set;}
    public bool Intern {get; set;} = true;
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    [Url(ErrorMessage = "Le champ CV doit être une URL valide.")]
    public string CopyCI {get;set;}
    public string Picture{get;set;}

    [JsonConverter(typeof(JsonStringEnumConverter))]    
    public Enterprise enterprise {get; set;}
    [Required]
    public string Phone { get; set; }
}