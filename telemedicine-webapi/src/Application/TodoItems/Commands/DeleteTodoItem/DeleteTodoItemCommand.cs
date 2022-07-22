using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Events;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest<BaseResponse>;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeleteTodoItemCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.TodoItemRepository.GetById(request.Id);
        if (entity == null)return OperationResult.NotSuccessful("Not found");

        _context.TodoItemRepository.Delete(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete");
    }
}
