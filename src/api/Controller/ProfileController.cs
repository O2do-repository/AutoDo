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
            Skills = profile.Skills ?? new List<string>(),
            Keywords = profile.Keywords ?? new List<string>()
        }).ToList();

        return Ok(outputProfiles);
    }



    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] DtoInputProfile profileDto)
    {
        if (profileDto == null || profileDto.ConsultantUuid == Guid.Empty)
        {
            return BadRequest("Le profil est invalide ou le consultant est manquant.");
        }

        Console.WriteLine($"Consultant UUID reçu : {profileDto.ConsultantUuid}");
        try
        {
            if (profileDto.ConsultantUuid == Guid.Empty)
            {
                throw new ArgumentException("ConsultantUuid is empty or invalid");
            }

            var profile = new Profile
            {
                ProfileUuid = Guid.NewGuid(),
                ConsultantUuid = profileDto.ConsultantUuid,
                Ratehour = profileDto.RateHour,
                CV = profileDto.Cv,
                CVDate = profileDto.CvDate,
                JobTitle = profileDto.JobTitle,
                ExperienceLevel = profileDto.ExperienceLevel,
                Skills = profileDto.Skills.ToList(),
                Keywords = profileDto.Keywords.ToList()
            };


            // Ajouter le profil à la base de données
            var newProfile = _profileService.AddProfile(profile);

            //temporaire -> switch vers un bus
            // Appeler la méthode de matching après la création du profil
            var matchings = await _matchingService.MatchingsForProfileAsync(newProfile);

            return CreatedAtAction(nameof(GetAllProfils), new { id = newProfile.ProfileUuid }, newProfile);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] DtoUpdateProfile profileDto)
    {
        if (profileDto == null)
        {
            return BadRequest("Données de profil invalides.");
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
                Skills = profileDto.Skills?.ToList() ?? new List<string>(),
                Keywords = profileDto.Keywords?.ToList() ?? new List<string>()
            };
            
            var updatedProfile = _profileService.UpdateProfile(profile);

            var matchings = await _matchingService.MatchingsForProfileAsync(updatedProfile);

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            return ex.Message.Contains("n'existe pas") 
                ? NotFound(ex.Message) 
                : BadRequest(ex.Message);
        }
    }


}