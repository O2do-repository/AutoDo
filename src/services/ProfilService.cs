using models;  // votre using existant



public class ProfilService : IProfilService
{
    // Méthode pour récupérer tous les profils
    public List<Profil> GetAllProfils()
    {
        return DummyProfilData.Profils;
    }
}