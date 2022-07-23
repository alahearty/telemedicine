using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Patients.Commands.CreatePatients;
using telemedicine_webapi.Application.Patients.Commands.DeletePatients;
using telemedicine_webapi.Application.Patients.Commands.UpdatePatients;
using telemedicine_webapi.Application.Patients.Queries.GetPatients;
namespace telemedicine.api.Controllers;

////[Authorize]
public class PatientController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPatientsWithPagination([FromQuery] GetPatientsQuery query)
    {
        var response= await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] UpdatePatientCommand command)
    {
        var response=await Mediator.Send(command);

        return Ok(response.Data);
    }

    //[HttpPut("[action]")]
    //public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    //{
    //    if (id != command.Id)
    //    {
    //        return BadRequest();
    //    }

    //    await Mediator.Send(command);

    //    return NoContent();
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeletePatientCommand(id));

        return Ok(response.Data);
    }
}
