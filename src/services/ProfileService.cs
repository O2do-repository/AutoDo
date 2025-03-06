

public class ProfileService : IProfileService
{
    private readonly AutoDoDbContext _context;


    public ProfileService(AutoDoDbContext context)
    {
        _context = context;
    }
    
    // Méthode pour récupérer tous les profils
    public List<Profile> GetAllProfiles()
    {
        return _context.Profiles.ToList();
    }

 // Ajouter un profil pour un consultant existant
    public Profile AddProfile(Profile profile)
    {
    var consultant = _context.Consultants
        .FirstOrDefault(c => c.ConsultantUuid.ToString() == profile.ConsultantUuid.ToString());


    if (consultant == null)
    {
        throw new Exception($"Le consultant avec UUID {profile.ConsultantUuid} n'existe pas.");
    }
        // Associer le consultant au profil
        profile.Consultant = consultant;
        profile.ProfileUuid = Guid.NewGuid();

        _context.Profiles.Add(profile);
        _context.SaveChanges();

        return profile;
    }
}