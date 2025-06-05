
public class DtoOutputMatchingFeedback
{
    public Guid MatchingFeedbackUuid { get; set; }
    public Guid MatchingUuid { get; set; }
    public Matching Matching { get; set; }
    public int TotalScore { get; set; }
    public int JobTitleScore { get; set; }
    public int ExperienceScore { get; set; }
    public int SkillsScore { get; set; }
    public int LocationScore { get; set; }
    public string JobTitleFeedback { get; set; }
    public string ExperienceFeedback { get; set; }
    public string SkillsFeedback { get; set; }
    public string LocationFeedback { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
