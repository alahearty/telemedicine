using MediatR;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResponse(BaseResponse response)
    {
        if (response.WasSuccesful)
        {
            return Ok(response);
        }

        if (response.Errors.Any())
        {
            return BadRequest(response);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    protected IActionResult HandleResponse(BaseResponse response, int successStatusCode)
    {
        if (response.WasSuccesful)
        {
            return StatusCode(successStatusCode, response);
        }

        if (response.Errors.Any())
        {
            return BadRequest(response);
        }

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
