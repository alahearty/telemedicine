using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> b)
    {
        b.HasIndex(x => x.IdentityUserId).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
    }
}

public sealed class PhysicianConfiguration : IEntityTypeConfiguration<Physician>
{
    public void Configure(EntityTypeBuilder<Physician> b)
    {
        b.HasIndex(x => x.IdentityUserId).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.License).IsUnique();
    }
}

public sealed class PhysicianAvailabilityConfiguration : IEntityTypeConfiguration<PhysicianAvailability>
{
    public void Configure(EntityTypeBuilder<PhysicianAvailability> b)
    {
        b.HasOne(x => x.Physician).WithMany(x => x.Availability).HasForeignKey(x => x.PhysicianId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.PhysicianId, x.DayOfWeek, x.StartTime, x.EndTime }).IsUnique();
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_PhysicianAvailabilities_TimeRange", "\"StartTime\" < \"EndTime\"");
            t.HasCheckConstraint("CK_PhysicianAvailabilities_SlotDuration", "\"SlotDurationMinutes\" BETWEEN 10 AND 240");
            t.HasCheckConstraint("CK_PhysicianAvailabilities_DayOfWeek", "\"DayOfWeek\" BETWEEN 0 AND 6");
        });
    }
}

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.HasIndex(x => new { x.PhysicianId, x.AppointmentDate, x.ScheduleEndTime });
        b.HasIndex(x => new { x.Status, x.AppointmentDate });
        b.Property(x => x.AppointmentDate).IsRequired();
        b.Property(x => x.ScheduleEndTime).IsRequired();
        b.Property(x => x.StatusReason).HasMaxLength(1000);
        b.ToTable(t => t.HasCheckConstraint("CK_Appointments_TimeRange", "\"ScheduleEndTime\" > \"AppointmentDate\""));
    }
}
