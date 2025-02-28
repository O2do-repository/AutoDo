using models;  // votre using existant

public class ProfilService : IProfilService
{
    private readonly AutoDoDbContext _context;


    public ProfilService(AutoDoDbContext context)
    {
        _context = context;
    }
    
    // Méthode pour récupérer tous les profils
    public List<Profil> GetAllProfils()
    {
        return _context.Profils.ToList();
    }
}