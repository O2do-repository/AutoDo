using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class RfpService : IRfpService
{
    private readonly AutoDoDbContext _context;
    private readonly IMatchingService _matchingService;
    private readonly IAiNormalizationService _aiService;

    public RfpService(AutoDoDbContext context, IMatchingService matchingService, IAiNormalizationService aiService)
    {
        _context = context;
        _matchingService = matchingService;
        _aiService = aiService;
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

        var incomingReferences = rfps.Select(r => r.Reference).Where(r => !string.IsNullOrEmpty(r)).ToList();

        var existingRfps = await _context.Rfps
            .Where(r => incomingReferences.Contains(r.Reference))
            .ToDictionaryAsync(r => r.Reference);

        var newRfps = new List<RFP>();
        var updatedRfps = new List<RFP>();

        foreach (var rfp in rfps)
        {
            if (string.IsNullOrEmpty(rfp.Reference))
                continue;

            rfp.Skills ??= new List<string>();

            if (existingRfps.TryGetValue(rfp.Reference, out var existing))
            {
                bool hasChange =
                    existing.JobTitle != rfp.JobTitle ||
                    existing.ExperienceLevel != rfp.ExperienceLevel ||
                    existing.Workplace != rfp.Workplace ||
                    (existing.Skills == null && rfp.Skills.Count > 0) ||
                    (existing.Skills != null && !existing.Skills.SequenceEqual(rfp.Skills));

                existing.JobTitle = rfp.JobTitle;
                existing.DescriptionBrut = rfp.DescriptionBrut;
                existing.PublicationDate = rfp.PublicationDate;
                existing.DeadlineDate = rfp.DeadlineDate;
                existing.RfpUrl = rfp.RfpUrl;
                existing.Workplace = rfp.Workplace;
                existing.RfpPriority = rfp.RfpPriority;
                existing.ExperienceLevel = rfp.ExperienceLevel;
                existing.Skills = rfp.Skills;

                if (hasChange || existing.LastNormalizationDate == null)
                {
                    existing.NormalizedJobTitle = null;
                    existing.NormalizedSkillsJson = null;
                    existing.LastNormalizationDate = null;
                    updatedRfps.Add(existing);
                }
            }
            else
            {
                rfp.RFPUuid = Guid.NewGuid();
                _context.Rfps.Add(rfp);
                newRfps.Add(rfp);
            }
        }

        if (newRfps.Count > 0 || updatedRfps.Count > 0)
            await _context.SaveChangesAsync();

        var toProcess = newRfps.Concat(updatedRfps).ToList();
        var toProcessIds = toProcess.Select(x => x.RFPUuid).ToList();

        var neverNormalized = await _context.Rfps
            .Where(r => r.LastNormalizationDate == null && !toProcessIds.Contains(r.RFPUuid))
            .ToListAsync();

        var toNormalize = toProcess.Concat(neverNormalized).ToList();

        if (toNormalize.Count == 0)
            return;

        foreach (var rfp in toNormalize)
        {
            try
            {
                var normalized = await _aiService.NormalizeAsync(rfp.JobTitle, rfp.Skills, null);

                rfp.NormalizedJobTitle = normalized.NormalizedJobTitle;
            var combinedList = normalized.NormalizedKeywords
                .Concat(normalized.NormalizedSkills)
                .Distinct() 
                .ToList();
                rfp.NormalizedSkillsJson = JsonSerializer.Serialize(combinedList);
                rfp.LastNormalizationDate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Échec de la normalisation IA pour l'offre de référence '{rfp.Reference}' (ID: {rfp.RFPUuid}).", 
                    ex);
            }
        }

        await _context.SaveChangesAsync();
        await _matchingService.MatchingsForRfpsAsync(toNormalize);
    }

    public void DeleteOldRFPs()
    {
        var today = DateTime.Today;

        var oldRFPs = _context.Rfps
            .Where(r => r.DeadlineDate < today)
            .ToList();

        _context.Rfps.RemoveRange(oldRFPs);
        _context.SaveChanges();
    }
    public void DeleteAllRFPs()
    {
        _context.Rfps.RemoveRange(_context.Rfps.ToList());
        _context.SaveChanges();
    }
}