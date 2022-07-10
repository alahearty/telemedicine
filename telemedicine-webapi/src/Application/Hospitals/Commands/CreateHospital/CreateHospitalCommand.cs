using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;

public record CreateHospitalCommand : IRequest<int>
{
    public string? Name { get; init; }
}

public class CreateHospitalCommandHandler : IRequestHandler<CreateHospitalCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateHospitalCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = new Hospital();

        entity.HospitalName = request.Name;

        _context.Hospitals.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
