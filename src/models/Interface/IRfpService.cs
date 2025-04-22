

public interface IRfpService{
    Task<List<RFP>> FilterRfpDeadlineNotReachedYet();
    void LoadRfpFromJson();
    Task ImportRfpAndGenerateMatchings();
    Task ImportFromJsonData(List<RFP> rfps);
    
}