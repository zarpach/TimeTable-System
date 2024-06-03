using iuca.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class AttendanceFolderConfiguration : IEntityTypeConfiguration<AttendanceFolder>
    {
        public void Configure(EntityTypeBuilder<AttendanceFolder> builder) 
        {
            builder.HasOne(x => x.Semester)
                .WithMany(x => x.AttendanceFolders)
                .HasForeignKey(x => x.SemesterId);
        }
    }
}
