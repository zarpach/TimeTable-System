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
    public class OldStudyCardCourseConfiguration : IEntityTypeConfiguration<OldStudyCardCourse>
    {
        public void Configure(EntityTypeBuilder<OldStudyCardCourse> builder) 
        {

            builder.HasOne(x => x.OldStudyCard)
                .WithMany(x => x.OldStudyCardCourses)
                .HasForeignKey(x => x.OldStudyCardId);

            builder.HasOne(x => x.CyclePartCourse)
                .WithMany(x => x.OldStudyCardCourses)
                .HasForeignKey(x => x.CyclePartCourseId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.OldStudyCardCourses)
                .HasForeignKey(x => x.InstructorUserId)
                .IsRequired(false);

        }
    }
}
