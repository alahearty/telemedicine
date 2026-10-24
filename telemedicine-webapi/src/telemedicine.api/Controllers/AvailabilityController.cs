using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/physicians")]
public class AvailabilityController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _user;

    public AvailabilityController(IApplicationDbContext db, ICurrentUserService user)
    {
        _db = db;
        _user = user;
    }

    [HttpPut("me/availability")]
    [Permit("Physician")]
    public async Task<IActionResult> Replace([FromBody] List<AvailabilityInput>? input, CancellationToken ct)
    {
        var physician = await _db.Physicians.SingleOrDefaultAsync(x =>
            (_user.UserId != Guid.Empty && x.IdentityUserId == _user.UserId)
            || x.Email == _user.Email, ct);
        if (physician is null) return NotFound("Physician profile not found");

        input ??= new List<AvailabilityInput>();
        if (input.Any(x => x.StartTime >= x.EndTime || x.SlotDurationMinutes is < 10 or > 240))
            return BadRequest("Each availability interval requires start < end and a slot duration between 10 and 240 minutes");
        foreach (var dayGroup in input.GroupBy(x => x.DayOfWeek))
        {
            var ordered = dayGroup.OrderBy(x => x.StartTime).ToList();
            for (var i = 1; i < ordered.Count; i++)
            {
                if (ordered[i].StartTime < ordered[i - 1].EndTime)
                    return BadRequest($"Availability intervals overlap on {dayGroup.Key}");
            }
        }

        await using var transaction = _db.Database.IsRelational()
            ? await _db.Database.BeginTransactionAsync(ct)
            : null;

        try
        {
            var old = _db.PhysicianAvailabilities.Where(x => x.PhysicianId == physician.Id);
            _db.PhysicianAvailabilities.RemoveRange(old);
            await _db.SaveChangesAsync(ct);

            foreach (var item in input.OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime))
            {
                _db.PhysicianAvailabilities.Add(new PhysicianAvailability
                {
                    PhysicianId = physician.Id,
                    DayOfWeek = item.DayOfWeek,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    SlotDurationMinutes = item.SlotDurationMinutes,
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    CreatedBy = _user.Email
                });
            }

            await _db.SaveChangesAsync(ct);
            if (transaction is not null) await transaction.CommitAsync(ct);
        }
        catch
        {
            if (transaction is not null) await transaction.RollbackAsync(ct);
            throw;
        }

        return Ok(await _db.PhysicianAvailabilities
            .Where(x => x.PhysicianId == physician.Id && x.IsActive)
            .OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime)
            .Select(x => new { x.Id, x.PhysicianId, x.DayOfWeek, x.StartTime, x.EndTime, x.SlotDurationMinutes, x.IsActive })
            .ToListAsync(ct));
    }

    [HttpGet("{physicianId:int}/availability")]
    [Permit("Patient, Physician, Administrator")]
    public async Task<IActionResult> Get(int physicianId, CancellationToken ct) => Ok(await _db.PhysicianAvailabilities
        .Where(x => x.PhysicianId == physicianId && x.IsActive)
        .OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime)
        .Select(x => new { x.Id, x.PhysicianId, x.DayOfWeek, x.StartTime, x.EndTime, x.SlotDurationMinutes, x.IsActive })
        .ToListAsync(ct));

    [HttpGet("{physicianId:int}/slots")]
    [Permit("Patient, Physician, Administrator")]
    public async Task<IActionResult> Slots(
        int physicianId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var fromUtc = SlotCalculator.AsUtc(from ?? now);
        var toUtc = SlotCalculator.AsUtc(to ?? now.AddDays(14));
        if (toUtc <= fromUtc || toUtc - fromUtc > TimeSpan.FromDays(62))
            return BadRequest("Invalid slot range (maximum 62 days)");

        var physicianExists = await _db.Physicians.AnyAsync(x => x.Id == physicianId, ct);
        if (!physicianExists) return NotFound("Physician not found");

        var rules = await _db.PhysicianAvailabilities
            .Where(x => x.PhysicianId == physicianId && x.IsActive)
            .ToListAsync(ct);
        var bookings = await _db.Appointments
            .Where(x => x.PhysicianId == physicianId
                        && x.Status != ScheduleStatus.Cancelled
                        && x.Status != ScheduleStatus.Declined
                        && x.AppointmentDate < toUtc
                        && x.ScheduleEndTime > fromUtc)
            .ToListAsync(ct);

        var slots = SlotCalculator.Generate(rules, bookings, fromUtc, toUtc, now)
            .Select(x => new
            {
                start = x.StartUtc,
                end = x.EndUtc,
                startUtc = x.StartUtc,
                endUtc = x.EndUtc,
                durationMinutes = x.DurationMinutes
            });

        return Ok(slots);
    }
}

public record AvailabilityInput(DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes = 30);
