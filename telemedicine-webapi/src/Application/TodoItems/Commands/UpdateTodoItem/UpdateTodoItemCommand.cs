using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : IRequest<BaseResponse>
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public UpdateTodoItemCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.TodoItemRepository.GetById(request.Id);

        if (entity == null)return OperationResult.NotSuccessful("Not found");

        entity.Title = request.Title;
        entity.Done = request.Done;

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to update");
    }
}
