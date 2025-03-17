public interface IProfileService{
    List<Profile> GetAllProfiles();
    Profile AddProfile(Profile profile);
    Profile UpdateProfile(Profile updatedProfile);
    void DeleteProfile(Guid profileUuid);
}