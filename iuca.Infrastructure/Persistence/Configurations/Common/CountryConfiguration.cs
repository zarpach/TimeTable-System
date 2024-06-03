using iuca.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder) 
        {
            builder.Property(x => x.Code)
                .HasMaxLength(50);

            builder.Property(x => x.NameEng)
                .HasMaxLength(100);

            builder.Property(x => x.NameRus)
                .HasMaxLength(100);

            builder.Property(x => x.NameKir)
                .HasMaxLength(100);

            builder.HasMany(x => x.UserBasicInfo)
                .WithOne(x => x.Citizenship)
                .HasForeignKey(x => x.CitizenshipId)
                .IsRequired(false);

            builder.HasData(new Country
                {
                    Id = 1,
                    Code = "NA",
                    NameEng = "Not assigned",
                    NameRus = "Не указана",
                    NameKir = "Не указана"
                });
        }
    }
}
