using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class StudentTotalGPAConfiguration : IEntityTypeConfiguration<StudentTotalGPA>
    {
        public void Configure(EntityTypeBuilder<StudentTotalGPA> builder) 
        {
            builder.HasKey(x =>  x.StudentUserId);

            builder.HasOne<ApplicationUser>()
               .WithOne(x => x.StudentTotalGPA)
               .HasForeignKey<StudentTotalGPA>(x => x.StudentUserId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.StudentTotalGPAs)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
