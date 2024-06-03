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
    public class StudentContactInfoConfiguration : IEntityTypeConfiguration<StudentContactInfo>
    {
        public void Configure(EntityTypeBuilder<StudentContactInfo> builder) 
        {
            builder.HasOne(x => x.StudentBasicInfo)
                .WithOne(x => x.StudentContactInfo)
                .HasForeignKey<StudentContactInfo>(x => x.StudentBasicInfoId);

            builder.HasOne(x => x.Country)
                .WithMany(x => x.StudentContactInfo)
                .HasForeignKey(x => x.CountryId);

            builder.HasOne(x => x.CitizenshipCountry)
                .WithMany(x => x.CitizenshipStudentContactInfo)
                .HasForeignKey(x => x.CitizenshipCountryId);

            builder.Property(x => x.StreetEng)
                .HasMaxLength(100);

            builder.Property(x => x.CityEng)
                .HasMaxLength(100);

            builder.Property(x => x.StreetRus)
                .HasMaxLength(100);

            builder.Property(x => x.CityRus)
                .HasMaxLength(100);

            builder.Property(x => x.Zip)
                .HasMaxLength(50);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipStreetEng)
                .HasMaxLength(100);

            builder.Property(x => x.CitizenshipCityEng)
                .HasMaxLength(100);

            builder.Property(x => x.CitizenshipStreetRus)
                .HasMaxLength(100);

            builder.Property(x => x.CitizenshipCityRus)
                .HasMaxLength(100);

            builder.Property(x => x.CitizenshipZip)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipPhone)
                .HasMaxLength(50);

            builder.Property(x => x.ContactNameEng)
                .HasMaxLength(100);

            builder.Property(x => x.ContactNameRus)
                .HasMaxLength(100);

            builder.Property(x => x.ContactPhone)
                .HasMaxLength(50);

            builder.Property(x => x.RelationEng)
                .HasMaxLength(50);

            builder.Property(x => x.RelationRus)
                .HasMaxLength(50);

            builder.Property(x => x.RelationKir)
                .HasMaxLength(50);
        }
    }
}
