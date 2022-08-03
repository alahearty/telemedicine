using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.TeleMedicinePayment;

namespace telemedicine.api.Controllers;

public class TelemedicinePaymentController:ApiControllerBase
{
    [Permit("Patient")]
    [HttpPost]
    public async Task<IActionResult> MakePayment([FromBody] MakePaymentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}
