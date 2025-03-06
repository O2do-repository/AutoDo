using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/profil")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

[HttpGet]
public IActionResult GetAllProfils()
{
    var profiles = _profileService.GetAllProfiles();

        var outputProfiles = profiles.Select(profile => new DtoOutputProfile
        {
            RateHour = profile.Ratehour,
            Cv = profile.CV,
            CvDate = profile.CVDate,
            JobTitle = profile.JobTitle,
            ExperienceLevel = profile.ExperienceLevel,  // 🔹 Grâce au JsonStringEnumConverter, ça renverra une string
            Skills = profile.Skills ?? new List<string>(),
            Keywords = profile.Keywords ?? new List<string>()
        }).ToList();

        return Ok(outputProfiles);
    }



    [HttpPost]
    public IActionResult CreateProfile([FromBody] DtoInputProfile profileDto)
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

                

            var newProfile = _profileService.AddProfile(profile);
            return CreatedAtAction(nameof(GetAllProfils), new { id = newProfile.ProfileUuid }, newProfile);
        }
catch (Exception ex)
{
    return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
}

    }

}