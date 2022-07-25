using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;
using telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;
using telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;

namespace telemedicine.api.Controllers;

//[Authorize]
public class HospitalController : ApiControllerBase
{
    // [HttpGet]
    // public async Task<IActionResult> GetTodoItemsWithPagination([FromQuery] GetHospitaltemsWithPaginationQuery query)
    // {
    //     var response= await Mediator.Send(query);
    //     return Ok(response);
    // }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHospitalCommand command)
    {
        var createAction= await Mediator.Send(command);
        return Ok(createAction.Data);
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] UpdateHospitalCommand command)
    {
        var updateAction=await Mediator.Send(command);

        return Ok(updateAction.Data);
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
        var deleteAction=await Mediator.Send(new DeleteHospitalCommand(id));

        return Ok(deleteAction.Data);
    }
}
