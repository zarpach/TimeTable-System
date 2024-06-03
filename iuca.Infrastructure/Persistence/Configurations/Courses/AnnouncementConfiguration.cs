using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.HasOne(x => x.Semester)
                .WithMany(x => x.Announcements)
                .HasForeignKey(x => x.SemesterId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.Announcements)
                .HasForeignKey(x => x.CourseId);
        }
    }
}
