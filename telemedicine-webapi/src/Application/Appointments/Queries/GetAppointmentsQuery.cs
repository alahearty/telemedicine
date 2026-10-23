using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Queries;
public record GetAppointmentsQuery : IRequest<BaseResponse>;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, BaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetAppointmentsQueryHandler(
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

    public async Task<BaseResponse> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Set<Appointment>()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Physician)
            .Include(appointment => appointment.TelemedicineService)
            .Where(appointment => appointment.Status == ScheduleStatus.NotAttended);

        if (AppointmentAccess.IsPhysician(_currentUser))
        {
            var physician = await AppointmentAccess.EnsureCurrentPhysicianAsync(_unitOfWork, _currentUser);
            query = query.Where(appointment =>
                appointment.PhysicianId == null || appointment.PhysicianId == physician.Id);
        }
        else if (!AppointmentAccess.IsAdministrator(_currentUser))
        {
            throw new ForbiddenAccessException();
        }

        var appointments = await query
            .OrderByDescending(appointment => appointment.Id)
            .ToListAsync(cancellationToken);

        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);
        return OperationResult.Successful(appointmentDtos);
    }
}
