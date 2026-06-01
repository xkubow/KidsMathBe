using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KidsMath.Persistence.Configurations;

public class MathTaskDefinitionConfiguration : IEntityTypeConfiguration<MathTaskDefinition>
{
    public void Configure(EntityTypeBuilder<MathTaskDefinition> builder)
    {
        builder.ToTable("math_task_definitions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaskType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.ConfigJson).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.DisplayNameCs).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayNameEn).HasMaxLength(200).IsRequired();
    }
}
