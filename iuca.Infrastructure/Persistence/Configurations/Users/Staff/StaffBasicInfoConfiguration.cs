using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Staff;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Staff
{
    public class StaffBasicInfoConfiguration : IEntityTypeConfiguration<StaffBasicInfo>
    {
        public void Configure(EntityTypeBuilder<StaffBasicInfo> builder) 
        {
            builder.HasOne<ApplicationUser>()
               .WithOne(x => x.StaffBasicInfo)
               .HasForeignKey<StaffBasicInfo>(x => x.ApplicationUserId);

        }
    }
}
