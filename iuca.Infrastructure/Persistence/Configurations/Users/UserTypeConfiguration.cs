using iuca.Domain.Entities.Users;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users
{
    public class UserTypeConfiguration : IEntityTypeConfiguration<UserTypeOrganization>
    {
        public void Configure(EntityTypeBuilder<UserTypeOrganization> builder) 
        {
            builder.HasKey(x => new { x.ApplicationUserId, x.UserType, x.OrganizationId });

            builder.HasOne<ApplicationUser>()
               .WithMany(x => x.UserTypeOrganizations)
               .HasForeignKey(x => x.ApplicationUserId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.UserTypeOrganizations)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
