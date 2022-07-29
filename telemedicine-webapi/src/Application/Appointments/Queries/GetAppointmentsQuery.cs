using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Queries;
public record GetAppointmentsQuery : IRequest<BaseResponse>;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetAppointmentsQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _context.AppointmentRepository.FindAsync(app => app.Status == ScheduleStatus.NotAttended);
        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

        return OperationResult.Successful(appointmentDtos);
    }
}
