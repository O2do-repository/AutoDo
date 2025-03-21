public interface IMatchingService{
    int CalculateMatchingScore(Profile profile, RFP rfp);
    Task<List<Matching>> MatchingsForProfileAsync(Profile profile);
    Task<List<Matching>> GetAllMatchingsAsync();
    Task<Matching> UpdateMatchingAsync(Guid id,Matching updatedMatching);
}