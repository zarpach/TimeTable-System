using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class SyllabusConfiguration : IEntityTypeConfiguration<Syllabus>
    {
        public void Configure(EntityTypeBuilder<Syllabus> builder)
        {
            builder.Property(x => x.InstructorPhone)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.OfficeHours)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(x => x.CourseDescription)
                .HasMaxLength(5000)
                .IsRequired();


            builder.Property(x => x.Objectives)
                .HasMaxLength(5000);

            builder.Property(x => x.TeachMethods)
                .HasMaxLength(5000);

            builder.Property(x => x.PrimaryResources)
                .HasMaxLength(5000);

            builder.Property(x => x.AdditionalResources)
                .HasMaxLength(5000);


            builder.Property(x => x.Link)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.GradingComment)
                .HasMaxLength(5000);


            builder.Property(x => x.InstructorComment)
                .HasMaxLength(5000);

            builder.Property(x => x.ApproverComment)
                .HasMaxLength(5000);

            builder.Property(x => x.Language)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();
        }
    }
}
