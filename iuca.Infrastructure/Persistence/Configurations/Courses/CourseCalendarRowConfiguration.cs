using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class CourseCalendarRowConfiguration : IEntityTypeConfiguration<CourseCalendarRow>
    {
        public void Configure(EntityTypeBuilder<CourseCalendarRow> builder)
        {
            builder.HasOne(x => x.Syllabus)
                .WithMany(x => x.CourseCalendar)
                .HasForeignKey(x => x.SyllabusId);

            builder.Property(x => x.Week)
                .IsRequired();

            builder.Property(x => x.Topics)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Assignments)
                .HasMaxLength(255)
                .IsRequired(false);
        }
    }
}
