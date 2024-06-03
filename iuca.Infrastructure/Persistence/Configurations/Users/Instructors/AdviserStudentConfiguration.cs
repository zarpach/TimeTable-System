using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Instructors
{
    public class AdviserStudentConfiguration : IEntityTypeConfiguration<AdviserStudent>
    {
        public void Configure(EntityTypeBuilder<AdviserStudent> builder) 
        {
            builder.HasKey(x => new { x.InstructorUserId, x.StudentUserId, x.OrganizationId });

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.AdviserStudentInstructors)
                .HasForeignKey(x => x.InstructorUserId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.AdviserStudentStudents)
                .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.AdviserStudents)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
