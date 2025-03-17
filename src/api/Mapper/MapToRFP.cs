

public static class MapToRFP
{
    public static RFP ToRFP(this ReceivedRequestInfoOutputModel source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new RFP  
        {
            RFPUuid = Guid.TryParse(source.Uuid, out var guid) ? guid : Guid.Empty,

            DeadlineDate = source.ResponseDate.UtcDateTime,
            
            DescriptionBrut = source.Description,
            
            ExperienceLevel = Enum.TryParse<Experience>(source.ExternalStatus?.TranslatedName, out var level) ? level : Experience.Junior,
            
            RfpPriority = source.AssignmentCategory?.Code ?? string.Empty,
            
            PublicationDate = source.CreatedAt.UtcDateTime,
            
            Skills = new List<string>(),
            
            JobTitle = source.Title,
            
            RfpUrl = source.ProjectRequest?.ResourceUri,
            
            Workplace = source.Location?.Name,

            Reference = source.Reference
        };
    }

}
