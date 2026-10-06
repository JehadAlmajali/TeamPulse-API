using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamPulse.Domain.Entities;

namespace TeamPulse.Infrastructure.Persistence.Configurations;

public class PulseResponseConfiguration
    : IEntityTypeConfiguration<PulseResponse>
{
    public void Configure(EntityTypeBuilder<PulseResponse> builder)
    {
        builder.ToTable("PulseResponses", table =>
        {
            table.HasCheckConstraint(
                "CK_PulseResponses_Rating",
                "\"Rating\" BETWEEN 1 AND 5");
        });

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.QuestionId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.AnsweredDate)
            .IsRequired();

        builder.Property(r => r.CreatedDate)
            .IsRequired();

        builder.HasIndex(r => new
        {
            r.UserId,
            r.AnsweredDate
        })
        .IsUnique();

        builder.HasOne(r => r.Question)
            .WithMany()
            .HasForeignKey(r => r.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}