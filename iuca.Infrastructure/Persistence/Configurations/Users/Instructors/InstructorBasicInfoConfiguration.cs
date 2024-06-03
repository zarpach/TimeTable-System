using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Instructors
{
    public class InstructorBasicInfoConfiguration : IEntityTypeConfiguration<InstructorBasicInfo>
    {
        public void Configure(EntityTypeBuilder<InstructorBasicInfo> builder) 
        {
            builder.HasOne<ApplicationUser>()
               .WithOne(x => x.InstructorBasicInfo)
               .HasForeignKey<InstructorBasicInfo>(x => x.InstructorUserId);
        }
    }
}
