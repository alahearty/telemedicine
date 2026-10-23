using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetPatientAppointmentsQuery(int PatientId) : IRequest<BaseResponse>;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, BaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetPatientAppointmentsQueryHandler(
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

    public async Task<BaseResponse> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        if (AppointmentAccess.IsPatient(_currentUser))
        {
            var currentPatient = await AppointmentAccess.EnsureCurrentPatientAsync(_unitOfWork, _currentUser);
            if (currentPatient.Id != request.PatientId)
                throw new ForbiddenAccessException();
        }
        else if (!AppointmentAccess.IsAdministrator(_currentUser))
        {
            throw new ForbiddenAccessException();
        }

        var appointments = await _context.Set<Appointment>()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Physician)
            .Include(appointment => appointment.TelemedicineService)
            .Where(appointment => appointment.PatientId == request.PatientId)
            .OrderByDescending(appointment => appointment.Id)
            .ToListAsync(cancellationToken);

        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);
        return OperationResult.Successful(appointmentDtos);
    }
}
