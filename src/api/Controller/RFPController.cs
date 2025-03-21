using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("/rfp")] 
public class RfpController : ControllerBase
{
    private readonly IRfpService _rfpService;

    public RfpController(IRfpService rfpService)
    {
        _rfpService = rfpService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredRfpsAsync()
    {
        try
        {
            var filteredRfps = await _rfpService.FilterRfpDeadlineNotReachedYet();
            return Ok(filteredRfps);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur est survenue.", error = ex.Message });
        }
    }
}
