using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using iuca.Domain.Entities.Courses;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class ProposalCourseConfiguration : IEntityTypeConfiguration<ProposalCourse>
    {
        public void Configure(EntityTypeBuilder<ProposalCourse> builder)
        {
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.ProposalCourses)
                .HasForeignKey(x => x.ProposalId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.ProposalCourses)
                .HasForeignKey(x => x.CourseId);

            builder.Property(x => x.InstructorsJson)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.YearsOfStudyJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.Comment)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(x => x.Schedule)
                .HasMaxLength(50)
                .IsRequired(false);
        }
    }
}
