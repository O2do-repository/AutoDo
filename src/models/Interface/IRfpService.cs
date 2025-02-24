using models;

public interface IRfpService{
    List<RFP> FilterRfpDeadlineNotReachedYet(List<RFP> rfps);
}