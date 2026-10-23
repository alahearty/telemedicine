using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Appointments.Queries;

public record GetPhysicianAppointmentsQuery(int PhysicianId) : IRequest<BaseResponse>;

public class GetPhysicianAppointmentsQueryHandler : IRequestHandler<GetPhysicianAppointmentsQuery, BaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetPhysicianAppointmentsQueryHandler(
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

    public async Task<BaseResponse> Handle(GetPhysicianAppointmentsQuery request, CancellationToken cancellationToken)
    {
        if (AppointmentAccess.IsPhysician(_currentUser))
        {
            var currentPhysician = await AppointmentAccess.EnsureCurrentPhysicianAsync(_unitOfWork, _currentUser);
            if (currentPhysician.Id != request.PhysicianId)
                throw new ForbiddenAccessException();
        }
        else if (!AppointmentAccess.IsAdministrator(_currentUser))
        {
            throw new ForbiddenAccessException();
        }

        var appointments = await _context.Set<Domain.Entities.Appointment>()
            .Include(a => a.Patient)
            .Include(a => a.Physician)
            .Include(a => a.TelemedicineService)
            .Where(app => app.PhysicianId == request.PhysicianId)
            .OrderByDescending(app => app.Id)
            .ToListAsync(cancellationToken);
        var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

        return OperationResult.Successful(appointmentDtos);
    }
}
