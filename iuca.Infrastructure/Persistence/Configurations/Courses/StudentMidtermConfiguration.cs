using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class StudentMidtermConfiguration : IEntityTypeConfiguration<StudentMidterm>
    {
        public void Configure(EntityTypeBuilder<StudentMidterm> builder) 
        {
            builder.HasOne(x => x.StudentCourse)
                .WithOne(x => x.StudentMidterm)
                .HasForeignKey<StudentMidterm>(x => x.StudentCourseId);

            builder.Property(x => x.Comment)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(x => x.Recommendation)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(x => x.AdviserComment)
                .HasMaxLength(1000)
                .IsRequired(false);
        }
    }
}
