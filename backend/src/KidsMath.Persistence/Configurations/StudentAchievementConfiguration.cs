using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class StudentAchievementConfiguration : IEntityTypeConfiguration<StudentAchievement>
{
    public void Configure(EntityTypeBuilder<StudentAchievement> builder)
    {
        builder.ToTable("student_achievements");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.StudentProfileId, x.AchievementId }).IsUnique();
    }
}
