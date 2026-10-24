using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Infrastructure.Persistence.Configurations;

public sealed class AppointmentChatMessageConfiguration : IEntityTypeConfiguration<AppointmentChatMessage>
{
    public void Configure(EntityTypeBuilder<AppointmentChatMessage> b)
    {
        b.Property(x => x.Body).IsRequired().HasMaxLength(4000);
        b.Property(x => x.SenderRole).IsRequired().HasMaxLength(20);
        b.HasIndex(x => new { x.AppointmentId, x.SentAtUtc });
        b.HasOne(x => x.Appointment).WithMany().HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        b.ToTable(t => t.HasCheckConstraint(
            "CK_AppointmentChatMessages_Body_NotBlank",
            "length(btrim(\"Body\")) > 0"));
    }
}
