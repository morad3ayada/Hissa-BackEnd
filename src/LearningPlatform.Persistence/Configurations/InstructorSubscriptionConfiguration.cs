using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class InstructorSubscriptionConfiguration : BaseEntityConfiguration<InstructorSubscription>
{
    protected override void ConfigureEntity(EntityTypeBuilder<InstructorSubscription> builder)
    {
        builder.ToTable("InstructorSubscriptions");

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(SubscriptionStatus.Active);

        builder.Property(s => s.PaymentReference)
            .HasMaxLength(200);

        builder.HasOne(s => s.Instructor)
            .WithMany(u => u.InstructorSubscriptions)
            .HasForeignKey(s => s.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.InstructorId)
            .HasDatabaseName("IX_InstructorSubscriptions_InstructorId");

        builder.HasIndex(s => new { s.InstructorId, s.Status })
            .HasDatabaseName("IX_InstructorSubscriptions_InstructorId_Status");
    }
}
