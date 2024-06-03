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
    public class StudentCourseGradeConfiguration : IEntityTypeConfiguration<StudentCourseGrade>
    {
        public void Configure(EntityTypeBuilder<StudentCourseGrade> builder) 
        {
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.StudentCourseGrades)
                .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.StudentCourseGrades)
                .HasForeignKey(x => x.CourseId);

            builder.HasOne(x => x.Grade)
                .WithMany(x => x.StudentCourseGrades)
                .HasForeignKey(x => x.GradeId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.StudentCourseGrades)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
