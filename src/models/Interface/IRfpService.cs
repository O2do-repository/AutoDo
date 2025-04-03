

public interface IRfpService{
    Task<List<RFP>> FilterRfpDeadlineNotReachedYet();
    void LoadRfpFromJson();
    Task ImportRfpAndGenerateMatchingsAsync();
    
}