using iuca.Domain.Entities.Users;
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
    public class InstructorOtherJobInfoConfiguration : IEntityTypeConfiguration<InstructorOtherJobInfo>
    {
        public void Configure(EntityTypeBuilder<InstructorOtherJobInfo> builder) 
        {
            builder.Property(x => x.PlaceNameEng)
                .HasMaxLength(100);

            builder.Property(x => x.PlaceNameRus)
                .HasMaxLength(100);

            builder.Property(x => x.PlaceNameKir)
                .HasMaxLength(100);

            builder.Property(x => x.PositionEng)
                .HasMaxLength(100);

            builder.Property(x => x.PositionRus)
                .HasMaxLength(100);

            builder.Property(x => x.PositionKir)
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.HasOne(x => x.instructorBasicInfo)
                .WithMany(x => x.InstructorOtherJobInfo)
                .HasForeignKey(x => x.InstructorBasicInfoId);
        }
    }
}
