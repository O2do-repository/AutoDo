using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/keyword")]
[Authorize]
public class KeywordController : ControllerBase
{
    private readonly IKeywordService _keywordService;

    public KeywordController(IKeywordService keywordService)
    {
        _keywordService = keywordService;
    }

    [HttpGet]
    public IActionResult GetAllKeywords()
    {
        var keywords = _keywordService.GetAllKeywords();

        var outputKeywords = keywords.Select(keyword => new DtoOutputKeyword
        {
            KeywordUuid = keyword.KeywordUuid,
            Name = keyword.Name,
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Liste des mots-clés récupérée avec succès.",
            data = outputKeywords
        });
    }

    [HttpPost]
    public IActionResult CreateKeyword([FromBody] DtoInputKeyword keywordDto)
    {
        if (keywordDto == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Les données du mot-clé sont invalides."
            });
        }

        try
        {
            var keyword = new Keyword
            {
                KeywordUuid = Guid.NewGuid(),
                Name = keywordDto.Name,
            };

            var newKeyword = _keywordService.AddKeyword(keyword);

            return StatusCode(201, new
            {
                success = true,
                message = "Mot-clé créé avec succès.",
                data = newKeyword
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Erreur lors de la création du mot-clé.",
                details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpDelete("{keywordUuid}")]
    public IActionResult DeleteKeyword(Guid keywordUuid)
    {
        try
        {

            _keywordService.DeleteKeyword(keywordUuid);

            return Ok(new
            {
                success = true,
                message = "Mot-clé supprimé avec succès."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erreur lors de la suppression du mot-clé.",
                details = ex.Message
            });
        }
    }
}
