using Microsoft.AspNetCore.Authorization;
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

        return Ok(new
        {
            success = true,
            message = "Liste des compétences récupérée avec succès.",
            data = outputSkill
        });
    }

    [HttpPost]
    public IActionResult CreateSkill([FromBody] DtoInputSkill skillDto)
    {
        if (skillDto == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Les données de la compétence sont invalides."
            });
        }

        try
        {
            var skill = new Skill
            {
                SkillUuid = Guid.NewGuid(),
                Name = skillDto.Name,
            };

            var newSkill = _skillService.AddSkill(skill);

            return StatusCode(201, new
            {
                success = true,
                message = "Compétence créée avec succès.",
                data = newSkill
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Erreur lors de la création de la compétence.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpDelete("{skillUuid}")]
    public IActionResult DeleteSkill(Guid skillUuid)
    {
        try
        {


            _skillService.DeleteSkill(skillUuid);

            return Ok(new
            {
                success = true,
                message = "Compétence supprimée avec succès."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erreur lors de la suppression de la compétence.",
                details = ex.Message
            });
        }
    }
}
