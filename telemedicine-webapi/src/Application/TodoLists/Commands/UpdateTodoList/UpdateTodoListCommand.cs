using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TodoLists.Commands.UpdateTodoList;

public record UpdateTodoListCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }

    public string? Title { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public UpdateTodoListCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity =  _context.TodoListRepository.GetById(request.Id);
        if (entity == null)return OperationResult.NotSuccessful("Not found");

        entity.Title = request.Title;

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to update todo list");
    }
}
