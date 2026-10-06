using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamPulse.Domain.Entities;

namespace TeamPulse.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions", table =>
        {
            table.HasCheckConstraint(
                "CK_Questions_NotBlank",
                "length(btrim(\"Question\")) > 0");
        });

        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuestionText)
            .HasColumnName("Question")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(q => q.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(q => q.DisplayOrder)
            .IsRequired();

        builder.HasIndex(q => q.DisplayOrder)
            .IsUnique();

        builder.Property(q => q.IsActive)
            .IsRequired();

        builder.Property(q => q.CreatedBy)
            .IsRequired();

        builder.Property(q => q.CreatedDate)
            .IsRequired();

        builder.Property(q => q.UpdatedBy)
            .IsRequired(false);

        builder.Property(q => q.UpdatedDate)
            .IsRequired(false);
    }
}