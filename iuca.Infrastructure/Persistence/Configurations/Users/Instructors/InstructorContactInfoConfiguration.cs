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
    public class InstructorContactInfoConfiguration : IEntityTypeConfiguration<InstructorContactInfo>
    {
        public void Configure(EntityTypeBuilder<InstructorContactInfo> builder) 
        {
            builder.Property(x => x.CityEng)
                .HasMaxLength(50);

            builder.Property(x => x.StreetEng)
                .HasMaxLength(50);

            builder.Property(x => x.AddressEng)
                .HasMaxLength(50);

            builder.Property(x => x.CityRus)
                .HasMaxLength(50);

            builder.Property(x => x.StreetRus)
                .HasMaxLength(50);

            builder.Property(x => x.AddressRus)
                .HasMaxLength(50);

            builder.Property(x => x.ZipCode)
                .HasMaxLength(50);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipCityEng)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipStreetEng)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipAddressEng)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipCityRus)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipStreetRus)
                .HasMaxLength(50);

            builder.Property(x => x.CitizenshipAddressRus)
               .HasMaxLength(50);

            builder.Property(x => x.CitizenshipZipCode)
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

            builder.HasOne(x => x.InstructorBasicInfo)
                .WithOne(x => x.InstructorContactInfo)
                .HasForeignKey<InstructorContactInfo>(x => x.InstructorBasicInfoId);

            builder.HasOne(x => x.Country)
                .WithMany(x => x.InstructorCountries)
                .HasForeignKey(x => x.CountryId)
                .IsRequired(false);

            builder.HasOne(x => x.CitizenshipCountry)
                .WithMany(x => x.InstructorCitizenshipCountries)
                .HasForeignKey(x => x.CitizenshipCountryId)
                .IsRequired(false);
        }
    }
}
