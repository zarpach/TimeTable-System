using iuca.Domain.Entities.Users.Instructors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Instructors
{
    public class InstructorEducationInfoConfiguration : IEntityTypeConfiguration<InstructorEducationInfo>
    {
        public void Configure(EntityTypeBuilder<InstructorEducationInfo> buidler) 
        {
            buidler.Property(x => x.MajorEng)
                .HasMaxLength(50)
                .IsRequired();

            buidler.Property(x => x.MajorRus)
                .HasMaxLength(50)
                .IsRequired();

            buidler.Property(x => x.MajorKir)
                .HasMaxLength(50);

            buidler.HasOne(x => x.InstructorBasicInfo)
                .WithMany(x => x.InstructorEducationInfo)
                .HasForeignKey(x => x.InstructorBasicInfoId);

            buidler.HasOne(x => x.University)
                .WithMany(x => x.InstructorEducationInfo)
                .HasForeignKey(x => x.UniversityId)
                .IsRequired(false);

            buidler.HasOne(x => x.EducationType)
                .WithMany(x => x.InstructorEducationInfo)
                .HasForeignKey(x => x.EducationTypeId)
                .IsRequired(false);
        }
    }
}
