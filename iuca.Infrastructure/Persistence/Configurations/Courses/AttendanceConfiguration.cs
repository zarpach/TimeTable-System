using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasOne(x => x.AnnouncementSection)
                .WithMany(x => x.Attendances)
                .HasForeignKey(x => x.AnnouncementSectionId);
        }
    }
}
