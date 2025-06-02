using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Matching
{
    [Key]
    public Guid MatchingUuid { get; set; }

    public string Comment { get; set; }

    [Required]
    public Guid ProfileUuid { get; set; }

    [Required]
    public Guid RfpUuid { get; set; }

    [Required]
    public int Score { get; set; }
    [Required]
    public StatutMatching StatutMatching{get; set;} = StatutMatching.New;

    [ForeignKey("ProfileUuid")]
    public Profile Profile { get; set; }

    [ForeignKey("RfpUuid")]
    public RFP Rfp { get; set; }
}