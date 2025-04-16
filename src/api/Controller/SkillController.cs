using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/skill")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;


    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;

    }

    [HttpGet]
    public IActionResult GetAllSkills()
    {
        var skills = _skillService.GetAllSkills();

        var outputSkill = skills.Select(skill => new DtoOutputSkill
        {
            SkillUuid = skill.SkillUuid,                     
            Name = skill.Name,                            
                  
        }).ToList();

        return Ok(outputSkill);
    }

    [HttpPost]
    public IActionResult CreateSkill([FromBody] DtoInputSkill skillDto)
    {
        if (skillDto == null)
        {
            throw new ArgumentException("SkillUuid is empty or invalid");
        }

        try
        {
            var skill = new Skill
            {
                SkillUuid = Guid.NewGuid(),
                Name = skillDto.Name,
            };

            var newSkill = _skillService.AddSkill(skill);

            return Ok(newSkill);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }


    [HttpDelete("{skillUuid}")]
    public IActionResult DeleteSkill(Guid skillUuid)
    {
        try
        {
            _skillService.DeleteSkill(skillUuid);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
  

}