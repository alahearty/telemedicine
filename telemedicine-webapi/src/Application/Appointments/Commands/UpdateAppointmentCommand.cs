using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Commands;

/// <summary>
/// Updates notes and/or reschedules an appointment. Status transitions are
/// intentionally excluded from this command; they have dedicated operations.
/// </summary>
public record UpdateAppointmentCommand : IRequest<BaseResponse>
{
    [Required]
    public int Id { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public int? DurationMinutes { get; set; }

    public string? Notes { get; set; }
}

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentCommandHandler(
        IUnitOfWork context,
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _context = context;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.AppointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null)
            return OperationResult.NotSuccessful($"Appointment with ID {request.Id} not found");

        await AppointmentAccess.EnsureCanAccessAsync(_context, _currentUser, appointment);

        if (!request.AppointmentDate.HasValue && request.Notes is null)
            return OperationResult.NotSuccessful("Provide a new appointment date or notes");

        if (request.AppointmentDate.HasValue)
        {
            if (appointment.Status is ScheduleStatus.Cancelled or ScheduleStatus.Declined or ScheduleStatus.Attended)
                return OperationResult.NotSuccessful("Only pending or active appointments can be rescheduled");
            if (!appointment.PhysicianId.HasValue)
                return OperationResult.NotSuccessful("The appointment has no assigned physician");

            var start = SlotCalculator.AsUtc(request.AppointmentDate.Value);
            var rules = await _db.PhysicianAvailabilities
                .Where(x => x.PhysicianId == appointment.PhysicianId.Value && x.IsActive)
                .ToListAsync(cancellationToken);
            var rangeStart = start.AddHours(-4);
            var rangeEnd = start.AddHours(5);
            var bookings = await _db.Appointments
                .Where(x => x.Id != appointment.Id
                            && x.PhysicianId == appointment.PhysicianId.Value
                            && x.Status != ScheduleStatus.Cancelled
                            && x.Status != ScheduleStatus.Declined
                            && x.AppointmentDate < rangeEnd
                            && x.ScheduleEndTime > rangeStart)
                .ToListAsync(cancellationToken);

            if (!SlotCalculator.TryResolve(rules, bookings, start, DateTime.UtcNow, out var slot))
                return OperationResult.NotSuccessful("The selected slot is outside the physician's availability or is no longer available");

            if (request.DurationMinutes.HasValue && request.DurationMinutes.Value != slot.DurationMinutes)
                return OperationResult.NotSuccessful($"The selected slot duration is {slot.DurationMinutes} minutes");

            appointment.AppointmentDate = slot.StartUtc;
            appointment.ScheduleEndTime = slot.EndUtc;
        }

        if (request.Notes is not null)
            appointment.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        appointment.LastModified = DateTime.UtcNow;
        appointment.LastModifiedBy = _currentUser.Email;

        _context.AppointmentRepository.Update(appointment);
        try
        {
            var commitResult = await _context.SaveChangesAsync(cancellationToken);
            return commitResult.WasSuccesful
                ? OperationResult.Successful(new
                {
                    appointment.Id,
                    appointment.AppointmentDate,
                    appointment.ScheduleEndTime,
                    appointment.Status
                })
                : OperationResult.NotSuccessful("Unable to update appointment");
        }
        catch (DbUpdateException)
        {
            return OperationResult.NotSuccessful("The selected slot is no longer available");
        }
    }
}
