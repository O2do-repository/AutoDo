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
    private readonly string _jsonFilePath;

    public RfpService(AutoDoDbContext context)
    {
        _context = context;
        _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"../api/RfpJson/opportunities.json");
    }

    public async Task<List<RFP>> FilterRfpDeadlineNotReachedYet()
    {
        return await _context.Rfps
            .Where(rfp => rfp.DeadlineDate >= DateTime.Today)
            .ToListAsync();
    }
    public void LoadRfpFromJson()
    {
        if (!File.Exists(_jsonFilePath))
            throw new FileNotFoundException($"Le fichier {_jsonFilePath} est introuvable.");

        try
        {
            string jsonContent = File.ReadAllText(_jsonFilePath);
            var rfps = JsonConvert.DeserializeObject<List<RFP>>(jsonContent) ?? new List<RFP>();

            if (rfps.Count > 0)
            {
                var existingReferences = _context.Rfps
                    .Select(rfp => rfp.Reference)
                    .ToHashSet();

                var newRfps = rfps
                    .Where(rfp => !existingReferences.Contains(rfp.Reference))
                    .ToList();

                if (newRfps.Count > 0)
                {
                    // Ajouter les nouveaux RFPs
                    _context.Rfps.AddRange(newRfps);
                    _context.SaveChanges();
                }
            }
        }
        catch (DbUpdateException dbEx)
        {
            throw new InvalidOperationException($"Erreur lors de la mise à jour de la base de données : {dbEx.Message}", dbEx);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erreur lors de la lecture du fichier JSON et de l'enregistrement en base de données.", ex);
        }
    }
}
