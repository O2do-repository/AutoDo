using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/keyword")]
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

        return Ok(outputKeywords);
    }

    [HttpPost]
    public IActionResult CreateKeyword([FromBody] DtoInputKeyword keywordDto)
    {
        if (keywordDto == null)
        {
            throw new ArgumentException("Keyword data is invalid");
        }

        try
        {
            var keyword = new Keyword
            {
                KeywordUuid = Guid.NewGuid(),
                Name = keywordDto.Name,
            };

            var newKeyword = _keywordService.AddKeyword(keyword);

            return Ok(newKeyword);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }


    [HttpDelete("{keywordUuid}")]
    public IActionResult DeleteKeyword(Guid keywordUuid)
    {
        try
        {
            _keywordService.DeleteKeyword(keywordUuid);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
