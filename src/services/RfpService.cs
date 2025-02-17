using models;

public class RfpServices
{
    // Méthode pour filtrer les RFPs dont la date limite n'est pas atteinte
    public List<RFP> FilterRfpDeadlineNotReachedYet(List<RFP> rfps)
    {
        return rfps.Where(data => data.DeadlineDate.CompareTo(DateTime.Today) >= 0).ToList();
    }
}
