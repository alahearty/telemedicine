using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

public record UpdatePatientCommand : IRequest
{
    public int Id { get; init; }

    public string? Name { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdatePatientCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Hospitals
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Hospital), request.Id);
        }

        entity.HospitalName = request.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
