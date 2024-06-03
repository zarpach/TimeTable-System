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
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder) 
        {
            builder.Property(x => x.NameEng)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.NameRus)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(300);

            builder.Property(x => x.Abbreviation)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Number)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.DepartmentId);

            builder.HasOne(x => x.Language)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.LanguageId);
        }
    }
}
