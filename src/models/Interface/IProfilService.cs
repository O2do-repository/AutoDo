public interface IProfileService
{
    List<Profile> GetAllProfiles();
    Task<Profile> AddProfile(Profile profile, List<Guid> skillUuids, List<Guid> keywordUuids);
    Task<Profile> UpdateProfile(Profile updatedProfile, List<Guid> skillUuids, List<Guid> keywordUuids);
    void DeleteProfile(Guid profileUuid);
    List<Profile> GetProfilesByConsultant(Guid consultantUuid);
    Task UpdateAllProfilesNormalizationAsync();
    //Task<int> BackupOldProfileSkillAndKeywordData();
}