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
    public class EducationTypeConfiguration : IEntityTypeConfiguration<EducationType>
    {
        public void Configure(EntityTypeBuilder<EducationType> builder) 
        {
            builder.Property(x => x.NameEng)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.NameRus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(50);

            builder.HasData(new EducationType
            {
                Id = 1,
                NameEng = "Not assigned",
                NameRus = "Не указана",
                NameKir = "Не указана"
            });
        }
    }
}
