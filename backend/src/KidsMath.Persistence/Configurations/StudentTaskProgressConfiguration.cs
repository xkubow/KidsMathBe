using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class StudentTaskProgressConfiguration : IEntityTypeConfiguration<StudentTaskProgress>
{
    public void Configure(EntityTypeBuilder<StudentTaskProgress> builder)
    {
        builder.ToTable("student_task_progress");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaskType).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(x => new { x.StudentProfileId, x.Grade, x.TaskType, x.DifficultyLevel }).IsUnique();
    }
}
