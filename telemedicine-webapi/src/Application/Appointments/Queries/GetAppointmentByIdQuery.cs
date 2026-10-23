using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetAppointmentByIdQuery(int Id) : IRequest<BaseResponse>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, BaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetAppointmentByIdQueryHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Set<Appointment>()
            .Include(item => item.Patient)
            .Include(item => item.Physician)
            .Include(item => item.TelemedicineService)
            .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken);

        if (appointment == null)
        {
            return OperationResult.NotSuccessful($"Appointment with ID {request.Id} not found");
        }

        await AppointmentAccess.EnsureCanAccessAsync(_unitOfWork, _currentUser, appointment);

        var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
        return OperationResult.Successful(appointmentDto);
    }
}
