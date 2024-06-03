using iuca.Domain.Entities.Users.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class StudentMinorInfoConfiguration : IEntityTypeConfiguration<StudentMinorInfo>
    {
        public void Configure(EntityTypeBuilder<StudentMinorInfo> builder)
        {
            builder.HasOne(x => x.StudentBasicInfo)
                .WithMany(x => x.StudentMinorInfo)
                .HasForeignKey(x => x.StudentBasicInfoId);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.StudentMinorInfo)
                .HasForeignKey(x => x.DepartmentId);
        }
    }
}
