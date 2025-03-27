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
}