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
        var existingConsultant = _context.Consultants
            .SingleOrDefault(c => c.Email == consultant.Email);

        if (existingConsultant != null)
        {
            throw new InvalidOperationException($"Un consultant avec l'email '{consultant.Email}' existe déjà.");
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

    // Delete Consultant
    public void DeleteConsultant(Guid consultantUuid)
    {
        var existingConsultant = _context.Consultants.SingleOrDefault(c => c.ConsultantUuid == consultantUuid);

        if (existingConsultant == null)
        {
            throw new KeyNotFoundException($"Aucun consultant trouvé avec l'UUID : {consultantUuid}");
        }

        _context.Consultants.Remove(existingConsultant);
        _context.SaveChanges();
    }

    // Update Consultant 
    public Consultant UpdateConsultant(Consultant updatedConsultant)
    {
        var existingConsultant = _context.Consultants
            .FirstOrDefault(p => p.ConsultantUuid == updatedConsultant.ConsultantUuid);
        
        if (existingConsultant == null)
        {
            throw new Exception($"Le consultant avec UUID {updatedConsultant.ConsultantUuid} n'existe pas.");
        }

        existingConsultant.ConsultantUuid = updatedConsultant.ConsultantUuid;
        existingConsultant.Email = updatedConsultant.Email;
        existingConsultant.AvailabilityDate = updatedConsultant.AvailabilityDate;
        existingConsultant.ExpirationDateCI = updatedConsultant.ExpirationDateCI;
        existingConsultant.Intern = updatedConsultant.Intern;
        existingConsultant.Name = updatedConsultant.Name;
        existingConsultant.Surname = updatedConsultant.Surname;
        existingConsultant.Phone = updatedConsultant.Phone;
        existingConsultant.Picture = updatedConsultant.Picture;
        existingConsultant.CopyCI = updatedConsultant.CopyCI;
        existingConsultant.enterprise = updatedConsultant.enterprise;

        _context.Consultants.Update(existingConsultant);
        _context.SaveChanges();

        return existingConsultant;
    }


}