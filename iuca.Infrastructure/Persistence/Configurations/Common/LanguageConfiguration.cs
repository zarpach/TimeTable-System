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
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(x => x.Code)
                .HasMaxLength(50);

            builder.Property(x => x.NameEng)
                .HasMaxLength(100);

            builder.Property(x => x.NameRus)
                .HasMaxLength(100);

            builder.Property(x => x.NameKir)
                .HasMaxLength(100);

            builder.HasData(
                new Language
                {
                    Id = 1,
                    Code = "Na",
                    NameEng = "Not assigned",
                    NameRus = "Не указан",
                    NameKir = "Не указан",
                    ImportCode = 0
                });
        }
    }
}
