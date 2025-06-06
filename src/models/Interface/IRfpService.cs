

public interface IRfpService
{
    Task<List<RFP>> FilterRfpDeadlineNotReachedYet();
    Task ImportFromJsonData(List<RFP> rfps);
    void DeleteOldRFPs();
    
}