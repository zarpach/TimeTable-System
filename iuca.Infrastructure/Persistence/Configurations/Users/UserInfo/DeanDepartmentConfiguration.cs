using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.UserInfo
{
    public class DeanDepartmentConfiguration : IEntityTypeConfiguration<DeanDepartment>
    {
        public void Configure(EntityTypeBuilder<DeanDepartment> builder) 
        {
            builder.HasKey(x => new { x.DeanUserId, x.DepartmentId, x.OrganizationId });

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.DeanDepartments)
                .HasForeignKey(x => x.DeanUserId);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.DeanDepartments)
                .HasForeignKey(x => x.DepartmentId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.DeanDepartments)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
