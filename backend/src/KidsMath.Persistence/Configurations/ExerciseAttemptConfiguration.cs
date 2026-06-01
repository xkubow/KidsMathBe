using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class ExerciseAttemptConfiguration : IEntityTypeConfiguration<ExerciseAttempt>
{
    public void Configure(EntityTypeBuilder<ExerciseAttempt> builder)
    {
        builder.ToTable("exercise_attempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GeneratedQuestionJson).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.QuestionTextCs).IsRequired();
        builder.Property(x => x.QuestionTextEn).IsRequired();
        builder.HasOne(x => x.MathTaskDefinition).WithMany().HasForeignKey(x => x.MathTaskDefinitionId);
    }
}
