﻿using Microsoft.EntityFrameworkCore;
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

    public RfpService(AutoDoDbContext context,IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
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
                    .Where(rfp => rfps.Select(x => x.Reference).Contains(rfp.Reference)) // Vérifie si la référence existe dans les données existantes
                    .ToList();

                foreach (var rfp in rfps)
                {
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


        public async Task ImportRfpAndGenerateMatchingsAsync()
    {
        // Début de la transaction
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
