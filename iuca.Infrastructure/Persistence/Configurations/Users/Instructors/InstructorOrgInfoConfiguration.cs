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
    public class InstructorOrgInfoConfiguration : IEntityTypeConfiguration<InstructorOrgInfo>
    {
        public void Configure(EntityTypeBuilder<InstructorOrgInfo> builder) 
        {
            builder.HasKey(x => new { x.InstructorBasicInfoId, x.OrganizationId});

            builder.HasOne(x => x.InstructorBasicInfo)
                .WithMany(x => x.InstructorOrgInfo)
                .HasForeignKey(x => x.InstructorBasicInfoId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.InstructorOrgInfo)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.InstructorOrgInfo)
                .HasForeignKey(x => x.DepartmentId);
        }
    }
}
