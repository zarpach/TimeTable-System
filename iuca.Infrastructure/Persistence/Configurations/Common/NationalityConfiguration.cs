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
    public class NationalityConfiguration : IEntityTypeConfiguration<Nationality>
    {
        public void Configure(EntityTypeBuilder<Nationality> builder) 
        {
            builder.Property(x => x.NameEng)
                .HasMaxLength(50);

            builder.Property(x => x.NameRus)
                .HasMaxLength(50);

            builder.Property(x => x.NameKir)
                .HasMaxLength(50);

            builder.HasMany(x => x.UserBasicInfo)
                .WithOne(x => x.Nationality)
                .HasForeignKey(x => x.NationalityId)
                .IsRequired(false);

            builder.HasData(new Nationality
            {
                Id = 1,
                NameEng = "Not assigned",
                NameRus = "Не указана",
                NameKir = "Не указана"
            });
        }
    }
}
