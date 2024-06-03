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
    public class CyclePartCourseConfiguration : IEntityTypeConfiguration<CyclePartCourse>
    {
        public void Configure(EntityTypeBuilder<CyclePartCourse> builder) 
        {
            builder.HasOne(x => x.CyclePart)
                .WithMany(x => x.CyclePartCourses)
                .HasForeignKey(x => x.CyclePartId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.CyclePartCourses)
                .HasForeignKey(x => x.CourseId);
        }
    }
}
