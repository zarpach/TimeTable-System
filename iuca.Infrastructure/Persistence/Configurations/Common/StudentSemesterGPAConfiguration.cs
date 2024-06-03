using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class StudentSemesterGPAConfiguration : IEntityTypeConfiguration<StudentSemesterGPA>
    {
        public void Configure(EntityTypeBuilder<StudentSemesterGPA> builder) 
        {
            builder.HasKey(x => new { x.StudentUserId, x.Season, x.Year });

            builder.HasOne<ApplicationUser>()
               .WithMany(x => x.StudentSemesterGPA)
               .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.StudentSemesterGPAs)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
