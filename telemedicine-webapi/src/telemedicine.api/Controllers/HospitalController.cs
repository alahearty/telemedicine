using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;
using telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;
using telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;
using telemedicine_webapi.Application.Hospitals.Queries.GetTodoItemsWithPagination;
using telemedicine_webapi.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace telemedicine.api.Controllers;

[Authorize]
public class HospitalController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<HospitalDataDto>>> GetTodoItemsWithPagination([FromQuery] GetHospitaltemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateHospitalCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateHospitalCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
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
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteHospitalCommand(id));

        return NoContent();
    }
}
