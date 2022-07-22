using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.TodoLists.Commands.PurgeTodoLists;

// [Authorize(Roles = "Administrator")]
// [Authorize(Policy = "CanPurge")]
public record PurgeTodoListsCommand : IRequest<BaseResponse>;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand,BaseResponse>
{
    private readonly IUnitOfWork _context;

    public PurgeTodoListsCommandHandler(IUnitOfWork context)
    {
        _context = context;
    }

    public async Task<BaseResponse> Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        var todoLists=await _context.TodoListRepository.GetAllAsync();
        _context.TodoListRepository.DeleteMany(todoLists);

        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete all todo lists");
    }
}
