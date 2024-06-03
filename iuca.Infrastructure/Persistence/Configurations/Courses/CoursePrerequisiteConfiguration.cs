using iuca.Domain.Entities.Common;
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
    public class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
    {
        public void Configure(EntityTypeBuilder<CoursePrerequisite> builder) 
        {
            builder.HasKey(x => new { x.CourseId, x.PrerequisiteId});

            builder.HasOne(x => x.Course)
                .WithMany(x => x.CoursePrerequisites)
                .HasForeignKey(x => x.CourseId);

            builder.HasOne(x => x.Prerequisite)
                .WithMany(x => x.Prerequisites)
                .HasForeignKey(x => x.PrerequisiteId);
        }
    }
}
