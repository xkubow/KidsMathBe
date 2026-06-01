using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class ExerciseSessionConfiguration : IEntityTypeConfiguration<ExerciseSession>
{
    public void Configure(EntityTypeBuilder<ExerciseSession> builder)
    {
        builder.ToTable("exercise_sessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaskType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.HasMany(x => x.Attempts).WithOne(a => a.ExerciseSession).HasForeignKey(a => a.ExerciseSessionId);
    }
}
