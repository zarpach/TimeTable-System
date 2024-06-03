using iuca.Domain.Entities.Users.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class StudentOrgInfoConfiguration : IEntityTypeConfiguration<StudentOrgInfo>
    {
        public void Configure(EntityTypeBuilder<StudentOrgInfo> builder) 
        {
            builder.HasKey(x => new { x.StudentBasicInfoId, x.OrganizationId });

            builder.HasOne(x => x.StudentBasicInfo)
                .WithMany(x => x.StudentOrgInfo)
                .HasForeignKey(x => x.StudentBasicInfoId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.StudentOrgInfo)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.DepartmentGroup)
                .WithMany(x => x.StudentOrgInfo)
                .HasForeignKey(x => x.DepartmentGroupId);

            builder.HasOne(x => x.PrepDepartmentGroup)
                .WithMany(x => x.PrepStudentOrgInfo)
                .HasForeignKey(x => x.PrepDepartmentGroupId)
                .IsRequired(false);
        }
    }
}
