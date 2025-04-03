using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/consultant")]
public class ConsultantController : ControllerBase
{
    private readonly IConsultantService _consultantService;


    public ConsultantController(IConsultantService consultantService)
    {
        _consultantService = consultantService;

    }

    [HttpGet]
    public IActionResult GetAllConsultants()
    {
        var consultants = _consultantService.GetAllConsultants();

        var outputConsultants = consultants.Select(consultant => new DtoOutputConsultant
        {
            ConsultantUuid = consultant.ConsultantUuid,      
            Email = consultant.Email,                          
            AvailabilityDate = consultant.AvailabilityDate,    
            ExpirationDateCI = consultant.ExpirationDateCI,    
            Intern = consultant.Intern,                        
            Name = consultant.Name,                            
            Surname = consultant.Surname,                      
            enterprise = consultant.enterprise,                
            Phone = consultant.Phone,
            CopyCI = consultant.CopyCI,
            Picture = consultant.Picture                        
        }).ToList();

        return Ok(outputConsultants);
        }
    [HttpPost]
    public async Task<IActionResult> CreateConsultant([FromBody] DtoInputConsultant consultantDto)
    {
        if (consultantDto == null)
        {
            throw new ArgumentException("ConsultantUuid is empty or invalid");
        }

        try
        {
            var consultant = new Consultant
            {
                ConsultantUuid = Guid.NewGuid(),
                Email = consultantDto.Email,
                AvailabilityDate = consultantDto.AvailabilityDate,
                ExpirationDateCI = consultantDto.ExpirationDateCI,
                Intern = consultantDto.Intern,
                Name = consultantDto.Name,
                Surname = consultantDto.Surname,
                enterprise = consultantDto.enterprise,
                Phone = consultantDto.Phone,
                CopyCI = consultantDto.CopyCI,
                Picture = consultantDto.Picture
            };

            // Ajouter le consultant à la base de données
            var newConsultant = _consultantService.AddConsultant(consultant);

            // Retourner le profil créé
            return CreatedAtAction(nameof(GetConsultantById), new { id = newConsultant.ConsultantUuid }, newConsultant);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }
    [HttpGet("{id}")]
    public IActionResult GetConsultantById(Guid id)
    {
        var consultant = _consultantService.GetConsultantById(id);
        if (consultant == null)
        {
            return NotFound(new { message = "Consultant non trouvé." });
        }
        return Ok(consultant);
    }


}