public class EnterpriseService : IEnterpriseService
{
    private readonly AutoDoDbContext _context;

    public EnterpriseService(AutoDoDbContext context)
    {
        _context = context;
    }

    // Récupérer toutes les entreprises
    public List<Enterprise> GetAllEnterprises()
    {
        return _context.Enterprises.ToList();
    }

    // Ajouter une nouvelle entreprise
    public Enterprise AddEnterprise(Enterprise enterprise)
    {


        var existingEnterprise= _context.Enterprises
            .SingleOrDefault(k => k.Name.ToLower() == enterprise.Name.ToLower());

        if (existingEnterprise != null)
        {
            throw new InvalidOperationException($"L'entreprise '{enterprise.Name}' existe déjà.");
        }

        enterprise.EnterpriseUuid = Guid.NewGuid();
        _context.Enterprises.Add(enterprise);
        _context.SaveChanges();

        return enterprise;
    }

    // Supprimer une entreprise
    public void DeleteEnterprise(Guid enterpriseUuid)
    {
        var existingEnterprise = _context.Enterprises
            .SingleOrDefault(e => e.EnterpriseUuid == enterpriseUuid);

        if (existingEnterprise == null)
        {
            throw new Exception($"L'entreprise avec UUID {enterpriseUuid} n'existe pas.");
        }

        _context.Enterprises.Remove(existingEnterprise);
        _context.SaveChanges();
    }
}
