using models;

public static class MapToRFP
{
    public static RFP ToRFP(this ReceivedRequestInfoOutputModel source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new RFP
        {
            Uuid = source.Uuid,
            
            DeadlineDate = source.ResponseDate.UtcDateTime,
            
            DescriptionBrut = source.Description,
            
            ExperienceLevel = source.ExternalStatus?.TranslatedName ?? string.Empty,
            
            RfpPriority = source.AssignmentCategory?.Code ?? string.Empty,
            
            PublicationDate = source.CreatedAt.UtcDateTime,
            
            Skills = new List<string>(),
            
            JobTitle = source.Title,
            
            RfpUrl = source.ProjectRequest?.ResourceUri,
            
            Workplace = source.Location?.Name,
            
        };
    }
}
