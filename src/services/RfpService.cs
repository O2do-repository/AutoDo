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
    private readonly IMatchingService _matchingService;
    private readonly ITranslationService _translationService;

    public RfpService(AutoDoDbContext context,IMatchingService matchingService, ITranslationService translationService)
    {
        _context = context;
        _matchingService = matchingService;
        _translationService = translationService;
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
                // Charger toutes les références déjà présentes en base
                var existingReferences = _context.Rfps
                    .Where(rfp => rfps.Select(x => x.Reference).Contains(rfp.Reference)) 
                    .ToList();

                foreach (var rfp in rfps)
                {
                    rfp.Skills ??= new List<string>();
                    var existingRfp = existingReferences.FirstOrDefault(r => r.Reference == rfp.Reference);

                    if (existingRfp != null)
                    {
                        // Mettre à jour les champs existants
                        existingRfp.JobTitle = rfp.JobTitle;
                        existingRfp.DescriptionBrut = rfp.DescriptionBrut;
                        existingRfp.PublicationDate = rfp.PublicationDate;
                        existingRfp.DeadlineDate = rfp.DeadlineDate;
                        existingRfp.RfpUrl = rfp.RfpUrl;
                        existingRfp.Workplace = rfp.Workplace;
                        existingRfp.RfpPriority = rfp.RfpPriority;
                        existingRfp.ExperienceLevel = rfp.ExperienceLevel;
                    }
                    else
                    {
                        // Ajouter les nouveaux RFPs à la base
                        _context.Rfps.Add(rfp);
                    }
                }

                // Sauvegarde en une seule transaction
                _context.SaveChanges();
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

            // Traduction des champs vers l'anglais
            rfp.JobTitle = await _translationService.TranslateTextAsync(rfp.JobTitle);
            rfp.DescriptionBrut = await _translationService.TranslateTextAsync(rfp.DescriptionBrut);

            var translatedSkills = new List<string>();
            foreach (var skill in rfp.Skills)
            {
                var translatedSkill = await _translationService.TranslateTextAsync(skill);
                translatedSkills.Add(translatedSkill);
            }
            rfp.Skills = translatedSkills;

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




    public async Task ImportRfpAndGenerateMatchings()
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                LoadRfpFromJson();
                
                var rfps = await _context.Rfps.ToListAsync();

                await _matchingService.MatchingsForRfpsAsync(rfps);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }
    }

}
