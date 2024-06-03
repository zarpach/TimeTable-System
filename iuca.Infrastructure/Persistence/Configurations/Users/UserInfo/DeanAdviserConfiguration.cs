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
    public class DeanAdviserConfiguration: IEntityTypeConfiguration<DeanAdviser>
    {
        public void Configure(EntityTypeBuilder<DeanAdviser> builder) 
        {
            builder.HasKey(x => new { x.DeanUserId, x.AdviserUserId, x.OrganizationId });

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.DeanAdviserDeans)
                .HasForeignKey(x => x.DeanUserId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.DeanAdviserAdvisers)
                .HasForeignKey(x => x.AdviserUserId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.DeanAdvisers)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
