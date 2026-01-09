using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetAppointmentByIdQuery(int Id) : IRequest<BaseResponse>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(IUnitOfWork context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.AppointmentRepository.GetByIdAsync(request.Id);
        
        if (appointment == null)
        {
            return OperationResult.NotSuccessful($"Appointment with ID {request.Id} not found");
        }

        var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
        return OperationResult.Successful(appointmentDto);
    }
}

