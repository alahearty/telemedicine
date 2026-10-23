using MediatR;
using Microsoft.Extensions.Caching.Memory;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public ScheduleAppointmentCommandHandler(IUnitOfWork context, IMemoryCache memoryCache, ICurrentUserService user, IApplicationDbContext db)
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

        if (!ValidatePaymentToken(request.paymentToken))
            return OperationResult.NotSuccessful("Payment token is invalid or has already been used");

        if (!request.appointmentDate.HasValue || request.appointmentDate.Value <= DateTime.UtcNow)
            return OperationResult.NotSuccessful("A future appointment date is required");
        if (request.durationMinutes is < 10 or > 240)
            return OperationResult.NotSuccessful("Appointment duration must be between 10 and 240 minutes");

        Physician? physician = null;
        if (request.physicianId.HasValue)
        {
            physician = await _context.PhysicianRepository.GetByIdAsync(request.physicianId.Value);
            if (physician == null)
                return OperationResult.NotSuccessful("Selected physician was not found");
        }
        else return OperationResult.NotSuccessful("A physician is required");

        var patient = await _context.PatientRepository.FirstOrDefaultAsync(user => user.Email == _user.Email!);
        if (patient == null)
            return OperationResult.NotSuccessful("Patient profile not found. Please complete your profile first.");

        var start = request.appointmentDate.Value;
        var end = start.AddMinutes(request.durationMinutes);
        var availability = await _db.PhysicianAvailabilities.AnyAsync(x => x.PhysicianId == request.physicianId && x.IsActive && x.DayOfWeek == start.DayOfWeek && x.StartTime <= start.TimeOfDay && x.EndTime >= end.TimeOfDay, cancellationToken);
        if (!availability) return OperationResult.NotSuccessful("The selected time is outside the physician's availability");
        var overlaps = await _db.Appointments.AnyAsync(x => x.PhysicianId == request.physicianId && x.Status != ScheduleStatus.Cancelled && x.Status != ScheduleStatus.Declined && x.AppointmentDate < end && x.ScheduleEndTime > start, cancellationToken);
        if (overlaps) return OperationResult.NotSuccessful("The selected slot is no longer available");

        // A payment token is single-use; consume it only once all validations pass.
        _memoryCache.Remove(request.paymentToken);

        var appointment = new Appointment
        {
            Created = DateTime.UtcNow,
            Patient = patient,
            Physician = physician,
            AppointmentDate = start,
            ScheduleEndTime = end,
            Status = ScheduleStatus.Pending,
            TelemedicineService = teleMedService
        };

        _context.AppointmentRepository.Add(appointment);

        var commitResult = await _context.SaveChangesAsync(cancellationToken);
        return commitResult.WasSuccesful
            ? OperationResult.Successful(appointment.Id)
            : OperationResult.NotSuccessful("Unable to schedule appointment");
    }

    private bool ValidatePaymentToken(string paymentToken)
    {
        var storedToken = _memoryCache.Get(paymentToken);
        return storedToken != null;
    }
}
