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
            Phone = consultant.Phone                           
        }).ToList();

        return Ok(outputConsultants);
    }

}