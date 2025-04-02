public class ConsultantService : IConsultantService
{
    private readonly AutoDoDbContext _context;


    public ConsultantService(AutoDoDbContext context)
    {
        _context = context;
    }
    
    // Get all Consultants
    public List<Consultant> GetAllConsultants()
    {
        return _context.Consultants.ToList();
    }


    public Consultant AddConsultant(Consultant consultant)
    {
        
        if (string.IsNullOrWhiteSpace(consultant.Email) ||
            string.IsNullOrWhiteSpace(consultant.Phone) ||
            string.IsNullOrWhiteSpace(consultant.Name) ||
            string.IsNullOrWhiteSpace(consultant.Surname) ||
            consultant.ExpirationDateCI == null)
        {
            throw new ArgumentException("Tous les champs obligatoires doivent être renseignés : Copie CI, Courriel, N° de téléphone, Nom & Prénom.");
        }


        _context.Consultants.Add(consultant);
        _context.SaveChanges();

        return consultant;
    }
        public Consultant GetConsultantById(Guid consultantUuid)
    {
        var consultant = _context.Consultants.SingleOrDefault(c => c.ConsultantUuid == consultantUuid);

        if (consultant == null)
        {
            throw new Exception($"Aucun consultant trouvé avec l'UUID {consultantUuid}.");
        }

        return consultant;
    }
}