using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class AttendanceClassConfiguration : IEntityTypeConfiguration<AttendanceClass>
    {
        public void Configure(EntityTypeBuilder<AttendanceClass> builder)
        {
            builder.HasOne(x => x.Attendance)
                .WithMany(x => x.AttendanceClasses)
                .HasForeignKey(x => x.AttendanceId);

            builder.Property(x => x.Mark)
                .HasDefaultValue(0);

            builder.Property(x => x.Data)
                .HasMaxLength(255);
        }
    }
}
