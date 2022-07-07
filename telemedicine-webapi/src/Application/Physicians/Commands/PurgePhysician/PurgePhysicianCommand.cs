using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;

namespace telemedicine_webapi.Application.Physicians.Commands.PurgePhysician;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public record PurgePhysicianCommand : IRequest;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgePhysicianCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeTodoListsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgePhysicianCommand request, CancellationToken cancellationToken)
    {
        _context.TodoLists.RemoveRange(_context.TodoLists);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
