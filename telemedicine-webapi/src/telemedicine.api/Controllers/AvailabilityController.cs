using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/physicians")]
public class AvailabilityController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _user;
    public AvailabilityController(IApplicationDbContext db, ICurrentUserService user) { _db = db; _user = user; }

    [HttpPut("me/availability")]
    [Permit("Physician")]
    public async Task<IActionResult> Replace([FromBody] List<AvailabilityInput> input, CancellationToken ct)
    {
        var physician = await _db.Physicians.SingleOrDefaultAsync(x => x.IdentityUserId == _user.UserId, ct);
        if (physician is null) return NotFound("Physician profile not found");
        if (input.Any(x => x.StartTime >= x.EndTime || x.SlotDurationMinutes is < 10 or > 240)) return BadRequest("Invalid availability interval");
        var old = _db.PhysicianAvailabilities.Where(x => x.PhysicianId == physician.Id);
        _db.PhysicianAvailabilities.RemoveRange(old);
        foreach (var x in input) _db.PhysicianAvailabilities.Add(new PhysicianAvailability
        {
            PhysicianId = physician.Id, DayOfWeek = x.DayOfWeek, StartTime = x.StartTime,
            EndTime = x.EndTime, SlotDurationMinutes = x.SlotDurationMinutes, IsActive = true,
            Created = DateTime.UtcNow, CreatedBy = _user.Email
        });
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("{physicianId:int}/availability")]
    [Permit("Patient, Physician, Administrator")]
    public async Task<IActionResult> Get(int physicianId, CancellationToken ct) => Ok(await _db.PhysicianAvailabilities
        .Where(x => x.PhysicianId == physicianId && x.IsActive).OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime).ToListAsync(ct));

    [HttpGet("{physicianId:int}/slots")]
    [Permit("Patient, Physician, Administrator")]
    public async Task<IActionResult> Slots(int physicianId, DateTime from, DateTime to, CancellationToken ct)
    {
        if (to <= from || to - from > TimeSpan.FromDays(62)) return BadRequest("Invalid slot range");
        var rules = await _db.PhysicianAvailabilities.Where(x => x.PhysicianId == physicianId && x.IsActive).ToListAsync(ct);
        var booked = await _db.Appointments.Where(x => x.PhysicianId == physicianId && x.Status != ScheduleStatus.Cancelled && x.Status != ScheduleStatus.Declined && x.AppointmentDate < to && x.ScheduleEndTime > from).ToListAsync(ct);
        var slots = new List<object>();
        for (var date = from.Date; date <= to.Date; date = date.AddDays(1)) foreach (var rule in rules.Where(x => x.DayOfWeek == date.DayOfWeek))
        {
            for (var start = date.Add(rule.StartTime); start.AddMinutes(rule.SlotDurationMinutes) <= date.Add(rule.EndTime); start = start.AddMinutes(rule.SlotDurationMinutes))
            {
                var end = start.AddMinutes(rule.SlotDurationMinutes);
                if (start >= DateTime.UtcNow && start >= from && end <= to && !booked.Any(x => x.AppointmentDate < end && x.ScheduleEndTime > start)) slots.Add(new { start, end });
            }
        }
        return Ok(slots);
    }
}

public record AvailabilityInput(DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes = 30);
