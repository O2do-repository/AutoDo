using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/profil")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IMatchingService _matchingService;

    public ProfileController(IProfileService profileService, IMatchingService matchingService)
    {
        _profileService = profileService;
        _matchingService = matchingService;
    }

    [HttpGet]
    public IActionResult GetAllProfils()
    {
        var profiles = _profileService.GetAllProfiles();

        var outputProfiles = profiles.Select(profile => new DtoOutputProfile
        {
            ProfileUuid = profile.ProfileUuid,
            ConsultantUuid = profile.ConsultantUuid,
            RateHour = profile.Ratehour,
            Cv = profile.CV,
            CvDate = profile.CVDate,
            JobTitle = profile.JobTitle,
            ExperienceLevel = profile.ExperienceLevel,
            Skills = profile.Skills ?? new List<Skill>(),
            Keywords = profile.Keywords ?? new List<Keyword>()
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Liste des profils récupérée avec succès.",
            data = outputProfiles
        });
    }

    [HttpGet("consultant/{consultantUuid}")]
    public IActionResult GetProfilesByConsultant(Guid consultantUuid)
    {
        var profiles = _profileService.GetProfilesByConsultant(consultantUuid);

        var outputProfiles = profiles.Select(profile => new DtoOutputProfile
        {
            ProfileUuid = profile.ProfileUuid,
            ConsultantUuid = profile.ConsultantUuid,
            RateHour = profile.Ratehour,
            Cv = profile.CV,
            CvDate = profile.CVDate,
            JobTitle = profile.JobTitle,
            ExperienceLevel = profile.ExperienceLevel,
            Skills = profile.Skills ?? new List<Skill>(),
            Keywords = profile.Keywords ?? new List<Keyword>()

        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Profils du consultant récupérés avec succès.",
            data = outputProfiles
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] DtoInputProfile profileDto)
    {
        if (profileDto == null || profileDto.ConsultantUuid == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Le profil est invalide ou le consultant est manquant."
            });
        }

        try
        {
            var profile = new Profile
            {
                ProfileUuid = Guid.NewGuid(),
                ConsultantUuid = profileDto.ConsultantUuid,
                Ratehour = profileDto.RateHour,
                CV = profileDto.Cv,
                CVDate = profileDto.CvDate,
                JobTitle = profileDto.JobTitle,
                ExperienceLevel = profileDto.ExperienceLevel,
            };

            var newProfile = await _profileService.AddProfile(profile, profileDto.SkillUuids, profileDto.KeywordUuids);
            var matchings = await _matchingService.MatchingsForProfileAsync(newProfile);

            return StatusCode(201, new
            {
                success = true,
                message = "Profil créé avec succès.",
                data = newProfile
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Erreur lors de la création du profil.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] DtoUpdateProfile profileDto)
    {
        if (profileDto == null || profileDto.ProfileUuid == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Données de profil invalides ou UUID manquant."
            });
        }

        try
        {
            var profile = new Profile
            {
                ProfileUuid = profileDto.ProfileUuid,
                ConsultantUuid = profileDto.ConsultantUuid,
                Ratehour = profileDto.RateHour,
                CV = profileDto.Cv,
                CVDate = profileDto.CvDate,
                JobTitle = profileDto.JobTitle,
                ExperienceLevel = profileDto.ExperienceLevel,
            };

            var updatedProfile = await _profileService.UpdateProfile(profile, profileDto.SkillUuids, profileDto.KeywordUuids);
            var matchings = await _matchingService.MatchingsForProfileAsync(updatedProfile);

            return Ok(new
            {
                success = true,
                message = "Profil mis à jour avec succès.",
                data = updatedProfile
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Erreur lors de la mise à jour du profil.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }


    [HttpDelete("{profileUuid}")]
    public IActionResult DeleteProfile(Guid profileUuid)
    {
        try
        {
            _profileService.DeleteProfile(profileUuid);

            return Ok(new
            {
                success = true,
                message = "Profil supprimé avec succès."
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                success = false,
                message = "Erreur lors de la suppression du profil.",
                details = ex.Message
            });
        }
    }
    
    [HttpPut("normalize-all")]
    public async Task<IActionResult> UpdateAllProfiles()
    {
        try
        {
            // Appel de la nouvelle méthode qui inclut la normalisation IA
            await _profileService.UpdateAllProfilesNormalizationAsync();
            
            return Ok(new
            {
                success = true,
                message = "Normalisation IA de tous les profils terminée avec succès."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erreur lors de la normalisation en masse.",
                details = ex.Message
            });
        }
    }
    // [HttpPut("backup-skills-keywords")]
    // public async Task<IActionResult> BackupSkillsAndKeywords()
    // {
    //     try
    //     {
    //         var updatedCount = await _profileService.BackupOldProfileSkillAndKeywordData();
    //         return Ok(new
    //         {
    //             success = true,
    //             message = $"Sauvegarde des champs Skills et Keywords effectuée pour {updatedCount} profils.",
    //             count = updatedCount
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new
    //         {
    //             success = false,
    //             message = "Erreur lors de la sauvegarde des champs Skills et Keywords.",
    //             details = ex.Message
    //         });
    //     }
    // }

}
