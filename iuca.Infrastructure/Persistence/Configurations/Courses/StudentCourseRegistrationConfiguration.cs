using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class StudentCourseRegistrationConfiguration : IEntityTypeConfiguration<StudentCourseRegistration>
    {
        public void Configure(EntityTypeBuilder<StudentCourseRegistration> builder) 
        {
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.StudentCourseRegistrations)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.StudentCourseRegistrations)
                .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.Semester)
                .WithMany(x => x.StudentCourseRegistrations)
                .HasForeignKey(x => x.SemesterId);

            builder.Property(x => x.AddDropState)
                .HasDefaultValue(1);
        }
    }
}
