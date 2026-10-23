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
    }
}

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.HasIndex(x => new { x.PhysicianId, x.AppointmentDate, x.ScheduleEndTime });
        b.Property(x => x.AppointmentDate).IsRequired();
        b.Property(x => x.ScheduleEndTime).IsRequired();
    }
}
