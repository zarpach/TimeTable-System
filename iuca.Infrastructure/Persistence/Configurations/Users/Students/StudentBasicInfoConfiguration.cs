using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class StudentBasicInfoConfiguration : IEntityTypeConfiguration<StudentBasicInfo>
    {
        public void Configure(EntityTypeBuilder<StudentBasicInfo> builder) 
        {
            builder.HasOne<ApplicationUser>()
               .WithOne(x => x.StudentBasicInfo)
               .HasForeignKey<StudentBasicInfo>(x => x.ApplicationUserId);
        }
    }
}
