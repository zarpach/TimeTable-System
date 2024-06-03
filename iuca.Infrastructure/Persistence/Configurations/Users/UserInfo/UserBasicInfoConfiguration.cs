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
    public class UserBasicInfoConfiguration : IEntityTypeConfiguration<UserBasicInfo>
    {
        public void Configure(EntityTypeBuilder<UserBasicInfo> builder) 
        {
            builder.HasOne<ApplicationUser>()
               .WithOne(x => x.UserBasicInfo)
               .HasForeignKey<UserBasicInfo>(x => x.ApplicationUserId);

            builder.Property(x => x.LastNameRus)
                .HasMaxLength(50);
            builder.Property(x => x.FirstNameRus)
                .HasMaxLength(50);
            builder.Property(x => x.MiddleNameRus)
                .HasMaxLength(50);
        }
    }
}
