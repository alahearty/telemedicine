using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetPatientAppointmentsQuery(int PatientId) : IRequest<BaseResponse>;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetPatientAppointmentsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _context.AppointmentRepository.FindAsync(app => app.PatientId == request.PatientId);
        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

        return OperationResult.Successful(appointmentDtos);
    }
}

