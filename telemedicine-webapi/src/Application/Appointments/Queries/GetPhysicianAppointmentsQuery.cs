using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetPhysicianAppointmentsQuery(int PhysicianId) : IRequest<BaseResponse>;

public class GetPhysicianAppointmentsQueryHandler : IRequestHandler<GetPhysicianAppointmentsQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetPhysicianAppointmentsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetPhysicianAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _context.AppointmentRepository.FindAsync(app => app.PhysicianId == request.PhysicianId);
        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

        return OperationResult.Successful(appointmentDtos);
    }
}

