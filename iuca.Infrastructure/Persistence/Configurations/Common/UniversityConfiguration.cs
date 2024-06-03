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
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder) 
        {
            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.NameEng)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.NameRus)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(200);

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Universities)
                .HasForeignKey(x => x.CountryId)
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
