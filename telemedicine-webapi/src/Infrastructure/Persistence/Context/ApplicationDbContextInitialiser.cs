using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;
using telemedicine_webapi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

public class ApplicationDbContextInitialiser : IApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser<Guid>> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        await SeedRolesAsync();
        var demoDataEnabled = _configuration.GetValue("DemoData:Enabled", false);
        if (demoDataEnabled || _configuration.GetValue("SeedAdmin", false))
        {
            await EnsureIdentityUserAsync("administrator@localhost", "Administrator1!", "Administrator");
        }
        if (demoDataEnabled)
        {
            await SeedDemoDataAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in new[] { "Administrator", "Physician", "Patient" })
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }
    }

    private async Task<IdentityUser<Guid>> EnsureIdentityUserAsync(string email, string password, string role)
    {
        email = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser<Guid>
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                throw new InvalidOperationException($"Unable to create demo user {email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }
        else if (!await _userManager.CheckPasswordAsync(user, password))
        {
            // Demo accounts are synthetic. Reset to a documented password so the
            // demonstration is reproducible from any prior local state.
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
            if (!resetResult.Succeeded)
                throw new InvalidOperationException($"Unable to reset demo password for {email}: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    private async Task SeedDemoDataAsync()
    {
        var patientUser = await EnsureIdentityUserAsync("patient@demo.local", "Patient@12345", "Patient");
        var physicianUser = await EnsureIdentityUserAsync("physician@demo.local", "Physician@12345", "Physician");
        var secondPhysicianUser = await EnsureIdentityUserAsync("doctor@demo.local", "Doctor@12345", "Physician");

        var patient = await _context.Patients.FirstOrDefaultAsync(x => x.Email == patientUser.Email);
        if (patient == null)
        {
            patient = new Patient
            {
                IdentityUserId = patientUser.Id,
                FirstName = "Demo",
                LastName = "Patient",
                Email = patientUser.Email!,
                Gender = Gender.Female,
                Birth = new DateTime(1990, 4, 12, 0, 0, 0, DateTimeKind.Utc),
                Phone = "+27 11 555 0100",
                Address = "12 Demo Street, Johannesburg",
                EmergencyContactName = "Demo Next of Kin",
                EmergencyContactPhone = "+27 11 555 0111",
                AccountType = AccountType.PATIENT,
                Created = DateTime.UtcNow,
                CreatedBy = "demo-seeder"
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }
        else if (patient.IdentityUserId == null)
        {
            patient.IdentityUserId = patientUser.Id;
            await _context.SaveChangesAsync();
        }

        var physician = await EnsurePhysicianAsync(
            physicianUser,
            new Physician
            {
                FirstName = "Demo",
                LastName = "Physician",
                Gender = Gender.Male,
                Birth = new DateTime(1980, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Phone = "+27 11 555 0200",
                Address = "8 Demo Medical Centre, Johannesburg",
                License = "DEMO-MP-001",
                MedicalSpecialization = "General Practice",
                Qualifications = "MBChB (Synthetic), Dip. Prac. Med. (Synthetic)",
                Bio = "Synthetic demonstration physician for the Release 1 booking workflow."
            });

        var secondPhysician = await EnsurePhysicianAsync(
            secondPhysicianUser,
            new Physician
            {
                FirstName = "Amara",
                LastName = "Cardio",
                Gender = Gender.Female,
                Birth = new DateTime(1983, 2, 20, 0, 0, 0, DateTimeKind.Utc),
                Phone = "+27 11 555 0300",
                Address = "21 Demo Heart Clinic, Pretoria",
                License = "DEMO-MP-002",
                MedicalSpecialization = "Cardiology",
                Qualifications = "MBChB (Synthetic), MMed Cardiology (Synthetic)",
                Bio = "Synthetic cardiology demonstration profile."
            });

        await EnsureAvailabilityAsync(physician.Id);
        await EnsureAvailabilityAsync(secondPhysician.Id);

        if (!await _context.Hospitals.AnyAsync(x => x.HospitalName == "Demo Community Hospital"))
        {
            _context.Hospitals.Add(new Hospital
            {
                HospitalName = "Demo Community Hospital",
                Address = "1 Demo Health Avenue, Johannesburg",
                Created = DateTime.UtcNow,
                CreatedBy = "demo-seeder"
            });
            await _context.SaveChangesAsync();
        }

        await EnsureServiceAsync("General Consultation", 500m, "Synthetic general-practice telemedicine consultation.");
        await EnsureServiceAsync("Cardiology Consultation", 750m, "Synthetic cardiology telemedicine consultation.");
        await EnsureServiceAsync("Paediatric Consultation", 600m, "Synthetic paediatric telemedicine consultation.");

        await EnsureDemoAppointmentsAsync(patient, physician, secondPhysician);
        await EnsureDemoChatAsync(patient, physician);
        if (_configuration.GetValue("DemoData:ResetConsultations", false))
            await _context.ConsultationSessions.Where(x => x.PatientId == patient.Id).ExecuteDeleteAsync();

        _logger.LogInformation("Release 1 demo data is available for synthetic local demonstrations.");
    }

    private async Task<Physician> EnsurePhysicianAsync(IdentityUser<Guid> identityUser, Physician template)
    {
        var physician = await _context.Physicians.FirstOrDefaultAsync(x => x.Email == identityUser.Email);
        if (physician == null)
        {
            physician = new Physician
            {
                IdentityUserId = identityUser.Id,
                FirstName = template.FirstName,
                LastName = template.LastName,
                Email = identityUser.Email!,
                Gender = template.Gender,
                Birth = template.Birth,
                Phone = template.Phone,
                Address = template.Address,
                License = template.License,
                MedicalSpecialization = template.MedicalSpecialization,
                Qualifications = template.Qualifications,
                Bio = template.Bio,
                AccountType = AccountType.PHYSICIAN,
                Created = DateTime.UtcNow,
                CreatedBy = "demo-seeder"
            };
            _context.Physicians.Add(physician);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (physician.IdentityUserId == null) physician.IdentityUserId = identityUser.Id;
            if (string.IsNullOrWhiteSpace(physician.License)) physician.License = template.License;
            if (string.IsNullOrWhiteSpace(physician.MedicalSpecialization)) physician.MedicalSpecialization = template.MedicalSpecialization;
            if (string.IsNullOrWhiteSpace(physician.Qualifications)) physician.Qualifications = template.Qualifications;
            if (string.IsNullOrWhiteSpace(physician.Bio)) physician.Bio = template.Bio;
            await _context.SaveChangesAsync();
        }

        return physician;
    }

    private async Task EnsureAvailabilityAsync(int physicianId)
    {
        if (await _context.PhysicianAvailabilities.AnyAsync(x => x.PhysicianId == physicianId))
            return;

        foreach (var day in new[]
                 {
                     DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                     DayOfWeek.Thursday, DayOfWeek.Friday
                 })
        {
            _context.PhysicianAvailabilities.Add(new PhysicianAvailability
            {
                PhysicianId = physicianId,
                DayOfWeek = day,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                SlotDurationMinutes = 30,
                IsActive = true,
                Created = DateTime.UtcNow,
                CreatedBy = "demo-seeder"
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task EnsureServiceAsync(string name, decimal cost, string description)
    {
        if (await _context.TelemedicineServices.AnyAsync(x => x.ServiceName == name))
            return;

        _context.TelemedicineServices.Add(new TelemedicineService
        {
            ServiceName = name,
            Cost = cost,
            Description = description,
            Created = DateTime.UtcNow,
            CreatedBy = "demo-seeder"
        });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureDemoAppointmentsAsync(Patient patient, Physician physician, Physician secondPhysician)
    {
        var generalService = await _context.TelemedicineServices.FirstAsync(x => x.ServiceName == "General Consultation");
        var cardiologyService = await _context.TelemedicineServices.FirstAsync(x => x.ServiceName == "Cardiology Consultation");

        var baseDate = DateTime.UtcNow.Date.AddDays(7);
        while (baseDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            baseDate = baseDate.AddDays(1);

        await EnsureAppointmentAsync(patient, physician, generalService, baseDate.AddHours(9), ScheduleStatus.Pending, "Synthetic pending request.");
        await EnsureAppointmentAsync(patient, physician, generalService, baseDate.AddDays(1).AddHours(10), ScheduleStatus.Active, "Synthetic accepted appointment.");
        await EnsureAppointmentAsync(patient, secondPhysician, cardiologyService, baseDate.AddDays(2).AddHours(11), ScheduleStatus.Pending, "Synthetic cardiology request.");
    }

    private async Task EnsureDemoChatAsync(Patient patient, Physician physician)
    {
        if (patient.IdentityUserId is null || physician.IdentityUserId is null) return;
        var appointment = await _context.Appointments
            .Where(x => x.PatientId == patient.Id && x.PhysicianId == physician.Id && x.Status == ScheduleStatus.Active)
            .OrderBy(x => x.AppointmentDate)
            .FirstOrDefaultAsync();
        if (appointment is null) return;
        if (await _context.AppointmentChatMessages.AnyAsync(x => x.AppointmentId == appointment.Id)) return;

        var now = DateTime.UtcNow;
        _context.AppointmentChatMessages.AddRange(
            new AppointmentChatMessage
            {
                AppointmentId = appointment.Id,
                SenderUserId = physician.IdentityUserId.Value,
                SenderRole = "Physician",
                Body = "Hello Demo, this is Dr. Demo Physician. You can message me here before your consultation.",
                SentAtUtc = now.AddMinutes(-25),
                PhysicianReadAtUtc = now,
                Created = now,
                CreatedBy = "demo-seeder"
            },
            new AppointmentChatMessage
            {
                AppointmentId = appointment.Id,
                SenderUserId = patient.IdentityUserId.Value,
                SenderRole = "Patient",
                Body = "Thank you! I have a quick question about my upcoming appointment.",
                SentAtUtc = now.AddMinutes(-20),
                PatientReadAtUtc = now,
                PhysicianReadAtUtc = now,
                Created = now,
                CreatedBy = "demo-seeder"
            });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureAppointmentAsync(
        Patient patient,
        Physician physician,
        TelemedicineService service,
        DateTime startUtc,
        ScheduleStatus status,
        string notes)
    {
        var start = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        var end = start.AddMinutes(30);
        if (await _context.Appointments.AnyAsync(x =>
                x.PatientId == patient.Id && x.PhysicianId == physician.Id && x.AppointmentDate == start))
            return;

        _context.Appointments.Add(new Appointment
        {
            PatientId = patient.Id,
            PhysicianId = physician.Id,
            TelemedicineService = service,
            AppointmentDate = start,
            ScheduleEndTime = end,
            Status = status,
            Notes = notes,
            Created = DateTime.UtcNow,
            CreatedBy = "demo-seeder"
        });
        await _context.SaveChangesAsync();
    }
}
