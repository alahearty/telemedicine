using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public record PurgePatientCommand : IRequest;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgePatientCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeTodoListsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgePatientCommand request, CancellationToken cancellationToken)
    {
        _context.TodoLists.RemoveRange(_context.TodoLists);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
