using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Security;
using MediatR;

namespace telemedicine_webapi.Application.Hospitals.Commands.PurgeHospital;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public record PurgeHospitalsCommand : IRequest;

public class PurgeHospitalsCommandHandler : IRequestHandler<PurgeHospitalsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeHospitalsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeHospitalsCommand request, CancellationToken cancellationToken)
    {
        _context.Hospitals.RemoveRange(_context.Hospitals);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
