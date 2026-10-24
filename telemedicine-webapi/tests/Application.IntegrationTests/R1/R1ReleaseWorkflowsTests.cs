using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests.R1;

[TestFixture]
public class R1ReleaseWorkflowsTests
{
    [SetUp]
    public async Task SetUp()
    {
        await Testing.ResetState();
        await R1TestSupport.SeedBaselineAsync();
    }

    [Test]
    public async Task Registration_creates_linked_patient_and_physician_profiles_and_rejects_duplicates()
    {
        using var anonymous = R1TestSupport.AnonymousClient();
        var patient = await R1TestSupport.RegisterAsync(anonymous, "Patient");
        patient.ProfileId.Should().BeGreaterThan(0);

        var patientToken = await R1TestSupport.LoginAsync(anonymous, patient.Email, patient.Password);
        using var patientClient = R1TestSupport.AuthorizedClient(patientToken);
        var me = await R1TestSupport.GetJsonAsync(patientClient, "/api/Patient/me");
        me.GetProperty("data").GetProperty("id").GetInt32().Should().Be(patient.ProfileId);
        me.GetProperty("data").GetProperty("email").GetString().Should().Be(patient.Email);

        var duplicateProfile = await R1TestSupport.PostJsonAsync(patientClient, "/api/Patient", new
        {
            firstName = "Duplicate",
            lastName = "Patient",
            email = patient.Email,
            gender = "Female",
            birth = "1990-04-12",
            phone = "+27 11 800 1000",
            accountType = "Patient"
        }, HttpStatusCode.BadRequest);
        duplicateProfile.GetProperty("notSuccesful").GetBoolean().Should().BeTrue();

        using var duplicateClient = R1TestSupport.AnonymousClient();
        var duplicateRegistration = await duplicateClient.PostAsJsonAsync("/api/auth/register", new
        {
            email = patient.Email,
            password = R1TestSupport.DefaultPassword,
            accountType = "Patient",
            firstName = "Duplicate",
            lastName = "Patient",
            gender = "Female",
            birth = "1990-04-12",
            phone = "+27 11 800 1000"
        });
        duplicateRegistration.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var license = $"R1-{Guid.NewGuid():N}"[..12];
        var physician = await R1TestSupport.RegisterAsync(anonymous, "Physician", license: license);
        var physicianToken = await R1TestSupport.LoginAsync(anonymous, physician.Email, physician.Password);
        using var physicianClient = R1TestSupport.AuthorizedClient(physicianToken);
        var physicianMe = await R1TestSupport.GetJsonAsync(physicianClient, "/api/Physician/me");
        physicianMe.GetProperty("data").GetProperty("id").GetInt32().Should().Be(physician.ProfileId);
        physicianMe.GetProperty("data").GetProperty("license").GetString().Should().Be(license);

        var duplicateLicense = await anonymous.PostAsJsonAsync("/api/auth/register", new
        {
            email = R1TestSupport.UniqueEmail("physician.duplicate"),
            password = R1TestSupport.DefaultPassword,
            accountType = "Physician",
            firstName = "Duplicate",
            lastName = "Physician",
            gender = "Male",
            birth = "1980-01-01",
            phone = "+27 11 800 2000",
            license,
            medicalSpecialization = "General Practice"
        });
        duplicateLicense.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Role_authorization_is_enforced_on_profile_and_scheduling_operations()
    {
        var patient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        var physician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        using var patientClient = R1TestSupport.AuthorizedClient(patient.Token);
        using var physicianClient = R1TestSupport.AuthorizedClient(physician.Token);
        using var anonymous = R1TestSupport.AnonymousClient();

        (await patientClient.GetAsync("/api/Physician/me")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await patientClient.PostAsJsonAsync("/api/Appointment/respond", new { appointmentId = 1, accept = true }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await patientClient.PutAsJsonAsync("/api/physicians/me/availability", Array.Empty<object>()))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await physicianClient.GetAsync("/api/Patient/me")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await anonymous.GetAsync("/api/Appointment/1")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var adminToken = await R1TestSupport.AdminTokenAsync();
        using var adminClient = R1TestSupport.AuthorizedClient(adminToken);
        (await adminClient.GetAsync("/api/Patient")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Physician_availability_persists_and_generates_real_slots()
    {
        var physician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        var patient = await R1TestSupport.RegisterAndLoginAsync("Patient");

        string[] days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
        var rules = days.Select((_, index) => new
        {
            dayOfWeek = index + 1,
            startTime = "08:00:00",
            endTime = "16:00:00",
            slotDurationMinutes = 30
        }).ToArray();

        using var physicianClient = R1TestSupport.AuthorizedClient(physician.Token);
        var saved = await R1TestSupport.PutJsonAsync(physicianClient, "/api/physicians/me/availability", rules);
        saved.GetArrayLength().Should().Be(5);

        using var patientClient = R1TestSupport.AuthorizedClient(patient.Token);
        var availability = await R1TestSupport.GetJsonAsync(patientClient, $"/api/physicians/{physician.ProfileId}/availability");
        availability.GetArrayLength().Should().Be(5);

        var slots = await R1TestSupport.GetSlotsAsync(patient.Token, physician.ProfileId, 5);
        slots.Should().NotBeEmpty();
        var first = slots[0];
        first.GetProperty("durationMinutes").GetInt32().Should().Be(30);
        first.GetProperty("start").GetDateTime().Should().BeBefore(first.GetProperty("end").GetDateTime());

        var overlap = new[]
        {
            new { dayOfWeek = 1, startTime = "08:00:00", endTime = "12:00:00", slotDurationMinutes = 30 },
            new { dayOfWeek = 1, startTime = "11:00:00", endTime = "16:00:00", slotDurationMinutes = 30 }
        };
        (await physicianClient.PutAsJsonAsync("/api/physicians/me/availability", overlap))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var tooWideFrom = DateTime.UtcNow.AddDays(1).ToString("O");
        var tooWideTo = DateTime.UtcNow.AddDays(90).ToString("O");
        (await patientClient.GetAsync($"/api/physicians/{physician.ProfileId}/slots?from={Uri.EscapeDataString(tooWideFrom)}&to={Uri.EscapeDataString(tooWideTo)}"))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Valid_booking_creates_a_pending_appointment_and_removes_the_slot()
    {
        var context = await PrepareBookingContextAsync();
        var slot = context.Slots[0];
        var start = slot.GetProperty("start").GetDateTime();

        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, slot);

        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);
        var appointment = await R1TestSupport.GetJsonAsync(patientClient, $"/api/Appointment/{appointmentId}");
        appointment.GetProperty("data").GetProperty("status").GetString().Should().Be("Pending");
        appointment.GetProperty("data").GetProperty("appointmentDate").GetDateTime().Should().Be(start);
        appointment.GetProperty("data").GetProperty("scheduleEndTime").GetDateTime().Should().Be(start.AddMinutes(30));

        var patientAppointments = await R1TestSupport.GetJsonAsync(
            patientClient, $"/api/Appointment/patient/{context.Patient.ProfileId}");
        patientAppointments.GetProperty("data").EnumerateArray()
            .Any(x => x.GetProperty("id").GetInt32() == appointmentId).Should().BeTrue();

        var remainingSlots = await R1TestSupport.GetSlotsAsync(
            context.Patient.Token, context.Physician.ProfileId, 1);
        remainingSlots.Any(x => x.GetProperty("start").GetDateTime() == start).Should().BeFalse();

        using var scope = Testing.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await db.Appointments.CountAsync(x => x.Id == appointmentId)).Should().Be(1);
    }

    [Test]
    public async Task Invalid_bookings_are_rejected()
    {
        var context = await PrepareBookingContextAsync();
        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);

        var pastToken = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);
        var past = await patientClient.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId = context.ServiceId,
            paymentToken = pastToken,
            appointmentDate = DateTime.UtcNow.AddDays(-1),
            physicianId = context.Physician.ProfileId,
            durationMinutes = 30
        });
        past.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var outsideDate = DateTime.UtcNow.Date.AddDays(2).AddHours(3);
        var outsideToken = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);
        var outside = await patientClient.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId = context.ServiceId,
            paymentToken = outsideToken,
            appointmentDate = outsideDate,
            physicianId = context.Physician.ProfileId,
            durationMinutes = 30
        });
        outside.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var firstSlot = context.Slots[0];
        await R1TestSupport.BookAsync(context.Patient.Token, context.ServiceId, context.Physician.ProfileId, firstSlot);
        var duplicateToken = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);
        var duplicate = await patientClient.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId = context.ServiceId,
            paymentToken = duplicateToken,
            appointmentDate = firstSlot.GetProperty("start").GetDateTime(),
            physicianId = context.Physician.ProfileId,
            durationMinutes = 30
        });
        duplicate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var singleUseToken = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);
        await R1TestSupport.BookWithTokenAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[1], singleUseToken);
        var reused = await patientClient.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId = context.ServiceId,
            paymentToken = singleUseToken,
            appointmentDate = context.Slots[2].GetProperty("start").GetDateTime(),
            physicianId = context.Physician.ProfileId,
            durationMinutes = 30
        });
        reused.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var physicianClient = R1TestSupport.AuthorizedClient(context.Physician.Token);
        var physicianBooking = await physicianClient.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId = context.ServiceId,
            paymentToken = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m),
            appointmentDate = context.Slots[2].GetProperty("start").GetDateTime(),
            physicianId = context.Physician.ProfileId,
            durationMinutes = 30
        });
        physicianBooking.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Concurrent_booking_attempts_allow_exactly_one_appointment()
    {
        var context = await PrepareBookingContextAsync();
        var slot = context.Slots[0];
        var start = slot.GetProperty("start").GetDateTime();
        var tokenA = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);
        var tokenB = await R1TestSupport.CreatePaymentTokenAsync(context.Patient.Token, context.ServiceId, 500m);

        using var clientA = R1TestSupport.AuthorizedClient(context.Patient.Token);
        using var clientB = R1TestSupport.AuthorizedClient(context.Patient.Token);
        var barrier = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<HttpStatusCode> Attempt(HttpClient client, string paymentToken)
        {
            await barrier.Task;
            var response = await client.PostAsJsonAsync("/api/Appointment", new
            {
                serviceId = context.ServiceId,
                paymentToken,
                appointmentDate = start,
                physicianId = context.Physician.ProfileId,
                durationMinutes = 30
            });
            return response.StatusCode;
        }

        var attempts = new[] { Attempt(clientA, tokenA), Attempt(clientB, tokenB) };
        barrier.SetResult();
        var statuses = await Task.WhenAll(attempts);

        statuses.Count(x => x == HttpStatusCode.Created).Should().Be(1);
        statuses.Count(x => x == HttpStatusCode.BadRequest).Should().Be(1);

        using var scope = Testing.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.Appointments.CountAsync(x =>
            x.PhysicianId == context.Physician.ProfileId && x.AppointmentDate == start);
        count.Should().Be(1);
    }

    [Test]
    public async Task Database_exclusion_constraint_rejects_overlapping_rows_under_concurrency()
    {
        var physician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        var patient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        var start = DateTime.UtcNow.Date.AddDays(3).AddHours(9);

        using var scopeA = Testing.CreateScope();
        using var scopeB = Testing.CreateScope();
        var dbA = scopeA.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbB = scopeB.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbA.Appointments.Add(new Appointment
        {
            PatientId = patient.ProfileId,
            PhysicianId = physician.ProfileId,
            AppointmentDate = start,
            ScheduleEndTime = start.AddMinutes(30),
            Status = ScheduleStatus.Pending,
            Created = DateTime.UtcNow
        });
        dbB.Appointments.Add(new Appointment
        {
            PatientId = patient.ProfileId,
            PhysicianId = physician.ProfileId,
            AppointmentDate = start,
            ScheduleEndTime = start.AddMinutes(30),
            Status = ScheduleStatus.Pending,
            Created = DateTime.UtcNow
        });

        var barrier = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<bool> TrySave(ApplicationDbContext db)
        {
            await barrier.Task;
            try
            {
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
            {
                return false;
            }
        }

        var saves = new[] { TrySave(dbA), TrySave(dbB) };
        barrier.SetResult();
        var results = await Task.WhenAll(saves);

        results.Count(x => x).Should().Be(1);
        using var countScope = Testing.CreateScope();
        var countDb = countScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await countDb.Appointments.CountAsync(x =>
            x.PhysicianId == physician.ProfileId && x.AppointmentDate == start)).Should().Be(1);
    }

    [Test]
    public async Task Rescheduling_moves_the_appointment_and_releases_the_old_slot()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        var oldStart = context.Slots[0].GetProperty("start").GetDateTime();
        var available = await R1TestSupport.GetSlotsAsync(context.Patient.Token, context.Physician.ProfileId, 2);
        var newSlot = available[0];

        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);
        var rescheduled = await R1TestSupport.PutJsonAsync(patientClient, $"/api/Appointment/{appointmentId}", new
        {
            appointmentDate = newSlot.GetProperty("start").GetDateTime(),
            durationMinutes = 30
        });
        rescheduled.GetProperty("wasSuccesful").GetBoolean().Should().BeTrue();

        var appointment = await R1TestSupport.GetJsonAsync(patientClient, $"/api/Appointment/{appointmentId}");
        appointment.GetProperty("data").GetProperty("appointmentDate").GetDateTime()
            .Should().Be(newSlot.GetProperty("start").GetDateTime());

        var afterMove = await R1TestSupport.GetSlotsAsync(context.Patient.Token, context.Physician.ProfileId, 1);
        afterMove.Any(x => x.GetProperty("start").GetDateTime() == oldStart).Should().BeTrue();
        afterMove.Any(x => x.GetProperty("start").GetDateTime() == newSlot.GetProperty("start").GetDateTime()).Should().BeFalse();

        var otherSlot = afterMove[0];
        await R1TestSupport.BookAsync(context.Patient.Token, context.ServiceId, context.Physician.ProfileId, otherSlot);
        var conflict = await patientClient.PutAsJsonAsync($"/api/Appointment/{appointmentId}", new
        {
            appointmentDate = otherSlot.GetProperty("start").GetDateTime(),
            durationMinutes = 30
        });
        conflict.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var otherPatient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        using var otherClient = R1TestSupport.AuthorizedClient(otherPatient.Token);
        (await otherClient.PutAsJsonAsync($"/api/Appointment/{appointmentId}", new
        {
            appointmentDate = afterMove[0].GetProperty("start").GetDateTime(),
            durationMinutes = 30
        })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Cancellation_releases_the_slot_and_is_owner_only()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        var start = context.Slots[0].GetProperty("start").GetDateTime();
        var otherPatient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        using var otherClient = R1TestSupport.AuthorizedClient(otherPatient.Token);
        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);

        (await otherClient.DeleteAsync($"/api/Appointment/{appointmentId}")).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        (await patientClient.DeleteAsync($"/api/Appointment/{appointmentId}?reason=Owner%20cancellation")).StatusCode
            .Should().Be(HttpStatusCode.OK);

        var appointment = await R1TestSupport.GetJsonAsync(patientClient, $"/api/Appointment/{appointmentId}");
        appointment.GetProperty("data").GetProperty("status").GetString().Should().Be("Cancelled");
        appointment.GetProperty("data").GetProperty("statusReason").GetString().Should().Be("Owner cancellation");

        var available = await R1TestSupport.GetSlotsAsync(context.Patient.Token, context.Physician.ProfileId, 1);
        available.Any(x => x.GetProperty("start").GetDateTime() == start).Should().BeTrue();
        (await patientClient.DeleteAsync($"/api/Appointment/{appointmentId}")).StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Physician_can_accept_and_decline_only_their_own_pending_requests()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        var otherPhysician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        using var otherPhysicianClient = R1TestSupport.AuthorizedClient(otherPhysician.Token);
        using var physicianClient = R1TestSupport.AuthorizedClient(context.Physician.Token);
        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);

        (await otherPhysicianClient.PostAsJsonAsync("/api/Appointment/respond", new
        {
            appointmentId, accept = true
        })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await patientClient.PostAsJsonAsync("/api/Appointment/respond", new
        {
            appointmentId, accept = true
        })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await physicianClient.PostAsJsonAsync("/api/Appointment/respond", new
        {
            appointmentId, accept = false
        })).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await physicianClient.PostAsJsonAsync("/api/Appointment/respond", new
        {
            appointmentId, accept = false, reason = "Synthetic decline"
        })).StatusCode.Should().Be(HttpStatusCode.OK);

        var declined = await R1TestSupport.GetJsonAsync(patientClient, $"/api/Appointment/{appointmentId}");
        declined.GetProperty("data").GetProperty("status").GetString().Should().Be("Declined");
        declined.GetProperty("data").GetProperty("statusReason").GetString().Should().Be("Synthetic decline");

        var nextSlot = (await R1TestSupport.GetSlotsAsync(context.Patient.Token, context.Physician.ProfileId, 1))[0];
        var secondId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, nextSlot);
        (await physicianClient.PostAsJsonAsync("/api/Appointment/respond", new
        {
            appointmentId = secondId, accept = true
        })).StatusCode.Should().Be(HttpStatusCode.OK);
        var accepted = await R1TestSupport.GetJsonAsync(patientClient, $"/api/Appointment/{secondId}");
        accepted.GetProperty("data").GetProperty("status").GetString().Should().Be("Active");
    }

    [Test]
    public async Task Patient_and_physician_record_level_access_is_enforced()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        var otherPatient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        var otherPhysician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);
        using var otherPatientClient = R1TestSupport.AuthorizedClient(otherPatient.Token);
        using var physicianClient = R1TestSupport.AuthorizedClient(context.Physician.Token);
        using var otherPhysicianClient = R1TestSupport.AuthorizedClient(otherPhysician.Token);

        (await patientClient.GetAsync($"/api/Appointment/{appointmentId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await otherPatientClient.GetAsync($"/api/Appointment/{appointmentId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await physicianClient.GetAsync($"/api/Appointment/{appointmentId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await otherPhysicianClient.GetAsync($"/api/Appointment/{appointmentId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await otherPatientClient.GetAsync($"/api/Appointment/patient/{context.Patient.ProfileId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await otherPhysicianClient.GetAsync($"/api/Appointment/physician/{context.Physician.ProfileId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await physicianClient.GetAsync($"/api/Patient/{context.Patient.ProfileId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await otherPhysicianClient.GetAsync($"/api/Patient/{context.Patient.ProfileId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Chat_requires_authentication_and_a_treating_relationship()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        using var physicianClient = R1TestSupport.AuthorizedClient(context.Physician.Token);
        await R1TestSupport.PostJsonAsync(physicianClient, "/api/Appointment/respond", new
        {
            appointmentId, accept = true
        });

        using var patientClient = R1TestSupport.AuthorizedClient(context.Patient.Token);
        using var anonymous = R1TestSupport.AnonymousClient();
        (await anonymous.PostAsJsonAsync("/chat-hub/send-to-user", new
        {
            recipientEmail = context.Physician.Email, message = "anonymous"
        })).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        (await patientClient.PostAsJsonAsync("/chat-hub/send-to-user", new
        {
            recipientEmail = context.Physician.Email, message = "hello"
        })).StatusCode.Should().Be(HttpStatusCode.OK);

        var unrelatedPhysician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        (await patientClient.PostAsJsonAsync("/chat-hub/send-to-user", new
        {
            recipientEmail = unrelatedPhysician.Email, message = "not allowed"
        })).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        (await patientClient.PostAsJsonAsync("/chat-hub/publish-to-all", new { })).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        (await patientClient.PostAsJsonAsync("/chat-hub/join-group?groupName=other", new { })).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Emr_read_returns_reports_and_comments_without_null_reference()
    {
        var context = await PrepareBookingContextAsync();
        var appointmentId = await R1TestSupport.BookAsync(
            context.Patient.Token, context.ServiceId, context.Physician.ProfileId, context.Slots[0]);
        using var physicianClient = R1TestSupport.AuthorizedClient(context.Physician.Token);
        await R1TestSupport.PostJsonAsync(physicianClient, "/api/Appointment/respond", new
        {
            appointmentId, accept = true
        });

        var created = await R1TestSupport.PostJsonAsync(physicianClient, "/api/HealthAnalysisReport/create", new
        {
            patientid = context.Patient.ProfileId,
            title = "Synthetic review",
            description = "Synthetic regression record.",
            physicianId = context.Physician.ProfileId
        }, HttpStatusCode.Created);
        created.GetProperty("wasSuccesful").GetBoolean().Should().BeTrue();

        var reports = await R1TestSupport.GetJsonAsync(
            physicianClient, $"/api/HealthAnalysisReport/{context.Patient.ProfileId}");
        var data = reports.GetProperty("data");
        data.GetArrayLength().Should().Be(1);
        var comments = data[0].GetProperty("comments");
        comments.GetArrayLength().Should().Be(1);
        comments[0].GetProperty("title").GetString().Should().Be("Synthetic review");
        comments[0].GetProperty("physicianName").GetString().Should().Contain("Release");

        var unrelatedPhysician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        using var unrelatedClient = R1TestSupport.AuthorizedClient(unrelatedPhysician.Token);
        (await unrelatedClient.GetAsync($"/api/HealthAnalysisReport/{context.Patient.ProfileId}")).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<BookingContext> PrepareBookingContextAsync()
    {
        var adminToken = await R1TestSupport.AdminTokenAsync();
        var serviceId = await R1TestSupport.CreateServiceAsync(adminToken);
        var physician = await R1TestSupport.RegisterAndLoginAsync("Physician");
        await R1TestSupport.ReplaceDefaultAvailabilityAsync(physician.Token);
        var patient = await R1TestSupport.RegisterAndLoginAsync("Patient");
        var slots = await R1TestSupport.GetSlotsAsync(patient.Token, physician.ProfileId, 3);
        return new BookingContext(physician, patient, serviceId, slots);
    }

    private sealed record BookingContext(
        AuthSession Physician,
        AuthSession Patient,
        int ServiceId,
        List<JsonElement> Slots);
}
