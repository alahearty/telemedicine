using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(int Id) : IRequest<BaseResponse>;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public DeleteTodoListCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoListRepository.GetByIdAsync(request.Id);
        if (entity == null)return OperationResult.NotSuccessful("Not found");
        _context.TodoListRepository.Delete(entity);

        var commitResult=await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete todo list");
    }
}
