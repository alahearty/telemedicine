using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.ScheduleAppointment;

public record ScheduleAppointmentCommand(
    int serviceId,
    string paymentToken,
    DateTime? appointmentDate = null,
    int? physicianId = null,
    int durationMinutes = 30) : IRequest<BaseResponse>;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ICurrentUserService _user;
    private readonly IApplicationDbContext _db;

    public ScheduleAppointmentCommandHandler(
        IUnitOfWork context,
        IMemoryCache memoryCache,
        ICurrentUserService user,
        IApplicationDbContext db)
    {
        _context = context;
        _memoryCache = memoryCache;
        _user = user;
        _db = db;
    }

    public async Task<BaseResponse> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var teleMedService = await _context.TelemedicineServiceRepository.GetByIdAsync(request.serviceId);
        if (teleMedService == null)
            return OperationResult.NotSuccessful("Telemedicine service not found");

        if (string.IsNullOrWhiteSpace(request.paymentToken) || !_memoryCache.TryGetValue(request.paymentToken, out _))
            return OperationResult.NotSuccessful("Payment token is invalid or has already been used");

        if (!request.appointmentDate.HasValue)
            return OperationResult.NotSuccessful("A future appointment date is required");

        if (!request.physicianId.HasValue)
            return OperationResult.NotSuccessful("A physician is required");

        var physician = await _context.PhysicianRepository.GetByIdAsync(request.physicianId.Value);
        if (physician == null)
            return OperationResult.NotSuccessful("Selected physician was not found");

        var patient = await _db.Patients.FirstOrDefaultAsync(x =>
            (_user.UserId != Guid.Empty && x.IdentityUserId == _user.UserId)
            || x.Email == _user.Email, cancellationToken);
        if (patient == null)
            return OperationResult.NotSuccessful("Patient profile not found. Please complete your profile first.");

        var start = SlotCalculator.AsUtc(request.appointmentDate.Value);
        var rules = await _db.PhysicianAvailabilities
            .Where(x => x.PhysicianId == request.physicianId.Value && x.IsActive)
            .ToListAsync(cancellationToken);
        var rangeStart = start.AddHours(-4);
        var rangeEnd = start.AddHours(5);
        var booked = await _db.Appointments
            .Where(x => x.PhysicianId == request.physicianId.Value
                        && x.Status != ScheduleStatus.Cancelled
                        && x.Status != ScheduleStatus.Declined
                        && x.AppointmentDate < rangeEnd
                        && x.ScheduleEndTime > rangeStart)
            .ToListAsync(cancellationToken);

        if (!SlotCalculator.TryResolve(rules, booked, start, DateTime.UtcNow, out var slot))
            return OperationResult.NotSuccessful("The selected slot is outside the physician's availability or is no longer available");

        var appointment = new Appointment
        {
            Created = DateTime.UtcNow,
            CreatedBy = _user.Email,
            Patient = patient,
            Physician = physician,
            AppointmentDate = slot.StartUtc,
            ScheduleEndTime = slot.EndUtc,
            Status = ScheduleStatus.Pending,
            TelemedicineService = teleMedService
        };

        _context.AppointmentRepository.Add(appointment);

        try
        {
            var commitResult = await _context.SaveChangesAsync(cancellationToken);
            if (!commitResult.WasSuccesful)
                return OperationResult.NotSuccessful("Unable to schedule appointment");
        }
        catch (DbUpdateException)
        {
            // PostgreSQL exclusion constraint EX_Appointments_Physician_Time is the
            // final guard when two requests race past the application-level check.
            return OperationResult.NotSuccessful("The selected slot is no longer available");
        }

        // Consume the token only after the booking has committed successfully.
        _memoryCache.Remove(request.paymentToken);

        return OperationResult.Successful(new
        {
            id = appointment.Id,
            start = slot.StartUtc,
            end = slot.EndUtc,
            durationMinutes = slot.DurationMinutes,
            status = ScheduleStatus.Pending
        });
    }
}
