using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;

namespace telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;

public record UpdateHospitalCommand : IRequest
{
    public int Id { get; init; }

    public string? HospitalName { get; init; }
}

public class UpdateHospitalCommandHandler : IRequestHandler<UpdateHospitalCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateHospitalCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Hospitals
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        entity.HospitalName = request.HospitalName;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
