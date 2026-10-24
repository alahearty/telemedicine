using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Infrastructure.Persistence.Configurations;

public sealed class ConsultationSessionConfiguration : IEntityTypeConfiguration<ConsultationSession>
{
    public void Configure(EntityTypeBuilder<ConsultationSession> b)
    {
        b.HasIndex(x => x.SessionId).IsUnique();
        b.HasIndex(x => x.AppointmentId).IsUnique();
        b.HasIndex(x => new { x.PatientId, x.Status });
        b.HasIndex(x => new { x.PhysicianId, x.Status });
        b.Property(x => x.EndReason).HasMaxLength(500);
        b.HasOne(x => x.Appointment).WithOne().HasForeignKey<ConsultationSession>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Physician).WithMany().HasForeignKey(x => x.PhysicianId).OnDelete(DeleteBehavior.Restrict);
        b.ToTable(t => t.HasCheckConstraint(
            "CK_ConsultationSessions_EndAfterStart",
            "\"EndedAtUtc\" IS NULL OR \"StartedAtUtc\" IS NULL OR \"EndedAtUtc\" >= \"StartedAtUtc\""));
    }
}
