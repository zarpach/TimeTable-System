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
    public class TransferCourseConfiguration : IEntityTypeConfiguration<TransferCourse>
    {
        public void Configure(EntityTypeBuilder<TransferCourse> builder) 
        {
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.TransferCourses)
                .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.University)
                .WithMany(x => x.TransferCourses)
                .HasForeignKey(x => x.UniversityId);

            builder.HasOne(x => x.Grade)
                .WithMany(x => x.TransferCourses)
                .HasForeignKey(x => x.GradeId)
                .IsRequired(false);

            builder.HasOne(x => x.CyclePartCourse)
                .WithMany(x => x.TransferCourses)
                .HasForeignKey(x => x.CyclePartCourseId)
                .IsRequired(false);
            
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.TransferCourses)
                .HasForeignKey(x => x.OrganizationId);

            builder.Property(x => x.NameEng)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameRus)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(100);
        }
    }
}
