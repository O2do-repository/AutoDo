using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class RfpService : IRfpService
{
    private readonly AutoDoDbContext _context;
    private readonly IMatchingService _matchingService;
    public RfpService(AutoDoDbContext context, IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    public async Task<List<RFP>> FilterRfpDeadlineNotReachedYet()
    {
        return await _context.Rfps
            .Where(rfp => rfp.DeadlineDate >= DateTime.Today)
            .ToListAsync();
    }
    public async Task ImportFromJsonData(List<RFP> rfps)
    {
        if (rfps == null || rfps.Count == 0)
            throw new ArgumentException("Liste vide.");

        var existingReferences = _context.Rfps
            .Where(rfp => rfps.Select(x => x.Reference).Contains(rfp.Reference))
            .ToList();

        var newRfps = new List<RFP>();

        foreach (var rfp in rfps)
        {
            rfp.Skills ??= new List<string>();

            var existing = existingReferences.SingleOrDefault(x => x.Reference == rfp.Reference);

            if (existing != null)
            {
                existing.JobTitle = rfp.JobTitle;
                existing.DescriptionBrut = rfp.DescriptionBrut;
                existing.PublicationDate = rfp.PublicationDate;
                existing.DeadlineDate = rfp.DeadlineDate;
                existing.RfpUrl = rfp.RfpUrl;
                existing.Workplace = rfp.Workplace;
                existing.RfpPriority = rfp.RfpPriority;
                existing.ExperienceLevel = rfp.ExperienceLevel;
                existing.Skills = rfp.Skills;
            }
            else
            {
                rfp.RFPUuid = Guid.NewGuid();
                _context.Rfps.Add(rfp);
                newRfps.Add(rfp);
            }
        }

        await _context.SaveChangesAsync();

        if (newRfps.Count > 0)
        {
            await _matchingService.MatchingsForRfpsAsync(newRfps);
        }
    }

        // Delete RFP
    public void DeleteOldRFPs()
    {
        var today = DateTime.Today;

        var oldRFPs = _context.Rfps
            .Where(r => r.DeadlineDate < today)
            .ToList();

        _context.Rfps.RemoveRange(oldRFPs);
        _context.SaveChanges();
    }
}
