using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class StudyCardCourseConfiguration : IEntityTypeConfiguration<StudyCardCourse>
    {
        public void Configure(EntityTypeBuilder<StudyCardCourse> builder) 
        {
            builder.HasOne(x => x.StudyCard)
                .WithMany(x => x.StudyCardCourses)
                .HasForeignKey(x => x.StudyCardId);

            builder.HasOne(x => x.AnnouncementSection)
                .WithMany(x => x.StudyCardCourses)
                .HasForeignKey(x => x.AnnouncementSectionId);

            builder.Property(x => x.Comment)
                .HasMaxLength(255);

            builder.HasIndex(x => new { x.AnnouncementSectionId, x.StudyCardId })
                .IsUnique();
        }
    }
}
