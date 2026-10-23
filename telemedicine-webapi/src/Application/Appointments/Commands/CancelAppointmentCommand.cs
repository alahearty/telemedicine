using MediatR;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Commands;

public record CancelAppointmentCommand(int Id, string? Reason = null) : IRequest<BaseResponse>;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly ICurrentUserService _currentUser;

    public CancelAppointmentCommandHandler(IUnitOfWork context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.AppointmentRepository.GetByIdAsync(request.Id);

        if (appointment == null)
            return OperationResult.NotSuccessful($"Appointment with ID {request.Id} not found");

        await AppointmentAccess.EnsureCanAccessAsync(_context, _currentUser, appointment);

        if (appointment.Status == ScheduleStatus.Cancelled)
            return OperationResult.NotSuccessful("Appointment is already cancelled");
        if (appointment.Status == ScheduleStatus.Attended)
            return OperationResult.NotSuccessful("A completed appointment cannot be cancelled");

        appointment.Status = ScheduleStatus.Cancelled;
        appointment.StatusReason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
        appointment.LastModified = DateTime.UtcNow;
        appointment.LastModifiedBy = _currentUser.Email;

        _context.AppointmentRepository.Update(appointment);
        var commitResult = await _context.SaveChangesAsync(cancellationToken);

        return commitResult.WasSuccesful
            ? OperationResult.Successful("Appointment cancelled successfully")
            : OperationResult.NotSuccessful("Unable to cancel appointment");
    }
}
