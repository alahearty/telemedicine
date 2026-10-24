using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.TeleMedicinePayment;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemedicinePaymentController : ApiControllerBase
{
    [Permit("Patient")]
    [HttpPost]
    [EnableRateLimiting("payment")]
    public async Task<IActionResult> MakePayment([FromBody] MakePaymentCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }
}
