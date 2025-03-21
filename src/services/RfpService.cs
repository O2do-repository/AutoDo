

using Microsoft.EntityFrameworkCore;

public class RfpService : IRfpService
{
    private readonly AutoDoDbContext _context;

    public RfpService(AutoDoDbContext context)
    {
        _context = context;
    }

    public async Task<List<RFP>> FilterRfpDeadlineNotReachedYet()
    {
        return await _context.Rfps
            .Where(rfp => rfp.DeadlineDate >= DateTime.Today)
            .ToListAsync();
    }
}
