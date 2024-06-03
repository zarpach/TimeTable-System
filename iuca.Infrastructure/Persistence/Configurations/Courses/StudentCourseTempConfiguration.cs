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
    public class StudentCourseTempConfiguration : IEntityTypeConfiguration<StudentCourseTemp>
    {
        public void Configure(EntityTypeBuilder<StudentCourseTemp> builder) 
        {
            builder.Property(x => x.Comment)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.HasOne(x => x.StudentCourseRegistration)
                .WithMany(x => x.StudentCoursesTemp)
                .HasForeignKey(x => x.StudentCourseRegistrationId);

            builder.HasOne(x => x.AnnouncementSection)
                .WithMany(x => x.StudentCourses)
                .HasForeignKey(x => x.AnnouncementSectionId);

            builder.HasOne(x => x.Grade)
                .WithMany(x => x.StudentCourseTemps)
                .HasForeignKey(x => x.GradeId)
                .IsRequired(false);
        }
    }
}
