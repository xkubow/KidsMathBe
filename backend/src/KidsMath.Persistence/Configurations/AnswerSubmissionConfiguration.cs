using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class AnswerSubmissionConfiguration : IEntityTypeConfiguration<AnswerSubmission>
{
    public void Configure(EntityTypeBuilder<AnswerSubmission> builder)
    {
        builder.ToTable("answer_submissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Answer).IsRequired();
        builder.HasIndex(x => new { x.ExerciseAttemptId, x.AttemptNumber }).IsUnique();
        builder.HasOne(x => x.ExerciseAttempt)
            .WithMany(a => a.AnswerSubmissions)
            .HasForeignKey(x => x.ExerciseAttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
