using MediatR;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;

public record RespondToScheduledAppointmentCommand(
    int AppointmentId,
    bool Accept = true,
    string? Reason = null) : IRequest<BaseResponse>;

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

        if (appointment.Status is ScheduleStatus.Cancelled or ScheduleStatus.Attended or ScheduleStatus.Active or ScheduleStatus.Declined)
            return OperationResult.NotSuccessful($"An appointment with status {appointment.Status} cannot be responded to");
        if (!request.Accept && string.IsNullOrWhiteSpace(request.Reason))
            return OperationResult.NotSuccessful("A decline reason is required");

        var physician = await AppointmentAccess.EnsureCurrentPhysicianAsync(_context, _currentUser);
        if (appointment.PhysicianId.HasValue && appointment.PhysicianId != physician.Id)
            throw new ForbiddenAccessException();

        appointment.PhysicianId = physician.Id;
        appointment.Physician = physician;
        appointment.Status = request.Accept ? ScheduleStatus.Active : ScheduleStatus.Declined;
        appointment.StatusReason = request.Accept ? null : request.Reason!.Trim();
        appointment.LastModified = DateTime.UtcNow;
        appointment.LastModifiedBy = _currentUser.Email;

        _context.AppointmentRepository.Update(appointment);
        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful
            ? OperationResult.Successful(request.Accept ? "Appointment accepted" : "Appointment declined")
            : OperationResult.NotSuccessful("Unable to respond to appointment");
    }
}
