using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.TeleMedicinePayment;

namespace telemedicine.api.Controllers;

public class TelemedicinePaymentController:ApiControllerBase
{
    [HttpPost] 
    public async Task<IActionResult> MakePayment([FromBody] MakePaymentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}
