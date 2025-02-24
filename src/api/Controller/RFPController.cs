using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/rfp")]
public class RFPontroller : ControllerBase{
    private readonly IRfpService _rfpService;

    public RFPontroller(IRfpService rfpService)
    {
        _rfpService = rfpService;
    }

    [HttpGet]
    public IActionResult GetFilteredRfps()
    {
        var receivedDataList = DummyRfpData.GetDummyRfpList();
        var rfpList = receivedDataList.Select(data => data.ToRFP()).ToList();
        var filteredRfps = _rfpService.FilterRfpDeadlineNotReachedYet(rfpList);

        return Ok(filteredRfps);
    }
}