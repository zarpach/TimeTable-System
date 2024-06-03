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
    public class StudentParentsInfoConfiguration : IEntityTypeConfiguration<StudentParentsInfo>
    {
        public void Configure(EntityTypeBuilder<StudentParentsInfo> builder) 
        {
            builder.HasOne(x => x.StudentBasicInfo)
                .WithMany(x => x.StudentParentsInfo)
                .HasForeignKey(x => x.StudentBasicInfoId);

            builder.Property(x => x.LastName)
                .HasMaxLength(100);

            builder.Property(x => x.FirstName)
                .HasMaxLength(100);

            builder.Property(x => x.MiddleName)
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.WorkPlace)
                .HasMaxLength(100);

            builder.Property(x => x.Relation)
                .HasMaxLength(50);
        }
    }
}
