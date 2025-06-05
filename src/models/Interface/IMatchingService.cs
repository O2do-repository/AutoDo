public interface IMatchingService{
    MatchingFeedback CalculateMatchingFeedback(Profile profile, RFP rfp, Guid matchingUuid);
    Task<List<Matching>> MatchingsForProfileAsync(Profile profile);
    Task<List<Matching>> GetAllMatchingsAsync();
    Task<Matching> UpdateMatchingAsync(Guid id,Matching updatedMatching);
    Task<List<Matching>> MatchingsForRfpsAsync(List<RFP> rfps);
    Task<List<Matching>> GetAllMatchingsFiltered();
}