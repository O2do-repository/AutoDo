

public class ProfileService : IProfileService
{
    private readonly AutoDoDbContext _context;


    public ProfileService(AutoDoDbContext context)
    {
        _context = context;
    }
    
    // Get profile
    public List<Profile> GetAllProfiles()
    {
        return _context.Profiles.ToList();
    }

    // Add a new profile 
    public Profile AddProfile(Profile profile)
    {
        var consultant = _context.Consultants
            .FirstOrDefault(c => c.ConsultantUuid.ToString() == profile.ConsultantUuid.ToString());


        if (consultant == null)
        {
            throw new Exception($"Le consultant avec UUID {profile.ConsultantUuid} n'existe pas.");
        }
        // Link profile to a consultant
        profile.Consultant = consultant;
        profile.ProfileUuid = Guid.NewGuid();

        _context.Profiles.Add(profile);
        _context.SaveChanges();

        return profile;
    }

        // Update profile
    public Profile UpdateProfile(Profile updatedProfile)
    {
        var existingProfile = _context.Profiles
            .FirstOrDefault(p => p.ProfileUuid == updatedProfile.ProfileUuid);
        
        if (existingProfile == null)
        {
            throw new Exception($"Le profil avec UUID {updatedProfile.ProfileUuid} n'existe pas.");
        }


        existingProfile.Ratehour = updatedProfile.Ratehour;
        existingProfile.CV = updatedProfile.CV;
        existingProfile.CVDate = updatedProfile.CVDate;
        existingProfile.JobTitle = updatedProfile.JobTitle;
        existingProfile.ExperienceLevel = updatedProfile.ExperienceLevel;
        existingProfile.Skills = updatedProfile.Skills;
        existingProfile.Keywords = updatedProfile.Keywords;

        _context.Profiles.Update(existingProfile);
        _context.SaveChanges();

        return existingProfile;
    }

    // Delete profile
    public void DeleteProfile(Guid profileUuid)
    {
        var existingProfile = _context.Profiles
            .FirstOrDefault(p => p.ProfileUuid == profileUuid);

        if (existingProfile == null)
        {
            throw new Exception($"Le profil avec UUID {profileUuid} n'existe pas.");
        }

        _context.Profiles.Remove(existingProfile);
        _context.SaveChanges();
    }

}