using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder) 
        {
            builder.Property(x => x.Comment)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.HasOne(x => x.StudentCourseRegistration)
                .WithMany(x => x.StudentCourses)
                .HasForeignKey(x => x.StudentCourseRegistrationId);

            builder.HasOne(x => x.OldStudyCardCourse)
                .WithMany(x => x.StudentCourses)
                .HasForeignKey(x => x.StudyCardCourseId);
        }
    }
}
