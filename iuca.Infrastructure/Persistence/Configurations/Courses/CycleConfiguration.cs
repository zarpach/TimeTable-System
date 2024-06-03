using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class CycleConfiguration : IEntityTypeConfiguration<Cycle>
    {
        public void Configure(EntityTypeBuilder<Cycle> builder) 
        {
            builder.Property(x => x.NameEng)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameRus)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(100);

            builder.Property(x => x.NameEng)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
