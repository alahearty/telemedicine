using MediatR;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;

public record RespondToScheduledAppointmentCommand(int AppointmentId, bool Accept = true) : IRequest<BaseResponse>;

public class RespondToScheduledAppointmentCommandHandler
    : IRequestHandler<RespondToScheduledAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;

    public RespondToScheduledAppointmentCommandHandler(
        IUnitOfWork context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(
        RespondToScheduledAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var appointment = await _context.AppointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            throw new NotFoundException("Appointment", request.AppointmentId);

        if (appointment.Status == ScheduleStatus.Cancelled)
            return OperationResult.NotSuccessful("A cancelled appointment cannot be accepted");
        if (appointment.Status == ScheduleStatus.Active)
            return OperationResult.NotSuccessful("This appointment has already been accepted");
        if (appointment.Status == ScheduleStatus.Attended)
            return OperationResult.NotSuccessful("This appointment has already been completed");

        if (string.Equals(_currentUser.Role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            var physician = await AppointmentAccess.EnsureCurrentPhysicianAsync(_context, _currentUser);
            if (appointment.PhysicianId.HasValue && appointment.PhysicianId != physician.Id)
                throw new ForbiddenAccessException();

            // Accepting an unassigned request assigns it to the responding physician.
            appointment.PhysicianId = physician.Id;
            appointment.Physician = physician;
        }
        else if (!string.Equals(_currentUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException();
        }

        appointment.Status = request.Accept ? ScheduleStatus.Active : ScheduleStatus.Declined;
        appointment.LastModified = DateTime.UtcNow;
        appointment.LastModifiedBy = _currentUser.Email;

        _context.AppointmentRepository.Update(appointment);
        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful
            ? OperationResult.Successful(request.Accept ? "Appointment accepted" : "Appointment declined")
            : OperationResult.NotSuccessful("Unable to accept appointment");
    }
}
